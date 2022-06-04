using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Model;
using POSHWeb.Environment.PowerShell51.Runspace.Host;
using POSHWeb.Environment.Runspace.Host;

namespace POSHWeb.Environment.Runspace;

public class RunspaceManager : IDisposable, IRunspaceManager
{
    private readonly IPSLogger _logger;
    private readonly IPSInteraction _interaction;
    private UnifiedPSHost _unifiedPsHost;
    public RunspaceSettings RunspaceSettings { get; }
    public bool Running { get; private set; } = false;
    public RunspacePool RunspacePool { get; private set; }
    public System.Management.Automation.Runspaces.Runspace Runspace { get; private set; }
    private InitialSessionState _initialSessionState;

    public RunspaceManager(RunspaceSettings runspaceSettings, IPSLogger logger, IPSInteraction interaction)
    {
        ValidateSettings(runspaceSettings);
        _logger = logger;
        _interaction = interaction;
        RunspaceSettings = runspaceSettings;
        _initialSessionState = CreateInitialSessionState(RunspaceSettings);
        _initialSessionState.ThrowOnRunspaceOpenError = false;
        var rawUi = new UnifiedUIRawInterface();
        var ui = new UnifiedHostUserInterface(_interaction, rawUi);
        _unifiedPsHost = new UnifiedPSHost(ui);
    }

    private void ValidateSettings(RunspaceSettings runspaceSettings)
    {
        if (runspaceSettings.ThreadType == null) throw new ArgumentException("The ThreadType isn't set.");
        if (runspaceSettings.Type == null) throw new ArgumentException("The Type isn't set.");
        if (runspaceSettings.Type == RunspaceType.Pool)
        {
            if (runspaceSettings.MinRunspaces == null) throw new ArgumentException("MinRunspaces isn't set.");
            if (runspaceSettings.MaxRunspaces == null) throw new ArgumentException("MinRunspaces isn't set.");
        }
    }



    public void Start()
    {
        switch (RunspaceSettings.Type)
        {
            case RunspaceType.Single:
                if (Runspace != null) throw new InvalidOperationException("Runspace is already iniziated");
                Runspace = RunspaceFactory.CreateRunspace(_unifiedPsHost, _initialSessionState);
                Runspace.Open();
                break;
            case RunspaceType.Pool:
                if (RunspacePool != null) throw new InvalidOperationException("RunspacePool is already iniziated");
                RunspacePool = RunspaceFactory.CreateRunspacePool(
                    RunspaceSettings.MinRunspaces,
                    RunspaceSettings.MaxRunspaces,
                    _initialSessionState,
                    _unifiedPsHost);
                RunspacePool.Open();
                break;
            case RunspaceType.Isolated:
                break;
        }

        Running = true;
    }

    public void Attach(PowerShell powershell)
    {
        switch (RunspaceSettings.Type)
        {
            case RunspaceType.Single:
                if (Runspace == null) throw new InvalidOperationException("Runspace not iniziated");
                powershell.Runspace = Runspace;
                powershell.InvocationStateChanged += (sender, args) =>
                {
                    _interaction.StateChange(args.InvocationStateInfo.State, args.InvocationStateInfo.Reason);
                };
                if (_logger != null) _logger.Register(powershell);
                break;
            case RunspaceType.Pool:
                if (RunspacePool == null) throw new InvalidOperationException("RunspacePool not iniziated");
                powershell.RunspacePool = RunspacePool;
                powershell.InvocationStateChanged += (sender, args) =>
                {
                    _interaction.StateChange(args.InvocationStateInfo.State, args.InvocationStateInfo.Reason);
                };
                if (_logger != null) _logger.Register(powershell);
                break;
            case RunspaceType.Isolated:
                var runspace = RunspaceFactory.CreateRunspace(_unifiedPsHost, _initialSessionState);

                runspace.Open();
                powershell.Runspace = runspace;
                powershell.InvocationStateChanged += (sender, args) =>
                {
                    _interaction.StateChange(args.InvocationStateInfo.State, args.InvocationStateInfo.Reason);
                };
                if (_logger != null) _logger.Register(powershell);
                break;
        }
    }

    public void Stop()
    {
        switch (RunspaceSettings.Type)
        {
            case RunspaceType.Single:
                if (Runspace == null) return;
                Runspace.Close();
                Runspace.Dispose();
                Runspace = null;
                break;
            case RunspaceType.Pool:
                if (RunspacePool == null) return;
                ;
                RunspacePool.Close();
                Runspace.Dispose();
                RunspacePool = null;
                break;
            case RunspaceType.Isolated:
                break;
        }

        Running = false;
    }

    private InitialSessionState CreateInitialSessionState(RunspaceSettings runspaceSettings)
    {
        var iis = InitialSessionState.CreateDefault();
        if (runspaceSettings.Modules.Any())
        {
            iis.ImportPSModule(runspaceSettings.Modules.ToArray());
        }

        foreach (var command in runspaceSettings.Commands)
        {
            iis.Commands.Add(new SessionStateCmdletEntry(command, null, null));
        }

        foreach (var variable in runspaceSettings.Variables)
        {
            iis.Variables.Add(new SessionStateVariableEntry(variable.Name, variable.Value, variable.Description));
        }

        iis.ThreadOptions = runspaceSettings.ThreadType;
        iis.LanguageMode = PSLanguageMode.FullLanguage;
        iis.AuthorizationManager = new AuthorizationManager("Microsoft.PowerShell");
        return iis;
    }

    public void Dispose()
    {
        Stop();
    }
}