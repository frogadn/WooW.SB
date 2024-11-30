using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMemoEdit
{
    public class WoMemoEditView
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
        /// todo el codigo necesario para construir un MemoEdit
        /// </summary>
        private StringBuilder _memoEdit = new StringBuilder();

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
        public WoMemoEditView(string _identMethodsAndProperties)
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

            _memoEdit.Clear();

            _memoEdit.AppendLine($@"{_identMethodsAndProperties}#region {item.BindedProperty}");

            _memoEdit.AppendLine(BuildTag());

            _memoEdit.AppendLine(BuildComponent());

            _memoEdit.AppendLine(BuildContainerComponent());

            _memoEdit.AppendLine(BuildAlertComponent());

            _memoEdit.AppendLine($@"{_identMethodsAndProperties}#endregion {item.BindedProperty}");

            _partialReady.Details = $@"Se creo la view para el componente: {item.BindedProperty}.";
            _observer.SetLog(_partialReady);

            return _memoEdit.ToString();
        }
        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoMemoEditView.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoMemoEdit\WoMemoEditView.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag

        #region Construcción del código

        /// <summary>
        /// Instancia del componente
        /// </summary>
        /// <returns></returns>
        private string BuildComponent()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del input de texto Usuario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoEditView {_item.BindedProperty} {{ get; set; }} = new WoEditView();";
        }

        /// <summary>
        /// Contenedor del componente
        /// </summary>
        /// <returns></returns>
        private string BuildContainerComponent()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del input de texto Usuario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoFormItemView {_item.BindedProperty}Container {{ get; set; }} = new WoFormItemView();";
        }

        /// <summary>
        /// Alerta del componente
        /// </summary>
        /// <returns></returns>
        private string BuildAlertComponent()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del input de texto Usuario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoInputAlertView {_item.BindedProperty}Alert {{ get; set; }} = new WoInputAlertView();";
        }

        #endregion Construcción del código

        #region Logs

        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado la view de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoMemoEditView",
                MethodOrContext = "GetMemoEditView"
            }
        };

        #endregion Logs
    }
}
