using System.ComponentModel;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.GeneralComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Menus.MenuModels
{
    public class WoMenuItem // Interfaces generales
        : /*IWoComponentIdEnglish,*/
        IWoComponentBehaviour,
            //IWoIcono,
            // Interfaces de diseño
            IWoDesignAplied
    //Interfaces propias de los menus
    //IWoMenuItem
    {
        #region Temas

        /// <summary>
        /// Indica si se tiene un diseño custom o si se trabajara con el con el diseño del tema aplicado.
        /// </summary>
        public bool CustomDesignAplied { get; set; } = false;

        /// <summary>
        /// Indica si el tema superior se encuentra aplicado y en caso de aplicar uno mas general
        /// este ultimo no modificara este diseño.
        /// </summary>
        public bool ThemeSuperiorAplied { get; set; } = false;

        /// <summary>
        /// Nombre del tema aplicado en caso de tener alguno.
        /// </summary>
        public string Theme { get; set; } = null;

        #endregion Temas

        #region Nombre y texto

        /// <summary>
        /// Nombre, identificador o forma mediante la cual podemos identificar un componente del diseñador, es único.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Etiqueta o texto que se muestra en el componente.
        /// Puede ser uno para varios componentes.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Funciona como una etiqueta auxiliar para que el componente pueda mostrar contenido con propiedades HTML
        /// en la vista previa de diseño.
        /// </summary>
        public string MaskText { get; set; } = string.Empty;

        #endregion Nombre y texto

        #region Posicionamiento
        /// <summary>
        /// Indica cual es el componente padre del control (dentro de que componente se encuentra).
        /// </summary>
        public string Parent { get; set; } = string.Empty;
        #endregion Posicionamiento

        #region Comportamiento base

        /// <summary>
        /// Indica el tipo de contenedor con el que se esta trabajando en caso de ser un contenedor.
        /// - None : Indica que no es un contenedor.
        /// - FormTabGroup
        /// - FormTab
        /// - FormGroup
        /// </summary>
        public eTypeContainer TypeContainer { get; set; } = eTypeContainer.None;

        /// <summary>
        /// Indica el tipo de item con el que se esta trabajando en caso de ser un item.
        /// - None: Indica que no es un item.
        /// - FormItem
        /// - Slave
        /// </summary>
        public eTypeItem TypeItem { get; set; } = eTypeItem.None;

        /// <summary>
        /// Indica si el componente estará habilitado o no de inicio.
        /// </summary>
        public eItemEnabled Enable { get; set; } = eItemEnabled.Activo;

        /// <summary>
        /// Indica si el componente estará habilitado o no de inicio.
        /// </summary>
        [Category("Propiedades del menú")]
        [DisplayName("Login")]
        [Description("Login")]
        [Browsable(true)]
        public eMenuLogin IsLogin { get; set; } = eMenuLogin.SoloConSesionIniciada;

        /// <summary>
        /// Agrega una linea al inicio indicando que el inicio de un grupo de menus.
        /// </summary>
        [Category("Propiedades del menú")]
        [DisplayName("Comienza en grupo")]
        [Description("Comienza en grupo")]
        [Browsable(true)]
        public bool BeginRow { get; set; } = false;

        /// <summary>
        /// Agrega una linea al final indicando que el inicio de un grupo de menus.
        /// </summary>
        [Category("Propiedades del menú")]
        [DisplayName("Finaliza en grupo")]
        [Description("Finaliza en grupo")]
        [Browsable(true)]
        public bool EndingRow { get; set; } = false;

        #endregion Comportamiento base

        #region Propiedades de diseño generales
        //ToDo: Agregar las descripciones

        /// <summary>
        /// Color de fondo del componente.
        /// </summary>
        public eMenuColor BackgroundColor { get; set; } = eMenuColor.Default;

        /// <summary>
        /// Color del menu del componente.
        /// </summary>
        public eBorderColor BorderColor { get; set; } = eBorderColor.Default;

        #endregion Propiedades de diseño generales

        #region Iconos

        /// <summary>
        /// Indica cual es el icono
        /// </summary>
        public string Icon { get; set; }

        #endregion Iconos

        #region Propiedades de diseño de la letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        public eTextColor FontColor { get; set; } = eTextColor.FontDefault;

        ///// <summary>
        ///// Color de fuente del componente.
        ///// </summary>
        //public eTextSize FontSize { get; set; } = eTextSize.Normal;

        /// <summary>
        /// Tipo de fuente itálica.
        /// </summary>
        public eTextItalic Italic { get; set; } = eTextItalic.None;

        /// <summary>
        /// Ancho de la letra.
        /// </summary>
        public eTextWeightMenu Wide { get; set; } = eTextWeightMenu.Normal;

        /// <summary>
        /// Indica si la letra tiene algún decorado como tachado o subrayado.
        /// </summary>
        public eTextDecoration Decoration { get; set; } = eTextDecoration.None;

        #endregion Propiedades de diseño de la letra

        #region Caracteristicas del modelo

        public string Process { get; set; } = string.Empty;

        #endregion Caracteristicas del modelo

        #region Menu item
        public string Reference { get; set; }

        public int Order { get; set; }

        public bool InNewTab { get; set; }

        public bool IsExternalReference { get; set; }

        #endregion Menu item

        #region Propiedades de diseño de la letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Color de la letra del componente")]
        [Description("Color de la letra del componente")]
        [Browsable(true)]
        public eTextColor ComponentFontColor { get; set; } = eTextColor.FontDefault;

        ///// <summary>
        ///// Color de fuente del componente.
        ///// </summary>
        //[Category("Diseño del texto")]
        //[DisplayName("Tamaño de la letra del componente")]
        //[Description("Tamaño de letra del componente")]
        //[Browsable(true)]
        //public eTextSize ComponentFontSize { get; set; } = eTextSize.Normal;

        /// <summary>
        /// Tipo de fuente itálica.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Tipo de letra cursiva del componente")]
        [Description("Tipo de letra cursiva del componente")]
        [Browsable(true)]
        public eTextItalic ComponentFontItalic { get; set; } = eTextItalic.None;

        /// <summary>
        /// Ancho de la letra.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Ancho de la letra del componente")]
        [Description("Ancho de la letra del componente")]
        [Browsable(true)]
        public eTextWeightMenu ComponentFontWide { get; set; } = eTextWeightMenu.Normal;

        /// <summary>
        /// Indica si la letra tiene algún decorado como tachado o subrayado.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Decorado del componente")]
        [Description("Decorado del componente")]
        [Browsable(true)]
        public eTextDecoration ComponentFontDecoration { get; set; } = eTextDecoration.None;

        #endregion Propiedades de diseño de la letra
    }
}
