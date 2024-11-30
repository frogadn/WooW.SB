using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpEdit
{
    public class WoLookUpEditPartial
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Contiene la definición del componente en código, contiene
        /// todo el código necesario para construir un LookUP Edit
        /// </summary>
        private StringBuilder _lookUpEdit = new StringBuilder();

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Contiene strings para poder dentar el código
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Lkp";

        /// <summary>
        /// Prefijo del componente en minúsculas
        /// </summary>
        private string _lowShortComponent = "lkp";

        /// <summary>
        /// nombre para servicios e index del classModelType
        /// </summary>
        private string _classModelTypeMultiple = string.Empty;

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="modelName"></param>
        public WoLookUpEditPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar la generación del código del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _classModelTypeMultiple = _item.ClassModelType;
            if (_item.MultipleReference > 0)
            {
                _classModelTypeMultiple = $@"{_classModelTypeMultiple}{_item.MultipleReference}";
            }

            _lookUpEdit.Clear();

            _lookUpEdit.AppendLine($@"{_identMethodsAndProperties}#region {item.BindedProperty}");

            _lookUpEdit.AppendLine(BuildTag());

            _lookUpEdit.AppendLine(BuildRefComponent());

            _lookUpEdit.AppendLine(BuildSetStatus());

            _lookUpEdit.AppendLine(BuildRefContainer());

            _lookUpEdit.AppendLine(BuildSetStatusContainer());

            _lookUpEdit.AppendLine(BuildRefAlert());

            _lookUpEdit.AppendLine(BuildSetStatusAlert());

            _lookUpEdit.AppendLine(BuildLookupDataRefs());

            _lookUpEdit.AppendLine(BuildLookupData());

            _lookUpEdit.AppendLine(BuildServiceRefs());

            _lookUpEdit.AppendLine(BuildService());

            _lookUpEdit.AppendLine(BuildStates());

            _lookUpEdit.AppendLine(BuildValueChange());

            _lookUpEdit.AppendLine(
                $@"{_identMethodsAndProperties}#endregion {item.BindedProperty}"
            );

            _partialReady.Details =
                $@"Se creo el código parcial para el componente: {item.BindedProperty}.";
            _observer.SetLog(_partialReady);

            return _lookUpEdit.ToString();
        }
        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoLookUpEditPartial.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoLookUpEdit\WoLookUpEditPartial.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag

        #region Construcción del código

        /// <summary>
        /// Referencia del componente que se esta generando.
        /// </summary>
        /// <returns></returns>
        private string BuildRefComponent()
        {
            return $@"
        #region Item

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia que representa el componente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoLookupEdit<WoColumnMetaAttribute, WoLabelModelAttribute, {_item.ClassModelType}, App>? _{_lowShortComponent}{_item.BindedProperty};";
        }

        /// <summary>
        /// Construye la función para enlazar los eventos de la vista al componente
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatus()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Suscribe los controladores de eventos de la vista del componente a los
{_identMethodsAndProperties}/// eventos del componente a través de la referencia.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public async void SetStatus{_shortComponent}{_item.BindedProperty}()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_{_lowShortComponent}{_item.BindedProperty} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeEnabledEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeStatus;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeSizeEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeFontSize;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontColorEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontColor;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontWeightEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontWeight;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontItalicEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontItalic;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontDecorationEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontDecoration;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeBackColorEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeBackgroundColor;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ActualizarComponente();
{_identMethodsAndProperties}        CheckForm();
{_identMethodsAndProperties}        await {_lowShortComponent}{_item.BindedProperty}_InitializeServices();
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Item";
        }

        /// <summary>
        /// Referencia del componente que se esta generando.
        /// </summary>
        /// <returns></returns>
        ///
        /// <summary>
        /// Referencia del contenedor que se esta generando.
        /// </summary>
        /// <returns></returns>
        private string BuildRefContainer()
        {
            return $@"
{_identMethodsAndProperties}#region Contenedor

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia del contenedor del item.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoFormItem? _fli{_item.BindedProperty};";
        }

        /// <summary>
        /// Construye la función para enlazar los eventos de la vista a los eventos del contenedor.
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatusContainer()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Suscribe los controladores de eventos de la vista del contenedor a los 
{_identMethodsAndProperties}/// eventos del contenedor a través de la referencia.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""status""></param>
{_identMethodsAndProperties}public void SetStatusFli{_item.BindedProperty}()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_fli{_item.BindedProperty} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontColorEvt += _fli{_item.BindedProperty}.ChangeCaptionFontColor;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontWeightEvt += _fli{_item.BindedProperty}.ChangeCaptionFontWeight;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontItalicEvt += _fli{_item.BindedProperty}.ChangeCaptionFontItalic;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontDecorationEvt += _fli{_item.BindedProperty}.ChangeCaptionFontDecoration;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeSizeEvt += _fli{_item.BindedProperty}.ChangeFontSize;
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Contenedor";
        }

        /// <summary>
        /// Referencia de la alerta del input.
        /// </summary>
        /// <returns></returns>
        private string BuildRefAlert()
        {
            return $@"
{_identMethodsAndProperties}#region Alertas

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia de la alerta del item.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoInputAlert? _wia{_item.BindedProperty};";
        }

        /// <summary>
        /// Suscribe los controladores de eventos de la vista a los eventos del componente a través de la referencia.
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatusAlert()
        {
            return $@"
 {_identMethodsAndProperties}/// <summary>
 {_identMethodsAndProperties}/// Suscribe los controladores de eventos de la vista a los eventos de la alerta a través de la referencia.
 {_identMethodsAndProperties}/// </summary>
 {_identMethodsAndProperties}public void SetStatusWia{_item.BindedProperty}()
 {_identMethodsAndProperties}{{
 {_identMethodsAndProperties}    if (_wia{_item.BindedProperty} != null)
 {_identMethodsAndProperties}    {{
 {_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Alert.SetAlertValueEvt += _wia{_item.BindedProperty}.SetAlertValue;
 {_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Alert.ClearAlertsEvt += _wia{_item.BindedProperty}.ClearAlerts;
 {_identMethodsAndProperties}    }}
 {_identMethodsAndProperties}}}
{_identMethodsAndProperties}#endregion Alertas";
        }

        private string BuildLookupDataRefs()
        {
            return $@"
{_identMethodsAndProperties}#region Data del LookUp

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Contiene los componentes para pintar la grid dentro del combo.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private IEnumerable<MemberInfo>? _{_lowShortComponent}{_classModelTypeMultiple}ColumnsCol;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Nombre de la propiedad de id del modelo.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string _id{_classModelTypeMultiple}PropertyName = string.Empty;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Nombre de la propiedad que servirá de descripción en el lookup
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string _description{_classModelTypeMultiple}PropertyName = string.Empty;
";
        }

        private string BuildLookupData()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Recupera la instancia de configuración del modelo principal y a través de este recupera la
{_identMethodsAndProperties}/// información para: las columnas que se muestran en el combo, los nombres de la propiedades de
{_identMethodsAndProperties}/// descripción y de id, y la información del filtro del dto para la realización de peticiones.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}/// <exception cref=""Exception""></exception>
{_identMethodsAndProperties}public async Task<(string select, string filter, string orderBy)> {_shortComponent}{_item.BindedProperty}_OnInitialize()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        WoLookUpConfigAttribute? woLookUpConfig{_item.BindedProperty} = typeof({_item.ClassModelType})
{_identMethodsAndProperties}            .GetCustomAttributes(typeof(WoLookUpConfigAttribute), false)
{_identMethodsAndProperties}            .Cast<WoLookUpConfigAttribute>()
{_identMethodsAndProperties}            .FirstOrDefault();
{_identMethodsAndProperties}
{_identMethodsAndProperties}        if (woLookUpConfig{_item.BindedProperty} != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _id{_classModelTypeMultiple}PropertyName = woLookUpConfig{_item.BindedProperty}.KeyField;
{_identMethodsAndProperties}            _description{_classModelTypeMultiple}PropertyName = woLookUpConfig{_item.BindedProperty}.DescriptionField;
{_identMethodsAndProperties}
{_identMethodsAndProperties}            _{_lowShortComponent}{_classModelTypeMultiple}ColumnsCol = from Member in typeof({_item.ClassModelType}).GetMembers()
{_identMethodsAndProperties}                                           join SelectMember in woLookUpConfig{_item.BindedProperty}.Select.Split("","").ToList()
{_identMethodsAndProperties}                                           on Member.Name equals SelectMember
{_identMethodsAndProperties}                                           select Member;

{_identMethodsAndProperties}            return (woLookUpConfig{_item.BindedProperty}.Select, woLookUpConfig{_item.BindedProperty}.Filter, woLookUpConfig{_item.BindedProperty}.OrderBy);
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            throw new Exception(""_woLookUpConfig{_item.BindedProperty} es nulo"");
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        throw new Exception(ex.Message);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Data del LookUp";
        }

        private string BuildServiceRefs()
        {
            StringBuilder strServiceRef = new StringBuilder();

            strServiceRef.AppendLine($@"{_identMethodsAndProperties}#region Servicios");

            strServiceRef.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Lista de los datos que mostrara el combo interno.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private IEnumerable<{_item.ClassModelType}>? _{_classModelTypeMultiple}Col;"
            );

            strServiceRef.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio del lookup de consulta de los datos.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoLookupService<{_item.ClassModelType}, {_item.ClassModelType}Get, {_item.ClassModelType}List>? _{_lowShortComponent}Service{_item.BindedProperty};

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Tupla con la data del select, filter y orderby del dto.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private (string select, string filter, string orderBy)? _dtoSettings{_item.BindedProperty};
"
            );

            return strServiceRef.ToString();
        }

        private string BuildService()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Inicializa los servicios del componente.
{_identMethodsAndProperties}/// Detonar en el Set Status del componente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private async Task {_lowShortComponent}{_item.BindedProperty}_InitializeServices()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _dtoSettings{_item.BindedProperty} = await {_shortComponent}{_item.BindedProperty}_OnInitialize();
{_identMethodsAndProperties}    _{_lowShortComponent}Service{_item.BindedProperty} = new WoLookupService<{_item.ClassModelType}, {_item.ClassModelType}Get, {_item.ClassModelType}List>(Client, _dtoSettings{_item.BindedProperty});
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Actualiza la data de la lista.
{_identMethodsAndProperties}/// Va al servicio y realiza una consulta en función de la data del dto recuperada al
{_identMethodsAndProperties}/// inicial el componente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}public async Task {_item.BindedProperty}_UpdateData()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_{_lowShortComponent}Service{_item.BindedProperty} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _{_classModelTypeMultiple}Col = await _{_lowShortComponent}Service{_item.BindedProperty}.GetDataList();
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Servicios";
        }

        private string BuildStates()
        {
            StringBuilder strStates = new StringBuilder();
            strStates.AppendLine($@"{_identMethodsAndProperties}#region Estados");

            if (_item.ClassModelType != string.Empty)
            {
                ModelHelper modelHelper = new ModelHelper();

                Modelo lokUpModel = modelHelper.SearchModel(_item.ClassModelType);
                ModeloColumna column = lokUpModel
                    .Columnas.Where(x => x.EsVisibleEnLookUp && x.Id != "Id")
                    .FirstOrDefault();

                strStates.AppendLine(
                    $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Cambia el componente a modo de navegación,
{_identMethodsAndProperties}/// solo con los datos del modelo.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void {_shortComponent}{_item.BindedProperty}_NavigationMode()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    {_item.ClassModelType} simpleData = new {_item.ClassModelType}();
{_identMethodsAndProperties}    simpleData.Id = _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty};
{_identMethodsAndProperties}
{_identMethodsAndProperties}    simpleData.{column.Id} = _scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty};
{_identMethodsAndProperties}
{_identMethodsAndProperties}    _{_classModelTypeMultiple}Col = new List<{_item.ClassModelType}>() {{ simpleData }};
{_identMethodsAndProperties}}}"
                );
            }
            else
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar generar el LookUp {_item.Id}, es probable que el tipo {_item.BindingType} o el control {_item.Control} sea incorrecto para la propiedad {_item.BindedProperty}",
                    caption: "Error",
                    buttons: System.Windows.Forms.MessageBoxButtons.OK,
                    icon: System.Windows.Forms.MessageBoxIcon.Error
                );
            }

            strStates.AppendLine($@"{_identMethodsAndProperties}#endregion Estados");

            return strStates.ToString();
        }

        private string BuildValueChange()
        {
            return $@"

{_identMethodsAndProperties}#region Cambio de valor

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Evento del componente que se ejecuta cuando se modifica el valor del componente {_item.BindedProperty}.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void {_item.BindedProperty}_OnChange(string newValue)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty} = newValue;

{_identMethodsAndProperties}    _scriptsUser.{_item.BindedProperty}_OnChange();

{_identMethodsAndProperties}    var searchResult = _{_classModelTypeMultiple}Col?.Where(x => x.Id == newValue).FirstOrDefault();

{_identMethodsAndProperties}    PropertyInfo? propertyDescription = typeof({_item.ClassModelType}).GetProperty(_description{_item.ClassModelType}PropertyName);

{_identMethodsAndProperties}    if (searchResult != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        string? description = propertyDescription?.GetValue(searchResult)?.ToString();
{_identMethodsAndProperties}        _scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty} = description;
{_identMethodsAndProperties}
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Alert.LimpiarAlertas();
{_identMethodsAndProperties}       {_item.BaseModelName}Validate(""{_item.BindedProperty}"", _modelControls.{_item.BindedProperty}Alert);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    else
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty} = string.Empty;
{_identMethodsAndProperties}        _scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty} = string.Empty;
{_identMethodsAndProperties}
{_identMethodsAndProperties}        {_item.BaseModelName}Validate(""{_item.BindedProperty}"", _modelControls.{_item.BindedProperty}Alert);
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Alert.AgregarAlerta(""No se encontró el registro"");
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Cambio de valor";
        }

        #endregion Construcción del código

        #region Logs
        /// <summary>
        /// Log del sistema
        /// </summary>
        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el código interno de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoLookUpEditPartial",
                MethodOrContext = "GetLookUpEditPartial"
            }
        };

        #endregion Logs
    }
}
