using WooW.SB.Config;
using WooW.SB.Helpers;

namespace WooW.SB.Interfaces
{
    public interface IForm
    {
        /// <summary>
        /// Es el ribbon del formulario para efectos de integrarlo al ribbon principal
        /// </summary>
        DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon { get; }

        /// <summary>
        /// Indica si el formulario tiene cambios pendientes
        /// </summary>
        bool CambiosPendientes { get; }

        /// <summary>
        /// Nombre del formulario
        /// </summary>
        string Nombre { get; }

        WooWConfigParams wooWConfigParams { get; set; }

        /// <summary>
        /// Metodo que se ejecuta al cargar el formulario
        /// </summary>
        void Cargar();

        /// <summary>
        /// Metodo para refrescar el formulario
        /// </summary>
        void Refrescar();

        /// <summary>
        /// Proyecto que esta asociado al formulario
        /// </summary>
        Proyecto proyecto { get; set; }
    }
}
