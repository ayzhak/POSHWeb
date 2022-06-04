using POSHWeb.Enum;
using POSHWeb.Model.Job;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class DefaultValue : BasicType
{
    public int Id { get; set; }
}

public class PSParameter : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public SupportedType Type { get; set; }
    public string TrueType { get; set; }
    public bool Mandatory { get; set; }
    public int Order { get; set; }
    public DefaultValue? Default { get; set; }
    public string? ErrorMessage { get; set; }
    public string? HelpMessage { get; set; }
    public PSParameterOptions Options { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        var other = obj as PSParameter;

        return Name == other.Name
               && Type == other.Type;
    }
}