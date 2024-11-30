using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoFormToolbar
{
    public class WoFormToolbarView
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Salva la informacion de la instancia de la vista del componente.
        /// </summary>
        private StringBuilder _strInstance = new StringBuilder();

        /// <summary>
        /// Instancia con la informacion de la vista del componente.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Identacion del codigo.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        public WoFormToolbarView(string identMethodsAndProperties)
        {
            this._identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor

        #region Metodo principal

        /// <summary>
        /// Recupera el codigo de la instancia de la vista.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _strInstance.Clear();

            _strInstance.AppendLine($@"{_identMethodsAndProperties}#region Toolbar");

            _strInstance.AppendLine(BuildTag());

            _strInstance.AppendLine(BuildToolBarView());

            _strInstance.AppendLine(BuildSharedView());

            _strInstance.AppendLine($@"{_identMethodsAndProperties}#endregion Toolbar");

            _viewInstanceReady.Details =
                $@"La instancia de la view con el nombre {item.Id} fue creada";
            _observer.SetLog(_viewInstanceReady);

            return _strInstance.ToString();
        }

        #endregion Metodo principal


        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoRecordToolBarView.cs";
        }

        #endregion Tag


        #region Construcción del codigo

        private string BuildToolBarView()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del toolbar.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoFormToolbarView {_item.Id} {{ get; set; }} = new WoFormToolbarView();";
        }

        /// <summary>
        /// Construye el codigo para la vista del boton de compartir del record.
        /// </summary>
        /// <returns></returns>
        private string BuildSharedView()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del boton de compartir del toolbar.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoShareButtonView Compartir {{ get; set; }} = new WoShareButtonView();";
        }

        #endregion Construcción del codigo


        #region Logs

        private WoLog _viewInstanceReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado la instancia de la view de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoRecordToolBarView",
                MethodOrContext = "GetViewInstance"
            }
        };

        #endregion Logs
    }
}
