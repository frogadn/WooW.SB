using DevExpress.XtraEditors;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WooW.SB.Config.Helpers
{
    public class woVisualStudio
    {
        public static Process AbreVisualStudio(string commandLineArgs, bool WaitForExit)
        {

            string PathBase =
                @"C:\Program Files\Microsoft Visual Studio\2022\{0}\Common7\IDE\devenv.exe";

            string exe = string.Format(PathBase, "Preview");

            if (!File.Exists(exe))
            {
                exe = string.Format(PathBase, "Community");
                if (!File.Exists(exe))
                {
                    XtraMessageBox.Show(
                        "Visual Studio no existe en " + exe,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return null;
                }
            }

            Process processVisualStudio = new Process();
            processVisualStudio.StartInfo.FileName = exe;
            processVisualStudio.StartInfo.Arguments = commandLineArgs;
            processVisualStudio.StartInfo.ErrorDialog = true;
            processVisualStudio.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            processVisualStudio.Start();
            if (WaitForExit)
                processVisualStudio.WaitForExit();

            return processVisualStudio;

        }

    }
}
