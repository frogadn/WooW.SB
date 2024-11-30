using System;
using System.IO;
using System.Linq;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoMoveDirectory
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public void MoveFile(string oldPath, string newPath, bool createFullPath = true)
        {
            oldPath = oldPath.Replace("/", "\\");
            newPath = newPath.Replace("/", "\\");

            if (File.Exists(oldPath))
            {
                string[] folders = newPath.Split('\\');
                string basePath = newPath.Replace(folders.Last(), "");

                if (createFullPath)
                {
                    string pathFolder = folders[0];

                    for (int i = 1; i < folders.Count() - 1; i++)
                    {
                        pathFolder += "\\" + folders[i];
                        WoDirectory.CreateDirectory(pathFolder);
                    }

                    MoveFileInternal(oldPath, newPath);
                }
                else
                {
                    if (Directory.Exists(basePath))
                    {
                        MoveFileInternal(oldPath, newPath);
                    }
                }
            }
        }

        private void MoveFileInternal(string oldPath, string newPath)
        {
            if (File.Exists(oldPath))
            {
                try
                {
                    File.Move(oldPath, newPath);
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se movió el fichero con éxito.",
                            Details =
                                $@"Se movió exitosamente el fichero de la ruta ""{oldPath}"" a la ruta ""{newPath}"".",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoMoveDirectory",
                                MethodOrContext = "MoveDirectory"
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
                            Title = $@"Se produjo una excepción al intentar mover el fichero.",
                            Details =
                                $@"Se produjo una excepción al intentar mover el fichero del path ""{oldPath}"" al path ""{newPath}"".",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoMoveDirectory",
                                MethodOrContext = "MoveDirectory"
                            }
                        }
                    );
                }
            }
        }

        public void MoveDirectory(string oldPath, string newPath, bool createFullPath = true)
        {
            oldPath = oldPath.Replace("/", "\\");
            newPath = newPath.Replace("/", "\\");

            if (Directory.Exists(oldPath))
            {
                string[] folders = newPath.Split('\\');
                string basePath = newPath.Replace(folders.Last(), "");

                if (createFullPath)
                {
                    string pathFolder = folders[0];

                    for (int i = 1; i < folders.Count() - 1; i++)
                    {
                        pathFolder += "\\" + folders[i];
                        WoDirectory.CreateDirectory(pathFolder);
                    }

                    MoveDirectoryInternal(oldPath, newPath);
                }
                else
                {
                    if (Directory.Exists(basePath))
                    {
                        MoveDirectoryInternal(oldPath, newPath);
                    }
                }
            }
        }

        private void MoveDirectoryInternal(string oldPath, string newPath)
        {
            if (Directory.Exists(oldPath))
            {
                try
                {
                    Directory.Move(oldPath, newPath);
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se movió el directorio con éxito.",
                            Details =
                                $@"Se movió exitosamente el directorio de la ruta ""{oldPath}"" a la ruta ""{newPath}"".",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoMoveDirectory",
                                MethodOrContext = "MoveDirectory"
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
                            Title = $@"Se produjo una excepción al intentar mover el fichero.",
                            Details =
                                $@"Se produjo una excepción al intentar mover el fichero del path ""{oldPath}"" al path ""{newPath}"".",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoMoveDirectory",
                                MethodOrContext = "MoveDirectory"
                            }
                        }
                    );
                }
            }
        }
    }
}
