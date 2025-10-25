using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using SQLitePCL; // SQLCipher için

namespace LicenseGenerator.Helpers
{
    public static class SQLiteHelper
    {
        private static string _dbPath;
        private static string _password;

        public static void Initialize(string iniPath)
        {
            if (!File.Exists(iniPath))
            {
                MessageBox.Show("INI dosyası bulunamadı: " + iniPath);
                return;
            }

            string iniContent = File.ReadAllText(iniPath);
            string[] lines = iniContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("dbpath=", StringComparison.OrdinalIgnoreCase))
                    _dbPath = line.Substring("dbpath=".Length).Trim();

                if (line.StartsWith("dbkey=", StringComparison.OrdinalIgnoreCase))
                    _password = line.Substring("dbkey=".Length).Trim();
            }
        }

        public static SqliteConnection GetConnection()
        {
            Batteries_V2.Init(); // SQLCipher için gerekli

            if (string.IsNullOrWhiteSpace(_dbPath))
                throw new ArgumentNullException("Veritabanı yolu boş olamaz");

            string connStr = $"Data Source={_dbPath};";

            if (!string.IsNullOrEmpty(_password))
                connStr += $"Password={_password};";

            var conn = new SqliteConnection(connStr);
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteQuery(string query, params SqliteParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (var conn = GetConnection())
            using (var cmd = new SqliteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (var reader = cmd.ExecuteReader()) // DataAdapter yerine
                {
                    dt.Load(reader); // Direkt DataTable'a yükleme
                }
            }
            return dt;
        }

        public static int ExecuteNonQuery(string query, params SqliteParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqliteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string query, params SqliteParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqliteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    return conn.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}