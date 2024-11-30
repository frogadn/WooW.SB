using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WooW.Core;

namespace WooW.SB.Config
{
    public class EtiquetaCol
    {

        public EtiquetaCol()
        {
            Etiquetas = new List<Etiqueta>();
        }

        public List<Etiqueta> Etiquetas { get; set; }

        public EtiquetaCol Clone()
        {
            return JsonConvert.DeserializeObject<EtiquetaCol>(JsonConvert.SerializeObject(this));
        }

        public static string Get(string Etiqueta)
        {
            return Get(Etiqueta, Proyecto.getInstance().esMX);
        }

        public static string Get(string Etiqueta, string Idioma)
        {
            var e = Proyecto.getInstance().EtiquetaCol.Etiquetas.Where(g =>
                g.Id == Etiqueta).FirstOrDefault();

            if (e.IsNull())
                return " ETIQUETA NO EXISTE";

            var f = e.Idiomas.Where(g => g.IdiomaId == Idioma).FirstOrDefault();

            if (f.IsNull())
                return " ETIQUETA CON IDIOMA NO EXISTE";

            return f.Texto;
        }
    }
}