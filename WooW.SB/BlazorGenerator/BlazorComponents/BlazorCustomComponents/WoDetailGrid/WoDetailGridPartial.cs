using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoDetailGrid
{
    public class WoDetailGridPartial
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = new WoLogObserver();

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Codigo de la parcial del codigo.
        /// </summary>
        private StringBuilder _strPartialGrid = new StringBuilder();

        /// <summary>
        /// Instancia con la informacion del componente.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Identacion del componente de razor.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente.
        /// </summary>
        private string _shortComponent = "Grd";

        /// <summary>
        /// Prefijo del componente en minusculas.
        /// </summary>
        private string _lowShortComponent = "grd";

        /// <summary>
        /// Modelo del componente.
        /// </summary>
        private Modelo _model = new Modelo();

        /// <summary>
        /// Helper de los medelos.
        /// </summary>
        private WoToolModelsHelper _woToolModelsHelper = new WoToolModelsHelper();

        #endregion Atributos


        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="modelName"></param>
        public WoDetailGridPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoDetailGridPartial.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorCustomComponents\WoDetailGrid\WoDetailGridPartial.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 5-10-2023";
        }

        #endregion Tag

        #region Metodo principal

        /// <summary>
        /// Metodo principal que retorna la informacion de la parcial del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slaveName"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            //_model = _woToolModelsHelper.SearchModel(_item.ClassModelType);
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            _model = woProjectDataHelper.GetMainModel(_item.ClassModelType);

            _strPartialGrid.Clear();

            _strPartialGrid.AppendLine($@"{_identMethodsAndProperties}#region {item.Id}");

            _strPartialGrid.AppendLine(BuildTag());

            _strPartialGrid.AppendLine(BuildRefComponent());

            _strPartialGrid.AppendLine(BuildDetailColumns());

            _strPartialGrid.AppendLine(BuildSetStatus());

            _strPartialGrid.AppendLine(BuildOpenPopup());

            _strPartialGrid.AppendLine(BuildNewDetail());

            _strPartialGrid.AppendLine(BuildOpenEditPopup());

            _strPartialGrid.AppendLine(BuildDeleteDetail());

            _strPartialGrid.AppendLine($@"{_identMethodsAndProperties}#endregion {item.Id}");

            _partialReady.Details = $@"Se creo el código parcial para el componente: {item.Id}.";
            _observer.SetLog(_partialReady);

            return _strPartialGrid.ToString();
        }

        #endregion Metodo principal



        #region Contrucción del codigo

        #region Referencia

        /// <summary>
        /// Referencia de la grid.
        /// </summary>
        /// <returns></returns>
        private string BuildRefComponent()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Referencia del componente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoDetailGrid<{_item.ClassModelType}, App>? _{_lowShortComponent}{_item.BindedProperty};";
        }

        #endregion Referencia

        #region Columnas

        /// <summary>
        /// Construlle la lista con las columnas de la grid.
        /// </summary>
        /// <returns></returns>
        private string BuildDetailColumns()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Columnas de la grid del detalle.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private IEnumerable<(string Field, string Label)> _detailColumnsCol{_item.BindedProperty} = new List<(string Field, string Label)>()
{_identMethodsAndProperties}{{
{BuildColumns()}   
{_identMethodsAndProperties}}};
";
        }

        /// <summary>
        /// Construlle las columnas de la grid.
        /// </summary>
        /// <returns></returns>
        private string BuildColumns()
        {
            StringBuilder columnas = new StringBuilder();
            //string gridPath = $@"{_project.DirLayOuts}\Grids\{_model.Id}GridList.json";
            //WoGridProperties woGrid = new WoGridProperties();
            //if (File.Exists(gridPath))
            //{
            //    woGrid = JsonConvert.DeserializeObject<WoGridProperties>(
            //        File.ReadAllText(gridPath)
            //    );
            //}
            //else
            //{
            //    WoGridDesignerRawHelper woGridDesignerRawHelper = new WoGridDesignerRawHelper();
            //    woGrid = woGridDesignerRawHelper.GetRawGrid(_model.Id, isSlave: true);
            //}

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            WoGridProperties woGrid = woProjectDataHelper.GetGridFullDesign(_model.Id);

            foreach (WoColumnProperties column in woGrid.WoColumnPropertiesCol)
            {
                if (column.IsVisible)
                {
                    columnas.AppendLine(
                        $@"{_identMethodsAndProperties}    (Field: nameof({_item.ClassModelType}.{column.Id}), Label: ""{column.Etiqueta}""),"
                    );
                }
            }

            return columnas.ToString();
        }

        #endregion Columnas

        #region SetStatus

        /// <summary>
        /// Construlle el metodo que se encarga de actualizar el estado de la grid.
        /// </summary>
        /// <returns></returns>
        private string BuildSetStatus()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Suscrive los controladores de eventos de la vista del componente a los 
{_identMethodsAndProperties}/// eventos del componente a través de la referencia.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""status""></param>
{_identMethodsAndProperties}public void SetStatus{_shortComponent}{_item.BindedProperty}()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_{_lowShortComponent}{_item.BindedProperty} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeEditionModeEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeEditionMode;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeHeaderFontColorEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeHeaderFontColor;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeHeaderFontSizeEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeHeaderFontSize;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeHeaderFontWeightEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeHeaderFontWeight;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeHeaderFontItalicEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeHeaderFontItalic;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeHeaderFontDecorationEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeHeaderFontDecoration;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeHeaderBackgroundColorEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeHeaderBackgroundColor;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeRowsFontColorEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeRowsFontColor;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeRowsFontSizeEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeRowsFontSize;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeRowsFontWeightEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeRowsFontWeight;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeRowsFontItalicEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeRowsFontItalic;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeRowsFontDecorationEvt += _{_lowShortComponent}{_item.BindedProperty}.ChangeRowsFontDecoration;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeRowsBackgroundColor1Evt += _{_lowShortComponent}{_item.BindedProperty}.ChangeRowsBackgroundColor1;
{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ChangeRowsBackgroundColor2Evt += _{_lowShortComponent}{_item.BindedProperty}.ChangeRowsBackgroundColor2;

{_identMethodsAndProperties}        _modelControls.{_item.BindedProperty}.ActualizarComponente();
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}
";
        }

        #endregion SetStatus

        #region Open popup

        /// <summary>
        /// Construlle el codigo que se encarga de abrir el popup.
        /// </summary>
        /// <returns></returns>
        private string BuildOpenPopup()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Abre el popup con el formulario para agregar un nuevo detalle.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void {_item.BindedProperty}OpenNewPopup()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    long renglon = 0;

{_identMethodsAndProperties} if (_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty} == null)
{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty} = new List<{_item.ClassModelType}>();
{_identMethodsAndProperties} }}
{_identMethodsAndProperties} if (_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}.Count > 0)
{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     renglon = _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}.Max(x => x.Renglon);
{_identMethodsAndProperties} }}

{_identMethodsAndProperties}    _{_item.ClassModelType}ScriptsUser.{_item.ClassModelType} = new {_item.ClassModelType}()
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        Renglon = renglon + 1
{_identMethodsAndProperties}    }};
{_identMethodsAndProperties}}}
";
        }

        #endregion Open popup

        #region New detail

        /// <summary>
        /// Construlle el metodo que salva un nuevo registro de detalle en la lista de detalles.
        /// </summary>
        /// <returns></returns>
        private string BuildNewDetail()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Metodo que salva el nuevo detalle con los datos ingresados en el formulario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""isNewDetail""></param>
{_identMethodsAndProperties}private void {_item.BindedProperty}_SaveNewDetail(bool isNewDetail)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (!isNewDetail)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        long renglon = _{_item.ClassModelType}ScriptsUser.{_item.ClassModelType}.Renglon;

{_identMethodsAndProperties}        int FoundIndex = _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}.ToList().FindIndex(x => x.Renglon == renglon);
{_identMethodsAndProperties}        if (FoundIndex != -1)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}[FoundIndex] = _{_item.ClassModelType}ScriptsUser.{_item.ClassModelType};
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    else
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}.Add(_{_item.ClassModelType}ScriptsUser.{_item.ClassModelType});
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}";
        }

        #endregion New detail

        #region Open poopup edición

        /// <summary>
        /// Construlle el codigo que se encarga de abrir el popup de edición.
        /// </summary>
        /// <returns></returns>
        private string BuildOpenEditPopup()
        {
            return $@"
{_identMethodsAndProperties}#region Abrir edición

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Metodo que se ocupa de abrri un metodo para editar el detalle.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""newValue""></param>
{_identMethodsAndProperties}private void {_item.BindedProperty}_OpenEditPopup({_item.ClassModelType} newValue)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _{_item.ClassModelType}ScriptsUser.{_item.ClassModelType} = newValue;
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Abrir edición
";
        }

        #endregion Open poopup edición

        #region Elimina el detalle

        /// <summary>
        /// Construlle el metodo para eliminar el detalle de la grid y de la lista.
        /// </summary>
        /// <returns></returns>
        private string BuildDeleteDetail()
        {
            return $@"
{_identMethodsAndProperties}#region Eliminar detalle

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Metodo que se ocupa de eliminar el detalle.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""DeletedItem""></param>
{_identMethodsAndProperties}private void {_item.BindedProperty}_DeleteDetail({_item.ClassModelType} DeletedItem)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}.Remove(DeletedItem);
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Eliminar detalle
";
        }

        #endregion Elimina el detalle

        #endregion Contrucción del codigo


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
