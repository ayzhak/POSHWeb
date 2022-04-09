using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace POSHWeb.Client.Web.Core.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseAppCore(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            app.UseCors(builder =>
            {
                if (environment.IsDevelopment())
                {
                    app.UseCors("DevCorsPolicy");
                }
                else
                {
                    app.UseCors("ProdCorsPolicy");
                }

            });
            return app.UseConfiguredSwagger()
                .UseProblemDetails();
        }

        private static IApplicationBuilder UseConfiguredSwagger(this IApplicationBuilder app)
        {
            IServiceProvider services = app.ApplicationServices;
            var provider = services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                    options.RoutePrefix = "docs";
                }
            });

            return app;
        }
    }
}