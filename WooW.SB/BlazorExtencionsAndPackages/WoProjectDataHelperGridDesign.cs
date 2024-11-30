using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorExtencionsAndPackages
{
    public partial class WoProjectDataHelper
    {
        #region Recuperación de la lista de los diseños de la grid

        /// <summary>
        /// Recuperación de los diseños de la grid del modelo principal y extensiones
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<WoGridProperties> GetGridDesigns(string modelName)
        {
            try
            {
                _readedProyects.Clear();
                _gridDesingsCol.Clear();

                SearchGridDesings(_project, modelName);

                return _gridDesingsCol;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar los diseños de la grid {modelName}. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de la lista de los diseños de la grid

        #region Recuperación del diseño full de la grid

        /// <summary>
        /// Recupera el diseño completo de la grid
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public WoGridProperties GetGridFullDesign(string modelName, bool blazorIntegral = false)
        {
            try
            {
                _readedProyects.Clear();
                _gridDesingsCol.Clear();

                SearchGridDesings(_project, modelName, blazorIntegral);

                WoGridProperties mainDesign = _gridDesingsCol.FirstOrDefault(grid =>
                    !grid.IsExtension
                );

                if (mainDesign != null)
                {
                    foreach (WoGridProperties gridDesing in _gridDesingsCol)
                    {
                        if (gridDesing.IsExtension)
                        {
                            mainDesign.WoColumnPropertiesCol.AddRange(
                                gridDesing.WoColumnPropertiesCol
                            );
                        }
                    }
                }
                else
                {
                    throw new Exception(
                        $@"No se pudo encontrar un diseño principal de grid para el modelo {modelName}"
                    );
                }

                return mainDesign;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar el diseño full de la grid {modelName}. {ex.Message}"
                );
            }
        }

        #endregion Recuperación del diseño full de la grid


        #region Búsqueda de los diseños de la grid

        /// <summary>
        /// Lista de los diseños recuperados
        /// </summary>
        private List<WoGridProperties> _gridDesingsCol = new List<WoGridProperties>();

        /// <summary>
        /// Búsqueda de los diseños recursiva mente entre los paquetes
        /// </summary>
        private void SearchGridDesings(
            Proyecto project,
            string modelName,
            bool blazorIntegral = false
        )
        {
            try
            {
                string projectName = GetWWSBName(project);
                if (!_readedProyects.Contains(projectName))
                {
                    _readedProyects.Add(projectName);

                    Modelo findModel = project.ModeloCol.Modelos.FirstOrDefault(model =>
                        model.Id == modelName
                    );

                    if (findModel != null)
                    {
                        string pathGridDesign =
                            $"{project.DirLayOuts}\\ListDesign\\{modelName}GridList.json";

                        WoGridProperties gridDesign = null;

                        if (File.Exists(pathGridDesign))
                        {
                            string jsonRaw = WoDirectory.ReadFile(pathGridDesign);
                            gridDesign = JsonConvert.DeserializeObject<WoGridProperties>(jsonRaw);
                        }
                        else
                        {
                            WoGridDesignerRawHelper woGridDesignerRawHelper =
                                new WoGridDesignerRawHelper();
                            gridDesign = woGridDesignerRawHelper.GetRawGridOfProject(
                                project,
                                modelName,
                                blazorIntegral
                            );
                        }

                        if (gridDesign != null)
                        {
                            _gridDesingsCol.Add(gridDesign);
                        }
                    }

                    foreach (Paquete pack in project.Paquetes)
                    {
                        string[] pathBaseDirCol = project.Dir.Split("\\");
                        string pathPacks = string.Empty;
                        for (int i = 0; i < pathBaseDirCol.Count() - 1; i++)
                        {
                            pathPacks += $"{pathBaseDirCol[i]}\\";
                        }

                        string path = $"{pathPacks}{pack.Archivo}";
                        Proyecto subProyect = new Proyecto();
                        subProyect.Load(path);

                        SearchGridDesings(subProyect, modelName, blazorIntegral);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recorrer los proyectos buscando los diseños de la grid. {ex.Message}"
                );
            }
        }

        #endregion Búsqueda de los diseños de la grid
    }
}
