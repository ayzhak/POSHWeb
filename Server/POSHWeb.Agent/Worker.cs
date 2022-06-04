using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using POSHWeb.Agent.Environment;
using POSHWeb.Agent.Model;
using POSHWeb.Client.Environment;

namespace POSHWeb.Agent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly EnvironmentManager _environmentManager;
    private readonly IServer _server;
    private readonly TaskCompletionSource _source = new();
    private readonly IHostApplicationLifetime _lifetime;

    public Worker(ILogger<Worker> logger, EnvironmentManager environmentManager, IServer server,
        IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _environmentManager = environmentManager;
        _server = server;
        _lifetime = lifetime;
        _lifetime.ApplicationStarted.Register(() => _source.SetResult());
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            if (!WaitForAppStartup(_lifetime, stoppingToken).Result)
            {
                return;
            }

            var address = _server
                .Features
                .Get<IServerAddressesFeature>()!
                .Addresses
                .FirstOrDefault();

            var env = new Agent.Model.Environment
            {
                Id = Guid.NewGuid(),
                Version = "1.0",
                RunspaceSettings = new RunspaceSettings
                {
                    MinRunspaces = 1,
                    MaxRunspaces = 2,
                    OperationMode = RunspaceOperationMode.Single,
                    ThreadType = RunspaceThread.Default
                }
            };
            Console.WriteLine(address);
            _environmentManager.Start(env, address);

            _environmentManager.WaitToComplete(env);
            env.Stream.WriteAsync(new ExecutionResponse {Content = $"Write-Host \"asdf\""});
        });
    }

    static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
    {
        var startedSource = new TaskCompletionSource();
        var cancelledSource = new TaskCompletionSource();

        using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
        using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

        Task completedTask = await Task.WhenAny(
            startedSource.Task,
            cancelledSource.Task).ConfigureAwait(false);

        // If the completed tasks was the "app started" task, return true, otherwise false
        return completedTask == startedSource.Task;
    }
}