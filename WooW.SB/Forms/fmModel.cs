//using DevExpress.CodeParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json;
using ServiceStack;
using WooW.Core;
using WooW.Core.Common;
using WooW.SB.Config;
using WooW.SB.Config.Class;
using WooW.SB.Config.ControlProperties.Class;
using WooW.SB.Config.ControlProperties.Editors;
using WooW.SB.Config.Enum;
using WooW.SB.Config.Helpers;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.SB.UI.EnumExtension;

// ToDo falta propiedades tipo de campo

namespace WooW.SB.Forms
{
    public partial class fmModel : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        public Proyecto proyecto { get; set; }

        private bool bNuevo = false;

        private RepositoryItemComboBox txtModeloRep;

        private fmColumnsSelector ColSelector = null;

        private bool bControlCambioDeSubTipo = false;

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public fmModel()
        {
            InitializeComponent();
        }

        public bool CambiosPendientes
        {
            get { return buRegistrar.Enabled; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Cargar()
        {
            repIdioma.Items.Clear();

            foreach (var Idioma in proyecto.Idiomas)
                repIdioma.Items.Add(Idioma.Id);

            txtApps.Properties.Items.Clear();
            repFiltroApps.Items.Clear();
            repFiltroApps.Items.Add(@" Todos ");
            repFiltroApps.Items[0].CheckState = CheckState.Checked;
            foreach (var app in proyecto.Apps)
            {
                txtApps.Properties.Items.Add(app.Id);
                repFiltroApps.Items.Add(app.Id);
            }
            txtFiltroApps.EditValue = " Todos ";
            //txtFiltroApps.EditValue = " Todos ";

            ReCargar();
        }

        public void Refrescar()
        {
            if (buRefrescar.Enabled)
                ReCargar();
        }

        public void ReCargar()
        {
            CargarColumnas();
            CargarModelo();
            grdModelosView_FocusedRowChanged(null, null);
        }

        public void CargarModelo()
        {
            #region Modelos

            DataTable dt = new DataTable();

            dt.Columns.Add(@"Proceso", typeof(string));
            dt.Columns.Add(@"Tipo", typeof(string));
            dt.Columns.Add(@"SubTipo", typeof(string));
            dt.Columns.Add(@"OrdenCreacion", typeof(int));
            dt.Columns.Add(@"Repositorio", typeof(string));
            dt.Columns.Add(@"Modelo", typeof(string));
            dt.Columns.Add(@"Fecha", typeof(DateTime));
            dt.Columns.Add(@"Json", typeof(string));

            foreach (var Modelo in proyecto.ModeloCol.Modelos.OrderBy(e => e.Id).ToList())
            {
                bool bFound = false;
                foreach (CheckedListBoxItem item in repFiltroApps.Items)
                {
                    if (item.CheckState != CheckState.Checked)
                        continue;

                    if (
                        (item.Value.ToString() == " Todos ")
                        || (
                            Modelo
                                .Apps.Where(x => x.AppId == item.Value.ToString())
                                .FirstOrDefault() != null
                        )
                    )
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                    continue;

                DataRow drRow = dt.NewRow();
                drRow[@"Proceso"] = Modelo.ProcesoId;
                drRow[@"Tipo"] = Modelo.TipoModelo.ToString();
                drRow[@"SubTipo"] = Modelo.SubTipoModelo.ToString();
                drRow[@"OrdenCreacion"] = Modelo.OrdenDeCreacion.ToInt32();
                drRow[@"Repositorio"] = Modelo.Repositorio.ToString();
                drRow[@"Modelo"] = Modelo.Id;
                drRow[@"Fecha"] = Modelo.FechaActualizacion;

                drRow[@"Json"] = Modelo.ToJson();
                dt.Rows.Add(drRow);
            }

            grdModelos.DataSource = dt;

            GridColumn col = grdModelosView.Columns[@"Proceso"];
            col.Width = 100;

            col = grdModelosView.Columns[@"Tipo"];
            col.Width = 100;

            col = grdModelosView.Columns[@"SubTipo"];
            col.Width = 100;

            col = grdModelosView.Columns[@"Repositorio"];
            col.Width = 100;

            col = grdModelosView.Columns[@"Modelo"];
            col.Width = 300;

            grdModelosView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdModelosView.Columns[@"Fecha"];
            col.Caption = "Actualización";
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            col.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";

            col = grdModelosView.Columns[@"Json"];
            col.Visible = false;

            txtTipoModelo.Properties.Items.Clear();

            foreach (WoTypeModel tipo in Enum.GetValues(typeof(WoTypeModel)))
                txtTipoModelo.Properties.Items.Add(tipo.GetDescription());

            txtSubTipoModelo.Properties.Items.Clear();

            foreach (WoSubTypeModel tipo in Enum.GetValues(typeof(WoSubTypeModel)))
                txtSubTipoModelo.Properties.Items.Add(tipo.GetDescription());

            txtProceso.Properties.Items.Clear();

            foreach (var Proceso in proyecto.Procesos.OrderBy(e => e.Id))
                txtProceso.Properties.Items.Add(Proceso.Id);

            txtFiltro.Properties.Items.Clear();

            foreach (woGetListFilterType tipo in Enum.GetValues(typeof(woGetListFilterType)))
                txtFiltro.Properties.Items.Add(tipo.GetDescription());

            #endregion Modelos
        }

        private void CargaModelos()
        {
            ProyectoConPaquetes.Clear();
            var locProyecto = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

            txtModeloRep.Items.Clear();
            foreach (
                var modelo in locProyecto
                    .ModeloCol.Modelos.Where(f =>
                        (f.TipoModelo != WoTypeModel.Interface)
                        && (f.TipoModelo != WoTypeModel.Parameter)
                        && (f.TipoModelo != WoTypeModel.Class)
                        && (f.TipoModelo != WoTypeModel.View)
                        && (f.TipoModelo != WoTypeModel.Request)
                        && (f.TipoModelo != WoTypeModel.Response)
                    )
                    .OrderBy(f => f.Id)
            )
                txtModeloRep.Items.Add(modelo.Id);

            txtInterface1.Properties.Items.Clear();
            txtInterface2.Properties.Items.Clear();
            txtInterface3.Properties.Items.Clear();

            //foreach (
            var interfases = locProyecto
                .ModeloCol.Modelos.Where(f => f.TipoModelo == WoTypeModel.Interface)
                .OrderBy(f => f.Id)
                .Select(f => f.Id)
                .ToList();
            txtInterface1.Properties.Items.AddRange(interfases);
            txtInterface2.Properties.Items.AddRange(interfases);
            txtInterface3.Properties.Items.AddRange(interfases);
        }

        public void CargarColumnas()
        {
            grdColumna.DataSource = null;
            grdColumnaView.Columns.Clear();

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
            dt.Columns.Add(@"Origen", typeof(string));

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
            txtColumna.MaskSettings.Set(@"mask", @"[A-Z_][a-zA-Z0-9_]*");
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
            col.Caption = "Etiqueta para\nFormularios";

            col = grdColumnaView.Columns[@"EtiquetaGrid"];
            col.Width = 300;
            col.MinWidth = 150;
            col.Caption = "Etiqueta para\nTablas";

            col = grdColumnaView.Columns[@"TipoCol"];
            col.Width = 100;
            col.Caption = "Tipo de\nColumna";

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
            txtTipoCol.EditValueChanged += TxtTipoCol_EditValueChanged;
            col.ColumnEdit = txtTipoCol;

            col = grdColumnaView.Columns[@"Coleccion"];
            col.Width = 50;
            col.Caption = "Es\nColección?";

            col = grdColumnaView.Columns[@"Modelo"];
            col.Width = 200;
            col.Caption = "Modelo";

            txtModeloRep = new RepositoryItemComboBox();
            txtModeloRep.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtModeloRep.ImmediatePopup = true;
            col.ColumnEdit = txtModeloRep;

            col = grdColumnaView.Columns[@"Origen"];
            col.Width = 70;
            RepositoryItemTextEdit txtOrigen = new RepositoryItemTextEdit();
            txtOrigen.ReadOnly = true;
            col.ColumnEdit = txtOrigen;

            CargaModelos();
            txtModeloRep.EditValueChanged += TxtModel_EditValueChanged;
            txtModeloRep.Buttons.Add(new EditorButton(ButtonPredefines.Clear));
            txtModeloRep.ButtonClick += TxtModeloRep_ButtonClick;

            col = grdColumnaView.Columns[@"Propiedades"];

            RepositoryItemButtonEdit txtPropiedades = new RepositoryItemButtonEdit();
            txtPropiedades.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtPropiedades.Buttons.Add(new EditorButton(ButtonPredefines.Clear));
            txtPropiedades.ButtonClick += TxtPropiedades_ButtonClick;
            col.ColumnEdit = txtPropiedades;
            col.Width = 150;
            col.MaxWidth = 150;
            col.MinWidth = 100;

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
            col.Caption = "Tipo de\nDato";

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
            col.Caption = "Acepta\nNulos?";

            col = grdColumnaView.Columns[@"EsVisibleEnLookUp"];
            col.Width = 50;
            col.Caption = "Es Visible\nEn LookUp?";

            col = grdColumnaView.Columns[@"Persistente"];
            col.Width = 50;
            col.Caption = "Persistente?";

            col = grdColumnaView.Columns[@"Primaria"];
            col.Width = 50;
            col.Caption = "LLave\nPrimaria?";

            col = grdColumnaView.Columns[@"Default"];
            col.Width = 100;
            col.Caption = "Valor\nDefault";

            col = grdColumnaView.Columns[@"Apps"];
            col.Width = 100;
            col.Caption = "Omitir\nApps";

            RepositoryItemCheckedComboBoxEdit txtApp = new RepositoryItemCheckedComboBoxEdit();
            txtApp.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtApp.Items.Clear();
            foreach (var app in proyecto.Apps)
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

            if (Screen.FromHandle(this.Handle).WorkingArea.Width > 1920)
                grdColumnaView.ColumnPanelRowHeight = 76;
            else
                grdColumnaView.ColumnPanelRowHeight = 38;
        }

        private void TxtModeloRep_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Clear)
            {
                if (
                    XtraMessageBox.Show(
                        "Limpiar el modelo?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) == DialogResult.Yes
                )
                    (sender as ComboBoxEdit).EditValue = null;
            }
        }

        private void TxtTipoCol_EditValueChanged(object sender, EventArgs e)
        {
            DataRow drRow = grdColumnaView.GetFocusedDataRow();

            if (drRow == null)
                return;

            if (!(e is ChangingEventArgs))
                return;

            string Valor = (e as ChangingEventArgs).NewValue.ToSafeString();

            if (Valor.IsNullOrStringEmpty())
                return;

            var TipoColumna = (WoTypeColumn)Enum.Parse(typeof(WoTypeColumn), Valor);

            if (TipoColumna == WoTypeColumn.String)
            {
                if (drRow[@"Longitud"].ToInt16() == 0)
                    drRow[@"Longitud"] = 60;
                drRow[@"Precision"] = 0;
                return;
            }

            if (TipoColumna == WoTypeColumn.Decimal)
            {
                if ((drRow[@"Longitud"].ToInt16() == 0) || (drRow[@"Longitud"].ToInt16() > 28))
                    drRow[@"Longitud"] = 24;
                if ((drRow[@"Precision"].ToInt16() == 0) || (drRow[@"Precision"].ToInt16() > 10))
                    drRow[@"Precision"] = 6;
                return;
            }

            drRow[@"Longitud"] = 0;
            drRow[@"Precision"] = 0;
        }

        private void TxtModel_EditValueChanged(object sender, EventArgs e)
        {
            DataRow drRow = grdColumnaView.GetFocusedDataRow();

            if (drRow == null)
                return;

            if (!(e is ChangingEventArgs))
                return;

            string Valor = (e as ChangingEventArgs).NewValue.ToSafeString();

            WoTypeColumn TipoColumna = WoTypeColumn.Reference;
            if (!drRow[@"TipoCol"].IsDBNull())
                TipoColumna = (WoTypeColumn)
                    Enum.Parse(typeof(WoTypeColumn), drRow[@"TipoCol"].ToString());

            if ((TipoColumna == WoTypeColumn.EnumString) || (TipoColumna == WoTypeColumn.EnumInt))
                return;

            var model = proyecto.ModeloCol.Modelos.Where(x => x.Id == Valor).FirstOrDefault();

            if (model == null)
            {
                var principal = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);
                model = principal.ModeloCol.Modelos.Where(x => x.Id == Valor).FirstOrDefault();
            }

            string Descripcion = string.Empty;
            string Ayuda = string.Empty;
            string Formulario = string.Empty;
            string Grid = string.Empty;

            if (model != null)
            {
                var columna = model.Columnas.Where(x => x.Id == "Id").FirstOrDefault();
                if (columna != null)
                {
                    if (
                        (columna.Descripcion == "Id")
                        || (columna.Descripcion == "Código")
                        || (columna.Descripcion == "Codigo")
                    )
                    {
                        Descripcion = model.EtiquetaId.SplitCamelCase();
                        Ayuda = model.EtiquetaId.SplitCamelCase();
                        Formulario = model.EtiquetaId.SplitCamelCase();
                        Grid = model.EtiquetaId.SplitCamelCase();
                    }
                    else
                    {
                        Descripcion = columna.Descripcion;
                        Ayuda = columna.Ayuda;
                        Formulario = columna.Formulario;
                        Grid = columna.Grid;
                    }
                }
            }

            string NombreCorrecto;

            if (drRow["Coleccion"].ToBoolean())
                NombreCorrecto = Valor + "Col";
            else
                NombreCorrecto = Valor + "Id";

            drRow[@"Columna"] = NombreCorrecto;
            drRow[@"Descripcion"] = Descripcion;
            drRow[@"Ayuda"] = Ayuda;
            drRow[@"EtiquetaFormulario"] = Formulario;
            drRow[@"EtiquetaGrid"] = Grid;
            drRow[@"Longitud"] = 0;
            drRow[@"Precision"] = 0;
            drRow[@"TipoCol"] = WoTypeColumn.Reference.ToString();
            drRow[@"Control"] = WoTypeControl.LookUpDialog.ToString();
        }

        private void TxtPropiedades_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            if (e.Button.Kind == ButtonPredefines.Clear)
            {
                if (
                    XtraMessageBox.Show(
                        "Limpiar las propiedades?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) == DialogResult.Yes
                )
                    (sender as ButtonEdit).EditValue = null;
            }
            else
            {
                DataRow drRow = grdColumnaView.GetFocusedDataRow();

                if (drRow == null)
                    return;

                var TipoControl = (WoTypeControl)
                    Enum.Parse(typeof(WoTypeControl), drRow[@"Control"].ToString());

                ITypeControlEditor fmProperties = null;
                switch (TipoControl)
                {
                    case WoTypeControl.EnumInt:
                        fmProperties = new TypeControlEnumIntEditor<TypeControlEnumInt>();
                        break;

                    case WoTypeControl.EnumString:
                        fmProperties = new TypeControlEnumStringEditor<TypeControlEnumString>();
                        break;

                    default:
                        return;
                }

                fmProperties.Properties = drRow[@"Propiedades"].ToString();

                fmProperties.ShowEditor();

                drRow[@"Propiedades"] = fmProperties.Properties;
            }
        }

        private void buNuevo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bNuevo = true;
            grdModelosView.AddNewRow();

            buNuevo.Enabled = false;
            buEditar.Enabled = false;
            buRegistrar.Enabled = true;
            buAnular.Enabled = true;
            buBorrar.Enabled = false;
            buRenombrar.Enabled = false;
            buRefrescar.Enabled = false;
            txtFiltroApps.Enabled = false;

            pnlDatosBotones.Enabled = false;
            pnlDatosModelo.Enabled = true;

            grdColumnaView.OptionsBehavior.Editable = true;

            txtModelo.Enabled = true;
            buExtension.Enabled = true;
            txtTipoModelo.Enabled = true;
            txtSubTipoModelo.Enabled = true;
            txtInterface1.Enabled = true;
            txtInterface2.Enabled = true;
            txtInterface3.Enabled = true;
            txtFiltro.Enabled = true;

            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;

            grdColumnaView.FocusedColumn = grdColumnaView.Columns[@"Columna"];
        }

        private void grdModelosView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            buNuevo.Enabled = true;
            buEditar.Enabled = false;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buBorrar.Enabled = false;
            buRenombrar.Enabled = false;
            pnlDatosBotones.Enabled = false;
            pnlDatosModelo.Enabled = false;
            buRefrescar.Enabled = true;
            txtFiltroApps.Enabled = true;

            txtModelo.Enabled = true;
            txtTipoModelo.Enabled = true;
            txtSubTipoModelo.Enabled = true;
            txtDescripcion.Enabled = true;
            txtOrdenDeCreacion.Enabled = true;
            txtProceso.Enabled = true;
            txtLegacy.Enabled = true;
            txtFiltro.Enabled = true;
            txtRoles.Enabled = true;
            txtPermisos.Enabled = true;
            txtApps.Enabled = true;
            txtInterface1.Enabled = true;
            txtInterface2.Enabled = true;
            txtInterface3.Enabled = true;

            grdColumnaView.OptionsBehavior.Editable = false;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                txtModelo.EditValue = string.Empty;
                txtTipoModelo.EditValue = string.Empty;
                txtSubTipoModelo.EditValue = string.Empty;
                txtDescripcion.EditValue = string.Empty;
                txtProceso.EditValue = string.Empty;
                txtFiltro.EditValue = string.Empty;
                txtOrdenDeCreacion.EditValue = -1;
                txtLegacy.EditValue = string.Empty;
                txtInterface1.EditValue = null;
                txtInterface2.EditValue = null;
                txtInterface3.EditValue = null;

                (grdColumna.DataSource as DataTable).Rows.Clear();
                return;
            }

            DataTable dt = grdColumna.DataSource as DataTable;

            dt.Rows.Clear();

            txtRoles.Enabled = false;
            txtPermisos.Enabled = false;
            txtFiltro.Enabled = false;

            foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                i.CheckState = CheckState.Unchecked;

            foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                i.CheckState = CheckState.Unchecked;

            if (drRow[@"Json"] != DBNull.Value)
            {
                Modelo modelo = Modelo.FromJson(drRow[@"Json"].ToString(), proyecto);

                txtDescripcion.EditValue = modelo.EtiquetaId;
                txtProceso.EditValue = modelo.ProcesoId;
                txtOrdenDeCreacion.EditValue = modelo.OrdenDeCreacion;
                txtModelo.EditValue = modelo.Id;
                txtTipoModelo.EditValue = modelo.TipoModelo.GetDescription();

                try
                {
                    bControlCambioDeSubTipo = true;
                    txtSubTipoModelo.EditValue = (
                        modelo.SubTipoModelo.IsNullOrStringEmpty()
                            ? null
                            : modelo.SubTipoModelo.GetDescription()
                    );
                }
                finally
                {
                    bControlCambioDeSubTipo = false;
                }

                txtFiltro.EditValue = modelo.Filtro.GetDescription();

                dt.Rows.Clear(); // Porque duplica las columnas automaticas
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

                foreach (var col in modelo.Columnas)
                {
                    DataRow dr = dt.NewRow();
                    AgregarRenglon(dr, col);
                    dt.Rows.Add(dr);
                }

                txtRoles.Properties.Items.Clear();
                txtRoles.Properties.Items.Add(Proyecto.NOVALIDAR);
                foreach (var Rol in proyecto.Roles)
                    txtRoles.Properties.Items.Add(Rol.Id);

                txtPermisos.Properties.Items.Clear();
                txtPermisos.Properties.Items.Add(Proyecto.NOVALIDAR);
                foreach (var Permiso in proyecto.Permisos)
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
            else
            {
                txtDescripcion.EditValue = string.Empty;
                txtProceso.EditValue = string.Empty;
                txtOrdenDeCreacion.EditValue = -1;
                txtModelo.EditValue = string.Empty;
                txtTipoModelo.EditValue = string.Empty;
                txtSubTipoModelo.EditValue = string.Empty;
                txtFiltro.EditValue = string.Empty;
                txtLegacy.EditValue = string.Empty;
                txtInterface1.EditValue = null;
                txtInterface2.EditValue = null;
                txtInterface3.EditValue = null;

                foreach (CheckedListBoxItem item in txtApps.Properties.Items)
                    item.CheckState = CheckState.Unchecked;

                (grdColumna.DataSource as DataTable).Rows.Clear();
            }

            txtModelo.Enabled = false;
            buExtension.Enabled = false;
            txtTipoModelo.Enabled = false;
            txtSubTipoModelo.Enabled = false;
            txtFiltro.Enabled = false;
            txtInterface1.Enabled = false;
            txtInterface2.Enabled = false;
            txtInterface3.Enabled = false;

            buEditar.Enabled = true;
            buBorrar.Enabled = true;
            buRenombrar.Enabled = true;
        }

        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string CambiosPendientes = string.Empty;
            if ((this.Parent as fmMain).CambiosPendientesLogicaOScript(out CambiosPendientes))
            {
                XtraMessageBox.Show(
                    "No se puede editar si tiene cambios por aplicar en " + CambiosPendientes,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            DataRow dr = grdModelosView.GetFocusedDataRow();

            if (dr.IsNull())
                return;

            buNuevo.Enabled = false;
            buEditar.Enabled = false;
            buRegistrar.Enabled =
                (dr[@"Modelo"].ToSafeString().StartsWith(wooWConfigParams.Origen))
                || (
                    (dr[@"Tipo"].ToSafeString() == WoTypeModel.Interface.GetDescription())
                    && (
                        dr[@"Modelo"]
                            .ToSafeString()
                            .Substring(1)
                            .StartsWith(wooWConfigParams.Origen)
                    )
                )
                || (dr[@"SubTipo"].ToSafeString() == WoSubTypeModel.Extension.GetDescription())
                || (dr[@"SubTipo"].ToSafeString() == WoSubTypeModel.Override.GetDescription());

            buAnular.Enabled = true;
            buBorrar.Enabled = false;
            buRenombrar.Enabled = false;
            buRefrescar.Enabled = false;

            if (
                (dr[@"SubTipo"].ToSafeString() == WoSubTypeModel.Extension.GetDescription())
                || (dr[@"SubTipo"].ToSafeString() == WoSubTypeModel.Override.GetDescription())
            )
            {
                var TipoModelo = EnumExtensionMethods.GetValueFromDescription<WoTypeModel>(
                    txtTipoModelo.EditValue.ToString()
                );

                pnlDatosBotones.Enabled = true;
                if (
                    (TipoModelo == WoTypeModel.Configuration)
                    || (TipoModelo == WoTypeModel.CatalogType)
                    || (TipoModelo == WoTypeModel.Catalog)
                    || (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                    || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                    || (TipoModelo == WoTypeModel.Control)
                    || (TipoModelo == WoTypeModel.Kardex)
                    || (TipoModelo == WoTypeModel.Parameter)
                )
                {
                    pnlDatosModelo.Enabled = false;
                }
                else if (TipoModelo == WoTypeModel.View)
                {
                    txtModelo.Enabled = false;
                    txtTipoModelo.Enabled = false;
                    txtSubTipoModelo.Enabled = false;
                    txtDescripcion.Enabled = true;
                    txtOrdenDeCreacion.Enabled = true;
                    txtProceso.Enabled = true;
                    txtLegacy.Enabled = false;
                    txtFiltro.Enabled = false;
                    txtRoles.Enabled = true;
                    txtPermisos.Enabled = true;
                    txtApps.Enabled = true;
                    txtInterface1.Enabled = false;
                    txtInterface2.Enabled = false;
                    txtInterface3.Enabled = false;
                    pnlDatosModelo.Enabled = true;
                }
                else if (TipoModelo == WoTypeModel.Class)
                {
                    txtModelo.Enabled = false;
                    txtTipoModelo.Enabled = false;
                    txtSubTipoModelo.Enabled = false;
                    txtDescripcion.Enabled = true;
                    txtOrdenDeCreacion.Enabled = false;
                    txtProceso.Enabled = true;
                    txtLegacy.Enabled = false;
                    txtFiltro.Enabled = false;
                    txtRoles.Enabled = false;
                    txtPermisos.Enabled = false;
                    txtApps.Enabled = true;
                    txtInterface1.Enabled = false;
                    txtInterface2.Enabled = false;
                    txtInterface3.Enabled = false;
                    pnlDatosModelo.Enabled = true;
                }
            }
            else
            {
                txtFiltroApps.Enabled = false;
                txtModelo.Enabled = false;
                buExtension.Enabled = false;
                txtTipoModelo.Enabled = false;
                txtSubTipoModelo.Enabled = true;
                txtFiltro.Enabled = true;
                txtInterface1.Enabled = true;
                txtInterface2.Enabled = true;
                txtInterface3.Enabled = true;

                txtTipoModelo_EditValueChanged(null, null);
                pnlDatosBotones.Enabled = true;
                pnlDatosModelo.Enabled = true;
            }

            grdColumnaView.OptionsBehavior.Editable = true;

            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            grdColumnaView.FocusedColumn = grdColumnaView.Columns[@"Columna"];

            bNuevo = false;
        }

        private bool ValidarColumnas()
        {
            // ToDo Validar que para los spin deba ser integer o smallint
            // ToDo validar spin tipo de dato y valores maximos y minimos
            DataTable dt = grdColumna.DataSource as DataTable;

            if (dt.IsNull())
                return false;

            ProyectoConPaquetes.Clear();

            var TipoModelo = EnumExtensionMethods.GetValueFromDescription<WoTypeModel>(
                txtTipoModelo.EditValue.ToString()
            );

            var SubTipoModelo = EnumExtensionMethods.GetValueFromDescription<WoSubTypeModel>(
                txtSubTipoModelo.EditValue.ToString()
            );

            if (SubTipoModelo != WoSubTypeModel.Extension)
            {
                string locModelo = txtModelo.EditValue.ToSafeString();
                if (TipoModelo == WoTypeModel.Interface)
                    locModelo = locModelo.Substring(1);

                if (!locModelo.StartsWith(wooWConfigParams.Origen))
                    throw new Exception(
                        $"Nombre del modelo {txtModelo.EditValue.ToSafeString()} debe comenzar con Origen '{wooWConfigParams.Origen}'"
                    );
            }

            var Repositorio = TypeRepository.User;
            //    EnumExtensionMethods.GetValueFromDescription<TypeRepository>(
            //    txtSubTipoModelo.EditValue.ToString()
            //);

            try
            {
                if (
                    (TipoModelo == WoTypeModel.Interface)
                    || (TipoModelo == WoTypeModel.TransactionSlave)
                    || (TipoModelo == WoTypeModel.CatalogSlave)
                    || (TipoModelo == WoTypeModel.DataMart)
                    || (TipoModelo == WoTypeModel.Complex)
                    || (TipoModelo == WoTypeModel.Response)
                    || (TipoModelo == WoTypeModel.View)
                )
                {
                    if (SubTipoModelo != WoSubTypeModel.NotApply)
                        throw new Exception(
                            "Para Tipo Interface, TransactionSlave, CatalogSlave, DataMart, Response y Complex, SubTipo debe ser NotApply"
                        );
                }
                else if (TipoModelo == WoTypeModel.Request)
                {
                    if (
                        (SubTipoModelo == WoSubTypeModel.NotApply)
                        || (SubTipoModelo == WoSubTypeModel.StateDiagram)
                    )
                        throw new Exception(
                            "Para Tipo Request, SubTipo deber ser Report o DataService o MicroService o BackGroudTask "
                        );
                }
                else if (
                    (TipoModelo == WoTypeModel.Configuration)
                    || (TipoModelo == WoTypeModel.CatalogType)
                    || (TipoModelo == WoTypeModel.Catalog)
                    || (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                    || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                    || (TipoModelo == WoTypeModel.Control)
                    || (TipoModelo == WoTypeModel.Kardex)
                    || (TipoModelo == WoTypeModel.Parameter)
                )
                {
                    if (SubTipoModelo != WoSubTypeModel.StateDiagram)
                        throw new Exception(
                            "Para Tipo Configuration, CatalogType, Catalog, TransactionContable, TransactionNoContable, TransactionFreeStyle, Control, Kardex, Parameter, DataBase View; SubTipo debe ser StateDiagram"
                        );
                }
                else if ((TipoModelo == WoTypeModel.Class))
                {
                    if (
                        (SubTipoModelo != WoSubTypeModel.Standard)
                        && (SubTipoModelo != WoSubTypeModel.Static)
                            & (SubTipoModelo != WoSubTypeModel.Singleton)
                    )
                        throw new Exception(
                            "Para Tipo Class, SubTipo deber ser Standard, Static o Singleton "
                        );
                }

                bool Primary = false;
                int iPrimaryLength = -1;
                WoTypeColumn PrimaryType = WoTypeColumn.String;

                bool UniqueGet = false;
                bool woState = false;
                List<int> slOrden = new List<int>();
                List<string> slColumna = new List<string>();

                bool[] bColumnasCatalogo = new bool[WoConst.ColumnasCatalogo.Count];
                int indice = 0;
                foreach (var catcol in WoConst.ColumnasCatalogo)
                    bColumnasCatalogo[indice++] = false;

                bool[] bColumnasEsclava = new bool[WoConst.ColumnasEsclava.Count];
                indice = 0;
                foreach (var catcol in WoConst.ColumnasEsclava)
                    bColumnasEsclava[indice++] = false;

                bool[] bColumnasTransaccion = new bool[WoConst.ColumnasTransaccion.Count];
                indice = 0;
                foreach (var catcol in WoConst.ColumnasTransaccion)
                    bColumnasTransaccion[indice++] = false;

                bool[] bColumnasSistema = new bool[WoConst.ColumnasSistema.Count];
                indice = 0;
                foreach (var catcol in WoConst.ColumnasSistema)
                    bColumnasSistema[indice++] = false;

                bool EsVisibleEnLookUpId = false;
                int EsVisibleEnLookUp = 0;

                int iFaltaColumnasLookUps = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if ((dr[@"Modelo"].IsDBNullOrStringEmpty()) || (dr[@"Coleccion"].ToBoolean()))
                        continue;

                    // Existe la columna ya creada de tipo lookup descripción
                    string Columna = $"__{dr[@"Columna"].ToSafeString()}";

                    DataRow[] drRows = dt.Select($"Columna = '{Columna}'");
                    if (drRows.Length == 0)
                        iFaltaColumnasLookUps++;
                }

                if (
                    (TipoModelo == WoTypeModel.CatalogSlave) // Las esclavas no se toma el campo referencia
                    || (TipoModelo == WoTypeModel.TransactionSlave)
                )
                    iFaltaColumnasLookUps--;

                if (iFaltaColumnasLookUps > 0)
                {
                    if (
                        XtraMessageBox.Show(
                            "Faltan columnas de tipo lookup descripción, grabar de cualquier manera?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) != DialogResult.Yes
                    )
                        throw new Exception("Grabación cancelada");
                }

                List<Modelo> interfazes = new List<Modelo>();

                // La columna es parte de alguna interfaz?
                if (
                    (!txtInterface1.EditValue.IsNullOrStringEmpty())
                    || (!txtInterface2.EditValue.IsNullOrStringEmpty())
                    || (!txtInterface3.EditValue.IsNullOrStringEmpty())
                )
                {
                    var principal = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

                    interfazes = principal
                        .ModeloCol.Modelos.Where(x =>
                            (x.Id == txtInterface1.EditValue.ToSafeString())
                            || (x.Id == txtInterface2.EditValue.ToSafeString())
                            || (x.Id == txtInterface3.EditValue.ToSafeString())
                        )
                        .ToList();
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (slOrden.IndexOf(dr[@"Orden"].ToInt32()) != -1)
                        throw new Exception($"Orden {dr[@"Orden"].ToInt32()} se repite");

                    slOrden.Add(dr[@"Orden"].ToInt32());

                    if (dr[@"Columna"].IsDBNull())
                        throw new Exception("Falta registrar nombre columna");

                    var columna = dr[@"Columna"].ToString();
                    var modelo = dr[@"Modelo"].ToString();

                    bool bEsUnaColumanaDeInterfaz = false;

                    foreach (var interfaz in interfazes)
                    {
                        if (interfaz.Columnas.Where(x => x.Id == columna).FirstOrDefault() != null)
                        {
                            bEsUnaColumanaDeInterfaz = true;
                            break;
                        }
                    }

                    bool bEsUnaColumanaLookUpDescription = false;
                    if (columna.StartsWith("__"))
                    {
                        // Busca las columnas con modelos
                        foreach (DataRow drLoc in dt.Rows)
                        {
                            var modeloLoc = drLoc[@"Modelo"].ToSafeString();
                            var columnaLoc = columna.Substring(2);
                            if (columnaLoc.StartsWith(modeloLoc)) // Es una columna de descripción de una referencia
                            // El nombre puede no empezar con el origen
                            {
                                bEsUnaColumanaLookUpDescription = true;
                                break;
                            }
                        }
                    }

                    if (
                        !bEsUnaColumanaDeInterfaz
                        && !bEsUnaColumanaLookUpDescription
                        && WoConst.ColumnasOmitirEnOrigen.IndexOf(columna) == -1
                        && modelo.IsNullOrStringEmpty()
                    )
                    {
                        int start = 0;
                        if (columna.StartsWith("_"))
                            start = 1;

                        if (!columna.Substring(start).StartsWith(wooWConfigParams.Origen))
                        {
                            throw new Exception(
                                $"Nombre de columna {columna} debe comenzar con Origen '{wooWConfigParams.Origen}'"
                            );
                        }
                        else
                        {
                            if (columna.Length < (3 + start))
                                throw new Exception(
                                    $"Nombre de columna {columna} debe tener más de {3 + start} caracteres"
                                );
                            else
                            // El tercer carácter es mayúscula?
                            if (!char.IsUpper(columna[2 + start]))
                                throw new Exception(
                                    $"Nombre de columna {columna} debe tener la tercera letra en mayúscula y comenzar con el origen"
                                );
                        }
                    }

                    if ((columna == WoConst.WORENGLON) && (TipoModelo == WoTypeModel.Complex))
                    {
                        throw new Exception($"Columna {columna} no se puede ocupar en Complex");
                    }

                    if ((!dr[@"Persistente"].ToBoolean()) && (!columna.StartsWith("_")))
                        throw new Exception(
                            $"Columna {columna} no persistentes deben comenzar con '_'"
                        );

                    if (columna.StartsWith("_"))
                    {
                        if (dr[@"Persistente"].ToBoolean())
                            throw new Exception(
                                $"Columna {columna} que comienzan con '_' no puede ser persistente"
                            );

                        if (columna.StartsWith("__"))
                        {
                            string ColumnaModelo = columna.Substring(2);
                            DataTable dtTable = grdColumna.DataSource as DataTable;
                            bool bFound = false;
                            foreach (DataRow drRow in dtTable.Rows)
                            {
                                if (
                                    (!drRow[@"Modelo"].IsNullOrStringEmpty())
                                    && (drRow[@"Columna"].ToSafeString() == ColumnaModelo)
                                )
                                {
                                    bFound = true;
                                    break;
                                }
                            }

                            if (!bFound)
                                throw new Exception(
                                    $"Columna {columna} es descripción de una referencia, debe tener el mismo nombre de la columna de modelo referenciado comenzando con '__'"
                                );

                            // Debe ser TipoCol string no tener modelo y tipo de control no aplica
                            if (
                                (dr[@"TipoCol"].ToSafeString() != WoTypeColumn.String.ToString())
                                || (!dr[@"Modelo"].ToSafeString().IsNullOrStringTrimEmpty())
                                || (dr[@"Coleccion"].ToBoolean())
                                || dr[@"Control"].ToString() != WoTypeControl.NA.ToString()
                                || !dr[@"AceptaNulos"].ToBoolean()
                            )
                                throw new Exception(
                                    $"Columna {columna} que comienzan con '__' debe ser tipo string, no tener modelo, no ser colección, aceptar nulos y tipo de control NoAplica"
                                );
                        }
                    }

                    if (columna == "Id")
                    {
                        if (dr[@"EsVisibleEnLookUp"].ToBoolean())
                            EsVisibleEnLookUpId = true;
                    }
                    else
                    {
                        if (dr[@"EsVisibleEnLookUp"].ToBoolean())
                            EsVisibleEnLookUp++;
                    }

                    if (dr[@"Coleccion"].ToBoolean())
                    {
                        if (!dr[@"Default"].IsNullOrStringEmpty())
                            throw new Exception("Las colecciones no pueden tener default");
                        if (!columna.EndsWith("Col"))
                            throw new Exception("Las colecciones deben terminar en 'Col'");
                    }

                    if (slColumna.IndexOf(columna.ToUpper()) != -1)
                        throw new Exception($"Columna {columna} se repite");

                    slColumna.Add(columna.ToUpper());

                    if (dr[@"Orden"].IsDBNull())
                        throw new Exception($"Falta registrar nombre Orden, columna {columna}");

                    if (dr[@"Descripcion"].IsDBNull())
                        throw new Exception($"Falta registrar Descripción, columna {columna}");

                    if (dr[@"Ayuda"].IsDBNull())
                        throw new Exception($"Falta registrar Ayuda, columna {columna}");

                    if (dr[@"EtiquetaFormulario"].IsDBNull())
                        throw new Exception(
                            $"Falta registrar Etiqueta Formulario, columna {columna}"
                        );

                    if (dr[@"EtiquetaGrid"].IsDBNull())
                        throw new Exception($"Falta registrar Etiqueta Grid, columna {columna}");

                    if (dr[@"TipoCol"].IsDBNull())
                        throw new Exception($"Falta registrar TipoCol, columna {columna}");

                    if (dr[@"TipoDato"].IsDBNull())
                        throw new Exception($"Falta registrar TipoDato, columna {columna}");

                    if (dr[@"Control"].IsDBNull())
                        throw new Exception($"Falta registrar Control, columna {columna}");

                    indice = WoConst.ColumnasTransaccion.IndexOf(columna);
                    if (indice > -1)
                    {
                        bColumnasTransaccion[indice] = true;
                        if (!dr[@"Persistente"].ToBoolean())
                            throw new Exception($"Columna {columna} debe ser persistente");
                        if (dr[@"Coleccion"].ToBoolean())
                            throw new Exception($"Columna {columna} no puede ser colección");
                    }

                    indice = WoConst.ColumnasCatalogo.IndexOf(columna);
                    if (indice > -1)
                    {
                        bColumnasCatalogo[indice] = true;
                        if (!dr[@"Persistente"].ToBoolean())
                            throw new Exception($"Columna {columna} debe ser persistente");
                        if (dr[@"Coleccion"].ToBoolean())
                            throw new Exception($"Columna {columna} no puede ser colección");
                    }

                    indice = WoConst.ColumnasEsclava.IndexOf(columna);
                    if (indice > -1)
                    {
                        bColumnasEsclava[indice] = true;
                        if (!dr[@"Persistente"].ToBoolean())
                            throw new Exception($"Columna {columna} debe ser persistente");
                        if (dr[@"Coleccion"].ToBoolean())
                            throw new Exception($"Columna {columna} no puede ser colección");
                    }

                    indice = WoConst.ColumnasSistema.IndexOf(columna);
                    if (indice > -1)
                    {
                        bColumnasSistema[indice] = true;
                        if (!dr[@"Persistente"].ToBoolean())
                            throw new Exception($"Columna {columna} debe ser persistente");
                        if (dr[@"Coleccion"].ToBoolean())
                            throw new Exception($"Columna {columna} no puede ser colección");
                    }

                    var TipoControl = (WoTypeControl)
                        Enum.Parse(typeof(WoTypeControl), dr[@"Control"].ToString());

                    var TipoColumna = (WoTypeColumn)
                        Enum.Parse(typeof(WoTypeColumn), dr[@"TipoCol"].ToString());

                    var TipoDato = (WoTypeData)
                        Enum.Parse(typeof(WoTypeData), dr[@"TipoDato"].ToString());

                    if ((columna == WoConst.WORENGLON) && (TipoColumna != WoTypeColumn.Long))
                        throw new Exception($"Columna {columna} debe ser long");

                    if (
                        (
                            (TipoColumna == WoTypeColumn.EnumInt)
                            || (TipoColumna == WoTypeColumn.EnumString)
                        ) && (dr[@"AceptaNulos"].ToBoolean())
                    )
                        throw new Exception("Los Enum no pueden aceptar nulos");

                    if (columna == "WoState")
                    {
                        if (
                            (TipoColumna != WoTypeColumn.WoState)
                            || (TipoControl != WoTypeControl.WoState)
                        )
                            throw new Exception(
                                $"La columna WoState debe ser de tipo y control WoState"
                            );
                        woState = true;

                        if (!dr[@"Persistente"].ToBoolean())
                            throw new Exception($"Columna {columna} debe ser persistente");
                        if (dr[@"Coleccion"].ToBoolean())
                            throw new Exception($"Columna {columna} no puede ser colección");
                    }
                    else
                    {
                        if (
                            (TipoColumna == WoTypeColumn.WoState)
                            || (TipoControl == WoTypeControl.WoState)
                        )
                            throw new Exception(
                                $"Solo la columna WoState puede ser de tipo y control WoState, columna {columna}"
                            );
                    }

                    if ((dr[@"Coleccion"].ToBoolean()) && (TipoColumna == WoTypeColumn.Reference))
                    {
                        if (dr[@"Modelo"].IsNullOrStringEmpty())
                            throw new Exception(
                                $"Una colección de tipo referencia debe lleva modelo, columna {columna}"
                            );

                        // si el tipo de modelo es catalogo la referencia debe se catalogodetalle
                        // si el tipo de modelo es transaccion la referencia debes ser transacciondetalle
                        Modelo mod = proyecto
                            .ModeloCol.Modelos.Where(j => j.Id == dr[@"Modelo"].ToString())
                            .FirstOrDefault();

                        if (TipoModelo == WoTypeModel.Catalog)
                        {
                            if (mod.TipoModelo != WoTypeModel.CatalogSlave)
                                throw new Exception(
                                    $"Una colección de tipo colección y referencia, para un catalogo, el modelo debe ser catalogo slave {columna} o ocupe complejos en lugar de referencias"
                                );
                        }
                        else if (
                            (TipoModelo == WoTypeModel.TransactionContable)
                            || (TipoModelo == WoTypeModel.TransactionNoContable)
                        )
                        {
                            if (mod.TipoModelo != WoTypeModel.TransactionSlave)
                                throw new Exception(
                                    $"Una coleccion de tipo colección y referencia, para una transacción, el modelo debe ser transaccion slave {columna} o ocupe complejos en lugar de referencias"
                                );
                        }
                        else
                        {
                            throw new Exception(
                                "Solamente los catálogos y las transacciones pueden tener columnas de tipo colección y sean referencia "
                            );
                        }

                        var col = mod
                            .Columnas.Where(e => e.Id == txtModelo.EditValue.ToString() + "Id")
                            .FirstOrDefault();

                        if (col == null)
                            throw new Exception(
                                $"El modelo {mod.Id} deber tener una referencia a este modelo para poder enlazar la coleción, columna {columna}"
                            );

                        if (
                            col.EsColeccion
                            || col.TipoColumna != WoTypeColumn.Reference
                            || col.ModeloId != txtModelo.EditValue.ToString()
                        )
                            throw new Exception(
                                $"La columna {col} en modelo {mod.Id} no cumple con: no ser colección, tipo de columna reference y tener el modelo {txtModelo.EditValue.ToString()}, columna {columna}"
                            );
                    }

                    if (TipoDato == WoTypeData.UniqueGet)
                        UniqueGet = true;

                    if (
                        (
                            (TipoColumna == WoTypeColumn.EnumInt)
                            && (
                                (TipoControl != WoTypeControl.EnumInt)
                                && (TipoControl != WoTypeControl.NA)
                            )
                        )
                    )
                    {
                        throw new Exception(
                            $"Los tipos de columna enum debe tener control enumint o NA, columna {columna}"
                        );
                    }

                    if (
                        (
                            (TipoColumna == WoTypeColumn.EnumString)
                            && (
                                (TipoControl != WoTypeControl.EnumString)
                                && (TipoControl != WoTypeControl.NA)
                            )
                        )
                    )
                    {
                        throw new Exception(
                            $"Los tipos de columna enum debe tener control enumstring o NA, columna {columna}"
                        );
                    }

                    if (TipoColumna == WoTypeColumn.EnumInt)
                    {
                        if (!dr[@"Modelo"].IsNullOrStringTrimEmpty())
                        {
                            if (!dr[@"Propiedades"].IsNullOrStringTrimEmpty())
                                throw new Exception(
                                    $"Los tipos de columna enum que dependen de un modelo no se les debe registrar propiedades, columna {columna}"
                                );
                        }
                        else
                        {
                            if (dr[@"Propiedades"].IsNullOrStringTrimEmpty())
                                throw new Exception(
                                    $"Los tipos de columna enum se les debe registrar propiedades, columna {columna}"
                                );

                            try
                            {
                                var prop = JsonConvert.DeserializeObject<TypeControlEnumInt>(
                                    dr[@"Propiedades"].ToSafeString()
                                );

                                if (prop.Items.Count == 0)
                                    throw new Exception(
                                        $"Los tipos de columna enum se les debe registrar propiedades, columna {columna}"
                                    );
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(
                                    $"error al tratar de deserealizar las propiedades, columna {columna} del enum "
                                        + ex.Message
                                );
                            }
                        }
                    }

                    if (TipoColumna == WoTypeColumn.EnumString)
                    {
                        if (!dr[@"Modelo"].IsNullOrStringTrimEmpty())
                        {
                            if (!dr[@"Propiedades"].IsNullOrStringTrimEmpty())
                                throw new Exception(
                                    $"Los tipos de columna enum que dependen de un modelo no se les debe registrar propiedades, columna {columna}"
                                );
                        }
                        else
                        {
                            if (dr[@"Propiedades"].IsNullOrStringTrimEmpty())
                                throw new Exception(
                                    $"Los tipos de columna enum se les debe registrar propiedades, columna {columna}"
                                );

                            try
                            {
                                var prop = JsonConvert.DeserializeObject<TypeControlEnumString>(
                                    dr[@"Propiedades"].ToSafeString()
                                );

                                if (prop.Items.Count == 0)
                                    throw new Exception(
                                        $"Los tipos de columna enum se les debe registrar propiedades, columna {columna}"
                                    );
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(
                                    $"error al tratar de deserealizar las propiedades, columna {columna} del enum "
                                        + ex.Message
                                );
                            }
                        }
                    }

                    if (
                        (TipoColumna == WoTypeColumn.String)
                        || (TipoColumna == WoTypeColumn.Decimal)
                    )
                    {
                        if (dr[@"Longitud"].ToInt32() == 0)
                            throw new Exception(
                                $"Registre longitud para tipo string o decimal, columna {columna}"
                            );

                        if (
                            (TipoColumna == WoTypeColumn.String)
                            && (dr[@"Longitud"].ToInt32() > 1024)
                        )
                            throw new Exception(
                                $"La longitud máxima para una string es 1024, columna {columna}"
                            );

                        if (
                            (TipoColumna == WoTypeColumn.Decimal)
                            && (dr[@"Longitud"].ToInt32() > 28)
                        )
                            throw new Exception(
                                $"La longitud máxima para una decimal es 28, columna {columna}"
                            );
                    }
                    else
                    {
                        if (dr[@"Longitud"].ToInt32() != 0)
                            throw new Exception(
                                $"No Registre longitud para tipo diferentes a string o decimal, columna {columna}"
                            );
                    }

                    if (TipoColumna == WoTypeColumn.Decimal)
                    {
                        if (dr[@"Precision"].ToInt32() == 0)
                            throw new Exception(
                                $"Registre precisión para columnas decimal, columna {columna}"
                            );

                        if (
                            (TipoColumna == WoTypeColumn.Decimal)
                            && (dr[@"Precision"].ToInt32() > 10)
                        )
                            throw new Exception(
                                $"La precisión máxima para una decimal es 10, columna {columna}"
                            );
                    }
                    else
                    {
                        if (dr[@"Precision"].ToInt32() != 0)
                            throw new Exception(
                                $"No Registre precisión que sean diferentes a decimal, columna {columna}"
                            );
                    }

                    if (dr[@"Primaria"].ToBoolean())
                    {
                        if (columna != "Id")
                            throw new Exception("Las columnas primarias solo pueden llamarse Id");
                        if (Primary)
                            throw new Exception("Solo puede haber una columna primaria");
                        if (dr[@"Coleccion"].ToBoolean())
                            throw new Exception($"Columna {columna} no puede ser colección");

                        iPrimaryLength = dr[@"Longitud"].ToInt32();
                        PrimaryType = TipoColumna;

                        if (
                            (TipoModelo == WoTypeModel.Control)
                            || (TipoModelo == WoTypeModel.Kardex)
                            || (TipoModelo == WoTypeModel.DataMart)
                            || (TipoModelo == WoTypeModel.Parameter)
                        )
                        {
                            if (TipoColumna != WoTypeColumn.Autoincrement)
                                throw new Exception(
                                    "Columna solo puede pueden ser autoincrement si es primaria"
                                );
                        }
                        else
                        {
                            if ((TipoColumna != WoTypeColumn.String))
                                throw new Exception(
                                    "Columna solo puede pueden ser string si es primaria"
                                );
                        }
                        if (dr[@"AceptaNulos"].ToBoolean())
                            throw new Exception("Las primarias no pueden aceptar nulos");

                        Primary = true;

                        if (!dr[@"Persistente"].ToBoolean())
                            throw new Exception($"Columna {columna} debe ser persistente");
                    }
                    else
                    {
                        if (TipoColumna == WoTypeColumn.Autoincrement)
                            throw new Exception(
                                $"Solo las primarias puede ser de tipo autoincrement, columna {columna}"
                            );

                        if (
                            (columna == "Id")
                            && (
                                (TipoModelo == WoTypeModel.Configuration)
                                || (TipoModelo == WoTypeModel.CatalogType)
                                || (TipoModelo == WoTypeModel.Catalog)
                                || (TipoModelo == WoTypeModel.TransactionContable)
                                || (TipoModelo == WoTypeModel.TransactionNoContable)
                                || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                                || (TipoModelo == WoTypeModel.Control)
                                || (TipoModelo == WoTypeModel.Kardex)
                                || (TipoModelo == WoTypeModel.Parameter)
                            )
                        )
                            throw new Exception($"La columna Id debe ser primaria");
                    }

                    if (!dr[@"Modelo"].IsNullOrStringTrimEmpty())
                    {
                        var principal = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

                        Modelo mod = principal
                            .ModeloCol.Modelos.Where(j => j.Id == dr[@"Modelo"].ToString())
                            .FirstOrDefault();

                        if (mod == null)
                        {
                            throw new Exception($"Modelo {dr[@"Modelo"].ToString()} no existe");
                        }

                        if (
                            (TipoColumna != WoTypeColumn.Reference)
                            && (TipoColumna != WoTypeColumn.Complex)
                            && (TipoColumna != WoTypeColumn.EnumInt)
                            && (TipoColumna != WoTypeColumn.EnumString)
                        )
                            throw new Exception(
                                $"Cuando registra un modelo el tipo de columna debe ser Reference o Complex o EnumInt o EnumString"
                            );

                        if (
                            (TipoColumna == WoTypeColumn.EnumInt)
                            || (TipoColumna == WoTypeColumn.EnumString)
                        )
                        {
                            string sColBuscar = dr[@"Columna"].ToSafeString();
                            if (dr[@"Coleccion"].ToBoolean())
                            {
                                if (!sColBuscar.EndsWith("Col"))
                                    throw new Exception(
                                        $"Columna {dr[@"Columna"].ToSafeString()} debe terminar en Col"
                                    );
                                sColBuscar = sColBuscar.Substring(0, sColBuscar.Length - 3);
                            }

                            ModeloColumna col = mod
                                .Columnas.Where(j =>
                                    (j.Id == sColBuscar)
                                    || (j.EsColeccion && (j.Id == sColBuscar + "Col"))
                                )
                                .FirstOrDefault();

                            if (col == null)
                            {
                                throw new Exception(
                                    $"El modelo {dr[@"Modelo"].ToSafeString()} no tiene la columna {sColBuscar}"
                                );
                            }

                            if (
                                (col.TipoColumna != WoTypeColumn.EnumInt)
                                && (col.TipoColumna != WoTypeColumn.EnumString)
                            )
                            {
                                throw new Exception(
                                    $"En el modelo {dr[@"Modelo"].ToSafeString()} la columna {dr[@"Columna"].ToSafeString()} no es EnumInt o EnumString"
                                );
                            }

                            if ((col.TipoColumna != TipoColumna))
                            {
                                throw new Exception(
                                    $"En el modelo {dr[@"Modelo"].ToSafeString()} la columna {dr[@"Columna"].ToSafeString()} el tipo es diferente"
                                );
                            }
                        }
                        else
                        {
                            string NombreCorrecto;

                            if (dr[@"Coleccion"].ToBoolean())
                                NombreCorrecto = dr[@"Modelo"].ToSafeString() + "Col";
                            else if (TipoColumna == WoTypeColumn.Complex)
                                NombreCorrecto = dr[@"Modelo"].ToSafeString();
                            else
                                NombreCorrecto = dr[@"Modelo"].ToSafeString() + "Id";

                            if (
                                (dr[@"Columna"].ToSafeString() != NombreCorrecto)
                                && (
                                    (
                                        !dr[@"Columna"]
                                            .ToSafeString()
                                            .StartsWith(NombreCorrecto + "_")
                                    )
                                    || (
                                        dr[@"Columna"].ToSafeString().Length
                                        <= (NombreCorrecto + "_").Length
                                    )
                                )
                            )
                                throw new Exception(
                                    $"La columna debe llamarse {NombreCorrecto} o comenzar con {NombreCorrecto}_ para múltiples referencias al mismo modelo "
                                );

                            if (dr[@"Coleccion"].ToBoolean())
                            {
                                if (
                                    (TipoControl != WoTypeControl.Custom)
                                    && (TipoControl != WoTypeControl.CollectionEditor)
                                    && (TipoControl != WoTypeControl.NA)
                                )
                                    throw new Exception(
                                        $"Tipo de Columna Collection debe tener Tipo de Control Custom o CollectionEditor o NA"
                                    );
                            }
                            else if ((TipoColumna == WoTypeColumn.Reference))
                            {
                                if (
                                    (TipoControl != WoTypeControl.Text)
                                    && (TipoControl != WoTypeControl.LookUp)
                                    && (TipoControl != WoTypeControl.LookUpDialog)
                                    && (TipoControl != WoTypeControl.NA)
                                )
                                    throw new Exception(
                                        $"Tipo de Columna Reference debe tener Tipo de Control Text o Lookup o LookupDialog o NA"
                                    );
                            }
                            else
                            {
                                if (
                                    (TipoControl != WoTypeControl.Custom)
                                    && (TipoControl != WoTypeControl.NA)
                                )
                                    throw new Exception(
                                        $"Tipo de Columna Complex o debe tener Tipo de Control Custom o NA"
                                    );
                            }
                        }
                    }
                    else
                    {
                        if (
                            (TipoColumna == WoTypeColumn.Reference)
                            || (TipoColumna == WoTypeColumn.Complex)
                        )
                            throw new Exception(
                                $"Solamente tipo de columna Reference o Collection o Complex pueden llevar modelo"
                            );

                        if (
                            (TipoControl == WoTypeControl.Text)
                            && (TipoControl == WoTypeControl.LookUp)
                            && (TipoControl == WoTypeControl.LookUpDialog)
                            && (TipoControl == WoTypeControl.CollectionEditor)
                        )
                            throw new Exception(
                                $"Tipo de control Lookup, LookUpDialog, CollectionEditor solo se ocupa en columnas de tipo Reference o Collection o Complex"
                            );
                    }

                    if (TipoControl == WoTypeControl.Urn)
                    {
                        if (
                            (TipoColumna != WoTypeColumn.String)
                            || (dr[@"Longitud"].ToInt32() != 256)
                        )
                            throw new Exception($"Tipo de Control Urn deben ser string de 256");
                    }

                    if (
                        (TipoControl == WoTypeControl.Spin)
                        && (
                            (TipoColumna != WoTypeColumn.Smallint)
                            && (TipoColumna != WoTypeColumn.Integer)
                            && (TipoColumna != WoTypeColumn.Long)
                        )
                    )
                    {
                        throw new Exception(
                            $"Tipo de Control Spin deben ser de tipo smallint, int o long"
                        );
                    }
                }

                if (
                    (TipoModelo == WoTypeModel.Configuration)
                    || (TipoModelo == WoTypeModel.CatalogType)
                    || (TipoModelo == WoTypeModel.Catalog)
                    || (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                    || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                )
                {
                    if (!EsVisibleEnLookUpId)
                        throw new Exception(
                            $"Seleccione el campo Id para la columna 'Es Visible En LookUp?'"
                        );
                    if (EsVisibleEnLookUp != 1)
                        throw new Exception(
                            $"Seleccione solo un renglón, aparte de Id en columna 'Es Visible En LookUp?'"
                        );
                }
                else
                {
                    if (EsVisibleEnLookUp != 0)
                        throw new Exception($"El tipo de modelo no ocupa 'Es Visible En LookUp?'");
                }

                if (
                    (TipoModelo == WoTypeModel.Configuration)
                    || (TipoModelo == WoTypeModel.CatalogType)
                    || (TipoModelo == WoTypeModel.Catalog)
                    || (TipoModelo == WoTypeModel.CatalogSlave)
                    || (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                    || (TipoModelo == WoTypeModel.TransactionSlave)
                    || (TipoModelo == WoTypeModel.Control)
                    || (TipoModelo == WoTypeModel.Kardex)
                    || (TipoModelo == WoTypeModel.DataMart)
                    || (TipoModelo == WoTypeModel.Parameter)
                )
                {
                    if (!Primary)
                        throw new Exception("Falta columna primaria");

                    if (
                        (TipoModelo == WoTypeModel.CatalogSlave)
                        || (TipoModelo == WoTypeModel.TransactionSlave)
                    )
                    {
                        if (iPrimaryLength < 40)
                            throw new Exception(
                                "La longitud de la llave primaria debe ser mayor o igual a 40"
                            );
                        if (PrimaryType != WoTypeColumn.String)
                            throw new Exception("El tipo de la llave primaria debe ser string");
                    }
                }
                else
                {
                    if (Primary)
                        throw new Exception("Modelo no necesita primaria");
                }

                if (
                    (TipoModelo == WoTypeModel.Configuration)
                    || (TipoModelo == WoTypeModel.CatalogType)
                    || (TipoModelo == WoTypeModel.Catalog)
                    || (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                    || (TipoModelo == WoTypeModel.Control)
                    || (TipoModelo == WoTypeModel.Kardex)
                    || (TipoModelo == WoTypeModel.DataMart)
                    || (TipoModelo == WoTypeModel.Parameter)
                )
                {
                    if (!woState)
                        throw new Exception("Modelo requiere la columna WoState");
                }

                if (
                    (TipoModelo == WoTypeModel.Complex)
                    || (TipoModelo == WoTypeModel.Interface)
                    || (TipoModelo == WoTypeModel.Request)
                    || (TipoModelo == WoTypeModel.Response)
                    || (TipoModelo == WoTypeModel.CatalogSlave)
                    || (TipoModelo == WoTypeModel.TransactionSlave)
                )
                {
                    indice = 0;
                    foreach (var catcol in WoConst.ColumnasSistema)
                    {
                        if (bColumnasSistema[indice])
                            throw new Exception($"Sobra la columna {catcol}");
                        indice++;
                    }
                }

                if ((TipoModelo == WoTypeModel.CatalogType) || (TipoModelo == WoTypeModel.Catalog))
                {
                    indice = 0;
                    foreach (var catcol in WoConst.ColumnasCatalogo)
                    {
                        if (!bColumnasCatalogo[indice])
                            throw new Exception($"Falta la columna obligatoria {catcol}");
                        indice++;
                    }
                }
                else
                {
                    indice = 0;
                    foreach (var catcol in WoConst.ColumnasCatalogo)
                    {
                        if (bColumnasCatalogo[indice])
                            throw new Exception($"Sobra la columna {catcol}");
                        indice++;
                    }

                    if (
                        (Repositorio == TypeRepository.General)
                        && (TipoModelo != WoTypeModel.Configuration)
                    )
                        throw new Exception(
                            $"Repositorio solo puede ser tipo de modelo: configuración, catalogo o catalogo esclava"
                        );
                }

                if (
                    (TipoModelo == WoTypeModel.CatalogSlave)
                    || (TipoModelo == WoTypeModel.TransactionSlave)
                )
                {
                    indice = 0;
                    foreach (var catcol in WoConst.ColumnasEsclava)
                    {
                        if (!bColumnasEsclava[indice])
                            throw new Exception($"Falta la columna obligatoria {catcol}");
                        indice++;
                    }
                }
                else
                {
                    indice = 0;
                    foreach (var catcol in WoConst.ColumnasEsclava)
                    {
                        if (bColumnasEsclava[indice])
                            throw new Exception($"Sobra la columna {catcol}");
                        indice++;
                    }
                }

                if (
                    (TipoModelo == WoTypeModel.Control)
                    || (TipoModelo == WoTypeModel.Kardex)
                    || (TipoModelo == WoTypeModel.DataMart)
                )
                {
                    if (!UniqueGet)
                        throw new Exception("Debe marcar una o mas columnas como UniqueGet");
                }

                if (
                    (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                )
                {
                    indice = 0;
                    foreach (var catcol in WoConst.ColumnasTransaccion)
                    {
                        if (!bColumnasTransaccion[indice])
                            throw new Exception($"Falta la columna obligatoria {catcol}");
                        indice++;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                XtraMessageBox.Show(
                    e.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
        }

        private void buRegistrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (
                (txtModelo.EditValue.IsNullOrStringTrimEmpty())
                || (
                    txtTipoModelo.EditValue.IsNullOrStringTrimEmpty()
                    || (txtSubTipoModelo.EditValue.IsNullOrStringTrimEmpty())
                )
            )
            {
                XtraMessageBox.Show(
                    $"Faltan registrar nombre del modelo o tipo o subtipo",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var SubTipoModelo = EnumExtensionMethods.GetValueFromDescription<WoSubTypeModel>(
                txtSubTipoModelo.EditValue.ToString()
            );

            if (SubTipoModelo == WoSubTypeModel.Extension)
            {
                // Comprobar que no tenga primarias
                foreach (DataRow dr in (grdColumna.DataSource as DataTable).Rows)
                {
                    if (dr[@"Primaria"].ToBoolean())
                    {
                        XtraMessageBox.Show(
                            $"Modelos extension no pueden tener columnas primarias",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }
                    if (!dr[@"AceptaNulos"].ToBoolean() && dr[@"AceptaNulos"].IsNullOrStringEmpty())
                    {
                        XtraMessageBox.Show(
                            $"Modelos extensión sus propiedades debe aceptar null o tener valor default",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }
                }
            }
            else if (SubTipoModelo == WoSubTypeModel.Override)
            {
                if (txtDescripcion.EditValue.IsNullOrStringTrimEmpty())
                {
                    XtraMessageBox.Show(
                        $"Faltan datos que registrar",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (txtApps.Properties.GetCheckedItems().IsNullOrStringEmpty())
                {
                    XtraMessageBox.Show(
                        $"Registre apps",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }
            else
            {
                if (
                    (txtDescripcion.EditValue.IsNullOrStringTrimEmpty())
                    || (txtProceso.EditValue.IsNullOrStringTrimEmpty())
                    || (txtFiltro.EditValue.IsNullOrStringTrimEmpty())
                )
                {
                    XtraMessageBox.Show(
                        $"Faltan datos que registrar",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (txtApps.Properties.GetCheckedItems().IsNullOrStringEmpty())
                {
                    XtraMessageBox.Show(
                        $"Registre apps",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                try
                {
                    if (!ValidarColumnas())
                        return;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                bool InterfacesRepetidas = false;

                if (txtInterface1.EditValue != null)
                {
                    if (
                        (
                            txtInterface1.EditValue.ToSafeString()
                            == txtInterface2.EditValue.ToSafeString()
                        )
                        || (
                            txtInterface1.EditValue.ToSafeString()
                            == txtInterface3.EditValue.ToSafeString()
                        )
                    )
                        InterfacesRepetidas = true;
                }
                else if (txtInterface2.EditValue != null)
                {
                    if (
                        txtInterface2.EditValue.ToSafeString()
                        == txtInterface3.EditValue.ToSafeString()
                    )
                        InterfacesRepetidas = true;
                }

                if (SubTipoModelo == WoSubTypeModel.Static)
                {
                    if (
                        (!txtInterface1.EditValue.IsNullOrStringEmpty())
                        || (!txtInterface2.EditValue.IsNullOrStringEmpty())
                        || (!txtInterface2.EditValue.IsNullOrStringEmpty())
                    )
                    {
                        XtraMessageBox.Show(
                            $"Clases de subtipo static no permiten interfaces",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }
                }

                if (InterfacesRepetidas)
                {
                    XtraMessageBox.Show(
                        $"No puede seleccionar dos veces la misma interface",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (txtInterface1.EditValue != null)
                    if (!ValidarInterface(txtInterface1.EditValue.ToString()))
                        return;

                if (txtInterface2.EditValue != null)
                    if (!ValidarInterface(txtInterface2.EditValue.ToString()))
                        return;

                if (txtInterface3.EditValue != null)
                    if (!ValidarInterface(txtInterface3.EditValue.ToString()))
                        return;
            }

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                grdModelosView_FocusedRowChanged(null, null);
                return;
            }

            Modelo modelo;
            Modelo modeloBak;

            if (bNuevo)
            {
                var m = proyecto
                    .ModeloCol.Modelos.Where(j => j.Id == txtModelo.EditValue.ToString())
                    .FirstOrDefault();

                if (m != null)
                {
                    XtraMessageBox.Show(
                        $"Nombre de modelo se repite",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                modelo = new Modelo(proyecto) { Id = txtModelo.EditValue.ToString(), };
                modeloBak = new Modelo(proyecto);
            }
            else
            {
                modelo = Modelo.FromJson(drRow[@"Json"].ToString(), proyecto);
                modeloBak = Modelo.FromJson(drRow[@"Json"].ToString(), proyecto);
            }

            modelo.TipoModelo = EnumExtensionMethods.GetValueFromDescription<WoTypeModel>(
                txtTipoModelo.EditValue.ToString()
            );

            modelo.SubTipoModelo = EnumExtensionMethods.GetValueFromDescription<WoSubTypeModel>(
                txtSubTipoModelo.EditValue.ToString()
            );

            modelo.Filtro = EnumExtensionMethods.GetValueFromDescription<woGetListFilterType>(
                txtFiltro.EditValue.ToString()
            );

            modelo.Repositorio = TypeRepository.User;
            ////    EnumExtensionMethods.GetValueFromDescription<TypeRepository>(
            ////    txtSubTipoModelo.EditValue.ToString()
            ////);

            modelo.EtiquetaId = txtDescripcion.EditValue.ToString();
            modelo.ProcesoId = txtProceso.EditValue.ToString();
            modelo.OrdenDeCreacion = txtOrdenDeCreacion.EditValue.ToInt32();
            modelo.Legacy = txtLegacy.EditValue.ToString();
            modelo.FechaActualizacion = DateTime.Now;

            modelo.Interface1 = (
                txtInterface1.EditValue.IsNullOrStringEmpty()
                    ? null
                    : txtInterface1.EditValue.ToSafeString()
            );
            modelo.Interface2 = (
                txtInterface2.EditValue.IsNullOrStringEmpty()
                    ? null
                    : txtInterface2.EditValue.ToSafeString()
            );
            modelo.Interface3 = (
                txtInterface3.EditValue.IsNullOrStringEmpty()
                    ? null
                    : txtInterface3.EditValue.ToSafeString()
            );

            modelo.Apps.Clear();
            foreach (CheckedListBoxItem item in txtApps.Properties.Items)
            {
                if (item.CheckState == CheckState.Checked)
                    modelo.Apps.Add(new ModeloApp() { AppId = item.Value.ToString() });
            }

            modelo.Columnas.Clear();

            if (
                (
                    (
                        (modelo.TipoModelo == WoTypeModel.Configuration)
                        || (modelo.TipoModelo == WoTypeModel.CatalogType)
                        || (modelo.TipoModelo == WoTypeModel.Catalog)
                        || (modelo.TipoModelo == WoTypeModel.CatalogSlave)
                        || (modelo.TipoModelo == WoTypeModel.TransactionContable)
                        || (modelo.TipoModelo == WoTypeModel.TransactionNoContable)
                        || (modelo.TipoModelo == WoTypeModel.TransactionSlave)
                        || (modelo.TipoModelo == WoTypeModel.TransactionFreeStyle)
                        || (modelo.TipoModelo == WoTypeModel.Control)
                        || (modelo.TipoModelo == WoTypeModel.Kardex)
                        || (modelo.TipoModelo == WoTypeModel.DataMart)
                        || (modelo.TipoModelo == WoTypeModel.Parameter)
                    )
                    && (
                        (modelo.SubTipoModelo != WoSubTypeModel.Extension)
                        && (modelo.SubTipoModelo != WoSubTypeModel.Override)
                    )
                ) || (modelo.TipoModelo == WoTypeModel.View)
            )
            {
                if (txtOrdenDeCreacion.EditValue.ToInt32() == -1)
                {
                    XtraMessageBox.Show(
                        $"Registre un Orden de Creación",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }
            else
            {
                if (txtOrdenDeCreacion.EditValue.ToInt32() != -1)
                    XtraMessageBox.Show(
                        $"Modelo no requiere Orden de Creación, se pondrá en -1",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                modelo.OrdenDeCreacion = -1;
            }

            if (
                (modelo.TipoModelo == WoTypeModel.Request)
                || (modelo.TipoModelo == WoTypeModel.View)
            )
            {
                ////foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                ////{
                ////    if (i.CheckState == CheckState.Checked)
                ////        modelo.ScriptVistaRoles.Roles.Add(i.Value.ToString());
                ////}

                ////foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                ////{
                ////    if (i.CheckState == CheckState.Checked)
                ////        modelo.ScriptVistaRoles.Permisos.Add(i.Value.ToString());
                ////}

                modelo.ScriptVistaRoles.Roles.Clear();
                modelo.ScriptVistaRoles.Permisos.Clear();

                bool RolesNoValidar = false;
                foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                {
                    if (i.CheckState == CheckState.Checked)
                    {
                        modelo.ScriptVistaRoles.Roles.Add(i.Value.ToString());
                        if (i.Value.ToString() == Proyecto.NOVALIDAR)
                            RolesNoValidar = true;
                    }
                }

                bool PermisosNoValidar = false;
                foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                {
                    if (i.CheckState == CheckState.Checked)
                    {
                        modelo.ScriptVistaRoles.Permisos.Add(i.Value.ToString());
                        if (i.Value.ToString() == Proyecto.NOVALIDAR)
                            PermisosNoValidar = true;
                    }
                }

                if (
                    (RolesNoValidar && (modelo.ScriptVistaRoles.Roles.Count != 1))
                    || (PermisosNoValidar && (modelo.ScriptVistaRoles.Permisos.Count != 1))
                )
                {
                    XtraMessageBox.Show(
                        $"Si selecciona {Proyecto.NOVALIDAR} en roles o permisos, selecciónelo solo ",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                if (
                    (
                        (modelo.ScriptVistaRoles.Roles.Count == 0)
                        || (modelo.ScriptVistaRoles.Permisos.Count == 0)
                    )
                    && (
                        (modelo.SubTipoModelo != WoSubTypeModel.Extension)
                        && (modelo.SubTipoModelo != WoSubTypeModel.Override)
                    )
                )
                {
                    XtraMessageBox.Show(
                        "Registre roles y permisos",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            DataTable dt = grdColumna.DataSource as DataTable;
            // Traspasa renglones
            foreach (DataRow dr in dt.Rows)
            {
                ModeloColumna col = new ModeloColumna(proyecto)
                {
                    Orden = dr[@"Orden"].ToInt32(),
                    Id = dr[@"Columna"].ToString().Trim(),
                    TipoColumna = (WoTypeColumn)
                        Enum.Parse(typeof(WoTypeColumn), dr[@"TipoCol"].ToString()),
                    EsColeccion = dr[@"Coleccion"].ToBoolean(),
                    Longitud = dr[@"Longitud"].ToInt32(),
                    ModeloId = dr[@"Modelo"].ToString().Trim(),
                    Precision = dr[@"Precision"].ToInt32(),
                    TipoDato = (WoTypeData)
                        Enum.Parse(typeof(WoTypeData), dr[@"TipoDato"].ToString()),
                    Nulo = dr[@"AceptaNulos"].ToBoolean(),
                    EsVisibleEnLookUp = dr[@"EsVisibleEnLookUp"].ToBoolean(),
                    Persistente = dr[@"Persistente"].ToBoolean(),
                    Primaria = dr[@"Primaria"].ToBoolean(),
                    Default = dr[@"Default"].ToString(),
                    TipoControl = (WoTypeControl)
                        Enum.Parse(typeof(WoTypeControl), dr[@"Control"].ToString()),
                    TipoControlPropiedades = dr[@"Propiedades"].ToString(),
                    Legacy = dr[@"Legacy"].ToString().Trim(),
                    Origen = wooWConfigParams.Origen
                };

                col.Descripcion = dr[@"Descripcion"].ToString();
                col.Ayuda = dr[@"Ayuda"].ToString();
                col.Formulario = dr[@"EtiquetaFormulario"].ToString();
                col.Grid = dr[@"EtiquetaGrid"].ToString();

                foreach (string App in dr[@"Apps"].ToString().Split(','))
                {
                    if (proyecto.Apps.Where(a => a.Id == App.Trim()).FirstOrDefault() != null)
                        col.Apps.Add(new ModeloApp() { AppId = App.Trim() });
                }

                modelo.Columnas.Add(col);
            }

            drRow[@"Proceso"] = modelo.ProcesoId;
            drRow[@"Tipo"] = modelo.TipoModelo.ToString();
            drRow[@"SubTipo"] = modelo.SubTipoModelo.ToString();
            drRow[@"Repositorio"] = modelo.Repositorio.ToString();
            drRow[@"OrdenCreacion"] = modelo.OrdenDeCreacion.ToInt32();
            drRow[@"Modelo"] = modelo.Id;
            drRow[@"Json"] = modelo.ToJson();

            proyecto.AddModel(modelo);

            grdModelosView.UpdateCurrentRow();

            grdModelos.Refresh();
            CargaModelos();
            CargarColumnas();

            (this.Parent as fmMain).RefrescarLogicaOScript();
            grdModelosView_FocusedRowChanged(null, null);

            bNuevo = false;
        }

        private void buBorrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string CambiosPendientes = string.Empty;
            if ((this.Parent as fmMain).CambiosPendientesLogicaOScript(out CambiosPendientes))
            {
                XtraMessageBox.Show(
                    "No se puede editar si tiene cambios por aplicar en " + CambiosPendientes,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            DataRow dr = grdModelosView.GetFocusedDataRow();

            if (dr != null)
            {
                if (
                    XtraMessageBox.Show(
                        "Borrar el modelo?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes
                )
                    return;

                try
                {
                    proyecto.DeleteModel(dr[@"Modelo"].ToString());

                    (this.Parent as fmMain).RefrescarLogicaOScript();
                    Cargar();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }

            CargaModelos();
            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buAnular_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (
                XtraMessageBox.Show(
                    "Anular los cambios al modelo?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            if (bNuevo)
                grdModelosView.DeleteRow(grdModelosView.FocusedRowHandle);

            bNuevo = false;

            CargaModelos();
            grdModelosView_FocusedRowChanged(null, null);
        }

        private void txtDescripcion_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            Config.fmLabelsSelector modalEditor = new Config.fmLabelsSelector();

            if (modalEditor.ShowDialog() == DialogResult.OK)
                txtDescripcion.EditValue = modalEditor.Id;
        }

        private void buDescartarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if (
                XtraMessageBox.Show(
                    "Descartar los cambios?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            Cargar();
        }

        private void grdColumnaView_InitNewRow(
            object sender,
            DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e
        )
        {
            DataTable dt = grdColumna.DataSource as DataTable;

            DataRow drMod = grdModelosView.GetFocusedDataRow();

            if (drMod.IsNull())
                return;

            bool bExtension =
                (drMod[@"SubTipo"].ToSafeString() == WoSubTypeModel.Extension.GetDescription())
                || (drMod[@"SubTipo"].ToSafeString() == WoSubTypeModel.Override.GetDescription());

            int Next = (bExtension ? 100 : 0);

            foreach (DataRow drRow in dt.Rows)
            {
                bool bFound = false;
                foreach (var col in WoConst.ColumnasCatalogo)
                    if (drRow[@"Orden"].ToString() == col)
                    {
                        bFound = true;
                        break;
                    }
                if (!bFound)
                {
                    if (drRow[@"Orden"].ToInt32() >= 200) // Evita campos automáticos
                        continue;
                    if (drRow[@"Orden"].ToInt32() >= Next)
                        Next = drRow[@"Orden"].ToInt32() + 2;
                }
            }

            DataRow dr = (DataRow)grdColumnaView.GetDataRow(e.RowHandle);

            dr[@"Orden"] = Next;

            dr[@"Columna"] = string.Empty;
            dr[@"ColumnaOriginal"] = string.Empty;
            dr[@"Descripcion"] = string.Empty;
            dr[@"Ayuda"] = string.Empty;
            dr[@"EtiquetaFormulario"] = string.Empty;
            dr[@"EtiquetaGrid"] = string.Empty;
            dr[@"TipoCol"] = WoTypeColumn.String.ToString();
            dr[@"Coleccion"] = false;
            dr[@"Longitud"] = 60;
            dr[@"Precision"] = 0;
            dr[@"TipoDato"] = WoTypeData.Volatil.ToString();
            dr[@"AceptaNulos"] = false;
            dr[@"EsVisibleEnLookUp"] = false;
            dr[@"Persistente"] = true;
            dr[@"Primaria"] = false;
            dr[@"Default"] = string.Empty;
            dr[@"Apps"] = string.Empty;
            dr[@"Control"] = WoTypeControl.Text.ToString();
            dr[@"Origen"] = wooWConfigParams.Origen;

            var TipoModelo = EnumExtensionMethods.GetValueFromDescription<WoTypeModel>(
                txtTipoModelo.EditValue.ToString()
            );

            if (
                (Next == 0)
                && (
                    (TipoModelo == WoTypeModel.Configuration)
                    || (TipoModelo == WoTypeModel.CatalogType)
                    || (TipoModelo == WoTypeModel.Catalog)
                    || (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                    || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                    || (TipoModelo == WoTypeModel.Control)
                    || (TipoModelo == WoTypeModel.Kardex)
                    || (TipoModelo == WoTypeModel.Parameter)
                )
            )
            {
                dr[@"Columna"] = "Id";
                dr[@"Primaria"] = true;
            }
        }

        private void txtFormInPlace_EditValueChanged(object sender, EventArgs e)
        {
            if (txtFormInPlace.EditValue.ToBoolean())
                grdColumnaView.OptionsBehavior.EditingMode = DevExpress
                    .XtraGrid
                    .Views
                    .Grid
                    .GridEditingMode
                    .Inplace;
            else
                grdColumnaView.OptionsBehavior.EditingMode = DevExpress
                    .XtraGrid
                    .Views
                    .Grid
                    .GridEditingMode
                    .EditFormInplace;
        }

        private void grdColumnaView_CellValueChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e
        )
        {
            // Insertando
            if (e.RowHandle < 0)
            {
                if (e.Column.FieldName == @"Columna")
                {
                    DataRow dr = (DataRow)grdColumnaView.GetDataRow(e.RowHandle);
                    if (dr[@"Descripcion"].IsNullOrStringTrimEmpty())
                        dr[@"Descripcion"] = e.Value;
                }

                if (e.Column.FieldName == @"Descripcion")
                {
                    DataRow dr = (DataRow)grdColumnaView.GetDataRow(e.RowHandle);
                    if (dr[@"Ayuda"].IsNullOrStringTrimEmpty())
                        dr[@"Ayuda"] = e.Value;
                    if (dr[@"EtiquetaFormulario"].IsNullOrStringTrimEmpty())
                        dr[@"EtiquetaFormulario"] = e.Value;
                    if (dr[@"EtiquetaGrid"].IsNullOrStringTrimEmpty())
                        dr[@"EtiquetaGrid"] = e.Value;
                }

                if (e.Column.FieldName == @"Coleccion")
                {
                    DataRow dr = (DataRow)grdColumnaView.GetDataRow(e.RowHandle);

                    string Columna = dr[@"Columna"].ToSafeString();

                    if (e.Value.ToBoolean())
                    {
                        if (Columna.EndsWith("Id"))
                            dr[@"Columna"] = Columna.Substring(0, Columna.Length - 2) + "Col";
                    }
                    else
                    {
                        if (Columna.EndsWith("Col"))
                            dr[@"Columna"] = Columna.Substring(0, Columna.Length - 3) + "Id";
                    }
                }
            }
        }

        private void grdColumnaView_ValidateRow(
            object sender,
            DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e
        )
        {
            ProyectoConPaquetes.Clear();

            DataRow dr = (DataRow)grdColumnaView.GetDataRow(e.RowHandle);

            if (dr.IsNull())
                return;

            if (dr[@"Columna"].IsNullOrStringTrimEmpty())
            {
                e.ErrorText = $"Registre un nombre para la columna";
                e.Valid = false;
                grdColumnaView.FocusedColumn = grdColumnaView.Columns[@"Columna"];
                return;
            }

            string ColumnaOriginal = dr[@"ColumnaOriginal"].ToSafeString();
            string Columna = dr[@"Columna"].ToSafeString();

            if ((!dr[@"Persistente"].ToBoolean()) && (!Columna.StartsWith("_")))
            {
                e.ErrorText = $"Columna {Columna} no persistentes deben comenzar con '_'";
                e.Valid = false;
                return;
            }

            if (Columna.StartsWith("_"))
            {
                if (dr[@"Persistente"].ToBoolean())
                {
                    e.ErrorText =
                        $"Columna {Columna} que comienzan con '_' no puede ser persistente";
                    e.Valid = false;
                    return;
                }

                if (Columna.StartsWith("__"))
                {
                    string ColumnaModelo = Columna.Substring(2);
                    DataTable dtTable = grdColumna.DataSource as DataTable;
                    bool bFound = false;
                    foreach (DataRow drRow in dtTable.Rows)
                    {
                        if (
                            (!drRow[@"Modelo"].IsNullOrStringEmpty())
                            && (drRow[@"Columna"].ToSafeString() == ColumnaModelo)
                        )
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        e.ErrorText =
                            $"Columna {Columna} es descripción de una referencia, debe tener el mismo nombre de la columna de modelo referenciado comenzando con '__'";
                        e.Valid = false;
                        return;
                    }

                    // Debe ser TipoCol string no tener modelo y tipo de control no aplica
                    if (
                        (dr[@"TipoCol"].ToSafeString() != WoTypeColumn.String.ToString())
                        || (!dr[@"Modelo"].ToSafeString().IsNullOrStringTrimEmpty())
                        || (dr[@"Coleccion"].ToBoolean())
                        || dr[@"Control"].ToString() != WoTypeControl.NA.ToString()
                        || !dr[@"AceptaNulos"].ToBoolean()
                    )
                    {
                        e.ErrorText =
                            $"Columna {Columna} que comienzan con '__' debe ser tipo string, no tener modelo, no ser colección, aceptar nulos y tipo de control NoAplica";
                        e.Valid = false;
                        return;
                    }
                }
            }

            int indice = WoConst.ColumnasSistema.IndexOf(ColumnaOriginal);
            var TipoModelo = EnumExtensionMethods.GetValueFromDescription<WoTypeModel>(
                txtTipoModelo.EditValue.ToString()
            );
            if (indice == -1)
            {
                if (
                    (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                )
                    indice = WoConst.ColumnasTransaccion.IndexOf(ColumnaOriginal);
            }
            if (indice == -1)
            {
                if ((TipoModelo == WoTypeModel.CatalogType) || (TipoModelo == WoTypeModel.Catalog))
                    indice = WoConst.ColumnasCatalogo.IndexOf(ColumnaOriginal);
            }

            if (WoConst.ColumnasSistema.IndexOf(ColumnaOriginal) != -1)
            {
                e.ErrorText = $"Columna no puede editarse";
                e.Valid = false;
                return;
            }

            if (ColumnaOriginal == "Id")
            {
                if (ColumnaOriginal != dr[@"Columna"].ToSafeString())
                {
                    e.ErrorText = $"Columna Id no puede cambiarse de nombre";
                    e.Valid = false;
                    return;
                }
            }

            if (
                (dr[@"Descripcion"].IsNullOrStringTrimEmpty())
                || (dr[@"Ayuda"].IsNullOrStringTrimEmpty())
                || (dr[@"EtiquetaFormulario"].IsNullOrStringTrimEmpty())
                || (dr[@"EtiquetaGrid"].IsNullOrStringTrimEmpty())
            )
            {
                e.ErrorText = $"Faltan etiquetas que registrar";
                e.Valid = false;
                grdColumnaView.FocusedColumn = grdColumnaView.Columns[@"Descripcion"];
                return;
            }

            if (dr[@"TipoCol"].IsNullOrStringTrimEmpty())
            {
                e.ErrorText = $"Registre un tipo para la columna";
                e.Valid = false;
                grdColumnaView.FocusedColumn = grdColumnaView.Columns[@"TipoCol"];
                return;
            }

            if (dr[@"TipoDato"].IsNullOrStringTrimEmpty())
            {
                e.ErrorText = $"Registre un tipo de dato para la columna";
                e.Valid = false;
                grdColumnaView.FocusedColumn = grdColumnaView.Columns[@"TipoDato"];
                return;
            }

            if (dr[@"Control"].IsNullOrStringTrimEmpty())
            {
                e.ErrorText = $"Registre un control para la columna";
                e.Valid = false;
                grdColumnaView.FocusedColumn = grdColumnaView.Columns[@"TipoDato"];
                return;
            }

            if (!dr[@"Modelo"].IsNullOrStringTrimEmpty())
            {
                var principal = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

                Modelo mod = principal
                    .ModeloCol.Modelos.Where(j => j.Id == dr[@"Modelo"].ToString())
                    .FirstOrDefault();

                if (mod == null)
                {
                    e.ErrorText = $"Modelo {dr[@"Modelo"].ToString()} no existe";
                    e.Valid = false;
                    return;
                }

                WoTypeColumn TipoColumna = (WoTypeColumn)
                    Enum.Parse(typeof(WoTypeColumn), dr[@"TipoCol"].ToString());

                if (
                    (TipoColumna != WoTypeColumn.Reference)
                    && (TipoColumna != WoTypeColumn.Complex)
                    && (TipoColumna != WoTypeColumn.EnumInt)
                    && (TipoColumna != WoTypeColumn.EnumString)
                )
                {
                    e.ErrorText =
                        $"Cuando registra un modelo el tipo de columna debe ser Reference o Collection o Complex o CollectionComplex o EnumInt o EnumString";
                    e.Valid = false;
                    return;
                }

                if (
                    (TipoColumna == WoTypeColumn.EnumInt)
                    || (TipoColumna == WoTypeColumn.EnumString)
                )
                {
                    string sColBuscar = dr[@"Columna"].ToSafeString();
                    if (dr[@"Coleccion"].ToBoolean())
                    {
                        if (!sColBuscar.EndsWith("Col"))
                        {
                            e.ErrorText =
                                $"Columna {dr[@"Columna"].ToSafeString()} debe terminar en Col";
                            e.Valid = false;
                        }

                        sColBuscar = sColBuscar.Substring(0, sColBuscar.Length - 3);
                    }

                    ModeloColumna col = mod
                        .Columnas.Where(j =>
                            (j.Id == sColBuscar) || (j.EsColeccion && (j.Id == sColBuscar + "Col"))
                        )
                        .FirstOrDefault();

                    if (col == null)
                    {
                        e.ErrorText =
                            $"El modelo {dr[@"Modelo"].ToSafeString()} no tiene la columna {sColBuscar}";
                        e.Valid = false;
                    }

                    if (
                        (col.TipoColumna != WoTypeColumn.EnumInt)
                        && (col.TipoColumna != WoTypeColumn.EnumString)
                    )
                    {
                        e.ErrorText =
                            $"En el modelo {dr[@"Modelo"].ToSafeString()} la columna {dr[@"Columna"].ToSafeString()} no es EnumInt o EnumString";
                        e.Valid = false;
                    }

                    //ModeloColumna col = mod.Columnas
                    //    .Where(j => j.Id == dr[@"Columna"].ToSafeString())
                    //    .FirstOrDefault();
                    //if (col == null)
                    //{
                    //    e.ErrorText =
                    //        $"El modelo {dr[@"Modelo"].ToSafeString()} no tiene la columna {dr[@"Columna"].ToSafeString()}";
                    //    e.Valid = false;
                    //    return;

                    if (
                        (col.TipoColumna != WoTypeColumn.EnumInt)
                        && (col.TipoColumna != WoTypeColumn.EnumString)
                    )
                    {
                        e.ErrorText =
                            $"En el modelo {dr[@"Modelo"].ToSafeString()} la columna {dr[@"Columna"].ToSafeString()} no es EnumInt o EnumString";
                        e.Valid = false;
                        return;
                    }

                    if ((col.TipoColumna != TipoColumna))
                    {
                        e.ErrorText =
                            $"En el modelo {dr[@"Modelo"].ToSafeString()} la columna {dr[@"Columna"].ToSafeString()} el tipo es diferente";
                        e.Valid = false;
                        return;
                    }
                }
                else
                {
                    string NombreCorrecto;

                    if (dr[@"Coleccion"].ToBoolean())
                        NombreCorrecto = dr[@"Modelo"].ToSafeString() + "Col";
                    else if ((TipoColumna == WoTypeColumn.Complex))
                        NombreCorrecto = dr[@"Modelo"].ToSafeString();
                    else
                        NombreCorrecto = dr[@"Modelo"].ToSafeString() + "Id";

                    if (
                        (dr[@"Columna"].ToSafeString() != NombreCorrecto)
                        && (
                            (!dr[@"Columna"].ToSafeString().StartsWith(NombreCorrecto + "_"))
                            || (
                                dr[@"Columna"].ToSafeString().Length
                                <= (NombreCorrecto + "_").Length
                            )
                        )
                    )
                    {
                        e.ErrorText =
                            $"La columna debe llamarse {NombreCorrecto} o comenzar con {NombreCorrecto}_ para múltiples referencias al mismo modelo ";
                        e.Valid = false;
                        return;
                    }

                    var TipoControl = (WoTypeControl)
                        Enum.Parse(typeof(WoTypeControl), dr[@"Control"].ToString());

                    if (dr[@"Coleccion"].ToBoolean())
                    {
                        if (
                            (TipoControl != WoTypeControl.Text)
                            && (TipoControl != WoTypeControl.CollectionEditor)
                            && (TipoControl != WoTypeControl.Custom)
                            && (TipoControl != WoTypeControl.NA)
                        )
                        {
                            e.ErrorText =
                                $"Si la columna es colección el Tipo de Control debe ser Text o CollectionEditor o Custom o NA";
                            e.Valid = false;
                        }
                    }
                    else if ((TipoColumna == WoTypeColumn.Reference))
                    {
                        if (
                            (TipoControl != WoTypeControl.Text)
                            && (TipoControl != WoTypeControl.LookUp)
                            && (TipoControl != WoTypeControl.LookUpDialog)
                            && (TipoControl != WoTypeControl.NA)
                        )
                        {
                            e.ErrorText =
                                $"Tipo de Columna Reference o Complex debe tener Tipo de Control Text o Lookup o LookupDialog o NA";
                            e.Valid = false;
                        }
                    }
                    else if ((TipoColumna == WoTypeColumn.Complex))
                    {
                        if (
                            (TipoControl != WoTypeControl.Custom)
                            && (TipoControl != WoTypeControl.NA)
                        )
                        {
                            e.ErrorText =
                                $"Tipo de Columna Complex debe tener Tipo de Control Custom o NA";
                            e.Valid = false;
                        }
                    }
                }
            }
        }

        private void txtTipoModelo_EditValueChanged(object sender, EventArgs e)
        {
            if (bControlCambioDeSubTipo)
                return;

            DataTable dt = grdColumna.DataSource as DataTable;

            if (dt.IsNull())
                return;

            if (txtTipoModelo.EditValue.IsNullOrStringEmpty())
                return;

            var TipoModelo = EnumExtensionMethods.GetValueFromDescription<WoTypeModel>(
                txtTipoModelo.EditValue.ToString()
            );

            if ((TipoModelo == WoTypeModel.Request) || (TipoModelo == WoTypeModel.View))
            {
                txtRoles.Enabled = true;
                txtPermisos.Enabled = true;
            }
            else
            {
                txtRoles.Enabled = false;
                txtPermisos.Enabled = false;
                foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                    i.CheckState = CheckState.Unchecked;

                foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                    i.CheckState = CheckState.Unchecked;
            }

            GridColumn col = grdColumnaView.Columns[@"Legacy"];
            col.OptionsColumn.AllowEdit = (TipoModelo != WoTypeModel.View);

            if (TipoModelo == WoTypeModel.Class)
                return;

            if (TipoModelo == WoTypeModel.Interface)
            {
                txtInterface1.EditValue = null;
                txtInterface2.EditValue = null;
                txtInterface3.EditValue = null;
                txtInterface1.Enabled = false;
                txtInterface2.Enabled = false;
                txtInterface3.Enabled = false;
            }

            if (
                (TipoModelo != WoTypeModel.Complex)
                && (TipoModelo != WoTypeModel.Request)
                && (TipoModelo != WoTypeModel.Response)
                && (TipoModelo != WoTypeModel.Interface)
                && (TipoModelo != WoTypeModel.View)
            )
            {
                WoTypeColumn tc = WoTypeColumn.String;
                int iLongitud = 20;
                if (
                    (TipoModelo == WoTypeModel.Configuration)
                    || (TipoModelo == WoTypeModel.CatalogType)
                )
                    iLongitud = 6;
                else if (TipoModelo == WoTypeModel.Catalog)
                    iLongitud = 12;
                else if (TipoModelo == WoTypeModel.CatalogSlave)
                    iLongitud = 40; // Catalogo + '-' + Row
                else if (
                    (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                )
                { // udn 6 '/' serie 6 '-' folio 7
                    iLongitud = 60;

                    var proyectoConPaquetes = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

                    var tablaPeriodoContable = proyectoConPaquetes
                        .ModeloCol.Modelos.Where(f => f.Id == WoConst.WOPERIODO)
                        .FirstOrDefault();

                    if (tablaPeriodoContable == null)
                    {
                        XtraMessageBox.Show(
                            $"Necesita dar de alta el modelo {WoConst.WOPERIODO}",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    var tablaWoUdn = proyectoConPaquetes
                        .ModeloCol.Modelos.Where(f => f.Id == WoConst.WOUDN)
                        .FirstOrDefault();

                    if (tablaWoUdn == null)
                    {
                        XtraMessageBox.Show(
                            $"Necesita dar de alta el modelo {WoConst.WOUDN}",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    if (TipoModelo == WoTypeModel.TransactionContable)
                    {
                        if (txtModelo.EditValue.ToSafeString() != WoConst.WOCONPOLIZA)
                        {
                            // Verifica si existe el modelo ConPoliza y WoPeriodo
                            var Poliza = proyectoConPaquetes
                                .ModeloCol.Modelos.Where(f => f.Id == WoConst.WOCONPOLIZA)
                                .FirstOrDefault();

                            if (Poliza == null)
                            {
                                XtraMessageBox.Show(
                                    $"Necesita dar de alta el modelo {WoConst.WOCONPOLIZA}",
                                    "Verifique...",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return;
                            }
                        }
                    }
                }
                else if (TipoModelo == WoTypeModel.TransactionSlave) // transaccion + ' ' + row 7
                    iLongitud = 40;
                else if (
                    (TipoModelo == WoTypeModel.Control)
                    || (TipoModelo == WoTypeModel.Kardex)
                    || (TipoModelo == WoTypeModel.DataMart)
                    || (TipoModelo == WoTypeModel.Parameter)
                )
                {
                    tc = WoTypeColumn.Autoincrement;
                    iLongitud = 0;
                }

                DataRow[] drRows = drRows = dt.Select($"Columna = 'Id'");
                if (drRows.Length == 0)
                    AgregarColumna(
                        1,
                        @"Id",
                        @"Id",
                        tc,
                        iLongitud,
                        0,
                        WoTypeData.Primary,
                        false,
                        true,
                        WoTypeControl.Text
                    );
            }
            else
            {
                //BorraColumna("Id");
            }

            if (
                (TipoModelo == WoTypeModel.Configuration)
                || (TipoModelo == WoTypeModel.CatalogType)
                || (TipoModelo == WoTypeModel.Catalog)
                || (TipoModelo == WoTypeModel.TransactionContable)
                || (TipoModelo == WoTypeModel.TransactionNoContable)
                || (TipoModelo == WoTypeModel.Control)
                || (TipoModelo == WoTypeModel.Kardex)
                || (TipoModelo == WoTypeModel.DataMart)
                || (TipoModelo == WoTypeModel.Parameter)
            )
            {
                AgregarColumna(
                    200,
                    @"WoState",
                    @"State",
                    WoTypeColumn.WoState,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.WoState
                );
            }

            if ((TipoModelo == WoTypeModel.CatalogType) || (TipoModelo == WoTypeModel.Catalog))
            {
                AgregarColumna(
                    210,
                    WoConst.SUSPENDINFO,
                    @"Causa de Suspensión",
                    WoTypeColumn.String,
                    256,
                    0,
                    WoTypeData.Control,
                    true,
                    false,
                    WoTypeControl.Text
                );
                AgregarColumna(
                    211,
                    WoConst.SUSPENDBY,
                    @"Suspendido Por",
                    WoTypeColumn.String,
                    64,
                    0,
                    WoTypeData.Control,
                    true,
                    false,
                    WoTypeControl.Text
                );
                AgregarColumna(
                    212,
                    WoConst.SUSPENDDATE,
                    @"Fecha de Suspensión",
                    WoTypeColumn.DateTime,
                    0,
                    0,
                    WoTypeData.Control,
                    true,
                    false,
                    WoTypeControl.Date
                );
                AgregarColumna(
                    225,
                    WoConst.DELETEINFO,
                    @"Causa de Borrado",
                    WoTypeColumn.String,
                    256,
                    0,
                    WoTypeData.Control,
                    true,
                    false,
                    WoTypeControl.Text
                );
            }
            else
            {
                if ((TipoModelo != WoTypeModel.Request) && (TipoModelo != WoTypeModel.Response))
                    foreach (string sCol in WoConst.ColumnasCatalogo)
                        BorraColumna(sCol);
            }

            if (
                (TipoModelo == WoTypeModel.TransactionContable)
                || (TipoModelo == WoTypeModel.TransactionNoContable)
            )
            {
                DataRow dr = AgregarColumna(
                    2,
                    WoConst.WOUDNID,
                    @"Udn",
                    WoTypeColumn.Reference,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Text
                );

                dr["Modelo"] = "WoUdn";

                AgregarColumna(
                    3,
                    WoConst.WOSERIE,
                    WoConst.WOSERIE,
                    WoTypeColumn.String,
                    6,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Text
                );
                AgregarColumna(
                    4,
                    WoConst.WOFOLIO,
                    WoConst.WOFOLIO,
                    WoTypeColumn.Long,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Spin
                );
                AgregarColumna(
                    6,
                    WoConst.WOTRANSACCIONTIPOID,
                    @"Tipo de Transacción",
                    WoTypeColumn.Reference,
                    "WoTransaccionTipo",
                    0,
                    0,
                    WoTypeData.Control,
                    true,
                    false,
                    WoTypeControl.LookUp,
                    string.Empty
                );
                AgregarColumna(
                    7,
                    WoConst.WOFECHA,
                    @"Fecha",
                    WoTypeColumn.Date,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Date
                );
                AgregarColumna(
                    8,
                    WoConst.WOORIGEN,
                    @"Origen",
                    WoTypeColumn.EnumInt,
                    string.Empty,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.EnumInt,
                    WooW.SB.Properties.Resources.JsonColumnaOrigen
                );
                AgregarColumna(
                    9,
                    WoConst.WOOBSERVACION,
                    @"Observación",
                    WoTypeColumn.Clob,
                    0,
                    0,
                    WoTypeData.Volatil,
                    true,
                    false,
                    WoTypeControl.Memo
                );
                ;

                AgregarColumna(
                    10,
                    WoConst.WOPERIODOID,
                    @"Periodo",
                    WoTypeColumn.Reference,
                    WoConst.WOPERIODO,
                    0,
                    0,
                    WoTypeData.Contable,
                    false,
                    false,
                    WoTypeControl.LookUp,
                    string.Empty
                );

                if (TipoModelo == WoTypeModel.TransactionContable)
                    AgregarColumna(
                        11,
                        WoConst.WOCONPOLIZAID,
                        @"Poliza Id",
                        WoTypeColumn.Reference,
                        "WoConPoliza",
                        0,
                        0,
                        WoTypeData.Contable,
                        true,
                        false,
                        WoTypeControl.LookUp,
                        string.Empty
                    );
            }
            else
            {
                //foreach (string sCol in WoConst.ColumnasTransaccion)
                //    BorraColumna(sCol);
                //BorraReferencia(WoConst.WOPERIODO);
                //BorraReferencia("ConPoliza");
            }

            if (
                (TipoModelo == WoTypeModel.CatalogSlave)
                || (TipoModelo == WoTypeModel.TransactionSlave)
            )
            {
                AgregarColumna(
                    (TipoModelo == WoTypeModel.CatalogSlave ? 3 : 2),
                    WoConst.WORENGLON,
                    @"Renglón",
                    WoTypeColumn.Long,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Spin
                );
            }
            else
            {
                if ((TipoModelo != WoTypeModel.Request) && (TipoModelo != WoTypeModel.Response))
                    BorraColumna(WoConst.WORENGLON);
            }

            if (
                (TipoModelo == WoTypeModel.Configuration)
                || (TipoModelo == WoTypeModel.CatalogType)
                || (TipoModelo == WoTypeModel.Catalog)
                || (TipoModelo == WoTypeModel.TransactionContable)
                || (TipoModelo == WoTypeModel.TransactionNoContable)
                || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                || (TipoModelo == WoTypeModel.Parameter)
            )
            {
                AgregarColumna(
                    201,
                    WoConst.ROWVERSION,
                    @"Versión en Db",
                    WoTypeColumn.Long,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Spin
                );

                DataRow locDr = AgregarColumna(
                    221,
                    WoConst.CREATEDDATE,
                    @"Fecha de Creación",
                    WoTypeColumn.DateTime,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Date
                );
                locDr["Default"] = "DateTime.Today";

                AgregarColumna(
                    222,
                    WoConst.CREATEDBY,
                    @"Creado Por",
                    WoTypeColumn.String,
                    64,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Text
                );

                locDr = AgregarColumna(
                    223,
                    WoConst.MODIFIEDDATE,
                    @"Fecha de Modificación",
                    WoTypeColumn.DateTime,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Date
                );
                locDr["Default"] = "DateTime.Today";

                AgregarColumna(
                    224,
                    WoConst.MODIFIEDBY,
                    @"Modificado Por",
                    WoTypeColumn.String,
                    64,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Text
                );
                AgregarColumna(
                    226,
                    WoConst.DELETEDDATE,
                    @"Fecha de Borrado",
                    WoTypeColumn.DateTime,
                    0,
                    0,
                    WoTypeData.Control,
                    true,
                    false,
                    WoTypeControl.Date
                );
                AgregarColumna(
                    227,
                    WoConst.DELETEDBY,
                    @"Borrado Por",
                    WoTypeColumn.String,
                    64,
                    0,
                    WoTypeData.Control,
                    true,
                    false,
                    WoTypeControl.Text
                );

                if (
                    (TipoModelo == WoTypeModel.TransactionContable)
                    || (TipoModelo == WoTypeModel.TransactionNoContable)
                )
                {
                    DataRow dr = AgregarColumna(
                        228,
                        WoConst.WOGUID,
                        @"Guid Id",
                        WoTypeColumn.String,
                        36,
                        0,
                        WoTypeData.Control,
                        true,
                        false,
                        WoTypeControl.NA
                    );

                    dr["Default"] = "Guid.NewGuid().ToString()";
                }
            }
            else if (
                (TipoModelo != WoTypeModel.Complex)
                && (TipoModelo != WoTypeModel.Interface)
                && (TipoModelo != WoTypeModel.View)
                && (TipoModelo != WoTypeModel.Request)
                && (TipoModelo != WoTypeModel.Response)
                && (TipoModelo != WoTypeModel.CatalogSlave)
                && (TipoModelo != WoTypeModel.TransactionSlave)
            )
            {
                AgregarColumna(
                    201,
                    WoConst.ROWVERSION,
                    @"Versión en Db",
                    WoTypeColumn.Long,
                    0,
                    0,
                    WoTypeData.Control,
                    false,
                    false,
                    WoTypeControl.Spin
                );
                foreach (string sCol in WoConst.ColumnasSistema)
                    if (sCol != WoConst.ROWVERSION)
                        BorraColumna(sCol);
            }
            else
            {
                if ((TipoModelo != WoTypeModel.Request) && (TipoModelo != WoTypeModel.Response))
                {
                    foreach (string sCol in WoConst.ColumnasSistema)
                        BorraColumna(sCol);
                }
            }

            pnlDatosBotones.Enabled = true;

            try
            {
                DataTable dtMod = (grdModelos.DataSource as DataTable);

                if (dtMod.IsNull())
                    return;
            }
            catch { }
        }

        private DataRow AgregarColumna(
            int Orden,
            string Columna,
            string Descripcion,
            WoTypeColumn typecolumn,
            int longitud,
            int presicion,
            WoTypeData typedata,
            bool aceptanulos,
            bool primaria,
            WoTypeControl typecontrol
        )
        {
            return AgregarColumna(
                Orden,
                Columna,
                Descripcion,
                typecolumn,
                string.Empty,
                longitud,
                presicion,
                typedata,
                aceptanulos,
                primaria,
                typecontrol,
                string.Empty
            );
        }

        private DataRow AgregarColumna(
            int Orden,
            string Columna,
            string Descripcion,
            WoTypeColumn typecolumn,
            string modelo,
            int longitud,
            int presicion,
            WoTypeData typedata,
            bool aceptanulos,
            bool primaria,
            WoTypeControl typecontrol,
            string Propiedades
        )
        {
            DataTable dt = grdColumna.DataSource as DataTable;

            if (dt.IsNull())
                return null;

            DataRow dr = null;

            DataRow[] drRows = drRows = dt.Select($"Columna = '{Columna}'");
            if (drRows.Length != 0)
            {
                dr = drRows[0];
                //dr[@"Orden"] = Orden;
                dr[@"Columna"] = Columna;
                dr[@"ColumnaOriginal"] = Columna;

                if (
                    (dr[@"Descripcion"].ToSafeString().IsDBNullOrStringEmpty()) || (Columna != "Id")
                )
                    dr[@"Descripcion"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);
                if ((dr[@"Ayuda"].ToSafeString().IsDBNullOrStringEmpty()) || (Columna != "Id"))
                    dr[@"Ayuda"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);
                if (
                    (dr[@"EtiquetaFormulario"].ToSafeString().IsDBNullOrStringEmpty())
                    || (Columna != "Id")
                )
                    dr[@"EtiquetaFormulario"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);
                if (
                    (dr[@"EtiquetaGrid"].ToSafeString().IsDBNullOrStringEmpty())
                    || (Columna != "Id")
                )
                    dr[@"EtiquetaGrid"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);

                dr[@"TipoCol"] = typecolumn;
                dr[@"Coleccion"] = false;
                dr[@"Modelo"] = modelo;
                dr[@"Longitud"] = longitud;
                dr[@"Precision"] = presicion;
                dr[@"TipoDato"] = typedata;
                dr[@"AceptaNulos"] = aceptanulos;
            }
            else
            {
                dr = dt.NewRow();
                dr[@"Orden"] = Orden;
                dr[@"Columna"] = Columna;
                dr[@"ColumnaOriginal"] = Columna;
                dr[@"Descripcion"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);
                dr[@"Ayuda"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);
                dr[@"EtiquetaFormulario"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);
                dr[@"EtiquetaGrid"] = new EtiquetaIdioma(proyecto.esMX, Descripcion);
                dr[@"TipoCol"] = typecolumn;
                dr[@"Coleccion"] = false;
                dr[@"Modelo"] = modelo;
                dr[@"Longitud"] = longitud;
                dr[@"Precision"] = presicion;
                dr[@"TipoDato"] = typedata;
                dr[@"AceptaNulos"] = aceptanulos;
                dr[@"EsVisibleEnLookUp"] = false;
                dr[@"Persistente"] = true;
                dr[@"Primaria"] = primaria;
                dr[@"Control"] = typecontrol;
                dr[@"Propiedades"] = Propiedades;
                dt.Rows.Add(dr);
            }

            return dr;
        }

        private void BorraColumna(string Columna)
        {
            DataTable dt = grdColumna.DataSource as DataTable;
            DataRow[] drRows = dt.Select($"Columna = '{Columna}'");
            if (drRows.Length > 0)
                drRows[0].Delete();
        }

        private void buModeloStandard_Click(object sender, EventArgs e)
        {
            string ArchivoProyecto;

            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Exportación veXMAX Model (*.json)|*.json";
                openDialog.FileName = string.Empty;

#if DEBUG
                string sDirFijo = @"C:\Temp\ExportacionParaWooW";
                if (Directory.Exists(sDirFijo))
                    openDialog.InitialDirectory = sDirFijo;
#endif
                openDialog.RestoreDirectory = true;

                if (openDialog.ShowDialog() != DialogResult.OK)
                    return;

                ArchivoProyecto = openDialog.FileName;
            }

            ImportVexMAXModel(ArchivoProyecto);
        }

        private void ImportVexMAXModel(string sArchivo)
        {
            veXMAXModelExport vxModel;
            try
            {
                vxModel = veXMAXModelExport.FromJson(File.ReadAllText(sArchivo));
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataTable dt = grdColumna.DataSource as DataTable;
            DataTable dtMod = grdModelos.DataSource as DataTable;

            int Next = 0;
            foreach (DataRow drRow in dt.Rows)
            {
                if (drRow[@"Orden"].ToInt32() >= 200) // Evita campos automaticos
                    continue;
                if (drRow[@"Orden"].ToInt32() >= Next)
                    Next = drRow[@"Orden"].ToInt32() + 1;
            }

            StringBuilder sb = new StringBuilder();

            txtLegacy.EditValue = vxModel.Legacy;

            foreach (var col in vxModel.Columnas)
            {
                int Orden = Next;

                DataRow[] drRows = drRows = dt.Select($"Legacy = '{col.Legacy}'");

                if (drRows.Length > 0)
                {
                    drRows[0]["AceptaNulos"] = col.Nulo;
                    continue;
                }

                drRows = drRows = dt.Select($"Columna = '{col.Id}'");
                int IncrementaRenglon = 0;
                if (drRows.Length == 0)
                    IncrementaRenglon = 1;
                else
                    Orden = drRows[0]["Orden"].ToInt16();

                if (col.Referencia.IsNullOrStringTrimEmpty())
                {
                    DataRow dr = AgregarColumna(
                        Orden,
                        col.Id,
                        col.Formulario,
                        (WoTypeColumn)Enum.Parse(typeof(WoTypeColumn), col.TipoColumna.ToString()),
                        col.Longitud,
                        col.Precision,
                        WoTypeData.Volatil,
                        col.Nulo,
                        col.Primaria,
                        (WoTypeControl)Enum.Parse(typeof(WoTypeControl), col.TipoControl.ToString())
                    );

                    dr[@"Coleccion"] = false;
                    dr[@"Descripcion"] = col.Descripcion;
                    dr[@"Ayuda"] = col.Ayuda;
                    dr[@"EtiquetaFormulario"] = col.Formulario;
                    dr[@"EtiquetaGrid"] = col.Grid;
                    dr[@"Default"] = col.Default;
                    dr[@"Legacy"] = col.Legacy;
                    dr[@"Apps"] = string.Empty;
                }
                else
                {
                    var Modelo = this
                        .proyecto.ModeloCol.Modelos.Where(f => f.Legacy == col.Referencia)
                        .FirstOrDefault();

                    if (Modelo == null)
                    {
                        sb.AppendLine($"Referencia no existe {col.Referencia}");
                        continue;
                    }

                    DataRow dr = AgregarColumna(
                        Orden,
                        Modelo.Id + "Id",
                        col.Formulario,
                        (WoTypeColumn)Enum.Parse(typeof(WoTypeColumn), col.TipoColumna.ToString()),
                        Modelo.Id,
                        col.Longitud,
                        col.Precision,
                        WoTypeData.Volatil,
                        col.Nulo,
                        col.Primaria,
                        (WoTypeControl)
                            Enum.Parse(typeof(WoTypeControl), col.TipoControl.ToString()),
                        string.Empty
                    );

                    dr[@"TipoCol"] = WoTypeColumn.Reference.ToString();
                    dr[@"Coleccion"] = false;
                    dr[@"Descripcion"] = col.Descripcion;
                    dr[@"Ayuda"] = col.Ayuda;
                    dr[@"EtiquetaFormulario"] = col.Formulario;
                    dr[@"EtiquetaGrid"] = col.Grid;

                    dr[@"Longitud"] = 0;
                    dr[@"Precision"] = 0;

                    dr[@"Default"] = col.Default;
                    dr[@"Legacy"] = col.Legacy;
                    dr[@"Apps"] = string.Empty;
                }

                Next += IncrementaRenglon;
            }

            if (!sb.ToString().IsNullOrStringEmpty())
                XtraMessageBox.Show(
                    sb.ToString(),
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
        }

        private void buCopiarColumnas_Click(object sender, EventArgs e)
        {
            if (ColSelector == null)
                ColSelector = new fmColumnsSelector();

            DataTable dt = grdColumna.DataSource as DataTable;

            if (txtTipoModelo.EditValue.ToString() == WoTypeModel.View.GetDescription())
                ColSelector.HabilitaCombinar();

            if (ColSelector.ShowDialog() == DialogResult.OK)
            {
                bool CombinarModeloColumna = ColSelector.CombinarModeloColumna;

                var modelo = this
                    .proyecto.ModeloCol.Modelos.Where(f => f.Id == ColSelector.Modelo)
                    .FirstOrDefault();
                if (modelo == null)
                    return;

                foreach (var colselected in ColSelector.ColSelected)
                {
                    var colFound = modelo.Columnas.Where(f => f.Id == colselected).FirstOrDefault();

                    if (colFound == null)
                        continue;

                    var col = colFound.CreateCopy();

                    string BaseName = col.Id;

                    col.Id = (CombinarModeloColumna ? $"{modelo.Id}{col.Id}" : col.Id);
                    col.Primaria = false;

                    if (txtTipoModelo.EditValue.ToString() == WoTypeModel.View.GetDescription())
                    {
                        col.Legacy = $"{modelo.Id}.{BaseName}";

                        var EtiquetaModelo = string.Empty;

                        if (col.TipoColumna == WoTypeColumn.Reference)
                        {
                            int Longitud = 0;
                            var locModelo = proyecto
                                .ModeloCol.Modelos.Where(x => x.Id == col.ModeloId)
                                .FirstOrDefault();

                            if (locModelo != null)
                            {
                                var locCol = locModelo
                                    .Columnas.Where(x => x.Id == "Id")
                                    .FirstOrDefault();

                                if (locCol != null)
                                    Longitud = locCol.Longitud;

                                col.ModeloId = string.Empty;
                                col.TipoColumna = WoTypeColumn.String;
                                col.Longitud = Longitud;
                            }
                        }

                        if (
                            (col.TipoControl == WoTypeControl.LookUp)
                            || (col.TipoControl == WoTypeControl.LookUpDialog)
                        )
                            col.TipoControl = WoTypeControl.Text;
                    }

                    int Orden = 0;
                    foreach (DataRow m in dt.Rows)
                        if ((m["Orden"].ToInt32() >= Orden) && (m["Orden"].ToInt32() < 200))
                            Orden = m["Orden"].ToInt32() + 2;

                    DataRow dr = dt.NewRow();
                    AgregarRenglon(dr, col);
                    dr["Orden"] = Orden;
                    dt.Rows.Add(dr);
                }
            }
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

        private void buRenombrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = grdModelosView.GetFocusedDataRow();

            if (dr == null)
                return;

            fmDialogRename fm = new fmDialogRename();

            fm.Modelo = dr[@"Modelo"].ToString();

            if (fm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    proyecto.RenameModel(dr[@"Modelo"].ToString(), fm.Modelo);
                    dr[@"Modelo"] = fm.Modelo;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
                Cargar();
            }
        }

        private void buHabilitarTipo_Click(object sender, EventArgs e)
        {
            if (
                (
                    txtSubTipoModelo.EditValue.ToSafeString()
                    == WoSubTypeModel.Extension.GetDescription()
                )
                || (
                    txtSubTipoModelo.EditValue.ToSafeString()
                    == WoSubTypeModel.Override.GetDescription()
                )
            )
                return;

            txtTipoModelo.Enabled = true;
        }

        private void optMostrarEtiquetas_EditValueChanged(object sender, EventArgs e)
        {
            if (grdColumna.DataSource != null)
            {
                GridColumn col = grdColumnaView.Columns[@"Orden"];
                col.Visible = optMostrarEtiquetas.EditValue.ToBoolean();
                if (col.Visible)
                    col.VisibleIndex = 1;
                col = grdColumnaView.Columns[@"Descripcion"];
                col.Visible = optMostrarEtiquetas.EditValue.ToBoolean();
                if (col.Visible)
                    col.VisibleIndex = 2;
                col = grdColumnaView.Columns[@"Ayuda"];
                col.Visible = optMostrarEtiquetas.EditValue.ToBoolean();
                if (col.Visible)
                    col.VisibleIndex = 3;
                col = grdColumnaView.Columns[@"EtiquetaFormulario"];
                col.Visible = optMostrarEtiquetas.EditValue.ToBoolean();
                if (col.Visible)
                    col.VisibleIndex = 4;
                col = grdColumnaView.Columns[@"EtiquetaGrid"];
                col.Visible = optMostrarEtiquetas.EditValue.ToBoolean();
                if (col.Visible)
                    col.VisibleIndex = 5;

                col = grdColumnaView.Columns[@"Legacy"];
                col.Visible = optMostrarEtiquetas.EditValue.ToBoolean();
                if (col.Visible)
                    col.VisibleIndex = grdColumnaView.Columns.Count;
            }

            grdColumna.ForceInitialize();
            grdColumnaView.BestFitColumns();
        }

        private void txtInterface_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            var txtInterface = (sender as ButtonEdit);

            if (e.Button.Kind == ButtonPredefines.Clear)
                txtInterface.EditValue = null;
            else if (e.Button.Kind == ButtonPredefines.Right)
            {
                if (txtInterface.EditValue == null)
                    return;

                if (
                    XtraMessageBox.Show(
                        "Cargar las columnas de la Interface?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes
                )
                    return;
                CargaColumnaInterface(txtInterface.EditValue.ToString());
            }
        }

        private void CargaColumnaInterface(string Interface)
        {
            var modelo = proyecto.ModeloCol.Modelos.Where(e => e.Id == Interface).FirstOrDefault();

            if (modelo == null)
                modelo = ProyectoConPaquetes
                    .Get(proyecto.ArchivoDeProyecto)
                    .ModeloCol.Modelos.Where(e => e.Id == Interface)
                    .FirstOrDefault();

            if (modelo == null)
            {
                XtraMessageBox.Show(
                    $"No existe la interface {Interface}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            DataTable dt = grdColumna.DataSource as DataTable;

            int Next = 0;
            foreach (DataRow drRow in dt.Rows)
            {
                if (drRow[@"Orden"].ToInt32() >= 200) // Evita campos automaticos
                    continue;
                if (drRow[@"Orden"].ToInt32() >= Next)
                    Next = drRow[@"Orden"].ToInt32() + 1;
            }

            foreach (var col in modelo.Columnas.OrderBy(e => e.Orden))
            {
                DataRow[] drRows = drRows = dt.Select($"Columna = '{col.Id}'");

                if (drRows.Length == 0)
                {
                    DataRow dr = AgregarColumna(
                        Next++,
                        col.Id,
                        col.Formulario,
                        (WoTypeColumn)Enum.Parse(typeof(WoTypeColumn), col.TipoColumna.ToString()),
                        col.Longitud,
                        col.Precision,
                        WoTypeData.Volatil,
                        col.Nulo,
                        col.Primaria,
                        (WoTypeControl)Enum.Parse(typeof(WoTypeControl), col.TipoControl.ToString())
                    );

                    dr[@"Coleccion"] = col.EsColeccion;
                    dr[@"Modelo"] = col.ModeloId;
                    dr[@"Descripcion"] = col.Descripcion;
                    dr[@"Ayuda"] = col.Ayuda;
                    dr[@"EtiquetaFormulario"] = col.Formulario;
                    dr[@"EtiquetaGrid"] = col.Grid;
                    dr[@"Default"] = col.Default;
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
            }
        }

        private bool ValidarInterface(string Interface)
        {
            try
            {
                ProyectoConPaquetes.Clear();
                Proyecto principal = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

                var modelo = principal
                    .ModeloCol.Modelos.Where(e => e.Id == Interface)
                    .FirstOrDefault();

                if (modelo == null)
                    throw new Exception($"No existe la interface {Interface}");

                DataTable dt = grdColumna.DataSource as DataTable;

                int Next = 0;
                foreach (DataRow drRow in dt.Rows)
                {
                    if (drRow[@"Orden"].ToInt32() >= 200) // Evita campos automáticos
                        continue;
                    if (drRow[@"Orden"].ToInt32() >= Next)
                        Next = drRow[@"Orden"].ToInt32() + 1;
                }

                foreach (var col in modelo.Columnas.OrderBy(e => e.Orden))
                {
                    DataRow[] drRows = drRows = dt.Select($"Columna = '{col.Id}'");

                    if (drRows.Length == 0)
                        throw new Exception(
                            $"La columna {col.Id} de la interface {Interface} no existe"
                        );

                    if (drRows[0][@"TipoCol"].ToString() != col.TipoColumna.ToString())
                        throw new Exception(
                            $"La columna {col.Id} tiene un tipo diferente a la interface {Interface}"
                        );

                    if (drRows[0][@"Modelo"].ToString() != col.ModeloId)
                        throw new Exception(
                            $"La columna {col.Id} tiene un modelo diferente a la interface {Interface}"
                        );

                    if (drRows[0][@"Longitud"].ToInt32() != col.Longitud)
                        throw new Exception(
                            $"La columna {col.Id} tiene una Longitud diferente a la interface {Interface}"
                        );

                    if (drRows[0][@"Precision"].ToInt32() != col.Precision)
                        throw new Exception(
                            $"La columna {col.Id} tiene una Precisión diferente a la interface {Interface}"
                        );
                    if (drRows[0][@"TipoDato"].ToString() != col.TipoDato.ToString())
                        throw new Exception(
                            $"La columna {col.Id} tiene un TipoDato diferente a la interface {Interface}"
                        );
                    if (drRows[0][@"Coleccion"].ToBoolean() != col.EsColeccion)
                        throw new Exception(
                            $"La columna {col.Id} tiene la propiedad Es Colección? diferente a la interface {Interface}"
                        );
                    if (drRows[0][@"AceptaNulos"].ToBoolean() != col.Nulo)
                        throw new Exception(
                            $"La columna {col.Id} tiene la propiedad Acepta nulos ? diferente a la interface {Interface}"
                        );
                }

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
        }

        private void buRenumerar_Click(object sender, EventArgs e)
        {
            if (grdColumnaView.IsEditing)
            {
                XtraMessageBox.Show(
                    "No puede realizar la acción si se encuentra editando",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DataTable dt = grdColumna.DataSource as DataTable;

            DataRow[] drRows = drRows = dt.Select($"Orden < 200", "Orden");

            int i = 0;
            foreach (DataRow drRow in drRows)
            {
                i += 2;
                drRow["Orden"] = i;
            }

            grdColumna.Refresh();
        }

        private void txtFiltroApps_HiddenEditor(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            ReCargar();
        }

        private void buRefrescar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReCargar();
        }

        private void grdColumnaView_FocusedColumnChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e
        )
        {
            if (
                (e.PrevFocusedColumn != null)
                && (e.PrevFocusedColumn.FieldName == @"Descripcion")
                && (
                    (grdColumnaView.IsEditing)
                    || (grdColumnaView.IsNewItemRow(grdColumnaView.FocusedRowHandle))
                )
            )
            {
                DataRow dr = grdColumnaView.GetFocusedDataRow();

                if (dr == null)
                    return;
                if (
                    grdColumnaView
                        .GetRowCellValue(
                            grdColumnaView.FocusedRowHandle,
                            grdColumnaView.Columns[@"Ayuda"]
                        )
                        .IsDBNullOrStringEmpty()
                )
                    grdColumnaView.SetRowCellValue(
                        grdColumnaView.FocusedRowHandle,
                        grdColumnaView.Columns[@"Ayuda"],
                        dr[@"Descripcion"].ToSafeString()
                    );
                if (
                    grdColumnaView
                        .GetRowCellValue(
                            grdColumnaView.FocusedRowHandle,
                            grdColumnaView.Columns[@"EtiquetaFormulario"]
                        )
                        .IsDBNullOrStringEmpty()
                )
                    grdColumnaView.SetRowCellValue(
                        grdColumnaView.FocusedRowHandle,
                        grdColumnaView.Columns[@"EtiquetaFormulario"],
                        dr[@"Descripcion"].ToSafeString()
                    );
                if (
                    grdColumnaView
                        .GetRowCellValue(
                            grdColumnaView.FocusedRowHandle,
                            grdColumnaView.Columns[@"EtiquetaGrid"]
                        )
                        .IsDBNullOrStringEmpty()
                )
                    grdColumnaView.SetRowCellValue(
                        grdColumnaView.FocusedRowHandle,
                        grdColumnaView.Columns[@"EtiquetaGrid"],
                        dr[@"Descripcion"].ToSafeString()
                    );
                grdColumnaView.PostEditor();
            }
        }

        private void buCrearCamposLookUp_Click(object sender, EventArgs e)
        {
            buRenumerar.PerformClick();

            DataTable dt = grdColumna.DataSource as DataTable;

            int Total = 0;

            List<DataRow> drList = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                if ((dr[@"Modelo"].IsDBNullOrStringEmpty()) || (dr[@"Coleccion"].ToBoolean()))
                    continue;

                // Existe la columna ya creada de tipo lookup descripcion
                string Columna = $"__{dr[@"Columna"].ToSafeString()}";

                DataRow[] drRows = dt.Select($"Columna = '{Columna}'");
                if (drRows.Length > 0)
                    continue;

                drList.Add(dr);
            }

            foreach (DataRow dr in drList)
            {
                string Columna = $"__{dr[@"Columna"].ToSafeString()}";

                // Crear la columna de tipo lookup descripcion
                DataRow drAdd = AgregarColumna(
                    dr[@"Orden"].ToInt16() + 1,
                    $"__{dr[@"Columna"].ToSafeString()}",
                    $"{dr[@"Descripcion"].ToSafeString()} Desc.",
                    WoTypeColumn.String,
                    120,
                    0,
                    WoTypeData.Volatil,
                    true,
                    false,
                    WoTypeControl.NA
                );
                drAdd[@"Persistente"] = false;

                Total++;
            }

            var TipoModelo = EnumExtensionMethods.GetValueFromDescription<WoTypeModel>(
                txtTipoModelo.EditValue.ToString()
            );

            if (Total > 0)
                XtraMessageBox.Show(
                    $"Se crearon {Total} columnas de tipo lookup descripción "
                        + (
                            (TipoModelo == WoTypeModel.CatalogSlave)
                            || (TipoModelo == WoTypeModel.TransactionSlave)
                                ? "\r\n\r\nElimine el campo descripción lookup para la referencia a la maestra"
                                : string.Empty
                        ),
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e) { }

        private void barButtonItem1_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            DataTable dtMod = grdModelos.DataSource as DataTable;

            List<string> cols = new List<string>()
            {
                @"Descripcion",
                @"Ayuda",
                @"EtiquetaFormulario",
                @"EtiquetaGrid"
            };

            grdModelosView.MoveFirst();

            string errores = string.Empty;

            foreach (DataRow drMod in dtMod.Rows)
            {
                buEditar.PerformClick();
                //buRegistrar.PerformClick();

                DataTable dt = grdColumna.DataSource as DataTable;

                bool bModificado = false;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[@"TipoCol"].IsDBNull())
                        continue;

                    var TipoColumna = (WoTypeColumn)
                        Enum.Parse(typeof(WoTypeColumn), dr[@"TipoCol"].ToString());
                    var TipoControl = (WoTypeControl)
                        Enum.Parse(typeof(WoTypeControl), dr[@"Control"].ToString());

                    if (TipoColumna == WoTypeColumn.Reference)
                    {
                        if (dr["Coleccion"].ToBoolean())
                        {
                            if (TipoControl != WoTypeControl.CollectionEditor)
                            {
                                dr[@"Control"] = WoTypeControl.CollectionEditor.ToString();
                                //errores += "\r\n" + drMod["Modelo"].ToString();
                                //break;
                            }
                        }
                        else
                        {
                            if (
                                (TipoControl != WoTypeControl.LookUp)
                                && (TipoControl != WoTypeControl.LookUpDialog)
                            )
                            {
                                if (
                                    (dr[@"Modelo"].ToString() == WoConst.WOPERIODO)
                                    || (dr[@"Modelo"].ToString() == "WoUdn")
                                )
                                {
                                    dr[@"Control"] = WoTypeControl.LookUp.ToString();
                                    bModificado = true;
                                }
                                else if (
                                    (dr[@"Modelo"].ToString().StartsWith("Sat"))
                                    || (dr[@"Modelo"].ToString().StartsWith("Con"))
                                    || (dr[@"Modelo"].ToString() == "Producto")
                                    || (dr[@"Modelo"].ToString() == "Empleado")
                                    || (dr[@"Modelo"].ToString() == "Proveedor")
                                    || (dr[@"Modelo"].ToString() == "Cliente")
                                    || (dr[@"Modelo"].ToString() == "CxpSaldo")
                                    || (dr[@"Modelo"].ToString() == "CxcSaldo")
                                    || (dr[@"Modelo"].ToString() == "EmpleadoSaldo")
                                )
                                {
                                    dr[@"Control"] = WoTypeControl.LookUpDialog.ToString();
                                    bModificado = true;
                                }
                                else
                                {
                                    errores += "\r\n" + drMod["Modelo"].ToString();
                                }

                                //dr[@"Control"].ToString());


                                //break;
                            }
                        }
                    }

                    //if (
                    //    (TipoColumna == WoTypeColumn.Integer)
                    //    || (TipoColumna == WoTypeColumn.Smallint)
                    //)
                    //{
                    //    if (TipoControl != WoTypeControl.Spin)
                    //    {
                    //        dr[@"Control"] = WoTypeControl.Spin.ToString();
                    //        bModificado = true;
                    //    }
                    //}

                    //if (TipoColumna == WoTypeColumn.Boolean)
                    //{
                    //    if (TipoControl != WoTypeControl.Bool)
                    //    {
                    //        dr[@"Control"] = WoTypeControl.Bool.ToString();
                    //        bModificado = true;
                    //    }
                    //}

                    //if (TipoColumna == WoTypeColumn.Date)
                    //{
                    //    if (TipoControl != WoTypeControl.Date)
                    //    {
                    //        dr[@"Control"] = WoTypeControl.Date.ToString();
                    //        bModificado = true;
                    //    }
                    //}

                    //foreach (var col in cols)
                    //{
                    //    dr[col] = "Periodo Desc."; // dr[col].ToString().Replace("Descripción", "Desc.");
                    //    bModificado = true;
                    //}
                }

                if (bModificado)
                    buRegistrar.PerformClick();
                else
                {
                    CargaModelos();
                    grdModelosView_FocusedRowChanged(null, null);
                }

                grdModelosView.MoveNext();
            }
        }

        private void buExtension_Click(object sender, EventArgs e)
        {
            var fmSelect = new fmSelectExtension();

            if (fmSelect.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    bControlCambioDeSubTipo = true;

                    txtModelo.EditValue = fmSelect.ModeloExtension;
                    txtTipoModelo.EditValue = fmSelect.TipoModelo.GetDescription();

                    // Todo Corregir override
                    if (fmSelect.ExtOvr == "Override")
                        txtSubTipoModelo.EditValue = WoSubTypeModel.Override.GetDescription();
                    else
                        txtSubTipoModelo.EditValue = WoSubTypeModel.Extension.GetDescription();

                    txtFiltro.EditValue = woGetListFilterType.Ninguno.GetDescription();

                    pnlDatosBotones.Enabled = true;

                    if (
                        (fmSelect.TipoModelo == WoTypeModel.Configuration)
                        || (fmSelect.TipoModelo == WoTypeModel.CatalogType)
                        || (fmSelect.TipoModelo == WoTypeModel.Catalog)
                        || (fmSelect.TipoModelo == WoTypeModel.TransactionContable)
                        || (fmSelect.TipoModelo == WoTypeModel.TransactionNoContable)
                        || (fmSelect.TipoModelo == WoTypeModel.TransactionFreeStyle)
                        || (fmSelect.TipoModelo == WoTypeModel.Control)
                        || (fmSelect.TipoModelo == WoTypeModel.Kardex)
                        || (fmSelect.TipoModelo == WoTypeModel.Parameter)
                    )
                    {
                        pnlDatosModelo.Enabled = false;
                    }
                    else if (fmSelect.TipoModelo == WoTypeModel.Class)
                    {
                        var principal = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

                        var modelo = principal
                            .ModeloCol.Modelos.Where(e => e.Id == fmSelect.ModeloExtension)
                            .FirstOrDefault();

                        if (modelo != null)
                        {
                            txtDescripcion.EditValue = modelo.EtiquetaId;
                            txtProceso.EditValue = modelo.ProcesoId;
                            txtInterface1.EditValue = (
                                modelo.Interface1.IsNullOrStringEmpty()
                                    ? null
                                    : modelo.Interface1.ToString()
                            );

                            txtInterface2.EditValue = (
                                modelo.Interface2.IsNullOrStringEmpty()
                                    ? null
                                    : modelo.Interface2.ToString()
                            );

                            txtInterface3.EditValue = (
                                modelo.Interface3.IsNullOrStringEmpty()
                                    ? null
                                    : modelo.Interface3.ToString()
                            );

                            foreach (CheckedListBoxItem item in txtApps.Properties.Items)
                            {
                                if (
                                    modelo
                                        .Apps.Where(x => x.AppId == item.Value.ToString())
                                        .FirstOrDefault() != null
                                )
                                    item.CheckState = CheckState.Checked;
                                else
                                    item.CheckState = CheckState.Unchecked;
                            }
                        }

                        txtModelo.Enabled = false;
                        txtTipoModelo.Enabled = false;
                        txtSubTipoModelo.Enabled = false;
                        txtDescripcion.Enabled = true;
                        txtOrdenDeCreacion.Enabled = false;
                        txtProceso.Enabled = true;
                        txtLegacy.Enabled = false;
                        txtFiltro.Enabled = false;
                        txtRoles.Enabled = false;
                        txtPermisos.Enabled = false;
                        txtApps.Enabled = true;
                        txtInterface1.Enabled = false;
                        txtInterface2.Enabled = false;
                        txtInterface3.Enabled = false;
                        pnlDatosModelo.Enabled = true;
                    }
                    else if (fmSelect.TipoModelo == WoTypeModel.View)
                    {
                        var principal = ProyectoConPaquetes.Get(proyecto.ArchivoDeProyecto);

                        var modelo = principal
                            .ModeloCol.Modelos.Where(e => e.Id == fmSelect.ModeloExtension)
                            .FirstOrDefault();

                        if (modelo != null)
                        {
                            txtDescripcion.EditValue = modelo.EtiquetaId;
                            txtProceso.EditValue = modelo.ProcesoId;
                            txtOrdenDeCreacion.EditValue = modelo.OrdenDeCreacion;

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

                            foreach (CheckedListBoxItem item in txtApps.Properties.Items)
                            {
                                if (
                                    modelo
                                        .Apps.Where(x => x.AppId == item.Value.ToString())
                                        .FirstOrDefault() != null
                                )
                                    item.CheckState = CheckState.Checked;
                                else
                                    item.CheckState = CheckState.Unchecked;
                            }
                        }

                        txtModelo.Enabled = false;
                        txtTipoModelo.Enabled = false;
                        txtSubTipoModelo.Enabled = false;
                        txtDescripcion.Enabled = true;
                        txtOrdenDeCreacion.Enabled = true;
                        txtProceso.Enabled = true;
                        txtLegacy.Enabled = false;
                        txtFiltro.Enabled = false;
                        txtRoles.Enabled = true;
                        txtPermisos.Enabled = true;
                        txtApps.Enabled = true;
                        txtInterface1.Enabled = false;
                        txtInterface2.Enabled = false;
                        txtInterface3.Enabled = false;
                    }
                }
                finally
                {
                    bControlCambioDeSubTipo = false;
                }
            }
        }

        private void txtTipoModelo_SelectedIndexChanged(object sender, EventArgs e) { }

        private void txtSubTipoModelo_EditValueChanged(object sender, EventArgs e)
        {
            if (bControlCambioDeSubTipo)
                return;

            if (
                (txtSubTipoModelo.EditValue == null)
                || (
                    (
                        txtSubTipoModelo.EditValue.ToSafeString()
                        == WoSubTypeModel.Extension.GetDescription()
                    )
                    || (
                        txtSubTipoModelo.EditValue.ToSafeString()
                        == WoSubTypeModel.Override.GetDescription()
                    )
                )
            )
                return;
        }

        private void txtSubTipoModelo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bControlCambioDeSubTipo)
                return;
            if (
                (
                    txtSubTipoModelo.EditValue.ToSafeString()
                    == WoSubTypeModel.Extension.GetDescription()
                )
                || (
                    txtSubTipoModelo.EditValue.ToSafeString()
                    == WoSubTypeModel.Override.GetDescription()
                )
            )
                txtSubTipoModelo.EditValue = null;
        }
    }
}
