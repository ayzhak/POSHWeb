using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class DateTimeInputParameter : InputParameter
{
    public DateTimeInputParameter()
    {
        Type = SupportedTypes.DateTime;
    }
    public new DateTime PValue { get; set; }
    public override void ParseValue(string value)
    {
        PValue = DateTime.Parse(value);
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