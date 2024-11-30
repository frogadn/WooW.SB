using System.Collections.Generic;

namespace WooW.SB.Config.ControlProperties.Class
{
    public class TypeControlEnumInt
    {
        public TypeControlEnumInt()
        {
            Items = new List<TypeControlEnumIntItem>();
        }

        public List<TypeControlEnumIntItem> Items { get; set; }
    }
}