using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Helpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Menus.MenuModels;

namespace WooW.SB.Menus.MenusComponents
{
    public class WoGetModelsMenu
    {
        #region Variables

        private ModelHelper _modelHelper = new ModelHelper();

        #endregion Variables

        #region Instancias singleton

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Metodo principal
        public List<Modelo> GetModelComplete(string process)
        {
            List<Modelo> modelsIn = new List<Modelo>();
            try
            {
                List<Modelo> modelsCol = new List<Modelo>();

                List<string> modelsNamesCol = WoDirectory.ReadDirectoryFiles(
                    path: _project.DirFormDesign,
                    onlyNames: true
                );
                //Agrega los modelos que exista creado su formulario
                foreach (string modelName in modelsNamesCol)
                {
                    Modelo model = _modelHelper.SearchModel(modelName);
                    if (model != null)
                    {
                        modelsCol.Add(model);
                    }
                }
                List<string> modelsNamesColReports = WoDirectory.ReadDirectoryFiles(
                    path: _project.DirReportesTemp,
                    onlyNames: true
                );
                //Agrega los modelos que tengan creado su reporte
                foreach (string modelName in modelsNamesColReports)
                {
                    int indicePunto = modelName.IndexOf('.'); // Busca la posición del primer punto

                    if (indicePunto != -1)
                    {
                        string modelNameReport = modelName.Substring(0, indicePunto);
                        Modelo model = _modelHelper.SearchModel(modelNameReport);
                        if (modelsCol != null)
                        {
                            if (model != null)
                            {
                                if (!modelsCol.Contains(model))
                                {
                                    modelsCol.Add(model);
                                }
                            }
                        }
                    }
                }
                IEnumerable<Modelo> modelsColAux = modelsCol.Where(model =>
                    (
                        model.ProcesoId == process
                        && model.TipoModelo != WoTypeModel.CatalogSlave
                        && model.TipoModelo != WoTypeModel.TransactionSlave
                        && model.TipoModelo != WoTypeModel.Request
                    )
                    || (
                        model.ProcesoId == process
                        && model.TipoModelo != WoTypeModel.CatalogSlave
                        && model.TipoModelo != WoTypeModel.TransactionSlave
                        && model.SubTipoModelo == WoSubTypeModel.Report
                    )
                );

                List<Modelo> modelsInProcess = new List<Modelo>();
                foreach (var x in modelsCol)
                {
                    if (x.ProcesoId == process)
                    {
                        modelsInProcess.Add(x);
                    }
                }

                foreach (var model in modelsInProcess)
                {
                    if (
                        (
                            model.TipoModelo != WoTypeModel.CatalogSlave
                            && model.TipoModelo != WoTypeModel.TransactionSlave
                            && model.TipoModelo != WoTypeModel.Request
                            && model.TipoModelo != WoTypeModel.Configuration
                            && model.TipoModelo != WoTypeModel.Control
                            && model.TipoModelo != WoTypeModel.Kardex
                            && model.TipoModelo != WoTypeModel.View
                        )
                        || (
                            model.TipoModelo != WoTypeModel.CatalogSlave
                            && model.TipoModelo != WoTypeModel.TransactionSlave
                            && model.TipoModelo != WoTypeModel.Request
                            && model.TipoModelo != WoTypeModel.Configuration
                            && model.TipoModelo != WoTypeModel.Control
                            && model.TipoModelo != WoTypeModel.Kardex
                            && model.TipoModelo != WoTypeModel.View
                            && model.SubTipoModelo == WoSubTypeModel.Report
                        )
                    )
                    {
                        modelsIn.Add(model);
                    }
                }
                //Agrega las listas de los modelos ya verificados
                List<Modelo> modelsList = new List<Modelo>();
                modelsList = GetList(process);
                foreach (var model in modelsList)
                {
                    if (!modelsIn.Contains(model))
                    {
                        modelsIn.Add(model);
                    }
                }
                return modelsIn;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return modelsIn;
            }
        }

        /// <summary>
        /// Función para obtener los modelos de tipo lista
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public List<Modelo> GetList(string process, WoMenuProperties _woMenu = null)
        {
            if (_woMenu == null)
            {
                _woMenu = new WoMenuProperties();
            }
            List<Modelo> modelsCol = new List<Modelo>();

            List<string> modelsNamesColList = WoDirectory.ReadDirectoryFiles(
                path: $@"{_project.DirLayOuts}\ListDesign",
                onlyNames: true
            );
            List<string> ListCol = new List<string>();
            foreach (string model in modelsNamesColList)
            {
                var aux = model.Split('.')[0].Replace("GridList", "");
                if (!ListCol.Contains(aux))
                {
                    ListCol.Add(model.Split('.')[0]);
                }
            }

            foreach (string modelName in ListCol)
            {
                try
                {
                    var aux = modelName.Replace("GridList", "");
                    Modelo modelProcess = _modelHelper.SearchModel(aux);
                    if (modelProcess != null)
                    {
                        if (modelProcess.SubTipoModelo != WoSubTypeModel.Report)
                            if (modelProcess != null)
                            {
                                //WoDesignerLabels woDesignerLabels = new WoDesignerLabels();
                                //woDesignerLabels.AddLabel(
                                //    @$"{modelProcess.Id}List",
                                //    @$"{modelProcess.Id}List"
                                //);
                                Modelo model = new Modelo();
                                model.Id = (@$"{modelProcess.Id}GridList");
                                model.EtiquetaId = (@$"{modelProcess.EtiquetaId}");
                                model.ProcesoId = modelProcess.ProcesoId;

                                if (!SearchNode(_woMenu, model.Id))
                                    modelsCol.Add(model);
                            }
                    }
                }
                catch (System.Exception ex)
                {
                    var ruteModelInexists = $@"{_project.DirLayOuts}\ListDesign\{modelName}.json";
                    if (File.Exists(ruteModelInexists))
                    {
                        File.Delete(ruteModelInexists);
                        Console.WriteLine("El archivo se ha borrado exitosamente.");
                    }
                }
            }

            IEnumerable<Modelo> modelsColAuxReports = modelsCol
                .Where(model => model.ProcesoId == process)
                .ToList();

            return modelsColAuxReports.ToList();
        }
        #endregion Metodo principal

        #region Busqueda
        /// <summary>
        /// Función para buscar buscar la existencia de un modelo en el arbol actual
        /// </summary>
        /// <param name="menuProperties"></param>
        /// <param name="idSeach"></param>
        /// <returns></returns>
        private bool SearchNode(WoMenuProperties menuProperties, string idSeach)
        {
            bool result = false;

            if (menuProperties.Id == idSeach)
            {
                result = true;
            }
            else
            {
                if (menuProperties.ContentCol.Count() > 0)
                {
                    foreach (WoMenuProperties subNode in menuProperties.ContentCol)
                    {
                        result = SearchNode(subNode, idSeach);
                        if (result)
                            break;
                    }
                }
            }

            return result;
        }

        #endregion Busqueda
    }
}
