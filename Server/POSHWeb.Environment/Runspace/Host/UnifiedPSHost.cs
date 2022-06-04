using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation.Host;
using System.Text;
using System.Threading.Tasks;

namespace POSHWeb.Environment.Runspace.Host
{
    public class UnifiedPSHost : PSHost, IHostSupportsInteractiveSession
    {
        public override string Name { get; } = "POSHWeb Host";
        public override Version Version { get; } = new Version(1, 0);
        public override CultureInfo CurrentCulture { get; } = CultureInfo.CurrentCulture;
        public override CultureInfo CurrentUICulture { get; } = CultureInfo.CurrentUICulture;
        public System.Management.Automation.Runspaces.Runspace Runspace { get; private set; }
        public bool IsRunspacePushed => Runspace != null;
        public override Guid InstanceId { get; }
        public override PSHostUserInterface UI { get; }
        public UnifiedPSHost(PSHostUserInterface ui)
        {
            UI = ui;
            InstanceId = Guid.NewGuid();
        }
        public override void SetShouldExit(int exitCode) => throw new NotImplementedException();

        public override void EnterNestedPrompt() => throw new NotImplementedException();

        public override void ExitNestedPrompt() => throw new NotImplementedException();

        public override void NotifyBeginApplication() => throw new NotImplementedException();

        public override void NotifyEndApplication() => throw new NotImplementedException();

        public void PushRunspace(System.Management.Automation.Runspaces.Runspace runspace) => Runspace = runspace;

        public void PopRunspace()
        {
            if (Runspace == null)
                throw new InvalidOperationException("No runspace to pop.");
            Runspace = null;
        }


    }
}
