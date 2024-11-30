using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;

namespace WooW.SB.BlazorGenerator.BlazorGeneratorTools
{
    public class WoBlazorExecute
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Levantara el hilo con el cmd y enviara los commandos que se le definan.
        /// </summary>
        private Process _process = new Process();

        /// <summary>
        /// Ruta donde se encuentra el  proyecto de blazor que se busca ejecutar.
        /// </summary>
        private string _workDirectory = string.Empty;

        /// <summary>
        /// Bandera que mantiene el estado de si se encuentra o no en ejecución.
        /// </summary>
        private bool _isRunning = false;

        #endregion Variables globales

        #region Constructor

        /// <summary>
        /// Instancia que se cargara al levantar el software, y permitirá el funcionamiento en modo
        /// singleton de la clase.
        /// </summary>
        private static readonly WoBlazorExecute _instance = new WoBlazorExecute();

        /// <summary>
        /// Constructor que usara la clase al generar su instancia para el singleton.
        /// </summary>
        private WoBlazorExecute() { }

        /// <summary>
        /// Método a través del cual podemos obtener la instancia de la clase como singleton.
        /// </summary>
        /// <returns></returns>
        public static WoBlazorExecute GetInstance()
        {
            return _instance;
        }

        #endregion Constructor

        #region Estatus

        /// <summary>
        /// Retorna la bandera que se usa en la clase para almacenar cual es el estatus del
        /// proyecto, ya se a en ejecución o no.
        /// </summary>
        /// <returns></returns>
        public bool getStatus()
        {
            return _isRunning;
        }

        /// <summary>
        /// Verifica que la ejecución se encuentra activa.
        /// Ayuda a que no se levanten multiples hilos o a que no se rompa
        /// intentando cerrar una instancia ya cerrada.
        /// </summary>
        private void VerificarEjecucion()
        {
            try
            {
                var therads = _process.Threads;
                _isRunning = true;
            }
            catch (Exception ex)
            {
                _isRunning = false;
            }
        }

        #endregion Estatus

        #region Ejecutar Blazor

        /// <summary>
        /// Indica cual es el nombre del proyecto en ejecución, solo sirve para el log.
        /// Se inicializa en el método StartBlazor.
        /// </summary>
        private string _projectName = string.Empty;

        /// <summary>
        /// Ejecuta con el watch el proyecto con el nombre que se recibe por parámetro.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task StartBlazor(string projectName)
        {
            VerificarEjecucion();

            if (!_isRunning)
            {
                try
                {
                    if (_process != null)
                    {
                        StopBlazor();
                    }
                    _process = new Process();
                    _process.Exited += Process_Exited;
                    _workDirectory = $@"{_project.DirProyectTemp}\\{projectName}";
                    _process.EnableRaisingEvents = true;

                    _process.StartInfo.FileName = "cmd";
                    _process.StartInfo.Arguments = "/c dotnet watch run -- arg0";
                    _process.StartInfo.WorkingDirectory = _workDirectory;

                    _isRunning = true;
                    _process.Start();

                    _observer.SetLog(
                        woLog: _successExecution,
                        details: $@"Se ejecuto el proyecto de blazor ""{projectName}"" exitosamente."
                    );
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(
                        log: _executionException,
                        exceptionMessage: ex.Message
                    );
                }

                VerificarEjecucion();
            }
            else
            {
                _observer.SetLog(
                    woLog: _alreadyRunning,
                    details: $@"Se esta intentando ejecutar el proyecto de blazor ""{projectName}"" cuando este ya se encuentra en ejecución."
                );
            }
        }

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
        /// <exception cref="WoObserverException"></exception>
        private void Process_Exited(object sender, EventArgs e)
        {
            StopBlazor();
            VerificarEjecucion();

            ProcessStatusChangeEvt?.Invoke(this, _isRunning);
        }

        #endregion Ejecutar Blazor

        #region Detener Blazor

        /// <summary>
        /// Verifica si la ejecución esta corriendo y en caso de ser así, cierra la ventana
        /// y detona el dispose en el process.
        /// </summary>
        public void StopBlazor()
        {
            VerificarEjecucion();

            if (_isRunning)
            {
                try
                {
                    _process.Kill(true);
                    _process.Dispose();
                    _observer.SetLog(
                        woLog: _successStopExecution,
                        details: $@"Se detuvo el proyecto ""{_projectName}"" de blazor con éxito."
                    );
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(
                        log: _stopException,
                        exceptionMessage: ex.Message
                    );
                }
                VerificarEjecucion();
            }
            else
            {
                _observer.SetLog(_cantFindRunProyect);
            }
        }

        #endregion Detener Blazor

        #region Alertas

        private WoLog _successExecution = new WoLog()
        {
            CodeLog = "000",
            Title = "El proyecto blazor se ejecuto correctamente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoBlazorExecute",
                MethodOrContext = "StartBlazor"
            }
        };

        private WoLog _successStopExecution = new WoLog()
        {
            CodeLog = "000",
            Title = "El proyecto blazor se detuvo correctamente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoBlazorExecute",
                MethodOrContext = "StopBlazor"
            }
        };

        private WoLog _alreadyRunning = new WoLog()
        {
            CodeLog = "000",
            Title = "Se esta intentando ejecutar el proyecto ya en ejecución.",
            LogType = eLogType.Warning,
            FileDetails = new WoFileDetails()
            {
                Class = "WoBlazorExecute",
                MethodOrContext = "Iniciar"
            }
        };

        private WoLog _cantFindRunProyect = new WoLog()
        {
            CodeLog = "000",
            Title = "No hay ningún proyecto en ejecución.",
            Details = "No hay ningún proyecto en ejecución que detener.",
            UserMessage = "No hay ningún proyecto en ejecución que detener.",
            LogType = eLogType.Warning,
            FileDetails = new WoFileDetails()
            {
                Class = "WoBlazorExecute",
                MethodOrContext = "StopBlazor"
            }
        };

        private WoLog _executionException = new WoLog()
        {
            CodeLog = "000",
            Title = "Se Produjo una excepción al intentar ejecutar el proyecto blazor.",
            UserMessage =
                "Se produjo una excepción al momento de intentar ejecutar el proyecto de blazor.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoBlazorExecute",
                MethodOrContext = "Iniciar"
            }
        };

        private WoLog _stopException = new WoLog()
        {
            CodeLog = "000",
            Title = "Se Produjo una excepción al intentar detener el proyecto blazor.",
            UserMessage =
                "Se produjo una excepción al momento de intentar detener el proyecto de blazor.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoBlazorExecute",
                MethodOrContext = "StopBlazor"
            }
        };

        #endregion Alertas
    }
}
