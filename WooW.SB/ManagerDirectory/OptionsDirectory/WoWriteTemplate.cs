using System;
using System.IO;
using System.Linq;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.ManagerDirectory.OptionsDirectory
{
    public class WoWriteTemplate
    {
        #region Instancia singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancia singleton

        public void WriteTemplate(string pathTemplate, string data, bool createAllPath = true)
        {
            pathTemplate = pathTemplate.Replace("/", "\\");

            if (File.Exists(pathTemplate))
            {
                File.Delete(pathTemplate);
            }
            if (createAllPath)
            {
                string[] folders = pathTemplate.Split('\\');
                string pathFolder = folders[0];

                for (int i = 1; i < folders.Count() - 1; i++)
                {
                    pathFolder += "\\" + folders[i];
                    WoDirectory.CreateDirectory(pathFolder);
                }

                WriteTemplateInternal(pathTemplate, data);
            }
            else
            {
                WriteTemplateInternal(pathTemplate, data);
            }
        }

        private void WriteTemplateInternal(string pathTemplate, string data)
        {
            try
            {
                File.WriteAllText(pathTemplate, data);
                _templateReady.Details =
                    $@"La plantilla se escribió correctamente en la ruta {pathTemplate}.";
                _observer.SetLog(_templateReady);
            }
            catch (Exception ex)
            {
                _cantWriteTemplate.ExceptionMessage = ex.Message;
                _cantWriteTemplate.Details =
                    $@"Ocurrió un error al intentar escribir la plantilla en la ruta: {pathTemplate}";
                throw new WoObserverException(_cantWriteTemplate);
            }
        }

        #region Logs

        private WoLog _cantWriteTemplate = new WoLog()
        {
            CodeLog = "000",
            Title = "No es posible escribir la plantilla.",
            UserMessage = $@"Ocurrió un error al intentar escribir la plantilla.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoWriteTemplate",
                MethodOrContext = "WriteTemplate"
            }
        };

        private WoLog _templateReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Plantilla escrita correctamente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoWriteTemplate",
                MethodOrContext = "WriteTemplate"
            }
        };

        #endregion Logs
    }
}
