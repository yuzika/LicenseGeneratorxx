using System;
using System.IO;
using System.Linq;
using System.Management;

namespace LicenseGenerator.Helpers
{
    public static class UsbHelper
    {
        public static string GetUsbSerialNumber(string driveLetter)
        {
            if (string.IsNullOrWhiteSpace(driveLetter)) return null;
            driveLetter = driveLetter.TrimEnd('\\');

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition"))
                {
                    foreach (var queryObj in searcher.Get())
                    {
                        string[] split = queryObj["Dependent"].ToString().Split(',');
                        if (split.Length > 0 && split[0].Contains(driveLetter))
                        {
                            string[] parts = queryObj["Antecedent"].ToString().Split(',');
                            string diskIndex = parts[0].Split('#')[1].Replace("\"", "");
                            return GetDiskSerialFromIndex(diskIndex);
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        private static string GetDiskSerialFromIndex(string index)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        if (mo["Index"].ToString() == index)
                            return mo["SerialNumber"]?.ToString()?.Trim();
                    }
                }
            }
            catch { }
            return null;
        }
        public static string GetUsbSerialNumberFromPath(string drivePath)
        {
            var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name == drivePath);
            return drive != null ? GetUsbSerialNumber(drive.RootDirectory.FullName) : null;
        }
    }
}
