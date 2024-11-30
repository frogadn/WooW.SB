using Newtonsoft.Json;

namespace WooW.SB.Config
{
    public enum eTypePruebaUnitaria
    {
        PruebaUnitaria,
        InicializaDB,
        RespaldarDB,
        RecuperarDB,
        PruebaIntegral,
        CambiaUdn
    };

    public class PruebaUnitaria
    {
        private string archivoPruebaUnitaria;
        #region " Constructor"

        public PruebaUnitaria()
        {
            Orden = 0;
            Tipo = eTypePruebaUnitaria.PruebaUnitaria;
            ArchivoPruebaUnitaria = string.Empty;
            Diagnostico = string.Empty;
        }

        #endregion " Constructor"

        #region " Propiedades"

        public int Orden { get; set; }

        public eTypePruebaUnitaria Tipo { get; set; }
        public string ArchivoPruebaUnitaria { get => archivoPruebaUnitaria; set => archivoPruebaUnitaria = (value == null ? string.Empty : value.Trim()); }

        [JsonIgnore]
        public string Diagnostico { get; set; }

        #endregion " Propiedades"


        public string ToJson()
        {
            //foreach (var c in this.Columnas)
            //{
            //    c.Descripcion = c.sDescripcion;
            //    c.Ayuda = c.sAyuda;
            //    c.Formulario = c.sFormulario;
            //    c.Grid = c.sGrid;
            //}

            return JsonConvert.SerializeObject(this);
        }

        public static PruebaUnitaria FromJson(string Json)
        {
            var model = JsonConvert.DeserializeObject<PruebaUnitaria>(Json);

            return model;
        }

        public PruebaUnitaria Clone()
        {
            var model = JsonConvert.DeserializeObject<PruebaUnitaria>(JsonConvert.SerializeObject(this));

            return model;
        }

    }
}