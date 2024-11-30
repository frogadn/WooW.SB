using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.MenuDesigner.MenuDesignerModels
{
    public class WoMenuItem
    //: IWoComponentId,
    //    IWoComponentBehaviour,
    //    IWoComponentDesign,
    //    IWoFontDesing,
    //    IWoIcono,
    //    IWoMenuItem,
    //    IWoDesignAplied
    {
        #region Instancias singleton

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Tema
        /// <summary>
        /// Indica si se tiene un diseño custom o si se trabajara con el con el diseño del tema aplicado.
        /// </summary>
        [Category("Diseño custom")]
        [DisplayName("Diseño custom")]
        [Description("Evita que la aplicación de un tema modifique el diseño de este componente")]
        [Browsable(true)]
        public bool CustomDesignAplied { get; set; } = false;

        /// <summary>
        /// Indica si el tema superior se encuentra aplicado y en caso de aplicar uno mas general
        /// este ultimo no modificara este diseño.
        /// </summary>
        [Browsable(false)]
        public bool ThemeSuperiorAplied { get; set; } = false;

        /// <summary>
        /// Almacena el nombre del tema y permite recuperarlo
        /// </summary>
        [Browsable(false)]
        public string Theme { get; set; } = null;

        #endregion Tema

        #region Nombre y texto

        /// <summary>
        /// Nombre, identificador o forma mediante la cual podemos identificar un componente del diseñador, es único.
        /// </summary>
        [Browsable(false)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Etiqueta o texto que se muestra en el componente.
        /// Puede ser uno para varios componentes.
        /// </summary>
        [Description("Etiqueta"), Category("SubMenu")]
        [DisplayName("Etiqueta")]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string Etiqueta
        {
            get { return label; }
            set
            {
                label = value;
                Etiqueta findLabel = _project
                    .EtiquetaCol.Etiquetas.Where(x => x.Id == value)
                    .FirstOrDefault();
                if (findLabel != null)
                {
                    MaskText = findLabel
                        .Idiomas.Where(f => f.IdiomaId == _project.esMX)
                        .FirstOrDefault()
                        .Texto;
                }
            }
        }

        [Browsable(false)]
        private string label = string.Empty;

        /// <summary>
        /// Funciona como una etiqueta auxiliar para que el componente pueda mostrar contenido con propiedades HTML
        /// en la vista previa de diseño.
        /// </summary>
        [Description("Texto"), Category("SubMenu")]
        [DisplayName("Texto")]
        [Browsable(true)]
        public string MaskText { get; set; } = string.Empty;

        #endregion Nombre y texto

        #region Comportamiento base

        /// <summary>
        /// Indica el tipo de item con el que se esta trabajando en caso de ser un item.
        /// - None: Indica que no es un item.
        /// - FormItem
        /// - Slave
        /// </summary>
        [Browsable(false)]
        public eTypeItem TypeItem { get; set; } = eTypeItem.None;

        /// <summary>
        /// Comportamiento base.
        /// </summary>
        [Description("Menú Activo"), Category("SubMenu")]
        [DisplayName("Menú Activo")]
        [Browsable(true)]
        public bool Enable { get; set; } = true;

        /// <summary>
        /// Indica cual es el componente padre del control (dentro de que componente se encuentra).
        /// </summary>
        [Browsable(false)]
        public string Parent { get; set; } = string.Empty;

        #endregion Comportamiento base

        #region Propiedades de diseño generales

        /// <summary>
        /// Indica la alineación del componente.
        /// </summary>
        [Description("Alineación"), Category("Diseño de la Página")]
        [DisplayName("Alineación")]
        [Browsable(true)]
        public eAlineacion Align { get; set; } = eAlineacion.Izquierda;

        /// <summary>
        /// Indica el tamaño del componente.
        /// </summary>
        [Description("Tamaño"), Category("Diseño de la Página")]
        [DisplayName("Tamaño")]
        [Browsable(true)]
        public eTamaño FontSize { get; set; } = eTamaño.Pequeño;

        /// <summary>
        /// Color de fondo del componente.
        /// </summary>
        [Description("Color de fondo"), Category("Diseño de la Página")]
        [DisplayName("Color de fondo")]
        [Browsable(true)]
        public Color BackgorundColor { get; set; } = Color.White;

        /// <summary>
        /// Indica si el componente tendrá un borde.
        /// </summary>
        [Description("Tipo de borde"), Category("Diseño de la Página")]
        [DisplayName("Tipo de borde")]
        [Browsable(true)]
        public eBorderType Border { get; set; } = eBorderType.None;

        /// <summary>
        /// Indica el color del borde del componente.
        /// </summary>
        [Description("Color de borde"), Category("Diseño de la Página")]
        [DisplayName("Color de borde")]
        [Browsable(true)]
        public Color BorderColor { get; set; } = Color.White;

        #endregion Propiedades de diseño generales

        #region Iconos

        [Description("Icono Visible"), Category("Icono")]
        [DisplayName("Icono Visible")]
        [Browsable(true)]
        public bool IconoActivo { get; set; } = false;

        [Description("Icono Seleccionado"), Category("Icono")]
        [DisplayName("Icono Seleccionado")]
        [Browsable(false)]
        public string IconoSelected { get; set; } = "Icono 0";

        [Description("Estilo del Icono"), Category("Icono")]
        [DisplayName("Estilo del Icono")]
        [Browsable(false)]
        public eEstiloIcono EstiloIcono { get; set; } = eEstiloIcono.Etiqueta_Icono;

        [Description("Posición del Icono"), Category("Icono")]
        [DisplayName("Posición del Icono")]
        [Browsable(false)]
        public ePosicionImagen PositionIcon { get; set; } = ePosicionImagen.Derecha;

        #endregion Iconos

        #region Propiedades de letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Description("Color de fuente"), Category("Diseño de la Página")]
        [DisplayName("Color de fuente")]
        [Browsable(true)]
        public Color FontColor { get; set; } = Color.Black;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Cursiva"), Category("Diseño de la Página")]
        [DisplayName("Cursiva")]
        [Browsable(true)]
        public bool Italic { get; set; } = false;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Subrayado"), Category("Diseño de la Página")]
        [DisplayName("Subrayado")]
        [Browsable(true)]
        public bool Underline { get; set; } = false;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Tachado"), Category("Diseño de la Página")]
        [DisplayName("Tachado")]
        [Browsable(true)]
        public bool Strikeout { get; set; } = false;

        /// <summary>
        /// Tipo de fuente del componente.
        /// </summary>
        [Description("Negrita"), Category("Diseño de la Página")]
        [DisplayName("Negrita")]
        [Browsable(true)]
        public bool Bold { get; set; } = false;

        #endregion Propiedades de letra

        #region Menu item
        [Description("Referencia"), Category("Página")]
        [DisplayName("Referencia")]
        [Browsable(true)]
        public string Referencia { get; set; }

        [Browsable(false)]
        public int Orden { get; set; }

        [Description("Abrir en nueva Tab"), Category("Página")]
        [DisplayName("Abrir en nueva Tab")]
        [Browsable(true)]
        public bool InNewTab { get; set; }

        [Description("Referencia Externa"), Category("Página")]
        [DisplayName("Referencia Externa")]
        [Browsable(true)]
        public bool IsExternalReference { get; set; }

        public override string ToString()
        {
            if (MaskText != string.Empty)
            {
                return MaskText;
            }
            return Id;
        }

        #endregion Menu item

        #region Mapeo para asignar temas

        public void SetThemeItems(WoMenuItemStyles woMenuItemsStyle)
        {
            Align = woMenuItemsStyle.Align;
            FontSize = woMenuItemsStyle.FontSize;
            BackgorundColor = woMenuItemsStyle.BackgorundColor;
            FontColor = woMenuItemsStyle.FontColor;
            Italic = woMenuItemsStyle.Italic;
            Underline = woMenuItemsStyle.Underline;
            Strikeout = woMenuItemsStyle.Strikeout;
            Bold = woMenuItemsStyle.Bold;
            Border = woMenuItemsStyle.Border;
            BorderColor = woMenuItemsStyle.BorderColor;
        }
        #endregion Mapeo para asignar temas
    }
}
