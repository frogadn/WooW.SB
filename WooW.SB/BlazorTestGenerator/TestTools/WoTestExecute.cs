using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WooW.SB.Config;

namespace WooW.SB.BlazorTestGenerator.TestTools
{
    public class WoTestExecute
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Nombre del test
        /// </summary>
        private string _testPath = string.Empty;

        /// <summary>
        /// Path del directorio que contiene el test
        /// </summary>
        private string _directoryPath = string.Empty;

        /// <summary>
        /// Nombre del test a ejecutar
        /// </summary>
        private string _testName = string.Empty;

        #endregion Atributos


        #region Método principal

        /// <summary>
        /// Método principal para orquestar la ejecución de test utilizando node y testCafe
        /// </summary>
        /// <param name="testName"></param>
        /// <exception cref="Exception"></exception>
        public async void ExecuteTest(string testPath)
        {
            try
            {
                _testPath = testPath;
                _testName = Path.GetFileName(_testPath);
                _directoryPath = testPath.Replace($"\\{_testName}", "");

                if (!File.Exists(_testPath))
                {
                    throw new Exception("El fichero no existe");
                }
                if (!Directory.Exists(_directoryPath))
                {
                    throw new Exception("El directorio no existe");
                }

                await Execute();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Método principal


        #region Ejecución del test

        /// <summary>
        /// Ejecuta con un process el test con la herramienta de TestCafe
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task Execute()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process testBlazor = new Process();

                // Settings del app que se ejecutara, en este caso PowerShell
                //testBlazor.EnableRaisingEvents = true;
                testBlazor.StartInfo.FileName = "cmd";

                // Pasamos los argumentos al process
                testBlazor.StartInfo.Arguments = $@"/c testcafe edge {_testName}";

                // Ruta de ejecución del comando
                testBlazor.StartInfo.WorkingDirectory = _directoryPath;

                // Settings de la salida de la consola
                //testBlazor.StartInfo.RedirectStandardOutput = true;
                //testBlazor.StartInfo.RedirectStandardError = true;
                //testBlazor.StartInfo.CreateNoWindow = true;
                //testBlazor.StartInfo.UseShellExecute = false;

                // Arrancamos el process
                testBlazor.Start();

                //testBlazor.OutputDataReceived += DataRecived;

                // En caso de que exista un error en el proceso arrojamos una exception.
                //if (error != string.Empty)
                //{
                //    throw new Exception(error);
                //}
                //else
                //{

                //}
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error durante la ejecución del comando para la ejecución del test. {ex.Message}"
                );
            }
        }

        #endregion Ejecución del test

        #region Envió de información a la consola de forma asíncrona

        /// <summary>
        /// Action que permite que lo que se le suscriba reciba la información resultante del test
        /// </summary>
        public Action<string> SendToConsoleEvt;

        /// <summary>
        /// Evento suscrito al controlador de eventos de la salida del process que ejecuta el test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataRecived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                SendToConsoleEvt?.Invoke(e.Data);
            }
        }

        #endregion Envió de información a la consola de forma asíncrona
    }
}
