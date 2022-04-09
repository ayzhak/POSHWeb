using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class IntegerInputParameter : InputParameter
{
    public IntegerInputParameter()
    {
        Type = SupportedTypes.Int;
    }
    public new int PValue { get; set; }
    public override void ParseValue(string value)
    {
        PValue = int.Parse(value);
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