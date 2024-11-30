using System;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm
{
    public class WoFormScriptsUser
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

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
        /// Contiene el código del usuario que se ejecutara al iniciar el formulario.
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private String _userCode = "";

        /// <summary>
        /// Identacion del codigo que se genera.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos


        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        public WoFormScriptsUser(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public string GetCode(WoItem item, String userCode)
        {
            _item = item;
            _userCode = userCode;

            _strInstance.Clear();

            _strInstance.AppendLine($@"{_identMethodsAndProperties}#region Formulario");

            _strInstance.AppendLine(BuildTag());

            _strInstance.AppendLine(BuildOnInitializeUserCode());

            _strInstance.AppendLine($@"{_identMethodsAndProperties}#endregion Formulario");

            _partialReady.Details =
                $@"Se creo el código con el método que ejecuta el código del usuario al iniciar el formulario";
            _observer.SetLog(_partialReady);

            return _strInstance.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoFormScriptsUser.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorContainers\WoForm\WoFormScriptsUser.cs
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag

        #region Construcción del código

        /// <summary>
        /// Genera el método FormularioIniciado con el código del usuario
        /// </summary>
        /// <returns></returns>
        private string BuildOnInitializeUserCode()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Metodo que se ejecuta cuando se inicia el formulario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public async Task FormularioIniciado()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    // Codigo que se ejecutara al iniciar el formulario.
{_identMethodsAndProperties}    {_userCode}  
{_identMethodsAndProperties}}}";
        }

        #endregion Construcción del código

        #region Logs

        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el contenedor del script del usuario.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoCheckBoxEditScriptUser",
                MethodOrContext = "GetCheckBoxEditScriptUser"
            }
        };

        #endregion Logs
    }
}
