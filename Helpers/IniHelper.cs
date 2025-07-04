using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LicenseGenerator.Helpers
{
    public static class IniHelper
    {
        private static string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue,
            StringBuilder retVal, int size, string filePath);

        public static void SetIniFilePath(string path)
        {
            iniFilePath = path;
        }

        public static void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, iniFilePath);
        }

        public static string Read(string section, string key, string defaultValue = "")
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, iniFilePath);
            return sb.ToString();
        }

        public static bool IniExists()
        {
            return File.Exists(iniFilePath);
        }

        public static string GetIniPath()
        {
            return iniFilePath;
        }
    }
}
