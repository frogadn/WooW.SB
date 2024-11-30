using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Pages;
using WooW.SB.BlazorGenerator.BlazorTemplates.ServerTemplates.Pages;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorGeneration.BlazorForm
{
    public class WoBlazorDefault
    {
        #region Instancias singleton

        private Proyecto _project = Proyecto.getInstance();

        public WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Nombre del proyecto sobre el que se esta trabajando.
        /// (Puede ser diferente al nombre del modelo en caso de no ser una unitaria).
        /// </summary>
        private string _projectName = string.Empty;

        /// <summary>
        /// Ruta del proyecto.
        /// </summary>
        private string _pathProject = string.Empty;

        #endregion Variables globales

        #region Constructor

        public WoBlazorDefault(string projectName)
        {
            _projectName = projectName;
            _pathProject = $@"{_project.DirApplication}/Temp/{projectName}";
        }

        #endregion Constructor

        public void BuildDefault(string referencia, string data = null)
        {
            StringBuilder template = new StringBuilder();
            template.AppendLine($@"@page ""/{referencia}""");
            template.AppendLine(data ?? $@"<h1>{referencia}</h1>");

            WoTemplateLayoutRazorBlazor woTemplateLayoutRazorBlazor =
                new WoTemplateLayoutRazorBlazor();
            woTemplateLayoutRazorBlazor.Template = template.ToString();

            string[] refCol = referencia.Split('/');
            string fileName = refCol.Last();

            WoDirectory.WriteTemplate(
                pathTemplate: $@"{_pathProject}/Pages/{fileName}.razor",
                data: woTemplateLayoutRazorBlazor.TransformText()
            );
        }

        /// <summary>
        /// Se ocupa de editar el fichero con la configuración base del HTML.
        /// </summary>
        public void BuildLayout(string theme)
        {
            WoTemplateLayoutServer woTemplateLayoutServer = new WoTemplateLayoutServer();
            woTemplateLayoutServer.Project = _projectName;
            woTemplateLayoutServer.ThemeName = theme;
            WoDirectory.WriteTemplate(
                pathTemplate: $@"{_project.DirProyectTemp}\{_projectName}\Pages\_Layout.cshtml",
                data: woTemplateLayoutServer.TransformText()
            );
        }

        #region Logs

        private WoLog _templateDefaultReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Pantalla default escrita correctamente.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoTextEditPartial",
                MethodOrContext = "GetTextEditPartial"
            }
        };

        #endregion Logs
    }
}
