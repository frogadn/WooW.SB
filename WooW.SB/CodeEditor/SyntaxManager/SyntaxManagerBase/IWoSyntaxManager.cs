namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase
{
    public interface IWoSyntaxManager
    {
        /// <summary>
        /// Metodo principal de cualquier clase gestora de codigo.
        /// Inicializa el arbol de codigo y carga lo nesesario para el funcionamiento de
        /// los metodos de la clase.
        /// </summary>
        /// <param name="pathScriptCopy"></param>
        /// <param name="modelName"></param>
        void InitializeManager(string pathScript, string className, string modelName);
    }
}
