using System;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using DevExpress.XtraEditors;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorTemplates.WasmTemplates;
using WooW.SB.BlazorGenerator.BlazorTemplates.WasmTemplates.Localizer;
using WooW.SB.BlazorGenerator.BlazorTemplates.WasmTemplates.Shared;
using WooW.SB.BlazorGenerator.BlazorTemplates.WasmTemplates.WwwRoot;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorProjects
{
    public class WoBlazorProjectWasm : AWoBlazorProjectClass
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
        /// El path y el cmdInstaller se inicializa de forma interna igual en el constructor.
        /// </summary>
        /// <param name="projectName"></param>
        public WoBlazorProjectWasm(string projectName)
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

                /// Wasm class
                BuildBaseFiles();
                BuildPropertiesFiles();
                BuildWwwrootFiles();
                BuildLocalizerFiles();
                BuildSharedFiles();
            }
            catch (Exception ex)
            {
                //send to alert log
                XtraMessageBox.Show(
                    $@"Ocurrió un error al intentar generar el proyecto WASM. {ex.Message}",
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

                string result = Cmd.Execute("/c dotnet new blazorwasm");
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
        /// Remueve ficheros y carpetas del proyecto de blazor que no son necesarios o que se van a reescribir.
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

                if (File.Exists($@"{PathProject}/service-worker.js"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/service-worker.js");
                }

                if (File.Exists($@"{PathProject}/libman.json"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/libman.json");
                }

                if (File.Exists($@"{PathProject}/wwwroot/index.html"))
                {
                    WoDirectory.DeleteFile($@"{PathProject}/wwwroot/index.html");
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

                //if (Directory.Exists($@"{PathProject}/Properties"))
                //{
                //    WoDirectory.DeleteDirectory($@"{PathProject}/Properties");
                //}

                if (Directory.Exists($@"{PathProject}/sample-data"))
                {
                    WoDirectory.DeleteDirectory($@"{PathProject}/sample-data");
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
        /// Crea los directorios necesarios para la generación de un proyecto web assembly.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void CreateDirectories()
        {
            try
            {
                //WoDirectory.CreateDirectory($@"{PathProject}/Properties");
                WoDirectory.CreateDirectory($@"{PathProject}/Auth");
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


        #region Ficheros en _proj

        /// <summary>
        /// Construye los ficheros en el path _proj.
        /// Solo para web assembly.
        /// </summary>
        private void BuildBaseFiles()
        {
            try
            {
                WoTemplateCsProjectWasm woTemplateCsProjectWasm = new WoTemplateCsProjectWasm();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/{ProjectName}.csproj",
                    data: woTemplateCsProjectWasm.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar escribir las T4 para el proyecto base de Wasm: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj

        #region Ficheros en _proj/Shared

        /// <summary>
        /// Construye los ficheros para el proyecto de server en _proj/Shared
        /// </summary>
        private void BuildSharedFiles()
        {
            WoTemplateWoHeaderLayoutPartialWasm woTemplateWoHeaderLayoutPartialWasm =
                new WoTemplateWoHeaderLayoutPartialWasm();
            woTemplateWoHeaderLayoutPartialWasm.Project = ProjectName;
            WoDirectory.WriteTemplate(
                pathTemplate: $@"{PathProject}/Shared/WoHeaderLayout.razor.cs",
                data: woTemplateWoHeaderLayoutPartialWasm.TransformText()
            );
        }

        #endregion Ficheros en _proj/Shared

        #region Ficheros en _proj/Properties

        /// <summary>
        /// Construye los ficheros en el path _proj/Properties.
        /// </summary>
        private void BuildPropertiesFiles()
        {
            try
            {
                string path = $@"{PathProject}/Properties/launchSettings.json";
                if (File.Exists(path))
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

                //WoTemplateLaunchSettingsWasm woTemplateLaunchSettingsWasm =
                //    new WoTemplateLaunchSettingsWasm();
                //woTemplateLaunchSettingsWasm.Project = ProjectName;
                //WoDirectory.WriteTemplate(
                //    pathTemplate: $@"{PathProject}/Properties/launchSettings.json",
                //    data: woTemplateLaunchSettingsWasm.TransformText()
                //);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar escribir las T4 para la carpeta de properties de wasm: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Properties

        #region Ficheros en _proj/wwwroot

        /// <summary>
        /// Construye los proyectos en el path _proj/wwwroot.
        /// </summary>
        private void BuildWwwrootFiles()
        {
            try
            {
                WoTemplateIndexHtmlWasm woTemplateIndexHtmlWasm = new WoTemplateIndexHtmlWasm();
                woTemplateIndexHtmlWasm.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/wwwroot/index.html",
                    data: woTemplateIndexHtmlWasm.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar escribir las T4 en wwwroot en wasm: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/wwwroot

        #region Ficheros en _proj/Localizer

        /// <summary>
        /// Escribe los ficheros en el path _proj/Localizer.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildLocalizerFiles()
        {
            try
            {
                WoTemplateWebAssemblyHostExtensionsWasm woTemplateWebAssemblyHostExtensions =
                    new WoTemplateWebAssemblyHostExtensionsWasm();
                woTemplateWebAssemblyHostExtensions.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Localizer/WebAssemblyHostExtensions.cs",
                    data: woTemplateWebAssemblyHostExtensions.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar escribir las T4 en Localizer en wasm: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Localizer
    }
}
