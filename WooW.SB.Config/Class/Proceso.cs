using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using WooW.SB.UI;

namespace WooW.SB.Config
{
    public class Proceso
    {
        private string id;
        private string etiquetaId;

        [WoRegEx(@"[A-Z][a-zA-Z0-9]*")]
        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        [Description("Etiqueta"), DisplayName("Etiqueta"),
            EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        [Editable(true)]
        public string EtiquetaId { get => etiquetaId; set => etiquetaId = (value == null ? string.Empty : value.Trim()); }

        public override string ToString()
        {
            return $"{Id}-{EtiquetaCol.Get(EtiquetaId)}";
        }
    }
}