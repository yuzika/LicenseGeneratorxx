using System;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseGenerator.Helpers;
using Microsoft.Data.Sqlite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LicenseGenerator.Forms
{
    public partial class MainForm : Form
    {
        // public string OutputDirectory => txtUsbPath2.Text; // Property

        // public Func<string> GetOutputDirectory => () => txtUsbPath2.Text;
        private Stopwatch stopwatch;
        private int estimatedTotalSeconds = 60; // Varsayılan süre, sonra güncellenebilir
        private int progressIncrement = 1;
        private string selectedDbPath = string.Empty;
        private string selectedUsbPath1 = string.Empty;
        private string selectedUsbPath2 = string.Empty;
        private string selectedUsbSerial1 = string.Empty;
        private string selectedUsbSerial2 = string.Empty;
        private string _dbPath;
        private string _usbPath;
        private string _usbPath2;
        private string _usbSerial1;
        private string _usbSerial2;
        private string _licenseFilePath;
        private string _licenseFilePath2;
        private string dbKey;
        public MainForm()
        {

            //dbKey = "";
            InitializeComponent();
            this.Load += MainForm_Load;
            dgvLisanslar.CellClick += dgvLisanslar_CellClick;
            btnUpdateLisans.Click += btnUpdateLisans_Click;
            btnSelectDb.Click += btnSelectDb_Click;
            btnDbTargetPath.Click += btnDbTargetPath_Click;
            btnSelectUsb1.Click += btnSelectUsb1_Click;
            btnSelectUsb2.Click += btnSelectUsb2_Click;
            btnEncryptDb.Click += btnEncryptDb_Click;
           // btnCreateLicense.Click += btnCreateLicense_Click;
            btnTestEncryptedDb.Click += btnTestEncryptedDb_Click;
            // btnTestLicense.Click += btnTestLicense_Click;
            btnTopluTestEt.Click += btnTopluTestEt_Click;
            btnLogTemizle.Click += btnLogTemizle_Click;
            btnSecEncDb.Click += btnSecEncDb_Click;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tabControlMain.TabPages.Remove(tabPageLicense);
            //dgvLisanslar.DataSource = DatabaseHelper.GetAllLicenses();
            try
            {
                DataTable licenseTable = LicenseRepository.GetAllLicenses();
                dgvLisanslar.DataSource = licenseTable;

                dgvLisanslar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLisanslar.AutoResizeColumns();

                dgvlic.DataSource = licenseTable;

                dgvlic.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvlic.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata: " + ex.Message);
            }
        }

        private void dgvLisanslar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvLisanslar.Rows[e.RowIndex];
                txtUserName.Text = row.Cells["KullaniciAdi"].Value?.ToString();
                txtCompanyName.Text = row.Cells["FirmaAdi"].Value?.ToString();
                dtpDeliveryDate.Value = DateTime.TryParse(row.Cells["DeliveryDate"].Value?.ToString(), out var date) ? date : DateTime.Now;
                lblCreatedAt.Text = "Oluşturulma: " + row.Cells["CreateDate"].Value?.ToString();
            }
        }

        private void btnUpdateLisans_Click(object sender, EventArgs e)
        {
            if (dgvLisanslar.CurrentRow != null)
            {
                int id = int.Parse(dgvLisanslar.CurrentRow.Cells["id"].Value.ToString());
                string serial = dgvLisanslar.CurrentRow.Cells["USBSeriNo"].Value.ToString();
                string user = txtUserName.Text.Trim();
                string firm = txtCompanyName.Text.Trim();
                string teslim = dtpDeliveryDate.Value.ToString("yyyy-MM-dd");

                string message;
                bool ok = DatabaseHelper.UpdateLicenseInfo(id, serial, user, teslim, out message);
                //bool ok = DatabaseHelper.UpdateLicenseInfo(id, serial, user, firm, teslim);
                if (ok)
                {
                    MessageBox.Show("Güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvLisanslar.DataSource = DatabaseHelper.GetAllLicenses();
                }
                else
                {
                    MessageBox.Show("Güncelleme başarısız!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSelectDb_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQLite Veritabanı (*.db;*.sqlite)|*.db;*.sqlite|Tüm Dosyalar|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _dbPath = ofd.FileName;
                    lblDbPath.Text = $"Veritabanı: {_dbPath}";
                    txtDbPath.Text = ofd.FileName;
                }
            }
        }
        private void btnDbTargetPath_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Şifreli veritabanının kaydedileceği klasörü seçin";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtEncryptedDbPath.Text = folderDialog.SelectedPath;
                }
            }
        }
        private void btnSelectUsb1_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Lütfen USB sürücüsünü seçin";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string usbPath = folderDialog.SelectedPath;
                    txtUsbPath1.Text = usbPath;
                    string usbSerial = UsbHelper.GetBestEffortUsbSerialNumber(txtUsbPath1.Text);

                    // string usbSerial = UsbHelper.GetUsbSerialNumberFromPath(usbPath);
                    if (string.IsNullOrWhiteSpace(usbSerial))
                    {
                        MessageBox.Show("USB seri numarası alınamadı! Lütfen farklı bir USB veya port deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUsbSerial1.Text = "";
                        return;
                    }

                    txtUsbSerial1.Text = usbSerial;
                    LogHelper.Log($"[USB] Seçilen: {usbPath} → Seri No: {usbSerial}");
                }
            }

        }

        private void btnSelectUsb2_Click(object sender, EventArgs e)
        {
            // using (var conn = new SQLiteConnection(connStr))
            var usbDrives = DriveInfo.GetDrives()
        .Where(d => d.IsReady && d.DriveType == DriveType.Removable)
        .ToList();

            if (usbDrives.Count == 0)
            {
                MessageBox.Show("Takılı bir USB sürücü bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "USB Sürücüsünü Seçin";
                fbd.SelectedPath = usbDrives[0].RootDirectory.FullName;
                string serial = UsbHelper.GetUsbSerialNumber(fbd.SelectedPath);
                if (!string.IsNullOrEmpty(serial))
                {
                    selectedUsbPath2 = fbd.SelectedPath;
                    selectedUsbSerial2 = UsbHelper.GetUsbSerialNumberFromPath(selectedUsbPath2);
                    _usbSerial2 = serial;
                    lblUsbSerial2.Text = $"USB Serial: {serial}";

                    //selectedUsbPath1 = fbd.SelectedPath;
                    //lblUsbSerial1.Text = $"USB Serial: {serial}";
                    txtUsbPath2.Text = selectedUsbPath2;
                    txtUsbSerial2.Text = serial;

                    //lblUsbSerial2.Text = "USB Serial: " + selectedUsbSerial2;
                    //txtUsbPath2.Text = selectedUsbPath2;
                }
                else
                {
                    MessageBox.Show("Geçerli bir USB seçilmedi veya seri numarası alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void btnEncryptDb_Click(object sender, EventArgs e)
        {
            string sourceDbPath = txtDbPath.Text.Trim();
            string targetFolder = txtEncryptedDbPath.Text.Trim();
            string usbPath = txtUsbPath1.Text.Trim();
            string usbSerial = txtUsbSerial1.Text.Trim();
            string DbOrjMasterkey = txtPassword.Text.Trim(); // ✅
            string UsbSeriNoHash = UsbHelper.GetStableUsbSerial(usbSerial);

            if (!File.Exists(sourceDbPath))
            {
                MessageBox.Show("Kaynak veritabanı dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(targetFolder))
            {
                MessageBox.Show("Şifreli veritabanının kaydedileceği klasör geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(usbPath))
            {
                MessageBox.Show("USB sürücü yolu geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(usbSerial))
            {
                MessageBox.Show("USB seri numarası boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string dbGuidKey = Guid.NewGuid().ToString("N");
                string encryptedDbPath = Path.Combine(targetFolder, "encrypted.db");

                string resultMessage;
                bool success = DbEncryptor.EncryptDatabase(sourceDbPath, encryptedDbPath, dbGuidKey, out resultMessage);
                txtDbPassword.Text = dbGuidKey;
                btnCreateLicense.Enabled = true;
                button6.Enabled = true;
                txtAppMasterKey.Enabled = true;
                // ✅ DB info kayıt
                LicenseRepository.UpsertDbEncryptionInfo(usbSerial, dbGuidKey, DbOrjMasterkey, UsbSeriNoHash);

                if (!success)
                {
                    MessageBox.Show("Veritabanı şifrelenemedi: " + resultMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ Eksik olan bu satır:
                DbKeyFileHelper.SetPassword(DbOrjMasterkey);

                bool keySaved = DbKeyFileHelper.GenerateAndSaveDbKeyFile(usbPath, usbSerial, dbGuidKey, out string dbkeyFilePath, out string error);
                if (!keySaved)
                {
                    MessageBox.Show("dbkey.dat oluşturulamadı: " + error, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Veritabanı başarıyla şifrelendi ve dbkey.dat dosyası oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında beklenmeyen hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (stopwatch == null || !stopwatch.IsRunning)
                return;

            TimeSpan elapsed = stopwatch.Elapsed;
            int elapsedSeconds = (int)elapsed.TotalSeconds;

            lblElapsed.Text = $"Geçen Süre: {elapsed:mm\\:ss}";

            // Kalan süre tahmini
            int remainingSeconds = Math.Max(estimatedTotalSeconds - elapsedSeconds, 0);
            lblRemaining.Text = $"Tahmini Kalan Süre: {TimeSpan.FromSeconds(remainingSeconds):mm\\:ss}";

            // Progress tahmini (yaklaşık)
            int progress = Math.Min((elapsedSeconds * 100) / estimatedTotalSeconds, 100);
            if (progressBar1.Value < progress)
                progressBar1.Value = progress;
        }

        private string DetectUsbDrive()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    // En az 100MB boş alan kontrolü
                    if (drive.AvailableFreeSpace > 100 * 1024 * 1024)
                    {
                        return drive.RootDirectory.FullName;
                    }
                }
            }
            return null;
        }
        private string GetUsbDrivePath()
        {
            // Örnek USB tespit mantığı (gerçek projede daha detaylı olmalı)
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    return drive.RootDirectory.FullName;
                }
            }
            return null;
        }
        private bool IsDriveWritable(string drivePath)
        {

            try
            {
                string testFile = Path.Combine(drivePath, Path.GetRandomFileName());
                using (FileStream fs = File.Create(testFile, 1, FileOptions.DeleteOnClose)) { }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void btnCreateKeyAndLicense_Click(object sender, EventArgs e)
        {
            string usbPath = txtSelectedUsbPath.Text.Trim();
            string usbSerial = txtUsbSerial3.Text.Trim();
            string dbGuidKey = txtDbGuidKey.Text.Trim();

            if (string.IsNullOrWhiteSpace(usbPath) || string.IsNullOrWhiteSpace(usbSerial) || string.IsNullOrWhiteSpace(dbGuidKey))
            {
                MessageBox.Show("Lütfen USB yolu, seri no ve dbGuidKey bilgisini girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // dbkey.dat
            if (!DbKeyFileHelper.GenerateAndSaveDbKeyFile(usbPath, usbSerial, dbGuidKey, out string dbKeyPath, out string dbKeyError))
            {
                MessageBox.Show("dbkey.dat oluşturulamadı: " + dbKeyError, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // license.dat
            string licensePath = Path.Combine(usbPath, "license.dat");
           

            MessageBox.Show("Hem dbkey.dat hem license.dat başarıyla oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void btnSecEncDb_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Şifreli Veritabanı Seç";
                ofd.Filter = "SQLite Database (*.db)|*.db|Tüm Dosyalar (*.*)|*.*";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtEncryptedDbTestPath.Text = ofd.FileName;
                }
            }
        }
        private void btnTestEncryptedDb_Click(object sender, EventArgs e)
        {
            lstLogTest.Items.Clear();

            string encryptedDbPath = txtEncryptedDbTestPath.Text.Trim();
            string usbRoot = txtUsbPath1.Text.Trim();
            string usbSerial = txtUsbSerial1.Text.Trim();

            if (!File.Exists(encryptedDbPath))
            {
                lstLogTest.Items.Add("❌ Şifreli veritabanı dosyası bulunamadı.");
                return;
            }

            if (string.IsNullOrEmpty(usbRoot) || string.IsNullOrEmpty(usbSerial))
            {
                lstLogTest.Items.Add("❌ USB dizini veya seri numarası girilmemiş.");
                return;
            }

            // 1. Lisans doğrulama
            /*if (!LicenseValidator.IsLicenseValid(usbRoot, usbSerial, out string decryptedLicense, out string appMasterKey, out string dbMasterKey))
            {
                lstLogTest.Items.Add("❌ Lisans doğrulaması başarısız.");
                return;
            }*/

            // 2. dbkey.dat çözümleme
            /*if (!DbKeyFileHelper.TryResolveDbKey(usbRoot, usbSerial, dbMasterKey, out string dbGuidKey, out string dbKeyError))
            {
                lstLogTest.Items.Add("❌ dbkey.dat çözümleme başarısız: " + dbKeyError);
                return;
            }*/

            // 3. Veritabanına bağlanma denemesi
            try
            {
                SQLitePCL.Batteries_V2.Init();

                using (var conn = new SqliteConnection($"Data Source={encryptedDbPath};"))
                {
                    conn.Open();

                    /*using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"PRAGMA key = '{dbGuidKey}';";
                        cmd.ExecuteNonQuery();
                    }*/

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT count(*) FROM sqlite_master;";
                        long tableCount = (long)cmd.ExecuteScalar();
                        lstLogTest.Items.Add("✔ Veritabanına bağlantı başarılı. Tablo sayısı: " + tableCount);
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                lstLogTest.Items.Add("❌ Veritabanı bağlantı hatası: " + ex.Message);
            }
        }

        /*private void btnTestLicense_Click(object sender, EventArgs e)
        {
            // 1. Validasyonlar
            if (string.IsNullOrEmpty(txtUsbPath2.Text))
            {
                MessageBox.Show("USB yolu boş olamaz!");
                return;
            }

            // 2. Helper'ı kullanma
            bool success = LicenseFileHelper.CreateLicenseFile(
                usbSerial: txtUsbSerial2.Text,
                guid: Guid.NewGuid().ToString(),
                outputDirectory: txtUsbPath2.Text // txtUsbPath2 değerini parametre olarak geçiyoruz
            );

            // 3. Sonuç
            if (success) MessageBox.Show("Başarılı!");
            // string result = LicenseFileHelper.ValidateLicense(selectedUsbPath2,aa);
            //MessageBox.Show(result);
        }*/

        private void btnTopluTestEt_Click(object sender, EventArgs e)
        {
            lstLogTest.Items.Clear();
            lstLogTest.Items.Add("🔑 Lisans Kontrolü...");
            // string result1 = LicenseFileHelper.ValidateLicense(selectedUsbPath2,aa);
            // lstLogTest.Items.Add(result1);

            lstLogTest.Items.Add("📦 Veritabanı Şifre Kontrolü...");
            // string key = DbKeyFileHelper.ReadDbKey(selectedUsbPath1);
            // bool ok = DbEncryptor.TestDecryption(selectedDbPath, key);
            // lstLogTest.Items.Add(ok ? "Veritabanı geçerli." : "Veritabanı şifre hatalı.");
        }

        private void btnLogTemizle_Click(object sender, EventArgs e)
        {
            lstLogTest.Items.Clear();
        }

        private void btnCreateNewDbKeyFile_Click(object sender, EventArgs e)
        {
            string usbPath = txtSelectedUsbPath.Text.Trim();
            string usbSerial = txtUsbSerial3.Text.Trim();
            string dbGuidKey = txtDbGuidKey.Text.Trim();
            string DbOrjMasterkey = txtNewDbMasterkey.Text.Trim();
            string UsbSeriNoHash = UsbHelper.GetStableUsbSerial(usbSerial);

            if (string.IsNullOrWhiteSpace(usbPath) || string.IsNullOrWhiteSpace(usbSerial) || string.IsNullOrWhiteSpace(dbGuidKey))
            {
                MessageBox.Show("Lütfen USB yolu, seri no ve dbGuidKey bilgisini girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Eksik olan bu satır:
            DbKeyFileHelper.SetPassword(DbOrjMasterkey);

            if (DbKeyFileHelper.GenerateAndSaveDbKeyFile(usbPath, usbSerial, dbGuidKey, out string path, out string error))
            {
                LicenseRepository.UpsertDbEncryptionInfo(usbSerial, dbGuidKey, DbOrjMasterkey, UsbSeriNoHash);
                MessageBox.Show("dbkey.dat başarıyla oluşturuldu:\n" + path, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Hata: " + error, "dbkey.dat Oluşturulamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateNewApLicenseFile_Click(object sender, EventArgs e)
        {
            string usbPath = txtSelectedUsbPath.Text.Trim();
            string usbSerial = txtUsbSerial3.Text.Trim();
            string dbGuidKey = txtDbGuidKey.Text.Trim();
            string dbMasterKey = txtNewDbMasterkey.Text;//AppConstants.DbMasterKey;
            string appMasterKey = txtMasterKey.Text;//AppConstants.AppMasterKey;

            if (string.IsNullOrWhiteSpace(usbPath) ||
                string.IsNullOrWhiteSpace(usbSerial) ||
                string.IsNullOrWhiteSpace(dbGuidKey))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string licensePath = Path.Combine(usbPath, "license.dat");

            try
            {
                LicenseFileHelper.CreateLicenseFile(
                    usbSerialRaw: usbSerial,
                    dbGuidKey: dbGuidKey,
                    dbMasterKey: dbMasterKey,
                    resolvedAppMasterKey: appMasterKey,
                    outputPath: licensePath
                );
                string rsaPublicXml = AppConstants.RsaPrivateKeyXml;
                LicenseRepository.UpsertAppLicenseInfo(usbSerial, appMasterKey, rsaPublicXml);

                MessageBox.Show("license.dat başarıyla oluşturuldu:\n" + licensePath, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "license.dat Oluşturulamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string lisansGuid = null;
            string portableMasterKey = null;
            //string rsaPublicXml=AppConstants.RsaPrivateKeyXml;
            
        }

        private void btnGetDbPass_Click(object sender, EventArgs e)
        {
            /*string dbKeyFilePath = txtdbKeyFilePath.Text.Trim();
            string usbSerial = txtUsbSerial3.Text.Trim();

            if (string.IsNullOrWhiteSpace(dbKeyFilePath) )
            {
                MessageBox.Show("Lütfen dbkey.dat dosya yolunu ve USB seri numarasını girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(dbKeyFilePath))
            {
                MessageBox.Show("Seçilen dbkey.dat dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // USB sürücüsünden dbMasterKey'i al
            string dbMasterKey = AppConstants.DbMasterKey;
            string usbPath = Path.GetDirectoryName(dbKeyFilePath);

            if (DbKeyFileHelper.DecryptDbKeyFile(usbPath, usbSerial, dbMasterKey, out string dbGuidKey, out string error))
            {
                txtDbGuidKey.Text = dbGuidKey;
                MessageBox.Show("dbGuidKey başarıyla çözüldü ve alındı.", "Tamam", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("dbkey.dat dosyası çözülemedi: " + error, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void btnSelectDbKeyFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "dbkey.dat|dbkey.dat";
            ofd.Title = "dbkey.dat Dosyasını Seç";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtdbKeyFilePath.Text = ofd.FileName;
            }
        }

        private void btnUsbSec_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Lütfen USB sürücüsünü seçin";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string usbPath = folderDialog.SelectedPath;
                    txtSelectedUsbPath.Text = usbPath;
                    string usbSerial = UsbHelper.GetBestEffortUsbSerialNumber(txtSelectedUsbPath.Text);

                   // string usbSerial = UsbHelper.GetUsbSerialNumberFromPath(usbPath);
                    if (string.IsNullOrWhiteSpace(usbSerial))
                    {
                        MessageBox.Show("USB seri numarası alınamadı! Lütfen farklı bir USB veya port deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUsbSerial3.Text = "";
                        return;
                    }

                    txtUsbSerial3.Text = usbSerial;
                    LogHelper.Log($"[USB] Seçilen: {usbPath} → Seri No: {usbSerial}");
                }
            }
        }

        private void btnGenerateMasterkey_Click(object sender, EventArgs e)
        {
            string newKey = GenerateRandomHexKey(64); // 64 hex karakter → 256 bit
            txtMasterKey.Text = newKey;
        }

        private static string GenerateRandomHexKey(int length)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] bytes = new byte[length / 2];
            rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        private void btnGeneratedbMasterkey_Click(object sender, EventArgs e)
        {
            string newKey = GenerateRandomHexKey(64); // 64 hex karakter → 256 bit
            txtNewDbMasterkey.Text = newKey;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string newKey = GenerateRandomHexKey(64); // 64 hex karakter → 256 bit
            txtDbMasterKeyForLic.Text = newKey;
        }


        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            txtDbMasterKeyForLic.Text = txtPassword.Text.Trim();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string newKey = GenerateRandomHexKey(64); // 64 hex karakter → 256 bit
            txtPassword.Text = newKey;
        }

        private void btnCreateLicense_Click(object sender, EventArgs e)
        {
            string usbPath = txtUsbPath1.Text.Trim();
            string usbSerialRaw = txtUsbSerial1.Text.Trim();
            string dbGuidKey = txtDbPassword.Text.Trim();
            string dbMasterKey = txtPassword.Text;//AppConstants.DbMasterKey;
            string appMasterKey = txtAppMasterKey.Text;//AppConstants.AppMasterKey;

            if (string.IsNullOrWhiteSpace(usbPath) ||
                string.IsNullOrWhiteSpace(usbSerialRaw) ||
                string.IsNullOrWhiteSpace(dbGuidKey))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string licensePath = Path.Combine(usbPath, "license.dat");

            try
            {
                LicenseFileHelper.CreateLicenseFile(
                    usbSerialRaw: usbSerialRaw,
                    dbGuidKey: dbGuidKey,
                    dbMasterKey: dbMasterKey,
                    resolvedAppMasterKey: appMasterKey,
                    outputPath: licensePath
                );

                MessageBox.Show("license.dat başarıyla oluşturuldu:\n" + licensePath, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "license.dat Oluşturulamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string lisansGuid = null;
            string portableMasterKey = null;
            string rsaPublicXml = AppConstants.RsaPrivateKeyXml;
            LicenseRepository.UpsertAppLicenseInfo(usbSerialRaw, appMasterKey, rsaPublicXml);
            btnCreateLicense.Enabled = false;
            button6.Enabled = false;
            txtAppMasterKey.Enabled = false;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string newKey = GenerateRandomHexKey(64); // 64 hex karakter → 256 bit
            txtAppMasterKey.Text = newKey;
        }

        private void dgvlic_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvlic.Rows[e.RowIndex];
                txtNewDbMasterkey.Text = row.Cells["DbMasterKey"].Value?.ToString();
                txtDbGuidKey.Text = row.Cells["DbGuidKey"].Value?.ToString();
                txtMasterKey.Text = row.Cells["AppMasterkey"].Value?.ToString();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable licenseTable = LicenseRepository.GetAllLicenses();
                dgvlic.DataSource = licenseTable;

                dgvlic.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvlic.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yenilenirken hata oluştu:\n" + ex.Message,
                    "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable licenseTable = LicenseRepository.GetAllLicenses();
                dgvLisanslar.DataSource = licenseTable;

                dgvLisanslar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLisanslar.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yenilenirken hata oluştu:\n" + ex.Message,
                    "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateRSA_Click(object sender, EventArgs e)
        {

        }
    }
}
