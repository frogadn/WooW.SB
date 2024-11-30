using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WooW.Core;

namespace WooW.SB.Config
{
    public class Mensaje
    {
        private string id;

        public Mensaje()
        {
            Id = String.Empty;
            Idiomas = new List<MensajeIdioma>();
        }

        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        public List<MensajeIdioma> Idiomas { get; set; }

        public override string ToString()
        {
            if (Idiomas.Count == 0)
                return $"{Id}";
            else
                return $"{Id}-{Idiomas[0]}";
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Mensaje FromJson(string Json)
        {
            if (Json.IsNullOrStringEmpty())
                return new Mensaje();
            else
                return JsonConvert.DeserializeObject<Mensaje>(Json);
        }

        public Mensaje Clone()
        {
            return JsonConvert.DeserializeObject<Mensaje>(JsonConvert.SerializeObject(this));
        }

    }
}