using Microsoft.AspNetCore.SignalR;
using POSHWeb.Enum;
using POSHWeb.Model;
using POSHWeb.Model.Job;

namespace SignalRChat.Hubs;

public class ScriptHub : Hub<IScriptHub>
{
    public async Task SendScriptUpdate(PSScript script)
    {
        await Clients.All.ReceiveScriptCreated(script);
    }

    public async Task SendScriptMutation(PSScript script)
    {
        await Clients.All.ReceiveScriptChanged(script);
    }

    public async Task SendScriptRemoved(int id)
    {
        await Clients.All.ReceiveScriptRemoved(id);
    }

    public async Task SendJobUpdate(Job job)
    {
        await Clients.All.ReceiveJobUpdate(job);
    }

    public async Task SendJobStateChanged(int id, JobState jobState)
    {
        await Clients.All.ReceiveJobStateChanged(id, jobState.ToString());
    }
}