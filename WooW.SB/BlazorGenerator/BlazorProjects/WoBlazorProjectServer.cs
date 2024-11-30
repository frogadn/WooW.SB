using System;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using DevExpress.XtraEditors;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorTemplates.ServerTemplates;
using WooW.SB.BlazorGenerator.BlazorTemplates.ServerTemplates.Controllers;
using WooW.SB.BlazorGenerator.BlazorTemplates.ServerTemplates.Pages;
using WooW.SB.BlazorGenerator.BlazorTemplates.ServerTemplates.Shared;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorProjects
{
    public class WoBlazorProjectServer : AWoBlazorProjectClass
    {
        #region Atributos

        /// <summary>
        /// Controlador de eventos que envía la data a la consola.
        /// </summary>
        public Action<string> SendToConsole { get; set; }

        #endregion Atributos

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase, inicializa el nombre del proyecto
        /// y la bandera que indica si es una unitaria por parámetros.
        /// El path y el cmdInstaller se inicializan de forma interna igual en el constructor.
        /// </summary>
        /// <param name="projectName"></param>
        public WoBlazorProjectServer(string projectName)
        {
            ProjectName = projectName;
            PathProject = $@"{Project.DirApplication}\Temp\{ProjectName}";
            Cmd = new WoCmdInstaller(PathProject);
        }

        #endregion Constructor


        #region Método principal

        /// <summary>
        /// Método de inicio ocupado de orquestar los métodos para la creación del proyecto.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CreateProject(string code)
        {
            try
            {
                CreateBlazor();
                //ChargeNugets();

                CleanProject();
                CreateDirectories();

                /// Abstract class
                GenerateUrnRedirection();
                BuildGenericBaseProject();
                BuildAuthFiles();
                BuildControlModels();
                BuildFluentValidators();
                BuildLocalizer();
                BuildPages(code);
                BuildShared();
                BuildCssFiles();
                CopyCssFiles();
                BuildUserCode();
                BuildJsFiles();
                CopyWwwrootFiles();
                BuildThemes();

                /// Server class
                BuildPropertyFiles();
                BuildBaseFiles();
                BuildPagesFiles();
                BuildControllersFiles();
                BuildSharedFiles();
            }
            catch (Exception ex)
            {
                // send to alert log
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar generar el proyecto Clase: WoBlazorProjectServer.cs. {ex.Message}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
            }
        }

        #endregion Método principal


        #region Creación del proyecto e instalación de NuGets

        /// <summary>
        /// Método que se detona al ejecutar el método CreateProject, se encarga de crear el proyecto y
        /// validar la existencia del fichero donde se creara.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void CreateBlazor()
        {
            try
            {
                if (Directory.Exists(PathProject))
                    WoDirectory.DeleteDirectory(PathProject);

                WoDirectory.CreateDirectory(PathProject);

                string result = Cmd.Execute("/c dotnet new blazorserver");

                SendToConsole?.Invoke(result);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar generar el proyecto {ProjectName} en el path {PathProject}: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Instala los paquetes nuget necesarios para el correcto funcionamiento del proyecto.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void ChargeNugets()
        {
            try
            {
                string result = string.Empty;

                result = Cmd.Execute(
                    "/c dotnet add package Blazor.Extensions.Logging --version 2.0.4"
                );
                SendToConsole?.Invoke(result);

                result = Cmd.Execute("/c dotnet add package FastMember --version 1.5.0");
                SendToConsole?.Invoke(result);

                result = Cmd.Execute("/c dotnet add package FluentValidation --version 11.7.1");
                SendToConsole?.Invoke(result);

                result = Cmd.Execute("/c dotnet add package Newtonsoft.Json --version 13.0.3");
                SendToConsole?.Invoke(result);

                result = Cmd.Execute("/c dotnet add package WooW.Blazor --version 2.5.1");
                //result = Cmd.Execute("/c dotnet add package WooW.Blazor.Test --version 2.0.3");
                SendToConsole?.Invoke(result);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar instalar los nuget en {ProjectName}: {ex.Message}"
                );
            }
        }

        #endregion Creación del proyecto e instalación de NuGets

        #region Limpieza y creación de ficheros

        /// <summary>
        /// Elimina los ficheros y carpetas que no son necesarios para el proyecto o que se van a reescribir.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void CleanProject()
        {
            try
            {
                /// Delete files

                if (File.Exists($@"{PathProject}/{ProjectName}.csproj"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/{ProjectName}.csproj");
                }

                if (File.Exists($@"{PathProject}/_Imports.razor"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/_Imports.razor");
                }

                if (File.Exists($@"{PathProject}/App.razor"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/App.razor");
                }

                if (File.Exists($@"{PathProject}/Program.cs"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/Program.cs");
                }

                if (File.Exists($@"{PathProject}/libman.json"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/libman.json");
                }

                /// Delete directories

                if (Directory.Exists($@"{PathProject}/wwwroot/css/open-iconic"))
                {
                    WoDirectory.DeleteDirectory($@"{PathProject}/wwwroot/css/open-iconic");
                }

                if (Directory.Exists($@"{PathProject}/Shared"))
                {
                    WoDirectory.DeleteDirectory($@"{PathProject}/Shared");
                }

                if (Directory.Exists($@"{PathProject}/Pages"))
                {
                    WoDirectory.DeleteDirectory($@"{PathProject}/Pages");
                }

                if (Directory.Exists($@"{PathProject}/Data"))
                {
                    WoDirectory.DeleteDirectory($@"{PathProject}/Data");
                }

                if (Directory.Exists($@"{PathProject}/wwwroot/sample-data"))
                {
                    WoDirectory.DeleteDirectory($@"{PathProject}/wwwroot/sample-data");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar limpiar el proyecto {ProjectName} en {PathProject}: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Crea los directorios necesarios para la generación de un proyecto server side.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void CreateDirectories()
        {
            try
            {
                //WoDirectory.CreateDirectory($@"{PathProject}/Properties");
                WoDirectory.CreateDirectory($@"{PathProject}/Auth");
                WoDirectory.CreateDirectory($@"{PathProject}/Controllers");
                WoDirectory.CreateDirectory($@"{PathProject}/ControlModels");
                WoDirectory.CreateDirectory($@"{PathProject}/FluentValidators");
                WoDirectory.CreateDirectory($@"{PathProject}/Localizer");
                WoDirectory.CreateDirectory($@"{PathProject}/Pages");
                WoDirectory.CreateDirectory($@"{PathProject}/ReportForms");
                WoDirectory.CreateDirectory($@"{PathProject}/Reports");
                WoDirectory.CreateDirectory($@"{PathProject}/Resources");
                WoDirectory.CreateDirectory($@"{PathProject}/Shared");
                WoDirectory.CreateDirectory($@"{PathProject}/Slaves");
                WoDirectory.CreateDirectory($@"{PathProject}/TransitionSettings");
                WoDirectory.CreateDirectory($@"{PathProject}/UserCode");
                WoDirectory.CreateDirectory($@"{PathProject}/Themes");
                WoDirectory.CreateDirectory($@"{PathProject}/wwwroot/Js");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar crear los directorios para el proyecto {ProjectName}: {ex.Message}"
                );
            }
        }

        #endregion Limpieza y creación de ficheros


        #region Ficheros en _proj/Shared

        /// <summary>
        /// Construye los ficheros para el proyecto de server en _proj/Shared
        /// </summary>
        private void BuildSharedFiles()
        {
            WoTemplateWoHeaderLayoutPartialServer woTemplateWoHeaderLayoutPartialServer =
                new WoTemplateWoHeaderLayoutPartialServer();
            woTemplateWoHeaderLayoutPartialServer.Project = ProjectName;
            WoDirectory.WriteTemplate(
                pathTemplate: $@"{PathProject}/Shared/WoHeaderLayout.razor.cs",
                data: woTemplateWoHeaderLayoutPartialServer.TransformText()
            );
        }

        #endregion Ficheros en _proj/Shared

        #region Ficheros en _proj

        /// <summary>
        /// Construye los ficheros en proyecto base _proj
        /// </summary>
        private void BuildBaseFiles()
        {
            try
            {
                WoTemplateCsProjectServer woTemplateCsProjectServer =
                    new WoTemplateCsProjectServer();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/{ProjectName}.csproj",
                    data: woTemplateCsProjectServer.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj de Server {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj

        #region Ficheros en _proj/Properties

        /// <summary>
        /// Construye  los ficheros en proyecto base _proj/Properties
        /// </summary>
        private void BuildPropertyFiles()
        {
            try
            {
                string path = $@"{PathProject}/Properties/launchSettings.json";
                if (File.Exists(path))
                {
                    string rawData = WoDirectory.ReadFile(path);
                    if (rawData.Contains(":5101") || rawData.Contains(":5100"))
                    {
                        StringBuilder newData = new StringBuilder();
                        string data = WoDirectory.ReadFile(path);
                        string[] lines = data.Split("\n");
                        foreach (string line in lines)
                        {
                            if (line.Contains("applicationUrl"))
                            {
                                newData.AppendLine(
                                    "\"applicationUrl\": \"https://localhost:7208;http://localhost:7209\","
                                );
                            }
                            else
                            {
                                newData.AppendLine(line);
                            }
                        }

                        WoDirectory.WriteFile(path, newData.ToString());
                    }
                }

                //WoTemplateLaunchSettingsServer woTemplateLaunchServer =
                //    new WoTemplateLaunchSettingsServer();
                //WoDirectory.WriteTemplate(
                //    pathTemplate: $@"{PathProject}/Properties/launchSettings.json",
                //    data: woTemplateLaunchServer.TransformText()
                //);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/Properties de Server {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Properties

        #region Ficheros en _proj/Pages

        /// <summary>
        /// Construye los ficheros en proyecto base _proj/Pages en server.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildPagesFiles()
        {
            try
            {
                WoTemplateHostCsHtmlServer woTemplateHostCsHtmlServer =
                    new WoTemplateHostCsHtmlServer();
                woTemplateHostCsHtmlServer.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Pages/_Host.cshtml",
                    data: woTemplateHostCsHtmlServer.TransformText()
                );

                WoTemplateLayoutServer woTemplateLayoutServer = new WoTemplateLayoutServer();
                woTemplateLayoutServer.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Pages/_Layout.cshtml",
                    data: woTemplateLayoutServer.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/Pages para server: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Pages

        #region Ficheros en _proj/Controllers

        /// <summary>
        /// Escribe los ficheros en Controllers para server.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildControllersFiles()
        {
            try
            {
                WoTemplateCultureControllerServer woTemplateCultureControllerServer =
                    new WoTemplateCultureControllerServer();
                woTemplateCultureControllerServer.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Controllers/CultureController.cs",
                    data: woTemplateCultureControllerServer.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/Controllers para server: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Controllers
    }
}
