using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ServiceStack;
using WooW.SB.Config;

namespace WooW.SB.Forms
{
    public partial class fmCompareModel : DevExpress.XtraEditors.XtraForm
    {
        public Modelo modeloDestinoCambiado
        {
            get => modeloDestino;
        }

        private Proyecto proyectoDestino;
        private Proyecto proyectoFuente;
        private Modelo modeloDestino;
        private Modelo modeloFuente;

        public fmCompareModel()
        {
            InitializeComponent();
        }

        public void ModelSetter(
            Proyecto _proyectoDestino,
            Proyecto _proyectoFuente,
            Modelo _modeloDestino,
            Modelo _modeloFuente
        )
        {
            this.proyectoDestino = _proyectoDestino;
            this.proyectoFuente = _proyectoFuente;
            this.modeloDestino = _modeloDestino.Clone(proyectoDestino);
            this.modeloFuente = _modeloFuente.Clone(proyectoFuente);
        }

        private void fmCompareModel_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            objModelDestino.ModelSetter(
                $"Proyecto Destino: {proyectoDestino.ArchivoDeProyecto}",
                proyectoDestino,
                proyectoFuente,
                modeloDestino,
                modeloFuente
            );
            objModelFuente.ModelSetter(
                $"Proyecto Fuente: {proyectoFuente.ArchivoDeProyecto}",
                proyectoFuente,
                proyectoDestino,
                modeloFuente,
                modeloDestino
            );
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buCopiarRenglon_Click(object sender, EventArgs e)
        {
            string Id = objModelFuente.GetSelectRow();

            if (string.IsNullOrEmpty(Id))
            {
                XtraMessageBox.Show(
                    "No se ha seleccionado un renglón para copiar",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            var colDestino = modeloDestino.Columnas.Where(x => x.Id == Id).FirstOrDefault();
            var colFuente = modeloFuente.Columnas.Where(x => x.Id == Id).FirstOrDefault();

            if (colFuente == null)
                return;

            if (colDestino != null)
            {
                XtraMessageBox.Show(
                    $"El renglón {Id} existe en destino",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            modeloDestino.Columnas.Add(colFuente.Clone(proyectoDestino));
            LoadData();

            buAceptar.Enabled = true;
        }

        private void buIgualarRenglon_Click(object sender, EventArgs e)
        {
            string Id = objModelFuente.GetSelectRow();

            if (string.IsNullOrEmpty(Id))
            {
                XtraMessageBox.Show(
                    "No se ha seleccionado un renglón para copiar",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            var colDestino = modeloDestino.Columnas.Where(x => x.Id == Id).FirstOrDefault();
            var colFuente = modeloFuente.Columnas.Where(x => x.Id == Id).FirstOrDefault();

            if (colFuente == null)
                return;

            if (colDestino == null)
            {
                XtraMessageBox.Show(
                    $"El renglón {Id} no existe en destino",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (colDestino.ToJson() == colFuente.ToJson())
            {
                XtraMessageBox.Show(
                    $"Los renglones {Id} son iguales",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            modeloDestino.Columnas.Remove(colDestino);
            modeloDestino.Columnas.Add(colFuente.Clone(proyectoDestino));
            LoadData();

            buAceptar.Enabled = true;
        }

        private void buCopiarPropiedades_Click(object sender, EventArgs e)
        {
            modeloDestino.TipoModelo = modeloFuente.TipoModelo;
            modeloDestino.SubTipoModelo = modeloFuente.SubTipoModelo;
            modeloDestino.ProcesoId = modeloFuente.ProcesoId;
            modeloDestino.EtiquetaId = modeloFuente.EtiquetaId;
            modeloDestino.OrdenDeCreacion = modeloFuente.OrdenDeCreacion;
            modeloDestino.Id = modeloFuente.Id;
            modeloDestino.Legacy = modeloFuente.Legacy;
            modeloDestino.Interface1 = modeloFuente.Interface1;
            modeloDestino.Interface2 = modeloFuente.Interface2;
            modeloDestino.Interface3 = modeloFuente.Interface3;
            modeloDestino.Apps = modeloFuente.Apps.ToList();
            modeloDestino.ScriptVistaRoles = modeloFuente.ScriptVistaRoles.Clone();

            LoadData();

            buAceptar.Enabled = true;
        }

        private void buBorrarRenglon_Click(object sender, EventArgs e)
        {
            string Id = objModelDestino.GetSelectRow();

            if (string.IsNullOrEmpty(Id))
            {
                XtraMessageBox.Show(
                    "No se ha seleccionado un renglón para borrar",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            var colDestino = modeloDestino.Columnas.Where(x => x.Id == Id).FirstOrDefault();

            if (colDestino == null)
            {
                XtraMessageBox.Show(
                    $"El renglón {Id} no existe",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            modeloDestino.Columnas.Remove(colDestino);
            LoadData();

            buAceptar.Enabled = true;
        }

        private void buIgualarOrden_Click(object sender, EventArgs e)
        {
            foreach (var columna in modeloFuente.Columnas)
            {
                var colDestino = modeloDestino
                    .Columnas.Where(x => x.Id == columna.Id)
                    .FirstOrDefault();

                if (colDestino == null)
                    continue;

                colDestino.Orden = columna.Orden;
            }

            LoadData();

            buAceptar.Enabled = true;
        }
    }
}
