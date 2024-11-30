using System;
using System.IO;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.BuildTest.BaseForm
{
    public class WoBuildBaseTemplate
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Método principal

        /// <summary>
        /// Construye el template principal para la ejecución del proyecto
        /// </summary>
        public void BuildBaseTemplate(string pathNewTest)
        {
            try
            {
                // Nombre del test
                string testName = Path.GetFileNameWithoutExtension(pathNewTest);

                // Recuperación del url en el proyecto de blazor
                string testPath = string.Empty;

                if (File.Exists($"{_project.DirProyectTemp}\\IntegralTestServer\\Program.cs"))
                {
                    testPath = $"{_project.DirProyectTemp}\\IntegralTestServer";
                }
                else if (File.Exists($"{_project.DirProyectTemp}\\IntegralTestWasm\\Program.cs"))
                {
                    testPath = $"{_project.DirProyectTemp}\\IntegralTestWasm";
                }

                string url = "URLDELPROYECTO_NO_MODIFICAR";

                if (testPath != string.Empty)
                {
                    string launchSettingsPath = $"{testPath}\\Properties\\launchSettings.json";
                    string lauchSettings = WoDirectory.ReadFile(launchSettingsPath);
                    string[] launchSettingsCol = lauchSettings.Split("profiles");
                    string[] launchSettingsCol1 = launchSettingsCol[1]
                        .Split($@"""applicationUrl"":");
                    string[] launchSettingsCol2 = launchSettingsCol1[1].Split(";");
                    url = launchSettingsCol2[0].Replace("\"", "").Replace(" ", "");
                }

                // Configuración del template T4
                WoTemplateBaseTestJs woTemplateBaseTestJs = new WoTemplateBaseTestJs();
                woTemplateBaseTestJs.Url = url;
                woTemplateBaseTestJs.TestName = testName;

                // Escritura del T4
                WoDirectory.WriteTemplate(pathNewTest, woTemplateBaseTestJs.TransformText());
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la construcción del template base. {ex.Message}");
            }
        }

        #endregion Método principal
    }
}
