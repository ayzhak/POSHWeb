using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class DoubleInputParameter : InputParameter
{
    public DoubleInputParameter()
    {
        Type = SupportedTypes.Double;
    }
    public new double PValue { get; set; }
    public override void ParseValue(string value)
    {
        PValue = double.Parse(value);
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