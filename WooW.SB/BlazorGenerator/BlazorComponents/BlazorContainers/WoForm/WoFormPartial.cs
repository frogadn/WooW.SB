using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm
{
    public class WoFormPartial
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Resultado final del codigo de la clase parcial del componente.
        /// </summary>
        private StringBuilder _strInstance = new StringBuilder();

        /// <summary>
        /// Instancia con la metadata del componente.
        /// </summary>
        private WoContainer _container = new WoContainer();

        /// <summary>
        /// Prefijo del componente.
        /// </summary>
        private string _shortComponent = "Wf";

        /// <summary>
        /// Prefijo del componente en minusculas.
        /// </summary>
        private string _lowShortComponent = "wf";

        /// <summary>
        /// Identacion del codigo que se genera.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        public WoFormPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo parcial del componente.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public string GetCode(WoContainer container)
        {
            _container = container;

            _strInstance.Clear();

            _strInstance.AppendLine($@"{_identMethodsAndProperties}#region {container.Id}");

            _strInstance.AppendLine(BuildTag());

            _strInstance.AppendLine(BuildRefComponent());

            _strInstance.AppendLine(BuildSetStatusMethod());

            _strInstance.AppendLine($@"{_identMethodsAndProperties}#endregion {container.Id}");

            _partialReady.Details =
                $@"Se creo el código parcial para el componente: {container.Id}.";
            _observer.SetLog(_partialReady);

            return _strInstance.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoFormPartial.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorContainers\WoForm\WoFormPartial.cs
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
{_identMethodsAndProperties}private WoForm? _{_lowShortComponent}{_container.Id};
";
        }

        /// <summary>
        /// Genera el método setStatus
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatusMethod()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Suscribe los controladores de eventos de la vista del componente a los
{_identMethodsAndProperties}/// eventos del componente a través de la referencia.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void SetStatus{_shortComponent}{_container.Id}()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_{_lowShortComponent}{_container.Id} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_container.Id}.ChangeFontWeightEvt += _{_lowShortComponent}{_container.Id}.ChangeFontWeight;
{_identMethodsAndProperties}        _modelControls.{_container.Id}.ChangeFontItalicEvt += _{_lowShortComponent}{_container.Id}.ChangeFontItalic;
{_identMethodsAndProperties}        _modelControls.{_container.Id}.ChangeFontDecorationEvt += _{_lowShortComponent}{_container.Id}.ChangeFontDecoration;
{_identMethodsAndProperties}        _modelControls.{_container.Id}.ChangeFontColorEvt += _{_lowShortComponent}{_container.Id}.ChangeFontColor;
{_identMethodsAndProperties}        _modelControls.{_container.Id}.ChangeBackColorEvt += _{_lowShortComponent}{_container.Id}.ChangeBackgroundColor;

{_identMethodsAndProperties}        _modelControls.{_container.Id}.ActualizarComponente();
{_identMethodsAndProperties}        CheckForm();
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}
";
        }

        #endregion Construcción del código

        #region Logs

        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el código interno de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoFormPartial",
                MethodOrContext = "GetFormPartial"
            }
        };

        #endregion Logs
    }
}
