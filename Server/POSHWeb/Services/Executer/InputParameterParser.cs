using System.Reflection;
using POSHWeb.Exceptions;
using POSHWeb.Model;

namespace POSHWeb.Services;

public class InputParameterParser
{
    public static ICollection<InputParameter> Parse(ICollection<InputParameter> parameters)
    {
        var list = new List<InputParameter>();
        foreach (var parameter in parameters) list.Add(Parse(parameter));
        return list;
    }

    public static InputParameter Parse(InputParameter parameter)
    {
        if (parameter.Type == null || parameter.Type == "")
            throw new ParameterParseException($"The parameter '{parameter.Name}' don't has a matching type.");
        var typeResolvers = new Dictionary<string, Type>();
        var allTypes = FindDerivedTypes(typeof(InputParameter).Assembly, typeof(InputParameter));

        foreach (var type in allTypes)
        {
            var obj = (InputParameter) Activator.CreateInstance(type);
            typeResolvers.Add(obj.Type, type);
        }

        var t = typeResolvers[parameter.Type];
        var p = (InputParameter) Activator.CreateInstance(t)!;

        p.Id = parameter.Id;
        p.Name = parameter.Name;
        p.Value = parameter.Value;
        parameter.State = parameter.State;
        p.ParseValue(parameter.Value);
        return p;
    }

    private static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
    {
        return assembly.GetTypes().Where(t => t != baseType &&
                                              baseType.IsAssignableFrom(t));
    }
}