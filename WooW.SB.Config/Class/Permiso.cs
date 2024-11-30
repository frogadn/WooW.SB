using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing.Design;
using WooW.SB.UI;

namespace WooW.SB.Config
{
    public class Permiso
    {
        private string id;
        private string etiquetaId;

        [WoRegEx(@"[A-Z][a-zA-Z0-9]*")]
        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        [Description("Etiqueta"), DisplayName("Etiqueta"),
            EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string EtiquetaId { get => etiquetaId; set => etiquetaId = (value == null ? string.Empty : value.Trim()); }

        public override string ToString()
        {
            return $"{Id}-{EtiquetaCol.Get(EtiquetaId)}";
        }

        public Permiso Clone()
        {
            return JsonConvert.DeserializeObject<Permiso>(JsonConvert.SerializeObject(this));
        }

    }
}