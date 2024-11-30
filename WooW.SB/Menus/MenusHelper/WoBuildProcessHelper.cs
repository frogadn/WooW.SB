using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Helpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Menus.MenuModels;
using WooW.SB.Menus.MenusComponents;

namespace WooW.SB.Menus.MenusHelper
{
    public class WoBuildProcessHelper
    {
        #region Instancias singleton

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Varible para almacenar el menú cargado en el treelist actual de los menus
        /// </summary>
        private WoMenuProperties _woMenu = new WoMenuProperties();

        /// <summary>
        /// Lista para almacenar los procesos seleccionados por el usuario
        /// </summary>
        private List<string> _processCol = new List<string>();

        /// <summary>
        /// Helper model para recupera el modelo a través del nombre
        /// </summary>
        private ModelHelper _modelHelper = new ModelHelper();

        private bool _allProcess = false;

        #endregion Atributos

        #region Constructor
        public WoBuildProcessHelper(WoMenuProperties dataSetMenu, bool allProcess = true)
        {
            _allProcess = allProcess;
            _woMenu = dataSetMenu;

            if (allProcess)
            {
                InitalizeProcessList();
            }
            else
            {
                _processCol.Clear();
                _processCol = dataSetMenu.ProcessList;
            }
        }
        #endregion Constructor

        #region Métodos públicos


        public List<List<Modelo>> BuildTree()
        {
            List<List<Modelo>> modelsColWithReports = GetMenuModels();
            //List<List<Modelo>> modelsColWithReports = GetModels(modelsCol);

            return modelsColWithReports;
        }

        #endregion  Métodos públicos

        #region Recuperación de procesos

        /// <summary>
        /// Función para obtener toda la colección de procesos
        /// </summary>
        public void InitalizeProcessList()
        {
            _processCol.Clear();
            foreach (var model in _project.ModeloCol.Modelos)
            {
                if (!_processCol.Contains(model.ProcesoId))
                {
                    _processCol.Add(model.ProcesoId);
                }
            }
        }

        #endregion Recuperación de procesos

        #region Models

        private List<List<Modelo>> GetMenuModels()
        {
            List<List<Modelo>> processModelsCol = new List<List<Modelo>>();
            if (_processCol == null)
            {
                return processModelsCol;
            }
            foreach (string processName in _processCol)
            {
                List<Modelo> procesModels = GetModelList(processName);
                if (procesModels.Count > 0)
                {
                    if (!processModelsCol.Contains(procesModels))
                    {
                        processModelsCol.Add(procesModels);
                    }
                }
                List<Modelo> procesModelsReports = GetModelListReport(processName);
                if (procesModelsReports.Count > 0)
                {
                    if (!processModelsCol.Contains(procesModelsReports))
                    {
                        processModelsCol.Add(procesModelsReports);
                    }
                }
                WoGetModelsMenu woGetModelsMenu = new WoGetModelsMenu();
                List<Modelo> procesList = woGetModelsMenu.GetList(processName, _woMenu);
                if (procesList.Count > 0)
                {
                    if (!processModelsCol.Contains(procesList))
                    {
                        processModelsCol.Add(procesList);
                    }
                }
            }
            List<List<Modelo>> processModelsColSinDup = new List<List<Modelo>>();
            processModelsColSinDup = processModelsCol.Distinct().ToList();
            return processModelsColSinDup;
        }

        private List<Modelo> GetModelList(string process)
        {
            List<Modelo> modelsCol = new List<Modelo>();

            List<string> modelsNamesCol = WoDirectory.ReadDirectoryFiles(
                path: _project.DirFormDesign,
                onlyNames: true
            );

            foreach (string modelName in modelsNamesCol)
            {
                try
                {
                    Modelo model = _modelHelper.SearchModel(modelName);
                    if (model == null)
                    {
                        return null;
                    }
                    if (!SearchNode(_woMenu, model.Id))
                    {
                        if (model.SubTipoModelo != WoSubTypeModel.Report)
                        {
                            modelsCol.Add(model);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    var ruteModelInexists = _project.DirFormDesign + "\\" + modelName + ".json";
                    if (File.Exists(ruteModelInexists))
                    {
                        File.Delete(ruteModelInexists);
                        Console.WriteLine("El archivo se ha borrado exitosamente.");
                    }
                }
            }

            IEnumerable<Modelo> modelsColAux = modelsCol
                .Where(model =>
                    model.ProcesoId == process
                    && model.TipoModelo != WoTypeModel.CatalogSlave
                    && model.TipoModelo != WoTypeModel.TransactionSlave
                    && model.TipoModelo != WoTypeModel.Request
                    && model.TipoModelo != WoTypeModel.Kardex
                    && model.TipoModelo != WoTypeModel.Configuration
                    && model.TipoModelo != WoTypeModel.Control
                    && model.TipoModelo != WoTypeModel.View
                )
                .ToList();

            return modelsColAux.ToList();
        }

        private List<Modelo> GetModelListReport(string process)
        {
            List<Modelo> modelsCol = new List<Modelo>();

            var modelsNamesColReports = Directory.GetFiles(_project.DirVistasReports, "*.xml");
            List<string> reportModelsColName = new List<string>();
            foreach (string nombreArchivo in modelsNamesColReports)
            {
                reportModelsColName.Add(Path.GetFileName(nombreArchivo));
            }
            List<string> reportModelsCol = new List<string>();
            foreach (string model in reportModelsColName)
            {
                if (model.Contains("ODATA"))
                {
                    if (!reportModelsCol.Contains(model.Split('.')[0] + "ODATA"))
                    {
                        reportModelsCol.Add(model.Split('.')[0] + "ODATA");
                    }
                }
                else
                {
                    if (!reportModelsCol.Contains(model.Split('.')[0]))
                    {
                        reportModelsCol.Add(model.Split('.')[0]);
                    }
                }
            }

            foreach (string modelName in reportModelsCol)
            {
                try
                {
                    Modelo model = _modelHelper.SearchModel(modelName);
                    if (model == null)
                    {
                        model = new Modelo();
                        //Se busca el modelo para obtener la descripción y el proceso y agregarse al nuevo objeto de tipo modelo
                        /// de un reporte ODATA
                        Modelo modelaux = _modelHelper.SearchModel(modelName.Replace("ODATA", ""));
                        if (modelaux != null)
                        {
                            model.Id = (@$"{modelaux.Id}ReportOdata");
                            model.EtiquetaId = (@$"{modelaux.EtiquetaId}");
                            model.ProcesoId = (@$"{modelaux.ProcesoId}");
                        }
                    }
                    if (model != null)
                    {
                        if (!SearchNode(_woMenu, model.Id))
                            //if (!modelsCol.Contains(model))
                            modelsCol.Add(model);
                    }
                }
                catch (System.Exception ex)
                {
                    var ruteModelInexists = _project.DirFormDesign + "\\" + modelName + ".json";
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
        #endregion Models

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

        #region Recuperación de reportes

        /// <summary>
        /// Función axuliar para integrar los modelos de reportes
        /// en de los procesos
        /// </summary>
        /// <param name="processModelsCol"></param>
        /// <returns></returns>
        private List<List<Modelo>> GetModels(List<List<Modelo>> processModelsCol)
        {
            List<List<Modelo>> modelsWithReportsCol = new List<List<Modelo>>();

            List<string> designedModels = new List<string>();
            designedModels = WoDirectory.ReadDirectoryFiles(
                path: $@"{_project.DirVistasReports}",
                onlyNames: true
            );

            List<string> reportModelsCol = new List<string>();
            foreach (string model in designedModels)
            {
                if (!reportModelsCol.Contains(model.Split('.')[0]))
                {
                    reportModelsCol.Add(model.Split('.')[0]);
                }
            }

            ///Variable auxiliar, carga los modelos a los que pertenecen los reportes
            ///estos modelos no se han diseñado en el diseñador de formularios, es por eso que no los toma
            ///en cuenta, con esta variable se almacena el valor del modelo de esos reportes
            List<Modelo> ListModelsInexistis = new List<Modelo>();
            foreach (List<Modelo> models in processModelsCol)
            {
                if (models.Count > 0)
                {
                    foreach (string reportModel in reportModelsCol)
                    {
                        try
                        {
                            Modelo model = _modelHelper.SearchModel(reportModel);
                            if (model != null)
                            {
                                if (model.ProcesoId == models[0].ProcesoId)
                                {
                                    models.Add(model);
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            var ruteModelInexists =
                                _project.DirFormDesign + "\\" + reportModel + ".json";
                            if (File.Exists(ruteModelInexists))
                            {
                                File.Delete(ruteModelInexists);
                                Console.WriteLine("El archivo se ha borrado exitosamente.");
                            }
                        }
                    }
                }

                modelsWithReportsCol.Add(models);
            }

            return modelsWithReportsCol;
        }

        #endregion Recuperación de reportes
    }
}
