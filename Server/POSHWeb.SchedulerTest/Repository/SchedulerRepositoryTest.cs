using System.Collections.Specialized;
using Moq;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Model;
using POSHWeb.Environment.Runspace;
using POSHWeb.Model;
using POSHWeb.Model.Job;
using POSHWeb.Scheduler.Trigger;
using Quartz;
using Quartz.Impl;
using Xunit;
using Xunit.Abstractions;
using SchedulerRepository = POSHWeb.Scheduler.Repository.SchedulerRepository;

namespace POSHWeb.SchedulerTest.Repository;

public class SchedulerRepositoryTest
{
    private readonly ITestOutputHelper o;

    public SchedulerRepositoryTest(ITestOutputHelper output)
    {
        o = output;
    }
    [Fact]
    public async void SchedulerPositiveTest()
    {
        NameValueCollection properties = new NameValueCollection();
        ISchedulerFactory sf = new StdSchedulerFactory(properties);
        IScheduler sched = await sf.GetScheduler();

        var repo = new SchedulerRepository(sf);
        await sched.Start();
        var job = new Job
        {
            Id = 1,
            RunspaceSettings = new RunspaceSettings(),
            Script = new PSScript
            {
                Content = "Write-Host 'Hello'"
            },
            Triggers = new List<ScheduleTrigger>
            {
                new() {Mode = SchedulerOperationMode.Now}
            }
        };
        var mock = new Mock<IPSInteraction>();
        string? result = null;
        mock.Setup(interaction => interaction.Log(It.IsAny<SeverityLevel>(), It.Is<string>(r => r == "Hello")))
            .Callback((SeverityLevel level, string? msg) => o.WriteLine(msg))
            .Verifiable();
        var executer = new Executer(job.RunspaceSettings, mock.Object);
        var key = new JobKey("1", "Local");
        repo.Add(job, executer);
        Assert.True(await sched.CheckExists(key));
        Assert.Single(sched.GetTriggersOfJob(key).Result);
        await Task.Delay(5000);
        mock.Verify();
    }
}