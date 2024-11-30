using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Designer.DesignerModels
{
    public class WoGridProperties
    {
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
        public string Proceso { get; set; } = string.Empty;

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

        /// <summary>
        /// Indicador de si es extension o un modelo principal
        /// </summary>
        [Browsable(false)]
        public bool IsExtension = false;

        #endregion Modelo


        #region Panel de propiedades

        /// <summary>
        /// Indica cual es el modelo al que se dirija la grid.
        /// </summary>
        [Category("Formulario relacionado")]
        [DisplayName("Formulario relacionado")]
        [Browsable(true)]
        [ReadOnly(true)]
        [Editable(false)]
        public string DirectModel { get; set; } = "";

        /// <summary>
        /// indica si el formulario se abrirá en una nueva tab o si se abrirá en el mismo formulario.
        /// </summary>
        [Category("Formulario relacionado")]
        [DisplayName("Abrir en nueva tab")]
        [Browsable(true)]
        public bool OpenInNewTab { get; set; } = true;

        /// <summary>
        /// Indica cual es el modelo al que se dirija la grid.
        /// </summary>
        [Category("Pantalla")]
        [DisplayName("Titulo de la pantalla")]
        [ReadOnly(true)]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string LabelId { get; set; } = "Root";

        /// <summary>
        /// Muestra el texto de la etiqueta seleccionada
        /// </summary>
        [Category("Pantalla")]
        [DisplayName("Texto del titulo")]
        [Browsable(true)]
        [ReadOnly(true)]
        [Editable(false)]
        public string MaskText { get; set; } = "Root";

        /// <summary>
        /// Indica si se habilita la selección de los campos en la grid
        /// </summary>
        [Category("Pantalla")]
        [DisplayName("Tabla Seleccionable")]
        [Browsable(true)]
        public bool GridSelect { get; set; } = false;

        #endregion Panel de propiedades


        #region Contenido

        /// <summary>
        /// Lista de las columnas que contendrá la grid.
        /// </summary>
        [Browsable(false)]
        public List<WoColumnProperties> WoColumnPropertiesCol { get; set; } =
            new List<WoColumnProperties>();

        #endregion Contenido
    }
}
