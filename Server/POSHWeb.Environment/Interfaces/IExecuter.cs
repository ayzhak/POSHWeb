using System.Collections.Generic;
using System.Management.Automation;

namespace POSHWeb.Environment.Interfaces;

public interface IExecuter
{
   IRunspaceManager RunspaceManager { get; }
   ICollection<PSObject> Run(string script, Dictionary<string, object> parameters = null);
}