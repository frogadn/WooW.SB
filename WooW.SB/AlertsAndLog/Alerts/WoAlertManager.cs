using WooW.Core.Common.Observer.LogModel;

namespace WooW.SB.AlertsAndLog.Alerts
{
    public class WoAlertManager
    {
        /// <summary>
        /// Método principal que define como se comportaran y mostraran las alertas
        /// en función del log recibido.
        /// </summary>
        public void ShowAlert(object sender, WoLog woLog)
        {
            if (woLog.LogType == eLogType.Error || woLog.LogType == eLogType.Warning)
            {
                ShowSystemMessage(woLog);
            }
        }

        #region Instancia y fachada de la interfaz

        /// <summary>
        /// Instancia que puede ser de lo que sea en función de la
        /// plataforma en la que nos encontremos.
        /// Permite reutilizar este código en otras plataforma como blazor.
        /// </summary>
        private IWoAlertManager _woAlertManager = new WoAlertManagerWinForms();

        /// <summary>
        /// Alertas con las funciones base que nos permite la plataforma.
        /// </summary>
        /// <param name="woLog"></param>
        private void ShowSystemMessage(WoLog woLog)
        {
            _woAlertManager.ShowSystemMessage(woLog);
        }

        /// <summary>
        /// Alertas customizadas pero aun de la plataforma.
        /// </summary>
        /// <param name="woLog"></param>
        private void ShowCustomAlert(WoLog woLog)
        {
            _woAlertManager.ShowCustomAlert(woLog);
        }

        /// <summary>
        /// Alertas de un sistema interno independiente de la plataforma.
        /// </summary>
        /// <param name="woLog"></param>
        private void ShowInternalAlert(WoLog woLog)
        {
            _woAlertManager.ShowInternalAlert(woLog);
        }

        #endregion Instancia y fachada de la interfaz
    }
}
