using System.Management.Automation;
using POSHWeb.Data;
using POSHWeb.Model;
using System.Management.Automation.Language;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using POSHWeb.Exceptions;
using POSHWeb.ScriptRunner;
using SignalRChat.Hubs;
using static Crayon.Output;
using JobState = POSHWeb.Enum.JobState;

namespace POSHWeb.Services
{
    public class InputParameterParser
    {
        public static ICollection<InputParameter> Parse(ICollection<InputParameter> parameters)
        {
            var list = new List<InputParameter>();
            foreach (var parameter in parameters)
            {
                list.Add(Parse(parameter));
            }
            return list;
        }
         
        public static InputParameter Parse(InputParameter parameter)
        {
            if (parameter.Type == null || parameter.Type == "") throw new ParameterParseException($"The parameter '{parameter.Name}' don't has a matching type.");
            var typeResolvers = new Dictionary<string, Type>();
            var allTypes = FindDerivedTypes(typeof(InputParameter).Assembly, typeof(InputParameter));

            foreach (var type in allTypes)
            {
                var obj = (InputParameter) Activator.CreateInstance(type);
                typeResolvers.Add(obj.Type,  type);
            }

            var t = typeResolvers[parameter.Type];
            var p  = (InputParameter) Activator.CreateInstance(t)!;

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
}