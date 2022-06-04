using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using static System.Security.Principal.WindowsIdentity;

namespace POSHWeb.Scheduler.Util
{
    public enum LogonType
    {
        LOGON32_LOGON_BATCH = 4,
        LOGON32_LOGON_INTERACTIVE = 2,
        LOGON32_LOGON_NETWORK = 3,
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8,
        LOGON32_LOGON_NEW_CREDENTIALS = 9,
        LOGON32_LOGON_SERVICE = 5,
        LOGON32_LOGON_UNLOCK = 7,
    }

    public class Impersonator : IDisposable
    {
        private IntPtr tokenDuplicate = IntPtr.Zero;
        private IntPtr token = IntPtr.Zero;

        public Impersonator(string userName, string domainName, string password, LogonType logonType)
        {
            ImpersonateValidUser(userName, domainName, password, logonType);
        }
        public R Run<R>(Func<R> func)
        {
            WindowsIdentity tempWindowsIdentity = new WindowsIdentity(token);
            return RunImpersonated<R>(tempWindowsIdentity.AccessToken,
                () =>
                {
                    return func();
                });
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(string lpszUserName, string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);

        private const int LOGON32_PROVIDER_DEFAULT = 0;

        private void ImpersonateValidUser(string userName, string domain, string password, LogonType logonType)
        {
            if (LogonUser(userName, domain, password, (int)logonType, LOGON32_PROVIDER_DEFAULT, ref token) == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (DuplicateToken(token, 2, ref tokenDuplicate) == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public void Dispose()
        {
            if (token != IntPtr.Zero)
            {
                CloseHandle(token);
            }

            if (tokenDuplicate != IntPtr.Zero)
            {
                CloseHandle(tokenDuplicate);
            }
        }
    }
}