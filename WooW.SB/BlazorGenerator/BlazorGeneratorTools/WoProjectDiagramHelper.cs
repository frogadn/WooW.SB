using System;
using System.Collections.Generic;
using System.Linq;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;

namespace WooW.SB.BlazorGenerator.BlazorGeneratorTools
{
    public class WoProjectDiagramHelper
    {
        #region Atributos

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        /// <summary>
        /// Helper para recuperar información de los proyectos
        /// </summary>
        private WoProjectDataHelper _dataHelper = new WoProjectDataHelper();

        #endregion Atributos


        #region Columnas de la transición

        /// <summary>
        /// Recuperamos la lista de todas las columnas presentes en el
        /// DTO de la transición editables
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="transitionName"></param>
        public List<string> GetFullTransitionEditColumns(string modelName, string transitionName)
        {
            try
            {
                List<string> columnsEdit = new List<string>();

                List<Transicion> findTransitions = GetTransitions(modelName, transitionName);
                foreach (Transicion transition in findTransitions)
                {
                    foreach (string column in transition.DTO.Columnas)
                    {
                        if (!columnsEdit.Contains(column))
                        {
                            columnsEdit.Add(column);
                        }
                    }
                }

                return columnsEdit;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar la lista de las columnas de la transición. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Recupera la lista de las columnas no editables de la transición
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="transitionName"></param>
        /// <returns></returns>
        public List<string> GetFullTransitionNoEditColumns(string modelName, string transitionName)
        {
            try
            {
                List<string> columnsNoEdit = new List<string>();

                List<Transicion> findTransitions = GetTransitions(modelName, transitionName);

                foreach (Transicion transicion in findTransitions)
                {
                    foreach (string column in transicion.DTO.ColumnasNoEditar)
                    {
                        if (!columnsNoEdit.Contains(column))
                        {
                            columnsNoEdit.Add(column);
                        }
                    }
                }

                return columnsNoEdit;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar las columnas no editables de la transición. {ex.Message}"
                );
            }
        }

        #endregion Columnas de la transición


        #region Recuperación de la transición

        /// <summary>
        /// Recupera las transiciones del modelo y sus extensiones
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<Transicion> GetTransitions(string modelName, string transitionName)
        {
            try
            {
                List<Transicion> transitionsResultCol = new List<Transicion>();

                List<Modelo> fullModelCol = _dataHelper.GetModelWithExtencions(modelName);

                foreach (Modelo model in fullModelCol)
                {
                    Transicion transicion = GetTransition(model.Diagrama, transitionName);
                    transitionsResultCol.Add(transicion);
                }

                return transitionsResultCol;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar las transiciones. {ex.Message}");
            }
        }

        /// <summary>
        /// Recuperamos la transición con el nombre indicado sobre del diagrama indicado
        /// </summary>
        /// <param name="model"></param>
        /// <param name="transitionName"></param>
        /// <returns></returns>
        private Transicion GetTransition(ModeloDiagrama diagram, string transitionName)
        {
            try
            {
                Transicion findTransition = diagram.Transiciones.FirstOrDefault(transition =>
                    transition.Id == transitionName
                );

                if (findTransition == null)
                {
                    throw new Exception($@"No se encontró la transición {transitionName}.");
                }

                return findTransition;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar la transición {transitionName} del modelo. {ex.Message}"
                );
            }
        }

        #endregion Recuperación de la transición


        #region Id de las transiciones

        /// <summary>
        /// Recupera los identificadores de las transiciones del modelo actual
        /// </summary>
        /// <returns></returns>
        public List<string> GetTransitionsId(string modelName)
        {
            try
            {
                List<string> transitionsName = new List<string>();

                Modelo findModel = _dataHelper.GetActualModel(modelName);

                foreach (Transicion transition in findModel.Diagrama.Transiciones)
                {
                    transitionsName.Add(transition.Id);
                }

                return transitionsName;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar los id de las transiciones. {ex.Message}"
                );
            }
        }

        #endregion Id de las transiciones
    }
}
