using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace POSHWeb.ScriptRunner
{
    /// <summary>
    /// Contains functionality for executing PowerShell scripts.
    /// </summary>
    public class HostedRunspace
    {
        /// <summary>
        /// The PowerShell runspace pool.
        /// </summary>
        private RunspacePool RsPool { get; set; }

        /// <summary>
        /// Initialize the runspace pool.
        /// </summary>
        /// <param name="minRunspaces"></param>
        /// <param name="maxRunspaces"></param>
        public void InitializeRunspaces(int minRunspaces, int maxRunspaces, string[] modulesToLoad)
        {
            // create the default session state.
            // session state can be used to set things like execution policy, language constraints, etc.
            // optionally load any modules (by name) that were supplied.

            var defaultSessionState = InitialSessionState.CreateDefault();
            defaultSessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;

            foreach (var moduleName in modulesToLoad)
            {
                defaultSessionState.ImportPSModule(moduleName);
            }

            // use the runspace factory to create a pool of runspaces
            // with a minimum and maximum number of runspaces to maintain.

            RsPool = RunspaceFactory.CreateRunspacePool(defaultSessionState);
            RsPool.SetMinRunspaces(minRunspaces);
            RsPool.SetMaxRunspaces(maxRunspaces);

            // set the pool options for thread use.
            // we can throw away or re-use the threads depending on the usage scenario.

            RsPool.ThreadOptions = PSThreadOptions.UseNewThread;

            // open the pool. 
            // this will start by initializing the minimum number of runspaces.

            RsPool.Open();
        }

        /// <summary>
        /// Runs a PowerShell script with parameters and prints the resulting pipeline objects to the console output. 
        /// </summary>
        /// <param name="scriptContents">The script file contents.</param>
        /// <param name="scriptParameters">A dictionary of parameter names and parameter values.</param>
        public async Task RunScript(
            string scriptContents,
            Dictionary<string, object> scriptParameters,
            EventHandler<DataAddedEventArgs> information,
            EventHandler<DataAddedEventArgs> warning,
            EventHandler<DataAddedEventArgs> error)
        {
            if (RsPool == null)
            {
                throw new ApplicationException("Runspace Pool must be initialized before calling RunScript().");
            }

            // create a new hosted PowerShell instance using a custom runspace.
            // wrap in a using statement to ensure resources are cleaned up.

            using (PowerShell ps = PowerShell.Create())
            {
                // use the runspace pool.
                ps.RunspacePool = RsPool;

                // specify the script code to run.
                ps.AddScript(scriptContents);

                // specify the parameters to pass into the script.
                ps.AddParameters(scriptParameters);

                // subscribe to events from some of the streams
                ps.Streams.Error.DataAdded += error;
                ps.Streams.Warning.DataAdded += warning;
                ps.Streams.Information.DataAdded += information;

                // execute the script and await the result.
                var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);

                // print the resulting pipeline objects to the console.
                Console.WriteLine("----- Pipeline Output below this point -----");
                foreach (var item in pipelineObjects)
                {
                    Console.WriteLine(item.BaseObject.ToString());
                }
            }
        }

        /// <summary>
        /// Runs a PowerShell script with parameters and prints the resulting pipeline objects to the console output. 
        /// </summary>
        /// <param name="scriptContents">The script file contents.</param>
        /// <param name="scriptParameters">A dictionary of parameter names and parameter values.</param>
        public async Task<bool> SimpleRunScript(
            string scriptContents,
            Dictionary<string, object> parameters,
            EventHandler<DataAddedEventArgs> information,
            EventHandler<DataAddedEventArgs> warning,
            EventHandler<DataAddedEventArgs> error,
            EventHandler<DataAddedEventArgs> debug,
            EventHandler<DataAddedEventArgs> verbose,
            EventHandler<DataAddedEventArgs> progress
        )
        {
            // create a new hosted PowerShell instance using the default runspace.
            // wrap in a using statement to ensure resources are cleaned up.

            using PowerShell ps = PowerShell.Create();

            ps.AddScript(scriptContents);
            ps.AddParameters(parameters);
            ps.Streams.Error.DataAdded += error;
            ps.Streams.Warning.DataAdded += warning;
            ps.Streams.Information.DataAdded += information;
            ps.Streams.Debug.DataAdded += debug;
            ps.Streams.Verbose.DataAdded += verbose;
            ps.Streams.Progress.DataAdded += progress;

            var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);

            // print the resulting pipeline objects to the console.
            foreach (var item in pipelineObjects)
            {
                Console.WriteLine(item.BaseObject.ToString());
            }

            return ps.HadErrors;
        }
    }
}