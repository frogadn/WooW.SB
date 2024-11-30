using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;

namespace MyClosingReport
{
    public class MyClosingReportCommandHandler : ICommandHandler
    {
        XRDesignMdiController controller;

        public MyClosingReportCommandHandler(XRDesignMdiController controller)
        {
            this.controller = controller;
        }

        public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler)
        {
            useNextHandler = !(command == ReportCommand.Closing || command == ReportCommand.Close);
            return !useNextHandler;
        }

        public void HandleCommand(ReportCommand command, object[] args)
        {
            for (int i = 0; i < controller.XtraTabbedMdiManager.View.Documents.Count; i++)
            {
                BaseDocument document = controller.XtraTabbedMdiManager.View.Documents[i];

                XRDesignPanelForm designForm = document.Form as XRDesignPanelForm;
                if (designForm != null)
                {
                    XRDesignPanel panel = designForm.DesignPanel;
                    panel.ReportState = ReportState.Saved;
                }
            }
        }
    }
}
