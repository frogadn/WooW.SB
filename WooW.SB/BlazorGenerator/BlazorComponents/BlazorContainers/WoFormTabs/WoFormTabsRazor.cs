using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormTabs
{
    public class WoFormTabsRazor
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Resultado final del codigo de la etiqueta de apertura del componente.
        /// </summary>
        private StringBuilder _strInstanceOpen = new StringBuilder();

        /// <summary>
        /// Resultado final del codigo de la etiqueta de cierre del componente.
        /// </summary>
        private StringBuilder _strInstanceClose = new StringBuilder();

        /// <summary>
        /// Identacion del codigo que se genera.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public (string containerOpen, string containerClose) GetCode(
            string identItemProperty,
            WoContainer container
        )
        {
            this._identMethodsAndProperties = identItemProperty;

            _strInstanceOpen.Clear();
            _strInstanceClose.Clear();

            _strInstanceOpen.AppendLine(
                $@"
{_identMethodsAndProperties}{BuildTag()}
{_identMethodsAndProperties}@*Grupo de tabs*@
{_identMethodsAndProperties}<WoFormTabs ColSpan={container.ColSpan} BeginRow=""{container.BeginRow.ToString().ToLower()}"">"
            );

            _strInstanceClose.AppendLine($@"{_identMethodsAndProperties}</WoFormTabs>");

            _razorReady.Details =
                $@"Se creo el código razor para el componente contenedor: {container.Id}.";
            _observer.SetLog(_razorReady);

            return (_strInstanceOpen.ToString(), _strInstanceClose.ToString());
        }
        #endregion Método principal


        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoFormTabsRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorContainers\WoFormTabs\WoFormTabsRazor.cs.*@
{_identMethodsAndProperties}@*WoWSB por el generador a día 5-10-2023*@";
        }

        #endregion Tag

        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoFormTabsRazor",
                MethodOrContext = "GetFormTabsRazor"
            }
        };

        #endregion Logs
    }
}
