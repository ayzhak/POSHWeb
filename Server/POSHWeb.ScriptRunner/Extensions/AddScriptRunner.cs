using Microsoft.Extensions.DependencyInjection;

namespace POSHWeb.ScriptRunner.Extensions;

public static class UseScriptRunner
{
    public static IServiceCollection AddScriptRunner(this IServiceCollection services)
    {
        return services
            .AddHostedService<QueuedHostedService>()
            .AddSingleton<IBackgroundTaskQueue>(_ => { return new DefaultBackgroundTaskQueue(100); })
            .AddSingleton(provider =>
            {
                var hostedRunspace = new HostedRunspace();
                hostedRunspace.InitializeRunspaces(1, 10, Array.Empty<string>());
                return hostedRunspace;
            });
    }
}