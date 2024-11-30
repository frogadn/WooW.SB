using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm
{
    public class WoFormView
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
        /// Identacion del codigo que se genera.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        public WoFormView(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo de la view del componente.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public string GetCode(WoContainer container)
        {
            _strInstance.Clear();

            _strInstance.AppendLine(
                $@"
{_identMethodsAndProperties}#region {container.Id}
{_identMethodsAndProperties}{BuildTag()}

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del formulario {container.Id}.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoFormView {container.Id} {{ get; set; }} = new WoFormView();

{_identMethodsAndProperties}#endregion {container.Id}"
            );

            _viewInstanceReady.Details =
                $@"La instancia de la view con el nombre {container.Id} fue creada";
            _observer.SetLog(_viewInstanceReady);

            return _strInstance.ToString();
        }
        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}//Este código fue generado por el fichero WoFormView.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorContainers\WoForm\WoFormView.cs
{_identMethodsAndProperties}//WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag

        #region Logs

        private WoLog _viewInstanceReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado la instancia de la view del formulario principal.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoFormView",
                MethodOrContext = "GetViewInstance"
            }
        };

        #endregion Logs
    }
}
