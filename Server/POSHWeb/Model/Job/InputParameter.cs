using Microsoft.VisualBasic.FileIO;
using POSHWeb.Enum;
using POSHWeb.Model.Script;

namespace POSHWeb.Model;

public class InputParameter : SimpleInputParameter
{
    public int Id { get; set; }
    public string Type { get; set; }
    public JobParameterState State { get; set; }

    public virtual void ParseValue(string value)
    {
        throw new NotImplementedException();
    }

    public virtual JobParameterState ValidateValue(PSParameterOptions options)
    {
        throw new NotImplementedException();
    }

    public virtual void AddToDictonary(Dictionary<string, object> dictionary)
    {
        throw new NotImplementedException();
    }

    protected T[] ParseArrayValue<T>(string value)
    {
        if (value == "" || value == null) return null;
        var list = new List<T>();
        var parser = new TextFieldParser(new StringReader(value));
        parser.SetDelimiters(",", ";");
        parser.HasFieldsEnclosedInQuotes = true;
        var fields = parser.ReadFields();
        if (fields == null) return null;
        foreach (var s in fields) list.Add((T) Convert.ChangeType(s, typeof(T)));

        return list.ToArray();
    }
}