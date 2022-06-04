using Serilog;

namespace POSHWeb.Extensions;

public static class AppHostExtensions
{
    public static IHostBuilder UseHostCore(this IHostBuilder host)
    {
        return host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
    }
}