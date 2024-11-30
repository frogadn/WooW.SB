using System.ComponentModel;
using System.Drawing.Design;
using WooW.SB.UI;

namespace WooW.SB.Config
{
    public class Idioma
    {
        private string id;
        private string etiquetaId;

        [WoRegEx(@"[a-z][a-z]-[A-Z][A-Z]")]
        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        [Description(@"Etiqueta"), DisplayName("Etiqueta"),
            EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string EtiquetaId { get => etiquetaId; set => etiquetaId = (value == null ? string.Empty : value.Trim()); }

        public override string ToString()
        {
            return $"{Id}-{EtiquetaCol.Get(EtiquetaId)}";
        }
    }
}