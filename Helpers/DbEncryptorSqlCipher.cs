using System;
using System.Data.SQLite;
using System.IO;

namespace LicenseGenerator.Helpers
{
    public static class DbEncryptorSqlCipher
    {
        public static bool EncryptDatabase(string sourcePath, string targetPath, string password)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    LogHelper.Log("Kaynak veritabanı bulunamadı: " + sourcePath);
                    return false;
                }

                // Hedef dosya zaten varsa sil
                if (File.Exists(targetPath))
                {
                    File.SetAttributes(targetPath, FileAttributes.Normal);
                    File.Delete(targetPath);
                }

                // Kaynak bağlantı (şifresiz)
                using (var source = new SQLiteConnection($"Data Source={sourcePath};Version=3;"))
                {
                    source.Open();

                    // Hedef bağlantı (şifreli)
                    using (var dest = new SQLiteConnection($"Data Source={targetPath};Version=3;Password={password};"))
                    {
                        dest.Open();

                        // Tüm verileri yedekle
                        source.BackupDatabase(dest, "main", "main", -1, null, 0);

                        LogHelper.Log("BackupDatabase ile şifreleme başarılı.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log($"BackupDatabase şifreleme hatası: {ex}");
                return false;
            }
        }

        public static bool TestDecryption(string dbPath, string password, out string result)
        {
            result = string.Empty;
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Password={password};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' LIMIT 1", conn))
                    {
                        var tableName = cmd.ExecuteScalar() as string;
                        result = tableName != null
                            ? $"Başarılı. İlk tablo: {tableName}"
                            : "Veritabanı boş";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = $"Hata: {ex.Message}";
                return false;
            }
        }
    }
}
