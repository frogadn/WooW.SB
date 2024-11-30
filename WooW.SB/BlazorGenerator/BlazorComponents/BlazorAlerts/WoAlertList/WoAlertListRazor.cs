using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorAlerts.WoAlertList
{
    public class WoAlertListRazor
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Resultado final del codigo de la clase parcial del componente.
        /// </summary>
        private StringBuilder _strInstance = new StringBuilder();

        /// <summary>
        /// Instancia con la metadata del item para la generación.
        /// </summary>
        private WoItem _woItem = new WoItem();

        /// <summary>
        /// Identación del codigo.
        /// </summary>
        private string _identItemProperty = string.Empty;

        #endregion Atributos

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo razor del componente.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public string GetCode(string identItemProperty, WoItem item)
        {
            _woItem = item;
            _identItemProperty = identItemProperty;

            _strInstance.Clear();

            if (item.AddFormItem)
            {
                _strInstance.AppendLine(GetAlertWoFormRazor());
            }
            else
            {
                _strInstance.AppendLine(GetAlertRazor());
            }

            _razorReady.Details =
                $@"Se a generado con éxito el código razor para el componente: {item.Id}";
            _observer.SetLog(_razorReady);

            return _strInstance.ToString();
        }

        #endregion Método principal


        #region Build razor encapsulado

        private string GetAlertWoFormRazor()
        {
            return $@"
{_identItemProperty}@*Este código fue generado por el fichero WoAlertListRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorAlerts\WoAlertList\WoAlertListRazor.cs*@
{_identItemProperty}@*WoWSB por el generador a día 5-10-2023*@
{_identItemProperty}@*Formulario {_woItem.Id}*@
{_identItemProperty}<WoFormItem ColSpan=""12"" BeginRow=""true"">
{_identItemProperty}    <TemplateFragment>
{_identItemProperty}         <WoFormAlert SetStatus=""@SetStatusAlerts"" RemoveAlertEvc=""_modelControls.{_woItem.Id}.RemoverAlerta"" @ref=""@_alerts"" />
{_identItemProperty}    </TemplateFragment>
{_identItemProperty}</WoFormItem>
";
        }

        #endregion Build razor encapsulado

        #region Build razor

        private string GetAlertRazor()
        {
            return $@"
{_identItemProperty}@*Este código fue generado por el fichero WoAlertListRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorAlerts\WoAlertList\WoAlertListRazor.cs*@
{_identItemProperty}@*WoWSB por el generador a día 5-10-2023*@
{_identItemProperty}@*Formulario {_woItem.Id}*@
{_identItemProperty}<WoFormAlert SetStatus=""@SetStatusAlerts"" RemoveAlertEvc=""_modelControls.{_woItem.Id}.RemoverAlerta"" @ref=""@_alerts"" />
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
                Class = "WoTextEditRazor",
                MethodOrContext = "GetTextEditRazor"
            }
        };

        #endregion Logs
    }
}
