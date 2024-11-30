using System.IO;
using System.Linq;
using WooW.Core;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.FluentValidators;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.UserCode;
using WooW.SB.CodeEditor.CodeDialogs;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorFiles
{
    public class WoSyntaxEditorFiles
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

        #region Atributos

        /// <summary>
        /// Nombre del modelo sobre el que se va a trabajar.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Nombre de la clase que se va a generar para la unitaria de server.
        /// </summary>
        private string _unitClassModelNameServer = string.Empty;

        /// <summary>
        /// Nombre de la clase que se va a generar para la unitaria de wasm.
        /// </summary>
        private string _unitClassModelNameWasm = string.Empty;

        /// <summary>
        /// Ruta del script del usuario para el modelo y/o generado con el que se esta trabajando
        /// (path en la carpeta de layout).
        /// </summary>
        private string _pathScriptUser = string.Empty;

        /// <summary>
        /// Ruta del script del usuario para el modelo y/o generado con el que se esta trabajando (clase parcial)
        /// (path en la carpeta de layout).
        /// </summary>
        private string _pathScriptUserPartial = string.Empty;

        /// <summary>
        /// Ruta de la clase con el validador con el que se puede estar trabajando.
        /// </summary>
        private string _pathFluent = string.Empty;

        /// <summary>
        /// Path del proyecto generado de blazor wasm.
        /// </summary>
        private string _pathBlazorWasm = string.Empty;

        /// <summary>
        /// Path del proyecto generao de blazor server
        /// </summary>
        private string _pathBlazorServer = string.Empty;

        /// <summary>
        /// Nombre del proyecto generado.
        /// </summary>
        private string _projectNameServer = string.Empty;

        /// <summary>
        /// Nombre del proyecto generado.
        /// </summary>
        private string _projectNameWasm = string.Empty;

        /// <summary>
        /// Indicador de si se esta trabajando con el codigo de la grid.
        /// </summary>
        private bool _isGridCode = false;

        #endregion Atributos

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase,
        /// inicializa paths e instancia clases.
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="unitClassModelNameWasm"></param>
        /// <param name="unitClassModelNameServer"></param>
        public WoSyntaxEditorFiles(
            string modelName,
            string unitClassModelNameServer,
            string unitClassModelNameWasm,
            bool isGridCode = false
        )
        {
            _isGridCode = isGridCode;

            _modelName = modelName;
            _unitClassModelNameServer = unitClassModelNameServer;
            _unitClassModelNameWasm = unitClassModelNameWasm;

            _projectNameServer = $@"{unitClassModelNameServer}_proj";
            _projectNameWasm = $@"{unitClassModelNameWasm}_proj";

            _pathBlazorServer = $@"{_project.DirApplication}/Temp/ServerUnitModel_proj";
            _pathBlazorWasm = $@"{_project.DirApplication}/Temp/WasmUnitModel_proj";

            _pathScriptUser =
                $@"{_project.DirProyectData}/LayOuts/UserCode/{modelName}_proj/{modelName}ScriptsUser.cs";
            _pathScriptUserPartial =
                $@"{_project.DirProyectData}/LayOuts/UserCode/{modelName}_proj/{modelName}ScriptsUserPartial.cs";
            _pathFluent =
                $@"{_project.DirProyectData}/LayOuts/UserCode/{modelName}_proj/{modelName}Validator.cs";
        }

        #endregion Constructor

        #region Copiar ficheros al generado

        /// <summary>
        /// Actualiza los ficheros al proyecto de blazor para unitarias en caso de existir.
        /// </summary>
        public void UpdateBlazorProject()
        {
            // BlazorServer
            if (File.Exists($@"{_pathBlazorServer}/Program.cs"))
            {
                //ScriptUser
                WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                woSyntaxManagerUserCode.InitializeManager(
                    pathScript: _pathScriptUser,
                    className: _modelName,
                    modelName: _modelName
                );

                string baseModelName = _modelName.Replace("GridList", "");

                string newClassName =
                    (_isGridCode)
                        ? $@"{_unitClassModelNameServer}GridListScriptsUser"
                        : $@"{_unitClassModelNameServer}ScriptsUser";

                string newUnitClassModelNameServer =
                    (_isGridCode)
                        ? $@"{_unitClassModelNameServer}GridList"
                        : _unitClassModelNameServer;

                WoTemplateScriptsUserBlazor woTemplateScriptsUserBlazor =
                    new WoTemplateScriptsUserBlazor();
                woTemplateScriptsUserBlazor.Project = _projectNameServer;
                string rawClass = woSyntaxManagerUserCode
                    .GetClassCodeWithNewName($@"{_modelName}ScriptsUser", newClassName)
                    .Replace(
                        $@"public {_modelName}Controls {_modelName}Controles = {_modelName}Controls.GetInstance();",
                        $@"public {_unitClassModelNameServer}Controls {_modelName}Controles = {_unitClassModelNameServer}Controls.GetInstance();"
                    )
                    .Replace(
                        $@"{_modelName}ScriptsUser",
                        $@"{_unitClassModelNameServer}ScriptsUser"
                    )
                    .Replace($@"{_modelName}Controls", $@"{newUnitClassModelNameServer}Controls")
                    .Replace(
                        $@"{baseModelName}GridListControls",
                        $@"{_unitClassModelNameServer}GridListControls"
                    )
                    .Replace($@"{baseModelName}Controls", $@"{newUnitClassModelNameServer}Controls")
                    .Replace(
                        $@"public {baseModelName}GridList {baseModelName}GridList = new {baseModelName}GridList();",
                        $@"public {baseModelName} {baseModelName} = new {baseModelName}();"
                    );

                Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(m =>
                    m.Id == _modelName
                );
                if (findModel != null)
                {
                    int slaveCount = 0;
                    foreach (ModeloColumna column in findModel.Columnas)
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

                woTemplateScriptsUserBlazor.Code = rawClass;

                string usingsPath =
                    $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Usings.cs";

                string usings = string.Empty;

                if (!File.Exists(usingsPath))
                {
                    fmUsingSelector fmUsingSelector = new fmUsingSelector(_modelName);
                }

                usings = WoDirectory
                    .ReadFile(usingsPath)
                    .Replace(
                        "using System.Private.CoreLib;",
                        $@"using {_projectNameServer}.ControlModels;"
                    );

                woTemplateScriptsUserBlazor.Usings = usings;

                if (_isGridCode)
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorServer}/UserCode/{_unitClassModelNameServer}GridListScriptsUser.cs",
                        data: woTemplateScriptsUserBlazor.TransformText()
                    );
                }
                else
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorServer}/UserCode/{_unitClassModelNameServer}ScriptsUser.cs",
                        data: woTemplateScriptsUserBlazor.TransformText()
                    );
                }

                //ScriptUserPartial
                WoSyntaxManagerUserCodePartial woSyntaxManagerUserCodePartial =
                    new WoSyntaxManagerUserCodePartial();
                woSyntaxManagerUserCodePartial.InitializeManager(
                    pathScript: _pathScriptUserPartial,
                    className: _modelName,
                    modelName: _modelName
                );

                WoTemplateScriptsUserPartialBlazor woTemplateScriptsUserPartialBlazor =
                    new WoTemplateScriptsUserPartialBlazor();
                woTemplateScriptsUserPartialBlazor.Project = _projectNameServer;
                woTemplateScriptsUserPartialBlazor.Code =
                    woSyntaxManagerUserCodePartial.GetClassCodeWithNewName(
                        $@"{_modelName}ScriptsUser",
                        $@"{_unitClassModelNameServer}ScriptsUser"
                    );

                if (_isGridCode)
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorServer}/UserCode/{_unitClassModelNameServer}GridListScriptsUserPartial.cs",
                        data: woTemplateScriptsUserPartialBlazor.TransformText()
                    );
                }
                else
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorServer}/UserCode/{_unitClassModelNameServer}ScriptsUserPartial.cs",
                        data: woTemplateScriptsUserPartialBlazor.TransformText()
                    );
                }

                if (!_isGridCode)
                {
                    //UnitModelValidator
                    WoSyntaxManagerFluentValidation woSyntaxManagerFluentValidation =
                        new WoSyntaxManagerFluentValidation();
                    woSyntaxManagerFluentValidation.InitializeManager(
                        pathScript: _pathFluent,
                        className: _modelName,
                        modelName: _modelName
                    );

                    WoTemplateFluentValidatorBlazor woTemplateFluentValidatorBlazor =
                        new WoTemplateFluentValidatorBlazor();
                    woTemplateFluentValidatorBlazor.Project = _projectNameServer;
                    woTemplateFluentValidatorBlazor.Code =
                        woSyntaxManagerFluentValidation.GetClassCodeWithNewName(
                            $@"{_modelName}Validator",
                            $@"{_unitClassModelNameServer}Validator"
                        );

                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorServer}/FluentValidators/{_unitClassModelNameServer}Validator.cs",
                        data: woTemplateFluentValidatorBlazor.TransformText()
                    );
                }
            }

            // BlazorWasm
            if (File.Exists($@"{_pathBlazorWasm}/Program.cs"))
            {
                //ScriptUser
                WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                woSyntaxManagerUserCode.InitializeManager(
                    pathScript: _pathScriptUser,
                    className: _modelName,
                    modelName: _modelName
                );

                string baseModelName = _modelName.Replace("GridList", "");

                string newClassName =
                    (_isGridCode)
                        ? $@"{_unitClassModelNameWasm}GridListScriptsUser"
                        : $@"{_unitClassModelNameWasm}ScriptsUser";

                string newUnitClassModelNameWasm =
                    (_isGridCode) ? $@"{_unitClassModelNameWasm}GridList" : _unitClassModelNameWasm;

                WoTemplateScriptsUserBlazor woTemplateScriptsUserBlazor =
                    new WoTemplateScriptsUserBlazor();
                woTemplateScriptsUserBlazor.Project = _projectNameWasm;
                woTemplateScriptsUserBlazor.Code = woSyntaxManagerUserCode
                    .GetClassCodeWithNewName($@"{_modelName}ScriptsUser", newClassName)
                    .Replace(
                        $@"public {_modelName}Controls {_modelName}Controles = {_modelName}Controls.GetInstance();",
                        $@"public {_unitClassModelNameWasm}Controls {_modelName}Controles = {_unitClassModelNameWasm}Controls.GetInstance();"
                    )
                    .Replace(
                        $@"{_modelName}ScriptsUser",
                        $@"{newUnitClassModelNameWasm}ScriptsUser"
                    )
                    .Replace(
                        $@"{baseModelName}GridListControls",
                        $@"{_unitClassModelNameWasm}GridListControls"
                    )
                    .Replace($@"{baseModelName}Controls", $@"{newUnitClassModelNameWasm}Controls")
                    .Replace(
                        $@"public {baseModelName}GridList {baseModelName}GridList = new {baseModelName}GridList();",
                        $@"public {baseModelName} {baseModelName} = new {baseModelName}();"
                    );

                string usingsPath =
                    $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Usings.cs";

                string usings = string.Empty;

                if (!File.Exists(usingsPath))
                {
                    fmUsingSelector fmUsingSelector = new fmUsingSelector(_modelName);
                }

                usings = WoDirectory
                    .ReadFile(usingsPath)
                    .Replace(
                        "using System.Private.CoreLib;",
                        $@"using {_projectNameWasm}.ControlModels;"
                    );

                woTemplateScriptsUserBlazor.Usings = usings;

                if (_isGridCode)
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorWasm}/UserCode/{_unitClassModelNameWasm}GridListScriptsUser.cs",
                        data: woTemplateScriptsUserBlazor.TransformText()
                    );
                }
                else
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorWasm}/UserCode/{_unitClassModelNameWasm}ScriptsUser.cs",
                        data: woTemplateScriptsUserBlazor.TransformText()
                    );
                }

                //ScriptUserPartial
                WoSyntaxManagerUserCodePartial woSyntaxManagerUserCodePartial =
                    new WoSyntaxManagerUserCodePartial();
                woSyntaxManagerUserCodePartial.InitializeManager(
                    pathScript: _pathScriptUserPartial,
                    className: _modelName,
                    modelName: _modelName
                );

                WoTemplateScriptsUserPartialBlazor woTemplateScriptsUserPartialBlazor =
                    new WoTemplateScriptsUserPartialBlazor();
                woTemplateScriptsUserPartialBlazor.Project = _projectNameWasm;
                woTemplateScriptsUserPartialBlazor.Code =
                    woSyntaxManagerUserCodePartial.GetClassCodeWithNewName(
                        $@"{_modelName}ScriptsUser",
                        $@"{_unitClassModelNameWasm}ScriptsUser"
                    );

                if (_isGridCode)
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorWasm}/UserCode/{_unitClassModelNameWasm}GridListScriptsUserPartial.cs",
                        data: woTemplateScriptsUserPartialBlazor.TransformText()
                    );
                }
                else
                {
                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorWasm}/UserCode/{_unitClassModelNameWasm}ScriptsUserPartial.cs",
                        data: woTemplateScriptsUserPartialBlazor.TransformText()
                    );
                }

                if (!_isGridCode)
                {
                    //UnitModelValidator
                    WoSyntaxManagerFluentValidation woSyntaxManagerFluentValidation =
                        new WoSyntaxManagerFluentValidation();
                    woSyntaxManagerFluentValidation.InitializeManager(
                        pathScript: _pathFluent,
                        className: _modelName,
                        modelName: _modelName
                    );

                    WoTemplateFluentValidatorBlazor woTemplateFluentValidatorBlazor =
                        new WoTemplateFluentValidatorBlazor();
                    woTemplateFluentValidatorBlazor.Project = _projectNameWasm;
                    woTemplateFluentValidatorBlazor.Code =
                        woSyntaxManagerFluentValidation.GetClassCodeWithNewName(
                            $@"{_modelName}Validator",
                            $@"{_unitClassModelNameWasm}Validator"
                        );

                    WoDirectory.WriteTemplate(
                        pathTemplate: $@"{_pathBlazorWasm}/FluentValidators/{_unitClassModelNameWasm}Validator.cs",
                        data: woTemplateFluentValidatorBlazor.TransformText()
                    );
                }
            }
        }

        #endregion Copiar ficheros al generado
    }
}
