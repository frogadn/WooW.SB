using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;

namespace WooW.SB.Designer.DesignerFactory
{
    public abstract class AWoDesignerFactoryBase
    {
        #region Instancias singleton

        protected WoLogObserver Observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        /// <summary>
        /// Texto del componente.
        /// </summary>
        protected string _text = "";

        /// <summary>
        /// Nombre del componente.
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// Columnas ocupadas por el componente.
        /// </summary>
        public int ColumnSpan { get; set; } = 1;

        /// <summary>
        /// Filas o renglones ocupadas por el componente.
        /// </summary>
        public int RowSpan { get; set; } = 1;

        /// <summary>
        /// Posición inicial del componente respecto a las columnas.
        /// Desde que columna inicia.
        /// </summary>
        public int ColumnIndex { get; set; } = 0;

        /// <summary>
        /// Posición inicial del componente respecto a las filas o renglones.
        /// Desde que fila o renglón inicia.
        /// </summary>
        public int RowIndex { get; set; } = 0;

        #region Logs

        protected WoLog InvalidColumnSpan = new WoLog()
        {
            CodeLog = "000",
            Title = "ColumnSpan invalido",
            Details =
                "El column span que se esta intentando asignar es invalido (No puede ser nulo ni menor a 1).",
            UserMessage =
                "El column span que se esta intentando asignar es invalido (No puede ser nulo ni menor a 1).",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoGroupFactory",
                MethodOrContext = "ValidateProperties",
                LineAprox = "66"
            }
        };

        protected WoLog InvalidRowSpan = new WoLog()
        {
            CodeLog = "000",
            Title = "RowSpan invalido",
            Details =
                "El row span que se esta intentando asignar es invalido (No puede ser nulo ni menor a 1).",
            UserMessage =
                "El row span que se esta intentando asignar es invalido (No puede ser nulo ni menor a 1).",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoGroupFactory",
                MethodOrContext = "ValidateProperties",
                LineAprox = "66"
            }
        };

        protected WoLog InvalidColumnIndex = new WoLog()
        {
            CodeLog = "000",
            Title = "ColumnIndex invalido",
            Details =
                "El column index que se esta intentando asignar es invalido (No puede ser nulo ni menor a 0).",
            UserMessage =
                "El column index que se esta intentando asignar es invalido (No puede ser nulo ni menor a 0).",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoGroupFactory",
                MethodOrContext = "ValidateProperties",
                LineAprox = "66"
            }
        };

        protected WoLog InvalidRowIndex = new WoLog()
        {
            CodeLog = "000",
            Title = "RowIndex invalido",
            Details =
                "El row index que se esta intentando asignar es invalido (No puede ser nulo ni menor a 0).",
            UserMessage =
                "El row index que se esta intentando asignar es invalido (No puede ser nulo ni menor a 0).",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoGroupFactory",
                MethodOrContext = "ValidateProperties",
                LineAprox = "66"
            }
        };

        protected WoLog InvalidInternalRows = new WoLog()
        {
            CodeLog = "000",
            Title = "InternalRows invalido",
            Details =
                "La cantidad de rows internas que esta intentando asignar es invalido (No puede ser nulo ni menor a 1).",
            UserMessage =
                "La cantidad de rows internas que esta intentando asignar es invalido (No puede ser nulo ni menor a 1).",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoGroupFactory",
                MethodOrContext = "ValidateProperties",
                LineAprox = "66"
            }
        };

        #endregion Logs
    }
}
