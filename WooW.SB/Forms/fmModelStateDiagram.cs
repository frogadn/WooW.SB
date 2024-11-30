using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraReports.UI;
using WooW.Core;
using WooW.Core.Common;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmModelStateDiagram : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        public Proyecto proyecto { get; set; }

        public fmModelStateDiagram()
        {
            InitializeComponent();
        }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return buRegistrar.Enabled; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Refrescar()
        {
            if (buRefrescar.Enabled)
                buRefrescar.PerformClick();
        }

        public void Cargar()
        {
            woDiagram1.proyecto = proyecto;

            repFiltroApps.Items.Clear();
            repFiltroApps.Items.Add(@" Todos ");
            repFiltroApps.Items[0].CheckState = CheckState.Checked;
            foreach (var app in proyecto.Apps)
            {
                repFiltroApps.Items.Add(app.Id);
            }
            txtFiltroApps.EditValue = " Todos ";

            CargarModelo();

            grdModelosView_FocusedRowChanged(null, null);

            woDiagram1.Enabled = true;
            woDiagram1.SetReadOnly(true);
        }

        public void CargarModelo()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Proceso", typeof(string));
            dt.Columns.Add(@"Tipo", typeof(string));
            dt.Columns.Add(@"SubTipo", typeof(string));
            dt.Columns.Add(@"Hecho", typeof(bool));
            dt.Columns.Add(@"Modelo", typeof(string));
            dt.Columns.Add(@"Roles", typeof(string));
            dt.Columns.Add(@"OtrosRoles", typeof(string));
            dt.Columns.Add(@"Json", typeof(string));

            foreach (var Modelo in proyecto.ModeloCol.Modelos.OrderBy(e => e.Id).ToList())
            {
                if (
                    (Modelo.TipoModelo == WoTypeModel.Configuration)
                    || (Modelo.TipoModelo == WoTypeModel.CatalogType)
                    || (Modelo.TipoModelo == WoTypeModel.Catalog)
                    || (Modelo.TipoModelo == WoTypeModel.TransactionContable)
                    || (Modelo.TipoModelo == WoTypeModel.TransactionNoContable)
                    || (Modelo.TipoModelo == WoTypeModel.Control)
                    || (Modelo.TipoModelo == WoTypeModel.Kardex)
                    || (Modelo.TipoModelo == WoTypeModel.DataMart)
                    || (Modelo.TipoModelo == WoTypeModel.Parameter)
                )
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
                    drRow["Proceso"] = Modelo.ProcesoId;
                    drRow["Tipo"] = Modelo.TipoModelo.ToString();
                    drRow["SubTipo"] = Modelo.SubTipoModelo.ToString();
                    drRow["Modelo"] = Modelo.Id;
                    drRow["Roles"] = Modelo.GetRolesCSV();
                    drRow["OtrosRoles"] = Modelo.GetOtrosRolesCSV();
                    drRow["Json"] = Modelo.Diagrama.ToJson();
                    drRow["Hecho"] = !(
                        (Modelo.Diagrama == null) || (Modelo.Diagrama.Estados.Count == 0)
                    );
                    dt.Rows.Add(drRow);
                }
            }

            grdModelos.DataSource = dt;

            GridColumn col = grdModelosView.Columns["Proceso"];
            col.Width = 100;

            col = grdModelosView.Columns["Tipo"];
            col.Width = 100;

            col = grdModelosView.Columns["SubTipo"];
            col.Width = 100;

            col = grdModelosView.Columns["Modelo"];
            col.Width = 300;

            col = grdModelosView.Columns["Modelo"];
            col.Width = 300;

            col = grdModelosView.Columns["Json"];
            col.Visible = false;

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buRefrescarModelos_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            CargarModelo();
        }

        private void fmModelosLogica_Load(object sender, EventArgs e) { }

        private void buZoomMas_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            woDiagram1.ZoomIn();
        }

        private void buZoomMenos_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            woDiagram1.ZoomOut();
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

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                XtraMessageBox.Show(
                    "Seleccione un renglón",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            Modelo model = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            if (model == null)
            {
                XtraMessageBox.Show(
                    "Al parecer el modelo fue borrado, no es posible continuar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (model.SubTipoModelo == WoSubTypeModel.Extension)
            {
                model.DiagramaTipo = Modelo.tDiagramaTipo.Extender;
                Modelo modeloBase = ProyectoConPaquetes.GetModeloBase(proyecto, model.Id);
                model.Diagrama = ProyectoConPaquetes.DiagramaExtend(modeloBase, model);
                woDiagram1.currModelo = model;
                woDiagram1.currDiagrama = model.Diagrama;
                woDiagram1.AsignaDTO(model);
                woDiagram1.CreaDiagrama();
            }
            else if (model.SubTipoModelo == WoSubTypeModel.Override)
            {
                model.DiagramaTipo = Modelo.tDiagramaTipo.Remplazar;
                Modelo modeloBase = ProyectoConPaquetes.GetModeloBase(proyecto, model.Id);
                model.Diagrama = ProyectoConPaquetes.DiagramaExtend(modeloBase, model);
                woDiagram1.currModelo = model;
                woDiagram1.currDiagrama = model.Diagrama;
                woDiagram1.AsignaDTO(model);
                woDiagram1.CreaDiagrama();
            }
            else if (
                (drRow["Json"].IsNullOrStringTrimEmpty())
                || (ModeloDiagrama.FromJson(drRow["Json"].ToString()).Estados.Count == 0)
            )
            {
                bool bCargoDefault = false;

                if (
                    (model.SubTipoModelo == WoSubTypeModel.Extension)
                    || (model.SubTipoModelo == WoSubTypeModel.Override)
                ) { }
                else if (
                    (model.TipoModelo == WoTypeModel.Catalog)
                    || (model.TipoModelo == WoTypeModel.CatalogType)
                )
                {
                    woDiagram1.currDiagrama = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonCatalogo
                    );
                    bCargoDefault = true;
                }
                else if ((model.TipoModelo == WoTypeModel.Configuration))
                {
                    woDiagram1.currDiagrama = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonConfiguracion
                    );
                    bCargoDefault = true;
                }
                else if ((model.TipoModelo == WoTypeModel.Parameter))
                {
                    woDiagram1.currDiagrama = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonParametro
                    );
                    bCargoDefault = true;
                }
                else if (model.TipoModelo == WoTypeModel.TransactionContable)
                {
                    woDiagram1.currDiagrama = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonTransaccionContable
                    );
                    bCargoDefault = true;
                }
                else if (model.TipoModelo == WoTypeModel.TransactionNoContable)
                {
                    woDiagram1.currDiagrama = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonTransaccion
                    );
                    bCargoDefault = true;
                }
                else if (
                    (model.TipoModelo == WoTypeModel.Kardex)
                    || (model.TipoModelo == WoTypeModel.DataMart)
                )
                {
                    woDiagram1.currDiagrama = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonKardex
                    );
                    bCargoDefault = true;
                }
                else if (model.TipoModelo == WoTypeModel.Control)
                {
                    woDiagram1.currDiagrama = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonControl
                    );
                    bCargoDefault = true;
                }

                if (bCargoDefault)
                {
                    woDiagram1.AsignaDTO(model);
                    woDiagram1.CreaDiagrama();
                    woDiagram1.MarcarPorFlujo();

                    XtraMessageBox.Show("Se cargo la plantilla default para el modelo");
                }
            }

            woDiagram1.MarcarDefault();

            buEditar.Enabled = false;
            buRefrescar.Enabled = false;
            buRegistrar.Enabled = (
                drRow[@"Modelo"].ToSafeString().StartsWith(wooWConfigParams.Origen)
                || (drRow[@"SubTipo"].ToSafeString() == WoSubTypeModel.Extension.GetDescription())
                || (drRow[@"SubTipo"].ToSafeString() == WoSubTypeModel.Override.GetDescription())
            );
            buCopiar.Enabled = false;
            buAnular.Enabled = true;
            buBorrar.Enabled = false;
            buZoomMas.Enabled = true;
            buZoomMenos.Enabled = true;
            buRedibujar.Enabled = true;
            buAnalizaRolesDeLectura.Enabled = true;
            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            grpCambios.Enabled = false;
            optMostrarPermisos.Enabled = true;
            buMuestraDTO.Enabled = true;
            txtFiltroApps.Enabled = false;
            woDiagram1.SetReadOnly(false);

            woDiagram1.SetReadOnlyDiagram(model.SubTipoModelo == WoSubTypeModel.Extension);
        }

        private void buGrabar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            woDiagram1.Validate();
            AnalizaRolesDeLectura(false);

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                grdModelosView_FocusedRowChanged(null, null);
                return;
            }

            Modelo model = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            if (model == null)
            {
                XtraMessageBox.Show(
                    "Al parecer el modelo fue borrado, no es posible continuar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            woDiagram1.ActulizaPosiciones();

            if (!ValidarDiagrama())
                return;

            drRow["Json"] = woDiagram1.currDiagrama.Clone().ToJson();
            drRow["Hecho"] = !(
                (woDiagram1.currDiagrama == null) || (woDiagram1.currDiagrama.Estados.Count == 0)
            );

            proyecto.SaveDiagram(drRow["Modelo"].ToString(), woDiagram1.currDiagrama);

            drRow["Roles"] = model.GetRolesCSV();

            buEditar.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCopiar.Enabled = true;
            buBorrar.Enabled = true;
            buZoomMas.Enabled = false;
            buZoomMenos.Enabled = false;
            buRedibujar.Enabled = false;
            buRefrescar.Enabled = true;
            buAnalizaRolesDeLectura.Enabled = false;
            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;
            optMostrarPermisos.Enabled = false;
            buMuestraDTO.Enabled = false;
            txtFiltroApps.Enabled = true;

            woDiagram1.SetReadOnly(true);
        }

        private void buDeshacer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

            grdModelosView_FocusedRowChanged(null, null);

            buEditar.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCopiar.Enabled = true;
            buBorrar.Enabled = true;
            buZoomMas.Enabled = false;
            buZoomMenos.Enabled = false;
            buRedibujar.Enabled = false;
            txtFiltroApps.Enabled = true;
            buAnalizaRolesDeLectura.Enabled = false;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;

            woDiagram1.SetReadOnly(true);
        }

        private bool ValidarDiagrama()
        {
            try
            {
                DataRow drRow = grdModelosView.GetFocusedDataRow();

                Modelo m = this
                    .proyecto.ModeloCol.Modelos.Where(e => e.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                var Referencia = m.Id + "Id";

                if (
                    (m.SubTipoModelo != WoSubTypeModel.Extension)
                    && (woDiagram1.currDiagrama.Roles.Count == 0)
                )
                    throw new Exception("Registre al menos un Rol");

                bool RolesNoValidar = false;
                int iRolesNoValidar = 0;
                // Valida que todos los roles existan
                foreach (var rol in woDiagram1.currDiagrama.Roles)
                {
                    if (rol.RolId == Proyecto.NOVALIDAR)
                    {
                        RolesNoValidar = true;
                        continue;
                    }

                    iRolesNoValidar++;

                    var a = this.proyecto.Roles.Where(e => e.Id == rol.RolId).FirstOrDefault();
                    if (a.IsNull())
                        throw new Exception($"Rol {rol.RolId} no existe");
                }

                // Valida que todos los permisos existan
                foreach (var tra in woDiagram1.currDiagrama.Transiciones)
                {
                    if (tra.Permisos.Count == 0)
                        throw new Exception(
                            $"Registre al menos un Permiso para la transición {tra.Id} no existe"
                        );

                    if (tra.Id.ToUpper().Equals("BORRAR"))
                        throw new Exception("La transición no puede llamarse Borrar");

                    if (
                        woDiagram1
                            .currDiagrama.Transiciones.Where(x => x.Id == tra.Id)
                            .ToList()
                            .Count() > 1
                    )
                        throw new Exception(
                            $"El nombre de la transicion {tra.Id} no puede repetirse"
                        );

                    bool PermisosNoValidar = false;
                    int iPermisosNoValidar = 0;
                    foreach (var per in tra.Permisos)
                    {
                        if (per.PermisoId == Proyecto.NOVALIDAR)
                        {
                            PermisosNoValidar = true;
                            continue;
                        }

                        iPermisosNoValidar++;

                        var a = this
                            .proyecto.Permisos.Where(e => e.Id == per.PermisoId)
                            .FirstOrDefault();
                        if (a.IsNull())
                            throw new Exception(
                                $"Permiso {per.PermisoId} de la transición {tra.Id} no existe"
                            );
                    }

                    if (PermisosNoValidar && (iPermisosNoValidar > 0))
                        throw new Exception(
                            $"Para la transición {tra.Id}, si selecciona {Proyecto.NOVALIDAR} en permisos, selecciónelo solo "
                        );
                }

                if (RolesNoValidar && (iRolesNoValidar > 0))
                    throw new Exception(
                        $"Si selecciona {Proyecto.NOVALIDAR} en roles, seleccionelo solo "
                    );

                // Las extensiones no se validan
                if (m.SubTipoModelo == WoSubTypeModel.Extension)
                    return true;

                // Valida que todas las transiciones al menos tengan una propiedad seleccionada para generar el DTO
                foreach (var tra in woDiagram1.currDiagrama.Transiciones)
                {
                    // ToDo validar que todas las transiciones tenga estados inicial y final

                    if ((tra.EstadoInicial == 0) && (tra.EstadoFinal == 0))
                        throw new Exception(
                            $"No se permiten transiciones de estado 0 a 0 DTO Transición {tra.Id}"
                        );

                    // Quitar los campos que sean obsoletos
                    bool bSeleccionoControlTransaccion = false;
                    bool bSeleccionoWoPolizaId = false;
                    bool bRowVersion = false;
                    bool bWoState = false;
                    bool bId = false;
                    bool bIdIsAutoIncrement = false;
                    List<string> sl = new List<string>();
                    foreach (var dto in tra.DTO.Columnas)
                    {
                        var a = m.Columnas.Where(e => e.Id == dto).FirstOrDefault();
                        if (a.IsNull())
                        {
                            // referencias que no se borren
                            //var b = m.Referencias
                            //    .Where(e => $"{e.ModeloId}Id" == dto)
                            //    .FirstOrDefault();
                            //if (b.IsNull())
                            sl.Add(dto);
                        }
                        else
                        {
                            if (a.Primaria)
                            {
                                bId = true;
                                bIdIsAutoIncrement = a.TipoColumna == WoTypeColumn.Autoincrement;
                            }
                            if (a.Id == "WoState")
                                bWoState = true;
                            if (a.Id == WoConst.ROWVERSION)
                                bRowVersion = true;
                            if (a.Id == WoConst.WOCONPOLIZAID)
                            {
                                throw new Exception(
                                    $"No debe seleccionar {WoConst.WOCONPOLIZAID} para la DTO Transición {tra.Id}"
                                );
                            }
                            if (
                                (a.Id == WoConst.WOUDNID)
                                || (a.Id == WoConst.WOSERIE)
                                || (a.Id == WoConst.WOFOLIO)
                                || (a.Id == WoConst.WOGUID)
                                || (a.Id == WoConst.WOPERIODOID)
                            )
                                bSeleccionoControlTransaccion = true;
                        }

                        if (tra.EstadoInicial == 0)
                        {
                            List<string> ColumnasEscluir = new List<string>
                            {
                                WoConst.CREATEDBY,
                                WoConst.MODIFIEDBY,
                                WoConst.DELETEDBY,
                                WoConst.CREATEDDATE,
                                WoConst.MODIFIEDDATE,
                                WoConst.DELETEDDATE,
                                WoConst.ROWVERSION
                            };

                            foreach (var col in m.Columnas)
                            {
                                if (
                                    (!col.Nulo)
                                    && (col.TipoColumna != WoTypeColumn.Autoincrement)
                                    && (col.Default.IsNullOrStringEmpty())
                                    && (ColumnasEscluir.IndexOf(col.Id) == -1)
                                )
                                {
                                    if (
                                        (col.TipoColumna == WoTypeColumn.Reference)
                                        && (col.EsColeccion)
                                    )
                                        continue;

                                    if (
                                        tra.DTO.Columnas.Where(e => e == col.Id).FirstOrDefault()
                                        == null
                                    )
                                        throw new Exception(
                                            $"Debe seleccionar la propiedad {col.Id} no nula, sin default, para la DTO Transición {tra.Id}"
                                        );
                                }
                            }
                        }
                    }

                    if (
                        (m.TipoModelo == WoTypeModel.TransactionContable)
                        || (m.TipoModelo == WoTypeModel.TransactionNoContable)
                    )
                    {
                        if (tra.EstadoInicial == 0)
                        {
                            if (!bSeleccionoControlTransaccion)
                                throw new Exception(
                                    $"Debe seleccionar {WoConst.WOUDNID}, {WoConst.WOSERIE}, {WoConst.WOFOLIO}, {WoConst.WOPERIODOID} y Guid para la DTO Transición {tra.Id}"
                                );
                        }
                        else
                        {
                            if (bSeleccionoControlTransaccion)
                                throw new Exception(
                                    $"No debe seleccionar WoUdnId o WoSerie o WoFolio o {WoConst.WOPERIODOID} o Guid para la DTO Transición {tra.Id}"
                                );
                        }
                    }

                    foreach (string s in sl)
                        tra.DTO.Columnas.Remove(s);

                    // Todo debe tener seleccionado Id y Renglon

                    List<ModeloDiagramaTransicionDTOColeccion> Borrar =
                        new List<ModeloDiagramaTransicionDTOColeccion>();

                    // Verifica las colleciones
                    foreach (var coll in tra.DTO.Colleccion)
                    {
                        bool bSeleccionoReferencia = false;
                        bool bExisteReferencia = false;

                        Modelo mColl = this
                            .proyecto.ModeloCol.Modelos.Where(e => e.Id == coll.ModeloId)
                            .FirstOrDefault();

                        if (mColl == null)
                        {
                            Borrar.Add(coll);
                            continue;
                        }

                        if (mColl.Columnas.Where(e => e.Id == Referencia).FirstOrDefault() != null)
                            bExisteReferencia = true;

                        bool bIdColl = false;
                        bool bRenglonColl = false;
                        List<string> slColl = new List<string>();
                        foreach (var dto in coll.Columnas)
                        {
                            var a = mColl.Columnas.Where(e => e.Id == dto).FirstOrDefault();

                            if (a.IsNull())
                            {
                                // referencias que no se borren
                                //var b = mColl.Referencias
                                //    .Where(e => $"{e.ModeloId}Id" == dto)
                                //    .FirstOrDefault();
                                //if (b.IsNull())
                                slColl.Add(dto);
                            }
                            else
                            {
                                if (a.Primaria)
                                    bIdColl = true;
                                if (a.Id == WoConst.WORENGLON)
                                    bRenglonColl = true;
                                if (a.Id == Referencia)
                                    bSeleccionoReferencia = true;
                            }
                        }

                        if (!bExisteReferencia)
                            throw new Exception(
                                $"La colleccion {coll.ModeloId} le falta la Referencia {Referencia}"
                            );

                        foreach (string s in slColl)
                            coll.Columnas.Remove(s);

                        if (coll.Insertar || coll.Actualizar || coll.Borrar)
                        {
                            if (coll.Columnas.Count == 0)
                                throw new Exception(
                                    $"Seleccione al menos una propiedad para la DTO Transición {tra.Id} colleccion {coll.ModeloId}"
                                );
                            if (!bRenglonColl)
                                throw new Exception(
                                    $"Siempre debe seleccionar la propiedad Renglón para el DTO Transición {tra.Id} colleccion {coll.ModeloId}"
                                );
                            if (bIdColl)
                                throw new Exception(
                                    $"No debe seleccionar la propiedad Id para el DTO Transición {tra.Id} colleccion {coll.ModeloId}"
                                );

                            if (bSeleccionoReferencia)
                                throw new Exception(
                                    $"No debe seleccionar la propiedad {Referencia} para el DTO Transición {tra.Id} colleccion {coll.ModeloId}"
                                );

                            if ((tra.EstadoInicial == 0) && ((coll.Actualizar || coll.Borrar)))
                                throw new Exception(
                                    $"Solo debe seleccionar insertar para estado inicial DTO Transición {tra.Id} colleccion {coll.ModeloId}"
                                );
                        }

                        if (coll.Columnas.Count > 0)
                        {
                            if (!coll.Insertar && !coll.Actualizar && !coll.Borrar)
                                throw new Exception(
                                    $"No seleccione propiedades para la DTO Transición {tra.Id} colleccion {coll.ModeloId} cuando se no seleccione insertar, actualizar o borrar"
                                );
                        }
                    }

                    foreach (var borrar in Borrar)
                    {
                        tra.DTO.Colleccion.Remove(borrar);
                    }

                    if (tra.DTO.Columnas.Count == 0)
                        throw new Exception(
                            $"Seleccione al menos una propiedad para el DTO Transición {tra.Id}"
                        );

                    if (
                        (m.TipoModelo == WoTypeModel.Control)
                        || (m.TipoModelo == WoTypeModel.Kardex)
                        || (m.TipoModelo == WoTypeModel.DataMart)
                    )
                    {
                        if ((tra.EstadoInicial == 0) && (bId))
                            throw new Exception(
                                $"No seleccione la propiedad Id para DTO Transición {tra.Id}"
                            );
                        if ((tra.EstadoInicial != 0) && (!bId))
                            throw new Exception(
                                $"Seleccione la propiedad Id para DTO Transición {tra.Id}"
                            );
                    }
                    else
                    {
                        if (!bId)
                            throw new Exception(
                                $"Seleccione la propiedad Id para DTO Transición {tra.Id}"
                            );

                        if (!bWoState)
                            throw new Exception(
                                $"Seleccione la propiedad WoState para DTO Transición {tra.Id}"
                            );
                    }

                    if (tra.EstadoInicial == 0)
                    {
                        if (bRowVersion)
                            throw new Exception(
                                $"No seleccione RowVersion para transición con estado inicial = 0, DTO Transición {tra.Id}"
                            );
                    }
                    else
                    {
                        if (!bRowVersion)
                            throw new Exception(
                                $"Seleccione RowVersion para la transición {tra.Id}"
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private void grdModelosView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            buEditar.Enabled = false;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCopiar.Enabled = false;
            buBorrar.Enabled = true;
            buRefrescar.Enabled = true;
            txtFiltroApps.Enabled = true;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                woDiagram1.New();
                return;
            }

            if (drRow["Json"] != DBNull.Value)
            {
                var Model = this
                    .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                woDiagram1.New();
                woDiagram1.currDiagrama = ModeloDiagrama.FromJson(drRow["Json"].ToString());
                woDiagram1.currDiagrama.ProyectoSetter(proyecto);
                woDiagram1.AsignaDTO(Model);
                woDiagram1.CreaDiagrama();
            }
            else { }

            buEditar.Enabled = true;
            buCopiar.Enabled = true;
            buRefrescar.Enabled = true;
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

        private void GrabaEnProyecto()
        {
            DataTable dt = grdModelos.DataSource as DataTable;

            foreach (DataRow drRow in dt.Rows)
            {
                foreach (Modelo m in proyecto.ModeloCol.Modelos)
                {
                    if (m.Id == drRow["Modelo"].ToString())
                    {
                        m.Diagrama = ModeloDiagrama.FromJson(drRow["Json"].ToString());
                        m.ProyectoSetter(proyecto);
                        break;
                    }
                }
            }
        }

        private void buRedibujar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            woDiagram1.ActulizaPosiciones();
            woDiagram1.CreaDiagrama();
        }

        private void optMostrarPermisos_EditValueChanged(object sender, EventArgs e)
        {
            woDiagram1.MostrarPermisos = optMostrarPermisos.EditValue.ToBoolean();
            buRedibujar.PerformClick();
        }

        private void buMuestraDTO_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                return;
            }

            if (drRow["Json"] != DBNull.Value)
            {
                var Model = this
                    .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                var fmMostrar = new fmSummaryDTO(Model, woDiagram1.currDiagrama);

                fmMostrar.ShowDialog();
            }
        }

        private void txtFiltroApps_HiddenEditor(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            CargarModelo();
        }

        private void buBorrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
                return;

            Modelo model = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            if (model == null)
            {
                XtraMessageBox.Show(
                    "Al parecer el modelo fue borrado, no es posible continuar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (
                XtraMessageBox.Show(
                    "Borrar el diagrama?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            woDiagram1.currModelo = model;
            woDiagram1.currDiagrama = new ModeloDiagrama();

            drRow["Json"] = woDiagram1.currDiagrama.Clone().ToJson();
            drRow["Hecho"] = !(
                (woDiagram1.currDiagrama == null) || (woDiagram1.currDiagrama.Estados.Count == 0)
            );

            proyecto.SaveDiagram(drRow["Modelo"].ToString(), woDiagram1.currDiagrama);

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buCopiar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                XtraMessageBox.Show(
                    "Seleccione un renglón",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            Modelo model = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            if (model.Diagrama.Transiciones.Count() > 0)
            {
                XtraMessageBox.Show(
                    "El modelo actual ya tiene diagrama",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (model.SubTipoModelo == WoSubTypeModel.Extension)
            {
                XtraMessageBox.Show(
                    "No se puede copiar a un modelo de extensión",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var models = this
                .proyecto.ModeloCol.Modelos.Where(j => j.TipoModelo == model.TipoModelo)
                .ToList();

            var DiagramSelect = new fmDiagramSelect(models.Select(x => x.Id).ToList());

            if (DiagramSelect.ShowDialog() != DialogResult.OK)
                return;

            Modelo modelDiagrama = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == DiagramSelect.DiagramName)
                .FirstOrDefault();

            if (modelDiagrama == null)
                return;

            var cloneDiagram = modelDiagrama.Diagrama.Clone();

            foreach (var transicion in cloneDiagram.Transiciones)
            {
                transicion.DTO.Columnas.Clear();
                transicion.DTO.ColumnasNoEditar.Clear();
                transicion.DTO.Colleccion.Clear();
            }

            woDiagram1.currDiagrama = cloneDiagram;
            woDiagram1.currModelo = model;
            woDiagram1.AsignaDTO(model);
            woDiagram1.CreaDiagrama();
            woDiagram1.MarcarPorFlujo();

            proyecto.SaveDiagram(drRow["Modelo"].ToString(), woDiagram1.currDiagrama);

            drRow["Json"] = woDiagram1.currDiagrama.ToJson();
            drRow["Hecho"] = (woDiagram1.currDiagrama.Estados.Count > 0);

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buAnalizaRolesDeLectura_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            AnalizaRolesDeLectura(true);
        }

        private void AnalizaRolesDeLectura(bool MensajesCorrecto)
        {
            var RolesActuales = new List<string>();

            foreach (var rol in woDiagram1.currDiagrama.Roles)
                if (RolesActuales.IndexOf(rol.RolId) == -1)
                    RolesActuales.Add(rol.RolId);
            foreach (var rol in woDiagram1.currDiagrama.RolesLectura)
                if (RolesActuales.IndexOf(rol.RolId) == -1)
                    RolesActuales.Add(rol.RolId);
            foreach (var transicion in woDiagram1.currDiagrama.Transiciones)
            foreach (var rol in transicion.Roles)
                RolesActuales.Add(rol.RolId);

            var RolesNecesarios = new List<string>();

            RolesNecesarios = new List<string>();
            foreach (var rol in woDiagram1.currDiagrama.Roles)
                if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                    RolesNecesarios.Add(rol.RolId);
            foreach (var transicion in woDiagram1.currDiagrama.Transiciones)
            foreach (var rol in transicion.Roles)
                if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                    RolesNecesarios.Add(rol.RolId);

            foreach (var modelo in proyecto.ModeloCol.Modelos)
            {
                if (modelo == woDiagram1.currModelo)
                    continue;

                foreach (var columna in modelo.Columnas)
                {
                    if (columna.ModeloId == woDiagram1.currModelo.Id)
                    {
                        foreach (var rol in modelo.Diagrama.Roles)
                            if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                                RolesNecesarios.Add(rol.RolId);
                    }
                }

                // Igual para coleciones
                foreach (
                    var columna in modelo.Columnas.Where(e =>
                        (e.EsColeccion) && (e.TipoColumna == WoTypeColumn.Reference)
                    )
                )
                {
                    var locModelo = proyecto
                        .ModeloCol.Modelos.Where(e => e.Id == columna.ModeloId)
                        .FirstOrDefault();

                    if (locModelo != null)
                    {
                        foreach (var locColumna in locModelo.Columnas)
                        {
                            if (locColumna.ModeloId == woDiagram1.currModelo.Id)
                            {
                                // Debe tomar los roles del modelo no de la coleccion
                                foreach (var rol in modelo.Diagrama.Roles)
                                    if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                                        RolesNecesarios.Add(rol.RolId);
                            }
                        }
                    }
                }
            }

            Modelo currModel = woDiagram1.currModelo;

            var RolesSobrantes = new List<string>();

            var RolesFaltantes = currModel.AnalizaRolesDeLectura(
                RolesActuales,
                RolesNecesarios,
                RolesSobrantes
            );

            if (RolesFaltantes.Count == 0)
            {
                if (MensajesCorrecto)
                {
                    XtraMessageBox.Show(
                        $"No faltan roles de lectura",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                if (RolesSobrantes.Count > 0)
                {
                    string sRolesSobrantes = string.Join(", ", RolesSobrantes);
                    XtraMessageBox.Show(
                        $"Sobran los roles de lectura {sRolesSobrantes}",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                return;
            }

            string sRolesFaltantes = string.Join(", ", RolesFaltantes);

            if (
                XtraMessageBox.Show(
                    $"Faltan los roles {sRolesFaltantes} agregarlos a la lista?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            foreach (var rol in RolesFaltantes)
                woDiagram1.currDiagrama.RolesLectura.Add(new ModeloRol { RolId = rol });
        }

        private void woDiagram1_Load(object sender, EventArgs e) { }
    }
}
