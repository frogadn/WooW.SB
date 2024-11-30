using System;
using System.Text;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.Reports
{
    public class WoReportRazorGenerator
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Resultado final del razor generado.
        /// </summary>
        private StringBuilder _finalRazor = new StringBuilder();

        /// <summary>
        /// Item del que se esta generando el razor.
        /// </summary>
        private WoItem _item = new WoItem();

        #endregion Atributos

        #region Instancias generadoras

        /// <summary>
        /// Instancia del generador del componente del razor
        /// </summary>
        private WoReportViewerRazor _woReportViewerRazor = new WoReportViewerRazor();

        #endregion Instancias generadoras

        #region Identación

        /// <summary>
        /// Nivel de identación para grupos internos y controles.
        /// </summary>
        private int _identLevel = -1;

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de la clase;
        /// </summary>
        private String _identGroups = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de las propiedades;
        /// </summary>
        private String _identItems = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado del código dentro de los métodos;
        /// </summary>
        private String _identItemProperties = "";

        /// <summary>
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identGroups = FormatClassHelper.Ident(_identLevel);
            _identItems = FormatClassHelper.Ident(_identLevel + 1);
            _identItemProperties = FormatClassHelper.Ident(_identLevel + 2);
        }

        #endregion Identación

        #region Método principal

        /// <summary>
        /// Retorna el razor para escribirse en el proyecto
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pathPage"></param>
        /// <returns></returns>
        public string GetRazor(WoItem item, string pathPage, string projectName, string formName)
        {
            _item = item;

            _finalRazor.Clear();

            _finalRazor.AppendLine(
                $@"
@page ""/{pathPage}""

@inherits AWoComponentBase

@attribute [Authorize]"
            );

            _finalRazor.AppendLine(BuildBody(projectName, formName));

            return _finalRazor.ToString();
        }

        #endregion Método principal

        #region Body

        /// <summary>
        /// Retorna el cuerpo del razor de una pantalla de reporte
        /// </summary>
        /// <returns></returns>
        private string BuildBody(string projectName, string formName)
        {
            //            CalculateIdentSpaces();

            //            (string open, string close) reportViewer = _woReportViewerRazor.GetReportViewRazor(
            //                identItem: _identItems,
            //                identItemProperty: _identItemProperties,
            //                woItem: _item
            //            );

            //            return $@"
            //{reportViewer.open}

            //<{projectName}.Pages.{formName}Layout/>

            //{reportViewer.close}

            //@*ToDo: Solo en lo que se acomoda un diseño mejor*@
            //<WoButton OnClick=""@BuildSharedUrl""
            //          ButtonType=""eButtonColor.Success""
            //          ColSpan=6
            //          Caption=""Share this report""/>
            //<p>@_sharedUrl</p>
            //";
            return "";
        }

        #endregion Body
    }
}
