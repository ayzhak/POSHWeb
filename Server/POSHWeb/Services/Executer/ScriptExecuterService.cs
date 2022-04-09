using System.Collections;
using System.Management.Automation;
using POSHWeb.Data;
using POSHWeb.Model;
using System.Management.Automation.Language;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using POSHWeb.Enum;
using POSHWeb.Exceptions;
using POSHWeb.ScriptRunner;
using SignalRChat.Hubs;
using static Crayon.Output;
using Job = POSHWeb.Model.Job;
using JobState = POSHWeb.Enum.JobState;

namespace POSHWeb.Services
{
    public class ScriptExecuter
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HostedRunspace _hostedRunspace;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IHubContext<ScriptHub, IScriptHub> _scriptHub;

        public ScriptExecuter(IServiceScopeFactory scopeFactory, HostedRunspace hostedRunspace,
            IBackgroundTaskQueue taskQueue, IHubContext<ScriptHub, IScriptHub> scriptHub)
        {
            _scopeFactory = scopeFactory;
            _hostedRunspace = hostedRunspace;
            _taskQueue = taskQueue;
            _scriptHub = scriptHub;
        }

        public async Task QueueScriptExecution(Model.Job job, PSScript script)
        {
            await _taskQueue.QueueBackgroundWorkItemAsync(RunJobFactory(job, script));
        }

        public Func<CancellationToken, ValueTask> RunJobFactory(Model.Job job, PSScript script)
        {
            return async (CancellationToken token) =>
            {
                var scriptParameters = new Dictionary<string, object>();

                SetJobState(job.Id, JobState.Validating);
                job.Parameters = InputParameterParser.Parse(job.Parameters);
                UpdateJob(job);
                SetJobState(job.Id, JobState.Validating);
                if (!InputParameterValidator.HasAllMandatories(job.Parameters, script.Parameters))
                {
                    SetJobState(job.Id, JobState.Invalid);
                    return;
                }
                job.Parameters = InputParameterValidator.SetParametersState(job.Parameters, script.Parameters);
                UpdateJob(job);
                if(job.Parameters.Any(parameter => parameter.State != JobParameterState.Valid))
                {
                    SetJobState(job.Id, JobState.Invalid);
                    return;
                }
                foreach (var inputParameter in job.Parameters)
                {
                    inputParameter.AddToDictonary(scriptParameters);
                }
                SetJobState(job.Id, JobState.Running);
                WriteToLog(job.Id, Green("EXECUTION STARTED"));
                try
                {
                    var hasFailed = await _hostedRunspace.SimpleRunScript(job.Content,
                        scriptParameters,
                        Information_DataAdded(job.Id),
                        Warning_DataAdded(job.Id),
                        Error_DataAdded(job.Id),
                        Verbose_DataAdded(job.Id),
                        Verbose_DataAdded(job.Id),
                        Progress_DataAdded(job.Id));
                }
                catch (ParameterBindingException e)
                {
                    SetJobState(job.Id, JobState.Failed);
                    WriteToLog(job.Id, Red(e.Message));
                    return;
                }

                WriteToLog(job.Id, Green("EXECUTION FINISHED"));
                SetJobState(job.Id, JobState.Finished);
            };
        }

        private void UpdateJob(Job job)
        {
            using var scope = _scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            _dbContext.Jobs.Update(job);
            _dbContext.SaveChanges();
        }

        private EventHandler<DataAddedEventArgs> Information_DataAdded(int id)
        {
            return delegate(object? sender, DataAddedEventArgs e)
            {
                var streamObjectsReceived = sender as PSDataCollection<InformationRecord>;
                var currentStreamRecord = streamObjectsReceived[e.Index];
                WriteToLog(id, currentStreamRecord.MessageData.ToString());
            };
        }

        private EventHandler<DataAddedEventArgs> Warning_DataAdded(int id)
        {
            return delegate(object? sender, DataAddedEventArgs e)
            {
                var streamObjectsReceived = sender as PSDataCollection<WarningRecord>;
                var currentStreamRecord = streamObjectsReceived[e.Index];
                WriteToLog(id, Yellow(currentStreamRecord.Message));
            };
        }

        private EventHandler<DataAddedEventArgs> Error_DataAdded(int id)
        {
            return delegate(object? sender, DataAddedEventArgs e)
            {
                var streamObjectsReceived = sender as PSDataCollection<ErrorRecord>;
                var currentStreamRecord = streamObjectsReceived[e.Index];
                WriteToLog(id, Red(currentStreamRecord.Exception.ToString()));
            };
        }

        private EventHandler<DataAddedEventArgs> Debug_DataAdded(int id)
        {
            return delegate(object? sender, DataAddedEventArgs e)
            {
                var streamObjectsReceived = sender as PSDataCollection<DebugRecord>;
                var currentStreamRecord = streamObjectsReceived[e.Index];
                WriteToLog(id, Cyan(currentStreamRecord.Message));
            };
        }

        private EventHandler<DataAddedEventArgs> Verbose_DataAdded(int id)
        {
            return delegate(object? sender, DataAddedEventArgs e)
            {
                var streamObjectsReceived = sender as PSDataCollection<VerboseRecord>;
                var currentStreamRecord = streamObjectsReceived[e.Index];
                WriteToLog(id, Magenta(currentStreamRecord.Message));
            };
        }

        private EventHandler<DataAddedEventArgs> Progress_DataAdded(int id)
        {
            return delegate(object? sender, DataAddedEventArgs e)
            {
                var streamObjectsReceived = sender as PSDataCollection<ProgressRecord>;
                var currentStreamRecord = streamObjectsReceived[e.Index];
                string progress = ProgressBarAsString(currentStreamRecord);
                WriteToLog(id, Cyan(Bold(progress)));
            };
        }

        private string ProgressBarAsString(ProgressRecord progress)
        {
            string leftBars = new String('=', progress.PercentComplete / 10);
            string rightBars = new String(' ', 10 - (progress.PercentComplete / 10));
            TimeSpan time = TimeSpan.FromSeconds(progress.SecondsRemaining);
            return
                $"{progress.Activity} : {progress.CurrentOperation} : {progress.StatusDescription} [{leftBars}{rightBars}] {progress.PercentComplete}/100  {time.ToString(@"hh\:mm\:ss\:fff")} remaining";
        }


        private void WriteToLog(int id, string log)
        {
            using var scope = _scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var job = _dbContext.Jobs.Find(id);
            string logMessage = $"[{DateTime.Now.ToString()}] {log}";
            job.Log = $"{job.Log}\n{logMessage}";
            _dbContext.Jobs.Update(job);
            _dbContext.SaveChanges();
            _scriptHub.Clients.All.ReceiveJobUpdate(job);
        }

        private void SetJobState(int id, JobState jobState)
        {
            using var scope = _scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var job = _dbContext.Jobs.Find(id);
            job.State = jobState;
            _dbContext.Jobs.Update(job);
            _dbContext.SaveChanges();
            _scriptHub.Clients.All.ReceiveJobStateChanged(id, jobState.ToString());
        }
    }

}