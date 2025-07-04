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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // Tam nitelikli ad kullan
        }
    }
}