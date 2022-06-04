using Microsoft.PowerShell.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace POSHWeb.Environment.Util
{
    public static class PSObjectJsonConverter
    {
        public static string ToJson(ICollection<PSObject> psObject, int depth = 2, bool enumAsString = true, bool jsonCompress = true)
        {
            throw new NotImplementedException();
        }

        public static string ToJson(PSObject psObject, int depth = 2, bool enumAsString = true, bool jsonCompress = true)
        {
            if (psObject.TypeNames.FirstOrDefault() == "System.String") 
                return psObject.BaseObject.ToString() ?? throw new InvalidOperationException();

            var settings = new JsonSerializerSettings
                { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

            var dict = new Dictionary<string, object>();
            var objMembers = typeof(object).GetMembers();
            var ignoreMembers = new List<string>();

            ignoreMembers.AddRange(
                psObject.Members.Where(
                    i => i.TypeNameOfValue.StartsWith("Deserialized.")).Select(i => i.Name).ToList());

            ignoreMembers.AddRange(objMembers.Select(i => i.Name));

            var filteredMembers =
                psObject.Members.Where(
                    i => ignoreMembers.All(
                        ig => ig.ToLower() != i.Name.ToLower())).ToList();

            foreach (var mem in filteredMembers)
            {
                if (!dict.ContainsKey(mem.Name))
                {
                    dict.Add(mem.Name, "");
                }
                dict[mem.Name] = mem.Value;
            }

            try
            {
                return JsonConvert.SerializeObject(dict, settings);
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public static object FromJson(string json)
        {
            throw new NotImplementedException();
        }
    }
}
