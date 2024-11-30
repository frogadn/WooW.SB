using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WooW.SB.Config;

namespace WooW.SB.Reportes.Helpers
{
    public static class WoGetReportFilesHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        static Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        /// <summary>
        /// Función para conbertir una lista de tipo <T> en un Datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool GetReportFiles(string woReport)
        {
            bool Contains = false;
            try
            {
                List<string> ReportsCreated = new List<string>();
                string[] archivos = Directory.GetFiles(_project.DirVistasReports, "*.xml");
                // Obtén solo los nombres de archivo
                string[] nombresArchivos = archivos
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
                foreach (var files in nombresArchivos)
                {
                    // Dividir la cadena por el punto
                    string[] partes = files.Split('.');
                    ReportsCreated.Add(partes[0]);
                }

                dynamic views = ReportsCreated.Where(x => x == (woReport)).ToList();
                foreach (string file in views)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //put a breakpoint here and check datatable
            return Contains;
        }
    }
}
