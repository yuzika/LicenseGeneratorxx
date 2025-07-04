using System;
using System.IO;
using System.Windows.Forms;
using CryptoHelper;

namespace LicenseGenerator.Helpers
{
    public static class LicenseFileHelper
    {
        private static readonly string LicenseFileName = "license.dat";
        private static readonly string SecretKey = "MY_STATIC_KEY_1234"; // 16/24/32 karakter uzunluğunda AES key

        public static bool CreateLicenseFile(string usbSerial, string guid, string outputDirectory)
        {
            try
            {
                // outputDirectory parametresi artık doğrudan txtUsbPath2.Text olarak gönderilmeli
                string fullPath = Path.Combine(outputDirectory, LicenseFileName);
                string data = $"{usbSerial}|{guid}";

                string encrypted = CryptoHelper.CryptoHelper.EncryptString(data, SecretKey);
                File.WriteAllText(fullPath, encrypted);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lisans dosyası oluşturulurken hata: " + ex.Message);
                return false;
            }
        }

        public static bool ValidateLicenseFile(string expectedUsbSerial, string licenseFilePath, out string decryptedGuid)
        {
            decryptedGuid = string.Empty;

            try
            {
                // Artık dosya yolu parametre olarak alınıyor
                if (!File.Exists(licenseFilePath))
                {
                    MessageBox.Show("Lisans dosyası bulunamadı.");
                    return false;
                }

                string encrypted = File.ReadAllText(licenseFilePath);
                string decrypted = CryptoHelper.CryptoHelper.DecryptString(encrypted, SecretKey);

                string[] parts = decrypted.Split('|');
                if (parts.Length != 2)
                    return false;

                string storedUsbSerial = parts[0];
                decryptedGuid = parts[1];

                return string.Equals(storedUsbSerial, expectedUsbSerial, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // Bu metod gereksiz görünüyor, ValidateLicenseFile ile birleştirilebilir
        public static bool ValidateLicense(string licenseFilePath, string expectedUsbSerial)
        {
            string _;
            return ValidateLicenseFile(expectedUsbSerial, licenseFilePath, out _);
        }
    }
}