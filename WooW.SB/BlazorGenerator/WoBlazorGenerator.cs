using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorGeneration.BlazorForm;
using WooW.SB.BlazorGenerator.BlazorGeneration.BlazorForms;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorProjects;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Pages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Menus.MenuModels;

namespace WooW.SB.BlazorGenerator
{
    public class WoBlazorGenerator
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto.
        /// Para enviar alertas e información al log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Permite recuperar información de los modelos y modelos mismos del proyecto.
        /// </summary>
        private WoToolModelsHelper _woToolModelsHelper = new WoToolModelsHelper();

        #endregion Atributos


        #region Generadores base del proyecto

        /// <summary>
        /// Controlador de eventos que envía la data a la consola.
        /// </summary>
        public Action<string> SendToConsole { get; set; }

        /// <summary>
        /// Código de dirección para la untaría.
        /// </summary>
        private string _unitCodeIndex =
            $@"
            protected override async Task OnAfterRenderAsync(bool firstRender)
            {{
                await base.OnAfterRenderAsync(firstRender);

                if (NavigationManager != null)
                {{
                    NavigationManager.NavigateTo(""/Index"");
                }}
            }}";

        /// <summary>
        /// Genera el proyecto base de blazor server side, que funciona como base para el resto de
        /// componentes que se generaran.
        /// </summary>
        /// <param name="projectName"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateBaseServer(string projectName, bool isUnit = false)
        {
            WoBlazorProjectServer woBlazorProjectServer = new WoBlazorProjectServer(projectName);
            woBlazorProjectServer.SendToConsole += (string data) =>
            {
                SendToConsole?.Invoke(data);
            };

            if (isUnit)
            {
                woBlazorProjectServer.CreateProject(_unitCodeIndex);
            }
            else
            {
                woBlazorProjectServer.CreateProject(string.Empty);
            }
        }

        /// <summary>
        /// Genera el proyecto base para una generación de tipo web assembly.
        /// </summary>
        /// <param name="projectName"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateBaseWasm(string projectName, bool isUnit = false)
        {
            WoBlazorProjectWasm woBlazorProjectWasm = new WoBlazorProjectWasm(
                projectName: projectName
            );
            woBlazorProjectWasm.SendToConsole += (string data) =>
            {
                SendToConsole?.Invoke(data);
            };

            if (isUnit)
            {
                woBlazorProjectWasm.CreateProject(_unitCodeIndex);
            }
            else
            {
                woBlazorProjectWasm.CreateProject(string.Empty);
            }
        }

        #endregion Generadores base del proyecto


        #region Generar layouts

        /// <summary>
        /// Genera el proyecto base de blazor para los formularios.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="selectedModels"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateProyectForms(
            string projectName,
            List<string> selectedModels,
            List<string> selectedReports,
            List<string> selectedLists,
            List<string> selectedODataReports,
            bool isServer
        )
        {
            WoBlazorForms woBlazorForms = new WoBlazorForms(projectName);
            string extraUsings = string.Empty;

            string model = _project.ModeloCol.Modelos.First().Id;

            BuildExtraBase(projectName, model, "1");

            if (selectedModels.Count > 0)
            {
                woBlazorForms.BuildForms(selectedModels, isReport: false);
                extraUsings += $@"using {projectName}.TransitionSettings;";
            }

            if (selectedReports.Count > 0)
            {
                woBlazorForms.BuildForms(selectedReports, isReport: true);
                extraUsings += $@"using {projectName}.Reports;";
                extraUsings += $@"using {projectName}.ReportForms;";
            }

            if (selectedLists.Count > 0)
            {
                woBlazorForms.GenerateLists(selectedLists);
            }

            if (selectedODataReports.Count > 0)
            {
                foreach (string oDataReport in selectedODataReports)
                {
                    woBlazorForms.GenerateOdataReports(oDataReport);

                    if (!extraUsings.Contains("ReportForms"))
                    {
                        extraUsings += $@"using {projectName}.Reports;";
                        extraUsings += $@"using {projectName}.ReportForms;";
                    }
                }
            }

            woBlazorForms.BuildProgramFiles(
                isServer: isServer,
                models: selectedModels,
                reports: selectedReports,
                lists: selectedLists,
                oDataReports: selectedODataReports,
                extraUsings
            );
        }

        #endregion Generar layouts


        #region Generar layout unitaria

        /// <summary>
        /// Contiene los using custom que pueden ir o no ir en el imports del blazor.
        /// </summary>
        private string _customUsings = string.Empty;

        /// <summary>
        /// Permite que el usuario pueda generar un formulario como prueba unitaria.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="classModelName"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateUnitLayout(
            WoContainer container,
            string classModelName,
            bool isServer,
            bool isReport = false
        )
        {
            string projectName = $@"{classModelName}_proj";
            _customUsings = string.Empty;

            if (
                File.Exists($"{_project.DirApplication}\\Temp\\{projectName}\\{projectName}.csproj")
            )
            {
                WoContainer editContainer = GenerateSlaves(container, projectName);

                BuildExtraBase(projectName, editContainer.ModelId, "1");

                if (isReport)
                {
                    WoContainer editContainerWithReportForm = GenerateReports(
                        editContainer,
                        projectName
                    );

                    editContainerWithReportForm.IsUnit = true;

                    WoBlazorForm woBlazorForm = new WoBlazorForm(
                        woContainer: editContainerWithReportForm,
                        projectName: projectName,
                        folderName: "Reports",
                        classModelName: classModelName
                    );

                    woBlazorForm.BuildCommonForm(_customUsings, true);

                    woBlazorForm.BuildProgramFiles(
                        isServer: isServer,
                        isUnit: true,
                        slaves: _slaves,
                        isReport: isReport
                    );
                }
                else
                {
                    editContainer.IsUnit = true;

                    WoBlazorForm woBlazorForm = new WoBlazorForm(
                        woContainer: editContainer,
                        projectName: projectName,
                        folderName: "Pages",
                        classModelName: classModelName
                    );
                    woBlazorForm.BuildCommonForm(_customUsings);

                    GenerateList(
                        modelName: editContainer.ModelId,
                        classModelName: classModelName,
                        isUnit: editContainer.IsUnit
                    );

                    woBlazorForm.BuildProgramFiles(
                        isServer: isServer,
                        isUnit: true,
                        slaves: _slaves,
                        isReport: isReport
                    );
                }
            }
            else
            {
                XtraMessageBox.Show(
                    "Realiza primero la generación de la base para poder realizar este proceso",
                    "Alerta",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }
        }

        private void BuildExtraBase(string projectName, string model, string id)
        {
            WoTemplateLogPartialBlazor woTemplateLogPartialBlazor =
                new WoTemplateLogPartialBlazor();
            woTemplateLogPartialBlazor.Project = projectName;
            woTemplateLogPartialBlazor.Model = model;
            woTemplateLogPartialBlazor.Id = id;
            WoDirectory.WriteTemplate(
                pathTemplate: $@"{_project.DirApplication}/Temp/{projectName}/Pages/WoLog.razor.cs",
                data: woTemplateLogPartialBlazor.TransformText()
            );
        }

        #endregion Generar layout unitaria

        #region Generación de list

        /// <summary>
        /// Genera el listado de la unidad.
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="classModelName"></param>
        /// <param name="isUnit"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateList(string modelName, string classModelName, bool isUnit)
        {
            string projectName = $@"{classModelName}_proj";
            _customUsings += $@"@using {projectName}.GridLists";

            WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
            WoContainer listRawContainer = woDesignerRawListHelper.BuildRawListForm(modelName);

            if (isUnit)
            {
                listRawContainer.IsUnit = true;
            }

            WoBlazorForm woBlazorForm = new WoBlazorForm(
                woContainer: listRawContainer,
                projectName: projectName,
                folderName: "GridLists",
                classModelName: $@"{classModelName}GridList"
            );

            woBlazorForm.BuildCommonForm(customUsing: _customUsings, isList: true);
        }

        #endregion Generación de list

        #region Generación de reports

        /// <summary>
        /// Recupera todos los reportes del modelo y las va generando una a una de la misma forma que los formularios.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="projectName"></param>
        [SupportedOSPlatform("windows")]
        public WoContainer GenerateReports(WoContainer container, string projectName)
        {
            WoBlazorAnalize woBlazorAnalize = new WoBlazorAnalize();

            List<string> reportModelNames = woBlazorAnalize.GetReportNames(container);

            if (reportModelNames.Count > 0)
            {
                container = woBlazorAnalize.GetContainerWithReportNames(container);
                _customUsings += $@"@using {projectName}.ReportForms";

                int reportCount = 0;
                foreach (string reportModelName in reportModelNames)
                {
                    string pathReport =
                        $@"{_project.DirProyectData}\LayOuts\FormDesign\{reportModelName}.json";
                    WoContainer reportContainer = new WoContainer();
                    if (File.Exists(pathReport))
                    {
                        string json = File.ReadAllText(pathReport);
                        reportContainer = JsonConvert.DeserializeObject<WoContainer>(json);
                    }
                    else
                    {
                        WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                            new WoDesignerRawSerializerHelper();

                        reportContainer = woDesignerRawSerializerHelper.BuildRawWoContainer(
                            reportModelName
                        );
                    }

                    WoBlazorForm woBlazorreport = new WoBlazorForm(
                        woContainer: reportContainer,
                        projectName: projectName,
                        folderName: "ReportForms",
                        classModelName: $@"Report{reportCount}Layout"
                    );

                    woBlazorreport.BuildSmallForm();
                    reportCount++;
                }
            }
            else
            {
                string pathProject = $@"{_project.DirProyectTemp}/{projectName}";

                if (Directory.Exists($@"{pathProject}/ReportForms"))
                {
                    List<string> reportsFilesCol = WoDirectory.ReadDirectoryFiles(
                        $@"{pathProject}/ReportForms"
                    );
                    foreach (string reportFile in reportsFilesCol)
                    {
                        WoDirectory.DeleteFile(reportFile);
                    }
                }

                List<string> pathsControlsCol = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/ControlModels"
                );
                foreach (string pathControl in pathsControlsCol)
                {
                    if (pathControl.Contains("Report"))
                    {
                        WoDirectory.DeleteFile(pathControl);
                    }
                }

                List<string> pathsUserCodeCol = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/UserCode"
                );
                foreach (string pathUserCode in pathsUserCodeCol)
                {
                    if (pathUserCode.Contains("Report"))
                    {
                        WoDirectory.DeleteFile(pathUserCode);
                    }
                }

                List<string> pathFluentCol = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/FluentValidators"
                );
                foreach (string pathFluent in pathFluentCol)
                {
                    if (pathFluent.Contains("Slave"))
                    {
                        WoDirectory.DeleteFile(pathFluent);
                    }
                }

                List<string> pathsTransitionSettings = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/TransitionSettings"
                );
                foreach (string pathTransitionSettings in pathsTransitionSettings)
                {
                    if (pathTransitionSettings.Contains("Slave"))
                    {
                        WoDirectory.DeleteFile(pathTransitionSettings);
                    }
                }
            }

            return container;
        }

        #endregion Generación de reports

        #region Generación de reports odata

        [SupportedOSPlatform("windows")]
        public void GenerateODataReport(
            string modelName,
            string classModelName,
            bool isServer,
            string labelId = "Root"
        )
        {
            string projectName = $@"{classModelName}_proj";
            _customUsings = string.Empty;
            _customUsings += $@"@using {projectName}.ReportForms";

            if (
                File.Exists($"{_project.DirApplication}\\Temp\\{projectName}\\{projectName}.csproj")
            )
            {
                Modelo model = _project.ModeloCol.Modelos.FirstOrDefault(x => x.Id == modelName);

                if (model != null)
                {
                    WoDesignerRawReportHelper woDesignerRawReportHelper =
                        new WoDesignerRawReportHelper();
                    WoContainer baseContainer = woDesignerRawReportHelper.BuildRawReportForm(
                        modelName,
                        labelId: labelId,
                        isOData: true
                    );
                    baseContainer.IsUnit = true;

                    WoBlazorForm woBlazorForm = new WoBlazorForm(
                        woContainer: baseContainer,
                        projectName: projectName,
                        folderName: "Reports",
                        classModelName: classModelName
                    );

                    woBlazorForm.BuildCommonForm(_customUsings, isReport: true);

                    woBlazorForm.BuildProgramFiles(
                        isServer: isServer,
                        isUnit: true,
                        slaves: _slaves,
                        isReport: true
                    );

                    WoContainer odataFilters = woDesignerRawReportHelper.GetContainerOdataFilters(
                        modelName,
                        labelId: labelId
                    );

                    WoBlazorForm woBlazorreport = new WoBlazorForm(
                        woContainer: odataFilters,
                        projectName: projectName,
                        folderName: "ReportForms",
                        classModelName: $@"Report0Layout"
                    );

                    woBlazorreport.BuildSmallForm(isOdataReport: true);
                }
            }
            else
            {
                XtraMessageBox.Show(
                    "Realiza primero la generación de la base para poder realizar este proceso",
                    "Alerta",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }
        }

        #endregion Generación de reports odata

        #region Generación de slaves

        /// <summary>
        /// Lista con los nombres de las clases de las slaves.
        /// </summary>
        private List<string> _slaves = new List<string>();

        /// <summary>
        /// Recupera todas las slaves del modelo y las va generando una a una de la misma forma que los formularios.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="projectName"></param>
        [SupportedOSPlatform("windows")]
        public WoContainer GenerateSlaves(WoContainer container, string projectName)
        {
            _slaves.Clear();

            WoBlazorAnalize woBlazorAnalize = new WoBlazorAnalize();

            List<string> slaveModelNames = woBlazorAnalize.GetSlaveNames(container);

            if (slaveModelNames.Count > 0)
            {
                container = woBlazorAnalize.GetWoContainer(container);
                _customUsings += $@"@using {projectName}.Slaves";

                int slaveCount = 0;
                foreach (string slaveModelName in slaveModelNames)
                {
                    string pathSlave =
                        $@"{_project.DirProyectData}\LayOuts\FormDesign\{slaveModelName}.json";
                    WoContainer slaveContainer = new WoContainer();
                    if (File.Exists(pathSlave))
                    {
                        string json = File.ReadAllText(pathSlave);
                        slaveContainer = JsonConvert.DeserializeObject<WoContainer>(json);
                    }
                    else
                    {
                        WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                            new WoDesignerRawSerializerHelper();

                        slaveContainer = woDesignerRawSerializerHelper.BuildRawWoContainer(
                            slaveModelName
                        );
                    }

                    WoBlazorForm woBlazorslave = new WoBlazorForm(
                        woContainer: slaveContainer,
                        projectName: projectName,
                        folderName: "Slaves",
                        classModelName: $@"Slave{slaveCount}Slave"
                    );

                    _slaves.Add($@"Slave{slaveCount}Slave");

                    woBlazorslave.BuildSmallForm(container.ModelId);
                    slaveCount++;
                }
            }
            else
            {
                string pathProject = $@"{_project.DirProyectTemp}/{projectName}";

                if (Directory.Exists($@"{pathProject}/Slaves"))
                {
                    List<string> slavesFilesCol = WoDirectory.ReadDirectoryFiles(
                        $@"{pathProject}/Slaves"
                    );
                    foreach (string slaveFile in slavesFilesCol)
                    {
                        WoDirectory.DeleteFile(slaveFile);
                    }
                }

                List<string> pathsControlsCol = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/ControlModels"
                );
                foreach (string pathControl in pathsControlsCol)
                {
                    if (pathControl.Contains("Slave"))
                    {
                        WoDirectory.DeleteFile(pathControl);
                    }
                }

                List<string> pathsUserCodeCol = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/UserCode"
                );
                foreach (string pathUserCode in pathsUserCodeCol)
                {
                    if (pathUserCode.Contains("Slave"))
                    {
                        WoDirectory.DeleteFile(pathUserCode);
                    }
                }

                List<string> pathFluentCol = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/FluentValidators"
                );
                foreach (string pathFluent in pathFluentCol)
                {
                    if (pathFluent.Contains("Slave"))
                    {
                        WoDirectory.DeleteFile(pathFluent);
                    }
                }

                List<string> pathsTransitionSettings = WoDirectory.ReadDirectoryFiles(
                    $@"{pathProject}/TransitionSettings"
                );
                foreach (string pathTransitionSettings in pathsTransitionSettings)
                {
                    if (pathTransitionSettings.Contains("Slave"))
                    {
                        WoDirectory.DeleteFile(pathTransitionSettings);
                    }
                }
            }

            return container;
        }

        #endregion Generación de slaves


        #region Generar menu

        public void GenerateMenuTest(string projectName, WoMenuProperties menu)
        {
            WoBlazorForm woBlazorForm = new WoBlazorForm(
                woContainer: null,
                projectName: projectName,
                folderName: string.Empty,
                classModelName: string.Empty
            );
            woBlazorForm.BuildBaseForMenus();
            BuildMenuPages(projectName, menu);
            CounterPages = 0;
            WoBlazorDefault woBlazorDefault = new WoBlazorDefault(projectName);
            woBlazorDefault.BuildLayout(menu.Theme);
        }

        public void ReGenerateMenuTest(string projectName, WoMenuProperties menu)
        {
            List<string> files = WoDirectory.ReadDirectoryFiles(
                $@"{_project.DirProyectTemp}\{projectName}\Pages"
            );
            foreach (string pathFile in files)
            {
                string name = pathFile.Split('\\').Last();
                if (
                    !(
                        name == "_Host.cshtml"
                        || name == "_Layout.cshtml"
                        || name == "Index.razor"
                        || name == "Index.razor.cs"
                    )
                )
                {
                    WoDirectory.DeleteFile(pathFile);
                }
            }

            BuildMenuPages(projectName, menu);
            CounterPages = 0;
        }

        private int CounterPages = 0;

        private void BuildMenuPages(string projectName, WoMenuProperties node)
        {
            WoBlazorDefault woBlazorDefault = new WoBlazorDefault(projectName);
            CounterPages++;
            if (node.Reference != "/")
            {
                woBlazorDefault.BuildDefault(referencia: node.Reference ?? $@"Page{CounterPages}");
            }

            if (node.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties nodeChild in node.ContentCol)
                {
                    BuildMenuPages(projectName, nodeChild);
                }
            }
        }

        #endregion Generar menu

        #region Logs

        private WoLog _incorrectInstance = new WoLog()
        {
            CodeLog = "000",
            Title = "Instancia del generador configurada incorrectamente.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoTextEditPartial",
                MethodOrContext = "GetTextEditPartial"
            }
        };

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

        #endregion Logs
    }
}
