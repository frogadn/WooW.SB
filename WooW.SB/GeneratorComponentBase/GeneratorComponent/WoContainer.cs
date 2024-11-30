using System.Collections.Generic;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.FormDesigner;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.GeneralComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.GeneratorComponentBase.GeneratorComponent
{
    public class WoContainer
        :
        // Interfaces comunes para todos los componentes
        IWoComponentId,
            IWoComponentBehaviour,
            IWoIcono,
            // Interfaces de diseño
            IWoDesignAplied,
            IWoContainerDesign,
            IWoFontDesing,
            // Interfaces propias de los formularios
            IWoFormPosition,
            IWoContainerSettings
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
        public string Theme { get; set; } = "Default";

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
        public string Etiqueta { get; set; } = string.Empty;

        /// <summary>
        /// Funciona como una etiqueta auxiliar para que el componente pueda mostrar contenido con propiedades HTML
        /// en la vista previa de diseño.
        /// </summary>
        public string MaskText { get; set; } = string.Empty;

        #endregion Nombre y texto

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
        /// Comportamiento base.
        /// </summary>
        public eItemEnabled Enable { get; set; } = eItemEnabled.Activo;

        #endregion Comportamiento base

        #region Propiedades de diseño generales

        /// <summary>
        /// Color del contenedor para tabs.
        /// </summary>
        public eContainerItemColor BackgorundColorContainerItem { get; set; } =
            eContainerItemColor.Default;

        /// <summary>
        /// Color del contenedor para grupos.
        /// </summary>
        public eGroupColor BackgorundColorGroup { get; set; } = eGroupColor.Default;

        #endregion Propiedades de diseño generales

        #region Propiedades de diseño de la letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        public eTextColor ComponentFontColor { get; set; } = eTextColor.FontDefault;

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        public eTextSize ComponentFontSize { get; set; } = eTextSize.Normal;

        /// <summary>
        /// Tipo de fuente itálica del componente.
        /// </summary>
        public eTextItalic ComponentFontItalic { get; set; } = eTextItalic.None;

        /// <summary>
        /// Ancho de la letra del componente.
        /// </summary>
        public eTextWeight ComponentFontWide { get; set; } = eTextWeight.Normal;

        /// <summary>
        /// Decorado del texto del componente.
        /// </summary>
        public eTextDecoration ComponentFontDecoration { get; set; } = eTextDecoration.None;

        #endregion Propiedades de diseño de la letra

        #region Diseño de la slave

        /// <summary>
        /// Indica el ancho de un control de tipo slave.
        /// </summary>
        public string WidthSlave { get; set; } = "auto";

        /// <summary>
        /// Indica el alto de un control de tipo slave.
        /// </summary>
        public string HeightSlave { get; set; } = "auto";

        /// <summary>
        /// Indica si es un pooper slave.
        /// </summary>
        public bool IsSlavePooper { get; set; } = false;

        #endregion Diseño de la slave

        #region Configuración de posicionamiento

        /// <summary>
        /// Indica cual es el componente padre del control (dentro de que componente se encuentra).
        /// </summary>
        public string Parent { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el componente inicia en una nueva fila o si en caso de aver espacio continuará en la fila anterior.
        /// </summary>
        public bool BeginRow { get; set; } = false;

        /// <summary>
        /// Indica el tamaño del control en columnas (Ancho).
        /// </summary>
        public int ColSpan { get; set; } = 3;

        /// <summary>
        /// Indica el tamaño del control en filas (Largo).
        /// </summary>
        public int RowSpan { get; set; } = 1;

        /// <summary>
        /// Indica la posición del control, indica desde que columna del contenedor inicia.
        /// </summary>
        public int ColumnIndex { get; set; } = 0;

        /// <summary>
        /// Indica la posición del control, indica desde que fila del contenedor inicia.
        /// </summary>
        public int RowIndex { get; set; } = 0;

        #endregion Configuración de posicionamiento

        #region Configuración de contenedor

        /// <summary>
        /// Indica las columnas internas del contenedor, por defecto siempre serán 12.
        /// </summary>
        public int Col { get; set; } = 12;

        /// <summary>
        /// Indica las filas/renglones internas del contenedor.
        /// </summary>
        public int Row { get; set; } = 0;

        #endregion Configuración de contenedor

        #region Contenido

        /// <summary>
        /// Propiedad General: Lista de items.
        /// FormGoup: Indica la lista de controles que se encuentran dentro del contenedor (text, label, combo,...).
        /// Menu: Almacena la lista de menus / nodos finales
        /// </summary>
        public List<WoItem> ItemsCol { get; set; } = new List<WoItem>();

        /// <summary>
        /// Propiedad General: Lista de grupos internos.
        /// FormGoup: Indica la lista de grupos dentro del grupo (TabControl, GroupControl,...).
        /// Menu: Contiene la lista de Sub menus
        /// </summary>
        public List<WoContainer> ContainersCol { get; set; } = new List<WoContainer>();

        #endregion Contenido

        #region Iconos

        /// <summary>
        /// Icono del contenedor.
        /// </summary>
        public eBoostrapIcons Icon { get; set; } = eBoostrapIcons.None;

        #endregion Iconos

        #region Model

        /// <summary>
        /// Indica el proceso al que pertenece el formulario.
        /// </summary>
        public string Proceso { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del modelo del que se realizo el formulario o del layout custom.
        /// </summary>
        public string ModelId = string.Empty;

        /// <summary>
        /// Indica el tipo de modelo del formulario.
        /// </summary>
        public Core.WoTypeModel ModelType { get; set; } = Core.WoTypeModel.Request;

        /// <summary>
        /// Indica el subtipo del modelo del formulario.
        /// </summary>
        public Core.WoSubTypeModel SubType { get; set; } = Core.WoSubTypeModel.Report;

        /// <summary>
        /// Indicador de si es una prueba unitaria o no y se llena al generar si no por default es false.
        /// </summary>
        public bool IsUnit = false;

        /// <summary>
        /// Indica si el formulario tendrá o no un modelo como referencia.
        /// </summary>
        public bool HaveModelReference = true;

        /// <summary>
        /// Indicador si el diseño es de una extension o si es del modelo principal
        /// </summary>
        public bool IsExtension = false;

        #endregion Model

        #region Grupos

        /// <summary>
        /// Solo para los grupos, indica si es visible la agrupacion o no
        /// </summary>
        public bool Visible { get; set; } = true;

        #endregion Grupos

        #region Mapeo para formulario

        public WoComponentProperties ConvertToComponentProperties()
        {
            return new WoComponentProperties()
            {
                CustomDesignAplied = this.CustomDesignAplied,
                ThemeSuperiorAplied = this.ThemeSuperiorAplied,
                Theme = this.Theme,
                Id = this.Id,
                Etiqueta = this.Etiqueta,
                MaskText = this.MaskText,
                TypeContainer = this.TypeContainer,
                Enable = this.Enable,
                BackgorundColorContainerItem = this.BackgorundColorContainerItem,
                BackgorundColorGroup = this.BackgorundColorGroup,
                ComponentFontColor = this.ComponentFontColor,
                ComponentFontSize = this.ComponentFontSize,
                ComponentFontItalic = this.ComponentFontItalic,
                ComponentFontWide = this.ComponentFontWide,
                ComponentFontDecoration = this.ComponentFontDecoration,
                Parent = this.Parent,
                BeginRow = this.BeginRow,
                ColSpan = this.ColSpan,
                RowSpan = this.RowSpan,
                ColumnIndex = this.ColumnIndex,
                RowIndex = this.RowIndex,
                Col = this.Col,
                Row = this.Row,
                Icon = this.Icon,
                ModelType = this.ModelType,
                SubType = this.SubType,
                Proceso = this.Proceso,
                ModelId = this.ModelId,
                Visible = this.Visible,
                WidthSlave = this.WidthSlave,
                HeightSlave = this.HeightSlave,
                IsSlavePooper = this.IsSlavePooper,
                IsExtension = this.IsExtension,
            };
        }

        public WoContainer GetInstance()
        {
            return new WoContainer()
            {
                CustomDesignAplied = this.CustomDesignAplied,
                ThemeSuperiorAplied = this.ThemeSuperiorAplied,
                Theme = this.Theme,
                Id = this.Id,
                Etiqueta = this.Etiqueta,
                MaskText = this.MaskText,
                TypeContainer = this.TypeContainer,
                Enable = this.Enable,
                BackgorundColorContainerItem = this.BackgorundColorContainerItem,
                BackgorundColorGroup = this.BackgorundColorGroup,
                ComponentFontColor = this.ComponentFontColor,
                ComponentFontSize = this.ComponentFontSize,
                ComponentFontItalic = this.ComponentFontItalic,
                ComponentFontWide = this.ComponentFontWide,
                ComponentFontDecoration = this.ComponentFontDecoration,
                Parent = this.Parent,
                BeginRow = this.BeginRow,
                ColSpan = this.ColSpan,
                RowSpan = this.RowSpan,
                ColumnIndex = this.ColumnIndex,
                RowIndex = this.RowIndex,
                Col = this.Col,
                Row = this.Row,
                Icon = this.Icon,
                Proceso = this.Proceso,
                ModelId = this.ModelId,
                ModelType = this.ModelType,
                SubType = this.SubType,
                Visible = this.Visible,
                WidthSlave = this.WidthSlave,
                HeightSlave = this.HeightSlave,
                IsSlavePooper = this.IsSlavePooper,
                IsExtension = this.IsExtension,
            };
        }

        #endregion Mapeo para formulario
    }
}
