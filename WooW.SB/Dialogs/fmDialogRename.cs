using System;
using System.Windows.Forms;
using WooW.Core;

namespace WooW.SB.Dialogs
{
    public partial class fmDialogRename : DevExpress.XtraEditors.XtraForm
    {
        private string modelo;
        public string Modelo
        {
            get { return modelo; }
            set { modelo = value; }
        }

        public fmDialogRename()
        {
            InitializeComponent();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (txtModelo.EditValue.ToSafeString().IsNullOrStringTrimEmpty())
                return;

            if (txtModelo.EditValue.ToString() == txtAnterior.EditValue.ToString())
                return;

            modelo = txtModelo.EditValue.ToSafeString();

            this.DialogResult = DialogResult.OK;
        }

        private void fmModelRename_Load(object sender, EventArgs e)
        {
            txtAnterior.EditValue = modelo;
            txtModelo.EditValue = modelo;
        }
    }
}
