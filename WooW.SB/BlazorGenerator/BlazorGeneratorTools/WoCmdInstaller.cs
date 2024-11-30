using System;
using System.Diagnostics;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;

namespace WooW.SB.BlazorGenerator.BlazorGeneratorTools
{
    public class WoCmdInstaller
    {
        /// <summary>
        /// Instancia del proceso que permite enviar commands al cmd.
        /// </summary>
        private Process _process = new Process();

        /// <summary>
        /// Permite la complementación de mutex sobre código para proteger algoritmos.
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// Constructor que configura la instancia del process.
        /// </summary>
        /// <param name="workDirectory"></param>
        public WoCmdInstaller(string workDirectory)
        {
            _process.EnableRaisingEvents = false;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.FileName = "cmd";
            _process.StartInfo.WorkingDirectory = workDirectory;
        }

        /// <summary>
        /// Ejecuta el proceso con los argumentos que se reciben por parámetro.
        /// </summary>
        /// <param name="arguments"></param>
        public string Execute(string arguments)
        {
            string output = string.Empty;

            lock (lockObject)
            {
                try
                {
                    _process.StartInfo.RedirectStandardOutput = true;
                    _process.StartInfo.RedirectStandardError = true;
                    _process.StartInfo.CreateNoWindow = true;
                    _process.StartInfo.UseShellExecute = false;

                    _process.StartInfo.Arguments = arguments;
                    _process.Start();

                    output = _process.StandardOutput.ReadToEnd();

                    _process.WaitForExit();
                }
                catch (Exception ex)
                {
                    _process.Dispose();
                    _failExecution.Details =
                        $@"Ocurrió un error al intentar ejecutar los comandos: ""{arguments}"".";
                    _failExecution.UserMessage =
                        $@"Ocurrió un error al intentar ejecutar los comandos: ""{arguments}"".";
                    _failExecution.ExceptionMessage = ex.Message;

                    output =
                        $@"Ocurrió un error al intentar ejecutar los comandos: ""{arguments}"". {ex.Message}";
                    throw new WoObserverException(_failExecution);
                }
            }

            return output;
        }

        /// <summary>
        /// Saca de memoria la instancia del process.
        /// </summary>
        public void Dispose()
        {
            _process.Dispose();
        }

        #region Logs

        private WoLog _failExecution = new WoLog()
        {
            CodeLog = "000",
            Title = "Ocurrió un error al intentar ejecutar el comando.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoCmdInstaller",
                MethodOrContext = "Execute"
            }
        };

        #endregion Logs
    }
}
