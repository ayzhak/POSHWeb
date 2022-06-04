using System.Management.Automation;

namespace POSHWeb.Environment.Interfaces;

public interface IRunspaceManager
{
    bool Running { get; }
    void Start();
    void Attach(PowerShell powershell);
    void Stop(); 
}