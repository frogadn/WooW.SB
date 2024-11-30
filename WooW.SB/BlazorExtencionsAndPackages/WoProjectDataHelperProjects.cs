using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using WooW.SB.Config;

namespace WooW.SB.BlazorExtencionsAndPackages
{
    public partial class WoProjectDataHelper
    {
        #region Recuperación del proyecto

        /// <summary>
        /// Recuperamos el proyecto que contiene el modelo con el
        /// nombre que se recibe por parámetro
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public Proyecto GetTopProjectWhit(string modelName)
        {
            try
            {
                Proyecto findProject = SearchTopProject(modelName, _project);
                if (findProject != null)
                {
                    return findProject;
                }
                else
                {
                    throw new Exception(
                        $@"Error al recuperar el proyecto con el modelo. {modelName}"
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar el proyecto con el modelo {modelName}. {ex.Message}"
                );
            }
        }

        #endregion Recuperación del proyecto


        #region Búsqueda de proyectos

        /// <summary>
        /// Búsqueda del proyecto top que contiene el modelo
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="proyecto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Proyecto SearchTopProject(string modelName, Proyecto proyecto)
        {
            try
            {
                Modelo findModel = proyecto.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == modelName
                );

                if (findModel != null)
                {
                    return proyecto;
                }
                else
                {
                    Proyecto findProject = null;

                    foreach (Paquete pack in proyecto.Paquetes)
                    {
                        Proyecto subProyect = new Proyecto();
                        string[] pathBaseDirCol = proyecto.Dir.Split("\\");
                        string pathPacks = string.Empty;
                        for (int i = 0; i < pathBaseDirCol.Count() - 1; i++)
                        {
                            pathPacks += $"{pathBaseDirCol[i]}\\";
                        }
                        subProyect.Load($"{pathPacks}{pack.Archivo}");

                        findProject = SearchTopProject(modelName, subProyect);

                        if (findProject != null)
                        {
                            break;
                        }
                    }

                    return findProject;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al buscar el proyecto con el modelo {modelName}. {ex.Message}"
                );
            }
        }

        #endregion Búsqueda de proyectos
    }
}
