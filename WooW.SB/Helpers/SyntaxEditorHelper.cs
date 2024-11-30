using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Forms;
using ActiproSoftware.Text.Languages.DotNet.Reflection;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using WooW.SB.Config;

namespace WooW.SB.Class
{
    public class SyntaxEditorHelper
    {
        private List<MetadataReference> references;

        public SyntaxEditorHelper(
            List<string> SystemReferencias,
            string AssemblyPath,
            List<string> Referencias
        )
        {
            references = new List<MetadataReference>();

            string ReferencePath = AssemblyPath;

            //string ReferencePath = string.Format(
            //    "{0}{1}",
            //    Proyecto.getInstance().Dir,
            //    AssemblyPath
            //);

            string corePath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            foreach (var Referencia in SystemReferencias)
                references.Add(
                    MetadataReference.CreateFromFile(Path.Combine(corePath, Referencia) + ".dll")
                );

            foreach (var Referencia in Referencias)
            {
                try
                {
                    references.Add(
                        MetadataReference.CreateFromFile(Path.Combine(ReferencePath, Referencia))
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public static void AddCommonDotNetSystemAssemblyReferences(
            IProjectAssembly projectAssembly,
            List<string> SystemReferencias,
            string AssemblyPath,
            List<string> Referencias
        )
        {
            if (projectAssembly is null)
                throw new ArgumentNullException(nameof(projectAssembly));

            List<string> SystemReferenciasFound = new List<string>();

            // Iterate the assemblies in the AppDomain and load all "System" assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var found = SystemReferencias
                    .Where(e => assembly.FullName.StartsWith(e + ","))
                    .FirstOrDefault();

                if (found != null)
                {
                    projectAssembly.AssemblyReferences.Add(assembly);
                    SystemReferenciasFound.Add(found);
                }
            }

            foreach (var sr in SystemReferencias)
            {
                if (SystemReferenciasFound.IndexOf(sr) != -1)
                {
                    var ass = projectAssembly.AssemblyReferences.AddFrom(sr + ".dll");
                    //if (ass == null)
                    //    XtraMessageBox.Show(
                    //        $"Referencia no encontrada {sr}",
                    //        "Verifique...",
                    //        MessageBoxButtons.OK,
                    //        MessageBoxIcon.Error
                    //    );
                }
            }

            string filePathReferencias =
                string.Format("{0}{1}\\", Proyecto.getInstance().Dir, AssemblyPath) + "{0}";

            foreach (string itemReference in Referencias)
            {
                string sReferenciaReal = string.Format(filePathReferencias, itemReference);

                try
                {
                    if (!File.Exists(sReferenciaReal))
                        throw new Exception($"Archivo {sReferenciaReal} no existe");
                    //var asm = Assembly.LoadFile(sReferenciaReal);
                    //projectAssembly.AssemblyReferences.Add(asm);
                    var ass = projectAssembly.AssemblyReferences.AddFrom(sReferenciaReal);
                    //if (ass == null)
                    //    XtraMessageBox.Show(
                    //        $"Referencia no encontrada {itemReference}",
                    //        "Verifique...",
                    //        MessageBoxButtons.OK,
                    //        MessageBoxIcon.Error
                    //    );
                }
#pragma warning disable CS0168
                catch (Exception Ex)
#pragma warning restore CS0168
                {
                    if (Ex.InnerException != null)
                        XtraMessageBox.Show(
                            sReferenciaReal
                                + "\r\n\r\n"
                                + Ex.Message
                                + "\r\n\r\n"
                                + Ex.InnerException.Message,
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    else
                        XtraMessageBox.Show(
                            sReferenciaReal + "\r\n\r\n" + Ex.Message,
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );

                    //Debug.Write(string.Format("Error al agregar referencia externa -> {0}", ex.Message));
                }
            }
        }

        public bool Validar(string sCode, out List<ScriptErrorDescriptor> parErrors)
        {
            bool bResult = false;
            parErrors = null;

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sCode);

            var compilation = CSharpCompilation
                .Create("Test.dll", options: options)
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError
                        || diagnostic.Severity == DiagnosticSeverity.Error
                    );

                    parErrors = new List<ScriptErrorDescriptor>();

                    foreach (var diag in failures)
                    {
                        int Line = 0;
                        int Column = 0;

                        if (diag.ToString().IndexOf(":") > 0)
                        {
                            var parts = diag.ToString()
                                .Substring(0, diag.ToString().IndexOf(":"))
                                .Replace("(", "")
                                .Replace(")", "")
                                .Split(',');
                            if (parts.Length > 1)
                            {
                                int.TryParse(parts[0], out Line);
                                int.TryParse(parts[1], out Column);
                            }
                        }

                        ScriptErrorDescriptor scriptError = new ScriptErrorDescriptor()
                        {
                            Code = diag.Id,
                            Description = diag.GetMessage(),
                            Line = Line,
                            Column = Column,
                        };

                        parErrors.Add(scriptError);
                    }
                }

                return result.Success;
            }

            /*

            AppDomain appCompile = AppDomain.CreateDomain("ScriptEditor");

            ScriptBuilderHelper scriptBuilder = (ScriptBuilderHelper)
                appCompile.CreateInstanceAndUnwrap(
                    typeof(ScriptBuilderHelper).Assembly.FullName,
                    typeof(ScriptBuilderHelper).FullName
                );

            scriptBuilder.LoadCode(sCode);

            foreach (string item in SystemReferencias)
                scriptBuilder.AddReference(item + ".dll");

            foreach (string item in Referencias)
                scriptBuilder.AddReference(item);

            //foreach (string item in Referencias)
            //    scriptBuilder.AddReference(item);

            // se agregan opciones de compilacion

            StringBuilder sbCompilerOptions = new StringBuilder();
            sbCompilerOptions.Append(" /optimize+ ");

            string filePathReferencias = string.Format(
                "{0}{1}",
                Proyecto.getInstance().Dir,
                AssemblyPath
            );

            sbCompilerOptions.Append($" /lib:\"{filePathReferencias}\"");

            scriptBuilder.CompilerOptions = sbCompilerOptions.ToString();

            if (scriptBuilder.IsValidCode())
                bResult = true;
            else
            {
                parErrors = new List<ScriptErrorDescriptor>();

                if (scriptBuilder.ErrorCollection != null)
                {
                    for (int i = 0; i < scriptBuilder.ErrorCollection.Count; i++)
                    {
                        CompilerError currentError = scriptBuilder.ErrorCollection[i];

                        if (currentError.IsWarning)
                            continue;

                        int iErrorLine = currentError.Line;

                        ScriptErrorDescriptor scriptError = new ScriptErrorDescriptor()
                        {
                            Code = currentError.ErrorNumber,
                            Description = currentError.ErrorText,
                            Line = iErrorLine,
                            Column = currentError.Column
                        };

                        parErrors.Add(scriptError);
                    }
                }
            }

            AppDomain.Unload(appCompile);

            */

            return bResult;
        }

        public Assembly CreateInstanceForRun(
            string sCode,
            out List<ScriptErrorDescriptor> parErrors
        )
        {
            bool bResult = false;
            parErrors = null;

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sCode);

            var compilation = CSharpCompilation
                .Create("Test.dll", options: options)
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError
                        || diagnostic.Severity == DiagnosticSeverity.Error
                    );

                    parErrors = new List<ScriptErrorDescriptor>();

                    foreach (var diag in failures)
                    {
                        int Line = 0;
                        int Column = 0;

                        if (diag.ToString().IndexOf(":") > 0)
                        {
                            var parts = diag.ToString()
                                .Substring(0, diag.ToString().IndexOf(":"))
                                .Replace("(", "")
                                .Replace(")", "")
                                .Split(',');
                            if (parts.Length > 1)
                            {
                                int.TryParse(parts[0], out Line);
                                int.TryParse(parts[1], out Column);
                            }
                        }

                        ScriptErrorDescriptor scriptError = new ScriptErrorDescriptor()
                        {
                            Code = diag.Id,
                            Description = diag.GetMessage(),
                            Line = Line,
                            Column = Column,
                        };

                        parErrors.Add(scriptError);

                        return null;
                    }
                }

                var assemblyName = AssemblyLoadContext.GetAssemblyName("WooW.WebClient.dll");

                var alreadyLoadedAssembly = System
                    .Reflection.Assembly.GetExecutingAssembly()
                    .GetReferencedAssemblies()
                    .FirstOrDefault(x => x.FullName == assemblyName.FullName);

                if (object.ReferenceEquals(null, alreadyLoadedAssembly))
                {
                    //var assembly = AssemblyLoadContext.LoadFromAssemblyName(assemblyName);
                    var assembly = Assembly.LoadFrom(
                        Path.Combine(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                            "WooW.WebClient.dll"
                        )
                    );
                }

                return Assembly.Load(stream.GetBuffer());
            }
        }

        public static string PrettyPrint(string code)
        {
            for (int i = 0; i < 10; i++)
                code = code.Replace("\r\n\r\n\r\n", "\r\n\r\n");

            try
            {
                AdhocWorkspace msWorkspace = new AdhocWorkspace();

                OptionSet options = msWorkspace.Options;

                OptionSet formattingOptions = msWorkspace
                    .Options.WithChangedOption(CSharpFormattingOptions.SpaceAfterComma, true)
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceWithinExpressionParentheses,
                        false
                    )
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceWithinMethodCallParentheses,
                        false
                    )
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceWithinMethodDeclarationParenthesis,
                        false
                    )
                    .WithChangedOption(CSharpFormattingOptions.SpaceAfterMethodCallName, false)
                    .WithChangedOption(CSharpFormattingOptions.SpaceAfterCast, false)
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceAfterControlFlowStatementKeyword,
                        false
                    )
                    .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4)
                    .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 4)
                    .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, true)
                    .WithChangedOption(CSharpFormattingOptions.NewLineForCatch, true)
                    .WithChangedOption(CSharpFormattingOptions.NewLineForClausesInQuery, true)
                    .WithChangedOption(CSharpFormattingOptions.NewLineForFinally, true)
                    .WithChangedOption(CSharpFormattingOptions.WrappingPreserveSingleLine, true)
                    .WithChangedOption(CSharpFormattingOptions.NewLineForMembersInObjectInit, true)
                    .WithChangedOption(CSharpFormattingOptions.NewLineForClausesInQuery, true)
                    .WithChangedOption(CSharpFormattingOptions.IndentSwitchSection, true)
                    .WithChangedOption(CSharpFormattingOptions.IndentSwitchCaseSection, true)
                    .WithChangedOption(CSharpFormattingOptions.IndentBlock, true)
                    .WithChangedOption(CSharpFormattingOptions.IndentBraces, false)
                    .WithChangedOption(
                        CSharpFormattingOptions.IndentSwitchCaseSectionWhenBlock,
                        false
                    )
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceWithinExpressionParentheses,
                        false
                    )
                    .WithChangedOption(CSharpFormattingOptions.SpaceAfterCast, false)
                    .WithChangedOption(CSharpFormattingOptions.SpaceAfterCast, false)
                    .WithChangedOption(CSharpFormattingOptions.SpaceAfterComma, true)
                    .WithChangedOption(CSharpFormattingOptions.SpaceAfterDot, false)
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceAfterSemicolonsInForStatement,
                        true
                    //).WithChangedOption(
                    //    CSharpFormattingOptions.WrappingKeepStatementsOnSingleLine,
                    //    false
                    )
                    .WithChangedOption(CSharpFormattingOptions.SpaceBeforeOpenSquareBracket, false)
                    .WithChangedOption(CSharpFormattingOptions.WrappingPreserveSingleLine, false)
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceBetweenEmptyMethodCallParentheses,
                        false
                    )
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceBetweenEmptyMethodDeclarationParentheses,
                        false
                    )
                    .WithChangedOption(CSharpFormattingOptions.SpaceWithinCastParentheses, false)
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceWithinExpressionParentheses,
                        false
                    )
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceWithinMethodCallParentheses,
                        false
                    )
                    .WithChangedOption(
                        CSharpFormattingOptions.SpaceWithinMethodDeclarationParenthesis,
                        false
                    )
                    .WithChangedOption(CSharpFormattingOptions.SpaceWithinOtherParentheses, false);

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
                SyntaxNode root = syntaxTree.GetRoot();

                SyntaxNode formattedNode = Formatter.Format(root, msWorkspace, formattingOptions);
                string formattedCode = formattedNode.ToFullString();

                return formattedCode;
            }
            catch (Exception ex)
            {
                return code;
            }
        }
    }
}
