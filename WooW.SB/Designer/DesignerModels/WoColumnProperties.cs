using System.ComponentModel;
using System.Drawing.Design;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Designer.DesignerModels
{
    public class WoColumnProperties
    {
        #region Identificador y texto

        /// <summary>
        /// Identificador de la columna.
        /// </summary>
        [Browsable(false)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de la etiqueta de la columna.
        /// </summary>
        [Category("Nombre y texto")]
        [DisplayName("Etiqueta")]
        [Description("Id de la etiqueta que permite recuperar la etiqueta.")]
        [Browsable(true)]
        [ReadOnly(true)]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string Etiqueta { get; set; } = string.Empty;

        /// <summary>
        /// Texto de la etiqueta de la columna.
        /// </summary>
        [Browsable(false)]
        public string MaskText { get; set; } = string.Empty;

        /// <summary>
        /// Bandera que indica si la etiqueta fue modificada por el usuario
        /// o se tomara del modelo.
        /// </summary>
        [Browsable(false)]
        public bool IsCustomLabel { get; set; } = false;

        #endregion Identificador y texto


        #region Modelo

        /// <summary>
        /// Indica el nombre del modelo al que pertenece la grid.
        /// </summary>
        [Browsable(false)]
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// Indica si es una grid de slave o de master.
        /// </summary>
        [Browsable(false)]
        public bool IsSlave = false;

        /// <summary>
        /// Indica el proceso al que pertenece el formulario.
        /// </summary>
        [Browsable(false)]
        public string Process { get; set; } = string.Empty;

        /// <summary>
        /// Tipo del modelo al que pertenece la grid.
        /// </summary>
        [Browsable(false)]
        public WoTypeModel TypeModel { get; set; } = WoTypeModel.Catalog;

        /// <summary>
        /// Tipo del sub modelo al que pertenece la grid.
        /// </summary>
        [Browsable(false)]
        public WoSubTypeModel SubTypeModel { get; set; } = WoSubTypeModel.Report;

        #endregion Modelo


        #region Tipo de columna

        /// <summary>
        /// Indica el tipo de columna.
        /// </summary>
        [Browsable(false)]
        public string BindingType { get; set; } = "string";

        /// <summary>
        /// Indica el tipo de control que corresponde al campo.
        /// </summary>
        [Browsable(false)]
        public WoTypeControl Control { get; set; } = WoTypeControl.NA;

        /// <summary>
        /// Indica si la columna es una columna custom.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        public bool IsCustomColumn { get; set; } = false;

        /// <summary>
        /// Indicador de la columna para marcarla como identificador
        /// </summary>
        [Browsable(true)]
        [DisplayName("Identificador de la grid")]
        public bool IsId { get; set; } = false;

        #endregion Tipo de columna


        #region Formulario del control

        /// <summary>
        /// Indica cual es la operación que recuperara la información de la columna.
        /// </summary>
        [Category("Columna custom")]
        [DisplayName("Operación")]
        [Browsable(false)]
        public string Data { get; set; } = string.Empty;

        #endregion Formulario del control


        #region Dimensiones

        /// <summary>
        /// Tamaño de la columna.
        /// </summary>
        public int Size { get; set; } = 100;

        /// <summary>
        /// Unidad de medida de la columna.
        /// </summary>
        public string SizeType { get; set; } = "px";

        #endregion Dimensiones


        #region Posicionamiento

        /// <summary>
        /// Indica el orden en el que se mostrara la columna.
        /// </summary>
        [Browsable(false)]
        public int Index { get; set; } = 0;

        /// <summary>
        /// Indica si la columna es visible o no.
        /// </summary>
        [Browsable(false)]
        public bool IsVisible { get; set; } = true;

        #endregion Posicionamiento

        #region Referencia

        /// <summary>
        /// Indica si el campo es una referencia.
        /// y hay que poner un botón para dirigirlo al formulario.
        /// </summary>
        [Browsable(false)]
        public bool IsReference { get; set; } = false;

        /// <summary>
        /// Indica el nombre del modelo al que se dirija la celda de la grid.
        /// </summary>
        [Browsable(false)]
        public string ModeReference { get; set; } = string.Empty;

        /// <summary>
        /// Indica el proceso al que pertenece el modelo al que se esta haciendo referencia
        /// </summary>
        [Browsable(false)]
        public string ProcessReference { get; set; } = string.Empty;

        /// <summary>
        /// Puede contener la url base de la referencia en caso de ser una
        /// referencia directa.
        /// </summary>
        [Browsable(false)]
        public string UrlBaseReference { get; set; } = string.Empty;

        #endregion Referencia
    }
}
