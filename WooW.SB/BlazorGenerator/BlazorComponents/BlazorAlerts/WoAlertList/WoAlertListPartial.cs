using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorAlerts.WoAlertList
{
    public class WoAlertListPartial
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Resultado final del codigo de la clase parcial del componente.
        /// </summary>
        private StringBuilder _strInstance = new StringBuilder();

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Identacion del codigo que se genera.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        public WoAlertListPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo parcial del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _strInstance.Clear();

            _strInstance.Append($@"{_identMethodsAndProperties}#region {_item.Id}");

            _strInstance.AppendLine(BuildTag());

            _strInstance.AppendLine(BuildRefComponent());

            _strInstance.AppendLine(BuildSetStatusMethod());

            _strInstance.Append($@"{_identMethodsAndProperties}#endregion {_item.Id}");

            _partialReady.Details =
                $@"Se creo el código parcial para el componente de alertas: {_item.Id}.";
            _observer.SetLog(_partialReady);

            return _strInstance.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoAlertListPartial.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorAlerts\WoAlertList\WoAlertListPartial.cs
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
{_identMethodsAndProperties}/// Referencia que representa el componente de alertas.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoFormAlert? _alerts;
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
{_identMethodsAndProperties}public void SetStatusAlerts()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_alerts != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_item.Id}.AddAlertEvt += _alerts.AddAlert;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.RemoveAlertEvt += _alerts.RemoveAlert;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.CleanAlertsEvt += _alerts.CleanAll;
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
                Class = "WoAlertListPartial",
                MethodOrContext = "GetAlertListPartial"
            }
        };

        #endregion Logs
    }
}
