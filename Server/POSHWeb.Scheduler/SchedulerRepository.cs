using System.ComponentModel.DataAnnotations;
using POSHWeb.Environment;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Model;
using POSHWeb.Environment.Runspace;
using POSHWeb.Scheduler.Converter;
using POSHWeb.Scheduler.Job;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace POSHWeb.Scheduler.Repository;

public class SchedulerRepository
{
    private readonly IScheduler scheduler;

    public SchedulerRepository(ISchedulerFactory _factory)
    {
        scheduler = _factory.GetScheduler().Result;
    }
    
    public void Add(Model.Job.Job job, IExecuter executer)
    {
        var jobToRun = JobBuilder.Create<LocalScriptJob>()
            .WithIdentity(job.Id.ToString(), "Local")
            .Build();
        jobToRun.JobDataMap.Put(nameof(LocalScriptJob.Executer), executer);
        jobToRun.JobDataMap.Put(nameof(LocalScriptJob.Job), job);
        foreach (var trigger in job.Triggers)
        {
            var t = TriggerConverter.Convert(trigger);
            scheduler.ScheduleJob(jobToRun,t);
        }
    }
}