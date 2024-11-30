using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WooW.Core;

namespace WooW.SB.Config
{
    public partial class fmRolSelector : DevExpress.XtraEditors.XtraForm
    {
        private Proyecto proyecto;

        private bool bCambio = false;
        public string Id { get; set; }

        public fmRolSelector()
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

            dt.Columns.Add(@"Id", typeof(string));
            dt.Columns.Add(@"Texto", typeof(string));

            DataRow drRow = dt.NewRow();
            drRow["Id"] = Proyecto.NOVALIDAR;
            drRow["Texto"] = Proyecto.NOVALIDAR;
            dt.Rows.Add(drRow);


            foreach (var Rol in proyecto.Roles)
            {
                drRow = dt.NewRow();
                drRow["Id"] = Rol.Id;
                drRow["Texto"] = EtiquetaCol.Get(Rol.EtiquetaId);
                dt.Rows.Add(drRow);
            }

            grdRol.DataSource = dt;

            GridColumn col = grdRolView.Columns["Id"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;

            col = grdRolView.Columns["Texto"];
            col.Width = 300;

            grdRolView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            grdRolView.OptionsBehavior.ReadOnly = false;

            bCambio = false;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            DataRow drLoc = grdRolView.GetFocusedDataRow();

            if (drLoc.IsNull())
                return;

            Id = drLoc["Id"].ToString();

            if (bCambio)
            {
                grdRolView.PostEditor();
                if (!grdRolView.UpdateCurrentRow())
                    return;

                var dicc = new Dictionary<string, Etiqueta>();

                DataTable dt = grdRol.DataSource as DataTable;

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
                        var Etiqueta = new Etiqueta();
                        Etiqueta.Id = drRow["Id"].ToString();
                        var EtiquetaIdioma = new EtiquetaIdioma(proyecto.esMX, drRow["Texto"].ToString());
                        Etiqueta.Idiomas.Add(EtiquetaIdioma);
                        Etiquetas.Etiquetas.Add(Etiqueta);
                    }
                }

                proyecto.EtiquetaCol = Etiquetas;
            }

            DialogResult = DialogResult.OK;
        }

        private void grdEtiquetaView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            DataRow dr = (DataRow)grdRolView.GetDataRow(e.RowHandle);
            dr["Texto"] = string.Empty;
        }

        private void grdEtiquetaView_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {
            bCambio = true;
        }

        private void buActualizarDatos_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}