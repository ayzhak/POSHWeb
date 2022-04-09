using POSHWeb.Model;

namespace POSHWeb.Services;

public static class InputParameterValidator
{
    public static ICollection<InputParameter> SetParametersState(ICollection<InputParameter> parameters,
        ICollection<PSParameter> psParameters)
    {
        var mergeParameters = FactoryMergeParameter(parameters, psParameters);
        foreach (var mp in mergeParameters)
            mp.InputParameter.State = mp.InputParameter.ValidateValue(mp.PSParameter.Options);
        return parameters;
    }

    public static bool HasAllMandatories(ICollection<InputParameter> parameters, ICollection<PSParameter> psParameters)
    {
        var mergeParameters = FactoryMergeParameter(parameters, psParameters);
        return mergeParameters.Any(ValidateMandatory);
    }

    private static bool ValidateMandatory(MergeParameter mergeParameter)
    {
        return mergeParameter.PSParameter.Mandatory && mergeParameter.InputParameter != null;
    }

    private static ICollection<MergeParameter> FactoryMergeParameter(ICollection<InputParameter> parameters,
        ICollection<PSParameter> psParameters)
    {
        var list = new List<MergeParameter>();
        foreach (var psParameter in psParameters)
        {
            var mergeParameter = new MergeParameter();
            mergeParameter.PSParameter = psParameter;
            mergeParameter.InputParameter = parameters.First(parameter => parameter.Name == psParameter.Name);
            list.Add(mergeParameter);
        }

        return list;
    }

    internal class MergeParameter
    {
        public PSParameter PSParameter { get; set; }
        public InputParameter? InputParameter { get; set; }
    }
}