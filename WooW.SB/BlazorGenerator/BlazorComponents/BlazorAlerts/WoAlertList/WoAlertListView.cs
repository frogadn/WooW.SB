using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorAlerts.WoAlertList
{
    public class WoAlertListView
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
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="modelName"></param>
        public WoAlertListView(string _identMethodsAndProperties)
        {
            this._identMethodsAndProperties = _identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo parcial del componente.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _strInstance.Clear();

            _strInstance.Append(
                $@"
{_identMethodsAndProperties}#region {_item.Id}
{_identMethodsAndProperties}{BuildTag()}
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista de alertas.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoFormAlertView {_item.Id} {{ get; set; }} = new WoFormAlertView();

{_identMethodsAndProperties}#endregion {_item.Id}"
            );

            _viewInstanceReady.Details =
                $@"La instancia de la view con el nombre {_item.Id} fue creada";
            _observer.SetLog(_viewInstanceReady);

            return _strInstance.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoAlertListView.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorAlerts\WoAlertList\WoAlertListView.cs
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag

        #region Logs

        private WoLog _viewInstanceReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado la instancia de la view de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoAlertListView",
                MethodOrContext = "GetViewInstance"
            }
        };

        #endregion Logs
    }
}
