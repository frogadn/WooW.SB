using System.Collections.Generic;

namespace WooW.SB.Config.ControlProperties.Class
{
    public class TypeControlEnumString
    {
        public TypeControlEnumString()
        {
            Items = new List<TypeControlEnumStringItem>();
        }

        public List<TypeControlEnumStringItem> Items { get; set; }
    }
}