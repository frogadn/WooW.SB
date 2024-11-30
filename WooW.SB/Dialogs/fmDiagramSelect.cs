using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;

namespace WooW.SB.Dialogs
{
    public partial class fmDiagramSelect : DevExpress.XtraEditors.XtraForm
    {
        public string DiagramName => txtDiagrama.EditValue.ToSafeString();

        public fmDiagramSelect(List<string> diagrams)
        {
            InitializeComponent();

            LoadData(diagrams);
        }

        private void LoadData(List<string> diagrams)
        {
            txtDiagrama.Properties.Items.Clear();
            txtDiagrama.Properties.Items.AddRange(diagrams);
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (txtDiagrama.EditValue.IsNullOrStringEmpty())
                return;

            if (
                XtraMessageBox.Show(
                    $"Copiar el diagrama de {txtDiagrama.EditValue.ToSafeString()} ?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            this.DialogResult = DialogResult.OK;
        }
    }
}
