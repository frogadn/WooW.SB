using System;
using System.IO;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoRenameDirectory
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public void RenameFile(string path, string newName)
        {
            path = path.Replace("/", "\\");
            if (File.Exists(path))
            {
                try
                {
                    string[] pathCol = path.Split('\\');
                    if (pathCol.Length < 2)
                        pathCol = path.Split('/');

                    if (pathCol.Length < 2)
                    {
                        throw new WoObserverException(
                            new WoLog()
                            {
                                CodeLog = "000",
                                Title = $@"Existe un problema con la ruta.",
                                Details =
                                    $@"La ruta ""{path}"", es posible que se encuentre incorrecta.",
                                LogType = eLogType.Error,
                                FileDetails = new WoFileDetails()
                                {
                                    Class = "WoRenameDirectory",
                                    MethodOrContext = "RenameFile"
                                }
                            }
                        );
                    }

                    string newPath =
                        path.Substring(0, (path.Length - pathCol[pathCol.Length - 1].Length))
                        + newName;

                    File.Move(path, newPath);
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se actualizo con éxito el nombre del fichero.",
                            Details =
                                $@"Se actualizo exitosamente el nombre del fichero ""{pathCol[pathCol.Length - 1]}"" a ""{newName}"".",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoRenameDirectory",
                                MethodOrContext = "RenameFile"
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
                            Title = $@"Se produjo una excepción al intentar renombrar el fichero.",
                            Details =
                                $@"Se produjo una excepción al intentar renombrar el fichero en el path ""{path}"", es posible que no sea accesible.",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoRenameDirectory",
                                MethodOrContext = "RenameFile"
                            }
                        }
                    );
                }
            }
            else
            {
                throw new WoObserverException(
                    new WoLog()
                    {
                        CodeLog = "000",
                        Title = $@"No existe el fichero que intenta renombrar.",
                        Details =
                            $@"El fichero en el path ""{path}"", no existe o es imposible de acceder.",
                        UserMessage =
                            $@"El fichero en el path ""{path}"", no existe o es imposible de acceder.",
                        LogType = eLogType.Error,
                        FileDetails = new WoFileDetails()
                        {
                            Class = "WoRenameDirectory",
                            MethodOrContext = "RenameFile"
                        }
                    }
                );
            }
        }

        public void RenameDirectory(string path, string newName)
        {
            path = path.Replace("/", "\\");
            if (Directory.Exists(path))
            {
                try
                {
                    string[] pathCol = path.Split('\\');
                    if (pathCol.Length < 2)
                        pathCol = path.Split('/');

                    if (pathCol.Length < 2)
                    {
                        throw new WoObserverException(
                            new WoLog()
                            {
                                CodeLog = "000",
                                Title = $@"Existe un problema con la ruta.",
                                Details =
                                    $@"La ruta ""{path}"", es posible que se encuentre incorrecta.",
                                LogType = eLogType.Error,
                                FileDetails = new WoFileDetails()
                                {
                                    Class = "WoRenameDirectory",
                                    MethodOrContext = "RenameDirectory"
                                }
                            }
                        );
                    }

                    string newPath =
                        path.Substring(0, (path.Length - pathCol[pathCol.Length - 1].Length))
                        + newName;

                    Directory.Move(path, newPath);
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se actualizo con éxito el nombre del directorio.",
                            Details =
                                $@"Se actualizo exitosamente el nombre del directorio ""{pathCol[pathCol.Length - 1]}"" a ""{newName}"".",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoRenameDirectory",
                                MethodOrContext = "RenameDirectory"
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
                            Title =
                                $@"Se produjo una excepción al intentar renombrar el directorio.",
                            Details =
                                $@"Se produjo una excepción al intentar renombrar el directorio en el path ""{path}"", es posible que no sea accesible.",
                            ExceptionMessage = ex.Message,
                            LogType = eLogType.Error,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoRenameDirectory",
                                MethodOrContext = "RenameDirectory"
                            }
                        }
                    );
                }
            }
            else
            {
                WoDirectory.CreateDirectory(path);
            }
        }
    }
}
