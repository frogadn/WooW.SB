using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Newtonsoft.Json;
using WooW.SB.BlazorGenerator;
using WooW.SB.BlazorGenerator.BlazorDialogs.BlazorDialogsModels;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.TestTools
{
    public class WoTestIntegralProjectManager
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

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
        /// Tipo del proyecto sobre el que se esta trabajando (Server o Wasm).
        /// </summary>
        private string _proyectType = "Server";

        /// <summary>
        /// Instancia del generador de los proyectos de blazor
        /// </summary>
        private WoBlazorGenerator _woBlazorGenerator = new WoBlazorGenerator();

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        /// <param name="projectType"></param>
        public WoTestIntegralProjectManager(string projectType)
        {
            _proyectType = projectType;
            _selectedProjectName = $"IntegralTest{projectType}";
            _selectedProjectPath = $"{_project.DirProyectTemp}\\{_selectedProjectName}";
            _selectedDataProject = $"{_project.DirProyectData}\\Test\\TestCafe\\IntegralTest.json";
        }

        #endregion Constructor


        #region Método principal

        /// <summary>
        /// Orquesta la generación y ejecución del proyecto integral para pruebas
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public string GenerateUpdateBlazor()
        {
            try
            {
                BuildTestBlazorProyect();

                return GetUrlBlazorGeneration();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al ejecutar el blazor {_selectedProjectName}{_proyectType}. {ex.Message}"
                );
            }
        }

        #endregion Método principal

        #region Recuperación de las listas de modelos

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

        #endregion Recuperación de las listas de modelos

        #region Generación y actualización del proyecto

        /// <summary>
        /// Realiza la generación del proyecto de blazor para la prueba
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void BuildTestBlazorProyect()
        {
            try
            {
                _woBlazorGenerator.SendToConsole += SendDataToConsole;

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
                        _woBlazorGenerator.GenerateBaseWasm(_selectedProjectName);
                    }
                    else if (_proyectType == "Server")
                    {
                        _woBlazorGenerator.GenerateBaseServer(_selectedProjectName);
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

                _woBlazorGenerator.GenerateProyectForms(
                    projectName: $@"{_selectedProjectName}",
                    selectedModels: lists.forms,
                    selectedReports: lists.reports,
                    selectedLists: lists.lists,
                    selectedODataReports: lists.oDataReports,
                    isServer: (_proyectType == "Server")
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error en la generación del blazor para el test. {ex.Message}"
                );
            }
        }

        #endregion Generación y actualización del proyecto

        #region Recuperación de la url de la generación

        /// <summary>
        /// Actualiza la url de la prueba
        /// </summary>
        public string GetUrlBlazorGeneration()
        {
            try
            {
                string testPath = string.Empty;

                if (
                    File.Exists($"{_project.DirProyectTemp}\\IntegralTestServer\\Program.cs")
                    && _proyectType == "Server"
                )
                {
                    testPath = $"{_project.DirProyectTemp}\\IntegralTestServer";
                }
                else if (
                    File.Exists($"{_project.DirProyectTemp}\\IntegralTestWasm\\Program.cs")
                    && _proyectType == "Wasm"
                )
                {
                    testPath = $"{_project.DirProyectTemp}\\IntegralTestWasm";
                }

                string url = "URLDELPROYECTO_NO_MODIFICAR";

                if (testPath != string.Empty)
                {
                    string launchSettingsPath = $"{testPath}\\Properties\\launchSettings.json";
                    string lauchSettings = WoDirectory.ReadFile(launchSettingsPath);
                    string[] launchSettingsCol = lauchSettings.Split("profiles");
                    string[] launchSettingsCol1 = launchSettingsCol[1]
                        .Split($@"""applicationUrl"":");
                    string[] launchSettingsCol2 = launchSettingsCol1[1].Split(";");
                    url = launchSettingsCol2[0].Replace("\"", "").Replace(" ", "");

                    return url;
                }
                else
                {
                    throw new Exception("Error al generar blazor");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al intentar recuperar la url del proyecto de blazor a ejecutar. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de la url de la generación

        #region Envió de información a la consola

        /// <summary>
        /// Action que se detona cuando se envía información para la consola
        /// </summary>
        public Action<string> SendDataToConsoleEvt;

        /// <summary>
        /// Envió de la información a la consola a través de la consola
        /// </summary>
        private void SendDataToConsole(string data)
        {
            try
            {
                SendDataToConsoleEvt?.Invoke(data);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al enviar información a la consola desde el action. {ex.Message}"
                );
            }
        }

        #endregion Envió de información a la consola
    }
}
