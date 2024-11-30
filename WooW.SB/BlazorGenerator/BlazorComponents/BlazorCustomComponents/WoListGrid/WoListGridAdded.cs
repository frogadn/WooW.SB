using System.Text;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoListGrid
{
    public class WoListGridAdded
    {
        #region Attributes

        /// <summary>
        /// Item con la información para la generación del razor.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Resultado final de lo que se tiene que agregar al razor.
        /// </summary>
        private StringBuilder _strGridResult = new StringBuilder();

        /// <summary>
        /// Identación al nivel de los métodos y propiedades.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Attributes


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase, inicializa la identación.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        public WoListGridAdded(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor


        #region Construcción del código para el ciclo de vida

        /// <summary>
        /// Construye el código para el ciclo de vida de blazor.
        /// </summary>
        /// <returns></returns>
        public string BuildOnInitializedCode(WoItem item)
        {
            _item = item;

            return $@"
{_identMethodsAndProperties}        {_item.Id}_GetColumns();";
        }

        /// <summary>
        /// Construye el código para el ciclo de vida de blazor luego del rende-rizado del razor.
        /// </summary>
        /// <returns></returns>
        public string BuildOnAfterRenderCode(WoItem item)
        {
            _item = item;

            return $@"
{_identMethodsAndProperties}        if (_{_item.Id}ListGrid != null && Client.BearerToken != null && _{_item.Id}ListGrid.WoTarget == null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _{_item.Id}ListGrid.WoTarget = Client;
{_identMethodsAndProperties}            StateHasChanged();
{_identMethodsAndProperties}        }}";
        }

        #endregion Construcción del código para el ciclo de vida
    }
}
