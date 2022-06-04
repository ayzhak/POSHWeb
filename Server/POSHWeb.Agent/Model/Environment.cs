using System.Diagnostics;
using System.Security.Claims;
using Grpc.Core;
using POSHWeb.Client.Environment;

namespace POSHWeb.Agent.Model;

public class Environment
{
    public Guid Id { get; set; }
    public EnvState State { get; set; }
    public string ExecutablePath { get; set; }
    public Process Process { get; set; }
    public string Version { get; set; }
    public RunspaceSettings RunspaceSettings { get; set; }
    public IServerStreamWriter<ExecutionResponse>? Stream { get; set; }

    public IEnumerable<Claim> Claims => new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Id.ToString()) };
}

public enum EnvState
{
    Starting,
    Registred,
    Ready,
    Running
}