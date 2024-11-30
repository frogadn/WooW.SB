using Newtonsoft.Json;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using WooW.Core;
using WooW.SB.Config.Class;
using WooW.SB.Config.Enum;


// Todo Documentar

namespace WooW.SB.Config
{
    public class Modelo
    {
        public enum tDiagramaTipo { NoAplica, Extender, Remplazar };

        private Proyecto proyecto;
        private string id;
        private string etiquetaId;
        private string procesoId;
        private string legacy;
        private string interface1;
        private string interface2;
        private string interface3;

        private bool esPaquete = false;

        public Modelo()
        {
            Id = String.Empty;
            EtiquetaId = String.Empty;
            OrdenDeCreacion = -1;
            ProcesoId = String.Empty;
            TipoModelo = WoTypeModel.Catalog;
            SubTipoModelo = WoSubTypeModel.StateDiagram;
            Repositorio = TypeRepository.User;

            Columnas = new List<ModeloColumna>();
            Apps = new List<ModeloApp>();
            Legacy = String.Empty;
            Interface1 = String.Empty;
            Interface2 = String.Empty;
            Interface3 = String.Empty;
            DiagramaTipo = tDiagramaTipo.NoAplica;
            Diagrama = new ModeloDiagrama();

            ScriptRequest = new ModeloScriptRequest();
            ScriptVistaRoles = new ModeloScriptVistaRoles();
            proyecto = null;
            FechaActualizacion = DateTime.MinValue;

            EsPaqueteExterno = false;
            DirectorioPaquete = string.Empty;
        }

        public Modelo(Proyecto _proyecto) : this()
        {
            proyecto = _proyecto;
            Diagrama.ProyectoSetter(proyecto);
        }

        public void ProyectoSetter(Proyecto _proyecto)
        {
            proyecto = _proyecto;
            foreach (var c in Columnas)
            {
                c.ProyectoSetter(proyecto);
            }

            Diagrama.ProyectoSetter(proyecto);
        }


        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        [Description(@"Etiqueta"), DisplayName("Etiqueta"),
            EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string EtiquetaId { get => etiquetaId; set => etiquetaId = (value == null ? string.Empty : value.Trim()); }

        public int OrdenDeCreacion { get; set; }

        public string ProcesoId { get => procesoId; set => procesoId = (value == null ? string.Empty : value.Trim()); }
        public WoTypeModel TipoModelo { get; set; }
        public WoSubTypeModel SubTipoModelo { get; set; }

        public woGetListFilterType Filtro { get; set; }

        public TypeRepository Repositorio { get; set; } = TypeRepository.User;

        public List<ModeloColumna> Columnas { get; set; }
        public List<ModeloApp> Apps { get; set; }
        public string Legacy { get => legacy; set => legacy = (value == null ? string.Empty : value.Trim()); }

        public string Interface1 { get => interface1; set => interface1 = (value == null ? string.Empty : value.Trim()); }
        public string Interface2 { get => interface2; set => interface2 = (value == null ? string.Empty : value.Trim()); }
        public string Interface3 { get => interface3; set => interface3 = (value == null ? string.Empty : value.Trim()); }

        [JsonIgnore]
        [BrowsableAttribute(false)]
        public bool EsPaqueteExterno { get; set; }
        [JsonIgnore]
        [BrowsableAttribute(false)]
        public string DirectorioPaquete { get; set; }

        [BrowsableAttribute(false)]
        public ModeloDiagrama Diagrama { get; set; }

        [BrowsableAttribute(false)]
        public tDiagramaTipo DiagramaTipo { get; set; }

        public ModeloScriptRequest ScriptRequest { get; set; }

        public ModeloScriptVistaRoles ScriptVistaRoles { get; set; }


        public override string ToString()
        {
            return $"{Id}-{EtiquetaCol.Get(EtiquetaId)}";
        }

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

        public static Modelo FromJson(string Json, Proyecto _proyecto)
        {
            var model = JsonConvert.DeserializeObject<Modelo>(Json);
            model.ProyectoSetter(_proyecto);

            return model;
        }

        public Modelo Clone(Proyecto _proyecto)
        {
            var model = JsonConvert.DeserializeObject<Modelo>(JsonConvert.SerializeObject(this));
            model.ProyectoSetter(_proyecto);

            return model;
        }

        public string GetRolesCSV()
        {
            string sRoles = string.Empty;
            foreach (var rol in Diagrama.Roles)
            {
                if (!sRoles.IsNullOrStringTrimEmpty())
                    sRoles += ", ";
                sRoles += rol.RolId;
            }

            return sRoles;
        }

        public string GetOtrosRolesCSV()
        {
            string sRoles = string.Empty;
            foreach (Transicion tra in this.Diagrama.Transiciones)
            {
                if (tra.Roles.Count > 0)
                {
                    if (!sRoles.IsNullOrStringTrimEmpty())
                        sRoles += " ";

                    sRoles += tra.Id + ": ";

                    foreach (var rol in tra.Roles)
                    {
                        if (!sRoles.EndsWith(": "))
                            sRoles += ", ";
                        sRoles += rol.RolId;
                    }
                }
            }

            return sRoles;
        }


        public string GetRoles()
        {
            if ((Diagrama.Roles.Count == 0) || (Diagrama.Roles.Where(e => e.RolId == Proyecto.NOVALIDAR).FirstOrDefault() != null))
                return string.Empty;

            string sRoles = string.Empty;
            foreach (var rol in Diagrama.Roles)
            {
                if (!sRoles.IsNullOrStringTrimEmpty())
                    sRoles += ", ";
                sRoles += $"nameof(Rol.{rol.RolId})";
            }

            if (sRoles.IsNullOrStringTrimEmpty())
                throw new Exception($"faltan indicar roles");

            return $"\r\n    [RequiresAnyRole({sRoles})]";
        }

        public string GetRolesList()
        {

            List<string> listRoles = new List<string>();

            foreach (Transicion tra in this.Diagrama.Transiciones)
            {
                foreach (var rol in tra.Roles)
                {
                    if (listRoles.IndexOf(rol.RolId) == -1)
                        listRoles.Add(rol.RolId);
                }
            }

            foreach (var rol in Diagrama.Roles)
            {
                if (listRoles.IndexOf(rol.RolId) == -1)
                    listRoles.Add(rol.RolId);
            }

            foreach (var rol in Diagrama.RolesLectura)
            {
                if (listRoles.IndexOf(rol.RolId) == -1)
                    listRoles.Add(rol.RolId);
            }

            if ((listRoles.Count == 0) || (listRoles.Where(e => e == Proyecto.NOVALIDAR).FirstOrDefault() != null))
                return string.Empty;

            string sRoles = string.Empty;
            foreach (var rol in listRoles)
            {
                if (!sRoles.IsNullOrStringTrimEmpty())
                    sRoles += ", ";
                sRoles += $"nameof(Rol.{rol})";
            }

            if (sRoles.IsNullOrStringTrimEmpty())
                throw new Exception($"faltan indicar roles");

            return $"\r\n    [RequiresAnyRole({sRoles})]";
        }

        public string GetRolesTra(Transicion tra)
        {
            string sRoles = string.Empty;

            if (tra.Roles.Count > 0)
            {
                if (tra.Roles.Where(e => e.RolId == Proyecto.NOVALIDAR).FirstOrDefault() != null)
                    return string.Empty;

                foreach (var rol in tra.Roles)
                {
                    if (!sRoles.IsNullOrStringTrimEmpty())
                        sRoles += ", ";
                    sRoles += $"nameof(Rol.{rol.RolId})";
                }

            }
            else
            {
                if ((Diagrama.Roles.Count == 0) || (Diagrama.Roles.Where(e => e.RolId == Proyecto.NOVALIDAR).FirstOrDefault() != null))
                    return string.Empty;

                foreach (var rol in Diagrama.Roles)
                {
                    if (!sRoles.IsNullOrStringTrimEmpty())
                        sRoles += ", ";
                    sRoles += $"nameof(Rol.{rol.RolId})";
                }
            }

            if (sRoles.IsNullOrStringTrimEmpty())
                throw new Exception($"faltan indicar roles");

            return $"\r\n    [RequiresAnyRole({sRoles})]";
        }

        public string GetRequestRoles()
        {

            if ((this.ScriptVistaRoles.Roles.Count == 0) || (this.ScriptVistaRoles.Roles.Where(e => e == Proyecto.NOVALIDAR).FirstOrDefault() != null))
                return string.Empty;

            string sRoles = string.Empty;
            foreach (var rol in this.ScriptVistaRoles.Roles)
            {
                if (!sRoles.IsNullOrStringTrimEmpty())
                    sRoles += ", ";
                sRoles += $"nameof(Rol.{rol})";
            }

            if (sRoles.IsNullOrStringTrimEmpty())
                throw new Exception($"faltan indicar roles en el modelo");

            return $"\r\n    [RequiresAnyRole({sRoles})]";
        }

        public string GetRequestPermisos()
        {
            if ((this.ScriptVistaRoles.Permisos.Count == 0) || (this.ScriptVistaRoles.Permisos.Where(e => e == Proyecto.NOVALIDAR).FirstOrDefault() != null))
                return string.Empty;

            string sPermisos = string.Empty;
            foreach (var permiso in this.ScriptVistaRoles.Permisos)
            {
                if (!sPermisos.IsNullOrStringTrimEmpty())
                    sPermisos += ", ";
                sPermisos += $"nameof(Permiso.{permiso})";
            }

            if (sPermisos.IsNullOrStringTrimEmpty())
                throw new Exception($"faltan indicar permisos en el modelo");

            return $"\r\n    [RequiresAnyPermission({sPermisos})]";
        }

        public string GetModelNameExtensionOverride()
        {
            if ((SubTipoModelo == WoSubTypeModel.Extension) ||
                (SubTipoModelo == WoSubTypeModel.Override))
                return "_" + proyecto.GetNameExtension();
            else
                return string.Empty;
        }


        [JsonIgnore]
        public string PreConditionsFile
        {
            get
            {
                return (EsPaqueteExterno ?
                    $"{DirectorioPaquete}\\ProyectData\\Logic\\PreConditions\\{Id}PreConditions{GetModelNameExtensionOverride()}.cs" :
                    $"{proyecto.DirProyectDataLogicPreConditions}\\{Id}PreConditions{GetModelNameExtensionOverride()}.cs");
            }
        }

        [JsonIgnore]
        public string PostConditionsFile
        {
            get
            {
                return (EsPaqueteExterno ?
                    $"{DirectorioPaquete}\\ProyectData\\Logic\\PostConditions\\{Id}PostConditions{GetModelNameExtensionOverride()}.cs" :
                    $"{proyecto.DirProyectDataLogicPostConditions}\\{Id}PostConditions{GetModelNameExtensionOverride()}.cs");
            }
        }

        [JsonIgnore]
        public string ModelPolizaFile
        {
            get
            {
                return (EsPaqueteExterno ?
                    $"{DirectorioPaquete}\\ProyectData\\Logic\\ModelPoliza\\{Id}ModelPoliza{GetModelNameExtensionOverride()}.cs" :
                    $"{proyecto.DirProyectDataLogicModelPoliza}\\{Id}ModelPoliza{GetModelNameExtensionOverride()}.cs");
            }
        }


        [JsonIgnore]
        public string ModelCreationFile
        {
            get
            {
                return (EsPaqueteExterno ?
                    $"{DirectorioPaquete}\\ProyectData\\Logic\\ModelCreation\\{Id}ModelCreation{GetModelNameExtensionOverride()}.cs" :
                    $"{proyecto.DirProyectDataLogicModelCreation}\\{Id}ModelCreation{GetModelNameExtensionOverride()}.cs");
            }
        }


        [JsonIgnore]
        public string ScriptFile
        {
            get
            {
                return (EsPaqueteExterno ?
                    $"{DirectorioPaquete}\\ProyectData\\Logic\\Script\\{Id}Script{GetModelNameExtensionOverride()}.cs" :
                    $"{proyecto.DirProyectDataLogicScripts}\\{Id}Script{GetModelNameExtensionOverride()}.cs");
            }
        }

        public List<Tuple<string, string>> ListPreConditionsFiles()
        {
            var list = new List<Tuple<string, string>>();

            foreach (var dir in this.proyecto.DirectorioDeLosPaquetes)
            {
                var path = $"{dir.Item2}\\Logic\\PreConditions\\{Id}PreConditions.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>(string.Empty, path));

                path = $"{dir.Item2}\\Logic\\PreConditions\\{Id}PreConditions_{dir.Item1}.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>("_" + dir.Item1, path));
            }

            list.Reverse();

            return list;
        }

        public List<Tuple<string, string>> ListPostConditionsFiles()
        {
            var list = new List<Tuple<string, string>>();

            foreach (var dir in this.proyecto.DirectorioDeLosPaquetes)
            {
                var path = $"{dir.Item2}\\Logic\\PostConditions\\{Id}PostConditions.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>(string.Empty, path));

                path = $"{dir.Item2}\\Logic\\PostConditions\\{Id}PostConditions_{dir.Item1}.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>("_" + dir.Item1, path));
            }

            list.Reverse();

            return list;
        }

        public List<Tuple<string, string>> ListModelCreationFiles()
        {
            var list = new List<Tuple<string, string>>();

            foreach (var dir in this.proyecto.DirectorioDeLosPaquetes)
            {
                var path = $"{dir.Item2}\\Logic\\ModelCreation\\{Id}ModelCreation.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>(string.Empty, path));

                path = $"{dir.Item2}\\Logic\\ModelCreation\\{Id}ModelCreation_{dir.Item1}.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>("_" + dir.Item1, path));
            }

            list.Reverse();

            return list;
        }

        public List<Tuple<string, string>> ListModelPolizaFiles()
        {
            var list = new List<Tuple<string, string>>();

            foreach (var dir in this.proyecto.DirectorioDeLosPaquetes)
            {
                var path = $"{dir.Item2}\\Logic\\ModelPoliza\\{Id}ModelPoliza.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>(string.Empty, path));

                path = $"{dir.Item2}\\Logic\\ModelPoliza\\{Id}ModelPoliza_{dir.Item1}.cs";
                if (File.Exists(path))
                    list.Add(new Tuple<string, string>("_" + dir.Item1, path));

            }

            return list;
        }



        [JsonIgnore]
        public bool bPreCondicionesExist
        { get { return File.Exists(PreConditionsFile); } }

        [JsonIgnore]
        public bool bPostCondicionesExist
        { get { return File.Exists(PostConditionsFile); } }

        [JsonIgnore]
        public bool bModelPolizaExist
        { get { return File.Exists(ModelPolizaFile); } }

        [JsonIgnore]
        public bool bModelCreationExist
        { get { return File.Exists(ModelCreationFile); } }

        [JsonIgnore]
        public bool bScriptExist
        { get { return File.Exists(ScriptFile); } }

        [JsonIgnore]
        public bool bScriptExistAnyPackage
        {
            get
            {
                if (File.Exists(ScriptFile))
                    return true;

                foreach (var paquete in proyecto.Paquetes)
                {
                    if (VerificaScripPaquete(this.proyecto, paquete.Archivo, Path.GetFileName(ScriptFile)))
                        return true;
                }

                return false;
            }
        }

        public bool VerificaScripPaquete(Proyecto principal, string Paquete, string ScriptFile)
        {
            string pathReal = Proyecto.ArchivoPaquete(principal.DirApplication, Paquete);

            Proyecto local = new Proyecto();
            local.Load(pathReal);

            var locScriptFile = Path.Combine(local.DirProyectDataLogicScripts, ScriptFile);

            if (File.Exists(locScriptFile))
                return true;

            foreach (var paquete in local.Paquetes)
            {
                if (VerificaScripPaquete(local, paquete.Archivo, ScriptFile))
                    return true;
            }

            return false;
        }

        [JsonIgnore]
        public Proyecto Proyecto { get => proyecto; set => proyecto = value; }
        public DateTime FechaActualizacion { get; set; }

        public bool IsInTheApp(string AppId)
        {
            return Apps.Where(e => e.AppId == AppId).FirstOrDefault() != null;
        }

        public string GetPreCondiciones()
        {
            if (!bPreCondicionesExist)
                return string.Empty;

            return File.ReadAllText(PreConditionsFile);
        }

        public string GetPostCondiciones()
        {
            if (!bPostCondicionesExist)
                return string.Empty;

            return File.ReadAllText(PostConditionsFile);
        }

        public string GetModelPoliza()
        {
            if (!bModelPolizaExist)
                return string.Empty;

            return File.ReadAllText(ModelPolizaFile);
        }

        public string GetModelCreation()
        {
            if (!bModelCreationExist)
                return string.Empty;

            return File.ReadAllText(ModelCreationFile);
        }

        public string GetScript()
        {
            if (!bScriptExist)
                return string.Empty;

            return File.ReadAllText(ScriptFile);
        }

        public void SavePreCondiciones(string Text)
        {
            File.WriteAllText(PreConditionsFile, Text);
        }

        public void SavePostCondiciones(string Text)
        {
            File.WriteAllText(PostConditionsFile, Text);
        }

        public void SaveModelPoliza(string Text)
        {
            File.WriteAllText(ModelPolizaFile, Text);
        }

        public void SaveModelCreation(string Text)
        {
            File.WriteAllText(ModelCreationFile, Text);
        }

        public void SaveScript(string Text)
        {
            File.WriteAllText(ScriptFile, Text);
        }

        public void DeletePreCondiciones()
        {
            if (File.Exists(PreConditionsFile))
                File.Delete(PreConditionsFile);
        }

        public void DeletePostCondiciones()
        {
            if (File.Exists(PostConditionsFile))
                File.Delete(PostConditionsFile);
        }

        public void DeleteModelPoliza()
        {
            if (File.Exists(ModelPolizaFile))
                File.Delete(ModelPolizaFile);
        }

        public void DeleteModelCreation()
        {
            if (File.Exists(ModelCreationFile))
                File.Delete(ModelCreationFile);
        }

        public void DeleteScript()
        {
            if (File.Exists(ScriptFile))
                File.Delete(ScriptFile);
        }

        public string ToAttributes(bool bClient)
        {
            return ToAttributes(bClient, string.Empty);
        }

        public string ToAppDefines(string Texto)
        {
            StringBuilder sb = new StringBuilder();

            string defs = string.Empty;
            foreach (var app in Apps)
            {
                if (!defs.IsNullOrStringTrimEmpty())
                    defs += " || ";
                defs += app.AppId.ToUpper();
            }

            sb.AppendLine($"#if {defs}");
            sb.AppendLine(Texto);
            sb.AppendLine("#endif");
            return sb.ToString();
        }

        public string ToAttributes(bool bClient, string ServiceSubFix)
        {

            string Attributes = string.Empty;

            bool bUniqueGet = (this.Columnas.Where(e => e.TipoDato == WoTypeData.UniqueGet).FirstOrDefault() != null);
            if (bUniqueGet)
            {
                string CompositeIndex = "    [CompositeIndex(";
                bool bInit = true;
                foreach (var col in Columnas.Where(e => e.TipoDato == WoTypeData.UniqueGet).OrderBy(e => e.Orden).ToList())
                {
                    if (!bInit)
                        CompositeIndex += ", ";
                    bInit = false;
                    CompositeIndex += $"\"{col.Id}\"";
                }

                CompositeIndex += $", Name = \"uidx_{this.id}_wwuk\", Unique = true)]";
                Attributes += CompositeIndex;
            }


            if (TipoModelo == WoTypeModel.Request)
            {
                Attributes += $"\r\n    [ValidateIsAuthenticated]";
                Attributes += $"\r\n    #if SERVER";
                Attributes += this.GetRequestRoles();
                Attributes += $"\r\n    [AutoApply(Behavior.AuditQuery)]";
                Attributes += this.GetRequestPermisos();
                Attributes += $"\r\n    #endif";
                Attributes += $"\r\n    [Tag(\"{this.Id}s\")]";

                string Servicos = string.Empty;
                if (this.ScriptRequest.UsaGetEnRequest)
                    Servicos += "GET";
                if (this.ScriptRequest.UsaPostEnRequest)
                {
                    if (!Servicos.IsNullOrStringEmpty())
                        Servicos += ",";
                    Servicos += "POST";
                }
                if (this.ScriptRequest.UsaPutEnRequest)
                {
                    if (!Servicos.IsNullOrStringEmpty())
                        Servicos += ",";
                    Servicos += "PUT";
                }
                if (this.ScriptRequest.UsaPatchEnRequest)
                {
                    if (!Servicos.IsNullOrStringEmpty())
                        Servicos += ",";
                    Servicos += "PATCH";
                }
                if (this.ScriptRequest.UsaDeleteEnRequest)
                {
                    if (!Servicos.IsNullOrStringEmpty())
                        Servicos += ",";
                    Servicos += "DELETE";
                }

                Attributes += $"\r\n    [Route(\"/api/{this.Id}{ServiceSubFix}\", \"{Servicos}\")]";
            }

            if (!Attributes.IsNullOrStringEmpty())
                Attributes += "\r\n";

            Attributes += $@"    [WoLabel(Descripcion = nameof(WooW_Label.{EtiquetaId}))]";
            Attributes += "\r\n";
            Attributes +=
$@"    [WoModelMeta(
        ProcesoId = ""{this.ProcesoId}"",
        TipoModelo = WoTypeModel.{this.TipoModelo.ToString()},
        Filtro = woGetListFilterType.{this.Filtro.ToString()},
        Legacy  = ""{this.Legacy}"")]";
            Attributes += "\r\n";
            Attributes += $@"    [JsonObject(MemberSerialization.OptOut)]";


            // Revisar en todos los modelos donde se referencia agrega los attributos de referencia
            foreach (Modelo modelo in proyecto.ModeloCol.Modelos.Where(e => e.Id != this.Id))
            {
                if (
                    (modelo.TipoModelo == WoTypeModel.Configuration)
                    || (modelo.TipoModelo == WoTypeModel.CatalogType)
                    || (modelo.TipoModelo == WoTypeModel.Catalog)
                    || (modelo.TipoModelo == WoTypeModel.CatalogSlave)
                    || (modelo.TipoModelo == WoTypeModel.TransactionContable)
                    || (modelo.TipoModelo == WoTypeModel.TransactionNoContable)
                    || (modelo.TipoModelo == WoTypeModel.TransactionSlave)
                    || (modelo.TipoModelo == WoTypeModel.TransactionFreeStyle)
                    || (modelo.TipoModelo == WoTypeModel.Control)
                    || (modelo.TipoModelo == WoTypeModel.Kardex)
                    || (modelo.TipoModelo == WoTypeModel.DataMart)
                    || (modelo.TipoModelo == WoTypeModel.Parameter))
                {
                    foreach (var col in modelo.Columnas.Where(e => e.ModeloId == this.Id).ToList())
                    {
                        if ((col.TipoColumna == WoTypeColumn.EnumInt) ||
                            (col.TipoColumna == WoTypeColumn.EnumString))
                            continue;

                        Attributes += "\r\n";
                        if ((col.TipoColumna == WoTypeColumn.Reference) ||
                            (col.TipoColumna == WoTypeColumn.Complex))
                        {
                            if (col.EsColeccion)
                                Attributes += modelo.ToAppDefines($@"    [WoReferenceColBy(typeof({modelo.Id}), ""{col.Id}"" )]");
                            else
                                Attributes += modelo.ToAppDefines($@"    [WoReferenceBy(typeof({modelo.Id}), ""{col.Id}"" )]");
                        }
                    }
                }
            }

            string csCode;
            WoLookUpConfigAttribute lk = this.ToWoLookUpConfig(out csCode);

            if (lk != null)
                Attributes += "\r\n" + csCode;


            if (!bClient)
            {
                // Coloca el atributo WoInterfaceAttribute
                if (!string.IsNullOrEmpty(this.Interface1))
                {
                    Modelo ModelInterface = proyecto.ModeloCol.Modelos.Where(e => e.Id == this.Interface1).FirstOrDefault();

                    if ((ModelInterface != null) && (ModelInterface.bScriptExistAnyPackage))
                    {
                        Attributes += "\r\n    #if SERVER\r\n";
                        if (TipoModelo == WoTypeModel.Class)
                            Attributes += $@"    [WoInterface(typeof({ModelInterface.Id}), null)]";
                        else
                            Attributes += $@"    [WoInterface(typeof({ModelInterface.Id}), typeof({ModelInterface.Id}Script))]";
                        Attributes += "\r\n    #endif";
                    }
                }

                // Coloca el atributo WoInterfaceAttribute
                if (!string.IsNullOrEmpty(this.Interface2))
                {
                    Modelo ModelInterface = proyecto.ModeloCol.Modelos.Where(e => e.Id == this.Interface2).FirstOrDefault();
                    if ((ModelInterface != null) && (ModelInterface.bScriptExistAnyPackage))
                    {
                        Attributes += "\r\n    #if SERVER\r\n";
                        if (TipoModelo == WoTypeModel.Class)
                            Attributes += $@"    [WoInterface(typeof({ModelInterface.Id}), null)]";
                        else
                            Attributes += $@"    [WoInterface(typeof({ModelInterface.Id}), typeof({ModelInterface.Id}Script))]";
                        Attributes += "\r\n    #endif";
                    }
                }

                // Coloca el atributo WoInterfaceAttribute
                if (!string.IsNullOrEmpty(this.Interface3))
                {
                    Modelo ModelInterface = proyecto.ModeloCol.Modelos.Where(e => e.Id == this.Interface3).FirstOrDefault();
                    if ((ModelInterface != null) && (ModelInterface.bScriptExistAnyPackage))
                    {
                        Attributes += "\r\n    #if SERVER\r\n";
                        if (TipoModelo == WoTypeModel.Class)
                            Attributes += $@"    [WoInterface(typeof({ModelInterface.Id}), null)]";
                        else
                            Attributes += $@"    [WoInterface(typeof({ModelInterface.Id}), typeof({ModelInterface.Id}Script))]";
                        Attributes += "\r\n    #endif";
                    }
                }
            }

            return Attributes;
        }

        public WoLookUpConfigAttribute ToWoLookUpConfig(out string csCode)
        {
            csCode = string.Empty;

            if (!((TipoModelo == WoTypeModel.Configuration)
                || (TipoModelo == WoTypeModel.CatalogType)
                || (TipoModelo == WoTypeModel.Catalog)
                || (TipoModelo == WoTypeModel.TransactionContable)
                || (TipoModelo == WoTypeModel.TransactionNoContable)
                || (TipoModelo == WoTypeModel.TransactionFreeStyle)))
                return null;

            // LookUp Attributes
            List<ModeloColumna> LookUp = Columnas.Where(e => e.EsVisibleEnLookUp == true).ToList();
            if (LookUp.Count > 0)
            {
                if (LookUp.Count > 2)
                    throw new Exception($"Modelo solo puede tener 2 columnas seleccionadas, para lookup");

                string Key = string.Empty;
                string Descripcion = string.Empty;

                foreach (var column in LookUp)
                {
                    if (column.Primaria)
                        Key = column.Id;
                    else
                        Descripcion = column.Id;
                }

                if (Key.IsNullOrStringEmpty())
                    throw new Exception($"Una de las columnas lookup debe ser llave, para lookup");

                if (Descripcion.IsNullOrStringEmpty())
                    throw new Exception($"Una de las columnas lookup debe ser descripción, para lookup");


                var Estado = Diagrama.Estados.Where(e => e.NumId == 100).FirstOrDefault();
                string Filter = string.Empty;
                if (Estado != null)
                    Filter = "WoState eq 100";

                csCode =
$@"    [WoLookUpConfig(KeyField = nameof({Key}), DescriptionField = nameof({Descripcion}),
        Filter = ""{Filter}"",
        Select = nameof({Key}) + "","" + nameof({Descripcion}),
        OrderBy = nameof({Key}))]";

                return new WoLookUpConfigAttribute()
                {
                    KeyField = Key,
                    DescriptionField = Descripcion,
                    Filter = Filter,
                    Select = Key + "," + Descripcion,
                    OrderBy = Key
                };
            }
            else
                return null;
        }

        public List<string> AnalizaRolesDeLectura(List<string> RolesSobrantes)
        {
            var RolesActuales = new List<string>();


            if (this.Diagrama.RolesLectura.Where(e => e.RolId == Proyecto.NOVALIDAR).FirstOrDefault() != null)
                return RolesActuales;

            foreach (var rol in this.Diagrama.Roles)
                if (RolesActuales.IndexOf(rol.RolId) == -1)
                    RolesActuales.Add(rol.RolId);
            foreach (var rol in this.Diagrama.RolesLectura)
                if (RolesActuales.IndexOf(rol.RolId) == -1)
                    RolesActuales.Add(rol.RolId);
            foreach (var transicion in this.Diagrama.Transiciones)
                foreach (var rol in transicion.Roles)
                    if (RolesActuales.IndexOf(rol.RolId) == -1)
                        RolesActuales.Add(rol.RolId);

            var RolesNecesarios = new List<string>();

            RolesNecesarios = new List<string>();
            foreach (var rol in this.Diagrama.Roles)
                if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                    RolesNecesarios.Add(rol.RolId);
            foreach (var transicion in this.Diagrama.Transiciones)
                foreach (var rol in transicion.Roles)
                    if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                        RolesNecesarios.Add(rol.RolId);

            foreach (var modelo in proyecto.ModeloCol.Modelos)
            {
                if (this == modelo)
                    continue;

                foreach (var columna in modelo.Columnas)
                {
                    if (columna.ModeloId == this.Id)
                    {
                        foreach (var rol in modelo.Diagrama.Roles)
                            if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                                RolesNecesarios.Add(rol.RolId);
                    }
                }

                // Igual para coleciones
                foreach (
                    var columna in modelo.Columnas.Where(
                        e => (e.EsColeccion) && (e.TipoColumna == WoTypeColumn.Reference)
                    )
                )
                {
                    var locModelo = proyecto.ModeloCol.Modelos
                        .Where(e => e.Id == columna.ModeloId)
                        .FirstOrDefault();

                    if (locModelo != null)
                    {
                        foreach (var locColumna in locModelo.Columnas)
                        {
                            if (locColumna.ModeloId == this.Id)
                            {
                                // Debe tomar los roles del modelo no de la coleccion
                                foreach (var rol in modelo.Diagrama.Roles)
                                    if (RolesNecesarios.IndexOf(rol.RolId) == -1)
                                        RolesNecesarios.Add(rol.RolId);
                            }
                        }
                    }
                }

            }

            return AnalizaRolesDeLectura(RolesActuales, RolesNecesarios, RolesSobrantes);
        }

        public List<string> AnalizaRolesDeLectura(List<string> RolesActuales, List<string> RolesNecesarios, List<string> RolesSobrantes)
        {
            // Los roles que no estan en el modelo actual
            List<string> RolesFaltantes = new List<string>();
            foreach (var rol in RolesNecesarios)
                if (RolesActuales.IndexOf(rol) == -1)
                    RolesFaltantes.Add(rol);

            // Los roles que sobran 
            foreach (var rol in RolesActuales)
                if (RolesNecesarios.IndexOf(rol) == -1)
                    RolesSobrantes.Add(rol);

            return RolesFaltantes;
        }
    }
}