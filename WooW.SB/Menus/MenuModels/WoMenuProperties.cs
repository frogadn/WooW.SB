using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using WooW.SB.Config;
using WooW.SB.Config.Editors;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.GeneralComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Menus.MenuModels
{
    public class WoMenuProperties
        // Interfaces generales
        : /*IWoComponentIdEnglish,*/
        IWoComponentBehaviour,
            // Interfaces de diseño
            IWoDesignAplied
    //Interfaces propias de los menus
    //IWoMenuContainer,
    //IWoMenuItem
    {
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
        /// Nombre del tema aplicado en caso de tener alguno.
        /// </summary>
        [Browsable(false)]
        public string Theme { get; set; } = "Default";

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
        [Category("Nombre y texto")]
        [DisplayName("Etiqueta")]
        [Description("Id de la etiqueta que se visualiza en el editor.")]
        [Browsable(true)]
        [ReadOnly(true)]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Funciona como una etiqueta auxiliar para que el componente pueda mostrar contenido con propiedades HTML
        /// en la vista previa de diseño.
        /// </summary>
        [Browsable(false)]
        public string MaskText { get; set; } = string.Empty;

        #endregion Nombre y texto

        #region Posicionamiento
        /// <summary>
        /// Indica cual es el componente padre del control (dentro de que componente se encuentra).
        /// </summary>
        [Browsable(false)]
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
        [Browsable(false)]
        public eTypeContainer TypeContainer { get; set; } = eTypeContainer.None;

        /// <summary>
        /// Indica el tipo de item con el que se esta trabajando en caso de ser un item.
        /// - None: Indica que no es un item.
        /// - FormItem
        /// - Slave
        /// - MenuItem
        /// </summary>
        [Browsable(false)]
        public eTypeItem TypeItem { get; set; } = eTypeItem.None;

        /// <summary>
        /// Indica si el componente estará habilitado o no de inicio.
        /// </summary>
        [Category("Comportamiento")]
        [DisplayName("Habilitado")]
        [Description("Indica si el componente se encontrara habilitado o no")]
        [Browsable(true)]
        public eItemEnabled Enable { get; set; } = eItemEnabled.Activo;

        /// <summary>
        /// Indica si el componente estará habilitado o no de inicio.
        /// </summary>
        [Category("Comportamiento")]
        [DisplayName("Visible")]
        [Description("Visible")]
        [Browsable(true)]
        public eMenuLogin IsLogin { get; set; } = eMenuLogin.SoloConSesionIniciada;

        /// <summary>
        /// Indica si el menu se despliega cuando el cursor pasa sobre el.
        /// </summary>
        [Description("Desplegable"), Category("Comportamiento")]
        [DisplayName("Desplegable")]
        [Browsable(true)]
        public eDropDownMenu DropDown { get; set; } = eDropDownMenu.Click;

        /// <summary>
        /// Agrega una linea al inicio indicando que el inicio de un grupo de menus.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Comienza en grupo")]
        [Description("Comienza en grupo")]
        [Browsable(true)]
        public bool BeginRow { get; set; } = false;

        /// <summary>
        /// Agrega una linea al final indicando que el inicio de un grupo de menus.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Finaliza en grupo")]
        [Description("Finaliza en grupo")]
        [Browsable(true)]
        public bool EndingRow { get; set; } = false;

        #endregion Comportamiento base

        #region Propiedades de diseño generales
        //ToDo: Agregar las descripciones

        /// <summary>
        /// Color de fondo del menu.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Color de fondo")]
        [Description("Color del fondo")]
        [Browsable(true)]
        public eMenuColor BackgroundColor { get; set; } = eMenuColor.Default;

        /// <summary>
        /// Color del menu del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Color del borde")]
        [Description("Color del borde")]
        [Browsable(true)]
        public eBorderColor BorderColor { get; set; } = eBorderColor.Default;

        #endregion Propiedades de diseño generales

        #region Iconos

        /// <summary>
        /// Indica cual es el icono
        /// </summary>
        [Category("Icono")]
        [DisplayName("Icono")]
        [Description("Icono seleccionado")]
        [EditorAttribute(typeof(ImageComboBoxEditorCreator), typeof(UITypeEditor))]
        [Browsable(true)]
        [ReadOnly(true)]
        public string Icon { get; set; }

        #endregion Iconos

        #region Propiedades de diseño de la letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Color de la letra")]
        [Description("Color de la letra")]
        [Browsable(true)]
        public eTextColor FontColor { get; set; } = eTextColor.FontDefault;

        /// <summary>
        /// Tipo de fuente itálica.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Cursiva")]
        [Description("Cursiva")]
        [Browsable(true)]
        public eTextItalic Italic { get; set; } = eTextItalic.None;

        /// <summary>
        /// Ancho de la letra.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Ancho de la letra")]
        [Description("Ancho de la letra")]
        [Browsable(true)]
        public eTextWeightMenu Wide { get; set; } = eTextWeightMenu.Normal;

        /// <summary>
        /// Indica si la letra tiene algún decorado como tachado o subrayado.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Decorado")]
        [Description("Decorado")]
        [Browsable(true)]
        public eTextDecoration Decoration { get; set; } = eTextDecoration.None;

        #endregion Propiedades de diseño de la letra

        #region Menu container
        [Browsable(false)]
        public List<string> ProcessList { get; set; }

        [Description("Orientación"), Category("Diseño del Menú")]
        [DisplayName("Orientación")]
        [Browsable(false)]
        public eOrientation Orientation { get; set; }

        [Browsable(false)]
        public int Order { get; set; }

        [Description("Roles"), Category("Propiedades del menú")]
        [DisplayName("Roles")]
        [Browsable(false)]
        public List<Rol> Roles { get; set; } = new List<Rol>();

        /// <summary>
        /// Propiedad general que indica el contenido interno del container
        /// </summary>
        [Browsable(false)]
        public List<WoMenuProperties> ContentCol { get; set; } = new List<WoMenuProperties>();

        #endregion Menu container

        #region Menu Root
        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Propiedades del menú")]
        [DisplayName("Tamaño")]
        [Description("Tamaño")]
        [Browsable(true)]
        public eMenuSize MenuSize { get; set; } = eMenuSize.Normal;

        /// <summary>
        /// Color de fondo para los root menu
        /// </summary>
        [Description("Color de fondo"), Category("Propiedades del menú")]
        [DisplayName("Color de fondo")]
        [Browsable(true)]
        public eMenuColor BackgroundRoot { get; set; }

        /// <summary>
        /// Color de borde para los root menu
        /// </summary>
        [Description("Color de borde"), Category("Propiedades del menú")]
        [DisplayName("Color de borde")]
        [Browsable(true)]
        public eBorderColor BorderColorRoot { get; set; }

        #endregion Menu Root

        #region Menu item
        [Description("Referencia"), Category("Propiedades de la página")]
        [DisplayName("Referencia")]
        [Browsable(true)]
        public string Reference { get; set; }

        [Description("Abrir en nueva Tab"), Category("Propiedades de la página")]
        [DisplayName("Abrir en nueva Tab")]
        [Browsable(true)]
        public bool InNewTab { get; set; }

        [Description("Referencia Externa"), Category("Propiedades de la página")]
        [DisplayName("Referencia Externa")]
        [ReadOnly(true)]
        [Browsable(true)]
        public bool IsExternalReference { get; set; }

        #endregion Menu item

        #region Caracteristicas del modelo

        [Browsable(false)]
        public string Process { get; set; } = string.Empty;

        #endregion Caracteristicas del modelo

        #region Para el diseñador

        /// <summary>
        /// Indica si el modelo existe o no.
        /// </summary>
        [Browsable(false)]
        public bool NotFound = false;

        /// <summary>
        /// Indica que el control se encuentra seleccionado.
        /// </summary>
        [Browsable(false)]
        public bool Selected = false;

        /// <summary>
        /// Indica si el nodo en el diseñador se encuentra cerrado o abierto
        /// principalmente para la persistencia de este tipo de estado a la hora de estar diseñado.
        /// </summary>
        [Browsable(false)]
        public bool ExpandedNode = false;

        /// <summary>
        /// Indica si el nodo se encuentra agregado al árbol de diseño.
        /// </summary>
        [Browsable(false)]
        public bool Added = false;

        #endregion Para el diseñador

        public WoMenuContainer ConvertToWoMenuContainer()
        {
            return new WoMenuContainer()
            {
                CustomDesignAplied = this.CustomDesignAplied,
                ThemeSuperiorAplied = this.ThemeSuperiorAplied,
                Theme = this.Theme,
                Id = this.Id,
                Label = this.Label,
                MaskText = this.MaskText,
                TypeContainer = this.TypeContainer,
                Enable = this.Enable,
                BackgroundColor = this.BackgroundColor,
                DropDown = this.DropDown,
                BackgroundRoot = this.BackgroundRoot,
                BorderColorRoot = this.BorderColorRoot,
                BorderColor = this.BorderColor,
                FontColor = this.FontColor,
                MenuSize = this.MenuSize,
                Italic = this.Italic,
                Wide = this.Wide,
                Decoration = this.Decoration,
                Parent = this.Parent,
                Icon = this.Icon,
                Orientation = this.Orientation,
                Order = this.Order,
                Roles = this.Roles,
                IsLogin = this.IsLogin,
                ProcessList = this.ProcessList,
                ExpandedNode = this.ExpandedNode,
                BeginRow = this.BeginRow,
                EndingRow = this.EndingRow,
                NotFound = this.NotFound
            };
        }

        public WoMenuItem ConvertToWoMenuItem()
        {
            return new WoMenuItem()
            {
                CustomDesignAplied = this.CustomDesignAplied,
                ThemeSuperiorAplied = this.ThemeSuperiorAplied,
                Theme = this.Theme,
                Id = this.Id,
                Label = this.Label,
                MaskText = this.MaskText,
                TypeItem = this.TypeItem,
                Enable = this.Enable,
                BackgroundColor = this.BackgroundColor,
                BorderColor = this.BorderColor,
                FontColor = this.FontColor,
                Italic = this.Italic,
                Wide = this.Wide,
                Decoration = this.Decoration,
                Parent = this.Parent,
                Icon = this.Icon,
                IsLogin = this.IsLogin,
                Order = this.Order,
                InNewTab = this.InNewTab,
                Reference = this.Reference,
                IsExternalReference = this.IsExternalReference,
                Process = this.Process,
                BeginRow = this.BeginRow,
                EndingRow = this.EndingRow
            };
        }

        public WoMenuProperties GetCopy()
        {
            return new WoMenuProperties()
            {
                Orientation = this.Orientation,
                Order = this.Order,
                Roles = this.Roles,
                ContentCol = this.ContentCol,
                Reference = this.Reference,
                FontColor = this.FontColor,
                MenuSize = this.MenuSize,
                Italic = this.Italic,
                Wide = this.Wide,
                Decoration = this.Decoration,
                Icon = this.Icon,
                BackgroundColor = this.BackgroundColor,
                DropDown = this.DropDown,
                BorderColor = this.BorderColor,
                TypeContainer = this.TypeContainer,
                TypeItem = this.TypeItem,
                Enable = this.Enable,
                Parent = this.Parent,
                Id = this.Id,
                IsLogin = this.IsLogin,
                Label = this.Label,
                MaskText = this.MaskText,
                Theme = this.Theme,
                ThemeSuperiorAplied = this.ThemeSuperiorAplied,
                CustomDesignAplied = this.CustomDesignAplied,
                ProcessList = this.ProcessList,
                ExpandedNode = this.ExpandedNode,
                BeginRow = this.BeginRow,
                EndingRow = this.EndingRow,
                BackgroundRoot = this.BackgroundRoot,
                BorderColorRoot = this.BorderColorRoot,
                NotFound = this.NotFound,
            };
        }
    }
}
