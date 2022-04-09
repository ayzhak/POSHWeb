using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalRChat.Hubs;

namespace POSHWebTests.Mocks
{
    public class MockScriptHub
    {
        public static IHubContext<ScriptHub, IScriptHub> CreateMockScriptHub()
        {
            var mock = new Mock<IHubContext<ScriptHub, IScriptHub>>();

            return mock.Object;
        }
    }
}
