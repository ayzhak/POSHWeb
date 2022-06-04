using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using POSHWeb.Client.Web.Core.Options;
using POSHWeb.Core.Policies;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace POSHWeb.Extensions;

public static class AppServiceExtensions
{
    public static IServiceCollection AddServiceCore(this IServiceCollection services)
    {
        AddCorsPolicies(services);
        return services.AddVersioning()
            .AddSwaggerVersioning()
            .AddProblemDetails()
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    private static void AddCorsPolicies(IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(mytype => mytype.GetInterfaces().Contains(typeof(ICorsPolicy)));
        foreach (var t in types)
        {
            var policy = (ICorsPolicy) Activator.CreateInstance(t)!;
            services.AddCors(options => { options.AddPolicy(policy.Name, policy.Apply); });
        }
    }

    private static IServiceCollection AddProblemDetails(this IServiceCollection services)
    {
        return services.AddProblemDetails(setup =>
        {
            setup.IncludeExceptionDetails = (ctx, ex) =>
            {
                // Fetch services from HttpContext.RequestServices
                var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };
        });
    }

    private static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-API-Version"),
                new MediaTypeApiVersionReader("v"),
                new UrlSegmentApiVersionReader());
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    private static IServiceCollection AddSwaggerVersioning(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // for further customization
            //options.OperationFilter<DefaultValuesFilter>();
            options.DocumentFilter<SwaggerDocumentFilter>();
        });
        services.ConfigureOptions<SwaggerOptions>();

        return services;
    }

    internal class SwaggerDocumentFilter : IDocumentFilter
    {
        private readonly string _swaggerDocHost;

        public SwaggerDocumentFilter(IHttpContextAccessor httpContextAccessor)
        {
            var host = httpContextAccessor.HttpContext.Request.Host.Value;
            var scheme = httpContextAccessor.HttpContext.Request.Scheme;
            _swaggerDocHost = $"{scheme}://{host}";
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers.Add(new OpenApiServer
            {
                Url = _swaggerDocHost
            });
        }
    }
}