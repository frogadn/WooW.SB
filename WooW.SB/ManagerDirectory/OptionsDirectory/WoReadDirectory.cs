using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoReadDirectory
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public List<string> ReadDirectoryFiles(string path, bool onlyNames = false)
        {
            List<string> resultCol = new List<string>();
            path = path.Replace("/", "\\");

            if (Directory.Exists(path))
            {
                try
                {
                    var result = Directory.GetFiles(path);
                    resultCol = result.ToList();
                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se recupero la información exitosamente.",
                            Details = $@"Se recupero la información del directorio: {path}",
                            LogType = eLogType.Information,
                            FileDetails = new WoFileDetails()
                            {
                                Class = "WoReadFile",
                                MethodOrContext = "ReadFile"
                            }
                        }
                    );

                    if (onlyNames)
                    {
                        resultCol.Clear();
                        foreach (string filePath in result.ToList())
                        {
                            string[] filePathCol = filePath.Split('\\');
                            string[] nameCol = filePathCol.Last().Split('.');
                            string fileName = string.Empty;
                            for (int i = 0; i < nameCol.Length - 1; i++)
                            {
                                fileName +=
                                    ((i + 1) == (nameCol.Length - 1))
                                        ? $@"{nameCol[i]}"
                                        : $@"{nameCol[i]}.";
                            }
                            resultCol.Add(fileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se produjo una excepción al intentar leer el fichero.",
                            Details =
                                $@"Se produjo una excepción al intentar leer el directorio en la ruta ""{path}"".",
                            UserMessage =
                                $@"Se produjo una excepción al intentar leer el directorio en la ruta ""{path}"".",
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
                WoDirectory.CreateDirectory(path);
                XtraMessageBox.Show(
                    $@"El directorio en el path {path}. Aun no existia y acaba de ser creado",
                    $@"Directorio no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            return resultCol;
        }

        public List<(string path, string name)> ReadDirectoryFilesPathName(string path, bool create)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    if (create)
                    {
                        WoDirectory.CreateDirectory(path);
                        XtraMessageBox.Show(
                            text: $@"El directorio en el path {path}. Aun no existía y acaba de ser creado",
                            caption: $@"Alert",
                            buttons: MessageBoxButtons.OK,
                            icon: MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        throw new Exception("El directorio que esta intentando leer no existe.");
                    }
                }

                if (Directory.Exists(path))
                {
                    List<(string path, string name)> resultCol =
                        new List<(string path, string name)>();

                    path = path.Replace("/", "\\");

                    var result = Directory.GetFiles(path);

                    foreach (string filePath in result.ToList())
                    {
                        string[] filePathCol = filePath.Split('\\');
                        string[] nameCol = filePathCol.Last().Split('.');
                        string fileName = string.Empty;
                        for (int i = 0; i < nameCol.Length - 1; i++)
                        {
                            fileName +=
                                ((i + 1) == (nameCol.Length - 1))
                                    ? $@"{nameCol[i]}"
                                    : $@"{nameCol[i]}.";
                        }
                        resultCol.Add((path: filePath, name: fileName));
                    }

                    return resultCol;
                }
                else
                {
                    throw new Exception("El directorio que esta intentando leer no existe.");
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                XtraMessageBox.Show(
                    text: $@"Error al leer el directorio en el path {path}. {ex.Message}",
                    caption: $@"Error",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }

            return new List<(string path, string name)>();
        }

        public List<string> ReadDirectoryDirectories(string path, bool onlyNames)
        {
            List<string> resultCol = new List<string>();
            path = path.Replace("/", "\\");

            if (Directory.Exists(path))
            {
                try
                {
                    var result = Directory.GetDirectories(path);
                    resultCol = result.ToList();

                    if (onlyNames)
                    {
                        resultCol.Clear();
                        foreach (string dir in result.ToList())
                        {
                            string[] dirCol = dir.Split("\\");
                            resultCol.Add(dirCol.Last());
                        }
                    }

                    _observer.SetLog(
                        new WoLog()
                        {
                            CodeLog = "000",
                            Title = $@"Se recupero la información exitosamente.",
                            Details = $@"Se recupero la información del directorio: {path}",
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
                                $@"Se produjo una excepción al intentar leer el directorio en la ruta ""{path}"".",
                            UserMessage =
                                $@"Se produjo una excepción al intentar leer el directorio en la ruta ""{path}"".",
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
                WoDirectory.CreateDirectory(path);
                XtraMessageBox.Show(
                    $@"El directorio en el path {path}. Aun no existia y acaba de ser creado",
                    $@"Directorio no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            return resultCol;
        }

        public List<(string path, string name)> ReadDirectoryDirectoriesPathName(
            string path,
            bool create
        )
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    if (create)
                    {
                        WoDirectory.CreateDirectory(path);
                        XtraMessageBox.Show(
                            text: $@"El directorio en el path {path}. Aun no existía y acaba de ser creado",
                            caption: $@"Alert",
                            buttons: MessageBoxButtons.OK,
                            icon: MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        throw new Exception("El directorio que esta intentando leer no existe.");
                    }
                }

                if (Directory.Exists(path))
                {
                    List<(string path, string name)> resultCol =
                        new List<(string path, string name)>();

                    path = path.Replace("/", "\\");

                    var result = Directory.GetDirectories(path);

                    foreach (string filePath in result.ToList())
                    {
                        string[] filePathCol = filePath.Split('\\');
                        resultCol.Add((path: filePath, name: filePathCol.Last()));
                    }

                    return resultCol;
                }
                else
                {
                    throw new Exception("El directorio que esta intentando leer no existe.");
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                XtraMessageBox.Show(
                    text: $@"Error al leer el directorio en el path {path}. {ex.Message}",
                    caption: $@"Error",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
            }
            return new List<(string path, string name)>();
        }
    }
}
