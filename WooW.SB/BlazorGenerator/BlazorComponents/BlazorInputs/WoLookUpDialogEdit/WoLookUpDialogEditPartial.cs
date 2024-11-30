using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpDialogEdit
{
    public class WoLookUpDialogEditPartial
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// instancia singleton con la data del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Contiene la definición del componente en código, contiene
        /// todo el código necesario para construir un LookUP Dialog Edit
        /// </summary>
        private StringBuilder _lookUpDialogEdit = new StringBuilder();

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Contiene strings para poder identar el código
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Lkd";

        /// <summary>
        /// Prefijo del componente en minúsculas
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

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase, recibe e inicializa el nombre del modelo
        /// al que pertenecerá el componente.
        /// </summary>
        /// <param name="modelName"></param>
        public WoLookUpDialogEditPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar la generación del código del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            GetColumn();

            _classModelTypeMultiple = _item.ClassModelType;
            if (_item.MultipleReference > 0)
            {
                _classModelTypeMultiple = $@"{_classModelTypeMultiple}{_item.MultipleReference}";
            }

            _lookUpDialogEdit.Clear();

            Grid();

            _lookUpDialogEdit.AppendLine(
                $@"{_identMethodsAndProperties}#region {item.BindedProperty}"
            );

            _lookUpDialogEdit.AppendLine(BuildTag());

            _lookUpDialogEdit.AppendLine(BuildRefComponent());

            _lookUpDialogEdit.AppendLine(BuildSetStatus());

            _lookUpDialogEdit.AppendLine(BuildRefContainer());

            _lookUpDialogEdit.AppendLine(BuildSetStatusContainer());

            _lookUpDialogEdit.AppendLine(BuildRefAlert());

            _lookUpDialogEdit.AppendLine(BuildSetStatusAlert());

            _lookUpDialogEdit.AppendLine(BuildRefDto());

            _lookUpDialogEdit.AppendLine(BuildRefOData());

            _lookUpDialogEdit.AppendLine(BuildRefPropDto());

            _lookUpDialogEdit.AppendLine(BuildComponentDecorated());

            _lookUpDialogEdit.AppendLine(BuildOnclick());

            _lookUpDialogEdit.AppendLine(BuildCellNavigation());

            _lookUpDialogEdit.AppendLine(
                $@"{_identMethodsAndProperties}#endregion {item.BindedProperty}"
            );

            _partialReady.Details =
                $@"Se creo el código parcial para el componente: {item.BindedProperty}.";
            _observer.SetLog(_partialReady);

            return _lookUpDialogEdit.ToString();
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
{_identMethodsAndProperties}// Este código fue generado por el fichero WoLookUpDialogEditPartial.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoLookUpDialogEdit\WoLookUpDialogEditPartial.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag


        #region Construcción del código

        /// <summary>
        /// Referencia del componente que se esta generando.
        /// </summary>
        /// <returns></returns>
        private string BuildRefComponent()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia que representa el componente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoLookupDialogEdit<{_item.ClassModelType}, App>? _{_lowShortComponent}{_item.BindedProperty};";
        }

        /// <summary>
        /// Construye la función para enlazar los eventos de la vista al componente
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatus()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Suscribe los controladores de eventos de la vista del componente a los
{_identMethodsAndProperties}/// eventos del componente a través de la referencia.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void SetStatus{_shortComponent}{_item.BindedProperty}()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_{_lowShortComponent}{_item.BindedProperty} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeEnabledEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeStatus;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeSizeEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeFontSize;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontColorEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontColor;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontWeightEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontWeight;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontItalicEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontItalic;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeControlFontDecorationEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeControlFontDecoration;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeBackColorEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeBackgroundColor;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ActualizarComponente();
{_identMethodsAndProperties}        CheckForm();
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}";
        }

        /// <summary>
        /// Referencia del contenedor que se esta generando.
        /// </summary>
        /// <returns></returns>
        private string BuildRefContainer()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia del contenedor del item.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoFormItem? _fli{_item.BindedProperty};";
        }

        /// <summary>
        /// Construye la función para enlazar los eventos de la vista a los eventos del contenedor.
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatusContainer()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Suscribe los controladores de eventos de la vista del contenedor a los 
{_identMethodsAndProperties}/// eventos del contenedor a través de la referencia.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""status""></param>
{_identMethodsAndProperties}public void SetStatusFli{_item.BindedProperty}()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_fli{_item.BindedProperty} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontColorEvt += _fli{_item.BindedProperty}.ChangeCaptionFontColor;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontWeightEvt += _fli{_item.BindedProperty}.ChangeCaptionFontWeight;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontItalicEvt += _fli{_item.BindedProperty}.ChangeCaptionFontItalic;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeCaptionFontDecorationEvt += _fli{_item.BindedProperty}.ChangeCaptionFontDecoration;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Container.ChangeSizeEvt += _fli{_item.BindedProperty}.ChangeFontSize;
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}";
        }

        /// <summary>
        /// Referencia de la alerta del input.
        /// </summary>
        /// <returns></returns>
        private string BuildRefAlert()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia de la alerta del item.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoInputAlert? _wia{_item.BindedProperty};

{_identMethodsAndProperties}private void {_item.BindedProperty}SetAlert(string alert) => _modelControls.{_item.BindedProperty}Alert.AgregarAlerta(alert);
";
        }

        /// <summary>
        /// Suscribe los controladores de eventos de la vista a los eventos del componente a través de la referencia.
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatusAlert()
        {
            return $@"
 {_identMethodsAndProperties}/// <summary>
 {_identMethodsAndProperties}/// Suscribe los controladores de eventos de la vista a los eventos de la alerta a través de la referencia.
 {_identMethodsAndProperties}/// </summary>
 {_identMethodsAndProperties}public void SetStatusWia{_item.BindedProperty}()
 {_identMethodsAndProperties}{{
 {_identMethodsAndProperties}    if (_wia{_item.BindedProperty} != null)
 {_identMethodsAndProperties}    {{
 {_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Alert.SetAlertValueEvt += _wia{_item.BindedProperty}.SetAlertValue;
 {_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Alert.ClearAlertsEvt += _wia{_item.BindedProperty}.ClearAlerts;
 {_identMethodsAndProperties}    }}
 {_identMethodsAndProperties}}}";
        }

        /// <summary>
        /// Referencia a los atributos con los valores necesarios para las propiedades del dto
        /// </summary>
        /// <returns></returns>
        private string BuildRefDto()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Atributo del cual se pueden recuperar los datos para el oData 
{_identMethodsAndProperties}/// y para definir las columnas del LookUp.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoLookUpConfigAttribute? _woLookUpConfig{_item.BindedProperty};";
        }

        /// <summary>
        /// Atributo con los valores necesarios para pasar las propiedades del OData.
        /// </summary>
        /// <returns></returns>
        private string BuildRefOData()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Atributo con los valores necesarios para pasar las propiedades del OData.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoODataModel? _woOData{_item.BindedProperty};";
        }

        /// <summary>
        /// Referencia a los atributos con los valores necesarios para las propiedades del data grid
        /// </summary>
        /// <returns></returns>
        private string BuildRefPropDto()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}///  Data
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoLookupDialogService<{_item.ClassModelType}, {_item.ClassModelType}Get, {_item.ClassModelType}List>? _{_classModelTypeMultiple}Service;
{_identMethodsAndProperties}private IEnumerable<{_item.ClassModelType}>? _{_classModelTypeMultiple}Col;
{_identMethodsAndProperties}private {_item.ClassModelType}? _{_classModelTypeMultiple}SelectedValue;";
        }

        /// <summary>
        /// Recuperación del decorado del modelo para llenar las variables
        /// </summary>
        /// <returns></returns>
        private string BuildComponentDecorated()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}///  Lista de tuplas con los datos recuperados desde el decorado de la propiedad para definir las columnas del LookUp.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties} private List<WoGridColumn> _listColumns{_item.BindedProperty} = new List<WoGridColumn>();
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}///  Nombre de la propiedad que se pintara en el campo principal del LookUp.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string _keyField{_item.BindedProperty} = string.Empty;
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}///  Nombre de la propiedad que se pintara en el campo secundario del LookUp.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string _descriptionField{_item.BindedProperty} = string.Empty;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Se ocupa de recuperar la información requerida por las propiedades del LookUp para realizar la petición oData e inicializar el componente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void {_shortComponent}{_item.BindedProperty}_OnInitialize()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _listColumns{_item.BindedProperty}.Clear();
{_identMethodsAndProperties}        StringBuilder oDataSelect = new StringBuilder();
{_identMethodsAndProperties}
{_identMethodsAndProperties}        _woLookUpConfig{_item.BindedProperty} = typeof({_item.ClassModelType})
{_identMethodsAndProperties}            .GetCustomAttributes(typeof(WoLookUpConfigAttribute), false)
{_identMethodsAndProperties}            .Cast<WoLookUpConfigAttribute>()
{_identMethodsAndProperties}            .FirstOrDefault();

{BuildListColumns()}

{_identMethodsAndProperties}
{_identMethodsAndProperties}        if (_woLookUpConfig{_item.BindedProperty} != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _keyField{_item.BindedProperty} = _woLookUpConfig{_item.BindedProperty}.KeyField;
{_identMethodsAndProperties}            _descriptionField{_item.BindedProperty} = _woLookUpConfig{_item.BindedProperty}.DescriptionField;
{_identMethodsAndProperties}
{_identMethodsAndProperties}            _woOData{_item.BindedProperty} = new WoODataModel()
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                Select = oDataSelect.ToString().Substring(0, oDataSelect.ToString().Length - 1),
{_identMethodsAndProperties}                Filter = _woLookUpConfig{_item.BindedProperty}.Filter,
{_identMethodsAndProperties}                OrderBy = _woLookUpConfig{_item.BindedProperty}.OrderBy
{_identMethodsAndProperties}            }};
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}";
        }

        private WoGridProperties _grid = new WoGridProperties();

        private void Grid()
        {
            _grid = new WoGridProperties();

            WoGridDesignerRawHelper woGridDesignerRawHelper = new WoGridDesignerRawHelper();
            _grid = woGridDesignerRawHelper.GetRawGrid(_item.ClassModelType, isSlave: false);
        }

        /// <summary>
        /// COnstrulle la lista de las columnas que se van a mostrar en la lista.
        /// </summary>
        /// <returns></returns>
        private string BuildListColumns()
        {
            StringBuilder strColumns = new StringBuilder();

            foreach (WoColumnProperties gridColumn in _grid.WoColumnPropertiesCol)
            {
                string label = string.Empty;
                if (gridColumn.Etiqueta == null)
                {
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    List<ModeloColumna> fullcolumnsCol = woProjectDataHelper.GetFullColumns(
                        _item.ClassModelType
                    );
                    //Modelo modelSelected = _project
                    //    .ModeloCol.Modelos.Where(x => x.Id == _item.ClassModelType)
                    //    .FirstOrDefault();
                    ModeloColumna columnSelected = fullcolumnsCol.FirstOrDefault(x =>
                        x.Id == gridColumn.Id
                    );
                    label = Etiqueta.ToId(columnSelected.Grid);
                }
                else
                {
                    label = gridColumn.Etiqueta;
                }

                string type = gridColumn.BindingType.Replace("?", "");

                strColumns.AppendLine(
                    $@"
{_identMethodsAndProperties}        _listColumns{_item.BindedProperty}.Add(
{_identMethodsAndProperties}            new WoGridColumn() 
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                PropertyName = ""{gridColumn.Id}"",
{_identMethodsAndProperties}                Label = ""{label}"",
{_identMethodsAndProperties}                PropertyType = typeof({type}),
{_identMethodsAndProperties}                ListVisible = {gridColumn .IsVisible.ToString() .ToLower()},
{_identMethodsAndProperties}                Width = ""{gridColumn.Size}{gridColumn.SizeType}"",
{_identMethodsAndProperties}                IsRef = {gridColumn.IsReference.ToString().ToLower()},
{_identMethodsAndProperties}                FormRoute = ""{gridColumn.UrlBaseReference}""
{_identMethodsAndProperties}            }});
{_identMethodsAndProperties}            oDataSelect.Append($@""{gridColumn.Id},"");
"
                );
            }

            return strColumns.ToString();
        }

        /// <summary>
        /// Cambio del elemento seleccionado
        /// </summary>
        /// <returns></returns>
        private string BuildOnclick()
        {
            return $@"
{_identMethodsAndProperties}public async Task {_item.BindedProperty}_OnChange(string newValue)
{_identMethodsAndProperties}{{

{_identMethodsAndProperties}    _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty} = newValue;
{_identMethodsAndProperties}    _scriptsUser.{_item.BindedProperty}_OnChange();
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        {_item.ClassModelType}? result = await _{_classModelTypeMultiple}Service?.GetModelById(newValue);
{_identMethodsAndProperties}        if (result != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            PropertyInfo? descriptionProperty = typeof({_item.ClassModelType}).GetProperty(_descriptionField{_item.BindedProperty});
{_identMethodsAndProperties}            _scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty} = descriptionProperty?.GetValue(result)?.ToString();
{_identMethodsAndProperties}
{_identMethodsAndProperties}            _modelControls.{_item.BindedProperty}Alert.LimpiarAlertas();
{_identMethodsAndProperties}            {_item.BaseModelName}Validate(""{_item.BindedProperty}"", _modelControls.{_item.BindedProperty}Alert);
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty} = string.Empty;
{_identMethodsAndProperties}            {_item.BaseModelName}Validate(""{_item.BindedProperty}"", _modelControls.{_item.BindedProperty}Alert);
{_identMethodsAndProperties}            _modelControls.{_item.BindedProperty}Alert.AgregarAlerta(""No se encontró el registro"");
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _scriptsUser.{_item.BaseModelName}.__{_item.BindedProperty} = string.Empty;
{_identMethodsAndProperties}        {_item.BaseModelName}Validate(""{_item.BindedProperty}"", _modelControls.{_item.BindedProperty}Alert);
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}Alert.AgregarAlerta(""No se encontró el registro"");
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}
";
        }

        /// <summary>
        /// Construye el apartado del código para la función de navegación de celdas del componente
        /// (Re dirección a otro formulario desde una celda)
        /// </summary>
        /// <returns></returns>
        private string BuildCellNavigation()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Re dirección de una celda a otro formulario
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private async void {_item.BindedProperty}_CellNavigation((string fieldName, object value, string route) redirect)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    string redirectRoute = $""{{(redirect.route == string.Empty ? ""/"" : redirect.route)}}?{{redirect.fieldName}}={{redirect.value}}"";

{_identMethodsAndProperties}    await JSRuntime.InvokeAsync<object>(""OpenInNewtab"", new object[2] {{ redirectRoute, ""_blank"" }});
{_identMethodsAndProperties}}}";
        }
        #endregion Construcción del código

        #region Logs
        /// <summary>
        /// Log del sistema
        /// </summary>
        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el código interno de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoLookUpDialogEditPartial",
                MethodOrContext = "GetLookUpDialogEditPartial"
            }
        };

        #endregion Logs
    }
}
