using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ServiceStack.OrmLite;
using WooW.Core;

namespace WooW.SB.CodeEditor.CodeDialogs
{
    public partial class fmNewMethodParams : DevExpress.XtraEditors.XtraForm
    {
        #region Atributos públicos

        public event EventHandler<List<(string type, string name, string value)>> ParamsReadyEvt;
        public event EventHandler CancelEvt;
        public event EventHandler ContinueEvt;

        #endregion Atributos públicos

        #region Atributos privados

        private List<string> _typesCol = new List<string>()
        {
            "string",
            "int",
            "char",
            "bool",
            "double",
            "DateTime",
        };

        private List<(string type, string name, string value)> _paramsCol =
            new List<(string type, string name, string value)>();

        #endregion Atributos privados

        #region Constructores

        /// <summary>
        /// Constructor principal de la clase que inicializa el componente y
        /// el combo con los tipos de datos
        /// </summary>
        [SupportedOSPlatform("windows")]
        public fmNewMethodParams()
        {
            InitializeComponent();

            cmbTypes.DataSource = _typesCol;
        }

        #endregion Constructores

        #region Gestión de parámetros

        /// <summary>
        /// Agregamos a la lista de parámetros el nuevo parámetro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAdd_Click(object sender, EventArgs e)
        {
            switch (cmbTypes.SelectedItem.ToString())
            {
                case "char":
                    if (_txtValue.Text.Count() > 1)
                    {
                        XtraMessageBox.Show(
                            text: "El tipo char solo puede recibir un carácter",
                            caption: "Error",
                            icon: MessageBoxIcon.Error,
                            buttons: MessageBoxButtons.OK
                        );
                        return;
                    }
                    break;
                case "int":
                    if (_speValue.Value.ToString().Contains("."))
                    {
                        XtraMessageBox.Show(
                            text: "El tipo int solo puede recibir números enteros",
                            caption: "Error",
                            icon: MessageBoxIcon.Error,
                            buttons: MessageBoxButtons.OK
                        );
                        return;
                    }
                    break;
            }

            var findRepeatParam = _paramsCol
                .Where(data => data.name == txtParametro.Text)
                .FirstOrDefault();

            if (findRepeatParam.name != null)
            {
                MessageBox.Show("Ya existe un parametro con ese nombre");
                return;
            }

            var parameter = GetParamView();

            if (!parameter.paramView.IsNullOrStringEmpty())
            {
                lbcParams.Items.Add(parameter.paramView);

                _paramsCol.Add(parameter.paramTuple);

                txtParametro.Text = string.Empty;
                _txtValue?.Clear();
                if (!_cmbValue.IsNull())
                    _cmbValue.SelectedItem = "Seleccione(opcional)...";
            }
        }

        [SupportedOSPlatform("windows")]
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var parameter = GetParamView();

            if (!parameter.paramView.IsNullOrStringEmpty())
            {
                lbcParams.Items.Remove(parameter.paramView);

                _paramsCol.Remove(parameter.paramTuple);

                txtParametro.Text = string.Empty;
                _txtValue?.Clear();
                if (!_cmbValue.IsNull())
                    _cmbValue.SelectedItem = "Seleccione(opcional)...";
            }
        }

        [SupportedOSPlatform("windows")]
        private (
            string paramView,
            (string type, string name, string value) paramTuple
        ) GetParamView()
        {
            string paramView = string.Empty;
            dynamic paramTuple = (string.Empty, string.Empty, string.Empty);

            if (txtParametro.Text.IsNullOrStringEmpty())
            {
                MessageBox.Show("Ingrese un nombre de parámetro valido", "Alerta");
            }
            else
            {
                string value = string.Empty;

                switch (cmbTypes.SelectedItem.ToString())
                {
                    case "bool":
                        value =
                            (_cmbValue.SelectedItem.ToString() == "Seleccione(opcional)...")
                                ? ""
                                : _cmbValue.SelectedItem.ToString();
                        break;
                    case "DateTime":
                        value = string.Empty;
                        break;
                    case "int":
                        value = _speValue.Value.ToString();
                        break;
                    case "double":
                        value = _speValue.Value.ToString();
                        break;
                    default:
                        value = _txtValue.Text.Replace("\"", "");
                        break;
                }

                paramTuple = (cmbTypes.SelectedItem.ToString(), txtParametro.Text, value);

                paramView =
                    (value.IsNullOrStringEmpty())
                        ? $@"{cmbTypes.SelectedItem.ToString()} {txtParametro.Text}"
                        : $@"{cmbTypes.SelectedItem.ToString()} {txtParametro.Text} = {value}";
            }

            return (paramView, paramTuple);
        }

        [SupportedOSPlatform("windows")]
        private void OrderParams()
        {
            var auxParamsCol = _paramsCol.OrderBy(data => !data.value.IsNullOrStringEmpty());
            _paramsCol = auxParamsCol.ToList();
        }

        #endregion Gestión de parámetros

        #region Eventos de los controles

        [SupportedOSPlatform("windows")]
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (_paramsCol.Count == 0)
            {
                MessageBox.Show("Ingrese algún parámetro para continuar", "Alerta");
            }
            else
            {
                OrderParams();
                ParamsReadyEvt?.Invoke(this, _paramsCol);
                this.Close();
            }
        }

        [SupportedOSPlatform("windows")]
        private void btnContinue_Click(object sender, EventArgs e)
        {
            ContinueEvt.Invoke(this, EventArgs.Empty);
            this.Close();
        }

        [SupportedOSPlatform("windows")]
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelEvt.Invoke(this, EventArgs.Empty);
            this.Close();
        }

        [SupportedOSPlatform("windows")]
        private void lbcParams_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!lbcParams.SelectedItem.IsNull())
            {
                var selectedItemCol = lbcParams.SelectedItem.ToString().Split(' ');
                cmbTypes.SelectedItem = selectedItemCol[0];
                txtParametro.Text = selectedItemCol[1];
                if (selectedItemCol.Length > 3)
                    ChargeControl(selectedItemCol[3]);
            }
        }

        #endregion Eventos de los controles

        #region Selección del tipo de dato

        private TextBox _txtValue = null;
        private System.Windows.Forms.ComboBox _cmbValue = null;
        private SpinEdit _speValue = null;

        [SupportedOSPlatform("windows")]
        private void cmbTypes_SelectedValueChanged(object sender, EventArgs e)
        {
            ChargeControl();
        }

        /// <summary>
        /// Limpiamos el contenedor del control para asignar un valor al parámetro
        /// y asignamos un control en función del tipo de dato que se aya seleccionado
        /// </summary>
        /// <param name="value"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeControl(string value = "")
        {
            List<string> dataCombo = new List<string>() { "Seleccione(opcional)..." };

            switch (cmbTypes.SelectedItem.ToString())
            {
                case "bool":
                    pnlControlValue.Controls.Clear();
                    _cmbValue = new System.Windows.Forms.ComboBox();
                    _cmbValue.Parent = pnlControlValue;
                    _cmbValue.Dock = DockStyle.Fill;
                    dataCombo.Add(@"true");
                    dataCombo.Add(@"false");
                    _cmbValue.DataSource = dataCombo;
                    if (!value.IsNullOrStringEmpty())
                        _cmbValue.SelectedItem = value;
                    break;
                case "DateTime":
                    pnlControlValue.Controls.Clear();
                    break;
                case "int":
                    pnlControlValue.Controls.Clear();
                    _speValue = new SpinEdit();
                    _speValue.Parent = pnlControlValue;
                    _speValue.Dock = DockStyle.Fill;
                    if (!value.IsNullOrStringEmpty())
                        _speValue.Value = 0;
                    break;
                case "double":
                    pnlControlValue.Controls.Clear();
                    _speValue = new SpinEdit();
                    _speValue.Parent = pnlControlValue;
                    _speValue.Dock = DockStyle.Fill;
                    if (!value.IsNullOrStringEmpty())
                        _speValue.Value = 0;
                    break;
                default:
                    pnlControlValue.Controls.Clear();
                    _txtValue = new TextBox();
                    _txtValue.Parent = pnlControlValue;
                    _txtValue.Dock = DockStyle.Fill;
                    if (!value.IsNullOrStringEmpty())
                        _txtValue.Text = value;
                    break;
            }
        }

        #endregion Selección del tipo de dato
    }
}
