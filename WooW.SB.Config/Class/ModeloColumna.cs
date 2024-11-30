using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WooW.Core;
using WooW.Core.Common;
using WooW.SB.Config.Class;
using WooW.SB.Config.ControlProperties.Class;

namespace WooW.SB.Config
{
    public class ModeloColumna
    {
        private Proyecto proyecto;
        private string id;
        private string descripcion;
        private string ayuda;
        private string formulario;
        private string grid;
        private string descripcionEtiqueta;
        private string formularioEtiqueta;
        private string gridEtiqueta;
        private string modeloId;
        private string _default;
        private string legacy;
        private string origen;


        public ModeloColumna()
        {
            Id = string.Empty;
            Orden = 0;
            Longitud = 0;
            Precision = 0;
            Descripcion = string.Empty;
            Ayuda = string.Empty;
            Formulario = string.Empty;
            Grid = string.Empty;
            DescripcionEtiqueta = string.Empty;
            FormularioEtiqueta = string.Empty;
            GridEtiqueta = string.Empty;

            TipoColumna = WoTypeColumn.String;
            EsColeccion = false;
            ModeloId = string.Empty;
            TipoDato = WoTypeData.Volatil;
            EsVisibleEnLookUp = false;
            Persistente = true;
            Primaria = false;
            Nulo = false;
            Default = string.Empty;
            Legacy = string.Empty;
            TipoControl = WoTypeControl.Text;
            TipoControlPropiedades = string.Empty;
            Origen = string.Empty;
            Apps = new List<ModeloApp>();
            proyecto = null;
        }

        public ModeloColumna(Proyecto _proyecto) : this()
        {
            proyecto = _proyecto;
        }

        public void ProyectoSetter(Proyecto _proyecto)
        {
            proyecto = _proyecto;
        }


        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }
        public int Orden { get; set; }
        public int Longitud { get; set; }
        public int Precision { get; set; }
        public string Descripcion { get => descripcion; set => descripcion = (value == null ? string.Empty : value.Trim()); }
        public string Ayuda { get => ayuda; set => ayuda = (value == null ? string.Empty : value.Trim()); }
        public string Formulario { get => formulario; set => formulario = (value == null ? string.Empty : value.Trim()); }
        public string Grid { get => grid; set => grid = (value == null ? string.Empty : value.Trim()); }

        [JsonIgnore]
        public string DescripcionEtiqueta { get => (descripcionEtiqueta.IsNullOrStringEmpty() ? Etiqueta.ToId(Descripcion) : descripcionEtiqueta); set => descripcionEtiqueta = (value.IsNullOrStringEmpty() ? string.Empty : value.Trim()); }

        [JsonIgnore]
        public string FormularioEtiqueta { get => (formularioEtiqueta.IsNullOrStringEmpty() ? Etiqueta.ToId(Formulario) : formularioEtiqueta); set => formularioEtiqueta = (value.IsNullOrStringEmpty() ? string.Empty : value.Trim()); }

        [JsonIgnore]
        public string GridEtiqueta { get => (gridEtiqueta.IsNullOrStringEmpty() ? Etiqueta.ToId(Grid) : gridEtiqueta); set => gridEtiqueta = (value.IsNullOrStringEmpty() ? string.Empty : value.Trim()); }

        public WoTypeColumn TipoColumna { get; set; }
        public bool EsColeccion { get; set; }

        public string ModeloId { get => modeloId; set => modeloId = (value == null ? string.Empty : value.Trim()); }
        public WoTypeData TipoDato { get; set; }
        public bool EsVisibleEnLookUp { get; set; }
        public bool Primaria { get; set; }

        public bool Persistente { get; set; }
        public bool Nulo { get; set; }
        public string Default { get => _default; set => _default = (value == null ? string.Empty : value.Trim()); }

        public string Legacy { get => legacy; set => legacy = (value == null ? string.Empty : value.Trim()); }

        public string Origen { get => origen; set => origen = (value == null ? string.Empty : value.Trim()); }

        public WoTypeControl TipoControl { get; set; }

        public string TipoControlPropiedades { get; set; }

        public List<ModeloApp> Apps { get; set; }

        public List<ModeloColumnaIdioma> Idiomas { get; set; }

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

        public static ModeloColumna FromJson(string Json, Proyecto _proyecto)
        {
            var modelColumna = JsonConvert.DeserializeObject<ModeloColumna>(Json);
            modelColumna.ProyectoSetter(_proyecto);

            return modelColumna;
        }

        public ModeloColumna Clone(Proyecto _proyecto)
        {
            var modelColumna = JsonConvert.DeserializeObject<ModeloColumna>(JsonConvert.SerializeObject(this));
            modelColumna.ProyectoSetter(_proyecto);

            return modelColumna;
        }


        public override string ToString()
        {
            if ((Idiomas == null) || (Idiomas.Count == 0))
                return $"{Id}";
            else
                return $"{Id}-{Idiomas[0]}";
        }

        public string ToAttributesString()
        {
            string Attributes = string.Empty;
            if (TipoColumna == WoTypeColumn.String)
                Attributes += $"        [StringLength({Longitud})]\r\n";

            return Attributes;
        }

        public string ToAttributes()
        {
            string Attributes = string.Empty;
            if (this.Primaria)
                Attributes += @"        [PrimaryKey]";
            if (!Nulo)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @"        [Required]";
            }

            if (Id == WoConst.WOGUID)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @"        [Index(true)]";
            }


            if (TipoColumna == WoTypeColumn.String)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += $"        [StringLength({Longitud})]";
            }

            if (TipoColumna == WoTypeColumn.Clob)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += $"        [StringLength(int.MaxValue)]";
            }

            if (TipoColumna == WoTypeColumn.Decimal)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += $"        [DecimalLength({Longitud},{Precision})]";
            }

            if (!Default.IsNullOrStringEmpty())
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";

                if (TipoColumna == WoTypeColumn.Boolean)
                {
                    /*
                     * No funciona en service Stack
                    if (Default.ToLower() == "true")
                        Attributes += $"        [Default(typeof(bool), \"1\")]";
                    else if (Default.ToLower() == "false")
                        Attributes += $"        [Default(typeof(bool), \"0\")]";
                    else
                        throw new Exception($"El valor default {Default} para la columna {Id} no es valido");
                    */
                }
                if (TipoColumna == WoTypeColumn.Smallint)
                {
                    Int16 iDefault = 0;
                    if (Int16.TryParse(Default, out iDefault))
                        Attributes += $"        [Default(typeof(System.Int16), \"{iDefault}\")]";
                    else
                        throw new Exception($"El valor default {Default} para la columna {Id} no es valido");
                }
                if (TipoColumna == WoTypeColumn.Integer)
                {
                    Int32 iDefault = 0;
                    if (Int32.TryParse(Default, out iDefault))
                        Attributes += $"        [Default(typeof(System.Int32), \"{iDefault}\")]";
                    else
                        throw new Exception($"El valor default {Default} para la columna {Id} no es valido");
                }
                if (TipoColumna == WoTypeColumn.Long)
                {
                    Int64 iDefault = 0;
                    if (Int64.TryParse(Default, out iDefault))
                        Attributes += $"        [Default(typeof(System.Int64), \"{iDefault}\")]";
                    else
                        throw new Exception($"El valor default {Default} para la columna {Id} no es valido");
                }
                if (TipoColumna == WoTypeColumn.Decimal)
                {
                    string sDefault = Default.Trim().ToLower();
                    if (!sDefault.EndsWith("m"))
                        throw new Exception($"El valor default {Default} para la columna {Id} no es valido");

                    sDefault = sDefault.Substring(0, sDefault.Length - 1);

                    Decimal dDefault = 0;
                    if (Decimal.TryParse(sDefault, out dDefault))
                        Attributes += $"        [Default(typeof(System.Decimal), \"{dDefault}\")]";
                    else
                        throw new Exception($"El valor default {Default} para la columna {Id} no es valido");
                }
                if (TipoColumna == WoTypeColumn.String)
                {
                    Default = Default.Trim();
                    if (Default.EndsWith("\"") && Default.StartsWith("\""))
                        Attributes += $"        [Default(typeof(System.String), \"'{Default.Substring(1, Default.Length - 2)}'\")]";
                }
            }


            if (TipoColumna == WoTypeColumn.Reference)
            {
                Modelo modloc = null;
                if (proyecto != null)
                    modloc = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                if (modloc != null)
                {
                    var colloc = modloc.Columnas.Where(g => g.Id == "Id").FirstOrDefault();

                    if (colloc.TipoColumna == WoTypeColumn.String)
                    {
                        if (!Attributes.IsNullOrStringEmpty())
                            Attributes += "\r\n";
                        Attributes += $@"        [StringLength({colloc.Longitud})]";
                    }

                    if (!Attributes.IsNullOrStringEmpty())
                        Attributes += "\r\n";
                    Attributes += $@"        [References(typeof({ModeloId}))]";

                }
            }

            if ((EsColeccion) && (TipoColumna == WoTypeColumn.Reference))
            {


                var modloc = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                if (modloc != null)
                {
                    var colloc = modloc.Columnas.Where(g => g.Id == "Id").FirstOrDefault();

                    if (!Attributes.IsNullOrStringEmpty())
                        Attributes += "\r\n";
                    Attributes += $@"        [Reference]";
                    Attributes += "\r\n";
                    Attributes += @$"        [WoReferenceBase(typeof({ModeloId}))]";
                }
            }

            if (!Persistente)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @$"        [Ignore]";
            }

            if (!Attributes.IsNullOrStringEmpty())
                Attributes += "\r\n";
            Attributes += @$"        [Description(nameof(WooW_ModelData.{DescripcionEtiqueta}))]";

            if (!Attributes.IsNullOrStringEmpty())
                Attributes += "\r\n";

            Attributes +=
$@"        [WoLabelModel(Descripcion = nameof(WooW_ModelData.{DescripcionEtiqueta}),
           Formulario = nameof(WooW_ModelLabel.{FormularioEtiqueta}),
           Grid = nameof(WooW_ModelLabel.{GridEtiqueta}),
           Help = nameof(WooW_ModelComment.{DescripcionEtiqueta}))]";

            Attributes += "\r\n";


            string DescripcionLookUp = string.Empty;
            if (!this.ModeloId.IsNullOrStringEmpty())
            {
                var modloc = proyecto.ModeloCol.Modelos.Where(x => x.Id == ModeloId).FirstOrDefault();
                if (modloc != null)
                {
                    var colloc = modloc.Columnas.Where(x => x.EsVisibleEnLookUp && x.Id != "Id").FirstOrDefault();
                    if (colloc != null)
                        DescripcionLookUp = colloc.Id;
                }
            }

            Attributes +=
$@"        [WoColumnMeta(
           Orden = {this.Orden},
           Longitud = {this.Longitud},
           Precision = {this.Precision},
           TipoColumna = WoTypeColumn.{this.TipoColumna.ToString()},
           ModeloId = ""{this.ModeloId}"",
           DescripcionLookUp = ""{DescripcionLookUp}"",
           TipoDato = WoTypeData.{this.TipoDato.ToString()},
           EsVisibleEnLookUp = {this.EsVisibleEnLookUp.ToString().ToLower()},
           Primaria = {this.Primaria.ToString().ToLower()},
           Nulo = {this.Nulo.ToString().ToLower()},
           Default = ""{SinComillas(this.Default)}"",
           Legacy = ""{this.Legacy}"",
           TipoControl = WoTypeControl.{this.TipoControl})]";

            return ToAppNotDefines(Attributes);
        }

        private string SinComillas(string valor)
        {
            if (valor == null)
                return string.Empty;
            else
            {
                if (valor.StartsWith(@""""))
                    valor = valor.Substring(1);
                if (valor.EndsWith(@""""))
                    valor = valor.Substring(0, valor.Length - 1);
                return valor;
            }
        }

        public string ToAttributesDTO()
        {
            string Attributes = string.Empty;

            if (!Nulo)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @"        [Required]";
            }
            if (TipoColumna == WoTypeColumn.String)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @$"        [StringLength({Longitud})]";
            }
            if (TipoColumna == WoTypeColumn.Decimal)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @$"        [DecimalLength({Longitud},{Precision})]";
            }
            if (TipoColumna == WoTypeColumn.Clob)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @$"        [StringLength(StringLengthAttribute.MaxText)]";
            }



            if (TipoColumna == WoTypeColumn.Reference)
            {
                var modloc = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                if (modloc != null)
                {
                    var colloc = modloc.Columnas.Where(g => g.Id == "Id").FirstOrDefault();

                    if (TipoColumna == WoTypeColumn.String)
                    {
                        if (!Attributes.IsNullOrStringEmpty())
                            Attributes += "\r\n";
                        Attributes += $"        [StringLength({colloc.Longitud})]";
                    }

                    if (!Attributes.IsNullOrStringEmpty())
                        Attributes += "\r\n";
                    Attributes += @$"        [References(typeof({ModeloId}))]";
                }
            }

            if (!Attributes.IsNullOrStringEmpty())
                Attributes += "\r\n";
            Attributes += @$"        [Description(nameof(WooW_ModelData.{DescripcionEtiqueta}))]";

            if (!Attributes.IsNullOrStringEmpty())
                Attributes += "\r\n";

            if (!Persistente)
            {
                if (!Attributes.IsNullOrStringEmpty())
                    Attributes += "\r\n";
                Attributes += @$"        [Ignore]";
            }

            Attributes +=
$@"        [WoLabelModel(Descripcion = nameof(WooW_ModelData.{DescripcionEtiqueta}),
           Formulario = nameof(WooW_ModelLabel.{FormularioEtiqueta}),
           Grid = nameof(WooW_ModelLabel.{GridEtiqueta}),
           Help = nameof(WooW_ModelComment.{DescripcionEtiqueta}))]";

            Attributes += "\r\n";

            Attributes +=
$@"        [WoColumnMeta(
           Orden = {this.Orden},
           Longitud = {this.Longitud},
           Precision = {this.Precision},
           TipoColumna = WoTypeColumn.{this.TipoColumna.ToString()},
           ModeloId = ""{this.ModeloId}"",
           TipoDato = WoTypeData.{this.TipoDato.ToString()},
           EsVisibleEnLookUp = {this.EsVisibleEnLookUp.ToString().ToLower()},
           Primaria = {this.Primaria.ToString().ToLower()},
           Nulo = {this.Nulo.ToString().ToLower()},
           Default = ""{SinComillas(this.Default)}"",
           Legacy = ""{this.Legacy}"",
           TipoControl = WoTypeControl.{this.TipoControl})]";

            return ToAppNotDefines(Attributes);
        }


        public string ToProperty(Modelo modelo)
        {
            return ToProperty(modelo, false);
        }


        public string ToProperty(Modelo modelo, bool bInterface)
        {
            string OverRide = string.Empty;
            string sProperty = string.Empty;


            if (modelo.SubTipoModelo == WoSubTypeModel.Static)
                OverRide = @"static ";


            if (this.Primaria)
            {
                if ((modelo.TipoModelo == WoTypeModel.Control)
                    || (modelo.TipoModelo == WoTypeModel.Kardex)
                    || (modelo.TipoModelo == WoTypeModel.DataMart))
                {
                    sProperty = $"[AutoIncrement]\r\n        public override ulong Id {{ get; set; }}\r\n";
                    return sProperty;
                }
                OverRide = @"override ";
            }

            if ((modelo.TipoModelo == WoTypeModel.CatalogSlave) ||
                (modelo.TipoModelo == WoTypeModel.TransactionSlave))
            {
                if (Id == WoConst.WORENGLON)
                    OverRide = @"override ";
            }

            if ((modelo.TipoModelo == WoTypeModel.TransactionContable) ||
                (modelo.TipoModelo == WoTypeModel.TransactionNoContable))
            {
                List<string> sl = new List<string>() { "Id", WoConst.WOUDNID, WoConst.WOSERIE, WoConst.WOFOLIO, WoConst.WOFECHA, WoConst.WOOBSERVACION, WoConst.WOGUID };

                if (sl.IndexOf(Id) != -1)
                    OverRide = @"override ";
            }

            string sList1 = string.Empty;
            string sList2 = string.Empty;
            if (EsColeccion)
            {
                if (bInterface)
                    sList1 = @" IList <";
                else
                    sList1 = @"virtual IList <";
                sList2 = @">";
            }


            if ((TipoColumna == WoTypeColumn.EnumInt) ||
                     (TipoColumna == WoTypeColumn.EnumString))
            {
                if (Nulo)
                    throw new Exception($"Modelo {ModeloId}, columna {Id} de tipo enum no permite nulos, corrija el modelo");
                sProperty = $"public {OverRide}{sList1}{TipoCS((ModeloId.IsNullOrStringEmpty() ? modelo.Id : ModeloId), EsColeccion)}{sList2} {Id} {{ get; set; }}\r\n";
            }
            else if (TipoColumna == WoTypeColumn.Reference)
            {
                if (EsColeccion)
                {
                    var modRef = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                    if (modRef == null)
                        throw new Exception($"Modelo no encontrado {ModeloId}");

                    var colloc = modRef.Columnas.Where(g => g.Id == "Id").FirstOrDefault();
                    sProperty = $"public {OverRide}{sList1}{modRef.Id}{sList2} {Id} {{ get; set; }}\r\n";
                }
                else
                {
                    var modRef = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                    if (modRef == null)
                        throw new Exception($"Modelo no encontrado {ModeloId}");

                    var colloc = modRef.Columnas.Where(g => g.Id == "Id").FirstOrDefault();
                    sProperty = $"public {OverRide}{sList1}{colloc.TipoCS(ModeloId, false)}{sList2} {Id} {{ get; set; }}\r\n";
                }
            }
            else if (this.TipoColumna == WoTypeColumn.Complex)
            {
                var modRef = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                if (modRef == null)
                    throw new Exception($"Modelo no encontrado {ModeloId}");

                var colloc = modRef.Columnas.Where(g => g.Id == "Id").FirstOrDefault();
                sProperty = $"public {sList1}{modRef.Id}{sList2} {Id} {{ get; set; }}\r\n";
            }
            else if (Id == WoConst.ROWVERSION)
                sProperty = $"public override ulong {Id} {{ get; set; }}\r\n";
            else if ((!Nulo) || (TipoColumna == WoTypeColumn.String))
                sProperty = $"public {OverRide}{sList1}{TipoCS(modelo.Id, EsColeccion)}{sList2} {Id} {{ get; set; }}\r\n";
            else
                sProperty = $"public {OverRide}{sList1}{TipoCS(modelo.Id, EsColeccion)}{sList2} ? {Id} {{ get; set; }}\r\n";

            return ToAppNotDefines("        " + sProperty);
        }


        /// <summary>
        /// Regresa el codigo en C# de la propiedad
        /// </summary>
        /// <param name="modelo"></param>
        /// <param name="ForceNull"></param> // Este parametro obliga a que la propiedad sea nula, es util para identificar 
        ///                                     entre un patch y un put que parametros no se estan enviando (se manda en null)
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ToPropertyDTO(Modelo modelo, bool ForceNull)
        {
            string sProperty = string.Empty;

            if (this.Primaria)
                sProperty = $"public {TipoCS(modelo.Id, EsColeccion)} {Id} {{ get; set; }}\r\n";
            else
            {
                string sList1 = string.Empty;
                string sList2 = string.Empty;
                if (EsColeccion)
                {
                    sList1 = "virtual IList <";
                    sList2 = ">";
                }

                if ((TipoColumna == WoTypeColumn.EnumInt) ||
                         (TipoColumna == WoTypeColumn.EnumString))
                {
                    sProperty = $"public {sList1}{TipoCS((ModeloId.IsNullOrStringEmpty() ? modelo.Id : ModeloId), EsColeccion)}{sList2} {Id} {{ get; set; }}\r\n";
                }
                else if (TipoColumna == WoTypeColumn.Reference)
                {
                    if (EsColeccion)
                    {
                        var modRef = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                        if (modRef == null)
                            throw new Exception($"Modelo no encontrado {ModeloId}");

                        var colloc = modRef.Columnas.Where(g => g.Id == "Id").FirstOrDefault();
                        sProperty = $"public {sList1}{modRef.Id}{sList2} {Id} {{ get; set; }}\r\n";
                    }
                    else
                    {
                        var modRef = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                        if (modRef == null)
                            throw new Exception($"Modelo no encontrado {ModeloId}");

                        var colloc = modRef.Columnas.Where(g => g.Id == "Id").FirstOrDefault();
                        sProperty = $"public {sList1}{colloc.TipoCS(ModeloId, false)}{sList2} {Id} {{ get; set; }}\r\n";
                    }
                }
                else if (this.TipoColumna == WoTypeColumn.Complex)
                {
                    var modRef = proyecto.ModeloCol.Modelos.Where(m => m.Id == ModeloId).FirstOrDefault();

                    if (modRef == null)
                        throw new Exception($"Modelo no encontrado {ModeloId}");

                    var colloc = modRef.Columnas.Where(g => g.Id == "Id").FirstOrDefault();
                    sProperty = $"public {sList1}{modRef.Id}{sList2} {Id} {{ get; set; }}\r\n";
                }
                else if (Id == WoConst.ROWVERSION)
                    sProperty = $"public ulong {Id} {{ get; set; }}\r\n";
                else
                {
                    string IdNul = "";
                    if ((!EsColeccion) && (TipoColumna != WoTypeColumn.String) && (Id != "Id") && (Id != WoConst.WORENGLON) && ((Nulo) || (ForceNull)))
                        IdNul = "?";
                    sProperty = $"public {sList1}{TipoCS(modelo.Id, EsColeccion)}{IdNul}{sList2} {Id} {{ get; set; }}\r\n";
                }
            }

            return ToAppNotDefines("        " + sProperty);
        }

        public string ToDefault()
        {
            return ToAppNotDefines("            " + $"{Id} = {Default};");
        }

        public string ToCollectionDefault(string Transicion)
        {
            if (ModeloId.IsNullOrStringEmpty())
                return string.Empty;

            //ProductoAgpCol = new List<ProductoAgpNuevo>(),
            return ToAppNotDefines("            " + $"{Id} = new List<{ModeloId}{Transicion}>();");
        }



        public string ToAppNotDefines(string Texto)
        {
            if (Apps.Count == 0)
                return Texto;

            StringBuilder sb = new StringBuilder();

            string defs = string.Empty;
            foreach (var app in Apps)
            {
                if (!defs.IsNullOrStringTrimEmpty())
                    defs += " || ";
                defs += "!" + app.AppId.ToUpper();
            }

            sb.AppendLine($"\r\n#if {defs}");
            if (Texto.EndsWith("\r\n"))
                sb.Append(Texto);
            else
                sb.AppendLine(Texto);
            sb.AppendLine("#endif");
            return sb.ToString();
        }



        private string TipoCS(string sModelo, bool Coleccion)
        {
            switch (this.TipoColumna)
            {
                case WoTypeColumn.String:
                    return @"string";

                case WoTypeColumn.Autoincrement:
                    return @"ulong";

                case WoTypeColumn.Smallint:
                    return @"short";

                case WoTypeColumn.EnumInt:
                case WoTypeColumn.EnumString:
                    if ((Coleccion) && (this.Id.Length < 3))
                        throw new Exception($"La columna {this.Id} no tiene el nombre correcto si es una colección");

                    return $"e{sModelo}_{(Coleccion ? this.Id.Substring(0, this.Id.Length - 3) : this.Id)}";
                case WoTypeColumn.Integer:
                    return @"int";

                case WoTypeColumn.Long:
                    return @"long";

                case WoTypeColumn.Boolean:
                    return @"bool";

                case WoTypeColumn.Decimal:
                    return @"decimal";

                case WoTypeColumn.Double:
                    return @"double";

                case WoTypeColumn.Date:
                    return @"DateTime";

                case WoTypeColumn.DateTime:
                    return @"DateTime";

                case WoTypeColumn.Clob:
                    return @"string";

                case WoTypeColumn.Blob:
                    return @"byte[]";

                case WoTypeColumn.WoState:
                    return $"e{sModelo}_WoState";

                case WoTypeColumn.Complex:
                    return $"{this.Id}";
            }

            return string.Empty;
        }

        public string ToEnumInt()
        {
            try
            {
                var prop = JsonConvert.DeserializeObject<TypeControlEnumInt>(this.TipoControlPropiedades);

                string s = string.Empty;
                foreach (var item in prop.Items)
                {
                    if (!s.IsNullOrStringEmpty())
                        s += ",\r\n        ";

                    s += $"[DescriptionAttribute(\"{item.EtiquetaId}\")]";
                    s += $"\r\n        {item.Nombre} = {item.Numero}";
                }
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al tratar de deserealizar las propiedades del enum " + ex.Message);
            }
        }

        public string ToEnumString()
        {
            try
            {
                var prop = JsonConvert.DeserializeObject<TypeControlEnumString>(this.TipoControlPropiedades);

                string s = string.Empty;
                foreach (var item in prop.Items)
                {
                    if (!s.IsNullOrStringEmpty())
                        s += ",\r\n        ";

                    s += $"[DescriptionAttribute(\"{item.EtiquetaId}\")]";
                    s += $"\r\n        {item.Nombre}";
                }
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al tratar de deserializar las propiedades del enum " + ex.Message);
            }
        }

        public string ToForeign(string parModelId, string parModelRefereceId, string parColumnId, string parColumnReferenceId)
        {

            return ToAppNotDefines(
$@"        try
        {{
            db.AddForeignKey <{parModelId}, {parModelRefereceId}> (
                field: t => t.{parColumnId},
                foreignField: tr => tr.{parColumnReferenceId},
                onUpdate: OnFkOption.NoAction,
                onDelete: OnFkOption.NoAction,
                ""FK_{parModelId}_{parModelRefereceId}_{parColumnReferenceId}"");
        }}
        catch (Exception) {{ }};");
        }



    }
}