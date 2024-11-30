using System;
using System.Text;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.UserCode
{
    public class WoScriptsUserPartialGenerator
    {
        #region Variables globales

        /// <summary>
        /// Contiene el nombre de la clase y del fichero que se generara
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        #endregion Variables globales

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="classModelName"></param>
        public WoScriptsUserPartialGenerator(string classModelName)
        {
            _classModelName = classModelName;

            CalculateIdentSpaces();
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
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identClass = FormatClassHelper.Ident(_identLevel);
        }

        #endregion Identación


        #region Método principal

        /// <summary>
        /// Método principal que retorna la clase ya construida, igual se ocupa de orquestar
        /// el resto de métodos.
        /// </summary>
        /// <returns></returns>
        public string GetScriptsUserClass()
        {
            CalculateIdentSpaces();

            BuildHeaderClass();

            BuilFooterClass();

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Header

        /// <summary>
        /// Construye el Header de la clase.
        /// </summary>
        private void BuildHeaderClass()
        {
            _finalClass.AppendLine(
                $@"
{_identClass}public partial class {_classModelName}ScriptsUser
{_identClass}{{
"
            );
        }

        #endregion Header

        #region Footer

        /// <summary>
        /// Construye el footer de la clase.
        /// </summary>
        private void BuilFooterClass()
        {
            _finalClass.AppendLine($@"{_identClass}}}");
        }

        #endregion Footer
    }
}
