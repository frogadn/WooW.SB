using System;
using System.Text;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Auth;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.ControlModels;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.FluentValidators;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Localizer;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Pages;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Shared;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Themes;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.UserCode;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.WwwRoot.Css;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.WwwRoot.Js;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorProjects
{
    public abstract class AWoBlazorProjectClass
    {
        #region Instancias singleton

        /// <summary>
        /// Observador que permite enviar logs.
        /// </summary>
        protected WoLogObserver Observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Instancia singleton de la clase contodas las configuraciones del proyecto con el
        /// que se esta trabajando.
        /// </summary>
        protected Proyecto Project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Variables globales

        /// <summary>
        /// Nombre del proyecto que se generar.
        /// </summary>
        protected string ProjectName = string.Empty;

        /// <summary>
        /// Ruta del proyecto que se va a generar.
        /// </summary>
        protected string PathProject = string.Empty;

        /// <summary>
        /// Declaración de la variable de la clase que instala los componentes de la generación.
        /// </summary>
        protected WoCmdInstaller Cmd = null;

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Variables globales


        #region Creación del proyecto e instalación de NuGets (Métodos abstractos)

        /// <summary>
        /// Método que ejecuta los comandos requeridos para crear el proyecto.
        /// </summary>
        protected abstract void CreateBlazor();

        /// <summary>
        /// Método que instala los paquetes NuGet desde el CLI.
        /// </summary>
        protected abstract void ChargeNugets();

        #endregion Creación del proyecto e instalación de NuGets (Métodos abstractos)


        #region Limpieza y creación de ficheros (Métodos abstractos)

        /// <summary>
        /// Elimina todo lo que se va a reescribir.
        /// </summary>
        protected abstract void CleanProject();

        /// <summary>
        /// Agrega las carpetas que se eliminaron o que asen falta en el proyecto.
        /// </summary>
        protected abstract void CreateDirectories();

        #endregion Limpieza y creación de ficheros (Métodos abstractos)


        #region Ficheros en _proj

        /// <summary>
        /// Genera los ficheros comunes para los proyectos de blazor en el path _proj.
        /// </summary>
        protected void BuildGenericBaseProject()
        {
            try
            {
                WoTemplateAppBlazor woTemplateAppBlazor = new WoTemplateAppBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/App.razor",
                    data: woTemplateAppBlazor.TransformText()
                );

                WoTemplateLibmanBlazor woTemplateLibmanBlazor = new WoTemplateLibmanBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/libman.json",
                    data: woTemplateLibmanBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj

        #region Ficheros en _proj/Auth

        /// <summary>
        /// Función para construir los archivo de de la carpeta Auth
        /// </summary>
        protected void BuildAuthFiles()
        {
            try
            {
                WoTemplateAWoComponentBaseBlazor woTemplateAWoComponentBaseBlazor =
                    new WoTemplateAWoComponentBaseBlazor();
                woTemplateAWoComponentBaseBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Auth/AWoComponentBase.cs",
                    data: woTemplateAWoComponentBaseBlazor.TransformText()
                );

                WoTemplateWoAuthenticationStateProviderBlazor woTemplateWoAuthenticationStateProviderBlazor =
                    new WoTemplateWoAuthenticationStateProviderBlazor();
                woTemplateWoAuthenticationStateProviderBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Auth/WoAuthenticationStateProvider.cs",
                    data: woTemplateWoAuthenticationStateProviderBlazor.TransformText()
                );

                WoTemplateSessionDataBlazor woTemplateSessionDataBlazor =
                    new WoTemplateSessionDataBlazor();
                woTemplateSessionDataBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Auth/SessionData.cs",
                    data: woTemplateSessionDataBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/Auth: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Auth

        #region Ficheros en _proj/ControlModels

        /// <summary>
        /// Función para construir los archivo de de la carpeta Control Models
        /// </summary>
        protected void BuildControlModels()
        {
            try
            {
                WoTemplateLoginControlsBlazor woTemplateLoginControlsBlazor =
                    new WoTemplateLoginControlsBlazor();
                woTemplateLoginControlsBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/ControlModels/WoLoginControls.cs",
                    data: woTemplateLoginControlsBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/ControlModels: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/ControlModels

        #region Ficheros en _proj/FluentValidators

        /// <summary>
        /// Función para construir los archivo de de la carpeta Fluent Validators
        /// </summary>
        protected void BuildFluentValidators()
        {
            try
            {
                WoTemplateAutenticateValidatorBlazor woTemplateAutenticateValidatorBlazor =
                    new WoTemplateAutenticateValidatorBlazor();
                woTemplateAutenticateValidatorBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/FluentValidators/AutenticateValidator.cs",
                    data: woTemplateAutenticateValidatorBlazor.TransformText()
                );
                WoTemplateInstanciaUdnAsignarValidatorBlazor woTemplateInstanciaUdnAsignarValidatorBlazor =
                    new WoTemplateInstanciaUdnAsignarValidatorBlazor();
                woTemplateInstanciaUdnAsignarValidatorBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/FluentValidators/InstanciaUdnAsignarValidator.cs",
                    data: woTemplateInstanciaUdnAsignarValidatorBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/FluentValidators: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/FluentValidators

        #region Ficheros en _proj/Localizer

        /// <summary>
        /// Función para construir los archivo de de la carpeta Localizer
        /// </summary>
        protected void BuildLocalizer()
        {
            try
            {
                WoTemplateCultureWithNameBlazor woTemplateCultureWithNameBlazor =
                    new WoTemplateCultureWithNameBlazor();
                woTemplateCultureWithNameBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Localizer/CultureWithName.cs",
                    data: woTemplateCultureWithNameBlazor.TransformText()
                );

                WoTemplateLocalizerSettingsBlazor woTemplateLocalizerSettingsBlazor =
                    new WoTemplateLocalizerSettingsBlazor();
                woTemplateLocalizerSettingsBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Localizer/LocalizerSettings.cs",
                    data: woTemplateLocalizerSettingsBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/Localizer: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Localizer

        #region Ficheros en _proj/Pages

        /// <summary>
        /// Función para construir los archivos de la carpeta pages
        /// </summary>
        protected void BuildPages(string code)
        {
            try
            {
                WoTemplateIndexRazorBlazor woTemplateIndexRazorBlazor =
                    new WoTemplateIndexRazorBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Pages/Index.razor",
                    data: woTemplateIndexRazorBlazor.TransformText()
                );

                WoTemplateIndexPartialBlazor woTemplateIndexPartialBlazor =
                    new WoTemplateIndexPartialBlazor();
                woTemplateIndexPartialBlazor.Project = ProjectName;
                woTemplateIndexPartialBlazor.Code = code;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Pages/Index.razor.cs",
                    data: woTemplateIndexPartialBlazor.TransformText()
                );

                WoTemplateLogRazorBlazor woTemplateLogRazorBlazor = new WoTemplateLogRazorBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Pages/WoLog.razor",
                    data: woTemplateLogRazorBlazor.TransformText()
                );

                WoTemplateLogPartialBlazor woTemplateLogPartialBlazor =
                    new WoTemplateLogPartialBlazor();
                woTemplateLogPartialBlazor.Project = ProjectName;
                woTemplateLogPartialBlazor.Model = "";
                woTemplateLogPartialBlazor.Id = "";
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Pages/WoLog.razor.cs",
                    data: woTemplateLogPartialBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/Pages: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Método de generation de la pagina ocupada de la redirección de la urn.
        /// </summary>
        public void GenerateUrnRedirection()
        {
            // Escritura del codigo razor de la página de redirección de la urn.
            WoTemplateWoUrnRedirectionRazorBlazor woTemplateWoUrnRedirectionRazorBlazor =
                new WoTemplateWoUrnRedirectionRazorBlazor();
            WoDirectory.WriteTemplate(
                pathTemplate: $@"{PathProject}\Pages\WoUrnRedirection.razor",
                data: woTemplateWoUrnRedirectionRazorBlazor.TransformText()
            );

            // Escritura del codigo C# de la pagina de redirección de la urn.
            WoTemplateWoUrnRedirectionPartialBlazor woTemplateWoUrnRedirectionPartialBlazor =
                new WoTemplateWoUrnRedirectionPartialBlazor();

            StringBuilder strModelsData = new StringBuilder();
            foreach (Modelo model in _project.ModeloCol.Modelos)
            {
                strModelsData.AppendLine(
                    $@"{{ ""{model.Id.ToLower()}"", (""{model.TipoModelo}"", ""{model.ProcesoId}"") }},"
                );
            }

            woTemplateWoUrnRedirectionPartialBlazor.Project = ProjectName;
            woTemplateWoUrnRedirectionPartialBlazor.Models = strModelsData.ToString();
            WoDirectory.WriteTemplate(
                pathTemplate: $@"{PathProject}\Pages\WoUrnRedirection.razor.cs",
                data: woTemplateWoUrnRedirectionPartialBlazor.TransformText()
            );
        }

        #endregion Ficheros en _proj/Pages


        #region Ficheros en _proj/Shared

        /// <summary>
        /// Función para construir los archivos de la carpeta Shared
        /// </summary>
        protected void BuildShared()
        {
            try
            {
                WoTemplateWoLoginLayoutPartialBlazor woTemplateWoLoginLayoutPartialBlazor =
                    new WoTemplateWoLoginLayoutPartialBlazor();
                woTemplateWoLoginLayoutPartialBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Shared/WoLoginLayout.razor.cs",
                    data: woTemplateWoLoginLayoutPartialBlazor.TransformText()
                );
                WoTemplateWoLoginLayoutRazorBlazor woTemplateWoLoginLayoutRazorBlazor =
                    new WoTemplateWoLoginLayoutRazorBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Shared/WoLoginLayout.razor",
                    data: woTemplateWoLoginLayoutRazorBlazor.TransformText()
                );

                WoTemplateMainLayoutPartialBlazor woTemplateMainLayoutPartialBlazor =
                    new WoTemplateMainLayoutPartialBlazor();
                woTemplateMainLayoutPartialBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Shared/MainLayout.razor.cs",
                    data: woTemplateMainLayoutPartialBlazor.TransformText()
                );
                WoTemplateMainLayoutRazorBlazor woTemplateMainLayoutRazorBlazor =
                    new WoTemplateMainLayoutRazorBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Shared/MainLayout.razor",
                    data: woTemplateMainLayoutRazorBlazor.TransformText()
                );
                WoTemplateMainLayoutCssBlazor woTemplateMainLayoutCssBlazor =
                    new WoTemplateMainLayoutCssBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Shared/MainLayout.razor.css",
                    data: woTemplateMainLayoutCssBlazor.TransformText()
                );

                WoTemplateWoHeaderSubItemsRazorBlazor woTemplateWoHeaderSubItemsRazorBlazor =
                    new WoTemplateWoHeaderSubItemsRazorBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Shared/WoHeaderSubItems.razor",
                    data: woTemplateWoHeaderSubItemsRazorBlazor.TransformText()
                );

                WoTemplateWoHeaderLayoutRazorBlazor woTemplateWoHeaderLayoutRazorBlazor =
                    new WoTemplateWoHeaderLayoutRazorBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Shared/WoHeaderLayout.razor",
                    data: woTemplateWoHeaderLayoutRazorBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/Shared: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Shared

        #region Ficheros en _proj/Themes

        /// <summary>
        /// Función para construir los archivos de la carpeta Themes
        /// </summary>
        protected void BuildThemes()
        {
            try
            {
                WoTemplateWoThemeSwitcherRazorBlazor woTemplateWoThemeSwitcherRazorBlazor =
                    new WoTemplateWoThemeSwitcherRazorBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Themes/WoThemeSwitcher.razor",
                    data: woTemplateWoThemeSwitcherRazorBlazor.TransformText()
                );

                WoTemplateWoThemeSwitcherPartialBlazor woTemplateWoThemeSwitcherPartialBlazor =
                    new WoTemplateWoThemeSwitcherPartialBlazor();
                woTemplateWoThemeSwitcherPartialBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/Themes/WoThemeSwitcher.razor.cs",
                    data: woTemplateWoThemeSwitcherPartialBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/Themes: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/Themes

        #region Ficheros en _proj/wwwroot

        /// <summary>
        /// Función para copiar los archivos a la carpeta wwwroot/css
        /// </summary>
        protected void CopyWwwrootFiles()
        {
            try
            {
                string cssIconsOldPath =
                    $@"{_project.DirProyectData}/LayOuts/BlazorLibraries/Assets";
                string cssIconsNewPath = $@"{PathProject}/wwwroot/Assets";

                WoDirectory.CopyDirectory(cssIconsOldPath, cssIconsNewPath);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudo copiar los archivos css a _proj/wwwroot/css: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/wwwroot

        #region Ficheros en _proj/wwwroot/css

        /// <summary>
        /// Función para construir los archivos de la carpeta wwwroot/css
        /// </summary>
        protected void BuildCssFiles()
        {
            try
            {
                WoTemplateStyleComponentsBlazor woTemplateStyleComponentsBlazor =
                    new WoTemplateStyleComponentsBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/wwwroot/css/StyleComponents.css",
                    data: woTemplateStyleComponentsBlazor.TransformText()
                );

                WoTemplateSiteBlazor woTemplateSiteBlazor = new WoTemplateSiteBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/wwwroot/css/site.css",
                    data: woTemplateSiteBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/wwwroot/css: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Función para copiar los archivos a la carpeta wwwroot/css
        /// </summary>
        protected void CopyCssFiles()
        {
            try
            {
                string cssIconsOldPath = $@"{_project.DirProyectData}/LayOuts/BlazorLibraries";
                string cssIconsNewPath = $@"{PathProject}/wwwroot/css";

                WoDirectory.CopyDirectory(cssIconsOldPath, cssIconsNewPath);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudo copiar los archivos css a _proj/wwwroot/css: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/wwwroot/css

        #region Ficheros en _proj/wwwroot/Js

        /// <summary>
        /// Función para construir los archivos de la carpeta wwwroot/css
        /// </summary>
        protected void BuildJsFiles()
        {
            try
            {
                WoTemplateWoScriptsBlazor woTemplateWoScriptsBlazor =
                    new WoTemplateWoScriptsBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/wwwroot/Js/WoScripts.js",
                    data: woTemplateWoScriptsBlazor.TransformText()
                );

                WoTemplateWoThemeSwitcherBlazor woTemplateWoThemeSwitcherBlazor =
                    new WoTemplateWoThemeSwitcherBlazor();
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/wwwroot/Js/WoThemeSwitcher.js",
                    data: woTemplateWoThemeSwitcherBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/wwwroot/js: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/wwwroot/Js


        #region Ficheros en _proj/UserCode

        /// <summary>
        /// Escribe los ficheros en la carpeta UserCode.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void BuildUserCode()
        {
            try
            {
                WoTemplateLoginScriptsUserBlazor woTemplateLoginScriptsUserBlazor =
                    new WoTemplateLoginScriptsUserBlazor();
                woTemplateLoginScriptsUserBlazor.Project = ProjectName;
                WoDirectory.WriteTemplate(
                    pathTemplate: $@"{PathProject}/UserCode/LoginScriptsUser.cs",
                    data: woTemplateLoginScriptsUserBlazor.TransformText()
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"no se pudieron escribir las plantillas en _proj/UserCode: {ex.Message}"
                );
            }
        }

        #endregion Ficheros en _proj/UserCode
    }
}
