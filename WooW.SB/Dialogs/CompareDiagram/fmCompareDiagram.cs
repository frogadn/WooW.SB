using System;
using System.Windows.Forms;
using WooW.SB.Config;

namespace WooW.SB.Dialogs.CompareDiagram
{
    public partial class fmCompareDiagram : DevExpress.XtraEditors.XtraForm
    {
        public Modelo modeloFuenteCambiado
        {
            get => modeloFuente;
        }

        private Proyecto proyectoFuente;
        private Proyecto proyectoDestino;
        private Modelo modeloFuente;
        private Modelo modeloDestino;

        public fmCompareDiagram()
        {
            InitializeComponent();
        }

        public void ModelSetter(
            Proyecto _proyectoFuente,
            Proyecto _proyectoDestino,
            Modelo _modeloFuente,
            Modelo _modeloDestino
        )
        {
            this.proyectoFuente = _proyectoFuente;
            this.proyectoDestino = _proyectoDestino;
            this.modeloFuente = _modeloFuente.Clone(proyectoFuente);
            this.modeloDestino = _modeloDestino.Clone(proyectoDestino);
        }

        private void fmCompareDiagram_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            grpDestino.Text = $"Proyecto Destino: {proyectoDestino.ArchivoDeProyecto}";
            grpFuente.Text = $"Proyecto Fuente: {proyectoFuente.ArchivoDeProyecto}";

            //woDiagram1.MostraSoloDiagrama();
            //woDiagram1.OcultaReglas();
            woDiagram1.New();
            woDiagram1.currDiagrama = modeloFuente.Diagrama;
            woDiagram1.AsignaDTO(modeloFuente);
            woDiagram1.CreaDiagrama();
            woDiagram1.Enabled = true;

            //woDiagram2.MostraSoloDiagrama();
            //woDiagram2.OcultaReglas();
            woDiagram2.New();
            woDiagram2.currDiagrama = modeloDestino.Diagrama;
            woDiagram2.AsignaDTO(modeloDestino);
            woDiagram2.CreaDiagrama();
            woDiagram2.Enabled = true;

            buCopiarDiagrama.Enabled = (
                modeloFuente.Diagrama.ToJson() != modeloDestino.Diagrama.ToJson()
            );
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buCopiarDiagrama_Click(object sender, EventArgs e)
        {
            modeloFuente.Diagrama = modeloDestino.Diagrama.Clone();
            LoadData();

            buAceptar.Enabled = true;
        }
    }
}
