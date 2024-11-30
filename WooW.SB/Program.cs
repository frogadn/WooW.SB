using System;
using System.Windows.Forms;

namespace WooW.SB
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            string Licensee = "FROG S.A. de C.V.";
            string LicenseKey = "WIN241-JP43J-0P98V-DLRVY-PFGG"; //  "WIN231 -8VY9J-TV9L1-UKRYY-GFGG";
            ActiproSoftware.Products.ActiproLicenseManager.RegisterLicense(Licensee, LicenseKey);

            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fmMain(args));
        }
    }
}
