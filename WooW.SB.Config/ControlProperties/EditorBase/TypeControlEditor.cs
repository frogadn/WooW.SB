using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using WooW.SB.Config.ControlProperties.Class;

namespace WooW.SB.Config.ControlProperties.Editors
{
    public delegate bool Validar();

    public partial class TypeControlEditor<T> : DevExpress.XtraEditors.XtraForm, ITypeControlEditor
    {
        private T model;

        public string Properties
        {
            get { return JsonConvert.SerializeObject(model); }
            set { model = JsonConvert.DeserializeObject<T>(value); }
        }

        private Validar validar = null;

        public DialogResult ShowEditor()
        {
            return this.ShowDialog();
        }

        public TypeControlEditor()
        {
            InitializeComponent();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            if (
                XtraMessageBox.Show(
                    "Descartar los cambios?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) == DialogResult.Yes
            )
                this.DialogResult = DialogResult.Cancel;

            return;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            // ToDo meter delegado para validar
            if (validar != null)
                if (!validar())
                    return;

            this.DialogResult = DialogResult.OK;
        }
    }
}