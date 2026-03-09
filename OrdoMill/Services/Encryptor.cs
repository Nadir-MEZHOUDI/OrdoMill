using System;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Versioning;
using System.Management;

namespace OrdoMill.Services
{
    internal static class Encryptor
    {

        internal static bool CompareKey(string key) => key?.Equals(GetKey()) ?? false;

        static string GetKey() => Encrypt(GetPrimaryKey(), "Mill").Substring(0, 16);

        [SupportedOSPlatform("windows")]
        private static string GetCpuId()
        {
            string cpuInfo = string.Empty;
            var mc = new ManagementClass("win32_processor");
            var moc = mc.GetInstances();

            foreach (var mo in moc)
            {
                if (cpuInfo != "") continue;
                //Get only the first CPU's ID
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return cpuInfo;
        }


        private static string Encrypt(string plainText, string salt)
        {
            var result = GenerateSaltedHash(Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(salt));
            return Convert.ToBase64String(result).ToUpper().Replace("+", "").Replace("/", "");
        }

        internal static string GetPrimaryKey()
        {
            var primaryKey = Encrypt(GetCpuId(), "Ordo");

            return primaryKey.ToUpper().Substring(0, 8);
        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            using HashAlgorithm algorithm = SHA256.Create();

            byte[] plainTextWithSaltBytes =
                new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }


    }
}
