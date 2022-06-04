using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace POSHWeb.Environment.Util;

public static class SecureStringUtil
{
    public static string ConvertTo(SecureString secureString)
    {
        return new NetworkCredential("", secureString).Password;
    }

    public static SecureString ConvertTo(string str)
    {
        return new NetworkCredential("", str).SecurePassword;
        ;
    }
}