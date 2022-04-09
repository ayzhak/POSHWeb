using System.Security.Cryptography;
using System.Text;

namespace POSHWeb.Services
{
    public class HasherService
    {
        public string Sha256(string content)
        {
            SHA256 mySHA256 = SHA256.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            byte[] bytesHash = mySHA256.ComputeHash(bytes);
            return BytesAsString(bytesHash);
        }

        public byte[] Sha256Bytes(string content)
        {
            SHA256 mySHA256 = SHA256.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            byte[] bytesHash = mySHA256.ComputeHash(bytes);
            return bytesHash;
        }

        private static string BytesAsString(byte[] array)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append($"{array[i]:x2}");
            }

            return sb.ToString();
        }
    }
}
