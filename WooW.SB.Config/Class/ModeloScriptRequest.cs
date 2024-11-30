using Newtonsoft.Json;

namespace WooW.SB.Config
{
    public class ModeloScriptRequest
    {
        private string modelId;

        public ModeloScriptRequest()
        {
            this.ModelId = string.Empty;
            this.ResponseId = string.Empty;
            this.Coleccion = false;
            this.EjecutarEnBackGround = false;
            this.UsaGetEnRequest = false;
            this.UsaPostEnRequest = false;
            this.UsaPutEnRequest = false;
            this.UsaPatchEnRequest = false;
            this.UsaDeleteEnRequest = false;
        }
        public string ModelId { get => modelId; set => modelId = (value == null ? string.Empty : value.Trim()); }
        public string ResponseId { get; set; }
        public bool Coleccion { get; set; }

        public bool EjecutarEnBackGround { get; set; }

        public bool UsaGetEnRequest { get; set; } = false;
        public bool UsaPostEnRequest { get; set; } = false;
        public bool UsaPutEnRequest { get; set; } = false;
        public bool UsaPatchEnRequest { get; set; } = false;
        public bool UsaDeleteEnRequest { get; set; } = false;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static ModeloScriptRequest FromJson(string Json)
        {
            return JsonConvert.DeserializeObject<ModeloScriptRequest>(Json);
        }
        public ModeloScriptRequest Clone()
        {
            return JsonConvert.DeserializeObject<ModeloScriptRequest>(JsonConvert.SerializeObject(this));
        }
    }
}
