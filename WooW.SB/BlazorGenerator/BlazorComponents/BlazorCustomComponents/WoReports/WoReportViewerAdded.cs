using System.Text;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports
{
    public class WoReportViewerAdded
    {
        private WoItem _item = new WoItem();

        private StringBuilder _strResult = new StringBuilder();

        public string GetCodeLoadReports(WoItem item)
        {
            _item = item;
            _strResult.Clear();

            _strResult.AppendLine(BuildCode());

            return _strResult.ToString();
        }

        public string GetCodeOnInitialized(WoItem item)
        {
            return $@"
                        await {item.Id}_InitializeReport();
                        GetListParams();";
        }

        public string GetCodeOnAfterRender(WoItem item)
        {
            return $@"  await {item.Id}_GetParameters();
                    if (_selectedReport{item.Id} != null && _selectedReport{item.Id} != string.Empty)
                    {{
                        await {item.Id}_SelectedReportChanged(_selectedReport{item.Id});
                        {item.Id}_CalculateReport();
                    }}";
        }

        private string BuildCode()
        {
            return $@"await {_item.BaseModelName}Report_InitializeReport();";
        }
    }
}
