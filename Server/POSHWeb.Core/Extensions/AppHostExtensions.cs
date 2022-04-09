using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POSHWeb.Client.Web.Core.Options;
using POSHWeb.Options;
using Serilog;

namespace POSHWeb.Client.Web.Core.Extensions
{
    public static class AppHostExtensions
    {
        public static IHostBuilder UseHostCore(this IHostBuilder host)
        {

            return host.UseSerilog(((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration)));
        }
    }
}
