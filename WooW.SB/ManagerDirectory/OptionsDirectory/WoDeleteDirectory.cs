using System;
using System.IO;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoDeleteDirectory
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public void DeleteDirectory(string pathDirectory)
        {
            pathDirectory = pathDirectory.Replace("/", "\\");
            if (Directory.Exists(pathDirectory))
            {
                try
                {
                    Directory.Delete(pathDirectory, true);
                    _deleteSuccess.Details =
                        $@"El fichero con el path: {pathDirectory} se elimino correctamente.";
                    _deleteSuccess.FileDetails.MethodOrContext = "DeleteDirectory";
                    _observer.SetLog(_deleteSuccess);
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = "El fichero no se pudo eliminar",
                            Details = $@"El fichero no se pudo eliminar",
                            UserMessage = $@"El fichero no se pudo eliminar",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Path =
                                    "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                                Class = "WoDeleteDirectory",
                                MethodOrContext = "DeleteDirectory",
                                LineAprox = "26"
                            }
                        }
                    );
                }
            }
            else
            {
                throw new WoObserverException(_pathNoFound);
            }
        }

        public void DeleteFile(string pathFile)
        {
            pathFile = pathFile.Replace("/", "\\");
            if (File.Exists(pathFile))
            {
                try
                {
                    File.Delete(pathFile);
                    _deleteSuccess.Details =
                        $@"El fichero con el path: {pathFile} se elimino correctamente.";
                    _deleteSuccess.FileDetails.MethodOrContext = "DeleteFile";
                    _observer.SetLog(_deleteSuccess);
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = "El fichero no se pudo eliminar",
                            Details = $@"El fichero no se pudo eliminar",
                            UserMessage = $@"El fichero no se pudo eliminar",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Path =
                                    "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                                Class = "WoDeleteDirectory",
                                MethodOrContext = "DeleteFile",
                                LineAprox = "65"
                            }
                        }
                    );
                }
            }
            else
            {
                throw new WoObserverException(_pathNoFound);
            }
        }

        #region Alertas

        private WoLog _pathNoFound = new WoLog()
        {
            CodeLog = "000",
            Title = "El path no se encontró",
            Details =
                $@"El path que se esta intentando eliminar no se encontró o no se puede acceder.",
            UserMessage =
                $@"El path que se esta intentando eliminar no se encontró o no se puede acceder.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Path = "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                Class = "WoDeleteDirectory"
            }
        };

        private WoLog _deleteSuccess = new WoLog()
        {
            CodeLog = "000",
            Title = "El fichero se elimino correctamente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Path = "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                Class = "WoDeleteDirectory"
            }
        };

        #endregion Alertas
    }
}
