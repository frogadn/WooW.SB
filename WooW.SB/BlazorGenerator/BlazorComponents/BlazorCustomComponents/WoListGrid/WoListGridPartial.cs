using System.Linq;
using System.Text;
using WooW.Core;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoListGrid
{
    public class WoListGridPartial
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
        /// Identificación a nivel de la declaración de los métodos y las propiedades.
        /// </summary>
        private string _identMethodsAndProperties = "";

        /// <summary>
        /// Item del que se esta generando la parcial.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Modelo del grid que se esta generando.
        /// </summary>
        private Modelo _model;

        /// <summary>
        /// Grid que se va a generar.
        /// </summary>
        private WoGridProperties _grid = new WoGridProperties();

        #endregion Atributos

        #region Código en generación

        /// <summary>
        /// Código que se va generando para el componente.
        /// </summary>
        private StringBuilder _strComponentCode = new StringBuilder();

        #endregion Código en generación


        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        public WoListGridPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase


        #region Método principal

        public string GetCode(WoItem woItem, bool blazorIntegral = false)
        {
            _item = woItem;

            _model = _project.ModeloCol.Modelos.FirstOrDefault(model =>
                model.Id == _item.BaseModelName
            );

            Grid(blazorIntegral);

            _strComponentCode.Clear();

            _strComponentCode.AppendLine($@"{_identMethodsAndProperties}#region {_item.Id}");

            _strComponentCode.AppendLine(BuildTag());

            _strComponentCode.AppendLine(BuildToolBar());

            _strComponentCode.AppendLine(BuildColumns());

            _strComponentCode.AppendLine(BuildSelectionChanged());

            _strComponentCode.AppendLine(BuildCellNavigation());

            _strComponentCode.AppendLine(BuildAlerts());

            _strComponentCode.AppendLine($@"{_identMethodsAndProperties}#endregion {_item.Id}");

            _partialReady.Details = $@"Se creo el código parcial para el componente: {_item.Id}.";
            _observer.SetLog(_partialReady);

            return _strComponentCode.ToString();
        }

        private void Grid(bool blazorIntegral)
        {
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            _grid = woProjectDataHelper.GetGridFullDesign(_item.BaseModelName, blazorIntegral);
            //string pathJson =
            //    $@"{_project.DirLayOuts}\ListDesign\{_item.BaseModelName}GridList.json";
            //_grid = new WoGridProperties();

            //if (File.Exists(pathJson))
            //{
            //    string json = WoDirectory.ReadFile(pathJson);
            //    _grid = JsonConvert.DeserializeObject<WoGridProperties>(json);
            //}
            //else
            //{
            //    WoGridDesignerRawHelper woGridDesignerRawHelper = new WoGridDesignerRawHelper();
            //    _grid = woGridDesignerRawHelper.BuildRawActualModelo(
            //        _item.BaseModelName,
            //        writeFile: true
            //    );
            //}
        }

        #endregion Método principal


        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoListGridPartial.cs.";
        }

        #endregion Tag

        #region Generación de los métodos de la ToolBar

        /// <summary>
        /// Construcción del codigo de la toolbar
        /// </summary>
        /// <returns></returns>
        private string BuildToolBar()
        {
            StringBuilder strGridCode = new StringBuilder();

            //Modelo model = _project
            //    .ModeloCol.Modelos.Where(m => m.Id == _item.BaseModelName)
            //    .FirstOrDefault();
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            Modelo model = woProjectDataHelper.GetMainModel(_item.BaseModelName);

            string id = (model.TipoModelo == Core.WoTypeModel.View) ? $@"{model.Id}Id" : "Id";
            id = id.Replace("View", "");

            strGridCode.AppendLine($@"{_identMethodsAndProperties}#region Toolbar");

            strGridCode.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia del grid
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoODataGrid<{_item.BaseModelName}, App> _{_item.Id}ListGrid;"
            );

            strGridCode.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Valor seleccionado.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private {_item.BaseModelName}? _{_item.Id}selectedvalue;"
            );

            strGridCode.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Ruta del formulario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string _{_item.Id}FormRoute => ""{model.ProcesoId}/{model.TipoModelo}/{_item.BaseModelName}"";"
            );

            if (model.TipoModelo != WoTypeModel.View)
            {
                strGridCode.AppendLine(
                    $@"
{_identMethodsAndProperties}private async Task {_item.Id}_RedirectTo()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    string value = _{_item.Id}selectedvalue.{id}.ToString();
{_identMethodsAndProperties}    if (!string.IsNullOrEmpty(value))
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        string route = (_{_item.Id}FormRoute == string.Empty) ? ""/"" : $@""{{_{_item.Id}FormRoute}}?Id={{value}}"";
{_identMethodsAndProperties}        //await JSRuntime.InvokeAsync<object>(""OpenInNewtab"", new object[2] {{ route, ""_blank"" }});
{_identMethodsAndProperties}        NavigationManager?.NavigateTo(route);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    else
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        string route = (_{_item.Id}FormRoute == string.Empty) ? ""/"" : $@""{{_{_item.Id}FormRoute}}"";
{_identMethodsAndProperties}        //await JSRuntime.InvokeAsync<object>(""OpenInNewtab"", new object[2] {{ route, ""_blank"" }});
{_identMethodsAndProperties}        NavigationManager?.NavigateTo(route);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}"
                );
            }

            strGridCode.AppendLine($@"{_identMethodsAndProperties}#endregion Toolbar");

            return strGridCode.ToString();
        }

        #endregion Generación de los métodos de la ToolBar

        #region Generación de columns

        /// <summary>
        /// Recuperación de las columnas en los meta datos del modelo.
        /// </summary>
        /// <returns></returns>
        private string BuildColumns()
        {
            return $@"
{_identMethodsAndProperties}#region Columnas

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Lista de las columnas que se ban a mostrar por default.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private List<WoGridColumn> _{_item.Id}Columns = new List<WoGridColumn>();

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Recupera por reflexión las columnas que se van a mostrar en la lista.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void {_item.Id}_GetColumns()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _{_item.Id}Columns.Clear();

{BuildListColumns()}

{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Columnas
";
        }

        /// <summary>
        /// COnstrulle la lista de las columnas que se van a mostrar en la lista.
        /// </summary>
        /// <returns></returns>
        private string BuildListColumns()
        {
            StringBuilder strColumns = new StringBuilder();

            foreach (
                WoColumnProperties gridColumn in _grid.WoColumnPropertiesCol.OrderByDescending(x =>
                    x.IsVisible
                )
            )
            {
                string label = string.Empty;
                if (gridColumn.Etiqueta == null)
                {
                    Modelo modelSelected = _project
                        .ModeloCol.Modelos.Where(x => x.Id == _item.BaseModelName)
                        .FirstOrDefault();
                    ModeloColumna columnSelected = modelSelected
                        .Columnas.Where(x => x.Id == gridColumn.Id)
                        .FirstOrDefault();

                    // FChio si no existe la columna fue porque se elimino del modelo.
                    if (columnSelected.IsNull())
                        continue;

                    label = Etiqueta.ToId(columnSelected.Grid);
                }
                else
                {
                    label = gridColumn.Etiqueta;
                }

                string type = gridColumn.BindingType.Replace("?", "");

                type = (type.ToLower().Contains("complex")) ? "string" : type;

                if (gridColumn.IsCustomColumn)
                {
                    strColumns.AppendLine(
                        $@"
{_identMethodsAndProperties}        _{_item.Id}Columns.Add(
{_identMethodsAndProperties}            new WoGridColumn() 
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                PropertyName = ""{gridColumn.Id}"",
{_identMethodsAndProperties}                Label = ""{label}"",
{_identMethodsAndProperties}                PropertyType = typeof({type}),
{_identMethodsAndProperties}                ListVisible = {gridColumn .IsVisible.ToString() .ToLower()},
{_identMethodsAndProperties}                Width = ""{gridColumn.Size}{gridColumn.SizeType}"",
{_identMethodsAndProperties}                IsRef = {gridColumn.IsReference.ToString().ToLower()},
{_identMethodsAndProperties}                FormRoute = ""{gridColumn.UrlBaseReference}"",
{_identMethodsAndProperties}                UnboundType = eGridUnboundColumnType.String,
{_identMethodsAndProperties}                UnboundExpression = ""{gridColumn.Data}""
{_identMethodsAndProperties}            }});"
                    );
                }
                else
                {
                    strColumns.AppendLine(
                        $@"
{_identMethodsAndProperties}        _{_item.Id}Columns.Add(
{_identMethodsAndProperties}            new WoGridColumn() 
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                PropertyName = ""{gridColumn.Id}"",
{_identMethodsAndProperties}                Label = ""{label}"",
{_identMethodsAndProperties}                PropertyType = typeof({type}),
{_identMethodsAndProperties}                ListVisible = {gridColumn .IsVisible.ToString() .ToLower()},
{_identMethodsAndProperties}                Width = ""{gridColumn.Size}{gridColumn.SizeType}"",
{_identMethodsAndProperties}                IsRef = {gridColumn.IsReference.ToString().ToLower()},
{_identMethodsAndProperties}                FormRoute = ""{gridColumn.UrlBaseReference}""
{_identMethodsAndProperties}            }});"
                    );
                }
            }

            return strColumns.ToString();
        }

        #endregion Generación de columns

        #region Generación del cambio de selección

        /// <summary>
        /// Construye el método que se detona cuando se cambia la selección de la lista.
        /// </summary>
        /// <returns></returns>
        private string BuildSelectionChanged()
        {
            return $@"
{_identMethodsAndProperties}#region Selección de la data

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Modelo seleccionado en la lista.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""selectedModel""></param>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private void {_item.Id}_SelectedRowChanged({_item.BaseModelName} selectedModel)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _{_item.Id}selectedvalue = selectedModel;
{_identMethodsAndProperties}    _scriptsUser.OnSelectedRowChanged(selectedModel);
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Selección de la data
";
        }

        #endregion Generación del cambio de selección

        #region Navegación por celda

        /// <summary>
        /// Construye el método que se detona cuando se cambia la selección de la lista.
        /// </summary>
        /// <returns></returns>
        private string BuildCellNavigation()
        {
            return $@"
{_identMethodsAndProperties}#region Navegación por celdas

{_identMethodsAndProperties}private async void {_item.Id}_CellNavigation((string fieldName, object value, string route) redirect)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    string redirectRoute = $""{{(redirect.route == string.Empty ? ""/"" : redirect.route)}}?{{redirect.fieldName}}={{redirect.value}}"";

{_identMethodsAndProperties}    await JSRuntime.InvokeAsync<object>(""OpenInNewtab"", new object[2] {{ redirectRoute, ""_blank"" }});
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Navegación por celdas
";
        }

        #endregion Navegación por celda

        #region Alertas

        private string BuildAlerts()
        {
            return $@"
{_identMethodsAndProperties}#region Alertas de la grid

{_identMethodsAndProperties}private void {_item.Id}_SetGridAlert(string message) 
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_alerts != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _alerts.AddAlert(_{_item.Id}ListGrid, new WoFormAlertModel(message));
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Alertas de la grid";
        }

        #endregion Alertas


        #region Logs

        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el código interno de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoTextEditPartial",
                MethodOrContext = "GetTextEditPartial"
            }
        };

        #endregion Logs
    }
}
