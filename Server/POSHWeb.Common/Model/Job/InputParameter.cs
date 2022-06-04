using POSHWeb.Enum;

namespace POSHWeb.Model.Job;
public class InputParameter : BasicType
{
    public InputParameter()
    {
        State = JobParameterState.NotValidated;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public JobParameterState State { get; set; }
    public void Validate(PSParameter scriptParameter)
    {
        State = GetValidationState(scriptParameter);
    }
}