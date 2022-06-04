using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Model;
using POSHWeb.Model.Job.Logs;

namespace POSHWeb.Environment.Interfaces;

public interface IPSInteraction
{
    public void Log(SeverityLevel level, string message);

    public void Progress(SeverityLevel level, LogProgress record);
    public string Input();
    public Dictionary<string, PSObject> Prompt(string caption, string message,
        Collection<FieldDescription> descriptions);

    public PSCredential PromptForCredential(string caption, string message, string userName, string targetName);
    public int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices,
        int defaultChoice);
    public void StateChange(PSInvocationState state, Exception reason);
    void Exception(Exception ex);
}