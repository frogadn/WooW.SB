using System;
using System.IO;
using System.Linq;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoWriteFile
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton



        public void WriteFile(string path, string data, bool createAllPath = true)
        {
            path = path.Replace("/", "\\");

            if (createAllPath)
            {
                string[] folders = path.Split('\\');
                string pathFolder = folders[0];

                for (int i = 1; i < folders.Count() - 1; i++)
                {
                    pathFolder += "\\" + folders[i];
                    WoDirectory.CreateDirectory(pathFolder);
                }

                WriteFileInternal(path, data);
            }
            else
            {
                WriteFileInternal(path, data);
            }
        }

        private void WriteFileInternal(string path, string data)
        {
            try
            {
                File.WriteAllText(path, data);
                _observer.SetLog(
                    new WoLog()
                    {
                        CodeLog = "000",
                        Title = "Fichero escrito correctamente.",
                        Details =
                            $@"El fichero en el path : {path}, fue escrito correctamente con la información proporcionada.",
                        LogType = eLogType.Information,
                        FileDetails = new WoFileDetails()
                        {
                            Path =
                                "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                            Class = "WoWriteFile",
                            MethodOrContext = "WriteFile"
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
                        Title = "Ocurrió un error al intentar escribir el fichero.",
                        Details = $@"Se produjo una exception al intentar escribir el fichero.",
                        UserMessage = $@"Se produjo una exception al intentar escribir el fichero.",
                        ExceptionMessage = ex.Message,
                        LogType = eLogType.Error,
                        FileDetails = new WoFileDetails()
                        {
                            Path =
                                "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                            Class = "WoWriteFile",
                            MethodOrContext = "WriteFile"
                        }
                    }
                );
            }
        }

        public void AddText(string path, string data)
        {
            try
            {
                path = path.Replace("/", "\\");
                File.AppendAllText(path, data);
                _observer.SetLog(
                    new WoLog()
                    {
                        CodeLog = "000",
                        Title = "El texto se añadió correctamente.",
                        Details = $@"El texto se añadió correctamente al fichero: {path}",
                        LogType = eLogType.Information,
                        FileDetails = new WoFileDetails()
                        {
                            Path =
                                "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                            Class = "WoWriteFile",
                            MethodOrContext = "AddText"
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
                        Title = "Ocurrió un error al intentar añadir el texto.",
                        Details =
                            $@"Se produjo una excepción al intentar añadir el texto al fichero en el path: {path}.",
                        UserMessage =
                            $@"Se produjo una excepción al intentar añadir el texto al fichero en el path: {path}.",
                        ExceptionMessage = ex.Message,
                        LogType = eLogType.Error,
                        FileDetails = new WoFileDetails()
                        {
                            Path =
                                "C:\\Frog\\WooW.SB\\WooW.SB\\WooW.SB\\ManagerDirectory\\OptionsDirectory",
                            Class = "WoWriteFile",
                            MethodOrContext = "AddText"
                        }
                    }
                );
            }
        }
    }
}
