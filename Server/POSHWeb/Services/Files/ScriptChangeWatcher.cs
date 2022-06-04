using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using POSHWeb.Exceptions;
using POSHWeb.Options;
using SignalRChat.Hubs;

namespace POSHWeb.Services
{
    public class ScriptChangeWatcher : IHostedService, IDisposable
    {
        private readonly ILogger<ScriptChangeWatcher> _logger;
        private readonly POSHWebOptions _options;
        private readonly ScriptFSSyncService _psFileService;
        private readonly IHubContext<ScriptHub, IScriptHub> _scriptHub;
        private readonly DirectoryInfo directory;
        private readonly FileSystemWatcher watcher;

        public ScriptChangeWatcher(
            ScriptFSSyncService psFileService,
            IOptions<POSHWebOptions> options,
            IHubContext<ScriptHub, IScriptHub> scriptHub,
            ILogger<ScriptChangeWatcher> logger)
        {
            _psFileService = psFileService;
            _scriptHub = scriptHub;
            _logger = logger;
            _options = options.Value;
            if (Path.IsPathRooted(_options.ScriptFolder))
            {
                directory = new DirectoryInfo(_options.ScriptFolder);
            }
            else
            {
                directory = new DirectoryInfo(Path.GetFullPath(_options.ScriptFolder));
            }

            watcher = new FileSystemWatcher(directory.FullName);
        }

        public void Dispose()
        {
            watcher.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start wachting on direcotry {directory.FullName}");
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            watcher.Changed += async (object sender, FileSystemEventArgs e) =>
            {
                if (e.ChangeType != WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created) return;
                var script = _psFileService.Modified(e.FullPath);
                await _scriptHub.Clients.All.ReceiveScriptChanged(script);
            };
            watcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                var script = _psFileService.Create(e.FullPath);
                await _scriptHub.Clients.All.ReceiveScriptCreated(script);
            };
            watcher.Deleted += async (object sender, FileSystemEventArgs e) =>
            {
                try
                {
                    var script = _psFileService.Remove(e.FullPath);
                    await _scriptHub.Clients.All.ReceiveScriptRemoved(script.Id);
                }
                catch (FileStillExistException exception)
                {
                    _logger.LogError(exception.Message);
                }
            };
            watcher.Renamed += async (object sender, RenamedEventArgs e) =>
            {
                try
                {
                    var script = _psFileService.Rename(e.OldFullPath, e.FullPath);
                    await _scriptHub.Clients.All.ReceiveScriptChanged(script);
                }
                catch (FileStillExistException exception)
                {
                    _logger.LogError(exception.Message);
                }
            };


            _psFileService.Sync(directory.FullName);

            watcher.Filter = "*.ps1";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            watcher.EnableRaisingEvents = false;
            return Task.CompletedTask;
        }
    }
}