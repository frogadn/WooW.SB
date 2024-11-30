using System;
using System.Text;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.Reports
{
    public class WoReportPartialGenerator
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
        private StringBuilder _finalClass = new StringBuilder();

        /// <summary>
        /// Item del que se esta generando el razor.
        /// </summary>
        private WoItem _item = new WoItem();

        #endregion Atributos

        #region Constructor

        #region Instancias generadoras

        /// <summary>
        /// Instancia del generador del componente de la parcial del razor
        /// </summary>
        private WoReportViewerPartial _woReportViewerPartial = null;

        #endregion Instancias generadoras

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="woItem"></param>
        public WoReportPartialGenerator(WoItem woItem)
        {
            CalculateIdentSpaces();

            _item = woItem;

            _woReportViewerPartial = new WoReportViewerPartial(
                identMethodsAndProperties: _identMethodsAndProperties
            );
        }

        #endregion Constructor

        #region Identación

        /// <summary>
        /// Nivel de identación para grupos internos y controles.
        /// </summary>
        private int _identLevel = 0;

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de la clase;
        /// </summary>
        private String _identClass = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de las propiedades;
        /// </summary>
        private String _identMethodsAndProperties = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado del código dentro de los métodos;
        /// </summary>
        private String _identCode = "";

        /// <summary>
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identClass = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identMethodsAndProperties = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identCode = FormatClassHelper.Ident(_identLevel);
        }

        #endregion Identación

        #region Método principal

        /// <summary>
        /// Genera el código de la parcial del razor.
        /// </summary>
        /// <returns></returns>
        public string GetPartialClass(string classModelName, string formName)
        {
            BuildHeaderClass(classModelName);

            BuildBodyClass(formName);

            BuildFooterClass();

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Header

        private void BuildHeaderClass(string classModelName)
        {
            _finalClass.AppendLine(
                $@"
{_identClass}public partial class {classModelName}Report
{_identClass}{{"
            );
        }

        #endregion Header

        #region Body

        private void BuildBodyClass(string formName)
        {
            //_finalClass.AppendLine(_woReportViewerPartial.GetReportViewerPartial(_item, formName));
        }

        #endregion Body

        #region Footer

        private void BuildFooterClass()
        {
            _finalClass.AppendLine(
                $@"
{_identClass}}}"
            );
        }

        #endregion Footer
    }
}
