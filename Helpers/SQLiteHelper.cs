using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

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

        public static SQLiteConnection GetConnection()
        {
            if (string.IsNullOrWhiteSpace(_dbPath) || !File.Exists(_dbPath))
                throw new FileNotFoundException("Veritabanı yolu geçersiz veya dosya bulunamadı: " + _dbPath);

            string connStr = $"Data Source={_dbPath};";

            if (!string.IsNullOrEmpty(_password))
                connStr += $"Password={_password};";

            var conn = new SQLiteConnection(connStr);
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteQuery(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }
    }
}
