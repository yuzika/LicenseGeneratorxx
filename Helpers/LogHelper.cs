using System;
using System.IO;

namespace LicenseGenerator.Helpers
{
    public static class LogHelper
    {
        private static string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugLog.txt");

        public static void Log(string message)
        {
            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            File.AppendAllText(logFilePath, log + Environment.NewLine);
        }

        public static void Clear()
        {
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);
        }
    }
}
