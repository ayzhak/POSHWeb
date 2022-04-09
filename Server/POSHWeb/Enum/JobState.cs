namespace POSHWeb.Enum;

/// <summary>
/// This are the States of a execution job
/// </summary>
public enum JobState{
    Created,
    Running,
    Finished,
    Failed,
    Validating,
    Invalid
}