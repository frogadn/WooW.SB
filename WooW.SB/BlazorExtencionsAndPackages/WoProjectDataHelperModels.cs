using System;
using System.Collections.Generic;
using System.Linq;
using WooW.SB.Config;

namespace WooW.SB.BlazorExtencionsAndPackages
{
    public partial class WoProjectDataHelper
    {
        #region Valida si el modelo se encuentra en el proyecto

        /// <summary>
        /// Busca en el proyecto actual el modelo indicado por parámetro y
        /// en caso de encontrarlo retorna true y en vaso contrario false
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public bool ModelInProyect(string modelName)
        {
            try
            {
                Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == modelName
                );
                return (findModel != null);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al buscar el modelo {modelName} en el proyecto actual. {ex.Message}"
                );
            }
        }

        #endregion Valida si el modelo se encuentra en el proyecto


        #region Recuperación del modelo actual

        /// <summary>
        /// Recuperamos el modelo actual del proyecto.
        /// </summary>
        /// <returns></returns>
        public Modelo GetActualModel(string modelName)
        {
            try
            {
                Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == modelName
                );

                if (findModel == null)
                {
                    throw new Exception($"El modelo {modelName} no existe en el proyecto actual.");
                }

                return findModel;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar el modelo {modelName} en el proyecto actual. {ex.Message}"
                );
            }
        }

        #endregion Recuperación del modelo actual

        #region Recuperación de las columnas

        /// <summary>
        /// Recupera la lista de las columnas del modelo y sus extensiones
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public List<ModeloColumna> GetFullColumns(string modelName)
        {
            try
            {
                List<ModeloColumna> fullColumns = new List<ModeloColumna>();

                _readedProyects.Clear();
                _findModels.Clear();
                SearchModel(_project, modelName);

                foreach (Modelo model in _findModels)
                {
                    fullColumns.AddRange(model.Columnas);
                }

                return fullColumns;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar las columnas completas del modelo. {ex.Message}"
                );
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public ModeloColumna GetColumn(string modelName, string property)
        {
            try
            {
                ModeloColumna findColumn = null;

                Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == modelName
                );
                if (findModel != null)
                {
                    findColumn = findModel.Columnas.FirstOrDefault(column => column.Id == property);
                    if (findColumn == null)
                    {
                        List<Modelo> models = GetModelWithExtencions(modelName);
                        foreach (Modelo model in models)
                        {
                            findColumn = model.Columnas.FirstOrDefault(column =>
                                column.Id == property
                            );
                            if (findColumn != null)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    List<Modelo> models = GetModelWithExtencions(modelName);
                    foreach (Modelo model in models)
                    {
                        findColumn = model.Columnas.FirstOrDefault(column => column.Id == property);
                        if (findColumn != null)
                        {
                            break;
                        }
                    }
                }

                if (findColumn == null)
                {
                    throw new Exception($@"No se encontró {property}");
                }
                else
                {
                    return findColumn;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar la columna {property}, en el modelo {modelName}. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de las columnas

        #region Recuperación de los modelos

        /// <summary>
        /// Orquesta la recuperación de los modelos
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Modelo> GetModelWithExtencions(string modelName)
        {
            try
            {
                _readedProyects.Clear();
                _findModels.Clear();
                SearchModel(_project, modelName);

                return _findModels;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar los modelos del paquete. {ex.Message}");
            }
        }

        #endregion Recuperación de los modelos

        #region Recuperación del modelo principal

        /// <summary>
        /// Recuperamos el modelo principal desde los paquetes
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public Modelo GetMainModel(string modelName)
        {
            _readedProyects.Clear();
            _findModels.Clear();
            SearchModel(_project, modelName);

            Modelo _findModel = new Modelo();

            foreach (Modelo model in _findModels)
            {
                if (model.ProcesoId != string.Empty)
                {
                    _findModel = model;
                    break;
                }
            }

            return _findModel;
        }

        #endregion Recuperación del modelo principal

        #region Recuperación de las extensiones (Extension models)

        /// <summary>
        /// Recupera la lista de los modelos que son extensiones del principal
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public List<Modelo> GetExtensions(string modelName)
        {
            try
            {
                _readedProyects.Clear();
                _findModels.Clear();
                SearchModel(_project, modelName);

                Modelo findModel = _findModels.FirstOrDefault(model =>
                    model.ProcesoId != string.Empty
                );
                _findModels.Remove(findModel);

                return _findModels;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar las extensiones. {ex.Message}");
            }
        }

        #endregion Recuperación de las extensiones (Extension models)


        #region Búsqueda de los modelos

        /// <summary>
        /// Lista de los modelos y sus extensiones
        /// </summary>
        private List<Modelo> _findModels = new List<Modelo>();

        /// <summary>
        /// Búsqueda re cursiva de los modelos con el nombre indicado
        /// </summary>
        /// <param name="project"></param>
        /// <param name="modelName"></param>
        /// <exception cref="Exception"></exception>
        private void SearchModel(Proyecto project, string modelName)
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
                        _findModels.Add(findModel);
                    }

                    foreach (Paquete pack in project.Paquetes)
                    {
                        Proyecto subProyect = new Proyecto();
                        string[] pathBaseDirCol = project.Dir.Split("\\");
                        string pathPacks = string.Empty;
                        for (int i = 0; i < pathBaseDirCol.Count() - 1; i++)
                        {
                            pathPacks += $"{pathBaseDirCol[i]}\\";
                        }
                        subProyect.Load($"{pathPacks}{pack.Archivo}");

                        SearchModel(subProyect, modelName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la búsqueda del modelo. {ex.Message}");
            }
        }

        #endregion Búsqueda de los modelos
    }
}
