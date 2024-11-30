using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerBase;

namespace WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass
{
    public class WoSyntaxManagerCompilation : AWoSyntaxManagerBase, IWoSyntaxManager
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

        #region Compilar código

        /// <summary>
        /// Compila el código sobre el que se esta trabajando
        ///     - Carga las referencias(dlls) requeridas para compilar el código.
        ///     - Compila y genera una dll.
        /// </summary>
        /// <param name="dlls"></param>
        [SupportedOSPlatform("windows")]
        public (List<ListViewItem> errorList, bool success) CompileCode(List<string> dlls)
        {
            List<ListViewItem> output = new List<ListViewItem>();
            bool success = false;

            List<MetadataReference> references = new List<MetadataReference>();

            foreach (var dll in dlls)
            {
                if (File.Exists(dll))
                {
                    references.Add(MetadataReference.CreateFromFile(dll));
                }
                else
                {
                    XtraMessageBox.Show(
                        "No se encontró la dll: " + dll,
                        "Alerta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }

            CSharpCompilation compilation = CSharpCompilation
                .Create("Prog.dll")
                .AddReferences(references)
                .AddSyntaxTrees(SyntaxTree);

            try
            {
                Stream resultStream = new MemoryStream();
                var result = compilation.Emit(resultStream);
                success = result.Success;

                resultStream.Dispose();
                if (!result.Success)
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        string messageFormat = diagnostic.GetMessage();
                        if (
                            !messageFormat.Contains(
                                "La anotación para tipos de referencia que aceptan valores NULL solo debe usarse en el código dentro de un contexto de anotaciones \"#nullable\""
                            )
                        )
                        {
                            ListViewItem outputItem = new ListViewItem(
                                new string[]
                                {
                                    diagnostic.Id,
                                    diagnostic.DefaultSeverity.ToString(),
                                    messageFormat,
                                }
                            );
                            output.Add(outputItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error durante la compilación", "Alerta");
                /// ToDo: enviar a un log.
                /// var x = ex.Data;
            }

            return (output, success);
        }

        #endregion Compilar código
    }
}
