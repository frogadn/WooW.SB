using System.Collections.Generic;
using Newtonsoft.Json;

namespace WooW.SB.Forms
{
    public enum veXMAXTypeColumn
    {
        String,
        Autoincrement,
        Smallint,
        Integer,
        Long,
        Boolean,
        Decimal,
        Double,
        Date,
        EnumInt,
        EnumString,
        Timestamp,
        Clob,
        Blob
    }

    public enum veXMAXTypeData
    {
        Primary,
        Volatil,
        Internal,
        Control,
        Reference,
        Contable
    }

    public enum veXMAXTypeControl
    {
        Text,
        Memo,
        Bool,
        Spin,
        Decimal,
        Date,
        EnumInt,
        EnumString,
        UnKnown
    }

    public class veXMAXModelExport
    {
        public string Legacy { get; set; }
        public List<veXMAXModelColumn> Columnas { get; set; }

        public veXMAXModelExport()
        {
            Legacy = string.Empty;
            Columnas = new List<veXMAXModelColumn>();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static veXMAXModelExport FromJson(string Json)
        {
            return JsonConvert.DeserializeObject<veXMAXModelExport>(Json);
        }
    }

    public class veXMAXModelColumn
    {
        public veXMAXModelColumn()
        {
            Id = string.Empty;
            Orden = 0;
            Longitud = 0;
            Precision = 0;
            Descripcion = string.Empty;
            Ayuda = string.Empty;
            Formulario = string.Empty;
            Grid = string.Empty;
            TipoColumna = veXMAXTypeColumn.String;
            TipoDato = veXMAXTypeData.Volatil;
            Primaria = false;
            Nulo = false;
            Default = string.Empty;
            Legacy = string.Empty;
            TipoControl = veXMAXTypeControl.Text;
            TipoControlPropiedades = string.Empty;
            Referencia = string.Empty;
        }

        public string Id { get; set; }
        public int Orden { get; set; }
        public int Longitud { get; set; }
        public int Precision { get; set; }
        public string Descripcion { get; set; }
        public string Ayuda { get; set; }
        public string Formulario { get; set; }
        public string Grid { get; set; }
        public veXMAXTypeColumn TipoColumna { get; set; }
        public veXMAXTypeData TipoDato { get; set; }
        public bool Primaria { get; set; }
        public bool Nulo { get; set; }
        public string Default { get; set; }

        public string Legacy { get; set; }

        public veXMAXTypeControl TipoControl { get; set; }

        public string TipoControlPropiedades { get; set; }

        public string Referencia { get; set; }
    }
}
