using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.SB.Config;

namespace WooW.SB.Dialogs.CompareIntegralTest
{
    public partial class fmCompareIntegralTest : DevExpress.XtraEditors.XtraForm
    {
        private string archivoDestino;
        private string archivoFuente;

        public PruebaIntegral PruebaDestino
        {
            get => pruebaDestino;
            set => pruebaDestino = value;
        }

        private PruebaIntegral pruebaDestino = null;
        private PruebaIntegral pruebaFuente = null;

        public fmCompareIntegralTest(string _archivoActual, string _archivoComp)
        {
            InitializeComponent();

            archivoDestino = _archivoActual;
            archivoFuente = _archivoComp;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void fmCompareIntegralTest_Load(object sender, EventArgs e)
        {
            grpActual.Text = "Prueba Integral Destino: " + archivoDestino;
            grpComp.Text = "Prueba Integral Fuente: " + archivoFuente;

            string contenidoDestino = File.ReadAllText(archivoDestino);
            string contenidoFuente = File.ReadAllText(archivoFuente);

            pruebaDestino = PruebaIntegral.FromJson(contenidoDestino);
            pruebaFuente = PruebaIntegral.FromJson(contenidoFuente);

            LoadData();
        }

        private void LoadData()
        {
            objIntegralTestDestino.CargarPruebaIntegrarl(
                archivoDestino,
                archivoFuente,
                pruebaDestino,
                pruebaFuente
            );
            objIntegralTestFuente.CargarPruebaIntegrarl(
                archivoFuente,
                archivoDestino,
                pruebaFuente,
                pruebaDestino
            );
        }

        private void buCopiarRenglon_Click(object sender, EventArgs e)
        {
            int Orden = objIntegralTestFuente.GetSelectRow();

            if (Orden == -1)
            {
                XtraMessageBox.Show(
                    "No se ha seleccionado un renglón para copiar",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            var renglonDestino = pruebaDestino
                .Pruebas.Where(x => x.Orden == Orden)
                .FirstOrDefault();
            var renglonFuente = pruebaFuente.Pruebas.Where(x => x.Orden == Orden).FirstOrDefault();

            if (renglonFuente == null)
                return;

            if (renglonDestino != null)
            {
                XtraMessageBox.Show(
                    $"El renglón {Orden} existe en destino",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            this.pruebaDestino.Pruebas.Add((PruebaUnitaria)renglonFuente.Clone());

            LoadData();

            buAceptar.Enabled = true;
        }

        private void buIgualarRenglon_Click(object sender, EventArgs e)
        {
            int Orden = objIntegralTestFuente.GetSelectRow();

            if (Orden == -1)
            {
                XtraMessageBox.Show(
                    "No se ha seleccionado un renglón para copiar",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            var renglonActual = pruebaDestino.Pruebas.Where(x => x.Orden == Orden).FirstOrDefault();
            var renglonComp = pruebaFuente.Pruebas.Where(x => x.Orden == Orden).FirstOrDefault();

            if (renglonComp == null)
            {
                XtraMessageBox.Show(
                    $"El renglón {Orden} existe en destino",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (renglonActual == null)
            {
                XtraMessageBox.Show(
                    $"El renglón {Orden} existe en fuente",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            this.pruebaDestino.Pruebas.Remove((PruebaUnitaria)renglonActual);
            this.pruebaDestino.Pruebas.Add((PruebaUnitaria)renglonComp.Clone());

            LoadData();

            buAceptar.Enabled = true;
        }

        private void buCopiarPropiedades_Click(object sender, EventArgs e)
        {
            pruebaDestino.WorkItem = pruebaFuente.WorkItem;
            pruebaDestino.Descripcion = pruebaFuente.Descripcion;
            pruebaDestino.ProcesoId = pruebaFuente.ProcesoId;

            LoadData();

            buAceptar.Enabled = true;
        }
    }
}
