using Newtonsoft.Json;
using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class ModeloScriptVistaRoles
    {
        private string modelId;

        public ModeloScriptVistaRoles()
        {
            this.ModelId = string.Empty;
            this.Permisos = new List<string>();
            this.Roles = new List<string>();
        }
        public string ModelId { get => modelId; set => modelId = (value == null ? string.Empty : value.Trim()); }
        public List<string> Roles { get; set; }
        public List<string> Permisos { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static ModeloScriptVistaRoles FromJson(string Json)
        {
            return JsonConvert.DeserializeObject<ModeloScriptVistaRoles>(Json);
        }
        public ModeloScriptVistaRoles Clone()
        {
            return JsonConvert.DeserializeObject<ModeloScriptVistaRoles>(JsonConvert.SerializeObject(this));
        }
    }
}
