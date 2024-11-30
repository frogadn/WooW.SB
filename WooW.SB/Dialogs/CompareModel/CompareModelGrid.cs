using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using ServiceStack;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs.CompareModel
{
    public partial class CompareModelGrid : DevExpress.XtraEditors.XtraUserControl
    {
        private Proyecto proyectoFuente;
        private Proyecto proyectoDestino;
        private Modelo modeloFuente;
        private Modelo modeloDestino;

        private RepositoryItemComboBox txtModeloRep;

        private DataTable dtDestino = null;

        public CompareModelGrid()
        {
            InitializeComponent();
        }

        public string GetSelectRow()
        {
            string result = string.Empty;

            DataRow dr = grdColumnaView.GetFocusedDataRow();
            if (dr != null)
            {
                result = dr[@"Columna"].ToString();
            }

            return result;
        }

        public void ModelSetter(
            string Titulo,
            Proyecto _proyectoFuente,
            Proyecto _proyectoDestino,
            Modelo _modeloFuente,
            Modelo _modeloDestino
        )
        {
            this.proyectoFuente = _proyectoFuente;
            this.proyectoDestino = _proyectoDestino;
            this.modeloFuente = _modeloFuente;
            this.modeloDestino = _modeloDestino;

            grdGroup.Text = Titulo;

            Load();
        }

        private void Load()
        {
            dtDestino = DataTableBase();
            foreach (var col in modeloDestino.Columnas)
            {
                DataRow dr = dtDestino.NewRow();
                AgregarRenglon(dr, col);
                dtDestino.Rows.Add(dr);
            }
            dtDestino.PrimaryKey = new DataColumn[] { dtDestino.Columns[@"Columna"] };

            DataTable dt = CargarColumnas();
            CargaModelo(modeloFuente, dt);
            Colores();
        }

        public DataTable DataTableBase()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Orden", typeof(int));
            dt.Columns.Add(@"Columna", typeof(string));
            dt.Columns.Add(@"ColumnaOriginal", typeof(string));

            dt.Columns.Add(@"Descripcion", typeof(string));
            dt.Columns.Add(@"Ayuda", typeof(string));
            dt.Columns.Add(@"EtiquetaFormulario", typeof(string));
            dt.Columns.Add(@"EtiquetaGrid", typeof(string));

            dt.Columns.Add(@"TipoCol", typeof(string));
            dt.Columns.Add(@"Coleccion", typeof(bool));
            dt.Columns.Add(@"Modelo", typeof(string));
            dt.Columns.Add(@"Longitud", typeof(int));
            dt.Columns.Add(@"Precision", typeof(int));

            dt.Columns.Add(@"TipoDato", typeof(string));
            dt.Columns.Add(@"AceptaNulos", typeof(bool));
            dt.Columns.Add(@"EsVisibleEnLookUp", typeof(bool));
            dt.Columns.Add(@"Primaria", typeof(bool));
            dt.Columns.Add(@"Persistente", typeof(bool));
            dt.Columns.Add(@"Default", typeof(string));
            dt.Columns.Add(@"Apps", typeof(string));
            dt.Columns.Add(@"Control", typeof(string));
            dt.Columns.Add(@"Propiedades", typeof(string));
            dt.Columns.Add(@"Legacy", typeof(string));

            return dt;
        }

        public DataTable CargarColumnas()
        {
            grdColumna.DataSource = null;
            grdColumnaView.Columns.Clear();

            DataTable dt = DataTableBase();

            grdColumna.DataSource = dt;

            GridColumn col = grdColumnaView.Columns[@"Orden"];
            col.Width = 100;
            RepositoryItemSpinEdit txtOrden = new RepositoryItemSpinEdit();
            txtOrden.MinValue = 0;
            txtOrden.MaxValue = 999;
            col.ColumnEdit = txtOrden;

            grdColumnaView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdColumnaView.Columns[@"Columna"];
            col.Width = 200;
            col.MinWidth = 150;
            RepositoryItemTextEdit txtColumna = new RepositoryItemTextEdit();
            txtColumna.MaskSettings.Set(
                @"MaskManagerType",
                typeof(DevExpress.Data.Mask.RegularMaskManager)
            );
            txtColumna.MaskSettings.Set(@"mask", @"[A-Z][a-zA-Z0-9]*");
            col.ColumnEdit = txtColumna;

            col = grdColumnaView.Columns[@"ColumnaOriginal"];
            col.Visible = false;

            col = grdColumnaView.Columns[@"Descripcion"];
            col.Width = 300;
            col.MinWidth = 150;

            col = grdColumnaView.Columns[@"Ayuda"];
            col.Width = 300;
            col.MinWidth = 150;

            col = grdColumnaView.Columns[@"EtiquetaFormulario"];
            col.Width = 300;
            col.MinWidth = 150;
            col.Caption = "Etiqueta para Formularios";

            col = grdColumnaView.Columns[@"EtiquetaGrid"];
            col.Width = 300;
            col.MinWidth = 150;
            col.Caption = "Etiqueta para Tablas";

            col = grdColumnaView.Columns[@"TipoCol"];
            col.Width = 100;
            col.Caption = "Tipo de Columna";

            RepositoryItemComboBox txtTipoCol = new RepositoryItemComboBox();
            txtTipoCol.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtTipoCol.ImmediatePopup = true;
            txtTipoCol.Items.Clear();
            foreach (var tipo in Enum.GetValues(typeof(WoTypeColumn)))
                txtTipoCol.Items.Add(tipo.ToString());
            //txtTipoCol.EditValueChanged += TxtTipoCol_EditValueChanged;
            col.ColumnEdit = txtTipoCol;

            col = grdColumnaView.Columns[@"Coleccion"];
            col.Width = 50;
            col.Caption = "Es Colección?";

            col = grdColumnaView.Columns[@"Modelo"];
            col.Width = 100;
            col.Caption = "Modelo";

            txtModeloRep = new RepositoryItemComboBox();
            txtModeloRep.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtModeloRep.ImmediatePopup = true;
            //CargaModelos();
            //txtModeloRep.EditValueChanged += TxtModel_EditValueChanged;
            txtModeloRep.Buttons.Add(new EditorButton(ButtonPredefines.Clear));
            //txtModeloRep.ButtonClick += TxtModeloRep_ButtonClick;

            col.ColumnEdit = txtModeloRep;

            col = grdColumnaView.Columns[@"Propiedades"];

            RepositoryItemButtonEdit txtPropiedades = new RepositoryItemButtonEdit();
            txtPropiedades.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtPropiedades.Buttons.Add(new EditorButton(ButtonPredefines.Clear));
            //txtPropiedades.ButtonClick += TxtPropiedades_ButtonClick;
            col.ColumnEdit = txtPropiedades;
            col.Width = 100;
            col.MaxWidth = 100;
            col.MinWidth = 50;

            col = grdColumnaView.Columns[@"Longitud"];
            col.Width = 50;
            RepositoryItemSpinEdit txtLongitud = new RepositoryItemSpinEdit();
            txtLongitud.MinValue = 0;
            txtLongitud.MaxValue = 2048;
            col.ColumnEdit = txtLongitud;

            col = grdColumnaView.Columns[@"Precision"];
            col.Width = 50;
            RepositoryItemSpinEdit txtPrecision = new RepositoryItemSpinEdit();
            txtPrecision.MinValue = 0;
            txtPrecision.MaxValue = 16;
            col.ColumnEdit = txtPrecision;

            col = grdColumnaView.Columns[@"TipoDato"];
            col.Width = 100;
            col.Caption = "Tipo de Dato";

            RepositoryItemComboBox txtTipoDato = new RepositoryItemComboBox();
            txtTipoDato.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtTipoDato.ImmediatePopup = true;
            txtTipoDato.Items.Clear();
            foreach (var tipo in Enum.GetValues(typeof(WoTypeData)))
                txtTipoDato.Items.Add(tipo.ToString());
            col.ColumnEdit = txtTipoDato;

            col = grdColumnaView.Columns[@"AceptaNulos"];
            col.Width = 50;
            col.Caption = "Acepta Nulos?";

            col = grdColumnaView.Columns[@"EsVisibleEnLookUp"];
            col.Width = 50;
            col.Caption = "Es Visible En LookUp?";

            col = grdColumnaView.Columns[@"Persistente"];
            col.Width = 50;
            col.Caption = "Persistente?";

            col = grdColumnaView.Columns[@"Primaria"];
            col.Width = 50;
            col.Caption = "LLave Primaria?";

            col = grdColumnaView.Columns[@"Default"];
            col.Width = 100;
            col.Caption = "Valor Default";

            col = grdColumnaView.Columns[@"Apps"];
            col.Width = 100;
            col.Caption = "Omitir Apps";

            RepositoryItemCheckedComboBoxEdit txtApp = new RepositoryItemCheckedComboBoxEdit();
            txtApp.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtApp.Items.Clear();
            foreach (var app in proyectoFuente.Apps)
                txtApp.Items.Add(app.Id);
            col.ColumnEdit = txtApp;

            col = grdColumnaView.Columns[@"Control"];
            col.Width = 50;

            RepositoryItemComboBox txtControl = new RepositoryItemComboBox();
            txtControl.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtControl.ImmediatePopup = true;
            txtControl.Items.Clear();
            foreach (var tipo in Enum.GetValues(typeof(WoTypeControl)))
                txtControl.Items.Add(tipo.ToString());
            col.ColumnEdit = txtControl;

            col = grdColumnaView.Columns[@"Legacy"];
            col.Width = 80;
            col.MinWidth = 50;
            col.MaxWidth = 150;

            foreach (GridColumn colLoc in grdColumnaView.Columns)
            {
                colLoc.OptionsFilter.AllowFilter = false;
            }

            grdColumna.ForceInitialize();
            grdColumnaView.BestFitColumns();

            return dt;
        }

        private void CargaModelo(Modelo modelo, DataTable dt)
        {
            txtTipoModelo.Properties.Items.Clear();

            foreach (WoTypeModel tipo in Enum.GetValues(typeof(WoTypeModel)))
                txtTipoModelo.Properties.Items.Add(tipo.GetDescription());

            txtSubTipoModelo.Properties.Items.Clear();

            foreach (WoSubTypeModel tipo in Enum.GetValues(typeof(WoSubTypeModel)))
                txtSubTipoModelo.Properties.Items.Add(tipo.GetDescription());

            txtProceso.Properties.Items.Clear();

            foreach (var Proceso in proyectoFuente.Procesos)
                txtProceso.Properties.Items.Add(Proceso.Id);

            txtDescripcion.EditValue = modelo.EtiquetaId;
            txtProceso.EditValue = modelo.ProcesoId;
            txtOrdenDeCreacion.EditValue = modelo.OrdenDeCreacion;
            txtModelo.EditValue = modelo.Id;
            txtTipoModelo.EditValue = modelo.TipoModelo.GetDescription();
            txtSubTipoModelo.EditValue = modelo.SubTipoModelo.GetDescription();

            txtLegacy.EditValue = modelo.Legacy.ToString();
            txtInterface1.EditValue = (
                modelo.Interface1.IsNullOrStringEmpty() ? null : modelo.Interface1.ToString()
            );

            txtInterface2.EditValue = (
                modelo.Interface2.IsNullOrStringEmpty() ? null : modelo.Interface2.ToString()
            );

            txtInterface3.EditValue = (
                modelo.Interface3.IsNullOrStringEmpty() ? null : modelo.Interface3.ToString()
            );

            foreach (CheckedListBoxItem item in txtApps.Properties.Items)
            {
                if (
                    modelo.Apps.Where(x => x.AppId == item.Value.ToString()).FirstOrDefault()
                    != null
                )
                    item.CheckState = CheckState.Checked;
                else
                    item.CheckState = CheckState.Unchecked;
            }

            dt.Rows.Clear(); // Porque duplica las columnas automaticas

            foreach (var col in modelo.Columnas)
            {
                DataRow dr = dt.NewRow();
                AgregarRenglon(dr, col);
                dt.Rows.Add(dr);
            }

            txtRoles.Properties.Items.Clear();
            txtRoles.Properties.Items.Add(Proyecto.NOVALIDAR);
            foreach (var Rol in proyectoFuente.Roles)
                txtRoles.Properties.Items.Add(Rol.Id);

            txtPermisos.Properties.Items.Clear();
            txtPermisos.Properties.Items.Add(Proyecto.NOVALIDAR);
            foreach (var Permiso in proyectoFuente.Permisos)
                txtPermisos.Properties.Items.Add(Permiso.Id);

            if (
                (modelo.TipoModelo == WoTypeModel.Request)
                || (modelo.TipoModelo == WoTypeModel.View)
            )
            {
                txtRoles.Enabled = true;
                txtPermisos.Enabled = true;

                foreach (string rol in modelo.ScriptVistaRoles.Roles)
                {
                    foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                    {
                        if (i.Value.ToString() == rol)
                        {
                            i.CheckState = CheckState.Checked;
                            break;
                        }
                    }
                }

                foreach (string permiso in modelo.ScriptVistaRoles.Permisos)
                {
                    foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                    {
                        if (i.Value.ToString() == permiso)
                        {
                            i.CheckState = CheckState.Checked;
                            break;
                        }
                    }
                }
            }
            else
            {
                txtRoles.Enabled = false;
                txtPermisos.Enabled = false;
            }
        }

        private void Colores()
        {
            if (modeloFuente.TipoModelo != modeloDestino.TipoModelo)
                txtTipoModelo.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtTipoModelo.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.SubTipoModelo != modeloDestino.SubTipoModelo)
                txtSubTipoModelo.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtSubTipoModelo.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.ProcesoId != modeloDestino.ProcesoId)
                txtProceso.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtProceso.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.EtiquetaId != modeloDestino.EtiquetaId)
                txtDescripcion.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtDescripcion.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.OrdenDeCreacion != modeloDestino.OrdenDeCreacion)
                txtOrdenDeCreacion.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtOrdenDeCreacion.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.Id != modeloDestino.Id)
                txtModelo.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtModelo.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.Legacy != modeloDestino.Legacy)
                txtLegacy.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtLegacy.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.Interface1 != modeloDestino.Interface1)
                txtInterface1.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtInterface1.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.Interface2 != modeloDestino.Interface2)
                txtInterface2.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtInterface2.Properties.Appearance.BackColor = Color.White;

            if (modeloFuente.Interface3 != modeloDestino.Interface3)
                txtInterface3.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtInterface3.Properties.Appearance.BackColor = Color.White;

            //if (modeloFuente.Apps.ToJson() != modeloDestino.Apps.ToJson())
            //    txtApps.Properties.Appearance.BackColor = Color.LightPink;
            //else
            //    txtApps.Properties.Appearance.BackColor = Color.White;

            //if (modeloFuente.Columnas.ToJson() != modeloDestino.Columnas.ToJson())
            //    grdColumnaView.Appearance.Row.BackColor = Color.LightPink;
            //else
            //    grdColumnaView.Appearance.Row.BackColor = Color.White;

            if (
                modeloFuente.ScriptVistaRoles.Roles.ToJson()
                != modeloDestino.ScriptVistaRoles.Roles.ToJson()
            )
                txtRoles.Properties.Appearance.BackColor = Color.LightPink;
            else
                txtRoles.Properties.Appearance.BackColor = Color.White;
        }

        private void AgregarRenglon(DataRow dr, ModeloColumna col)
        {
            dr[@"Orden"] = col.Orden;

            dr[@"Columna"] = col.Id;

            dr[@"ColumnaOriginal"] = col.Id;

            dr[@"Descripcion"] = col.Descripcion;
            dr[@"Ayuda"] = col.Ayuda;
            dr[@"EtiquetaFormulario"] = col.Formulario;
            dr[@"EtiquetaGrid"] = col.Grid;

            dr[@"TipoCol"] = col.TipoColumna.ToString();
            dr[@"Coleccion"] = col.EsColeccion;
            dr[@"Modelo"] = col.ModeloId.ToString();
            dr[@"Longitud"] = col.Longitud;
            dr[@"Precision"] = col.Precision;

            dr[@"TipoDato"] = col.TipoDato.ToString();
            dr[@"AceptaNulos"] = col.Nulo;
            dr[@"EsVisibleEnLookUp"] = col.EsVisibleEnLookUp;
            dr[@"Persistente"] = col.Persistente;

            dr[@"Primaria"] = col.Primaria;
            dr[@"Default"] = col.Default;
            dr[@"Control"] = col.TipoControl.ToString();
            dr[@"Propiedades"] = col.TipoControlPropiedades;

            dr[@"Legacy"] = col.Legacy;

            string sApps = string.Empty;
            foreach (var app in col.Apps)
            {
                if (!sApps.IsNullOrStringEmpty())
                    sApps += ", ";
                sApps += app.AppId;
            }

            dr[@"Apps"] = sApps;
        }

        private void grdColumnaView_CustomDrawCell(
            object sender,
            DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e
        )
        {
            DataRow drRow = grdColumnaView.GetDataRow(e.RowHandle);

            if (drRow == null)
                return;

            string Id = drRow[@"Columna"].ToString();

            var colFuente = modeloFuente.Columnas.Where(x => x.Id == Id).FirstOrDefault();
            var colDest = modeloDestino.Columnas.Where(x => x.Id == Id).FirstOrDefault();

            if (colDest == null)
            {
                // e.Handled = true;
                e.Appearance.BackColor = System.Drawing.Color.LightGreen;
                return;
            }

            DataRow drDest = dtDestino.Rows.Find(Id);

            if (drDest == null)
                return;

            if (
                drRow[e.Column.FieldName].ToSafeString()
                != drDest[e.Column.FieldName].ToSafeString()
            )
            {
                // e.Handled = true;
                e.Appearance.BackColor = System.Drawing.Color.LightPink;
                return;
            }
        }
    }
}
