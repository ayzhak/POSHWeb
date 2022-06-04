using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Model;
using POSHWeb.Environment.Remote.Protos;
using POSHWeb.Environment.Util;
using POSHWeb.Model.Job.Logs;

namespace POSHWeb.Environment.Remote.Client
{
    public class Client : IPSInteraction
    {
        private readonly GrpcChannel channel;
        private readonly PSEnvironment.PSEnvironmentClient client;

        public Client(GrpcChannel channel)
        {
            this.channel = channel;
            this.client = new PSEnvironment.PSEnvironmentClient(this.channel);
        }

        public void Log(SeverityLevel level, string message)
        {
            Dictionary<SeverityLevel, Severity> map = new Dictionary<SeverityLevel, Severity>
            {
                {SeverityLevel.Information, Severity.Information},
                {SeverityLevel.Debug, Severity.Debug},
                {SeverityLevel.Warning, Severity.Warning},
                {SeverityLevel.Error, Severity.Error},
                {SeverityLevel.Verbose, Severity.Verbose},
                {SeverityLevel.Progress, Severity.Progress},
            };

            client.Log(new LogRequest
            {
                Message = message,
                Severity = map[level],
                Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
            });
        }

        public void Progress(SeverityLevel level, LogProgress record)
        {
            var progressEntry = new ProgressEntry
            {
                StatusDescription = record.StatusDescription,
                Activity = record.Activity,
                CurrentOperation = record.CurrentOperation,
                PercentComplete = record.PercentComplete,
                SecondsRemaining = record.SecondsRemaining,
            };

            client.Log(new LogRequest
            {
                Progress = progressEntry,
                Message = record.ToString(),
                Severity = Severity.Progress,
                Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
            });
        }

        public string Input()
        {
            var inputRequest = new InputPromptRequest();
            var response = client.Prompt(new PromptRequest
            {
                PromptInput = inputRequest
            });
            return response.PromptInput.Value;
        }

        public Dictionary<string, PSObject> Prompt(string caption, string message,
            Collection<FieldDescription> descriptions)
        {
            var inputs = new InputsPromptRequest();
            foreach (var description in descriptions)
            {
                inputs.Inputs.Add(new InputPromptRequest
                {
                    Name = description.Name,
                    Label = description.Label,
                    HelpMessage = description.HelpMessage,
                    DefaultValue = PSObjectJsonConverter.ToJson(description.DefaultValue),
                    Mandatory = description.IsMandatory,
                    TypeName = description.ParameterTypeName,
                    TypeFullName = description.ParameterTypeFullName,
                    AssemblyFullName = description.ParameterAssemblyFullName
                });
            }

            var response = client.Prompt(new PromptRequest
            {
                Caption = caption,
                Message = message,
                PromptInputs = inputs,
            });
            var result = new Dictionary<string, PSObject>();
            foreach (var value in response.PromptInputs.Values)
            {
                result.Add(value.Name, new PSObject(PSObjectJsonConverter.FromJson(value.Value)));
            }

            return result;
        }

        public int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices,
            int defaultChoice)
        {
            var choicesRequest = new ChoicePromptRequest
            {
                DefaultValue = defaultChoice,
            };

            foreach (var choice in choices)
            {
                choicesRequest.Choices.Add(new ChoicePrompt
                {
                    Label = choice.Label,
                    HelpMessage = choice.HelpMessage
                });
            }

            var response = client.Prompt(new PromptRequest
            {
                Caption = caption,
                Message = message,
                PromptChoice = choicesRequest
            });
            return response.Choice.Choice;
        }

        public PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            var credentialsRequest = new CredentialsPromptRequest
            {
                TargetName = targetName,
                Username = userName
            };
            var response = client.Prompt(new PromptRequest
            {
                PromptCredentials = credentialsRequest,
                Caption = caption,
                Message = message
            });
            return new PSCredential(response.Credentials.Username,
                SecureStringUtil.ConvertTo(response.Credentials.Password));
        }

        public void StateChange(PSInvocationState state, Exception reason)
        {
            Dictionary<PSInvocationState, InvocationState> map = new Dictionary<PSInvocationState, InvocationState>
            {
                {PSInvocationState.NotStarted, InvocationState.NotStarted},
                {PSInvocationState.Stopping, InvocationState.Stopping},
                {PSInvocationState.Stopped, InvocationState.Stopped},
                {PSInvocationState.Running, InvocationState.Running},
                {PSInvocationState.Disconnected, InvocationState.Disconnected},
                {PSInvocationState.Completed, InvocationState.Completed},
                {PSInvocationState.Failed, InvocationState.Failed},
            };

            client.InvocationStateChange(new InvocationStateChangeRequest
            {
                State = map[state],
                Reason = ConvertException(reason)
            });
        }

        public void Exception(Exception ex)
        {
            client.Log(new LogRequest
            {
                Severity = Severity.Error,
                Exception = ConvertException(ex)
            });
        }

        private ExceptionEntry? ConvertException(Exception? ex)
        {
            if (ex == null) return null;
            return new ExceptionEntry
            {
                FullName = ex.GetType().FullName,
                Name = ex.GetType().Name,
                HelpLink = ex.HelpLink,
                HResult = ex.HResult,
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                InnerException = ConvertException(ex.InnerException)
            };
        }
    }
}