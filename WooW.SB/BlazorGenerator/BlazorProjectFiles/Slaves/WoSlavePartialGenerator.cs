using System;
using System.Collections.Generic;
using System.Text;
using WooW.Core;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorAlerts.WoAlertList;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormGroup;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormTab;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoListGrid;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.CheckBoxEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoComboEnumEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoDateEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpDialogEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMaskedEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMemoEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoSpinEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoTextEdit;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.Slaves
{
    public class WoSlavePartialGenerator
    {
        #region Variables globales

        /// <summary>
        /// Nombre de la clase y fichero
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        /// <summary>
        /// Métodos y atributos de los contenedores.
        /// </summary>
        private StringBuilder _containersMethodsProperties = new StringBuilder();

        /// <summary>
        /// Métodos y atributos de los controles de la clase.
        /// </summary>
        private StringBuilder _componentsMethodsProperties = new StringBuilder();

        /// <summary>
        /// Contiene una lista con los items que utilizan o dependen de un servicio para
        /// su funcionamiento base.
        /// </summary>
        private List<WoItem> _itemsWithServiceCol = new List<WoItem>();

        /// <summary>
        /// Genera el código necesario para el lookup que se utiliza por otros componentes.
        /// </summary>
        private WoLookUpDialogEditAdded _woLookUpDialogEditAdded = null;

        /// <summary>
        /// Genera el código necesario para el lookup que se utiliza por otros componentes.
        /// </summary>
        private WoLookUpEditAdded _woLookUpEditAdded = null;

        /// <summary>
        /// Genera el código necesario para el lookup que se utiliza por otros componentes.
        /// </summary>
        private StringBuilder _strScriptUserInstances = new StringBuilder();

        /// <summary>
        /// Indica si el formulario es o no un reporte.
        /// </summary>
        private bool _isReport = false;

        /// <summary>
        /// Indica si el formulario a generar es una lista del modelo.
        /// </summary>
        private bool _isList = false;

        #endregion Variables globales

        #region Instance generators

        private WoAlertListPartial _woAlertListPartial = null;

        private WoTextEditPartial _woTextEditPartial = null;
        private WoMaskedEditPartial _woMaskedEditPartial = null;
        private WoMemoEditPartial _woMemoEditPartial = null;
        private WoSpinEditPartial _woSpinEditPartial = null;
        private WoLookUpEditPartial _woLookUpEditPartial = null;
        private WoDateEditPartial _woDateEditPartial = null;
        private WoComboEnumEditPartial _woComboEnumEditPartial = null;
        private WoLookUpDialogEditPartial _woLookUpDialogEditPartial = null;
        private WoCheckBoxEditPartial _woCheckBoxEditPartial = null;

        private WoFormPartial _woFormPartial = null;
        private WoFormGroupPartial _woFormGroupPartial = null;
        private WoFormTabPartial _woFormTabPartial = null;

        private WoReportViewerPartial _woReportViewerPartial = null;
        private WoListGridPartial _woListPartial = null;

        private WoReportViewerAdded _woReportViewerAdded = null;
        private WoListGridAdded _woListGridAdded = null;

        #endregion Instance generators

        #region Constructor

        /// <summary>
        /// Asigna el nombre de a clase, calcula la identación del código a generar e instancia las clases que generan ell código razor.
        /// </summary>
        /// <param name="classModelName"></param>
        public WoSlavePartialGenerator(string classModelName)
        {
            CalculateIdentSpaces();
            _classModelName = classModelName;

            _woAlertListPartial = new WoAlertListPartial(_identMethodsAndProperties);

            _woTextEditPartial = new WoTextEditPartial(_identMethodsAndProperties);
            _woMaskedEditPartial = new WoMaskedEditPartial(_identMethodsAndProperties);
            _woSpinEditPartial = new WoSpinEditPartial(_identMethodsAndProperties);
            _woMemoEditPartial = new WoMemoEditPartial(_identMethodsAndProperties);
            _woLookUpEditPartial = new WoLookUpEditPartial(_identMethodsAndProperties);
            _woLookUpDialogEditPartial = new WoLookUpDialogEditPartial(_identMethodsAndProperties);
            _woDateEditPartial = new WoDateEditPartial(_identMethodsAndProperties);
            _woComboEnumEditPartial = new WoComboEnumEditPartial(_identMethodsAndProperties);
            _woCheckBoxEditPartial = new WoCheckBoxEditPartial(_identMethodsAndProperties);

            _woFormPartial = new WoFormPartial(_identMethodsAndProperties);
            _woFormGroupPartial = new WoFormGroupPartial(_identMethodsAndProperties);
            _woFormTabPartial = new WoFormTabPartial(_identMethodsAndProperties);

            /// Instancias extra para los items con dependencia a servicios
            _woLookUpEditAdded = new WoLookUpEditAdded(_identMethodsAndProperties);
            _woLookUpDialogEditAdded = new WoLookUpDialogEditAdded(_identMethodsAndProperties);

            _woReportViewerPartial = new WoReportViewerPartial(_identMethodsAndProperties);
            _woListPartial = new WoListGridPartial(_identMethodsAndProperties);

            _woReportViewerAdded = new WoReportViewerAdded();
            _woListGridAdded = new WoListGridAdded(_identMethodsAndProperties);
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
        /// Retorna la clase generada en función del WoContainer,
        /// Limpia los StringBuilder ocupados de almacenar el código generado de forma temporal.
        /// Construimos el código para los contenedores y para los items
        /// (se almacena en _containersMethodsProperties y _componentsMethodsProperties).
        /// Arma la clase con los métodos y propiedades previamente construidos.
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        public string GetPartialClass(
            WoContainer baseContainer,
            bool isReport,
            bool isList,
            bool blazorIntegral = false
        )
        {
            _isReport = isReport;
            _isList = isList;
            _finalClass.Clear();
            _componentsMethodsProperties.Clear();
            _containersMethodsProperties.Clear();

            BuildCotrolsData(baseContainer, blazorIntegral);

            _finalClass.AppendLine(BuildHeaderClass(baseContainer));

            _finalClass.AppendLine(BuildExtraMethodsForm());

            if (!_isList)
            {
                _finalClass.AppendLine(BuildFluent(baseContainer));
            }
            _finalClass.AppendLine(BuildLifeCycle());

            _finalClass.AppendLine(_containersMethodsProperties.ToString());

            _finalClass.AppendLine(_componentsMethodsProperties.ToString());

            BuildFooterClass();

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Header

        /// <summary>
        /// Construye el header de la clase.
        /// Contiene las instancias de model controls (instancia singleton), scripts user y el modelo.
        /// </summary>
        public string BuildHeaderClass(WoContainer container)
        {
            bool scriptUserReady = BuildSlavesScriptsInstances(container);

            StringBuilder strHeader = new StringBuilder();

            strHeader.AppendLine(
                $@"
{_identClass}public partial class {_classModelName}
{_identClass}{{

{_identClass}    #region Atributos

{_identClass}    // Código generado por el método BuildHeaderClass en la clase WoLayoutPartialGenerator.cs 

{_identClass}    /// <summary>
{_identClass}    /// Inyección de la clase con las vistas para manipular los controles.
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    public {_classModelName}Controls _modelControls {{ get; set; }}
"
            );

            if (
                (
                    (container.SubType != WoSubTypeModel.Report)
                    || (!_isReport && container.SubType == WoSubTypeModel.Report)
                ) && !scriptUserReady
            )
            {
                strHeader.AppendLine(
                    $@"
{_identClass}    /// <summary>
{_identClass}    /// Instancia con los métodos de los eventos de los controles.
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    public {_classModelName}ScriptsUser _scriptsUser {{ get; set; }}
"
                );
            }

            strHeader.AppendLine(
                $@"
{_identClass}    /// <summary>
{_identClass}    /// Variable con el valor del parámetro del identificador que se puede recibir como parámetro.
{_identClass}    /// Indica en que registro se posicionara la navegación.
{_identClass}    /// </summary>
{_identClass}    private string _idParameter = string.Empty;

 {_identClass}#region Instancias de los scripts de otros formularios

{_strScriptUserInstances}

{_identClass} #endregion Instancias de los scripts de otros formularios

{_identClass}    #endregion Atributos
"
            );

            return strHeader.ToString();
        }

        /// <summary>
        /// Construye las instancias de las esclavas.
        /// </summary>
        public bool BuildSlavesScriptsInstances(WoContainer container)
        {
            bool scriptUserReady = false;

            if (!container.ContainersCol.IsNull())
            {
                foreach (var subContainer in container.ContainersCol)
                {
                    BuildSlavesScriptsInstances(subContainer);
                }
            }

            if (!container.ItemsCol.IsNull())
            {
                foreach (WoItem item in container.ItemsCol)
                {
                    if (item.Control == "Slave")
                    {
                        _strScriptUserInstances.AppendLine(
                            $@"
{_identClass}    /// <summary>
{_identClass}    /// Instancia con los metodos de los eventos del formulario de esclavas.
{_identClass}    /// Generado por el método BuildSlavesScriptsInstances en la clase WoLayoutPartialGenerator.cs 
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    public {item.InternalFrom}ScriptsUser _{item.ClassModelType}ScriptsUser {{ get; set; }}
"
                        );
                    }

                    if (item.TypeItem == eTypeItem.ReportItem)
                    {
                        _strScriptUserInstances.AppendLine(
                            $@"
{_identClass}    /// <summary>
{_identClass}    /// Instancia con los metodos de los eventos del formulario de esclavas.
{_identClass}    /// Generado por el método BuildSlavesScriptsInstances en la clase WoLayoutPartialGenerator.cs 
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    public {item.InternalFrom}ScriptsUser _scriptsUser {{ get; set; }}
"
                        );

                        scriptUserReady = true;
                    }
                }
            }

            return scriptUserReady;
        }

        #endregion Header

        #region Footer

        /// <summary>
        /// Construye el footer de la clase.
        /// Solo sierra la clase.
        /// </summary>
        public void BuildFooterClass()
        {
            _finalClass.AppendLine($@"{_identClass}}}");
        }

        #endregion Footer

        #region Body

        #region Métodos adicionales

        /// <summary>
        /// Construye los métodos adicionales del formulario.
        /// Actualización del formulario y validación de que ya existe todo para ejecutar el método principal.
        /// </summary>
        public string BuildExtraMethodsForm()
        {
            return $@"
{_identMethodsAndProperties}#region Métodos adicionales

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Método que permite actualizar el formulario.
{_identMethodsAndProperties}/// Solo realiza un re dibujado de la vista.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void UpdateEntireForm() => StateHasChanged();

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Indicador del primer rende rizado de la vista.
{_identMethodsAndProperties}/// Permite controlar lo que se ejecuta en métodos sin esta bandera.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private bool _firstRender = true;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Verifica que el formulario se haya iniciado y
{_identMethodsAndProperties}/// si es el caso, ejecuta el método de inicio.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void CheckForm()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _scriptsUser.FormularioIniciado();
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Métodos adicionales";
        }

        #endregion Métodos adicionales

        #region Fluent

        private string BuildFluent(WoContainer baseContainer)
        {
            StringBuilder strFluent = new StringBuilder();

            if (baseContainer.HaveModelReference)
            {
                strFluent.AppendLine(
                    $@"
{_identMethodsAndProperties}        #region Fluent

{_identMethodsAndProperties}        /// <summary>
{_identMethodsAndProperties}        /// Instancia de la clase con las reglas de fluent, permite la validación del objeto.
{_identMethodsAndProperties}        /// <summary>
{_identMethodsAndProperties}        private {_classModelName}Validator _authValidator = new {_classModelName}Validator();

{_identMethodsAndProperties}        /// <summary>
{_identMethodsAndProperties}        /// Guarda el resultado de la validación, los errores que contenga se asignaran a las alertas del componente.
{_identMethodsAndProperties}        /// <summary>
{_identMethodsAndProperties}        private ValidationResult? _authValidationResult;

{_identMethodsAndProperties}        /// <summary>
{_identMethodsAndProperties}        /// Se llama en cada uno de los métodos OnChange del modelo para validar el cambio del campo con fluent
{_identMethodsAndProperties}        /// </summary>
{_identMethodsAndProperties}        private void {baseContainer.ModelId}Validate(string id, dynamic control)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _authValidationResult = _authValidator.Validate(_scriptsUser.{baseContainer.ModelId});
{_identMethodsAndProperties}            bool noAlerts = true;
{_identMethodsAndProperties}
{_identMethodsAndProperties}            foreach (var validation in _authValidationResult.Errors)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                if (id == validation.PropertyName)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    noAlerts = false;
{_identMethodsAndProperties}                    control.AgregarAlerta(validation.ToString());
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}
{_identMethodsAndProperties}            if (noAlerts)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                control.LimpiarAlertas();
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}

{_identMethodsAndProperties}        #endregion Fluent"
                );
            }

            return strFluent.ToString();
        }

        #endregion Fluent

        #region Ciclo de vida

        /// <summary>
        /// Construye el método OnInitializedAsync del formulario.
        /// </summary>
        /// <param name="container"></param>
        public string BuildLifeCycle()
        {
            return (
                $@"
{_identMethodsAndProperties}#region métodos del ciclo de vida

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Se detona madamas iniciar la pantalla, re instancia el cliente a partir de los datos en el
{_identMethodsAndProperties}/// local storage y obtiene parámetros en el caso que de la url venga con ellos.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}protected override async Task OnInitializedAsync()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        Client = await GetClientAsync();
{_identMethodsAndProperties}        if (_firstRender)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            if (Client != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _scriptsUser.woTarget = Client;

{_identMethodsAndProperties}                ///Servicios de controles específicos                     
{_identMethodsAndProperties}                try
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                /// Inicializa los servicios de los componentes
{BuildOnInitializeServiceItem()}
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                catch (Exception ex)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    throw new Exception($@""Error al inicializar los servicios de los componentes (lookups, report...) {{ex.Message}}"");
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}
{_identMethodsAndProperties}            /// Actualizamos el rende rizado de la vista con los datos de la consulta de navegación, ya sea
{_identMethodsAndProperties}            /// move to o last.
{_identMethodsAndProperties}            StateHasChanged();
{_identMethodsAndProperties}
{_identMethodsAndProperties}            /// Actualizamos el valor de la bandera para evitar que el código dentro del if se ejecute en cada state has change
{_identMethodsAndProperties}            _firstRender = false;
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        /// En caso de error, se detona la alerta con la configuración de error.
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error en el método OnInitialize del formulario: {{ex.Message}}"", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}


{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Evento del ciclo de vida de blazor, se ejecuta después de rende rizar el formulario.
{_identMethodsAndProperties}/// Recibe un parámetro que indica si es la primera vez que se rende riza el formulario, 
{_identMethodsAndProperties}/// permite detonar código solo la primera vez que rende riza.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""firstRender""></param>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}protected override async Task OnAfterRenderAsync(bool firstRender)
{_identMethodsAndProperties}{{
{BuildOnAfterRender()}
{_identMethodsAndProperties}    if (firstRender) 
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        try
{_identMethodsAndProperties}        {{
{BuildOnAfterRenderFirstRender()}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        catch (Exception ex)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error en el método OnAfterRenderAsync del formulario: {{ex.Message}}"");
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}
 
{_identMethodsAndProperties}#endregion métodos del ciclo de vida"
            );
        }

        /// <summary>
        /// Recorre los elementos tipo lookup y genera su código para el ciclo de vida.
        /// </summary>
        /// <returns></returns>
        private string BuildOnInitializeServiceItem()
        {
            StringBuilder strServiceItemData = new StringBuilder();

            foreach (WoItem item in _itemsWithServiceCol)
            {
                if (item.Control == "LookUpDialog")
                {
                    strServiceItemData.AppendLine(_woLookUpDialogEditAdded.GetCodeLifeCycle(item));
                }
                else if (item.TypeItem == eTypeItem.ReportItem)
                {
                    strServiceItemData.AppendLine(_woReportViewerAdded.GetCodeOnInitialized(item));
                }
                else if (item.TypeItem == eTypeItem.List)
                {
                    strServiceItemData.AppendLine(_woListGridAdded.BuildOnInitializedCode(item));
                }
            }

            return strServiceItemData.ToString();
        }

        /// <summary>
        /// Recorre los elementos tipo lookup y genera su código para el onAfterRender
        /// Dentro del firstRender.
        /// </summary>
        /// <returns></returns>
        private string BuildOnAfterRenderFirstRender()
        {
            StringBuilder strServiceItemData = new StringBuilder();

            foreach (WoItem item in _itemsWithServiceCol)
            {
                if (item.TypeItem == eTypeItem.ReportItem)
                {
                    strServiceItemData.AppendLine(_woReportViewerAdded.GetCodeOnAfterRender(item));
                }
            }

            return strServiceItemData.ToString();
        }

        /// <summary>
        /// Recorre los elementos tipo lookup y genera su código para el onAfterRender
        /// </summary>
        /// <returns></returns>
        private string BuildOnAfterRender()
        {
            StringBuilder strServiceItemData = new StringBuilder();

            foreach (WoItem item in _itemsWithServiceCol)
            {
                if (item.TypeItem == eTypeItem.List)
                {
                    strServiceItemData.AppendLine(_woListGridAdded.BuildOnAfterRenderCode(item));
                }
                else if (item.Control == "LookUpDialog")
                {
                    strServiceItemData.AppendLine(
                        _woLookUpDialogEditAdded.GetCodeOnAfterRender(item)
                    );
                }
            }

            return strServiceItemData.ToString();
        }

        #endregion Ciclo de vida

        #region Controls

        #region CommonControls

        /// <summary>
        /// Construye el cuerpo de la clase y en función del tipo de input va asignando
        /// en StringBuilders el código correspondiente a métodos del ciclo de vida o a
        /// métodos y propiedades.
        /// </summary>
        /// <param name="container"></param>
        public void BuildCotrolsData(WoContainer container, bool blazorIntegral = false)
        {
            if (container.TypeContainer == eTypeContainer.FormRoot)
            {
                string dataForm = _woFormPartial.GetCode(container);
                _componentsMethodsProperties.AppendLine(dataForm);
            }
            else if (container.TypeContainer == eTypeContainer.FormGroup)
            {
                string dataFormGroup = _woFormGroupPartial.GetCode(container);
                _componentsMethodsProperties.AppendLine(dataFormGroup);
            }
            else if (container.TypeContainer == eTypeContainer.FormTab)
            {
                string dataFormTab = _woFormTabPartial.GetCode(container);
                _componentsMethodsProperties.AppendLine(dataFormTab);
            }

            if (!container.ContainersCol.IsNull())
            {
                foreach (var subgroup in container.ContainersCol)
                {
                    BuildCotrolsData(subgroup, blazorIntegral);
                }
            }
            if (!container.ItemsCol.IsNull())
            {
                foreach (var item in container.ItemsCol)
                {
                    if (item.Control == "Label")
                    {
                        if (item.Id == "Alertas")
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woAlertListPartial.GetCode(item)
                            );
                        }
                        else if (item.TypeItem == eTypeItem.ReportItem)
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woReportViewerPartial.GetCode(item)
                            );
                            _itemsWithServiceCol.Add(item);
                        }
                        else if (item.TypeItem == eTypeItem.List)
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woListPartial.GetCode(item, blazorIntegral)
                            );
                            _itemsWithServiceCol.Add(item);
                        }
                    }
                    else if (
                        item.Control == "Decimal"
                        || item.Control == "TextMask"
                        || item.BindingType.ToLower().Contains("decimal")
                        || item.BindingType.ToLower().Contains("double")
                    )
                    {
                        var dataInputText = _woMaskedEditPartial.GetCode(item: item);
                        _componentsMethodsProperties.AppendLine(dataInputText);
                    }
                    else if (item.Control == "Text")
                    {
                        if (
                            item.BindingType.ToLower() == "string"
                            || item.BindingType.ToLower() == "string?"
                        )
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woTextEditPartial.GetCode(item)
                            );
                        }
                        else
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woMaskedEditPartial.GetCode(item)
                            );
                        }
                    }
                    else if (item.Control == "Memo")
                    {
                        _componentsMethodsProperties.AppendLine(_woMemoEditPartial.GetCode(item));
                    }
                    else if (item.Control == "Spin")
                    {
                        _componentsMethodsProperties.AppendLine(_woSpinEditPartial.GetCode(item));
                    }
                    else if (item.Control == "Bool")
                    {
                        _componentsMethodsProperties.AppendLine(
                            _woCheckBoxEditPartial.GetCode(item)
                        );
                    }
                    else if (item.Control == "Date")
                    {
                        _componentsMethodsProperties.AppendLine(_woDateEditPartial.GetCode(item));
                    }
                    else if (item.Control == "LookUp")
                    {
                        _componentsMethodsProperties.AppendLine(_woLookUpEditPartial.GetCode(item));
                        _itemsWithServiceCol.Add(item);
                    }
                    else if (item.Control == "LookUpDialog")
                    {
                        _componentsMethodsProperties.AppendLine(
                            _woLookUpDialogEditPartial.GetCode(item)
                        );
                        _itemsWithServiceCol.Add(item);
                    }
                    else if (item.Control == "EnumString")
                    {
                        _componentsMethodsProperties.AppendLine(
                            _woComboEnumEditPartial.GetCode(item)
                        );
                    }
                    else if (item.Control == "EnumInt")
                    {
                        var dataInputEnumString = _woComboEnumEditPartial.GetCode(item);
                        _componentsMethodsProperties.AppendLine(
                            _woComboEnumEditPartial.GetCode(item)
                        );
                    }
                    else if (item.Control == "WoState")
                    {
                        _componentsMethodsProperties.AppendLine(
                            _woComboEnumEditPartial.GetCode(item)
                        );
                    }
                    else if (item.Control == "Decimal" || item.Control == "Custom")
                    {
                        _componentsMethodsProperties.AppendLine(_woMaskedEditPartial.GetCode(item));
                    }
                }
            }
        }

        #endregion CommonControls

        #endregion Controls

        #endregion Body
    }
}
