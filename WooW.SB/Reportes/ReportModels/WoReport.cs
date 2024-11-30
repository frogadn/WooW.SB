using System;
using System.ComponentModel;
using System.Drawing.Design;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Reportes.ReportModels
{
    public class WoReport
    {
        /// <summary>
        /// Nombre, identificador o forma mediante la cual podemos identificar un componente del diseñador, es único.
        /// </summary>
        ///  [Category("Identificador")]
        [DisplayName("Id")]
        [Description("Id único que permite reconocer el componente.")]
        [Browsable(true)]
        public string idReport { get; set; }

        #region Nombre y texto
        /// <summary>
        /// Etiqueta o texto que se muestra en el componente.
        /// Puede ser uno para varios componentes.
        /// </summary>
        [Category("Nombre y texto")]
        [DisplayName("Etiqueta")]
        [Description("Id de la etiqueta que permite recuperar la etiqueta.")]
        [Browsable(false)]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string Etiqueta { get; set; } = string.Empty;

        /// <summary>
        /// Funciona como una etiqueta auxiliar para que el componente pueda mostrar contenido con propiedades HTML
        /// en la vista previa de diseño.
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        public string MaskText { get; set; } = string.Empty;

        #endregion Nombre y texto
        /// <summary>
        /// Contiene el Id del proceso al que pertenece el reporte.
        /// </summary>
        [DisplayName("proccessId")]
        [Description("Id del proceso al que pertenece el reporte.")]
        [Browsable(true)]
        public string proccessId { get; set; }

        /// <summary>
        /// Contiene el tipo de modelo que se va a utilizar para el reporte.
        /// </summary>
        [DisplayName("modelType")]
        [Description("Tipo de modelo.")]
        [Browsable(false)]
        public WoTypeModel modelType { get; set; }

        /// <summary>
        /// Nos indica si existen reportes creados para el modelo actual.
        /// </summary>
        [DisplayName("reportDesign")]
        [Description("Nos indica si existen reprotes creados para el modelo actual.")]
        [Browsable(true)]
        public bool reportDesign { get; set; }

        /// <summary>
        /// Contiene la estructura Json del modelo.
        /// </summary>
        [DisplayName("jsonModel")]
        [Description("Estructura Json del modelo.")]
        [Browsable(false)]
        public string jsonModel { get; set; }

        /// <summary>
        /// Indica cual es el modelo al que se dirigirá la grid.
        /// </summary>
        [Category("Pantalla")]
        [DisplayName("Titulo de la pantalla")]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        [Browsable(false)]
        [ReadOnly(true)]
        public string LabelId { get; set; } = "Root";

        /// <summary>
        /// Nos permite configurar una auto impresion dentro del reporte.
        /// </summary>
        [Category("Imprimir")]
        [DisplayName("Auto Imprimir")]
        [Browsable(false)]
        public bool AutoPrint { get; set; }

        /// <summary>
        /// Indica configurar el numero de copias de impresión.
        /// </summary>
        [Category("Imprimir")]
        [DisplayName("Numero de Impresiones")]
        [Browsable(false)]
        public string NumPrint { get; set; } = "";

        /// <summary>
        /// Permite configurar el control de si puede o no exportarse el reporte.
        /// </summary>
        [Category("Pantalla")]
        [DisplayName("Permitir Exportar")]
        [Browsable(false)]
        public bool Export { get; set; }

        /// <summary>
        /// Nos permite determinar mostrar el titulo de la udn o el titulo definido en la propiedad LabelId.
        /// </summary>
        [Category("Pantalla")]
        [DisplayName("Titulo UDN/Empresa")]
        [Browsable(false)]
        public TittleType TittleType { get; set; }

        /// <summary>
        /// Indica cual es el modelo al que se dirigirá la grid.
        /// </summary>
        [Category("Imprimir")]
        [DisplayName("Imprimir y cerrar")]
        [Browsable(false)]
        public bool PrintClose { get; set; }

        public WoReport()
        {
            idReport = String.Empty;
            Etiqueta = String.Empty;
            proccessId = String.Empty;
            modelType = WoTypeModel.Catalog;
            jsonModel = String.Empty;
            MaskText = String.Empty;
            reportDesign = false;
            LabelId = "Root";
        }
    }

    public enum TittleType
    {
        UDN,
        Empresa
    }
}
