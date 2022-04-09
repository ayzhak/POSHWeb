using System.Text.RegularExpressions;
using POSHWeb.Enum;
namespace POSHWeb.Model.Script;

public partial class PSParameterOptions
{
    public int Id { get; set; }
    public Int32? MinLength { get; set; }
    public Int32? MaxLength { get; set; }
    public string? RegexString { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public int? MinCount { get; set; }
    public int? MaxCount { get; set; }
    public string[]? ValidValues { get; set; }
    public string? ScriptBlock { get; set; }

    public JobParameterState Valid(string text)
    {
        if (! ValidateLength(text.Length)) return JobParameterState.InvalidLength;
        if (! ValidateRegex(text)) return JobParameterState.NotMatchingWithRegex;
        if (! ValidateValidValues(text)) return JobParameterState.ValueNotInPredefinedSet;
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(int number)
    {
        if(!ValidateNumber(number)) return JobParameterState.NumberNotInRange;
        if(!ValidateValidValues(number.ToString())) return JobParameterState.ValueNotInPredefinedSet;
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(float number)
    {
        if (!ValidateNumber(number)) return JobParameterState.NumberNotInRange;
        if (!ValidateValidValues(number.ToString())) return JobParameterState.ValueNotInPredefinedSet;
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(double number)
    {
        if (!ValidateNumber(number)) return JobParameterState.NumberNotInRange;
        if (!ValidateValidValues(number.ToString())) return JobParameterState.ValueNotInPredefinedSet;
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(bool number)
    {
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(DateTime number)
    {
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(string[] array)
    {
        if (!ValidateCount(array.Count())) return JobParameterState.ItemCountNotInRange;
        foreach (var s in array)
        {
            var state = Valid(s);
            if(state != JobParameterState.Valid ) return state;
        }
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(int[] array)
    {
        if (!ValidateCount(array.Count())) return JobParameterState.ItemCountNotInRange;
        foreach (var s in array)
        {
            var state = Valid(s);
            if (state != JobParameterState.Valid) return state;
        }
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(double[] array)
    {
        if (!ValidateCount(array.Count())) return JobParameterState.ItemCountNotInRange;
        foreach (var s in array)
        {
            var state = Valid(s);
            if (state != JobParameterState.Valid) return state;
        }
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(float[] array)
    {
        if (!ValidateCount(array.Count())) return JobParameterState.ItemCountNotInRange;
        foreach (var s in array)
        {
            var state = Valid(s);
            if (state != JobParameterState.Valid) return state;
        }
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(bool[] array)
    {
        if (!ValidateCount(array.Count())) return JobParameterState.ItemCountNotInRange;
        return JobParameterState.Valid;
    }

    public JobParameterState Valid(DateTime[] array)
    {
        if (!ValidateCount(array.Count())) return JobParameterState.ItemCountNotInRange;
        return JobParameterState.Valid;
    }

    private bool ValidateValidValues(string text)
    {
        if (ValidValues == null) return true;
        return ValidValues.Contains(text);
    }

    private bool ValidateLength(int length)
    {
        if (MinLength == null && MaxLength == null) return true;
        return MinLength <= length && length <= MaxLength;
    }


    private bool ValidateCount(int count)
    {
        if (MinCount == null && MaxCount == null) return true;
        return MinCount <= count && count <= MaxCount;
    }

    private bool ValidateNumber(double count)
    {
        if (MinValue == null && MaxValue == null) return true;
        return MinValue <= count && count <= MaxValue;
    }

    private bool ValidateRegex(string text)
    {
        if (RegexString == null) return true;
        return Regex.IsMatch(text, RegexString);
    }
}