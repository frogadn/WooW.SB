using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using WooW.Core;

namespace WooW.SB.Config
{
    public partial class fmElementNameDialog : DevExpress.XtraEditors.XtraForm
    {
        public string Nombre
        {
            get { return txtNombre.EditValue.ToString(); }
        }

        public fmElementNameDialog()
        {
            InitializeComponent();
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (txtNombre.EditValue.IsNullOrStringEmpty())
            {
                XtraMessageBox.Show("Debe introducir un nombre para el nuevo estado");
                txtNombre.Focus();
            }
            else
                this.DialogResult = DialogResult.OK;
        }
    }
}