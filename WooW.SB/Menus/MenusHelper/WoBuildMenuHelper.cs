using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using ServiceStack;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Menus.MenuModels;
using WooW.SB.Menus.MenusComponents;

namespace WooW.SB.Menus.MenusHelper
{
    public class WoBuildMenuHelper
    {
        #region Instancias singleton

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        private string _nombre = string.Empty;

        private List<string> _process = new List<string>();

        public List<Rol> _roles { get; set; } = new List<Rol>();

        private ModelHelper _modelHelper = new ModelHelper();

        #endregion Atributos

        #region Constructor

        public WoBuildMenuHelper(string Nombre, List<string> process, List<Rol> roles)
        {
            _nombre = Nombre;
            _process = process;
            _roles = roles;
        }

        #endregion Constructor

        #region Metodo principal

        public WoMenuProperties GetRawMenu()
        {
            WoMenuProperties menu = new WoMenuProperties();
            //List<List<Modelo>> models = GetMenuModels();
            List<List<Modelo>> modelsWithReports = GetMenuModelsComplete();

            menu = CreateMenu(modelsWithReports);
            menu.ExpandedNode = true;

            string json = JsonConvert.SerializeObject(menu);

            WoDirectory.WriteFile(path: $@"{_project.DirMenus}\{menu.Id}.json", json);
            WoDirectory.WriteFile(path: $@"{_project.DirMenusTemp}\{menu.Id}.json", json);

            return menu;
        }

        #endregion Metodo principal

        #region Models

        private List<List<Modelo>> GetMenuModels()
        {
            List<List<Modelo>> procesModels = new List<List<Modelo>>();

            foreach (string processName in _process)
            {
                List<Modelo> listModel = GetModelList(processName);
                if (listModel.Count > 0)
                {
                    procesModels.Add(listModel);
                }
            }

            return procesModels;
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
                Modelo model = _modelHelper.SearchModel(modelName);
                modelsCol.Add(model);
            }

            IEnumerable<Modelo> modelsColAux = modelsCol.Where(model =>
                model.ProcesoId == process
                && model.TipoModelo != WoTypeModel.CatalogSlave
                && model.TipoModelo != WoTypeModel.TransactionSlave
                && model.TipoModelo != WoTypeModel.Request
            );

            return modelsColAux.ToList();
        }

        private List<List<Modelo>> GetMenuModelsComplete()
        {
            List<List<Modelo>> procesModelsReportsList = new List<List<Modelo>>();
            WoGetModelsMenu getModelsMenu = new WoGetModelsMenu();
            foreach (string processName in _process)
            {
                List<Modelo> listModel = getModelsMenu.GetModelComplete(processName);
                if (listModel.Count > 0)
                {
                    procesModelsReportsList.Add(listModel);
                }
            }

            return procesModelsReportsList;
        }

        #endregion Models

        #region Creacion del menu

        /// <summary>
        ///Función utilizada para crear un menú
        /// </summary>
        /// <returns></returns>
        private WoMenuProperties CreateMenu(List<List<Modelo>> procesModels)
        {
            WoMenuProperties menu = new WoMenuProperties()
            {
                Id = Etiqueta.ToId(_nombre),
                Label = _nombre,
                MaskText = "Root",
                ProcessList = _process,
                Roles = _roles,
                TypeContainer = eTypeContainer.Menu,
                TypeItem = eTypeItem.None,
                Reference = "/",
                Icon = "Sin Icono",
                Added = true
            };

            foreach (List<Modelo> models in procesModels)
            {
                if (models.Count > 0)
                {
                    WoMenuProperties subMenuProcess = new WoMenuProperties()
                    {
                        Id = models[0].ProcesoId,
                        TypeContainer = eTypeContainer.SubMenu,
                        TypeItem = eTypeItem.None,
                        Label = models[0].EtiquetaId,
                        //MaskText = EtiquetaCol.Get(models[0].EtiquetaId),
                        MaskText = models[0].EtiquetaId,
                        Order = 1,
                        Parent = menu.Id,
                        Reference = "/",
                        Icon = "Sin Icono",
                        Added = true
                    };

                    foreach (Modelo model in models)
                    {
                        var woMenuProperties = new WoMenuProperties()
                        {
                            Process = models[0]?.ProcesoId ?? default,
                            Id = model?.Id ?? default,
                            TypeItem = eTypeItem.MenuItem,
                            TypeContainer = eTypeContainer.None,
                            Label = model?.EtiquetaId ?? default,
                            //MaskText = EtiquetaCol.Get(model.EtiquetaId),
                            MaskText = model?.EtiquetaId ?? default,
                            Parent = subMenuProcess.Id,
                            Added = true,
                            Icon = "Sin Icono",
                            Reference = GenerateReference(model)
                        };

                        subMenuProcess.ContentCol.Add(woMenuProperties);
                    }

                    menu.ContentCol.Add(subMenuProcess);
                }
            }

            return menu;
        }

        // Definir una función para generar la referencia
        private string GenerateReference(Modelo model)
        {
            if (model.Id.ToString().EndsWith("GridList", StringComparison.OrdinalIgnoreCase))
            {
                return $"{model.ProcesoId}/List/{model.Id.Replace("GridList", "", StringComparison.OrdinalIgnoreCase)}";
            }
            else if (model.SubTipoModelo == WoSubTypeModel.Report)
            {
                return $"{model.ProcesoId}/Report/{model.Id}";
            }
            else if (model.Id.ToString().EndsWith("Odata", StringComparison.OrdinalIgnoreCase))
            {
                return $"{model.ProcesoId}/Report/{model.Id.Replace("ReportOdata", "", StringComparison.OrdinalIgnoreCase)}";
            }
            else
            {
                return $"{model.ProcesoId}/{model.TipoModelo}/{model.Id}";
            }
        }
        #endregion Creacion del menu

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
            ///en cuenta, con esta variable se almacena el valor del modelo de esos reportes en este momento
            ///solo agrega los que coincidan con el proceso seleccionado, la lista se queda hasta aqui solamente
            List<Modelo> ListModelsInexistis = new List<Modelo>();
            foreach (List<Modelo> models in processModelsCol)
            {
                if (models.Count > 0)
                {
                    foreach (string reportModel in reportModelsCol)
                    {
                        Modelo model = _modelHelper.SearchModel(reportModel);
                        if (model != null)
                        {
                            if (model.ProcesoId == models[0].ProcesoId)
                            {
                                models.Add(model);
                            }
                            else if (!ListModelsInexistis.Contains(model))
                            {
                                ListModelsInexistis.Add(model);
                            }
                        }
                    }
                }

                modelsWithReportsCol.Add(models);
            }
            //if (ListModelsInexistis.Count > 0)
            //{
            //    modelsWithReportsCol.Add(ListModelsInexistis);
            //}

            return modelsWithReportsCol;
        }

        #endregion Recuperación de reportes
    }
}
