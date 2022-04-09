using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class BooleanInputParameter : InputParameter
{
    public BooleanInputParameter()
    {
        Type = SupportedTypes.Boolean;
    }

    public new Boolean PValue { get; set; }

    public override void ParseValue(string value)
    {
        PValue = bool.Parse(value);
    }

    public override JobParameterState ValidateValue(PSParameterOptions options)
    {
        return options.Valid(PValue);
    }
    public override void AddToDictonary(Dictionary<string, object> dictionary)
    {
        dictionary.Add(Name, PValue);
    }
}