using System;
using System.Windows.Forms;
using WooW.SB.Helpers;

namespace WooW.SB.Dialogs
{
    public partial class fmConfiguracion : DevExpress.XtraEditors.XtraForm
    {
        private WooWConfigParams ConfigParams;

        public fmConfiguracion(string KeyName)
        {
            InitializeComponent();
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigParams.GrabaValores();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void fmConfiguracion_Load(object sender, EventArgs e)
        {
            ConfigParams = WooWConfigParams.getInstance();
            propertyGridControl1.SelectedObject = ConfigParams;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
