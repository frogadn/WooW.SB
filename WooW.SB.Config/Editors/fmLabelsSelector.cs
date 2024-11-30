using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WooW.Core;

namespace WooW.SB.Config
{
    public partial class fmLabelsSelector : XtraForm
    {
        private Proyecto proyecto;

        private bool bCambio = false;

        public string Id { get; set; }

        public fmLabelsSelector()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();
        }

        private void EtiquetaSimple_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {

            DataTable dt = new DataTable();

            dt.Columns.Add(@"Nuevo", typeof(bool));
            dt.Columns.Add(@"Id", typeof(string));
            dt.Columns.Add(@"Texto", typeof(string));

            foreach (var Etiqueta in proyecto.EtiquetaCol.Etiquetas)
            {
                DataRow drRow = dt.NewRow();
                drRow["Nuevo"] = false;
                drRow["Id"] = Etiqueta.Id;
                var EtiquetaIdioma = Etiqueta.Idiomas.Where(f => f.IdiomaId == proyecto.esMX).FirstOrDefault();
                if (EtiquetaIdioma != null)
                    drRow["Texto"] = EtiquetaIdioma.Texto;

                dt.Rows.Add(drRow);
            }

            grdEtiqueta.DataSource = dt;

            GridColumn col = grdEtiquetaView.Columns["Nuevo"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 50;
            col.OptionsColumn.AllowFocus = false;

            grdEtiquetaView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Descending
                    ),
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        grdEtiquetaView.Columns["Texto"],
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdEtiquetaView.Columns["Id"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 200;
            col.OptionsColumn.AllowFocus = false;

            col = grdEtiquetaView.Columns["Texto"];
            col.Width = 300;

            grdEtiquetaView.OptionsBehavior.ReadOnly = proyecto.EtiquetasEditandose;

            bCambio = false;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            DataRow drLoc = grdEtiquetaView.GetFocusedDataRow();

            if (drLoc.IsNull())
            {
                XtraMessageBox.Show(
                    $"Seleccione una etiqueta",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            Id = drLoc["Id"].ToString();


            if (bCambio)
            {
                grdEtiquetaView.PostEditor();
                if (!grdEtiquetaView.UpdateCurrentRow())
                    return;

                var dicc = new Dictionary<string, Etiqueta>();

                DataTable dt = grdEtiqueta.DataSource as DataTable;

                foreach (var Etiqueta in proyecto.EtiquetaCol.Etiquetas)
                {
                    if (dicc.ContainsKey((string)Etiqueta.Id))
                    {
                        DataRow[] drLocRows = dt.Select($"Id = '{Etiqueta.Id}'");
                        for (int i = 1; i < drLocRows.Length; i++)
                            drLocRows[i].Delete();
                        continue;
                    }

                    DataRow[] drRows = dt.Select($"Id = '{Etiqueta.Id}'");
                    if (drRows.Length > 0)
                        dicc.Add((string)Etiqueta.Id, Etiqueta);
                }

                var Etiquetas = new EtiquetaCol();

                foreach (DataRow drRow in dt.Rows)
                {
                    var Id = drRow["Id"].ToString();

                    if (dicc.ContainsKey(Id))
                    {
                        var Etiqueta = new Etiqueta();
                        dicc.TryGetValue(Id, out Etiqueta);
                        var EtiquetaIdioma = Etiqueta.Idiomas.Where(f => f.IdiomaId == proyecto.esMX).FirstOrDefault();
                        if (EtiquetaIdioma.IsNull())
                        {
                            EtiquetaIdioma = new EtiquetaIdioma(proyecto.esMX, drRow["Texto"].ToString());
                            Etiqueta.Idiomas.Add(EtiquetaIdioma);
                        }
                        else
                        {
                            EtiquetaIdioma.Texto = drRow["Texto"].ToString();
                        }
                        Etiquetas.Etiquetas.Add(Etiqueta);
                    }
                    else
                    {
                        var etiqueta = new Etiqueta();
                        etiqueta.Id = drRow["Id"].ToString();
                        var EtiquetaIdioma = new EtiquetaIdioma(proyecto.esMX, drRow["Texto"].ToString());
                        etiqueta.Idiomas.Add(EtiquetaIdioma);
                        Etiquetas.Etiquetas.Add(etiqueta);
                    }
                }

                proyecto.EtiquetaCol = Etiquetas;
                proyecto.EtiquetasCambiadas = true;
                proyecto.SaveLabels();
            }

            DialogResult = DialogResult.OK;
        }

        private void grdEtiquetaView_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            DataRow dr = (DataRow)grdEtiquetaView.GetDataRow(e.RowHandle);

            if (dr["Texto"].ToString().Trim().IsNullOrStringEmpty())
            {
                e.Valid = false;
                e.ErrorText = "Etiqueta no puede ser nula";
                return;
            }

            if (dr["Nuevo"].ToBoolean())
            {
                dr["Id"] = Etiqueta.ToId(dr["Texto"].ToString());
            }

            if (e.RowHandle < 0)
            {
                DataTable dt = grdEtiqueta.DataSource as DataTable;
                foreach (DataRow drLoc in dt.Rows)
                {
                    if (drLoc["Id"].ToString() == dr["Id"].ToString())
                    {
                        e.ErrorText = "Identificador se repite";
                        e.Valid = false;
                        return;
                    }
                }
            }

            e.Valid = true;

            bCambio = true;
        }

        private void grdEtiquetaView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            DataRow dr = (DataRow)grdEtiquetaView.GetDataRow(e.RowHandle);
            dr["Nuevo"] = true;
            dr["Texto"] = string.Empty;
        }

        private void grdEtiquetaView_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {
            bCambio = true;
        }

        private void buActualizarEtiquetas_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}