using System;
using System.Windows.Forms;
using LicenseGenerator.Forms; // MainForm'ün namespace'i

namespace LicenseGenerator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlcipher());
            SQLitePCL.Batteries_V2.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // Tam nitelikli ad kullan
        }
    }
}