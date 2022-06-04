using Microsoft.AspNetCore.Cors.Infrastructure;

namespace POSHWeb.Core.Policies;

public class DevCorsPolicy : ICorsPolicy
{
    public string Name { get; } = "DevCorsPolicy";

    public void Apply(CorsPolicyBuilder builder)
    {
        builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials();
    }
}