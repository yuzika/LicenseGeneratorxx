using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using CryptoHelper; // hash için

namespace LicenseGenerator.Helpers // veya Sorgu_V1._0
{
    public static class UsbHelper
    {
        /// <summary>
        /// Sistemdeki ilk USB diskin seri numarası.
        /// Önce PNPDeviceID’den çıkarır, yoksa PhysicalMedia’dan dener.
        /// </summary>
        public static string GetFirstUsbSerialNumber()
        {
            try
            {
                using (var drives = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'"))
                {
                    foreach (ManagementObject drive in drives.Get())
                    {
                        // 1) PNPDeviceID'den çıkar
                        string pnpId = drive["PNPDeviceID"]?.ToString();
                        string extracted = ExtractSerialFromPnpId(pnpId);
                        if (IsLikelyValidSerial(extracted))
                            return NormalizeSerial(extracted);

                        // 2) PhysicalMedia.SerialNumber (fallback)
                        string deviceId = drive["DeviceID"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(deviceId))
                        {
                            string pmSerial = TryGetPhysicalMediaSerialByDeviceId(deviceId);
                            if (IsLikelyValidSerial(pmSerial))
                                return NormalizeSerial(pmSerial);
                        }
                    }
                }
            }
            catch { /* yut */ }

            return null;
        }

        /// <summary>
        /// "C:\" gibi bir sürücü harfinden USB seri numarasını getirir.
        /// </summary>
        public static string GetUsbSerialNumber(string driveLetter)
        {
            if (string.IsNullOrWhiteSpace(driveLetter)) return null;
            driveLetter = driveLetter.TrimEnd('\\');

            try
            {
                // 1) LogicalDisk -> Partition eşleşmesi
                using (var link1 = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition"))
                {
                    foreach (var rel in link1.Get())
                    {
                        string dependent = rel["Dependent"]?.ToString();   // "Win32_LogicalDisk.DeviceID=\"C:\""
                        if (dependent == null || !dependent.Contains($"\"{driveLetter}:\"")) continue;

                        string antecedent = rel["Antecedent"]?.ToString(); // "Win32_DiskPartition.DeviceID=\"Disk #3, Partition #1\""
                        if (string.IsNullOrWhiteSpace(antecedent)) continue;

                        // Partition’dan DiskIndex'i bul
                        string diskIndex = GetDiskIndexFromPartition(antecedent);
                        if (string.IsNullOrWhiteSpace(diskIndex)) continue;

                        // 2) DiskDrive(Index) -> PNP serial dene; olmazsa PhysicalMedia
                        return GetDiskSerialFromIndex(diskIndex);
                    }
                }
            }
            catch { /* yut */ }

            return null;
        }

        /// <summary>
        /// Disk Index’ten seri numarayı döndürür (önce PNP, sonra PhysicalMedia).
        /// </summary>
        private static string GetDiskSerialFromIndex(string index)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        string moIndex = mo["Index"]?.ToString();
                        if (moIndex != index) continue;

                        // 1) PNPDeviceID’den çıkar
                        string pnpId = mo["PNPDeviceID"]?.ToString();
                        string extracted = ExtractSerialFromPnpId(pnpId);
                        if (IsLikelyValidSerial(extracted))
                            return NormalizeSerial(extracted);

                        // 2) Disk’in kendi SerialNumber alanı (çoğu zaman boş/yanlış)
                        string embedded = mo["SerialNumber"]?.ToString();
                        if (IsLikelyValidSerial(embedded))
                            return NormalizeSerial(embedded);

                        // 3) PhysicalMedia’dan dene
                        string deviceId = mo["DeviceID"]?.ToString();
                        string pmSerial = TryGetPhysicalMediaSerialByDeviceId(deviceId);
                        if (IsLikelyValidSerial(pmSerial))
                            return NormalizeSerial(pmSerial);
                    }
                }
            }
            catch { /* yut */ }

            return null;
        }

        /// <summary>
        /// "D:\" gibi bir yol verilince seri numarayı getirir (GetUsbSerialNumber’a delegasyon).
        /// </summary>
        public static string GetUsbSerialNumberFromPath(string drivePath)
        {
            try
            {
                var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name.Equals(drivePath, StringComparison.OrdinalIgnoreCase));
                if (drive != null)
                    return GetUsbSerialNumber(drive.RootDirectory.FullName);
            }
            catch { }

            return null;
        }

        /// <summary>
        /// En iyi çabayla seri numara: önce "ilk USB" dene, olmazsa path üzerinden dene.
        /// </summary>
        public static string GetBestEffortUsbSerialNumber(string drivePath)
        {
            string serial = GetFirstUsbSerialNumber();
            if (IsLikelyValidSerial(serial))
                return NormalizeSerial(serial);

            serial = GetUsbSerialNumberFromPath(drivePath);
            if (IsLikelyValidSerial(serial))
                return NormalizeSerial(serial);

            return null;
        }

        /// <summary>
        /// SHA256 ile stabil seri üret (HEX).
        /// </summary>
        public static string GetStableUsbSerial(string drivePath)
        {
            string rawSerial = GetBestEffortUsbSerialNumber(drivePath);
            if (string.IsNullOrWhiteSpace(rawSerial)) return null;
            return CryptoHelper.CryptoHelper.ComputeSHA256(rawSerial);
        }

        // -------------------- Yardımcılar --------------------

        /// <summary>
        /// USBSTOR\...\{SERIAL}&0 biçiminden {SERIAL} kısmını alır.
        /// </summary>
        private static string ExtractSerialFromPnpId(string pnpId)
        {
            if (string.IsNullOrWhiteSpace(pnpId)) return null;

            // Örnek: USBSTOR\DISK&VEN_INTENSO&PROD_SPEED_LINE&REV_PMAP\90004BCB09CCCA31&0
            var parts = pnpId.Split('\\');
            if (parts.Length > 2)
            {
                var tail = parts[2];
                var serial = tail.Split('&')[0];
                return serial;
            }
            return null;
        }

        /// <summary>
        /// Win32_PhysicalMedia’dan, verilen DeviceID’ye (\\.\PHYSICALDRIVE#) göre SerialNumber okur.
        /// </summary>
        private static string TryGetPhysicalMediaSerialByDeviceId(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId)) return null;

            try
            {
                // Tag alanı, ör: "\\.\PHYSICALDRIVE4"
                string escaped = deviceId.Replace("\\", "\\\\");
                using (var mediaQuery = new ManagementObjectSearcher($"SELECT SerialNumber FROM Win32_PhysicalMedia WHERE Tag = '{escaped}'"))
                {
                    foreach (ManagementObject media in mediaQuery.Get())
                    {
                        string serial = media["SerialNumber"]?.ToString();
                        if (IsLikelyValidSerial(serial))
                            return NormalizeSerial(serial);
                    }
                }

                // Bazı sistemlerde Tag eşleşmeyebilir; o zaman ilk dolu SerialNumber’ı al.
                using (var allMedia = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia"))
                {
                    foreach (ManagementObject media in allMedia.Get())
                    {
                        string serial = media["SerialNumber"]?.ToString();
                        if (IsLikelyValidSerial(serial))
                            return NormalizeSerial(serial);
                    }
                }
            }
            catch { /* yut */ }

            return null;
        }

        /// <summary>
        /// Win32_DiskPartition.DeviceID metninden disk index’ini (string) bulur.
        /// </summary>
        private static string GetDiskIndexFromPartition(string antecedent)
        {
            // Ör: Antecedent = "Win32_DiskPartition.DeviceID=\"Disk #3, Partition #1\""
            // Buradan "3" değerini çıkaralım.
            try
            {
                int i1 = antecedent.IndexOf("Disk #", StringComparison.OrdinalIgnoreCase);
                if (i1 >= 0)
                {
                    i1 += "Disk #".Length;
                    var sb = new StringBuilder();
                    while (i1 < antecedent.Length && char.IsDigit(antecedent[i1]))
                    {
                        sb.Append(antecedent[i1]);
                        i1++;
                    }
                    return sb.ToString();
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Boş, "0", çok kısa, kontrol karakterli vb. seri numaraları eler.
        /// </summary>
        private static bool IsLikelyValidSerial(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();

            if (s == "0") return false;
            if (s.Length < 4) return false;

            // Kontrol karakteri içeriyorsa (örn. "\x03")
            if (s.Any(ch => char.IsControl(ch))) return false;

            return true;
        }

        /// <summary>
        /// Seriyi normalize eder: trim, sonda nokta sil, gereksiz boşlukları temizle, uppercase.
        /// </summary>
        private static string NormalizeSerial(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;

            s = s.Trim();

            // Bazı PhysicalMedia serileri sondaki '.' ile gelir.
            if (s.EndsWith(".", StringComparison.Ordinal)) s = s.Substring(0, s.Length - 1);

            // Görünmeyen/ASCII dışı kontrol karakterleri temizle
            s = new string(s.Where(ch => !char.IsControl(ch)).ToArray());

            // Sık karşılaşılan boşluk/alt çizgi normalizasyonu
            s = Regex.Replace(s, @"\s+", "");
            s = s.Trim('_');

            return s.ToUpperInvariant();
        }
    }
}
