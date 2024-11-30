using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoDesignerRawSerializerBaseHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto.
        /// Para enviar alertas e información al log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Método principal
        public WoContainer BuidBaseRaw(string layoutName)
        {
            _rootBase.ModelId = layoutName;
            return _rootBase;
        }
        #endregion Método principal


        #region Creación de componentes base

        /// <summary>
        /// Instancia del contenedor base.
        /// </summary>
        private WoContainer _rootBase = new WoContainer()
        {
            Id = "Root",
            Etiqueta = "Root",
            TypeContainer = eTypeContainer.FormRoot,
            Col = 12,
            Row = 6,
            ColSpan = 1,
            RowSpan = 1,
            BackgorundColorGroup = eGroupColor.Background,
            Theme = "Default"
        };

        /// <summary>
        /// Indica en que row poner el siguiente control dentro del contenedor base.
        /// </summary>
        private int _indexRoot = 0;

        #endregion Creación de componentes base
    }
}
