using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Runspace;
using Quartz;

namespace POSHWeb.Scheduler.Job;

[DisallowConcurrentExecution]
public class LocalScriptJob : IJob
{
    public IExecuter Executer { get; set; }
    public Model.Job.Job Job { get; set; }

    public Task Execute(IJobExecutionContext context)
    {
        Executer.RunspaceManager.Start();
        Executer.Run(Job.Script.Content, Job.GetParameters());
        Executer.RunspaceManager.Stop();
        return Task.CompletedTask;
    }
}