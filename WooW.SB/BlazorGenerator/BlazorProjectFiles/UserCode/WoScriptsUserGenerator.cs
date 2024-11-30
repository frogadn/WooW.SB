using System;
using System.Linq;
using System.Text;
using WooW.Core;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoListGrid;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.CheckBoxEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoComboEnumEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoDateEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoFileEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpDialogEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMaskedEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMemoEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoSpinEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoTextEdit;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.UserCode
{
    public class WoScriptsUserGenerator
    {
        #region Instancias singleton

        /// <summary>
        /// Proyecto que contiene la clase que se esta generando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Nombre de la clase y fichero
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        private bool _isGrid = false;

        #endregion Variables globales

        #region Instancias de las clases que generan los eventos de los controles

        private WoTextEditScriptUser _woTextEditScriptUser = null;
        private WoMaskedEditScriptUser _woMaskedEditScriptUser = null;
        private WoMemoEditScriptUser _woMemoEditScriptUser = null;
        private WoSpinEditScriptUser _woSpinEditScriptUser = null;
        private WoDateEditScriptUser _woDateEditScriptUser = null;
        private WoLookUpEditScriptUser _woLookUpEditScriptUser = null;
        private WoComboEnumScriptUser _woComboEnumScriptUser = null;
        private WoLookUpDialogEditScriptUser _woLookUpDialogEditScriptUser = null;
        private WoCheckBoxEditScriptUser _woCheckBoxEditScriptUser = null;
        private WoFileEditScriptUser _woFileEditScriptUser = null;

        private WoListGridScriptUser _woListGridScriptUser = null;

        #endregion Instancias de las clases que generan los eventos de los controles

        #region Constructor

        /// <summary>
        /// Asignaa el nombre de a clase, calcula la identacion del codigo a generar e instancia las clases que generan los scriptsUser.
        /// </summary>
        /// <param name="className"></param>
        public WoScriptsUserGenerator(string classModelName)
        {
            _classModelName = classModelName;
            _isGrid = classModelName.Contains("GridList");

            CalculateIdentSpaces();

            _woTextEditScriptUser = new WoTextEditScriptUser(_identMethodsAndProperties);
            _woMaskedEditScriptUser = new WoMaskedEditScriptUser(_identMethodsAndProperties);
            _woMemoEditScriptUser = new WoMemoEditScriptUser(_identMethodsAndProperties);
            _woSpinEditScriptUser = new WoSpinEditScriptUser(_identMethodsAndProperties);
            _woDateEditScriptUser = new WoDateEditScriptUser(_identMethodsAndProperties);
            _woLookUpEditScriptUser = new WoLookUpEditScriptUser(_identMethodsAndProperties);
            _woLookUpDialogEditScriptUser = new WoLookUpDialogEditScriptUser(
                _identMethodsAndProperties
            );
            _woComboEnumScriptUser = new WoComboEnumScriptUser(_identMethodsAndProperties);
            _woCheckBoxEditScriptUser = new WoCheckBoxEditScriptUser(_identMethodsAndProperties);
            _woFileEditScriptUser = new WoFileEditScriptUser(_identMethodsAndProperties);

            _woListGridScriptUser = new WoListGridScriptUser(_identMethodsAndProperties);
        }

        #endregion Constructor

        #region Identación

        /// <summary>
        /// Nivel de identación para grupos internos y controles.
        /// </summary>
        private int _identLevel = 0;

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de la clase;
        /// </summary>
        private String _identClass = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de las propiedades;
        /// </summary>
        private String _identMethodsAndProperties = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado del código dentro de los métodos;
        /// </summary>
        private String _identCode = "";

        /// <summary>
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identClass = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identMethodsAndProperties = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identCode = FormatClassHelper.Ident(_identLevel);
        }

        #endregion Identación

        #region Método principal

        /// <summary>
        /// Método principal que retorna la clase ya construida, igual se ocupa de orquestar
        /// el resto de métodos.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public string GetScriptsUserClass(WoContainer baseContainer)
        {
            _finalClass.Clear();

            string mainSettings =
                (baseContainer.Id == "RootNoIndexed") ? string.Empty : GetMainConfig(baseContainer);

            BuildHeaderClass(mainSettings, baseContainer);

            BuildBodyClass(baseContainer);

            BuilFooterClass();

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Configuración del formulario

        /// <summary>
        /// Retorna el código en el método de inicio del formulario en los scripts del usuario.
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        public string GetMainConfig(WoContainer baseContainer)
        {
            StringBuilder _strInitialFormCode = new StringBuilder();

            /// Métodos de la personalización de los controles para los contenedores
            _strInitialFormCode.AppendLine(
                $@"{_identCode}///Todo, vistas para los contenedores vista de {baseContainer.Id}."
            );

            if (baseContainer.BackgorundColorGroup != eGroupColor.Default)
            {
                _strInitialFormCode.AppendLine(
                    $@"{_identCode}{_classModelName}Controles.{baseContainer.Id}.ColorDeFondo(eGroupColor.{baseContainer.BackgorundColorGroup.ToString()});"
                );
            }
            if (baseContainer.ComponentFontColor != eTextColor.FontDefault)
            {
                _strInitialFormCode.AppendLine(
                    $@"{_identCode}{_classModelName}Controles.{baseContainer.Id}.ColorDeLetra(eTextColor.{baseContainer.ComponentFontColor});"
                );
            }

            _strInitialFormCode.AppendLine(
                $@"
{_identCode}{_classModelName}Controles.{baseContainer.Id}.GrosorDeLetra(eTextWeight.{baseContainer.ComponentFontWide});
{_identCode}{_classModelName}Controles.{baseContainer.Id}.Italica(eTextItalic.{baseContainer.ComponentFontItalic});
{_identCode}{_classModelName}Controles.{baseContainer.Id}.DecoracionDeLetra(eTextDecoration.{baseContainer.ComponentFontDecoration});"
            );

            if (baseContainer.ContainersCol != null && baseContainer.ContainersCol.Count > 0)
            {
                foreach (var container in baseContainer.ContainersCol)
                {
                    _strInitialFormCode.AppendLine(GetMainConfig(container));
                }
            }

            if (baseContainer.ItemsCol != null && baseContainer.ItemsCol.Count > 0)
            {
                foreach (var item in baseContainer.ItemsCol)
                {
                    _strInitialFormCode.AppendLine($@"{_identCode}// Start code for {item.Id}");

                    if (
                        item.Control == "Text"
                        || item.Control == "Date"
                        || item.Control == "Spin"
                        || item.Control == "Decimal"
                        || item.Control == "Memo"
                        || item.Control == "WoState"
                        || item.Control == "EnumInt"
                        || item.Control == "EnumString"
                        || item.Control == "LookUp"
                        || item.Control == "LookUpDialog"
                        || item.Control == "TextMask"
                    )
                    {
                        if (item.BackgorundColorContainerItem != eContainerItemColor.Default)
                        {
                            _strInitialFormCode.AppendLine(
                                $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}.ColorDeFondo(eContainerItemColor.{item.BackgorundColorContainerItem.ToString()});"
                            );
                        }
                        if (item.CaptionColor != eTextColor.FontDefault)
                        {
                            _strInitialFormCode.AppendLine(
                                $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}Container.ColorDeLetraDeTitulo(eTextColor.{item.CaptionColor.ToString()});"
                            );
                        }
                        if (item.ComponentFontColor != eTextColor.FontDefault)
                        {
                            _strInitialFormCode.AppendLine(
                                $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}.ColorDeLetraDeControl(eTextColor.{item.ComponentFontColor.ToString()});"
                            );
                        }

                        _strInitialFormCode.AppendLine(
                            $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}Container.GrosorDeLetraDeTitulo(eTextWeight.{item.CaptionWide.ToString()});"
                        );
                        _strInitialFormCode.AppendLine(
                            $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}.GrosorDeLetraDeControl(eTextWeight.{item.ComponentFontWide.ToString()});"
                        );

                        _strInitialFormCode.AppendLine(
                            $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}.ControlEnItalica(eTextItalic.{item.ComponentFontItalic.ToString()});"
                        );
                        _strInitialFormCode.AppendLine(
                            $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}Container.TtuloEnItalica(eTextItalic.{item.CaptionItalic.ToString()});"
                        );

                        _strInitialFormCode.AppendLine(
                            $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}Container.DecoracionDeLetraDeTitulo(eTextDecoration.{item.CaptionDecoration.ToString()});"
                        );
                        _strInitialFormCode.AppendLine(
                            $@"{_identCode}{_classModelName}Controles.{item.BindedProperty}.DecoracionDeLetraDeControl(eTextDecoration.{item.ComponentFontDecoration.ToString()});"
                        );
                    }
                }
            }

            return _strInitialFormCode.ToString();
        }

        #endregion Configuración del formulario

        #region Usings

        /// <summary>
        /// Recupera los usings nesesarios para la clase de los scripts para los
        /// reportes de odata.
        /// </summary>
        /// <returns></returns>
        public string GetODataUsings(string projectName)
        {
            return $@"
using Microsoft.JSInterop;
using {projectName}.ControlModels;
using {projectName}.ReportForms;
using ServiceStack;
using WooW.Blazor.Resources;";
        }

        #endregion Usings

        #region Header

        /// <summary>
        /// Construye el Header de la clase.
        /// </summary>
        private void BuildHeaderClass(string mainSettings, WoContainer woContainer)
        {
            string pathClassModelName =
                $@"{_project.DirLayOuts}\UserCode\{woContainer.ModelId}_proj\{woContainer.ModelId}ScriptsUser.cs";

            _finalClass.AppendLine(
                $@"
{_identClass}public partial class {_classModelName}ScriptsUser
{_identClass}{{

{_identClass}    /// <summary>
{_identClass}    /// Inyección del runtime de blazor para la ejecución de código js por parte del usuario.
{_identClass}    /// </summary>
{_identClass}    public IJSRuntime JS {{ get; set; }}

{_identClass}    /// <summary>
{_identClass}    /// Permite al usuario detonar un evento de actualización de la vista.
{_identClass}    /// </summary>
{_identClass}    public Action? StateHasChangeEvt {{ get; set; }}

{_identClass}    /// <summary>
{_identClass}    /// Indica cual es la transición en la que se encuentra el formulario actualmente.
{_identClass}    /// </summary>
{_identClass}    public string Transicion {{ get; set; }} = ""Navegacion"";
"
            );

            Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(m =>
                m.Id == woContainer.ModelId
            );

            if (!_isGrid)
            {
                if (findModel != null)
                {
                    foreach (ModeloColumna column in findModel.Columnas)
                    {
                        if (column.TipoControl == WoTypeControl.CollectionEditor)
                        {
                            _finalClass.AppendLine(
                                $@"public {column.Id.Replace("Col", "")}SlaveControls {column.Id.Replace("Col", "")}Controles {{ get; set; }}"
                            );
                        }
                    }
                }
            }

            _finalClass.AppendLine(
                $@"
{_identClass}    /// <summary>
{_identClass}    /// Asigna las instancias inyectadas desde el despachador de dependencias de blazor.
{_identClass}    /// </summary>
{_identClass}    public {_classModelName}ScriptsUser({_classModelName}Controls {_classModelName}Controles, IJSRuntime js"
            );

            if (!_isGrid)
            {
                if (findModel != null)
                {
                    foreach (ModeloColumna column in findModel.Columnas)
                    {
                        if (column.TipoControl == WoTypeControl.CollectionEditor)
                        {
                            _finalClass.Append(
                                $@", {column.Id.Replace("Col", "")}SlaveControls {column.Id.Replace("Col", "")}Controles"
                            );
                        }
                    }
                }
            }

            _finalClass.Append(
                $@")
{_identClass}    {{
{_identClass}        this.{_classModelName}Controles = {_classModelName}Controles;
{_identClass}        this.JS = js;
"
            );

            if (!_isGrid)
            {
                if (findModel != null)
                {
                    foreach (ModeloColumna column in findModel.Columnas)
                    {
                        if (column.TipoControl == WoTypeControl.CollectionEditor)
                        {
                            _finalClass.AppendLine(
                                $@"this.{column.Id.Replace("Col", "")}Controles = {column.Id.Replace("Col", "")}Controles;"
                            );
                        }
                    }
                }
            }

            _finalClass.Append(
                $@"
{_identClass}    }}"
            );

            _finalClass.AppendLine(
                $@"
{_identClass}    #region Atributos (modelos y controles)"
            );

            _finalClass.AppendLine(
                $@"


{_identClass}    /// <summary>
{_identClass}    /// Cliente que se puede usar en el codigo para llamar a los servicios.
{_identClass}    /// </summary>
{_identClass}    public JsonApiClient woTarget {{ get; set; }}

{_identClass}    /// <summary>
{_identClass}    /// Instancia de la clase que contiene los valores bindeados a los controles de la vista.
{_identClass}    /// </summary>
{_identClass}    public {_classModelName} {_classModelName} = new {_classModelName}();

{_identClass}    /// <summary>
{_identClass}    /// Inyeccion del la clase que contiene las vistas de los controles de la vista.
{_identClass}    /// </summary>
{_identClass}    public {_classModelName}Controls {_classModelName}Controles {{ get; set; }}
"
            );

            if (_isGrid)
            {
                _finalClass.AppendLine(
                    $@"
{_identClass}    /// <summary>
{_identClass}    /// Lista de los elementos seleccionados en la lista
{_identClass}    /// </summary>
{_identClass}    public List<{woContainer.ModelId}> {woContainer.ModelId}Seleccionados = new List<{woContainer.ModelId}>();"
                );
            }

            _finalClass.AppendLine(
                $@"
{_identClass}    #endregion Atributos (modelos y controles)"
            );

            _finalClass.AppendLine(
                $@"
{_identClass}    #region Formulario

{_identClass}    /// <summary>
{_identClass}    /// Metodo que se ejecuta cuando se inicia el formulario.
{_identClass}    /// </summary>
{_identClass}    /// <returns></returns>
{_identClass}    public void FormularioIniciado()
{_identClass}    {{
{_identClass}        // Codigo que se ejecutara al iniciar el formulario.
{_identClass}        FormSettings();
{_identClass}    }}

{_identClass}    /// <summary>
{_identClass}    /// Metodo con la configuración del formulario, se llama desde el metodo FormularioIniciado.
{_identClass}    /// </summary>
{_identClass}    /// <returns></returns>
{_identClass}    private void FormSettings()
{_identClass}    {{
{_identClass}        {mainSettings}    
{_identClass}    }}

{_identClass}    #endregion Formulario"
            );
        }

        #endregion Header

        #region Footer

        /// <summary>
        /// Construye el footer de la clase.
        /// </summary>
        private void BuilFooterClass()
        {
            _finalClass.AppendLine($@"{_identClass}}}");
        }

        #endregion Footer

        #region Body

        /// <summary>
        /// Construye el body de la clase con el código de los observadores de
        /// todos los items del grupo base.
        /// </summary>
        /// <param name="container"></param>
        public void BuildBodyClass(WoContainer container)
        {
            if (!container.ContainersCol.IsNull())
            {
                foreach (var subContainer in container.ContainersCol)
                {
                    BuildBodyClass(subContainer);
                }
            }

            if (!container.ItemsCol.IsNull())
            {
                foreach (WoItem item in container.ItemsCol)
                {
                    if (item.Control == "Text")
                    {
                        _finalClass.AppendLine(_woTextEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "Memo")
                    {
                        _finalClass.AppendLine(_woMemoEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "Spin")
                    {
                        _finalClass.AppendLine(_woSpinEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "Date")
                    {
                        _finalClass.AppendLine(_woDateEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "LookUp")
                    {
                        _finalClass.AppendLine(_woLookUpEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "LookUpDialog")
                    {
                        _finalClass.AppendLine(_woLookUpDialogEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "Bool")
                    {
                        _finalClass.AppendLine(_woCheckBoxEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "EnumString" || item.Control == "EnumInt")
                    {
                        _finalClass.AppendLine(_woComboEnumScriptUser.GetCode(item));
                    }
                    else if (item.Control == "WoState")
                    {
                        _finalClass.AppendLine(_woComboEnumScriptUser.GetCode(item));
                    }
                    else if (item.Control == "Decimal" || item.Control == "Custom")
                    {
                        _finalClass.AppendLine(_woMaskedEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "TextMask")
                    {
                        _finalClass.AppendLine(_woMaskedEditScriptUser.GetCode(item));
                    }
                    else if (item.Control == "File")
                    {
                        _finalClass.AppendLine(_woFileEditScriptUser.GetCode(item));
                    }
                    else if (item.TypeItem == eTypeItem.List)
                    {
                        _finalClass.AppendLine(_woListGridScriptUser.GetCode(item));
                    }
                }
            }
        }

        #endregion Body
    }
}
