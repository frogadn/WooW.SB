using System;
using System.Collections.Generic;
using System.Linq;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorExtencionsAndPackages
{
    public partial class WoProjectDataHelper
    {
        #region Recuperación del nombre de los modelos diseñados

        /// <summary>
        /// Recupera la lista de los nombres de los modelos diseñados
        /// </summary>
        /// <returns></returns>
        public List<string> GetFormDesignedModels()
        {
            try
            {
                _readedProyects.Clear();
                _designedModels.Clear();

                SearchDesignedModels(_project);

                return _designedModels;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar los nombres de los modelos diseñados. {ex.Message}"
                );
            }
        }

        #endregion Recuperación del nombre de los modelos diseñados


        #region Búsqueda de los modelos diseñados

        /// <summary>
        /// Lista de los modelos ya diseñados
        /// </summary>
        private List<string> _designedModels = new List<string>();

        /// <summary>
        /// Búsqueda de los diseños de los modelos en todos los paquetes internos.
        /// </summary>
        /// <param name="project"></param>
        /// <exception cref="Exception"></exception>
        private void SearchDesignedModels(Proyecto project)
        {
            try
            {
                string projectName = GetWWSBName(project);

                if (!_readedProyects.Contains(projectName))
                {
                    _readedProyects.Add(projectName);

                    List<string> findModels = WoDirectory.ReadDirectoryFiles(
                        path: $@"{project.DirLayOuts}\FormDesign",
                        onlyNames: true
                    );

                    foreach (string findModel in findModels)
                    {
                        if (!_designedModels.Contains(findModel))
                        {
                            _designedModels.Add(findModel);
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

                        SearchDesignedModels(subProyect);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al buscar los modelos diseñados. {ex.Message}");
            }
        }

        #endregion Búsqueda de los modelos diseñados


        #region Recuperación del nombre del los modelos de listas diseñados

        /// <summary>
        /// Recupera la lista de los nombres de los modelos de tipo lista diseñados
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetListDesignedModels()
        {
            try
            {
                _readedProyects.Clear();
                _gridModelDesigned.Clear();

                SearchListDesingModelName(_project);

                return _gridModelDesigned;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar los nombres de los modelos con listas diseñadas. {ex.Message}"
                );
            }
        }

        #endregion Recuperación del nombre del los modelos de listas diseñados


        #region Búsqueda del nombre de los modelos

        /// <summary>
        /// Lista de los nombres de los modelos de listas diseñados
        /// </summary>
        private List<string> _gridModelDesigned = new List<string>();

        /// <summary>
        /// Búsqueda recursiva de los nombres de los modelos diseñados
        /// </summary>
        /// <param name="project"></param>
        /// <exception cref="Exception"></exception>
        private void SearchListDesingModelName(Proyecto project)
        {
            try
            {
                try
                {
                    string projectName = GetWWSBName(project);

                    if (!_readedProyects.Contains(projectName))
                    {
                        _readedProyects.Add(projectName);

                        List<string> findModels = WoDirectory.ReadDirectoryFiles(
                            path: $@"{project.DirLayOuts}\ListDesign",
                            onlyNames: true
                        );

                        foreach (string findModel in findModels)
                        {
                            if (!_gridModelDesigned.Contains(findModel))
                            {
                                _gridModelDesigned.Add(findModel);
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

                            SearchListDesingModelName(subProyect);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($@"Error al buscar los modelos diseñados. {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la búsqueda de los diseños. {ex.Message}");
            }
        }

        #endregion Búsqueda del nombre de los modelos
    }
}
