using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WooW.Core;
using WooW.Core.Common;
using WooW.SB.Config.Helpers;
using WooW.SB.UI;

namespace WooW.SB.Config
{

    [WoPreviousName("Proyecto")]
    public class Proyecto
    {

        public const string NOVALIDAR = "__NoValidar__";

        private volatile static Proyecto uniqueInstance = null;
        private static readonly object padlock = new object();

        public static Proyecto getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (padlock)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new Proyecto();
                    }
                }
            }
            return uniqueInstance;
        }

        public static void Clear()
        {
            lock (padlock)
            {
                if (uniqueInstance != null)
                {
                    ProyectoConPaquetes.Clear();
                    uniqueInstance = null;
                }
            }
        }



        public string ValidaOrigen(string origen)
        {
            int iTotal = 0;
            StringBuilder sb = new StringBuilder();

            ValidaOrigenModelos(origen, sb, 20);
            if (sb.ToString().IsNullOrStringEmpty())
                ValidaOrigenPropiedades(origen, sb, 20);

            return sb.ToString();
        }

        public void ValidaOrigenModelos(string origen, StringBuilder sb, int iMax)
        {
            int iTotal = 0;

            // Valida que las modelos comiencen con el origen
            foreach (var modelo in this.ModeloCol.Modelos)
            {
                string locModelo = modelo.Id;
                if (modelo.TipoModelo == WoTypeModel.Interface) // Las intefaces comienzan con I
                    locModelo = modelo.Id.Substring(1);

                if (!locModelo.StartsWith(WoConst.ORIGENWOOW) && !locModelo.StartsWith(origen))
                {
                    sb.AppendLine($"El modelo {modelo.Id} no comienza con el origen {origen}");
                    if (iTotal++ > iMax)
                        break;
                }
            }

            Proyecto principal = null;
            try
            {
                principal = ProyectoConPaquetes.Get(this.ArchivoDeProyecto);
            } catch(Exception ex)
            {
                sb.AppendLine($"Error al cargar {this.ArchivoDeProyecto} {ex.Message}");
                return;
            }

            foreach (var modelo in this.ModeloCol.Modelos.Where(e => e.Id.StartsWith(origen)))
            {
                List<Modelo> interfazes = new List<Modelo>();

                // La columna es parte de alguna interfaz?
                if ((!modelo.Interface1.IsNullOrStringEmpty())
                    || (!modelo.Interface2.IsNullOrStringEmpty())
                    || (!modelo.Interface3.IsNullOrStringEmpty())
                )
                {
                    interfazes = principal.ModeloCol.Modelos.Where(x =>
                            (x.Id == modelo.Interface1.ToSafeString())
                            || (x.Id == modelo.Interface2.ToSafeString())
                            || (x.Id == modelo.Interface3.ToSafeString())
                        )
                        .ToList();
                }


                // Valida que las columnas comiencen con el origen excluye las columnas que se omiten
                foreach (var columna in modelo.Columnas.Where(x => WoConst.ColumnasOmitirEnOrigen.IndexOf(x.Id) == -1))
                {
                    bool bEsUnaColumanaDeInterfaz = false;
                    foreach (var interfaz in interfazes)
                    {
                        if (
                            interfaz.Columnas.Where(x => x.Id == columna.Id).FirstOrDefault()
                            != null
                        )
                        {
                            bEsUnaColumanaDeInterfaz = true;
                            break;
                        }
                    }

                    bool bEsUnaColumanaLookUpDescription = false;
                    if (columna.Id.StartsWith("__"))
                    {
                        // Busca las columnas con modelos
                        foreach (var modeloLoc in modelo.Columnas.Where(x => !x.ModeloId.IsNullOrStringEmpty()))
                        {
                            var columnaLoc = columna.Id.Substring(2);
                            if (columnaLoc.StartsWith(modeloLoc.Id))  // Es una columna de descripción de una referencia
                                                                      // El nombre puede no empezar con el origen
                            {
                                bEsUnaColumanaLookUpDescription = true;
                                break;
                            }

                        }
                    }

                    if (bEsUnaColumanaDeInterfaz ||
                        bEsUnaColumanaLookUpDescription ||
                        !columna.ModeloId.IsNullOrStringEmpty())
                        continue;

                    int start = 0;
                    if (columna.Id.StartsWith("__"))
                        start = 2;
                    else if (columna.Id.StartsWith("_"))
                        start = 1;

                    if (!columna.Id.Substring(start).StartsWith(origen))
                    {
                        sb.AppendLine($"La columna {columna.Id} del modelo {modelo.Id} no comienza con el origen {origen}");
                        if (iTotal++ > iMax)
                            break;
                    }
                    else
                    {
                        if (columna.Id.Length < (3 + start))
                            sb.AppendLine($"Nombre de columna {columna} debe tener más de {3 + start} caracteres");
                        else
                            // El tercer caracter es mayuscaula?
                            if (!char.IsUpper(columna.Id[2 + start]))
                            sb.AppendLine($"Nombre de columna {columna} debe tener la tercera letra en mayúscula y comenzar con el origen");
                    }
                }
            }
        }

        public void ValidaOrigenPropiedades(string origen, StringBuilder sb, int iMax)
        {
            int iTotal = 0;
            // Valida Proceso comienzen con el origen
            foreach (var proceso in this.Procesos)
                if (!proceso.Id.StartsWith(WoConst.ORIGENWOOW) && !proceso.Id.StartsWith(origen))
                {
                    sb.AppendLine($"El proceso {proceso.Id} no comienza con el origen {origen}");
                    if (iTotal++ > iMax)
                        break;
                }

            // Valida Roles comienzen con el origen
            foreach (var rol in this.Roles)
                if (!rol.Id.StartsWith(WoConst.ORIGENWOOW) && !rol.Id.StartsWith(origen))
                {
                    sb.AppendLine($"El rol {rol.Id} no comienza con el origen {origen}");
                    if (iTotal++ > iMax)
                        break;
                }

            // Valida Permisos comienzen con el origen
            foreach (var permiso in this.Permisos)
                if (!permiso.Id.StartsWith(WoConst.ORIGENWOOW) && !permiso.Id.StartsWith(origen))
                {
                    sb.AppendLine($"El permiso {permiso.Id} no comienza con el origen {origen}");
                    if (iTotal++ > iMax)
                        break;
                }
        }


        public void Load(string sFile)
        {

            try
            {
                string sJson = File.ReadAllText(sFile);

                Proyecto tmp = JsonConvert.DeserializeObject<Proyecto>(sJson);

                tmp.ArchivoDeProyecto = sFile;

                if (tmp.Paquetes == null)
                    tmp.Paquetes = new List<Paquete>();
                else
                    tmp.Paquetes.RemoveAll(e => e.Archivo.IsNullOrStringEmpty());

                foreach (var property in GetType().GetProperties())
                {
                    if ((property.GetSetMethod() != null) && (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).GetLength(0) == 0))
                        property.SetValue(this, property.GetValue(tmp, null), null);
                }

                string sDir = Path.GetDirectoryName(sFile);
                this.Dir = sDir;

                sFile = $"{this.DirResources}\\Etiquetas.json";
                if (File.Exists(sFile))
                {
                    sJson = File.ReadAllText(sFile);
                    this.EtiquetaCol = JsonConvert.DeserializeObject<EtiquetaCol>(sJson);
                }

                sFile = $"{this.DirResources}\\Mensajes.json";
                if (File.Exists(sFile))
                {
                    sJson = File.ReadAllText(sFile);
                    this.MensajeCol = JsonConvert.DeserializeObject<MensajeCol>(sJson);
                }

                sFile = $"{this.DirResources}\\ModeloTraduccion.json";
                if (File.Exists(sFile))
                {
                    sJson = File.ReadAllText(sFile);
                    this.ModeloTraduccion = JsonConvert.DeserializeObject<List<Traduccion>>(sJson);
                }

                sFile = $"{this.DirResources}\\ModeloAyudaTraduccion.json";
                if (File.Exists(sFile))
                {
                    sJson = File.ReadAllText(sFile);
                    this.ModeloAyudaTraduccion = JsonConvert.DeserializeObject<List<Traduccion>>(sJson);
                }


                this.ModeloCol = new ModeloCol();
                foreach (string sModel in Directory.EnumerateFiles(
                        this.DirModel,
                        "*.Model.json",
                        SearchOption.AllDirectories))
                {
                    sFile = sModel;
                    sJson = File.ReadAllText(sModel);
                    var model = JsonConvert.DeserializeObject<Modelo>(sJson);
                    model.ProyectoSetter(this);
                    this.ModeloCol.Modelos.Add(model);
                }

                this.ParConexion.ProyectoSetter(this);

                ProyectoConPaquetes.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar {sFile} {ex.Message}");
            }
        }


        public Proyecto()
        {
            Nombre = string.Empty;
            Version = string.Empty;
            Release = string.Empty;
            Fix = string.Empty;
            TipoProyecto = ProyectType.Libreria;
            HistorialDeVersiones = string.Empty;
            PropiedadesSoloLectura = false;

            Dir = string.Empty;
            Idiomas = new List<Idioma>();
            Roles = new List<Rol>();
            Permisos = new List<Permiso>();
            Procesos = new List<Proceso>();
            Apps = new List<App>();
            EtiquetaCol = new EtiquetaCol();
            MensajeCol = new MensajeCol();
            ModeloCol = new ModeloCol();

            ModeloTraduccion = new List<Traduccion>();
            ModeloAyudaTraduccion = new List<Traduccion>();
            Paquetes = new List<Paquete>();

            EtiquetasEditandose = false;
            MensajesEditandose = false;
            EtiquetasCambiadas = false;
            MensajesCambiados = false;
            FechaCompilacion = DateTime.MinValue;
            TemplateCompilacion = string.Empty;

            ParConexion = new ParametrosConexion();
            ParConexion.ProyectoSetter(this);



            AssemblyModelCargado = false;
        }

        public static string ArchivoPaquete(string DirBase, string ArchivoRelativo)
        {
            string path = Proyecto.GetAbsolutePath(
                Path.GetDirectoryName(DirBase),
                Path.GetDirectoryName(ArchivoRelativo)
            );

            return Path.Combine(path, Path.GetFileName(ArchivoRelativo));
        }

        public static string GetRelativePath(string path1, string path2)
        {
            Uri uri1 = new Uri(path1);
            Uri uri2 = new Uri(path2);

            Uri relativeUri = uri1.MakeRelativeUri(uri2);
            return relativeUri.ToString();
        }

        // Obtiene la ruta absoluta de un archivo
        public static string GetAbsolutePath(string path1, string path2)
        {
            Uri uri1 = new Uri(path1);
            Uri uri2 = new Uri(uri1, path2);

            return uri2.LocalPath;
        }


        [Description("Nombre asociado al proyecto"), Category("General")]
        public string Nombre { get; set; }


        [ReadOnly(true)]
        [Description("Version"), Category("General")]
        public string Version { get; set; }


        [ReadOnly(true)]
        [Description("Release"), Category("General")]
        public string Release { get; set; }


        [ReadOnly(true)]
        [Description("Fix"), Category("General")]
        public string Fix { get; set; }

        [ReadOnly(true)]
        [Description("Tipo de Proyecto"), Category("General")]
        public ProyectType TipoProyecto { get; set; }

        [Browsable(false)]
        public string HistorialDeVersiones { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public bool PropiedadesSoloLectura { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public string LegacyDirProjectReports
        { get { return $"{Dir}\\ProjectReports"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string FileWooWWebClientdll
        { get { return $"{DirApplication_WebClient}\\WooW.WebClient\\bin\\Debug\\net8.0\\WooW.WebClient.dll"; } }


        public string GetNameExtension()
        {
            string Archivo = Path.GetFileNameWithoutExtension(ArchivoDeProyecto);
            return Archivo.Substring(0, Archivo.IndexOf("."));
        }


        [JsonIgnore]
        public List<Tuple<string, string>> DirectorioDeLosPaquetes = null;

        [Browsable(false)]
        [JsonIgnore]
        public string Dir { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectData
        { get { return $"{Dir}\\ProyectData"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirModel
        { get { return $"{DirProyectData}\\Models"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirResources
        { get { return $"{DirProyectData}\\Resources"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirLayOuts
        { get { return $"{DirProyectData}\\LayOuts"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectDataLogic
        { get { return $"{DirProyectData}\\Logic"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectDataLogicPreConditions
        { get { return $"{DirProyectDataLogic}\\PreConditions"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectDataLogicPostConditions
        { get { return $"{DirProyectDataLogic}\\PostConditions"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectDataLogicModelPoliza
        { get { return $"{DirProyectDataLogic}\\ModelPoliza"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectDataLogicModelCreation
        { get { return $"{DirProyectDataLogic}\\ModelCreation"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectDataLogicScripts
        { get { return $"{DirProyectDataLogic}\\Scripts"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirVistasReports
        { get { return $"{DirLayOuts}\\Reports\\Vistas"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirPlantillasReportes
        { get { return $"{DirLayOuts}\\Reports\\Plantillas"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirImports
        { get { return $"{DirProyectData}\\Imports"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication
        { get { return $"{Dir}\\Application"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Common
        { get { return $"{DirApplication}\\Common"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Common_DTO
        { get { return $"{DirApplication_Common}\\DTO"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Common_Interface
        { get { return $"{DirApplication_Common}\\Interface"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Common_ServerServices
        { get { return $"{DirApplication_Common}\\ServerServices"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Common_ClientServices
        { get { return $"{DirApplication_Common}\\ClientServices"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Common_Miscellaneous
        { get { return $"{DirApplication_Common}\\Miscellaneous"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Common_Model
        { get { return $"{DirApplication_Common}\\Model"; } }



        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebClient
        { get { return $"{DirApplication}\\WebClient"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService
        { get { return $"{DirApplication}\\WebService"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService_WebService
        { get { return $"{DirApplication_WebService}\\WooW.Server"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService_WooWServer
        { get { return $"{DirApplication_WebService}\\WooW.Server"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService_WooWServer_ServiceInterface
        { get { return $"{DirApplication_WebService_WooWServer}\\ServiceInterface"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService_WebService_Reports
        { get { return $"{DirApplication_WebService_WebService}\\bin\\Debug\\net8.0\\Reportes"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService_WebService_Imports
        { get { return $"{DirApplication_WebService_WebService}\\Imports"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_Resources
        { get { return $"{DirApplication}\\Resources"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_UnitTest
        { get { return $"{DirApplication}\\UnitTest"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_UnitTest_UnitTest
        { get { return $"{DirApplication_UnitTest}\\WooW.UnitTest"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_UnitTest_UnitTestHybrid
        { get { return $"{DirApplication_UnitTest}\\WooW.UnitTest.Hybrid"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectData_Test
        { get { return $"{DirProyectData}\\Test"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectData_Test_UnitTest
        { get { return $"{DirProyectData_Test}\\UnitTest"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectData_Test_IntegralTest
        { get { return $"{DirProyectData_Test}\\IntegralTest"; } }


        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService_WooWServer_DBApp
        { get { return $"{DirApplication_WebService_WooWServer}\\bin\\Debug\\net8.0\\DBApp"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirApplication_WebService_WooWServer_DirDBService
        { get { return $"{DirApplication_WebService_WooWServer}\\bin\\Debug\\net8.0\\DBService"; } }

        /// <summary>
        /// Rutas para el diseñador.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirProyectTemp
        { get { return $"{DirApplication}\\Temp"; } }

        /// <summary>
        /// Ruta para almacenar los modelos creados
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirMenus
        { get { return $"{DirLayOuts}\\Menus"; } }

        /// <summary>
        /// Ruta para almacenar los formularios
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirFormDesign
        { get { return $"{DirLayOuts}\\FormDesign"; } }

        /// <summary>
        /// Ruta para almacenar los formularios
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirListDesign
        { get { return $"{DirLayOuts}\\ListDesign"; } }

        /// <summary>
        /// Ruta para almacenar los formularios
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirGrids
        { get { return $"{DirLayOuts}\\Grids"; } }

        /// <summary>
        /// Ruta para almacenar el codigo del usuario de los formularios
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirFormDesignUserCode
        { get { return $"{DirLayOuts}\\UserCode"; } }

        /// <summary>
        /// Ruta temporal para almacenar los menus 
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirMenusTemp
        { get { return $"{DirApplication}\\WebService\\WooW.Server\\bin\\Debug\\net8.0\\Menus"; } }

        /// <summary>
        ///  Ruta para almacenar los reportes creados
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirReportes
        { get { return $"{DirLayOuts}\\Reports"; } }

        /// <summary>
        ///  Ruta para almacenar los reportes creados
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirReportesVistas
        { get { return $"{DirReportes}\\Vistas"; } }

        /// <summary>
        ///  Ruta para almacenar las plantillas de los reportes 
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public string DirReportesPlantillas
        { get { return $"{DirReportes}\\Plantillas"; } }

        [Browsable(false)]
        [JsonIgnore]
        public string DirReportesTemp
        { get { return $"{DirApplication}\\WebService\\WooW.Server\\bin\\Debug\\net8.0\\Reportes"; } }

        [Category("General"), Description("Lista de Idiomas"), DisplayNameAttribute("Idiomas"),
        EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<Idioma> Idiomas { get; set; }

        [Category("General"), Description("Lista de Roles"), DisplayNameAttribute("Roles"),
        EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<Rol> Roles { get; set; }

        [Category("General"), Description("Lista de Permisos"), DisplayNameAttribute("Permisos"),
        EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<Permiso> Permisos { get; set; }

        [Category("General"), Description("Lista de Procesos"), DisplayNameAttribute("Procesos"),
        EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<Proceso> Procesos { get; set; }

        [Category("General"), Description("Lista de Apps"), DisplayNameAttribute("Apps"),
        EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<App> Apps { get; set; }

        [Browsable(false)]
        public List<Paquete> Paquetes { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public EtiquetaCol EtiquetaCol { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public MensajeCol MensajeCol { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public ModeloCol ModeloCol { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public List<Traduccion> ModeloTraduccion { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public List<Traduccion> ModeloAyudaTraduccion { get; set; }


        [Browsable(false)]
        [JsonIgnore]
        public string esMX = @"es-MX";

        [Browsable(false)]
        [JsonIgnore]
        public string ArchivoDeProyecto { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public bool EtiquetasEditandose { get; set; }


        [Browsable(false)]
        [JsonIgnore]
        public bool MensajesEditandose { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public bool EtiquetasCambiadas { get; set; }


        [Browsable(false)]
        [JsonIgnore]
        public bool MensajesCambiados { get; set; }

        [Browsable(false)]
        public DateTime FechaCompilacion { get; set; }

        [Browsable(false)]
        public bool RequiereCompilacion()
        {
            if (TienenCambioLosArchivos(
                    this.DirModel,
                    "*.Model.json", this.FechaCompilacion) ||
               TienenCambioLosArchivos(
                    this.DirProyectDataLogicPreConditions,
                    "*.cs", this.FechaCompilacion) ||
               TienenCambioLosArchivos(
                    this.DirProyectDataLogicPostConditions,
                    "*.cs", this.FechaCompilacion) ||
               TienenCambioLosArchivos(
                    this.DirProyectDataLogicModelCreation,
                    "*.cs", this.FechaCompilacion) ||
               TienenCambioLosArchivos(
                    this.DirProyectDataLogicModelPoliza,
                    "*.cs", this.FechaCompilacion) ||
               TienenCambioLosArchivos(
                    this.DirProyectDataLogicScripts,
                    "*.cs", this.FechaCompilacion))
                return true;
            return false;
        }
        
        private bool TienenCambioLosArchivos(string directory, string mask, DateTime comparisonDate)
        {
            var modelFiles = Directory.GetFiles(directory, mask);

            foreach (var file in modelFiles)
            {
                DateTime lastModified = File.GetLastWriteTime(file);
                if (lastModified > comparisonDate)
                {
                    return true;
                }
            }

            return false;
        }



        [Browsable(false)]
        public string TemplateCompilacion { get; set; }



        [Browsable(false)]
        [JsonIgnore]
        public bool AssemblyModelCargado { get; set; }


        [Browsable(false)]
        public ParametrosConexion ParConexion { get; set; }



        public void SaveProyect()
        {
            string sJson = JsonConvert.SerializeObject(this);
            File.WriteAllText(this.ArchivoDeProyecto, sJson);
        }


        public void SaveLabels()
        {
            string sFile = $"{this.DirResources}\\Etiquetas.json";
            string sJson = JsonConvert.SerializeObject(this.EtiquetaCol);
            File.WriteAllText(sFile, sJson);
        }

        public void SaveMessages()
        {
            string sFile = $"{this.DirResources}\\Mensajes.json";
            string sJson = JsonConvert.SerializeObject(this.MensajeCol);
            File.WriteAllText(sFile, sJson);
        }

        public void SaveModeloTraduccion()
        {
            string sFile = $"{this.DirResources}\\ModeloTraduccion.json";
            string sJson = JsonConvert.SerializeObject(this.ModeloTraduccion);
            File.WriteAllText(sFile, sJson);

            sFile = $"{this.DirResources}\\ModeloAyudaTraduccion.json";
            sJson = JsonConvert.SerializeObject(this.ModeloAyudaTraduccion);
            File.WriteAllText(sFile, sJson);
        }


        public void Save()
        {

            if (!Directory.Exists(this.DirModel))
                Directory.CreateDirectory(this.DirModel);

            if (!Directory.Exists(this.DirResources))
                Directory.CreateDirectory(this.DirResources);

            if (!Directory.Exists(this.DirProyectData_Test_UnitTest))
                Directory.CreateDirectory(this.DirProyectData_Test_UnitTest);

            if (!Directory.Exists(this.DirProyectData_Test_IntegralTest))
                Directory.CreateDirectory(this.DirProyectData_Test_IntegralTest);

            SaveProyect();
            SaveLabels();
            SaveMessages();
            SaveModeloTraduccion();

            string sJson = string.Empty;
            string sFile = string.Empty;

            //foreach (Modelo m in this.ModeloCol.Modelos)
            //{
            //    if (m.FechaActualizacion == DateTime.MinValue)
            //        m.FechaActualizacion = DateTime.Now;
            //    SaveModel(m);
            //}

            foreach (string sModel in Directory.GetFiles(this.DirModel, "*.Model.json"))
            {
                // Quita el Model.json
                string sModelFile = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(sModel));

                var modelo = this.ModeloCol.Modelos.Where(e => e.Id == sModelFile).FirstOrDefault();

                // Borrar todo lo generado
                if (modelo.IsNull())
                {
                    // Borra el Json
                    if (File.Exists(sModel))
                        File.Delete(sModel);
                    // Borra los Modelos
                    string sFileLoc =
                        $"{this.DirApplication_Common_Model}\\{sModelFile}.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    // Borra los DTO
                    sFileLoc =
                        $"{this.DirApplication_Common_DTO}\\{sModelFile}DTO.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    // Borra Servicios
                    sFileLoc = $"{this.DirApplication_Common_ServerServices}\\{sModelFile}.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    // Borra Servicios Cliente
                    sFileLoc = $"{this.DirApplication_Common_ClientServices}\\{sModelFile}.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    // Borra Intefaces
                    sFileLoc =
                        $"{this.DirApplication_WebService_WooWServer_ServiceInterface}\\{sModelFile}ServiceInterface.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    // Borra Scripts
                    sFileLoc = $"{this.DirProyectDataLogicPreConditions}\\{sModelFile}PreConditions.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    sFileLoc = $"{this.DirProyectDataLogicPostConditions}\\{sModelFile}PostConditions.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    sFileLoc = $"{this.DirProyectDataLogicModelPoliza}\\{sModelFile}ModelPoliza.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                    sFileLoc = $"{this.DirProyectDataLogicModelCreation}\\{sModelFile}ModelCreation.cs";
                    if (File.Exists(sFileLoc))
                        File.Delete(sFileLoc);

                }
            }
        }


        public Proyecto Clone()
        {
            return JsonConvert.DeserializeObject<Proyecto>(JsonConvert.SerializeObject(this));
        }

        public void SaveModel(Modelo model)
        {
            string sJson = JsonConvert.SerializeObject(model);
            string ArchivoNuevo = $"{this.DirModel}\\{model.Id}.Model.json";
            File.WriteAllText(ArchivoNuevo, sJson);
        }

        public void DeleteModel(string ModeloABorrar)
        {

            Modelo modelo = this.ModeloCol.Modelos.Where(e => e.Id == ModeloABorrar).FirstOrDefault();

            if (modelo == null)
                throw new Exception($"Modelo {ModeloABorrar} no fue encontrado");

            modelo.DeletePreCondiciones();
            modelo.DeletePostCondiciones();
            modelo.DeleteModelCreation();
            modelo.DeleteModelPoliza();
            modelo.DeleteScript();

            this.ModeloCol.Modelos.Remove(modelo);

            string ArchivoModeloABorrar = $"{this.DirModel}\\{ModeloABorrar}.Model.json";

            if (File.Exists(ArchivoModeloABorrar))
                File.Delete(ArchivoModeloABorrar);
        }


        public void RenameModelByFile(string ArchivoAnterior, string ArchivoNuevo, string ModeloAnterior, string ModeloNuevo)
        {
            if (File.Exists(ArchivoNuevo))
                File.Delete(ArchivoNuevo);

            if (File.Exists(ArchivoAnterior))
            {
                File.Move(ArchivoAnterior, ArchivoNuevo);

                string sFile = File.ReadAllText(ArchivoNuevo);

                // ToDo deberia hacerlo Roslyn
                sFile = sFile
                    .Replace($"class {ModeloAnterior}", $"class {ModeloNuevo}")
                    .Replace($"public {ModeloAnterior}", $"public {ModeloNuevo}")
                    .Replace($"<{ModeloAnterior}>", $"<{ModeloNuevo}>")
                    .Replace($": {ModeloAnterior}", $": {ModeloNuevo}")
                    .Replace($"req{ModeloAnterior}", $"req{ModeloNuevo}")
                    .Replace($"empty{ModeloAnterior}", $"empty{ModeloNuevo}")
                    .Replace($"dto{ModeloAnterior}", $"dto{ModeloNuevo}")
                    .Replace($"mod{ModeloAnterior}", $"mod{ModeloNuevo}")
                    .Replace($"cur{ModeloAnterior}", $"cur{ModeloNuevo}")
                    .Replace($"curr{ModeloAnterior}", $"curr{ModeloNuevo}")
                    .Replace($"o{ModeloAnterior}", $"o{ModeloNuevo}")
                    .Replace($"#region {ModeloAnterior}", $"#region  {ModeloNuevo}")
                    .Replace($"#endregion {ModeloAnterior}", $"#endregion  {ModeloNuevo}");

                File.WriteAllText(ArchivoNuevo, sFile);
            }
        }


        public void RenameModel(string Anterior, string Nuevo)
        {

            // ToDo renombrar los modelos referenciados 
            // ToDo renombrar los script dto y script


            Modelo modelo = this.ModeloCol.Modelos.Where(e => e.Id == Nuevo).FirstOrDefault();

            if (modelo != null)
                throw new Exception($"Ya existe un Modelo con el nombre {Nuevo}");

            modelo = this.ModeloCol.Modelos.Where(e => e.Id == Anterior).FirstOrDefault();

            if (modelo == null)
                throw new Exception($"Modelo {Anterior} no fue encontrado");


            string AnteriorPreConditionsFile = modelo.PreConditionsFile;
            string AnteriorPostConditionsFile = modelo.PostConditionsFile;
            string AnteriorScriptFile = modelo.ScriptFile;
            string AnteriorPolizaFile = modelo.ModelPolizaFile;
            string AnteriorCreationFile = modelo.ModelCreationFile;

            modelo.Id = Nuevo;

            RenameModelByFile(AnteriorPreConditionsFile, modelo.PreConditionsFile, Anterior, Nuevo);
            RenameModelByFile(AnteriorPostConditionsFile, modelo.PostConditionsFile, Anterior, Nuevo);
            RenameModelByFile(AnteriorScriptFile, modelo.ScriptFile, Anterior, Nuevo);
            RenameModelByFile(AnteriorPolizaFile, modelo.ModelPolizaFile, Anterior, Nuevo);
            RenameModelByFile(AnteriorCreationFile, modelo.ModelCreationFile, Anterior, Nuevo);

            foreach (var locModelo in this.ModeloCol.Modelos)
            {
                bool Save = false;
                foreach (var locColumna in locModelo.Columnas)
                {
                    if (locColumna.ModeloId == Anterior)
                    {
                        locColumna.ModeloId = Nuevo;
                        Save = true;
                    }
                }
                if (Save)
                    SaveModel(locModelo);
            }

            string ArchivoModeloABorrar = $"{this.DirModel}\\{Anterior}.Model.json";
            if (File.Exists(ArchivoModeloABorrar))
                File.Delete(ArchivoModeloABorrar);

            SaveModel(modelo);
        }

        public void AddModel(Modelo NewModel)
        {

            Modelo modelo = this.ModeloCol.Modelos.Where(e => e.Id == NewModel.Id).FirstOrDefault();

            if (modelo != null)
            {
                NewModel.Diagrama = modelo.Diagrama.Clone();
                NewModel.ScriptRequest = modelo.ScriptRequest.Clone();
                this.ModeloCol.Modelos.Remove(modelo);
            }

            this.ModeloCol.Modelos.Add(NewModel);

            SaveModel(NewModel);
        }

        public void SaveDiagram(string NombreModelo, ModeloDiagrama Diagrama)
        {

            Modelo modelo = this.ModeloCol.Modelos.Where(e => e.Id == NombreModelo).FirstOrDefault();

            if (modelo == null)
                throw new Exception($"Modelo {NombreModelo} no fue encontrado");

            modelo.FechaActualizacion = DateTime.Now;
            modelo.Diagrama = Diagrama.Clone();

            SaveModel(modelo);
        }

        public Dictionary<string, Tuple<string, string>> GetDiccTraduccion(string Idioma)
        {

            var TradActual = new Dictionary<string, Tuple<string, string>>();

            var mt = this.ModeloTraduccion.Where(e => e.IdiomaId == Idioma).FirstOrDefault();

            if (mt != null)
            {
                foreach (var item in mt.Items)
                    TradActual.Add(item.Etiqueta, Tuple.Create(item.Traduccion, item.Ayuda));
            }

            return TradActual;
        }

        public Dictionary<string, Tuple<string, string>> GetDiccAyudaTraduccion(string Idioma)
        {

            var TradActual = new Dictionary<string, Tuple<string, string>>();

            var mt = this.ModeloAyudaTraduccion.Where(e => e.IdiomaId == Idioma).FirstOrDefault();

            if (mt != null)
            {
                foreach (var item in mt.Items)
                    TradActual.Add(item.Etiqueta, Tuple.Create(item.Traduccion, item.Ayuda));
            }

            return TradActual;
        }


    }
}