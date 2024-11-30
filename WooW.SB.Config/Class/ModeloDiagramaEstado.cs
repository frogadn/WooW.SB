using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace WooW.SB.Config
{
    public enum eEstadoTipo
    {
        Nulo,
        Inicial,
        Intermedio,
        FinalNormal,
        FinalAlternativo
    }

    public enum eEstadoSubTipo
    {
        Nulo,

        // ESTADOS PARA CATALOGOS

        PreActivo,      // eEstadoTipo.Inicial

        // Catálogo que no se puede ocupar
        Activo,         // eEstadoTipo.Inicial o eEstadoTipo.Intermedio

        // Catálogo que se puede ocupar o modificar
        Suspendido,     // eEstadoTipo.Intermedio

        // Catalogo suspendido
        BajaLogica,     // eEstadoTipo.FinalAlternativo

        // Catalogo baja

        // ESTADOS PARA TRANSACCIONES

        Modificandose,  // eEstadoTipo.Inicial

        // Transacción que puede modificarse
        PreAplicada,    // eEstadoTipo.Intermedio

        // Transacción que esta pendiente de autorización
        Aplicada,       // eEstadoTipo.Intermedio o eEstadoTipo.FinalNormal (si no contabiliza)

        // Transacción aplicada
        Contabilizada,  // eEstadoTipo.FinalNormal

        // Transacción contabilizada
        Descartada      // eEstadoTipo.FinalAlternativo

        // Transacción descartada
    }

    #region " Clase Estado"

    [Serializable]
    public class Estado
    {
        private string id;
        private string etiquetaId;
        #region " Constructor"

        public override string ToString()
        {
            return Id;
        }

        public Estado()
        {
            Id = string.Empty;
            NumId = 0;
            Tipo = eEstadoTipo.Intermedio;
            SubTipo = eEstadoSubTipo.Nulo;
            EtiquetaId = string.Empty;
            Abscisa = 0;
            Ordenada = 0;
            Idgrafico = string.Empty;
        }

        #endregion " Constructor"

        #region " Propiedades"

        [Category("Datos"), Description("Id del estado"),
        DisplayNameAttribute("Estado")]
        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        [Category("Datos"), Description("Número asignado el estado"),
        DisplayNameAttribute("Número de Estado")]
        public int NumId { get; set; }

        [Category("Datos"), Description("Descripción del estado"),
        DisplayNameAttribute("Etiqueta")]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string EtiquetaId { get => etiquetaId; set => etiquetaId = (value == null ? string.Empty : value.Trim()); }

        [Category("Parámetros"), Description("Tipo de estado"),
        DisplayNameAttribute("Tipo")]
        public eEstadoTipo Tipo { get; set; }

        [Category("Parámetros"), Description("Sub Tipo de estado"),
        DisplayNameAttribute("Sub Tipo")]
        public eEstadoSubTipo SubTipo { get; set; }

        [BrowsableAttribute(false)]
        public string Idgrafico { get; set; }

        [BrowsableAttribute(false)]
        public float Ordenada { get; set; }

        [BrowsableAttribute(false)]
        public float Abscisa { get; set; }

        #endregion " Propiedades"
    }

    #endregion " Clase Estado"
}