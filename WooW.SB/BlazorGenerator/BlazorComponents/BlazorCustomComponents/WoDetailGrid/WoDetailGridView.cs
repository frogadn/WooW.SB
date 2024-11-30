using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoDetailGrid
{
    public class WoDetailGridView
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Salva la informacion razor del componente.
        /// </summary>
        private StringBuilder _strInstance = new StringBuilder();

        /// <summary>
        /// Informacion del componente que se esta generando.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Identacion del codigo generado.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor principal del componente.
        /// </summary>
        /// <param name="woItem"></param>
        public WoDetailGridView(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor


        #region Metodo principal

        /// <summary>
        /// Genera la instacia de la vista del componente.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _strInstance.Clear();
            _strInstance.AppendLine(BuildTag());
            _strInstance.Append(BuildInstance());

            _viewInstanceReady.Details =
                $@"La instancia de la view con el nombre {_item.BindedProperty} fue creada";
            _observer.SetLog(_viewInstanceReady);

            return _strInstance.ToString();
        }

        #endregion Metodo principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoDetailGridView.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorCustomComponents\WoDetailGrid\WoDetailGridView.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag

        #region Build code

        /// <summary>
        /// Construlle la instancia de la vista del componente.
        /// </summary>
        /// <returns></returns>
        private string BuildInstance()
        {
            return $@"
{_identMethodsAndProperties}#region {_item.BindedProperty}

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del input de tipo detalle.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoDetailGridView {_item.BindedProperty} {{ get; set; }} = new WoDetailGridView();

{_identMethodsAndProperties}#endregion {_item.BindedProperty}
";
        }

        #endregion Build code


        #region Logs

        private WoLog _viewInstanceReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado la instancia de la view de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoTextEditInstanceView",
                MethodOrContext = "GetViewInstance"
            }
        };

        #endregion Logs
    }
}
