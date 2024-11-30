using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;

namespace WooW.SB.Dialogs
{
    public partial class fmModelRequestNew : DevExpress.XtraEditors.XtraForm
    {
        private Proyecto proyecto;

        public Modelo model { get; set; }
        public string Response
        {
            get => response;
        }
        private string response;
        public bool Coleccion
        {
            get => coleccion;
        }
        private bool coleccion;

        public bool BackGround
        {
            get => backGround;
        }
        private bool backGround;

        public fmModelRequestNew(Modelo _model)
        {
            InitializeComponent();
            proyecto = Proyecto.getInstance();
            model = _model;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (txtResponse.EditValue == null)
            {
                XtraMessageBox.Show(
                    "Seleccione un Model Response",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            response = txtResponse.EditValue.ToString();
            coleccion = optColeccion.Checked;
            backGround = optBackGround.Checked;

            if ((model.SubTipoModelo == WoSubTypeModel.BackGroudTask) && (!backGround))
            {
                XtraMessageBox.Show(
                    $"EL subtipo de modelo es BackGroudTask seleccione Ejecuta en BackGroud",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if ((model.SubTipoModelo != WoSubTypeModel.BackGroudTask) && (backGround))
            {
                XtraMessageBox.Show(
                    $"EL subtipo de modelo no es BackGroudTask no seleccione Ejecuta en BackGroud",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            if ((model.SubTipoModelo == WoSubTypeModel.Report) && (!coleccion))
            {
                XtraMessageBox.Show(
                    $"EL subtipo de modelo es Report seleccione regresa una coleccion",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            this.DialogResult = DialogResult.OK;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void fmModelRequestNew_Load(object sender, EventArgs e)
        {
            txtResponse.Properties.Items.Clear();

            var proyectoConPaquetes = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

            txtResponse.Properties.Items.Add("WoResponseGeneric");

            foreach (
                var Modelo in proyectoConPaquetes.ModeloCol.Modelos.Where(f =>
                    (f.TipoModelo != WoTypeModel.Interface)
                    && (
                        (f.TipoModelo != WoTypeModel.Class)
                        && (f.SubTipoModelo != WoSubTypeModel.Static)
                    )
                )
            )
                txtResponse.Properties.Items.Add(Modelo.Id);
        }
    }
}
