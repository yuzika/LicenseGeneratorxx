// LogHelper.cs
using System;
using System.Diagnostics;
using System.IO;

namespace LicenseGenerator.Helpers
{
    public static class LogHelper
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugLog.txt");

        public static void Log(string message)
        {
            try
            {
                string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(LogFilePath, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Log yazılamadı: " + ex.Message);
            }
        }

        public static void ClearLog()
        {
            try
            {
                if (File.Exists(LogFilePath))
                    File.WriteAllText(LogFilePath, string.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Log temizlenemedi: " + ex.Message);
            }
        }

        public static string ReadLog()
        {
            try
            {
                if (File.Exists(LogFilePath))
                    return File.ReadAllText(LogFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Log okunamadı: " + ex.Message);
            }
            return string.Empty;
        }
        public static void LogError(string title, Exception ex)
        {
            try
            {
                string full = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {title}\n{ex}\n";
                File.AppendAllText(LogFilePath, full);
            }
            catch { /* log hatası yutulur */ }
        }
    }
}
