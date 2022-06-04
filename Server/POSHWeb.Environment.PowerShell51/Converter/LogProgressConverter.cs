using System.Management.Automation;
using POSHWeb.Environment.Enum;
using POSHWeb.Model.Job.Logs;

namespace POSHWeb.Environment.Runspace.Host;

public static class LogProgressConverter
{
    public static LogProgress ConvertLogEntry(ProgressRecord record)
    {
        var log = new LogProgress
        {
            Activity = record.Activity,
            Severity = SeverityLevel.Progress,
            CurrentOperation = record.CurrentOperation,
            StatusDescription = record.StatusDescription,
            SecondsRemaining = record.SecondsRemaining,
            PercentComplete = record.PercentComplete
        };
        log.Message = log.ToString();
        return log;
    }
}