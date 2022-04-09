using System.Management.Automation.Language;

namespace POSHWeb.Services;

public class PSScriptValidator
{
    private readonly ILogger<PSScriptValidator> _logger;

    public PSScriptValidator(ILogger<PSScriptValidator> logger)
    {
        _logger = logger;
    }

    public bool ValidateFile(string fullPath)
    {
        string content = File.ReadAllText(fullPath);
        return ValidateInput(content);
    }

    public bool ValidateInput(string content)
    {
        if (content == "")
        {
            _logger.LogDebug("Content is empty");
            return true;
        };
        if (content == null)
        {
            _logger.LogDebug("Content is null");
            return false;
        };
        Token[] tokens;
        ParseError[] parseErrors;
        Parser.ParseInput(content, out tokens, out parseErrors);
        if(parseErrors.Length != 0) _logger.LogWarning("Content isn't valid PowerShell Script", content);
        if (parseErrors.Length == 0) _logger.LogDebug("Content is valid PowerShell Script");
        return parseErrors.Length == 0;
    }

    public bool IsValidPsFile(string fullPath)
    {
        _logger.LogDebug("Check if file exist", fullPath);
        if (!File.Exists(fullPath))
        {
            _logger.LogError("File doesn't exist", fullPath);
            return false;
        }
        var file = new FileInfo(fullPath);
        _logger.LogDebug("Check if has .ps1 extension", fullPath);
        if (file.Extension.ToLower() != ".ps1")
        {
            _logger.LogWarning("Doesn't have file extension .ps1", fullPath);
            return false;
        };
        return true;
    }
}