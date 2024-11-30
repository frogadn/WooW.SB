using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using DevExpress.DataProcessing;
using WooW.Core;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.BlazorGenerator.BlazorGeneration.BlazorSave;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.ControlModels;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorSettings
{
    public class WoSyntaxEditorHeaderAndFooter
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
        /// Nombre del modelo sobre del que se esta trabajando.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Nombre base de la clase y del fichero sobre del que se trabaja.
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Path del fichero con la clase parcial del código del usuario.
        /// </summary>
        private string _pathPartialClass = string.Empty;

        /// <summary>
        /// Path del fichero con las instancias de las vies de los componentes del formulario.
        /// </summary>
        private string _pathViewsBase = string.Empty;

        /// <summary>
        /// Indicador si es una lista
        /// </summary>
        private bool _isList = false;

        #endregion Atributos

        #region Constructor

        /// <summary>
        /// Constructor base de la clase
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="classModelName"></param>
        public WoSyntaxEditorHeaderAndFooter(
            string modelName,
            string classModelName,
            bool isFreeStyle,
            bool isList
        )
        {
            _isList = isList;
            _modelName = modelName;
            _classModelName = classModelName;

            if (isList)
            {
                _pathPartialClass =
                    (isFreeStyle)
                        ? $@"{_project.DirProyectData}/LayOuts/UserCodeFreeStyle/{modelName}GridList_proj/{classModelName}ScriptsUserPartial.cs"
                        : $@"{_project.DirProyectData}/LayOuts/UserCode/{modelName}GridList_proj/{classModelName}ScriptsUserPartial.cs";
            }
            else
            {
                _pathPartialClass =
                    (isFreeStyle)
                        ? $@"{_project.DirProyectData}/LayOuts/UserCodeFreeStyle/{modelName}_proj/{classModelName}ScriptsUserPartial.cs"
                        : $@"{_project.DirProyectData}/LayOuts/UserCode/{modelName}_proj/{classModelName}ScriptsUserPartial.cs";
            }

            _pathViewsBase =
                (isFreeStyle)
                    ? $@"{_project.DirLayOuts}/UserCodeFreeStyle"
                    : $@"{_project.DirLayOuts}/UserCode";
        }

        #endregion Constructor

        #region Estructura de código

        /// <summary>
        /// Lista de los using que se utilizaran ya sea en la compilación o en el contexto de los scripts en el syntax.
        /// </summary>
        private string _usings =
            $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Private.CoreLib;
using System.Threading.Tasks;
using FluentValidation;

using WooW.Model;

using ServiceStack;

using WooW.DTO;
";

        /// <summary>
        /// Espacio de nombre principal que funge de contexto para el compilador o el contexto de los scripts.
        /// </summary>
        private (string open, string close) _namespace = (
            open: $@"namespace UnitTest_proj {{",
            close: $@"}}"
        );

        #endregion Estructura de código

        #region Métodos para el header del sintaxis

        /// <summary>
        /// Recupera el header del header del sintaxis.
        /// </summary>
        /// <returns></returns>
        public string SyntaxHeaderGetHeader()
        {
            StringBuilder strSyntaxHeader = new StringBuilder();

            string usingsForModel = string.Empty;
            string pathUsingsForModel =
                $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Usings.cs";

            if (File.Exists(pathUsingsForModel))
            {
                usingsForModel = WoDirectory.ReadFile(pathUsingsForModel);
                strSyntaxHeader.AppendLine(usingsForModel);
            }
            else
            {
                strSyntaxHeader.AppendLine(_usings);
            }

            strSyntaxHeader.AppendLine(_namespace.open);

            strSyntaxHeader.AppendLine($@"public class {_classModelName}Validator {{");

            return strSyntaxHeader.ToString();
        }

        /// <summary>
        /// Recupera el footer del header del sintaxis.
        /// </summary>
        /// <returns></returns>
        public string SyntaxHeaderGetFooter()
        {
            StringBuilder strSyntaxFooter = new StringBuilder();
            strSyntaxFooter.AppendLine($@"}}");
            strSyntaxFooter.AppendLine(_namespace.close);
            return strSyntaxFooter.ToString();
        }

        #endregion Métodos para el header del sintaxis

        #region Métodos para el syntax y el compilador

        /// <summary>
        /// Nombre del método sobre del que se va a trabajar.
        /// </summary>
        private string _methodName = string.Empty;

        /// <summary>
        /// Indica si el método es propio del usuario o de los eventos de los controles.
        /// </summary>
        private eCodeType _codeType = eCodeType.Constructor;

        /// <summary>
        /// Construye el header para el syntax y para el compiler.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="attributesCol"></param>
        /// <param name="paramsCol"></param>
        /// <param name="methodType"></param>
        /// <returns></returns>
        public string SyntaxGetHeader(
            eCodeType codeType,
            string methodName,
            List<string> attributesCol,
            List<(string type, string name, string value)> paramsCol = null,
            string methodType = "async void"
        )
        {
            methodType = (methodType == "") ? "void" : methodType;
            _methodName = methodName;
            _codeType = codeType;

            //Attributes
            StringBuilder attributes = new StringBuilder();
            foreach (var attribute in attributesCol)
            {
                if (attribute.Contains("GridList"))
                {
                    if (!attribute.Contains("GridListControls"))
                    {
                        attributes.AppendLine(attribute.Replace("GridList", ""));
                    }
                    else
                    {
                        attributes.AppendLine(attribute.ToString());
                    }
                }
                else
                {
                    attributes.AppendLine(attribute.ToString());
                }
            }

            //Parameter (solo para métodos custom)
            string parameters = string.Empty;
            string values = string.Empty;
            StringBuilder parametersData = new StringBuilder();
            StringBuilder parametersValue = new StringBuilder();
            if (codeType == eCodeType.CustomMethod)
            {
                if (paramsCol != null && paramsCol.Count > 0)
                {
                    foreach (var param in paramsCol)
                    {
                        parametersData.AppendLine($@"{param.type} {param.name}, ");
                        string value = string.Empty;
                        if (param.value.IsNullOrStringEmpty())
                        {
                            switch (param.type)
                            {
                                case "string":
                                    value = $@"""example""";
                                    break;
                                case "int":
                                    value = "0";
                                    break;
                                case "char":
                                    value = "a";
                                    break;
                                case "bool":
                                    value = "true";
                                    break;
                                case "double":
                                    value = "0";
                                    break;
                                case "DateTime":
                                    value = "DateTime.Now";
                                    break;
                            }
                        }
                        else
                        {
                            value = param.value.ToString();
                        }
                        parametersValue.AppendLine($@"{param.name}: {value}, ");
                    }

                    values = parametersValue.ToString();
                    values = values.Substring(0, values.Length - 4);

                    parameters = parametersData.ToString();
                    parameters = parameters.Substring(0, parameters.Length - 4);
                }
            }
            else if (_methodName == "OnSelectedRowChanged")
            {
                parameters = $@"{_modelName} SelectedRow";
                values = $@"new {_modelName}()";
            }

            string baseMainClass =
                $@"
public class Program
{{
    public static void Main(string[] args)
    {{
        {_classModelName}ScriptsUser su = new {_classModelName}ScriptsUser();
        su.MainMethod({values});
    }}
}}";

            StringBuilder strSyntaxHeader = new StringBuilder();

            string usingsForModel = string.Empty;
            string pathUsingsForModel =
                $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Usings.cs";

            if (File.Exists(pathUsingsForModel))
            {
                usingsForModel = WoDirectory.ReadFile(pathUsingsForModel);
                strSyntaxHeader.AppendLine(
                    usingsForModel
                        .Replace("using WooW.Blazor.Resources;", "")
                        .Replace("using WooW.Blazor;", "")
                );
            }
            else
            {
                strSyntaxHeader.AppendLine(_usings);
            }

            strSyntaxHeader.AppendLine(_namespace.open);

            strSyntaxHeader.AppendLine(baseMainClass);

            if (codeType == eCodeType.Constructor)
            {
                strSyntaxHeader.AppendLine(
                    $@"
public partial class {_classModelName}ScriptsUser{{ {attributes} public void MainMethod(){{}} }}
public partial class {_classModelName}Validator : AbstractValidator<{_modelName}>
{{
    {attributes}

    public {_classModelName}Validator()
    {{"
                );
            }
            else
            {
                strSyntaxHeader.AppendLine(
                    $@"
public partial class {_classModelName}ScriptsUser
{{
    {attributes}

    public {methodType} MainMethod({parameters})
    {{"
                );
            }

            return strSyntaxHeader.ToString();
        }

        /// <summary>
        /// Construye el footer para el syntax y para el compilador.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public string SyntaxGetFooter()
        {
            //Clase parcial
            WoSyntaxManagerUserCodePartial woSyntaxManagerUserCodePartial =
                new WoSyntaxManagerUserCodePartial();
            woSyntaxManagerUserCodePartial.InitializeManager(
                pathScript: _pathPartialClass,
                className: _classModelName,
                modelName: _modelName
            );
            if (_codeType == eCodeType.CustomMethod)
            {
                woSyntaxManagerUserCodePartial.DeleteMethodOnlyTree(_methodName);
            }
            string partialClass = woSyntaxManagerUserCodePartial.GetClassCode(
                $@"{_classModelName}ScriptsUser"
            );

            //Model controls
            //string modelControls = WoDirectory.ReadFile(
            //    (_isList)
            //        ? $@"{_pathViewsBase}\{_modelName}GridList_proj\{_classModelName}Controls.cs"
            //        : $@"{_pathViewsBase}\{_modelName}_proj\{_classModelName}Controls.cs"
            //);

            string modelControls = string.Empty;

            if (_isList)
            {
                modelControls = WoDirectory.ReadFile(
                    $@"{_pathViewsBase}\{_modelName}GridList_proj\{_classModelName}Controls.cs"
                );
            }
            else
            {
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                WoContainer fullDesign = woProjectDataHelper.GetFullDesing(_modelName);

                WoControlModelGenerator woControlModelGenerator = new WoControlModelGenerator(
                    _modelName
                );
                modelControls = woControlModelGenerator.GetControlModelClass(fullDesign);
            }

            StringBuilder modelsControls = new StringBuilder();
            Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(m => m.Id == _modelName);
            if (findModel != null)
            {
                foreach (ModeloColumna column in findModel.Columnas)
                {
                    if (column.TipoControl == WoTypeControl.CollectionEditor)
                    {
                        string controlPath =
                            $@"{_pathViewsBase}\{column.Id.Replace("Col", "")}_proj\{column.Id.Replace("Col", "")}Controls.cs";

                        if (!File.Exists(controlPath))
                        {
                            WoBlazorSave woBlazorSave = new WoBlazorSave(
                                classModelName: column.Id.Replace("Col", "")
                            );
                            WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                                new WoDesignerRawSerializerHelper();

                            woBlazorSave.BuildBaseSave(
                                woDesignerRawSerializerHelper.BuildRawWoContainer(
                                    column.Id.Replace("Col", "")
                                )
                            );
                        }

                        string rawClass = string.Empty;

                        if (!_isList)
                        {
                            string className = column.Id.Replace("Col", "");
                            rawClass = WoDirectory.ReadFile(controlPath);
                            rawClass = rawClass.Replace(
                                $@"{className}Controls",
                                $@"{className}SlaveControls"
                            );
                        }

                        modelsControls.AppendLine(rawClass);
                    }
                }
            }

            //View base
            string viewBase = WoDirectory.ReadFile(
                $@"{_project.DirProyectData}/Assembly/AWoEditViewBase.cs"
            );

            //Data resources
            string dataResources = WoDirectory.ReadFile(
                $@"{_project.DirProyectData}/Assembly/WoDesignOptions.cs"
            );

            StringBuilder strSyntaxFooter = new StringBuilder();

            strSyntaxFooter.AppendLine("\n\n} }");
            strSyntaxFooter.AppendLine(dataResources);
            strSyntaxFooter.AppendLine(partialClass);
            strSyntaxFooter.AppendLine(modelControls);
            strSyntaxFooter.AppendLine(modelsControls.ToString());
            strSyntaxFooter.AppendLine(viewBase);
            strSyntaxFooter.AppendLine(GetScriptUserJs());
            strSyntaxFooter.AppendLine(_namespace.close);

            return strSyntaxFooter.ToString();
        }

        private string GetScriptUserJs()
        {
            return $@"
public class IJSRuntime
    {{
        public void InvokeAsync<T>(string NombreMetodo, T parametros)
        {{
        }}
        public void InvokeAsync<T>(string NombreMetodo, T parametro1, T parametro2)
        {{
        }}
        public void InvokeAsync<T>(string NombreMetodo, T parametro1, T parametro2, T parametro3)
        {{
        }}

        public void InvokeVoidAsync(string NombreMetodo, string parametros)
        {{
        }}
        public void InvokeVoidAsync(string NombreMetodo, string parametro1, string parametro2)
        {{
        }}
        public void InvokeVoidAsync(string NombreMetodo, string parametro1, string parametro2, byte[] parametro3)
        {{
        }}
    }}
";
        }

        #endregion Métodos para el syntax y el compilador


        #region Base default de los usings

        public static string GetMainUsings()
        {
            return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Private.CoreLib;
using System.Threading.Tasks;
using FluentValidation;

using WooW.Model;

using ServiceStack;

using WooW.DTO;

using WooW.Blazor;
using WooW.Blazor.Resources; 

using Microsoft.JSInterop;
";
        }

        #endregion Base default de los usings
    }
}
