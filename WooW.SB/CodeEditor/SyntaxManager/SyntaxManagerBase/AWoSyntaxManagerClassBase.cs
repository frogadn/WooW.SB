using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WooW.Core;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerHelpers;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase
{
    public abstract class AWoSyntaxManagerClassBase : AWoSyntaxManagerBase
    {
        #region Búsqueda

        /// <summary>
        /// Recupera un objeto de tipo ClassDeclarationSyntax a partir del nombre de la clase.
        /// En caso de no encontrar la clase, devuelve null.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        protected ClassDeclarationSyntax SearchClass(string className)
        {
            ClassDeclarationSyntax findClass = (
                from clase in Root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                where clase.Identifier.ValueText == className
                select clase
            ).FirstOrDefault();

            if (findClass.IsNull())
            {
                ///ToDo; send log
                XtraMessageBox.Show(
                    $@"La clase ""{className}"" no se encontró.",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }

            return findClass;
        }

        #endregion Búsqueda

        #region Recuperar clase

        /// <summary>
        /// Recupera el código de la clase a partir del nombre de la clase.
        /// En caso de no encontrar la clase, devuelve string.Empty.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public string GetClassCode(string className)
        {
            ClassDeclarationSyntax findClass = SearchClass(className);
            if (findClass.IsNull())
            {
                //Send trow: no se puede recuperar la clase y send Log
                return string.Empty;
            }

            return findClass.NormalizeWhitespace().ToString();
        }

        /// <summary>
        /// Recupera el código de la clase a partir del nombre de la clase con un nuevo nombre de clase.
        /// En caso de no encontrar la clase, devuelve string.Empty.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public string GetClassCodeWithNewName(string oldName, string newName)
        {
            ClassDeclarationSyntax findClass = SearchClass(oldName);

            if (findClass.IsNull())
            {
                /// Send log
                //Send trow: no se puede recuperar la clase
                return string.Empty;
            }

            ConstructorDeclarationSyntax findConstructor = (
                from constructor in Root.DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                where constructor.Identifier.ValueText == oldName
                select constructor
            ).FirstOrDefault();

            if (!findConstructor.IsNull())
            {
                ConstructorDeclarationSyntax newConstructor = findConstructor.WithIdentifier(
                    SyntaxFactory.Identifier(newName)
                );
                findClass = findClass.ReplaceNode(findConstructor, newConstructor);
            }

            ClassDeclarationSyntax newClass = findClass.WithIdentifier(
                SyntaxFactory.Identifier(newName)
            );

            return newClass.NormalizeWhitespace().ToString();
        }

        #endregion Recuperar clase


        #region Tokenización del código

        /// <summary>
        /// Retorna un bloque de código tokenizado a partir del string que recibe por parámetro.
        /// </summary>
        /// <param name="codeBlock"></param>
        /// <returns></returns>
        protected BlockSyntax TokenizeCode(string codeBlock)
        {
            var codeLinesCol = codeBlock.Split(';');

            List<StatementSyntax> statements = new List<StatementSyntax>();

            foreach (var line in codeLinesCol)
            {
                if (
                    line != string.Empty
                    && line != " "
                    && line != "   "
                    && line != "\r"
                    && line != "\n"
                    && line != "\r\n"
                    && line != null
                ) //eliminamos las lineas en blanco
                    statements.Add(SyntaxFactory.ParseStatement(line + ";"));
            }

            return SyntaxFactory.Block(statements);
        }

        #endregion Tokenización del código

        #region Actualización de los ficheros

        /// <summary>
        /// Escribe el código que se recibe en ambas rutas indicadas.
        /// El código completo cargado desde el fichero y la copia que salva el código.
        /// </summary>
        /// <param name="classOnly"></param>
        protected void WriteCode(string classOnly)
        {
            string CheckClass = WoSyntaxManagerHelper.ValidateRegions(classOnly);

            WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                new WoTemplateGenericClassBlazor();
            woTemplateGenericClassBlazor.Code = CheckClass;
            WoDirectory.WriteTemplate(PathScript, woTemplateGenericClassBlazor.TransformText());
        }

        #endregion Actualización de los ficheros


        #region RawCode

        /// <summary>
        /// Agrega código al final del fichero.
        /// No compila y solo se recomienda para agregar cosas simples y para agilizar
        /// la gestión de código.
        /// Se recomienda igual solo pasar código desde los generadores ya echos.
        /// </summary>
        /// <param name="rawCode"></param>
        public void AddRawCode(string rawCode)
        {
            WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                new WoTemplateGenericClassBlazor();

            ClassDeclarationSyntax baseClass = (
                from clase in Root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                select clase
            ).FirstOrDefault();

            if (baseClass.IsNull())
            {
                //Send trow: no se puede recuperar la clase
                return;
            }

            string rawClass = baseClass.NormalizeWhitespace().ToString();

            rawClass = rawClass.Substring(0, rawClass.Length - 1);

            rawClass += rawCode;

            rawClass += "\n\r}";

            WriteCode(rawClass);
        }

        #endregion RawCode
    }
}
