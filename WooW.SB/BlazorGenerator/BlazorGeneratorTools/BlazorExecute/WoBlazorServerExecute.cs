using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;

namespace WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute
{
    public class WoBlazorServerExecute
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Constructor singleton

        /// <summary>
        /// Instancia que se cargara al levantar el software, y permitirá el funcionamiento en modo
        /// singleton de la clase.
        /// </summary>
        private static readonly WoBlazorServerExecute _instance = new WoBlazorServerExecute();

        /// <summary>
        /// Constructor que usara la clase al generar su instancia para el singleton.
        /// </summary>
        private WoBlazorServerExecute() { }

        /// <summary>
        /// Método a través del cual podemos obtener la instancia de la clase como singleton.
        /// </summary>
        /// <returns></returns>
        public static WoBlazorServerExecute GetInstance()
        {
            return _instance;
        }

        #endregion Constructor singleton

        #region Atributos

        /// <summary>
        /// Path del proyecto a ejecutar.
        /// </summary>
        private string _workDirectory = string.Empty;

        /// <summary>
        /// Nombre del proyecto a ejecutar
        /// </summary>
        private string _projectName = string.Empty;

        /// <summary>
        /// Url de blazor estática donde levantar blazor fuera de los perfiles d ejecución
        /// </summary>
        private string _urlBlazor = "https://localhost:5201";

        /// <summary>
        /// Action que se detona cuando el proyecto de blazor ya se encuentra en ejecución
        /// </summary>
        public Action BlazorRuningEvt;

        /// <summary>
        /// Indica si ya se disparo el action que indica que el proyecto ya se
        /// encuentra el ejecución para evitar que se re ejecute
        /// </summary>
        public bool SendedRunInfo = false;

        #endregion Atributos


        #region Envió de información a la consola

        /// <summary>
        /// Action que envió la información de la ejecución a la consola.
        /// </summary>
        public Action<string> SendToConsoleEvt;

        /// <summary>
        /// Método suscrito al controlador de eventos de la salida de la ejecución del proceso.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private async void DataRecived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data != null)
                {
                    if (
                        e.Data.Contains("error")
                        && e.Data.Contains("Could not copy")
                        && e.Data.Contains("The file is locked by")
                    )
                    {
                        WoBlazorExecuteHelper.StopExtraBlazorExecute();

                        _simpleExecutionProcess.OutputDataReceived -= DataRecived;
                        _wathcExecutionProcess.OutputDataReceived -= DataRecived;

                        SendToConsoleEvt?.Invoke("CLSCLEAR");

                        if (_projectName == string.Empty)
                        {
                            throw new Exception(
                                "Error en la ejecución, no se a definido el proyecto a ejecutar"
                            );
                        }

                        if (_watchRunning)
                        {
                            await StartWatchServer(_projectName);
                        }
                        else
                        {
                            await StartSimpleServer(_projectName);
                        }
                    }
                    else
                    {
                        SendToConsoleEvt?.Invoke(e.Data);

                        if (e.Data.ToLower().Contains("https://") && !SendedRunInfo)
                        {
                            SendedRunInfo = true;
                            BlazorRuningEvt?.Invoke();

                            WoBlazorExecuteHelper.StartNavigator(_urlBlazor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al enviar la información a la consola por el controlador de eventos. {ex.Message}"
                );
            }
        }

        #endregion Envió de información a la consola


        #region Ejecución con watcher

        /// <summary>
        /// Proceso con el hilo con la ejecución del proyecto.
        /// </summary>
        private Process _wathcExecutionProcess = new Process();

        /// <summary>
        /// Bandera de control que indica si el proceso de ejecución esta corriendo.
        /// </summary>
        private bool _watchRunning = false;

        /// <summary>
        /// Intenta recuperar si el proceso tiene algún hilo en ejecución,
        /// en caso de que no lo tenga, se retorna false, en caso contrario
        /// retorna true.
        /// </summary>
        /// <returns></returns>
        private bool WatchIsRunning()
        {
            try
            {
                var threads = _wathcExecutionProcess.Threads;
                _watchRunning = true;
            }
            catch (Exception ex)
            {
                _watchRunning = false;
            }

            return _watchRunning;
        }

        /// <summary>
        /// Inicia el proceso de ejecución del proyecto.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task StartWatchServer(string projectName)
        {
            if (!IsRuning())
            {
                try
                {
                    _projectName = projectName;

                    if (_wathcExecutionProcess != null)
                    {
                        StopWatchServer();
                    }

                    _wathcExecutionProcess = new Process();
                    _wathcExecutionProcess.Exited += Process_Exited;
                    _workDirectory = $@"{_project.DirProyectTemp}\\{projectName}";
                    _wathcExecutionProcess.EnableRaisingEvents = true;

                    _wathcExecutionProcess.StartInfo.FileName = "cmd";
                    _wathcExecutionProcess.StartInfo.Arguments =
                        $@"/c dotnet watch run --urls ""{_urlBlazor}"" --arg0 --woow";
                    _wathcExecutionProcess.StartInfo.WorkingDirectory = _workDirectory;

                    _wathcExecutionProcess.StartInfo.RedirectStandardOutput = true;
                    _wathcExecutionProcess.StartInfo.RedirectStandardError = true;
                    _wathcExecutionProcess.StartInfo.CreateNoWindow = true;
                    _wathcExecutionProcess.StartInfo.UseShellExecute = false;

                    _wathcExecutionProcess.Start();
                    _watchRunning = true;

                    _wathcExecutionProcess.OutputDataReceived += DataRecived;

                    _wathcExecutionProcess.BeginOutputReadLine();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $@"Ocurrió un error al intentar realizar la ejecución: {ex.Message}"
                    );
                }
            }
            else
            {
                throw new Exception(
                    "Solo se puede realizar la ejecución de server una vez en simultaneo ya sea server o wasm"
                );
            }
        }

        #endregion Ejecución con watcher


        #region Ejecución simple

        /// <summary>
        /// Proceso con el hilo con la objeción del proyecto.
        /// </summary>
        private Process _simpleExecutionProcess = new Process();

        /// <summary>
        /// Bandera de control que indica si el proceso de ejecución esta corriendo.
        /// </summary>
        private bool _simpleRunning = false;

        /// <summary>
        /// Intenta recuperar si el proceso tiene algún hilo en ejecución,
        /// en caso de que no lo tenga, se retorna false, en caso contrario
        /// retorna true.
        /// </summary>
        /// <returns></returns>
        private bool SimpleIsRunning()
        {
            try
            {
                var threads = _simpleExecutionProcess.Threads;
                _simpleRunning = true;
            }
            catch (Exception ex)
            {
                _simpleRunning = false;
            }

            return _simpleRunning;
        }

        /// <summary>
        /// Inicia el proceso de ejecución del proyecto.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public async Task StartSimpleServer(string projectName)
        {
            if (!IsRuning())
            {
                try
                {
                    _projectName = projectName;

                    if (_simpleExecutionProcess != null)
                    {
                        StopSimpleServer();
                    }

                    _simpleExecutionProcess = new Process();

                    _simpleExecutionProcess.Exited += Process_Exited;

                    _workDirectory = $@"{_project.DirProyectTemp}\\{projectName}";
                    _simpleExecutionProcess.EnableRaisingEvents = true;

                    _simpleExecutionProcess.StartInfo.FileName = "cmd";
                    _simpleExecutionProcess.StartInfo.Arguments =
                        $@"/c dotnet run --urls ""{_urlBlazor}"" --arg0 --woow";
                    _simpleExecutionProcess.StartInfo.WorkingDirectory = _workDirectory;

                    _simpleExecutionProcess.StartInfo.RedirectStandardOutput = true;
                    _simpleExecutionProcess.StartInfo.RedirectStandardError = true;
                    _simpleExecutionProcess.StartInfo.CreateNoWindow = true;
                    _simpleExecutionProcess.StartInfo.UseShellExecute = false;

                    _simpleRunning = true;
                    _simpleExecutionProcess.Start();

                    _simpleExecutionProcess.OutputDataReceived += DataRecived;

                    _simpleExecutionProcess.BeginOutputReadLine();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $@"Ocurrió un error al intentar realizar la ejecución: {ex.Message}"
                    );
                }
            }
            else
            {
                throw new Exception(
                    "Solo se puede realizar la ejecución de server una vez en simultaneo ya sea server o wasm"
                );
            }
        }

        #endregion Ejecución simple


        #region Controladores de eventos y observadores

        /// <summary>
        /// Controlador de eventos que se detona cuando se modifica el estatus de la ejecución desde
        /// una forma externa al código.
        /// </summary>
        public EventHandler<bool> ProcessStatusChangeEvt;

        /// <summary>
        /// Método suscrito al controlador de evento del process para cuando se detenga la ejecución
        /// de una forma externa principalmente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Process_Exited(object sender, EventArgs e)
        {
            if (_watchRunning)
            {
                StopWatchServer();
            }

            if (_simpleRunning)
            {
                StopSimpleServer();
            }

            ProcessStatusChangeEvt?.Invoke(this, IsRuning());
        }

        #endregion Controladores de eventos y observadores


        #region Detiene la ejecución

        /// <summary>
        /// Detiene el proyecto en ejecución de forma independiente.
        /// </summary>
        public void GeneralStop()
        {
            if (_watchRunning)
            {
                StopWatchServer();
            }

            if (_simpleRunning)
            {
                StopSimpleServer();
            }

            ProcessStatusChangeEvt?.Invoke(this, IsRuning());
        }

        /// <summary>
        /// Detiene el proceso de ejecución del proyecto.
        /// </summary>
        public void StopWatchServer()
        {
            if (WatchIsRunning())
            {
                try
                {
                    _wathcExecutionProcess.Kill(true);
                    _wathcExecutionProcess.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $@"Ocurrió un error al intentar detener la ejecución: {ex.Message}"
                    );
                }
            }

            WatchIsRunning();
        }

        /// <summary>
        /// Detiene el proceso de ejecución del proyecto.
        /// </summary>
        public void StopSimpleServer()
        {
            SendedRunInfo = false;

            if (SimpleIsRunning())
            {
                try
                {
                    _simpleExecutionProcess.Kill(true);
                    _simpleExecutionProcess.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $@"Ocurrió un error al intentar detener la ejecución: {ex.Message}"
                    );
                }
            }

            SimpleIsRunning();
        }

        #endregion Detiene la ejecución


        #region Info

        /// <summary>
        /// Comprueba si alguno de los dos procesos esta corriendo y en caso de que si
        /// retorna true en caso contrario false.
        /// </summary>
        /// <returns></returns>
        public bool IsRuning()
        {
            bool isRuning = ((WatchIsRunning()) || (SimpleIsRunning())) ? true : false;
            return isRuning;
        }

        #endregion Info
    }
}
