using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class StringInputParameter : InputParameter
{
    public StringInputParameter()
    {
        Type = SupportedTypes.String;
    }

    public string PValue { get; set; }

    public override void ParseValue(string value)
    {
        PValue = value;
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