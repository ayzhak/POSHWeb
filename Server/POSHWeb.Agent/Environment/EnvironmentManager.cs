using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using Grpc.Core;
using POSHWeb.Agent.Model;
using POSHWeb.Agent.Services;
using POSHWeb.Client.Environment;

namespace POSHWeb.Agent.Environment;

public class EnvironmentManager
{
    private readonly TokenService _tokenService;
    private readonly EnvironmentDB _db;

    public EnvironmentManager(TokenService tokenService, EnvironmentDB db)
    {
        _tokenService = tokenService;
        _db = db;
    }

    public Model.Environment Start(Agent.Model.Environment environment, string url)
    {
        
        var jwtToken = _tokenService.GenerateToken(environment);
        //string dll = "C:\\Repos\\GitHub\\POSHWeb\\Server\\POSHWeb.Client.Environment.Executer\\bin\\Debug\\POSHWeb.Client.Environment.Executer.dll";
        //ProcessStartInfo info = new ProcessStartInfo();
        //info.UseShellExecute = false;
        //info.CreateNoWindow = false;
        //info.WindowStyle = ProcessWindowStyle.Maximized;
        //info.FileName = "C:\\Windows\\system32\\WindowsPowerShell\\v1.0\\powershell.exe";
        //info.Arguments =
        //    $"-NoLogo -NoProfile -Command \"& {{[System.Reflection.Assembly]::LoadFrom('{dll}') | Out-Null; [POSHWeb.Client.Environment.Executer.Client]::Start('{url}', '{jwtToken}') }}\"";
        //var process = Process.Start(info);
        //environment.Process = process;
        _db.Environments.Add(environment);
        Console.WriteLine("Token: " + jwtToken);
        return environment;
    }

    public void Stop(Agent.Model.Environment environment)
    {
        environment.Process.Kill();
        _db.Environments.Remove(environment);
    }

    public RunspaceSettings Registration(string token)
    {
        try
        {
            var env = _db.Environments.First(environment => environment.Id.ToString() == token);
            if (env == null) throw new Exception("No Env found");
            env.State = EnvState.Registred;
            return env.RunspaceSettings;
        }
        catch (InvalidOperationException e)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No environment found"));
        }
    }

    public bool HasStream(string token)
    {
        return
            false; //_db.Environments.Any(environment => environment.Id.ToString() == token && environment.Stream != null);
    }

    public void RegisterStream(string id, IServerStreamWriter<ExecutionResponse> stream)
    {
        var env = _db.Environments.First(environment => environment.Id.ToString() == id);
        env.Stream = stream;
    }

    public void Remove(string id)
    {
        // _db.Environments.Remove(_db.Environments.First(environment => environment.Id.ToString() == id));
    }

    public bool IsValid(string domain, string username, string password)
    {
        using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
        {
            return context.ValidateCredentials(domain + "\\" + username, password);
        }
    }

    public void WaitToComplete(Model.Environment env)
    {
        while (_db.Environments.Any(environment => environment.Id == env.Id && env.Stream == null))
        {
        }
    }
}