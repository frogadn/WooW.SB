using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpEdit
{
    public class WoLookUpEditRazor
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
        /// todo el codigo necesario para construir un LookUpEdit
        /// </summary>
        private StringBuilder _lookUpEdit = new StringBuilder();

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Lkp";

        /// <summary>
        /// Prefijo del componente en minusculas
        /// </summary>
        private string _lowShortComponent = "lkp";

        /// <summary>
        /// nombre para servicios e index del classModelType
        /// </summary>
        private string _classModelTypeMultiple = string.Empty;

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

            _lookUpEdit.Clear();

            _classModelTypeMultiple = _item.ClassModelType;
            if (_item.MultipleReference > 0)
            {
                _classModelTypeMultiple = $@"{_classModelTypeMultiple}{_item.MultipleReference}";
            }

            _lookUpEdit.AppendLine(BuildTag());
            _lookUpEdit.AppendLine(BuildItem());
            _lookUpEdit.AppendLine(BuildComponent());
            _lookUpEdit.AppendLine(BuildAlert());
            _lookUpEdit.AppendLine(BuildEndComponentes());
            _razorReady.Details =
                $@"Se a generado con éxito el código razor para el componente: {item.Id}";
            _observer.SetLog(_razorReady);

            return _lookUpEdit.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoLookUpEditRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoLookUpDialogEdit\WoLookUpEditRazor.cs.*@
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
            Modelo model = _project
                .ModeloCol.Modelos.Where(x => x.Id == _item.ClassModelType)
                .FirstOrDefault();

            string nulleablity = (_item.Nullable) ? "?" : string.Empty;

            string pathFormRoute = string.Empty;

            if (
                model.TipoModelo == Core.WoTypeModel.Configuration
                || model.TipoModelo == Core.WoTypeModel.Control
                || model.TipoModelo == Core.WoTypeModel.Kardex
                || model.TipoModelo == Core.WoTypeModel.View
            )
            {
                pathFormRoute = $@"/{model.ProcesoId.ToUpper()}/List/{_item.ClassModelType}";
            }
            else
            {
                pathFormRoute =
                    $@"/{model.ProcesoId.ToUpper()}/{model.TipoModelo}/{_item.ClassModelType}";
            }

            return $@"
{_identMethodsAndProperties}        <WoLookupEdit Id=""{_lowShortComponent}{_item.Id}""
{_identMethodsAndProperties}                      TMetaAttribute=""WoColumnMetaAttribute""
{_identMethodsAndProperties}                      TModelAttribute=""WoLabelModelAttribute""
{_identMethodsAndProperties}                      TModel=""{_item.ClassModelType}""
{_identMethodsAndProperties}                      TApp=""App""
{_identMethodsAndProperties}                      PlaceHolder=""{_item.PlaceHolder}""
{_identMethodsAndProperties}                      ColumnsCol=""@_{_lowShortComponent}{_classModelTypeMultiple}ColumnsCol""
{_identMethodsAndProperties}                      IdValue=""@_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}""
{_identMethodsAndProperties}                      DescriptionValue=""@_scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty}""
{_identMethodsAndProperties}                      DataCol=""@_{_classModelTypeMultiple}Col""
{_identMethodsAndProperties}                      FormRoute=""{pathFormRoute}""
{_identMethodsAndProperties}                      OnOpenEvc=""@{_item.Id}_UpdateData""
{_identMethodsAndProperties}                      SetStatus=""@SetStatus{_shortComponent}{_item.Id}""
{_identMethodsAndProperties}                      OnFocusEvc=""@_scriptsUser.{_item.Id}_OnFocus""
{_identMethodsAndProperties}                      OnBlurEvc=""@_scriptsUser.{_item.Id}_OnBlur""
{_identMethodsAndProperties}                      OnChangeEvc=""@{_item.Id}_OnChange""
{_identMethodsAndProperties}                      IsRequired=@{(!_item.Nullable) .ToString() .ToLower()}
{_identMethodsAndProperties}                      InputWidth=""eLookupInputWidth.{_item .LookUpInputSize.ToString() .ToLower()}""
{_identMethodsAndProperties}                      @ref=""@_lkp{_item.Id}""/>";
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
                Class = "WoLookUpEditRazor",
                MethodOrContext = "GetLookUpEditRazor"
            }
        };

        #endregion Logs
    }
}
