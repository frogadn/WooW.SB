using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase
{
    public abstract class AWoSyntaxManagerBase
    {
        /// <summary>
        /// Árbol de código sobre el que se trabajara.
        /// </summary>
        protected SyntaxTree SyntaxTree = null;

        /// <summary>
        /// Raíz del código sobre de la que se puede generar, buscar o eliminar código.
        /// </summary>
        protected CompilationUnitSyntax Root = null;

        /// <summary>
        /// Ruta del fichero de código sobre el que se trabajara.
        /// </summary>
        protected string PathScript = string.Empty;

        /// <summary>
        /// Nombre del modelo sobre del que se esta editando.
        /// </summary>
        protected string ModelName = string.Empty;

        /// <summary>
        /// Nombre base que se le dará a las clases en el caso de las unitarias.
        /// </summary>
        protected string ClassName = string.Empty;

        /// <summary>
        /// Carga el código al SyntaxTree que permite trabajar con el fichero a modo de árbol.
        /// </summary>
        protected void ChargeCode()
        {
            string raw = WoDirectory.ReadFile(PathScript).Replace("using Microsoft.JSInterop;", "");
            SyntaxTree = CSharpSyntaxTree.ParseText(raw);
            Root = (CompilationUnitSyntax)SyntaxTree.GetRoot();
        }
    }
}
