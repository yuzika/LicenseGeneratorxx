using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LicenseGenerator.Helpers
{
    public static class DbKeyFileHelperPortable
    {
        private const string masterKey = "MySecureAppLevelMasterKey_2025!";

        public static bool SavePortableKey(string dbKey, string outputDirectory)
        {
            try
            {
                var aes = Aes.Create();
                aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(masterKey));
                aes.GenerateIV();

                string fullPath = Path.Combine(outputDirectory, "dbkey.dat");

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    fs.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(dbKey);
                    }
                }

                File.SetAttributes(fullPath, FileAttributes.Hidden | FileAttributes.ReadOnly);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log("Taşınabilir dbkey.dat oluşturma hatası: " + ex.Message);
                return false;
            }
        }

        public static string LoadPortableKey(string inputDirectory)
        {
            try
            {
                string fullPath = Path.Combine(inputDirectory, "dbkey.dat");

                if (!File.Exists(fullPath))
                    return null;

                var aes = Aes.Create();
                aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(masterKey));

                using (var fs = new FileStream(fullPath, FileMode.Open))
                {
                    byte[] iv = new byte[16];
                    fs.Read(iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("Taşınabilir dbkey.dat okuma hatası: " + ex.Message);
                return null;
            }
        }
    }
}
