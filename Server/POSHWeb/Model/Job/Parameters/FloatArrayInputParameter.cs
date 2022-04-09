using Microsoft.VisualBasic.FileIO;
using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class FloatArrayInputParameter : InputParameter
{
    public FloatArrayInputParameter()
    {
        Type = SupportedTypes.FloatArray;
    }
    public new float[] PValue { get; set; }
    public override void ParseValue(string value)
    {
        PValue = ParseArrayValue<float>(value);
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