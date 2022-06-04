using POSHWeb.Client.Environment;

namespace POSHWeb.Agent.Model;

public class RunspaceSettings
{
    public RunspaceSettings()
    {
        ThreadType = RunspaceThread.Default;
        Modules = new List<string>();
        Commands = new List<string>();
        Variables = new List<Variables>();
    }

    public int MinRunspaces { get; set; }
    public int MaxRunspaces { get; set; }
    public RunspaceOperationMode OperationMode { get; set; }
    public RunspaceThread ThreadType { get; set; }
    public ICollection<string> Modules { get; }
    public ICollection<string> Commands { get; }
    public ICollection<Variables> Variables { get; }
}