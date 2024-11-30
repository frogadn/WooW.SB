using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs.CompareIntegralTest
{
    public partial class CompareIntegralTestGrid : DevExpress.XtraEditors.XtraUserControl
    {
        private PruebaIntegral pruebaDestino = null;
        private PruebaIntegral pruebaFuente = null;
        private DataTable dtFuente = null;

        public CompareIntegralTestGrid()
        {
            InitializeComponent();
        }

        public int GetSelectRow()
        {
            int result = -1;

            DataRow dr = grdElementoPruebaView.GetFocusedDataRow();
            if (dr != null)
            {
                result = dr[@"Orden"].ToInt32();
            }

            return result;
        }

        public DataTable DataTableBase()
        {
            DataTable dtDet = new DataTable();

            dtDet.Columns.Add(@"Orden", typeof(int));
            dtDet.Columns.Add(@"Tipo", typeof(string));
            dtDet.Columns.Add(@"PruebaUnitaria", typeof(string));
            dtDet.Columns.Add(@"Diagnostico", typeof(string));

            dtDet.PrimaryKey = new DataColumn[] { dtDet.Columns[@"Orden"] };

            return dtDet;
        }

        private void PruebaFuente()
        {
            dtFuente = DataTableBase();
            foreach (var Det in pruebaFuente.Pruebas)
            {
                DataRow drDet = dtFuente.NewRow();
                drDet[@"Orden"] = Det.Orden;
                drDet[@"Tipo"] = Det.Tipo.ToString();
                drDet[@"PruebaUnitaria"] = Det.ArchivoPruebaUnitaria;
                dtFuente.Rows.Add(drDet);
            }
        }

        private void DataSetPruebasIntegrales()
        {
            //grdElementoPrueba
            DataTable dtDet = DataTableBase();

            grdElementoPrueba.DataSource = dtDet;

            GridColumn col = grdElementoPruebaView.Columns[@"Orden"];
            col.Width = 50;
            grdElementoPruebaView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdElementoPruebaView.Columns[@"PruebaUnitaria"];
            col.Width = 200;
            col.Caption = "Prueba Unitaria";

            RepositoryItemButtonEdit txtCasoPrueba = new RepositoryItemButtonEdit();
            txtCasoPrueba.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            //txtCasoPrueba.Buttons.Add(
            //    new DevExpress.XtraEditors.Controls.EditorButton(
            //        DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis
            //    )
            //);
            //txtCasoPrueba.ButtonClick += TxtCasoPrueba_ButtonClick;
            col.ColumnEdit = txtCasoPrueba;

            col = grdElementoPruebaView.Columns[@"Diagnostico"];
            col.Width = 200;
            col.OptionsColumn.AllowEdit = false;

            col = grdElementoPruebaView.Columns[@"Tipo"];

            RepositoryItemComboBox txtTipo = new RepositoryItemComboBox();
            txtTipo.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtTipo.ImmediatePopup = true;
            txtTipo.Items.Clear();
            foreach (var tipo in Enum.GetValues(typeof(eTypePruebaUnitaria)))
                txtTipo.Items.Add(tipo.ToString());
            //txtTipo.EditValueChanged += TxtTipo_EditValueChanged;
            col.ColumnEdit = txtTipo;
        }

        public void CargarPruebaIntegrarl(
            string ArchivoDestino,
            string ArchivoFuente,
            PruebaIntegral _pruebaDestino,
            PruebaIntegral _pruebaFuente
        )
        {
            string ContenidoActual = File.ReadAllText(ArchivoDestino);
            string ContenidoComp = File.ReadAllText(ArchivoFuente);

            pruebaDestino = _pruebaDestino;
            pruebaFuente = _pruebaFuente;

            DataSetPruebasIntegrales();
            PruebaFuente();

            try
            {
                txtPrueba.EditValue = Path.GetFileNameWithoutExtension(ArchivoDestino);
                txtWorkItem.EditValue = pruebaDestino.WorkItem;
                txtProceso.EditValue = pruebaDestino.ProcesoId;
                txtDescripcion.EditValue = pruebaDestino.Descripcion;

                Colores();

                foreach (var Det in pruebaDestino.Pruebas)
                {
                    DataTable dtDet = grdElementoPrueba.DataSource as DataTable;
                    DataRow drDet = dtDet.NewRow();
                    drDet[@"Orden"] = Det.Orden;
                    drDet[@"Tipo"] = Det.Tipo.ToString();
                    drDet[@"PruebaUnitaria"] = Det.ArchivoPruebaUnitaria;
                    dtDet.Rows.Add(drDet);
                }
            }
            catch { }
        }

        private void Colores()
        {
            if (pruebaDestino.WorkItem != pruebaFuente.WorkItem)
                txtWorkItem.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtWorkItem.Properties.Appearance.BackColor = Color.White;

            if (pruebaDestino.ProcesoId != pruebaFuente.ProcesoId)
                txtProceso.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtProceso.Properties.Appearance.BackColor = Color.White;

            if (pruebaDestino.Descripcion != pruebaFuente.Descripcion)
                txtDescripcion.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtDescripcion.Properties.Appearance.BackColor = Color.White;
        }

        private void grdElementoPruebaView_CustomDrawCell(
            object sender,
            DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e
        )
        {
            DataRow drRow = grdElementoPruebaView.GetDataRow(e.RowHandle);

            if (drRow == null)
                return;

            int Orden = drRow[@"Orden"].ToInt32();

            var colFuente = pruebaDestino.Pruebas.Where(x => x.Orden == Orden).FirstOrDefault();
            var colDest = pruebaFuente.Pruebas.Where(x => x.Orden == Orden).FirstOrDefault();

            if (colDest == null)
            {
                e.Appearance.BackColor = System.Drawing.Color.LightGreen;
                return;
            }

            DataRow drDest = dtFuente.Rows.Find(Orden);

            if (drDest == null)
                return;

            if (
                drRow[e.Column.FieldName].ToSafeString()
                != drDest[e.Column.FieldName].ToSafeString()
            )
            {
                e.Appearance.BackColor = System.Drawing.Color.LightPink;
            }
        }
    }
}
