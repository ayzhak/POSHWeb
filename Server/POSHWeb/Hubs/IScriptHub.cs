using POSHWeb.Model;
using POSHWeb.Model.Job;

namespace SignalRChat.Hubs;

public interface IScriptHub
{
    Task ReceiveScriptCreated(PSScript script);
    Task ReceiveScriptChanged(PSScript script);
    Task ReceiveScriptRemoved(int id);
    Task ReceiveJobUpdate(Job id);
    Task ReceiveJobStateChanged(int id, string state);
}