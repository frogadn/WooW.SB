using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class PruebaIntegral
    {
        private string workItem;
        private string procesoId;

        public PruebaIntegral()
        {
            WorkItem = string.Empty;
            ProcesoId = String.Empty;
            Descripcion = string.Empty;
            Pruebas = new List<PruebaUnitaria>();
        }

        public string WorkItem { get => workItem; set => workItem = (value == null ? string.Empty : value.Trim()); }
        public string ProcesoId { get => procesoId; set => procesoId = (value == null ? string.Empty : value.Trim()); }
        public string Descripcion { get; set; }
        public List<PruebaUnitaria> Pruebas { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static PruebaIntegral FromJson(string Json)
        {
            return JsonConvert.DeserializeObject<PruebaIntegral>(Json);
        }
    }
}