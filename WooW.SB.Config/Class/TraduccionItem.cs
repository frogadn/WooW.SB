using Newtonsoft.Json;

namespace WooW.SB.Config
{
    public class TraduccionItem
    {
        private string etiqueta;
        private string valor;

        public TraduccionItem()
        {
            Etiqueta = string.Empty;
            Valor = string.Empty;
            ValorAyuda = string.Empty;
            Traduccion = string.Empty;
            Ayuda = string.Empty;
        }
        public string Etiqueta { get => etiqueta; set => etiqueta = (value == null ? string.Empty : value.Trim()); }

        [JsonIgnore]
        public string Valor { get => valor; set => valor = (value == null ? string.Empty : value.Trim()); }

        [JsonIgnore]
        public string ValorAyuda { get; set; }

        public string Traduccion { get; set; }

        public string Ayuda { get; set; }


        public TraduccionItem Clone()
        {
            return JsonConvert.DeserializeObject<TraduccionItem>(JsonConvert.SerializeObject(this));
        }
    }
}
