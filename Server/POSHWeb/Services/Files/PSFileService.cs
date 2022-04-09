using Microsoft.EntityFrameworkCore;
using POSHWeb.Data;
using POSHWeb.Exceptions;
using POSHWeb.Model;

namespace POSHWeb.Services;

public class PSFileService
{
    private readonly HasherService _hasherService;
    private readonly ILogger<PSFileService> _logger;
    private readonly PSParameterParserService _parameterParserService;
    private readonly PSScriptValidator _psScriptValidator;
    private readonly IServiceScopeFactory _scopeFactory;

    public PSFileService(IServiceScopeFactory scopeFactory, HasherService hasherService,
        ILogger<PSFileService> logger, PSScriptValidator psScriptValidator,
        PSParameterParserService parameterParserService)
    {
        _scopeFactory = scopeFactory;
        _hasherService = hasherService;
        _logger = logger;
        _psScriptValidator = psScriptValidator;
        _parameterParserService = parameterParserService;
    }

    public void Sync(string directory)
    {
        var files = Directory.GetFiles(directory, "*.ps1", SearchOption.AllDirectories).ToList();
        files.ForEach(path =>
        {
            using var scope = _scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var content = File.ReadAllText(path);
            var hash = _hasherService.Sha256(content);
            var matchingFilenames = _dbContext.Script.Where(script => script.FullPath.Equals(path)).ToList();
            var matchingHashes = _dbContext.Script.Where(script => script.Content.Equals(hash)).ToList();
            if (matchingFilenames.Count == 1 && matchingHashes.Count == 1) return;
            if (matchingFilenames.Count == 1 && matchingHashes.Count == 0) Modified(path);
            if (matchingFilenames.Count == 0 && matchingHashes.Count == 1)
            {
                var script = matchingHashes[1];
                script.FullPath = path;
                script.FileName = Path.GetFileName(path);
                _dbContext.Update(script);
                _dbContext.SaveChanges();
            }

            ;
            Create(path);
        });
    }

    public PSScript Create(string fullPath)
    {
        using var scope = _scopeFactory.CreateScope();
        _logger.LogInformation("PowerShell created", fullPath);
        if (!_psScriptValidator.IsValidPsFile(fullPath)) throw new InvalidFileExtensionException();
        if (!_psScriptValidator.ValidateFile(fullPath)) throw new InvalidPowerShellContentException();

        var content = File.ReadAllText(fullPath);
        var script = new PSScript
        {
            Content = content,
            FileName = Path.GetFileName(fullPath),
            ContentHash = _hasherService.Sha256(content),
            FullPath = fullPath,
            Parameters = _parameterParserService.ExtractParameters(content)
        };
        _logger.LogDebug("Create new script and persist", script);
        var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        _dbContext.Script.Add(script);
        _dbContext.SaveChanges();
        return script;
    }


    public PSScript Remove(string fullPath)
    {
        using var scope = _scopeFactory.CreateScope();
        _logger.LogInformation("PowerShell file removed", fullPath);
        if (_psScriptValidator.IsValidPsFile(fullPath))
            throw new FileStillExistException($"Can not remove file {fullPath}, because it still exist.");
        var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var scripts = _dbContext.Script.Where(script => script.FullPath == fullPath).ToList();
        if (scripts.Count > 1) throw new MoreThanOneDatabaseEntryException();
        var script = scripts[0];
        _logger.LogDebug("Remove PowerShell file from DB", script);
        _dbContext.Script.Remove(script);
        _dbContext.SaveChanges();
        return script;
    }

    public PSScript Modified(string fullPath)
    {
        using var scope = _scopeFactory.CreateScope();
        _logger.LogInformation("PowerShell file modified", fullPath);
        if (!_psScriptValidator.IsValidPsFile(fullPath)) throw new InvalidFileExtensionException();
        if (!_psScriptValidator.ValidateFile(fullPath)) throw new InvalidPowerShellContentException();

        var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var scripts = _dbContext.Script
            .Where(script => script.FullPath == fullPath)
            .Include(psScript => psScript.Parameters)
            .ThenInclude(parameter => parameter.Options)
            .ToList();
        if (scripts.Count > 1 || scripts.Count == 0)
            throw new Exception($"The Script with '{fullPath}' has {scripts.Count} matches in the Database");
        var script = scripts[0];
        var content = File.ReadAllText(fullPath);
        script.Content = content;
        script.Parameters = _parameterParserService.ExtractParameters(content);
        script.ContentHash = _hasherService.Sha256(content);
        _logger.LogDebug("Updated PS File and persist", script);
        _dbContext.Script.Update(script);
        _dbContext.SaveChanges();
        return script;
    }

    public PSScript Rename(string oldFullName, string newFullName)
    {
        _logger.LogInformation("PS File rename", oldFullName, newFullName);
        var isNewFileValid = _psScriptValidator.IsValidPsFile(newFullName) &&
                             _psScriptValidator.ValidateFile(newFullName);

        using var scope = _scopeFactory.CreateScope();
        var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var dbCountOldFile = _dbContext.Script.Count(script => script.FullPath.Equals(oldFullName));
        var dbCountNewFile = _dbContext.Script.Count(script => script.FullPath.Equals(newFullName));
        if (dbCountOldFile > 1) throw new MoreThanOneDatabaseEntryException();
        if (dbCountNewFile > 1) throw new MoreThanOneDatabaseEntryException();

        if (dbCountOldFile == 1 && dbCountNewFile == 1 && isNewFileValid) return Modified(newFullName);
        if (dbCountOldFile == 1 && dbCountNewFile == 0 && isNewFileValid) return RenameHelper(oldFullName, newFullName);
        ;
        if (dbCountOldFile == 0 && dbCountNewFile == 1 && isNewFileValid) return Modified(newFullName);
        if (dbCountOldFile == 0 && dbCountNewFile == 0 && isNewFileValid) return Create(newFullName);
        if (dbCountOldFile == 1 && dbCountNewFile == 1 && !isNewFileValid) return Remove(newFullName);
        if (dbCountOldFile == 1 && dbCountNewFile == 0 && !isNewFileValid) return Remove(oldFullName);
        ;
        if (dbCountOldFile == 0 && dbCountNewFile == 1 && !isNewFileValid) return Remove(oldFullName);
        throw new Exception("The file doesn't fulfill the requirements for a rename.");
    }

    private PSScript RenameHelper(string oldFullName, string newFullName)
    {
        using var scope = _scopeFactory.CreateScope();
        var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var script = _dbContext.Script.First(psScript => psScript.FullPath.Equals(oldFullName));
        script.FullPath = newFullName;
        script.FileName = Path.GetFileName(newFullName);
        _logger.LogDebug("Renamed P SFile and persist", script);
        _dbContext.Script.Update(script);
        _dbContext.SaveChanges();
        return script;
    }
}