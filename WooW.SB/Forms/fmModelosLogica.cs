using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Config.Enum;
using WooW.SB.Config.Helpers;
using WooW.SB.Dialogs;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmModelosLogica : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public Proyecto proyecto { get; set; }

        public fmModelosLogica()
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

        public void Cargar()
        {
            CargarModelo();

            grdModelosView_FocusedRowChanged(null, null);

            woDiagram1.Enabled = true;
            woDiagram1.SetReadOnly(true);
        }

        public void CargarModelo()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Tipo", typeof(string));
            dt.Columns.Add("Modelo", typeof(string));
            dt.Columns.Add("Json", typeof(string));

            foreach (var Modelo in proyecto.ModeloCol.Modelos)
            {
                if (
                    (Modelo.TipoModelo == TypeModel.Configuration)
                    || (Modelo.TipoModelo == TypeModel.CatalogType)
                    || (Modelo.TipoModelo == TypeModel.Catalog)
                    || (Modelo.TipoModelo == TypeModel.TransactionContable)
                    || (Modelo.TipoModelo == TypeModel.TransactionNoContable)
                    || (Modelo.TipoModelo == TypeModel.Control)
                    || (Modelo.TipoModelo == TypeModel.Kardex)
                    || (Modelo.TipoModelo == TypeModel.DataMart)
                    || (Modelo.TipoModelo == TypeModel.Parameter)
                )
                {
                    DataRow drRow = dt.NewRow();
                    drRow["Proceso"] = Modelo.ProcesoId;
                    drRow["Tipo"] = Modelo.TipoModelo.ToString();
                    drRow["Modelo"] = Modelo.Id;
                    drRow["Json"] = Modelo.Diagrama.ToJson();
                    dt.Rows.Add(drRow);
                }
            }

            grdModelos.DataSource = dt;

            GridColumn col = grdModelosView.Columns["Proceso"];
            col.Width = 100;

            col = grdModelosView.Columns["Tipo"];
            col.Width = 100;

            col = grdModelosView.Columns["Modelo"];
            col.Width = 300;

            col = grdModelosView.Columns["Json"];
            col.Visible = false;
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
            // ToDo Generar diagrama default de acuerdo al tipo de modelo
            DataRow drRow = grdModelosView.GetFocusedDataRow();

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
                (drRow["Json"].IsNullOrStringTrimEmpty())
                || (ModeloDiagrama.FromJson(drRow["Json"].ToString()).Estados.Count == 0)
            )
            {
                bool bCargoDefault = false;

                if (
                    (model.TipoModelo == TypeModel.Catalog)
                    || (model.TipoModelo == TypeModel.CatalogType)
                )
                {
                    woDiagram1.currModel = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonCatalogo
                    );
                    bCargoDefault = true;
                }
                else if ((model.TipoModelo == TypeModel.Configuration))
                {
                    woDiagram1.currModel = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonConfiguracion
                    );
                    bCargoDefault = true;
                }
                else if ((model.TipoModelo == TypeModel.Parameter))
                {
                    woDiagram1.currModel = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonParametro
                    );
                    bCargoDefault = true;
                }
                else if (model.TipoModelo == TypeModel.TransactionContable)
                {
                    woDiagram1.currModel = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonTransaccion
                    );
                    bCargoDefault = true;
                }
                else if (
                    (model.TipoModelo == TypeModel.Control)
                    || (model.TipoModelo == TypeModel.Kardex)
                    || (model.TipoModelo == TypeModel.DataMart)
                )
                {
                    woDiagram1.currModel = ModeloDiagrama.FromJson(
                        WooW.SB.Properties.Resources.JsonControl
                    );
                    bCargoDefault = true;
                }

                if (bCargoDefault)
                {
                    woDiagram1.AsignaDTO(model);
                    woDiagram1.CreaDiagrama();

                    XtraMessageBox.Show("Se cargo la plantilla default para el modelo");
                }
            }

            buEditar.Enabled = false;
            buRefrescarModelos.Enabled = false;
            buRegistrar.Enabled = true;
            buAnular.Enabled = true;
            buZoomMas.Enabled = true;
            buZoomMenos.Enabled = true;
            buRedibujar.Enabled = true;
            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            grpCambios.Enabled = false;
            optMostrarPermisos.Enabled = true;
            buMuestraDTO.Enabled = true;

            woDiagram1.SetReadOnly(false);
        }

        private void buGrabar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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

            drRow["Json"] = woDiagram1.currModel.Clone().ToJson();

            proyecto.SaveDiagram(drRow["Modelo"].ToString(), woDiagram1.currModel);

            buEditar.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buZoomMas.Enabled = false;
            buZoomMenos.Enabled = false;
            buRedibujar.Enabled = false;
            buRefrescarModelos.Enabled = false;
            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;
            optMostrarPermisos.Enabled = false;
            buMuestraDTO.Enabled = false;

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
            buZoomMas.Enabled = false;
            buZoomMenos.Enabled = false;
            buRedibujar.Enabled = false;

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

                if (woDiagram1.currModel.Roles.Count == 0)
                    throw new Exception("Registre al menos un Rol");

                // Valida que todos los roles existan
                foreach (var rol in woDiagram1.currModel.Roles)
                {
                    var a = this.proyecto.Roles.Where(e => e.Id == rol.RolId).FirstOrDefault();
                    if (a.IsNull())
                        throw new Exception($"Rol {rol.RolId} no existe");
                }

                // Valida que todos los permisos existan
                foreach (var tra in woDiagram1.currModel.Transiciones)
                {
                    if (tra.Permisos.Count == 0)
                        throw new Exception(
                            $"Registre al menos un Permiso para la transición {tra.Id} no existe"
                        );

                    foreach (var per in tra.Permisos)
                    {
                        var a = this
                            .proyecto.Permisos.Where(e => e.Id == per.PermisoId)
                            .FirstOrDefault();
                        if (a.IsNull())
                            throw new Exception(
                                $"Permiso {per.PermisoId} de la transición {tra.Id} no existe"
                            );
                    }
                }

                // Valida que todas las transiciones al menos tengan una propiedad seleccionada para generar el DTO
                foreach (var tra in woDiagram1.currModel.Transiciones)
                {
                    // ToDo validar que todas las transiciones tenga estados inicial y final

                    if ((tra.EstadoInicial == 0) && (tra.EstadoFinal == 0))
                        throw new Exception(
                            $"No se permiten transiciones de estado 0 a 0 DTO Transición {tra.Id}"
                        );

                    // Quitar los campos que sean obsoletos
                    bool bRowVersion = false;
                    bool bVxState = false;
                    bool bId = false;
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
                                bId = true;
                            if (a.Id == "VxState")
                                bVxState = true;
                            if (a.Id == "RowVersion")
                                bRowVersion = true;
                        }
                    }

                    foreach (string s in sl)
                        tra.DTO.Columnas.Remove(s);

                    // Todo debe tener seleccionado Id y Renglon

                    // Verifica las colleciones
                    foreach (var coll in tra.DTO.Colleccion)
                    {
                        Modelo mColl = this
                            .proyecto.ModeloCol.Modelos.Where(e => e.Id == coll.ModeloId)
                            .FirstOrDefault();

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
                                if (a.Id == "Renglon")
                                    bRenglonColl = true;
                            }
                        }

                        foreach (string s in slColl)
                            coll.Columnas.Remove(s);

                        if (coll.Insertar || coll.Actualizar || coll.Borrar)
                        {
                            if (coll.Columnas.Count == 0)
                                throw new Exception(
                                    $"Seleccione al menos una propiedad para la DTO Transición {tra.Id} colleccion {coll.ModeloId}"
                                );
                            if ((!bIdColl) || (!bRenglonColl))
                                throw new Exception(
                                    $"Siempre debe seleccionar la propiedad Id y Renglon para el DTO Transición {tra.Id} colleccion {coll.ModeloId} cuando se seleccione insertar, actualizar o borrar"
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

                    if (tra.DTO.Columnas.Count == 0)
                        throw new Exception(
                            $"Seleccione al menos una propiedad para el DTO Transición {tra.Id}"
                        );

                    if (
                        (m.TipoModelo == TypeModel.Control)
                        || (m.TipoModelo == TypeModel.Kardex)
                        || (m.TipoModelo == TypeModel.DataMart)
                    )
                    {
                        if ((tra.EstadoInicial == 0) && (bId))
                            throw new Exception(
                                $"No registre la propiedad Id para el DTO Transición {tra.Id}"
                            );
                        if ((tra.EstadoInicial != 0) && (!bId))
                            throw new Exception(
                                $"Siempre debe seleccionar la propiedad Id para el DTO Transición {tra.Id}"
                            );
                    }
                    else
                    {
                        if (!bId)
                            throw new Exception(
                                $"Siempre debe seleccionar la propiedad Id para el DTO Transición {tra.Id}"
                            );

                        if (!bVxState)
                            throw new Exception(
                                $"Siempre debe seleccionar la propiedad VxState para el DTO Transición {tra.Id}"
                            );
                    }

                    if (tra.EstadoInicial == 0)
                    {
                        if (bRowVersion)
                            throw new Exception(
                                $"No seleccione RowVersion para transicion con estado inicial = 0, DTO Transición {tra.Id}"
                            );
                    }
                    else
                    {
                        if (!bRowVersion)
                            throw new Exception(
                                $"Seleccione RowVersion para todas las transiciones excepto estado inicial = 0, DTO Transición {tra.Id}"
                            );
                    }
                }

                // ToDo Validar Columnas que va y no van de acuerdo el tipo de tabla

                // ToDo Validar tipo de columna Id (debe llamarse y ser string)

                // ToDo

                // ToDo Validar SubTipo de acuerdo a Tipo

                // Checar el flujo de acurdo al SubTipo
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
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            buEditar.Enabled = false;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;

            if (drRow.IsNull())
            {
                return;
            }

            if (drRow["Json"] != DBNull.Value)
            {
                var Model = this
                    .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                woDiagram1.New();

                woDiagram1.currModel = ModeloDiagrama.FromJson(drRow["Json"].ToString());
                woDiagram1.AsignaDTO(Model);
                woDiagram1.CreaDiagrama();
            }
            else { }

            buEditar.Enabled = true;
            buRefrescarModelos.Enabled = true;
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

                var fmMostrar = new fmSummaryDTO(Model, woDiagram1.currModel);

                fmMostrar.ShowDialog();
            }
        }
    }
}
