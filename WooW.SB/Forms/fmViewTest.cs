using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json;
using WooW.Core;
using WooW.SB.BlazorGenerator;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.BlazorTestGenerator.Components;
using WooW.SB.BlazorTestGenerator.Models;
using WooW.SB.BlazorTestGenerator.TestTools;
using WooW.SB.CodeEditor.CodeComponents;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Forms
{
    public partial class fmViewTest : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        public Proyecto proyecto { get; set; }

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Nombre del proyecto de server generado
        /// </summary>
        private string _serverProjectName = "ServerUnitModel_proj";

        /// <summary>
        /// Nombre base de las clases que contendrán las pruebas unitarias en caso de ser para server.
        /// </summary>
        private string _serverClassModelName = "ServerUnitModel";

        /// <summary>
        /// Nombre del proyecto de wasm generado
        /// </summary>
        private string _wasmProjectName = "WasmUnitModel_proj";

        /// <summary>
        /// Nombre base de las clases que contendrán las pruebas unitarias en caso de ser para wasm.
        /// </summary>
        private string _wasmClassModelName = "WasmUnitModel";

        #endregion Atributos

        #region Constructor y métodos base del formulario

        /// <summary>
        /// Constructor principal del formulario
        /// </summary>
        public fmViewTest()
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


        #region Cargar Formulario

        /// <summary>
        /// Carga principal del formulario
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void Cargar()
        {
            try
            {
                // Pantalla
                ChargeSettings();
                sccSyntaxTest.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;

                // Funcionamiento base
                CheckNode();
                CheckLibrary();

                // Modelos
                ChargeModels();

                txtTestSpeed.EditValue = 0.5;
                BuildTestingProperties();

                // Integrales
                ChargeMainFormIntegral();
                ChargeIntegralTests();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error en el método principal de cargar {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Cargar Formulario

        #region Formulario

        public void Refrescar()
        {
            throw new NotImplementedException();
        }

        #endregion Formulario

        #region Cambio de tabs

        /// <summary>
        /// Modifica el ribbon en función del tab seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void baseTabs_SelectedPageChanged(
            object sender,
            DevExpress.XtraTab.TabPageChangedEventArgs e
        )
        {
            try
            {
                if (e.Page.Name == "tabUnitarias")
                {
                    rpgIntegrales.Visible = false;
                    rpgUnitarias.Visible = true;
                }
                else if (e.Page.Name == "tabIntegrales")
                {
                    rpgIntegrales.Visible = true;
                    rpgUnitarias.Visible = false;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al actualizar el ribbon {ex.Message}",
                    caption: "Error",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }
        }

        #endregion Cambio de tabs

        #region Build TestingProperties

        /// <summary>
        /// Construye el json con las propiedades para el apartado de pruebas
        /// </summary>
        private void BuildTestingProperties()
        {
            try
            {
                string propertiesPath =
                    $@"{proyecto.DirProyectData}\Test\TestCafe\TestingProperties.json";

                if (File.Exists(propertiesPath))
                {
                    File.Delete(propertiesPath);
                }

                WoTestingProperties woTestingProperties = new WoTestingProperties();
                woTestingProperties.User = proyecto.ParConexion.Usuario;
                woTestingProperties.Password = proyecto.ParConexion.Password;
                woTestingProperties.Instance = proyecto.ParConexion.Instance;
                woTestingProperties.Udn = proyecto.ParConexion.Udn;
                woTestingProperties.Year = proyecto.ParConexion.Year;
                woTestingProperties.InstanceType = proyecto.ParConexion.InstanceType.ToString();
                woTestingProperties.BasePath = proyecto.Dir;
                woTestingProperties.SpeedTest = _testSpeed;

                WoDirectory.WriteFile(
                    path: propertiesPath,
                    data: JsonConvert.SerializeObject(woTestingProperties)
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al construir el json de propiedades para pruebas. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Variable que define la velocidad a la que se ejecutaran las pruebas
        /// </summary>
        private double _testSpeed = 0.5;

        /// <summary>
        /// Evento para cuando se modifica el valor de la velocidad;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void txtTestSpeed_EditValueChanged(dynamic sender, EventArgs e)
        {
            var newValue = sender.EditValue;
            _testSpeed = 0.5;

            try
            {
                _testSpeed = Convert.ToDouble(newValue);

                if (_testSpeed >= 0 && _testSpeed <= 1)
                {
                    BuildTestingProperties();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                _testSpeed = 0.5;
                txtTestSpeed.EditValue = 0.5;

                XtraMessageBox.Show(
                    $@"El valor de la velocidad solo puede ser un decimal entre 0 y 1",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Build TestingProperties


        #region Carga de las configuraciones de la ejecución

        /// <summary>
        /// Cargamos las configuraciones de ejecución para los test
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeSettings()
        {
            try
            {
                DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbAproachEdit =
                    cmbAproach.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                cmbAproachEdit.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;
                cmbAproachEdit.Items.Add("Server");
                cmbAproachEdit.Items.Add("Wasm");

                cmbAproach.EditValue = "Server";

                rpgIntegrales.Visible = false;
                rpgUnitarias.Visible = true;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al cargar las configuraciones de ejecución de los test. {ex.Message}"
                );
            }
        }

        #endregion Carga de las configuraciones de la ejecución

        #region Validación de las herramientas

        /// <summary>
        /// Comprueba que se encuentre una version instalada de node que sea compatible con TestCafe
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CheckNode()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process nodeVersionProcess = new Process();

                // Settings del app que genera
                nodeVersionProcess.EnableRaisingEvents = false;
                nodeVersionProcess.StartInfo.UseShellExecute = false;
                nodeVersionProcess.StartInfo.FileName = "cmd";

                // Settings de la salida de la consola
                nodeVersionProcess.StartInfo.RedirectStandardOutput = true;
                nodeVersionProcess.StartInfo.RedirectStandardError = true;
                nodeVersionProcess.StartInfo.CreateNoWindow = true;
                nodeVersionProcess.StartInfo.UseShellExecute = false;

                // Parámetros con las configuraciones para iniciar el docker
                StringBuilder strDockerSettings = new StringBuilder();

                strDockerSettings.Append($@"/c ");
                strDockerSettings.Append($@"Node -v");

                // Argumentos para el generador
                nodeVersionProcess.StartInfo.Arguments = strDockerSettings.ToString();

                // Arrancamos el proceso
                nodeVersionProcess.Start();

                // Recuperamos la salida del generador
                string output = nodeVersionProcess.StandardOutput.ReadToEnd();

                // Recuperamos la salida de error
                string error = nodeVersionProcess.StandardError.ReadToEnd();

                // Esperamos que termine la salida del generador
                nodeVersionProcess.WaitForExit();

                // Validamos que venga el identificador del contenedor en la salida
                if (output.IsDBNullOrStringEmpty())
                {
                    throw new Exception(
                        $"Error verificar la version de node \nInstale una versión node 20 o superior"
                    );
                }
                else
                {
                    // Verificamos el resultado en función de la salida para comprobar que la versión de node sea superior
                    // o igual a la versión 20
                    string[] outCol = output.Split('.');
                    int principalVercion = int.Parse(outCol.First().Replace("v", ""));
                    if (principalVercion < 20)
                    {
                        throw new Exception(
                            "Instale al menos una versión de node 20 o superior para utilizar el modulo de pruebas"
                        );
                    }
                }

                // En caso de que el proceso arroje un error enviamos una exception con el error
                if (error != string.Empty)
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al ejecutar la verificación de docker \nInstale una versión node 20 o superior"
                );
            }
        }

        #endregion Validación de las herramientas

        #region Validación de la librería

        /// <summary>
        /// Path de la librería de WOOW para TestCafe
        /// </summary>
        private string _pathLibrary = string.Empty;

        /// <summary>
        /// Comprueba que se encuentre una version de la Liberia de woow para test café
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CheckLibrary()
        {
            try
            {
                _pathLibrary = $@"{proyecto.DirLayOuts}\Test\Libreria\WoTestCafe";
                if (!File.Exists($@"{_pathLibrary}\Tools\Settings\Login\WoLogin.js"))
                {
                    throw new Exception(
                        $@"No se encontró una versión de la librería de Woow para TestCafe"
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al ejecutar la verificación de la librería \nVerifique tener instalada la librería js de WOOW para TestCafe"
                );
            }
        }

        #endregion Validación de la librería


        #region Carga de las pruebas a la grid

        /// <summary>
        /// Data table que funciona como data del grid
        /// </summary>
        private DataTable _testModelCol = new DataTable();

        /// <summary>
        /// Cargamos la grid de pruebas con las pruebas que ya se tienen en la carpeta del modelo
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void ChargeTestGrid()
        {
            try
            {
                string testPath = $@"{proyecto.DirLayOuts}\Test\Unitarias\{_model.Id}";
                if (!Directory.Exists(testPath))
                {
                    XtraMessageBox.Show(
                        text: $@"No hay Pruebas para el modelo {_model.Id}",
                        caption: "Info",
                        buttons: MessageBoxButtons.OK,
                        icon: MessageBoxIcon.Information
                    );

                    _testModelCol.Clear();

                    return;
                }

                List<string> modelTestCol = WoDirectory.ReadDirectoryFiles(
                    testPath,
                    onlyNames: true
                );

                if (modelTestCol != null)
                {
                    _testModelCol = new DataTable();

                    _testModelCol.Columns.Add($@"Prueba", typeof(string));
                    _testModelCol.Columns.Add($@"Tipo", typeof(string));
                    _testModelCol.Columns.Add($@"Descripción", typeof(string));
                    _testModelCol.Columns.Add($@"Completada", typeof(bool));

                    foreach (string modelTest in modelTestCol)
                    {
                        string rawTest = WoDirectory.ReadFile(
                            $@"{proyecto.DirLayOuts}\Test\Unitarias\{_model.Id}\{modelTest}.js"
                        );
                        string[] rawTestCol = rawTest.Split("///");
                        string prueba = rawTestCol[2].Split("Prueba:").Last().Replace(" ", "");
                        string descripcion = rawTestCol[3]
                            .Split("Descripción:")
                            .Last()
                            .Replace(" ", "");
                        string tipo = rawTestCol[4].Split("Tipo:").Last().Replace(" ", "");

                        DataRow drRow = _testModelCol.NewRow();

                        drRow[$@"Prueba"] = modelTest;
                        drRow[$@"Tipo"] = tipo;
                        drRow[$@"Descripción"] = descripcion;
                        drRow[$@"Completada"] = false;

                        _testModelCol.Rows.Add(drRow);
                    }

                    grdTest.DataSource = _testModelCol;

                    grdTestView.Columns[@"Prueba"].OptionsColumn.AllowEdit = false;
                    grdTestView.Columns[@"Tipo"].OptionsColumn.AllowEdit = false;
                    grdTestView.Columns[@"Descripción"].OptionsColumn.AllowEdit = false;
                    grdTestView.Columns[@"Completada"].OptionsColumn.AllowEdit = false;
                }
                else
                {
                    XtraMessageBox.Show(
                        text: $@"No hay Pruebas para el modelo {_model.Id}",
                        caption: "Info",
                        buttons: MessageBoxButtons.OK,
                        icon: MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al cargar la grid de las pruebas. {ex.Message}"
                );
            }
        }

        #endregion Carga de las pruebas a la grid

        #region Selección de las pruebas en la grid

        /// <summary>
        /// Instancia del editor de código para la modification de pruebas
        /// </summary>
        private WoBaseEditor _woBaseEditor;

        /// <summary>
        /// Actualiza el editor de código en función de la prueba seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdTestView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            try
            {
                sccSyntaxTest.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;

                dynamic focusedRow = grdTestView.GetFocusedRow();
                if (focusedRow == null)
                    return;

                string test = focusedRow.Row.ItemArray[0];
                string testPath = $@"{proyecto.DirLayOuts}\Test\Unitarias\{_modelName}\{test}.js";

                string rawCode = WoDirectory.ReadFile(testPath);

                //if (_woBaseEditor != null)
                //{
                //    _woBaseEditor.Dispose();
                //}

                //_woBaseEditor = new WoBaseEditor(language: new JavaScriptSyntaxLanguage());
                //pnlSyntaxEditor.Controls.Add(_woBaseEditor);

                //_woBaseEditor.SetCode(rawCode);

                //sccSyntaxTest.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al recuperar la información del test seleccionado. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Selección de las pruebas en la grid


        #region Selección de un modelo de la grid

        /// <summary>
        /// Modelo seleccionado
        /// </summary>
        private Modelo _model = null;

        /// <summary>
        /// Nombre del modelo que se tiene seleccionado
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Actualiza la vista y las pruebas en función del modelo seleccionado
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void grdModelsView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            try
            {
                // Si hay algo corriendo al cambiar de modelo paramos todo
                _woBlazorWasmExecute.GeneralStop();
                _woBlazorServerExecute.GeneralStop();

                btnBlazorExecute.Enabled = true;
                btnStopBlazor.Enabled = false;

                // Iniciamos el cambio de modelo
                dynamic focusedRow = grdModelsView.GetFocusedRow();
                if (focusedRow == null)
                    return;

                _modelName = focusedRow.Row.ItemArray[3];
                _model = proyecto.ModeloCol.Modelos.Where(m => m.Id == _modelName).First();

                SaveUnitProjects();

                BuildModelClass();

                ChargeTestGrid();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al recuperar la información del modelo seleccionado. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Selección de un modelo de la grid

        #region Actualizar unitaria

        /// <summary>
        /// Actualiza los formularios de ambos approach de unitarias
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SaveUnitProjects()
        {
            string serverPath =
                $@"{proyecto.DirProyectTemp}\{_serverProjectName}\{_serverProjectName}.csproj";
            string wasmPath =
                $@"{proyecto.DirProyectTemp}\{_wasmProjectName}\{_wasmProjectName}.csproj";

            WoContainer woContainer = new WoContainer();
            if (
                _model.TipoModelo == WoTypeModel.Kardex
                || _model.TipoModelo == WoTypeModel.Control
                || _model.TipoModelo == WoTypeModel.Configuration
                || _model.TipoModelo == WoTypeModel.View
            )
            {
                WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
                woContainer = woDesignerRawListHelper.BuildRawListForm(_modelName);
            }
            else
            {
                string pathLayout = $@"{proyecto.DirLayOuts}\FormDesign\{_modelName}.json";
                string json = WoDirectory.ReadFile(pathLayout);

                woContainer = JsonConvert.DeserializeObject<WoContainer>(json);
            }

            WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();

            if (File.Exists(serverPath))
            {
                woBlazorGenerator.GenerateUnitLayout(
                    isServer: true,
                    container: woContainer,
                    classModelName: _serverClassModelName
                );
            }

            if (File.Exists(wasmPath))
            {
                woBlazorGenerator.GenerateUnitLayout(
                    isServer: false,
                    container: woContainer,
                    classModelName: _wasmClassModelName
                );
            }
        }

        #endregion Actualizar unitaria


        #region Carga de los modelos a la grid

        /// <summary>
        /// Carga la grid lateral con la información base de los modelos recuperada desde
        /// la instancia de proyecto
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeModels()
        {
            DataTable modelosCol = new DataTable();

            modelosCol.Columns.Add(@"Proceso", typeof(string));
            modelosCol.Columns.Add(@"Tipo", typeof(string));
            modelosCol.Columns.Add(@"Repositorio", typeof(string));
            modelosCol.Columns.Add(@"Modelo", typeof(string));
            modelosCol.Columns.Add(@"Probado", typeof(bool));

            foreach (var model in proyecto.ModeloCol.Modelos)
            {
                string pathDesign = $@"{proyecto.DirLayOuts}\FormDesign\{model.Id}.json";

                if (
                    (
                        !(
                            model.TipoModelo == WoTypeModel.Request
                            && model.SubTipoModelo != WoSubTypeModel.Report
                        )
                    )
                    && model.TipoModelo != WoTypeModel.Response
                    && model.TipoModelo != WoTypeModel.Complex
                    && model.TipoModelo != WoTypeModel.Interface
                    && model.TipoModelo != WoTypeModel.Class
                    && model.TipoModelo != WoTypeModel.Parameter
                    && model.TipoModelo != WoTypeModel.TransactionSlave
                    && model.TipoModelo != WoTypeModel.CatalogSlave
                    && model.TipoModelo.ToString() != "CollectionComplex"
                    && File.Exists(pathDesign)
                )
                {
                    DataRow drRow = modelosCol.NewRow();
                    drRow[@"Proceso"] = model.ProcesoId;
                    drRow[@"Tipo"] = model.TipoModelo.ToString();
                    drRow[@"Repositorio"] = model.Repositorio.ToString();
                    drRow[@"Modelo"] = model.Id;
                    drRow[@"Probado"] = false;
                    modelosCol.Rows.Add(drRow);
                }
            }

            grdModels.DataSource = modelosCol;

            GridColumn column = grdModelsView.Columns[@"Proceso"];
            column.Width = 100;
            column.OptionsColumn.AllowEdit = false;

            column = grdModelsView.Columns[@"Tipo"];
            column.Width = 100;
            column.OptionsColumn.AllowEdit = false;

            column = grdModelsView.Columns[@"Repositorio"];
            column.Width = 100;
            column.OptionsColumn.AllowEdit = false;

            column = grdModelsView.Columns[@"Modelo"];
            column.Width = 300;
            column.OptionsColumn.AllowEdit = false;

            column = grdModelsView.Columns[@"Probado"];
            column.Width = 100;
            column.OptionsColumn.AllowEdit = false;

            grdModelsView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        column,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );
        }

        #endregion Carga de los modelos a la grid


        #region Creación de las clases de js para pruebas

        /// <summary>
        /// Método que usa las clases de generación de pruebas para generar las clases del modelo
        /// y formulario para las pruebas de testCafe
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildModelClass()
        {
            try
            {
                //ModelGenerator modelGenerator = new ModelGenerator();
                //modelGenerator.BuildModelClass(_model);

                //FormModelGenerator formModelGenerator = new FormModelGenerator();
                //formModelGenerator.BuildFormModelClass(_model);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al construir la clase del modelo para las pruebas. {ex.Message}"
                );
            }
        }

        #endregion Creación de las clases de js para pruebas


        #region Creación de pruebas

        /// <summary>
        /// Instancia del componente que realiza la creación de la pruebas.
        /// </summary>
        private WoNewTest _woNewTest;

        /// <summary>
        /// Creación de una nueva prueba
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnNewTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //_woNewTest = new WoNewTest(_model);
                //_woNewTest.NewTestCreated += ChargeTestGrid;
                //_woNewTest.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error en el método de la creación de pruebas. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Creación de pruebas

        #region Menu contextual de las pruebas

        /// <summary>
        /// Creación del menu contextual para la ejecución de pruebas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdTestView_PopupMenuShowing(
            object sender,
            DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e
        )
        {
            try
            {
                DevExpress.XtraGrid.Menu.GridViewMenu menu =
                    new DevExpress.XtraGrid.Menu.GridViewMenu(grdTestView);

                menu.Items.Add(
                    new DevExpress.Utils.Menu.DXMenuItem(
                        "Ejecutar Prueba",
                        new EventHandler(
                            delegate(object s, EventArgs args)
                            {
                                ExecuteTest();
                            }
                        )
                    )
                );

                menu.Items.Add(
                    new DevExpress.Utils.Menu.DXMenuItem(
                        "Editar Prueba",
                        new EventHandler(
                            delegate(object s, EventArgs args)
                            {
                                OpenCode();
                            }
                        )
                    )
                );

                menu.Items.Add(
                    new DevExpress.Utils.Menu.DXMenuItem(
                        "Eliminar Prueba",
                        new EventHandler(
                            delegate(object s, EventArgs args)
                            {
                                DeleteTest();
                            }
                        )
                    )
                );

                menu.Show(e.Point);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error en la selección de la prueba. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Menu contextual de las pruebas


        #region Ejecución del proyecto

        /// <summary>
        /// Instancia que controla la ejecución del proyecto de blazor server
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Instancia que controla la ejecución del proyecto de blazor wasm
        /// </summary>
        private WoBlazorWasmExecute _woBlazorWasmExecute = WoBlazorWasmExecute.GetInstance();

        /// <summary>
        /// Ejecución del proyecto de blazor en función de las configuraciones
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void ExecuteBlazor()
        {
            try
            {
                _woBlazorWasmExecute.GeneralStop();
                _woBlazorServerExecute.GeneralStop();

                btnBlazorExecute.Enabled = false;
                btnStopBlazor.Enabled = true;

                if (cmbAproach.EditValue == "Server")
                {
                    if (!_woBlazorServerExecute.IsRuning())
                    {
                        _sendBlazorError = false;
                        _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
                        await _woBlazorServerExecute.StartSimpleServer(_serverProjectName);
                    }
                }
                else if (cmbAproach.EditValue == "Wasm")
                {
                    if (!_woBlazorWasmExecute.IsRuning())
                    {
                        _sendBlazorError = false;
                        _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
                        await _woBlazorWasmExecute.StartWatchWasm(_wasmProjectName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la ejecución de blazor. {ex.Message}");
            }
        }

        /// <summary>
        /// Permite la ejecución de un proyecto de blazor sin necesidad de ejecutar una prueba.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnBlazorExecute_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                ExecuteBlazor();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar realizar la ejecución de la prueba seleccionada {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Ejecución del proyecto

        #region Detener blazor

        /// <summary>
        /// Detiene el proyecto de blazor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnStopBlazor_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                _woBlazorWasmExecute.GeneralStop();
                _woBlazorServerExecute.GeneralStop();

                btnBlazorExecute.Enabled = true;
                btnStopBlazor.Enabled = false;

                blazorConsole.SelectionColor = Color.Green;
                blazorConsole.AppendText("Proyecto detenido con éxito.");

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
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar detener el proyecto de blazor. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Detener blazor


        #region Ejecución de las pruebas

        /// <summary>
        /// Ejecutamos el test seleccionado del modelo
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ExecuteTest()
        {
            try
            {
                WoTestExecute woTestExecute = new WoTestExecute();
                woTestExecute.SendToConsoleEvt += SendDataTestToConsole;

                dynamic dataRow = grdTestView.GetFocusedRow();
                string testName = dataRow.Row.ItemArray[0];

                //woTestExecute.ExecuteTest(_modelName, testName);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar realizar la ejecución de la prueba seleccionada {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Ejecución de las pruebas

        #region Ejecución de todo el panel de pruebas

        /// <summary>
        /// Ejecución de las pruebas una tras otra del panel en selección
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnExecuteAll_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try { }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error en la ejecución de los test. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Ejecución de todo el panel de pruebas


        #region Envió de la data del test a consola

        /// <summary>
        /// Bandera que indica si ya fue enviada una alerta de error para la ejecución actual.
        /// </summary>
        private bool _sendTestError = false;

        /// <summary>
        /// Envío de la data a la consola.
        /// </summary>
        /// <param name="Data"></param>
        [SupportedOSPlatform("windows")]
        private void SendDataTestToConsole(string Data)
        {
            //testConsole.Invoke(
            //    new Action(() =>
            //    {
            //        if (Data.Contains("Building") || Data.Contains("building"))
            //        {
            //            blazorConsole.SelectionColor = Color.LightBlue;
            //        }
            //        else if (Data.Contains("Warning") || Data.Contains("warning"))
            //        {
            //            blazorConsole.SelectionColor = Color.Yellow;
            //        }
            //        else if (
            //            Data.Contains("Error")
            //            || Data.Contains("error")
            //            || Data.Contains("Stop")
            //        )
            //        {
            //            blazorConsole.SelectionColor = Color.Red;

            //            if (!_sendTestError)
            //            {
            //                XtraMessageBox.Show(
            //                    $"Se produjo un error al intentar ejecutar el proyecto de blazor.\n\r Revise la pestaña de ejecución para mas información",
            //                    "Alerta",
            //                    MessageBoxButtons.OK,
            //                    MessageBoxIcon.Error
            //                );
            //                _sendTestError = true;
            //            }
            //        }
            //        else if (Data.Contains("Info") || Data.Contains("info"))
            //        {
            //            blazorConsole.SelectionColor = Color.Green;
            //        }
            //        else
            //        {
            //            blazorConsole.SelectionColor = Color.White;
            //        }

            //        blazorConsole.AppendText($"{Data}\n\r");
            //        blazorConsole.ScrollToCaret();

            //        if (_sendTestError)
            //        {
            //            if (_woBlazorServerExecute.SendToConsoleEvt != null)
            //            {
            //                _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
            //                _woBlazorServerExecute.SendToConsoleEvt = null;
            //            }
            //            if (_woBlazorWasmExecute.SendToConsoleEvt != null)
            //            {
            //                _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
            //                _woBlazorWasmExecute.SendToConsoleEvt = null;
            //            }
            //        }
            //    })
            //);
        }

        #endregion Envió de la data del test a consola


        #region Envió de la data del blazor a la consola

        /// <summary>
        /// Bandera que indica si ya fue enviada una alerta de error para la ejecución actual.
        /// </summary>
        private bool _sendBlazorError = false;

        /// <summary>
        /// Envío de la data a la consola.
        /// </summary>
        /// <param name="Data"></param>
        [SupportedOSPlatform("windows")]
        private void SendDataToConsole(string Data)
        {
            blazorConsole.Invoke(
                new Action(() =>
                {
                    if (Data.Contains("Building") || Data.Contains("building"))
                    {
                        blazorConsole.SelectionColor = Color.LightBlue;
                    }
                    else if (Data.Contains("Warning") || Data.Contains("warning"))
                    {
                        blazorConsole.SelectionColor = Color.Yellow;
                    }
                    else if (
                        Data.Contains("Error")
                        || Data.Contains("error")
                        || Data.Contains("Stop")
                    )
                    {
                        blazorConsole.SelectionColor = Color.Red;

                        if (!_sendBlazorError)
                        {
                            XtraMessageBox.Show(
                                $"Se produjo un error al intentar ejecutar el proyecto de blazor.\n\r Revise la pestaña de ejecución para mas información",
                                "Alerta",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            _sendBlazorError = true;
                        }
                    }
                    else if (Data.Contains("Info") || Data.Contains("info"))
                    {
                        blazorConsole.SelectionColor = Color.Green;
                    }
                    else
                    {
                        blazorConsole.SelectionColor = Color.White;
                    }

                    blazorConsole.AppendText($"{Data}\n\r");
                    blazorConsole.ScrollToCaret();

                    if (_sendBlazorError)
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

        #endregion Envió de la data del blazor a la consola


        #region Eliminado de las pruebas

        /// <summary>
        /// Eliminamos el test seleccionado
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void DeleteTest()
        {
            try
            {
                dynamic dataRow = grdTestView.GetFocusedRow();
                string testName = dataRow.Row.ItemArray[0];

                if (
                    File.Exists($@"{proyecto.DirLayOuts}\Test\Unitarias\{_modelName}\{testName}.js")
                )
                {
                    File.Delete(
                        $@"{proyecto.DirLayOuts}\Test\Unitarias\{_modelName}\{testName}.js"
                    );
                }

                if (
                    File.Exists($@"{proyecto.DirLayOuts}\Test\Unitarias\{_modelName}\{testName}.js")
                )
                {
                    throw new Exception(
                        $@"No se pudo eliminar el fichero {testName}.js de la prueba"
                    );
                }
                else
                {
                    ChargeTestGrid();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar eliminar la prueba seleccionada {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Eliminado de las pruebas


        #region Salvado de código

        /// <summary>
        /// Guardamos el codigo del syntax
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnSaveCode_Click(object sender, EventArgs e)
        {
            try
            {
                dynamic focusedRow = grdTestView.GetFocusedRow();
                if (focusedRow == null)
                    return;

                string test = focusedRow.Row.ItemArray[0];
                string testPath = $@"{proyecto.DirLayOuts}\Test\Unitarias\{_modelName}\{test}.js";

                string rawCode = WoDirectory.ReadFile(testPath);

                string[] rawCodeCol = rawCode.Split("await _uth.Login();");
                string header = rawCodeCol.First();
                string footer = rawCodeCol.Last().Split("await _uth.logSucc();").Last();

                StringBuilder strNewCode = new StringBuilder();
                strNewCode.Append(header);
                strNewCode.AppendLine("await _uth.Login();");
                strNewCode.Append(_woBaseEditor.GetCode());
                strNewCode.AppendLine("await _uth.logSucc();");
                strNewCode.Append(footer);

                WoDirectory.WriteFile(testPath, strNewCode.ToString());
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al salvar el código {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Salvado de código


        #region Gestión de la ventana de codigo

        /// <summary>
        /// Abrimos la pestaña de código para editar el código
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void OpenCode()
        {
            try
            {
                grdTestView_FocusedRowChanged(null, null);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al abrir la ventana de código. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Cierra la ventana de código
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCloseCode_Click(object sender, EventArgs e)
        {
            try
            {
                sccSyntaxTest.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al cerrar la ventana de código. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Apertura del test en visual code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnOpenInVisual_Click(object sender, EventArgs e)
        {
            try
            {
                string visualPaht =
                    "C:\\Users\\Frog\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe";
                if (File.Exists(visualPaht))
                {
                    dynamic dataRow = grdTestView.GetFocusedRow();
                    string testName = dataRow.Row.ItemArray[0];

                    Process processVisualStudio = new Process();
                    processVisualStudio.StartInfo.FileName = visualPaht;
                    processVisualStudio.StartInfo.Arguments =
                        $@"{proyecto.DirLayOuts}\Test\Unitarias\{_modelName}\{testName}.js";
                    processVisualStudio.StartInfo.ErrorDialog = true;
                    processVisualStudio.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    processVisualStudio.Start();
                    processVisualStudio.WaitForExit();
                }
                else
                {
                    throw new Exception(
                        $@"No se encontró el ejecutable de visual code: {visualPaht}"
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al abrir visual code. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Gestión de la ventana de codigo



        /// ---------------------------------------------------------------------------------------------------------------------
        /// ---------------------------------------------------------------------------------------------------------------------
        /// Integrales
        /// ---------------------------------------------------------------------------------------------------------------------
        /// ---------------------------------------------------------------------------------------------------------------------



        #region Creación de grupo de integrales

        /// <summary>
        /// Apertura del dialog para la creación de grupos de integrales
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void btnNewIntegralGroup_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                WoNewIntegralGroup woNewIntegralGroup = new WoNewIntegralGroup();
                woNewIntegralGroup.GroupCreatedEvt += ChargeIntegralTests;
                woNewIntegralGroup.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar crear el grupo. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Creación de grupo de integrales

        #region Creación de integral

        /// <summary>
        /// Creación de una prueba integral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnNewIntegralTest_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                WoNewIntegralTest woNewIntegralTest = new WoNewIntegralTest();
                woNewIntegralTest.IntegralCreatedEvt += ChargeIntegralTests;
                woNewIntegralTest.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar crear la integral. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Creación de integral

        #region Carga del formulario

        /// <summary>
        /// Carga del combo de grupos
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeMainFormIntegral()
        {
            try
            {
                string integralPath = $@"{proyecto.DirProyectData}\Test\TestCafe\Integral";
                if (Directory.Exists(integralPath))
                {
                    List<string> groups = WoDirectory.ReadDirectoryDirectories(
                        integralPath,
                        onlyNames: true
                    );
                    cmbIntegralGroup.Properties.Items.AddRange(groups);
                }
                else
                {
                    throw new Exception("El fichero de las pruebas integrales, no existe");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar cargar los grupos de pruebas. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Carga del formulario


        #region Carga de las integrales

        /// <summary>
        /// Test integrales
        /// </summary>
        private DataTable _integralTests = new DataTable();

        /// <summary>
        /// Carga de la grid de test integrales
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeIntegralTests()
        {
            try
            {
                _integralTests = new DataTable();

                _integralTests.Columns.Add($@"Prueba", typeof(string));
                _integralTests.Columns.Add($@"Grupo", typeof(string));
                _integralTests.Columns.Add($@"Número de pruebas", typeof(int));
                _integralTests.Columns.Add($@"Status", typeof(bool));

                string integralPath = $@"{proyecto.DirProyectData}\Test\TestCafe\Integral";
                List<(string path, string name)> testsGroupsCol =
                    WoDirectory.ReadDirectoryDirectoriesPathName(path: integralPath, create: true);

                foreach ((string path, string name) testGroup in testsGroupsCol)
                {
                    ChargeIntegralTests(testGroup.path);
                }

                grdIntegralTest.DataSource = _integralTests;

                grdIntegralTestView.Columns[@"Prueba"].OptionsColumn.AllowEdit = false;
                grdIntegralTestView.Columns[@"Grupo"].OptionsColumn.AllowEdit = false;
                grdIntegralTestView.Columns[@"Número de pruebas"].OptionsColumn.AllowEdit = false;
                grdIntegralTestView.Columns[@"Status"].OptionsColumn.AllowEdit = false;

                grdIntegralTestView.Columns[@"Grupo"].Group();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar cargar las pruebas integrales a la grid. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Cargamos las pruebas integrales
        /// </summary>
        /// <param name="path"></param>
        /// <param name="group"></param>
        private void ChargeIntegralTests(string path)
        {
            try
            {
                List<string> integralTestCol = WoDirectory.ReadDirectoryFiles(path);
                foreach (string integralTest in integralTestCol)
                {
                    string jsonRaw = File.ReadAllText(integralTest);
                    WoTestIntegral test = JsonConvert.DeserializeObject<WoTestIntegral>(jsonRaw);

                    DataRow drTest = _integralTests.NewRow();

                    drTest["Prueba"] = test.IntegralName;
                    drTest["Grupo"] = test.Group;
                    drTest["Número de pruebas"] = test.TestCol.Count;
                    drTest["Status"] = test.TestStatus;

                    _integralTests.Rows.Add(drTest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Ocurrió un error al carga la integral. {ex.Message}");
            }
        }

        #endregion Carga de las integrales


        #region Cambio de selección de integral

        /// <summary>
        /// Cambio de la selección de la integral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdIntegralTestView_SelectionChanged(
            object sender,
            DevExpress.Data.SelectionChangedEventArgs e
        )
        {
            try
            {
                txtIntegralName.Enabled = false;
                txtDescription.Enabled = false;
                txtStatus.Enabled = false;
                cmbIntegralGroup.Enabled = false;

                dynamic integralTest = grdIntegralTestView.GetFocusedRow();

                if (integralTest != null)
                {
                    string testName = integralTest.Row.ItemArray[0];
                    string group = integralTest.Row.ItemArray[1];

                    string integralTestPath =
                        $@"{proyecto.DirProyectData}\Test\TestCafe\Integral\{group}\{testName}.json";

                    if (File.Exists(integralTestPath))
                    {
                        string rawJson = WoDirectory.ReadFile(integralTestPath);
                        WoTestIntegral testIntegral = JsonConvert.DeserializeObject<WoTestIntegral>(
                            rawJson
                        );

                        txtIntegralName.Text = testIntegral.IntegralName;
                        txtDescription.Text = testIntegral.IntegralDescription;
                        txtStatus.Text = testIntegral.TestStatus.ToString();
                        cmbIntegralGroup.SelectedItem = testIntegral.Group;
                    }
                    else
                    {
                        throw new Exception($@"Error, no se encontró el test. {integralTestPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al seleccionar la prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Cambio de selección de integral


        #region Edición de la integral

        /// <summary>
        /// Pone en modo edición el panel de configuraciones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnEditIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                txtIntegralName.Enabled = true;
                txtDescription.Enabled = true;
                txtStatus.Enabled = false;
                cmbIntegralGroup.Enabled = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al intentar editar la prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Guardamos las nuevas configuraciones de la integral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnSaveIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                dynamic integralTest = grdIntegralTestView.GetFocusedRow();
                string testName = integralTest.Row.ItemArray[0];
                string group = integralTest.Row.ItemArray[1];

                string integralTestPath =
                    $@"{proyecto.DirProyectData}\Test\TestCafe\Integral\{group}\{testName}.json";

                if (File.Exists(integralTestPath))
                {
                    string rawJson = WoDirectory.ReadFile(integralTestPath);
                    WoTestIntegral oldTestIntegral = JsonConvert.DeserializeObject<WoTestIntegral>(
                        rawJson
                    );

                    oldTestIntegral.IntegralName = txtIntegralName.Text;
                    oldTestIntegral.IntegralDescription = txtDescription.Text;
                    oldTestIntegral.Group = cmbIntegralGroup.SelectedText;

                    File.Delete(integralTestPath);

                    if (!File.Exists(integralTestPath))
                    {
                        WoDirectory.WriteFile(
                            $@"{proyecto.DirProyectData}\Test\TestCafe\Integral\{cmbIntegralGroup.SelectedText}\{txtIntegralName.Text}.json",
                            JsonConvert.SerializeObject(oldTestIntegral)
                        );

                        txtIntegralName.Enabled = false;
                        txtDescription.Enabled = false;
                        txtStatus.Enabled = false;
                        cmbIntegralGroup.Enabled = false;

                        ChargeIntegralTests();
                    }
                    else
                    {
                        throw new Exception($@"Error no fue posible eliminar el fichero");
                    }
                }
                else
                {
                    throw new Exception($@"Error el fichero no existe");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al intentar salvar los cambios. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Edición de la integral


        #region Agregado de test a la integral

        /// <summary>
        /// Abre el componente para agregar test a la integral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAddTestIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                dynamic integralTest = grdIntegralTestView.GetFocusedRow();
                if (integralTest != null)
                {
                    string testName = integralTest.Row.ItemArray[0];
                    string group = integralTest.Row.ItemArray[1];

                    string integralTestPath =
                        $@"{proyecto.DirProyectData}\Test\TestCafe\Integral\{group}\{testName}.json";

                    WoAddTest woAddTest = new WoAddTest(integralTestPath);
                    woAddTest.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar agregar test. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Agregado de test a la integral
    }
}
