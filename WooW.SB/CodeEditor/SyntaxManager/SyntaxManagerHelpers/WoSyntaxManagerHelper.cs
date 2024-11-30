using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerHelpers
{
    public class WoSyntaxManagerHelper
    {
        /// <summary>
        /// Valida que las regiones se encuentren correctamente escritas
        /// </summary>
        /// <returns></returns>
        public static string ValidateRegions(string rawClass)
        {
            try
            {
                if (rawClass == string.Empty)
                {
                    return string.Empty;
                }

                string result = rawClass;

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(rawClass);
                CompilationUnitSyntax root = (CompilationUnitSyntax)syntaxTree.GetRoot();

                var openRegionsCol = root.DescendantTrivia()
                    .Where(trivia => trivia.IsKind(SyntaxKind.RegionDirectiveTrivia))
                    .ToList();

                var closeRegionCol = root.DescendantTrivia()
                    .Where(trivia => trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
                    .ToList();

                if (openRegionsCol.Count != closeRegionCol.Count)
                {
                    foreach (var region in openRegionsCol)
                    {
                        string regionName = region.ToString().Replace("#", "");
                        var findClose = closeRegionCol.FirstOrDefault(reg =>
                            reg.ToString().Contains(regionName)
                        );

                        if (findClose.ToString() == "")
                        {
                            WoTemplateGenericClassBlazor woTemplateGenericClassBlazor =
                                new WoTemplateGenericClassBlazor();

                            ClassDeclarationSyntax baseClass = (
                                from clase in root.DescendantNodes()
                                    .OfType<ClassDeclarationSyntax>()
                                select clase
                            ).FirstOrDefault();

                            string codeClass = baseClass.NormalizeWhitespace().ToString();
                            codeClass = codeClass.Substring(0, codeClass.Length - 1);

                            codeClass += $@"#end{regionName}";

                            codeClass += "\n\r}";

                            result = codeClass;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al Validar la region. {ex.Message}");
            }
        }
    }
}
