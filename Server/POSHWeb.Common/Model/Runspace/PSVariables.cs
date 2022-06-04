using POSHWeb.Model;

namespace POSHWeb.Common.Model.Runspace;

public class Variables: BaseEntity
{
    public Variables()
    {
    }

    public Variables(string name, object value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public object Value { get; set; }
    public string Description { get; set; }
}