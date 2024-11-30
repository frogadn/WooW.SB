using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorDialogs
{
    public partial class WoNewIntegralTest : DevExpress.XtraEditors.XtraForm
    {
        #region singleton

        /// <summary>
        /// Instancia singleton con las configuraciones del proyecto seleccionado.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion singleton

        public Action<string> NewProjectAdded { get; set; }

        public WoNewIntegralTest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Crea el nuevo proyecto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAddProyectSettings_Click(object sender, EventArgs e)
        {
            string proyectName = txtNameProyect.Text;

            string pathSettingFiles = $@"{_project.DirLayOuts}\IntegralGenerations";
            List<string> files = WoDirectory.ReadDirectoryFiles(pathSettingFiles, onlyNames: true);

            if (files.Contains(proyectName))
            {
                XtraMessageBox.Show(
                    text: "Ya existe un proyecto con ese nombre",
                    caption: "Error",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
                return;
            }

            string pathNewProyect = $@"{pathSettingFiles}\{proyectName}.json";
            WoDirectory.CreateFile(pathNewProyect);

            NewProjectAdded.Invoke(proyectName);

            this.Close();
        }
    }
}
