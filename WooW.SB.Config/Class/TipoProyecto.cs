using System.ComponentModel;

namespace WooW.SB.Config
{
    public enum ProyectType
    {
        [Description("Solución final con adecuaciones de usuario")]
        SolucionFinalConAdecuacionesDeUsuario,

        [Description("Solución final sin adecuaciones de usuario")]
        SolucionFinalSinAdecuacionesDeUsuario,

        [Description("Solución final base")]
        SolucionFinalBase,

        [Description("Librería")]
        Libreria
    }
}
