using System.Security.Cryptography;
using System.Text;

namespace POSHWeb.Services;

public class HasherService
{
    public string Sha256(string content)
    {
        var mySHA256 = SHA256.Create();
        var bytes = Encoding.ASCII.GetBytes(content);
        var bytesHash = mySHA256.ComputeHash(bytes);
        return BytesAsString(bytesHash);
    }

    public byte[] Sha256Bytes(string content)
    {
        var mySHA256 = SHA256.Create();
        var bytes = Encoding.ASCII.GetBytes(content);
        var bytesHash = mySHA256.ComputeHash(bytes);
        return bytesHash;
    }

    private static string BytesAsString(byte[] array)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < array.Length; i++) sb.Append($"{array[i]:x2}");

        return sb.ToString();
    }
}