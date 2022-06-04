using System.Text.Json;
using POSHWeb.Enum;

namespace POSHWeb.Model.Job;
public class BasicType
{
    public SupportedType Type { get; set; }
    public bool? Boolean { get; set; }
    public bool[]? BooleanArray { get; set; }
    public DateTime? DateTime { get; set; }
    public DateTime[]? DateTimeArray { get; set; }
    public double? Double { get; set; }
    public double[]? DoubleArray { get; set; }
    public float? Float { get; set; }
    public float[]? FloatArray { get; set; }
    public int? Integer { get; set; }
    public int[]? IntegerArray { get; set; }
    public uint? UInteger { get; set; }
    public uint[]? UIntegerArray { get; set; }
    public string? String { get; set; }
    public string[]? StringArray { get; set; }
    public char? Char { get; set; }
    public char[]? CharArray { get; set; }

    public void SetValue(object obj)
    {
        switch (Type)
        {
            case SupportedType.Char:
                Char = (char)obj;
                break;
            case SupportedType.CharArray:
                CharArray = Convert<char>(obj);
                break;
            case SupportedType.String:
                String = (string)obj;
                break;
            case SupportedType.StringArray:
                StringArray = Convert<string>(obj);
                break;
            case SupportedType.Boolean:
                Boolean = (bool)obj;
                break;
            case SupportedType.BooleanArray:
                BooleanArray = Convert<bool>(obj);
                break;
            case SupportedType.Int32:
                Integer = (int)obj;
                break;
            case SupportedType.Int32Array:
                IntegerArray = Convert<int>(obj);
                break;
            case SupportedType.UInt32:
                UInteger = (uint)obj;
                break;
            case SupportedType.UInt32Array:
                UIntegerArray = Convert<uint>(obj);
                break;
            case SupportedType.Double:
                Double = (double)obj;
                break;
            case SupportedType.DoubleArray:
                DoubleArray = Convert<double>(obj);
                break;
            case SupportedType.Float:
                Float = (float)obj;
                break;
            case SupportedType.FloatArray:
                FloatArray = Convert<float>(obj);
                break;
            case SupportedType.DateTime:
                DateTime = (DateTime)obj;
                break;
            case SupportedType.DateTimeArray:
                DateTimeArray = Convert<DateTime>(obj);
                break;
        }
    }

    public void SetType(string type)
    {
        Type = GetType(type);
    }
    public static SupportedType GetType(string type)
    {
        switch (type)
        {
            case "Char":
                return SupportedType.Char;
            case "Char[]":
                return SupportedType.CharArray;
            case "String":
                return SupportedType.String;
            case "String[]":
                return SupportedType.StringArray;
            case "Boolean":
                return SupportedType.Boolean;
            case "Boolean[]":
                return SupportedType.BooleanArray;
            case "UInt32":
                return SupportedType.UInt32;
            case "UInt32[]":
                return SupportedType.UInt32Array;
            case "Int32":
                return SupportedType.Int32;
            case "Int32[]":
                return SupportedType.Int32Array;
            case "Single":
                return SupportedType.Float;
            case "Single[]":
                return SupportedType.FloatArray;
            case "Double":
                return SupportedType.Double;
            case "Double[]":
                return SupportedType.DoubleArray;
            case "SwitchParameter":
                return SupportedType.Boolean;
            case "DateTime":
                return SupportedType.DateTime;
            case "DateTime[]":
                return SupportedType.DateTimeArray;
        }

        return SupportedType.None;
    }

    public static SupportedType GetType(Type type)
    {
        if (type.IsEnum) return SupportedType.Enum;
        return GetType(type.Name);
    }

    public void SetValue(JsonElement element)
    {
        switch (Type)
        {
            case SupportedType.Char:
                Char = element.GetString()[0];
                break;
            case SupportedType.CharArray:
                CharArray = GetArray(element, jsonElement => jsonElement.GetString()[0]);
                break;
            case SupportedType.String:
                String = element.GetString();
                break;
            case SupportedType.StringArray:
                StringArray = GetArray(element, jsonElement => jsonElement.GetString());
                break;
            case SupportedType.Boolean:
                Boolean = element.GetBoolean();
                break;
            case SupportedType.BooleanArray:
                BooleanArray = GetArray(element, jsonElement => jsonElement.GetBoolean());
                break;
            case SupportedType.Int32:
                Integer = element.GetInt32();
                break;
            case SupportedType.Int32Array:
                IntegerArray = GetArray(element, jsonElement => jsonElement.GetInt32());
                break;
            case SupportedType.UInt32:
                UInteger = element.GetUInt32();
                break;
            case SupportedType.UInt32Array:
                UIntegerArray = GetArray(element, jsonElement => jsonElement.GetUInt32());
                break;
            case SupportedType.Double:
                Double = element.GetDouble();
                break;
            case SupportedType.DoubleArray:
                DoubleArray = GetArray(element, jsonElement => jsonElement.GetDouble());
                break;
            case SupportedType.Float:
                Float = element.GetSingle();
                break;
            case SupportedType.FloatArray:
                FloatArray = GetArray(element, jsonElement => jsonElement.GetSingle());
                break;
            case SupportedType.DateTime:
                DateTime = element.GetDateTime();
                break;
            case SupportedType.DateTimeArray:
                DateTimeArray = GetArray(element, jsonElement => jsonElement.GetDateTime());
                break;
        }
    }

    public object? GetValue()
    {
        switch (Type)
        {
            case SupportedType.Char:
                return Char;
            case SupportedType.CharArray:
                return CharArray;
            case SupportedType.String:
                return String;
            case SupportedType.StringArray:
                return StringArray;
            case SupportedType.Boolean:
                return Boolean;
            case SupportedType.BooleanArray:
                return BooleanArray;
            case SupportedType.Int32:
                return Integer;
            case SupportedType.Int32Array:
                return IntegerArray;
            case SupportedType.UInt32:
                return UInteger;
            case SupportedType.UInt32Array:
                return UIntegerArray;
            case SupportedType.Double:
                return Double;
            case SupportedType.DoubleArray:
                return DoubleArray;
            case SupportedType.Float:
                return Float;
            case SupportedType.FloatArray:
                return FloatArray;
            case SupportedType.DateTime:
                return DateTime;
            case SupportedType.DateTimeArray:
                return DateTimeArray;
        }

        return null;
    }
    protected JobParameterState GetValidationState(PSParameter scriptParameter)
    {
        switch (Type)
        {
            case SupportedType.Char:
                return scriptParameter.Options.Valid(Char);
            case SupportedType.CharArray:
                return scriptParameter.Options.Valid(CharArray);
            case SupportedType.String:
                return scriptParameter.Options.Valid(String);
            case SupportedType.StringArray:
                return scriptParameter.Options.Valid(StringArray);
            case SupportedType.Boolean:
                return scriptParameter.Options.Valid(Boolean);
            case SupportedType.BooleanArray:
                return scriptParameter.Options.Valid(BooleanArray);
            case SupportedType.Int32:
                return scriptParameter.Options.Valid(Integer);
            case SupportedType.Int32Array:
                return scriptParameter.Options.Valid(IntegerArray);
            case SupportedType.UInt32:
                return scriptParameter.Options.Valid(UInteger);
            case SupportedType.UInt32Array:
                return scriptParameter.Options.Valid(UIntegerArray);
            case SupportedType.Double:
                return scriptParameter.Options.Valid(Double);
            case SupportedType.DoubleArray:
                return scriptParameter.Options.Valid(DoubleArray);
            case SupportedType.Float:
                return scriptParameter.Options.Valid(Float);
            case SupportedType.FloatArray:
                return scriptParameter.Options.Valid(FloatArray);
            case SupportedType.DateTime:
                return scriptParameter.Options.Valid(DateTime);
            case SupportedType.DateTimeArray:
                return scriptParameter.Options.Valid(DateTimeArray);
        }

        return JobParameterState.NotValidated;
    }

    private T[]? GetArray<T>(JsonElement element, Func<JsonElement, T> getValue)
    {
        var list = new List<T>();
        foreach (var e in element.EnumerateArray())
        {
            list.Add(getValue(e));
        }

        return list.ToArray();
    }

    private T[] Convert<T>(object obj)
    {
        var list = new List<T>();
        if (obj.GetType().Name != "Object[]")
        {
            list.Add((T)obj);
        }
        else
        {
            var objArray = (object[])obj;
            foreach (var o in objArray)
            {
                list.Add((T)o);
            }

        }

        return list.ToArray();
    }
}