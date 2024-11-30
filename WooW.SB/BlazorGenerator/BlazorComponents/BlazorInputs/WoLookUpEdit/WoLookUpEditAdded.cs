using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpEdit
{
    public class WoLookUpEditAdded
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Lkp";

        /// <summary>
        /// Prefijo del componente en minusculas
        /// </summary>
        private string _lowShortComponent = "lkp";

        #endregion Atributos


        #region Constructores

        /// <summary>
        /// Constructor principal de la clase, inicializa el atributo de identación.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        public WoLookUpEditAdded(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructores

        #region Codigo para las tranciciones

        /// <summary>
        /// Recupera el metodo nesesario para el funcionamiento del lookup en las tranciciones.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCodeTransitions(WoItem item)
        {
            return $@"{_identMethodsAndProperties}                {_shortComponent}{item.BindedProperty}_NavigationMode();";
        }

        #endregion Codigo para las tranciciones
    }
}
