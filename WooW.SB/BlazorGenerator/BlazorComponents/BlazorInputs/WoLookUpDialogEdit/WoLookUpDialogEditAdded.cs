using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpDialogEdit
{
    public class WoLookUpDialogEditAdded
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
        private string _shortComponent = "Lkd";

        /// <summary>
        /// Prefijo del componente en minusculas
        /// </summary>
        private string _lowShortComponent = "lkd";

        /// <summary>
        /// nombre para servicios e index del classModelType
        /// </summary>
        private string _classModelTypeMultiple = string.Empty;

        #endregion Atributos

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase, inicializa el atributo de identación.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        public WoLookUpDialogEditAdded(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor


        #region Código para el ciclo de vida

        /// <summary>
        /// Recupera el código necesario para el funcionamiento del lookup dialog en el ciclo de vida.
        /// </summary>
        /// <returns></returns>
        public string GetCodeLifeCycle(WoItem item)
        {
            _classModelTypeMultiple = item.ClassModelType;
            if (item.MultipleReference > 0)
            {
                _classModelTypeMultiple = $@"{_classModelTypeMultiple}{item.MultipleReference}";
            }

            return $@"
{_identMethodsAndProperties}                // Este código fue generado por el fichero WoLookUpDialogEditAdded.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoLookUpDialogEdit\WoLookUpDialogEditAdded.cs
{_identMethodsAndProperties}                _{_classModelTypeMultiple}Service = new WoLookupDialogService<{item.ClassModelType}, {item.ClassModelType}Get, {item.ClassModelType}List>(Client);
";
        }

        public string GetCodeOnAfterRender(WoItem item)
        {
            _classModelTypeMultiple = item.ClassModelType;
            if (item.MultipleReference > 0)
            {
                _classModelTypeMultiple = $@"{_classModelTypeMultiple}{item.MultipleReference}";
            }

            return $@"
                if(_{_lowShortComponent}{item.Id} != null)
                {{
                    _{_lowShortComponent}{item.Id}.WoTarget = Client;
                }}";
        }

        #endregion Código para el ciclo de vida
    }
}
