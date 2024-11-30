using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.TestServices
{
    public class ServiceTestExecute
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que estamos trabajando
        /// </summary>
        public Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Funciones para manejo y ejecución de codigo para los test
        /// </summary>
        private SyntaxEditorHelper _syntaxEditorHelper;

        /// <summary>
        /// Funciones para manejo y ejecución de codigo para los test de app
        /// </summary>
        private SyntaxEditorHelper _syntaxEditorHelperApp;

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase, principalmente inicializa atributos
        /// </summary>
        public ServiceTestExecute(List<string> systemRefCol, List<string> baseRefCol)
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _syntaxEditorHelper = new SyntaxEditorHelper(
                SystemReferencias: systemRefCol,
                AssemblyPath: assemblyPath,
                Referencias: baseRefCol
            );
        }

        #endregion Constructor


        #region Ejecución como ensamblado interno

        /// <summary>
        /// Ejecutamos el test como ensamblado interno
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool EjecuteInternalAssembly(string testPath)
        {
            try
            {
                bool correctExecuteResult = true;

                _project.AssemblyModelCargado = true;

                string rawCode = WoDirectory.ReadFile(testPath);

                List<ScriptErrorDescriptor> errorListCol;
                Assembly testInternalAssembly = _syntaxEditorHelper.CreateInstanceForRun(
                    rawCode,
                    out errorListCol
                );

                if (testInternalAssembly == null)
                {
                    SetErrorEvt?.Invoke(errorListCol);
                    throw new Exception("Error en el ensamblado");
                }

                return correctExecuteResult;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error en la ejecución del proyecto como ensamblado interno. {ex.Message}"
                );
            }
        }

        #endregion Ejecución como ensamblado interno


        #region Gestión de errores

        /// <summary>
        /// Evento que envía los errores
        /// </summary>
        public Action<List<ScriptErrorDescriptor>> SetErrorEvt;

        #endregion Gestión de errores
    }
}
