using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorSettings
{
    #region Enums

    /// <summary>
    /// Define cual es el tipo de método sobre el que se va a trabajar para determinar
    /// el funcionamiento del syntax.
    /// </summary>
    public enum eCodeType
    {
        Method,
        CustomMethod,
        Constructor
    }

    #endregion Enums

    public class WoSyntaxEditorOptions
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto permite enviar logs y alertas.
        /// </summary>
        private static WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private static Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        /// <summary>
        /// Retorna las dlls necesarias tanto para la compilación como para el editor de código.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetBaseDlls()
        {
            List<string> baseDlls = new List<string>();

            string pathBase = "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App";

            List<string> dotnetVer = WoDirectory.ReadDirectoryDirectories(path: pathBase);
            if (dotnetVer == null || dotnetVer.Count == 0)
            {
                throw new Exception("Instale alguna vercion de .net 8 en adelante para continuar.");
            }

            string directory = string.Empty;
            foreach (string dllDirectory in dotnetVer)
            {
                string[] dllDirectoryCol = dllDirectory.Split("\\");
                string dllDirectoryLast = dllDirectoryCol.Last();
                string ver = dllDirectoryLast.Split(".").First();
                if (ver == "8")
                {
                    directory = dllDirectory;
                    break;
                }
            }

            List<string> dllsNetCore = new List<string>()
            {
                "System.Runtime.dll",
                "mscorlib.dll",
                "System.Linq.dll",
                "System.Linq.Expressions.dll",
                "System.Private.CoreLib.dll",
                "System.Reflection.dll",
                "System.Collections.dll",
                "System.Text.RegularExpressions.dll",
                "netstandard.dll",
                "System.Threading.Tasks.dll"
            };

            foreach (string dllNetCore in dllsNetCore)
            {
                if (File.Exists($@"{directory}\{dllNetCore}"))
                {
                    baseDlls.Add($@"{directory}\{dllNetCore}");
                }
                else
                {
                    ///Todo: send exception falta una dll para el syntax editor
                    MessageBox.Show($@"Falta la dll de {dllNetCore}");

                    _dllNotFind.Details = $@"No se encuentra la dll: {directory}\{dllNetCore}";
                    _observer.SetLog(_dllNotFind);
                }
            }

            List<string> dlls = WoDirectory.ReadDirectoryFiles(
                path: $@"{_project.DirProyectData}\Assembly"
            );

            List<string> filterDlls = new List<string>();
            foreach (var dll in dlls)
            {
                string[] dllCol = dll.Split('.');
                if (dllCol.Last() == "dll")
                {
                    filterDlls.Add(dll);
                }
            }
            baseDlls.AddRange(filterDlls);

            return baseDlls;
        }

        #region Logs

        private static WoLog _dllNotFind = new WoLog()
        {
            CodeLog = "000",
            Title = "No se encuentra la dll.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoSyntaxEditorOptions",
                MethodOrContext = "GetBaseDlls"
            }
        };

        #endregion Logs
    }
}
