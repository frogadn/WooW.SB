using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.BlazorGenerator;
using WooW.SB.BlazorGenerator.BlazorDialogs;
using WooW.SB.BlazorGenerator.BlazorDialogs.BlazorDialogsModels;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Deploy.MetaDataManager;
using WooW.SB.Deploy.Models;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Forms
{
    public partial class fmIntegralGenerations : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        public Proyecto proyecto { get; set; }

        /// <summary>
        /// Observador de logs.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Attributes

        /// <summary>
        /// Colección de todos los modelos.
        /// </summary>
        private DataTable _modelsCol;

        #endregion Attributes

        #region Constructor y métodos base del formulario

        /// <summary>
        /// Constructor principal del formulario.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public fmIntegralGenerations()
        {
            InitializeComponent();
        }

        [SupportedOSPlatform("windows")]
        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        [SupportedOSPlatform("windows")]
        public bool CambiosPendientes
        {
            get { return false; }
        }

        [SupportedOSPlatform("windows")]
        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        #endregion Constructor y métodos base del formulario


        #region Cargar formulario

        /// <summary>
        /// Método principal de la interfaz que se ejecuta al cargar el formulario.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void Cargar()
        {
            if (grdModels.DataSource != null)
            {
                grdModels.DataSource = null;
            }

            if (File.Exists($@"{proyecto.DirProyectTemp}\IProjectType.txt"))
            {
                string projectType = WoDirectory.ReadFile(
                    $@"{proyecto.DirProyectTemp}\IProjectType.txt"
                );
                cmbAproach.EditValue = projectType;
                string ejecutionType = WoDirectory.ReadFile(
                    $@"{proyecto.DirProyectTemp}\IEjecutionType.txt"
                );
                cmbEjecution.EditValue = ejecutionType;
            }
            else
            {
                cmbAproach.EditValue = "Server";
                cmbEjecution.EditValue = "DotNet Cli";
            }

            pnlGrid.Enabled = false;

            ChargeProyects();

            ChargeGridModels();

            _woBlazorServerExecute.GeneralStop();
            _woBlazorWasmExecute.GeneralStop();

            DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbProyectControl =
                cmbProyecto.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
            cmbProyectControl.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;

            DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbEjecutionControl =
                cmbEjecution.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
            cmbEjecutionControl.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;

            DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbAproachControl =
                cmbAproach.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
            cmbAproachControl.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;

            DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbDockerDeployControl =
                cmbDockerDeploy.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
            cmbDockerDeployControl.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;

            sccDocker.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;

            string assemblyPath = $@"C:\Frog\WooW.Assemblies\WooW.WebClient.dll";
            if (File.Exists(assemblyPath))
            {
                //btnUpdateDockers_ItemClick(null, null);
            }
            else
            {
                XtraMessageBox.Show("No se encuentra el cliente para el servicio de deploy");
            }
        }

        #endregion Cargar formulario


        #region Cargar proyectos

        /// <summary>
        /// Carga los proyectos de la carpeta temporal.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeProyects(string selectedProyect = "Seleccione...")
        {
            List<string> projectsCol = WoDirectory.ReadDirectoryFiles(
                $@"{proyecto.DirLayOuts}\IntegralGenerations",
                onlyNames: true
            );

            projectsCol.Add("Seleccione...");

            if (!projectsCol.Contains(selectedProyect))
            {
                projectsCol.Add(selectedProyect);
            }

            RepositoryItemComboBox comboBox = cmbProyecto.Edit as RepositoryItemComboBox;

            if (comboBox != null)
            {
                // Agrega un nuevo item a la lista desplegable
                comboBox.Items.Clear();
                comboBox.Items.AddRange(projectsCol);
                cmbProyecto.EditValue = selectedProyect;
            }
        }

        #endregion Cargar proyectos


        #region Cargar grid

        /// <summary>
        /// Orquesta la carga de tipo de pantallas que se pueden seleccionar para el proyecto seleccionado.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeGridModels()
        {
            _modelsCol = new DataTable();

            _modelsCol.Columns.Add(@"Proceso", typeof(string));
            _modelsCol.Columns.Add(@"Tipo", typeof(string));
            _modelsCol.Columns.Add(@"Repositorio", typeof(string));
            _modelsCol.Columns.Add(@"Modelo", typeof(string));
            _modelsCol.Columns.Add(@"Tipo de pantalla", typeof(string));
            _modelsCol.Columns.Add(@"Generar", typeof(bool));

            ChargeFormsToGrid();
            ChargeReportsToGrid();
            ChargeListToGrid();

            grdModelsView.Columns[@"Proceso"].OptionsColumn.AllowEdit = false;
            grdModelsView.Columns[@"Tipo"].OptionsColumn.AllowEdit = false;
            grdModelsView.Columns[@"Repositorio"].OptionsColumn.AllowEdit = false;
            grdModelsView.Columns[@"Modelo"].OptionsColumn.AllowEdit = false;
            grdModelsView.Columns[@"Tipo de pantalla"].OptionsColumn.AllowEdit = false;

            grdModelsView.ClearSorting();
            grdModelsView.Columns[@"Proceso"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
        }

        #endregion Cargar grid

        #region Cargar formularios

        /// <summary>
        /// Cargar formularios a la grid
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeFormsToGrid()
        {
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            List<string> forms = woProjectDataHelper.GetFormDesignedModels();

            foreach (string form in forms)
            {
                Modelo model = woProjectDataHelper.GetMainModel(form);
                if (
                    model != null
                    && model.TipoModelo != Core.WoTypeModel.Request
                    && model.TipoModelo != Core.WoTypeModel.Kardex
                    && model.TipoModelo != Core.WoTypeModel.View
                    && model.TipoModelo != Core.WoTypeModel.Control
                    && model.TipoModelo != Core.WoTypeModel.Configuration
                    && model.TipoModelo != Core.WoTypeModel.CatalogSlave
                    && model.TipoModelo != Core.WoTypeModel.TransactionSlave
                )
                {
                    DataRow drRow = _modelsCol.NewRow();
                    drRow[@"Proceso"] = model.ProcesoId;
                    drRow[@"Tipo"] = model.TipoModelo.ToString();
                    drRow[@"Repositorio"] = model.Repositorio.ToString();
                    drRow[@"Modelo"] = model.Id;
                    drRow[@"Tipo de pantalla"] = "Formulario y lista";
                    drRow[@"Generar"] = false;
                    _modelsCol.Rows.Add(drRow);

                    grdModels.DataSource = _modelsCol;

                    GridColumn column = grdModelsView.Columns[@"Proceso"];
                    column.Width = 100;

                    column = grdModelsView.Columns[@"Tipo"];
                    column.Width = 100;

                    column = grdModelsView.Columns[@"Repositorio"];
                    column.Width = 100;

                    column = grdModelsView.Columns[@"Modelo"];
                    column.Width = 100;

                    column = grdModelsView.Columns[@"Tipo de pantalla"];
                    column.Width = 100;

                    column = grdModelsView.Columns[@"Generar"];
                    column.Width = 100;

                    grdModelsView.SortInfo.AddRange(
                        new GridColumnSortInfo[]
                        {
                            new GridColumnSortInfo(
                                column,
                                DevExpress.Data.ColumnSortOrder.Ascending
                            )
                        }
                    );
                }
            }
        }

        #endregion Cargar formularios

        #region Cargar reportes

        /// <summary>
        /// Cargar reportes a la grid
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeReportsToGrid()
        {
            List<string> reports = WoDirectory.ReadDirectoryFiles(
                path: $@"{proyecto.DirLayOuts}\Reports\Vistas"
            );

            List<string> reportModels = new List<string>();

            foreach (string report in reports)
            {
                string fileName = report.Split('\\').Last();

                string modelName = fileName.Split('.').First();
                if (modelName != string.Empty && fileName.Split('.').Last().ToLower() == "xml")
                {
                    if (!reportModels.Contains(modelName))
                    {
                        reportModels.Add(modelName);

                        Modelo model = SearchModelDet(modelName);
                        if (model != null && model.TipoModelo == Core.WoTypeModel.Request)
                        {
                            DataRow drRow = _modelsCol.NewRow();
                            drRow[@"Proceso"] = model.ProcesoId;
                            drRow[@"Tipo"] = model.TipoModelo.ToString();
                            drRow[@"Repositorio"] = model.Repositorio.ToString();
                            drRow[@"Modelo"] = model.Id;
                            drRow[@"Tipo de pantalla"] = "Reporte";
                            drRow[@"Generar"] = false;
                            _modelsCol.Rows.Add(drRow);

                            grdModels.DataSource = _modelsCol;

                            GridColumn column = grdModelsView.Columns[@"Proceso"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Tipo"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Repositorio"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Modelo"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Tipo de pantalla"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Generar"];
                            column.Width = 100;

                            grdModelsView.SortInfo.AddRange(
                                new GridColumnSortInfo[]
                                {
                                    new GridColumnSortInfo(
                                        column,
                                        DevExpress.Data.ColumnSortOrder.Ascending
                                    )
                                }
                            );
                        }
                        else if (model != null && model.TipoModelo != Core.WoTypeModel.Request)
                        {
                            DataRow drRow = _modelsCol.NewRow();
                            drRow[@"Proceso"] = model.ProcesoId;
                            drRow[@"Tipo"] = model.TipoModelo.ToString();
                            drRow[@"Repositorio"] = model.Repositorio.ToString();
                            drRow[@"Modelo"] = model.Id;
                            drRow[@"Tipo de pantalla"] = "Reporte Odata";
                            drRow[@"Generar"] = false;
                            _modelsCol.Rows.Add(drRow);

                            grdModels.DataSource = _modelsCol;

                            GridColumn column = grdModelsView.Columns[@"Proceso"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Tipo"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Repositorio"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Modelo"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Tipo de pantalla"];
                            column.Width = 100;

                            column = grdModelsView.Columns[@"Generar"];
                            column.Width = 100;

                            grdModelsView.SortInfo.AddRange(
                                new GridColumnSortInfo[]
                                {
                                    new GridColumnSortInfo(
                                        column,
                                        DevExpress.Data.ColumnSortOrder.Ascending
                                    )
                                }
                            );
                        }
                    }
                }
            }
        }

        #endregion Cargar reportes

        #region Cargar listas

        /// <summary>
        /// Cargar listas a la grid
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeListToGrid()
        {
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

            List<string> forms = woProjectDataHelper.GetListDesignedModels();

            List<string> added = new List<string>();

            foreach (string form in forms)
            {
                string formName = form.Replace("GridList", "");

                if (!added.Contains(formName))
                {
                    Modelo model = SearchModelDet(formName);

                    if (
                        model != null
                        && (
                            model.TipoModelo == Core.WoTypeModel.Kardex
                            || model.TipoModelo == Core.WoTypeModel.View
                            || model.TipoModelo == Core.WoTypeModel.Control
                            || model.TipoModelo == Core.WoTypeModel.Configuration
                        )
                    )
                    {
                        added.Add(formName);

                        DataRow drRow = _modelsCol.NewRow();
                        drRow[@"Proceso"] = model.ProcesoId;
                        drRow[@"Tipo"] = model.TipoModelo.ToString();
                        drRow[@"Repositorio"] = model.Repositorio.ToString();
                        drRow[@"Modelo"] = model.Id;
                        drRow[@"Tipo de pantalla"] = "Lista";
                        drRow[@"Generar"] = false;
                        _modelsCol.Rows.Add(drRow);

                        grdModels.DataSource = _modelsCol;
                    }
                }
            }

            GridColumn column = grdModelsView.Columns[@"Proceso"];
            column.Width = 100;

            column = grdModelsView.Columns[@"Tipo"];
            column.Width = 100;

            column = grdModelsView.Columns[@"Repositorio"];
            column.Width = 100;

            column = grdModelsView.Columns[@"Modelo"];
            column.Width = 100;

            column = grdModelsView.Columns[@"Tipo de pantalla"];
            column.Width = 100;

            column = grdModelsView.Columns[@"Generar"];
            column.Width = 100;

            grdModelsView.SortInfo.AddRange(
                new GridColumnSortInfo[]
                {
                    new GridColumnSortInfo(column, DevExpress.Data.ColumnSortOrder.Ascending)
                }
            );
        }

        #endregion Cargar listas


        #region Búsqueda del modelo

        /// <summary>
        /// Recupera desde la variable principal del proyecto la instancia del modelo a través del nombre.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        /// <exception cref="WoObserverException"></exception>
        [SupportedOSPlatform("windows")]
        private Modelo SearchModelDet(string modelName)
        {
            Modelo _findModel = null;
            try
            {
                if (modelName != "Seleccione")
                {
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

                    //_findModel = proyecto.ModeloCol.Modelos.Where(x => x.Id == modelName).First();
                    _findModel = woProjectDataHelper.GetMainModel(modelName);
                    if (_findModel == null)
                    {
                        XtraMessageBox.Show(
                            text: $@"El modelo {modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio.",
                            caption: "Error",
                            buttons: MessageBoxButtons.OK,
                            icon: MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"El modelo {modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio. {ex.Message}",
                    caption: "Error",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }

            return _findModel;
        }

        #endregion Búsqueda del modelo


        #region Creación de los proyectos

        /// <summary>
        /// Crea el fichero con las settings de la integral.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnNewProyect_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            WoNewIntegralTest woNewIntegralTest = new WoNewIntegralTest();
            woNewIntegralTest.NewProjectAdded += (string newProjectName) =>
            {
                ChargeProyects(newProjectName);
            };
            woNewIntegralTest.ShowDialog();

            dynamic data = grdModelsView.DataSource;
            foreach (DataRowView row in data)
            {
                row[5] = false;
            }
        }

        #endregion Creación de los proyectos

        #region Eliminado de los proyectos

        /// <summary>
        /// Elimina el proyecto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteProyect_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnStop_ItemClick(null, null);

            string selectedProyect = cmbProyecto.EditValue.ToString();
            string pathFileSelectedProyect =
                $@"{proyecto.DirLayOuts}\IntegralGenerations\{selectedProyect}.json";

            if (File.Exists(pathFileSelectedProyect))
            {
                WoDirectory.DeleteFile(pathFileSelectedProyect);
            }

            string pathProyects = $@"{proyecto.DirProyectTemp}\{selectedProyect}";

            if (Directory.Exists($@"{pathProyects}Server"))
            {
                WoDirectory.DeleteDirectory($@"{pathProyects}Server");
            }

            if (Directory.Exists($@"{pathProyects}Wasm"))
            {
                WoDirectory.DeleteDirectory($@"{pathProyects}Wasm");
            }

            cmbProyecto.EditValue = "Seleccione...";

            ChargeProyects();

            dynamic data = grdModelsView.DataSource;
            foreach (DataRowView row in data)
            {
                row[5] = false;
            }
        }

        #endregion Eliminado de los proyectos


        #region Atributos de ejecución

        /// <summary>
        /// Instancia que permite la ejecución del proyecto server de blazor.
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Instancia que permite la ejecución del proyecto wasm de blazor.
        /// </summary>
        private WoBlazorWasmExecute _woBlazorWasmExecute = WoBlazorWasmExecute.GetInstance();

        #endregion Atributos de ejecución

        #region Ejecución de los proyectos

        [SupportedOSPlatform("windows")]
        private async void btnEjecute_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnStop_ItemClick(null, null);

            consoleData.Text = string.Empty;
            _sendError = false;

            if (
                cmbProyecto.EditValue.ToString() != "Seleccione..."
                && cmbProyecto.EditValue.ToString() != ""
            )
            {
                GenerateProject();

                if (_executionMode == "Watch")
                {
                    if (_proyectType == "Wasm")
                    {
                        if (_woBlazorWasmExecute.SendToConsoleEvt == null)
                        {
                            _woBlazorWasmExecute.SendToConsoleEvt += SendDataToConsole;
                        }
                        await _woBlazorWasmExecute.StartWatchWasm($@"{_selectedProjectName}Wasm");
                    }
                    else if (_proyectType == "Server")
                    {
                        if (_woBlazorServerExecute.SendToConsoleEvt == null)
                        {
                            _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
                        }
                        await _woBlazorServerExecute.StartWatchServer(
                            $@"{_selectedProjectName}Server"
                        );
                    }

                    btnStop.Enabled = true;
                }
                else if (_executionMode == "DotNet Cli")
                {
                    if (_proyectType == "Wasm")
                    {
                        if (_woBlazorWasmExecute.SendToConsoleEvt == null)
                        {
                            _woBlazorWasmExecute.SendToConsoleEvt += SendDataToConsole;
                        }
                        await _woBlazorWasmExecute.StartSimpleWasm($@"{_selectedProjectName}Wasm");
                    }
                    else if (_proyectType == "Server")
                    {
                        if (_woBlazorServerExecute.SendToConsoleEvt == null)
                        {
                            _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
                        }
                        await _woBlazorServerExecute.StartSimpleServer(
                            $@"{_selectedProjectName}Server"
                        );
                    }

                    btnStop.Enabled = true;
                }
                else if (_executionMode == "Visual Studio")
                {
                    string commandLineArgs =
                        $@"/run {_selectedProjectPath}\{_selectedProjectName}{_proyectType}.csproj";
                    Process processInVisual = woVisualStudio.AbreVisualStudio(
                        commandLineArgs,
                        false
                    );
                }
            }
        }

        #endregion Ejecución de los proyectos

        #region Detener ejecución

        [SupportedOSPlatform("windows")]
        private void btnStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_woBlazorServerExecute.IsRuning() || _woBlazorWasmExecute.IsRuning())
            {
                if (_woBlazorServerExecute.IsRuning())
                {
                    _woBlazorServerExecute.GeneralStop();

                    consoleData.SelectionColor = System.Drawing.Color.Green;
                    consoleData.AppendText($"Proyecto detenido con éxito\n\r");

                    btnStop.Enabled = false;
                }
                if (_woBlazorWasmExecute.IsRuning())
                {
                    _woBlazorWasmExecute.GeneralStop();

                    consoleData.SelectionColor = System.Drawing.Color.Green;
                    consoleData.AppendText($"Proyecto detenido con éxito\n\r");

                    btnStop.Enabled = false;
                }
            }

            if (_woBlazorWasmExecute != null)
            {
                if (_woBlazorWasmExecute.SendToConsoleEvt != null)
                {
                    _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
                    _woBlazorWasmExecute.SendToConsoleEvt = null;
                }
            }

            if (_woBlazorServerExecute != null)
            {
                if (_woBlazorServerExecute.SendToConsoleEvt != null)
                {
                    _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
                    _woBlazorServerExecute.SendToConsoleEvt = null;
                }
            }
        }

        #endregion Detener ejecución


        #region Envió de la data a la consola

        /// <summary>
        /// Bandera que indica si ya fue enviada una alerta de error para la ejecución actual.
        /// </summary>
        private bool _sendError = false;

        /// <summary>
        /// Envío de la data a la consola.
        /// </summary>
        /// <param name="Data"></param>
        [SupportedOSPlatform("windows")]
        private void SendDataToConsole(string Data)
        {
            if (consoleData.IsHandleCreated)
            {
                consoleData.Invoke(
                    new Action(() =>
                    {
                        if (Data.Contains("Building") || Data.Contains("building"))
                        {
                            consoleData.SelectionColor = System.Drawing.Color.LightBlue;
                        }
                        else if (Data.Contains("Warning") || Data.Contains("warning"))
                        {
                            consoleData.SelectionColor = System.Drawing.Color.Yellow;
                        }
                        else if (
                            Data.Contains("Error")
                            || Data.Contains("error")
                            || Data.Contains("Stop")
                        )
                        {
                            consoleData.SelectionColor = System.Drawing.Color.Red;

                            if (!_sendError)
                            {
                                XtraMessageBox.Show(
                                    $"Se produjo un error al intentar ejecutar el proyecto de blazor.\n\r Revise la pestaña de ejecución para mas información",
                                    "Alerta",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                                _sendError = true;
                            }
                        }
                        else if (Data.Contains("Info") || Data.Contains("info"))
                        {
                            consoleData.SelectionColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            consoleData.SelectionColor = System.Drawing.Color.White;
                        }

                        consoleData.AppendText($"{Data}\n\r");
                        consoleData.ScrollToCaret();

                        if (_sendError)
                        {
                            if (_woBlazorServerExecute.SendToConsoleEvt != null)
                            {
                                _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
                                _woBlazorServerExecute.SendToConsoleEvt = null;
                            }
                            if (_woBlazorWasmExecute.SendToConsoleEvt != null)
                            {
                                _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
                                _woBlazorWasmExecute.SendToConsoleEvt = null;
                            }
                        }
                    })
                );
            }
        }

        #endregion Envió de la data a la consola


        #region Tipo del proyecto

        /// <summary>
        /// Tipo del proyecto sobre el que se esta trabajando (Server o Wasm).
        /// </summary>
        private string _proyectType = "Server";

        /// <summary>
        /// Selecciona el tipo de proyecto que se genera, (Server o Wasm).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void cmbAproach_EditValueChanged(object sender, EventArgs e)
        {
            btnStop_ItemClick(null, null);

            _proyectType = cmbAproach.EditValue.ToString();

            WoDirectory.WriteFile(
                path: $@"{proyecto.DirProyectTemp}\IProjectType.txt",
                data: _proyectType
            );

            if (cmbProyecto.EditValue != null)
            {
                if (
                    cmbProyecto.EditValue.ToString() != ""
                    && cmbProyecto.EditValue.ToString() != "Seleccione..."
                )
                {
                    _selectedProjectName = cmbProyecto.EditValue.ToString();
                    _selectedDataProject =
                        $@"{proyecto.DirLayOuts}\IntegralGenerations\{_selectedProjectName}.json";
                    _selectedProjectPath =
                        $@"{proyecto.DirProyectTemp}\{_selectedProjectName}{_proyectType}";

                    ChargeSelectedModels();

                    pnlGrid.Enabled = true;
                }
                else
                {
                    pnlGrid.Enabled = false;
                }
            }
            else
            {
                pnlGrid.Enabled = false;
            }
        }

        #endregion Tipo del proyecto


        #region Modo de ejecución del proyecto

        /// <summary>
        /// Modo de ejecución del proyecto (Watch, DotNet Cli, Visual Studio).
        /// </summary>
        private string _executionMode = "DotNet Cli";

        /// <summary>
        /// Selecciona el modo de ejecución del proyecto (Watch, DotNet Cli, Visual Studio).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void cmbEjecution_EditValueChanged(object sender, EventArgs e)
        {
            btnStop_ItemClick(null, null);

            _executionMode = cmbEjecution.EditValue.ToString();

            WoDirectory.WriteFile(
                path: $@"{proyecto.DirProyectTemp}\IEjecutionType.txt",
                data: _executionMode
            );
        }

        #endregion Modo de ejecución del proyecto

        #region Proyecto seleccionado

        /// <summary>
        /// Indicador del proyecto seleccionado.
        /// </summary>
        private string _selectedProjectName = string.Empty;

        /// <summary>
        /// Path del fichero json del proyecto seleccionado.
        /// </summary>
        private string _selectedDataProject = string.Empty;

        /// <summary>
        /// Path del proyecto seleccionado
        /// </summary>
        private string _selectedProjectPath = string.Empty;

        /// <summary>
        /// Cambio del valor de el valor seleccionado en el grid de formularios ya diseñados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void cmbProyecto_EditValueChanged(object sender, EventArgs e)
        {
            btnStop_ItemClick(null, null);

            consoleData.Text = string.Empty;

            ChargeGridModels();

            if (
                cmbProyecto.EditValue.ToString() != ""
                && cmbProyecto.EditValue.ToString() != "Seleccione..."
            )
            {
                _selectedProjectName = cmbProyecto.EditValue.ToString();
                _selectedDataProject =
                    $@"{proyecto.DirLayOuts}\IntegralGenerations\{_selectedProjectName}.json";
                _selectedProjectPath =
                    $@"{proyecto.DirProyectTemp}\{_selectedProjectName}{_proyectType}";

                ChargeSelectedModels();

                pnlGrid.Enabled = true;
            }
            else
            {
                pnlGrid.Enabled = false;
            }
        }

        #endregion Proyecto seleccionado

        #region Actualización de los modelos

        /// <summary>
        /// Actualiza la grid de los modelo en caso de tener una modificación en alguno de los proyectos integrales.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            cmbProyecto_EditValueChanged(null, null);
        }

        #endregion Actualización de los modelos


        #region Carga de modelos seleccionados

        /// <summary>
        /// Indica que un modelos se debe seleccionar como generable en función del
        /// proyecto seleccionado y su fichero de data
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeSelectedModels()
        {
            List<WoModelSelected> modelsFromJsonCol = JsonConvert.DeserializeObject<
                List<WoModelSelected>
            >(WoDirectory.ReadFile(_selectedDataProject));

            if (modelsFromJsonCol != null && modelsFromJsonCol.Count > 0)
            {
                dynamic data = grdModelsView.DataSource;
                foreach (DataRowView row in data)
                {
                    string proyectType = (string)row[4];
                    eGenerationType type =
                        (proyectType == "Reporte Odata")
                            ? eGenerationType.OdataReport
                            : (proyectType == "Reporte")
                                ? eGenerationType.Report
                                : (proyectType == "Lista")
                                    ? eGenerationType.List
                                    : eGenerationType.FormList;

                    WoModelSelected findModelSelected = modelsFromJsonCol.FirstOrDefault(x =>
                        x.ModelName == (string)row[3] && x.ModelType == type
                    );
                    row[5] = (findModelSelected != null);
                }
            }
        }

        #endregion Carga de modelos seleccionados


        #region Generación de los proyectos

        /// <summary>
        /// Genera el proyecto
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void GenerateProject()
        {
            // Instancia del generador
            WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
            woBlazorGenerator.SendToConsole += SendDataToConsole;

            (
                List<string> forms,
                List<string> reports,
                List<string> lists,
                List<string> oDataReports
            ) lists = GetListModels();

            // Generación de las bases
            if (!File.Exists($@"{_selectedProjectPath}\Program.cs"))
            {
                if (_proyectType == "Wasm")
                {
                    woBlazorGenerator.GenerateBaseWasm($@"{_selectedProjectName}Wasm");
                }
                else if (_proyectType == "Server")
                {
                    woBlazorGenerator.GenerateBaseServer($@"{_selectedProjectName}Server");
                }
            }
            else
            {
                List<string> controlModels = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\ControlModels"
                );
                foreach (string controlModel in controlModels)
                {
                    if (!controlModel.Contains("WoLoginControls.cs"))
                    {
                        WoDirectory.DeleteFile(controlModel);
                    }
                }

                List<string> fluentValidators = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\FluentValidators"
                );
                foreach (string fluentValidator in fluentValidators)
                {
                    if (
                        (!fluentValidator.Contains("InstanciaUdnAsignarValidator.cs"))
                        && (!fluentValidator.Contains("AutenticateValidator.cs"))
                    )
                    {
                        WoDirectory.DeleteFile(fluentValidator);
                    }
                }

                List<string> gridLists = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\GridLists"
                );
                foreach (string gridList in gridLists)
                {
                    WoDirectory.DeleteFile(gridList);
                }

                List<string> pages = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\Pages"
                );
                foreach (string page in pages)
                {
                    if (
                        (!page.Contains("_Host.cshtml"))
                        && (!page.Contains("_Layout.cshtml"))
                        && (!page.Contains("Index.razor"))
                        && (!page.Contains("Index.razor.cs"))
                    )
                    {
                        WoDirectory.DeleteFile(page);
                    }
                }

                List<string> reportForms = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\ReportForms"
                );
                foreach (string reportForm in reportForms)
                {
                    WoDirectory.DeleteFile(reportForm);
                }

                List<string> reports = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\Reports"
                );
                foreach (string report in reports)
                {
                    WoDirectory.DeleteFile(report);
                }

                List<string> slaves = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\Slaves"
                );
                foreach (string slave in slaves)
                {
                    WoDirectory.DeleteFile(slave);
                }

                List<string> transitionSettings = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\TransitionSettings"
                );
                foreach (string transitionSetting in transitionSettings)
                {
                    WoDirectory.DeleteFile(transitionSetting);
                }

                List<string> scriptsUser = WoDirectory.ReadDirectoryFiles(
                    $@"{_selectedProjectPath}\UserCode"
                );
                foreach (string scriptUser in scriptsUser)
                {
                    if (!scriptUser.Contains("LoginScriptsUser.cs"))
                    {
                        WoDirectory.DeleteFile(scriptUser);
                    }
                }

                WoDirectory.DeleteFile($@"{_selectedProjectPath}\Program.cs");
            }

            woBlazorGenerator.GenerateProyectForms(
                projectName: $@"{_selectedProjectName}{_proyectType}",
                selectedModels: lists.forms,
                selectedReports: lists.reports,
                selectedLists: lists.lists,
                selectedODataReports: lists.oDataReports,
                isServer: (_proyectType == "Server")
            );

            woBlazorGenerator.SendToConsole -= SendDataToConsole;
        }

        #endregion Generación de los proyectos

        #region Recuperación de las listas

        /// <summary>
        /// Recupera desde el fichero de la data las listas de los modelos.
        /// </summary>
        /// <returns></returns>
        private (
            List<string> forms,
            List<string> reports,
            List<string> lists,
            List<string> oDataReports
        ) GetListModels()
        {
            List<string> forms = new List<string>();
            List<string> reports = new List<string>();
            List<string> lists = new List<string>();
            List<string> oDataReports = new List<string>();

            List<WoModelSelected> modelsFromJsonCol = JsonConvert.DeserializeObject<
                List<WoModelSelected>
            >(WoDirectory.ReadFile(_selectedDataProject));
            forms = modelsFromJsonCol
                .Where(x => x.ModelType == eGenerationType.FormList)
                .Select(x => x.ModelName)
                .ToList();
            reports = modelsFromJsonCol
                .Where(x => x.ModelType == eGenerationType.Report)
                .Select(x => x.ModelName)
                .ToList();
            lists = modelsFromJsonCol
                .Where(x => x.ModelType == eGenerationType.List)
                .Select(x => x.ModelName)
                .ToList();
            oDataReports = modelsFromJsonCol
                .Where(x => x.ModelType == eGenerationType.OdataReport)
                .Select(x => x.ModelName)
                .ToList();
            return (forms, reports, lists, oDataReports);
        }

        #endregion Recuperación de las listas


        #region Eliminación del blazor

        /// <summary>
        /// Elimina el proyecto de blazor seleccionado y generado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteBlz_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnStop_ItemClick(null, null);

            string selectedProyect = cmbProyecto.EditValue.ToString();

            string pathProyects = $@"{proyecto.DirProyectTemp}\{selectedProyect}";

            if (Directory.Exists($@"{pathProyects}Server") && _proyectType == "Server")
            {
                WoDirectory.DeleteDirectory($@"{pathProyects}Server");

                XtraMessageBox.Show(
                    text: "Proyecto server eliminado con éxito",
                    caption: "Alerta",
                    icon: MessageBoxIcon.Information,
                    buttons: MessageBoxButtons.OK
                );
            }
            else if (!Directory.Exists($@"{pathProyects}Server") && _proyectType == "Server")
            {
                XtraMessageBox.Show(
                    text: $@"Aun no se encuentra una generación en server para el proyecto {selectedProyect}",
                    caption: "Alerta",
                    icon: MessageBoxIcon.Warning,
                    buttons: MessageBoxButtons.OK
                );
            }

            if (Directory.Exists($@"{pathProyects}Wasm") && _proyectType == "Wasm")
            {
                WoDirectory.DeleteDirectory($@"{pathProyects}Wasm");

                XtraMessageBox.Show(
                    text: "Proyecto wasm eliminado con éxito",
                    caption: "Alerta",
                    icon: MessageBoxIcon.Information,
                    buttons: MessageBoxButtons.OK
                );
            }
            else if (!Directory.Exists($@"{pathProyects}Wasm") && _proyectType == "Wasm")
            {
                XtraMessageBox.Show(
                    text: $@"Aun no se encuentra una generación en wasm para el proyecto {selectedProyect}",
                    caption: "Alerta",
                    icon: MessageBoxIcon.Warning,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Eliminación del blazor


        #region Selección y de selección de los modelos

        /// <summary>
        /// Creación del menu contextual para la grid de modelos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdModelsView_PopupMenuShowing(
            object sender,
            DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e
        )
        {
            DevExpress.XtraGrid.Menu.GridViewMenu menu = new DevExpress.XtraGrid.Menu.GridViewMenu(
                grdModelsView
            );

            menu.Items.Add(
                new DevExpress.Utils.Menu.DXMenuItem(
                    "Seleccionar Todo",
                    new EventHandler(
                        delegate(object s, EventArgs args)
                        {
                            SelectAllModels();
                        }
                    )
                )
            );

            menu.Items.Add(
                new DevExpress.Utils.Menu.DXMenuItem(
                    "De seleccionar Todo",
                    new EventHandler(
                        delegate(object s, EventArgs args)
                        {
                            UnSelectAllModels();
                        }
                    )
                )
            );

            menu.Show(e.Point);
        }

        /// <summary>
        /// Selecciona todos los modelos de la grid.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SelectAllModels()
        {
            dynamic data = grdModelsView.DataSource;
            foreach (DataRowView row in data)
            {
                row[5] = true;
            }

            UpdateDataFile(true);
        }

        /// <summary>
        /// De selecciona todos los modelos de la grid.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void UnSelectAllModels()
        {
            dynamic data = grdModelsView.DataSource;
            foreach (DataRowView row in data)
            {
                row[5] = false;
            }

            grdModelsView.RefreshData();
            grdModelsView.UpdateCurrentRow();

            UpdateDataFile(false);
        }

        #endregion Selección y de selección de los modelos


        #region Actualización del fichero de la data

        /// <summary>
        /// Selecciona o de selecciona un modelo de la grid.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void UpdateDataFile(bool newStatus)
        {
            List<WoModelSelected> modelosSeleccionados = new List<WoModelSelected>();

            if (newStatus)
            {
                dynamic data = grdModelsView.DataSource;

                foreach (DataRowView row in data)
                {
                    modelosSeleccionados.Add(GetModelSelected((string)row[3], (string)row[4]));
                }
            }

            if (File.Exists(_selectedDataProject))
            {
                WoDirectory.DeleteFile(_selectedDataProject);
            }

            WoDirectory.WriteFile(
                path: _selectedDataProject,
                data: JsonConvert.SerializeObject(modelosSeleccionados)
            );
        }

        #endregion Actualización del fichero de la data

        #region Selección de los modelos

        /// <summary>
        /// Validación de los modelos seleccionador para cuando se modifican los modelos seleccionado en la grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdModelsView_CellValueChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e
        )
        {
            if (e != null && e.RowHandle >= 0)
            {
                List<WoModelSelected> modelosSeleccionados = new List<WoModelSelected>();

                dynamic data = grdModelsView.DataSource;
                DataRowView rowChanged = (DataRowView)grdModelsView.GetRow((int)e.RowHandle);
                foreach (DataRowView row in data)
                {
                    if (
                        (string)rowChanged[4] == (string)row[4]
                        && (string)rowChanged[3] == (string)row[3]
                    )
                    {
                        if ((bool)e.Value)
                        {
                            modelosSeleccionados.Add(
                                GetModelSelected((string)row[3], (string)row[4])
                            );
                        }
                    }
                    else
                    {
                        if ((bool)row[5])
                        {
                            modelosSeleccionados.Add(
                                GetModelSelected((string)row[3], (string)row[4])
                            );
                        }
                    }
                }

                if (File.Exists(_selectedDataProject))
                {
                    WoDirectory.DeleteFile(_selectedDataProject);
                }

                WoDirectory.WriteFile(
                    path: _selectedDataProject,
                    data: JsonConvert.SerializeObject(modelosSeleccionados)
                );
            }
        }

        /// <summary>
        /// Recupera la instancia del modelo seleccionado con la información de ficheros y demás para el modelo.
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private WoModelSelected GetModelSelected(string modelName, string type)
        {
            WoModelSelected modelSelected = new WoModelSelected();
            modelSelected.ModelName = modelName;

            Modelo findModel = proyecto
                .ModeloCol.Modelos.Where(x => x.Id == modelName)
                .FirstOrDefault();

            if (type == "Formulario y lista")
            {
                modelSelected.ModelType = eGenerationType.FormList;
            }
            else if (type == "Lista")
            {
                modelSelected.ModelType = eGenerationType.List;
            }
            else if (type == "Reporte")
            {
                modelSelected.ModelType = eGenerationType.Report;
            }
            else if (type == "Reporte Odata")
            {
                modelSelected.ModelType = eGenerationType.OdataReport;
            }

            return modelSelected;
        }

        #endregion Selección de los modelos


        #region Formulario

        /// <summary>
        /// Cierra el formulario
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void FormClosed()
        {
            btnStop_ItemClick(null, null);
        }

        public void Refrescar()
        {
            throw new NotImplementedException();
        }

        #endregion Formulario


        #region Publicación de la aplicación

        /// <summary>
        /// Se ocupa de disparar el método para iniciar la publicación de la aplicación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeploy_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Directory.Exists($@"{proyecto.Dir}\Deploy"))
            {
                WoDirectory.DeleteDirectory($@"{proyecto.Dir}\Deploy");
            }

            if (
                cmbProyecto.EditValue.ToString() == "Seleccione..."
                || cmbProyecto.EditValue.ToString() == ""
            )
            {
                XtraMessageBox.Show(
                    text: "Debe seleccionar un proyecto para poder publicar la aplicación.",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Warning
                );
                return;
            }

            if (
                cmbDockerDeploy.EditValue.ToString() == "Seleccione..."
                || cmbDockerDeploy.EditValue.ToString() == ""
            )
            {
                XtraMessageBox.Show(
                    text: "Debe seleccionar un docker donde desplegar",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                btnStop_ItemClick(null, null);

                WoDeployGenerationSettings woDeployGenerationSettings =
                    new WoDeployGenerationSettings();
                woDeployGenerationSettings.MetadataName = "Proyecto";
                woDeployGenerationSettings.MetadataFileName = "Prueba";
                woDeployGenerationSettings.Docker = cmbDockerDeploy.EditValue.ToString();
                woDeployGenerationSettings.IntegralDataName = cmbProyecto.EditValue.ToString();
                if (_proyectType == "Server")
                {
                    woDeployGenerationSettings.ProyectType = eProyectType.Server;
                }
                else if (_proyectType == "Wasm")
                {
                    woDeployGenerationSettings.ProyectType = eProyectType.Wasm;
                }

                WoDeployMetaData woDeployMetaData = new WoDeployMetaData();
                woDeployMetaData.DeployMetaData(woDeployGenerationSettings);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $"Se produjo un error al intentar publicar la aplicación.\n\r{ex.Message}",
                    caption: "Error",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }

            if (Directory.Exists($@"{proyecto.Dir}\Deploy"))
            {
                WoDirectory.DeleteDirectory($@"{proyecto.Dir}\Deploy");
            }
        }

        #endregion Publicación de la aplicación


        #region Consulta de dockers

        /// <summary>
        /// Actualiza la lista de los contenedores de docker en la lista.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnUpdateDockers_ItemClick(object sender, ItemClickEventArgs e)
        {
            WoDockerManager woDockerManager = new WoDockerManager();
            List<WoDockerSettings> dockers = woDockerManager.GetDockers();

            if (dockers != null && dockers.Count > 0)
            {
                grdDocker.DataSource = null;
                grdDocker.Refresh();
                grdDocker.DataSource = dockers;
                ChargeDockersCombo(dockers);
                grdDocker.Refresh();

                grdDockerView.Columns[@"DockerName"].OptionsColumn.AllowEdit = false;
                grdDockerView.Columns[@"ImageName"].OptionsColumn.AllowEdit = false;
                grdDockerView.Columns[@"NetworkName"].OptionsColumn.AllowEdit = false;
                grdDockerView.Columns[@"Size"].OptionsColumn.AllowEdit = false;
                grdDockerView.Columns[@"Port"].OptionsColumn.AllowEdit = false;
                grdDockerView.Columns[@"CPU"].OptionsColumn.AllowEdit = false;
                grdDockerView.Columns[@"RAM"].OptionsColumn.AllowEdit = false;
                grdDockerView.Columns[@"EnLinea"].OptionsColumn.AllowEdit = false;
            }
        }

        /// <summary>
        /// Carga la lista de los dockers en el combo en el ribbon.
        /// </summary>
        /// <param name="dockers"></param>
        /// <param name="selectedDocker"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeDockersCombo(
            List<WoDockerSettings> dockers,
            string selectedDocker = "Seleccione..."
        )
        {
            List<string> dockersCol = new List<string>();

            dockersCol.Add("Seleccione...");

            foreach (WoDockerSettings docker in dockers)
            {
                if (!dockersCol.Contains(docker.DockerName))
                {
                    dockersCol.Add(docker.DockerName);
                }
            }

            RepositoryItemComboBox comboBox = cmbDockerDeploy.Edit as RepositoryItemComboBox;

            if (comboBox != null)
            {
                // Agrega un nuevo item a la lista desplegable
                comboBox.Items.Clear();
                comboBox.Items.AddRange(dockersCol);
                cmbDockerDeploy.EditValue = selectedDocker;
            }
        }

        #endregion Consulta de dockers


        #region Creación de un nuevo docker

        /// <summary>
        /// Indicador de que se esta creando un docker nuevo.
        /// </summary>
        private bool _newDocker = false;

        /// <summary>
        /// Pone el panel dos del split del docker para que el usuario configure su docker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnNewDocker_ItemClick(object sender, ItemClickEventArgs e)
        {
            _newDocker = true;

            sccDocker.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
            xtraTabControl1.SelectedTabPage = TabDockerManager;

            WoDockerSettings woDockerSettings = new WoDockerSettings()
            {
                DockerName = "Docker1",
                ImageName = "NA",
                NetworkName = "woow",
                Size = 10,
                Port = 9095,
                CPU = 1,
                RAM = 4,
                EnLinea = false
            };

            propDocker.SelectedObject = woDockerSettings;
        }

        /// <summary>
        /// Guarda la configuración del docker en la bd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDockerSave_Click(object sender, EventArgs e)
        {
            if (_newDocker)
            {
                CreateNewDocker();
            }
            else
            {
                SendEditDocker();
            }

            sccDocker.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
            btnUpdateDockers_ItemClick(null, null);
        }

        /// <summary>
        /// Creación del nuevo docker
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateNewDocker()
        {
            WoDockerSettings woDockerSettings = (WoDockerSettings)propDocker.SelectedObject;

            WoDockerManager woDockerManager = new WoDockerManager();
            woDockerManager.CreateDocker(woDockerSettings);
        }

        #endregion Creación de un nuevo docker

        #region Edición de los dockers

        /// <summary>
        /// Abre el menu con opciones del click derecho en la grid de dockers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void GrdDockerView_PopupMenuShowing(
            object sender,
            DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e
        )
        {
            DevExpress.XtraGrid.Menu.GridViewMenu menu = new DevExpress.XtraGrid.Menu.GridViewMenu(
                grdModelsView
            );

            menu.Items.Add(
                new DevExpress.Utils.Menu.DXMenuItem(
                    "Editar",
                    new EventHandler(
                        delegate(object s, EventArgs args)
                        {
                            EditDocker();
                        }
                    )
                )
            );

            menu.Show(e.Point);
        }

        /// <summary>
        /// Abre la grid de edición de los dockers.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void EditDocker()
        {
            _newDocker = false;

            sccDocker.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
            int rowHangle = grdDockerView.FocusedRowHandle;

            object dataRow = grdDockerView.GetFocusedRow();

            propDocker.SelectedObject = dataRow;
        }

        private void SendEditDocker() { }

        #endregion Edición de los dockers

        private void pnlDockerManager_Paint(object sender, PaintEventArgs e) { }
    }
}
