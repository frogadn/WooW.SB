using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpDialogEdit
{
    public class WoLookUpDialogEditRazor
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
        private string _shortComponent = "Lkd";

        /// <summary>
        /// Prefijo del componente en minusculas
        /// </summary>
        private string _lowShortComponent = "lkd";

        /// <summary>
        /// nombre para servicios e index del classModelType
        /// </summary>
        private string _classModelTypeMultiple = string.Empty;

        /// <summary>
        /// Columna de la que se realizara el control
        /// </summary>
        private ModeloColumna _column;

        #endregion Atributos

        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar la generación del codigo del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(string identItemProperty, WoItem item)
        {
            this._identMethodsAndProperties = identItemProperty;
            _item = item;

            GetColumn();

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

        #region Recuperación de la columna

        /// <summary>
        /// Recupera la columna buscando en el modelo y los paquetes
        /// </summary>
        /// <returns></returns>
        private void GetColumn()
        {
            try
            {
                Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(x =>
                    x.Id == _item.BaseModelName
                );
                _column = findModel.Columnas.FirstOrDefault(x => x.Id == _item.BindedProperty);
                if (_column == null)
                {
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    List<Modelo> models = woProjectDataHelper.GetModelWithExtencions(
                        _item.BaseModelName
                    );
                    foreach (Modelo model in models)
                    {
                        _column = model.Columnas.FirstOrDefault(x => x.Id == _item.BindedProperty);
                        if (_column != null)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar la columna. {ex.Message}");
            }
        }

        #endregion Recuperación de la columna

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoLookUpDialogEditRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoLookUpDialogEdit\WoLookUpDialogEditRazor.cs.*@
{_identMethodsAndProperties}@*WoWSB por el generador a día 5-10-2023*@";
        }

        #endregion Tag


        #region Construcción del código

        /// <summary>
        /// Construcción de WoFormItem
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
                etiqueta = Etiqueta.ToId(_column.Formulario);
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
        /// Construcción del componente
        /// </summary>
        private string BuildComponent()
        {
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

            Modelo model = woProjectDataHelper.GetMainModel(_item.ClassModelType);

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
{_identMethodsAndProperties}        <WoLookupDialogEdit Id=""{_lowShortComponent}{_item.Id}""
{_identMethodsAndProperties}                            TData=""{_item.ClassModelType}""          
{_identMethodsAndProperties}                            TApp=""App""
{_identMethodsAndProperties}                            PlaceHolder=""{_item.PlaceHolder}""
{_identMethodsAndProperties}                            GridColumnsCol=""@_listColumns{_item.BindedProperty}""
{_identMethodsAndProperties}                            Value=""@_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}""
{_identMethodsAndProperties}                            ValueChangedEvc=""@{_item.BindedProperty}_OnChange""
{_identMethodsAndProperties}                            ValueProperty=""@_keyField{_item.BindedProperty}""
{_identMethodsAndProperties}                            Description=""@_scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty}""
{_identMethodsAndProperties}                            FormRoute=""{pathFormRoute}""
{_identMethodsAndProperties}                            IsRequired=@{(!_item.Nullable) .ToString() .ToLower()}
{_identMethodsAndProperties}                            InputWidth=""eLookupInputWidth.{_item .LookUpInputSize.ToString() .ToLower()}""
{_identMethodsAndProperties}                            SetStatus=""@SetStatus{_shortComponent}{_item.Id}""
{_identMethodsAndProperties}                            OnInitializedEvc=""{_shortComponent}{_item.BindedProperty}_OnInitialize""
{_identMethodsAndProperties}                            OnFocusEvc=""@_scriptsUser.{_item.BindedProperty}_OnFocus""
{_identMethodsAndProperties}                            OnBlurEvc=""@_scriptsUser.{_item.BindedProperty}_OnBlur""
{_identMethodsAndProperties}                            CellNavigationEvc=""@{_item.BindedProperty}_CellNavigation""
{_identMethodsAndProperties}                            SetAlertEvc=""@{_item.BindedProperty}SetAlert""
{_identMethodsAndProperties}                            @ref=""@_{_lowShortComponent}{_item.Id}"" />
";
        }

        /// <summary>
        /// Construcción de la alerta
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
        #endregion Construcción del código

        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoLookUpDialogEditRazor",
                MethodOrContext = "GetLookUpDialogEditRazor"
            }
        };

        #endregion Logs
    }
}
