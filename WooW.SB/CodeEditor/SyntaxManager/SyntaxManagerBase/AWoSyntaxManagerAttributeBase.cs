using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServiceStack;
using WooW.Core;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase
{
    public class AWoSyntaxManagerAttributeBase : AWoSyntaxManagerConstructorBase
    {
        #region Búsqueda de atributos

        /// <summary>
        /// Bandera que indica si se muestra o no la alerta para cuando se busca un atributo.
        /// </summary>
        private bool _showAlert = true;

        /// <summary>
        /// Retorna el atributo base del fichero cargado.
        /// Busca a través del nombre.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        protected BasePropertyDeclarationSyntax SearchBaseAttribute(string attributeName)
        {
            BasePropertyDeclarationSyntax findAttribute = (
                from attribute in Root.DescendantNodes().OfType<BasePropertyDeclarationSyntax>()
                where attribute.GetText().ToString().Contains($@" {attributeName} {{")
                select attribute
            ).FirstOrDefault();

            if (findAttribute.IsNull() && _showAlert)
            {
                XtraMessageBox.Show(
                    $@"El atributo ""{attributeName}"" no existe.",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }

            return findAttribute;
        }

        /// <summary>
        /// Retorna el atributo custom del fichero cargado.
        /// Busca a través del nombre.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        protected FieldDeclarationSyntax SerchCustomAttribute(string attributeName)
        {
            FieldDeclarationSyntax findAttribute = (
                from attribute in Root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                where
                    (
                        attribute.ToString().Contains($@" {attributeName} =")
                        || attribute.ToString().Contains($@" {attributeName};")
                        || attribute.ToString() == attributeName
                    )
                select attribute
            ).FirstOrDefault();

            var x =
                from attribute in Root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                select attribute;

            if (findAttribute.IsNull() && _showAlert)
            {
                XtraMessageBox.Show(
                    $@"El atributo ""{attributeName}"" no existe.",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }

            return findAttribute;
        }

        /// <summary>
        /// Retorna el atributo custom del fichero cargado.
        /// Busca a través de la declaración completa del atributo.
        /// </summary>
        /// <param name="selectedAttribute"></param>
        /// <returns></returns>
        protected BasePropertyDeclarationSyntax SearchDirectBaseAttribute(string selectedAttribute)
        {
            BasePropertyDeclarationSyntax findAttribute = (
                from attribute in Root.DescendantNodes().OfType<BasePropertyDeclarationSyntax>()
                where attribute.ToString() == selectedAttribute
                select attribute
            ).FirstOrDefault();

            var attributes = Root.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

            if (findAttribute.IsNull())
            {
                XtraMessageBox.Show(
                    $@"El atributo ""{selectedAttribute}"" no existe.",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }

            return findAttribute;
        }

        protected FieldDeclarationSyntax SearchDirectCustomAttribute(string selectedAttribute)
        {
            FieldDeclarationSyntax findAttribute = (
                from attribute in Root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                where attribute.ToString() == selectedAttribute
                select attribute
            ).FirstOrDefault();

            var attributes = Root.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

            if (findAttribute.IsNull())
            {
                XtraMessageBox.Show(
                    $@"El atributo ""{selectedAttribute}"" no existe.",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }

            return findAttribute;
        }

        #endregion Búsqueda de atributos

        #region Peguntas

        /// <summary>
        /// Valida si el atributo ya existe:
        /// Ya existe el atributo : True
        /// No existe el atributo : False
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public bool AlreadyExistAttribute(string attributeName)
        {
            _showAlert = false;
            bool result =
                (!SearchBaseAttribute(attributeName).IsNull())
                || (!SerchCustomAttribute(attributeName).IsNull());
            _showAlert = true;

            return result;
        }

        #endregion Peguntas

        #region Recuperar lista de atributos

        /// <summary>
        /// Recupera la lista de los atributos del fichero que se encuentra cargado.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllAttributes()
        {
            List<string> attributesCol = new List<string>();

            IEnumerable<BasePropertyDeclarationSyntax> findBaseAttributesCol =
                from attribute in Root.DescendantNodes().OfType<BasePropertyDeclarationSyntax>()
                select attribute;

            if (!findBaseAttributesCol.IsNull())
            {
                foreach (var attribute in findBaseAttributesCol)
                {
                    attributesCol.Add($@"public {attribute.ToString().Substring(6)}");
                }
            }

            IEnumerable<FieldDeclarationSyntax> findCustomAttributesCol =
                from attribute in Root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                select attribute;

            if (!findCustomAttributesCol.IsNull())
            {
                foreach (var attribute in findCustomAttributesCol)
                {
                    attributesCol.Add(attribute.ToString());
                }
            }

            return attributesCol;
        }

        #endregion Recuperar lista de atributos

        #region Gestión de atributos

        /// <summary>
        /// Crea el atributo con la información que se recibe por parámetro.
        /// ToDo: cambiar accessModifier a un enum.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void CreateAttribute(
            string type,
            string name,
            string value,
            string accessModifier = "private",
            string classType = "ScriptsUser",
            bool getset = true
        )
        {
            if (!AlreadyExistAttribute(name))
            {
                ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}{classType}");
                if (findClass.IsNull())
                    return;

                if (accessModifier == "public")
                {
                    if (getset)
                    {
                        name = name + " { get; set; }";
                    }
                }

                MemberDeclarationSyntax newAttribute = null;
                if (value.IsNullOrStringEmpty())
                {
                    newAttribute = SyntaxFactory.ParseMemberDeclaration(
                        $@"{accessModifier} {type} {name};"
                    );
                }
                else
                {
                    newAttribute = SyntaxFactory.ParseMemberDeclaration(
                        $@"{accessModifier} {type} {name} = {value};"
                    );
                }

                ClassDeclarationSyntax newClass = findClass.AddMembers(newAttribute);

                Root = Root.ReplaceNode(findClass, newClass);

                WriteCode(newClass.NormalizeWhitespace().ToString());
            }
        }

        /// <summary>
        /// Cambia el nombre del atributo seleccionado.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void RenameAttribute(
            string type,
            string value,
            string oldName,
            string newName,
            string accessModifier = "private",
            string classType = "ScriptsUser"
        )
        {
            if (accessModifier == "public")
            {
                DeleteBaseAttribute(oldName, classType);
            }
            else
            {
                DeleteCustomAttribute(oldName, ClassName);
            }

            CreateAttribute(
                type: type,
                name: newName,
                value: value,
                accessModifier: accessModifier,
                classType: classType
            );
        }

        /// <summary>
        /// Elimina el atributo seleccionado de la clase.
        /// </summary>
        /// <param name="selectedAttribute"></param>
        public void DeleteAttribute(string selectedAttribute, string classType = "ScriptsUser")
        {
            BasePropertyDeclarationSyntax findAttribute = SearchDirectBaseAttribute(
                selectedAttribute
            );
            if (findAttribute.IsNull())
                return;

            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}{classType}");
            if (findClass.IsNull())
                return;

            ClassDeclarationSyntax newClass = findClass.RemoveNode(
                findAttribute,
                SyntaxRemoveOptions.KeepNoTrivia
            );

            Root = Root.ReplaceNode(findClass, newClass);

            WriteCode(newClass.NormalizeWhitespace().ToString());
        }

        /// <summary>
        /// Elimina un atributo que puede ser privado.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="className"></param>
        public void DeleteCustomAttribute(string name, string className = "ScriptsUser")
        {
            FieldDeclarationSyntax findAttribute = SerchCustomAttribute(name);
            if (findAttribute == null)
                return;

            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}{className}");
            if (findClass.IsNull())
                return;

            ClassDeclarationSyntax newClass = findClass.RemoveNode(
                findAttribute,
                SyntaxRemoveOptions.KeepNoTrivia
            );

            Root = Root.ReplaceNode(findClass, newClass);

            WriteCode(newClass.NormalizeWhitespace().ToString());
        }

        /// <summary>
        /// Elimina uno de los atributos base.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="className"></param>
        public void DeleteBaseAttribute(string name, string className = "ScriptsUser")
        {
            BasePropertyDeclarationSyntax findAttribute = SearchBaseAttribute(name);
            if (findAttribute == null)
                return;

            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}{className}");
            if (findClass.IsNull())
                return;

            ClassDeclarationSyntax newClass = findClass.RemoveNode(
                findAttribute,
                SyntaxRemoveOptions.KeepNoTrivia
            );

            Root = Root.ReplaceNode(findClass, newClass);

            WriteCode(newClass.NormalizeWhitespace().ToString());
        }

        #endregion Gestión de atributos
    }
}
