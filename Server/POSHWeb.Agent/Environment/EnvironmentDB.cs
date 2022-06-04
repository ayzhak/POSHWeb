using POSHWeb.Agent.Services;

namespace POSHWeb.Agent.Environment;

public class EnvironmentDB
{
    public ICollection<Model.Environment> Environments { get; }

    public EnvironmentDB(TokenService tokenService)
    {
        Environments = new List<Model.Environment>();
    }
}