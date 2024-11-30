using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports
{
    public class WoReportViewerRazor
    {
        #region Instancias singleton

        /// <summary>
        /// Inyección del observador de logs.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton


        #region Attributes

        /// <summary>
        /// Resultado final del razor del report.
        /// </summary>
        private StringBuilder _strReportViewer = new StringBuilder();

        /// <summary>
        /// Indica el item con la data para la generación del razor.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Nivel de identacion
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Attributes

        #region Metodo principal

        /// <summary>
        /// Recupera el codigo del razor del report.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        /// <param name="woItem"></param>
        /// <returns></returns>
        public string GetCode(string identMethodsAndProperties, WoItem woItem)
        {
            _item = woItem;
            _identMethodsAndProperties = identMethodsAndProperties;

            _strReportViewer.Clear();

            _strReportViewer.AppendLine(GetRazor());

            return _strReportViewer.ToString();
        }

        #endregion Metodo principal


        #region Build razor

        /// <summary>
        /// Construlle el razor del report.
        /// </summary>
        /// <returns></returns>
        private string GetRazor()
        {
            return $@"
{_identMethodsAndProperties}@*Botón de share*@
{_identMethodsAndProperties}<WoReportShareButton Url=""@_listUri""
{_identMethodsAndProperties}                     GetListEvt=""@GetListParams""
{_identMethodsAndProperties}                     TApp=""App""
{_identMethodsAndProperties}                     SetStatus=""@SetStatusWsbCopiar""
{_identMethodsAndProperties}                     @ref=""@_wsbCopiar"" />


{_identMethodsAndProperties}@*Visor de reportes*@
{_identMethodsAndProperties}<WoReportViewer TApp=""App""
{_identMethodsAndProperties}                SendAlert=""@{_item.Id}_AddAlert""
{_identMethodsAndProperties}                Report=""@_report{_item.Id}""
{_identMethodsAndProperties}                ReportsCol=""@_reportsCol{_item.Id}""
{_identMethodsAndProperties}                SelectedTemplateChangedEvc=""@{_item.Id}_SelectedReportChanged""
{_identMethodsAndProperties}                OnCalculateEvc=""@{_item.Id}_CalculateReport"">

{_identMethodsAndProperties}        <FormFragment>

{_identMethodsAndProperties}            <{_item.InternalFrom} />

{_identMethodsAndProperties}        </FormFragment>
{_identMethodsAndProperties}</WoReportViewer>
";
        }

        #endregion Build razor



        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoReportViewerPartial",
                MethodOrContext = "GetReportViewRazor"
            }
        };

        #endregion Logs
    }
}
