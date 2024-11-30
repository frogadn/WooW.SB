using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using WooW.Core;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.BlazorGenerator.BlazorGeneration.BlazorSave;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.ControlModels;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.Pages;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.ProjectFiles;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.Slaves;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.TransitionSettings;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.UserCode;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.ControlModels;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.FluentValidators;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Pages;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Slaves;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.TransitionSettings;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.UserCode;
using WooW.SB.BlazorGenerator.BlazorTemplates.ServerTemplates;
using WooW.SB.BlazorGenerator.BlazorTemplates.WasmTemplates;
using WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorSettings;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorGeneration.BlazorForm
{
    public class WoBlazorForm
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto permite enviar logs y alertas.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Variables globales

        /// <summary>
        /// Nombre del fichero y base de la clase para los ficheros generados.
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Nombre del proyecto sobre el que se esta trabajando.
        /// (Puede ser diferente al nombre del modelo en caso de no ser una unitaria).
        /// </summary>
        private string _projectName = string.Empty;

        /// <summary>
        /// Nombre de la carpeta contenedora del razor y la parcial principal del formulario generado.
        /// </summary>
        private string _folderName = string.Empty;

        /// <summary>
        /// Ruta del proyecto.
        /// </summary>
        private string _pathProject = string.Empty;

        /// <summary>
        /// Representa el grupo de lo recuperado de alguno de los diseñadores.
        /// </summary>
        private WoContainer _container = null;

        /// <summary>
        /// Indica si se esta generando una prueba unitaria o un proyecto completo.
        /// </summary>
        private eTypeOfForm _typeOfForm = eTypeOfForm.FormUnit;

        #endregion Variables globales


        #region Constructor

        /// <summary>
        /// Constructor principal ocupado de inicializar la mayoría de las variables globales.
        /// </summary>
        /// <param name="woContainer"></param>
        /// <param name="projectName"></param>
        public WoBlazorForm(
            WoContainer woContainer,
            string projectName,
            string folderName,
            string classModelName
        )
        {
            _folderName = folderName;

            _container = woContainer;

            _projectName = projectName;

            _pathProject = $@"{_project.DirProyectTemp}\{projectName}";

            _classModelName = classModelName;

            _typeOfForm =
                (classModelName.Contains("UnitModel"))
                    ? eTypeOfForm.FormUnit
                    : eTypeOfForm.FormProyect;
        }

        #endregion Constructor


        #region Generación de formularios

        /// <summary>
        /// Método Ocupado de orquestar el proceso de generación de formularios.
        /// Solo Formularios principales, los formularios de la esclava se generan aparte.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BuildCommonForm(
            string customUsing,
            bool isReport = false,
            bool isList = false,
            bool blazorIntegral = false
        )
        {
            if (isReport || isList)
            {
                BuildGenericForm(blazorIntegral, isReport, isList);
            }
            else if (
                _container.ModelType != WoTypeModel.Kardex
                && _container.ModelType != WoTypeModel.Control
                && _container.ModelType != WoTypeModel.Configuration
                && _container.ModelType != WoTypeModel.View
            )
            {
                BuildGenericPages();
                BuildTransitionSettings();
            }

            if (
                !isReport
                && !(
                    (
                        _container.ModelType == WoTypeModel.Kardex
                        || _container.ModelType == WoTypeModel.Control
                        || _container.ModelType == WoTypeModel.Configuration
                        || _container.ModelType == WoTypeModel.View
                    ) && !isList
                )
            )
            {
                BuildScriptUser(isList);
                BuildScriptUserPartial(isList);
            }

            if (
                !isList
                && _container.ModelType != WoTypeModel.Kardex
                && _container.ModelType != WoTypeModel.Control
                && _container.ModelType != WoTypeModel.Configuration
                && _container.ModelType != WoTypeModel.View
                && _container.HaveModelReference
            )
            {
                BuildGenericFluentValidator();
            }

            MoveFileResources();

            BuildGenericControlModel();

            BuildGenericImports(customUsing);

            if (
                (
                    _container.ModelType == WoTypeModel.Kardex
                    || _container.ModelType == WoTypeModel.Control
                    || _container.ModelType == WoTypeModel.Configuration
                    || _container.ModelType == WoTypeModel.View
                ) && !isList
            )
            {
                DeleteFilesForSimpleList(isReport);
            }

            if (
                _container.ModelType == WoTypeModel.Request
                && _container.SubType == WoSubTypeModel.Report
                && _container.IsUnit
            )
            {
                DeleteFilesForReports();
            }
        }

        /// <summary>
        /// Genera la base del proyecto para los menús.
        /// </summary>
        public void BuildBaseForMenus()
        {
            BuildGenericImports(string.Empty);
            MoveFileResources();
            string pathBase = $@"{_pathProject}\Pages\.razor";
            if (File.Exists(pathBase))
            {
                WoDirectory.DeleteFile(pathBase);
            }
        }

        #endregion Generación de formularios

        #region _proj/Pages

        /// <summary>
        /// Instancia del generador de las clases parciales del formulario.
        /// </summary>
        private WoLayoutPartialGenerator _woLayoutPartialGenerator = null;

        /// <summary>
        /// Instancia del generador del razor del formulario.
        /// </summary>
        private WoLayoutRazorGenerator _woLayoutRazorGenerator = null;

        /// <summary>
        /// Se ocupa de ejecutar el código y las plantillas para la creación del fichero con el razor
        /// y su clase parcial.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void BuildGenericPages()
        {
            /// Generación del razor
            _woLayoutRazorGenerator = new WoLayoutRazorGenerator(_classModelName);

            WoTemplateLayoutRazorBlazor woTemplateLayoutRazorWasm =
                new WoTemplateLayoutRazorBlazor();

            woTemplateLayoutRazorWasm.Template = _woLayoutRazorGenerator.GetControlModelClass(
                baseContainer: _container
            );

            string pathRazor = $@"{_pathProject}/{_folderName}/{_classModelName}Layout.razor";

            WoDirectory.WriteTemplate(
                pathTemplate: pathRazor,
                data: woTemplateLayoutRazorWasm.TransformText()
            );

            /// Generación de la clase parcial
            _woLayoutPartialGenerator = new WoLayoutPartialGenerator(_classModelName);

            WoTemplateLayoutPartialBlazor woTemplateLayoutPartialWasm =
                new WoTemplateLayoutPartialBlazor();
            woTemplateLayoutPartialWasm.Directory = _folderName;
            woTemplateLayoutPartialWasm.Project = _projectName;
            woTemplateLayoutPartialWasm.Code = _woLayoutPartialGenerator.GetPartialClass(
                _container
            );

            string pathPartial = $@"{_pathProject}/{_folderName}/{_classModelName}Layout.razor.cs";

            WoDirectory.WriteTemplate(
                pathTemplate: pathPartial,
                data: woTemplateLayoutPartialWasm.TransformText()
            );
        }

        #endregion _proj/Pages


        #region Generación de slaves

        /// <summary>
        /// Genera el formulario de la small.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BuildSmallForm(string masterModelName = "", bool isOdataReport = false)
        {
            BuildGenericForm();

            if (!isOdataReport)
            {
                BuildGenericFluentValidator();
            }

            BuildGenericControlModel();

            if (isOdataReport)
            {
                BuildODataScriptsUser();
            }
            else
            {
                BuildScriptUser();
                BuildScriptUserPartial(isList: false);
            }

            if (masterModelName != string.Empty)
            {
                BuildSlaveTransitionSettings(masterModelName);
            }
        }

        #endregion Generación de slaves

        #region _proj/Slave

        /// <summary>
        /// Instancia del generador del razor del formulario de la slave
        /// </summary>
        private WoSlaveRazorGenerator _woSlaveRazorGenerator = null;

        /// <summary>
        /// Instancia del generador de la clase parciales del formulario de la slave
        /// </summary>
        private WoSlavePartialGenerator _woSlavePartialGenerator = null;

        /// <summary>
        /// Construye las slaves del modelo a través de las clases para construir los ficheros de las clases.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BuildGenericForm(
            bool blazorIntegral = false,
            bool isReport = false,
            bool isList = false
        )
        {
            /// Generación del fichero
            if (!Directory.Exists($@"{_pathProject}/{_folderName}"))
            {
                WoDirectory.CreateDirectory($@"{_pathProject}/{_folderName}");
            }

            /// Construcción del razor
            _woSlaveRazorGenerator = new WoSlaveRazorGenerator();

            WoTemplateSlaveRazorBlazor woTemplateSlaveRazorBlazor =
                new WoTemplateSlaveRazorBlazor();
            woTemplateSlaveRazorBlazor.Template = _woSlaveRazorGenerator.GetControlModelClass(
                _container,
                isReport,
                isList,
                blazorIntegral
            );

            string pathRazor = $@"{_pathProject}/{_folderName}/{_classModelName}.razor";

            WoDirectory.WriteTemplate(
                pathTemplate: pathRazor,
                data: woTemplateSlaveRazorBlazor.TransformText()
            );

            /// Construcción de la clase parcial
            _woSlavePartialGenerator = new WoSlavePartialGenerator(_classModelName);

            WoTemplateSlavePartialBlazor woTemplateSlavePartialBlazor =
                new WoTemplateSlavePartialBlazor();
            woTemplateSlavePartialBlazor.Project = _projectName;
            woTemplateSlavePartialBlazor.FolderName = _folderName;
            woTemplateSlavePartialBlazor.Code = _woSlavePartialGenerator.GetPartialClass(
                _container,
                isReport,
                isList,
                blazorIntegral
            );

            string pathPartial = $@"{_pathProject}/{_folderName}/{_classModelName}.razor.cs";

            WoDirectory.WriteTemplate(
                pathTemplate: pathPartial,
                data: woTemplateSlavePartialBlazor.TransformText()
            );
        }

        #endregion _proj/Slave


        #region _proj

        private void BuildGenericImports(string customUsings)
        {
            if (File.Exists($"{_pathProject}\\_Imports.razor"))
            {
                WoDirectory.DeleteFile($"{_pathProject}\\_Imports.razor");
            }

            WoTemplateImportsBlazor woTemplateImportsBlazor = new WoTemplateImportsBlazor();
            woTemplateImportsBlazor.Project = _projectName;
            woTemplateImportsBlazor.CustomUsings = customUsings;
            WoDirectory.WriteTemplate(
                pathTemplate: $@"{_pathProject}/_Imports.razor",
                data: woTemplateImportsBlazor.TransformText()
            );
        }

        /// <summary>
        /// Genera el fichero de program para el proyecto.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void BuildProgramFiles(
            bool isServer,
            bool isUnit,
            List<string> slaves,
            bool isReport
        )
        {
            WoProgramClassGenerator woProgramClassGenerator = new WoProgramClassGenerator(isUnit);
            string extraUsings = string.Empty;
            bool onlyList = true;

            if (
                _container.ModelType != WoTypeModel.Kardex
                && _container.ModelType != WoTypeModel.Control
                && _container.ModelType != WoTypeModel.Configuration
                && _container.ModelType != WoTypeModel.View
            )
            {
                if (!isReport)
                {
                    extraUsings = $@"using {_projectName}.TransitionSettings;";
                }
                onlyList = false;
            }

            if (isServer)
            {
                /// Server
                try
                {
                    WoTemplateProgramServer woTemplateProgramServer = new WoTemplateProgramServer();
                    woTemplateProgramServer.Project = _projectName;
                    woTemplateProgramServer.ExtraUsings = extraUsings;
                    woTemplateProgramServer.Dependencias = woProgramClassGenerator.GetCode(
                        models: new List<string>(),
                        reports: new List<string>(),
                        lists: new List<string>(),
                        slaves,
                        oDataReports: new List<string>(),
                        blazorType: "Server",
                        onlyList: onlyList,
                        isReport: isReport
                    );
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathProject}/Program.cs",
                        data: woTemplateProgramServer.TransformText()
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
                        models: new List<string>(),
                        reports: new List<string>(),
                        lists: new List<string>(),
                        slaves,
                        oDataReports: new List<string>(),
                        blazorType: "Wasm",
                        onlyList: onlyList,
                        isReport: isReport
                    );
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathProject}/Program.cs",
                        data: woTemplateProgramWasm.TransformText()
                    );
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $@"Error al intentar escribir las T4 para el proyecto base de Wasm: {ex.Message}"
                    );
                }
            }
        }

        #endregion _proj

        #region _proj/FluentValidators

        /// <summary>
        /// Instancia el generador y se ocupa de ejecutar el código y las plantillas para escribir la clase
        /// con el funcionamiento del fluent.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void BuildGenericFluentValidator()
        {
            try
            {
                string pathDirectorySave =
                    $@"{_project.DirProyectData}\LayOuts\UserCode\{_container.ModelId}_proj";
                if (!File.Exists($@"{pathDirectorySave}\{_container.ModelId}Validator.cs"))
                {
                    WoBlazorSave woBlazorSave = new WoBlazorSave(
                        classModelName: _container.ModelId
                    );

                    woBlazorSave.BuildBaseSave(_container);
                }

                WoTemplateFluentValidatorBlazor woTemplateFluentValidatorBlazor =
                    new WoTemplateFluentValidatorBlazor();
                woTemplateFluentValidatorBlazor.Project = _projectName;

                WoSyntaxManagerFluentValidation woSyntaxManagerFluentValidation =
                    new WoSyntaxManagerFluentValidation();
                woSyntaxManagerFluentValidation.InitializeManager(
                    pathScript: $@"{pathDirectorySave}/{_container.ModelId}Validator.cs",
                    className: $@"{_container.ModelId}Validator",
                    modelName: _container.ModelId
                );

                woTemplateFluentValidatorBlazor.Code =
                    woSyntaxManagerFluentValidation.GetClassCodeWithNewName(
                        oldName: $@"{_container.ModelId}Validator",
                        newName: $@"{_classModelName}Validator"
                    );

                string filePathFluent =
                    $@"{_pathProject}/FluentValidators/{_classModelName}Validator.cs";

                WoDirectory.WriteTemplate(
                    pathTemplate: filePathFluent,
                    data: woTemplateFluentValidatorBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/FluentValidators {ex.Message}"
                );
            }
        }

        #endregion _proj/FluentValidators

        #region _proj/ControlModels

        /// <summary>
        /// Generador de la clase con las vistas.
        /// </summary>
        private WoControlModelGenerator _woControlModelGenerator = null;

        /// <summary>
        /// Permite ejecutar las templates y el código para escribir la clase con las views de los controles.
        /// </summary>
        private void BuildGenericControlModel()
        {
            try
            {
                _woControlModelGenerator = new WoControlModelGenerator(_classModelName);

                WoTemplateModelControlsBlazor woTemplateModelControlsBlazor =
                    new WoTemplateModelControlsBlazor();
                woTemplateModelControlsBlazor.Project = _projectName;
                woTemplateModelControlsBlazor.Code = _woControlModelGenerator.GetControlModelClass(
                    baseContainer: _container
                );

                string filePath = $@"{_pathProject}/ControlModels/{_classModelName}Controls.cs";

                WoDirectory.WriteTemplate(
                    pathTemplate: filePath,
                    data: woTemplateModelControlsBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/ControlModels {ex.Message}"
                );
            }
        }

        #endregion _proj/ControlModels

        #region _proj/UserCode

        #region OData Scripts

        /// <summary>
        /// Construye la clase con los scripts del usuario directo en el proyecto generado
        /// para cuando no se parte de un modelo.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void BuildODataScriptsUser()
        {
            string pathScriptUser =
                $@"{_project.DirProyectTemp}\{_projectName}\UserCode\{_classModelName}ScriptsUser.cs";

            WoScriptsUserGenerator woScriptsUserGenerator = new WoScriptsUserGenerator(
                classModelName: _classModelName
            );

            WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                new WoTemplateGenericClassBlazor();

            string classComplete = woScriptsUserGenerator.GetODataUsings(_projectName);
            classComplete += woScriptsUserGenerator.GetScriptsUserClass(_container);

            woTemplateGenericClassBlazor.Code = classComplete;

            WoDirectory.WriteTemplate(
                pathTemplate: pathScriptUser,
                data: woTemplateGenericClassBlazor.TransformText()
            );
        }

        #endregion OData Scripts


        #region MainMethod

        /// <summary>
        /// Método principal para orquestar la construcción de los scripts del usuario
        /// </summary>
        /// <param name="isLis"></param>
        [SupportedOSPlatform("windows")]
        private void BuildScriptUser(bool isList = false)
        {
            try
            {
                string modelName = _container.ModelId;

                if (isList)
                {
                    modelName = $@"{_container.ModelId}GridList";
                }

                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                Proyecto findTopProject = woProjectDataHelper.GetTopProjectWhit(_container.ModelId);

                string pathDirectoryCodeSave =
                    $@"{findTopProject.DirProyectData}\LayOuts\UserCode\{modelName}_proj";

                BuildScriptClass(isList, modelName);

                CopyClass(modelName, pathDirectoryCodeSave);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/UserCode {ex.Message}"
                );
            }
        }

        #endregion MainMethod

        #region Generación de la clase

        /// <summary>
        /// Realiza la generación de la clase solo con los datos base
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void BuildScriptClass(bool isList, string modelName)
        {
            try
            {
                if (!isList)
                {
                    // Recuperamos los diseños del modelo y de sus extensiones
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    List<(WoContainer desing, string project)> desingsProyect =
                        woProjectDataHelper.GetDesingProyect(modelName);

                    foreach ((WoContainer desing, string project) desingProyect in desingsProyect)
                    {
                        Proyecto project = new Proyecto();
                        project.Load(desingProyect.project);

                        if (
                            !File.Exists(
                                $"{project.DirProyectData}\\LayOuts\\UserCode\\{modelName}_proj\\{modelName}ScriptsUser.cs"
                            )
                        )
                        {
                            WoBlazorSave woBlazorSave = new WoBlazorSave(
                                classModelName: modelName,
                                project: project
                            );
                            woBlazorSave.BuildBaseSave(desingProyect.desing);
                        }
                    }
                }
                else
                {
                    if (
                        !File.Exists(
                            $"{_project.DirProyectData}\\LayOuts\\UserCode\\{modelName}_proj\\{modelName}ScriptsUser.cs"
                        )
                    )
                    {
                        WoBlazorSave woBlazorSave = new WoBlazorSave(classModelName: modelName);
                        woBlazorSave.BuildBaseSave(_container);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la generación de la clase. {ex.Message}");
            }
        }

        #endregion Generación de la clase

        #region Generación de la clase

        /// <summary>
        /// Pasa la clase de la carpeta de los scripts de usuario a el proyecto de blazor
        /// </summary>
        private void CopyClass(string modelName, string pathDirectoryCodeSave)
        {
            try
            {
                WoTemplateScriptsUserBlazor woTemplateScriptsUserBlazor =
                    new WoTemplateScriptsUserBlazor();
                woTemplateScriptsUserBlazor.Project = _projectName;

                WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                woSyntaxManagerUserCode.InitializeManager(
                    pathScript: $@"{pathDirectoryCodeSave}/{modelName}ScriptsUser.cs",
                    className: $@"{modelName}ScriptsUser",
                    modelName: modelName
                );

                string rawClass = woSyntaxManagerUserCode
                    .GetClassCodeWithNewName(
                        oldName: $@"{modelName}ScriptsUser",
                        newName: $@"{_classModelName}ScriptsUser"
                    )
                    .Replace($@"public {modelName}Controls", $@"public {_classModelName}Controls")
                    .Replace(
                        $@"= {modelName}Controls.GetInstance();",
                        $@"= {_classModelName}Controls.GetInstance();"
                    )
                    .Replace($@"{modelName}ScriptsUser", $@"{_classModelName}ScriptsUser")
                    .Replace(
                        $@"public {modelName} {modelName} = new {modelName}();",
                        $@"public {_container.ModelId} {_container.ModelId} = new {_container.ModelId}();"
                    )
                    .Replace($@"({modelName}Controls", $@"({_classModelName}Controls");

                rawClass = rawClass.Replace($@"{modelName}Controls", $@"{_classModelName}Controls");

                if (modelName != _classModelName)
                {
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    //Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(m =>
                    //    m.Id == modelName
                    //);
                    Modelo findModel = woProjectDataHelper.GetMainModel(modelName);
                    List<ModeloColumna> fullColumnsCol = woProjectDataHelper.GetFullColumns(
                        modelName
                    );
                    if (findModel != null)
                    {
                        int slaveCount = 0;
                        foreach (ModeloColumna column in fullColumnsCol)
                        {
                            if (column.TipoControl == WoTypeControl.CollectionEditor)
                            {
                                rawClass = rawClass.Replace(
                                    $"{column.Id.Replace("Col", "")}SlaveControls",
                                    $@"Slave{slaveCount}SlaveControls"
                                );
                                slaveCount++;
                            }
                        }
                    }
                }

                woTemplateScriptsUserBlazor.Code = rawClass;

                string usingsPath =
                    $@"{_project.DirLayOuts}\UserCode\{modelName}_proj\{modelName}Usings.cs";
                string usings = string.Empty;

                if (!File.Exists(usingsPath))
                {
                    WoDirectory.WriteFile(
                        usingsPath,
                        data: WoSyntaxEditorHeaderAndFooter.GetMainUsings()
                    );
                }

                usings = WoDirectory
                    .ReadFile(usingsPath)
                    .Replace(
                        "using System.Private.CoreLib;",
                        $@"using {_projectName}.ControlModels;"
                    );

                woTemplateScriptsUserBlazor.Usings = usings;

                // Escritura de la clase principal
                string pathFile = $@"{_pathProject}/UserCode/{_classModelName}ScriptsUser.cs";

                WoDirectory.WriteTemplate(
                    pathTemplate: pathFile,
                    data: woTemplateScriptsUserBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al copiar la clase al blazor. {ex.Message}");
            }
        }

        #endregion Generación de la clase

        #region Clase parcial del blazor

        /// <summary>
        /// Construye la clase parcial de los scripts del usuario, así como la copia en caso
        /// de requerir restaurarse en otro generado.
        /// Igual puede restaurar la copia en caso de que ya exista una.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void BuildScriptUserPartial(bool isList)
        {
            try
            {
                string modelName = _container.ModelId;

                if (isList)
                {
                    modelName = $@"{_container.ModelId}GridList";
                }

                string pathDirectoryCodeSave =
                    $@"{_project.DirProyectData}\LayOuts\UserCode\{modelName}_proj";
                if (
                    !File.Exists(
                        $@"{pathDirectoryCodeSave}\{_container.ModelId}ScriptsUserPartial.cs"
                    )
                )
                {
                    WoBlazorSave woBlazorSave = new WoBlazorSave(classModelName: modelName);

                    woBlazorSave.BuildBaseSave(_container);
                }

                WoTemplateScriptsUserPartialBlazor woTemplateScriptsUserPartialBlazor =
                    new WoTemplateScriptsUserPartialBlazor();
                woTemplateScriptsUserPartialBlazor.Project = _projectName;

                WoSyntaxManagerUserCodePartial woSyntaxManagerUserCodePartial =
                    new WoSyntaxManagerUserCodePartial();
                woSyntaxManagerUserCodePartial.InitializeManager(
                    pathScript: $@"{pathDirectoryCodeSave}/{modelName}ScriptsUserPartial.cs",
                    className: $@"{modelName}ScriptsUser",
                    modelName: modelName
                );

                // Recuperamos en raw los métodos de los paquetes para este modelo
                //string rawCodeMethos = GetRawMethods(_container.ModelId);
                string rawCodeMethos = GetRawMethods(modelName);

                string rawCodeClass = woSyntaxManagerUserCodePartial.GetClassCodeWithNewName(
                    oldName: $@"{modelName}ScriptsUser",
                    newName: $@"{_classModelName}ScriptsUser"
                );

                if (rawCodeMethos != string.Empty)
                {
                    rawCodeClass = rawCodeClass.Substring(0, rawCodeClass.Length - 1);
                    rawCodeClass += rawCodeMethos;
                    rawCodeClass += "\n\r}";
                }

                woTemplateScriptsUserPartialBlazor.Code = rawCodeClass;

                string pathFile =
                    $@"{_pathProject}/UserCode/{_classModelName}ScriptsUserPartial.cs";

                WoDirectory.WriteTemplate(
                    pathTemplate: pathFile,
                    data: woTemplateScriptsUserPartialBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/FluentValidators {ex.Message}"
                );
            }
        }

        #endregion Clase parcial del blazor

        #region Recuperación de los métodos de los paquetes

        /// <summary>
        /// Recuperamos en un string los métodos de las clases de los paquetes
        /// </summary>
        /// <returns></returns>
        private string GetRawMethods(string modelName)
        {
            try
            {
                if (modelName == "Empleado")
                {
                    var x = 0;
                }

                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                List<string> projectPaths = woProjectDataHelper.GetProyectPathsWhit(
                    _container.ModelId
                );
                Proyecto topProyectForModel = woProjectDataHelper.GetTopProjectWhit(
                    _container.ModelId
                );

                projectPaths.Remove(topProyectForModel.Dir);

                StringBuilder strProjects = new StringBuilder();

                foreach (string projectPath in projectPaths)
                {
                    string scriptUserPath =
                        $"{projectPath}\\ProyectData\\LayOuts\\UserCode\\{modelName}_proj\\{modelName}ScriptsUser.cs";
                    if (File.Exists(scriptUserPath))
                    {
                        WoSyntaxManagerUserCode woSyntaxManagerUserCode =
                            new WoSyntaxManagerUserCode();

                        woSyntaxManagerUserCode.InitializeManager(
                            pathScript: scriptUserPath,
                            className: modelName,
                            modelName: modelName
                        );

                        strProjects.AppendLine(
                            woSyntaxManagerUserCode.GetRawMethods(contains: "_OnChange")
                        );
                        strProjects.AppendLine(
                            woSyntaxManagerUserCode.GetRawMethods(contains: "_OnFocus")
                        );
                        strProjects.AppendLine(
                            woSyntaxManagerUserCode.GetRawMethods(contains: "_OnBlur")
                        );
                        strProjects.AppendLine(
                            woSyntaxManagerUserCode.GetRawMethods(contains: "_OnClick")
                        );
                    }
                }

                return strProjects.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar los métodos de los paquetes. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de los métodos de los paquetes

        #endregion _proj/UserCode

        #region _proj/TransitionSettings

        /// <summary>
        /// Instancia de la clase que genera la clase con las transiciones.
        /// </summary>
        private WoTransitionSettingsGenerator _woTransitionSettingsGenerator = null;

        /// <summary>
        /// Utiliza la información del contenedor principal y se apoya de las clases generadoras para
        /// generar el fichero de las transiciones.
        /// </summary>
        private void BuildTransitionSettings()
        {
            _woTransitionSettingsGenerator = new WoTransitionSettingsGenerator(
                woContainer: _container,
                classModelName: _classModelName
            );

            WoTemplateTransitionSettingsBlazor woTemplateTransitionSettingsBlazor =
                new WoTemplateTransitionSettingsBlazor();
            woTemplateTransitionSettingsBlazor.Project = _projectName;
            woTemplateTransitionSettingsBlazor.Code =
                _woTransitionSettingsGenerator.GetTransitionSettingsClass();

            string pathFile =
                $@"{_pathProject}/TransitionSettings/{_classModelName}TransitionSettings.cs";

            WoDirectory.WriteTemplate(
                pathTemplate: pathFile,
                data: woTemplateTransitionSettingsBlazor.TransformText()
            );
        }

        /// <summary>
        /// Instancia de la clase que general al clase con las transiciones de la slave.
        /// </summary>
        private WoSlaveTransitionSettingsGenerator _woSlaveTransitionSettingsGenerator = null;

        private void BuildSlaveTransitionSettings(string masterModelName)
        {
            _woSlaveTransitionSettingsGenerator = new WoSlaveTransitionSettingsGenerator(
                classModelName: _classModelName,
                masterModelName: masterModelName,
                slaveContainer: _container
            );

            WoTemplateTransitionSettingsBlazor woTemplateTransitionSettingsBlazor =
                new WoTemplateTransitionSettingsBlazor();
            woTemplateTransitionSettingsBlazor.Project = _projectName;
            woTemplateTransitionSettingsBlazor.Code = _woSlaveTransitionSettingsGenerator.GetCode();

            string pathFile =
                $@"{_pathProject}/TransitionSettings/{_classModelName}TransitionSettings.cs";

            WoDirectory.WriteTemplate(
                pathTemplate: pathFile,
                data: woTemplateTransitionSettingsBlazor.TransformText()
            );
        }

        #endregion _proj/TransitionSettings

        #region Carga de recursos al blazor

        /// <summary>
        /// Mueve el archivo de recursos del proyecto al blazor generado.
        /// Une los archivos de recursos del modelo con los d etiquetas y genera un archivo App..resx
        /// </summary>
        protected void MoveFileResources()
        {
            string dirResourcesFiles = $@"{_project.DirApplication}\Resources\Labels";
            List<string> resourceFiles = WoDirectory.ReadDirectoryFiles(path: dirResourcesFiles);

            string dirResourcesFilesModel = $@"{_project.DirApplication}\Resources\ModelLabel";
            List<string> resourcePathComplete = WoDirectory.ReadDirectoryFiles(
                path: dirResourcesFilesModel
            );

            List<string> resourceFilesModel = new List<string>();
            foreach (string resourceFileModel in resourcePathComplete)
            {
                string[] resourceFileModelCol = resourceFileModel.Split('\\');
                resourceFilesModel.Add(resourceFileModelCol[resourceFileModelCol.Length - 1]);
            }

            foreach (string file in resourceFiles)
            {
                string[] fileCol = file.Split('.');
                string extension = fileCol[fileCol.Length - 1];

                string[] fileColAux = file.Split('\\');
                string fileName = fileColAux[fileColAux.Length - 1];

                if (extension == "resx")
                {
                    StringBuilder strDataComplete = new StringBuilder();
                    string newName = string.Empty;
                    string fileModelName = string.Empty;

                    string[] fileNameCol = fileName.Split('.');
                    if (fileNameCol.Length > 2)
                    {
                        string culture = fileNameCol[fileNameCol.Length - 2];

                        fileModelName = resourceFilesModel
                            .Where(x => x.Contains(culture) && x.Contains(".resx"))
                            .FirstOrDefault();

                        if (fileModelName != null)
                        {
                            newName = $@"App.{culture}.resx";
                        }
                    }
                    else
                    {
                        fileModelName = resourceFilesModel
                            .Where(x => x.Split('.').Length <= 2 && x.Contains(".resx"))
                            .FirstOrDefault();

                        if (fileModelName != null)
                        {
                            newName = $@"App.es.resx";
                        }
                    }

                    string rawLabelResx = WoDirectory.ReadFile(file);
                    string dataLabelResx = rawLabelResx.Split("<root>")[1];

                    string rawModelResx = WoDirectory.ReadFile(
                        $@"{_project.DirApplication}\Resources\ModelLabel\{fileModelName}"
                    );
                    string[] rawModelResxCol = rawModelResx.Split("</resheader>");
                    string dataModelResx = rawModelResxCol[rawModelResxCol.Length - 1];

                    strDataComplete.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    strDataComplete.AppendLine("<root>");
                    strDataComplete.AppendLine(dataLabelResx.Replace("</root>", ""));
                    strDataComplete.AppendLine(dataModelResx.Replace("</root>", ""));
                    strDataComplete.AppendLine("</root>");

                    WoDirectory.WriteFile(
                        $@"{_pathProject}\Resources\{newName}",
                        strDataComplete.ToString()
                    );
                }
            }
        }

        #endregion Carga de recursos al blazor

        #region Eliminar ficheros

        /// <summary>
        /// Elimina los ficheros que no son requeridos para los reportes
        /// </summary>
        private void DeleteFilesForReports()
        {
            //    string unitPathRazorPage =
            //        $@"{_project.DirApplication}\Temp\{_projectName}\Pages\{_classModelName}Layout.razor";

            //    if (File.Exists(unitPathRazorPage))
            //    {
            //        WoDirectory.DeleteFile(unitPathRazorPage);
            //    }
        }

        /// <summary>
        /// Elimina los ficheros que no son requeridos para las grids de los kardex
        /// </summary>
        private void DeleteFilesForSimpleList(bool isReport)
        {
            string unitPathRazorPage =
                $@"{_project.DirApplication}\Temp\{_projectName}\Pages\{_classModelName}Layout.razor";

            if (File.Exists(unitPathRazorPage))
            {
                WoDirectory.DeleteFile(unitPathRazorPage);
            }

            string unitPathPartialPage =
                $@"{_project.DirApplication}\Temp\{_projectName}\Pages\{_classModelName}Layout.razor.cs";

            if (File.Exists(unitPathPartialPage))
            {
                WoDirectory.DeleteFile(unitPathPartialPage);
            }

            string unitPathFluentValidator =
                $@"{_project.DirApplication}\Temp\{_projectName}\FluentValidators\{_classModelName}Validator.cs";

            if (File.Exists(unitPathFluentValidator))
            {
                WoDirectory.DeleteFile(unitPathFluentValidator);
            }

            string unitPathScriptUser =
                $@"{_project.DirApplication}\Temp\{_projectName}\UserCode\{_classModelName}ScriptsUser.cs";

            if (File.Exists(unitPathScriptUser))
            {
                WoDirectory.DeleteFile(unitPathScriptUser);
            }

            string unitPathScriptUserPartial =
                $@"{_project.DirApplication}\Temp\{_projectName}\UserCode\{_classModelName}ScriptsUserPartial.cs";

            if (File.Exists(unitPathScriptUserPartial))
            {
                WoDirectory.DeleteFile(unitPathScriptUserPartial);
            }

            string unitPathTransitnSettings =
                $@"{_project.DirApplication}\Temp\{_projectName}\TransitionSettings\{_classModelName}TransitionSettings.cs";

            if (File.Exists(unitPathTransitnSettings))
            {
                WoDirectory.DeleteFile(unitPathTransitnSettings);
            }

            if (!isReport)
            {
                string unitPathControlModel =
                    $@"{_project.DirApplication}\Temp\{_projectName}\ControlModels\{_classModelName}Controls.cs";

                if (File.Exists(unitPathControlModel))
                {
                    WoDirectory.DeleteFile(unitPathControlModel);
                }
            }
        }

        #endregion Eliminar ficheros
    }
}
