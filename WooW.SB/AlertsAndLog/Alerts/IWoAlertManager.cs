using WooW.Core.Common.Observer.LogModel;

namespace WooW.SB.AlertsAndLog.Alerts
{
    /// <summary>
    /// Interfaz que determina cuales serán los métodos base que debe contener una
    /// implementación de alertas.
    /// </summary>
    public interface IWoAlertManager
    {
        /// <summary>
        /// Alertas de la plataforma donde nos encontremos.
        /// Un Alert en web o un MessageBox en windows.
        /// </summary>
        /// <param name="woLog"></param>
        void ShowSystemMessage(WoLog woLog);

        /// <summary>
        /// Alertas customizadas pero correspondientes a la plataforma.
        /// Un dialog o form personalizado en WinForms o una librería de alertas en web.
        /// </summary>
        /// <param name="woLog"></param>
        void ShowCustomAlert(WoLog woLog);

        /// <summary>
        /// Alertas internas del sistema.
        /// Sistemas de alerta con panel de notificaciones interno e independiente
        /// de las posibilidades que brinde la plataforma.
        /// </summary>
        /// <param name="woLog"></param>
        void ShowInternalAlert(WoLog woLog);
    }
}
