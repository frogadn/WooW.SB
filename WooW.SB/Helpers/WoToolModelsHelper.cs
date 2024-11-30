using System;
using System.Linq;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.SB.Config;

namespace WooW.SB.Helpers
{
    public class WoToolModelsHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Búsquedas

        /// <summary>
        /// Busca el modelo en función del nombre.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        /// <exception cref="WoObserverException"></exception>
        public Modelo SearchModel(string modelName)
        {
            Modelo _findModel = null;
            try
            {
                _findModel = _project.ModeloCol.Modelos.Where(x => x.Id == modelName).First();
                if (_findModel == null)
                {
                    throw new WoObserverException(
                        _cantFindModel,
                        $@"El modelo {modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                    );
                }
            }
            catch (Exception ex)
            {
                throw new WoObserverException(
                    _cantFindModel,
                    $@"El modelo {modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                );
            }

            return _findModel;
        }

        #endregion Búsquedas


        #region Logs

        /// <summary>
        /// Alerta del log cuando no se encuentra el modelo.
        /// </summary>
        private WoLog _cantFindModel = new WoLog()
        {
            CodeLog = "000",
            Title = $@"El modelo no se encuentra el proyecto.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = "ChargeModel"
            }
        };

        #endregion Logs
    }
}
