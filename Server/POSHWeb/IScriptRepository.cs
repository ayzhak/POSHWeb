using POSHWeb.Model;

namespace POSHWeb;

public interface IScriptRepository
{
    public PSScript Create(PSScript script);
}