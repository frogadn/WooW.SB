using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ServiceStack;
using WooW.Core;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Config.Templates;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Helpers
{
    public delegate void Log(string Mensaje);

    public class ModelMaker
    {
        public Log delLogtitulo { get; set; } = null;
        public Log delLogParrafo { get; set; } = null;
        public Log delLogError { get; set; } = null;

        private string ArchivoDeProyecto;
        private bool bCompilar;

        public ModelMaker(string _ArchivoDeProyecto, bool _bCompilar)
        {
            ArchivoDeProyecto = _ArchivoDeProyecto;
            bCompilar = _bCompilar;
        }

        public void Do()
        {
            ProyectoConPaquetes.Clear();
            Proyecto principal = ProyectoConPaquetes.Get(ArchivoDeProyecto);

            int Errores = 0;

            DateTime Inicio = DateTime.Now;

            Dictionary<string, StringBuilder> sbApps = new Dictionary<string, StringBuilder>();

            foreach (var app in principal.Apps)
                sbApps.Add(app.Id, new StringBuilder());

            LogTitulo("Generando Proyecto");

            LogLinea();
            LogTitulo("   Limpiando");

            #region Limpieza

            if (!Directory.Exists(principal.DirApplication))
                Directory.CreateDirectory(principal.DirApplication);
            if (!Directory.Exists(principal.DirApplication_Common))
                Directory.CreateDirectory(principal.DirApplication_Common);
            if (!Directory.Exists(principal.DirApplication_WebService))
                Directory.CreateDirectory(principal.DirApplication_WebService);
            if (!Directory.Exists(principal.DirApplication_WebService))
                Directory.CreateDirectory(principal.DirApplication_WebService);
            if (!Directory.Exists(principal.DirApplication_WebService_WebService))
                Directory.CreateDirectory(principal.DirApplication_WebService_WebService);

            VaciarCarpeta(principal.DirApplication_Common_Interface);
            VaciarCarpeta(principal.DirApplication_Common_Model);
            VaciarCarpeta(principal.DirApplication_Common_DTO);
            VaciarCarpeta(principal.DirApplication_Common_ServerServices);
            VaciarCarpeta(principal.DirApplication_Common_Miscellaneous);
            VaciarCarpeta(principal.DirApplication_Common_ClientServices);
            VaciarCarpeta(principal.DirApplication_WebService_WooWServer_ServiceInterface);
            VaciarCarpeta(principal.DirApplication_WebService_WebService_Reports);
            VaciarCarpeta(principal.DirApplication_WebService_WebService_Imports);

            #endregion Limpieza

            #region Miscelaneos

            if (principal.ModeloCol.Modelos.Count == 0)
            {
                LogError("No hay modelos para procesar");
                return;
            }

            LogTitulo("   Analizando Roles");

            // Analiza Roles Faltantes
            bool ErrorRoles = false;
            foreach (
                Modelo modelo in principal.ModeloCol.Modelos.Where(e =>
                    (e.TipoModelo == WoTypeModel.Configuration)
                    || (e.TipoModelo == WoTypeModel.CatalogType)
                    || (e.TipoModelo == WoTypeModel.Catalog)
                    || (e.TipoModelo == WoTypeModel.TransactionContable)
                    || (e.TipoModelo == WoTypeModel.TransactionNoContable)
                    || (e.TipoModelo == WoTypeModel.Control)
                    || (e.TipoModelo == WoTypeModel.Kardex)
                    || (e.TipoModelo == WoTypeModel.DataMart)
                    || (e.TipoModelo == WoTypeModel.Parameter)
                )
            )
            {
                var RolesSobrantes = new List<string>();

                var RolesFaltantes = modelo.AnalizaRolesDeLectura(RolesSobrantes);

                if (RolesFaltantes.Count != 0)
                {
                    string sRolesFaltantes = string.Join(", ", RolesFaltantes);
                    LogError(
                        $"      Faltan los roles de lectura {sRolesFaltantes} en el modelo {modelo.Id}"
                    );
                    ErrorRoles = true;
                }

                if (RolesSobrantes.Count != 0)
                {
                    string sRolesSobrantes = string.Join(", ", RolesSobrantes);
                    LogError(
                        $"      Sobran los roles de lectura {sRolesSobrantes} en el modelo {modelo.Id}"
                    );
                }
            }

            if (ErrorRoles)
                return;

            // Genera Roles
            ttRol ttrol = new ttRol();
            ttrol.proyecto = principal;

            string sBuffer = string.Empty;

            try
            {
                LogParrafo("   Roles");
                sBuffer = ttrol.TransformText();
            }
            catch (Exception ex)
            {
                LogError($"Error al generar Rol {ex.Message}");
                Errores++;
            }

            string sFile = $"{principal.DirApplication_Common_Miscellaneous}\\Rol.cs";
            File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

            sbApps
                .ToList()
                .ForEach(v =>
                    v.Value.AppendLine(
                        $"\t<Compile Include=\"..\\..\\Common\\Miscellaneous\\Rol.cs\" Link=\"Miscellaneous\\Rol.cs\" />"
                    )
                );

            LogTitulo("   Aplicando configuración ");

            // Genera Proyecto
            LogParrafo("   Proyecto");
            foreach (KeyValuePair<string, StringBuilder> app in sbApps)
            {
                if (app.Key == "WebClient")
                    continue;

                try
                {
                    ttProyectEnum ttproyectEnum = new ttProyectEnum();
                    ttproyectEnum.proyecto = principal;
                    ttproyectEnum.AppId = app.Key;
                    sBuffer = ttproyectEnum.TransformText();
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Proyecto {ex.Message}");
                    Errores++;
                }

                sFile =
                    @$"{principal.DirApplication}\{app.Key}\WooW.{app.Key}\Miscellaneous\Proyecto.cs";
                File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                //app.Value.AppendLine(
                //    $"\t<Compile Include=\"..\\..\\Common\\Miscellaneous\\principal.cs\" Link=\"Miscellaneous\\principal.cs\" />"
                //);
            }

            ttPermiso ttpermiso = new ttPermiso();
            ttpermiso.proyecto = principal;

            try
            {
                LogParrafo("   Permiso");
                sBuffer = ttpermiso.TransformText();
            }
            catch (Exception ex)
            {
                LogError($"Error al generar Permiso {ex.Message}");
                Errores++;
            }

            sFile = $"{principal.DirApplication_Common_Miscellaneous}\\Permiso.cs";
            File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

            sbApps
                .ToList()
                .ForEach(x =>
                    x.Value.AppendLine(
                        $"\t<Compile Include=\"..\\..\\Common\\Miscellaneous\\Permiso.cs\" Link=\"Miscellaneous\\Permiso.cs\" />"
                    )
                );

            // Genera CreateDB
            LogTitulo("Generando CreateDB");
            foreach (KeyValuePair<string, StringBuilder> app in sbApps)
            {
                if (app.Key == "WebClient")
                    continue;

                sBuffer = String.Empty;

                try
                {
                    ttCreateDB ttcreateDB = new ttCreateDB();
                    ttcreateDB.proyecto = principal;
                    ttcreateDB.AppId = app.Key;
                    sBuffer = ttcreateDB.TransformText();
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar CreateDB {ex.Message}");
                    Errores++;
                }

                sFile =
                    $"{principal.DirApplication}\\{app.Key}\\WooW.{app.Key}\\Miscellaneous\\CreateDB.cs";
                File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                //app.Value.AppendLine(
                //    $"\t<Compile Include=\"..\\..\\Common\\Miscellaneous\\CreateDB.cs\" Link=\"Miscellaneous\\CreateDB.cs\" />"
                //);
            }

            #endregion Miscelaneos

            #region Resources

            LogLinea();
            LogTitulo("Generando Resources");
            LogLinea();

            // El proceso asigna propiedades temporales
            // para la creación de modelos
            bool ErrorResources = false;
            foreach (var idioma in principal.Idiomas)
            {
                StringBuilder sbErrores = new StringBuilder();
                sbErrores = ResourcesMaker.Do(principal, idioma.Id, ref Errores);
                if (!sbErrores.ToString().IsNullOrEmpty())
                {
                    LogError(sbErrores.ToString());
                    ErrorResources = true;
                }
            }

            if (ErrorResources)
                return;

            #endregion Resources

            #region Interface

            LogLinea();
            LogTitulo("Generando Interfaces");

            foreach (
                Modelo modelo in principal.ModeloCol.Modelos.Where(e =>
                    e.TipoModelo == WoTypeModel.Interface
                )
            )
            {
                LogParrafo("   Interface " + modelo.Id);
                ttInterface ttinterface = new ttInterface();
                ttinterface.modelo = modelo;

                sBuffer = String.Empty;

                try
                {
                    sBuffer = ttinterface.TransformText();
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo {modelo.Id} {ex.Message}");
                    Errores++;
                }

                sFile = $"{principal.DirApplication_Common_Interface}\\{modelo.Id}.cs";
                File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                string Link =
                    $"\t<Compile Include=\"..\\..\\Common\\Interface\\{modelo.Id}.cs\" Link=\"Interface\\{modelo.Id}.cs\" />";

                foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                {
                    if (modelo.IsInTheApp(app.Key))
                        app.Value.AppendLine(Link);
                }
            }

            #endregion Interface

            #region Modelos

            LogLinea();
            LogTitulo("Generando Modelos");

            foreach (
                Modelo modelo in principal.ModeloCol.Modelos.Where(e =>
                    e.TipoModelo != WoTypeModel.Interface
                )
            )
            {
                // Validar que las columnas del modelo el origen sea diferente a null
                var columnasSinOrigen = modelo
                    .Columnas.Where(col => col.Origen.IsNullOrStringEmpty())
                    .ToList();
                foreach (var col in columnasSinOrigen)
                {
                    LogError(
                        $"Error en el modelo {modelo.Id} la columna {col.Id} no tiene origen, edite el modelo y grábelo de nuevo"
                    );
                    Errores++;
                }

                string IsStatic = string.Empty;
                if (modelo.SubTipoModelo == WoSubTypeModel.Static)
                    IsStatic = "static ";

                LogParrafo("   Modelo " + modelo.Id);
                ttModelo ttmodelo = new ttModelo();
                ttmodelo.modelo = modelo;
                ttmodelo.proyecto = principal;
                ttmodelo.IsStatic = (
                    modelo.SubTipoModelo == WoSubTypeModel.Static ? "static " : " "
                );

                if (
                    (modelo.TipoModelo == WoTypeModel.Configuration)
                    || (modelo.TipoModelo == WoTypeModel.Catalog)
                    || (modelo.TipoModelo == WoTypeModel.CatalogType)
                )
                    ttmodelo.ParentClass = ": AWoCatalog";
                else if (modelo.TipoModelo == WoTypeModel.CatalogSlave)
                    ttmodelo.ParentClass = ": AWoCatalogSlave";
                else if (
                    (modelo.TipoModelo == WoTypeModel.TransactionContable)
                    || (modelo.TipoModelo == WoTypeModel.TransactionNoContable)
                )
                    ttmodelo.ParentClass = ": AWoTransaction";
                else if (modelo.TipoModelo == WoTypeModel.TransactionSlave)
                    ttmodelo.ParentClass = ": AWoTransactionSlave";
                else if (
                    (modelo.TipoModelo == WoTypeModel.Control)
                    || (modelo.TipoModelo == WoTypeModel.Kardex)
                    || (modelo.TipoModelo == WoTypeModel.DataMart)
                )
                    ttmodelo.ParentClass = ": WoControl";
                else if (modelo.TipoModelo == WoTypeModel.Request)
                    ttmodelo.ParentClass = ": IWoInstanciaUdn";
                else if (modelo.TipoModelo == WoTypeModel.Parameter)
                    ttmodelo.ParentClass = ": AWoParameter";
                else
                    ttmodelo.ParentClass = String.Empty;

                if (!modelo.Interface1.IsNullOrStringEmpty())
                    if (ttmodelo.ParentClass.IsNullOrStringEmpty())
                        ttmodelo.ParentClass = $": {modelo.Interface1}";
                    else
                        ttmodelo.ParentClass = $"{ttmodelo.ParentClass}, {modelo.Interface1}";

                if (!modelo.Interface2.IsNullOrStringEmpty())
                    if (ttmodelo.ParentClass.IsNullOrStringEmpty())
                        ttmodelo.ParentClass = $": {modelo.Interface2}";
                    else
                        ttmodelo.ParentClass = $"{ttmodelo.ParentClass}, {modelo.Interface2}";

                if (!modelo.Interface3.IsNullOrStringEmpty())
                    if (ttmodelo.ParentClass.IsNullOrStringEmpty())
                        ttmodelo.ParentClass = $": {modelo.Interface3}";
                    else
                        ttmodelo.ParentClass = $"{ttmodelo.ParentClass}, {modelo.Interface3}";

                sBuffer = String.Empty;
                string sBufferClient = String.Empty;

                try
                {
                    ttmodelo.bClient = false;
                    sBuffer = ttmodelo.TransformText();

                    ttModelo ttmodeloClient = new ttModelo();
                    ttmodeloClient.modelo = ttmodelo.modelo;
                    ttmodeloClient.proyecto = ttmodelo.proyecto;
                    ttmodeloClient.ParentClass = ttmodelo.ParentClass;
                    ttmodeloClient.bClient = true;
                    ttmodeloClient.IsStatic = (
                        modelo.SubTipoModelo == WoSubTypeModel.Static ? "static " : " "
                    );
                    sBufferClient = ttmodeloClient.TransformText();
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo {modelo.Id} {ex.Message}");
                    Errores++;
                }

                sFile = $"{principal.DirApplication_Common_Model}\\{modelo.Id}.cs";
                File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                string Link =
                    $"\t<Compile Include=\"..\\..\\Common\\Model\\{modelo.Id}.cs\" Link=\"Model\\{modelo.Id}.cs\" />";

                foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                {
                    if (modelo.IsInTheApp(app.Key))
                        app.Value.AppendLine(Link);
                }
            }

            #endregion Modelos

            #region DTO

            LogLinea();
            LogTitulo("Modelos DTO");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (
                    (modelo.TipoModelo == WoTypeModel.CatalogSlave)
                    || (modelo.TipoModelo == WoTypeModel.TransactionSlave)
                    || (modelo.TipoModelo == WoTypeModel.Request)
                    || (modelo.TipoModelo == WoTypeModel.Response)
                    || (modelo.TipoModelo == WoTypeModel.Complex)
                    || (modelo.TipoModelo == WoTypeModel.Interface)
                    || (modelo.TipoModelo == WoTypeModel.Class)
                    || (modelo.TipoModelo == WoTypeModel.View)
                )
                    continue;

                try
                {
                    LogParrafo("   DTO " + modelo.Id);

                    if ((modelo.Diagrama == null) || (modelo.Diagrama.Estados.Count == 0))
                        throw new Exception($"Modelo {modelo.Id} no tiene diagrama");

                    if (modelo.TipoModelo == WoTypeModel.Parameter)
                    {
                        var ttmodelo = new ttModeloDTOParametro();
                        ttmodelo.modelo = modelo;
                        ttmodelo.proyecto = principal;
                        sBuffer = ttmodelo.TransformText();
                    }
                    else
                    {
                        var ttmodelo = new ttModeloDTO();
                        ttmodelo.modelo = modelo;

                        var col = modelo
                            .Columnas.Where(x => x.TipoDato == WoTypeData.Primary)
                            .FirstOrDefault();
                        if ((col != null) && (col.TipoColumna == WoTypeColumn.Autoincrement))
                            ttmodelo.TipoId = "ulong";
                        else
                            ttmodelo.TipoId = "string";

                        ttmodelo.proyecto = principal;
                        sBuffer = ttmodelo.TransformText();
                    }

                    sFile = $"{principal.DirApplication_Common_DTO}\\{modelo.Id}DTO.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                    string Link =
                        $"\t<Compile Include=\"..\\..\\Common\\DTO\\{modelo.Id}DTO.cs\" Link=\"DTO\\{modelo.Id}DTO.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo DTO {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            LogLinea();
            LogTitulo("Modelos DTO Slave");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (
                    (modelo.TipoModelo != WoTypeModel.CatalogSlave)
                    && (modelo.TipoModelo != WoTypeModel.TransactionSlave)
                )
                    continue;

                // Busca una tabla maestra que ocupe la esclava
                Modelo modeloMaster = null;
                foreach (var m in principal.ModeloCol.Modelos)
                {
                    var modelocoleccion = m
                        .Columnas.Where(c => c.ModeloId == modelo.Id)
                        .FirstOrDefault();

                    if (modelocoleccion != null)
                    {
                        var lstMod = principal
                            .ModeloCol.Modelos.Where(mm => mm.Id == m.Id)
                            .ToList();
                        if (lstMod.Count > 1)
                            throw new Exception(
                                "esclava no puede referenciarse por mas de una maestra"
                            );

                        modeloMaster = lstMod.FirstOrDefault();
                        break;
                    }
                }

                try
                {
                    if (modeloMaster == null)
                        throw new Exception("esclava no se ocupa por una maestra");

                    //Revisa que la modelMaster no este como una referencia
                    ////var mr = modelo.Referencias
                    ////    .Where(e => e.ModeloId == modeloMaster.Id)
                    ////    .FirstOrDefault();

                    ////if (mr != null)
                    ////    throw new Exception("esclava no puede referenciarse a la maestra");

                    LogParrafo("   DTO " + modelo.Id);
                    ttModeloDTOSlave ttmodeloslave = new ttModeloDTOSlave();
                    ttmodeloslave.modelo = modelo;
                    ttmodeloslave.modeloMaster = modeloMaster;

                    sBuffer = ttmodeloslave.TransformText();

                    sFile = $"{principal.DirApplication_Common_DTO}\\{modelo.Id}DTO.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                    string Link =
                        $"\t<Compile Include=\"..\\..\\Common\\DTO\\{modelo.Id}DTO.cs\" Link=\"DTO\\{modelo.Id}DTO.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo DTO Slave {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            LogLinea();
            LogTitulo("Modelos DTO List");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if ((modelo.TipoModelo != WoTypeModel.View))
                    continue;

                try
                {
                    LogParrafo("   DTO " + modelo.Id);
                    ttModeloDTOList ttmodeloslave = new ttModeloDTOList();
                    ttmodeloslave.modelo = modelo;

                    sBuffer = ttmodeloslave.TransformText();

                    sFile = $"{principal.DirApplication_Common_DTO}\\{modelo.Id}DTO.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                    string Link =
                        $"\t<Compile Include=\"..\\..\\Common\\DTO\\{modelo.Id}DTO.cs\" Link=\"DTO\\{modelo.Id}DTO.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo DTO List {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            #endregion DTO

            #region Server Services

            LogLinea();
            LogTitulo("Modelo Server Services");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (
                    (modelo.TipoModelo != WoTypeModel.Configuration)
                    && (modelo.TipoModelo != WoTypeModel.CatalogType)
                    && (modelo.TipoModelo != WoTypeModel.Catalog)
                    && (modelo.TipoModelo != WoTypeModel.TransactionContable)
                    && (modelo.TipoModelo != WoTypeModel.TransactionNoContable)
                    && (modelo.TipoModelo != WoTypeModel.Control)
                    && (modelo.TipoModelo != WoTypeModel.Kardex)
                    && (modelo.TipoModelo != WoTypeModel.DataMart)
                    && (modelo.TipoModelo != WoTypeModel.Parameter)
                    && (modelo.TipoModelo != WoTypeModel.Request)
                )
                    continue;

                try
                {
                    LogParrafo("   Servicio " + modelo.Id);

                    if (
                        (modelo.TipoModelo != WoTypeModel.Request)
                        && ((modelo.Diagrama == null) || (modelo.Diagrama.Estados.Count == 0))
                    )
                        throw new Exception($"Modelo {modelo.Id} no tiene diagrama");

                    if (modelo.TipoModelo == WoTypeModel.Request)
                    {
                        var ttmodelo = new ttModeloServicioRequest();
                        ttmodelo.modelo = modelo;
                        ttmodelo.responseObject = modelo.ScriptRequest.ResponseId;
                        sBuffer = ttmodelo.TransformText();
                    }
                    else if (modelo.TipoModelo == WoTypeModel.Parameter)
                    {
                        var ttmodelo = new ttModeloServicioParametro();
                        ttmodelo.modelo = modelo;
                        ttmodelo.PreConditionsFiles = modelo.ListPreConditionsFiles();
                        ttmodelo.PostConditionsFiles = modelo.ListPostConditionsFiles();
                        ttmodelo.WoGenericService = "WoGenericServiceParameter";
                        sBuffer = ttmodelo.TransformText();
                    }
                    else
                    {
                        var ttmodelo = new ttModeloServicio();
                        ttmodelo.modelo = modelo;
                        ttmodelo.PreConditionsFiles = modelo.ListPreConditionsFiles();
                        ttmodelo.PostConditionsFiles = modelo.ListPostConditionsFiles();

                        if (
                            (modelo.TipoModelo == WoTypeModel.Configuration)
                            || (modelo.TipoModelo == WoTypeModel.CatalogType)
                            || (modelo.TipoModelo == WoTypeModel.Catalog)
                        )
                            ttmodelo.WoGenericService = "WoGenericServiceCatalog";
                        else if (
                            (modelo.TipoModelo == WoTypeModel.TransactionContable)
                            || (modelo.TipoModelo == WoTypeModel.TransactionNoContable)
                        )
                            ttmodelo.WoGenericService = "WoGenericServiceTransaction";
                        else if (
                            (modelo.TipoModelo == WoTypeModel.Control)
                            || (modelo.TipoModelo == WoTypeModel.Kardex)
                            || (modelo.TipoModelo == WoTypeModel.DataMart)
                        )
                            ttmodelo.WoGenericService = "WoGenericServiceControl";

                        var col = modelo
                            .Columnas.Where(x => x.TipoDato == WoTypeData.Primary)
                            .FirstOrDefault();
                        if ((col != null) && (col.TipoColumna == WoTypeColumn.Autoincrement))
                            ttmodelo.TipoId = "ulong";
                        else
                            ttmodelo.TipoId = "string";

                        sBuffer = ttmodelo.TransformText();
                    }

                    sFile = $"{principal.DirApplication_Common_ServerServices}\\{modelo.Id}.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                    string Link =
                        $"\t<Compile Include=\"..\\..\\Common\\ServerServices\\{modelo.Id}.cs\" Link=\"ServerServices\\{modelo.Id}.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (app.Key == "WebClient")
                            continue;

                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo Servicio {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            #endregion Server Services

            #region Client Services

            LogLinea();
            LogTitulo("Modelo Client Services");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (
                    (modelo.TipoModelo == WoTypeModel.CatalogSlave)
                    || (modelo.TipoModelo == WoTypeModel.TransactionSlave)
                    || (modelo.TipoModelo == WoTypeModel.Request)
                    || (modelo.TipoModelo == WoTypeModel.Response)
                    || (modelo.TipoModelo == WoTypeModel.Complex)
                    || (modelo.TipoModelo == WoTypeModel.Interface)
                    || (modelo.TipoModelo == WoTypeModel.Class)
                    || (modelo.TipoModelo == WoTypeModel.View)
                )
                    continue;

                try
                {
                    LogParrafo("   Modelo Servicio Cliente " + modelo.Id);

                    if ((modelo.Diagrama == null) || (modelo.Diagrama.Estados.Count == 0))
                        throw new Exception($"Modelo {modelo.Id} no tiene diagrama");

                    if (modelo.TipoModelo == WoTypeModel.Parameter)
                    {
                        var ttmodelo = new ttModeloServicioClienteParametro();
                        ttmodelo.modelo = modelo;
                        sBuffer = ttmodelo.TransformText();
                    }
                    else
                    {
                        var ttmodelo = new ttModeloServicioCliente();
                        ttmodelo.modelo = modelo;
                        var col = modelo
                            .Columnas.Where(x => x.TipoDato == WoTypeData.Primary)
                            .FirstOrDefault();
                        if ((col != null) && (col.TipoColumna == WoTypeColumn.Autoincrement))
                            ttmodelo.TipoId = "ulong";
                        else
                            ttmodelo.TipoId = "string";
                        sBuffer = ttmodelo.TransformText();
                    }

                    sFile = $"{principal.DirApplication_Common_ClientServices}\\{modelo.Id}.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                    string Link =
                        $"\t<Compile Include=\"..\\..\\Common\\ClientServices\\{modelo.Id}.cs\" Link=\"ClientServices\\{modelo.Id}.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo Servicio Cliente {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            LogLinea();
            LogTitulo("Modelo Servicio Cliente");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (
                    (modelo.TipoModelo != WoTypeModel.View)
                    && (modelo.TipoModelo != WoTypeModel.CatalogSlave)
                    && (modelo.TipoModelo != WoTypeModel.TransactionSlave)
                )
                    continue;

                try
                {
                    LogParrafo("   Modelo Servicio Cliente " + modelo.Id);

                    var ttmodelo = new ttModeloServicioClienteList();
                    ttmodelo.modelo = modelo;
                    sBuffer = ttmodelo.TransformText();

                    sFile = $"{principal.DirApplication_Common_ClientServices}\\{modelo.Id}.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                    string Link =
                        $"\t<Compile Include=\"..\\..\\Common\\ClientServices\\{modelo.Id}.cs\" Link=\"ClientServices\\{modelo.Id}.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
                catch (Exception ex)
                {
                    LogError(
                        $"Error al generar Modelo Servicio Cliente List {modelo.Id} {ex.Message}"
                    );
                    Errores++;
                }
            }

            LogLinea();
            LogTitulo("Modelo Servicio Cliente Request");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (modelo.TipoModelo != WoTypeModel.Request)
                    continue;

                LogParrafo("   Modelo Servicio Cliente Request" + modelo.Id);

                try
                {
                    //var response = principal
                    //    .ModeloCol.Modelos.Where(e => e.Id == modelo.ScriptRequest.ResponseId)
                    //    .FirstOrDefault();

                    sBuffer = String.Empty;

                    string responseObject = modelo.ScriptRequest.ResponseId;

                    //if (response == null)
                    //{
                    //    //LogError(
                    //    //    $"Error al generar Modelo Servicio {modelo.Id} no esta definido el response"
                    //    //);
                    //    //Errores++;
                    //    //continue;
                    //}

                    if (
                        (modelo.TipoModelo == WoTypeModel.Request)
                        && (modelo.ScriptRequest.EjecutarEnBackGround)
                    )
                    {
                        ttModeloRequestServicioClienteTask ttmodelo =
                            new ttModeloRequestServicioClienteTask();
                        ttmodelo.modelo = modelo;
                        ttmodelo.responseObject = responseObject;

                        sBuffer = ttmodelo.TransformText();
                    }
                    else
                    {
                        ttModeloRequestServicioCliente ttmodelo =
                            new ttModeloRequestServicioCliente();
                        ttmodelo.modelo = modelo;
                        ttmodelo.responseObject = responseObject;
                        sBuffer = ttmodelo.TransformText();
                    }

                    sFile = $"{principal.DirApplication_Common_ClientServices}\\{modelo.Id}.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);

                    string Link =
                        $"\t<Compile Include=\"..\\..\\Common\\ClientServices\\{modelo.Id}.cs\" Link=\"ClientServices\\{modelo.Id}.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo Servicio Cliente {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            #endregion Client Services

            #region Service Interface

            LogLinea();
            LogTitulo("Modelo Service Interface");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (!modelo.IsInTheApp("WebService"))
                    continue;

                if (
                    (modelo.TipoModelo == WoTypeModel.CatalogSlave)
                    || (modelo.TipoModelo == WoTypeModel.TransactionSlave)
                    || (modelo.TipoModelo == WoTypeModel.Request)
                    || (modelo.TipoModelo == WoTypeModel.Response)
                    || (modelo.TipoModelo == WoTypeModel.Complex)
                    || (modelo.TipoModelo == WoTypeModel.Interface)
                    || (modelo.TipoModelo == WoTypeModel.Class)
                    || (modelo.TipoModelo == WoTypeModel.View)
                )
                    continue;

                try
                {
                    LogParrafo("  Service Interface " + modelo.Id);

                    if ((modelo.Diagrama == null) || (modelo.Diagrama.Estados.Count == 0))
                        throw new Exception($"Modelo {modelo.Id} no tiene diagrama");

                    ttModeloServiceInterface ttmodelo = new ttModeloServiceInterface();
                    ttmodelo.modelo = modelo;
                    sBuffer = ttmodelo.TransformText();

                    if (sBuffer.IsNullOrStringEmpty())
                        continue;

                    sFile =
                        $"{principal.DirApplication_WebService_WooWServer_ServiceInterface}\\{modelo.Id}ServiceInterface.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo Servicio {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            LogLinea();
            LogTitulo("Modelo Service Interface List");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (!modelo.IsInTheApp("WebService"))
                    continue;

                if (
                    (modelo.TipoModelo != WoTypeModel.View)
                    && (modelo.TipoModelo != WoTypeModel.CatalogSlave)
                    && (modelo.TipoModelo != WoTypeModel.TransactionSlave)
                )
                    continue;

                try
                {
                    LogParrafo("  Service Interface " + modelo.Id);

                    ttModeloServiceInterfaceList ttmodelo = new ttModeloServiceInterfaceList();
                    ttmodelo.modelo = modelo;
                    sBuffer = ttmodelo.TransformText();

                    if (sBuffer.IsNullOrStringEmpty())
                        continue;

                    sFile =
                        $"{principal.DirApplication_WebService_WooWServer_ServiceInterface}\\{modelo.Id}ServiceInterface.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo Servicio View {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            LogLinea();
            LogTitulo("Modelo Service Interface Request");

            foreach (Modelo modelo in principal.ModeloCol.Modelos)
            {
                if (!modelo.IsInTheApp("WebService"))
                    continue;

                if (modelo.TipoModelo != WoTypeModel.Request)
                    continue;

                LogParrafo("  Service Interface Request " + modelo.Id);

                //var response = principal
                //    .ModeloCol.Modelos.Where(e => e.Id == modelo.ScriptRequest.ResponseId)
                //    .FirstOrDefault();

                string responseObject = modelo.ScriptRequest.ResponseId;

                //if (response == null)
                //{
                //    LogError(
                //        $"Error al generar Modelo Servicio {modelo.Id} no esta definido el response"
                //    );
                //    Errores++;
                //    continue;
                //}

                try
                {
                    sBuffer = String.Empty;
                    if (
                        (modelo.TipoModelo == WoTypeModel.Request)
                        && (modelo.ScriptRequest.EjecutarEnBackGround)
                    )
                    {
                        ttModeloRequestServiceInterfaceTask ttmodelo =
                            new ttModeloRequestServiceInterfaceTask();
                        ttmodelo.request = modelo;
                        ttmodelo.responseObject = responseObject;
                        //ttmodelo.response = response;
                        sBuffer = ttmodelo.TransformText();
                    }
                    else
                    {
                        ttModeloRequestServiceInterface ttmodelo =
                            new ttModeloRequestServiceInterface();
                        ttmodelo.modelo = modelo;
                        ttmodelo.responseObject = responseObject;
                        //ttmodelo.response = response;

                        sBuffer = ttmodelo.TransformText();
                    }

                    if (sBuffer.IsNullOrStringEmpty())
                        continue;

                    sFile =
                        $"{principal.DirApplication_WebService_WooWServer_ServiceInterface}\\{modelo.Id}ServiceInterface.cs";
                    File.WriteAllText(SyntaxEditorHelper.PrettyPrint(sFile), sBuffer);
                }
                catch (Exception ex)
                {
                    LogError($"Error al generar Modelo Servicio {modelo.Id} {ex.Message}");
                    Errores++;
                }
            }

            #endregion Service Interface

            // Copia los reportes
            if (!Directory.Exists(principal.DirPlantillasReportes))
            {
                WoDirectory.CreateDirectory(principal.DirPlantillasReportes);
            }
            if (!Directory.Exists(principal.DirApplication_WebService_WebService_Reports))
            {
                WoDirectory.CreateDirectory(principal.DirApplication_WebService_WebService_Reports);
            }
            foreach (
                string Fuente in Directory.GetFiles($"{principal.DirPlantillasReportes}", "*.xml")
            )
            {
                string Destino =
                    $"{principal.DirApplication_WebService_WebService_Reports}\\{Path.GetFileName(Fuente)}";
                WoDirectory.CopyFile(Fuente, Destino);
            }

            // Copia los archivos de importación
            foreach (string Fuente in Directory.GetFiles($"{principal.DirImports}", "*.*"))
            {
                string Destino =
                    $"{principal.DirApplication_WebService_WebService_Imports}\\{Path.GetFileName(Fuente)}";
                File.Copy(Fuente, Destino);
            }

            if (Errores > 0)
            {
                LogLinea();
                LogTitulo($"Proceso Terminado con {Errores} Errores");

                if (bCompilar)
                    LogTitulo($"No se ejecuto la compilación");
                return;
            }

            LogLinea();
            LogTitulo("Proceso Terminado");

            LogLinea();
            LogTitulo("Lanzando Compilación");

            LogLinea();
            LogLinea();
            TimeSpan tsModelos = (DateTime.Now - Inicio);

            List<string> lstCreacionRepetida = new List<string>();

            foreach (
                string File in ProyectoConPaquetes.GetFiles(
                    principal,
                    principal.DirProyectDataLogicModelCreation,
                    "*.cs"
                )
            )
            {
                string FileWE = Path.GetFileNameWithoutExtension(File);

                if (!FileWE.Contains("ModelCreation"))
                    continue;

                string ModelName = FileWE.Substring(0, FileWE.IndexOf("ModelCreation"));

                var modelo = principal
                    .ModeloCol.Modelos.Where(e => e.Id == ModelName)
                    .FirstOrDefault();

                if (modelo == null)
                    continue;

                if (lstCreacionRepetida.Contains(modelo.Id))
                    continue;

                lstCreacionRepetida.Add(ModelName);

                //string Link =
                //    $"\t<Compile Include=\"..\\..\\..\\ProyectData\\Logic\\ModelCreation\\{FileWE}.cs\" Link=\"ModelCreation\\{FileWE}.cs\" />";

                var ModelCreationFiles = modelo.ListModelCreationFiles();

                foreach (var modelCreationFile in ModelCreationFiles)
                {
                    string Link =
                        $"\t<Compile Include=\"{modelCreationFile.Item2}\" Link=\"ModelCreation\\{modelo.Id}ModelCreation{modelCreationFile.Item1}.cs\" />";

                    foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                    {
                        if (app.Key == "WebClient")
                            continue;
                        if (modelo.IsInTheApp(app.Key))
                            app.Value.AppendLine(Link);
                    }
                }
            }

            foreach (
                string File in ProyectoConPaquetes.GetFiles(
                    principal,
                    principal.DirProyectDataLogicPreConditions,
                    "*.cs"
                )
            )
            {
                string FileWE = Path.GetFileNameWithoutExtension(File);
                string ModelName = FileWE.Substring(0, FileWE.IndexOf("PreConditions"));

                var modelo = principal
                    .ModeloCol.Modelos.Where(e => e.Id == ModelName)
                    .FirstOrDefault();

                if (modelo == null)
                    continue;

                var Link = $"\t<Compile Include=\"{File}\" Link=\"PreConditions\\{FileWE}.cs\" />";

                //var Link =
                //    $"\t<Compile Include=\"..\\..\\..\\ProyectData\\Logic\\PreConditions\\{FileWE}.cs\" Link=\"PreConditions\\{FileWE}.cs\" />";

                foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                {
                    if (app.Key == "WebClient")
                        continue;
                    if (modelo.IsInTheApp(app.Key))
                        app.Value.AppendLine(Link);
                }
            }

            foreach (
                string File in ProyectoConPaquetes.GetFiles(
                    principal,
                    principal.DirProyectDataLogicPostConditions,
                    "*.cs"
                )
            )
            {
                string FileWE = Path.GetFileNameWithoutExtension(File);
                string ModelName = FileWE.Substring(0, FileWE.IndexOf("PostConditions"));

                var modelo = principal
                    .ModeloCol.Modelos.Where(e => e.Id == ModelName)
                    .FirstOrDefault();

                if (modelo == null)
                    continue;

                var Link = $"\t<Compile Include=\"{File}\" Link=\"PostConditions\\{FileWE}.cs\" />";
                //var Link =
                //    $"\t<Compile Include=\"..\\..\\..\\ProyectData\\Logic\\PostConditions\\{FileWE}.cs\" Link=\"PostConditions\\{FileWE}.cs\" />";

                foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                {
                    if (app.Key == "WebClient")
                        continue;
                    if (modelo.IsInTheApp(app.Key))
                        app.Value.AppendLine(Link);
                }
            }

            foreach (var modelo in principal.ModeloCol.Modelos)
            {
                var list = modelo.ListModelPolizaFiles();

                if (list.Count == 0)
                    continue;

                string file = list[0].Item2;

                string FileWE = Path.GetFileNameWithoutExtension(file);
                string ModelName = FileWE.Substring(0, FileWE.IndexOf("ModelPoliza"));

                var Link = $"\t<Compile Include=\"{file}\" Link=\"ModelPoliza\\{FileWE}.cs\" />";
                //var Link =
                //    $"\t<Compile Include=\"..\\..\\..\\ProyectData\\Logic\\ModelPoliza\\{FileWE}.cs\" Link=\"ModelPoliza\\{FileWE}.cs\" />";

                foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                {
                    if (app.Key == "WebClient")
                        continue;
                    if (modelo.IsInTheApp(app.Key))
                        app.Value.AppendLine(Link);
                }
            }

            var lstLogicScripts = ProyectoConPaquetes.GetFiles(
                principal,
                principal.DirProyectDataLogicScripts,
                "*.cs"
            );

            foreach (string file in lstLogicScripts)
            {
                string FileWE = Path.GetFileNameWithoutExtension(file);
                string ModelName = FileWE.Substring(0, FileWE.IndexOf("Script"));

                var modelo = principal
                    .ModeloCol.Modelos.Where(e => e.Id == ModelName)
                    .FirstOrDefault();

                if (modelo == null)
                    continue;

                // Para aplicar el override
                if (
                    (modelo.TipoModelo == WoTypeModel.Class)
                    && (!IsFirstFile(file, lstLogicScripts))
                )
                    continue;

                var Link = $"\t<Compile Include=\"{file}\" Link=\"Scripts\\{FileWE}.cs\" />";
                //var Link =
                //    $"\t<Compile Include=\"..\\..\\..\\ProyectData\\Logic\\Scripts\\{FileWE}.cs\" Link=\"Scripts\\{FileWE}.cs\" />";

                foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                {
                    if (app.Key == "WebClient")
                        continue;
                    if (modelo.IsInTheApp(app.Key))
                        app.Value.AppendLine(Link);
                }
            }

            foreach (KeyValuePair<string, StringBuilder> app in sbApps)
                AgregarReferenciasAProyecto(
                    $"{principal.DirApplication}\\{app.Key}\\WooW.{app.Key}\\WooW.{app.Key}.csproj",
                    app.Value.ToString()
                );

            if (bCompilar)
            {
                Inicio = DateTime.Now;
                string Drive = Path.GetPathRoot(principal.DirApplication).Substring(0, 2);
                string sResult = Proceso(
                    principal,
                    $"{principal.DirApplication}\\Build.WooW.bat",
                    $"{Drive} \"{principal.DirApplication}\" \"{Application.StartupPath}\""
                );

                TimeSpan tsBuild = (DateTime.Now - Inicio);

                if (sResult.IsNullOrStringEmpty())
                {
                    LogTitulo("Generación y Compilación Terminada Correctamente");
                    LogTitulo(
                        "   Tiempo de generación  " + tsModelos.ToString(@"mm\:ss\:fff") + " min"
                    );
                    LogTitulo(
                        "   Tiempo de compilación " + tsBuild.ToString(@"mm\:ss\:fff") + " min"
                    );

                    Proyecto.getInstance().FechaCompilacion = DateTime.Now;

                    Proyecto.getInstance().Save();

                    try
                    {
                        fmMain.Restart(principal.ArchivoDeProyecto, string.Empty);
                    }
                    catch { }
                }
                else
                {
                    LogTitulo("Compilación Terminada con Errores");
                    LogLinea();
                    LogError(sResult);

                    XtraMessageBox.Show(
                        "Tiene errores la compilación",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else
            {
                LogTitulo("Generación Terminada Correctamente");
                LogTitulo(
                    "   Tiempo de generación  " + tsModelos.ToString(@"mm\:ss\:fff") + " min"
                );
            }
        }

        private bool IsFirstFile(string file, List<string> lstLogicScripts)
        {
            string locFile = Path.GetFileNameWithoutExtension(file);

            foreach (string itemFile in lstLogicScripts)
            {
                string itemFileWE = Path.GetFileNameWithoutExtension(itemFile);

                if (itemFileWE == locFile)
                    if (itemFile == file)
                        return true;
                    else
                        return false;
            }

            return false;
        }

        private void VaciarCarpeta(string Carpeta)
        {
            if (!Directory.Exists(Carpeta))
                Directory.CreateDirectory(Carpeta);

            var Archivos = Directory.GetFiles(Carpeta);

            foreach (var Archivo in Archivos)
                File.Delete(Archivo);
        }

        public void LogTitulo(string message)
        {
            if (delLogtitulo != null)
                delLogtitulo(message);
        }

        public void LogParrafo(string message)
        {
            if (delLogParrafo != null)
                delLogParrafo(message);
        }

        public void LogError(string message)
        {
            if (delLogError != null)
                delLogError(message);
        }

        public void LogLinea()
        {
            LogParrafo(string.Empty);
        }

        private string Proceso(Proyecto principal, string Proceso, string Parametros)
        {
            foreach (var app in principal.Apps)
            {
                string LogFile = $"{principal.DirApplication}\\{app.Id}\\WooW.{app.Id}.Errors.Log";
                if (File.Exists(LogFile))
                    File.Delete(LogFile);
            }

            if (!File.Exists(Proceso))
            {
                XtraMessageBox.Show(
                    $"No existe {Proceso}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return String.Empty;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(Proceso, Parametros);
            startInfo.WindowStyle = ProcessWindowStyle.Normal; // .Hidden;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true; //No utiliza RunDLL32 para lanzarlo   //Opcional: establecer la carpeta de trabajo en la que se ejecutará el proceso   //startInfo.WorkingDirectory = "C:\\MiCarpeta\\";
            //Redirige las salidas y los errores
            //startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            Process proc = Process.Start(startInfo); //Ejecuta el proceso
            proc.WaitForExit(); // Espera a que termine el proceso

            string error = string.Empty;

            foreach (var app in principal.Apps)
            {
                string LogFile = $"{principal.DirApplication}\\{app.Id}\\WooW.{app.Id}.Errors.Log";
                if (File.Exists(LogFile))
                {
                    if (!error.IsEmpty())
                        error += "\r\n\r\n";
                    error += File.ReadAllText(LogFile);
                }
            }

            return error;
        }

        private void AgregarReferenciasAProyecto(string Archivo, string Referencias)
        {
            StringBuilder sb = new StringBuilder(Referencias);

            // Abre el proyecto y agrega las referencias
            // Lee el archivo en arreglo string
            string[] lines = File.ReadAllLines(Archivo);

            StringBuilder sbNew = new StringBuilder();

            int i = 0;
            int iEstate = 0;
            for (; i < lines.Length; i++)
            {
                if (iEstate == 0)
                {
                    sbNew.AppendLine(lines[i]);
                    if (lines[i].Contains(@"<Compile Include=""*.cs"" />"))
                    {
                        iEstate = 1;
                        sbNew.AppendLine(sb.ToString());
                    }
                }
                else if (iEstate == 1)
                {
                    if (lines[i].Contains(@"</ItemGroup>"))
                    {
                        sbNew.AppendLine(lines[i]);
                        iEstate = 2;
                    }
                }
                else if (iEstate == 2)
                {
                    sbNew.AppendLine(lines[i]);
                }
            }

            // Escribe el archivo
            File.WriteAllText(Archivo, sbNew.ToString());
        }
    }
}
