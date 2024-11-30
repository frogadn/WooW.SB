using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.CheckBoxEdit
{
    public class WoCheckBoxEditPartial
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Contiene la definición del componente en codigo, contiene
        /// todo el codigo necesario para construir un check edit
        /// </summary>
        private StringBuilder _checkEdit = new StringBuilder();

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Chk";

        /// <summary>
        /// Prefijo del componente en minusculas
        /// </summary>
        private string _lowShortComponent = "chk";

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="modelName"></param>
        public WoCheckBoxEditPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _checkEdit.Clear();

            _checkEdit.AppendLine($@"{_identMethodsAndProperties}#region {item.BindedProperty}");

            _checkEdit.AppendLine(BuildTag());

            _checkEdit.AppendLine(BuildRefComponent());

            _checkEdit.AppendLine(BuildSetStatus());

            _checkEdit.AppendLine(BuildRefContainer());

            _checkEdit.AppendLine(BuildSetStatusContainer());

            _checkEdit.AppendLine(BuildRefAlert());

            _checkEdit.AppendLine(BuildSetStatusAlert());

            _checkEdit.AppendLine(BuildOnChangeEvent());

            _checkEdit.AppendLine($@"{_identMethodsAndProperties}#endregion {item.BindedProperty}");

            _partialReady.Details =
                $@"Se creo el código parcial para el componente: {item.BindedProperty}.";
            _observer.SetLog(_partialReady);

            return _checkEdit.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoCheckBoxEditPartial.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoCheckBoxEdit\WoCheckBoxEditPartial.cs.
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
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia que representa el componente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoCheckBox<{_item.BindingType}> _{_lowShortComponent}{_item.BindedProperty};";
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
{_identMethodsAndProperties}public void SetStatus{_shortComponent}{_item.BindedProperty}()
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
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}";
        }

        /// <summary>
        /// Referencia del contenedor que se esta generando.
        /// </summary>
        /// <returns></returns>
        private string BuildRefContainer()
        {
            return $@"
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
{_identMethodsAndProperties}}}";
        }

        /// <summary>
        /// Referencia de la alerta del input.
        /// </summary>
        /// <returns></returns>
        private string BuildRefAlert()
        {
            return $@"
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
 {_identMethodsAndProperties}}}";
        }

        /// <summary>
        /// Contruye el evento que se ejecuta cuando se modifica el valor del componente.
        /// </summary>
        /// <returns></returns>
        private string BuildOnChangeEvent()
        {
            string nulleablity = (_item.Nullable) ? "?" : string.Empty;
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Evento del componente que se ejecuta cuando se modifica el valor del componente {_item.BindedProperty}.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void {_item.BindedProperty}_OnChange({_item.BindingType} newValue)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty} = newValue;
{_identMethodsAndProperties}    {_item.BaseModelName}Validate(""{_item.BindedProperty}"", _modelControls.{_item.BindedProperty}Alert);
{_identMethodsAndProperties}    _scriptsUser.{_item.BindedProperty}_OnChange();
{_identMethodsAndProperties}}}";
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
                Class = "WoCheckEditPartial",
                MethodOrContext = "GetCheckEditPartial"
            }
        };

        #endregion Logs
    }
}
