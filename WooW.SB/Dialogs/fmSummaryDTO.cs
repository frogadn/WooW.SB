using System;
using System.Data;
using System.Linq;
using WooW.SB.Config;

namespace WooW.SB.Dialogs
{
    public partial class fmSummaryDTO : DevExpress.XtraEditors.XtraForm
    {
        private Modelo Model;
        private ModeloDiagrama Diagram;

        public fmSummaryDTO(Modelo _Modelo, ModeloDiagrama _Diagrama)
        {
            InitializeComponent();

            Model = _Modelo;
            Diagram = _Diagrama;
        }

        private void fmMostrarDTO_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Columna", typeof(string));

            foreach (var trans in Diagram.Transiciones)
            {
                dt.Columns.Add(trans.Id, typeof(bool));
            }

            grdDTO.DataSource = dt;

            foreach (var trans in Diagram.Transiciones)
            {
                dt.Columns[trans.Id].Caption =
                    $"{trans.Id} de {trans.EstadoInicial} a {trans.EstadoFinal}";
            }

            foreach (var col in Model.Columnas.OrderBy(j => j.Orden))
            {
                DataRow dr = dt.NewRow();
                dr["Columna"] = col.Id;

                foreach (var trans in Diagram.Transiciones)
                {
                    if (trans.DTO.Columnas.Where(f => f == col.Id).FirstOrDefault() != null)
                        dr[trans.Id] = true;
                    else
                        dr[trans.Id] = false;
                }

                dt.Rows.Add(dr);
            }

            //lstDTO.Items.Clear();
            //foreach (var col in model.Columnas.OrderBy(j => j.Orden))
            //    lstDTO.Items.Add(col.Id);
        }
    }
}
