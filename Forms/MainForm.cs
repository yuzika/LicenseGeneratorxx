using System;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LicenseGenerator.Helpers;

namespace LicenseGenerator.Forms
{
    public partial class MainForm : Form
    {
       // public string OutputDirectory => txtUsbPath2.Text; // Property

       // public Func<string> GetOutputDirectory => () => txtUsbPath2.Text;

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
            btnSelectUsb1.Click += btnSelectUsb1_Click;
            btnSelectUsb2.Click += btnSelectUsb2_Click;
            btnEncryptDb.Click += btnEncryptDb_Click;
            btnCreateLicense.Click += btnCreateLicense_Click;
            btnTestEncryptedDb.Click += btnTestEncryptedDb_Click;
            btnTestLicense.Click += btnTestLicense_Click;
            btnTopluTestEt.Click += btnTopluTestEt_Click;
            btnLogTemizle.Click += btnLogTemizle_Click;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dgvLisanslar.DataSource = DatabaseHelper.GetAllLicenses();
        }

        private void dgvLisanslar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvLisanslar.Rows[e.RowIndex];
                txtUserName.Text = row.Cells["user_name"].Value?.ToString();
                txtCompanyName.Text = row.Cells["company_name"].Value?.ToString();
                dtpDeliveryDate.Value = DateTime.TryParse(row.Cells["delivery_date"].Value?.ToString(), out var date) ? date : DateTime.Now;
                lblCreatedAt.Text = "Oluşturulma: " + row.Cells["created_at"].Value?.ToString();
            }
        }

        private void btnUpdateLisans_Click(object sender, EventArgs e)
        {
            if (dgvLisanslar.CurrentRow != null)
            {
                int id = int.Parse(dgvLisanslar.CurrentRow.Cells["id"].Value.ToString());
                string serial = dgvLisanslar.CurrentRow.Cells["serial_no"].Value.ToString();
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

        private void btnSelectUsb1_Click(object sender, EventArgs e)
        {
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

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    //selectedUsbPath = fbd.SelectedPath;
                    //txtUsbPath.Text = selectedUsbPath;
                    //lblStatus.Text = "USB sürücüsü seçildi.";
                    string serial = UsbHelper.GetUsbSerialNumber(fbd.SelectedPath);
                    if (!string.IsNullOrEmpty(serial))
                    {
                        _usbSerial1 = serial;
                        selectedUsbPath1= fbd.SelectedPath;
                        lblUsbSerial1.Text = $"USB Serial: {serial}";
                        txtUsbPath1.Text = selectedUsbPath1;
                        txtUsbSerial1.Text = serial;
                        
                    }
                    else
                    {
                        MessageBox.Show("Geçerli bir USB seçilmedi veya seri numarası alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            _dbPath = txtDbPath.Text;
            _usbPath=txtUsbPath1.Text;
            try
            {
                if (string.IsNullOrEmpty(_dbPath) || string.IsNullOrEmpty(_usbPath)) // _usbSerial1 yerine _usbDrivePath
                {
                    MessageBox.Show("Lütfen veritabanı ve USB sürücüsünü seçin",
                                  "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // USB'nin yazılabilir olduğunu kontrol et
                if (!Directory.Exists(_usbPath) || !IsDriveWritable(_usbPath))
                {
                    MessageBox.Show("USB sürücüsü yazılabilir değil veya bağlı değil",
                                  "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string password = Guid.NewGuid().ToString("N");
                string encryptedPath = Path.Combine(
                    Path.GetDirectoryName(_dbPath),
                    "encrypted_" + Path.GetFileName(_dbPath));

                bool result = DbEncryptor.EncryptDatabase(_dbPath, encryptedPath, password);

                if (result)
                {
                    // Anahtarı USB'ye kaydet
                    bool keySaved = DbKeyFileHelper.SaveDbKey(password, _usbPath);

                    if (keySaved)
                    {
                        lblStatusDb.Text = "Durum: Şifreleme ve anahtar kaydı başarılı";
                        MessageBox.Show($"Veritabanı başarıyla şifrelendi.\nAnahtar USB'ye kaydedildi: {_usbPath}",
                                      "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblStatusDb.Text = "Durum: Anahtar kaydı başarısız";
                        MessageBox.Show("Şifreleme başarılı ancak anahtar USB'ye kaydedilemedi",
                                      "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    lblStatusDb.Text = "Durum: Şifreleme başarısız";
                    MessageBox.Show("Şifreleme işlemi başarısız oldu",
                                  "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log($"Kritik hata: {ex}");
                MessageBox.Show($"Beklenmeyen hata: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsDriveWritable(string drivePath)
        {
            try
            {
                string testFile = Path.Combine(drivePath, "test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void btnCreateLicense_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(selectedUsbSerial2))
            {
                MessageBox.Show("USB seçilmelidir.");
                return;
            }

            //string guid = Guid.NewGuid().ToString();
            //string encrypted = LicenseFileHelper.CreateLicenseFile(selectedUsbPath2, selectedUsbSerial2, guid).ToString();
            //lblLicenseStatus.Text = "Durum: Lisans oluşturuldu.";
            bool success = LicenseFileHelper.CreateLicenseFile(
        usbSerial: selectedUsbSerial2,
        guid: Guid.NewGuid().ToString(),
        outputDirectory: txtUsbPath2.Text
    );

            if (success)
            {
                MessageBox.Show("Lisans dosyası başarıyla oluşturuldu!");
            }
        }

        private void btnTestEncryptedDb_Click(object sender, EventArgs e)
        {
            string dbPath = txtDbPath.Text;
            string password = txtUsbSerial1.Text;

            if (!File.Exists(dbPath))
            {
                MessageBox.Show("Veritabanı dosyası bulunamadı");
                return;
            }

            string result;
            bool success = DbEncryptor.TestDecryption(dbPath, password, out result);

            MessageBox.Show(result, success ? "Başarılı" : "Hata",
                MessageBoxButtons.OK,
                success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        private void btnTestLicense_Click(object sender, EventArgs e)
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
        }

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
    }
}
