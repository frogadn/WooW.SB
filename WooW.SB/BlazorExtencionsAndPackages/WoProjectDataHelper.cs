using System;
using System.Collections.Generic;
using System.Linq;
using WooW.SB.Config;

namespace WooW.SB.BlazorExtencionsAndPackages
{
    public partial class WoProjectDataHelper
    {
        #region Atributos

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        /// <summary>
        /// Lista de los proyectos que ya fueron analizados
        /// </summary>
        private List<string> _readedProyects = new List<string>();

        #endregion Atributos

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        public WoProjectDataHelper(Proyecto project = null)
        {
            if (project != null)
            {
                _project = project;
            }
        }

        #endregion Constructor


        #region Recuperación de los pathBase de los paquetes

        /// <summary>
        /// Recupera los paths base de todos los proyectos que contengan
        /// el modelo que se recibe por parámetro
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetProyectPathsWhit(string modelName)
        {
            try
            {
                _readedProyects.Clear();
                _paths.Clear();
                SearchFullPaths(modelName, _project);
                return _paths;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar los paths de los packs. {ex.Message}");
            }
        }

        #endregion Recuperación de los pathBase de los paquetes


        #region Búsqueda de los paths

        /// <summary>
        /// Lista con los paths de los paquetes cargados en el proyecto
        /// </summary>
        private List<string> _paths = new List<string>();

        /// <summary>
        /// Recorre los proyectos hacia abajo agregando los paths a una lista
        /// </summary>
        private void SearchFullPaths(string modelName, Proyecto project)
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
                        _paths.Add(project.Dir.Replace("/", "\\"));
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

                        SearchFullPaths(modelName, subProyect);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al buscar los paths. {ex.Message}");
            }
        }

        #endregion Búsqueda de los paths


        #region Recuperación de los paths de los proyectos con extensiones del modelo

        /// <summary>
        /// Recupera los paths base de los proyectos con extensiones
        /// pata el modelo con el nombre que se recibe por parámetro
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetExtencionsProjectPath(string modelName)
        {
            try
            {
                _readedProyects.Clear();
                _extensionPaths.Clear();

                SearchExtencionPaths(modelName, _project);

                return _extensionPaths;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar los paths de los proyectos con extensiones de {modelName}. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de los paths de los proyectos con extensiones del modelo

        #region Búsqueda de los paths de las extensiones

        /// <summary>
        /// Lista con los paths de los paquetes cargados en el proyecto
        /// </summary>
        private List<string> _extensionPaths = new List<string>();

        /// <summary>
        /// Recorre los proyectos hacia abajo agregando los paths a una lista
        /// </summary>
        private void SearchExtencionPaths(string modelName, Proyecto project)
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

                    if (findModel != null && findModel.ProcesoId == string.Empty)
                    {
                        _paths.Add(project.Dir.Replace("/", "\\"));
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

                        SearchExtencionPaths(modelName, subProyect);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al buscar los paths. {ex.Message}");
            }
        }

        #endregion Búsqueda de los paths de las extensiones


        #region Fichero WWSB

        /// <summary>
        /// Recupera desde la instancia de proyecto que se le pasa el path del
        /// fichero principal (wwsb).
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string GetWWSBPath(Proyecto project)
        {
            try
            {
                string result = project.Dir;

                string[] projectPahtCol = project.Dir.Split("\\");

                result += $"\\{projectPahtCol.Last()}.wwsb";

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar el path del fichero wwsb. {ex.Message}");
            }
        }

        /// <summary>
        /// Recupera el nombre del proyecto, a través de la data del proyecto
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetWWSBName(Proyecto project)
        {
            try
            {
                string[] projectDirCol = project.Dir.Split("\\");

                return projectDirCol.Last();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar el nombre del proyecto en {project.Dir}. {ex.Message}"
                );
            }
        }

        #endregion Fichero WWSB
    }
}
