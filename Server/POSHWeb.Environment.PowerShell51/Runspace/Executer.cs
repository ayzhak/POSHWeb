using System.Management.Automation;
using POSHWeb.Common.Exceptions;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Model;
using POSHWeb.Environment.Runspace;

namespace POSHWeb.Environment.PowerShell51.Runspace;

public class Executer: IExecuter
{
    private readonly IPSInteraction _interaction;
    private readonly IPSLogger _log;

    public IRunspaceManager RunspaceManager { get; private set; }

    public Executer(RunspaceSettings runspaceSettings, IPSInteraction interaction, IPSLogger logger = null)
    {
        _interaction = interaction;
        RunspaceManager = new RunspaceManager(runspaceSettings, logger, interaction);
    }

    public ICollection<PSObject> Run(string script, Dictionary<string, object> parameters = null)
    {
        if (!RunspaceManager.Running) throw new Exception("Runspace isn't running.");
        ICollection<PSObject> result;
        using (var ps = PowerShell.Create())
        {
            RunspaceManager.Attach(ps);
            ps.AddScript(script);
            if (parameters != null) ps.AddParameters(parameters);
            try
            {
                result = ps.Invoke();
                if (ps.HadErrors)
                {
                    foreach (var error in ps.Streams.Error)
                    {
                        var ex = error.Exception;
                        _interaction.Log(SeverityLevel.Error, ex.ToString());
                        _interaction.Exception(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _interaction.Log(SeverityLevel.Error, ex.ToString());
                _interaction.Exception(ex);
                throw new ExecutionException("Error while executing Script", ex);
            }
        }

        return result;
    }
}