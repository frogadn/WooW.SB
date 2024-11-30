using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;

namespace WooW.SB.CodeEditor.CodeDialogs
{
    public partial class fmNewMethod : DevExpress.XtraEditors.XtraForm
    {
        private List<string> _typesCol = new List<string>()
        {
            "void",
            "string",
            "int",
            "char",
            "bool",
            "double",
            "DateTime",
        };

        public event EventHandler<(string methodType, string methodName)> MethodReadyEvt;
        public event EventHandler CancelEvt;
        public event EventHandler<(string methodType, string methodName)> AddParamsEvt;

        private string _methodType = string.Empty;

        private string _methodName = string.Empty;

        /// <summary>
        /// Constructor principal de la clase que inicializa el componente
        /// e inicializa el combo con los tipos de dato de retorno
        /// </summary>
        [SupportedOSPlatform("windows")]
        public fmNewMethod()
        {
            InitializeComponent();

            cmbTypes.DataSource = _typesCol;
        }

        [SupportedOSPlatform("windows")]
        private void btnAcept_Click(object sender, EventArgs e)
        {
            _methodType = cmbTypes.SelectedItem.ToString();
            _methodName = txtName.Text;

            if (_methodName.IsNullOrStringEmpty() || _methodName.Contains(" "))
            {
                XtraMessageBox.Show(
                    text: "Ingrese un nombre valido para el método",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
            else
            {
                MethodReadyEvt?.Invoke(this, (_methodType, _methodName));
                this.Close();
            }
        }

        [SupportedOSPlatform("windows")]
        private void btnAddParams_Click(object sender, EventArgs e)
        {
            _methodType = cmbTypes.SelectedItem.ToString();
            _methodName = txtName.Text;

            if (_methodName.IsNullOrStringEmpty() || _methodName.Contains(" "))
            {
                XtraMessageBox.Show(
                    text: "Ingrese un nombre valido",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
            else
            {
                AddParamsEvt?.Invoke(this, (_methodType, _methodName));
                this.Close();
            }
        }

        [SupportedOSPlatform("windows")]
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelEvt?.Invoke(this, new EventArgs());
            this.Close();
        }
    }
}
