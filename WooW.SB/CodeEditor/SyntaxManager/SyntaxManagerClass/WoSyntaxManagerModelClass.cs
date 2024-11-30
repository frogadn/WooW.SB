using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WooW.Core;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass
{
    public class WoSyntaxManagerModelClass : AWoSyntaxManagerAttributeBase, IWoSyntaxManager
    {
        #region Implementación de la interfaz

        /// <summary>
        /// Inicializa los atributos base de la clase y carga el código de los ficheros a Roslyn.
        /// </summary>
        /// <param name="pathScript"></param>
        /// <param name="pathScriptSave"></param>
        /// <param name="modelName"></param>
        public void InitializeManager(string pathScript, string className, string modelName)
        {
            PathScript = pathScript;
            ClassName = className;
            ModelName = modelName;

            ChargeCode();
        }

        #endregion Implementación de la interfaz

        #region Métodos

        /// <summary>
        /// Usa el método de la clase abstracta para recuperar la clase base del
        /// fichero del modelo.
        /// Recupera la información ya formateada para cargar al syntax.
        /// </summary>
        /// <returns></returns>
        public string GetModelClass()
        {
            return SearchClass(ModelName).ToString();
        }

        /// <summary>
        /// Recupera la lista de enums del fichero del modelo.
        /// Recupera la información ya formateada para cargar al syntax.
        /// </summary>
        /// <returns></returns>
        public string GetEnums()
        {
            StringBuilder strEnums = new StringBuilder();

            IEnumerable<EnumDeclarationSyntax> findEnumsCol =
                from clase in Root.DescendantNodes().OfType<EnumDeclarationSyntax>()
                select clase;

            if (!findEnumsCol.IsNull())
            {
                foreach (var findEnum in findEnumsCol)
                {
                    strEnums.Append(findEnum.ToString());
                }
            }

            return strEnums.ToString();
        }

        #endregion Métodos
    }
}
