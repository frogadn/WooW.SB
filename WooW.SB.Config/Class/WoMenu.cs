using System.Collections.Generic;
using System.ComponentModel;

namespace WooW.SB.Config.Class
{
    public class WoMenu
    {
        public WoMenu()
        {
            Activo = false;
            Nombre = string.Empty;
            Icono = string.Empty;
            Referencia = string.Empty;
            Etiqueta = string.Empty;
            Menus = new List<WoMenu>();
        }
        [Description("Nombre asociado al menu"), Category("General")]
        public bool Activo { get; set; }
        [Description("Nombre asociado al menu"), Category("General")]
        public string Nombre { get; set; }

        [Description("Icono del Menu"), Category("General")]
        public string Icono { get; set; }
        [Description("Referencia enlazada"), Category("General")]
        public string Referencia { get; set; }
        [Description("Etiqueta del Menu"), Category("General")]
        public string Etiqueta { get; set; }
        [Description("Orden del menu"), Category("General")]
        public int Orden { get; set; }
        [Description("Lista de menus anidados"), Category("General")]
        public List<WoMenu> Menus;

    }
}
