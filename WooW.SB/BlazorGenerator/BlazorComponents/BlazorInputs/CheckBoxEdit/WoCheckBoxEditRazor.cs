using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.CheckBoxEdit
{
    public class WoCheckBoxEditRazor
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Contiene la definición del componente en codigo, contiene
        /// todo el codigo necesario para construir un CheckBox
        /// </summary>
        private StringBuilder _checkEdit = new StringBuilder();

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Chk";

        /// <summary>
        /// Prefijo del componente en minusculas
        /// </summary>
        private string _lowShortComponent = "chk";

        #endregion Atributos

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(string identItemProperty, WoItem item)
        {
            this._identMethodsAndProperties = identItemProperty;

            _item = item;

            _checkEdit.Clear();

            _checkEdit.AppendLine(BuildTag());
            _checkEdit.AppendLine(BuildItem());
            _checkEdit.AppendLine(BuildComponent());
            _checkEdit.AppendLine(BuildAlert());
            _checkEdit.AppendLine(BuildEndComponentes());
            _razorReady.Details =
                $@"Se a generado con éxito el código razor para el componente: {item.Id}";
            _observer.SetLog(_razorReady);

            return _checkEdit.ToString();
        }
        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoCheckBoxEditRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoCheckBoxEdit\WoCheckBoxEditRazor.cs.*@
{_identMethodsAndProperties}@*WoWSB por el generador a día 5-10-2023*@";
        }

        #endregion Tag

        #region Contrucción del codigo

        /// <summary>
        /// Contrucción de WoFormItem
        /// </summary>
        /// <returns></returns>
        private string BuildItem()
        {
            string etiqueta = string.Empty;
            if (_item.Etiqueta != null)
            {
                etiqueta = _item.Etiqueta;
            }
            else
            {
                ModeloColumna column = _project
                    .ModeloCol.Modelos.Where(x => x.Id == _item.BaseModelName)
                    .FirstOrDefault()
                    .Columnas.Where(x => x.Id == _item.BindedProperty)
                    .FirstOrDefault();
                etiqueta = Etiqueta.ToId(column.Formulario);
            }

            return $@"
{_identMethodsAndProperties}@*{_item.Id}*@
{_identMethodsAndProperties}<WoFormItem ColSpan=""{_item.ColSpan}""
{_identMethodsAndProperties}            BeginRow=""{_item.BeginRow.ToString().ToLower()}""
{_identMethodsAndProperties}            Caption=""@Localizer[""{etiqueta}""]""
{_identMethodsAndProperties}            SetStatus=""@SetStatusFli{_item.Id}""
{_identMethodsAndProperties}            @ref=""@_fli{_item.Id}"">
{_identMethodsAndProperties}    <TemplateFragment>";
        }

        /// <summary>
        /// Contrucción del componente
        /// </summary>
        private string BuildComponent()
        {
            string nulleablity = (_item.Nullable) ? "?" : string.Empty;
            return $@"
{_identMethodsAndProperties}        <WoCheckBox Id=""{_lowShortComponent}{_item.Id}""
{_identMethodsAndProperties}                    TValue=""{_item.BindingType}""
{_identMethodsAndProperties}                    Value=""@_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}""
{_identMethodsAndProperties}                    IsRequired=@{(!_item.Nullable).ToString().ToLower()}
{_identMethodsAndProperties}                    SetStatus=""@SetStatus{_shortComponent}{_item.Id}""
{_identMethodsAndProperties}                    ValueChangedEvc=""@{_item.Id}_OnChange""
{_identMethodsAndProperties}                    OnFocusEvc=""@_scriptsUser.{_item.Id}_OnFocus""
{_identMethodsAndProperties}                    OnBlurEvc=""@_scriptsUser.{_item.Id}_OnBlur""
{_identMethodsAndProperties}                    @ref=""@_{_lowShortComponent}{_item.Id}"" />";
        }

        /// <summary>
        /// Contrucción de la alerta
        /// </summary>
        private string BuildAlert()
        {
            return $@"
{_identMethodsAndProperties}        <WoInputAlert SetStatus=""@SetStatusWia{_item.Id}"" @ref=""@_wia{_item.Id}"" />";
        }

        /// <summary>
        /// Construcción de los cierres de etiquetas
        /// </summary>
        private string BuildEndComponentes()
        {
            return $@"
{_identMethodsAndProperties}   </TemplateFragment>
{_identMethodsAndProperties}</WoFormItem>";
        }
        #endregion Contrucción del codigo

        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoComboEnumEditRazor",
                MethodOrContext = "GetComboEnumEditRazor"
            }
        };

        #endregion Logs
    }
}
