using Microsoft.VisualBasic.FileIO;
using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class StringArrayInputParameter : InputParameter
{
    public StringArrayInputParameter()
    {
        Type = SupportedTypes.StringArray;
    }
    public new string[] PValue { get; set; }
    public override void ParseValue(string value)
    {
        PValue = ParseArrayValue<string>(value);
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