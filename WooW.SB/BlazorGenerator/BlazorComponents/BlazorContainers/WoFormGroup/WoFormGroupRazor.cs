using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormGroup
{
    public class WoFormGroupRazor
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
        private string _shortComponent = "Wfg";

        /// <summary>
        /// Prefijo del componente en minusculas.
        /// </summary>
        private string _lowShortComponent = "wfg";

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

            string decoration =
                (container.Visible) ? "eGroupDecoration.Card" : "eGroupDecoration.None";

            _strInstanceOpen.AppendLine(
                $@"
{_identMethodsAndProperties}{BuildTag()}
{_identMethodsAndProperties}@*Grupo {container.Id}*@
{_identMethodsAndProperties}<WoFormGroup Caption=""@Localizer[""{container.Etiqueta}""]"" 
{_identMethodsAndProperties}             ColSpan={container.ColSpan} 
{_identMethodsAndProperties}             BeginRow=""{container.BeginRow.ToString().ToLower()}""
{_identMethodsAndProperties}             SetStatus=""@SetStatus{_shortComponent}{container.Id}""
{_identMethodsAndProperties}             Decoration=""@{decoration}""
{_identMethodsAndProperties}             Collapsible = ""@true""
{_identMethodsAndProperties}             IsExpanded=""@true""
{_identMethodsAndProperties}             @ref=""@_{_lowShortComponent}{container.Id}"">"
            );
            _strInstanceClose.AppendLine($@"{_identMethodsAndProperties}</WoFormGroup>");

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
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoFormGroupRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorContainers\WoFormGroup\WoFormGroupRazor.cs*@
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
                Class = "WoFormGroupRazor",
                MethodOrContext = "GetFormGroupRazor"
            }
        };

        #endregion Logs
    }
}
