using System;
using System.IO;
using System.Linq;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoCreateDirectory
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public void CreateDirectory(string pathDirectory, bool createAllPath = true)
        {
            pathDirectory = pathDirectory.Replace("/", "\\");

            if (!Directory.Exists(pathDirectory))
            {
                if (createAllPath)
                {
                    string[] folders = pathDirectory.Split('\\');
                    string pathFolder = folders[0];

                    for (int i = 1; i < folders.Count(); i++)
                    {
                        pathFolder += "\\" + folders[i];
                        CreateDirectoryInternal(pathFolder);
                    }
                }
                else
                {
                    CreateDirectoryInternal(pathDirectory);
                }
            }
        }

        private void CreateDirectoryInternal(string pathDirectory)
        {
            if (!Directory.Exists(pathDirectory))
            {
                try
                {
                    Directory.CreateDirectory(pathDirectory);
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = "Fichero creado correctamente.",
                            Details =
                                $@"El fichero en el path: {pathDirectory} fue creado correctamente.",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Path =
                                    "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                                Class = "WoCreateDirectory",
                                MethodOrContext = "CreateDirectory",
                                LineAprox = "27"
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
                            Title = "Ocurrió un error al eliminar el path",
                            Details = $@"Ocurrió un error al intentar crear el fichero.",
                            UserMessage = $@"Ocurrió un error al intentar crear el fichero.",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Path =
                                    "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                                Class = "WoCreateDirectory",
                                MethodOrContext = "CreateDirectory",
                                LineAprox = "27"
                            }
                        }
                    );
                }
            }
        }

        public void CreateFile(string pathFile, bool createAllPath = true)
        {
            pathFile = pathFile.Replace("/", "\\");

            if (!File.Exists(pathFile))
            {
                if (createAllPath)
                {
                    string[] folders = pathFile.Split('\\');
                    string pathFolder = folders[0];

                    for (int i = 1; i < folders.Count() - 1; i++)
                    {
                        pathFolder += "\\" + folders[i];
                        CreateDirectoryInternal(pathFolder);
                    }

                    CreateFileInternal(pathFile);
                }
                else
                {
                    CreateFileInternal(pathFile);
                }
            }
        }

        private void CreateFileInternal(string pathFile)
        {
            if (!File.Exists(pathFile))
            {
                try
                {
                    File.Create(pathFile).Dispose();
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = "El fichero se creo correctamente.",
                            Details =
                                $@"El fichero con el path {pathFile} fue creado correctamente.",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Path =
                                    "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                                Class = "WoCreateDirectory",
                                MethodOrContext = "CreateFile"
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
                            Title = "Ocurrió un error al intentar crear el fichero.",
                            Details =
                                $@"Ocurrió un error al intentar crear el fichero con el path {pathFile}.",
                            UserMessage =
                                $@"Ocurrió un error al intentar crear el fichero con el path {pathFile}.",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Path =
                                    "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                                Class = "WoCreateDirectory",
                                MethodOrContext = "CreateFile"
                            }
                        }
                    );
                }
            }
        }
    }
}
