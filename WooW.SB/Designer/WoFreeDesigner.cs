using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using WooW.Core;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerComponents;
using WooW.SB.Designer.DesignerFactory;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;
using WooW.SB.Themes.ThemeHelpers;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer
{
    public partial class WoFreeDesigner : UserControl
    {
        #region Instancias singleton

        /// <summary>
        /// Observador principal.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Instancia singleton de project que contiene toda la información del proyecto base sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Fabricas de componentes

        /// <summary>
        /// Permite generar instancias de LayoutGroupControl, solo
        /// hay que ajustar las propiedades antes de mandar a generar la instancia.
        /// </summary>
        private WoGroupFactory _woGroupFactory { get; set; } = new WoGroupFactory();

        /// <summary>
        /// Permite generar instancias del grupo de tabs, ajustar las propiedades de
        /// esta instancia antes de mandar a llamar la instancia.
        /// </summary>
        private WoTabGroupFactory _woTabGroupFactory { get; set; } = new WoTabGroupFactory();

        #endregion Fabricas de componentes

        #region Atributos que representan los componentes seleccionados

        /// <summary>
        /// Se inicializa en un método dedicado a inicializar el diseñador que se detona desde el constructor.
        /// Representa el diseñador como tal y desde este se puede acceder a Root.
        /// Root se utiliza para poder asignar componentes desde código al diseñador.
        /// </summary>
        private LayoutControl _layoutDesigner = null;

        /// <summary>
        /// Indica el item que el usuario selecciono en el diseñador, en el modo de diseño.
        /// Sera nulo, hasta que el usuario seleccione un item.
        /// Solo puede contener items como: campos de texto, etiquetas, selectores, etc., pero no grupos o tabs.
        /// Se devolverá a nulo cuando el formulario pase a modo de uso o a modo de vista.
        /// </summary>
        private LayoutControlItem _itemSelected = null;

        /// <summary>
        /// Indica el grupo seleccionado por el usuario en el diseñador en modo de diseño.
        /// Sera nulo hasta que el usuario seleccione un grupo.
        /// Solo puede contener grupos y pestañas de la tab (estos igual son grupos), pero no puede contener tabs o items convencionales.
        /// Se devolverá a nulo cuando el formulario pase a modo de uso o a modo de vista.
        /// </summary>
        private LayoutControlGroup _groupSelected = null;

        /// <summary>
        /// Indica el grupo de tabs seleccionado por el usuario en el diseñador en modo de diseño.
        /// Sera nulo hasta que el usuario seleccione un grupo de tabs.
        /// Solo puede contener grupos de tabs, no pestañas de esta ni items convencionales.
        /// Se devolverá a nulo cuando el formulario pase a modo de uso o a modo de vista.
        /// </summary>
        private TabbedControlGroup _tabGroupSelected = null;

        /// <summary>
        /// Permite determinar si el item seleccionado en el modo diseño es un grupo de tabs, grupo o item.
        /// Opciones:
        ///     TabbedControlGroup
        ///     LayoutControlGroup
        ///     LayoutControlItem
        /// </summary>
        private string _typeSelectedControl = string.Empty;

        #endregion Atributos que representan los componentes seleccionados


        #region Constructor

        public WoFreeDesigner()
        {
            _iconPath = $"{_project.DirLayOuts}\\icons";

            InitializeControlerIcons();

            InitializeComponent();

            InitializeDesigner(); //Siempre iniciar primero el diseñador

            ChargeLayoutsGrid();

            InitializeMethodsTree();
        }

        #endregion Constructor


        #region Grid de layout custom

        /// <summary>
        /// Nombre del layout sobre el que se esta trabajando ya sea uno de los ya existentes
        /// o uno recién creado.
        /// </summary>
        string _layoutName = string.Empty;

        /// <summary>
        /// Instancia de la grid con los layouts.
        /// </summary>
        private WoSelectorCustomLayouts _woSelectorCustomLayouts;

        /// <summary>
        /// Instancia y agrega el grid de layouts custom al panel de layouts.
        /// </summary>
        private void ChargeLayoutsGrid()
        {
            _woSelectorCustomLayouts = new WoSelectorCustomLayouts();
            _woSelectorCustomLayouts.Dock = DockStyle.Fill;
            _woSelectorCustomLayouts.LayoutSelectedEvt += LayoutSelected;
            pnlGridLayouts.Controls.Add(_woSelectorCustomLayouts);
            _woSelectorCustomLayouts.ReSelected();
        }

        #endregion Grid de layout custom

        #region Seleccionado de un layout de la grid

        /// <summary>
        /// Método que se detona cada que se selecciona un layout de la grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="container"></param>
        private void LayoutSelected(object sender, WoContainer container)
        {
            _layoutName = container.ModelId;
            _className = container.ModelId;
            _modelName = container.ModelId;
            InitializeDesigner();
            ChargeFormToDesigner(container);

            InitializeWoSyntaxEditor();
        }

        #endregion Seleccionado de un layout de la grid


        #region Crear nuevo layout

        /// <summary>
        /// Crea el json del layout con el diseño del layout designer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="layoutName"></param>
        public void CreateLayout(object sender, string layoutName)
        {
            string pathLayoutName = $@"{_project.DirLayOuts}\FreeStyles\{layoutName}.json";

            if (!File.Exists(pathLayoutName))
            {
                WoDesignerRawSerializerBaseHelper woDesignerRawSerializerBaseHelper =
                    new WoDesignerRawSerializerBaseHelper();
                WoContainer container = woDesignerRawSerializerBaseHelper.BuidBaseRaw(
                    layoutName: layoutName
                );

                WoDirectory.WriteFile(
                    path: pathLayoutName,
                    data: JsonConvert.SerializeObject(container)
                );

                _layoutName = layoutName;
                _className = container.ModelId;

                _woSelectorCustomLayouts.ChargeFreeLayouts();

                BuildBaseClass(layoutName);
            }
        }

        #endregion Crear nuevo layout

        #region Salvar Layout

        /// <summary>
        /// Salva un json con lo del diseñador.
        /// </summary>
        public void SaveLayout()
        {
            WoDesignerSerializerHelper woDesignerSerializerHelper = new WoDesignerSerializerHelper(
                layoutDesigner: _layoutDesigner
            );

            WoContainer container = woDesignerSerializerHelper.SerilizeFormToJson();
            WoDirectory.WriteFile(
                path: $@"{_project.DirLayOuts}\FreeStyles\{_woSelectorCustomLayouts.GetLayoutName()}.json",
                data: JsonConvert.SerializeObject(container)
            );
        }

        #endregion Salvar Layout


        #region Diseñador

        /// <summary>
        /// Inicializa el diseñador.
        /// Detona los métodos para limpiar e inicializar el diseñador
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void InitializeDesigner()
        {
            CleanBaseControls();
            InitializeGridBase();
        }

        /// <summary>
        /// Si el layout esta en uso lo limpia, lo cambia y oculta la ventana del modo de diseño.
        /// Limpia el panel base antes de agregar un diseñador, en caso de que hubiese uno antes.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CleanBaseControls()
        {
            if (_layoutDesigner != null)
            {
                _layoutDesigner.Clear();
                _layoutDesigner.HideCustomizationForm();
                _layoutDesigner = null;
            }

            pnlBaseDesigner.Controls.Clear();
        }

        /// <summary>
        /// Inicializa la variable del _layoutDesigner y configura:
        ///     ventana de diseño con componentes
        ///     modo de diseño para que se trabaje con la grid
        ///     retira las filas y columnas y crea unas nuevas
        ///     suscribe el método para cuando se cambia el foco el modo diseño
        ///     agrega al panel base el diseñador
        ///     inicializa el grupo seleccionado como el root del layout
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeGridBase()
        {
            _layoutDesigner = new LayoutControl();
            _layoutDesigner.RegisterUserCustomizationForm(typeof(DesingFormOptions));
            _layoutDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            _layoutDesigner.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            _layoutDesigner.Root.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
            _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions.Clear();
            _layoutDesigner.Root.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 30;
            _layoutDesigner.ItemSelectionChanged += SelecctionChange_LcLayoutDesigner;
            _layoutDesigner.HideCustomization += ExitCustomization;
            _layoutDesigner.ItemDragging += DragEnter;
            _layoutDesigner.MouseClick += MouseClick_LcLayoutDesigner;

            _layoutDesigner.ItemAdded += LayoutDesigner_ItemAdded;

            List<ColumnDefinition> columnas = new List<ColumnDefinition>();
            for (int i = 0; i < 12; i++)
            {
                columnas.Add(
                    new ColumnDefinition() { SizeType = SizeType.Percent, Width = (100 / 12) }
                );
            }

            _layoutDesigner.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(
                columnas.ToArray()
            );

            for (int i = 0; i < 4; i++)
            {
                _layoutDesigner.Root.OptionsTableLayoutGroup.AddRow();
            }

            pnlBaseDesigner.Controls.Add(_layoutDesigner);
            _groupSelected = _layoutDesigner.Root;

            BuildBaseComponents();
        }

        /// <summary>
        /// Se suscribe al controlador de eventos para cuando se cierra la ventana del editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ExitCustomization(object sender, EventArgs e)
        {
            BlockMode();
            UpdateItemSelected(string.Empty);
        }

        #endregion Diseñador

        #region Cambio del foco del componente

        /// <summary>
        /// Lista de propiedades a ocultar en el property en función del componente seleccionado.
        /// </summary>
        List<string> hideProperties = new List<string>();

        /// <summary>
        /// Lista de propiedades a mostrar en el property en función del componente seleccionado.
        /// </summary>
        List<string> showProperties = new List<string>();

        /// <summary>
        /// Se detona cada que se cambia el foco de un control a otro en el diseñador,
        /// solo en modo de edición.
        /// Asigna la instancia del item que se selecciona a su respectiva variable global para guardar el control y
        /// el resto de viables para los demás tipos lo pasa a nulo, salvo el grupo,
        /// ese lo define a root.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        [SupportedOSPlatform("windows")]
        private void SelecctionChange_LcLayoutDesigner(object sender, EventArgs eventArgs)
        {
            hideProperties.Clear();
            showProperties.Clear();

            ///Todo agregar para cuando
            DesingFormOptions designerFormOptions = (DesingFormOptions)
                _layoutDesigner.CustomizationForm;

            _typeSelectedControl = sender.GetType().Name;
            string idSelectedComponent = string.Empty;

            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected = (sender as LayoutControlItem);
                _groupSelected = _layoutDesigner.Root;
                _tabGroupSelected = null;

                if (_itemSelected.Tag != null)
                {
                    WoComponentProperties properties = (WoComponentProperties)_itemSelected.Tag;
                    properties.Selected = true;

                    if (properties.Control == "Slave")
                    {
                        designerFormOptions.SetComponentProperties(
                            selectedControl: $@"{properties.Id}",
                            componentProperties: properties,
                            showProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize"
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorContainerItem",
                                "ItemSize",
                                "CaptionColor",
                                "CaptionItalic",
                                "CaptionWide",
                                "CaptionDecoration"
                            }
                        );
                    }
                    else if (properties.Control == "Form")
                    {
                        designerFormOptions.SetComponentProperties(
                            selectedControl: properties.Id,
                            componentProperties: properties,
                            showProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize"
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorContainerItem",
                                "ItemSize",
                                "CaptionColor",
                                "CaptionItalic",
                                "CaptionWide",
                                "CaptionDecoration"
                            }
                        );
                    }
                    else
                    {
                        designerFormOptions.SetComponentProperties(
                            selectedControl: $@"{properties.Id}",
                            componentProperties: properties,
                            showProperties: new List<string>()
                            {
                                "ItemSize",
                                "BackgorundColorContainerItem",
                                "CaptionColor",
                                "CaptionItalic",
                                "CaptionWide",
                                "CaptionDecoration"
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize"
                            }
                        );
                    }

                    idSelectedComponent = properties.Id;
                }
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _itemSelected = null;
                _groupSelected = (sender as LayoutControlGroup);
                _tabGroupSelected = null;

                WoComponentProperties properties = (WoComponentProperties)_groupSelected.Tag;
                properties.Selected = true;

                if (properties.TypeContainer == eTypeContainer.FormTab)
                {
                    _typeSelectedControl = "TabbedControlGroup";

                    _tabGroupSelected = (TabbedControlGroup)
                        _layoutDesigner
                            .Items.Where(x => x.Text == properties.Parent)
                            .FirstOrDefault();
                }

                designerFormOptions.SetComponentProperties(
                    selectedControl: $@"{properties.Id}",
                    componentProperties: properties,
                    showProperties: new List<string>()
                    {
                        "BackgorundColorGroup",
                        "ComponentFontSize"
                    },
                    hideProperties: new List<string>()
                    {
                        "BackgorundColorContainerItem",
                        "ItemSize",
                        "CaptionColor",
                        "CaptionItalic",
                        "CaptionWide",
                        "CaptionDecoration"
                    }
                );

                idSelectedComponent = properties.Id;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _itemSelected = null;
                _groupSelected = _layoutDesigner.Root;
                _tabGroupSelected = (sender as TabbedControlGroup);

                _groupSelected = (LayoutControlGroup)
                    _tabGroupSelected.TabPages.Where(x => x.Visible).FirstOrDefault();

                _groupSelected =
                    _groupSelected ?? (LayoutControlGroup)_tabGroupSelected.TabPages.First();

                WoComponentProperties properties = (WoComponentProperties)_groupSelected.Tag;
                properties.Selected = true;
                _groupSelected.Tag = properties;

                designerFormOptions.SetComponentProperties(
                    selectedControl: $@"{properties.Id}. Tab seleccionada: {_groupSelected.Name}",
                    componentProperties: properties,
                    showProperties: new List<string>()
                    {
                        "BackgorundColorGroup",
                        "ComponentFontSize"
                    },
                    hideProperties: new List<string>()
                    {
                        "BackgorundColorContainerItem",
                        "ItemSize",
                        "CaptionColor",
                        "CaptionItalic",
                        "CaptionWide",
                        "CaptionDecoration"
                    }
                );

                idSelectedComponent = properties.Id;
            }

            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                if (!_itemSelected.Visible && _tabAdedd)
                {
                    _tabAdedd = false;
                    WoDesignerSerializerHelper woDesignerSerializerHelper =
                        new WoDesignerSerializerHelper(layoutDesigner: _layoutDesigner);

                    WoContainer container = woDesignerSerializerHelper.SerilizeFormToJson();
                    ChargeJsonLayout(container);
                }
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                if (!_groupSelected.Visible && _tabAdedd)
                {
                    _tabAdedd = false;
                    WoDesignerSerializerHelper woDesignerSerializerHelper =
                        new WoDesignerSerializerHelper(layoutDesigner: _layoutDesigner);

                    WoContainer container = woDesignerSerializerHelper.SerilizeFormToJson();
                    ChargeJsonLayout(container);
                }
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                if (!_tabGroupSelected.Visible && _tabAdedd)
                {
                    _tabAdedd = false;
                    WoDesignerSerializerHelper woDesignerSerializerHelper =
                        new WoDesignerSerializerHelper(layoutDesigner: _layoutDesigner);

                    WoContainer container = woDesignerSerializerHelper.SerilizeFormToJson();
                    ChargeJsonLayout(container);
                }
            }

            UpdateItemSelected(idSelectedComponent);
        }

        /// <summary>
        /// Actualiza el indicador de seleccionado de todos los items
        /// a false cuando se selecciona un nuevo item.
        /// </summary>
        /// <param name="idSelected"></param>
        [SupportedOSPlatform("windows")]
        public void UpdateItemSelected(string idSelected)
        {
            _sizeModifyW = false;
            _sizeModifyH = false;

            _buttonsSizeDynamicCol.Clear();

            var items = _layoutDesigner.Items.Where(x => x.Name != idSelected);

            foreach (var item in items)
            {
                WoComponentProperties properties = (WoComponentProperties)item.Tag;
                properties.Selected = false;
                item.Tag = properties;
            }
        }

        #endregion Cambio del foco del componente


        #region Carga del formulario base

        private string _pathLayout = string.Empty;

        /// <summary>
        /// Permite convertir los tipos del diseñador a tipos de C#.
        /// Se inicializa en el constructor.
        /// </summary>
        private WoDesignerTypeHelper _designerTypeHelper;

        /// <summary>
        /// Define desde donde se carga el formulario, si desde un json
        /// o en caso de no aver ya un layout en json, carga los controles
        /// con un diseño por defecto.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeFormToDesigner(WoContainer woContainer)
        {
            _theme = woContainer.Theme;
            AnalizeTheme();

            DeserializeJsonToForm(woContainer);
        }

        #endregion Carga del formulario base

        #region Cargar formulario desde json

        /// <summary>
        /// Va guardando los textos de los items que se ban agregando.
        /// luego nos permite comparar contra el modelo de forma fácil y
        /// sacar las diferencias para cargar los items que no se an agregado al layout.
        /// </summary>
        private List<string> _itemsAdded = new List<string>();

        /// <summary>
        /// Carga el json en un string y luego pasa una instancia de group
        /// que es el json ya deserializado a un método ocupado de pasarlo al layout.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void DeserializeJsonToForm(WoContainer woContainer)
        {
            _layoutDesigner.Clear();
            ChargeJsonLayout(woContainer);
        }

        /// <summary>
        /// Funciona como método de entrada para la carga del json al layout
        /// verifica que el grupo superior sea la root, y define
        /// las rows en función del json, luego si los sub grupos no son nulos
        /// los recorremos y usamos métodos para generarlos, igual realiza
        /// esto mismo en los items para recorrerlos e irles agregando.
        /// </summary>
        /// <param name="group"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeJsonLayout(WoContainer group)
        {
            _layoutDesigner.Clear();
            if (group.Id == "Root")
            {
                _theme = group.Theme;
                AnalizeTheme();

                if (group.Row > 4)
                {
                    _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    List<RowDefinition> rows = new List<RowDefinition>();
                    for (int i = 0; i < group.Row + 1; i++)
                    {
                        rows.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    }

                    _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(
                        rows.ToArray()
                    );
                }

                _layoutDesigner.Root.Tag = group.ConvertToComponentProperties();
                _layoutDesigner.CustomDraw += ContainerCustomDraw;

                if (group.ContainersCol != null)
                {
                    foreach (var subGroup in group.ContainersCol)
                    {
                        if (subGroup.TypeContainer == eTypeContainer.FormTabGroup)
                        {
                            TabbedControlGroup tabbedControlGroup =
                                _layoutDesigner.Root.AddTabbedGroup(GetTab(subGroup));
                            tabbedControlGroup.CustomDraw += TabGroupCustomDraw;
                        }
                        else
                        {
                            LayoutControlGroup groupLayoutAdded = _layoutDesigner.Root.AddGroup(
                                GetGrupo(subGroup)
                            );
                            groupLayoutAdded.CustomDraw += ContainerCustomDraw;
                        }
                    }
                }

                if (group.ItemsCol != null)
                {
                    _groupSelected = _layoutDesigner.Root;

                    foreach (var item in group.ItemsCol)
                    {
                        _itemsAdded.Add(item.Id);
                        InstanceItem(item: item, isVisible: true);
                    }
                }
            }
        }

        /// <summary>
        /// Cuando uno de los grupos son tabs se utiliza este método que
        /// configura la fabrica con los ajustes del grupo que viene por parámetro
        /// genera la instancia desde el método que usa la fabrica.
        /// Recorre los sub grupos que en este caso al ser un grupo de tabs
        /// serán tabs.
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private TabbedControlGroup GetTab(WoContainer grupo)
        {
            _woTabGroupFactory.ColumnSpan = grupo.ColSpan;
            _woTabGroupFactory.RowSpan = grupo.RowSpan;
            _woTabGroupFactory.ColumnIndex = grupo.ColumnIndex;
            _woTabGroupFactory.RowIndex = grupo.RowIndex;

            TabbedControlGroup tcg = CreateTabGroup(
                internalRows: grupo.Row,
                tabGroupName: grupo.Id,
                tabGroupText: grupo.Etiqueta,
                add: false,
                withTab: false
            );

            if (grupo.ContainersCol != null)
            {
                foreach (var subgrupo in grupo.ContainersCol)
                {
                    tcg.AddTabPage(GetGrupo(subgrupo));
                }
            }

            tcg.Tag = grupo.ConvertToComponentProperties();

            return tcg;
        }

        /// <summary>
        /// En caso de que uno de los sub grupos de root o de algún otro grupo superior
        /// o para las tabs internas dentro de los grupos de tabs, se usa este método
        /// para configurar la fabrica de grupos y generar la instancia desde el método.
        /// Recorre los sub grupos en caso de existir y los va pasando ya sea al método de las tabs
        /// o así mismo en function del tipo de grupo.
        /// Recorre los items que se contienen y este lo envía al método que discierne de cual es cual
        /// genera la instancia y lo carga al layout.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private LayoutControlGroup GetGrupo(WoContainer group)
        {
            _woGroupFactory.ColumnSpan = group.ColSpan;
            _woGroupFactory.RowSpan = group.RowSpan;
            _woGroupFactory.ColumnIndex = group.ColumnIndex;
            _woGroupFactory.RowIndex = group.RowIndex;
            _woGroupFactory.InternalRows = group.Row;

            LayoutControlGroup lcgGrupo = CreateGroup(
                groupText: group.Etiqueta,
                groupName: group.Id,
                add: false
            );

            if (group.ContainersCol != null)
            {
                foreach (var subGroup in group.ContainersCol)
                {
                    if (subGroup.TypeContainer == eTypeContainer.FormTabGroup)
                    {
                        lcgGrupo.AddTabbedGroup(GetTab(subGroup));
                    }
                    else
                    {
                        lcgGrupo.AddGroup(GetGrupo(subGroup));
                    }
                }
            }

            if (!group.ItemsCol.IsNull())
            {
                foreach (var item in group.ItemsCol)
                {
                    _itemsAdded.Add(item.Id);
                    _groupSelected = lcgGrupo;
                    InstanceItem(item: item, isVisible: true);
                }
            }

            lcgGrupo.Tag = group.ConvertToComponentProperties();

            return lcgGrupo;
        }
        #endregion Cargar formulario desde json


        #region Cambiar el modo del designer
        /// <summary>
        /// Guarda el estado en el que se encuentra el formulario.
        /// </summary>
        private eModeDesigner _modeDesigner = eModeDesigner.Block;

        /// <summary>
        /// Opciones de diseño para los controles.
        /// </summary>
        private WoCommonDesignOptions _woCommonDesignOptions = new WoCommonDesignOptions();

        /// <summary>
        /// Controlador de eventos que se detonara cuando se cambie el modo del formulario.
        /// </summary>
        public event EventHandler<eModeDesigner> ModeChanged;

        /// <summary>
        /// Cambia el diseñador al modo del diseño.
        /// Permite editar el formulario, permite agregar y quitar controles.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void DesingMode()
        {
            pnlBaseDesigner.Enabled = true;
            _layoutDesigner.ShowCustomizationForm();
            _modeDesigner = eModeDesigner.Design;

            ModeChanged?.Invoke(this, _modeDesigner);

            DesingFormOptions designerFormOptions = (DesingFormOptions)
                _layoutDesigner.CustomizationForm;

            WoComponentProperties componentPropertiesRoot = (WoComponentProperties)
                _layoutDesigner.Root.Tag;

            designerFormOptions.ApliedThemeEvt += AplyTheme;

            designerFormOptions.PropertyChangedEvt += UpdateComponent;

            designerFormOptions.SetTheme(_theme);

            designerFormOptions.Size = new Size(400, 800);
            designerFormOptions.Location = new System.Drawing.Point(100, 100);

            ChargeClass();
        }

        /// <summary>
        /// Cambia al modo de bloqueo el formulario.
        /// solo para una vista rápida pero no permite ni uso, ni modificación.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void BlockMode()
        {
            pnlBaseDesigner.Enabled = false;
            _layoutDesigner.HideCustomizationForm();
            _modeDesigner = eModeDesigner.Block;

            ModeChanged?.Invoke(this, _modeDesigner);
        }

        #endregion Cambiar el modo del designer

        #region Temas

        /// <summary>
        /// Indica el tema recuperado desde el json.
        /// </summary>
        private string _theme = "Default";

        /// <summary>
        /// INstancia del analizador de temas, para leer el css.
        /// </summary>
        private WoThemeAnalizer _woThemeAnalizer = new WoThemeAnalizer();

        /// <summary>
        /// Propiedades recuperadas desde el css del tema.
        /// </summary>
        private WoThemeOptions _woThemeOptions = WoThemeOptions.GetInstance();

        /// <summary>
        /// Analiza el tema que se esta utilizando.
        /// </summary>
        private void AnalizeTheme()
        {
            _woThemeAnalizer.ReadCss(_theme);
        }

        /// <summary>
        /// Se detona cuando el diseñador de temas realiza una modificación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="theme"></param>
        private void AplyTheme(object sender, string theme)
        {
            WoComponentProperties componentProperties = (WoComponentProperties)
                _layoutDesigner.Root.Tag;
            componentProperties.Theme = theme;
            _layoutDesigner.Root.Tag = componentProperties;

            foreach (var item in _layoutDesigner.Items)
            {
                string controlType = item.GetType().Name;
                if (controlType == "LayoutControlItem")
                {
                    _itemSelected = (LayoutControlItem)item;
                    componentProperties = (WoComponentProperties)_itemSelected.Tag;
                    componentProperties.Theme = theme;
                    _itemSelected.Tag = componentProperties;

                    _itemSelected.Update();
                }
                if (controlType == "LayoutControlGroup")
                {
                    _groupSelected = (LayoutControlGroup)item;
                    componentProperties = (WoComponentProperties)_groupSelected.Tag;
                    componentProperties.Theme = theme;
                    _groupSelected.Tag = componentProperties;

                    _groupSelected.Update();
                }
                else if (controlType == "TabbedControlGroup")
                {
                    _tabGroupSelected = (TabbedControlGroup)item;
                    componentProperties = (WoComponentProperties)_tabGroupSelected.Tag;
                    componentProperties.Theme = theme;
                    _tabGroupSelected.Tag = componentProperties;

                    _tabGroupSelected.Update();
                }
            }
        }

        #endregion Temas

        #region Actualización y re dibujado base

        /// <summary>
        /// Permite actualizar el componente seleccionado con la información en la
        /// instancia de WoComponentPorperties que se recibe por parámetro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="woComponentProperties"></param>
        [SupportedOSPlatform("windows")]
        private void UpdateComponent(object sender, WoComponentProperties woComponentProperties)
        {
            if (woComponentProperties.ChangedProperty == "Etiqueta")
            {
                ChangeTitle(woComponentProperties.Etiqueta, woComponentProperties.MaskText);
            }

            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                if (woComponentProperties.ChangedProperty == "Id")
                {
                    _woSyntaxManagerUserCode.RenameMethod(
                        oldName: $@"{woComponentProperties.OldValue}_OnChange",
                        newName: $@"{woComponentProperties.Id}_OnChange"
                    );

                    _woSyntaxManagerUserCode.RenameMethod(
                        oldName: $@"{woComponentProperties.OldValue}_OnFocus",
                        newName: $@"{woComponentProperties.Id}_OnFocus"
                    );

                    _woSyntaxManagerUserCode.RenameMethod(
                        oldName: $@"{woComponentProperties.OldValue}_OnBlur",
                        newName: $@"{woComponentProperties.Id}_OnBlur"
                    );

                    string view = GetViewName(woComponentProperties.Control);

                    _woSyntaxManagerModelClass.RenameAttribute(
                        type: view,
                        value: $@"new {view}();",
                        oldName: woComponentProperties.OldValue.ToString(),
                        newName: woComponentProperties.Id,
                        accessModifier: "public",
                        classType: "Controls"
                    );
                }

                woComponentProperties.ChangedProperty = string.Empty;
                woComponentProperties.OldValue = null;

                RePaintItem(woComponentProperties);
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                if (woComponentProperties.TypeContainer == eTypeContainer.FormTab)
                {
                    RePaintTab(woComponentProperties);
                }
                else
                {
                    RePaintContainer(woComponentProperties);
                }
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                RePaintTab(woComponentProperties);
            }
        }

        /// <summary>
        /// Re dibuja el item seleccionado en función del parámetro que se recibe y actualiza
        /// el tag del item con la instancia de WoComponentProperties que se recibe por parámetro.
        /// </summary>
        /// <param name="woComponentPropertiesStyle"></param>
        [SupportedOSPlatform("windows")]
        private void RePaintItem(WoComponentProperties woComponentPropertiesStyle)
        {
            if (woComponentPropertiesStyle.Id != _itemSelected.Name)
            {
                var response = _layoutDesigner
                    .Items.Where(x => x.Name == woComponentPropertiesStyle.Id)
                    .FirstOrDefault();
                if (response == null)
                {
                    _itemSelected.Name = woComponentPropertiesStyle.Id;
                }
                else
                {
                    MessageBox.Show(
                        "EL id no se puede duplicar para dos componentes iguales",
                        "Alerta"
                    );
                    woComponentPropertiesStyle.Id = _itemSelected.Name;
                }
            }
            _itemSelected.Tag = woComponentPropertiesStyle;
            _itemSelected.Update();
        }

        /// <summary>
        /// Re dibuja el contenedor en función del parámetro que se recibe y actualiza
        /// el tag del contenedor con la instancia de WoComponentProperties que se recibe por parámetro.
        /// </summary>
        /// <param name="woComponentPropertiesStyle"></param>
        [SupportedOSPlatform("windows")]
        private void RePaintContainer(WoComponentProperties woComponentPropertiesStyle)
        {
            if (woComponentPropertiesStyle.Id != _groupSelected.Name)
            {
                var response = _layoutDesigner
                    .Items.Where(x => x.Name == woComponentPropertiesStyle.Id)
                    .FirstOrDefault();
                if (response == null)
                {
                    _groupSelected.Name = woComponentPropertiesStyle.Id;
                }
                else
                {
                    MessageBox.Show(
                        "EL id no se puede duplicar para dos componentes iguales",
                        "Alerta"
                    );
                    woComponentPropertiesStyle.Id = _groupSelected.Name;
                }
            }
            _groupSelected.Tag = woComponentPropertiesStyle;
            _groupSelected.Update();
        }

        /// <summary>
        /// Re dibuja el tab en función del parámetro que se recibe y actualiza
        /// el tag del contenedor con la instancia de WoComponentProperties que se recibe por parámetro.
        /// </summary>
        /// <param name="woComponentPropertiesStyle"></param>
        [SupportedOSPlatform("windows")]
        private void RePaintTab(WoComponentProperties woComponentPropertiesStyle)
        {
            if (woComponentPropertiesStyle.Id != _groupSelected.Name)
            {
                var response = _layoutDesigner
                    .Items.Where(x => x.Name == woComponentPropertiesStyle.Id)
                    .FirstOrDefault();
                if (response == null)
                {
                    _groupSelected.Name = woComponentPropertiesStyle.Id;
                }
                else
                {
                    MessageBox.Show(
                        "EL id no se puede duplicar para dos componentes iguales",
                        "Alerta"
                    );
                    woComponentPropertiesStyle.Id = _groupSelected.Name;
                }
            }
            _groupSelected.Tag = woComponentPropertiesStyle;
            _tabGroupSelected.Update();
        }

        #endregion Actualización y re dibujado base

        #region Modificación del formulario por drag and drop

        private List<(string parent, int colIndex, int labelSize)> _labelSizes =
            new List<(string parent, int colIndex, int labelSize)>();

        //Revisar
        [SupportedOSPlatform("windows")]
        public void DragEnter(object sender, ItemDraggingEventArgs e)
        {
            _labelSizes.Clear();
        }

        #endregion Modificación del formulario por drag and drop


        #region Gestión de clases

        /// <summary>
        /// Instancia del gestor de la clase
        /// </summary>
        private WoSyntaxManagerUserCode _woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();

        /// <summary>
        /// Instancia del gestor de la clase con los modelos del layout.
        /// </summary>
        private WoSyntaxManagerModelClass _woSyntaxManagerModelClass =
            new WoSyntaxManagerModelClass();

        /// <summary>
        /// Inicializa las clases que manejan la sintaxis de las views y el código del usuario.
        /// </summary>
        /// <param name="modelName"></param>
        private void ChargeClass()
        {
            _woSyntaxManagerUserCode.InitializeManager(
                pathScript: $@"{_project.DirLayOuts}\UserCodeFreeStyle\{_modelName}_proj\{_modelName}ScriptsUser.cs",
                className: _className,
                modelName: _modelName
            );

            _woSyntaxManagerModelClass.InitializeManager(
                pathScript: $@"{_project.DirLayOuts}\UserCodeFreeStyle\{_modelName}_proj\{_modelName}Controls.cs",
                className: _className,
                modelName: _modelName
            );
        }

        #endregion Gestión de clases


        #region Eliminar item o grupo seleccionado

        /// <summary>
        /// Elimina todos los grupos e items del diseñador.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CleanAll()
        {
            RemoveItems();

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            foreach (var item in _layoutDesigner.Items)
            {
                string type = item.GetType().Name;
                switch (type)
                {
                    case "LayoutControlGroup":
                        _groupSelected = (LayoutControlGroup)item;
                        if ((!(_groupSelected.Name == "Root")) && _groupSelected.Name != "")
                        {
                            _layoutDesigner.Remove(_groupSelected, true);
                        }
                        break;
                    case "TabbedControlGroup":
                        _tabGroupSelected = (TabbedControlGroup)item;
                        if (_tabGroupSelected.Name != "")
                        {
                            _layoutDesigner.Remove(_tabGroupSelected, true);
                        }
                        break;
                }
            }
            _groupSelected = _layoutDesigner.Root;

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Elimina unicamente todos los item control del diseñador.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void RemoveItems()
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            foreach (var item in _layoutDesigner.Items)
            {
                string tipo = item.GetType().Name;
                if (tipo == "LayoutControlItem" || tipo == "SimpleLabelItem")
                {
                    _itemSelected = (LayoutControlItem)item;
                    _layoutDesigner.Remove(_itemSelected, true);
                    _itemSelected = null;
                }
            }

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Elimina el control seleccionado.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void DeleteControl(bool onlyTab = false)
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            if (onlyTab)
            {
                if (_tabGroupSelected.TabPages.Count == 1)
                {
                    onlyTab = false;
                }
            }

            if (
                (
                    _typeSelectedControl == "LayoutControlItem"
                    || _typeSelectedControl == "SimpleLabelItem"
                )
                && _itemSelected != null
                && !onlyTab
            )
            {
                _woSyntaxManagerUserCode.DeleteMethod($@"{_itemSelected.Name}_OnChange");
                _woSyntaxManagerUserCode.DeleteMethod($@"{_itemSelected.Name}_OnFocus");
                _woSyntaxManagerUserCode.DeleteMethod($@"{_itemSelected.Name}_OnBlur");

                WoComponentProperties properties = (WoComponentProperties)_itemSelected.Tag;
                string view = GetViewName(properties.Control);

                _woSyntaxManagerModelClass.DeleteAttribute(
                    selectedAttribute: $@"public {view} {_itemSelected.Name} {{ get; set; }} = new {view}();",
                    classType: "Controls"
                );

                _layoutDesigner.Remove(_itemSelected, true);
                _itemSelected = null;
            }
            else if (
                (
                    _typeSelectedControl == "TabbedControlGroup"
                    || _typeSelectedControl == "LayoutControlGroup"
                )
                && _tabGroupSelected != null
                && !onlyTab
            )
            {
                if (_tabGroupSelected.TabPages.Count > 0)
                    DeleteTab(_tabGroupSelected);
                _layoutDesigner.Remove(_tabGroupSelected, true);
                _tabGroupSelected = null;
            }
            else if (
                (_typeSelectedControl == "LayoutControlGroup" || onlyTab)
                && _groupSelected != null
            )
            {
                if (_groupSelected.Items.Count > 0)
                    DeleteGroup(_groupSelected);
                _layoutDesigner.Remove(_groupSelected, true);
                _groupSelected = null;
            }

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Elimina el grupo de tabs.
        /// Recorre cada grupo del grupo de tabs.
        /// </summary>
        /// <param name="tab"></param>
        [SupportedOSPlatform("windows")]
        private void DeleteTab(TabbedControlGroup tab)
        {
            foreach (dynamic grupo in tab.TabPages)
            {
                DeleteGroup(grupo);
            }
        }

        /// <summary>
        /// Elimina el grupo y todo lo que contenga internamente
        /// ya sea un tab o otro grupo.
        /// </summary>
        /// <param name="grupo"></param>
        [SupportedOSPlatform("windows")]
        private void DeleteGroup(LayoutControlGroup grupo)
        {
            List<LayoutControlItem> items = new List<LayoutControlItem>();

            foreach (dynamic item in grupo.Items)
            {
                if (item.TypeName == "LayoutGroup")
                {
                    DeleteGroup(item);
                }
                else if (item.TypeName == "TabbedGroup")
                {
                    DeleteTab(item);
                }
                else
                {
                    try
                    {
                        items.Add((LayoutControlItem)item);
                    }
                    catch (Exception ex)
                    {
                        /// ToDo: enviar a un log,
                        /// No debería de entrar pero puede que pase con uno de los contenedores no indexados.
                    }
                }
            }

            if (items.Count > 0)
            {
                ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
                this.SuspendLayout();

                foreach (LayoutControlItem item in items)
                {
                    _layoutDesigner.Remove(_itemSelected, true);
                    //item.HideToCustomization();
                }

                ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }
        #endregion Eliminar item o grupo seleccionado

        #region Tamaño del item o grupo seleccionado
        /// <summary>
        /// Reduce el ancho del control seleccionado en numero de columnas
        /// (item, grupo o grupo de tabs)
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void ReduceWidth()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                if (_itemSelected.OptionsTableLayoutItem.ColumnSpan == 2)
                    _observer.SetLog(_controlSize);

                _itemSelected.OptionsTableLayoutItem.ColumnSpan =
                    (_itemSelected.OptionsTableLayoutItem.ColumnSpan == 2)
                        ? _itemSelected.OptionsTableLayoutItem.ColumnSpan
                        : _itemSelected.OptionsTableLayoutItem.ColumnSpan - 1;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                if (_groupSelected.OptionsTableLayoutItem.ColumnSpan == 3)
                    _observer.SetLog(_controlSize);

                _groupSelected.OptionsTableLayoutItem.ColumnSpan =
                    (_groupSelected.OptionsTableLayoutItem.ColumnSpan == 3)
                        ? _groupSelected.OptionsTableLayoutItem.ColumnSpan
                        : _groupSelected.OptionsTableLayoutItem.ColumnSpan - 1;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                if (_tabGroupSelected.OptionsTableLayoutItem.ColumnSpan == 3)
                    _observer.SetLog(_controlSize);

                _tabGroupSelected.OptionsTableLayoutItem.ColumnSpan =
                    (_tabGroupSelected.OptionsTableLayoutItem.ColumnSpan == 3)
                        ? _tabGroupSelected.OptionsTableLayoutItem.ColumnSpan
                        : _tabGroupSelected.OptionsTableLayoutItem.ColumnSpan - 1;
            }

            _labelSizes.Clear();
        }

        /// <summary>
        /// Aumenta el ancho del control seleccionado en numero de columnas
        /// (item, grupo o grupo de tabs)
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void IncreaseWidth()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected.OptionsTableLayoutItem.ColumnSpan++;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.OptionsTableLayoutItem.ColumnSpan++;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _tabGroupSelected.OptionsTableLayoutItem.ColumnSpan++;
            }

            _labelSizes.Clear();
        }

        /// <summary>
        /// Reduce el largo del control seleccionado en numero de renglones (filas)
        /// (item, grupo o grupo de tabs)
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void ReduceHeight()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected.OptionsTableLayoutItem.RowSpan--;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.OptionsTableLayoutItem.RowSpan--;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _tabGroupSelected.OptionsTableLayoutItem.RowSpan--;
            }

            _labelSizes.Clear();
        }

        /// <summary>
        /// Aumenta el largo del control seleccionado en numero de renglones (filas)
        /// (item, grupo o grupo de tabs)
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void IncreaseHeight()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected.OptionsTableLayoutItem.RowSpan++;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.OptionsTableLayoutItem.RowSpan++;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _tabGroupSelected.OptionsTableLayoutItem.RowSpan++;
            }

            _labelSizes.Clear();
        }
        #endregion Tamaño del item o grupo seleccionado

        #region Movimiento del item o grupo seleccionado
        /// <summary>
        /// Mueve el control o grupo seleccionado al la derecha.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void MoveRight()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected.OptionsTableLayoutItem.ColumnIndex++;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.OptionsTableLayoutItem.ColumnIndex++;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _tabGroupSelected.OptionsTableLayoutItem.ColumnIndex++;
            }

            _labelSizes.Clear();
        }

        /// <summary>
        /// Mueve el control o grupo seleccionado al la izquierda.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void MoveLeft()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected.OptionsTableLayoutItem.ColumnIndex =
                    (_itemSelected.OptionsTableLayoutItem.ColumnIndex == 0)
                        ? 0
                        : _itemSelected.OptionsTableLayoutItem.ColumnIndex - 1;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.OptionsTableLayoutItem.ColumnIndex =
                    (_groupSelected.OptionsTableLayoutItem.ColumnIndex == 0)
                        ? 0
                        : _groupSelected.OptionsTableLayoutItem.ColumnIndex - 1;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _tabGroupSelected.OptionsTableLayoutItem.ColumnIndex =
                    (_tabGroupSelected.OptionsTableLayoutItem.ColumnIndex == 0)
                        ? 0
                        : _tabGroupSelected.OptionsTableLayoutItem.ColumnIndex - 1;
            }

            _labelSizes.Clear();
        }

        /// <summary>
        /// Mueve el control o grupo seleccionado un espacio arriba.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void MoveTop()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected.OptionsTableLayoutItem.RowIndex =
                    (_itemSelected.OptionsTableLayoutItem.RowIndex == 0)
                        ? 0
                        : _itemSelected.OptionsTableLayoutItem.RowIndex - 1;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.OptionsTableLayoutItem.RowIndex =
                    (_groupSelected.OptionsTableLayoutItem.RowIndex == 0)
                        ? 0
                        : _groupSelected.OptionsTableLayoutItem.RowIndex - 1;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _tabGroupSelected.OptionsTableLayoutItem.RowIndex =
                    (_tabGroupSelected.OptionsTableLayoutItem.RowIndex == 0)
                        ? 0
                        : _tabGroupSelected.OptionsTableLayoutItem.RowIndex - 1;
            }

            _labelSizes.Clear();
        }

        /// <summary>
        /// Mueve el control o grupo seleccionado un espacio abajo.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void MoveDown()
        {
            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                _itemSelected.OptionsTableLayoutItem.RowIndex =
                    (_itemSelected.OptionsTableLayoutItem.RowIndex == 0)
                        ? 0
                        : _itemSelected.OptionsTableLayoutItem.RowIndex + 1;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.OptionsTableLayoutItem.RowIndex =
                    (_groupSelected.OptionsTableLayoutItem.RowIndex == 0)
                        ? 0
                        : _groupSelected.OptionsTableLayoutItem.RowIndex + 1;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _tabGroupSelected.OptionsTableLayoutItem.RowIndex =
                    (_tabGroupSelected.OptionsTableLayoutItem.RowIndex == 0)
                        ? 0
                        : _tabGroupSelected.OptionsTableLayoutItem.RowIndex + 1;
            }

            _labelSizes.Clear();
        }
        #endregion Movimiento del item o grupo seleccionado

        #region Cambio de titulo

        /// <summary>
        /// Cambia el key de la etiqueta del label del control.
        /// </summary>
        /// <param name="labelId"></param>
        [SupportedOSPlatform("windows")]
        public void ChangeTitle(string labelId, string labelText)
        {
            WoComponentProperties properties = new WoComponentProperties();

            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                properties = (WoComponentProperties)_itemSelected.Tag;

                properties.Etiqueta = labelId;
                properties.MaskText = labelText;

                _itemSelected.Text = labelText;

                _itemSelected.Tag = properties;
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                _groupSelected.Text = labelText;

                properties = (WoComponentProperties)_groupSelected.Tag;

                properties.Etiqueta = labelId;
                properties.MaskText = labelText;

                _groupSelected.Text = labelText;

                _groupSelected.Tag = properties;
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                _groupSelected = (LayoutControlGroup)
                    _tabGroupSelected.TabPages.Where(x => x.Visible).FirstOrDefault();

                properties = (WoComponentProperties)_groupSelected.Tag;

                properties.Etiqueta = labelId;
                properties.MaskText = labelText;

                _groupSelected.Text = labelText;

                _groupSelected.Tag = properties;
            }

            UpdateComponent(null, properties);
        }

        #endregion Cambio de titulo

        #region Ajustes de rows

        /// <summary>
        /// Agrega un row(renglón / fila) al grupo seleccionado.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void AddRow()
        {
            if (
                _typeSelectedControl == "LayoutControlGroup"
                || _typeSelectedControl == "TabbedControlGroup"
            )
            {
                try
                {
                    _groupSelected.OptionsTableLayoutGroup.AddRow();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Seleccione un grupo!");
                    _groupSelected = _layoutDesigner.Root;
                }
            }
        }

        /// <summary>
        /// ELimina un row(renglón / fila) al grupo seleccionado.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void DeleteRow()
        {
            if (
                (
                    _typeSelectedControl == "LayoutControlGroup"
                    || _typeSelectedControl == "TabbedControlGroup"
                )
                && _groupSelected.OptionsTableLayoutGroup.RowCount > 0
            )
            {
                _groupSelected.OptionsTableLayoutGroup.RemoveRowAt(
                    _groupSelected.OptionsTableLayoutGroup.RowCount - 1
                );
            }
        }
        #endregion Ajustes de rows


        #region Recuperar row

        /// <summary>
        /// En función del control recupera el nombre de la clase de la view para blazor.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private string GetViewName(string control)
        {
            string view = "";

            if (control == "Text")
            {
                view = "WoTextEditView";
            }
            else if (control == "Date")
            {
                view = "WoDateEditView";
            }
            else if (control == "Spin")
            {
                view = "WoSpinEditView";
            }
            else if (control == "Decimal")
            {
                view = "WoMaskedEditView";
            }
            else if (control == "Bool")
            {
                view = "WoMaskedEditView";
            }
            else if (control == "Memo")
            {
                view = "WoMemoEditView";
            }
            else if (control == "WoState" || control == "EnumInt" || control == "EnumString")
            {
                view = "WoComboEnumEditView";
            }
            else if (control == "LookUp")
            {
                view = "WoLookUpEditView";
            }
            else if (control == "LookUpDialog")
            {
                view = "WoLookUpDialogEditView";
            }
            else
            {
                view = "WoMaskedEditView";
            }

            return view;
        }

        #endregion Recuperar row


        #region Alertas

        private WoLog _controlSize = new WoLog()
        {
            CodeLog = "000",
            Title = "La propiedad de tamaño del control no puede ser inferior a 3.",
            Details = "La propiedad ColSize del control no puede ser inferior a 3.",
            UserMessage = "La propiedad de tamaño del control no puede ser inferior a 3.",
            LogType = eLogType.Warning,
            FileDetails = new WoFileDetails()
            {
                Class = "WoLabelAttributeManager",
                MethodOrContext = "ReduceWidth"
            }
        };

        private WoLog _cantFindModel = new WoLog()
        {
            CodeLog = "000",
            Title = $@"El modelo no se encuentra el proyecto.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = "ChargeModel"
            }
        };

        #endregion Alertas

        private void grdViewMethods_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        ) { }
    }
}
