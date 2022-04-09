using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class IntegerArrayInputParameter : InputParameter
{
    public IntegerArrayInputParameter()
    {
        Type = SupportedTypes.IntegerArray;
    }

    public int[] PValue { get; set; }

    public override void ParseValue(string value)
    {
        PValue = ParseArrayValue<int>(value);
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