using System.Management.Automation.Language;
using Microsoft.EntityFrameworkCore;
using Microsoft.PowerShell.Commands;
using POSHWeb.DAL;
using POSHWeb.Data;
using POSHWeb.Exceptions;
using POSHWeb.Model;

namespace POSHWeb.Services;

public class ScriptFSSyncService
{
    private readonly UnitOfWork unitOfWork;
    private readonly HasherService _hasherService;
    private readonly ILogger<ScriptFSSyncService> _logger;
    private readonly PSParserService _PSParserService;
    private readonly ScriptValidatorService _psScriptValidator;

    public ScriptFSSyncService(UnitOfWork unitOfWork, HasherService hasherService,
        ILogger<ScriptFSSyncService> logger, ScriptValidatorService psScriptValidator,
        PSParserService psParserService)
    {
        this.unitOfWork = unitOfWork;
        _hasherService = hasherService;
        _logger = logger;
        _psScriptValidator = psScriptValidator;
        _PSParserService = psParserService;
    }

    public void Sync(string directory)
    {
        var files = Directory.GetFiles(directory, "*.ps1", SearchOption.AllDirectories).ToList();
        files.ForEach(path =>
        {
            using (unitOfWork)
            {
                var content = File.ReadAllText(path);
                var hash = _hasherService.Sha256(content);
                var matchingFilenames =
                    unitOfWork.ScriptRepository.Get(script => script.FullPath.Equals(path)).ToList();
                var matchingHashes = unitOfWork.ScriptRepository.Get(script => script.Content.Equals(hash)).ToList();
                if (matchingFilenames.Count == 1 && matchingHashes.Count == 1) return;
                if (matchingFilenames.Count == 1 && matchingHashes.Count == 0) Modified(path);
                if (matchingFilenames.Count == 0 && matchingHashes.Count == 1)
                {
                    var script = matchingHashes[1];
                    script.FullPath = path;
                    script.FileName = Path.GetFileName(path);
                    unitOfWork.ScriptRepository.Update(script);
                    unitOfWork.Save();
                }

                Create(path);
            }
        });
    }

    public PSScript Create(string fullPath)
    {
        using (unitOfWork)
        {
            if (!_psScriptValidator.IsValidPsFile(fullPath)) throw new InvalidFileExtensionException();
            if (!_psScriptValidator.ValidateFile(fullPath)) throw new InvalidPowerShellContentException();

            var content = File.ReadAllText(fullPath);
            var script = new PSScript
            {
                Content = content,
                FileName = Path.GetFileName(fullPath),
                ContentHash = _hasherService.Sha256(content),
                FullPath = fullPath,
                Help = _PSParserService.GetScriptHelp(content)
            };
            script.Parameters = _PSParserService.Parse(content);
            _logger.LogDebug("Create new scriptExecution and persist", script);
            using (unitOfWork)
            {
                unitOfWork.ScriptRepository.Insert(script);
                unitOfWork.Save();
            }

            return script;
        }
    }


    public PSScript Remove(string fullPath)
    {
        _logger.LogInformation("PowerShell file removed", fullPath);
        if (_psScriptValidator.IsValidPsFile(fullPath))
            throw new FileStillExistException($"Can not remove file {fullPath}, because it still exist.");
        using (unitOfWork)
        {
            var scripts = unitOfWork.ScriptRepository.Get(script => script.FullPath == fullPath).ToList();
            if (scripts.Count > 1) throw new MoreThanOneDatabaseEntryException();
            var script = scripts[0];
            _logger.LogDebug("Remove PowerShell file from DB", script);
            unitOfWork.ScriptRepository.Delete(script);
            unitOfWork.Save();
            return script;
        }
    }

    public PSScript Modified(string fullPath)
    {
        _logger.LogInformation("PowerShell file modified", fullPath);
        if (!_psScriptValidator.IsValidPsFile(fullPath)) throw new InvalidFileExtensionException();
        if (!_psScriptValidator.ValidateFile(fullPath)) throw new InvalidPowerShellContentException();


        var scripts = unitOfWork.ScriptRepository
            .Get(script => script.FullPath == fullPath, include: queryable =>
            {
                return queryable.Include(psScript => psScript.Parameters)
                    .ThenInclude(parameter => parameter.Options);
            })
            .ToList();
        if (scripts.Count > 1 || scripts.Count == 0)
            throw new Exception($"The ScriptExecution with '{fullPath}' has {scripts.Count} matches in the Database");
        var script = scripts[0];
        var content = File.ReadAllText(fullPath);
        script.Content = content;
        script.Parameters = _PSParserService.Parse(content);
        script.ContentHash = _hasherService.Sha256(content);
        _logger.LogDebug("Updated PS File and persist", script);
        unitOfWork.ScriptRepository.Update(script);
        unitOfWork.Save();
        return script;
    }

    public PSScript Rename(string oldFullName, string newFullName)
    {
        _logger.LogInformation("PS File rename", oldFullName, newFullName);
        var isNewFileValid = _psScriptValidator.IsValidPsFile(newFullName) &&
                             _psScriptValidator.ValidateFile(newFullName);
        using (unitOfWork)
        {
            var dbCountOldFile = unitOfWork.ScriptRepository.Get(script => script.FullPath.Equals(oldFullName)).Count();
            var dbCountNewFile = unitOfWork.ScriptRepository.Get(script => script.FullPath.Equals(newFullName)).Count();
            if (dbCountOldFile > 1) throw new MoreThanOneDatabaseEntryException();
            if (dbCountNewFile > 1) throw new MoreThanOneDatabaseEntryException();
            if (dbCountOldFile == 1 && dbCountNewFile == 1 && isNewFileValid) return Modified(newFullName);
            if (dbCountOldFile == 1 && dbCountNewFile == 0 && isNewFileValid)
                return RenameHelper(oldFullName, newFullName);
            if (dbCountOldFile == 0 && dbCountNewFile == 1 && isNewFileValid) return Modified(newFullName);
            if (dbCountOldFile == 0 && dbCountNewFile == 0 && isNewFileValid) return Create(newFullName);
            if (dbCountOldFile == 1 && dbCountNewFile == 1 && !isNewFileValid) return Remove(newFullName);
            if (dbCountOldFile == 1 && dbCountNewFile == 0 && !isNewFileValid) return Remove(oldFullName);
            if (dbCountOldFile == 0 && dbCountNewFile == 1 && !isNewFileValid) return Remove(oldFullName);
            throw new Exception("The file doesn't fulfill the requirements for a rename.");
        }
    }

    private PSScript RenameHelper(string oldFullName, string newFullName)
    {
        using (unitOfWork)
        {
            var script = unitOfWork.ScriptRepository.Get(psScript => psScript.FullPath.Equals(oldFullName)).First();
            script.FullPath = newFullName;
            script.FileName = Path.GetFileName(newFullName);
            _logger.LogDebug("Renamed P SFile and persist", script);
            unitOfWork.ScriptRepository.Update(script);
            unitOfWork.Save();
            return script;
        }
    }
}