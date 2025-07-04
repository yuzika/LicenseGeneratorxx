using System;
using System.IO;

namespace LicenseGenerator.Helpers
{
    public static class DbKeyFileHelper
    {
        public static bool SaveDbKey(string dbKey, string usbDrivePath)
        {
            try
            {
                // USB yolunu ve dosya adını belirle
                string usbKeyPath = Path.Combine(usbDrivePath, "dbKey.dat");

                // Eğer dosya varsa sil
                if (File.Exists(usbKeyPath))
                {
                    File.SetAttributes(usbKeyPath, FileAttributes.Normal);
                    File.Delete(usbKeyPath);
                }

                // Anahtarı şifreleyerek kaydet
                string encryptedKey = CryptoHelper.CryptoHelper.EncryptString(dbKey, "STATIC_SALT_VALUE");
                File.WriteAllText(usbKeyPath, encryptedKey);

                // Dosya özniteliklerini güvenli hale getir
                File.SetAttributes(usbKeyPath, FileAttributes.Hidden | FileAttributes.ReadOnly);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log($"DbKey kaydetme hatası: {ex}");
                return false;
            }
        }

        public static string LoadDbKey(string usbDrivePath)
        {
            try
            {
                string usbKeyPath = Path.Combine(usbDrivePath, "dbKey.dat");

                if (!File.Exists(usbKeyPath))
                    return null;

                string encryptedKey = File.ReadAllText(usbKeyPath);
                return CryptoHelper.CryptoHelper.DecryptString(encryptedKey, "STATIC_SALT_VALUE");
            }
            catch (Exception ex)
            {
                LogHelper.Log($"DbKey yükleme hatası: {ex}");
                return null;
            }
        }
    }
}