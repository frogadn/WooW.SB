using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using WooW.Core;

namespace WooW.SB.Dialogs
{
    public partial class fmComboSelect : DevExpress.XtraEditors.XtraForm
    {
        public string SelectedItem
        {
            get { return txtCombo.EditValue.ToSafeString(); }
        }

        public string[] Items
        {
            set { txtCombo.Properties.Items.AddRange(value); }
        }

        public TextEditStyles EditStyle
        {
            set { txtCombo.Properties.TextEditStyle = value; }
        }

        public fmComboSelect()
        {
            InitializeComponent();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (SelectedItem.IsNullOrStringEmpty())
            {
                XtraMessageBox.Show(
                    "Seleccione un elemento",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
