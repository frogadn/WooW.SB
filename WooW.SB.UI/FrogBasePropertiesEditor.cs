using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace WooW.SB.UI
{
    public partial class FrogBasePropertiesEditor : XtraForm
    {
        public bool ReadOnly
        {
            get
            {
                return btnAceptar.Enabled;
            }
            set
            {
                btnAceptar.Enabled = !value;
            }
        }

        public Action<DialogResult> OnExit { get; set; }

        public FrogBasePropertiesEditor()
        {
            InitializeComponent();
        }

        protected virtual bool bValidar()
        {
            return true;
        }

        protected void PerformAceptar()
        {
            btnAceptar.PerformClick();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.bValidar())
            {
                this.DialogResult = DialogResult.OK;

                if (OnExit != null)
                    OnExit(DialogResult.OK);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            if (OnExit != null)
                OnExit(DialogResult.Cancel);
        }

        private void FrogBasePropertiesEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnCancelar.PerformClick();
        }

        private void FrogBasePropertiesEditor_Load(object sender, EventArgs e)
        {
            btnCancelar.Text += " <" + Keys.Escape.ToString() + ">";
        }

        private void FrogBasePropertiesEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (OnExit != null)
                    OnExit(DialogResult.Abort);
            }
        }
    }
}