using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;

namespace WooW.SB.CodeEditor.CodeDialogs
{
    public partial class fmNewAttribute : DevExpress.XtraEditors.XtraForm
    {
        #region Atributos públicos

        public event EventHandler<(string type, string name, string value)> DataReadyEvt;
        public event EventHandler CancelEvt;

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

        private (string type, string name, string value) _newAttribute;

        #endregion Atributos privados

        #region Constructores

        public fmNewAttribute()
        {
            InitializeComponent();

            cmbTypes.DataSource = _typesCol;
        }

        #endregion Constructores

        #region Eventos de los controles

        private void btnAcept_Click(object sender, EventArgs e)
        {
            if (txtAttribute.Text.IsNullOrStringEmpty())
            {
                MessageBox.Show("Complete el nombre del atributo");
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
                    case "string":
                        value = $@"""{_txtValue.Text}""";
                        break;
                    case "char":
                        value = $@"'{_txtValue.Text}'";
                        break;
                    case "DateTime":
                        value =
                            $@"new DateTime({_dteValue.Value.Year}, {_dteValue.Value.Month}, {_dteValue.Value.Day})";
                        break;
                    case "int":
                        value = _speValue.Value.ToString();
                        break;
                    case "double":
                        value = _speValue.Value.ToString();
                        break;
                    default:
                        value = _txtValue.Text;
                        break;
                }

                _newAttribute = (cmbTypes.SelectedItem.ToString(), txtAttribute.Text, value);
                DataReadyEvt?.Invoke(this, _newAttribute);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelEvt?.Invoke(this, EventArgs.Empty);
            this.Close();
        }

        #endregion Eventos de los controles

        #region Selección del tipo de dato

        private TextBox _txtValue = null;
        private System.Windows.Forms.ComboBox _cmbValue = null;
        private DateTimePicker _dteValue = null;
        private SpinEdit _speValue = null;

        private void cmbTypes_SelectedValueChanged(object sender, EventArgs e)
        {
            ChargeControl();
        }

        private void ChargeControl(string value = "")
        {
            List<string> dataComboCol = new List<string>() { "Seleccione(opcional)..." };

            switch (cmbTypes.SelectedItem.ToString())
            {
                case "bool":
                    pnlControlValue.Controls.Clear();
                    _cmbValue = new System.Windows.Forms.ComboBox();
                    _cmbValue.Parent = pnlControlValue;
                    _cmbValue.Dock = DockStyle.Fill;
                    dataComboCol.Add(@"true");
                    dataComboCol.Add(@"false");
                    _cmbValue.DataSource = dataComboCol;
                    if (!value.IsNullOrStringEmpty())
                        _cmbValue.SelectedItem = value;
                    break;
                case "DateTime":
                    pnlControlValue.Controls.Clear();
                    _dteValue = new DateTimePicker();
                    _dteValue.Parent = pnlControlValue;
                    _dteValue.Dock = DockStyle.Fill;
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
