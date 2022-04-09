using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class FloatInputParameter : InputParameter
{
    public FloatInputParameter()
    {
        Type = SupportedTypes.Float;
    }
    public new float PValue { get; set; }
    public override void ParseValue(string value)
    {
        PValue = float.Parse(value);
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