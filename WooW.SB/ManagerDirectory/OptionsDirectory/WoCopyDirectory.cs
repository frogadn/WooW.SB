using System;
using System.IO;
using System.Linq;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoCopyDirectory
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public void CopyFile(
            string oldPath,
            string newPath,
            bool replace,
            bool createFullPath = true
        )
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

                    CopyFileInternal(oldPath, newPath, replace);
                }
                else
                {
                    if (Directory.Exists(basePath))
                    {
                        CopyFileInternal(oldPath, newPath, replace);
                    }
                }
            }
        }

        private void CopyFileInternal(string oldPath, string newPath, bool remplace)
        {
            if (File.Exists(oldPath))
            {
                try
                {
                    if (remplace && File.Exists(newPath))
                    {
                        WoDirectory.DeleteFile(newPath);
                    }

                    File.Copy(oldPath, newPath);
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se copio el fichero con éxito.",
                            Details =
                                $@"Se copio exitosamente el fichero de la ruta ""{oldPath}"" a la ruta ""{newPath}"".",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoCopyDirectory",
                                MethodOrContext = "WoCopyDirectory"
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
                            Title = $@"Se produjo una excepción al intentar copiar el fichero.",
                            Details =
                                $@"Se produjo una excepción al intentar copiar el fichero del path ""{oldPath}"" al path ""{newPath}"".",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoCopyDirectory",
                                MethodOrContext = "WoCopyDirectory"
                            }
                        }
                    );
                }
            }
        }

        public void CopyDirectory(string oldPath, string newPath, bool createFullPath = true)
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

                    CopyDirectoryInternal(oldPath, newPath);
                }
                else
                {
                    if (Directory.Exists(basePath))
                    {
                        CopyDirectoryInternal(oldPath, newPath);
                    }
                }
            }
        }

        private void CopyDirectoryInternal(string oldPath, string newPath)
        {
            if (Directory.Exists(oldPath))
            {
                try
                {
                    Directory.CreateDirectory(newPath);

                    var internalDirectories = Directory.GetDirectories(oldPath);

                    if (internalDirectories.ToList().Count > 0)
                    {
                        foreach (var directory in internalDirectories)
                        {
                            string directoryName = directory.Split('\\').Last();

                            var newDirectory = $@"{newPath}\{directoryName}";
                            CopyDirectoryInternal(directory, newDirectory);
                        }
                    }

                    var internalFiles = Directory.GetFiles(oldPath);

                    if (internalFiles.ToList().Count > 0)
                    {
                        foreach (var file in internalFiles)
                        {
                            string fileName = file.Split('\\').Last();

                            WoDirectory.CopyFile(oldPath: file, newPath: $@"{newPath}\{fileName}");
                        }
                    }

                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se copio el directorio con éxito.",
                            Details =
                                $@"Se copio exitosamente el directorio de la ruta ""{oldPath}"" a la ruta ""{newPath}"".",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoCopyDirectory",
                                MethodOrContext = "WoCopyDirectory"
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
                            Title = $@"Se produjo una excepción al intentar copiar el fichero.",
                            Details =
                                $@"Se produjo una excepción al intentar copiar el fichero del path ""{oldPath}"" al path ""{newPath}"".",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoCopyDirectory",
                                MethodOrContext = "WoCopyDirectory"
                            }
                        }
                    );
                }
            }
        }
    }
}
