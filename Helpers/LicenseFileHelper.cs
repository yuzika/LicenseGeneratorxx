using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CryptoHelper;
using LicenseGenerator.Helpers;

namespace LicenseGenerator.Helpers
{
    public static class LicenseFileHelper
    {
        private const string Salt = "Z1x7p@#k!";

        public static void CreateLicenseFile(
            string usbSerialRaw,           // artık gelen ham serial
            string dbGuidKey,
            string dbMasterKey,            // 64 karakter HEX
            string resolvedAppMasterKey,   // plain text
            string outputPath)
        {
            try
            {
                // ✅ usbSerial SHA256 sabitleme
                string usbSerial = CryptoHelper.CryptoHelper.ComputeSHA256(usbSerialRaw);

                LogHelper.Log("[LICENSE] İşlem başlatıldı");
                LogHelper.Log("[LICENSE] usbSerial (raw): " + usbSerialRaw);
                LogHelper.Log("[LICENSE] usbSerial (hashed): " + usbSerial);
                LogHelper.Log("[LICENSE] dbGuidKey: " + dbGuidKey);
                LogHelper.Log("[LICENSE] dbMasterKey: " + dbMasterKey);
                LogHelper.Log("[LICENSE] dbMasterKey.Length: " + dbMasterKey.Length);

                string timestamp = DateTime.UtcNow.Ticks.ToString();
                LogHelper.Log("[LICENSE] timestamp: " + timestamp);

                string payload = $"{usbSerial}|{dbGuidKey}|{timestamp}";

                byte[] iv = CryptoHelper.CryptoHelper.GenerateRandomIV();
                LogHelper.Log("[LICENSE] IV: " + BitConverter.ToString(iv).Replace("-", ""));

                byte[] aesKey = CryptoHelper.CryptoHelper.ComputeSHA256Bytes(usbSerial + resolvedAppMasterKey);
                byte[] encryptedPayload = CryptoHelper.CryptoHelper.EncryptWithAes(payload, aesKey, iv);
                LogHelper.Log("[LICENSE] EncryptedPayload.Length: " + encryptedPayload.Length);

                byte[] hash = CryptoHelper.CryptoHelper.ComputeSha256Bytes(encryptedPayload);
                LogHelper.Log("[LICENSE] SHA256 Hash: " + BitConverter.ToString(hash).Replace("-", ""));

                byte[] signature = CryptoHelper.CryptoHelper.SignDataWithRsa(hash, AppConstants.RsaPrivateKeyXml);
                LogHelper.Log("[LICENSE] RSA Signature Length: " + signature.Length);

                byte[] appKeyKey = CryptoHelper.CryptoHelper.ComputeSHA256Bytes(usbSerial + Salt);
                byte[] encryptedAppMasterKeyBytes = CryptoHelper.CryptoHelper.EncryptWithAes(resolvedAppMasterKey, appKeyKey, iv);
                string encryptedAppMasterKeyBase64 = Convert.ToBase64String(encryptedAppMasterKeyBytes);
                LogHelper.Log("[LICENSE] EncryptedAppMasterKey Base64 (ilk 32): " + encryptedAppMasterKeyBase64.Substring(0, Math.Min(32, encryptedAppMasterKeyBase64.Length)));
                LogHelper.Log("[LICENSE] EncryptedAppMasterKey Length: " + encryptedAppMasterKeyBase64.Length);

                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                    LogHelper.Log("[LICENSE] Önceki license.dat silindi");
                }

                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(iv); // 16
                    bw.Write(encryptedPayload); // N
                    bw.Write(hash); // 32
                    bw.Write(signature); // 256

                    if (dbMasterKey.Length != 64)
                        throw new Exception("dbMasterKey 64 karakter HEX olmalıdır.");
                    bw.Write(Encoding.ASCII.GetBytes(dbMasterKey)); // 64

                    if (string.IsNullOrWhiteSpace(encryptedAppMasterKeyBase64))
                        throw new Exception("encryptedAppMasterKey boş.");
                    bw.Write(Encoding.UTF8.GetBytes(encryptedAppMasterKeyBase64)); // variable

                    bw.Flush();
                }

                LogHelper.Log("[LICENSE] license.dat yazıldı: " + outputPath);
                LogHelper.Log("[LICENSE] Toplam byte: " + new FileInfo(outputPath).Length);

                byte[] finalBytes = File.ReadAllBytes(outputPath);
                string fileHash = BitConverter.ToString(SHA256.Create().ComputeHash(finalBytes)).Replace("-", "");
                LogHelper.Log("[LICENSE] SHA256 (dosya): " + fileHash);
            }
            catch (Exception ex)
            {
                LogHelper.Log("[LICENSE] HATA: " + ex.Message);
                throw new Exception("Lisans dosyası oluşturulamadı", ex);
            }
        }
    }
}
