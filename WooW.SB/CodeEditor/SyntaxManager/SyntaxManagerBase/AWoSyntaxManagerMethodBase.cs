using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;
using DevExpress.DataProcessing;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WooW.Core;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase
{
    public abstract class AWoSyntaxManagerMethodBase : AWoSyntaxManagerAttributeBase
    {
        #region Búsqueda de métodos

        /// <summary>
        /// Bandera que indica si se muestra o no la alerta para cuando se busca un método.
        /// </summary>
        private bool _showAlert = true;

        /// <summary>
        /// Busca el método con el nombre que se recibe por parámetro.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        protected MethodDeclarationSyntax SearchMethod(string methodName)
        {
            MethodDeclarationSyntax findMethod = (
                from method in Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where method.Identifier.ValueText == methodName
                select method
            ).FirstOrDefault();

            if (findMethod.IsNull() && _showAlert)
            {
                XtraMessageBox.Show(
                    $@"El método que se esta intentando buscar ""{methodName}"" no existe",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }

            return findMethod;
        }

        /// <summary>
        /// Recibe un método y retorna una lista con los parámetros que tiene.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private List<(string type, string name, string value)> GetMethodParameters(
            MethodDeclarationSyntax method
        )
        {
            List<(string type, string name, string value)> parameters =
                new List<(string type, string name, string value)>();

            ParameterListSyntax parametersMethod = method.ParameterList;
            if (parametersMethod.Parameters.Count > 0)
            {
                foreach (var parameter in parametersMethod.Parameters)
                {
                    var parameterCol = parameter.ToString().Split(' ');
                    parameters.Add(
                        (
                            parameterCol[0],
                            parameterCol[1],
                            (parameterCol.Length < 3) ? string.Empty : parameterCol[3]
                        )
                    );
                }
            }

            return parameters;
        }

        #endregion Búsqueda de métodos

        #region Preguntas

        /// <summary>
        /// Busca si existe un método con el nombre que recibe por parámetro.
        /// Si existe : true
        /// SI no existe : false
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public bool AlreadyExistMethod(string methodName)
        {
            _showAlert = false;
            bool result = !SearchMethod(methodName).IsNull();
            _showAlert = true;
            return result;
        }

        /// <summary>
        /// Busca el método con el nombre que se recibe por parámetro y
        /// en caso de que tenga código retorna true.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public bool HaveCode(string methodName)
        {
            MethodDeclarationSyntax findMethod = SearchMethod(methodName);

            if (findMethod.IsNull())
            {
                return false;
            }
            else
            {
                return !findMethod.Body.Statements.ToString().IsNullOrStringEmpty();
            }
        }

        /// <summary>
        /// Busca la declaración del método con el nombre que se recibe por parámetro
        /// en todos los métodos de la clase.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool UsedMethod(string methodName)
        {
            bool result = false;
            IEnumerable<MethodDeclarationSyntax> classMethodsCol = GetMethodList();

            foreach (var method in classMethodsCol)
            {
                IEnumerable<InvocationExpressionSyntax> methodInvocations =
                    from invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>()
                    select invocation;

                foreach (var invocation in methodInvocations)
                {
                    result = invocation.Expression.ToString() == methodName;
                    if (result)
                        break;
                }
                if (result)
                    break;
            }

            return result;
        }

        #endregion Preguntas

        #region Recuperación de los métodos con base en un filtro

        /// <summary>
        /// Recupera un string con los métodos en raw siempre y cuando el nombre
        /// contenga el valor del parámetro
        /// </summary>
        /// <returns></returns>
        public string GetRawMethods(string contains)
        {
            try
            {
                StringBuilder strRawMethods = new StringBuilder();

                IEnumerable<MethodDeclarationSyntax> findMethods =
                    from method in Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    where method.Identifier.ValueText.Contains(contains)
                    select method;

                foreach (MethodDeclarationSyntax findMethod in findMethods)
                {
                    strRawMethods.AppendLine(findMethod.ToString());
                }

                return strRawMethods.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar los métodos que contengan: {contains}. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de los métodos con base en un filtro

        #region Obtener información de los métodos

        /// <summary>
        /// Recupera la información general del método a través del nombre.
        /// desde la root del código con linq.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public (List<(string type, string name, string value)> parameters, string body) GetMethod(
            string methodName
        )
        {
            _showAlert = false;
            MethodDeclarationSyntax findMethod = SearchMethod(methodName);

            List<(string type, string name, string value)> parameters =
                new List<(string type, string name, string value)>();

            if (findMethod.IsNull())
            {
                return (parameters, string.Empty);
            }
            else
            {
                parameters = GetMethodParameters(findMethod);
                //string x = findMethod.Body.Statements.ToString();
                return (parameters, findMethod.Body.Statements.ToString());
            }
        }

        /// <summary>
        /// Recupera una lista con los métodos del fichero y
        /// </summary>
        /// <returns></returns>
        public List<(string type, string method, bool haveCode)> GetMethodsInfo()
        {
            List<(string type, string method, bool haveCode)> methods =
                new List<(string type, string method, bool haveCode)>();

            foreach (MethodDeclarationSyntax method in GetMethodList())
            {
                methods.Add(
                    (
                        method.ReturnType.ToString(),
                        method.Identifier.ValueText,
                        !method.Body.Statements.ToString().IsNullOrStringEmpty()
                    )
                );
            }

            return methods;
        }

        /// <summary>
        /// Recupera la lista de los nombres de los métodos.
        /// </summary>
        /// <returns></returns>
        public List<string> GetMethodsName()
        {
            List<string> methodsNameCol = new List<string>();

            IEnumerable<MethodDeclarationSyntax> findMethodsCol = GetMethodList();

            foreach (var method in findMethodsCol)
            {
                methodsNameCol.Add(method.Identifier.ValueText);
            }

            return methodsNameCol;
        }

        /// <summary>
        /// Retorna la lista de los métodos tokenizados.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<MethodDeclarationSyntax> GetMethodList()
        {
            return (
                from method in Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                select method
            );
        }

        #endregion Obtener información de los métodos

        #region Gestión de métodos (Con persistencia)

        /// <summary>
        /// Genera un método nuevo en el árbol de Roslyn y lo escribe en el fichero.
        ///     - Separa el código en lineas a través de un split.
        ///     - Tokeniza el código.
        ///     - Tokeniza el nombre del método.
        ///     - Construye el método.
        ///     - Agrega el método como un nodo al árbol
        ///     - Reemplaza la clase, por la que contiene el nuevo método.
        ///     - Usa un método para escribir la clase en el fichero.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="bodyMethod"></param>
        public void CreateNewMethod(
            string methodName,
            string bodyMethod,
            string typeMethod,
            List<(string type, string name, string value)> methodParamsCol = null
        )
        {
            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}ScriptsUser");
            if (findClass.IsNull())
                return;

            BlockSyntax blockSyntax = TokenizeCode(bodyMethod);
            SyntaxToken identificador = SyntaxFactory.ParseToken(methodName);
            TypeSyntax typeSyntax = SyntaxFactory.ParseTypeName(typeMethod);
            MethodDeclarationSyntax newMethod;

            if (methodParamsCol.IsNull())
            {
                newMethod = SyntaxFactory
                    .MethodDeclaration(typeSyntax, identificador)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .WithBody(blockSyntax);
            }
            else
            {
                List<ParameterSyntax> methodParamSyntaxCol = new List<ParameterSyntax>();

                foreach (var param in methodParamsCol)
                {
                    ParameterSyntax parameter = null;
                    EqualsValueClauseSyntax defaultValue = null;

                    if (!param.value.IsNullOrStringEmpty())
                    {
                        ExpressionSyntax expressionSyntax = null;

                        switch (param.type)
                        {
                            case "string":
                                expressionSyntax = SyntaxFactory.ParseExpression(
                                    $@"""{param.value}"""
                                );
                                break;
                            case "char":
                                expressionSyntax = SyntaxFactory.ParseExpression(
                                    $@"'{param.value}'"
                                );
                                break;
                            default:
                                expressionSyntax = SyntaxFactory.ParseExpression(param.value);
                                break;
                        }

                        defaultValue = SyntaxFactory.EqualsValueClause(expressionSyntax);
                    }

                    parameter = SyntaxFactory.Parameter(
                        attributeLists: new SyntaxList<AttributeListSyntax>(),
                        modifiers: SyntaxFactory.TokenList(),
                        type: SyntaxFactory.ParseTypeName(param.type),
                        identifier: SyntaxFactory.ParseToken(param.name),
                        @default: (param.value.IsNullOrStringEmpty()) ? null : defaultValue
                    );

                    methodParamSyntaxCol.Add(parameter);
                }

                newMethod = SyntaxFactory
                    .MethodDeclaration(typeSyntax, identificador)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters(methodParamSyntaxCol.ToArray())
                    .WithBody(blockSyntax);
            }

            ClassDeclarationSyntax newClass = findClass.AddMembers(newMethod);

            Root = Root.ReplaceNode(findClass, newClass);

            WriteCode(newClass.NormalizeWhitespace().ToString());
        }

        /// <summary>
        /// Se ocupa de actualizar el nombre de un método y lo escribe en el fichero.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void RenameMethod(string oldName, string newName)
        {
            MethodDeclarationSyntax findMethod = SearchMethod(oldName);

            if (findMethod.IsNull())
                return;

            MethodDeclarationSyntax newMethod = findMethod.WithIdentifier(
                SyntaxFactory.Identifier(newName)
            );

            Root = Root.ReplaceNode(findMethod, newMethod);

            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}ScriptsUser");

            WriteCode(findClass.NormalizeWhitespace().ToString());
        }

        /// <summary>
        /// Actualiza el árbol con el método modificado.
        ///     - Separa el cuerpo en lineas por los ';'.
        ///     - Recorre las lineas separadas y las va cargando en una lista.
        ///     - Va validando que no se agreguen lineas en blanco o saltos de linea.
        ///     - Tokeniza el código.
        ///     - Busca el método que se esta editando.
        ///     - Valida que el método exista.
        ///     - Actualiza el cuerpo del método que se obtuvo con el código ya tokenizado.
        ///     - Actualizamos el árbol con el nuevo método modificado.
        ///     - Escribimos en el fichero con el uso del método.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="bodyMethod"></param>
        public void UpdateBodyMethod(string methodName, string bodyMethod)
        {
            MethodDeclarationSyntax findMethod = SearchMethod(methodName);
            if (findMethod.IsNull())
                return;

            BlockSyntax blockSyntax = TokenizeCode(bodyMethod);

            MethodDeclarationSyntax newMethod = null;

            if (bodyMethod.Contains("await"))
            {
                newMethod = findMethod
                    .WithBody(blockSyntax)
                    .WithModifiers(
                        SyntaxTokenList
                            .Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                            .Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                    );
            }
            else
            {
                newMethod = findMethod
                    .WithBody(blockSyntax)
                    .WithModifiers(
                        SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    );
            }

            Root = Root.ReplaceNode(findMethod, newMethod);

            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}ScriptsUser");

            WriteCode(findClass.NormalizeWhitespace().ToString().Replace("'; ", "';"));
        }

        /// <summary>
        /// Elimina el método con el nombre que se recibe por parámetro.
        /// </summary>
        /// <param name="methodName"></param>
        public void DeleteMethod(string methodName, bool alerted = false)
        {
            _showAlert = alerted;
            MethodDeclarationSyntax findMethod;
            ClassDeclarationSyntax newClass = null;

            do
            {
                findMethod = SearchMethod(methodName);
                if (!findMethod.IsNull())
                {
                    ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}ScriptsUser");
                    if (findClass.IsNull())
                        return;

                    newClass = findClass.RemoveNode(findMethod, SyntaxRemoveOptions.KeepNoTrivia);

                    Root = Root.ReplaceNode(findClass, newClass);
                }
            } while (findMethod != null);

            if (newClass != null)
            {
                WriteCode(newClass.NormalizeWhitespace().ToString());
            }
        }

        #endregion Gestión de métodos (Con persistencia)

        #region Gestión de métodos (Sin persistencia)

        /// <summary>
        /// Elimina el método con el nombre que se recibe por parámetro.
        /// </summary>
        /// <param name="methodName"></param>
        public void DeleteMethodOnlyTree(string methodName)
        {
            MethodDeclarationSyntax findMethod = SearchMethod(methodName);
            if (findMethod.IsNull())
                return;

            ClassDeclarationSyntax findClass = SearchClass($@"{ClassName}ScriptsUser");
            if (findClass.IsNull())
                return;

            ClassDeclarationSyntax newClass = findClass.RemoveNode(
                findMethod,
                SyntaxRemoveOptions.KeepNoTrivia
            );

            Root = Root.ReplaceNode(findClass, newClass);
        }

        #endregion Gestión de métodos (Sin persistencia)
    }
}
