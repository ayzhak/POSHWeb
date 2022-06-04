using System.Management.Automation;

namespace POSHWeb.Environment.Converter;

public static class PSObjectConverter
{
    public static string ToJson(PSObject pso)
    {
        var ps = PowerShell.Create();
        ps.AddCommand("ConvertTo-Json");
        ps.AddParameter("InputObject", pso);
        ps.AddParameter("Depth", 10);
        ps.AddParameter("Compress", true);
        ps.AddParameter("EnumsAsStrings", true);
        var json = ps.Invoke();
        return json.First().ToString();
    }
}