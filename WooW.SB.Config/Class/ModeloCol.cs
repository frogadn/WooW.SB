using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class ModeloCol
    {
        public ModeloCol()
        {
            Modelos = new List<Modelo>();
        }

        public List<Modelo> Modelos { get; set; }
    }
}