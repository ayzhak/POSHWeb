namespace POSHWeb.Enum;

/// <summary>
/// This are the states of a parameter in a execution job
/// </summary>
public enum JobParameterState
{
    NotValidated,
    Valid,
    InvalidLength,
    NotMatchingWithRegex,
    NumberNotInRange,
    ItemCountNotInRange,
    ValueNotInPredefinedSet
}