using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using ActiproSoftware.Text.Languages.DotNet.Reflection;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using ServiceStack;
using ServiceStack.Text;
using WooW.Core;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.CodeEditor.CodeComponents;
using WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorSettings;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.CodeEditor
{
    public partial class WoSyntaxEditor : UserControl
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

        #region Variables globales (Componentes)

        /// <summary>
        /// Editor base de código.
        /// </summary>
        private WoBaseEditor _woBaseEditor = null;

        /// <summary>
        /// Editor que muestra el header en la pantalla.
        /// </summary>
        private WoHeaderEditor _woHeaderEditor = null;

        #endregion Variables globales (Componentes)

        #region Variables globales (Gestores de código con Roslyn)

        /// <summary>
        /// Gestiona el fichero con el código de validación de fluent.
        /// </summary>
        private WoSyntaxManagerFluentValidation _woSyntaxManagerFluentValidation =
            new WoSyntaxManagerFluentValidation();

        /// <summary>
        /// Gestiona el fichero con los métodos que se ejecutan en función de la interacción con el control.
        /// </summary>
        private WoSyntaxManagerUserCode _woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();

        /// <summary>
        /// Gestiona el fichero con los métodos y atributos custom del usuario.
        /// </summary>
        private WoSyntaxManagerUserCodePartial _woSyntaxManagerUserCodePartial =
            new WoSyntaxManagerUserCodePartial();

        /// <summary>
        /// Gestiona un fichero auxiliar con el código escrito en el syntax editor para realizar una compilación y
        /// validar la lógica del código.
        /// </summary>
        private WoSyntaxManagerCompilation _woSyntaxManagerCompilation =
            new WoSyntaxManagerCompilation();

        #endregion Variables globales (Gestores de código con Roslyn)

        #region Variables globales

        /// <summary>
        /// Nombre del modelo sobre del que se esta trabajando.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Nombre base de las clases
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Path base del proyecto.
        /// </summary>
        //private string _pathGeneratedProject = string.Empty;

        /// <summary>
        /// Path project data.
        /// La ruta de la carpeta del proyecto donde se guardan los datos salvados del proyecto.
        /// </summary>
        private string _pathSaveDirectory = string.Empty;

        #endregion Variables globales

        #region variables del SyntaxEditor

        /// <summary>
        /// Permite la carga de dlls al SyntaxEditor.
        /// Se inicializa en el constructor y se puede obtener desde la definición del lenguaje de active pro.
        /// </summary>
        private IProjectAssembly _assemblyProject;

        /// <summary>
        /// Corre en segundo plano para ir revisando el código que se va cargando en el syntax y
        /// permitir cosas como la detección de errores de sintaxis.
        /// </summary>
        private BackgroundWorker _worker = new BackgroundWorker();

        /// <summary>
        /// Definición de las reglas de lenguaje
        /// (objeto de active pro)
        /// </summary>
        [SupportedOSPlatform("windows")]
        public CSharpSyntaxLanguage _language = new CSharpSyntaxLanguage();

        /// <summary>
        /// Permite recuperar un contexto para el header y footer, tanto de los
        /// syntax como de los compiladores.
        /// </summary>
        private WoSyntaxEditorHeaderAndFooter _woSyntaxEditorHeaderAndFooter;

        #endregion variables del SyntaxEditor

        #region Información del código sobre del que se esta trabajando

        /// <summary>
        /// Define que tipo de código se esta editando en el syntax.
        /// </summary>
        private eCodeType _typeOfCode = eCodeType.Method;

        /// <summary>
        /// Nombre del método o del constructor sobre el que se esta trabajando.
        /// </summary>
        private string _methodName = string.Empty;

        /// <summary>
        /// Nombre del tipo de método sobre el que se esta trabajando.
        /// </summary>
        private string _methodType = "void";

        /// <summary>
        /// Lista de parámetros que contendrá el método sobre el que se esta trabajando.
        /// </summary>
        private List<(string type, string name, string value)> _methodParamsCol = null;

        /// <summary>
        /// Lista de los atributos que contiene la clase sobre la que se esta trabajando.
        /// </summary>
        private List<string> _attributesCol = new List<string>();

        /// <summary>
        /// Indica si el código con el que se esta trabajado es código de un formulario de estilo libre
        /// o de un formulario convencional.
        /// </summary>
        private bool _isFreeStyle = false;

        /// <summary>
        /// Indica si el código con el que se esta trabajado es código de una grid.
        /// </summary>
        private bool _isGrid = false;

        #endregion Información del código sobre del que se esta trabajando


        #region Constructores

        /// <summary>
        /// Constructor principal de la clase.
        ///     - Inicializa atributos.
        ///     - Suscribe el método para cuando se inicie el worker del editor de código.
        ///     - Arranca el worker.
        ///     - Define el lenguaje con el que trabajara el syntax.
        /// Al compiler context se le pasa el path del copy para trabajar sobre el código de salvado y
        /// no requerir que se encuentre generado el proyecto.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public WoSyntaxEditor(
            string pathDirSaveProject,
            string modelName,
            string classModelName,
            List<(string type, string name, string value)> methodParamsCol = null,
            bool isFreeStyle = false,
            bool isGrid = false
        )
        {
            _pathSyntax = $@"{_project.DirApplication}\Temp\file.cs";
            _isFreeStyle = isFreeStyle;
            _isGrid = isGrid;

            _pathSaveDirectory = pathDirSaveProject;
            _worker = new BackgroundWorker();

            _modelName = modelName;
            _classModelName = classModelName;

            //_pathGeneratedProject = $@"{_project.DirProyectTemp}\{projectName}";
            //if (!File.Exists($@"{_pathGeneratedProject}\Program.cs"))
            //{
            //    _pathGeneratedProject = string.Empty;
            //}

            InitializeComponent();

            _woSyntaxEditorHeaderAndFooter = new WoSyntaxEditorHeaderAndFooter(
                modelName: modelName,
                classModelName: classModelName,
                isFreeStyle: isFreeStyle,
                isList: isGrid
            );

            if (!methodParamsCol.IsNull())
                _methodParamsCol = methodParamsCol;

            InitializeSyntaxManager();

            _assemblyProject = _language.GetService<IProjectAssembly>();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += ChargeDlls;
            _worker.RunWorkerAsync();

            InitializeCodeEditors(
                methodType: "void",
                methodName: "FormularioIniciado",
                typeOfCode: eCodeType.Method
            );
        }

        #endregion Constructores


        #region Eventos desde el editor

        /// <summary>
        /// Se dispara desde un método suscrito a un controlador de eventos del
        /// syntax e informa de una modificación de código.
        /// </summary>
        public Action CodeChangedEvt;

        #endregion Eventos desde el editor


        #region Cargar el código a la clase que usa Roslyn

        /// <summary>
        /// Carga la clase con el código del usuario desde el proyecto de blazor, a la clase
        /// que utiliza Roslyn para operar con el código tokenized.
        /// </summary>
        public void InitializeSyntaxManager()
        {
            if (!(_isFreeStyle || _isGrid))
            {
                _woSyntaxManagerFluentValidation.InitializeManager(
                    pathScript: $@"{_pathSaveDirectory}/{_classModelName}Validator.cs",
                    className: _classModelName,
                    modelName: _modelName
                );
            }

            _woSyntaxManagerUserCode.InitializeManager(
                pathScript: $@"{_pathSaveDirectory}/{_classModelName}ScriptsUser.cs",
                className: _classModelName,
                modelName: _modelName
            );
            _woSyntaxManagerUserCodePartial.InitializeManager(
                pathScript: $@"{_pathSaveDirectory}/{_classModelName}ScriptsUserPartial.cs",
                className: _classModelName,
                modelName: _modelName
            );
        }

        #endregion Cargar el código a la clase que usa Roslyn

        #region Inicializar editores de código

        /// <summary>
        /// Método que inicializa todo el syntax editor:
        ///     - Ambos editores de código.
        ///     - Carga de header y footer base.
        ///     - Código del editor del header.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void InitializeCodeEditors(
            string methodType,
            string methodName,
            eCodeType typeOfCode
        )
        {
            _methodType = methodType;
            _methodName = methodName;
            _typeOfCode = typeOfCode;

            if (!_woBaseEditor.IsNull())
            {
                sccSyntaxEditor.Panel1.Controls.Clear();
                _woHeaderEditor.Dispose();

                sccSyntaxEditor.Panel2.Controls.Clear();
                _woBaseEditor.Dispose();
            }

            //Syntax principal
            _woBaseEditor = new WoBaseEditor(_language);
            _woBaseEditor.Parent = sccSyntaxEditor.Panel2;
            _woBaseEditor.Dock = DockStyle.Fill;
            _woBaseEditor.AssignHeaderAndFooter(GetMainHeaderAndFooter());

            _woBaseEditor.CodeChangedEvt += () =>
            {
                CodeChangedEvt?.Invoke();
            };

            //Syntax del header
            _woHeaderEditor = new WoHeaderEditor(_language);
            _woHeaderEditor.Parent = sccSyntaxEditor.Panel1;
            _woHeaderEditor.Dock = DockStyle.Fill;
            _woHeaderEditor.AssignHeaderAndFooter(GetHeaderHeaderAndFooter());
            BuildHeaderCode();
        }

        #endregion Inicializar editores de código

        #region Carga de contexto de los editores de código

        /// <summary>
        /// Recupera el header y el footer del syntax principal,
        /// lo retorna en una tupla para asignarlo a la instancia del editor de active pro.
        /// </summary>
        /// <returns></returns>
        private (string header, string footer) GetMainHeaderAndFooter()
        {
            ActualizeAttributes();

            string header = _woSyntaxEditorHeaderAndFooter.SyntaxGetHeader(
                codeType: _typeOfCode,
                methodName: _methodName,
                attributesCol: _woSyntaxManagerUserCode.GetAllAttributes(),
                paramsCol: _methodParamsCol,
                methodType: _methodType
            );

            string footer = _woSyntaxEditorHeaderAndFooter.SyntaxGetFooter();

            return (header, footer);
        }

        /// <summary>
        /// Recupera el header y el footer del syntax del header,
        /// lo retorna en una tupla para asignarlo a la instancia del editor de active pro.
        /// </summary>
        /// <returns></returns>
        private (string header, string footer) GetHeaderHeaderAndFooter()
        {
            string header = _woSyntaxEditorHeaderAndFooter.SyntaxHeaderGetHeader();
            string footer = _woSyntaxEditorHeaderAndFooter.SyntaxHeaderGetFooter();
            return (header, footer);
        }

        /// <summary>
        /// Construye el código para mostrar en el apartado del syntax editor del header.
        /// </summary>
        public void BuildHeaderCode()
        {
            ActualizeAttributes();

            StringBuilder strHeader = new StringBuilder();
            strHeader.AppendLine($@"/// Atributos de la clase");
            foreach (var attribute in _attributesCol)
            {
                if (attribute.Contains("GridList"))
                {
                    if (
                        !(
                            attribute.Contains("GridListControls")
                            || attribute.Contains("GridListControles")
                        )
                    )
                    {
                        strHeader.AppendLine(attribute.Replace("GridList", ""));
                    }
                    else
                    {
                        strHeader.AppendLine(attribute.ToString());
                    }
                }
                else
                {
                    strHeader.AppendLine(attribute.ToString());
                }
            }

            StringBuilder strParams = new StringBuilder();
            string method = (_methodName.IsNullOrStringEmpty()) ? "Método" : _methodName.ToString();
            string parameters = string.Empty;
            if (!_methodParamsCol.IsNullOrEmpty())
            {
                foreach (var param in _methodParamsCol)
                {
                    strParams.Append(
                        (param.value.IsNullOrStringEmpty())
                            ? $@"{param.type} {param.name}, "
                            : $@"{param.type} {param.name} = {param.value}, "
                    );
                }
                parameters = strParams.ToString().Substring(0, strParams.Length - 2);
            }

            strHeader.AppendLine(
                $@"
/// Método
public {_methodType} {method}({parameters})
{{
"
            );

            _woHeaderEditor.SetCode(strHeader.ToString());
        }

        /// <summary>
        /// Actualiza la lista de atributos de la clase que se muestra en el editor de código.
        /// </summary>
        public void ActualizeAttributes()
        {
            _attributesCol.Clear();
            _attributesCol.AddRange(_woSyntaxManagerUserCode.GetAllAttributes());
            _attributesCol.AddRange(_woSyntaxManagerUserCodePartial.GetAllAttributes());
        }

        #endregion Carga de contexto de los editores de código

        #region Métodos base del editor de código principal

        /// <summary>
        /// Limpia el SyntaxEditor y reinicia las banderas de control.
        /// </summary>
        public void CleanEditor()
        {
            _woBaseEditor.CleanEditor();
        }

        /// <summary>
        /// Permite retornar un bool para saver si el syntax tiene o no código.
        /// </summary>
        /// <returns></returns>
        public bool HaveEditorCode()
        {
            return _woBaseEditor.HaveCode();
        }

        #endregion Métodos base del editor de código principal

        #region Carga de las dll

        /// <summary>
        /// Método suscrito al controlador de eventos del worker para que se carguen las dlls al momento
        /// que se inicia el hilo del worker para validar el código.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChargeDlls(object sender, EventArgs e)
        {
            _assemblyProject.AssemblyReferences.AddMsCorLib();

            /// System
            foreach (var dll in WoSyntaxEditorOptions.GetBaseDlls())
            {
                try
                {
                    _assemblyProject.AssemblyReferences.AddFrom(dll);
                }
                catch (Exception ex)
                {
                    ///send log
                }
            }
        }

        #endregion Carga de las dll

        #region Búsquedas y validaciones de métodos

        /// <summary>
        /// Consulta los métodos desde ambos apartados de las clases parciales y valida que no se pueda repetir un método con el mismo nombre.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool AlreadyExistMethod(string methodName)
        {
            return (
                _woSyntaxManagerUserCode.AlreadyExistMethod(methodName)
                || _woSyntaxManagerUserCodePartial.AlreadyExistMethod(methodName)
            );
        }

        /// <summary>
        /// Utiliza el método del mismo nombre para consultar una lista de tuplas con el nombre de los métodos custom
        /// y la información de si tienen o no código.
        /// </summary>
        /// <returns></returns>
        public List<(string type, string method, bool haveCode)> GetMethodsInfo(eCodeType eCodeType)
        {
            if (eCodeType == eCodeType.CustomMethod)
            {
                return _woSyntaxManagerUserCodePartial.GetMethodsInfo();
            }
            else if (eCodeType == eCodeType.Method)
            {
                return _woSyntaxManagerUserCode.GetMethodsInfo();
            }
            else
            {
                return new List<(string type, string method, bool haveCode)>();
            }
        }

        /// <summary>
        /// Se apoya del método que recupera el método desde el árbol para validar si el método
        /// tiene o no código.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool HaveCode(string methodName, eCodeType codeType)
        {
            switch (codeType)
            {
                case eCodeType.Method:
                    return _woSyntaxManagerUserCode.HaveCode(methodName);
                case eCodeType.CustomMethod:
                    return _woSyntaxManagerUserCodePartial.HaveCode(methodName);
                case eCodeType.Constructor:
                    return _woSyntaxManagerFluentValidation.ConstructorHaveCode(methodName);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Se apoya de los métodos internos de las clases que manejan el código con Roslyn para
        /// obtener la lista de métodos custom con su estatus de uso y combina el resultado de
        /// los métodos custom con el de los métodos base.
        /// </summary>
        /// <returns></returns>
        public List<(string method, bool used)> ValidateUseCustomMethods()
        {
            List<(string method, bool used)> methodsUsed = new List<(string method, bool used)>();
            List<string> customMethods = _woSyntaxManagerUserCodePartial.GetMethodsName();

            foreach (string method in customMethods)
            {
                methodsUsed.Add(
                    (
                        method,
                        _woSyntaxManagerUserCodePartial.UsedMethod(method)
                            || _woSyntaxManagerUserCode.UsedMethod(method)
                    )
                );
            }

            return methodsUsed;
        }

        #endregion Búsquedas y validaciones de métodos

        #region Carga de métodos

        /// <summary>
        /// Inicializa el syntax.
        /// Carga el código del método al SyntaxEditor y actualiza las banderas para indicar que se
        /// estará trabajando sobre un método.
        /// </summary>
        /// <param name="methodName"></param>
        [SupportedOSPlatform("windows")]
        public void ChargeCode(string methodType, string methodName, eCodeType typeOfCode)
        {
            InitializeSyntaxManager();
            switch (typeOfCode)
            {
                case eCodeType.Method:
                    var methodBase = _woSyntaxManagerUserCode.GetMethod(methodName);
                    _methodParamsCol = methodBase.parameters;
                    string code = string.Empty;
                    if (methodName == "FormularioIniciado")
                    {
                        code = methodBase.body.Replace("FormSettings();", "");
                    }
                    else
                    {
                        code = methodBase.body;
                    }

                    methodType = methodType.Replace("async", "");

                    if (code.Contains("await"))
                    {
                        methodType = $@"async {methodType}";
                    }

                    InitializeCodeEditors(methodType, methodName, typeOfCode);

                    _woBaseEditor.SetCode(code);
                    break;
                case eCodeType.CustomMethod:
                    var methodCustom = _woSyntaxManagerUserCodePartial.GetMethod(methodName);
                    _methodParamsCol = methodCustom.parameters;
                    InitializeCodeEditors(methodType, methodName, typeOfCode);
                    _woBaseEditor.SetCode(methodCustom.body);
                    break;
                case eCodeType.Constructor:
                    _methodParamsCol = null;
                    InitializeCodeEditors(methodType, methodName, typeOfCode);
                    _woBaseEditor.SetCode(
                        _woSyntaxManagerFluentValidation.GetBodyConstructor(methodName)
                    );
                    break;
            }

            CreateTempFile();
        }

        #endregion Carga de métodos

        #region Creación y edición de métodos

        /// <summary>
        /// Salva el código escrito por el usuario en el syntax.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public bool SaveCode()
        {
            if (_typeOfCode == eCodeType.Constructor)
            {
                return WriteCode($@"{_classModelName}Validator");
            }
            else
            {
                return WriteCode(_methodName);
            }
        }

        /// <summary>
        /// Limpia el constructor para que se pueda escribir el nuevo método
        /// y actualiza las banderas para indicar que sera un nuevo método.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CreateNewMethod(
            string type,
            string name,
            List<(string type, string name, string value)> methodParamsCol = null
        )
        {
            _methodParamsCol = methodParamsCol;

            string code = "//Escriba solo el código del cuerpo del nuevo método";

            _woSyntaxManagerUserCodePartial.CreateNewMethod(
                methodName: name,
                bodyMethod: (type == "void") ? code : code + "\nreturn default(" + type + ");",
                typeMethod: type,
                methodParamsCol: methodParamsCol
            );

            InitializeSyntaxManager();

            InitializeCodeEditors(
                methodType: type,
                methodName: name,
                typeOfCode: eCodeType.CustomMethod
            );
        }

        /// <summary>
        /// Compila y valida que el código se pueda salvar.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private bool WriteCode(string name, string type = "void")
        {
            type = CreateTempFile();
            if (CompileCode())
            {
                SaveAndWrite(name, type);

                return true;
            }
            else
            {
                WoBlazorExecute woExecute = WoBlazorExecute.GetInstance();
                if (woExecute.getStatus())
                {
                    MessageBox.Show(
                        "Cuando el proyecto se encuentra en ejecución no puede salvar código con errores",
                        "Alerta"
                    );
                }
                return false;
            }
        }

        /// <summary>
        /// Salva y escribe el código en el fichero de salvado.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        private void SaveAndWrite(string name, string type)
        {
            switch (_typeOfCode)
            {
                case eCodeType.Method:

                    string bodyCode = string.Empty;

                    if (name == "FormularioIniciado")
                    {
                        bodyCode = $"{_woBaseEditor.GetCode()} \n FormSettings();";
                    }
                    else
                    {
                        bodyCode = $"{_woBaseEditor.GetCode()}";
                    }

                    _woSyntaxManagerUserCode.UpdateBodyMethod(name, bodyCode);
                    break;
                case eCodeType.CustomMethod:
                    _woSyntaxManagerUserCodePartial.UpdateBodyMethod(name, _woBaseEditor.GetCode());
                    break;
                case eCodeType.Constructor:
                    _woSyntaxManagerFluentValidation.UpdateBodyCostructor(
                        name,
                        _woBaseEditor.GetCode()
                    );
                    InitializeSyntaxManager();
                    break;
            }
        }

        #endregion Creación y edición de métodos

        #region Eliminar métodos

        /// <summary>
        /// Elimina el método.
        /// </summary>
        /// <param name="methodName"></param>
        public void DeleteMethod(string methodName, bool alerted = false)
        {
            _woSyntaxManagerUserCodePartial.DeleteMethod(methodName, alerted: alerted);
            InitializeSyntaxManager();
            _woBaseEditor.CleanEditor();
            _woHeaderEditor.CleanEditor();
        }

        #endregion Eliminar métodos

        #region Gestión de atributos

        public List<string> GetCustomAttributes()
        {
            return _woSyntaxManagerUserCodePartial.GetAllAttributes();
        }

        /// <summary>
        /// Valida la creación de un nuevo atributo y usa la instancia del manejador de código
        /// para crear el nuevo atributo.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [SupportedOSPlatform("windows")]
        public void CreateNewAttribute(string type, string name, string value)
        {
            switch (type)
            {
                case "bool":
                    value = (value == string.Empty) ? null : value;
                    break;
                case "string":
                    value = (value == string.Empty) ? null : value;
                    break;
                case "char":
                    value = (value == "''") ? null : value;
                    if (value != null)
                    {
                        if (value.Count() > 3)
                        {
                            MessageBox.Show("El tipo char solo puede recibir un carácter");
                            ///send log
                            return;
                        }
                    }
                    break;
                case "int":
                    if (value.ToString().Contains("."))
                    {
                        MessageBox.Show("El tipo int solo puede recibir números enteros");
                        ///send log
                        return;
                    }
                    break;
            }

            _woSyntaxManagerUserCodePartial.CreateAttribute(type, name, value);

            ///Todo: Cambiar por algo que solo actualize parcialmente
            InitializeCodeEditors(_methodType, _methodName, _typeOfCode);
        }

        [SupportedOSPlatform("windows")]
        public void DeleteCustomAttribute(string selectedAttribute)
        {
            _woSyntaxManagerUserCodePartial.DeleteCustomAttribute(selectedAttribute);
            InitializeCodeEditors(_methodType, _methodName, _typeOfCode);
        }

        #endregion Gestión de atributos

        #region Búsqueda y validación de atributos

        public bool AlreadyExistAttribute(string methodName)
        {
            return (
                _woSyntaxManagerUserCode.AlreadyExistAttribute(methodName)
                || _woSyntaxManagerUserCodePartial.AlreadyExistAttribute(methodName)
            );
        }

        #endregion Búsqueda y validación de atributos

        #region Fichero de lo que se esta editando y compilación

        private string _pathSyntax = string.Empty;

        private string _headerCompiler = string.Empty;
        private string _bodyCompiler = string.Empty;
        private string _footerCompiler = string.Empty;

        /// <summary>
        /// Bandera que indica el resultado de la compilación.
        /// </summary>
        private bool _compileResult = true;

        /// <summary>
        /// Crea un fichero temporal con la información de clases y librerías requeridas para realizar una compilación del
        /// código que escribe el usuario en el editor de código y valida que no existan errores.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string CreateTempFile()
        {
            _bodyCompiler = _woBaseEditor.GetCode();

            if (_bodyCompiler.Contains("await") && !_methodType.Contains("async"))
            {
                _methodType = $@"async {_methodType}";
            }
            else if (!_bodyCompiler.Contains("await"))
            {
                _methodType = _methodType.Replace("async", "");
            }

            _headerCompiler = _woSyntaxEditorHeaderAndFooter.SyntaxGetHeader(
                codeType: _typeOfCode,
                methodName: _methodName,
                attributesCol: _woSyntaxManagerUserCode.GetAllAttributes(),
                paramsCol: _methodParamsCol,
                methodType: _methodType
            );

            _footerCompiler = _woSyntaxEditorHeaderAndFooter.SyntaxGetFooter();

            if (File.Exists(_pathSyntax))
            {
                try
                {
                    File.Delete(_pathSyntax);
                }
                catch (Exception ex)
                {
                    //
                    XtraMessageBox.Show(ex.Message);
                }
            }

            WoDirectory.CreateFile(_pathSyntax);
            File.WriteAllText(_pathSyntax, $@"{_headerCompiler} {_bodyCompiler} {_footerCompiler}");

            _woSyntaxManagerCompilation.InitializeManager(
                pathScript: $@"{_project.DirApplication}\Temp\file.cs",
                className: _classModelName,
                modelName: _modelName
            );

            return _methodType;
        }

        [SupportedOSPlatform("windows")]
        private bool CompileCode()
        {
            CreateTempFile();

            (List<ListViewItem> errorList, bool success) compileResult =
                _woSyntaxManagerCompilation.CompileCode(WoSyntaxEditorOptions.GetBaseDlls());

            _woBaseEditor.SetErrorList(compileResult.errorList);

            _compileResult = compileResult.success;

            return compileResult.success;
        }

        #endregion Fichero de lo que se esta editando y compilación


        #region Agregar snipets

        /// <summary>
        /// Pasa el snipet recibido desde el layout al editor de código.
        /// </summary>
        /// <param name="snipet"></param>
        [SupportedOSPlatform("windows")]
        public void SetSnipet(string snipet)
        {
            _woBaseEditor.SetCodeSnipet(snipet);
        }

        #endregion Agregar snipets
    }
}
