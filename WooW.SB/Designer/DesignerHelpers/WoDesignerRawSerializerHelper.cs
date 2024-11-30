using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WooW.Core;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer.DesignerHelpers
{
    public enum eFormType
    {
        TabsForm,
        SimpleForm
    }

    public class WoDesignerRawSerializerHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto.
        /// Para enviar alertas e información al log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Atributos

        #region Helpers

        /// <summary>
        /// Permite convertir los tipos del diseñador a tipos de C#.
        /// Se inicializa en el constructor.
        /// </summary>
        private WoDesignerTypeHelper _designerTypeHelper;

        #endregion Helpers

        #region Variables globales

        /// <summary>
        /// Nombre del modelo de que se generara el formulario.
        /// </summary>
        private string _modelName = string.Empty;

        #endregion Variables globales

        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar los métodos.
        /// </summary>
        /// <param name="modelName"></param>
        public WoContainer BuildRawWoContainer(string modelName, Proyecto proyect = null)
        {
            if (proyect != null)
            {
                _project = proyect;
            }

            if (modelName != string.Empty)
            {
                _labelsNotFound.Clear();

                _modelName = modelName;
                _designerTypeHelper = new WoDesignerTypeHelper(_modelName);

                SearchModel();
                ValidateForm();
                AddBaseControls();
                BuildBaseContainers();
                AddControls();

                switch (_formType)
                {
                    case eFormType.TabsForm:
                        _tabGroupBase.ContainersCol.Add(_tabForm);
                        _tabGroupBase.ContainersCol.Add(_tabControl);
                        _rootBase.ContainersCol.Add(_tabGroupBase);
                        break;
                    case eFormType.SimpleForm:
                        _rootBase.ContainersCol.Add(_baseGroup);
                        break;
                    default:
                        throw new WoObserverException(
                            new WoLog()
                            {
                                CodeLog = "000",
                                Details =
                                    "No fue posible definir el tipo de modelo sobre el que se esta trabajando",
                                FileDetails = new WoFileDetails()
                                {
                                    Class = "WoDesignerRawSerializerHelper",
                                    MethodOrContext = "BuildRawWoContainer"
                                }
                            }
                        );
                        break;
                }

                if (_slavesCol.Count > 0)
                {
                    _rootBase.Row += 6;
                    CreateTabGroupBase();
                    foreach (WoItem slave in _slavesCol)
                    {
                        WoContainer containerTabGroup = BuildTabGroup(slave.SlaveModelId);
                        slave.Parent = containerTabGroup.Id;
                        containerTabGroup.ItemsCol.Add(slave);
                        _tabGroupSlaves.ContainersCol.Add(containerTabGroup);
                    }
                    _rootBase.ContainersCol.Add(_tabGroupSlaves);
                }

                AlertToLabels();
            }

            _rootBase.Etiqueta = _baseModel.EtiquetaId;
            _rootBase.MaskText = EtiquetaCol.Get(_baseModel.EtiquetaId);

            return _rootBase;
        }

        #endregion Método principal

        #region Modelo

        /// <summary>
        /// Modelo sobre el que se esta trabajando.
        /// </summary>
        private Modelo _baseModel = null;

        /// <summary>
        /// Busca el modelo y en caso de no poder encontrarlo en los que contiene
        /// project envía una exception.
        /// </summary>
        /// <exception cref="WoObserverException"></exception>
        private void SearchModel()
        {
            try
            {
                _baseModel = _project.ModeloCol.Modelos.FirstOrDefault(x => x.Id == _modelName);

                if (_baseModel == null)
                {
                    WoProjectDataHelper WoProjectDataHelper = new WoProjectDataHelper();
                    _baseModel = WoProjectDataHelper.GetMainModel(_modelName);
                }

                if (_baseModel == null)
                {
                    throw new WoObserverException(
                        _cantFindModel,
                        $@"El modelo {_modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                    );
                }
                else
                {
                    _rootBase.Proceso = _baseModel.ProcesoId;
                    _rootBase.ModelId = _baseModel.Id;

                    // Indicador de si el diseño es de una extensión
                    _rootBase.IsExtension = _baseModel.ProcesoId == "";
                }
            }
            catch (Exception ex)
            {
                throw new WoObserverException(
                    _cantFindModel,
                    $@"El modelo {_modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                );
            }
        }

        /// <summary>
        /// Busca el modelo del detalle
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        /// <exception cref="WoObserverException"></exception>
        private Modelo SearchModelDet(string modelName)
        {
            Modelo _findModel = null;
            try
            {
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                _findModel = woProjectDataHelper.GetMainModel(_modelName);
                //_findModel = _project.ModeloCol.Modelos.Where(x => x.Id == modelName).First();
                if (_findModel == null)
                {
                    throw new WoObserverException(
                        _cantFindModel,
                        $@"El modelo {_modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                    );
                }
            }
            catch (Exception ex)
            {
                throw new WoObserverException(
                    _cantFindModel,
                    $@"El modelo {_modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                );
            }

            return _findModel;
        }

        #endregion Modelo

        #region Tipo de formulario

        /// <summary>
        /// Indica si es un formulario convencional o es un formulario de esclava o de request.
        /// </summary>
        private eFormType _formType = eFormType.SimpleForm;

        /// <summary>
        /// Indica si se esta generando el formulario de una esclava.
        /// </summary>
        private bool _isSlave = false;

        /// <summary>
        /// Indica si se esta generando un formulario de tipo request.
        /// </summary>
        private bool _isRequest = false;

        /// <summary>
        /// Indica si el modelo siendo diseñado es una extensión,
        /// tendrá formulario normal sin alertas
        /// </summary>
        private bool _isExtencion = false;

        /// <summary>
        /// Define el tipo de formulario que se generar en función del tipo de modelo.
        /// </summary>
        public void ValidateForm()
        {
            _rootBase.ModelType = _baseModel.TipoModelo;
            _rootBase.SubType = _baseModel.SubTipoModelo;

            if (_baseModel.ProcesoId == "")
            {
                _formType = eFormType.SimpleForm;
                _isExtencion = true;
            }
            else if (
                _baseModel.TipoModelo != Core.WoTypeModel.Request
                && _baseModel.TipoModelo != Core.WoTypeModel.TransactionSlave
                && _baseModel.TipoModelo != Core.WoTypeModel.CatalogSlave
            )
            {
                _formType = eFormType.TabsForm;
            }
            if (
                _baseModel.TipoModelo == Core.WoTypeModel.TransactionSlave
                || _baseModel.TipoModelo == Core.WoTypeModel.CatalogSlave
            )
            {
                _isSlave = true;
            }
            if (_baseModel.TipoModelo == Core.WoTypeModel.Request)
            {
                _isRequest = true;
            }
        }

        #endregion Tipo de formulario

        #region Controles principales (Record tool bar y alert)

        /// <summary>
        /// Crea los controles principales para las alertas y la barra de navegación.
        /// </summary>
        private void AddBaseControls()
        {
            AddAlerts();

            AddRecordToolBar();
        }

        /// <summary>
        /// Agrega el control de las alerta al formulario.
        /// </summary>
        private void AddAlerts()
        {
            if (_formType == eFormType.TabsForm)
            {
                _rootBase.Row += 2;

                string text = EtiquetaCol.Get("Alertas");
                if (text == " ETIQUETA NO EXISTE")
                {
                    _labelsNotFound.Add("Alertas");
                }

                _rootBase.ItemsCol.Add(
                    new WoItem()
                    {
                        Id = "Alertas",
                        Etiqueta = "Alertas",
                        MaskText = EtiquetaCol.Get("Alertas"),
                        Parent = "Root",
                        Enable = eItemEnabled.Activo,
                        TypeItem = eTypeItem.FormItem,
                        Control = "Label",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = _indexRoot,
                        ComponenteExtra = true,
                        BaseModelName = _modelName
                    }
                );

                _indexRoot++;
            }
        }

        /// <summary>
        /// Agrega la barra de navegación entre los formularios.
        /// </summary>
        private void AddRecordToolBar()
        {
            if (_formType == eFormType.TabsForm)
            {
                string text = EtiquetaCol.Get("Controles");
                if (text == " ETIQUETA NO EXISTE")
                {
                    _labelsNotFound.Add("Controles");
                }

                _rootBase.ItemsCol.Add(
                    new WoItem()
                    {
                        Id = "Controles",
                        Etiqueta = "Controles",
                        MaskText = EtiquetaCol.Get("Controles"),
                        Parent = "Root",
                        Enable = eItemEnabled.Activo,
                        TypeItem = eTypeItem.FormItem,
                        ClassModelType = _modelName,
                        Control = "Label",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = _indexRoot,
                        ComponenteExtra = true,
                        BaseModelName = _modelName,
                        ModelType = _baseModel.TipoModelo,
                        SubType = _baseModel.SubTipoModelo
                    }
                );

                _indexRoot++;
            }
        }

        #endregion Controles principales (Record tool bar y alert)

        #region Contenedores base

        /// <summary>
        /// Contenedor base de todo lo que se busque agregar al diseñador.
        /// </summary>
        private WoContainer _rootBase = new WoContainer()
        {
            Id = "Root",
            Etiqueta = "Root",
            TypeContainer = eTypeContainer.FormRoot,
            Col = 12,
            Row = 1,
            ColSpan = 1,
            RowSpan = 1,
            BackgorundColorGroup = eGroupColor.Background,
            Theme = "Default"
        };

        /// <summary>
        /// Indica en que row poner el siguiente control dentro del contenedor base.
        /// </summary>
        private int _indexRoot = 0;

        /// <summary>
        /// Grupo principal de los componentes, es para la estructura de los formularios de tipo request o los formularios de las esclavas.
        /// </summary>
        private WoContainer _baseGroup;

        /// <summary>
        /// Grupo de tabs para los formularios convencionales.
        /// </summary>
        private WoContainer _tabGroupBase = new WoContainer();

        /// <summary>
        /// Tab donde se colocaran los controles del formulario convencional.
        /// </summary>
        private WoContainer _tabForm = new WoContainer();

        /// <summary>
        /// Tab donde se colocaran los controles de control del registro.
        /// </summary>
        private WoContainer _tabControl = new WoContainer();

        /// <summary>
        /// Construye los contenedores en función del tipo de modelo con el que se este trabajando.
        /// </summary>
        private void BuildBaseContainers()
        {
            if (_formType == eFormType.TabsForm)
            {
                string text = EtiquetaCol.Get("GrupoDeTabs");
                if (text == " ETIQUETA NO EXISTE")
                {
                    _labelsNotFound.Add("GrupoDeTabs");
                }

                _tabGroupBase = new WoContainer()
                {
                    Id = "tabGroupBase",
                    Etiqueta = "GrupoDeTabs",
                    MaskText = EtiquetaCol.Get("GrupoDeTabs"),
                    Parent = "Root",
                    TypeContainer = eTypeContainer.FormTabGroup,
                    ColSpan = 6,
                    RowSpan = 2,
                    ColumnIndex = 0,
                    RowIndex = _indexRoot,
                };

                text = EtiquetaCol.Get("Datos");
                if (text == " ETIQUETA NO EXISTE")
                {
                    _labelsNotFound.Add("Datos");
                }

                _tabForm = new WoContainer()
                {
                    Id = "formFormModel",
                    Etiqueta = "Datos",
                    MaskText = EtiquetaCol.Get("Datos"),
                    Parent = "tabGroupBase",
                    TypeContainer = eTypeContainer.FormTab,
                    Row = 3,
                    Col = 12,
                    ColSpan = 1,
                    RowSpan = 2,
                    BackgorundColorGroup = eGroupColor.Default,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default"
                };

                text = EtiquetaCol.Get("Status");
                if (text == " ETIQUETA NO EXISTE")
                {
                    _labelsNotFound.Add("Status");
                }

                _tabControl = new WoContainer()
                {
                    Id = "formControlform",
                    Etiqueta = "Status",
                    MaskText = EtiquetaCol.Get("Status"),
                    Parent = "tabGroupBase",
                    TypeContainer = eTypeContainer.FormTab,
                    Row = 3,
                    Col = 12,
                    ColSpan = 1,
                    RowSpan = 1,
                    BackgorundColorGroup = eGroupColor.Default,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default"
                };

                _indexRoot += 2;
            }
            else
            {
                string text = EtiquetaCol.Get(_baseModel.EtiquetaId);
                if (text == " ETIQUETA NO EXISTE")
                {
                    _labelsNotFound.Add(_baseModel.EtiquetaId);
                }

                string idGroup =
                    $@"{_baseModel.Proyecto.Nombre.Replace(".", "").Replace(" ", "")}{_baseModel.Id}form";

                _baseGroup = new WoContainer()
                {
                    Id = idGroup,
                    Etiqueta =
                        (_isExtencion) ? $@"Extensión {_baseModel.Id}" : _baseModel.EtiquetaId,
                    MaskText =
                        (_isExtencion)
                            ? $@"Extensión {_baseModel.Id}"
                            : EtiquetaCol.Get(_baseModel.EtiquetaId),
                    Parent = "Root",
                    TypeContainer = eTypeContainer.FormGroup,
                    ColSpan = 12,
                    RowSpan = 2,
                    Row = 2,
                    Col = 12,
                    ColumnIndex = 0,
                    RowIndex = _indexRoot,
                    BackgorundColorGroup = eGroupColor.Default,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default",
                    Visible = _isExtencion,
                    IsSlavePooper = _isSlave
                };

                _indexRoot += 2;
            }
        }

        #endregion Contenedores base

        #region Tab para slaves

        /// <summary>
        /// Grupo de tabs para las slaves.
        /// </summary>
        private WoContainer _tabGroupSlaves = new WoContainer();

        private void CreateTabGroupBase()
        {
            string text = EtiquetaCol.Get("GrupoDeTabs");
            if (text == " ETIQUETA NO EXISTE")
            {
                _labelsNotFound.Add("GrupoDeTabs");
            }

            _tabGroupSlaves = new WoContainer()
            {
                Id = "tabGroupSlaves",
                Etiqueta = "GrupoDeTabs",
                MaskText = EtiquetaCol.Get("GrupoDeTabs"),
                Parent = "Root",
                TypeContainer = eTypeContainer.FormTabGroup,
                ColSpan = 6,
                RowSpan = 2,
                ColumnIndex = 0,
                RowIndex = _indexRoot,
            };
        }

        private WoContainer BuildTabGroup(string modelSlave)
        {
            Modelo masterModel = SearchModelDet(_modelName);

            ModeloColumna findSlave = masterModel
                .Columnas.Where(x => x.Id == $@"{modelSlave}Col")
                .FirstOrDefault();

            string text = EtiquetaCol.Get($@"{Etiqueta.ToId(findSlave.Formulario)}");
            //if (text == " ETIQUETA NO EXISTE")
            //{
            //    _labelsNotFound.Add($@"{Etiqueta.ToId(findSlave.Formulario)}");
            //}

            WoContainer tabContainer = new WoContainer()
            {
                Id = $@"{modelSlave}tabSlave",
                Etiqueta = Etiqueta.ToId(findSlave.Formulario),
                MaskText = findSlave.Formulario,
                Parent = "tabGroupSlaves",
                TypeContainer = eTypeContainer.FormTab,
                ColSpan = 6,
                RowSpan = 2,
                Row = 3,
                Col = 12,
                ColumnIndex = 0,
                RowIndex = _indexRoot,
                BackgorundColorGroup = eGroupColor.Default,
                ComponentFontColor = eTextColor.FontDefault,
                Theme = "Default"
            };

            return tabContainer;
        }

        #endregion Tab para slaves

        #region Creación de controles

        /// <summary>
        /// Lista de las esclavas para agregarse adicionalmente fuera del grupo principal
        /// </summary>
        private List<WoItem> _slavesCol = new List<WoItem>();

        private void AddControls()
        {
            if ((!_baseModel.Columnas.IsNull()) && _baseModel.Columnas.Count > 0)
            {
                switch (_formType)
                {
                    case eFormType.TabsForm:
                        BuildTabsForm();
                        break;
                    case eFormType.SimpleForm:
                        BuildSimpleForm();
                        break;
                }
            }
        }

        private void BuildSimpleForm()
        {
            int indexRowFormModel = 0;

            string text = EtiquetaCol.Get("Alertas");
            if (text == " ETIQUETA NO EXISTE")
            {
                _labelsNotFound.Add("Alertas");
            }

            if (!_isExtencion)
            {
                _baseGroup.ItemsCol.Add(
                    new WoItem()
                    {
                        Id = "Alertas",
                        Etiqueta = "Alertas",
                        MaskText = EtiquetaCol.Get("Alertas"),
                        Parent = "formModelform",
                        Enable = eItemEnabled.Activo,
                        TypeItem = eTypeItem.FormItem,
                        Control = "Label",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = indexRowFormModel,
                        ComponenteExtra = true,
                        BaseModelName = _modelName
                    }
                );

                indexRowFormModel++;
            }

            List<string> modelsReferenced = new List<string>();

            foreach (var column in _baseModel.Columnas)
            {
                string type = column.TipoColumna.ToString();
                string control = column.TipoControl.ToString();
                type = _designerTypeHelper.DesignerTypeToCodeType(
                    designerType: type,
                    isNullable: column.Nulo,
                    attributeName: column.Id
                );

                bool isMasterReference = false;
                if (
                    column.TipoColumna == WoTypeColumn.Reference
                    && (
                        _baseModel.TipoModelo == Core.WoTypeModel.TransactionSlave
                        || _baseModel.TipoModelo == Core.WoTypeModel.CatalogSlave
                    )
                )
                {
                    string masterModelName = column.ModeloId;
                    Modelo masterModel = SearchModelDet(masterModelName);

                    ModeloColumna findSlave = masterModel
                        .Columnas.Where(x => x.Id == $@"{_modelName}Col")
                        .FirstOrDefault();
                    if (findSlave != null)
                    {
                        isMasterReference = true;
                    }
                }

                if (
                    !(
                        (
                            _baseModel.TipoModelo == Core.WoTypeModel.TransactionSlave
                            || _baseModel.TipoModelo == Core.WoTypeModel.CatalogSlave
                        )
                        && column.Id == "Id"
                    ) && !isMasterReference
                )
                {
                    int multipleReference = 0;
                    if (
                        column.TipoControl == WoTypeControl.LookUp
                        || column.TipoControl == WoTypeControl.LookUpDialog
                    )
                    {
                        if (modelsReferenced.Contains(column.ModeloId))
                        {
                            multipleReference = modelsReferenced
                                .Where(model => model == column.ModeloId)
                                .Count();
                        }
                        modelsReferenced.Add(column.ModeloId);
                    }

                    _baseGroup.ItemsCol.Add(
                        new WoItem()
                        {
                            Id = column.Id,
                            Etiqueta = null,
                            MaskText = null,
                            BindedProperty = column.Id,
                            TypeItem = eTypeItem.FormItem,
                            Enable = eItemEnabled.Activo,
                            BindingType =
                                (type == "Complex" || type == "Complex?") ? "string" : type,
                            ClassModelType = column.ModeloId,
                            Nullable = column.Nulo,
                            Control = control,
                            Parent = "Modelform",
                            ColSpan = 12,
                            RowSpan = (control == "Memo") ? 3 : 1,
                            ColumnIndex = 0,
                            RowIndex = indexRowFormModel,
                            BackgorundColorContainerItem = eContainerItemColor.Default,
                            CaptionColor = eTextColor.FontDefault,
                            ComponentFontColor = eTextColor.FontDefault,
                            Theme = "Default",
                            BaseModelName = _modelName,
                            MultipleReference = multipleReference
                        }
                    );

                    if (control == "Memo")
                    {
                        indexRowFormModel += 3;
                        _baseGroup.Row += 3;
                    }
                    else
                    {
                        indexRowFormModel++;
                        _baseGroup.Row++;
                    }
                }
            }

            if (_isSlave)
            {
                _baseGroup.Row++;
            }
        }

        public void BuildTabsForm()
        {
            int indexRowFormModel = 0;

            List<string> modelsReferenced = new List<string>();

            foreach (var column in _baseModel.Columnas)
            {
                string control =
                    (
                        column.TipoColumna == WoTypeColumn.Decimal
                        || column.TipoColumna == WoTypeColumn.Double
                    )
                        ? WoTypeControl.Decimal.ToString()
                        : column.TipoControl.ToString();
                string type = column.TipoColumna.ToString();
                type = _designerTypeHelper.DesignerTypeToCodeType(
                    designerType: type,
                    isNullable: column.Nulo,
                    attributeName: column.Id
                );

                control = (type == "byte[]" || type == "byte[]?") ? "File" : control;
                control = (control.ToLower() == "urn") ? "Text" : control;

                if (column.Id == "WoState")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = "EnumInt",
                        Parent = "Modelform",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = 0,
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "CreatedDate")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 0, //col de inicio
                        RowIndex = 1, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "CreatedBy")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 6, //col de inicio
                        RowIndex = 1, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                }
                else if (column.Id == "ModifiedDate")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 0, //col de inicio
                        RowIndex = 2, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "ModifiedBy")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 6, //col de inicio
                        RowIndex = 2, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                }
                else if (column.Id == "SuspendDate")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 0, //col de inicio
                        RowIndex = 3, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "SuspendBy")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 6, //col de inicio
                        RowIndex = 3, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                }
                else if (column.Id == "SuspendInfo")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Modelform",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = 4,
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "DeletedDate")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 0, //col de inicio
                        RowIndex =
                            (
                                _baseModel.TipoModelo == WoTypeModel.TransactionContable
                                || _baseModel.TipoModelo == WoTypeModel.TransactionFreeStyle
                                || _baseModel.TipoModelo == WoTypeModel.TransactionNoContable
                                || _baseModel.TipoModelo == WoTypeModel.TransactionSlave
                            )
                                ? 3
                                : 5, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "DeletedBy")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Controlform",
                        ColSpan = 6, //Columnas
                        RowSpan = 1,
                        ColumnIndex = 6, //col de inicio
                        RowIndex =
                            (
                                _baseModel.TipoModelo == WoTypeModel.TransactionContable
                                || _baseModel.TipoModelo == WoTypeModel.TransactionFreeStyle
                                || _baseModel.TipoModelo == WoTypeModel.TransactionNoContable
                                || _baseModel.TipoModelo == WoTypeModel.TransactionSlave
                            )
                                ? 3
                                : 5, // row en la que estara
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                }
                else if (column.Id == "DeleteInfo")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Modelform",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = 6,
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "Nivel")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = "EnumInt",
                        Parent = "Modelform",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = 6,
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id == "Contabiliza")
                {
                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = "EnumInt",
                        Parent = "Modelform",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = 7,
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName
                    };

                    _tabControl.ItemsCol.Add(item);
                    _tabControl.Row++;
                }
                else if (column.Id != "RowVersion" && control != "CollectionEditor")
                {
                    int multipleReference = 0;
                    if (
                        column.TipoControl == WoTypeControl.LookUp
                        || column.TipoControl == WoTypeControl.LookUpDialog
                    )
                    {
                        if (modelsReferenced.Contains(column.ModeloId))
                        {
                            multipleReference = modelsReferenced
                                .Where(model => model == column.ModeloId)
                                .Count();
                        }
                        modelsReferenced.Add(column.ModeloId);
                    }

                    WoItem item = new WoItem()
                    {
                        Id = column.Id,
                        Etiqueta = null,
                        MaskText = null,
                        BindedProperty = column.Id,
                        TypeItem = eTypeItem.FormItem,
                        Enable = eItemEnabled.Activo,
                        BindingType = (type == "Complex" || type == "Complex?") ? "string" : type,
                        ClassModelType = column.ModeloId,
                        Nullable = column.Nulo,
                        Control = control,
                        Parent = "Modelform",
                        ColSpan = 12,
                        RowSpan = (control == "Memo") ? 2 : 1,
                        ColumnIndex = 0,
                        RowIndex = indexRowFormModel,
                        BackgorundColorContainerItem = eContainerItemColor.Default,
                        CaptionColor = eTextColor.FontDefault,
                        ComponentFontColor = eTextColor.FontDefault,
                        Theme = "Default",
                        BaseModelName = _modelName,
                        MultipleReference = multipleReference
                    };
                    _tabForm.ItemsCol.Add(item);

                    if (control == "Memo")
                    {
                        indexRowFormModel += 2;
                        _tabForm.Row += 2;
                    }
                    else
                    {
                        indexRowFormModel++;
                        _tabForm.Row++;
                    }
                }
                else if (control == "CollectionEditor")
                {
                    Modelo slaveModel = SearchModelDet(column.ModeloId);
                    StringBuilder strModelProperties = new StringBuilder();

                    foreach (var property in slaveModel.Columnas)
                    {
                        strModelProperties.Append($@",{property.Id}");
                    }

                    string maskText = strModelProperties.ToString();

                    int multipleReference = 0;
                    IEnumerable<string> findModel = _modelReferenceCol.Where(x =>
                        x == column.ModeloId
                    );
                    if (findModel != null)
                    {
                        multipleReference = findModel.Count();
                    }

                    _slavesCol.Add(
                        new WoItem()
                        {
                            Id = column.Id,
                            Etiqueta = null,
                            MaskText = null,
                            BindedProperty = column.Id,
                            TypeItem = eTypeItem.Slave,
                            Enable = eItemEnabled.Activo,
                            ClassModelType = column.ModeloId,
                            BindingType = "string",
                            Nullable = column.Nulo,
                            Control = "Slave",
                            Parent = "Root",
                            ColSpan = 12,
                            RowSpan = 2,
                            ColumnIndex = 0,
                            RowIndex = 0,
                            SlaveModelId = column.ModeloId,
                            BaseModelName = _modelName,
                            MultipleReference = multipleReference,
                        }
                    );

                    _modelReferenceCol.Add(column.ModeloId);
                }
            }
        }

        #endregion Creación de controles

        #region Validación para cuando hay mas de una referencia

        /// <summary>
        /// Lista de las referencias que van existiendo para validar que no se repitan.
        /// </summary>
        private List<string> _modelReferenceCol = new List<string>();

        #endregion Validación para cuando hay mas de una referencia

        #region Validación de las etiquetas base

        /// <summary>
        /// Lista de las etiquetas que no se encontraron.
        /// </summary>
        private List<string> _labelsNotFound = new List<string>();

        /// <summary>
        /// Valida que todas las etiquetas base se encuentren en el archivo de recursos.
        /// </summary>
        private void AlertToLabels()
        {
            if (_labelsNotFound.Count > 0)
            {
                StringBuilder lostLabels = new StringBuilder();
                foreach (string label in _labelsNotFound)
                {
                    lostLabels.Append($"{label}\n");
                }

                //XtraMessageBox.Show(
                //    text: $@"Faltan las siguientes etiquetas: {lostLabels}",
                //    caption: "Alert",
                //    buttons: MessageBoxButtons.OK,
                //    icon: MessageBoxIcon.Information
                //);
            }
        }

        #endregion Validación de las etiquetas base

        #region Logs

        private WoLog _cantFindModel = new WoLog()
        {
            CodeLog = "000",
            Title = $@"El modelo no se encuentra en el proyecto.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = "ChargeModel"
            }
        };

        private WoLog _rawReady = new WoLog()
        {
            CodeLog = "000",
            Title = $@"Se creo el formulario en raw para el modelo.",
            LogType = eLogType.Common,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = ""
            }
        };

        #endregion Logs
    }
}
