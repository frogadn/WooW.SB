using System.ComponentModel;
using System.Drawing;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.MenuDesigner.MenuDesignerModels
{
    public class WoMenuContainerStyles //: IWoComponentDesign, IWoFontDesing
    {
        #region Tema
        /// <summary>
        /// Almacena el nombre del tema y permite recuperarlo
        /// </summary>
        [Browsable(false)]
        public string Theme { get; set; } = null;

        #endregion Tema

        #region Propiedades de letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("ColorDeLetra")]
        [Description("ColorDeLetra")]
        [Browsable(true)]
        public Color FontColor { get; set; } = Color.Black;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Cursiva"), Category("Diseño del Menú")]
        [DisplayName("Cursiva")]
        [Browsable(true)]
        public bool Italic { get; set; } = false;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Subrayado"), Category("Diseño del Menú")]
        [DisplayName("Subrayado")]
        [Browsable(true)]
        public bool Underline { get; set; } = false;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Tachado"), Category("Diseño del Menú")]
        [DisplayName("Tachado")]
        [Browsable(true)]
        public bool Strikeout { get; set; } = false;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Negrita"), Category("Diseño del Menú")]
        [DisplayName("Negrita")]
        [Browsable(true)]
        public bool Bold { get; set; } = false;

        #endregion Propiedades de letra

        #region Propiedades de diseño

        /// <summary>
        /// Indica la alineación del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Alineación")]
        [Description("Alineación")]
        [Browsable(true)]
        public eAlineacion Align { get; set; } = eAlineacion.Izquierda;

        /// <summary>
        /// Indica el tamaño del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Tamaño")]
        [Description("Tamaño")]
        [Browsable(true)]
        public eTamaño FontSize { get; set; } = eTamaño.Pequeño;

        /// <summary>
        /// Color de fondo del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("ColorDeFondo")]
        [Description("ColorDeFondo")]
        [Browsable(true)]
        public Color BackgorundColor { get; set; } = Color.White;

        /// <summary>
        /// Indica si el componente tendrá un borde.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Borde")]
        [Description("Borde")]
        [Browsable(true)]
        public eBorderType Border { get; set; } = eBorderType.None;

        /// <summary>
        /// Indica el color del borde del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("ColorDeBorde")]
        [Description("ColorDeBorde")]
        [Browsable(true)]
        public Color BorderColor { get; set; } = Color.White;

        #endregion Propiedades de diseño generales
    }
}
