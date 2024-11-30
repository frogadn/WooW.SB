using System;
using System.IO;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoReadFile
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public string ReadFile(string path)
        {
            string result = string.Empty;
            path = path.Replace("/", "\\");

            if (File.Exists(path))
            {
                try
                {
                    result = File.ReadAllText(path);
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se recupero la información exitosamente.",
                            Details = $@"Se recupero la información del fichero: {path}",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoReadFile",
                                MethodOrContext = "ReadFile"
                            }
                        }
                    );
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se produjo una excepción al intentar leer el fichero.",
                            Details =
                                $@"Se produjo una excepción al intentar leer el fichero en la ruta ""{path}"".",
                            UserMessage =
                                $@"Se produjo una excepción al intentar leer el fichero en la ruta ""{path}"".",
                            LogType = eLogType.Error,
                            ExceptionMessage = ex.Message,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoReadFile",
                                MethodOrContext = "ReadFile"
                            }
                        }
                    );
                }
            }
            else
            {
                throw new Exception($@"El fichero que esta intentando leer, no existe. {path}");
                //throw new WoObserverException(
                //    new WoLog()
                //    {
                //        CodeLog = "000",
                //        Title = $@"El fichero que esta intentando leer, no existe.",
                //        Details =
                //            $@"El fichero en el path {path}. no existe o no ex posible encontrarlo.",
                //        UserMessage =
                //            $@"El fichero en el path {path}. no existe o no ex posible encontrarlo.",
                //        LogType = eLogType.Error,
                //        FileDetails = new WoFileDetails()
                //        {
                //            Class = "WoReadFile",
                //            MethodOrContext = "ReadFile"
                //        }
                //    }
                //);
            }

            return result;
        }
    }
}
