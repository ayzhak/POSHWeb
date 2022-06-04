using Microsoft.AspNetCore.Cors.Infrastructure;

namespace POSHWeb.Core.Policies;

public interface ICorsPolicy
{
    string Name { get; }
    void Apply(CorsPolicyBuilder builder);
}