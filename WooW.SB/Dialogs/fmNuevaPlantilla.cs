using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs
{
    public partial class fmNuevaPlantilla : DevExpress.XtraEditors.XtraForm
    {
        private Proyecto proyecto;

        private string plantilla;
        public string Plantilla
        {
            get => plantilla;
        }

        public fmNuevaPlantilla()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (txtPlantilla.EditValue.ToSafeString().IsNullOrStringEmpty())
                return;

            string Archivo =
                $"{proyecto.DirPlantillasReportes}\\{txtPlantilla.EditValue.ToString()}.repx";

            if (File.Exists(Archivo))
            {
                XtraMessageBox.Show(
                    "Plantilla ya existe",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            plantilla = txtPlantilla.EditValue.ToString();

            this.DialogResult = DialogResult.OK;
        }
    }
}
