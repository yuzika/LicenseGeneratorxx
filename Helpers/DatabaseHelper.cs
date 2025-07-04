using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace LicenseGenerator.Helpers
{
    public static class DatabaseHelper
    {
        public static string DbPath { get; set; }

        public static DataTable GetAllLicenses()
        {
            var dt = new DataTable();
            if (!File.Exists(DbPath)) return dt;

            using (var conn = new SQLiteConnection($"Data Source={DbPath};"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM lisanslar", conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public static bool UpdateLicenseInfo(int id, string user, string company, string date, out string message)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={DbPath};"))
                {
                    conn.Open();
                    string sql = "UPDATE lisanslar SET user=@user, company=@company, date=@date WHERE id=@id";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@company", company);
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                message = "Güncellendi";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
