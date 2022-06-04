using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace POSHWebTests.Lab;

public class EnvironmentManagerTest
{
    [Fact]
    public void ExecuteAsAnotherUser()
    {
        var process = new Process();
        SecureString password = new SecureString();
        password.AppendChar('t');
        password.AppendChar('m');
        password.AppendChar('p');
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.WorkingDirectory = "C:\\";
        process.StartInfo.Domain = ".";
        process.StartInfo.UserName = "tmp";
        process.StartInfo.Password = password;
        process.StartInfo.Arguments = "";
        process.Start();
        process.WaitForExit();
    }
}