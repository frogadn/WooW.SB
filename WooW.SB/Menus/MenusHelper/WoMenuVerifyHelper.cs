using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using WooW.SB.Config;
using WooW.SB.Helpers;
using WooW.SB.Menus.MenuModels;

namespace WooW.SB.Menus.MenusHelper
{
    public class WoMenuVerifyHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        private ModelHelper _modelHelper = new ModelHelper();

        WoMenuProperties _menu = new WoMenuProperties();

        /// <summary>
        /// Función para conbertir una lista de tipo <T> en un Datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public WoMenuProperties MenuVerificated(WoMenuProperties woMenu)
        {
            try
            {
                _menu = woMenu;
                CreateTreeList(_menu);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return _menu;
        }

        /// <summary>
        /// Función recursiva usada para verificar que existan los modelos de
        /// el arbol
        /// </summary>
        /// <param name="woMenu"></param>
        /// <param name="parent"></param>
        private void CreateTreeList(WoMenuProperties woMenu)
        {
            //Verificamos en los modelos
            Modelo model = _modelHelper.SearchModel(woMenu.Id);
            if (model == null)
            {
                if (woMenu.Id.Contains("GridList") || woMenu.Id.Contains("Report"))
                {
                    //Verificamos las listas
                    if (woMenu.Id.Contains("GridList", StringComparison.OrdinalIgnoreCase))
                    {
                        Modelo modelList = _modelHelper.SearchModel(
                            woMenu.Id.Replace("GridList", "", StringComparison.OrdinalIgnoreCase)
                        );
                        //verificamos si existe el modelo
                        if (modelList != null)
                        {
                            if (!File.Exists(_project.DirListDesign + "\\" + woMenu.Id + ".json"))
                            {
                                woMenu.NotFound = true;
                            }
                            else
                            {
                                woMenu.NotFound = false;
                            }
                        }
                        else
                        {
                            woMenu.NotFound = true;
                        }
                    }
                    if (woMenu.Id.Contains("ReportODATA", StringComparison.OrdinalIgnoreCase))
                    {
                        Modelo modelList = _modelHelper.SearchModel(
                            woMenu.Id.Replace("ReportODATA", "", StringComparison.OrdinalIgnoreCase)
                        );
                        //verificamos si existe el modelo
                        if (modelList != null)
                        {
                            //verificamos si existe el archivo en la carpeta de reportes
                            List<string> listReport = new List<string>();
                            string[] archivos = Directory.GetFiles(
                                _project.DirVistasReports,
                                "*.xml"
                            );
                            foreach (string archivo in archivos)
                            {
                                listReport.Add((Path.GetFileName(archivo).Split(".")[0]));
                            }

                            var aux = woMenu.Id.Replace(
                                "ReportODATA",
                                "",
                                StringComparison.OrdinalIgnoreCase
                            );
                            if (!listReport.Contains(aux))
                            {
                                woMenu.NotFound = true;
                            }
                            else
                            {
                                woMenu.NotFound = false;
                            }
                        }
                        else
                        {
                            woMenu.NotFound = true;
                        }
                    }
                }
                else
                {
                    woMenu.NotFound = true;
                }
            }
            else
            {
                woMenu.NotFound = false;
            }

            if (woMenu.ContentCol != null)
            {
                if (woMenu.ContentCol.Count > 0)
                {
                    foreach (var SubNode in woMenu.ContentCol)
                    {
                        CreateTreeList(SubNode);
                    }
                }
            }
        }
    }
}
