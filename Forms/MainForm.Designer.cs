namespace LicenseGenerator.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageDbEncrypt = new System.Windows.Forms.TabPage();
            this.txtUsbPath1 = new System.Windows.Forms.TextBox();
            this.txtDbPath = new System.Windows.Forms.TextBox();
            this.btnTestEncryptedDb = new System.Windows.Forms.Button();
            this.lblStatusDb = new System.Windows.Forms.Label();
            this.lblUsbSerial1 = new System.Windows.Forms.Label();
            this.lblDbPath = new System.Windows.Forms.Label();
            this.btnEncryptDb = new System.Windows.Forms.Button();
            this.btnSelectUsb1 = new System.Windows.Forms.Button();
            this.btnSelectDb = new System.Windows.Forms.Button();
            this.tabPageLicense = new System.Windows.Forms.TabPage();
            this.txtUsbPath2 = new System.Windows.Forms.TextBox();
            this.btnTestLicense = new System.Windows.Forms.Button();
            this.lblLicenseStatus = new System.Windows.Forms.Label();
            this.lblUsbSerial2 = new System.Windows.Forms.Label();
            this.btnCreateLicense = new System.Windows.Forms.Button();
            this.btnSelectUsb2 = new System.Windows.Forms.Button();
            this.tabPageRecords = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dgvLisanslar = new System.Windows.Forms.DataGridView();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.dtpDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.lblCreatedAt = new System.Windows.Forms.Label();
            this.btnUpdateLisans = new System.Windows.Forms.Button();
            this.tabPageTest = new System.Windows.Forms.TabPage();
            this.lstLogTest = new System.Windows.Forms.ListBox();
            this.btnTopluTestEt = new System.Windows.Forms.Button();
            this.btnLogTemizle = new System.Windows.Forms.Button();
            this.txtUsbSerial1 = new System.Windows.Forms.TextBox();
            this.txtUsbSerial2 = new System.Windows.Forms.TextBox();
            this.tabControlMain.SuspendLayout();
            this.tabPageDbEncrypt.SuspendLayout();
            this.tabPageLicense.SuspendLayout();
            this.tabPageRecords.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLisanslar)).BeginInit();
            this.tabPageTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageDbEncrypt);
            this.tabControlMain.Controls.Add(this.tabPageLicense);
            this.tabControlMain.Controls.Add(this.tabPageRecords);
            this.tabControlMain.Controls.Add(this.tabPageTest);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(635, 303);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageDbEncrypt
            // 
            this.tabPageDbEncrypt.Controls.Add(this.txtUsbSerial1);
            this.tabPageDbEncrypt.Controls.Add(this.txtUsbPath1);
            this.tabPageDbEncrypt.Controls.Add(this.txtDbPath);
            this.tabPageDbEncrypt.Controls.Add(this.btnTestEncryptedDb);
            this.tabPageDbEncrypt.Controls.Add(this.lblStatusDb);
            this.tabPageDbEncrypt.Controls.Add(this.lblUsbSerial1);
            this.tabPageDbEncrypt.Controls.Add(this.lblDbPath);
            this.tabPageDbEncrypt.Controls.Add(this.btnEncryptDb);
            this.tabPageDbEncrypt.Controls.Add(this.btnSelectUsb1);
            this.tabPageDbEncrypt.Controls.Add(this.btnSelectDb);
            this.tabPageDbEncrypt.Location = new System.Drawing.Point(4, 22);
            this.tabPageDbEncrypt.Name = "tabPageDbEncrypt";
            this.tabPageDbEncrypt.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDbEncrypt.Size = new System.Drawing.Size(627, 277);
            this.tabPageDbEncrypt.TabIndex = 0;
            this.tabPageDbEncrypt.Text = "Veritabanı Şifreleme";
            this.tabPageDbEncrypt.UseVisualStyleBackColor = true;
            // 
            // txtUsbPath1
            // 
            this.txtUsbPath1.Location = new System.Drawing.Point(333, 56);
            this.txtUsbPath1.Name = "txtUsbPath1";
            this.txtUsbPath1.Size = new System.Drawing.Size(27, 20);
            this.txtUsbPath1.TabIndex = 8;
            // 
            // txtDbPath
            // 
            this.txtDbPath.Location = new System.Drawing.Point(333, 28);
            this.txtDbPath.Name = "txtDbPath";
            this.txtDbPath.Size = new System.Drawing.Size(202, 20);
            this.txtDbPath.TabIndex = 7;
            // 
            // btnTestEncryptedDb
            // 
            this.btnTestEncryptedDb.Location = new System.Drawing.Point(185, 111);
            this.btnTestEncryptedDb.Name = "btnTestEncryptedDb";
            this.btnTestEncryptedDb.Size = new System.Drawing.Size(142, 23);
            this.btnTestEncryptedDb.TabIndex = 0;
            this.btnTestEncryptedDb.Text = "Şifreli Veritabanını Test Et";
            // 
            // lblStatusDb
            // 
            this.lblStatusDb.AutoSize = true;
            this.lblStatusDb.Location = new System.Drawing.Point(18, 18);
            this.lblStatusDb.Name = "lblStatusDb";
            this.lblStatusDb.Size = new System.Drawing.Size(68, 13);
            this.lblStatusDb.TabIndex = 1;
            this.lblStatusDb.Text = "Durum: Hazır";
            // 
            // lblUsbSerial1
            // 
            this.lblUsbSerial1.AutoSize = true;
            this.lblUsbSerial1.Location = new System.Drawing.Point(18, 56);
            this.lblUsbSerial1.Name = "lblUsbSerial1";
            this.lblUsbSerial1.Size = new System.Drawing.Size(89, 13);
            this.lblUsbSerial1.TabIndex = 2;
            this.lblUsbSerial1.Text = "USB Serial: [Yok]";
            // 
            // lblDbPath
            // 
            this.lblDbPath.AutoSize = true;
            this.lblDbPath.Location = new System.Drawing.Point(18, 151);
            this.lblDbPath.Name = "lblDbPath";
            this.lblDbPath.Size = new System.Drawing.Size(111, 13);
            this.lblDbPath.TabIndex = 3;
            this.lblDbPath.Text = "Veritabanı: [Seçilmedi]";
            // 
            // btnEncryptDb
            // 
            this.btnEncryptDb.Location = new System.Drawing.Point(185, 83);
            this.btnEncryptDb.Name = "btnEncryptDb";
            this.btnEncryptDb.Size = new System.Drawing.Size(142, 22);
            this.btnEncryptDb.TabIndex = 4;
            this.btnEncryptDb.Text = "Veritabanını Şifrele";
            // 
            // btnSelectUsb1
            // 
            this.btnSelectUsb1.Location = new System.Drawing.Point(185, 56);
            this.btnSelectUsb1.Name = "btnSelectUsb1";
            this.btnSelectUsb1.Size = new System.Drawing.Size(142, 21);
            this.btnSelectUsb1.TabIndex = 5;
            this.btnSelectUsb1.Text = "USB Seç";
            // 
            // btnSelectDb
            // 
            this.btnSelectDb.Location = new System.Drawing.Point(185, 31);
            this.btnSelectDb.Name = "btnSelectDb";
            this.btnSelectDb.Size = new System.Drawing.Size(142, 20);
            this.btnSelectDb.TabIndex = 6;
            this.btnSelectDb.Text = "Veritabanı Seç";
            // 
            // tabPageLicense
            // 
            this.tabPageLicense.Controls.Add(this.txtUsbSerial2);
            this.tabPageLicense.Controls.Add(this.txtUsbPath2);
            this.tabPageLicense.Controls.Add(this.btnTestLicense);
            this.tabPageLicense.Controls.Add(this.lblLicenseStatus);
            this.tabPageLicense.Controls.Add(this.lblUsbSerial2);
            this.tabPageLicense.Controls.Add(this.btnCreateLicense);
            this.tabPageLicense.Controls.Add(this.btnSelectUsb2);
            this.tabPageLicense.Location = new System.Drawing.Point(4, 22);
            this.tabPageLicense.Name = "tabPageLicense";
            this.tabPageLicense.Size = new System.Drawing.Size(627, 277);
            this.tabPageLicense.TabIndex = 1;
            this.tabPageLicense.Text = "Uygulama Lisanslama";
            // 
            // txtUsbPath2
            // 
            this.txtUsbPath2.Location = new System.Drawing.Point(311, 30);
            this.txtUsbPath2.Name = "txtUsbPath2";
            this.txtUsbPath2.Size = new System.Drawing.Size(55, 20);
            this.txtUsbPath2.TabIndex = 5;
            // 
            // btnTestLicense
            // 
            this.btnTestLicense.Location = new System.Drawing.Point(184, 84);
            this.btnTestLicense.Name = "btnTestLicense";
            this.btnTestLicense.Size = new System.Drawing.Size(121, 20);
            this.btnTestLicense.TabIndex = 0;
            this.btnTestLicense.Text = "Lisansı Test Et";
            // 
            // lblLicenseStatus
            // 
            this.lblLicenseStatus.Location = new System.Drawing.Point(19, 30);
            this.lblLicenseStatus.Name = "lblLicenseStatus";
            this.lblLicenseStatus.Size = new System.Drawing.Size(100, 23);
            this.lblLicenseStatus.TabIndex = 1;
            this.lblLicenseStatus.Text = "Durum: Hazır";
            // 
            // lblUsbSerial2
            // 
            this.lblUsbSerial2.Location = new System.Drawing.Point(19, 53);
            this.lblUsbSerial2.Name = "lblUsbSerial2";
            this.lblUsbSerial2.Size = new System.Drawing.Size(159, 17);
            this.lblUsbSerial2.TabIndex = 2;
            this.lblUsbSerial2.Text = "USB Serial: [Yok]";
            // 
            // btnCreateLicense
            // 
            this.btnCreateLicense.Location = new System.Drawing.Point(184, 56);
            this.btnCreateLicense.Name = "btnCreateLicense";
            this.btnCreateLicense.Size = new System.Drawing.Size(121, 22);
            this.btnCreateLicense.TabIndex = 3;
            this.btnCreateLicense.Text = "Lisans Oluştur";
            // 
            // btnSelectUsb2
            // 
            this.btnSelectUsb2.Location = new System.Drawing.Point(184, 28);
            this.btnSelectUsb2.Name = "btnSelectUsb2";
            this.btnSelectUsb2.Size = new System.Drawing.Size(121, 22);
            this.btnSelectUsb2.TabIndex = 4;
            this.btnSelectUsb2.Text = "USB Seç";
            // 
            // tabPageRecords
            // 
            this.tabPageRecords.Controls.Add(this.textBox1);
            this.tabPageRecords.Controls.Add(this.dgvLisanslar);
            this.tabPageRecords.Controls.Add(this.txtUserName);
            this.tabPageRecords.Controls.Add(this.txtCompanyName);
            this.tabPageRecords.Controls.Add(this.dtpDeliveryDate);
            this.tabPageRecords.Controls.Add(this.lblCreatedAt);
            this.tabPageRecords.Controls.Add(this.btnUpdateLisans);
            this.tabPageRecords.Location = new System.Drawing.Point(4, 22);
            this.tabPageRecords.Name = "tabPageRecords";
            this.tabPageRecords.Size = new System.Drawing.Size(627, 277);
            this.tabPageRecords.TabIndex = 2;
            this.tabPageRecords.Text = "Kayıtlar / Güncelleme";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(223, 217);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(86, 20);
            this.textBox1.TabIndex = 6;
            // 
            // dgvLisanslar
            // 
            this.dgvLisanslar.ColumnHeadersHeight = 69;
            this.dgvLisanslar.Location = new System.Drawing.Point(30, 30);
            this.dgvLisanslar.Name = "dgvLisanslar";
            this.dgvLisanslar.RowHeadersWidth = 123;
            this.dgvLisanslar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLisanslar.Size = new System.Drawing.Size(178, 207);
            this.dgvLisanslar.TabIndex = 0;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(235, 30);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(157, 20);
            this.txtUserName.TabIndex = 1;
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Location = new System.Drawing.Point(235, 66);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(157, 20);
            this.txtCompanyName.TabIndex = 2;
            // 
            // dtpDeliveryDate
            // 
            this.dtpDeliveryDate.Location = new System.Drawing.Point(235, 106);
            this.dtpDeliveryDate.Name = "dtpDeliveryDate";
            this.dtpDeliveryDate.Size = new System.Drawing.Size(157, 20);
            this.dtpDeliveryDate.TabIndex = 3;
            // 
            // lblCreatedAt
            // 
            this.lblCreatedAt.Location = new System.Drawing.Point(600, 180);
            this.lblCreatedAt.Name = "lblCreatedAt";
            this.lblCreatedAt.Size = new System.Drawing.Size(100, 23);
            this.lblCreatedAt.TabIndex = 4;
            // 
            // btnUpdateLisans
            // 
            this.btnUpdateLisans.Location = new System.Drawing.Point(235, 149);
            this.btnUpdateLisans.Name = "btnUpdateLisans";
            this.btnUpdateLisans.Size = new System.Drawing.Size(74, 31);
            this.btnUpdateLisans.TabIndex = 5;
            this.btnUpdateLisans.Text = "Kaydet";
            // 
            // tabPageTest
            // 
            this.tabPageTest.Controls.Add(this.lstLogTest);
            this.tabPageTest.Controls.Add(this.btnTopluTestEt);
            this.tabPageTest.Controls.Add(this.btnLogTemizle);
            this.tabPageTest.Location = new System.Drawing.Point(4, 22);
            this.tabPageTest.Name = "tabPageTest";
            this.tabPageTest.Size = new System.Drawing.Size(627, 277);
            this.tabPageTest.TabIndex = 3;
            this.tabPageTest.Text = "Test Paneli";
            // 
            // lstLogTest
            // 
            this.lstLogTest.Location = new System.Drawing.Point(30, 30);
            this.lstLogTest.Name = "lstLogTest";
            this.lstLogTest.Size = new System.Drawing.Size(295, 212);
            this.lstLogTest.TabIndex = 0;
            // 
            // btnTopluTestEt
            // 
            this.btnTopluTestEt.Location = new System.Drawing.Point(30, 248);
            this.btnTopluTestEt.Name = "btnTopluTestEt";
            this.btnTopluTestEt.Size = new System.Drawing.Size(115, 23);
            this.btnTopluTestEt.TabIndex = 1;
            this.btnTopluTestEt.Text = "Toplu Test Et";
            // 
            // btnLogTemizle
            // 
            this.btnLogTemizle.Location = new System.Drawing.Point(151, 248);
            this.btnLogTemizle.Name = "btnLogTemizle";
            this.btnLogTemizle.Size = new System.Drawing.Size(174, 23);
            this.btnLogTemizle.TabIndex = 2;
            this.btnLogTemizle.Text = "Logları Temizle";
            // 
            // txtUsbSerial1
            // 
            this.txtUsbSerial1.Location = new System.Drawing.Point(366, 57);
            this.txtUsbSerial1.Name = "txtUsbSerial1";
            this.txtUsbSerial1.Size = new System.Drawing.Size(145, 20);
            this.txtUsbSerial1.TabIndex = 9;
            // 
            // txtUsbSerial2
            // 
            this.txtUsbSerial2.Location = new System.Drawing.Point(372, 30);
            this.txtUsbSerial2.Name = "txtUsbSerial2";
            this.txtUsbSerial2.Size = new System.Drawing.Size(126, 20);
            this.txtUsbSerial2.TabIndex = 6;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(635, 303);
            this.Controls.Add(this.tabControlMain);
            this.Name = "MainForm";
            this.Text = "License Generator v1.0";
            this.tabControlMain.ResumeLayout(false);
            this.tabPageDbEncrypt.ResumeLayout(false);
            this.tabPageDbEncrypt.PerformLayout();
            this.tabPageLicense.ResumeLayout(false);
            this.tabPageLicense.PerformLayout();
            this.tabPageRecords.ResumeLayout(false);
            this.tabPageRecords.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLisanslar)).EndInit();
            this.tabPageTest.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageDbEncrypt;
        private System.Windows.Forms.TabPage tabPageLicense;
        private System.Windows.Forms.TabPage tabPageRecords;
        private System.Windows.Forms.TabPage tabPageTest;

        private System.Windows.Forms.Button btnSelectDb;
        private System.Windows.Forms.Button btnSelectUsb1;
        private System.Windows.Forms.Button btnEncryptDb;
        private System.Windows.Forms.Label lblDbPath;
        private System.Windows.Forms.Label lblUsbSerial1;
        private System.Windows.Forms.Label lblStatusDb;
        private System.Windows.Forms.Button btnTestEncryptedDb;

        private System.Windows.Forms.Button btnSelectUsb2;
        private System.Windows.Forms.Button btnCreateLicense;
        private System.Windows.Forms.Label lblUsbSerial2;
        private System.Windows.Forms.Label lblLicenseStatus;
        private System.Windows.Forms.Button btnTestLicense;

        private System.Windows.Forms.DataGridView dgvLisanslar;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtCompanyName;
        private System.Windows.Forms.DateTimePicker dtpDeliveryDate;
        private System.Windows.Forms.Label lblCreatedAt;
        private System.Windows.Forms.Button btnUpdateLisans;

        private System.Windows.Forms.ListBox lstLogTest;
        private System.Windows.Forms.Button btnTopluTestEt;
        private System.Windows.Forms.Button btnLogTemizle;
        private System.Windows.Forms.TextBox txtUsbPath2;
        private System.Windows.Forms.TextBox txtUsbPath1;
        private System.Windows.Forms.TextBox txtDbPath;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtUsbSerial1;
        private System.Windows.Forms.TextBox txtUsbSerial2;
    }
}
