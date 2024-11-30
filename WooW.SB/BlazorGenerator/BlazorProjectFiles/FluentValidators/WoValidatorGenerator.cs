using System;
using System.Text;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.FluentValidators
{
    public class WoValidatorGenerator
    {
        #region Variables globales

        /// <summary>
        /// Indica el nombre de la clase y del fichero.
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        #endregion Variables globales


        #region Constructores

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="classModelName"></param>
        public WoValidatorGenerator(string classModelName)
        {
            _classModelName = classModelName;
        }

        #endregion Constructores


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
        /// Metodo principal que genera la clase.
        /// </summary>
        /// <returns></returns>
        public string GetValidatorClass()
        {
            CalculateIdentSpaces();

            BuildHeader();

            BuildFooter();

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Header

        /// <summary>
        /// Construlle el header de la clase.
        /// </summary>
        private void BuildHeader()
        {
            _finalClass.AppendLine(
                $@"
{_identClass}public class {_classModelName}Validator : AbstractValidator<{_classModelName}>
{_identClass}{{
{_identClass}    public {_classModelName}Validator()
{_identClass}    {{"
            );
        }

        #endregion Header

        #region Footer

        /// <summary>
        /// Construye el footer de la clase.
        /// </summary>
        private void BuildFooter()
        {
            _finalClass.AppendLine(
                $@"
{_identMethodsAndProperties}}}
{_identClass}}}"
            );
        }

        #endregion Footer
    }
}
