using System;
using System.Security.Cryptography;
using System.Text;

namespace MShare_ASP.Utils {
    internal static class Hasher {
        public static string GetHash(string value) {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using (SHA256 hasher = SHA256.Create()) {
                byte[] hash = hasher.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                Array.ForEach(hash, h => sb.AppendFormat("{0:X2}", h));

                return sb.ToString();
            }
        }
    }
}