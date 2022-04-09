using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalRChat.Hubs;

namespace POSHWebTests.Mocks;

public class MockScriptHub
{
    public static IHubContext<ScriptHub, IScriptHub> CreateMockScriptHub()
    {
        var mock = new Mock<IHubContext<ScriptHub, IScriptHub>>();

        return mock.Object;
    }
}