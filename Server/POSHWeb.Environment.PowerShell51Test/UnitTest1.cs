using System.Management.Automation;
using Moq;
using POSHWeb.Environment.Interfaces;
using POSHWeb.Environment.Model;
using POSHWeb.Environment.PowerShell51.Runspace;
using Xunit.Abstractions;

namespace POSHWeb.Environment.PowerShell51Test;

public class UnitTest1
{
    
    private readonly ITestOutputHelper outputHelper;

    public UnitTest1(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper;
    }


    [Fact]
    public void CheckVersion()
    {
        var ipsInteractionMock = new Mock<IPSInteraction>();
        ipsInteractionMock.Name = "IPSInteraction";
        var executer = new Executer(new RunspaceSettings(), ipsInteractionMock.Object);
        executer.RunspaceManager.Start();
        var result = executer.Run("return $PSVersionTable");
        executer.RunspaceManager.Stop();
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
    }
}