using System.ComponentModel;
using System.Drawing.Design;

namespace WooW.SB.Config.ControlProperties.Class
{
    public class TypeControlEnumStringItem
    {
        public TypeControlEnumStringItem()
        {
            Nombre = string.Empty;
            EtiquetaId = string.Empty;
        }

        public string Nombre { get; set; }

        [Description("Etiqueta"), DisplayName("Etiqueta"),
            EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string EtiquetaId { get; set; }
    }
}