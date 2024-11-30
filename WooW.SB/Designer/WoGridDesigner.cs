using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using ServiceStack.Text;
using Svg;
using WooW.Core;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.Helpers.GeneratorsHelpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer
{
    public partial class WoGridDesigner : UserControl
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia del proyecto principal
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Atributos principales

        /// <summary>
        /// Indica el nombre del modelo, sobre del que se esta trabajando.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Indica la ruta donde se guardara el json de la grid.
        /// </summary>
        private string _saveFolder = string.Empty;

        /// <summary>
        /// Se inicializa en el constructor.
        /// Es la ruta del archivo json del layout que diseña el usuario.
        /// </summary>
        private string _pathGridSave = string.Empty;

        /// <summary>
        /// Se inicializa en el constructor.
        /// Es la instancia base de la grid que se usara como diseñador de la esclava para el blazor.
        /// </summary>
        private GridView _gridView;

        /// <summary>
        /// Modelo seleccionado.
        /// </summary>
        private Modelo _selectedModel;

        /// <summary>
        /// Extensiones del modelo seleccionado
        /// </summary>
        private List<Modelo> _extensions = new List<Modelo>();

        /// <summary>
        /// Instancia del diseño de la grid.
        /// </summary>
        private WoGridProperties _gridProperties;

        /// <summary>
        /// Instancia del helper para pasar los tipos de diseñador a tipos de código.
        /// </summary>
        private WoDesignerTypeHelper _woDesignerTypeHelper;

        #endregion Atributos principales


        #region Constructores

        /// <summary>
        /// Constructor principal del diseñador de esclavas.
        /// Funciones:
        ///     - Inicializar las variables globales a través de los parámetros.
        ///     - Inicializa el path donde se guarda el json.
        /// </summary>
        /// <param name="saveFolder"></param>
        /// <param name="modelName"></param>
        [SupportedOSPlatform("windows")]
        public WoGridDesigner(string saveFolder, string modelName)
        {
            try
            {
                InitializeComponent();

                Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == modelName
                );

                _iconsBasePath = $@"{_project.DirLayOuts}\BlazorLibraries\bootstrap-icons\icons";

                _modelName = modelName;
                _saveFolder = saveFolder;

                _woDesignerTypeHelper = new WoDesignerTypeHelper(_modelName);

                _pathGridSave =
                    $@"{_project.DirProyectData}/LayOuts/{_saveFolder}/{_modelName}GridList.json";
                BuildRawDesign();
                SearchModel();

                SetModelSettings();

                InitializeGrid();

                BlockMode();

                ChargeButtonList();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al inicializar el diseñador de la grid. {ex.Message}");
            }
        }

        #endregion Constructores


        #region Build raw design

        /// <summary>
        /// Construye el diseño de la grid en raw igual lo serializa y salva.
        /// </summary>
        private void BuildRawDesign()
        {
            try
            {
                if (!File.Exists(_pathGridSave))
                {
                    WoGridDesignerRawHelper rawHelper = new WoGridDesignerRawHelper();
                    WoGridProperties rawDesign = rawHelper.GetRawGrid(_modelName, isSlave: true);
                    string json = JsonConvert.SerializeObject(rawDesign);
                    WoDirectory.WriteFile(_pathGridSave, json);
                }
                else
                {
                    UpdateDesign();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al construir el diseño. {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza el diseño de la grid
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void UpdateDesign()
        {
            try
            {
                // Recuperamos la lista del modelo y sus extensiones
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                List<Modelo> fullModelCol = woProjectDataHelper.GetModelWithExtencions(_modelName);

                // Recuperamos el diseño actual del grid
                string json = WoDirectory.ReadFile(_pathGridSave);
                WoGridProperties jsonDesign = JsonConvert.DeserializeObject<WoGridProperties>(json);

                List<ModeloColumna> fullColumns = new List<ModeloColumna>();
                foreach (Modelo model in fullModelCol)
                {
                    fullColumns.AddRange(model.Columnas);
                }

                jsonDesign = AddColumns(fullColumns, jsonDesign);

                jsonDesign = RemoveColumns(fullColumns, jsonDesign);

                string newJson = JsonConvert.SerializeObject(jsonDesign);
                WoDirectory.WriteFile(_pathGridSave, newJson);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al actualizar el diseño de la grid. {ex.Message}");
            }
        }

        /// <summary>
        /// Agrega las columnas que falten
        /// </summary>
        /// <exception cref="Exception"></exception>
        private WoGridProperties AddColumns(
            List<ModeloColumna> fullColumns,
            WoGridProperties jsonDesing
        )
        {
            try
            {
                foreach (ModeloColumna column in fullColumns)
                {
                    WoColumnProperties findColumn = jsonDesing.WoColumnPropertiesCol.FirstOrDefault(
                        col => col.Id == column.Id
                    );

                    if (findColumn != null)
                    {
                        string etiqueta = Etiqueta.ToId(column.Grid);

                        string columnType = _woDesignerTypeHelper.DesignerTypeToCodeType(
                            designerType: column.TipoColumna.ToString().ToLower(),
                            isNullable: column.Nulo,
                            attributeName: column.Id
                        );

                        bool isReference = (column.TipoColumna == WoTypeColumn.Reference);

                        string urlBaseReference = null;

                        WoColumnProperties woColumnProperties = new WoColumnProperties()
                        {
                            Id = column.Id,
                            Etiqueta = etiqueta,
                            MaskText = column.Grid,
                            ModelName = _modelName,
                            IsSlave = jsonDesing.IsSlave,
                            Process = jsonDesing.Proceso,
                            Size = 100,
                            SizeType = "px",
                            Index = jsonDesing.WoColumnPropertiesCol.Count + 1,
                            IsVisible = true,
                            BindingType = columnType,
                            IsReference = isReference,
                            UrlBaseReference = urlBaseReference,
                            Control = column.TipoControl,
                        };

                        if (isReference)
                        {
                            string referenceModelId = column.ModeloId;

                            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                            Modelo referenceModel = woProjectDataHelper.GetMainModel(
                                referenceModelId
                            );

                            if (referenceModel != null)
                            {
                                woColumnProperties.UrlBaseReference =
                                    $@"{referenceModel.ProcesoId}/{referenceModel.TipoModelo}/{referenceModelId}";
                                woColumnProperties.TypeModel = referenceModel.TipoModelo;
                                woColumnProperties.SubTypeModel = referenceModel.SubTipoModelo;

                                if (column.TipoControl == WoTypeControl.Urn)
                                {
                                    woColumnProperties.IsReference = true;
                                }

                                if (
                                    jsonDesing.WoColumnPropertiesCol.FirstOrDefault(col =>
                                        col.Id == woColumnProperties.Id
                                    ) == null
                                )
                                {
                                    jsonDesing.WoColumnPropertiesCol.Add(woColumnProperties);
                                }
                            }
                            else
                            {
                                if (column.TipoControl == WoTypeControl.Urn)
                                {
                                    woColumnProperties.IsReference = true;
                                }

                                if (
                                    jsonDesing.WoColumnPropertiesCol.FirstOrDefault(col =>
                                        col.Id == woColumnProperties.Id
                                    ) == null
                                )
                                {
                                    jsonDesing.WoColumnPropertiesCol.Add(woColumnProperties);
                                }
                            }
                        }
                    }
                }

                return jsonDesing;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al agregar las columnas. {ex.Message}");
            }
        }

        /// <summary>
        /// Remueve las columnas que ya no pertenecen al modelo
        /// </summary>
        /// <returns></returns>
        private WoGridProperties RemoveColumns(
            List<ModeloColumna> fullColumns,
            WoGridProperties jsonDesing
        )
        {
            try
            {
                List<WoColumnProperties> newColumnProperties = new List<WoColumnProperties>();

                foreach (WoColumnProperties columnProperties in jsonDesing.WoColumnPropertiesCol)
                {
                    ModeloColumna column = fullColumns.FirstOrDefault(col =>
                        col.Id == columnProperties.Id
                    );

                    if (column != null && (column.TipoControl != WoTypeControl.CollectionEditor))
                    {
                        if (columnProperties.IsCustomLabel)
                        {
                            columnProperties.Etiqueta = columnProperties.Etiqueta;
                            columnProperties.MaskText = columnProperties.MaskText;
                        }
                        else
                        {
                            columnProperties.Etiqueta = Etiqueta.ToId(column.Grid);
                            columnProperties.MaskText = column.Grid;
                        }
                        newColumnProperties.Add(columnProperties);
                    }
                    else if (columnProperties.IsCustomColumn)
                    {
                        newColumnProperties.Add(columnProperties);
                    }
                }

                jsonDesing.WoColumnPropertiesCol.Clear();
                jsonDesing.WoColumnPropertiesCol = newColumnProperties;

                return jsonDesing;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al remover las columnas. {ex.Message}");
            }
        }

        #endregion Build raw design

        #region Modelo

        /// <summary>
        /// Busca el modelo seleccionado en el proyecto.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SearchModel()
        {
            try
            {
                //_selectedModel = _project.ModeloCol.Modelos.Find(x => x.Id == _modelName);
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                _selectedModel = woProjectDataHelper.GetMainModel(_modelName);
                _extensions = woProjectDataHelper.GetExtensions(_modelName);

                if (_selectedModel == null)
                {
                    throw new Exception(
                        $@"El modelo {_modelName} que esta intentando buscar no existe en WoGridDesigner"
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// En función de las propiedades del modelo encontrado se realizar configuraciones al
        /// diseñador
        /// </summary>
        private void SetModelSettings()
        {
            try
            {
                if (_selectedModel.TipoModelo == WoTypeModel.View)
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        props: new WoGridProperties(),
                        properties: new List<string>() { "GridSelect", },
                        value: true
                    );

                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        props: new WoColumnProperties(),
                        properties: new List<string>() { "IsId", },
                        value: true
                    );
                }
                else
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        props: new WoGridProperties(),
                        properties: new List<string>() { "GridSelect", },
                        value: false
                    );

                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        props: new WoColumnProperties(),
                        properties: new List<string>() { "IsId", },
                        value: false
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al asignar las settings del modelo. {ex.Message}");
            }
        }

        #endregion Modelo


        #region Inicialización de la grid

        /// <summary>
        /// Instancia principal de la grid base.
        /// (Es una abstracción de la grid, la mayoría de funciones se controlan desde GridView).
        /// </summary>
        private GridControl _gridControl;

        /// <summary>
        /// Método principal que se ocupa de cargar la grid en el panel.
        /// Se puede usar para restaurar la grid.
        ///     - Limpia y re inicializa las variables de la grid así como el panel.
        ///     - Crea un DataTable para cargar el modelo en la grid.
        ///     - Obtiene el modelo en función de si existe o no el json.
        ///     - Carga la grid en el panel base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void InitializeGrid()
        {
            pnlGridBase.Controls.Clear();
            _gridControl = null;
            _gridView = null;

            _gridControl = new GridControl();

            DataTable tbModel = new DataTable();
            tbModel = ChargeGridRaw();

            _gridControl.Parent = pnlGridBase;
            _gridControl.Dock = DockStyle.Fill;
            _gridControl.DataSource = tbModel;
            _gridControl.Load += ChargeGridView;
        }

        /// <summary>
        /// Se detona cuando la grid ya esta completamente cargada en pantalla.
        ///     - Valida que la GridView sea nula.
        ///     - Pasa la vista de la GridControl a la GridView.
        ///     - Suscribe un método para cuando se cierra la ventana del editor.
        ///     - Carga las columnas ocultas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeGridView(object sender, System.EventArgs e)
        {
            if (_gridView.IsNull())
            {
                _gridView = _gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                _gridView.HideInHeaderSearchTextBox();
                _gridView.HideCustomizationForm += ExitCustomization;
                _gridView.FocusedColumnChanged += SelecctionCahnged;
                _gridView.OptionsView.ShowGroupPanel = false;
                _gridView.OptionsView.ShowAutoFilterRow = false;
                _gridView.OptionsBehavior.Editable = false;
                _gridView.ColumnPositionChanged += (s, e) =>
                {
                    if (_modeDesigner == eModeDesigner.Design)
                    {
                        GridUpdatingEvt?.Invoke();
                    }
                };
            }

            if (File.Exists(_pathGridSave))
            {
                ChargeHideColumns();
            }

            foreach (GridColumn column in _gridView.Columns)
            {
                column.MinWidth = 300;
            }
        }

        /// <summary>
        /// En caso de que se cierre la ventana del editor en el modo
        /// de diseño, cambia de modo al de bloqueo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ExitCustomization(object sender, System.EventArgs e)
        {
            BlockMode();
        }

        #endregion Inicialización de la grid


        #region Carga de la grid en raw

        /// <summary>
        /// SE CARGA LA GRID CON TODOS LOS CAMPOS EN RAW Y LUEGO SE OCULTAN LOS QUE NO VAN
        /// UNA VEZ QUE EXISTE LA VISTA DE LA GRID.
        /// Carga los campo de la grid en raw:
        ///     - Usa un DataTable para usar como modelo que se le pasara a la grid.
        ///     - Retira el id de los campos que se agregan.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private DataTable ChargeGridRaw()
        {
            try
            {
                DataTable tbModel = new DataTable();

                if (File.Exists(_pathGridSave))
                {
                    string json = WoDirectory.ReadFile(_pathGridSave);
                    _gridProperties = JsonConvert.DeserializeObject<WoGridProperties>(json);

                    foreach (
                        WoColumnProperties columnProperties in _gridProperties.WoColumnPropertiesCol
                    )
                    {
                        ModeloColumna column = _selectedModel.Columnas.Find(x =>
                            x.Id == columnProperties.Id
                        );

                        if (column == null)
                        {
                            foreach (Modelo model in _extensions)
                            {
                                column = model.Columnas.Find(x => x.Id == columnProperties.Id);
                                if (column != null)
                                {
                                    break;
                                }
                            }
                        }

                        //var column = _selectedModel.Columnas.Find(x => x.Id == columnProperties.Id);
                        if (column != null)
                        {
                            tbModel.Columns.Add(columnProperties.Id, typeof(string));
                        }
                        else if (columnProperties.IsCustomColumn)
                        {
                            tbModel.Columns.Add(columnProperties.Id, typeof(string));
                        }
                    }

                    DataRow row = tbModel.NewRow();
                    foreach (
                        WoColumnProperties columnProperties in _gridProperties.WoColumnPropertiesCol
                    )
                    {
                        if (columnProperties.IsCustomLabel)
                        {
                            row[columnProperties.Id] = columnProperties.MaskText;
                        }
                        else
                        {
                            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                            List<ModeloColumna> fullColumns = woProjectDataHelper.GetFullColumns(
                                _modelName
                            );

                            ModeloColumna column = fullColumns.FirstOrDefault(x =>
                                x.Id == columnProperties.Id
                            );
                            if (column != null)
                            {
                                row[columnProperties.Id] = column.Grid;
                            }
                            else
                            {
                                throw new Exception("Columna no encontrada");
                            }
                        }
                    }
                    tbModel.Rows.Add(row);
                }
                else
                {
                    tbModel = BuildColumnsWhitExtencions();
                }

                return tbModel;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al cargar la grid en raw. {ex.Message}");
            }
        }

        #endregion Carga de la grid en raw

        #region Cargar la grid cuando estamos en el modelo base

        /// <summary>
        /// Construimos las columnas para cuando estamos en el modelo principal
        /// </summary>
        //private DataTable BuildColumns()
        //{
        //    try
        //    {
        //        DataTable tbModel = new DataTable();

        //        foreach (var column in _selectedModel.Columnas)
        //        {
        //            tbModel.Columns.Add(column.Id, typeof(string));
        //        }

        //        DataRow row = tbModel.NewRow();
        //        foreach (var column in _selectedModel.Columnas)
        //        {
        //            row[column.Id] = column.Grid;
        //        }
        //        tbModel.Rows.Add(row);

        //        return tbModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($@"Error en la creación de las columnas. {ex.Message}");
        //    }
        //}

        #endregion Cargar la grid cuando estamos en el modelo base

        #region Cargar la grid con exenciones

        /// <summary>
        /// Construimos las columnas con el uso de exenciones sobre del modelo principal
        /// </summary>
        /// <returns></returns>
        private DataTable BuildColumnsWhitExtencions()
        {
            try
            {
                DataTable tbModel = new DataTable();

                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

                List<Modelo> models = woProjectDataHelper.GetModelWithExtencions(_selectedModel.Id);
                foreach (Modelo model in models)
                {
                    foreach (var column in model.Columnas)
                    {
                        tbModel.Columns.Add(column.Id, typeof(string));
                    }

                    DataRow row = tbModel.NewRow();
                    foreach (var column in _selectedModel.Columnas)
                    {
                        row[column.Id] = column.Grid;
                    }
                    tbModel.Rows.Add(row);
                }

                return tbModel;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al construir la grid con las extensiones. {ex.Message}"
                );
            }
        }

        #endregion Cargar la grid con exenciones


        #region Modos del designer

        /// <summary>
        /// Guarda el estado en el que se encuentra la grid.
        /// </summary>
        private eModeDesigner _modeDesigner = eModeDesigner.Block;

        /// <summary>
        /// Controlador de eventos que se detonara cuando se cambie el modo del formulario.
        /// </summary>
        public event EventHandler<eModeDesigner> ModeChanged;

        /// <summary>
        /// Cambia la grid a modo de diseño.
        ///     - habilita el panel sobre el que se encuentra la grid.
        ///     - Pone la grid en modo de diseño.
        ///     - Cambia el valor de la variable y detona el controlador de eventos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void DesingMode()
        {
            pnlGridBase.Enabled = true;
            pnlButtons.Enabled = true;
            _gridView.ShowCustomization();

            _modeDesigner = eModeDesigner.Design;

            sccPanelDesigner.PanelVisibility = SplitPanelVisibility.Both;

            ModeChanged?.Invoke(this, _modeDesigner);
        }

        /// <summary>
        /// Cambia la grid a modo de vista o bloqueado.
        ///     - Deshabilita el panel de la grid.
        ///     - Oculta el panel de edición.
        ///     - Cambia el valor de la variable e invoca el controlador de eventos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BlockMode()
        {
            pnlGridBase.Enabled = false;
            pnlButtons.Enabled = false;

            sccPanelDesigner.PanelVisibility = SplitPanelVisibility.Panel1;

            if (_gridView != null)
            {
                _gridView.HideCustomization();
                _modeDesigner = eModeDesigner.Block;
                ModeChanged?.Invoke(this, _modeDesigner);
            }
        }

        #endregion Modos del designer


        #region Selection de una columna

        /// <summary>
        /// Instancia que permite modificar la visibilidad de la propiedades de la clase.
        /// </summary>
        private WoColumnProperties _propertiesFromChange = new WoColumnProperties();

        /// <summary>
        /// Columna seleccionada.
        /// </summary>
        private DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs _focusedColumn;

        /// <summary>
        /// Método para cuando se cambia la selección de la columna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        [SupportedOSPlatform("windows")]
        private void SelecctionCahnged(object sender, EventArgs args)
        {
            _focusedColumn = (DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs)args;

            if (_focusedColumn != null && _focusedColumn.FocusedColumn != null)
            {
                WoColumnProperties columnProperties = (WoColumnProperties)
                    _focusedColumn.FocusedColumn.Tag;

                if (columnProperties.IsCustomColumn)
                {
                    SetComponentProperties(
                        properties: new List<string>() { "Data" },
                        browsable: true
                    );

                    btnDeleteButton.Enabled = false;
                    btnDeleteColumn.Enabled = true;
                }
                else
                {
                    SetComponentProperties(
                        properties: new List<string>() { "Data" },
                        browsable: false
                    );

                    btnDeleteButton.Enabled = false;
                    btnDeleteColumn.Enabled = false;
                }

                grdColumnProperties.Update();

                grdColumnProperties.SelectedObject = columnProperties;

                grdColumnProperties.Update();
            }
        }

        /// <summary>
        /// Método que modifica la visibilidad de las propiedades de la clase.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="browsable"></param>
        [SupportedOSPlatform("windows")]
        private void SetComponentProperties(List<string> properties, bool browsable)
        {
            HidePropertiesHelper.ModifyBrowsableAttribute(
                props: _propertiesFromChange,
                properties: properties,
                value: browsable
            );
        }

        #endregion Selection de una columna


        #region De serializar Json a grid

        /// <summary>
        /// Carga las columnas que se retiraron del diseño y aplica las medidas de columnas del json.
        ///     - Recorre todas las columnas y verifica en el json si son visibles o no.
        ///     - En caso de si ser visibles, le aplica la medida del json.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeHideColumns()
        {
            string json = WoDirectory.ReadFile(_pathGridSave);
            _gridProperties = JsonConvert.DeserializeObject<WoGridProperties>(json);

            int index = 0;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsBehavior.ReadOnly = true;
            _gridView.ActiveFilterEnabled = false;
            _gridView.ActiveFilterEnabled = false;

            foreach (dynamic column in _gridView.Columns)
            {
                GridColumn gridColumn = column as GridColumn;
                gridColumn.Tag = _gridProperties.WoColumnPropertiesCol[index];

                string text = string.Empty;

                if (_gridProperties.WoColumnPropertiesCol[index].MaskText == null)
                {
                    Modelo model = _project
                        .ModeloCol.Modelos.Where(x => x.Id == _modelName)
                        .FirstOrDefault();
                    var modelColumn = model
                        .Columnas.Where(x =>
                            x.Id == _gridProperties.WoColumnPropertiesCol[index].Id
                        )
                        .FirstOrDefault();
                    text = modelColumn.Grid;
                }
                else
                {
                    text = _gridProperties.WoColumnPropertiesCol[index].MaskText;
                }

                _gridView.SetRowCellValue(0, gridColumn, text);

                if (!_gridProperties.WoColumnPropertiesCol[index].IsVisible)
                {
                    gridColumn.Visible = false;
                    gridColumn.Width = _gridProperties.WoColumnPropertiesCol[index].Size;
                    gridColumn.MinWidth = _gridProperties.WoColumnPropertiesCol[index].Size;
                    gridColumn.MaxWidth = _gridProperties.WoColumnPropertiesCol[index].Size;
                    gridColumn.VisibleIndex = -1;
                }
                else
                {
                    gridColumn.OptionsFilter.AllowFilter = false;
                    gridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    gridColumn.Width = _gridProperties.WoColumnPropertiesCol[index].Size;
                    gridColumn.MinWidth = _gridProperties.WoColumnPropertiesCol[index].Size;
                    gridColumn.MaxWidth = _gridProperties.WoColumnPropertiesCol[index].Size;
                    gridColumn.VisibleIndex = _gridProperties.WoColumnPropertiesCol[index].Index;

                    _gridView.UpdateCurrentRow();
                }

                index++;
            }

            grdGridProperties.SelectedObject = _gridProperties;
        }

        #endregion De serializar Json a grid

        #region Serializar grid a json

        /// <summary>
        /// Guarda el diseño del grid actual en un objeto de tipo WoGrid
        /// y lo serializa en el fichero que se define en el constructor.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void SerializeGridToJson()
        {
            WoGridProperties grid = new WoGridProperties()
            {
                IsSlave = _gridProperties.IsSlave,
                ModelName = _gridProperties.ModelName,
                Proceso = _gridProperties.Proceso,
                DirectModel = _gridProperties.DirectModel,
                OpenInNewTab = _gridProperties.OpenInNewTab,
                LabelId = _gridProperties.LabelId,
                MaskText = _gridProperties.MaskText,
                GridSelect = _gridProperties.GridSelect,
            };

            if (grid.GridSelect)
            {
                if (CanSelectRows())
                {
                    CreateGridSelectedCode();
                }
            }
            else
            {
                DeleteGridSelectedCode();
            }

            List<WoColumnProperties> columnsCol = new List<WoColumnProperties>();

            foreach (dynamic column in _gridView.Columns)
            {
                WoColumnProperties properties = (WoColumnProperties)column.Tag;
                properties.Index = column.VisibleIndex;
                properties.IsVisible = column.Visible;
                columnsCol.Add(properties);
            }

            grid.WoColumnPropertiesCol.Clear();

            grid.WoColumnPropertiesCol = columnsCol.OrderBy(x => x.Index).ToList();

            if (!File.Exists(_pathGridSave))
                WoDirectory.CreateFile(_pathGridSave);

            File.WriteAllText(_pathGridSave, JsonConvert.SerializeObject(grid));
        }

        #endregion Serializar grid a json


        #region Edición de la grid

        /// <summary>
        /// Controlador de eventos que se detona cuando se realiza una modificación en la grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdColumnProperties_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            if (_focusedColumn != null)
            {
                if (e.Row.Properties.FieldName == "Etiqueta")
                {
                    WoColumnProperties columnProperties = (WoColumnProperties)
                        _focusedColumn.FocusedColumn.Tag;

                    string text = EtiquetaCol.Get(e.Value.ToString());
                    columnProperties.Etiqueta = e.Value.ToString();
                    columnProperties.MaskText = text;
                    columnProperties.IsCustomLabel = true;

                    _focusedColumn.FocusedColumn.Tag = columnProperties;

                    _gridView.SetRowCellValue(0, _focusedColumn.FocusedColumn, text);
                    _gridView.UpdateCurrentRow();
                    _gridView.RefreshData();
                }
                else if (e.Row.Properties.FieldName == "Size")
                {
                    WoColumnProperties columnProperties = (WoColumnProperties)
                        _focusedColumn.FocusedColumn.Tag;

                    columnProperties.Size = Convert.ToInt32(e.Value);

                    _focusedColumn.FocusedColumn.Tag = columnProperties;

                    //_focusedColumn.FocusedColumn.MinWidth = columnProperties.Size;
                }
                else if (e.Row.Properties.FieldName == "SizeType")
                {
                    WoColumnProperties columnProperties = (WoColumnProperties)
                        _focusedColumn.FocusedColumn.Tag;

                    columnProperties.SizeType = e.Value.ToString();

                    _focusedColumn.FocusedColumn.Tag = columnProperties;
                }

                GridUpdatingEvt?.Invoke();
            }
        }

        #endregion Edición de la grid


        #region Clean all

        /// <summary>
        /// Limpia todas las columnas de la grid.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CleanAll()
        {
            foreach (dynamic column in _gridView.Columns)
            {
                column.Visible = false;
            }
        }

        #endregion Clean all

        #region Agregar columnas custom

        /// <summary>
        /// Agrega una columna custom a la grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            string id = GetNextId();

            WoColumnProperties columnProperties = new WoColumnProperties()
            {
                Id = id,
                Etiqueta = id,
                MaskText = id,
                IsCustomLabel = true,
                ModelName = _modelName,
                IsSlave = true,
                Process = string.Empty,
                Size = 100,
                SizeType = "px",
                Index = _gridProperties.WoColumnPropertiesCol.Count + 1,
                IsVisible = true,
                BindingType = "string",
                IsReference = false,
                UrlBaseReference = null,
                Control = WoTypeControl.NA,
                IsCustomColumn = true
            };

            GridColumn newColumn = new GridColumn();
            newColumn.FieldName = columnProperties.Etiqueta;
            newColumn.Caption = columnProperties.Etiqueta;
            newColumn.UnboundType = DevExpress.Data.UnboundColumnType.String;
            newColumn.Visible = true;
            newColumn.Tag = columnProperties;
            newColumn.MinWidth = 300;
            _gridView.Columns.Add(newColumn);

            _gridView.UpdateCurrentRow();

            GridUpdatingEvt?.Invoke();
        }

        /// <summary>
        /// Recupera un identificador que no exista en la grid para la columna siguiente.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private string GetNextId()
        {
            List<WoColumnProperties> customColumns = new List<WoColumnProperties>();

            foreach (GridColumn column in _gridView.Columns)
            {
                WoColumnProperties columnProperties = (WoColumnProperties)column.Tag;
                if (columnProperties.IsCustomColumn)
                {
                    customColumns.Add(columnProperties);
                }
            }

            int customColumnIndex = 0;
            string customColumnId = string.Empty;

            do
            {
                customColumnIndex++;
                customColumnId = $@"ColumnaCustom{customColumnIndex}";
            } while (customColumns.FirstOrDefault(x => x.Id == customColumnId) != null);

            return customColumnId;
        }

        #endregion Agregar columnas custom

        #region Eliminar columnas custom

        /// <summary>
        /// Elimina la columna seleccionada en caso de ser una columna custom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteColumn_Click(object sender, EventArgs e)
        {
            _gridView.Columns.Remove(_focusedColumn.FocusedColumn);
            btnDeleteColumn.Enabled = false;
        }

        #endregion Eliminar columnas custom


        #region Lista de los botones

        /// <summary>
        /// Lista de los botones custom.
        /// </summary>
        private List<WoCustomButtonProperties> _customButtons =
            new List<WoCustomButtonProperties>();

        /// <summary>
        /// Ruta del archivo json donde se guardan los botones custom.
        /// </summary>
        private string _pathFile = string.Empty;

        /// <summary>
        /// Carga la lista de los botones custom desde el json.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void ChargeButtonList()
        {
            _customButtons.Clear();
            pnlButtons.Controls.Clear();

            _pathFile =
                $@"{_project.DirLayOuts}\UserCode\{_modelName}GridList_proj\{_modelName}GridListCustomButtons.json";
            if (File.Exists(_pathFile))
            {
                string json = WoDirectory.ReadFile(_pathFile);
                _customButtons = JsonConvert.DeserializeObject<List<WoCustomButtonProperties>>(
                    json
                );
            }

            ChargeButtonsToList();
        }

        /// <summary>
        /// Carga los botones custom al panel con los botones
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeButtonsToList()
        {
            int buttonValidateCount = 0;
            pnlButtons.Controls.Clear();

            foreach (
                WoCustomButtonProperties button in _customButtons.OrderBy(button => button.Index)
            )
            {
                if (button.Index == 0)
                {
                    do
                    {
                        buttonValidateCount++;
                    } while (
                        _customButtons.FirstOrDefault(button => button.Index == buttonValidateCount)
                        != null
                    );

                    button.Index = buttonValidateCount;
                }
            }

            foreach (
                WoCustomButtonProperties button in _customButtons.OrderByDescending(button =>
                    button.Index
                )
            )
            {
                SimpleButton newBtn = new SimpleButton();
                newBtn.Name = button.ButtonId;
                newBtn.Text = button.MaskText;
                newBtn.Dock = DockStyle.Left;
                newBtn.Width = 300;
                newBtn.Tag = button;

                if (button.Icon != eBoostrapIcons.None)
                {
                    System.Drawing.Image icon = null;
                    var svgDoc = SvgDocument.Open(
                        $@"{_iconsBasePath}\{_woCommonDesignOptions.BoostrapIcons.Get(button.Icon.ToString())}"
                    );

                    icon = svgDoc.Draw();
                    newBtn.ImageOptions.Image = icon;
                }

                newBtn.Click += SelectButton;
                pnlButtons.Controls.Add(newBtn);
            }
        }

        #endregion Lista de los botones

        #region Creación de los botones

        /// <summary>
        /// Controlador de eventos que se detona cuando se crea un nuevo botón.
        /// </summary>
        public Action AddButtonEvt { get; set; }

        /// <summary>
        /// Controlador de eventos para actualizar los proyectos unitarias
        /// </summary>
        public Action UpdateProjectsEvt { get; set; }

        /// <summary>
        /// Agrega botones custom para el color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAddButton_Click(object sender, EventArgs e)
        {
            string newId = GetNewId();
            int newIndex = 0;

            WoCustomButtonProperties lastButton = _customButtons
                .OrderByDescending(button => button.Index)
                .FirstOrDefault();
            if (lastButton == null)
            {
                newIndex = 1;
            }
            else
            {
                newIndex = lastButton.Index + 1;
            }

            WoCustomButtonProperties newCustomButton = new WoCustomButtonProperties()
            {
                ButtonId = newId,
                Label = "Button",
                MaskText = "Button",
                MethodName = newId,
                Icon = eBoostrapIcons.None,
                Index = newIndex,
            };
            _customButtons.Add(newCustomButton);

            SimpleButton newBtn = new SimpleButton();
            newBtn.Name = newId;
            newBtn.Text = "Button";
            newBtn.Dock = DockStyle.Left;
            newBtn.Width = 300;
            newBtn.Tag = newCustomButton;
            newBtn.Click += SelectButton;
            pnlButtons.Controls.Add(newBtn);

            UpdateJson();

            CreateButton(newCustomButton);

            AddButtonEvt?.Invoke();

            UpdateProjectsEvt?.Invoke();

            ChargeButtonsToList();
        }

        /// <summary>
        /// Actualiza el json con la lista de los botones custom.
        /// </summary>
        private void UpdateJson()
        {
            if (File.Exists(_pathFile))
            {
                WoDirectory.DeleteFile(_pathFile);
            }

            WoDirectory.WriteFile(_pathFile, JsonConvert.SerializeObject(_customButtons));
        }

        /// <summary>
        /// Genera un identificador nuevo para el siguiente botón custom.
        /// </summary>
        /// <returns></returns>
        private string GetNewId()
        {
            string id = string.Empty;

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

            string projectName = woProjectDataHelper
                .GetWWSBName(_project)
                .Split(".")
                .First()
                .Replace(" ", "");

            int count = 0;
            do
            {
                count++;
                id = $"BtnCustom{count}{projectName}";
            } while ((_customButtons.FirstOrDefault(x => x.ButtonId == id)) != null);

            return id;
        }

        #endregion Creación de los botones

        #region Creación del método custom

        /// <summary>
        /// Controlador de eventos que se detona cuando se crea un nuevo método.
        /// </summary>
        public Action UpdateCodeEditor { get; set; }

        /// <summary>
        /// Creación del botón
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateButton(WoCustomButtonProperties newCustomButton)
        {
            WoSyntaxManagerUserCode userCode = new WoSyntaxManagerUserCode();

            userCode.InitializeManager(
                pathScript: $@"{_project.DirLayOuts}\UserCode\{_modelName}GridList_proj\{_modelName}GridListScriptsUser.cs",
                className: $@"{_modelName}GridList",
                modelName: _modelName
            );

            userCode.CreateNewMethod(
                methodName: $@"{newCustomButton.ButtonId}_OnClick",
                bodyMethod: string.Empty,
                typeMethod: "void"
            );

            UpdateCodeEditor?.Invoke();
        }

        #endregion Creación del método custom

        #region Selección de los botones

        /// <summary>
        /// Instancia del botón que se esta editando.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private SimpleButton _selectedButton = new SimpleButton();

        /// <summary>
        /// Selecciona el botón que se esta editando.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void SelectButton(object sender, EventArgs e)
        {
            _selectedButton = (SimpleButton)sender;
            _selectedButtonProperties = (WoCustomButtonProperties)_selectedButton.Tag;
            propButton.SelectedObject = _selectedButton.Tag;

            btnDeleteButton.Enabled = true;
            btnDeleteColumn.Enabled = false;
        }

        #endregion Selección de los botones

        #region Edición de los botones

        /// <summary>
        /// Opciones de diseño comunes para todos los botones.
        /// </summary>
        private WoCommonDesignOptions _woCommonDesignOptions = new WoCommonDesignOptions();

        /// <summary>
        /// Indica cual es el botón que se esta configurando
        /// </summary>
        private WoCustomButtonProperties _selectedButtonProperties = new WoCustomButtonProperties();

        /// <summary>
        /// PathBase de los iconos
        /// Se inicializa en el constructor.
        /// </summary>
        private string _iconsBasePath = string.Empty;

        /// <summary>
        /// Mantiene actualizado el json en función de las propiedades realizadas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void propButton_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            if (e.Row.Properties.FieldName == "Label")
            {
                _selectedButtonProperties.Label = e.Value.ToString();
                _selectedButtonProperties.MaskText = EtiquetaCol.Get(e.Value.ToString());
                _selectedButton.Text = _selectedButtonProperties.MaskText;
            }
            else if (e.Row.Properties.FieldName == "Icon")
            {
                _selectedButtonProperties.Icon = (eBoostrapIcons)e.Value;
                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open(
                    $@"{_iconsBasePath}\{_woCommonDesignOptions.BoostrapIcons.Get(_selectedButtonProperties.Icon.ToString())}"
                );

                icon = svgDoc.Draw();

                _selectedButton.ImageOptions.Image = icon;
            }

            _customButtons.Remove(button => button.ButtonId == _selectedButtonProperties.ButtonId);
            _customButtons.Add(_selectedButtonProperties);

            UpdateJson();

            UpdateProjectsEvt?.Invoke();
        }

        #endregion Edición de los botones

        #region Eliminar botón

        /// <summary>
        /// Controlador de eventos que se detona cuando se elimina un botón.
        /// </summary>
        public Action DeleteButtonEvt { get; set; }

        /// <summary>
        /// Elimina el botón seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteButton_Click(object sender, EventArgs e)
        {
            if (_selectedButtonProperties != null)
            {
                DeleteMethodButton(_selectedButtonProperties);

                pnlButtons.Controls.Remove(_selectedButton);
                _customButtons.Remove(button =>
                    button.ButtonId == _selectedButtonProperties.ButtonId
                );
                propButton.SelectedObject = null;

                UpdateJson();

                DeleteButtonEvt?.Invoke();

                UpdateProjectsEvt?.Invoke();

                _selectedButtonProperties = null;
            }
        }

        #endregion Eliminar botón

        #region Eliminar método

        /// <summary>
        /// Eliminación del botón
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void DeleteMethodButton(WoCustomButtonProperties newCustomButton)
        {
            WoSyntaxManagerUserCode userCode = new WoSyntaxManagerUserCode();

            userCode.InitializeManager(
                pathScript: $@"{_project.DirLayOuts}\UserCode\{_modelName}GridList_proj\{_modelName}GridListScriptsUser.cs",
                className: $@"{_modelName}GridList",
                modelName: _modelName
            );

            userCode.DeleteMethod(methodName: $@"{newCustomButton.ButtonId}_OnClick");

            UpdateCodeEditor?.Invoke();
        }

        #endregion Eliminar método

        #region Cambiar posición de los botones

        /// <summary>
        /// Modifica el orden de los botones para la grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnButtonLeft_Click(object sender, EventArgs e)
        {
            if (_selectedButtonProperties.Index > 1)
            {
                List<WoCustomButtonProperties> newList = new List<WoCustomButtonProperties>();
                int index = 1;

                foreach (
                    WoCustomButtonProperties buttonProperties in _customButtons.OrderBy(button =>
                        button.Index
                    )
                )
                {
                    if (buttonProperties.ButtonId == _selectedButtonProperties.ButtonId)
                    {
                        newList.Last().Index++;
                        buttonProperties.Index = index - 1;
                        newList.Add(buttonProperties);
                        index++;
                    }
                    else
                    {
                        buttonProperties.Index = index;
                        newList.Add(buttonProperties);
                        index++;
                    }
                }

                _customButtons.Clear();
                _customButtons.AddRange(newList);

                UpdateJson();
                ChargeButtonsToList();

                UpdateProjectsEvt?.Invoke();
            }
        }

        /// <summary>
        /// Modifica el orden de los botones para la grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnButtonRight_Click(object sender, EventArgs e)
        {
            List<WoCustomButtonProperties> newList = new List<WoCustomButtonProperties>();
            int index = 1;

            WoCustomButtonProperties findButtonProperties = null;

            foreach (
                WoCustomButtonProperties buttonProperties in _customButtons.OrderBy(button =>
                    button.Index
                )
            )
            {
                if (buttonProperties.ButtonId == _selectedButtonProperties.ButtonId)
                {
                    findButtonProperties = buttonProperties;
                }
                else if (findButtonProperties != null)
                {
                    buttonProperties.Index = index;
                    newList.Add(buttonProperties);
                    index++;
                    findButtonProperties.Index++;
                    newList.Add(findButtonProperties);
                    index++;
                    findButtonProperties = null;
                }
                else
                {
                    buttonProperties.Index = index;
                    newList.Add(buttonProperties);
                    index++;
                }
            }

            if (findButtonProperties != null)
            {
                newList.Add(findButtonProperties);
                findButtonProperties = null;
            }

            _customButtons.Clear();
            _customButtons.AddRange(newList);

            UpdateJson();
            ChargeButtonsToList();

            UpdateProjectsEvt?.Invoke();
        }

        #endregion Cambiar posición de los botones


        #region Edición de la grid

        /// <summary>
        /// Action que avisa al formulario principal que se esta editando la grid
        /// </summary>
        public Action GridUpdatingEvt { get; set; }

        #endregion Edición de la grid

        #region Deshabilitado de las opciones temporales

        /// <summary>
        /// Deshabilita temporalmente opciones que no están disponibles para las esclavas.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void DisableForSlaveOptions()
        {
            sccPanelDesigner.PanelVisibility = SplitPanelVisibility.Panel1;
            sccButtonGrid.PanelVisibility = SplitPanelVisibility.Panel2;
        }

        #endregion Deshabilitado de las opciones temporales


        #region Edición de las propiedades generales de la grid

        /// <summary>
        /// Edición de las propiedades de la grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdGridProperties_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            try
            {
                if (e.Row.Properties.FieldName == "LabelId")
                {
                    string text = EtiquetaCol.Get(e.Value.ToString());
                    _gridProperties.LabelId = e.Value.ToString();
                    _gridProperties.MaskText = text;

                    GridUpdatingEvt?.Invoke();
                }
                else if (e.Row.Properties.FieldName == "GridSelect")
                {
                    bool activeSelected = bool.Parse(e.Value.ToString());
                    if (activeSelected)
                    {
                        if (!CanSelectRows())
                        {
                            throw new Exception("Indique como Id una de las columnas.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al modificar la grid. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Edición de las propiedades generales de la grid

        #region Grid Seleccionable

        /// <summary>
        /// Valida si se puede seleccionar alguna row sobre de la grid
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private bool CanSelectRows()
        {
            try
            {
                bool resultValidate = false;

                GridColumnCollection columns = _gridView.Columns;
                foreach (GridColumn column in columns)
                {
                    WoColumnProperties columnProperties = (WoColumnProperties)column.Tag;
                    resultValidate = columnProperties.IsId;
                    if (resultValidate)
                    {
                        break;
                    }
                }
                return resultValidate;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al validar si se puede hacer seleccionable la grid. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Crea los métodos para la selección de rows en la grid en los scripts de usuario.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void CreateGridSelectedCode()
        {
            try
            {
                string scriptsUserPaht =
                    $@"{_project.DirLayOuts}\\UserCode\\{_modelName}GridList_proj\\{_modelName}GridListScriptsUser.cs";

                if (File.Exists(scriptsUserPaht))
                {
                    WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                    woSyntaxManagerUserCode.InitializeManager(
                        pathScript: scriptsUserPaht,
                        className: $@"{_modelName}GridList",
                        modelName: _modelName
                    );

                    woSyntaxManagerUserCode.CreateAttribute(
                        type: $@"List<{_modelName}>",
                        name: $@"{_modelName}Seleccionados",
                        value: $@"new List<{_modelName}>()",
                        accessModifier: "public",
                        getset: false
                    );

                    woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"Selected{_modelName}ItemsChanged",
                        bodyMethod: $@"{_modelName}Seleccionados = {_modelName.ToLower()}.ToList();",
                        typeMethod: $@"void",
                        methodParamsCol: new List<(string type, string name, string value)>()
                        {
                            (
                                type: $@"IEnumerable<{_modelName}>",
                                name: _modelName.ToLower(),
                                value: ""
                            )
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al crear los métodos de selección. {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina el código para la selección de alguna de las rows
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void DeleteGridSelectedCode()
        {
            try
            {
                string scriptsUserPaht =
                    $@"{_project.DirLayOuts}\\UserCode\\{_modelName}GridList_proj\\{_modelName}GridListScriptsUser.cs";

                if (File.Exists(scriptsUserPaht))
                {
                    WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();

                    woSyntaxManagerUserCode.InitializeManager(
                        pathScript: scriptsUserPaht,
                        className: $@"{_modelName}GridList",
                        modelName: _modelName
                    );

                    if (
                        woSyntaxManagerUserCode.AlreadyExistAttribute($@"{_modelName}Seleccionados")
                    )
                    {
                        woSyntaxManagerUserCode.DeleteCustomAttribute(
                            name: $@"{_modelName}Seleccionados"
                        );
                    }

                    if (
                        woSyntaxManagerUserCode.AlreadyExistMethod(
                            $@"Selected{_modelName}ItemsChanged"
                        )
                    )
                    {
                        woSyntaxManagerUserCode.DeleteMethod($@"Selected{_modelName}ItemsChanged");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al crear el codigo para la selección. {ex.Message}");
            }
        }

        #endregion Grid Seleccionable
    }
}
