using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Helpers
{
    public static class SHA256HashHelper
    {
        public static string CalculateSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool AreHashesEqual<T>(T obj1, T obj2)
        {
            string json1 = JsonConvert.SerializeObject(obj1, Formatting.Indented);
            string json2 = JsonConvert.SerializeObject(obj2, Formatting.Indented);
            string hash1 = CalculateSHA256Hash(json1);
            string hash2 = CalculateSHA256Hash(json2);

            return hash1 == hash2;
        }
    }
}
