namespace POSHWeb.Model;

public class PSHelp : BaseEntity
{
    public int Id { get; set; }
    public string? Component { get; set; }
    public string? Description { get; set; }
    public string[]? Examples { get; set; }
    public string? ForwardHelpCategory { get; set; }
    public string? FotwardHelpTragetName { get; set; }
    public string? Functionality { get; set; }
    public string[]? Inputs { get; set; }
    public string[]? Outputs { get; set; }
    public string[]? Links { get; set; }
    public string? MamlHelpFile { get; set; }
    public string? Notes { get; set; }
    public string? RemoteHelpRunspace { get; set; }
    public string? Role { get; set; }
    public string? Synopsis { get; set; }
}