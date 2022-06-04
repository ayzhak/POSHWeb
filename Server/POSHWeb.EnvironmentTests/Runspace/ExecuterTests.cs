using System;
using Moq;
using Xunit;
using Xunit.Abstractions;   
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using POSHWeb.Environment.Enum;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Util;
using POSHWeb.Environment.Runspace;
using POSHWeb.Environment.Model;

namespace POSHWeb.EnvironmentTests.Runspace;

public class ExecuterTests
{
    private readonly ITestOutputHelper outputHelper;
    private readonly IMessageSink _diagnosticMessageSink;

    public ExecuterTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper;
    }

    [Fact]
    public void RunTest()
    {
        string log = null;
        var ipsInteractionMock = new Mock<IPSInteraction>();
        ipsInteractionMock.Name = "IPSInteraction";
        ipsInteractionMock.Setup(interaction =>
                interaction.Log(It.Is<SeverityLevel>(level => level == SeverityLevel.Information),
                    It.Is<string>(s => s == "Hello")))
            .Callback((SeverityLevel level, string msg) => log = msg)
            .Verifiable("Verify");
        var executer = new Executer(new RunspaceSettings(), ipsInteractionMock.Object);
        executer.RunspaceManager.Start();
        executer.Run("Write-Host 'Hello'");
        executer.RunspaceManager.Stop();
        ipsInteractionMock.Verify(interaction =>
                interaction.Log(It.Is<SeverityLevel>(level => level == SeverityLevel.Information),
                    It.Is<string>(s => s == "Hello")),
            Times.Once());
        Assert.Equal("Hello", log);
    }

    [Fact]
    public void RunError()
    {
        var ipsInteractionMock = new Mock<IPSInteraction>();
        ipsInteractionMock.Setup(interaction => interaction.Log(It.Is<SeverityLevel>(level => level == SeverityLevel.Error), It.IsAny<string>()))
            .Callback((SeverityLevel level, string s) =>
                outputHelper.WriteLine(s));
        var executer = new Executer(new RunspaceSettings(), ipsInteractionMock.Object);
        executer.RunspaceManager.Start();
        executer.Run("Write-df 'Hello'");
        executer.RunspaceManager.Stop();
        ipsInteractionMock.Verify(interaction => interaction.Exception(It.IsAny<Exception>()), Times.Once());
        ipsInteractionMock.Verify(
            interaction => interaction.Log(It.Is<SeverityLevel>(level => level == SeverityLevel.Error), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void RunString()
    {
        var ipsInteractionMock = new Mock<IPSInteraction>();
        ipsInteractionMock.Name = "IPSInteraction";
        var executer = new Executer(new RunspaceSettings(), ipsInteractionMock.Object);
        executer.RunspaceManager.Start();
        var result = executer.Run("[PSCustomObject]@{\"Test\"=\"Test\"}");


        
        executer.RunspaceManager.Stop();
        var item = new List<PSObject>(result);
        ipsInteractionMock.VerifyAll();
        
        var ps = PowerShell.Create();
        ps.AddCommand("ConvertTo-Json");
        ps.AddParameter("InputObject", result);
        ps.AddParameter("Depth", 10);
        ps.AddParameter("Compress", true);
        ps.AddParameter("EnumsAsStrings", true);
        var json = ps.Invoke();
        outputHelper.WriteLine(json.First().ToString());
    }
}