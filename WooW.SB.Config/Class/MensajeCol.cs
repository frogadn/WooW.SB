using Newtonsoft.Json;
using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class MensajeCol
    {
        public MensajeCol()
        {
            Mensajes = new List<Mensaje>();
        }

        public List<Mensaje> Mensajes { get; set; }

        public Mensaje Clone()
        {
            return JsonConvert.DeserializeObject<Mensaje>(JsonConvert.SerializeObject(this));
        }
    }
}