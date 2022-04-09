using Microsoft.VisualBasic.FileIO;
using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class DoubleArrayInputParameter : InputParameter
{
    public DoubleArrayInputParameter()
    {
        Type = SupportedTypes.DoubleArray;
    }
    public new double[] PValue { get; set; }
    public override void ParseValue(string value)
    {
        PValue = ParseArrayValue<double>(value);
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