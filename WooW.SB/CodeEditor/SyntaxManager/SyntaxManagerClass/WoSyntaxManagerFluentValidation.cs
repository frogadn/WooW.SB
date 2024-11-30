using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass
{
    public class WoSyntaxManagerFluentValidation : AWoSyntaxManagerConstructorBase, IWoSyntaxManager
    {
        #region Implementación de la interfaz

        /// <summary>
        /// Inicializa los atributos base de la clase y carga el código de los ficheros a Roslyn.
        /// </summary>
        /// <param name="pathScript"></param>
        /// <param name="modelName"></param>
        public void InitializeManager(string pathScript, string className, string modelName)
        {
            PathScript = pathScript;
            ClassName = className;
            ModelName = modelName;

            ChargeCode();
        }

        #endregion Implementación de la interfaz
    }
}
