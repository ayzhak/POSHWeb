using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class DateTimeArrayInputParameter : InputParameter
{
    public DateTimeArrayInputParameter()
    {
        Type = SupportedTypes.DateTimeArray;
    }

    public DateTime[] PValue { get; set; }

    public override void ParseValue(string value)
    {
        PValue = ParseArrayValue<DateTime>(value);
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