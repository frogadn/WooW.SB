using System.Collections.Generic;
using WooW.Core;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorGeneratorTools
{
    public class WoBlazorAnalize
    {
        #region analizador esclavas

        /// <summary>
        /// Busca sobre el contenedor principal las esclavas del modelo y retorna una lista con los
        /// nombres de los modelos de las slaves.
        /// </summary>
        /// <param name="woContainer"></param>
        /// <returns></returns>
        public List<string> GetSlaveNames(WoContainer woContainer)
        {
            List<string> slavesModelNameCol = new List<string>();

            if (!woContainer.ContainersCol.IsNull())
            {
                foreach (var subGroup in woContainer.ContainersCol)
                {
                    slavesModelNameCol.AddRange(GetSlaveNames(subGroup));
                }
            }

            if (!woContainer.ItemsCol.IsNull())
            {
                foreach (var item in woContainer.ItemsCol)
                {
                    if (item.TypeItem == eTypeItem.Slave)
                    {
                        slavesModelNameCol.Add(item.SlaveModelId);
                    }
                }
            }
            return slavesModelNameCol;
        }

        #region Asignar el nombre de los formularios para las esclavas

        /// <summary>
        /// contador del numero de slaves encontradas
        /// </summary>
        private int _slaveCount = 0;

        /// <summary>
        /// Retorna el container con el nombre del formulario interno para las slaves
        /// (Solo para unitarias)
        /// </summary>
        /// <param name="woContainer"></param>
        /// <returns></returns>
        public WoContainer GetWoContainer(
            WoContainer woContainer,
            bool isCompleteGeneration = false
        )
        {
            _slaveCount = 0;
            return RenameSlaveForm(woContainer, isCompleteGeneration);
        }

        /// <summary>
        /// Va recorriendo y asignando el nuevo nombre de la slave;
        /// </summary>
        /// <param name="woContainer"></param>
        /// <returns></returns>
        private WoContainer RenameSlaveForm(WoContainer woContainer, bool isCompleteGeneration)
        {
            WoContainer newContainer = woContainer.GetInstance();

            if (!woContainer.ContainersCol.IsNull())
            {
                foreach (var subGroup in woContainer.ContainersCol)
                {
                    newContainer.ContainersCol.Add(RenameSlaveForm(subGroup, isCompleteGeneration));
                }
            }

            if (!woContainer.ItemsCol.IsNull())
            {
                foreach (var item in woContainer.ItemsCol)
                {
                    if (item.TypeItem == eTypeItem.Slave)
                    {
                        if (isCompleteGeneration)
                        {
                            item.InternalFrom = $"{item.ClassModelType}Slave";
                        }
                        else
                        {
                            item.InternalFrom = $"Slave{_slaveCount}Slave";
                        }
                        _slaveCount++;
                    }
                    newContainer.ItemsCol.Add(item);
                }
            }

            return woContainer;
        }

        #endregion Asignar el nombre de los formularios para las esclavas

        #endregion analizador esclavas

        #region analizador reportes

        /// <summary>
        /// Busca sobre el contenedor principal los reportes del modelo y retorna una lista con los
        /// nombres de los modelos de los reportes.
        /// </summary>
        /// <param name="woContainer"></param>
        /// <returns></returns>
        public List<string> GetReportNames(WoContainer woContainer)
        {
            List<string> reportsModelNameCol = new List<string>();

            if (!woContainer.ContainersCol.IsNull())
            {
                foreach (var subGroup in woContainer.ContainersCol)
                {
                    reportsModelNameCol.AddRange(GetReportNames(subGroup));
                }
            }

            if (!woContainer.ItemsCol.IsNull())
            {
                foreach (var item in woContainer.ItemsCol)
                {
                    if (item.TypeItem == eTypeItem.ReportItem)
                    {
                        reportsModelNameCol.Add(item.BaseModelName);
                    }
                }
            }
            return reportsModelNameCol;
        }

        #region Asignar el nombre de los formularios para los reportes

        /// <summary>
        /// contador del numero de reportes encontrados
        /// </summary>
        private int _reportCount = 0;

        /// <summary>
        /// Retorna el container con el nombre del formulario interno para las reports
        /// (Solo para unitarias)
        /// </summary>
        /// <param name="woContainer"></param>
        /// <returns></returns>
        public WoContainer GetContainerWithReportNames(
            WoContainer woContainer,
            bool isCompleteGeneration = false
        )
        {
            _reportCount = 0;
            return RenameReportForm(woContainer, isCompleteGeneration);
        }

        /// <summary>
        /// Va recorriendo y asignando el nuevo nombre de la report;
        /// </summary>
        /// <param name="woContainer"></param>
        /// <returns></returns>
        private WoContainer RenameReportForm(WoContainer woContainer, bool isCompleteGeneration)
        {
            WoContainer newContainer = woContainer.GetInstance();

            if (!woContainer.ContainersCol.IsNull())
            {
                foreach (var subGroup in woContainer.ContainersCol)
                {
                    newContainer.ContainersCol.Add(
                        RenameReportForm(subGroup, isCompleteGeneration)
                    );
                }
            }

            if (!woContainer.ItemsCol.IsNull())
            {
                foreach (var item in woContainer.ItemsCol)
                {
                    if (item.TypeItem == eTypeItem.ReportItem)
                    {
                        if (isCompleteGeneration)
                        {
                            item.InternalFrom = $"{item.ReportRequest}Layout";
                        }
                        else
                        {
                            item.InternalFrom = $"Report{_reportCount}Layout";
                        }
                        _reportCount++;
                    }
                    newContainer.ItemsCol.Add(item);
                }
            }

            return woContainer;
        }

        #endregion Asignar el nombre de los formularios para los reportes


        #endregion analizador reportes
    }
}
