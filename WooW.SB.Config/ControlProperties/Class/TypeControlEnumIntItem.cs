using System.ComponentModel;
using System.Drawing.Design;

namespace WooW.SB.Config.ControlProperties.Class
{
    public class TypeControlEnumIntItem
    {
        public TypeControlEnumIntItem()
        {
            Numero = 0;
            Nombre = string.Empty;
            EtiquetaId = string.Empty;
        }

        public int Numero { get; set; }
        public string Nombre { get; set; }

        [Description("Etiqueta"), DisplayName("Etiqueta"),
            EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string EtiquetaId { get; set; }
    }
}