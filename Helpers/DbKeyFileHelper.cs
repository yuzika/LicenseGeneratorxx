using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LicenseGenerator.Helpers
{
    public static class DbKeyFileHelper
    {
        private const string DbKeyFileName = "dbkey.dat";
        private static string _OrjMaskey = null;

        public static void SetPassword(string DbOrjMasterkey)
        {
            _OrjMaskey = DbOrjMasterkey;
        }

        public static bool GenerateAndSaveDbKeyFile(string usbPath, string usbSerial, string dbGuidKey, out string fullPath, out string error)
        {
            error = null;
            fullPath = Path.Combine(usbPath, DbKeyFileName);

            try
            {
                if (string.IsNullOrWhiteSpace(_OrjMaskey))
                    throw new Exception("Şifreleme anahtarı (MasterKey) atanmadı. Lütfen SetPassword(...) çağır.");

                byte[] key = DeriveKey(usbSerial + _OrjMaskey);
                byte[] iv = GenerateRandomBytes(16);
                byte[] encrypted;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(dbGuidKey);
                        sw.Close();
                        encrypted = ms.ToArray();
                    }
                }

                using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(iv, 0, iv.Length);
                    fs.Write(encrypted, 0, encrypted.Length);
                }

                return true;
            }
            catch (Exception ex)
            {
                error = "dbkey.dat oluşturulurken hata oluştu: " + ex.Message;
                return false;
            }
        }

        public static string LoadDbKey(string usbPath, string usbSerial)
        {
            string fullPath = Path.Combine(usbPath, DbKeyFileName);
            if (!File.Exists(fullPath))
                return null;

            try
            {
                byte[] fileBytes = File.ReadAllBytes(fullPath);
                byte[] iv = fileBytes.Take(16).ToArray();
                byte[] encrypted = fileBytes.Skip(16).ToArray();

                if (string.IsNullOrWhiteSpace(_OrjMaskey))
                    throw new Exception("Çözümleme anahtarı (MasterKey) atanmadı. Lütfen SetPassword(...) çağır.");

                byte[] key = DeriveKey(usbSerial + _OrjMaskey);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var ms = new MemoryStream(encrypted))
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static byte[] DeriveKey(string input)
        {
            using (SHA256 sha = SHA256.Create())
                return sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        private static byte[] GenerateRandomBytes(int count)
        {
            var data = new byte[count];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(data);
            return data;
        }
    }
}
