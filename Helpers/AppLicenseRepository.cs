using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace LicenseGenerator.Helpers
{
    public class AppLicenseRepository
    {
        private readonly string _connectionString;

        public AppLicenseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// USB seri numarası varsa AppMasterKey güncellenir, yoksa yeni satır eklenir.
        /// </summary>
        public void InsertOrUpdateAppKey(string usbSerialNo, string appMasterKey)
        {
            if (string.IsNullOrWhiteSpace(usbSerialNo) || string.IsNullOrWhiteSpace(appMasterKey))
                throw new ArgumentException("USB seri numarası veya anahtar boş olamaz.");

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Satır var mı kontrol et
                string checkSql = "SELECT COUNT(*) FROM lisanslar WHERE UsbSeriNo = @UsbSeriNo";
                using (var checkCmd = new SqliteCommand(checkSql, connection))
                {
                    checkCmd.Parameters.AddWithValue("@UsbSeriNo", usbSerialNo);
                    long count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Güncelle
                        string updateSql = "UPDATE lisanslar SET AppMasterKey = @AppMasterKey WHERE UsbSeriNo = @UsbSeriNo";
                        using (var updateCmd = new SqliteCommand(updateSql, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@AppMasterKey", appMasterKey);
                            updateCmd.Parameters.AddWithValue("@UsbSeriNo", usbSerialNo);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Yeni kayıt ekle
                        string insertSql = "INSERT INTO lisanslar (UsbSeriNo, AppMasterKey, AppCreateDate) VALUES (@UsbSeriNo, @AppMasterKey, @AppCreateDate)";
                        using (var insertCmd = new SqliteCommand(insertSql, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@UsbSeriNo", usbSerialNo);
                            insertCmd.Parameters.AddWithValue("@AppMasterKey", appMasterKey);
                            insertCmd.Parameters.AddWithValue("@AppCreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// USB seri numarasına ait AppMasterKey'i döner (varsa), yoksa null
        /// </summary>
        public string GetAppMasterKey(string usbSerialNo)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT AppMasterKey FROM lisanslar WHERE UsbSeriNo = @UsbSeriNo";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@UsbSeriNo", usbSerialNo);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
        public static void InsertOrUpdateRsaKey(string usbSerial, string rsaKeyXml)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "licenses.db");
            string connStr = $"Data Source={dbPath};";

            using (var conn = new Microsoft.Data.Sqlite.SqliteConnection(connStr))
            {
                conn.Open();

                // USB kaydı var mı kontrol et
                string checkQuery = "SELECT COUNT(*) FROM Lisanslar WHERE UsbSeriNo = @usb";
                using (var cmd = new Microsoft.Data.Sqlite.SqliteCommand(checkQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@usb", usbSerial);
                    long count = (long)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Güncelle
                        string update = "UPDATE Lisanslar SET RSAKey = @key WHERE UsbSeriNo = @usb";
                        using (var updateCmd = new Microsoft.Data.Sqlite.SqliteCommand(update, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@key", rsaKeyXml);
                            updateCmd.Parameters.AddWithValue("@usb", usbSerial);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Ekle
                        string insert = "INSERT INTO Lisanslar (UsbSeriNo, RSAKey, CreateDate) VALUES (@usb, @key, @dt)";
                        using (var insertCmd = new Microsoft.Data.Sqlite.SqliteCommand(insert, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@usb", usbSerial);
                            insertCmd.Parameters.AddWithValue("@key", rsaKeyXml);
                            insertCmd.Parameters.AddWithValue("@dt", DateTime.Now);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Belirtilen USB seri numarası veritabanında kayıtlı mı?
        /// </summary>
        public bool Exists(string usbSerialNo)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT 1 FROM lisanslar WHERE UsbSeriNo = @UsbSeriNo LIMIT 1";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@UsbSeriNo", usbSerialNo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
        }
    }
}
