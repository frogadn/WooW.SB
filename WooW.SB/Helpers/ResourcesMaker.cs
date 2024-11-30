using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ServiceStack;
using ServiceStack.OrmLite;
using WooW.SB.Config;

namespace WooW.SB.Helpers
{
    static class ResourcesMaker
    {
        public enum ResourceSuffix
        {
            None,
            Desc,
            Ayud,
            Grid
        }

        #region Header
        static string sHeader =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name=""resmimetype"">text/microsoft-resx</resheader>
    <resheader name=""version"">2.0</resheader>
    <resheader name=""reader"">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name=""writer"">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name=""Name1""><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name=""Color1"" type=""System.Drawing.Color, System.Drawing"">Blue</data>
    <data name=""Bitmap1"" mimetype=""application/x-microsoft.net.object.binary.base64"">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name=""Icon1"" type=""System.Drawing.Icon, System.Drawing"" mimetype=""application/x-microsoft.net.object.bytearray.base64"">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of ""resheader"" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader> 
";
        #endregion Header


        static private string XmlString(string text)
        {
            if (text.IsNullOrEmpty())
                return String.Empty;

            return new XElement("t", text).LastNode.ToString();
        }

        public static StringBuilder Do(Proyecto proyectoBase, string Idioma, ref int Errores)
        {
            List<string> intrinsic = new List<string>(
                new string[]
                {
                    "Rol",
                    "Menu",
                    "Html",
                    "Reporte",
                    "Reportes",
                    "Respuesta",
                    "Error",
                    "NumeroDeErrores",
                    "Model",
                    "ModelId",
                    "RequestBody",
                    "EventType",
                    "EventDate",
                    "EventDate",
                    "Userauthname"
                }
            );

            Proyecto principal = new Proyecto();
            principal.Load(proyectoBase.ArchivoDeProyecto);

            StringBuilder sbError = new StringBuilder();

            string Dir = principal.DirApplication_Resources;

            string SubFijo = principal.esMX == Idioma ? String.Empty : "." + Idioma.Substring(0, 2);

            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            string DirDestino = $"{Dir}\\Labels";
            if (!Directory.Exists(DirDestino))
                Directory.CreateDirectory(DirDestino);

            DoLabel(principal, DirDestino, SubFijo, Idioma, intrinsic, sbError, ref Errores);

            DirDestino = $"{Dir}\\Messages";
            if (!Directory.Exists(DirDestino))
                Directory.CreateDirectory(DirDestino);

            DoMessage(principal, DirDestino, SubFijo, Idioma, intrinsic, sbError, ref Errores);

            DirDestino = $"{Dir}\\Comments";
            if (!Directory.Exists(DirDestino))
                Directory.CreateDirectory(DirDestino);

            DoComment(principal, DirDestino, SubFijo, Idioma, intrinsic, sbError, ref Errores);

            DirDestino = $"{Dir}\\ModelData";
            if (!Directory.Exists(DirDestino))
                Directory.CreateDirectory(DirDestino);

            var DirDestinoAyuda = $"{Dir}\\ModelComment";
            if (!Directory.Exists(DirDestinoAyuda))
                Directory.CreateDirectory(DirDestinoAyuda);

            DoModelData(
                principal,
                DirDestino,
                DirDestinoAyuda,
                SubFijo,
                Idioma,
                intrinsic,
                sbError,
                ref Errores
            );

            DirDestino = $"{Dir}\\ModelLabel";
            if (!Directory.Exists(DirDestino))
                Directory.CreateDirectory(DirDestino);

            DoModelLabel(principal, DirDestino, SubFijo, Idioma, sbError, ref Errores);

            if (!sbError.ToString().IsNullOrEmpty())
                sbError.AppendLine();

            return sbError;
        }

        public static void DoLabel(
            Proyecto principal,
            string Dir,
            string SubFijo,
            string Idioma,
            List<string> intrinsic,
            StringBuilder sbError,
            ref int Errores
        )
        {
            List<string> Agregados = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append(sHeader);

            bool bError = false;

            foreach (var etiqueta in PackageHelper.GetPackagesEtiquetas(principal))
            {
                var EtiquetaIdioma = etiqueta
                    .Idiomas.Where(e => e.IdiomaId == Idioma)
                    .FirstOrDefault();

                if ((EtiquetaIdioma != null) && (!EtiquetaIdioma.Texto.IsNullOrEmpty()))
                    AgregaNodo(Agregados, sb, etiqueta.Id, EtiquetaIdioma.Texto);
                else
                {
                    if (!bError)
                    {
                        sbError.AppendLine();
                        sbError.AppendLine($"Etiquetas Faltantes para el Idioma {Idioma}");
                    }
                    bError = true;
                    Errores++;
                    sbError.AppendLine($"   {etiqueta.Id}");
                }
            }

            sb.AppendLine("</root>");

            File.WriteAllText($"{Dir}\\WooW_Label{SubFijo}.resx", sb.ToString());
        }

        public static void DoMessage(
            Proyecto principal,
            string Dir,
            string SubFijo,
            string Idioma,
            List<string> intrinsic,
            StringBuilder sbError,
            ref int Errores
        )
        {
            List<string> Agregados = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append(sHeader);

            bool bError = false;

            foreach (var mensaje in PackageHelper.GetPackagesMensaje(principal))
            {
                var EtiquetaIdioma = mensaje
                    .Idiomas.Where(e => e.IdiomaId == Idioma)
                    .FirstOrDefault();

                if ((EtiquetaIdioma != null) && (!EtiquetaIdioma.Texto.IsNullOrEmpty()))
                    AgregaNodo(Agregados, sb, mensaje.Id, EtiquetaIdioma.Texto);
                else
                {
                    if (!bError)
                    {
                        sbError.AppendLine();
                        sbError.AppendLine($"Mensajes Faltantes para el Idioma {Idioma}");
                    }
                    bError = true;
                    Errores++;
                    sbError.AppendLine($"   {mensaje.Id}");
                }
            }

            sb.AppendLine("</root>");

            File.WriteAllText($"{Dir}\\WooW_Message{SubFijo}.resx", sb.ToString());
        }

        public static void DoComment(
            Proyecto principal,
            string Dir,
            string SubFijo,
            string Idioma,
            List<string> intrinsic,
            StringBuilder sbError,
            ref int Errores
        )
        {
            List<string> Agregados = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append(sHeader);

            bool bError = false;

            foreach (var mensaje in PackageHelper.GetPackagesMensaje(principal))
            {
                var EtiquetaIdioma = mensaje
                    .Idiomas.Where(e => e.IdiomaId == Idioma)
                    .FirstOrDefault();

                if ((EtiquetaIdioma != null) && (!EtiquetaIdioma.Texto.IsNullOrEmpty()))
                    AgregaNodo(Agregados, sb, mensaje.Id, EtiquetaIdioma.Solucion);
                else
                {
                    if (!bError)
                    {
                        sbError.AppendLine();
                        sbError.AppendLine($"Mensajes Faltantes para el Idioma {Idioma}");
                    }
                    bError = true;
                    Errores++;
                    sbError.AppendLine($"   {mensaje.Id}");
                }
            }

            sb.AppendLine("</root>");

            File.WriteAllText($"{Dir}\\WooW_Comment{SubFijo}.resx", sb.ToString());
        }

        public static void DoModelData(
            Proyecto principal,
            string Dir,
            string DirAyuda,
            string SubFijo,
            string Idioma,
            List<string> intrinsic,
            StringBuilder sbError,
            ref int Errores
        )
        {
            var Duplicados = new Dictionary<string, Tuple<string, string>>();

            List<string> Agregados = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append(sHeader);

            List<string> AgregadosAyuda = new List<string>();
            StringBuilder sbAyuda = new StringBuilder();
            sbAyuda.Append(sHeader);

            DoModelDataPackage(
                principal,
                Idioma,
                Duplicados,
                Agregados,
                sb,
                AgregadosAyuda,
                sbAyuda,
                sbError,
                ref Errores
            );

            sb.AppendLine("</root>");
            sbAyuda.AppendLine("</root>");

            File.WriteAllText($"{Dir}\\WooW_ModelData{SubFijo}.resx", sb.ToString());
            File.WriteAllText($"{DirAyuda}\\WooW_ModelComment{SubFijo}.resx", sbAyuda.ToString());
        }

        public static void DoModelDataPackage(
            Proyecto principal,
            string Idioma,
            Dictionary<string, Tuple<string, string>> Duplicados,
            List<string> Agregados,
            StringBuilder sb,
            List<string> AgregadosAyuda,
            StringBuilder sbAyuda,
            StringBuilder sbError,
            ref int Errores
        )
        {
            var TradActual = principal.GetDiccAyudaTraduccion(Idioma);

            foreach (var modelo in principal.ModeloCol.Modelos)
            {
                bool bError = false;

                foreach (var col in modelo.Columnas)
                {
                    bool Faltante = false;

                    if (!col.Descripcion.IsNullOrEmpty())
                    {
                        var Id = Etiqueta.ToId(col.Descripcion);

                        bool bFound = false;

                        if (Duplicados.ContainsKey(Id))
                        {
                            for (int i = 1; ; i++)
                            {
                                var Valor = new Tuple<string, string>(string.Empty, string.Empty);
                                Duplicados.TryGetValue(Id, out Valor);

                                if (Valor.Item2 == col.Ayuda)
                                {
                                    col.DescripcionEtiqueta = Id;
                                    bFound = true;
                                    break;
                                }

                                Id = Etiqueta.ToId(col.Descripcion) + i.ToString();

                                if (!Duplicados.ContainsKey(Id))
                                {
                                    Duplicados.Add(Id, Tuple.Create(col.Descripcion, col.Ayuda));
                                    break;
                                }
                            }
                        }
                        else
                            Duplicados.Add(Id, Tuple.Create(col.Descripcion, col.Ayuda));

                        if (!bFound)
                        {
                            col.DescripcionEtiqueta = Id;

                            string Descripcion = col.Descripcion;
                            string Ayuda = col.Ayuda;

                            if (Idioma != principal.esMX)
                            {
                                Tuple<string, string> trad = Tuple.Create(
                                    string.Empty,
                                    string.Empty
                                );

                                if (TradActual.ContainsKey(Id))
                                {
                                    TradActual.TryGetValue(Id, out trad);
                                    Descripcion = trad.Item1;
                                    Ayuda = trad.Item2;
                                }
                                else
                                    Faltante = true;
                            }

                            if (!Faltante)
                            {
                                //if (Ayuda.IsNullOrEmpty())
                                AgregaNodo(Agregados, sb, Id, Descripcion);
                                AgregaNodo(AgregadosAyuda, sbAyuda, Id, Ayuda);
                                //else
                                //    AgregaNodo(sb, Id, Descripcion, Ayuda);
                            }
                        }
                    }
                    else
                    {
                        if (!bError)
                        {
                            sbError.AppendLine();
                            sbError.AppendLine(
                                $"Etiqueta descripción faltante para el Modelo {modelo.Id}, Idioma {Idioma}"
                            );
                        }
                        bError = true;
                        Errores++;
                        sbError.AppendLine($"   Columna {col.Id}");
                    }
                }
            }

            foreach (var package in principal.Paquetes)
            {
                string pathReal = Proyecto.ArchivoPaquete(
                    Path.GetFullPath(principal.ArchivoDeProyecto),
                    package.Archivo
                );
                Proyecto local = new Proyecto();
                local.Load(pathReal);

                DoModelDataPackage(
                    local,
                    Idioma,
                    Duplicados,
                    Agregados,
                    sb,
                    AgregadosAyuda,
                    sbAyuda,
                    sbError,
                    ref Errores
                );
            }
        }

        public static void DoModelLabel(
            Proyecto principal,
            string Dir,
            string SubFijo,
            string Idioma,
            StringBuilder sbError,
            ref int Errores
        )
        {
            List<string> Agregados = new List<string>();
            List<string> Duplicados = new List<string>();

            StringBuilder sb = new StringBuilder();
            sb.Append(sHeader);

            DoModelLabelPackage(principal, Idioma, Agregados, Duplicados, sb, sbError, ref Errores);

            //var TradActual = principal.GetDiccTraduccion(Idioma);

            //foreach (var modelo in principal.ModeloCol.Modelos)
            //{
            //    bool bError = false;

            //    foreach (var col in modelo.Columnas)
            //    {
            //        bool Faltante = false;

            //        if (!col.Formulario.IsNullOrEmpty())
            //        {
            //            string Id = Etiqueta.ToId(col.Formulario);
            //            col.FormularioEtiqueta = Id;

            //            if (Duplicados.IndexOf(Id) == -1)
            //            {
            //                Duplicados.Add(Id);

            //                string Texto = col.Formulario;

            //                if (Idioma != principal.esMX)
            //                {
            //                    Tuple<string, string> trad = Tuple.Create(
            //                        string.Empty,
            //                        string.Empty
            //                    );

            //                    if (TradActual.ContainsKey(Id))
            //                    {
            //                        TradActual.TryGetValue(Id, out trad);
            //                        Texto = trad.Item1;
            //                    }
            //                    else
            //                        Faltante = true;
            //                }

            //                AgregaNodo(Agregados, sb, Id, Texto);
            //            }
            //        }
            //        else
            //            Faltante = true;

            //        if (!col.Grid.IsNullOrEmpty())
            //        {
            //            string Id = Etiqueta.ToId(col.Grid);
            //            col.GridEtiqueta = Id;

            //            if (Duplicados.IndexOf(Id) == -1)
            //            {
            //                Duplicados.Add(Id);

            //                string Texto = col.Grid;

            //                if (Idioma != principal.esMX)
            //                {
            //                    Tuple<string, string> trad = Tuple.Create(
            //                        string.Empty,
            //                        string.Empty
            //                    );
            //                    if (TradActual.ContainsKey(Id))
            //                    {
            //                        TradActual.TryGetValue(Id, out trad);
            //                        Texto = trad.Item1;
            //                    }
            //                    else
            //                        Faltante = true;
            //                }

            //                AgregaNodo(Agregados, sb, Id, Texto);
            //            }
            //        }
            //        else
            //            Faltante = true;

            //        if (Faltante)
            //        {
            //            if (!bError)
            //            {
            //                sbError.AppendLine();
            //                sbError.AppendLine(
            //                    $"Etiqueta faltante para el Modelo {modelo.Id}, Idioma {Idioma}"
            //                );
            //            }
            //            bError = true;
            //            Errores++;
            //            sbError.AppendLine($"   Columna {col.Id}");
            //        }
            //    }
            //}

            sb.AppendLine("</root>");

            File.WriteAllText($"{Dir}\\WooW_ModelLabel{SubFijo}.resx", sb.ToString());
        }

        public static void DoModelLabelPackage(
            Proyecto principal,
            string Idioma,
            List<string> Agregados,
            List<string> Duplicados,
            StringBuilder sb,
            StringBuilder sbError,
            ref int Errores
        )
        {
            var TradActual = principal.GetDiccTraduccion(Idioma);

            foreach (var modelo in principal.ModeloCol.Modelos)
            {
                bool bError = false;

                foreach (var col in modelo.Columnas)
                {
                    bool Faltante = false;

                    if (!col.Formulario.IsNullOrEmpty())
                    {
                        string Id = Etiqueta.ToId(col.Formulario);
                        col.FormularioEtiqueta = Id;

                        if (Duplicados.IndexOf(Id) == -1)
                        {
                            Duplicados.Add(Id);

                            string Texto = col.Formulario;

                            if (Idioma != principal.esMX)
                            {
                                Tuple<string, string> trad = Tuple.Create(
                                    string.Empty,
                                    string.Empty
                                );

                                if (TradActual.ContainsKey(Id))
                                {
                                    TradActual.TryGetValue(Id, out trad);
                                    Texto = trad.Item1;
                                }
                                else
                                    Faltante = true;
                            }

                            AgregaNodo(Agregados, sb, Id, Texto);
                        }
                    }
                    else
                        Faltante = true;

                    if (!col.Grid.IsNullOrEmpty())
                    {
                        string Id = Etiqueta.ToId(col.Grid);
                        col.GridEtiqueta = Id;

                        if (Duplicados.IndexOf(Id) == -1)
                        {
                            Duplicados.Add(Id);

                            string Texto = col.Grid;

                            if (Idioma != principal.esMX)
                            {
                                Tuple<string, string> trad = Tuple.Create(
                                    string.Empty,
                                    string.Empty
                                );
                                if (TradActual.ContainsKey(Id))
                                {
                                    TradActual.TryGetValue(Id, out trad);
                                    Texto = trad.Item1;
                                }
                                else
                                    Faltante = true;
                            }

                            AgregaNodo(Agregados, sb, Id, Texto);
                        }
                    }
                    else
                        Faltante = true;

                    if (Faltante)
                    {
                        if (!bError)
                        {
                            sbError.AppendLine();
                            sbError.AppendLine(
                                $"Etiqueta faltante para el Modelo {modelo.Id}, Idioma {Idioma}"
                            );
                        }
                        bError = true;
                        Errores++;
                        sbError.AppendLine($"   Columna {col.Id}");
                    }
                }
            }

            foreach (var package in principal.Paquetes)
            {
                string pathReal = Proyecto.ArchivoPaquete(
                    Path.GetFullPath(principal.ArchivoDeProyecto),
                    package.Archivo
                );
                Proyecto local = new Proyecto();
                local.Load(pathReal);

                DoModelLabelPackage(local, Idioma, Agregados, Duplicados, sb, sbError, ref Errores);
            }
        }

        private static void AgregaNodo(
            List<string> Agregados,
            StringBuilder sb,
            string data,
            string value
        )
        {
            if (Agregados.IndexOf(data) == -1)
            {
                sb.AppendLine($"  <data name=\"{data}\" xml:space=\"preserve\">");
                sb.AppendLine($"    <value>{ResourcesMaker.XmlString(value)}</value>");
                sb.AppendLine("  </data>");
            }
        }

        //private static void AgregaNodo(
        //    StringBuilder sb,
        //    ResourceSuffix sub,
        //    string data,
        //    string value,
        //    string comment
        //)
        //{
        //    string subfijo = sub == ResourceSuffix.None ? string.Empty : sub.ToString();

        //    sb.AppendLine($"  <data name=\"{data}{subfijo}\" xml:space=\"preserve\">");
        //    sb.AppendLine($"    <value>{ResourcesMaker.XmlString(value)}</value>");
        //    sb.AppendLine($"    <comment>{ResourcesMaker.XmlString(comment)}</comment>");
        //    sb.AppendLine("  </data>");
        //}

        public static Traduccion GetModelTraduccion(Proyecto principal, string Idioma)
        {
            var t = new Traduccion();

            t.IdiomaId = Idioma;
            t.Items = new List<TraduccionItem>();

            List<string> Duplicados = new List<string>();

            var TradActual = principal.GetDiccTraduccion(Idioma);

            foreach (var modelo in principal.ModeloCol.Modelos)
            {
                foreach (var col in modelo.Columnas)
                {
                    if (!col.Formulario.IsNullOrEmpty())
                    {
                        string Id = Etiqueta.ToId(col.Formulario);
                        col.FormularioEtiqueta = Id;

                        if (Duplicados.IndexOf(Id) == -1)
                        {
                            Duplicados.Add(Id);
                            Tuple<string, string> trad = Tuple.Create(string.Empty, string.Empty);
                            if (TradActual.ContainsKey(Id))
                                TradActual.TryGetValue(Id, out trad);

                            var item = new TraduccionItem()
                            {
                                Etiqueta = Id,
                                Valor = col.Formulario,
                                ValorAyuda = string.Empty,
                                Traduccion = trad.Item1,
                                Ayuda = trad.Item2
                            };

                            t.Items.Add(item);
                        }
                    }

                    if (!col.Grid.IsNullOrEmpty())
                    {
                        string Id = Etiqueta.ToId(col.Grid);
                        col.GridEtiqueta = Id;

                        if (Duplicados.IndexOf(Id) == -1)
                        {
                            Duplicados.Add(Id);
                            Tuple<string, string> trad = Tuple.Create(string.Empty, string.Empty);
                            if (TradActual.ContainsKey(Id))
                                TradActual.TryGetValue(Id, out trad);
                            var item = new TraduccionItem()
                            {
                                Etiqueta = Id,
                                Valor = col.Grid,
                                ValorAyuda = string.Empty,
                                Traduccion = trad.Item1,
                                Ayuda = trad.Item2
                            };

                            t.Items.Add(item);
                        }
                    }
                }
            }

            return t;
        }

        public static Traduccion GetModelAyudaTraduccion(Proyecto principal, string Idioma)
        {
            var t = new Traduccion();

            t.IdiomaId = Idioma;
            t.Items = new List<TraduccionItem>();

            var Duplicados = new Dictionary<string, Tuple<string, string>>();

            var TradActual = principal.GetDiccAyudaTraduccion(Idioma);

            foreach (var modelo in principal.ModeloCol.Modelos)
            {
                foreach (var col in modelo.Columnas)
                {
                    if (!col.Formulario.IsNullOrEmpty())
                    {
                        string Id = Etiqueta.ToId(col.Descripcion);
                        col.FormularioEtiqueta = Id;

                        bool bFound = false;

                        if (Duplicados.ContainsKey(Id))
                        {
                            for (int i = 1; ; i++)
                            {
                                var Valor = new Tuple<string, string>(string.Empty, string.Empty);
                                Duplicados.TryGetValue(Id, out Valor);

                                if (Valor.Item2 == col.Ayuda)
                                {
                                    col.DescripcionEtiqueta = Id;
                                    bFound = true;
                                    break;
                                }

                                Id = Etiqueta.ToId(col.Descripcion) + i.ToString();

                                if (!Duplicados.ContainsKey(Id))
                                {
                                    Duplicados.Add(Id, Tuple.Create(col.Descripcion, col.Ayuda));
                                    break;
                                }
                            }
                        }
                        else
                            Duplicados.Add(Id, Tuple.Create(col.Descripcion, col.Ayuda));

                        if (!bFound)
                        {
                            Tuple<string, string> trad = Tuple.Create(string.Empty, string.Empty);
                            if (TradActual.ContainsKey(Id))
                                TradActual.TryGetValue(Id, out trad);

                            var item = new TraduccionItem()
                            {
                                Etiqueta = Id,
                                Valor = col.Descripcion,
                                ValorAyuda = col.Ayuda,
                                Traduccion = trad.Item1,
                                Ayuda = trad.Item2
                            };

                            t.Items.Add(item);
                        }
                    }
                }
            }

            return t;
        }
    }
}
