using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using POSHWeb.Common.Enum;
using POSHWeb.Common.Model.Runspace;
using POSHWeb.Environment.Enum;
using POSHWeb.Model;

namespace POSHWeb.Environment.Model
{
    public class RunspaceSettings: BaseEntity
    {
        public PSRuntimeVersion RuntimeVersion { get; set; } 
        public int MinRunspaces { get; set; }
        public int MaxRunspaces { get; set; }
        public RunspaceType Type { get; set; }
        public PSThreadOptions ThreadType { get; set; }
        public ICollection<string> Modules { get; }
        public ICollection<string> Commands { get; }
        public ICollection<Variables> Variables { get; }
        public RunspaceSettings()
        {
            RuntimeVersion = PSRuntimeVersion.PS7;
            Type = RunspaceType.Isolated;
            ThreadType = PSThreadOptions.Default;
            Modules = new List<string>();
            Commands = new List<string>();
            Variables = new List<Variables>
            {
                new(name: "DebugPreference", value: ActionPreference.Continue),
                new(name: "WarningPreference", value: ActionPreference.Continue),
                new(name: "ErrorActionPreference", value: ActionPreference.Continue),
                new(name: "VerbosePreference", value: ActionPreference.Continue),
                new(name: "InformationPreference", value: ActionPreference.Continue),
            };
            MinRunspaces = 1;
            MaxRunspaces = 1;
        }
    }
}