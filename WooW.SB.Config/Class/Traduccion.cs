using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class Traduccion
    {
        public Traduccion()
        {
            IdiomaId = string.Empty;

            Items = new List<TraduccionItem>();
        }
        public string IdiomaId { get => idiomaId; set => idiomaId = (value == null ? string.Empty : value.Trim()); }

        public List<TraduccionItem> Items;
        private string idiomaId;
    }
}
