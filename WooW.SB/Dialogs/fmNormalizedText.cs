using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;

namespace WooW.SB.Dialogs
{
    public partial class fmNormalizedText : DevExpress.XtraEditors.XtraForm
    {
        private string _ExpresionRegular;
        public string ExpresionRegular
        {
            get => _ExpresionRegular;
            set => _ExpresionRegular = value;
        }

        public string NormalizedText
        {
            get { return txtNormalizedText.EditValue.ToSafeString(); }
            set { txtNormalizedText.Text = value; }
        }

        public fmNormalizedText()
        {
            InitializeComponent();

            _ExpresionRegular = @"[A-Z][a-zA-Z0-9]*";
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (NormalizedText.IsNullOrStringEmpty())
            {
                XtraMessageBox.Show(
                    "Registre un texto",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (!_ExpresionRegular.IsNullOrStringEmpty())
            {
                Regex regex = new Regex(_ExpresionRegular);
                if (regex.Match(NormalizedText).Value.ToString() != NormalizedText)
                {
                    XtraMessageBox.Show(
                        "Nombre del archivo debe comenzar con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
