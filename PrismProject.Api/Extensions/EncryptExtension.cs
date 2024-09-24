using System.Security.Cryptography;
using System.Text;

namespace PrismProject.Api.Extensions
{
    public static class EncryptExtension
    {
        public static string GetMD5(this string input)
        {
            if(string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Input cannot be null or empty.");
            var hash=MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
