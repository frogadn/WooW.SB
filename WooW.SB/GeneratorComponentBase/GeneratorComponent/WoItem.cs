using System.ComponentModel;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.FormDesigner;
using WooW.SB.GeneratorComponentBase.GeneratorInterfaces.GeneralComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.GeneratorComponentBase.GeneratorComponent
{
    public class WoItem
        // Interfaces comunes para todos los componentes
        : IWoComponentId,
            IWoComponentBehaviour,
            IWoIcono,
            // Interfaces de diseño
            IWoDesignAplied,
            IWoFontDesing,
            IWoCaptionDesing,
            // Interfaces propias de los formularios
            IWoItemType,
            IWoItemDesign,
            IWoFormPosition
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
        public eTypeItem TypeItem { get; set; } = eTypeItem.None;

        /// <summary>
        /// Indicador si el componente que se generar contendra un WoFormIntem encaosulandolo.
        /// (Si no se quiere usar un WoForm, ponerlo en false)
        /// </summary>
        public bool AddFormItem = true;

        /// <summary>
        /// Comportamiento base.
        /// </summary>
        public eItemEnabled Enable { get; set; } = eItemEnabled.Activo;

        /// <summary>
        /// Referencia.
        /// </summary>
        public string Reference { get; set; } = string.Empty;

        #endregion Comportamiento base

        #region Propiedades de diseño generales

        /// <summary>
        /// Color de fondo del componente.
        /// </summary>
        public eContainerItemColor BackgorundColorContainerItem { get; set; } =
            eContainerItemColor.Default;

        /// <summary>
        /// Tamaño del componente.
        /// </summary>
        public eItemSize ItemSize { get; set; } = eItemSize.Normal;

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

        #region Propiedades de diseño del caption

        /// <summary>
        /// Color de fuente del caption.
        /// </summary>
        public eTextColor CaptionColor { get; set; } = eTextColor.FontDefault;

        /// <summary>
        /// Tipo de fuente itálica del caption.
        /// </summary>
        public eTextItalic CaptionItalic { get; set; } = eTextItalic.None;

        /// <summary>
        /// Ancho de la letra del caption.
        /// </summary>
        public eTextWeight CaptionWide { get; set; } = eTextWeight.Normal;

        /// <summary>
        /// Decorado del texto del caption.
        /// </summary>
        public eTextDecoration CaptionDecoration { get; set; } = eTextDecoration.None;

        #endregion Propiedades de diseño del caption

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

        #region Tipo

        /// <summary>
        /// En caso de ser una esclava, un look up o algún control custom se indica cual es el modelo de la referencia.
        /// </summary>
        public string ClassModelType { get; set; } = string.Empty;

        /// <summary>
        /// Indicador en caso de que la referencia se duplique se indica cual es el numero de referencia eetida.
        /// </summary>
        public int MultipleReference { get; set; } = 0;

        /// <summary>
        /// Indica el tipo de la variable a la que estará bandeado el campo.
        /// </summary>
        public string BindingType { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la variable bindeada al control prodra ser nula.
        /// </summary>
        public bool Nullable { get; set; } = false;

        /// <summary>
        /// Indica el tipo de control que se generara en blazor.
        /// </summary>
        public string Control { get; set; } = string.Empty;

        #endregion Tipo

        #region Iconos

        /// <summary>
        /// Icono del contenedor.
        /// </summary>
        public eBoostrapIcons Icon { get; set; } = eBoostrapIcons.None;

        #endregion Iconos

        #region Slaves

        /// <summary>
        /// Indica el nombre del modelo de la slave del formulario en caso de
        /// que el item este representando alguna.
        /// </summary>
        public string SlaveModelId { get; set; } = string.Empty;

        #endregion Slaves

        #region Modelo

        /// <summary>
        /// Indica cual es el nombre del modelo dor formulario que se esta diseñado o creando.
        /// </summary>
        public string BaseModelName = string.Empty;

        /// <summary>
        /// Indica el nombre de la propiedad del modelo que se relaciona con el control.
        /// </summary>
        public string BindedProperty = string.Empty;

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

        #endregion Modelo

        #region Items abstractos

        /// <summary>
        /// En caso de ser un objeto que representa un formulario del diseñador de formularios
        /// </summary>
        public string FormName = string.Empty;

        #endregion Items abstractos

        #region Items internos

        /// <summary>
        /// Indica un formulario interno del componente.
        /// Para esclavas y para reportes.
        /// Llenar dentro de la generacion puesto que el nombre varia en las unitarias.
        /// </summary>
        public string InternalFrom = string.Empty;

        #endregion Items internos

        /// <summary>
        /// Indica si es un componente no relacionado a un modelo.
        /// </summary>
        [Browsable(false)]
        public bool ComponenteExtra = false;

        /// <summary>
        /// Indica cuando el componente no tiene un modelo asociado.
        /// </summary>
        [Browsable(false)]
        public bool NoModelComponent { get; set; } = false;

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

        #region Simple TextComponents

        /// <summary>
        /// Texto que funje de placeholder para los tipos de texto.
        /// </summary>
        public string PlaceHolder { get; set; } = string.Empty;

        /// <summary>
        /// Indica si es o no un caption de tipo password.
        /// </summary>
        public bool Password { get; set; } = false;

        #endregion Simple TextComponents

        #region Lookups

        /// <summary>
        /// Indica el tamaño del lookup del input.
        /// </summary>
        public eLookupInputWidth LookUpInputSize { get; set; } = eLookupInputWidth.Medium;

        #endregion Lookups

        #region Reportes

        /// <summary>
        /// Nombre del modelo de request del reporte.
        /// Nombre del modelo que funge como petición de la data del reporte.
        /// </summary>
        [Browsable(false)]
        public string ReportRequest { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del modelo de response del reporte.
        /// Nombre del modelo que funge como respuesta de la data del reporte.
        /// </summary>
        [Browsable(false)]
        public string ReportResponse { get; set; } = string.Empty;

        /// <summary>
        /// Bandra que indica que el reporte es de tipo OData
        /// </summary>
        [Browsable(false)]
        public bool ReportOdata { get; set; } = false;

        #endregion Reportes

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
                ModelType = this.ModelType,
                SubType = this.SubType,
                RowIndex = this.RowIndex,
                ClassModelType = this.ClassModelType,
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
                CustomMask = this.CustomMask,
                InputString = this.InputString,
                InputDate = this.InputDate,
                InputNumeric = this.InputNumeric,
            };
        }

        #endregion Mapeo para formulario
    }
}
