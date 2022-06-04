using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Model;
using POSHWeb.Model.Job.Logs;

namespace POSHWeb.Environment;

public class PSInteractionConsole: IPSInteraction
{
    public void Log(SeverityLevel level, string message)
    {
        Console.Write($"[{level}] {message}");
    }

    public void Progress(SeverityLevel level, LogProgress record)
    {
        Console.Write($"[{level}] {record}");
    }

    public string Input()
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
    {
        throw new NotImplementedException();
    }

    public PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
    {
        throw new NotImplementedException();
    }

    public int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
    {
        throw new NotImplementedException();
    }

    public void StateChange(PSInvocationState state, Exception reason)
    {
        Console.Write($"[State Change] {state}");
    }

    public void Exception(Exception ex)
    {
        Console.Write($"[{SeverityLevel.Error}] {ex}");
    }
}