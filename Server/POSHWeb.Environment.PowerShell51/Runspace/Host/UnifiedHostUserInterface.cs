using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Runspace.Host;
using POSHWeb.Environment.Util;

namespace POSHWeb.Environment.PowerShell51.Runspace.Host
{
    public class UnifiedHostUserInterface : PSHostUserInterface
    {
        private readonly IPSInteraction _interaction;

        public UnifiedHostUserInterface(IPSInteraction interaction, PSHostRawUserInterface rawUi)
        {
            _interaction = interaction;
            RawUI = rawUi;
        }

        public override PSHostRawUserInterface RawUI { get; }

        public override string ReadLine() => _interaction.Input();

        public override SecureString ReadLineAsSecureString() =>
            SecureStringUtil.ConvertTo(_interaction.Input());

        public override void Write(string value) =>
            _interaction.Log(SeverityLevel.Information, value);

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value) =>
            _interaction.Log(SeverityLevel.Information, value);

        public override void WriteLine(string value) =>
            _interaction.Log(SeverityLevel.Information, value);

        public override void WriteErrorLine(string value) =>
            _interaction.Log(SeverityLevel.Error, value);

        public override void WriteDebugLine(string message) =>
            _interaction.Log(SeverityLevel.Debug, message);

        public override void WriteProgress(long sourceId, ProgressRecord record) =>
            _interaction.Progress(SeverityLevel.Progress, LogProgressConverter.ConvertLogEntry(record));

        public override void WriteVerboseLine(string message) =>
            _interaction.Log(SeverityLevel.Verbose, message);

        public override void WriteWarningLine(string message) =>
            _interaction.Log(SeverityLevel.Warning, message);

        public override Dictionary<string, PSObject> Prompt(string caption, string message,
            Collection<FieldDescription> descriptions) =>
            _interaction.Prompt(caption, message, descriptions);

        public override PSCredential PromptForCredential(string caption, string message, string userName,
            string targetName) =>
            _interaction.PromptForCredential(caption, message, userName, targetName);

        public override PSCredential PromptForCredential(string caption, string message, string userName,
            string targetName,
            PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options) =>
            _interaction.PromptForCredential(caption, message, userName, targetName);

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices,
            int defaultChoice) =>
            _interaction.PromptForChoice(caption, message, choices, defaultChoice);
    }
}