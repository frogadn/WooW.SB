using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorDialogs.BlazorDialogsModels;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorDialogs
{
    public partial class WoModelSelector : DevExpress.XtraEditors.XtraForm
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

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


        #region Constructor

        /// <summary>
        /// Constructor principal del formulario.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public WoModelSelector()
        {
            InitializeComponent();

            if (File.Exists($@"{_project.DirProyectTemp}\IProjectType.txt"))
            {
                string projectType = WoDirectory.ReadFile(
                    $@"{_project.DirProyectTemp}\IProjectType.txt"
                );
                cmbAproach.SelectedItem = projectType;
                string ejecutionType = WoDirectory.ReadFile(
                    $@"{_project.DirProyectTemp}\IEjecutionType.txt"
                );
                cmbEjecution.SelectedItem = ejecutionType;
            }
            else
            {
                cmbAproach.SelectedIndex = 0;
                cmbEjecution.SelectedIndex = 1;
            }

            pnlGrid.Enabled = false;

            ChargeProyects();

            ChargeGridModels();
        }

        #endregion Constructor


        #region Cargar proyectos

        /// <summary>
        /// Carga los proyectos de la carpeta temporal.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeProyects(string selectedProyect = "Seleccione...")
        {
            List<string> projectsCol = WoDirectory.ReadDirectoryFiles(
                $@"{_project.DirLayOuts}\IntegralGenerations",
                onlyNames: true
            );

            projectsCol.Add("Seleccione...");

            if (!projectsCol.Contains(selectedProyect))
            {
                projectsCol.Add(selectedProyect);
            }

            cmbProyecto.Properties.Items.Clear();
            cmbProyecto.Properties.Items.AddRange(projectsCol);
            cmbProyecto.SelectedItem = selectedProyect;
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
            List<string> forms = WoDirectory.ReadDirectoryFiles(
                path: $@"{_project.DirLayOuts}\FormDesign",
                onlyNames: true
            );

            foreach (string form in forms)
            {
                Modelo model = SearchModelDet(form);
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
                path: $@"{_project.DirLayOuts}\Reports\Vistas"
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
            List<string> forms = WoDirectory.ReadDirectoryFiles(
                path: $@"{_project.DirLayOuts}\ListDesign",
                onlyNames: true
            );

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


        #region Busqueda del modelo

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
                    _findModel = _project.ModeloCol.Modelos.Where(x => x.Id == modelName).First();
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
                    text: $@"El modelo {modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio.",
                    caption: "Error",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }

            return _findModel;
        }

        #endregion Busqueda del modelo


        #region Creacion y eliminacion completa de los proyectos

        /// <summary>
        /// Crea el fichero con las settings del proyecto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnNewProyect_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Elimina el fichero con las settings del proyecto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteProyect_Click(object sender, EventArgs e)
        {
            btnStop_Click(null, null);

            string selectedProyect = cmbProyecto.Text;
            string pathFileSelectedProyect =
                $@"{_project.DirLayOuts}\IntegralGenerations\{selectedProyect}.json";

            if (File.Exists(pathFileSelectedProyect))
            {
                WoDirectory.DeleteFile(pathFileSelectedProyect);
            }

            string pathProyects = $@"{_project.DirProyectTemp}\{selectedProyect}";

            if (Directory.Exists($@"{pathProyects}Server"))
            {
                WoDirectory.DeleteDirectory($@"{pathProyects}Server");
            }

            if (Directory.Exists($@"{pathProyects}Wasm"))
            {
                WoDirectory.DeleteDirectory($@"{pathProyects}Wasm");
            }

            cmbProyecto.SelectedItem = "Seleccione...";

            ChargeProyects();

            dynamic data = grdModelsView.DataSource;
            foreach (DataRowView row in data)
            {
                row[5] = false;
            }
        }

        #endregion Creacion y eliminacion completa de los proyectos


        #region Atributos de ejecucion

        /// <summary>
        /// Instancia que permite la ejecución del proyecto server de blazor.
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Instancia que permite la ejecución del proyecto wasm de blazor.
        /// </summary>
        private WoBlazorWasmExecute _woBlazorWasmExecute = WoBlazorWasmExecute.GetInstance();

        #endregion Atributos de ejecucion

        #region Ejecucion del proyecto

        /// <summary>
        /// Ejecuta el proyecto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private async void btnEjecute_Click(object sender, EventArgs e)
        {
            btnStop_Click(null, null);

            consoleData.Text = string.Empty;
            _sendError = false;

            if (cmbProyecto.Text != "Seleccione..." && cmbProyecto.Text != "")
            {
                GenerateProject();

                if (_executionMode == "Watch")
                {
                    if (_proyectType == "Wasm")
                    {
                        _woBlazorWasmExecute.SendToConsoleEvt += SendDataToConsole;
                        await _woBlazorWasmExecute.StartWatchWasm($@"{_selectedProjectName}Wasm");
                    }
                    else if (_proyectType == "Server")
                    {
                        _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
                        await _woBlazorServerExecute.StartWatchServer(
                            $@"{_selectedProjectName}Server"
                        );
                    }
                }
                else if (_executionMode == "DotNet Cli")
                {
                    if (_proyectType == "Wasm")
                    {
                        _woBlazorWasmExecute.SendToConsoleEvt += SendDataToConsole;
                        await _woBlazorWasmExecute.StartSimpleWasm($@"{_selectedProjectName}Wasm");
                    }
                    else if (_proyectType == "Server")
                    {
                        _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
                        await _woBlazorServerExecute.StartSimpleServer(
                            $@"{_selectedProjectName}Server"
                        );
                    }
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

        #endregion Ejecucion del proyecto

        #region Detener ejecucion

        /// <summary>
        /// Detiene la ejecucion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_woBlazorServerExecute.IsRuning() || _woBlazorWasmExecute.IsRuning())
            {
                if (_woBlazorServerExecute.IsRuning())
                {
                    _woBlazorServerExecute.GeneralStop();

                    consoleData.SelectionColor = Color.Green;
                    consoleData.AppendText($"Proyecto detenido con exito\n\r");
                }
                if (_woBlazorWasmExecute.IsRuning())
                {
                    _woBlazorWasmExecute.GeneralStop();

                    consoleData.SelectionColor = Color.Green;
                    consoleData.AppendText($"Proyecto detenido con exito\n\r");
                }
            }

            if (_woBlazorWasmExecute != null)
            {
                _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
            }

            if (_woBlazorServerExecute != null)
            {
                _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
            }
        }

        #endregion Detener ejecucion

        #region Envio de la data a la consola

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
                            consoleData.SelectionColor = Color.LightBlue;
                        }
                        else if (Data.Contains("Warning") || Data.Contains("warning"))
                        {
                            consoleData.SelectionColor = Color.Yellow;
                        }
                        else if (
                            Data.Contains("Error")
                            || Data.Contains("error")
                            || Data.Contains("Stop")
                        )
                        {
                            consoleData.SelectionColor = Color.Red;

                            if (!_sendError)
                            {
                                XtraMessageBox.Show(
                                    $"Se produjo un error al intentar ejecutar el proyecto de blazor.\n\r Revise la pestaña de ejecuión para mas información",
                                    "Alerta",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                                _sendError = true;
                            }
                        }
                        else if (Data.Contains("Info") || Data.Contains("info"))
                        {
                            consoleData.SelectionColor = Color.Green;
                        }
                        else
                        {
                            consoleData.SelectionColor = Color.White;
                        }

                        consoleData.AppendText($"{Data}\n\r");
                        consoleData.ScrollToCaret();
                    })
                );
            }
        }

        #endregion Envio de la data a la consola


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
        private void cmbAproach_SelectedValueChanged(object sender, EventArgs e)
        {
            btnStop_Click(null, null);

            _proyectType = cmbAproach.Text;

            WoDirectory.WriteFile(
                path: $@"{_project.DirProyectTemp}\IProjectType.txt",
                data: _proyectType
            );

            if (cmbProyecto.Text != "" && cmbProyecto.Text != "Seleccione...")
            {
                _selectedProjectName = cmbProyecto.Text;
                _selectedDataProject =
                    $@"{_project.DirLayOuts}\IntegralGenerations\{_selectedProjectName}.json";
                _selectedProjectPath =
                    $@"{_project.DirProyectTemp}\{_selectedProjectName}{_proyectType}";

                ChargeSelectedModels();

                pnlGrid.Enabled = true;
            }
            else
            {
                pnlGrid.Enabled = false;
            }
        }

        #endregion Tipo del proyecto

        #region Modo de ejecucion del proyecto

        /// <summary>
        /// Modo de ejecucion del proyecto (Watch, DotNet Cli, Visual Studio).
        /// </summary>
        private string _executionMode = "DotNet Cli";

        /// <summary>
        /// Selecciona el modo de ejecucion del proyecto (Watch, DotNet Cli, Visual Studio).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void cmbEjecution_SelectedValueChanged(object sender, EventArgs e)
        {
            btnStop_Click(null, null);

            _executionMode = cmbEjecution.Text;

            WoDirectory.WriteFile(
                path: $@"{_project.DirProyectTemp}\IEjecutionType.txt",
                data: _executionMode
            );
        }

        #endregion Modo de ejecucion del proyecto

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
        private void cmbProyecto_SelectedValueChanged(object sender, EventArgs e)
        {
            btnStop_Click(null, null);

            consoleData.Text = string.Empty;

            if (cmbProyecto.Text != "" && cmbProyecto.Text != "Seleccione...")
            {
                _selectedProjectName = cmbProyecto.Text;
                _selectedDataProject =
                    $@"{_project.DirLayOuts}\IntegralGenerations\{_selectedProjectName}.json";
                _selectedProjectPath =
                    $@"{_project.DirProyectTemp}\{_selectedProjectName}{_proyectType}";

                ChargeSelectedModels();

                pnlGrid.Enabled = true;
            }
            else
            {
                pnlGrid.Enabled = false;
            }
        }

        #endregion Proyecto seleccionado

        #region Carga de modelos seleccionados

        /// <summary>
        /// Indica que un modelos se deve seleccionar como generable en funcion del
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


        #region Generacion de los proyectos

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

            // Generacion de las bases
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
        }

        #endregion Generacion de los proyectos

        #region Recuperacion de las listas

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

        #endregion Recuperacion de las listas


        #region Seleccion y deseleccion de los modelos

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
                    "Deseleccionar Todo",
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
        /// Deselecciona todos los modelos de la grid.
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

        /// <summary>
        /// Selecciona o deselecciona un modelo de la grid.
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

        #endregion Seleccion y deseleccion de los modelos

        #region Seleccion de los modelos

        /// <summary>
        /// Validacion de los modelos seleccionador para cuando se modifican los modelos seleccionado en la grid.
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
        /// Recupera la instancia del modelo seleccionado con la informacion de ficheros y demas para el modelo.
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private WoModelSelected GetModelSelected(string modelName, string type)
        {
            WoModelSelected modelSelected = new WoModelSelected();
            modelSelected.ModelName = modelName;

            Modelo findModel = _project
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

        #endregion Seleccion de los modelos


        #region Formulario

        /// <summary>
        /// Cierra el blazor al cerrar el formulario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void WoModelSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            btnStop_Click(null, null);
        }

        #endregion Formulario

        #region Alertas

        private WoLog _cantFindModel = new WoLog()
        {
            CodeLog = "000",
            Title = $@"El modelo no se encuentra el proyecto.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = "ChargeModel"
            }
        };

        #endregion Alertas
    }
}
