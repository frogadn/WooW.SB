using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using ServiceStack;
using WooW.Core;
using WooW.Core.Server;
using WooW.SB.BlazorGenerator;
using WooW.SB.BlazorGenerator.BlazorDialogs.BlazorDialogsModels;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.BlazorTestGenerator.BuildBaseClass;
using WooW.SB.BlazorTestGenerator.BuildTest.BaseForm;
using WooW.SB.BlazorTestGenerator.BuildTest.SnippetsForm;
using WooW.SB.BlazorTestGenerator.Components;
using WooW.SB.BlazorTestGenerator.Models;
using WooW.SB.BlazorTestGenerator.TestTools;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Data;
using WooW.SB.Config.Editors;
using WooW.SB.Config.Templates;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Dialogs;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.SB.ManagerDirectory;
using WooW.SB.TestServices;

namespace WooW.SB.Forms
{
    public partial class fmTestCases : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        #region Instancias singleton

        public WooWConfigParams wooWConfigParams { get; set; }

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que estamos trabajando
        /// </summary>
        public Proyecto proyecto { get; set; }

        #endregion Instancias singleton


        #region Constructor principal

        /// <summary>
        /// Constructor principal, solo para inicializar parámetros
        /// </summary>
        public fmTestCases()
        {
            InitializeComponent();
        }

        #endregion Constructor principal


        #region Implementación extra de IForm

        /// <summary>
        /// Retornamos la instancia del ribbon
        /// </summary>
        [SupportedOSPlatform("windows")]
        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        /// <summary>
        /// Indica si hay o no cambios pendientes
        /// </summary>
        [SupportedOSPlatform("windows")]
        public bool CambiosPendientes
        {
            get { return false; }
        }

        /// <summary>
        /// Retorna el texto de la tab del ribbon seleccionada
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string Nombre
        {
            get { return rbpUnitTest.Text; }
        }

        #endregion Implementación extra de IForm


        #region Métodos de carga del formulario

        /// <summary>
        /// Método de carga principal del formulario, ya una vez los componentes cargados en la pantalla
        /// ideal para procesos de inicio.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        [SupportedOSPlatform("windows")]
        public void Cargar()
        {
            // Apartado de unitarias
            try
            {
                InitializeRibbon();
                InitializeComponents();

                InitializeManagerDirectory();

                InitializeConsoleUnitTest();

                InitializeSyntaxEditor(new CSharpSyntaxLanguage());

                InitializeGridOdata();

                BuildPropertiesJson(true);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al cargar las pruebas unitarias en la pantalla de test. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Función de refresh para cuando se busca re iniciar los datos
        /// //TODO: Implementar para los datos de la pantalla
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Refrescar()
        {
            throw new NotImplementedException();
        }

        #endregion Métodos de carga del formulario


        #region Ribbon

        /// <summary>
        /// Permite que el ribbon principal se actualize con las modificaciones sobre el ribbon actual
        /// </summary>
        public Action UpdateRibbonEvt;

        /// <summary>
        /// Pagina del ribbon principal
        /// </summary>
        private RibbonPage _mainRibbonPage;

        /// <summary>
        /// Método con ajustes que se hacen cuando se inicializa el ribbon
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeRibbon()
        {
            try
            {
                _mainRibbonPage = ribbonControl1.Pages.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error en la configuración principal del ribbon. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Actualiza el ribbon actual para el ribbon principal
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void UpdateRibbon()
        {
            try
            {
                // Limpia todos los componentes del ribbon
                ribbonControl1.Pages.Clear();

                //Pagina base principal
                ribbonControl1.Pages.Add(_mainRibbonPage);

                // Merge del ribbon del syntax
                if (ribbonControl1 != null && _woSyntaxEditor != null)
                {
                    RibbonControl childRibbon = _woSyntaxEditor.GetRibbon();
                    RibbonPageCollection pages = childRibbon.Pages;
                    for (int i = 0; i < pages.Count; i++)
                    {
                        ribbonControl1.Pages.Add(pages[i]);
                    }
                }

                // Merge del ribbon del árbol de directorios
                if (ribbonControl1 != null && _woManagerDirectoryUnitTest != null)
                {
                    RibbonControl childRibbon = _woManagerDirectoryUnitTest.GetRibbon();
                    RibbonPageGroupCollection groups = childRibbon.Pages.First().Groups;
                    foreach (RibbonPageGroup group in groups)
                    {
                        ribbonControl1.Pages.First().Groups.Add(group);
                    }
                }

                // Merge del ribbon del componente de la consola
                if (ribbonControl1 != null && _woConsole != null)
                {
                    RibbonControl childRibbon = _woConsole.GetRibbon();
                    RibbonPageGroupCollection groups = childRibbon.Pages.First().Groups;
                    foreach (RibbonPageGroup group in groups)
                    {
                        ribbonControl1.Pages.First().Groups.Add(group);
                    }
                }

                ribbonControl1.Refresh();

                // Actualizamos el ribbon principal con los últimos cambios
                UpdateRibbonEvt?.Invoke();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al actualizar el ribbon. {ex.Message}");
            }
        }

        #endregion Ribbon


        /// PRUEBAS UNITARIAS
        ///


        #region Build Properties json

        /// <summary>
        /// Instancia del componente de la consola para las ejecuciones
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BuildPropertiesJson(bool firstExecution = false)
        {
            try
            {
                string pathTestingProperties =
                    $"{proyecto.DirProyectData}\\Test\\TestCafe\\TestingProperties.json";

                if (firstExecution)
                {
                    if (File.Exists(pathTestingProperties))
                    {
                        string rawSettings = WoDirectory.ReadFile(pathTestingProperties);
                        WoTestingProperties testingProperties =
                            JsonConvert.DeserializeObject<WoTestingProperties>(rawSettings);

                        speTestSpeed.EditValue = testingProperties.SpeedTest;

                        return;
                    }
                    else
                    {
                        speTestSpeed.EditValue = 0.5;
                    }
                }

                double speed = double.Parse(speTestSpeed.EditValue.ToString());

                WoTestingProperties woTestingProperties = new WoTestingProperties()
                {
                    User = proyecto.ParConexion.Usuario,
                    Password = proyecto.ParConexion.Password,
                    Instance = proyecto.ParConexion.Instance,
                    Udn = proyecto.ParConexion.Udn,
                    Year = proyecto.ParConexion.Year,
                    InstanceType = proyecto.ParConexion.InstanceType.ToString(),
                    BasePath = proyecto.Dir,
                    SpeedTest = speed,
                };

                WoDirectory.WriteFile(
                    pathTestingProperties,
                    JsonConvert.SerializeObject(woTestingProperties)
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al actualizar el fichero de TestingProperties. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Cambio de velocidad del test de la pantalla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void speTestSpeed_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                double speed = double.Parse(speTestSpeed.EditValue.ToString());

                if (speed > 1 || speed < 0.1)
                {
                    speTestSpeed.EditValue = 0.5;
                    throw new Exception("La velocidad máxima es 1 y la mínima es 0.1");
                }

                BuildPropertiesJson();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al cambiar la velocidad de la prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Build Properties json


        #region Inicialización de la consola

        /// <summary>
        /// Instancia del componente de la consola para las ejecuciones
        /// </summary>
        private woConsole _woConsole;

        /// <summary>
        /// Instancia el componente de la consola y configura el ribbon
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeConsoleUnitTest()
        {
            try
            {
                _woConsole = new woConsole();
                pnlConsole.Controls.Add(_woConsole);
                _woConsole.Dock = DockStyle.Fill;

                UpdateRibbon();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al inicializar la consola de unitarias. {ex.Message}");
            }
        }

        #endregion Inicialización de la consola

        #region Inicialización del grid OData

        /// <summary>
        /// Inicializa el grid oData con los snippets
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeGridOdata()
        {
            try
            {
                woGridModel.MostraSoloDiagrama();

                if (_woSyntaxEditor != null)
                {
                    woGridModel.SetSnippet = _woSyntaxEditor.SetSnippet;
                    woGridModel.ModelFocusedChangeEvt += ReCreateSnippets;
                    woGridModel.Snippet = woGridModel.eSnippet.SnippetCliente;
                }

                CreateSnippetsCustom();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al inicializar el grid de OData. {ex.Message}");
            }
        }

        #endregion Inicialización del grid OData

        #region Snippets del grid de modelo

        /// <summary>
        /// Cambiamos la lista de snippets para el modelo seleccionado, cuando
        /// se modifica el modelo seleccionado
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ReCreateSnippets()
        {
            try
            {
                if (_woSyntaxEditor != null)
                {
                    woGridModel.CleanCustomMethods();
                }

                CreateSnippetsCustom();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al crear el snippet. {ex.Message}");
            }
        }

        /// <summary>
        /// Agregamos la lista de opciones de snippets para agregar código
        /// para realizar operaciones
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateSnippetsCustom()
        {
            try
            {
                //Transition snippets
                if (woGridModel != null)
                {
                    Modelo selectedModel = woGridModel.ModeloSeleccionado();
                    string pathFormDesign = $@"{proyecto.DirFormDesign}\{selectedModel.Id}.json";

                    if (selectedModel != null && File.Exists(pathFormDesign))
                    {
                        //Import
                        woGridModel.SetSnippetOption(
                            title: "Formulario - Import Modelo",
                            actionEvt: (modelName) => BuildImportModel(modelName)
                        );

                        //Instancia de los modelos
                        woGridModel.SetSnippetOption(
                            title: $@"Formulario - Instanciar modelo y formulario",
                            actionEvt: (modelName) => SetModels(modelName)
                        );

                        //Navegación directa
                        woGridModel.SetSnippetOption(
                            title: $@"Formulario - Navegar al formulario",
                            actionEvt: (modelName) => SetDirectNavigate(modelName)
                        );

                        //Transiciones del modelo
                        foreach (Transicion transition in selectedModel.Diagrama.Transiciones)
                        {
                            woGridModel.SetSnippetOption(
                                title: $@"Formulario - {transition.Id}",
                                actionEvt: (modelName) =>
                                    SetTransitionSnippet(modelName, transition.Id)
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al crear las opciones de los snippets para formularios. {ex.Message}"
                );
            }
        }

        #endregion Snippets del grid de modelo

        #region Imports de los modelos

        /// <summary>
        /// Construcción de los imports para los modelos
        /// </summary>
        /// <param name="modelName"></param>
        [SupportedOSPlatform("windows")]
        private void BuildImportModel(string modelName)
        {
            try
            {
                string scriptPath = _woSyntaxEditor.GetFilePath();

                string[] pathTestCol = scriptPath.Split("\\");
                string escs = string.Empty;
                bool unitTestPath = false;

                foreach (string pathPart in pathTestCol)
                {
                    if (unitTestPath && !pathPart.Contains(".js"))
                    {
                        escs += "../";
                    }
                    unitTestPath = (unitTestPath) ? unitTestPath : pathPart == "UnitTest";
                }

                string baseRefPath = $@"{escs}../TestCafe/Common";

                StringBuilder strImports = new StringBuilder();
                strImports.AppendLine(
                    $@"import {{ {modelName}Model }} from '{baseRefPath}/{modelName}Model.js';"
                );
                strImports.AppendLine(
                    $@"import {{ {modelName}Form }} from '{baseRefPath}/{modelName}Form.js';"
                );

                _woSyntaxEditor.SetSnippet(strImports.ToString());

                AddModelsToTest(modelName);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al agregar el codigo para el import. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Imports de los modelos

        #region Creación de los objetos

        /// <summary>
        /// Instanciamos el modelo del formulario y de los campos
        /// </summary>
        /// <param name="modelName"></param>
        [SupportedOSPlatform("windows")]
        private void SetModels(string modelName)
        {
            try
            {
                StringBuilder strInstanceModel = new StringBuilder();
                strInstanceModel.AppendLine($@"var _{modelName} = new {modelName}Model();");
                strInstanceModel.AppendLine(
                    $@"var _{modelName}Formulario = new {modelName}Form();"
                );

                _woSyntaxEditor.SetSnippet(strInstanceModel.ToString());

                AddModelsToTest(modelName);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al instanciar el modelo {modelName}. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Creación de los objetos

        #region Navegación del formulario

        /// <summary>
        /// Agrega el codigo para navegación directa al formulario
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SetDirectNavigate(string modelName)
        {
            try
            {
                Modelo findModel = proyecto.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == modelName
                );
                if (findModel != null)
                {
                    string urlModelForm =
                        $@"/{findModel.ProcesoId}/{findModel.TipoModelo}/{findModel.Id}";
                    _woSyntaxEditor.SetSnippet($@"_uth.Navegar('{urlModelForm}');");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al crear la navegación al formulario del modelo {modelName}. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Navegación del formulario

        #region Transiciones del modelo

        /// <summary>
        /// Crea y envía el snippet al syntax para las transacciones
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SetTransitionSnippet(string modelName, string transitionName)
        {
            try
            {
                WoBuildTransitionsCode woBuildTransitionsCode = new WoBuildTransitionsCode();
                string trnasitionCode = woBuildTransitionsCode.BuildTransitionCode(
                    modelName,
                    transitionName
                );

                _woSyntaxEditor.SetSnippet(trnasitionCode);

                AddModelsToTest(modelName);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al agregar el código para crear el snippet. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Transiciones del modelo

        #region Agregar modelos al test

        /// <summary>
        /// Agrega modelos al header del codigo en caso de que
        /// aun no se tengan
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void AddModelsToTest(string modelName)
        {
            try
            {
                string scriptTest = _woSyntaxEditor.GetFilePath();
                if (scriptTest != string.Empty && File.Exists(scriptTest))
                {
                    string rawTest = _woSyntaxEditor.GetCode();

                    string[] rawTestCol = rawTest.Split("//");
                    string newLine = string.Empty;
                    string oldLine = string.Empty;
                    foreach (string line in rawTestCol)
                    {
                        if (line.Contains("/Modelos:"))
                        {
                            oldLine = line;

                            string[] modelsCol = line.Replace("/Modelos:", "")
                                .Replace("\r", "")
                                .Replace("\n", "")
                                .Replace(" ", "")
                                .Split(",");

                            if (!modelsCol.Contains(modelName))
                            {
                                string models = line.Replace("/Modelos:", "")
                                    .Replace("\r", "")
                                    .Replace("\n", "")
                                    .Replace(" ", "");
                                newLine = $"/Modelos: {models.Replace(",", ", ")}, {modelName}\r\n";
                            }

                            break;
                        }
                    }

                    if (newLine != string.Empty)
                    {
                        _woSyntaxEditor.ReplaceCode(oldLine, newLine);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al agregar el modelo, {modelName}. {ex.Message}");
            }
        }

        #endregion Agregar modelos al test


        #region Inicialización del árbol de ficheros

        /// <summary>
        /// Instancia del componente de manejo de directorios para las pruebas unitarias
        /// </summary>
        [SupportedOSPlatform("windows")]
        private woManagerDirectory _woManagerDirectoryUnitTest = new woManagerDirectory();

        /// <summary>
        /// inicialización del componente de manejo de directorios para las pruebas unitarias
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeManagerDirectory()
        {
            try
            {
                // Configuraciones del componente
                string unitTestPath = $@"{proyecto.DirProyectData}\Test\UnitTest";
                _woManagerDirectoryUnitTest.MainPath = unitTestPath;

                // Suscripción de los eventos
                _woManagerDirectoryUnitTest.FocusNodeChangeEvt += FocusChange;
                _woManagerDirectoryUnitTest.EditSelectedFileEvt += EditFile;

                // Posicionamiento del componente en la pantalla
                pnlFilesUnitTest.Controls.Add(_woManagerDirectoryUnitTest);
                _woManagerDirectoryUnitTest.Dock = DockStyle.Fill;
                _woManagerDirectoryUnitTest.RefreshComponent();

                // Actualización del ribbon con los items de este componente
                UpdateRibbon();

                // Custom Menu
                CreateCustomMenu();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al intentar iniciar el manager directory. {ex.Message}"
                );
            }
        }

        #endregion Inicialización del árbol de ficheros


        #region Creación de menu custom para el manager de directorios

        /// <summary>
        /// Crea el menu custom para el directory manager
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void CreateCustomMenu()
        {
            try
            {
                _woManagerDirectoryUnitTest.ActiveDefaultMenu = false;

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Nueva Carpeta",
                    actionEvt: (path) => NewDirectory(path),
                    optionMenuType: eOptionMenuType.General
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Renombrar Carpeta",
                    actionEvt: (path) => RenameDirectory(),
                    optionMenuType: eOptionMenuType.Directories
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Eliminar Carpeta",
                    actionEvt: (path) => DeleteDirectory(path),
                    optionMenuType: eOptionMenuType.Directories
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Nuevo Caso Prueba De Servicio",
                    actionEvt: (path) => NewTestCaseCs(path),
                    optionMenuType: eOptionMenuType.Directories
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Nuevo Caso Prueba De Formulario",
                    actionEvt: (path) => NewTestCaseJs(path),
                    optionMenuType: eOptionMenuType.Directories
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Renombrar Fichero",
                    actionEvt: (path) => RenameFile(),
                    optionMenuType: eOptionMenuType.Files
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Eliminar Fichero",
                    actionEvt: (path) => DeleteFile(path),
                    optionMenuType: eOptionMenuType.Files
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Abrir en Visual Code",
                    actionEvt: (path) => OpenInVisualCode(path),
                    optionMenuType: eOptionMenuType.Files
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Ejecutar Prueba JS En Server Side",
                    actionEvt: (path) => ExecuteJs(path, "Server"),
                    optionMenuType: eOptionMenuType.FileTypes,
                    new List<string>() { "js" }
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Ejecutar Prueba JS En Web Assembly",
                    actionEvt: (path) => ExecuteJs(path, "Wasm"),
                    optionMenuType: eOptionMenuType.FileTypes,
                    new List<string>() { "js" }
                );

                _woManagerDirectoryUnitTest.AddCustomMenuOption(
                    title: "Ejecutar Prueba CS",
                    actionEvt: (path) => ExecuteCs(path),
                    optionMenuType: eOptionMenuType.FileTypes,
                    new List<string>() { "cs" }
                );
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al crear el menu custom. {ex.Message}");
            }
        }

        #endregion Creación de menu custom para el manager de directorios

        #region Gestión de directorios

        /// <summary>
        /// Nuevo caso de prueba de js
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void NewDirectory(string focusedPath)
        {
            try
            {
                if (File.Exists(focusedPath))
                {
                    string[] focusedPathCol = focusedPath.Split("\\");
                    focusedPath = focusedPath.Replace($"\\{focusedPathCol.Last()}", "");
                }

                int count = 0;
                string newDirectory = "Nueva Carpeta";

                do
                {
                    newDirectory = "Nueva Carpeta";
                    newDirectory = (count < 1) ? newDirectory : $@"{newDirectory} ({count})";
                    count++;
                } while (Directory.Exists($@"{focusedPath}\{newDirectory}"));

                WoDirectory.CreateDirectory(pathDirectory: $@"{focusedPath}\{newDirectory}");

                _woManagerDirectoryUnitTest.RefreshComponent();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al crear el directorio. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Re nombra el directorio
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void RenameDirectory()
        {
            try
            {
                _woManagerDirectoryUnitTest.RenameFolder();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al eliminar el directorio. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Elimina el directorio
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void DeleteDirectory(string focusedPath)
        {
            try
            {
                WoDirectory.DeleteDirectory(focusedPath);

                _woManagerDirectoryUnitTest.RefreshComponent();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al eliminar el directorio. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Gestión de directorios

        #region Gestión de ficheros

        /// <summary>
        /// Re nombra el fichero
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void RenameFile()
        {
            try
            {
                _woManagerDirectoryUnitTest.RenameFile();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al renombrar el fichero. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Elimina el fichero
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void DeleteFile(string focusedPath)
        {
            try
            {
                WoDirectory.DeleteFile(focusedPath);

                _woManagerDirectoryUnitTest.RefreshComponent();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al eliminar el fichero. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Gestión de ficheros

        #region Abrir en visual

        /// <summary>
        /// Abre el fichero seleccionado en visual studio code
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void OpenInVisualCode(string focusedPath)
        {
            try
            {
                if (File.Exists(focusedPath))
                {
                    string visualPaht =
                        "C:\\Users\\Frog\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe";
                    if (File.Exists(visualPaht))
                    {
                        Process processVisualStudio = new Process();
                        processVisualStudio.StartInfo.FileName = visualPaht;
                        processVisualStudio.StartInfo.Arguments = focusedPath;
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
                else
                {
                    throw new Exception($@"El fichero ""{focusedPath}"" no existe.");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al abrir el fichero en visual code. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Abrir en visual


        #region Creación de los ficheros de prueba



        /// <summary>
        /// Nuevo caso prueba cs
        /// </summary>
        /// <param name="data"></param>
        [SupportedOSPlatform("windows")]
        private void NewTestCaseCs(string focusedPath)
        {
            try
            {
                int count = 0;
                string newFile = "NuevoCasoPrueba";

                do
                {
                    newFile = "NuevoCasoPrueba";
                    newFile = (count < 1) ? newFile : $@"{newFile}({count})";
                    count++;
                } while (File.Exists($@"{focusedPath}\{newFile}.cs"));

                WoDirectory.WriteFile(path: $@"{focusedPath}\{newFile}.cs", string.Empty);

                _woManagerDirectoryUnitTest.RefreshComponent();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al crear el test de servicios. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Creación de los ficheros de prueba


        #region Creación de las pruebas de JS

        /// <summary>
        /// Nuevo caso de prueba de js
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void NewTestCaseJs(string focusedPath)
        {
            try
            {
                int count = 0;
                string newFile = "NuevoCasoPrueba";

                do
                {
                    newFile = "NuevoCasoPrueba";
                    newFile = (count < 1) ? newFile : $@"{newFile}({count})";
                    count++;
                } while (File.Exists($@"{focusedPath}\{newFile}.js"));

                string pathTest = $@"{focusedPath}\{newFile}.js";

                WoBuildBaseTemplate woBuildBaseTemplate = new WoBuildBaseTemplate();
                woBuildBaseTemplate.BuildBaseTemplate(pathTest);

                _woManagerDirectoryUnitTest.RefreshComponent();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al crear el test de servicios. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Creación de las pruebas de JS


        #region Cambio de foco de unitarias

        /// <summary>
        /// Método suscrito al action del componente que se ejecuta cuando
        /// cambiamos el foco en el tree de pruebas
        /// </summary>
        /// <param name="newSelectedPath"></param>
        [SupportedOSPlatform("windows")]
        private void FocusChange(string newSelectedPath)
        {
            try
            {
                if (File.Exists(newSelectedPath))
                {
                    string rawCode = WoDirectory.ReadFile(newSelectedPath);

                    if (Path.GetExtension(newSelectedPath).ToLower() == ".js")
                    {
                        //InitializeSyntaxEditor(new JavaScriptSyntaxLanguage());

                        _woSyntaxEditor.SetCode(rawCode);
                        _woSyntaxEditor.SetFilePath(newSelectedPath);
                    }
                    else if (Path.GetExtension(newSelectedPath).ToLower() == ".cs")
                    {
                        InitializeSyntaxEditor(new CSharpSyntaxLanguage());

                        _woSyntaxEditor.SetCode(rawCode);
                        _woSyntaxEditor.SetFilePath(newSelectedPath);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al cambiar el foco a la prueba en el path: {newSelectedPath}. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Cambio de foco de unitarias

        #region Edición de unitarias

        /// <summary>
        /// Pone el fichero en modo de edición
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void EditFile(string directoryPath)
        {
            try
            {
                string rawCode = WoDirectory.ReadFile(directoryPath);
                string fileName = Path.GetFileNameWithoutExtension(directoryPath);
                string extencion = Path.GetExtension(directoryPath);

                bool isHybrid = false;
                if (fileName.EndsWith(".Hybrid"))
                {
                    fileName = fileName.Replace('.', '_');
                    isHybrid = true;
                }

                if (!ValidateTestName(fileName))
                {
                    return;
                }

                if (rawCode.IsNullOrStringTrimEmpty())
                {
                    CreateBaseTestDefault(directoryPath, isHybrid);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al editar el fichero: {directoryPath}. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Edición de unitarias

        #region Validaciones de unitarias

        /// <summary>
        /// Validamos el nombre para los test unitarios
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private bool ValidateTestName(string fileName)
        {
            try
            {
                Regex regex = new Regex(@"[A-Z][a-zA-Z0-9]*");
                if (regex.Match(fileName).Value.ToString() != fileName)
                {
                    XtraMessageBox.Show(
                        "Nombre del archivo debe comenzar con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return false;
                }

                string Prefijo = "CasoPrueba";
                if (fileName.ToUpper().StartsWith(Prefijo.ToUpper()))
                {
                    XtraMessageBox.Show(
                        "Renombre el archivo, no puede comenzar con " + Prefijo,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error el validar el nombre del test. {ex.Message}");
            }
        }

        #endregion Validaciones de unitarias

        #region Creación de la base de los scripts

        /// <summary>
        /// En función del tipo de prueba se crea ya sea con la template o con el modulo de
        /// creación para js
        /// </summary>
        /// <param name="pathTest"></param>
        /// <param name="Hybrid"></param>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void CreateBaseTestDefault(string pathTest, bool Hybrid)
        {
            try
            {
                string sNombreCasoPrueba = Path.GetFileNameWithoutExtension(pathTest);

                string extencion = Path.GetExtension(pathTest);

                if (extencion == ".cs")
                {
                    if (Hybrid)
                    {
                        ttCasoPruebaApp ttprueba = new ttCasoPruebaApp();
                        ttprueba.NombreCasoPrueba = sNombreCasoPrueba;
                        //txtScriptApp.Text = SyntaxEditorHelper.PrettyPrint(
                        //    ttprueba.TransformText()
                        //);
                    }
                    else
                    {
                        ttCasoPrueba ttprueba = new ttCasoPrueba();
                        ttprueba.NombreCasoPrueba = sNombreCasoPrueba;
                        //txtScript.Text = SyntaxEditorHelper.PrettyPrint(ttprueba.TransformText());
                    }
                }
                else if (extencion == ".js")
                {
                    WoNewTest woNewTest = new WoNewTest(pathTest);
                    woNewTest.NewTestCreated += NewTestCreated;
                    woNewTest.CreateTestCancelEvt += CancelTestCreated;
                    woNewTest.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la generación base del script. {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private void NewTestCreated(string pathTest)
        {
            try
            {
                string rawCode = WoDirectory.ReadFile(pathTest);
                //GetCurrentScriptEditor().Text = rawCode;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error durante la creación de la prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        [SupportedOSPlatform("windows")]
        private void CancelTestCreated() { }

        #endregion Creación de la base de los scripts


        #region Actualizar unitaria de blazor

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

        /// <summary>
        /// Actualiza el proyecto de blazor de la unitaria para los test
        /// </summary>
        /// <param name="model"></param>
        [SupportedOSPlatform("windows")]
        private void UpdateUnitProject(Modelo model)
        {
            try
            {
                string serverPath =
                    $@"{proyecto.DirProyectTemp}\{_serverProjectName}\{_serverProjectName}.csproj";
                string wasmPath =
                    $@"{proyecto.DirProyectTemp}\{_wasmProjectName}\{_wasmProjectName}.csproj";

                WoContainer woContainer = new WoContainer();
                if (
                    model.TipoModelo == WoTypeModel.Kardex
                    || model.TipoModelo == WoTypeModel.Control
                    || model.TipoModelo == WoTypeModel.Configuration
                    || model.TipoModelo == WoTypeModel.View
                )
                {
                    WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
                    woContainer = woDesignerRawListHelper.BuildRawListForm(model.Id);
                }
                else
                {
                    string pathLayout = $@"{proyecto.DirLayOuts}\FormDesign\{model.Id}.json";
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
            catch (Exception ex)
            {
                throw new Exception($@"Error al actualizar la unitaria. {ex.Message}");
            }
        }

        #endregion Actualizar unitaria de blazor


        #region Ensamblados del syntax

        private List<string> _references = new List<string>()
        {
            "Newtonsoft.Json.dll",
            "ServiceStack.Client.dll",
            "ServiceStack.Common.dll",
            "ServiceStack.Interfaces.dll",
            "ServiceStack.Text.dll",
            "GemBox.Document.dll",
            "MimeKit.dll",
            "WooW.CFDI.dll",
            "WooW.Resources.dll",
            "WooW.Core.Common.dll",
            "WooW.WebClient.dll"
        };

        private List<string> _systemReferences = new List<string>()
        {
            "System.RunTime",
            "mscorlib",
            "System",
            "System.Core",
            "System.Data",
            "System.Data.Common",
            "System.Drawing",
            "System.ComponentModel.Primitives",
            "System.Diagnostics.Process",
            "System.Text.RegularExpressions",
            "System.Linq",
            "System.Linq.Expressions",
            "System.Private.CoreLib",
            "System.Reflection",
            "System.Collections",
            "System.IO.Compression.FileSystem",
            "System.Numerics",
            "System.RunTime.Serialization",
            "System.Xml",
            "System.Xml.Linq",
            "System.Xml.ReaderWriter",
            "System.Private.Xml"
        };

        private List<string> _referencesApp = new List<string>()
        {
            "DynamicODataToSQL.dll",
            "FastMember.dll",
            "Microsoft.OData.Core.dll",
            "Microsoft.OData.Edm.dll",
            "Microsoft.Spatial.dll",
            "Newtonsoft.Json.dll",
            "ServiceStack.Client.dll",
            "ServiceStack.Common.dll",
            "ServiceStack.dll",
            "ServiceStack.Interfaces.dll",
            "ServiceStack.OrmLite.dll",
            "ServiceStack.OrmLite.Sqlite.dll",
            "ServiceStack.Redis.dll",
            "ServiceStack.Server.dll",
            "ServiceStack.Text.dll",
            "SqlKata.dll",
            "WooW.Resources.dll",
            "WooW.Core.Common.dll",
            "WooW.Core.Server.dll"
        };

        private List<string> _systemReferencesApp = new List<string>()
        {
            "System.RunTime",
            "mscorlib",
            "System",
            "System.Core",
            "System.Data",
            "System.Data.Common",
            "System.Drawing",
            "System.Text.RegularExpressions",
            "System.Linq",
            "System.Linq.Expressions",
            "System.Private.CoreLib",
            "System.Reflection",
            "System.Collections",
            "System.IO.Compression.FileSystem",
            "System.Numerics",
            "System.RunTime.Serialization",
            "System.Xml",
            "System.Xml.Linq"
        };

        #endregion Ensamblados del syntax

        #region Inicialización del syntax editor

        /// <summary>
        /// Instancia principal del editor de codigo para la edición de las pruebas
        /// </summary>
        [SupportedOSPlatform("windows")]
        private woSyntaxEditor _woSyntaxEditor;

        /// <summary>
        /// Inicialización del editor de codigo
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeSyntaxEditor(ISyntaxLanguage syntaxLanguage)
        {
            try
            {
                // Re instan ciamos el syntax
                if (_woSyntaxEditor != null)
                {
                    pnlSyntaxEditor.Controls.Clear();

                    _woSyntaxEditor = null;
                }

                List<string> refCol = new List<string>();

                if (syntaxLanguage.GetType() == typeof(CSharpSyntaxLanguage))
                {
                    string basePaht = Path.GetDirectoryName(
                        typeof(object).GetTypeInfo().Assembly.Location
                    );

                    foreach (string reference in _references)
                    {
                        string pathRef = $"{basePaht}\\{reference}.dll";
                        if (refCol.Contains(pathRef))
                        {
                            refCol.Add(pathRef);
                        }
                    }

                    foreach (string reference in _systemReferences)
                    {
                        string pathRef = $"{basePaht}\\{reference}.dll";
                        if (refCol.Contains(pathRef))
                        {
                            refCol.Add(pathRef);
                        }
                    }

                    //foreach (string reference in _references)
                    //{
                    //    string pathRef = $"{basePaht}\\{reference}";
                    //    if (refCol.Contains(pathRef))
                    //    {
                    //        refCol.Add(pathRef);
                    //    }
                    //}

                    //foreach (string reference in _systemReferencesApp)
                    //{
                    //    string pathRef = $"{basePaht}\\{reference}";
                    //    if (refCol.Contains(pathRef))
                    //    {
                    //        refCol.Add(pathRef);
                    //    }
                    //}
                }

                _woSyntaxEditor = new woSyntaxEditor(syntaxLanguage, dlls: refCol);

                // Agregamos y posicionamos el syntax
                pnlSyntaxEditor.Controls.Add(_woSyntaxEditor);
                _woSyntaxEditor.Dock = DockStyle.Fill;

                // Actualización del ribbon con los items de este componente
                UpdateRibbon();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al intentar iniciar el syntax editor. {ex.Message}");
            }
        }

        #endregion Inicialización del syntax editor


        #region Ejecución de la prueba JS

        /// <summary>
        /// Instancia que permite la ejecución del proyecto server de blazor.
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Instancia que permite la ejecución del proyecto wasm de blazor.
        /// </summary>
        private WoBlazorWasmExecute _woBlazorWasmExecute = WoBlazorWasmExecute.GetInstance();

        /// <summary>
        /// Elimina el fichero
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void ExecuteJs(string focusedPath, string projectType)
        {
            try
            {
                if (File.Exists(focusedPath))
                {
                    string rawCode = WoDirectory.ReadFile(focusedPath);
                    string[] rawCodeCol = rawCode.Split("//");

                    List<string> modelsCol = new List<string>();

                    foreach (string line in rawCodeCol)
                    {
                        if (line.Contains("/Modelos:"))
                        {
                            modelsCol = line.Replace("/Modelos:", "")
                                .Replace("\r", "")
                                .Replace("\n", "")
                                .Replace(" ", "")
                                .Split(",")
                                .ToList();

                            break;
                        }
                    }

                    foreach (string modelName in modelsCol)
                    {
                        BuildAbstractTestModels(modelName);
                    }

                    tabInfo.SelectedTabPage = tabConsole;

                    ValidateJs(focusedPath);

                    BuildIntegralProject(projectType, focusedPath);

                    RunBlazor(baseProjectName: "IntegralTest", proyectType: projectType);
                }
                else
                {
                    throw new Exception("El fichero no existe");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al ejecutar el caso prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Elimina los modelos anteriores y reconstruye modelos nuevos
        /// con las ultimas versiones
        /// </summary>
        /// <param name="modelName"></param>
        /// <exception cref="Exception"></exception>
        private void BuildAbstractTestModels(string modelName)
        {
            try
            {
                string commonPath = $"{proyecto.DirProyectData}\\Test\\TestCafe\\Common";
                if (Directory.Exists(commonPath))
                {
                    WoDirectory.DeleteDirectory(commonPath);
                }

                WoDirectory.CreateDirectory(commonPath);
                Modelo findModel = proyecto.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == modelName
                );
                if (findModel != null)
                {
                    ModelGenerator modelGenerator = new ModelGenerator();
                    modelGenerator.BuildModelClass(findModel);

                    FormModelGenerator formModelGenerator = new FormModelGenerator();
                    formModelGenerator.BuildFormModelClass(findModel);
                }
                else
                {
                    throw new Exception($@"El modelo {modelName} no existe.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al construir los modelos nuevos de js. {ex.Message}");
            }
        }

        /// <summary>
        /// Ejecuta el blazor para el test
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private async void RunBlazor(string baseProjectName, string proyectType)
        {
            try
            {
                WoBlazorExecuteHelper.StopExtraBlazorExecute();

                if (_woBlazorServerExecute.IsRuning() || _woBlazorWasmExecute.IsRuning())
                {
                    if (_woBlazorServerExecute.IsRuning())
                    {
                        _woBlazorServerExecute.GeneralStop();

                        _woConsole.SendData($"Success: Proyecto detenido con éxito\n\r");
                    }
                    if (_woBlazorWasmExecute.IsRuning())
                    {
                        _woBlazorWasmExecute.GeneralStop();

                        _woConsole.SendData($"Success: Proyecto detenido con éxito\n\r");
                    }
                }

                if (proyectType == "Wasm")
                {
                    _woBlazorWasmExecute.SendToConsoleEvt += _woConsole.SendData;
                    _woBlazorServerExecute.BlazorRuningEvt += RunTest;
                    await _woBlazorWasmExecute.StartSimpleWasm($@"{baseProjectName}Wasm");
                }
                else if (proyectType == "Server")
                {
                    _woBlazorServerExecute.SendToConsoleEvt += _woConsole.SendData;
                    _woBlazorServerExecute.BlazorRuningEvt += RunTest;
                    await _woBlazorServerExecute.StartSimpleServer($@"{baseProjectName}Server");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la ejecución del blazor. {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        public void RunTest()
        {
            try
            {
                WoTestExecute woTestExecute = new WoTestExecute();
                woTestExecute.ExecuteTest(_woSyntaxEditor.GetFilePath());
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        #endregion Ejecución de la prueba JS

        #region Validaciones para la ejecución de js

        /// <summary>
        /// Valida que sea posible la ejecución de codigo js con testCafe
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ValidateJs(string focusedPath)
        {
            try
            {
                WoTestFormValidator woTestFormValidator = new WoTestFormValidator();
                woTestFormValidator.ValidateEjecution();

                string rawTestCode = WoDirectory.ReadFile(focusedPath);

                if (rawTestCode != string.Empty)
                {
                    string[] rawCodeCol = rawTestCode.Split("///");
                    string models = rawCodeCol[2]
                        .Replace("Modelos: ", "")
                        .Replace("\n", "")
                        .Replace("\r", "");

                    if (models.Contains("__") || models == string.Empty)
                    {
                        StringBuilder strErrorTestData = new StringBuilder();

                        strErrorTestData.AppendLine(
                            "El fichero tiene un error en la estructura del test."
                        );
                        strErrorTestData.AppendLine(
                            "Revise el comentario en el header e indique la lista de modelos "
                        );
                        strErrorTestData.AppendLine("involucrados en la prueba.");
                        strErrorTestData.AppendLine("Remplace: ");
                        strErrorTestData.AppendLine("///Modelos: __NombreDelModelo_");
                        strErrorTestData.AppendLine(
                            "Por la lista de los modelos que se utilizan, ejemplo: Banco, Usuario, ..."
                        );

                        throw new Exception(strErrorTestData.ToString());
                    }
                    else
                    {
                        List<WoModelSelected> modelsForTest = new List<WoModelSelected>();

                        string[] modelsCol = models.Split(",");
                        foreach (string modelName in modelsCol)
                        {
                            IEnumerable<Modelo> findModels = proyecto.ModeloCol.Modelos.Where(m =>
                                m.Id == modelName.Replace(" ", "")
                            );

                            if (findModels != null && findModels.Count() > 0)
                            {
                                Modelo model = findModels.First();

                                ModelGenerator modelGenerator = new ModelGenerator();
                                modelGenerator.BuildModelClass(model);

                                FormModelGenerator formModelGenerator = new FormModelGenerator();
                                formModelGenerator.BuildFormModelClass(model);

                                modelsForTest.Add(
                                    new WoModelSelected()
                                    {
                                        ModelName = modelName.Replace(" ", ""),
                                        ModelType = eGenerationType.FormList,
                                    }
                                );
                            }
                            else
                            {
                                throw new Exception(
                                    $"El modelo {modelName} no existe en el proyecto"
                                );
                            }
                        }

                        string json = JsonConvert.SerializeObject(modelsForTest);
                        string dataTestPath =
                            $"{proyecto.DirProyectData}\\Test\\TestCafe\\IntegralTest.json";

                        WoDirectory.WriteFile(dataTestPath, json);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la validación de la ejecución de js. {ex.Message}");
            }
        }

        #endregion Validaciones para la ejecución de js

        #region Creación de la integral

        /// <summary>
        /// Genera o edita el proyecto de blazor para la ejecución de la prueba
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void BuildIntegralProject(string projectType, string focusedPath)
        {
            try
            {
                string testPath = Path.GetFileNameWithoutExtension(focusedPath);

                WoTestIntegralProjectManager woTestIntegralProjectManager =
                    new WoTestIntegralProjectManager(projectType);
                woTestIntegralProjectManager.SendDataToConsoleEvt += _woConsole.SendData;
                string url = woTestIntegralProjectManager.GenerateUpdateBlazor();

                string rawCode = WoDirectory.ReadFile(focusedPath);
                if (rawCode.Contains("URLDELPROYECTO_NO_MODIFICAR"))
                {
                    rawCode = rawCode.Replace("URLDELPROYECTO_NO_MODIFICAR", url);
                }
                else
                {
                    string[] rawCodeCol = rawCode.Split($"fixture `{testPath}`.page");
                    string[] rawCodeUrlCol = rawCodeCol[1].Split(";");
                    string oldUrl = rawCodeUrlCol[0].Replace("`", "");
                    if (oldUrl.ToLower().Contains("https://"))
                    {
                        rawCode = rawCode.Replace(oldUrl, url);
                    }
                    else
                    {
                        throw new Exception("Error en la url anterior.");
                    }
                }

                File.WriteAllText(focusedPath, rawCode);

                FocusChange(focusedPath);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la generación de la integral. {ex.Message}");
            }
        }

        #endregion Creación de la integral

        #region Gestión de la integral

        [SupportedOSPlatform("windows")]
        private void StopBlazor()
        {
            try { }
            catch (Exception ex)
            {
                throw new Exception($@"Error al detener el blazor");
            }
        }

        #endregion Gestión de la integral


        #region Detener el provecto de blazor

        /// <summary>
        /// Método del botón para detener el blazor
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
                if (_woBlazorServerExecute.IsRuning() || _woBlazorWasmExecute.IsRuning())
                {
                    if (_woBlazorServerExecute.IsRuning())
                    {
                        _woBlazorServerExecute.GeneralStop();

                        _woConsole.SendData($"Success: Proyecto detenido con éxito\n\r");

                        _woBlazorServerExecute.BlazorRuningEvt = null;
                        _woBlazorServerExecute.SendToConsoleEvt = null;
                    }
                    if (_woBlazorWasmExecute.IsRuning())
                    {
                        _woBlazorWasmExecute.GeneralStop();

                        _woConsole.SendData($"Success: Proyecto detenido con éxito\n\r");

                        _woBlazorWasmExecute.SendToConsoleEvt = null;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al detener el blazor. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Detener el provecto de blazor


        #region Manejo de la ejecución de la prueba de CS

        private void btnExecute_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        ) { }

        /// <summary>
        /// Ejecuta el fichero de CS
        /// </summary>
        /// <param name="focusedPath"></param>
        [SupportedOSPlatform("windows")]
        private void ExecuteCs(string focusedPath)
        {
            try
            {
                if (File.Exists(focusedPath))
                {
                    ExecuteTestCs(focusedPath);
                }
                else
                {
                    throw new Exception("El fichero no existe");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al ejecutar el caso prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Ejecuta las pruebas de servicio
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ExecuteTestCs(string testPath)
        {
            try
            {
                tabInfo.SelectedTabPage = tabConsole;

                // Borrado de la BD, Items[1] = Borrar DB Y Ejecutar
                if (cmbTestAction.EditValue.ToString() == cmbTestActionControl.Items[1].ToString())
                {
                    DeleteDB();
                }

                // Recuperar BD, Items[2] = Recuperar DB y Ejecutar
                if (cmbTestAction.EditValue.ToString() == cmbTestActionControl.Items[2].ToString())
                {
                    if (cmbTestGetDb.EditValue.IsNullOrStringEmpty())
                    {
                        throw new Exception("Seleccione una base de datos a recuperar");
                    }

                    string dataBase = WoLib.GetDBName(
                        proyecto.ParConexion.Instance,
                        proyecto.ParConexion.InstanceType,
                        proyecto.ParConexion.Year
                    );

                    RestoreDB(dataBase);
                }

                // Determinamos el tipo de ejecución Ensamblado interno o aplicación
                // Items[0] = Ensamblado Interno
                if (
                    cmbExecuteMode.EditValue.ToString() == cmbExecuteModeControl.Items[0].ToString()
                )
                {
                    string testName = Path.GetFileNameWithoutExtension(testPath);

                    if (testName.EndsWith(".Hybrid"))
                    {
                        throw new Exception(
                            "Las pruebas híbridas para apps, solo se pueden ejecutar como 'Ejecutable Externo'"
                        );
                    }

                    try
                    {
                        ServiceTestExecute serviceTestExecute = new ServiceTestExecute(
                            systemRefCol: _systemReferences,
                            baseRefCol: _references
                        );

                        serviceTestExecute.SetErrorEvt += SetErrors;

                        serviceTestExecute.EjecuteInternalAssembly(testPath);
                    }
                    catch (Exception ex)
                    {
                        // Send Error to COnsole
                    }
                }
                // Items[1] = Ejecutable Externo
                else if (
                    cmbExecuteMode.EditValue.ToString() == cmbExecuteModeControl.Items[1].ToString()
                )
                {
                    //fads
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al ejecutar el caso prueba. {ex.Message}");
            }
        }

        #endregion Manejo de la ejecución de la prueba de CS

        #region Ejecución como ensamblado interno

        /// <summary>
        /// Ejecución de la prueba desde n ensamblado interno
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ExecuteInternalAssembly(string focusedTestPath)
        {
            try
            {
                proyecto.AssemblyModelCargado = true;

                Cursor.Current = Cursors.WaitCursor;

                lstErrors.Items.Clear();

                string rawCodeTest = WoDirectory.ReadFile(focusedTestPath);
                List<ScriptErrorDescriptor> errorsCol;
                //Assembly asembly = syntac
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al ejecutar como ensamblado interno. {ex.Message}");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        #endregion Ejecución como ensamblado interno


        #region Inicialización de los componentes del ribbon

        /// <summary>
        /// Inicializa los componentes principales del ribbon
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeComponents()
        {
            try
            {
                // Configuramos los combos para evitar la edición directa y pre asignar valores default
                DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbTestActionsControl =
                    cmbTestAction.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                cmbTestActionsControl.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;

                DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbTestGetDbControl =
                    cmbTestGetDb.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                cmbTestGetDbControl.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;

                cmbTestAction.EditValue = proyecto.ParConexion.DBAccion;

                DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbTestActionsControlApp =
                    cmbTestActionApp.Edit
                    as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                cmbTestActionsControlApp.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;

                DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbTestGetDbControlApp =
                    cmbTestGetDbApp.Edit
                    as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                cmbTestGetDbControlApp.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;

                cmbTestActionApp.EditValue = proyecto.ParConexion.DBAccionApp;

                DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbExecuteModeControl =
                    cmbExecuteMode.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                cmbExecuteModeControl.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;

                cmbExecuteMode.EditValue = cmbExecuteModeControl.Items[0];

                // Llenamos las opciones del combo de Get Db
                string db = WoLib.GetDBName(
                    proyecto.ParConexion.Instance,
                    proyecto.ParConexion.InstanceType,
                    proyecto.ParConexion.Year
                );

                string dbResult = ExecuteOperationDB(
                    dbOperation: new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.ListBackup,
                        DataBaseName = db,
                    }
                );

                cmbTestGetDbControl.Items.Clear();

                cmbTestGetDbControl.Items.AddRange(
                    dbResult.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                );
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al iniciar los componentes del ribbon. {ex.Message}");
            }
        }

        /// <summary>
        /// Recupera información para la bd
        /// </summary>
        /// <param name="dbOperation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private string ExecuteOperationDB(WoDataBaseOperations dbOperation)
        {
            try
            {
                Auth();
                return _woTarget.Send<string>(dbOperation);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al ejecutar la operación en la BD. {ex.Message}");
            }
        }

        #endregion Inicialización de los componentes del ribbon


        #region Autenticación

        /// <summary>
        /// Cliente para realizar peticiones e información a los usuarios
        /// </summary>
        private JsonApiClient _woTarget = null;

        /// <summary>
        /// Botón de log out y log in automático
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnLogOutLogIn_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                Auth();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error re autenticar. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Autenticación del cliente WoTraget
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void Auth()
        {
            try
            {
                if (_woTarget != null)
                {
                    _woTarget = null;
                }

                WoDataService woDataService = new WoDataService();

                _woTarget = woDataService.GetAuthClient(
                    userName: proyecto.ParConexion.Usuario,
                    password: proyecto.ParConexion.Password,
                    instance: proyecto.ParConexion.Instance,
                    udn: proyecto.ParConexion.Udn,
                    year: proyecto.ParConexion.Year,
                    instanceType: proyecto.ParConexion.InstanceType
                );
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al autenticar. {ex.Message}");
            }
        }

        #endregion Autenticación


        #region Abrir DB en app externa

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ExploreInstanceDB_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                if (proyecto.ParConexion.InstanceType == tWoIntanciaType.DEV)
                {
                    string filePath =
                        $"{proyecto.DirApplication_WebService_WooWServer_DirDBService}\\{proyecto.ParConexion.DbName()}";
                    if (!File.Exists(filePath))
                    {
                        throw new Exception($@"La base de datos en {filePath}, no existe");
                    }

                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true };
                    process.Start();
                }
                else if (
                    proyecto.ParConexion.InstanceType == tWoIntanciaType.PRO
                    || proyecto.ParConexion.InstanceType == tWoIntanciaType.QAS
                )
                {
                    //TODO: Validar cual es el tipo de bd que se esta utilizando
                    string sqlServerManagmentPath =
                        "C:\\Program Files (x86)\\Microsoft SQL Server Management Studio 20\\Common7\\IDE\\Ssms.exe";
                    if (!File.Exists(sqlServerManagmentPath))
                    {
                        throw new Exception(
                            $@"No se encuentra management studio en el path. {sqlServerManagmentPath}"
                        );
                    }
                    else
                    {
                        Process process = new Process();
                        process.StartInfo = new ProcessStartInfo(sqlServerManagmentPath)
                        {
                            UseShellExecute = true
                        };
                        process.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al intentar abrir la instancia de la DB. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Abrir DB en app externa

        #region Delete DB

        /// <summary>
        /// Codigo del botón de administración de la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteInstanceDB_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                DialogResult messageResult = XtraMessageBox.Show(
                    text: $@"¿Seguro que desea eliminar la base de datos?",
                    caption: "Alerta",
                    icon: MessageBoxIcon.Question,
                    buttons: MessageBoxButtons.YesNo
                );

                if (messageResult == DialogResult.Yes)
                {
                    string deleteResult = DeleteDB();

                    XtraMessageBox.Show(
                        text: deleteResult,
                        caption: "Info",
                        icon: MessageBoxIcon.Information,
                        buttons: MessageBoxButtons.OK
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"{ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Elimina la base de datos
        /// </summary>
        [SupportedOSPlatform("windows")]
        private string DeleteDB()
        {
            try
            {
                string db = WoLib.GetDBName(
                    proyecto.ParConexion.Instance,
                    proyecto.ParConexion.InstanceType,
                    proyecto.ParConexion.Year
                );

                string dbResult = ExecuteOperationDB(
                    dbOperation: new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.Drop,
                        DataBaseName = db,
                    }
                );

                _woTarget = null;

                return dbResult;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al eliminar la base de datos. {ex.Message}");
            }
        }

        #endregion Delete DB

        #region Respaldar DB

        /// <summary>
        /// Realiza un respaldo de la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnBackUpInstanceDB_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            try
            {
                var fmText = new fmNormalizedText();
                fmText.Text = "Nombre del Backup";

                if (fmText.ShowDialog() == DialogResult.OK)
                {
                    string DataBase = WoLib.GetDBName(
                        proyecto.ParConexion.Instance,
                        proyecto.ParConexion.InstanceType,
                        proyecto.ParConexion.Year
                    );

                    string dbResult = ExecuteOperationDB(
                        dbOperation: new WoDataBaseOperations()
                        {
                            Accion = WooW.Core.Common.tWoDataBaseAccion.Backup,
                            DataBaseName = DataBase,
                            BackupName = fmText.NormalizedText
                        }
                    );

                    _woTarget = null;

                    XtraMessageBox.Show(
                        text: dbResult,
                        caption: "Info",
                        icon: MessageBoxIcon.Information,
                        buttons: MessageBoxButtons.OK
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al ejecutar el caso prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Respaldar DB

        #region Restaurar DB

        /// <summary>
        /// Restauramos un backup de la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnRestoreDB_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string dataBase = WoLib.GetDBName(
                    proyecto.ParConexion.Instance,
                    proyecto.ParConexion.InstanceType,
                    proyecto.ParConexion.Year
                );

                var fmText = new fmComboSelect();
                fmText.Text = "Nombre del Backup";
                string result = ExecuteOperationDB(
                    new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.ListBackup,
                        DataBaseName = dataBase,
                    }
                );

                fmText.Items = result.Split(
                    new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries
                );

                if (fmText.ShowDialog() == DialogResult.OK)
                {
                    result = RestoreDB(dataBase, fmText.SelectedItem);

                    _woTarget = null;

                    XtraMessageBox.Show(
                        result,
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al restaurar la BD. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Restauración de la base de datos por el back up seleccionado
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private string RestoreDB(string dataBase, string backUpName = null)
        {
            try
            {
                string backUp = backUpName ?? cmbTestGetDb.EditValue.ToString();

                string resultRestore = ExecuteOperationDB(
                    new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.Restore,
                        DataBaseName = dataBase,
                        BackupName = backUp
                    }
                );

                if (resultRestore.StartsWith("Error"))
                {
                    throw new Exception(resultRestore);
                }

                return resultRestore;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al restaurar la db. {ex.Message}");
            }
        }

        #endregion Restaurar DB


        #region Gestión de la lista de errores

        /// <summary>
        /// Envía la lista de errores al list view de la pantalla
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SetErrors(List<ScriptErrorDescriptor> listErrorsCol)
        {
            try
            {
                if (listErrorsCol == null)
                {
                    return;
                }

                lstErrors.Items.Clear();
                int errorCount = 1;

                foreach (var item in listErrorsCol)
                {
                    ListViewItem itemView = new ListViewItem(errorCount.ToString());
                    itemView.SubItems.Add(item.Code);
                    itemView.SubItems.Add(item.Description);
                    itemView.SubItems.Add((item.Line == -1) ? "-" : item.Line.ToString());
                    itemView.SubItems.Add(item.Column.ToString());

                    lstErrors.Items.Add(itemView);
                    errorCount++;
                }

                tabInfo.SelectedTabPage = tabErrors;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al asignar la lista de errores. {ex.Message}");
            }
        }

        #endregion Gestión de la lista de errores
    }
}
