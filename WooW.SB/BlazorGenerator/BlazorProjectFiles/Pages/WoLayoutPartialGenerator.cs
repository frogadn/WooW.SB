using System;
using System.Collections.Generic;
using System.Text;
using WooW.Core;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorAlerts.WoAlertList;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormGroup;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormTab;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoDetailGrid;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoFormToolbar;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports;
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
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.Pages
{
    public class WoLayoutPartialGenerator
    {
        #region Atributos

        /// <summary>
        /// Instancia con la metadata del componente.
        /// </summary>
        private WoContainer _container = new WoContainer();

        /// <summary>
        /// Nombre de la clase y fichero
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        /// <summary>
        /// Contiene las intancias de las slaves
        /// </summary>
        private StringBuilder _strSlaveInstances = new StringBuilder();

        /// <summary>
        /// Métodos y atributos de los contenedores.
        /// </summary>
        private StringBuilder _containersMethodsProperties = new StringBuilder();

        /// <summary>
        /// Métodos y atributos de los controles de la clase.
        /// </summary>
        private StringBuilder _componentsMethodsProperties = new StringBuilder();

        /// <summary>
        /// Almacena el fragmento de codigo para la lista de componentes complejos.
        /// </summary>
        private StringBuilder _strComplexItems = new StringBuilder();

        /// <summary>
        /// Contiene controles complejos que deven de ser prosesador aparte por que
        /// tienen dependencia a otros componentes que se deven de generar previamente.
        /// </summary>
        private List<WoItem> _complexItemsCol = new List<WoItem>();

        /// <summary>
        /// Contiene una lista con los items que utilizan o dependen de un servicio para
        /// su funcionamiento base.
        /// </summary>
        private List<WoItem> _itemsWithServiceCol = new List<WoItem>();

        #endregion Atributos

        #region Instance generators

        private WoAlertListPartial _woAlertListPartial = null;
        private WoFormToolbarPartial _woFormToolbarPartial = null;

        private WoTextEditPartial _woTextEditPartial = null;
        private WoMaskedEditPartial _woMaskedEditPartial = null;
        private WoMemoEditPartial _woMemoEditPartial = null;
        private WoSpinEditPartial _woSpinEditPartial = null;
        private WoLookUpEditPartial _woLookUpEditPartial = null;
        private WoDateEditPartial _woDateEditPartial = null;
        private WoComboEnumEditPartial _woComboEnumEditPartial = null;
        private WoLookUpDialogEditPartial _woLookUpDialogEditPartial = null;
        private WoDetailGridPartial _woDetailGridPartial = null;
        private WoCheckBoxEditPartial _woCheckBoxEditPartial = null;
        private WoFileEditPartial _woFileEditPartial = null;

        private WoFormPartial _woFormPartial = null;
        private WoFormGroupPartial _woFormGroupPartial = null;
        private WoFormTabPartial _woFormTabPartial = null;

        private WoReportViewerPartial _woReportViewerPartial = null;

        #endregion Instance generators


        #region Constructor

        /// <summary>
        /// Asigna el nombre de a clase, calcula la identacion del codigo a generar e instancia las clases que generan ell código razor.
        /// </summary>
        /// <param name="classModelName"></param>
        public WoLayoutPartialGenerator(string classModelName)
        {
            _classModelName = classModelName;

            CalculateIdentSpaces();

            _woAlertListPartial = new WoAlertListPartial(_identMethodsAndProperties);
            _woFormToolbarPartial = new WoFormToolbarPartial(_identMethodsAndProperties);

            _woTextEditPartial = new WoTextEditPartial(_identMethodsAndProperties);
            _woMaskedEditPartial = new WoMaskedEditPartial(_identMethodsAndProperties);
            _woSpinEditPartial = new WoSpinEditPartial(_identMethodsAndProperties);
            _woMemoEditPartial = new WoMemoEditPartial(_identMethodsAndProperties);
            _woLookUpEditPartial = new WoLookUpEditPartial(_identMethodsAndProperties);
            _woLookUpDialogEditPartial = new WoLookUpDialogEditPartial(_identMethodsAndProperties);
            _woDateEditPartial = new WoDateEditPartial(_identMethodsAndProperties);
            _woComboEnumEditPartial = new WoComboEnumEditPartial(_identMethodsAndProperties);
            _woCheckBoxEditPartial = new WoCheckBoxEditPartial(_identMethodsAndProperties);
            _woFileEditPartial = new WoFileEditPartial(_identMethodsAndProperties);

            _woDetailGridPartial = new WoDetailGridPartial(_identMethodsAndProperties);

            _woFormPartial = new WoFormPartial(_identMethodsAndProperties);
            _woFormGroupPartial = new WoFormGroupPartial(_identMethodsAndProperties);
            _woFormTabPartial = new WoFormTabPartial(_identMethodsAndProperties);

            /// Instancias extra para los items con dependencia a servicios
            _woLookUpEditAdded = new WoLookUpEditAdded(_identMethodsAndProperties);
            _woLookUpDialogEditAdded = new WoLookUpDialogEditAdded(_identMethodsAndProperties);

            _woReportViewerPartial = new WoReportViewerPartial(_identMethodsAndProperties);
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
        /// Retorna la clase generada en función del Wo container,
        /// Asigna el container que se recibe por parámetro a la variable de la clase.
        /// Limpia los StringBuilder ocupados de almacenar el código generado de forma temporal.
        /// Construimos el código para los contenedores y para los items
        /// (se almacena en _containersMethodsProperties y _componentsMethodsProperties).
        /// Arma la clase con los métodos y propiedades previamente construidos.
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        public string GetPartialClass(WoContainer baseContainer)
        {
            _container = baseContainer;

            _finalClass.Clear();
            _strSlaveInstances.Clear();
            _containersMethodsProperties.Clear();
            _componentsMethodsProperties.Clear();
            _strComplexItems.Clear();

            _complexItemsCol.Clear();
            _itemsWithServiceCol.Clear();

            BuildCotrolsData(_container);
            BuildComplexItems();

            _finalClass.AppendLine(BuildHeaderClass());

            _finalClass.AppendLine(BuildExtraMethodsForm());

            _finalClass.AppendLine(BuildFluent());

            _finalClass.AppendLine(BuildOnInitialized());

            _finalClass.AppendLine(BuildOnAfterRender());

            _finalClass.AppendLine(_strComplexItems.ToString());

            _finalClass.AppendLine(_containersMethodsProperties.ToString());

            _finalClass.AppendLine(_componentsMethodsProperties.ToString());

            _finalClass.AppendLine(BuildFooterClass());

            return _finalClass.ToString();
        }

        #endregion Método principal


        #region Header

        /// <summary>
        /// Construye el header de la clase.
        /// Contiene las instancias de model controls (instancia singleton), scripts user y el modelo.
        /// </summary>
        public string BuildHeaderClass()
        {
            string classType = "Layout";

            BuildSlavesScriptsInstances(_container);

            StringBuilder strHeader = new StringBuilder();

            strHeader.AppendLine(
                $@"
{_identClass}public partial class {_classModelName}{classType}
{_identClass}{{

{_identClass}    #region Atributos

{_identClass}    // Inyección del runtime de JavaScript para ejecutar código js desde c#.
{_identClass}    [Inject]
{_identClass}    public IJSRuntime? JSRuntime {{ get; set; }}

{_identClass}    // Código generado por el método BuildHeaderClass en la clase WoLayoutPartialGenerator.cs 

{_identClass}    /// <summary>
{_identClass}    /// Inyección de la clase con las vistas para manipular los controles.
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    public {_classModelName}Controls _modelControls {{ get; set; }}
"
            );

            if (_container.SubType != WoSubTypeModel.Report)
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
{_identClass}    /// Instancia con las configuraciones de bloqueo y desbloqueo de los controles de la clase.
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    private {_classModelName}TransitionSettings _{_container.ModelId}TransitionSettings {{ get; set; }}

{_identClass}    /// <summary>
{_identClass}    /// Variable con el valor del parámetro del identificador que se puede recibir como parámetro.
{_identClass}    /// Indica en que registro se posicionara la navegación.
{_identClass}    /// </summary>
{_identClass}    private string _idParameter = string.Empty;

{_identClass}    /// <summary>
{_identClass}    /// Variable con el valor de la transición preseleccionada que se puede pasar como parámetro.
{_identClass}    /// Indica la transacción pre seleccionada
{_identClass}    /// </summary>
{_identClass}    private string _transition = string.Empty;

 {_identClass}#region Instancias de los scripts de otros formularios

{_strSlaveInstances}

{_identClass} #endregion Instancias de los scripts de otros formularios

{_identClass}    #endregion Atributos
"
            );

            return strHeader.ToString();
        }

        /// <summary>
        /// Construye las instancias de las esclavas.
        /// </summary>
        public void BuildSlavesScriptsInstances(WoContainer container)
        {
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
                        _strSlaveInstances.AppendLine(
                            $@"
{_identClass}    /// <summary>
{_identClass}    /// Instancia con los métodos de los eventos del formulario de esclavas.
{_identClass}    /// Generado por el método BuildSlavesScriptsInstances en la clase WoLayoutPartialGenerator.cs 
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    public {item.InternalFrom}ScriptsUser _{item.ClassModelType}ScriptsUser {{ get; set; }}
"
                        );
                    }

                    if (item.TypeItem == eTypeItem.ReportItem)
                    {
                        _strSlaveInstances.AppendLine(
                            $@"
{_identClass}    /// <summary>
{_identClass}    /// Instancia con los métodos de los eventos del formulario de esclavas.
{_identClass}    /// Generado por el método BuildSlavesScriptsInstances en la clase WoLayoutPartialGenerator.cs 
{_identClass}    /// </summary>
{_identClass}    [Inject]
{_identClass}    public {item.InternalFrom}ScriptsUser _{item.ClassModelType}ScriptsUser {{ get; set; }}
"
                        );
                    }
                }
            }
        }

        #endregion Header

        #region Footer

        /// <summary>
        /// Construye el footer de la clase.
        /// Solo sierra la clase.
        /// </summary>
        public string BuildFooterClass()
        {
            return ($@"{_identClass}}}");
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
            return (
                $@"
{_identMethodsAndProperties}#region Métodos adicionales
{_identMethodsAndProperties}// Código generado por el método BuildHeaderClass en la clase WoLayoutPartialGenerator.cs 

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Método que permite actualizar el formulario.
{_identMethodsAndProperties}/// Generado por el método BuildExtraMethodsForm en la clase WoLayoutPartialGenerator.cs 

{_identMethodsAndProperties}/// Solo realiza un re dibujado de la vista.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void UpdateEntireForm() => StateHasChanged();

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Indicador del primer renderizado de la vista.
{_identMethodsAndProperties}/// Permite controlar lo que se ejecuta en metodos sin esta bandera.
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

{_identMethodsAndProperties}#endregion Métodos adicionales"
            );
        }

        #endregion Métodos adicionales

        #region Fluent

        private string BuildFluent()
        {
            return $@"
{_identMethodsAndProperties}#region Fluent
{_identMethodsAndProperties}// Código generado por el método BuildHeaderClass en la clase WoLayoutPartialGenerator.cs 

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la clase con las reglas de fluent, permite la validación del objeto.
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}private {_classModelName}Validator _authValidator = new {_classModelName}Validator();

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Guarda el resultado de la validación, los errores que contenga se asignaran a las alertas del componente.
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}private ValidationResult? _authValidationResult;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Se llama en cada uno de los métodos OnChange del modelo para validar el cambio del campo con fluent
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void {_container.ModelId}Validate(string id, dynamic control)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _authValidationResult = _authValidator.Validate(_scriptsUser.{_container.ModelId});
{_identMethodsAndProperties}    bool noAlerts = true;

{_identMethodsAndProperties}    foreach (var validation in _authValidationResult.Errors)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (id == validation.PropertyName)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            noAlerts = false;
{_identMethodsAndProperties}            control.AgregarAlerta(validation.ToString());
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}

{_identMethodsAndProperties}    if (noAlerts)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        control.LimpiarAlertas();
{_identMethodsAndProperties}    }}

{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Fluent
";
        }

        #endregion Fluent

        #region Ciclo de vida

        /// <summary>
        /// Construye el método OnInitializedAsync del formulario.
        /// </summary>
        private string BuildOnInitialized()
        {
            return (
                $@"
{_identMethodsAndProperties}#region Ciclo de vida

{_identMethodsAndProperties}/// Código generado por el método BuildOnInitialized en la clase WoLayoutPartialGenerator.cs 
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Se detona na damas iniciar la pantalla, re instancia el cliente a partir de los datos en el
{_identMethodsAndProperties}/// local storage y obtiene parámetros en el caso que de la url venga con ellos.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}
{_identMethodsAndProperties}protected override async Task OnInitializedAsync()
{_identMethodsAndProperties}{{

{_identMethodsAndProperties}    /// Recuperación del parámetro del identificador para la navegación.
{_identMethodsAndProperties}    Uri? uri = NavigationManager?.ToAbsoluteUri(NavigationManager.Uri);
{_identMethodsAndProperties} 
{_identMethodsAndProperties}    if (uri != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        var queryStrings = QueryHelpers.ParseQuery(uri.Query);
{_identMethodsAndProperties} 
{_identMethodsAndProperties}        if (queryStrings.TryGetValue(""__Id"", out var id))
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _idParameter = id;
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        if (queryStrings.TryGetValue(""__Transition"", out var transition))
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _transition = transition;
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}

{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        Client = await GetClientAsync();

{_identMethodsAndProperties}        if (_firstRender)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            if (Client != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                /// Asignamos el cliente a los scripts de usuario.
{_identMethodsAndProperties}                _scriptsUser.woTarget = Client;

{_identMethodsAndProperties}                /// Suscripción del evento de actualización de la vista.
{_identMethodsAndProperties}                _scriptsUser.StateHasChangeEvt += UpdateEntireForm;

{_identMethodsAndProperties}                /// Instancia de los clientes específicos de controles como lookups.
{_identMethodsAndProperties}                _navService = new WoNavigationService<{_container.ModelId}, {_container.ModelId}Get, {_container.ModelId}List>(Client);
{_identMethodsAndProperties}                _transitionService = new WoTransitionService<{_container.ModelId}>(Client);

{((_container.ModelType == WoTypeModel.Catalog || _container.ModelType == WoTypeModel.CatalogType) ? $"{_identMethodsAndProperties}                _deleteService = new WoDeleteService<{_container.ModelId}, {_container.ModelId}Borrar>(Client);" : "")}

{BuildOnInitializeServiceItem()}
{_identMethodsAndProperties}                try
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    /// En caso de venir un parámetro usamos el MoveTo, en caso contrario usamos el Last.
{_identMethodsAndProperties}                    if (_idParameter == string.Empty)
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        await {((_container.ModelType == WoTypeModel.Catalog || _container.ModelType == WoTypeModel.CatalogType) ? "First_OnClick" : "Last_OnClick")}();
{_identMethodsAndProperties}                    }}
{_identMethodsAndProperties}                    else
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        try
{_identMethodsAndProperties}                        {{
{_identMethodsAndProperties}                            await MoveTo();
{_identMethodsAndProperties}                        }}
{_identMethodsAndProperties}                        catch (Exception ex)
{_identMethodsAndProperties}                        {{
{_identMethodsAndProperties}                            await {((_container.ModelType == WoTypeModel.Catalog || _container.ModelType == WoTypeModel.CatalogType) ? "First_OnClick" : "Last_OnClick")}();
{_identMethodsAndProperties}                            throw new Exception(ex.Message);
{_identMethodsAndProperties}                        }}
{_identMethodsAndProperties}                    }}

{_identMethodsAndProperties}                    if (_transition != null && _transition != string.Empty)
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        ControlesTransition_OnChange(_transition);
{_identMethodsAndProperties}                    }}
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                catch (Exception ex)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _{_container.ModelId}TransitionSettings.SetBlockSettings(""NavigationModeNoReg"");
{_identMethodsAndProperties}                    throw new Exception(ex.Message);
{_identMethodsAndProperties}                }}

{_identMethodsAndProperties}                /// Actualizamos el rendering de la vista con los datos de la consulta de navegación, ya sea
{_identMethodsAndProperties}                /// move to o last.
{_identMethodsAndProperties}                StateHasChanged();

{_identMethodsAndProperties}                /// Actualizamos el valor de la bandera para evitar que el código dentro del if se ejecute en cada state has change
{_identMethodsAndProperties}                _firstRender = false;
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        /// En caso de error, se detona la alerta con la configuración de error.
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}


{_identMethodsAndProperties}}}
            "
            );
        }

        /// <summary>
        /// Construye el método OnInitializedAsync del formulario.
        /// </summary>
        private string BuildOnAfterRender()
        {
            return (
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Se detona al terminar de dibujar los componentes.
{_identMethodsAndProperties}/// Asigna el modo de transición inicial en navigation.
{_identMethodsAndProperties}/// Código generado por el método BuildOnAfterRender en la clase WoLayoutPartialGenerator.cs 
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""firstRender""></param>
{_identMethodsAndProperties}protected override async Task OnAfterRenderAsync(bool firstRender)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    await base.OnAfterRenderAsync(firstRender);

{_identMethodsAndProperties}    if(!Client.BearerToken.IsNullOrStringEmpty())
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        {BuildOnAfterRenderServiceItem()}
{_identMethodsAndProperties}    }}

{_identMethodsAndProperties}
{_identMethodsAndProperties}    if (firstRender)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _{_container.ModelId}TransitionSettings.SetBlockSettings(""NavigationMode"");
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Ciclo de vida
"
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
            }

            return strServiceItemData.ToString();
        }

        /// <summary>
        /// Recorre los elementos tipo lookup y genera su código para el ciclo de vida.
        /// </summary>
        /// <returns></returns>
        private string BuildOnAfterRenderServiceItem()
        {
            StringBuilder strServiceItemData = new StringBuilder();

            foreach (WoItem item in _itemsWithServiceCol)
            {
                if (item.Control == "LookUpDialog")
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
        public void BuildCotrolsData(WoContainer container)
        {
            try
            {
                if (container.TypeContainer == eTypeContainer.FormRoot)
                {
                    _containersMethodsProperties.AppendLine(_woFormPartial.GetCode(container));
                }
                else if (container.TypeContainer == eTypeContainer.FormGroup)
                {
                    _containersMethodsProperties.AppendLine(_woFormGroupPartial.GetCode(container));
                }
                else if (container.TypeContainer == eTypeContainer.FormTab)
                {
                    _containersMethodsProperties.AppendLine(_woFormTabPartial.GetCode(container));
                }

                if (!container.ContainersCol.IsNull())
                {
                    foreach (var subgroup in container.ContainersCol)
                    {
                        BuildCotrolsData(subgroup);
                    }
                }

                if (!container.ItemsCol.IsNull())
                {
                    foreach (var item in container.ItemsCol)
                    {
                        if (item.Control == "Label")
                        {
                            if (item.Id == "Controles")
                            {
                                _complexItemsCol.Add(item);
                            }
                            else if (item.Id == "Alertas")
                            {
                                _componentsMethodsProperties.AppendLine(
                                    _woAlertListPartial.GetCode(item)
                                );
                            }
                            else if (item.Id.Contains("Report"))
                            {
                                _componentsMethodsProperties.AppendLine(
                                    _woReportViewerPartial.GetCode(item)
                                );
                            }
                        }
                        else if (
                            item.Control == "Decimal"
                            || item.BindingType.ToLower().Contains("decimal")
                            || item.BindingType.ToLower().Contains("double")
                        )
                        {
                            var dataInputText = _woMaskedEditPartial.GetCode(item: item);
                            _componentsMethodsProperties.AppendLine(dataInputText);
                        }
                        else if (item.Control == "Text" && !item.BindingType.Contains("byte[]"))
                        {
                            if (
                                item.BindingType.ToLower() == "string"
                                || item.BindingType.ToLower() == "string?"
                            )
                            {
                                var dataInputText = _woTextEditPartial.GetCode(item: item);
                                _componentsMethodsProperties.AppendLine(dataInputText);
                            }
                            else
                            {
                                var dataInputText = _woMaskedEditPartial.GetCode(item: item);
                                _componentsMethodsProperties.AppendLine(dataInputText);
                            }
                        }
                        else if (item.Control == "Urn")
                        {
                            var dataInputText = _woTextEditPartial.GetCode(item: item);
                            _componentsMethodsProperties.AppendLine(dataInputText);
                        }
                        else if (item.Control == "Memo")
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woMemoEditPartial.GetCode(item)
                            );
                        }
                        else if (item.Control == "Spin")
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woSpinEditPartial.GetCode(item)
                            );
                        }
                        else if (item.Control == "Bool")
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woCheckBoxEditPartial.GetCode(item)
                            );
                        }
                        else if (item.Control == "Date")
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woDateEditPartial.GetCode(item)
                            );
                        }
                        else if (item.Control == "LookUp")
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woLookUpEditPartial.GetCode(item)
                            );
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
                        else if (item.Control == "Slave")
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woDetailGridPartial.GetCode(item)
                            );
                        }
                        else if (
                            item.Control == "Decimal"
                            || item.Control == "Custom"
                            || item.Control == "TextMask"
                        )
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woMaskedEditPartial.GetCode(item)
                            );
                        }
                        else if (item.Control == "File" || item.BindingType.Contains("byte[]"))
                        {
                            _componentsMethodsProperties.AppendLine(
                                _woFileEditPartial.GetCode(item)
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al construir el controls. {ex.Message}");
            }
        }

        #endregion CommonControls


        #region Complex items

        /// <summary>
        /// Genera el codigo nesesario para el lookup que se utiliza por otros componentes.
        /// </summary>
        private WoLookUpDialogEditAdded _woLookUpDialogEditAdded = null;

        /// <summary>
        /// Genera el codigo nesesario para el lookup que se utiliza por otros componentes.
        /// </summary>
        private WoLookUpEditAdded _woLookUpEditAdded = null;

        /// <summary>
        /// Recorre la lista de controles complejos y genera el codigo nesesario para generar su parcial.
        /// Tipo ´Controles´:  Se genera el código para las transiciones de los lookups
        /// </summary>
        private void BuildComplexItems()
        {
            foreach (WoItem item in _complexItemsCol)
            {
                StringBuilder strLookUpData = new StringBuilder();

                if (item.Id == "Controles")
                {
                    foreach (WoItem serviceItem in _itemsWithServiceCol)
                    {
                        if (serviceItem.Control == "LookUp")
                        {
                            strLookUpData.AppendLine(
                                _woLookUpEditAdded.GetCodeTransitions(serviceItem)
                            );
                        }
                    }
                }

                _strComplexItems.AppendLine(
                    _woFormToolbarPartial.GetCode(item, strLookUpData.ToString())
                );
            }
        }

        #endregion Complex items


        #endregion Controls

        #endregion Body
    }
}
