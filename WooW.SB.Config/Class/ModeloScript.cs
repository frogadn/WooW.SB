using Newtonsoft.Json;
using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class ModeloScript
    {
        public ModeloScript()
        {
            ReferenciasPre = new List<string>();
            ReferenciasPost = new List<string>();
        }

        public List<string> ReferenciasPre { get; set; }
        public List<string> ReferenciasPost { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ModeloScript FromJson(string Json)
        {
            return JsonConvert.DeserializeObject<ModeloScript>(Json);
        }

        public ModeloScript Clone()
        {
            return JsonConvert.DeserializeObject<ModeloScript>(JsonConvert.SerializeObject(this));
        }
    }
}