using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm
{
    public class WoFormRazor
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

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo razor del componente.
        /// </summary>
        /// <param name="container"></param>
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
{_identMethodsAndProperties}@*Formulario {container.Id}*@
{_identMethodsAndProperties}<WoForm SetStatus=""@SetStatus{_shortComponent}{container.Id}"" @ref=""@_{_lowShortComponent}{container.Id}"">"
            );

            _strInstanceClose.AppendLine($@"</WoForm>");

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
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoFormRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorContainers\WoForm\WoFormRazor.cs*@
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
                Class = "WoFormRazor",
                MethodOrContext = "GetFormRazor"
            }
        };

        #endregion Logs
    }
}
