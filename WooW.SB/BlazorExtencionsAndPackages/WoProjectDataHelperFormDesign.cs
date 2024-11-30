using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorExtencionsAndPackages
{
    public partial class WoProjectDataHelper
    {
        #region Recuperación de los diseños

        /// <summary>
        /// Recupera la lista de los diseños en función del nombre del modelo
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public List<WoContainer> GetDesings(string modelName)
        {
            try
            {
                _readedProyects.Clear();
                string projectDir = GetWWSBPath(_project);
                BuildSubDesigns(projectDir, _project, modelName);
                return _desingsCol;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la recuperación de los diseños. {ex.Message}");
            }
        }

        /// <summary>
        /// Recupera una sola instancia de wo container con la combinación de todos los diseños
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public WoContainer GetFullDesing(string modelName)
        {
            try
            {
                _desingsCol.Clear();

                string projectDir = GetWWSBPath(_project);

                _readedProyects.Clear();
                BuildSubDesigns(projectDir, _project, modelName);

                WoContainer mainDesing = _desingsCol.FirstOrDefault(desing => !desing.IsExtension);
                int groupAdded = mainDesing.Row;
                int row = mainDesing.Row;

                if (mainDesing != null)
                {
                    foreach (WoContainer desing in _desingsCol)
                    {
                        if (desing.IsExtension)
                        {
                            groupAdded++;
                            foreach (WoContainer groupExtencion in desing.ContainersCol)
                            {
                                row += groupExtencion.RowSpan + 1;
                                groupExtencion.RowIndex = groupAdded;
                                mainDesing.ContainersCol.Add(groupExtencion);
                                groupAdded += groupExtencion.RowSpan;
                            }
                        }
                    }

                    mainDesing.Row = row;

                    return mainDesing;
                }
                else
                {
                    throw new Exception($@"No se encontró el diseño principal");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la recuperación de los diseños. {ex.Message}");
            }
        }

        #endregion Recuperación de los diseños

        #region Recuperación de los diseños con los proyectos

        /// <summary>
        /// Recuperamos una lista de los diseños y los proyectos
        /// </summary>
        /// <returns></returns>
        public List<(WoContainer desing, string project)> GetDesingProyect(string modelName)
        {
            try
            {
                string projectDir = GetWWSBPath(_project);
                _readedProyects.Clear();
                BuildSubDesigns(projectDir, _project, modelName);

                return _desingsProyects;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar la lista con los diseños y los proyectos. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de los diseños con los proyectos


        #region Búsqueda de los diseños

        /// <summary>
        /// Lista de los diseños de los formularios
        /// </summary>
        private List<WoContainer> _desingsCol = new List<WoContainer>();

        /// <summary>
        /// Lista con la relación entre diseños y proyectos
        /// </summary>
        private List<(WoContainer desing, string pathProject)> _desingsProyects =
            new List<(WoContainer desing, string pathProject)>();

        /// <summary>
        /// Recupera de forma recursiva la lista de los diseños
        /// </summary>
        private void BuildSubDesigns(string wwsbPath, Proyecto project, string modelName)
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
                        string pathDesignModel =
                            $"{project.DirLayOuts}\\FormDesign\\{modelName}.json";

                        WoContainer woContainer = null;

                        if (File.Exists(pathDesignModel))
                        {
                            string jsonRaw = WoDirectory.ReadFile(pathDesignModel);
                            woContainer = JsonConvert.DeserializeObject<WoContainer>(jsonRaw);
                        }
                        else
                        {
                            WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                                new WoDesignerRawSerializerHelper();
                            woContainer = woDesignerRawSerializerHelper.BuildRawWoContainer(
                                modelName,
                                project
                            );
                        }

                        if (woContainer != null)
                        {
                            _desingsCol.Add(woContainer);
                            _desingsProyects.Add((desing: woContainer, pathProject: wwsbPath));
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

                        BuildSubDesigns(path, subProyect, modelName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error en la construcción del diseño del modelo. {ex.Message}"
                );
            }
        }

        #endregion Búsqueda de los diseños
    }
}
