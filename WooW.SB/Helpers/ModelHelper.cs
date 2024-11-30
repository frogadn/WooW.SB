using System;
using System.Linq;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.SB.Config;

namespace WooW.SB.Helpers
{
    public class ModelHelper
    {
        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #region Búsquedas

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
                //throw new WoObserverException(
                //    _cantFindModel,
                //    $@"El modelo {modelName} no existe en el modelo, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                //);
            }
            return _findModel;
        }

        #endregion Búsquedas


        private WoLog _cantFindModel = new WoLog()
        {
            CodeLog = "000",
            Title = $@"El modelo no se encuentra en el proyecto.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = "ChargeModel"
            }
        };
    }
}
