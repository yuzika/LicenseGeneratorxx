using System;
using System.Data.SQLite;
using System.IO;

namespace LicenseGenerator.Helpers
{
    public static class DbEncryptor
    {
        public static bool EncryptDatabase(string sourcePath, string targetPath, string password)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    LogHelper.Log("Kaynak veritabanı dosyası bulunamadı: " + sourcePath);
                    return false;
                }

                string tempPath = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}.db");

                try
                {
                    using (var source = new SQLiteConnection($"Data Source={sourcePath};Version=3;"))
                    using (var dest = new SQLiteConnection($"Data Source={tempPath};Version=3;Password={password};"))
                    {
                        source.Open();
                        dest.Open();

                        // DÜZELTME: Callback kaldırıldı veya doğru şekilde uygulandı
                        source.BackupDatabase(
                            destination: dest,
                            destinationName: "main",
                            sourceName: "main",
                            pages: -1,
                            callback: null, // Callback kaldırıldı
                            retryMilliseconds: 100
                        );

                        // Alternatif ilerleme takibi (isteğe bağlı)
                        LogHelper.Log("Veritabanı yedekleme işlemi başladı");
                    }

                    if (File.Exists(targetPath))
                    {
                        File.SetAttributes(targetPath, FileAttributes.Normal);
                        File.Delete(targetPath);
                    }

                    File.Move(tempPath, targetPath);
                    File.SetAttributes(targetPath, FileAttributes.Normal);

                    return true;
                }
                finally
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log($"Şifreleme hatası: {ex}");
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

                    // Basit bir sorgu ile test etme
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
            catch (SQLiteException sqlEx)
            {
                result = $"SQL Hatası: {sqlEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                result = $"Genel Hata: {ex.Message}";
                return false;
            }
        }
    }
}