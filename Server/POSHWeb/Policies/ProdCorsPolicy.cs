using Microsoft.AspNetCore.Cors.Infrastructure;

namespace POSHWeb.Core.Policies;

/// <summary>
///     This is the production CORS policy
/// </summary>
public class ProdCorsPolicy : ICorsPolicy
{
    public string Name { get; } = "ProdCorsPolicy";

    public void Apply(CorsPolicyBuilder builder)
    {
        builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials();
    }
}