using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.CheckBoxEdit
{
    internal class WoCheckBoxEditScriptUser
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
        /// todo el codigo necesario para construir un TextEdit
        /// </summary>
        private StringBuilder _checkBoxEdit = new StringBuilder();

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
        public WoCheckBoxEditScriptUser(string _identMethodsAndProperties)
        {
            this._identMethodsAndProperties = _identMethodsAndProperties;
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

            _checkBoxEdit.Clear();

            _checkBoxEdit.AppendLine($@"{_identMethodsAndProperties}#region {item.BindedProperty}");

            _checkBoxEdit.AppendLine(BuildTag());

            _checkBoxEdit.AppendLine(BuildOnChangeUserCode());

            _checkBoxEdit.AppendLine(
                $@"{_identMethodsAndProperties}#endregion {item.BindedProperty}"
            );

            _partialReady.Details = $@"Se creo la view para el componente: {item.BindedProperty}.";
            _observer.SetLog(_partialReady);

            return _checkBoxEdit.ToString();
        }
        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoCheckBoxEditScriptUser.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoCheckBoxEdit\WoCheckBoxEditScriptUser.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag


        #region Construcción del código

        /// <summary>
        /// Instancia del componente
        /// </summary>
        /// <returns></returns>
        private string BuildOnChangeUserCode()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Evento que se cacha cuando existe un cambio.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void {_item.BindedProperty}_OnChange()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}   //Coloque su código aquí.
{_identMethodsAndProperties}}}
{_identMethodsAndProperties}public void {_item.BindedProperty}_OnFocus()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}   //Coloque su código aquí.
{_identMethodsAndProperties}}}
{_identMethodsAndProperties}public void {_item.BindedProperty}_OnBlur()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}   //Coloque su código aquí.
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
