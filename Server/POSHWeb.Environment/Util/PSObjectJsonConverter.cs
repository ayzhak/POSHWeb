using Microsoft.PowerShell.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace POSHWeb.Environment.Util
{
    public static class PSObjectJsonConverter
    {
        public static string ToJson(ICollection<PSObject> psObject, int depth = 2, bool enumAsString = true, bool jsonCompress = true)
        {
            object objectToProcess = (psObject.Count > 1) ? (psObject.ToArray() as object) : psObject.FirstOrDefault();
            var context = new JsonObject.ConvertToJsonContext(depth, enumAsString, jsonCompress);
            return JsonObject.ConvertToJson(objectToProcess, in context);
        }

        public static string ToJson(PSObject psObject, int depth = 2, bool enumAsString = true, bool jsonCompress = true)
        {
            var context = new JsonObject.ConvertToJsonContext(depth, enumAsString, jsonCompress);
            return JsonObject.ConvertToJson(psObject, in context);
        }

        public static object FromJson(string json)
        {
            ErrorRecord error = null;
            return JsonObject.ConvertFromJson(json, out error);
        }
    }
}
