using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WooW.Core;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase
{
    public abstract class AWoSyntaxManagerConstructorBase : AWoSyntaxManagerClassBase
    {
        #region Búsqueda de constructores

        /// <summary>
        /// Bandera que indica si se muestra o no la alerta para cuando se busca un constructor.
        /// </summary>
        private bool _showAlert = true;

        /// <summary>
        /// Se apoya de Roslyn y de linq para buscar un constructor con el nombre
        /// indicado por parámetro dentro del fichero que se encuentre cargado.
        /// </summary>
        /// <param name="constructorName"></param>
        /// <returns></returns>
        protected ConstructorDeclarationSyntax SearchConstructor(string constructorName)
        {
            ConstructorDeclarationSyntax findConstructor = (
                from constructor in Root.DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                where constructor.Identifier.ValueText == constructorName
                select constructor
            ).FirstOrDefault();

            if (findConstructor.IsNull() && _showAlert)
            {
                XtraMessageBox.Show(
                    $@"No se encontró un constructor con el nombre de ""{constructorName}"".",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }

            return findConstructor;
        }

        #endregion Búsqueda de constructores

        #region Preguntas

        /// <summary>
        /// Retorna un bool en función del estado del constructor
        /// Tiene código: true.
        /// No tiene código: false.
        /// </summary>
        /// <param name="constructorName"></param>
        /// <returns></returns>
        public bool ConstructorHaveCode(string constructorName)
        {
            ConstructorDeclarationSyntax findConstructor = SearchConstructor(constructorName);

            if (findConstructor.IsNull())
            {
                return false;
            }
            else
            {
                return !findConstructor.Body.Statements.ToString().IsNullOrStringEmpty();
            }
        }

        #endregion Preguntas

        #region Creación de constructores

        /// <summary>
        /// Crea un constructor con el nombre, el cuerpo y los parámetros que se reciban por parámetro.
        /// </summary>
        /// <param name="constructorName"></param>
        /// <param name="constructorBody"></param>
        /// <param name="constructorParams"></param>
        public void CreateConstructor(
            string constructorName,
            string constructorBody,
            List<(string type, string name)> constructorParams = null
        )
        {
            ClassDeclarationSyntax findClass = SearchClass(ClassName);
            if (findClass.IsNull())
                return;

            List<ParameterSyntax> methodParamSyntaxCol = new List<ParameterSyntax>();

            if (constructorParams != null)
            {
                foreach (var param in constructorParams)
                {
                    ParameterSyntax parameter = SyntaxFactory.Parameter(
                        new SyntaxList<AttributeListSyntax>(),
                        SyntaxFactory.TokenList(),
                        SyntaxFactory.ParseTypeName(param.type),
                        SyntaxFactory.ParseToken(param.name),
                        null
                    );

                    methodParamSyntaxCol.Add(parameter);
                }
            }

            BlockSyntax blockSyntax = TokenizeCode(constructorBody);

            ConstructorDeclarationSyntax newConstructor = SyntaxFactory
                .ConstructorDeclaration(ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(methodParamSyntaxCol.ToArray())
                .WithBody(blockSyntax);

            ClassDeclarationSyntax newClass = findClass.AddMembers(newConstructor);

            Root = Root.ReplaceNode(findClass, newClass);

            WriteCode(newClass.NormalizeWhitespace().ToString());
        }

        #endregion Creación de constructores

        #region Gestión del código

        /// <summary>
        /// Recupera el cuerpo del constructor en función del nombre que se recibe por parámetro.
        /// </summary>
        /// <param name="constructorName"></param>
        /// <returns></returns>
        public string GetBodyConstructor(string constructorName)
        {
            ConstructorDeclarationSyntax findConstructor = SearchConstructor(constructorName);

            if (findConstructor.IsNull())
            {
                return string.Empty;
            }
            else
            {
                return findConstructor.Body.Statements.ToString();
            }
        }

        /// <summary>
        /// Actualiza el constructor con el nuevo cuerpo definido en el syntax.
        /// </summary>
        /// <param name="constructorName"></param>
        /// <param name="constructorBody"></param>
        public void UpdateBodyCostructor(string constructorName, string constructorBody)
        {
            ConstructorDeclarationSyntax findConstructor = SearchConstructor(constructorName);
            if (findConstructor.IsNull())
                return;

            BlockSyntax blockSyntax = TokenizeCode(constructorBody);

            ConstructorDeclarationSyntax newConstructor = findConstructor.WithBody(blockSyntax);

            Root = Root.ReplaceNode(findConstructor, newConstructor);

            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}Validator");
            if (findClass.IsNull())
                return;

            WriteCode(findClass.NormalizeWhitespace().ToString());
        }

        /// <summary>
        /// Elimina el constructor con el nombre que se recibe por parámetro.
        /// </summary>
        /// <param name="constructorName"></param>
        public void DeleteConstructor(string constructorName)
        {
            ConstructorDeclarationSyntax findConstructor = SearchConstructor(constructorName);
            ClassDeclarationSyntax newClass = null;

            if (findConstructor.IsNull())
                return;

            ClassDeclarationSyntax findClass = SearchClass(ClassName);
            if (findClass.IsNull())
                return;

            newClass = findClass.RemoveNode(findConstructor, SyntaxRemoveOptions.KeepNoTrivia);
            Root = Root.ReplaceNode(findClass, newClass);

            WriteCode(newClass.NormalizeWhitespace().ToString());
        }

        #endregion Gestión del código
    }
}
