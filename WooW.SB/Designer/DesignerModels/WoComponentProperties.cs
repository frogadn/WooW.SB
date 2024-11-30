using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.FormDesigner;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.GeneralComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer.DesignerModels
{
    public class WoComponentProperties
        // Interfaces comunes para todos los componentes
        : IWoComponentId,
            IWoComponentBehaviour,
            IWoIcono,
            // Interfaces de diseño
            IWoDesignAplied,
            IWoContainerDesign,
            IWoFontDesing,
            IWoCaptionDesing,
            // Interfaces propias de los formularios
            IWoFormPosition,
            // Interfaces propias de los items
            IWoItemType,
            IWoItemDesign,
            // Interfaces propias de los contenedores
            IWoContainerSettings
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
        public string Theme { get; set; } = null;

        #endregion Temas

        #region Nombre y texto

        /// <summary>
        /// Nombre, identificador o forma mediante la cual podemos identificar un componente del diseñador, es único.
        /// </summary>
        [Category("Identificador")]
        [DisplayName("Id")]
        [Description("Id único que permite reconocer el componente.")]
        [Browsable(false)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Etiqueta o texto que se muestra en el componente.
        /// Puede ser uno para varios componentes.
        /// </summary>
        [Category("Nombre y texto")]
        [DisplayName("Etiqueta")]
        [Description("Id de la etiqueta que permite recuperar la etiqueta.")]
        [Browsable(true)]
        [ReadOnly(true)]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string Etiqueta { get; set; } = string.Empty;

        /// <summary>
        /// Funciona como una etiqueta auxiliar para que el componente pueda mostrar contenido con propiedades HTML
        /// en la vista previa de diseño.
        /// </summary>
        [Browsable(false)]
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
        [Browsable(false)]
        public eTypeContainer TypeContainer { get; set; } = eTypeContainer.None;

        /// <summary>
        /// Indica el tipo de item con el que se esta trabajando en caso de ser un item.
        /// - None: Indica que no es un item.
        /// - FormItem
        /// - Slave
        /// </summary>
        [Browsable(false)]
        public eTypeItem TypeItem { get; set; } = eTypeItem.None;

        /// <summary>
        /// Indicador si el componente que se generar contendra un WoFormIntem encaosulandolo.
        /// (Si no se quiere usar un WoForm, ponerlo en false)
        /// </summary>
        [Browsable(false)]
        public bool AddFormItem { get; set; } = true;

        /// <summary>
        /// Indica si el componente estará habilitado o no de inicio.
        /// </summary>
        [Category("Comportamiento")]
        [DisplayName("Enable")]
        [Description("Indica si el componente se encontrara habilitado o no")]
        [Browsable(true)]
        public eItemEnabled Enable { get; set; } = eItemEnabled.Activo;

        /// <summary>
        /// Indica el proceso al que pertenece el formulario.
        /// </summary>
        [Browsable(false)]
        public string Proceso { get; set; } = string.Empty;

        #endregion Comportamiento base

        #region Propiedades de diseño generales
        //ToDo: Agregar las descripciones

        /// <summary>
        /// Color de fondo del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Color de fondo")]
        [Description("ColorDeFondo")]
        [Browsable(true)]
        public eContainerItemColor BackgorundColorContainerItem { get; set; } =
            eContainerItemColor.Default;

        [Category("Propiedades de diseño")]
        [DisplayName("Color de fondo del grupo")]
        [Description("ColorDeFondo")]
        [Browsable(true)]
        public eGroupColor BackgorundColorGroup { get; set; } = eGroupColor.Default;

        /// <summary>
        /// Tamaño del componente.
        /// </summary>
        [Category("Propiedades de diseño")]
        [DisplayName("Tamaño del control")]
        [Description("Indica el tamaño del control")]
        [Browsable(true)]
        public eItemSize ItemSize { get; set; } = eItemSize.Normal;

        #endregion Propiedades de diseño generales

        #region Propiedades de diseño de la letra

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Color de la letra del componente")]
        [Description("Color de la letra del componente")]
        [Browsable(true)]
        public eTextColor ComponentFontColor { get; set; } = eTextColor.FontDefault;

        /// <summary>
        /// Color de fuente del componente.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Tamaño de la letra del componente")]
        [Description("Tamaño de letra del componente")]
        [Browsable(true)]
        public eTextSize ComponentFontSize { get; set; } = eTextSize.Normal;

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
        public eTextWeight ComponentFontWide { get; set; } = eTextWeight.Normal;

        /// <summary>
        /// Indica si la letra tiene algún decorado como tachado o subrayado.
        /// </summary>
        [Category("Diseño del texto")]
        [DisplayName("Decorado del componente")]
        [Description("Decorado del componente")]
        [Browsable(true)]
        public eTextDecoration ComponentFontDecoration { get; set; } = eTextDecoration.None;

        #endregion Propiedades de diseño de la letra

        #region Propiedades de diseño del caption

        /// <summary>
        /// Color de fuente del caption.
        /// </summary>
        [Category("Diseño del caption")]
        [DisplayName("Color del caption")]
        [Description("Color del caption")]
        [Browsable(true)]
        public eTextColor CaptionColor { get; set; } = eTextColor.FontDefault;

        /// <summary>
        /// Tipo de fuente itálica del caption.
        /// </summary>
        [Category("Diseño del caption")]
        [DisplayName("Italica del caption")]
        [Description("Italica del caption")]
        [Browsable(true)]
        public eTextItalic CaptionItalic { get; set; } = eTextItalic.None;

        /// <summary>
        /// Ancho de la letra del caption.
        /// </summary>
        [Category("Diseño del caption")]
        [DisplayName("Ancho del caption")]
        [Description("Ancho del caption")]
        [Browsable(true)]
        public eTextWeight CaptionWide { get; set; } = eTextWeight.Normal;

        /// <summary>
        /// Decorado del texto del caption.
        /// </summary>
        [Category("Diseño del caption")]
        [DisplayName("Decorado del caption")]
        [Description("Decorado del caption")]
        [Browsable(true)]
        public eTextDecoration CaptionDecoration { get; set; } = eTextDecoration.None;

        #endregion Propiedades de diseño del caption

        #region Configuración de posicionamiento

        /// <summary>
        /// Indica cual es el componente padre del control (dentro de que componente se encuentra).
        /// </summary>
        [Browsable(false)]
        public string Parent { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el componente inicia en una nueva fila o si en caso de aver espacio continuará en la fila anterior.
        /// </summary>
        [Browsable(false)]
        public bool BeginRow { get; set; } = false;

        /// <summary>
        /// Indica el tamaño del control en columnas (Ancho).
        /// </summary>
        [Browsable(false)]
        public int ColSpan { get; set; } = 3;

        /// <summary>
        /// Indica el tamaño del control en filas (Largo).
        /// </summary>
        [Browsable(false)]
        public int RowSpan { get; set; } = 1;

        /// <summary>
        /// Indica la posición del control, indica desde que columna del contenedor inicia.
        /// </summary>
        [Browsable(false)]
        public int ColumnIndex { get; set; } = 0;

        /// <summary>
        /// Indica la posición del control, indica desde que fila del contenedor inicia.
        /// </summary>
        [Browsable(false)]
        public int RowIndex { get; set; } = 0;

        #endregion Configuración de posicionamiento

        #region Diseño de la slave

        /// <summary>
        /// Indica el ancho de un control de tipo slave.
        /// </summary>
        [Category("Tamaño de la slave")]
        [DisplayName("Ancho")]
        [Browsable(true)]
        public string WidthSlave { get; set; } = "auto";

        /// <summary>
        /// Indica el alto de un control de tipo slave.
        /// </summary>
        [Category("Tamaño de la slave")]
        [DisplayName("Alto")]
        [Browsable(true)]
        public string HeightSlave { get; set; } = "auto";

        /// <summary>
        /// Indica si es un pooper slave.
        /// </summary>
        [Browsable(false)]
        public bool IsSlavePooper { get; set; } = false;

        #endregion Diseño de la slave

        #region Tipo

        /// <summary>
        /// En caso de ser una esclava, un look up o algún control custom se indica cual es el modelo de la referencia.
        /// </summary>
        [Browsable(false)]
        public string ClassModelType { get; set; } = string.Empty;

        /// <summary>
        /// Indicador en caso de que la referencia se duplique se indica cual es el numero de referencia repetida.
        /// </summary>
        [Browsable(false)]
        public int MultipleReference { get; set; } = 0;

        /// <summary>
        /// Indica el tipo de la variable a la que estará bandeado el campo.
        /// </summary>
        [Browsable(false)]
        public string BindingType { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la variable bindeada al control prodra ser nula.
        /// </summary>
        [Browsable(false)]
        public bool Nullable { get; set; } = false;

        /// <summary>
        /// Indica el tipo de control que se generara en blazor.
        /// </summary>
        [Browsable(false)]
        public string Control { get; set; } = string.Empty;

        #endregion Tipo

        #region Configuración de contenedor

        /// <summary>
        /// Indica las columnas internas del contenedor, por defecto siempre serán 12.
        /// </summary>
        [Browsable(false)]
        public int Col { get; set; } = 12;

        /// <summary>
        /// Indica las filas/renglones internas del contenedor.
        /// </summary>
        [Browsable(false)]
        public int Row { get; set; } = 0;

        #endregion Configuración de contenedor

        #region Iconos

        /// <summary>
        /// Indica cual es el icono
        /// </summary>
        [Category("Icono")]
        [DisplayName("Icono")]
        [Description("Icono seleccionado")]
        [Browsable(true)]
        public eBoostrapIcons Icon { get; set; } = eBoostrapIcons.None;

        #endregion Iconos

        #region Contenido

        /// <summary>
        /// Propiedad General: Lista de items.
        /// FormGoup: Indica la lista de controles que se encuentran dentro del contenedor (text, label, combo,...).
        /// Menu: Almacena la lista de menus / nodos finales
        /// </summary>
        [Browsable(false)]
        public List<WoItem> ItemsCol { get; set; } = new List<WoItem>();

        /// <summary>
        /// Propiedad General: Lista de grupos internos.
        /// FormGoup: Indica la lista de grupos dentro del grupo (TabControl, GroupControl,...).
        /// Menu: Contiene la lista de Sub menus
        /// </summary>
        [Browsable(false)]
        public List<WoContainer> ContainersCol { get; set; } = new List<WoContainer>();

        #endregion Contenido

        #region Modelo

        /// <summary>
        /// Indica el nombre de la propiedad que se encuentra bindeada al control.
        /// </summary>
        [Browsable(false)]
        public string BindedProperty = string.Empty;

        /// <summary>
        /// Identificador del modelo de la slave en caso de ser una esclava.
        /// </summary>
        [Browsable(false)]
        public string SlaveModelId = string.Empty;

        /// <summary>
        /// Identificador del modelo.
        /// </summary>
        [Browsable(false)]
        public string ModelId = string.Empty;

        /// <summary>
        /// Indica el tipo del formulario
        /// </summary>
        [Browsable(false)]
        public Core.WoTypeModel ModelType { get; set; } = Core.WoTypeModel.Request;

        /// <summary>
        /// Indica el subtipo del modelo del formulario.
        /// </summary>
        [Browsable(false)]
        public Core.WoSubTypeModel SubType { get; set; } = Core.WoSubTypeModel.Report;

        /// <summary>
        /// Indicador si el diseño es de una extension o si es del modelo principal
        /// </summary>
        public bool IsExtension = false;

        #endregion Modelo

        #region Para el diseñador

        /// <summary>
        /// Indica que el control se encuentra seleccionado.
        /// </summary>
        [Browsable(false)]
        public bool Selected = false;

        /// <summary>
        /// Indica el nombre de la propiedad que se modifico.
        /// Solo para la modificación de propiedades en el diseñador.
        /// </summary>
        [Browsable(false)]
        public string ChangedProperty = string.Empty;

        /// <summary>
        /// Valor anterior previo al cambio de la propiedad.
        /// Solo para la modificación de propiedades en el diseñador.
        /// Principalmente para recuperar valores como el id anterior y poder actualizar el nombre en cascada.
        /// </summary>
        [Browsable(false)]
        public object OldValue = null;

        [Browsable(false)]
        public bool ComponenteExtra = false;

        #endregion Para el diseñador

        #region Abstract components

        /// <summary>
        /// En caso de ser un objeto que representa un formulario del diseñador de formularios
        /// </summary>
        [Browsable(false)]
        public string FormName = string.Empty;

        #endregion Abstract components

        #region Simple TextComponents

        /// <summary>
        /// Texto que funge de placeholder para los tipos de texto.
        /// </summary>
        [Category("Diseño control de texto")]
        [DisplayName("PlaceHolder")]
        [Browsable(true)]
        [ReadOnly(true)]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string PlaceHolder { get; set; } = string.Empty;

        /// <summary>
        /// Indica si es o no un caption de tipo password.
        /// </summary>
        [Category("Diseño control de texto")]
        [DisplayName("Contraseña")]
        [Browsable(true)]
        public bool Password { get; set; } = false;

        #endregion Simple TextComponents

        #region Lookups

        /// <summary>
        /// Indica el tamaño del lookup del input.
        /// </summary>
        [Category("Diseño control del input")]
        [DisplayName("Tamaño del control")]
        [Browsable(true)]
        public eLookupInputWidth LookUpInputSize { get; set; } = eLookupInputWidth.Medium;

        #endregion Lookups

        #region Grupos

        /// <summary>
        /// Solo para los grupos, indica si es visible la agrupación o no
        /// </summary>
        [Category("Diseño del grupo")]
        [DisplayName("Grupo Visible")]
        [Browsable(false)]
        public bool Visible { get; set; } = true;

        #endregion Grupos

        #region Spin

        /// <summary>
        /// Indica el tamaño del lookup del input.
        /// </summary>
        [Category("Rango")]
        [DisplayName("Valor Maximo")]
        [Browsable(true)]
        public long Max { get; set; } = 999;

        /// <summary>
        /// Indica el tamaño del lookup del input.
        /// </summary>
        [Category("Rango")]
        [DisplayName("Valor Mínimo")]
        [Browsable(true)]
        public long Min { get; set; } = 0;

        /// <summary>
        /// Indica el tamaño del lookup del input.
        /// </summary>
        [Category("Rango")]
        [DisplayName("Incremento")]
        [Browsable(true)]
        public long Step { get; set; } = 1;

        #endregion Spin

        #region MaskText

        /// <summary>
        /// Mascara custom en el mask
        /// </summary>
        [Category("Mascara")]
        [DisplayName("Mascara custom")]
        [Browsable(true)]
        public string CustomMask { get; set; } = string.Empty;

        /// <summary>
        /// Mascara custom en el mask
        /// </summary>
        [Category("Mascara")]
        [DisplayName("Mascara")]
        [Browsable(true)]
        public eInputString InputString { get; set; } = eInputString.None;

        /// <summary>
        /// Mascara custom en el mask
        /// </summary>
        [Category("Mascara")]
        [DisplayName("Mascara")]
        [Browsable(false)]
        public eInputNumeric InputNumeric { get; set; } = eInputNumeric.None;

        /// <summary>
        /// Mascara custom en el mask
        /// </summary>
        [Category("Mascara")]
        [DisplayName("Mascara")]
        [Browsable(false)]
        public eInputDate InputDate { get; set; } = eInputDate.None;

        #endregion MaskText


        #region Clonación

        public WoComponentProperties GetCopyInstance()
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
                TypeItem = this.TypeItem,
                AddFormItem = this.AddFormItem,
                Enable = this.Enable,
                ModelType = this.ModelType,
                SubType = this.SubType,
                Proceso = this.Proceso,
                BackgorundColorContainerItem = this.BackgorundColorContainerItem,
                BackgorundColorGroup = this.BackgorundColorGroup,
                ItemSize = this.ItemSize,
                ComponentFontColor = this.ComponentFontColor,
                ComponentFontSize = this.ComponentFontSize,
                ComponentFontItalic = this.ComponentFontItalic,
                ComponentFontWide = this.ComponentFontWide,
                ComponentFontDecoration = this.ComponentFontDecoration,
                CaptionColor = this.CaptionColor,
                CaptionItalic = this.CaptionItalic,
                CaptionWide = this.CaptionWide,
                CaptionDecoration = this.CaptionDecoration,
                Parent = this.Parent,
                BeginRow = this.BeginRow,
                ColSpan = this.ColSpan,
                RowSpan = this.RowSpan,
                ColumnIndex = this.ColumnIndex,
                RowIndex = this.RowIndex,
                ClassModelType = this.ClassModelType,
                BindingType = this.BindingType,
                Nullable = this.Nullable,
                Control = this.Control,
                Col = this.Col,
                Row = this.Row,
                Icon = this.Icon,
                ItemsCol = this.ItemsCol,
                ContainersCol = this.ContainersCol,
                BindedProperty = this.BindedProperty,
                SlaveModelId = this.SlaveModelId,
                ModelId = this.ModelId,
                Selected = this.Selected,
                ChangedProperty = this.ChangedProperty,
                OldValue = this.OldValue,
                FormName = this.FormName,
                ComponenteExtra = this.ComponenteExtra,
                Visible = this.Visible,
                WidthSlave = this.WidthSlave,
                HeightSlave = this.HeightSlave,
                IsSlavePooper = this.IsSlavePooper,
                PlaceHolder = this.PlaceHolder,
                Password = this.Password,
                MultipleReference = this.MultipleReference,
                LookUpInputSize = this.LookUpInputSize,
                Max = this.Max,
                Min = this.Min,
                Step = this.Step,
                CustomMask = this.CustomMask,
                InputString = this.InputString,
                InputDate = this.InputDate,
                InputNumeric = this.InputNumeric,
                IsExtension = this.IsExtension,
            };
        }

        #endregion Clonación

        #region Mapeo para component base

        public WoContainer ConvertToWoContainer()
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

        public WoItem ConvertToWoItem()
        {
            return new WoItem()
            {
                CustomDesignAplied = this.CustomDesignAplied,
                ThemeSuperiorAplied = this.ThemeSuperiorAplied,
                Theme = this.Theme,
                Id = this.Id,
                Etiqueta = this.Etiqueta,
                MaskText = this.MaskText,
                TypeItem = this.TypeItem,
                AddFormItem = this.AddFormItem,
                Enable = this.Enable,
                BackgorundColorContainerItem = this.BackgorundColorContainerItem,
                ItemSize = this.ItemSize,
                ComponentFontColor = this.ComponentFontColor,
                ComponentFontSize = this.ComponentFontSize,
                ComponentFontItalic = this.ComponentFontItalic,
                ComponentFontWide = this.ComponentFontWide,
                ComponentFontDecoration = this.ComponentFontDecoration,
                CaptionColor = this.CaptionColor,
                CaptionItalic = this.CaptionItalic,
                CaptionWide = this.CaptionWide,
                CaptionDecoration = this.CaptionDecoration,
                Parent = this.Parent,
                BeginRow = this.BeginRow,
                ColSpan = this.ColSpan,
                RowSpan = this.RowSpan,
                ColumnIndex = this.ColumnIndex,
                RowIndex = this.RowIndex,
                ClassModelType = this.ClassModelType,
                ModelType = this.ModelType,
                SubType = this.SubType,
                BindingType = this.BindingType,
                Nullable = this.Nullable,
                Control = this.Control,
                Icon = this.Icon,
                SlaveModelId = this.SlaveModelId,
                BindedProperty = this.BindedProperty,
                FormName = this.FormName,
                ComponenteExtra = this.ComponenteExtra,
                MultipleReference = this.MultipleReference,
                PlaceHolder = this.PlaceHolder,
                Password = this.Password,
                LookUpInputSize = this.LookUpInputSize,
                Max = this.Max,
                Min = this.Min,
                Step = this.Step,
                CustomMask = this.CustomMask,
                InputString = this.InputString,
                InputDate = this.InputDate,
                InputNumeric = this.InputNumeric,
            };
        }

        #endregion Mapeo para component base
    }
}
