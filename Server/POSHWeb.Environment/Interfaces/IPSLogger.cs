using System;
using System.Management.Automation;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Model;
using POSHWeb.Environment.Runspace.Host;
using POSHWeb.Model.Job.Logs;

namespace POSHWeb.Environment.Interfaces;

public interface IPSLogger
{
    public void Register(PowerShell powershell)
    {
        powershell.Streams.Error.DataAdded += (sender, e) =>
        {
            var streamObjectsReceived = sender as PSDataCollection<ErrorRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];
            Log(SeverityLevel.Error, currentStreamRecord.Exception.ToString());
        };
        powershell.Streams.Warning.DataAdded += (sender, e) =>
        {
            var streamObjectsReceived = sender as PSDataCollection<WarningRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];
            Log(SeverityLevel.Warning, currentStreamRecord.Message);
        };

        powershell.Streams.Information.DataAdded += (sender, e) =>
        {
            var streamObjectsReceived = sender as PSDataCollection<InformationRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];
            Log(SeverityLevel.Information, currentStreamRecord.MessageData.ToString());
        };
        powershell.Streams.Debug.DataAdded += (sender, e) =>
        {
            var streamObjectsReceived = sender as PSDataCollection<DebugRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];
            Log(SeverityLevel.Information, currentStreamRecord.Message);
        };
        ;
        powershell.Streams.Verbose.DataAdded += (sender, e) =>
        {
            var streamObjectsReceived = sender as PSDataCollection<VerboseRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];
            Log(SeverityLevel.Verbose, currentStreamRecord.Message);
        };
        ;
        powershell.Streams.Progress.DataAdded += (sender, e) =>
        {
            var streamObjectsReceived = sender as PSDataCollection<ProgressRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];
            Log(SeverityLevel.Progress, LogProgressConverter.ConvertLogEntry(currentStreamRecord));
        };
    }

    public void Log(SeverityLevel level, string log);

    public void Log(SeverityLevel level, LogProgress progress);
}