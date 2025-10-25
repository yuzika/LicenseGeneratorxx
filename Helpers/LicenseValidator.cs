using System;
using System.IO;
using System.Text;
using CryptoHelper;
using LicenseGenerator;

namespace LicenseGenerator.Helpers
{
    public static class LicenseValidator
    {
        private const string Salt = "Z1x7p@#k!";

        public static LicenseValidationResult Validate(string licensePath, string usbSerialRaw)
        {
            var result = new LicenseValidationResult();

            try
            {
                string usbSerial = CryptoHelper.CryptoHelper.ComputeSHA256(usbSerialRaw);

                byte[] fileBytes = File.ReadAllBytes(licensePath);
                if (fileBytes.Length < 16 + 32 + 256 + 64 + 1)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "license.dat dosyası eksik veya bozuk.";
                    return result;
                }

                int pos = 0;

                byte[] iv = new byte[16];
                Array.Copy(fileBytes, pos, iv, 0, 16);
                pos += 16;

                byte[] encryptedPayload = new byte[64];
                Array.Copy(fileBytes, pos, encryptedPayload, 0, 64);
                pos += 64;

                byte[] hash = new byte[32];
                Array.Copy(fileBytes, pos, hash, 0, 32);
                pos += 32;

                byte[] signature = new byte[256];
                Array.Copy(fileBytes, pos, signature, 0, 256);
                pos += 256;

                string dbMasterKey = Encoding.ASCII.GetString(fileBytes, pos, 64);
                pos += 64;
                result.DbMasterKey = dbMasterKey;

                string encryptedAppMasterKeyBase64 = Encoding.UTF8.GetString(fileBytes, pos, fileBytes.Length - pos);

                string resolvedAppMasterKey = GetResolvedAppMasterKey(encryptedAppMasterKeyBase64, usbSerial, iv);
                result.ResolvedAppMasterKey = resolvedAppMasterKey;

                byte[] payloadKey = CryptoHelper.CryptoHelper.ComputeSHA256Bytes(usbSerial + resolvedAppMasterKey);
                string decryptedPayload = CryptoHelper.CryptoHelper.DecryptWithAes(encryptedPayload, payloadKey, iv);

                string[] parts = decryptedPayload.Split('|');
                if (parts.Length != 3)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Payload formatı geçersiz.";
                    return result;
                }

                result.UsbSerial = parts[0];
                result.DbGuidKey = parts[1];
                result.Timestamp = parts[2];
                result.IsValid = true;
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Log("[VALIDATOR] HATA: " + ex.Message);
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        private static string GetResolvedAppMasterKey(string encryptedBase64, string usbSerial, byte[] iv)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
            byte[] key = CryptoHelper.CryptoHelper.ComputeSHA256Bytes(usbSerial + Salt);
            return CryptoHelper.CryptoHelper.DecryptWithAes(encryptedBytes, key, iv);
        }
    }
}
