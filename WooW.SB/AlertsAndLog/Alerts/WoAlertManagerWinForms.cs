using System;
using System.Windows.Forms;
using WooW.Core.Common.Observer.LogModel;

namespace WooW.SB.AlertsAndLog.Alerts
{
    public class WoAlertManagerWinForms : IWoAlertManager
    {
        public void ShowSystemMessage(WoLog woLog)
        {
            MessageBox.Show(
                text: $@"{woLog.CodeLog}: {woLog.Title}",
                caption: $@"{woLog.UserMessage}"
            );
        }

        public void ShowCustomAlert(WoLog woLog) { }

        public void ShowInternalAlert(WoLog woLog)
        {
            throw new NotImplementedException();
        }
    }
}
