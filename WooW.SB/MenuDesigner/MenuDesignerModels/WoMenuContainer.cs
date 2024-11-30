using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.MenuDesigner.MenuDesignerModels
{
    public class WoMenuContainer
    //: IWoComponentBehaviour,
    //    IWoComponentDesign,
    //    IWoIcono,
    //    IWoMenuContainer,
    //    IWoDesignAplied
    {
        #region Instancias singleton

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Temas
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
        public string ThemeSubMenu { get; set; } = null;

        /// <summary>
        /// Almacena el nombre del tema y permite recuperarlo
        /// </summary>
        [Browsable(false)]
        public string ThemeItem { get; set; } = null;

        #endregion Temas

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
        [Description("Etiqueta"), Category("Diseño del Menú")]
        [DisplayName("Etiqueta")]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string Label
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
        [Description("Texto"), Category("Diseño del Menú")]
        [DisplayName("Texto")]
        [Browsable(true)]
        public string MaskText { get; set; } = string.Empty;

        #endregion Nombre y texto

        #region Contenido

        /// <summary>
        /// Propiedad General: Lista de items.
        /// FormGoup: Indica la lista de controles que se encuentran dentro del contenedor (text, label, combo,...).
        /// Menu: Almacena la lista de menus / nodos finales
        /// </summary>
        [Browsable(false)]
        public List<WoMenuItem> ItemsCol { get; set; } = new List<WoMenuItem>();

        /// <summary>
        /// Propiedad General: Lista de grupos internos.
        /// FormGoup: Indica la lista de grupos dentro del grupo (TabControl, GroupControl,...).
        /// Menu: Contiene la lista de Sub menus
        /// </summary>
        [Browsable(false)]
        public List<WoMenuContainer> ContainersCol { get; set; } = new List<WoMenuContainer>();

        #endregion Contenido

        #region Comportamiento base

        /// <summary>
        /// Indica el tipo de contenedor con el que se esta trabajando en caso de ser un contenedor.
        /// - None : Indica que no es un contenedor.
        /// - FormTabGroup
        /// - FormTab
        /// - FormGroup
        /// </summary>
        [Browsable(false)]
        public eTypeContainer TypeContainer { get; set; } = eTypeContainer.None;

        /// <summary>
        /// Comportamiento base.
        /// </summary>
        [Description("Menú Activo"), Category("Diseño del Menú")]
        [DisplayName("Menú Activo")]
        [Browsable(true)]
        public bool Enable { get; set; } = true;

        /// <summary>
        /// Indica cual es el componente padre del control (dentro de que componente se encuentra).
        /// </summary>
        [Browsable(false)]
        public string Parent { get; set; } = string.Empty;

        #endregion Comportamiento base

        #region Propiedades de letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Description("Color de fuente"), Category("Diseño del Menú")]
        [DisplayName("Color de fuente")]
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

        #region Propiedades de diseño generales

        /// <summary>
        /// Indica la alineación del componente.
        /// </summary>
        [Description("Alineación"), Category("Diseño del Menú")]
        [DisplayName("Alineación")]
        [Browsable(true)]
        public eAlineacion Align { get; set; } = eAlineacion.Izquierda;

        /// <summary>
        /// Indica el tamaño del componente.
        /// </summary>
        [Description("Tamaño"), Category("Diseño del Menú")]
        [DisplayName("Tamaño")]
        [Browsable(true)]
        public eTamaño FontSize { get; set; } = eTamaño.Pequeño;

        /// <summary>
        /// Color de fondo del componente.
        /// </summary>
        [Description("Color de fondo"), Category("Diseño del Menú")]
        [DisplayName("Color de fondo")]
        [Browsable(true)]
        public Color BackgorundColor { get; set; }

        /// <summary>
        /// Indica si el componente tendrá un borde.
        /// </summary>
        [Description("Tipo de borde"), Category("Diseño del Menú")]
        [DisplayName("Tipo de borde")]
        [Browsable(true)]
        public eBorderType Border { get; set; } = eBorderType.None;

        /// <summary>
        /// Indica el color del borde del componente.
        /// </summary>
        [Description("Color de borde"), Category("Diseño del Menú")]
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

        #region Menu container
        [Browsable(false)]
        public List<string> ProcessList { get; set; }

        [Description("Orientación"), Category("Diseño del Menú")]
        [DisplayName("Orientación")]
        [Browsable(true)]
        public eOrientation Orientation { get; set; }

        [Browsable(false)]
        public int Order { get; set; }

        [Browsable(false)]
        public string Rol { get; set; }

        public override string ToString()
        {
            if (MaskText != string.Empty)
            {
                return MaskText;
            }
            return Id;
        }

        #endregion Menu container

        #region Mapeo para asignar temas

        public void SetTheme(WoMenuContainerStyles woMenuContainerStyle)
        {
            Align = woMenuContainerStyle.Align;
            FontSize = woMenuContainerStyle.FontSize;
            BackgorundColor = woMenuContainerStyle.BackgorundColor;
            FontColor = woMenuContainerStyle.FontColor;
            Italic = woMenuContainerStyle.Italic;
            Underline = woMenuContainerStyle.Underline;
            Strikeout = woMenuContainerStyle.Strikeout;
            Bold = woMenuContainerStyle.Bold;
            Border = woMenuContainerStyle.Border;
            BorderColor = woMenuContainerStyle.BorderColor;
        }

        #endregion Mapeo para asignar temas
    }
}
