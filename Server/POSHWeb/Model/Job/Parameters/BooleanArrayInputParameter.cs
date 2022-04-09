using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class BooleanArrayInputParameter : InputParameter
{
    public BooleanArrayInputParameter()
    {
        Type = SupportedTypes.BooleanArray;
    }

    public bool[] PValue { get; set; }

    public override void ParseValue(string value)
    {
        PValue = ParseArrayValue<bool>(value);
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