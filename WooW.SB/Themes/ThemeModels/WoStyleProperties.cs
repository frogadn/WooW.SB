using System.ComponentModel;
using System.Drawing;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Themes.ThemeModels
{
    public class WoStyleProperties //: IWoComponentDesign, IWoFontDesing
    {
        [Browsable(false)]
        public string Theme { get; set; } = string.Empty;

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
        /// Color de fondo del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("ColorDeFondo")]
        [Description("ColorDeFondo")]
        [Browsable(true)]
        public Color BackgorundColor { get; set; } = Color.White;

        #endregion Propiedades de diseño generales

        #region Propiedades de diseño de la letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("ColorDeLetra")]
        [Description("Indica el color de la letra del texto")]
        [Browsable(true)]
        public Color FontColor { get; set; } = Color.Black;

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Tamaño")]
        [Description("Indica el tamaño de la letra del componente.")]
        [Browsable(true)]
        public eTamaño FontSize { get; set; } = eTamaño.Mediano;

        /// <summary>
        /// Tipo de fuente itálica.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Cursiva")]
        [Description("Cursiva")]
        [Browsable(true)]
        public bool Italic { get; set; } = false;

        /// <summary>
        /// Tipo de fuente subrayada.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Subrayado")]
        [Description("Subrayado")]
        [Browsable(true)]
        public bool Underline { get; set; } = false;

        /// <summary>
        /// Tipo de fuente tachado.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Tachado")]
        [Description("Tachado")]
        [Browsable(true)]
        public bool Strikeout { get; set; } = false;

        /// <summary>
        /// Tipo de fuente negrita.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Negrita")]
        [Description("Negrita")]
        [Browsable(true)]
        public bool Bold { get; set; } = false;

        #endregion Propiedades de diseño de la letra

        #region Propiedades de diseño del borde

        /// <summary>
        /// Indica si el componente tendrá un borde.
        /// </summary>
        [Category("Propiedades del borde")]
        [DisplayName("Borde")]
        [Description("Borde")]
        [Browsable(true)]
        public eBorderType Border { get; set; } = eBorderType.None;

        /// <summary>
        /// Indica el color del borde del componente.
        /// </summary>
        [Category("Propiedades del borde")]
        [DisplayName("ColorDeBorde")]
        [Description("ColorDeBorde")]
        [Browsable(true)]
        public Color BorderColor { get; set; } = Color.Black;

        #endregion Propiedades de diseño del borde
    }
}
