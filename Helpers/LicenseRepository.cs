using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace LicenseGenerator.Helpers
{
    public static class LicenseRepository
    {
        private static string _dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "licenses.db");

        public static void InsertLicenseRecord(
            string firmaAdi,
            string kullaniciAdi,
            string deliveryDate,
            string usbSeriNo,
            string dbGuidKey,
            string lisansGuid,
            string dbMasterKey,
            string portableMasterKey,
            string appMasterKey,
            string appCreateDate,
            string rsaKey, 
            string UsbSeriNoHash)
        
            
        {
            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                        INSERT INTO Lisanslar (
                            FirmaAdi,
                            KullaniciAdi,
                            DeliveryDate,
                            USBSeriNo,
                            DbGuidKey,
                            LisansGuid,
                            CreateDate,
                            DbMasterKey,
                            PortableMasterKey,
                            AppMasterkey,
                            AppCreateDate,
                            RSAKey,
                            UsbSeriNoHash
                        )
                        VALUES (
                            @FirmaAdi,
                            @KullaniciAdi,
                            @DeliveryDate,
                            @USBSeriNo,
                            @DbGuidKey,
                            @LisansGuid,
                            @CreateDate,
                            @DbMasterKey,
                            @PortableMasterKey,
                            @AppMasterkey,
                            @AppCreateDate,
                            @RSAKey,
                            @UsbSeriNoHash
                        );
                    ";

                    /*command.Parameters.AddWithValue("@FirmaAdi", firmaAdi);
                    command.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
                    command.Parameters.AddWithValue("@DeliveryDate", deliveryDate);
                    command.Parameters.AddWithValue("@USBSeriNo", usbSeriNo);
                    command.Parameters.AddWithValue("@DbGuidKey", dbGuidKey);
                    command.Parameters.AddWithValue("@LisansGuid", lisansGuid);
                    command.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@DbMasterKey", dbMasterKey);
                    command.Parameters.AddWithValue("@PortableMasterKey", portableMasterKey);
                    command.Parameters.AddWithValue("@AppMasterkey", appMasterKey);
                    command.Parameters.AddWithValue("@AppCreateDate", appCreateDate);
                    command.Parameters.AddWithValue("@RSAKey", rsaKey);*/

                    command.Parameters.AddWithValue("@FirmaAdi", (object)firmaAdi ?? DBNull.Value);
                    command.Parameters.AddWithValue("@KullaniciAdi", (object)kullaniciAdi ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DeliveryDate", (object)deliveryDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@USBSeriNo", (object)usbSeriNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DbGuidKey", (object)dbGuidKey ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LisansGuid", (object)lisansGuid ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@DbMasterKey", (object)dbMasterKey ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PortableMasterKey", (object)portableMasterKey ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AppMasterkey", (object)appMasterKey ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AppCreateDate", (object)appCreateDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RSAKey", (object)rsaKey ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UsbSeriNoHash", (object)UsbSeriNoHash ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Lisans veritabanına kayıt yapılamadı.\n\n{ex.Message}", "HATA", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void UpsertDbEncryptionInfo(string usbSerial, string dbGuidKey, string dbMasterKey, string UsbSeriNoHash)
        {
            string createDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();

                    var checkCmd = connection.CreateCommand();
                    checkCmd.CommandText = "SELECT Id, DbMasterKey FROM Lisanslar WHERE USBSeriNo = @serial LIMIT 1;";
                    checkCmd.Parameters.AddWithValue("@serial", usbSerial);

                    int? existingId = null;
                    bool dbMasterKeyEmpty = false;

                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            existingId = reader.GetInt32(0);
                            dbMasterKeyEmpty = reader.IsDBNull(1) || string.IsNullOrWhiteSpace(reader.GetString(1));
                        }
                    }

                    if (existingId.HasValue && dbMasterKeyEmpty)
                    {
                        var updateCmd = connection.CreateCommand();
                        updateCmd.CommandText = @"
                            UPDATE Lisanslar
                            SET DbGuidKey = @dbGuidKey,
                                DbMasterKey = @dbMasterKey,
                                CreateDate = @createDate
                            WHERE Id = @id;";
                        updateCmd.Parameters.AddWithValue("@dbGuidKey", dbGuidKey);
                        updateCmd.Parameters.AddWithValue("@dbMasterKey", dbMasterKey);
                        updateCmd.Parameters.AddWithValue("@createDate", createDate);
                        updateCmd.Parameters.AddWithValue("@id", existingId.Value);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        var existsCmd = connection.CreateCommand();
                        existsCmd.CommandText = "SELECT COUNT(*) FROM Lisanslar WHERE USBSeriNo = @serial;";
                        existsCmd.Parameters.AddWithValue("@serial", usbSerial);
                        long count = (long)existsCmd.ExecuteScalar();

                        string serialToUse = count > 0 ? "new_" + usbSerial : usbSerial;

                        var insertCmd = connection.CreateCommand();
                        insertCmd.CommandText = @"
                            INSERT INTO Lisanslar (
                                USBSeriNo, DbGuidKey, DbMasterKey, CreateDate,UsbSeriNoHash
                            )
                            VALUES (
                                @usbSerial, @dbGuidKey, @dbMasterKey, @createDate,@UsbSeriNoHash
                            );";
                        insertCmd.Parameters.AddWithValue("@usbSerial", serialToUse);
                        insertCmd.Parameters.AddWithValue("@dbGuidKey", dbGuidKey);
                        insertCmd.Parameters.AddWithValue("@dbMasterKey", dbMasterKey);
                        insertCmd.Parameters.AddWithValue("@createDate", createDate);
                        insertCmd.Parameters.AddWithValue("@UsbSeriNoHash", UsbSeriNoHash);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Db kayıt hatası:\n{ex.Message}", "HATA", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void UpsertAppLicenseInfo(string usbSerial, string appMasterKey, string rsaKey)
        {
            string createDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();

                    var checkCmd = connection.CreateCommand();
                    checkCmd.CommandText = "SELECT Id, AppMasterkey FROM Lisanslar WHERE USBSeriNo = @serial LIMIT 1;";
                    checkCmd.Parameters.AddWithValue("@serial", usbSerial);

                    int? existingId = null;
                    bool appMasterKeyEmpty = false;

                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            existingId = reader.GetInt32(0);
                            appMasterKeyEmpty = reader.IsDBNull(1) || string.IsNullOrWhiteSpace(reader.GetString(1));
                        }
                    }

                    if (existingId.HasValue && appMasterKeyEmpty)
                    {
                        var updateCmd = connection.CreateCommand();
                        updateCmd.CommandText = @"
                            UPDATE Lisanslar
                            SET AppMasterkey = @appMasterKey,
                                RSAKey = @rsaKey,
                                CreateDate = @createDate
                            WHERE Id = @id;";
                        updateCmd.Parameters.AddWithValue("@appMasterKey", appMasterKey);
                        updateCmd.Parameters.AddWithValue("@rsaKey", rsaKey);
                        updateCmd.Parameters.AddWithValue("@createDate", createDate);
                        updateCmd.Parameters.AddWithValue("@id", existingId.Value);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        var existsCmd = connection.CreateCommand();
                        existsCmd.CommandText = "SELECT COUNT(*) FROM Lisanslar WHERE USBSeriNo = @serial;";
                        existsCmd.Parameters.AddWithValue("@serial", usbSerial);
                        long count = (long)existsCmd.ExecuteScalar();

                        string serialToUse = count > 0 ? "new_" + usbSerial : usbSerial;

                        var insertCmd = connection.CreateCommand();
                        insertCmd.CommandText = @"
                            INSERT INTO Lisanslar (
                                USBSeriNo, AppMasterkey, RSAKey, CreateDate
                            )
                            VALUES (
                                @usbSerial, @appMasterKey, @rsaKey, @createDate
                            );";
                        insertCmd.Parameters.AddWithValue("@usbSerial", serialToUse);
                        insertCmd.Parameters.AddWithValue("@appMasterKey", appMasterKey);
                        insertCmd.Parameters.AddWithValue("@rsaKey", rsaKey);
                        insertCmd.Parameters.AddWithValue("@createDate", createDate);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"AppMasterKey kaydedilirken hata:\n{ex.Message}", "HATA", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static DataTable GetAllLicenses()
        {
            var table = new DataTable();

            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Lisanslar ORDER BY CreateDate DESC;";

                    using (var reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lisanslar yüklenirken hata oluştu:\n" + ex.Message,
                    "HATA", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return table;
        }
    }
}
