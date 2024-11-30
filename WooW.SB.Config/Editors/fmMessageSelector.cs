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
    public partial class fmMessageSelector : XtraForm
    {
        private Proyecto proyecto;

        private bool bCambio = false;

        public string Id { get; set; }

        public fmMessageSelector()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();
        }

        private void fmMessageSelector_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {

            DataTable dt = new DataTable();

            dt.Columns.Add(@"Nuevo", typeof(bool));
            dt.Columns.Add(@"Id", typeof(string));
            dt.Columns.Add(@"Texto", typeof(string));
            dt.Columns.Add(@"Solucion", typeof(string));

            foreach (var Mensaje in proyecto.MensajeCol.Mensajes)
            {
                DataRow drRow = dt.NewRow();
                drRow["Nuevo"] = false;
                drRow["Id"] = Mensaje.Id;
                var MensajeIdioma = Mensaje.Idiomas.Where(f => f.IdiomaId == proyecto.esMX).FirstOrDefault();
                if (MensajeIdioma != null)
                {
                    drRow["Texto"] = MensajeIdioma.Texto;
                    drRow["Solucion"] = MensajeIdioma.Solucion;
                }

                dt.Rows.Add(drRow);
            }

            grdMensaje.DataSource = dt;

            GridColumn col = grdMensajeView.Columns["Nuevo"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 50;
            col.OptionsColumn.AllowFocus = false;

            grdMensajeView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Descending
                    ),
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        grdMensajeView.Columns["Texto"],
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdMensajeView.Columns["Id"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 200;
            col.OptionsColumn.AllowFocus = false;

            col = grdMensajeView.Columns["Texto"];
            col.Width = 300;

            grdMensajeView.OptionsBehavior.ReadOnly = proyecto.MensajesEditandose;

            bCambio = false;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            DataRow drLoc = grdMensajeView.GetFocusedDataRow();

            if (drLoc.IsNull())
            {
                XtraMessageBox.Show(
                    $"Seleccione un mensaje",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            Id = drLoc["Id"].ToString();


            if (bCambio)
            {
                grdMensajeView.PostEditor();
                if (!grdMensajeView.UpdateCurrentRow())
                    return;

                var dicc = new Dictionary<string, Mensaje>();

                DataTable dt = grdMensaje.DataSource as DataTable;

                foreach (var Mensaje in proyecto.MensajeCol.Mensajes)
                {
                    if (dicc.ContainsKey((string)Mensaje.Id))
                    {
                        DataRow[] drLocRows = dt.Select($"Id = '{Mensaje.Id}'");
                        for (int i = 1; i < drLocRows.Length; i++)
                            drLocRows[i].Delete();
                        continue;
                    }

                    DataRow[] drRows = dt.Select($"Id = '{Mensaje.Id}'");
                    if (drRows.Length > 0)
                        dicc.Add((string)Mensaje.Id, Mensaje);
                }

                var Mensajes = new MensajeCol();

                foreach (DataRow drRow in dt.Rows)
                {
                    var Id = drRow["Id"].ToString();

                    if (dicc.ContainsKey(Id))
                    {
                        var Mensaje = new Mensaje();
                        dicc.TryGetValue(Id, out Mensaje);
                        var MensajeIdioma = Mensaje.Idiomas.Where(f => f.IdiomaId == proyecto.esMX).FirstOrDefault();
                        if (MensajeIdioma.IsNull())
                        {
                            MensajeIdioma = new MensajeIdioma(proyecto.esMX, drRow["Texto"].ToString(), drRow["Solucion"].ToSafeString());
                            Mensaje.Idiomas.Add(MensajeIdioma);
                        }
                        else
                        {
                            MensajeIdioma.Texto = drRow["Texto"].ToString();
                        }
                        Mensajes.Mensajes.Add(Mensaje);
                    }
                    else
                    {
                        var Mensaje = new Mensaje();
                        Mensaje.Id = drRow["Id"].ToString();
                        var MensajeIdioma = new MensajeIdioma(proyecto.esMX, drRow["Texto"].ToString(), drRow["Solucion"].ToSafeString());
                        Mensaje.Idiomas.Add(MensajeIdioma);
                        Mensajes.Mensajes.Add(Mensaje);
                    }
                }

                proyecto.MensajeCol = Mensajes;
                proyecto.MensajesCambiados = true;
                proyecto.SaveLabels();
            }

            DialogResult = DialogResult.OK;
        }

        private void grdMensajeView_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            DataRow dr = (DataRow)grdMensajeView.GetDataRow(e.RowHandle);

            if (dr["Texto"].ToString().Trim().IsNullOrStringEmpty())
            {
                e.Valid = false;
                e.ErrorText = "Mensaje no puede ser nulo";
                return;
            }

            if (dr["Nuevo"].ToBoolean())
            {
                dr["Id"] = Etiqueta.ToId(dr["Texto"].ToString());
            }

            if (e.RowHandle < 0)
            {
                DataTable dt = grdMensaje.DataSource as DataTable;
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

        private void grdMensaje_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            DataRow dr = (DataRow)grdMensajeView.GetDataRow(e.RowHandle);
            dr["Nuevo"] = true;
            dr["Texto"] = string.Empty;
        }

        private void grdMensajeView_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {
            bCambio = true;
        }

        private void buActualizarMensajes_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
