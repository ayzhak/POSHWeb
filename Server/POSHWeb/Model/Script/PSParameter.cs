using System.ComponentModel.DataAnnotations;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class PSParameter : BaseEntity
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; }
    public string Default { get; set; }

    public Boolean Mandatory { get; set; }
    public int Order { get; set; }
    public string? ErrorMessage { get; set; }
    public string? HelpMessage { get; set; }

    public PSParameterOptions Options { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        PSParameter other = obj as PSParameter;

        return this.Name == other.Name
               && this.Type == other.Type;
    }
}