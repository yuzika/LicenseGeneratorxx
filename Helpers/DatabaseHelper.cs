using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using SQLitePCL; // SQLCipher için

namespace LicenseGenerator.Helpers
{
    public static class DatabaseHelper
    {
        public static string DbPath { get; set; }
        public static string DbSifre { get; set; } // SQLCipher şifreleme için

        public static DataTable GetAllLicenses()
        {
            var tablo = new DataTable();
            if (!File.Exists(DbPath)) return tablo;

            // SQLCipher başlatma
            Batteries_V2.Init();

            using (var baglanti = new SqliteConnection($"Data Source={DbPath};Password={DbSifre}"))
            {
                baglanti.Open();

                // SQLiteDataAdapter yerine direk okuma
                using (var komut = new SqliteCommand("SELECT * FROM lisanslar", baglanti))
                using (var okuyucu = komut.ExecuteReader())
                {
                    tablo.Load(okuyucu); // Daha verimli yöntem
                }
            }
            return tablo;
        }

        public static bool UpdateLicenseInfo(int id, string kullanici, string firma, string tarih, out string mesaj)
        {
            try
            {
                // SQLCipher başlatma
                Batteries_V2.Init();

                using (var baglanti = new SqliteConnection($"Data Source={DbPath};Password={DbSifre}"))
                {
                    baglanti.Open();
                    string sql = "UPDATE lisanslar SET user=@user, company=@company, date=@date WHERE id=@id";

                    using (var komut = new SqliteCommand(sql, baglanti))
                    {
                        // Daha güvenli parametre ekleme
                        komut.Parameters.Add(new SqliteParameter("@user", kullanici));
                        komut.Parameters.Add(new SqliteParameter("@company", firma));
                        komut.Parameters.Add(new SqliteParameter("@date", tarih));
                        komut.Parameters.Add(new SqliteParameter("@id", id));

                        int etkilenenSatir = komut.ExecuteNonQuery();
                        mesaj = etkilenenSatir > 0 ? "Başarıyla güncellendi" : "Değişiklik yapılmadı";
                        return etkilenenSatir > 0;
                    }
                }
            }
            catch (Exception hata)
            {
                mesaj = $"Hata oluştu: {hata.Message}";
                return false;
            }
        }

        public static bool VeritabaniOlustur()
        {
            try
            {
                Batteries_V2.Init();

                using (var baglanti = new SqliteConnection($"Data Source={DbPath};Password={DbSifre}"))
                {
                    baglanti.Open();

                    var komut = baglanti.CreateCommand();
                    komut.CommandText = @"
                    CREATE TABLE IF NOT EXISTS lisanslar (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        user TEXT NOT NULL,
                        company TEXT NOT NULL,
                        date TEXT NOT NULL,
                        usb_serial TEXT UNIQUE
                    )";

                    komut.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}