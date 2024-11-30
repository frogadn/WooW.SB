using System.ComponentModel;
using System.Drawing.Design;
using WooW.SB.Config;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer.DesignerModels
{
    public class WoCustomButtonProperties
    {
        /// <summary>
        /// Identificador único del botón custom.
        /// </summary>
        [Browsable(false)]
        public string ButtonId { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de la etiqueta.
        /// </summary>
        [Category("Nombre y texto")]
        [DisplayName("Etiqueta")]
        [Browsable(true)]
        [ReadOnly(true)]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Texto que se mostrara en el botón.
        /// </summary>
        [Browsable(false)]
        public string MaskText { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del método que ejecuta
        /// </summary>
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// Icono que se visualizara en el botón.
        /// </summary>
        [Category("Diseño")]
        [DisplayName("Icono")]
        [Browsable(true)]
        public eBoostrapIcons Icon { get; set; } = eBoostrapIcons.None;

        /// <summary>
        /// Orden en el que se posicionara.
        /// </summary>
        [Browsable(false)]
        public int Index { get; set; } = 0;
    }
}
