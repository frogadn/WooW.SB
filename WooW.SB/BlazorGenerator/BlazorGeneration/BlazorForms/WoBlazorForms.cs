using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.BlazorGenerator.BlazorGeneration.BlazorForm;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.ProjectFiles;
using WooW.SB.BlazorGenerator.BlazorTemplates.ServerTemplates;
using WooW.SB.BlazorGenerator.BlazorTemplates.WasmTemplates;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorGeneration.BlazorForms
{
    public class WoBlazorForms
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

        #region Variables globales

        /// <summary>
        /// Nombre del proyecto sobre el que se esta trabajando.
        /// (Puede ser diferente al nombre del modelo en caso de no ser una unitaria).
        /// </summary>
        private string _projectName = string.Empty;

        #endregion Variables globales


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase.
        /// Recibe el nombre del proyecto que se generará.
        /// </summary>
        /// <param name="projectName"></param>
        public WoBlazorForms(string projectName)
        {
            _projectName = projectName;
        }

        #endregion Constructor


        #region Método principal

        /// <summary>
        /// Instancia de la clase de actualización de los componentes del formulario.
        /// </summary>
        private WoDesignerUpdateControls _woDesignerUpdateControls = new WoDesignerUpdateControls();

        /// <summary>
        /// Recibe por parámetro la lista de los modelos que se generarán y
        /// los va recorriendo a la par que va generando los formularios.
        /// </summary>
        /// <param name="woForms"></param>
        [SupportedOSPlatform("windows")]
        public void BuildForms(List<string> selectedModels, bool isReport)
        {
            foreach (string model in selectedModels)
            {
                WoContainer woContainer = new WoContainer();
                string pathJson = $@"{_project.DirProyectData}\LayOuts\FormDesign\{model}.json";

                if (File.Exists(pathJson) && !isReport)
                {
                    WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                    woSyntaxManagerUserCode.InitializeManager(
                        pathScript: $@"{_project.DirLayOuts}\UserCode\{model}_proj\{model}ScriptsUser.cs",
                        className: model,
                        modelName: model
                    );

                    WoSyntaxManagerModelClass woSyntaxManagerModelClass =
                        new WoSyntaxManagerModelClass();
                    woSyntaxManagerModelClass.InitializeManager(
                        pathScript: $@"{_project.DirLayOuts}\UserCode\{model}_proj\{model}Controls.cs",
                        className: model,
                        modelName: model
                    );

                    //WoContainer baseContainer = JsonConvert.DeserializeObject<WoContainer>(
                    //    WoDirectory.ReadFile(pathJson)
                    //);

                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    WoContainer baseContainer = woProjectDataHelper.GetFullDesing(model);

                    woContainer = _woDesignerUpdateControls.UpdateControls(
                        woContainer: baseContainer,
                        woSyntaxManagerUserCode: woSyntaxManagerUserCode,
                        woSyntaxManagerModelClass: woSyntaxManagerModelClass
                    );
                }
                else
                {
                    if (isReport)
                    {
                        WoDesignerRawReportHelper woDesignerRawReportHelper =
                            new WoDesignerRawReportHelper();
                        woContainer = woDesignerRawReportHelper.BuildRawReportForm(model);
                    }
                    else
                    {
                        WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                        woContainer = woProjectDataHelper.GetFullDesing(model);
                        //WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                        //    new WoDesignerRawSerializerHelper();
                        //woContainer = woDesignerRawSerializerHelper.BuildRawWoContainer(model);
                    }
                }

                if (!isReport)
                {
                    GenerateListLayout(model);
                }

                GenerateFormLayout(woContainer, isReport);
            }
        }

        /// <summary>
        /// Genera el fichero de program para el proyecto.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void BuildProgramFiles(
            bool isServer,
            List<string> models,
            List<string> reports,
            List<string> lists,
            List<string> oDataReports,
            string extraUsings
        )
        {
            WoProgramClassGenerator woProgramClassGenerator = new WoProgramClassGenerator(false);

            if (isServer)
            {
                /// Server
                try
                {
                    WoTemplateProgramServer woTemplateProgramServer = new WoTemplateProgramServer();
                    woTemplateProgramServer.Project = _projectName;
                    woTemplateProgramServer.ExtraUsings = extraUsings;
                    woTemplateProgramServer.Dependencias = woProgramClassGenerator.GetCode(
                        models: models,
                        reports: reports,
                        lists: lists,
                        _slaveNames,
                        oDataReports: oDataReports,
                        blazorType: "Server",
                        onlyList: false,
                        isReport: false
                    );
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_project.DirApplication}/Temp/{_projectName}/Program.cs",
                        data: woTemplateProgramServer.TransformText()
                    );

                    StringBuilder strUsings = new StringBuilder();

                    string[] extraUsingsCol = extraUsings.Split(';');
                    foreach (string usingStr in extraUsingsCol)
                    {
                        if (usingStr != string.Empty)
                        {
                            strUsings.AppendLine($@"@{usingStr}");
                        }
                    }

                    foreach (string usingStr in _customUsings)
                    {
                        if (!strUsings.ToString().Contains(usingStr))
                        {
                            strUsings.AppendLine($@"{usingStr}");
                        }
                    }

                    WoDirectory.AddText(
                        $@"{_project.DirApplication}/Temp/{_projectName}/_Imports.razor",
                        strUsings.ToString()
                    );
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $@"No se pudieron escribir las plantillas en _proj de Server {ex.Message}"
                    );
                }
            }
            else
            {
                /// Wasm
                try
                {
                    WoTemplateProgramWasm woTemplateProgramWasm = new WoTemplateProgramWasm();
                    woTemplateProgramWasm.Project = _projectName;
                    woTemplateProgramWasm.ExtraUsings = extraUsings;
                    woTemplateProgramWasm.Dependencias = woProgramClassGenerator.GetCode(
                        models: models,
                        reports: reports,
                        lists: lists,
                        _slaveNames,
                        oDataReports: oDataReports,
                        blazorType: "Wasm",
                        onlyList: false,
                        isReport: false
                    );
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_project.DirApplication}/Temp/{_projectName}/Program.cs",
                        data: woTemplateProgramWasm.TransformText()
                    );

                    StringBuilder strUsings = new StringBuilder();
                    string[] extraUsingsCol = extraUsings.Split(';');
                    foreach (string usingStr in extraUsingsCol)
                    {
                        if (usingStr != string.Empty)
                        {
                            strUsings.AppendLine($@"@{usingStr}");
                        }
                    }

                    WoDirectory.AddText(
                        $@"{_project.DirApplication}/Temp/{_projectName}/_Imports.razor",
                        strUsings.ToString()
                    );
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $@"Ocurrió un error al intentar escribir las T4 para el proyecto base de Wasm: {ex.Message}"
                    );
                }
            }
        }

        /// <summary>
        /// Recibe por parametro la lista de los modelos que se generarán y
        /// los ba recorriendo para generar las listas.
        /// </summary>
        /// <param name="selectedModels"></param>
        /// <param name="isList"></param>
        //[SupportedOSPlatform("windows")]
        //public void BuildLists(List<string> selectedModels, bool isList)
        //{
        //    foreach (string model in selectedModels)
        //    {
        //        GenerateListLayout(model);
        //    }
        //}

        #endregion Método principal

        #region Generacion del fromulario

        /// <summary>
        /// Lista de los using que pueden variar en funcion de la generación.
        /// </summary>
        private List<string> _customUsings = new List<string>();

        /// <summary>
        /// Permite que el usuario pueda generar un formulario como prueba unitaria.
        /// </summary>
        /// <param name="container"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateFormLayout(WoContainer container, bool isReport = false)
        {
            WoContainer editContainer = GenerateSlaves(container, _projectName);

            if (isReport)
            {
                WoContainer editContainerWithReportForm = GenerateReports(
                    editContainer,
                    _projectName
                );

                editContainerWithReportForm.IsUnit = false;

                WoBlazorForm woBlazorForm = new WoBlazorForm(
                    woContainer: editContainerWithReportForm,
                    projectName: _projectName,
                    folderName: "Reports",
                    classModelName: $@"{editContainerWithReportForm.ModelId}Report"
                );

                StringBuilder usings = new StringBuilder();

                foreach (string customUsing in _customUsings)
                {
                    usings.AppendLine(customUsing);
                }

                woBlazorForm.BuildCommonForm(usings.ToString(), isReport);
            }
            else
            {
                editContainer.IsUnit = false;

                WoBlazorForm woBlazorForm = new WoBlazorForm(
                    woContainer: editContainer,
                    projectName: _projectName,
                    folderName: "Pages",
                    classModelName: editContainer.ModelId
                );

                StringBuilder usings = new StringBuilder();

                foreach (string customUsing in _customUsings)
                {
                    usings.AppendLine(customUsing);
                }

                woBlazorForm.BuildCommonForm(usings.ToString());
            }
        }

        #endregion Generacion del fromulario

        #region Generacion de la lista

        /// <summary>
        /// Generacion de las listas para las pruebas integrales.
        /// </summary>
        /// <param name="selectedModels"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateLists(List<string> selectedModels)
        {
            foreach (string model in selectedModels)
            {
                GenerateListLayout(model);
            }
        }

        /// <summary>
        /// Permite que se pueda generar una lista en base a un modelo.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void GenerateListLayout(string modelName)
        {
            _customUsings.Add($@"@using {_projectName}.GridLists");

            WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
            WoContainer listRawContainer = woDesignerRawListHelper.BuildRawListForm(modelName);

            WoBlazorForm woBlazorForm = new WoBlazorForm(
                woContainer: listRawContainer,
                projectName: _projectName,
                folderName: "GridLists",
                classModelName: $@"{modelName}GridList"
            );

            if (modelName == "ExtensionDeVista")
            {
                var x = 0;
            }

            woBlazorForm.BuildCommonForm(
                customUsing: string.Empty,
                isList: true,
                blazorIntegral: true
            );
        }

        #endregion Generacion de la lista

        #region Generacion de reports

        /// <summary>
        /// Recupera todas las slaves del modelo y las va generando una a una de la misma forma que los formularios.
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
                container = woBlazorAnalize.GetContainerWithReportNames(
                    container,
                    isCompleteGeneration: true
                );

                string reportUsing = $@"@using {projectName}.ReportForms";

                if (!_customUsings.Contains(reportUsing))
                {
                    _customUsings.Add(reportUsing);
                }

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
                        classModelName: $@"{reportContainer.ModelId}Layout"
                    );

                    woBlazorreport.BuildSmallForm();
                    reportCount++;
                }
            }

            return container;
        }

        #endregion Generacion de reports

        #region Generación de slaves

        private List<string> _slaveNames = new List<string>();

        /// <summary>
        /// Recupera todas las slaves del modelo y las va generando una a una de la misma forma que los formularios.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="projectName"></param>
        [SupportedOSPlatform("windows")]
        private WoContainer GenerateSlaves(WoContainer container, string projectName)
        {
            WoBlazorAnalize woBlazorAnalize = new WoBlazorAnalize();

            List<string> slaveModelNames = woBlazorAnalize.GetSlaveNames(container);

            if (slaveModelNames.Count > 0)
            {
                container = woBlazorAnalize.GetWoContainer(container, isCompleteGeneration: true);

                string slaveUsing = $@"@using {projectName}.Slaves";

                if (!_customUsings.Contains(slaveUsing))
                {
                    _customUsings.Add(slaveUsing);
                }

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
                        classModelName: $@"{slaveContainer.ModelId}Slave"
                    );

                    _slaveNames.Add($@"{slaveContainer.ModelId}Slave");

                    woBlazorslave.BuildSmallForm(container.ModelId);
                    slaveCount++;
                }
            }

            return container;
        }

        #endregion Generación de slaves

        #region Generación de los reportes odata

        /// <summary>
        /// Generacion de los reportes odata.
        /// </summary>
        /// <param name="modelName"></param>
        [SupportedOSPlatform("windows")]
        public void GenerateOdataReports(string modelName, string labelId = "Root")
        {
            Modelo model = _project.ModeloCol.Modelos.FirstOrDefault(x => x.Id == modelName);

            if (model != null)
            {
                WoDesignerRawReportHelper woDesignerRawReportHelper =
                    new WoDesignerRawReportHelper();
                WoContainer baseContainer = woDesignerRawReportHelper.BuildRawReportForm(
                    modelName,
                    isOData: true,
                    odataComplete: true
                );
                baseContainer.IsUnit = false;

                WoBlazorForm woBlazorForm = new WoBlazorForm(
                    woContainer: baseContainer,
                    projectName: _projectName,
                    folderName: "Reports",
                    classModelName: $@"{modelName}Report"
                );

                StringBuilder usings = new StringBuilder();

                foreach (string customUsing in _customUsings)
                {
                    usings.AppendLine(customUsing);
                }

                woBlazorForm.BuildCommonForm(usings.ToString(), isReport: true);

                WoContainer odataFilters = woDesignerRawReportHelper.GetContainerOdataFilters(
                    modelName,
                    labelId: labelId
                );

                WoBlazorForm woBlazorreport = new WoBlazorForm(
                    woContainer: odataFilters,
                    projectName: _projectName,
                    folderName: "ReportForms",
                    classModelName: $@"{modelName}ReportLayout"
                );

                woBlazorreport.BuildSmallForm(isOdataReport: true);
            }
        }

        #endregion Generación de los reportes odata

        #region Alertas

        private WoLog _modelNull = new WoLog()
        {
            CodeLog = "000",
            Title = "El modelo no existe en la colección de modelos",
            LogType = eLogType.Warning,
            FileDetails = new WoFileDetails()
            {
                Class = "WoBlazorForms",
                MethodOrContext = "BuildForms"
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

        #endregion Alertas
    }
}
