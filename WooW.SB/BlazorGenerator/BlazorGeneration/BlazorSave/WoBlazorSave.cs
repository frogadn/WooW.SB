using System;
using System.IO;
using System.Runtime.Versioning;
using DevExpress.XtraEditors;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.ControlModels;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.FluentValidators;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.UserCode;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorGeneration.BlazorSave
{
    public class WoBlazorSave
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
        /// Nombre de la clase y del fichero que se va a generar.
        /// Como no se puede cambiar el nombre de un fichero cuando se esta ejecutando
        /// el watch esto permite que
        /// en caso de las pruebas unitarias se puedan llamar todas las clases igual.
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Ruta de los ficheros que se van a generar.
        /// </summary>
        private string _pathSaveCopy = string.Empty;

        /// <summary>
        /// Bandera que indica si el modelo es de tipo free layout.
        /// </summary>
        private bool _isFreeLayout = false;

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="classModelName"></param>
        /// <param name="isFreeLayout"></param>
        public WoBlazorSave(
            string classModelName,
            bool isFreeLayout = false,
            Proyecto project = null
        )
        {
            if (project != null)
            {
                _project = project;
            }

            _classModelName = classModelName;
            _isFreeLayout = isFreeLayout;

            if (isFreeLayout)
            {
                _pathSaveCopy = $@"{_project.DirLayOuts}\UserCodeFreeStyle\{_classModelName}_proj";
            }
            else
            {
                _pathSaveCopy = $@"{_project.DirLayOuts}\UserCode\{_classModelName}_proj";
            }

            BuildDirectory();
        }

        #endregion Constructor

        #region Construcción del directorio

        /// <summary>
        /// Valida que el directorio donde se guardaran los ficheros exista,
        /// en caso de no existir lo crea.
        /// </summary>
        private void BuildDirectory()
        {
            if (!Directory.Exists(_pathSaveCopy))
            {
                WoDirectory.CreateDirectory(_pathSaveCopy);
            }
        }

        #endregion Construcción del directorio


        #region Método principal

        /// <summary>
        /// Método ocupado de orquestar la ejecución del resto de métodos,
        /// permite la creación de todos los ficheros de salvado.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BuildBaseSave(WoContainer container, bool rewrite = false)
        {
            if (!_isFreeLayout)
            {
                BuildFluent(container, rewrite);
            }

            BuildScriptUser(container, rewrite);
            BuildScriptUserPartial(container, rewrite);
            BuildControlModels(container, rewrite);
        }

        #endregion Método principal


        #region FluentValidators

        /// <summary>
        /// Instancia del generador de la clase de fluent validar.
        /// </summary>
        private WoValidatorGenerator _woValidatorGenerator;

        /// <summary>
        /// Construye y escribe la clase de fluent validar en el path de copy.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BuildFluent(WoContainer container, bool rewrite = false)
        {
            try
            {
                _woValidatorGenerator = new WoValidatorGenerator(_classModelName);

                string path = $@"{_pathSaveCopy}\{_classModelName}Validator.cs";
                if (!File.Exists(path) || rewrite)
                {
                    WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                        new WoTemplateGenericClassBlazor();
                    woTemplateGenericClassBlazor.Code = _woValidatorGenerator.GetValidatorClass();

                    WoDirectory.WriteTemplate(
                        pathTemplate: path,
                        data: woTemplateGenericClassBlazor.TransformText()
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar generar el fichero de fluent para el modelo {container.ModelId} en el path: {_pathSaveCopy}. {ex.Message}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
            }
        }

        #endregion FluentValidators


        #region ScriptUser

        /// <summary>
        /// Instancia del generador de la clase con los métodos para cada componente.
        /// </summary>
        private WoScriptsUserGenerator _woScriptUserGenerator;

        /// <summary>
        /// Construye y escribe la clase con los métodos de los eventos para cada componente.
        /// </summary>
        /// <param name="rewrite"></param>
        [SupportedOSPlatform("windows")]
        public void BuildScriptUser(WoContainer container, bool rewrite = false)
        {
            try
            {
                if (container.ModelId == "ExtensionDeVista")
                {
                    var x = 0;
                }
                _woScriptUserGenerator = new WoScriptsUserGenerator(_classModelName);

                string path = $@"{_pathSaveCopy}\{_classModelName}ScriptsUser.cs";
                if (!File.Exists(path) || rewrite)
                {
                    WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                        new WoTemplateGenericClassBlazor();
                    woTemplateGenericClassBlazor.Code = _woScriptUserGenerator.GetScriptsUserClass(
                        container
                    );

                    WoDirectory.WriteTemplate(
                        pathTemplate: path,
                        data: woTemplateGenericClassBlazor.TransformText()
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar generar el fichero de ScriptsUser para el modelo {container.ModelId} en el path: {_pathSaveCopy}. {ex.Message}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
            }
        }

        #endregion ScriptUser


        #region ScriptUserPartial

        /// <summary>
        /// Instancia del generador de la clase que contendrá el código custom del usuario, métodos y atributos.
        /// </summary>
        private WoScriptsUserPartialGenerator _woScriptsUserPartialGenerator;

        /// <summary>
        /// Construye la clase parcial del código del usuario.
        /// </summary>
        /// <param name="rewrite"></param>
        [SupportedOSPlatform("windows")]
        public void BuildScriptUserPartial(WoContainer container, bool rewrite = false)
        {
            try
            {
                _woScriptsUserPartialGenerator = new WoScriptsUserPartialGenerator(_classModelName);

                string path = $@"{_pathSaveCopy}\{_classModelName}ScriptsUserPartial.cs";
                if (!File.Exists(path) || rewrite)
                {
                    WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                        new WoTemplateGenericClassBlazor();
                    woTemplateGenericClassBlazor.Code =
                        _woScriptsUserPartialGenerator.GetScriptsUserClass();

                    WoDirectory.WriteTemplate(
                        pathTemplate: path,
                        data: woTemplateGenericClassBlazor.TransformText()
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar generar el fichero de Scripts User Partial para el modelo {container.ModelId} en el path: {_pathSaveCopy}. {ex.Message}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
            }
        }

        #endregion ScriptUserPartial


        #region ControlModels

        /// <summary>
        /// Instancia del generador de clases del control.
        /// </summary>
        private WoControlModelGenerator _woControlModelGenerator;

        /// <summary>
        /// Construye la clase de controlModels.
        /// </summary>
        /// <param name="rewrite"></param>
        [SupportedOSPlatform("windows")]
        public void BuildControlModels(WoContainer container, bool rewrite = false)
        {
            try
            {
                _woControlModelGenerator = new WoControlModelGenerator(_classModelName);

                string path = $@"{_pathSaveCopy}\{_classModelName}Controls.cs";

                if (!File.Exists(path) || rewrite)
                {
                    WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                        new WoTemplateGenericClassBlazor();
                    woTemplateGenericClassBlazor.Code =
                        _woControlModelGenerator.GetControlModelClass(container);

                    WoDirectory.WriteTemplate(
                        pathTemplate: path,
                        data: woTemplateGenericClassBlazor.TransformText()
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar generar el fichero de ControlModels para el modelo {container.ModelId} en el path: {_pathSaveCopy}. {ex.Message}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
            }
        }

        #endregion ControlModels
    }
}
