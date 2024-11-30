#pragma warning disable CS1591
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using WooW.Core;

namespace WooW.SB.Class
{
    public class ScriptBuilderHelper : MarshalByRefObject
    {
        #region " Constructor"

        public ScriptBuilderHelper()
        {
            m_slReferences = new List<string>();
            m_sbErrors = new StringBuilder();
            ErrorCollection = null;

            CompiledAssembly = null;

            SourceCode = string.Empty;
            AllowDebug = false;
            IncludeErrorLine = true;
            CompilerOptions = string.Empty;
        }

        #endregion " Constructor"

        #region " Campos"

        private readonly List<string> m_slReferences;
        private readonly StringBuilder m_sbErrors;

        #endregion " Campos"

        #region " Propiedades"

        public string SourceCode { get; private set; }
        public bool AllowDebug { get; set; }
        public bool IncludeErrorLine { get; set; }
        public string CompilerOptions { get; set; }

        public string Errors => m_sbErrors.ToString();

        public CompilerErrorCollection ErrorCollection { get; private set; }

        public Assembly CompiledAssembly { get; private set; }

        #endregion " Propiedades"

        #region " Métodos Privados"

        private void Build()
        {
            Build(string.Empty);
        }

        private void Build(string parAssemblyFileName)
        {
            CompiledAssembly = null;
            m_sbErrors.Length = 0;

            if (SourceCode.IsNullOrStringEmpty())
                return;

            using (
                var cSharpProvider = new CSharpCodeProvider(
                    new Dictionary<string, string> { { "CompilerVersion", "v4.0" } }
                )
            )
            {
                var compilerParams = new CompilerParameters();

                if (parAssemblyFileName.IsNullOrStringEmpty())
                {
                    compilerParams.GenerateInMemory = true;
                }
                else
                {
                    compilerParams.GenerateInMemory = false;
                    compilerParams.OutputAssembly = parAssemblyFileName;
                }

                if (AllowDebug)
                {
                    compilerParams.TempFiles = new TempFileCollection(Path.GetTempPath(), true);
                    compilerParams.IncludeDebugInformation = true;
                }
                else
                {
                    compilerParams.TempFiles = new TempFileCollection(Path.GetTempPath(), false);
                    compilerParams.IncludeDebugInformation = false;
                }

                var sbCompilerOptions = new StringBuilder();
                sbCompilerOptions.Append(" /optimize+ ");

                if (!(CompilerOptions.IsNullOrStringEmpty()))
                {
                    sbCompilerOptions.Append(" ");
                    sbCompilerOptions.Append(CompilerOptions);
                }

                compilerParams.TreatWarningsAsErrors = false;
                compilerParams.CompilerOptions = sbCompilerOptions.ToString();
                compilerParams.ReferencedAssemblies.AddRange(m_slReferences.ToArray());

                var bErrores = false;

                try
                {
                    var compResults = cSharpProvider.CompileAssemblyFromSource(
                        compilerParams,
                        SourceCode
                    );
                    if (compResults.Errors.HasErrors)
                    {
                        ErrorCollection = compResults.Errors;

                        foreach (CompilerError iError in compResults.Errors)
                        {
                            if (iError.IsWarning)
                                continue;

                            if (IncludeErrorLine)
                                m_sbErrors
                                    .AppendFormat(
                                        "Linea {0} - {1}",
                                        iError.Line.ToString(),
                                        iError.ErrorText
                                    )
                                    .AppendLine();
                            else
                                m_sbErrors.AppendLine(iError.ErrorText);

                            bErrores = true;
                        }

                        if (bErrores)
                            m_sbErrors.Append(SourceCode);
                    }
                    else
                    {
                        CompiledAssembly = compResults.CompiledAssembly;
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        bErrores = true;
                        m_sbErrors.AppendFormat("Exception: {0}", e.Message);
                        m_sbErrors.AppendFormat("Inner Exception: {0}", e.InnerException);
                    }
                    catch { }
                }

                //if (bErrores)
                //    try
                //    {
                //        using (
                //            var file = new StreamWriter(
                //                Path.Combine(
                //                    Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                //                    "FrogScriptBuilder"
                //                        + Path.GetFileNameWithoutExtension(parAssemblyFileName)
                //                        + ".Log"
                //                ),
                //                true
                //            )
                //        )
                //        {
                //            file.WriteLine(m_sbErrors.ToString());
                //        }
                //    }
                //    catch { }
            }
        }

        #endregion " Métodos Privados"

        #region " Métodos Públicos"

        public void LoadCode(string parCode)
        {
            SourceCode = parCode;
        }

        public void LoadCode(string parCode, string[] parReferences)
        {
            SourceCode = parCode;
            m_slReferences.AddRange(parReferences);
        }

        public void AddReference(string parReference)
        {
            if (parReference.IsNullOrStringEmpty())
                return;

            var iPos = m_slReferences.IndexOf(parReference);

            if (iPos == -1)
                m_slReferences.Add(parReference);
        }

        public void AddReferences(IEnumerable<string> parReferences)
        {
            if (parReferences.IsNull())
                return;

            foreach (var item in parReferences)
                AddReference(item);
        }

        public bool IsValidCode()
        {
            Build();

            if (CompiledAssembly.IsNull())
                return false;

            if (m_sbErrors.Length > 0)
                return false;

            return true;
        }

        public bool CreateAssembly(string parAssemblyFileName)
        {
            if (parAssemblyFileName.IsNullOrStringEmpty())
                return false;

            try
            {
                if (File.Exists(parAssemblyFileName))
                    File.Delete(parAssemblyFileName);
            }
            catch
            {
                return false;
            }

            Build(parAssemblyFileName);

            if (CompiledAssembly.IsNull())
                return false;

            return File.Exists(parAssemblyFileName);
        }

        public object CreateObjectFromAssembly(string parAssemblyPath, string parClassName)
        {
            return CreateObjectFromAssembly(parAssemblyPath, parClassName, null);
        }

        public object CreateObjectFromAssembly(
            string parAssemblyPath,
            string parClassName,
            object[] parArguments
        )
        {
            if ((parAssemblyPath.IsNullOrStringEmpty()) || (parClassName.IsNullOrStringEmpty()))
                return null;

            try
            {
                CompiledAssembly = Assembly.LoadFile(parAssemblyPath);
            }
            catch
            {
                return null;
            }

            if (CompiledAssembly.IsNull())
                return null;

            var eFlags = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public;

            try
            {
                return CompiledAssembly.CreateInstance(
                    parClassName,
                    false,
                    eFlags,
                    null,
                    parArguments,
                    CultureInfo.InvariantCulture,
                    null
                );
            }
            catch
            {
                return null;
            }
        }

        public object CreateObjectByInterface(Type InterfaceType)
        {
            if (CompiledAssembly.IsNull())
                Build();

            if (CompiledAssembly.IsNull())
                return null;

            var types = CompiledAssembly
                .GetTypes()
                .Where(mytype => mytype.GetInterfaces().Contains(InterfaceType));

            if (types.Count() == 0)
                throw new Exception($"Assembly no implementa {InterfaceType.Name}");

            if (types.Count() > 1)
                throw new Exception($"Assembly implementa mas de una {InterfaceType.Name}");

            try
            {
                return CompiledAssembly.CreateInstance(types.FirstOrDefault().FullName);
            }
            catch
            {
                return null;
            }
        }

        public object CreateObject(string parClassName)
        {
            if (parClassName.IsNullOrStringEmpty())
                return null;

            if (CompiledAssembly.IsNull())
                Build();

            if (CompiledAssembly.IsNull())
                return null;

            try
            {
                return CompiledAssembly.CreateInstance(parClassName);
            }
            catch
            {
                return null;
            }
        }

        public object CreateObject(string parClassName, object[] parArguments)
        {
            if (parClassName.IsNullOrStringEmpty())
                return null;

            if (CompiledAssembly.IsNull())
                Build();

            if (CompiledAssembly.IsNull())
                return null;

            var eFlags = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public;
            try
            {
                return CompiledAssembly.CreateInstance(
                    parClassName,
                    false,
                    eFlags,
                    null,
                    parArguments,
                    CultureInfo.InvariantCulture,
                    null
                );
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Crea y ejecuta un método de la clase
        /// </summary>
        /// <param name="parClassName">Nombre de la Clase</param>
        /// <param name="parMethodName">método a ejecutar</param>
        /// <param name="parArguments">Arreglo de Argumentos</param>
        /// <returns>regresa un objeto con el resultado de la ejecución del método</returns>
        /// <example>
        ///     oBuilder.CreateAndExecute("ClaseReporte","Crear", new object[1] {dtReporte })
        /// </example>
        public object CreateAndExecute(
            string parClassName,
            string parMethodName,
            object[] parArguments
        )
        {
            if (parClassName.IsNullOrStringEmpty())
                return null;

            if (CompiledAssembly.IsNull())
                Build();

            if (CompiledAssembly.IsNull())
                return null;

            try
            {
                var objInstance = CompiledAssembly.CreateInstance(parClassName);

                if (objInstance.IsNull())
                    return null;

                var objType = CompiledAssembly.GetType(parClassName, false);
                var methodInfo = objType.GetMethod(parMethodName);

                return methodInfo.Invoke(objInstance, parArguments);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.IsNull())
                    throw new Exception(ex.Message);
                throw new Exception(ex.InnerException.Message);
            }
        }

        public void RunMethod(
            string parClassName,
            string parMethodName,
            object parInstance,
            object[] parArguments
        )
        {
            if (parClassName.IsNullOrStringEmpty())
                return;

            if (CompiledAssembly.IsNull())
                return;

            if (parInstance.IsNull())
                return;

            var oType = CompiledAssembly.GetType(parClassName, false);
            var oMethodInfo = oType.GetMethod(parMethodName);

            oMethodInfo.Invoke(parInstance, parArguments);
        }

        public void SetProperty(
            string parClassName,
            string parPropertyName,
            object parInstance,
            object parValue
        )
        {
            if (parClassName.IsNullOrStringEmpty())
                return;

            if (CompiledAssembly.IsNull())
                return;

            if (parInstance.IsNull())
                return;

            var oType = CompiledAssembly.GetType(parClassName, false);
            var propInfo = oType.GetProperty(parPropertyName);

            propInfo.SetValue(parInstance, parValue, null);
        }

        #endregion " Métodos Públicos"

        #region " Métodos Estáticos"

        public static object GetProperty(object parInstance, string parPropertyName)
        {
            if (parPropertyName.IsNullOrStringEmpty())
                return null;

            if (parInstance.IsNull())
                return null;

            var objType = parInstance.GetType();
            var propInfo = objType.GetProperty(parPropertyName);

            return propInfo.GetValue(parInstance, null);
        }

        public static void SetProperty(object parInstance, string parPropertyName, object parValue)
        {
            if (parPropertyName.IsNullOrStringEmpty())
                return;

            if (parInstance.IsNull())
                return;

            var objType = parInstance.GetType();
            var propInfo = objType.GetProperty(parPropertyName);

            propInfo.SetValue(parInstance, parValue, null);
        }

        public static void RunMethod(object parInstance, string parMethodName)
        {
            RunMethod(parInstance, parMethodName, null);
        }

        public static void RunMethod(
            object parInstance,
            string parMethodName,
            object[] parArguments
        )
        {
            if (parMethodName.IsNullOrStringEmpty())
                return;

            if (parInstance.IsNull())
                return;

            var objType = parInstance.GetType();
            var methodInfo = objType.GetMethod(parMethodName);

            methodInfo.Invoke(parInstance, parArguments);
        }

        public static bool IsValidIdentifier(string parIdentifier)
        {
            using (
                var cSharpProvider = new CSharpCodeProvider(
                    new Dictionary<string, string> { { "CompilerVersion", "v4.0" } }
                )
            )
            {
                return cSharpProvider.IsValidIdentifier(parIdentifier);
            }
        }

        public static void ClearInstanceAssemblyCache(string parInstanceName)
        {
            var sFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var sPattern = string.Format("__{0}_*.dll", parInstanceName);

            var sAsmFiles = Directory.GetFiles(sFolder, sPattern, SearchOption.TopDirectoryOnly);

            if (sAsmFiles.IsNull() || sAsmFiles.Length == 0)
                return;

            foreach (var sCurrentFile in sAsmFiles)
                try
                {
                    File.Delete(sCurrentFile);
                }
                catch { }
        }

        #endregion " Métodos Estáticos"
    }
}
#pragma warning restore CS1591
