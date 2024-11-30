using System.Runtime.Versioning;
using System.Windows.Forms;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using WooW.Core;

namespace WooW.SB.CodeEditor.CodeComponents
{
    public partial class WoHeaderEditor : UserControl
    {
        #region Constructores

        [SupportedOSPlatform("windows")]
        public WoHeaderEditor(CSharpSyntaxLanguage language)
        {
            InitializeComponent();

            syeHeader.Document.Language = language;
        }

        #endregion Constructores

        #region Definir header y footer

        /// <summary>
        /// Define el header y el footer para el syntax editor.
        /// </summary>
        /// <param name="code"></param>
        public void AssignHeaderAndFooter((string header, string footer) code)
        {
            syeHeader.Document.SetHeaderAndFooterText(code.header, code.footer);
        }

        #endregion Definir header y footer

        #region Preguntas a la clase

        /// <summary>
        /// Retorna true o false en función del estado del editor de código.
        /// Tienen código : true
        /// No tiene código : false
        /// </summary>
        /// <returns></returns>
        public bool HaveCode()
        {
            return !syeHeader.Text.IsNullOrStringEmpty();
        }

        #endregion Preguntas a la clase

        #region Gestión del código

        /// <summary>
        /// Remplaza el código actual del syntax por el que se recibe por parámetro.
        /// </summary>
        /// <param name="code"></param>
        public void SetCode(string code)
        {
            syeHeader.Text = string.Empty;
            syeHeader.Text = code;
        }

        /// <summary>
        /// Retorna el código actual del syntax.
        /// </summary>
        /// <returns></returns>
        public string GetCode()
        {
            return syeHeader.Text;
        }

        /// <summary>
        /// Limpia el editor de código.
        /// </summary>
        public void CleanEditor()
        {
            syeHeader.Text = string.Empty;
        }

        #endregion Gestión del código
    }
}
