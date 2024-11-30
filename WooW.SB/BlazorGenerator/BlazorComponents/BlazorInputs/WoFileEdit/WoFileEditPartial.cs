using System.Text;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoFileEdit
{
    public class WoFileEditPartial
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
        /// todo el codigo necesario para construir un file edit
        /// </summary>
        private StringBuilder _strFileEdit = new StringBuilder();

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
        private string _shortComponent = "Fil";

        /// <summary>
        /// Prefijo del componente en minúsculas
        /// </summary>
        private string _lowShortComponent = "fil";

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="modelName"></param>
        public WoFileEditPartial(string _identMethodsAndProperties)
        {
            this._identMethodsAndProperties = _identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar la generación del codigo del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _strFileEdit.Clear();

            _strFileEdit.AppendLine($@"{_identMethodsAndProperties}#region {item.BindedProperty}");

            _strFileEdit.AppendLine(BuildTag());

            _strFileEdit.AppendLine(BuildRefComponent());

            _strFileEdit.AppendLine(BuildSetStatus());

            _strFileEdit.AppendLine(BuildRefContainer());

            _strFileEdit.AppendLine(BuildSetStatusContainer());

            _strFileEdit.AppendLine(BuildRefAlert());

            _strFileEdit.AppendLine(BuildSetStatusAlert());

            _strFileEdit.AppendLine(BuildOnChangeEvent());

            _strFileEdit.AppendLine(
                $@"{_identMethodsAndProperties}#endregion {item.BindedProperty}"
            );

            return _strFileEdit.ToString();
        }
        #endregion Método principal

        #region Tag

        /// <summary>
        /// Tag con la data de la version del generador
        /// </summary>
        /// <returns></returns>
        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoFileEditPartial.cs
{_identMethodsAndProperties}// WoWSB por el generador a día 02-04-2024";
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
{_identMethodsAndProperties}private WoFileInput? _{_lowShortComponent}{_item.BindedProperty};";
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
            StringBuilder strOnChange = new StringBuilder();

            string nulleablity = (_item.Nullable) ? "?" : string.Empty;

            string validate = string.Empty;
            string modelName = string.Empty;
            if (!_item.NoModelComponent)
            {
                validate =
                    $@"{_identMethodsAndProperties}    {_item.BaseModelName}Validate(""{_item.BindedProperty}"", _modelControls.{_item.BindedProperty}Alert);";
                modelName = $@"{_item.BaseModelName}.";
            }

            strOnChange.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Evento del componente que se ejecuta cuando se modifica el valor del componente {_item.BindedProperty}.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void {_item.BindedProperty}_OnChange({_item.BindingType} newValue)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _scriptsUser.{modelName}{_item.BindedProperty} = newValue;
{validate}
{_identMethodsAndProperties}    _scriptsUser.{_item.BindedProperty}_OnChange();
{_identMethodsAndProperties}}}"
            );

            return strOnChange.ToString();
        }

        #endregion Construcción del código
    }
}
