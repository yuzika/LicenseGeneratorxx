using System;
using System.IO;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace LicenseGenerator.Helpers
{
    public static class DbEncryptor
    {
        public static bool EncryptDatabase(string sourceDbPath, string outputEncryptedDbPath, string dbGuidKey, out string message)
        {
            return InternalEncryptDatabase(sourceDbPath, outputEncryptedDbPath, dbGuidKey, out message);
        }

        private static bool InternalEncryptDatabase(string sourceDbPath, string outputEncryptedDbPath, string dbGuidKey, out string message)
        {
            message = string.Empty;

            try
            {
                if (!File.Exists(sourceDbPath))
                {
                    message = "Kaynak veritabanı bulunamadı.";
                    return false;
                }

                if (File.Exists(outputEncryptedDbPath))
                    File.Delete(outputEncryptedDbPath);

                Batteries_V2.Init();

                using (var conn = new SqliteConnection($"Data Source={sourceDbPath};"))
                {
                    conn.Open();

                    // Hedef veritabanını şifreli olarak attach et
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"ATTACH DATABASE '{outputEncryptedDbPath}' AS encrypted KEY '{dbGuidKey}';";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT sqlcipher_export('encrypted');";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DETACH DATABASE encrypted;";
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                message = "Veritabanı başarıyla SQLCipher formatında şifrelendi.";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Şifreleme hatası: {ex.Message}";
                return false;
            }
        }
    }
}
