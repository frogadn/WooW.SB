using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using ServiceStack.OrmLite;
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
    public partial class WoFormDesigner : UserControl
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

        #region Variables publicas y globales

        /// <summary>
        /// Permite generar instancias de LayoutGroupControl, solo
        /// hay que ajustar las propiedades antes de mandar a generar la instancia.
        /// </summary>
        public WoGroupFactory WoGroupFactory { get; set; } = new WoGroupFactory();

        /// <summary>
        /// Permite generar instancias del grupo de tabs, ajustar las propiedades de
        /// esta instancia antes de mandar a llamar la instancia.
        /// </summary>
        public WoTabGroupFactory WoTabGroupFactory { get; set; } = new WoTabGroupFactory();

        /// <summary>
        /// Controlador de eventos que se detona cuando se realiza una edición, sobre el formulario
        /// </summary>
        public EventHandler<bool> EditingEvt { get; set; }

        #endregion Variables publicas y globales

        #region Variables privadas y globales

        /// <summary>
        /// Se inicializa en el constructor.
        /// Representa una instancia del modelo que se encuentra en la columna json oculta de la grid.
        /// </summary>
        private Modelo _modelJson;

        /// <summary>
        /// Se revive como parámetro en el constructor e indica
        /// si el formulario es para un request, una esclava o un formulario normal.
        /// </summary>
        private string _typeModel = string.Empty;

        /// <summary>
        /// Nombre del modelo sobre el que se esta trabajando.
        /// Ej: Alumno o CfgMoneda.
        /// </summary>
        private string _modelName = string.Empty;

        #endregion Variables privadas y globales

        #region General paths

        /// <summary>
        /// Indica la ruta base de donde se salvan los json o recursos del sistema en general.
        /// ToDo: Mover a una ruta general.
        /// </summary>
        private string _pathBase = string.Empty;

        /// <summary>
        /// Contendrá la ruta del path temporal para salvar el json en raw.
        /// </summary>
        private string _pathTemp = string.Empty;

        /// <summary>
        /// Se inicializa en el constructor.
        /// Es la ruta del archivo json del layout que diseña el usuario.
        /// </summary>
        private string _pathLayout = string.Empty;

        #endregion General paths

        #region Control selected

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

        #endregion Control selected


        #region Constructores

        /// <summary>
        /// Constructor único y principal de la clase.
        /// Inicializa las variables globales.
        /// Carga el dll en un assembly.
        /// Obtienen el tipo y genera la instancia.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="className"></param>
        /// <param name="modelJson"></param>
        [SupportedOSPlatform("windows")]
        public WoFormDesigner(string typeModel, string modelName, Modelo modelJson)
        {
            InitializeComponent();

            InitializeControlerIcons();

            _modelJson = modelJson;

            _typeModel = typeModel;

            _modelName = modelName;

            _pathBase = $@"{_project.DirProyectData}";

            _pathLayout = $@"{_pathBase}/LayOuts/FormDesign/{_modelName}.json";

            _iconPath = $"{_project.DirLayOuts}\\icons";

            _pathTemp = _project.DirProyectTemp;

            _designerTypeHelper = new WoDesignerTypeHelper(_modelName);

            InitializeDesigner();
        }
        #endregion Constructores


        #region Diseñador

        /// <summary>
        /// Inicializa el diseñador.
        /// Detona los métodos para limpiar e inicializar el diseñador
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void InitializeDesigner()
        {
            ChargeClass();

            CleanBaseControls();
            InitializeGridBase();

            BuildBaseComponents();
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
            _layoutDesigner.Root.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 10;
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

            for (int i = 0; i < 6; i++)
            {
                _layoutDesigner.Root.OptionsTableLayoutGroup.AddRow();
                _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions[i].Height = 10;
            }

            pnlBaseDesigner.Controls.Add(_layoutDesigner);
            _groupSelected = _layoutDesigner.Root;

            ChargeFormToDesigner();
        }

        /// <summary>
        /// Se suscribe al controlador de eventos para cuando se cierra la ventana del editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ExitCustomization(object sender, EventArgs e)
        {
            pnlBaseDesigner.Enabled = false;
            _modeDesigner = eModeDesigner.Block;

            UpdateItemSelected(string.Empty);

            ModeChanged?.Invoke(this, _modeDesigner);
        }

        #endregion Diseñador

        #region Cambio del foco del componente

        /// <summary>
        /// LIsta de propiedades que se ocultaran cuando se seleccione un tipo de control.
        /// </summary>
        List<string> hideProperties = new List<string>();

        /// <summary>
        /// Lista de propiedades que se mostrara cuando se seleccione un tipo de control.
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
                                "ComponentFontSize",
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorContainerItem",
                                "ItemSize",
                                "CaptionColor",
                                "CaptionItalic",
                                "CaptionWide",
                                "CaptionDecoration",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "PlaceHolder",
                                "Password",
                                "LookUpInputSize",
                                "Max",
                                "Min",
                                "Step",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }
                    else if (properties.Control == "Text")
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
                                "CaptionDecoration",
                                "PlaceHolder",
                                "Password"
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "LookUpInputSize",
                                "Max",
                                "Min",
                                "Step",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }
                    else if (properties.Control == "Memo")
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
                                "CaptionDecoration",
                                "PlaceHolder",
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "Password",
                                "LookUpInputSize",
                                "Max",
                                "Min",
                                "Step",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }
                    else if (properties.Control == "Spin")
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
                                "CaptionDecoration",
                                "PlaceHolder",
                                "Max",
                                "Min",
                                "Step",
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "LookUpInputSize",
                                "Password",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }
                    else if (properties.Control == "Date")
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
                                "CaptionDecoration",
                                "PlaceHolder",
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "LookUpInputSize",
                                "Password",
                                "Max",
                                "Min",
                                "Step",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }
                    else if (
                        properties.Control == "LookUp"
                        || properties.Control == "LookUpDialog"
                        || properties.Control == "EnumString"
                        || properties.Control == "EnumInt"
                    )
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
                                "CaptionDecoration",
                                "LookUpInputSize",
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "Password",
                                "PlaceHolder",
                                "Max",
                                "Min",
                                "Step",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }
                    else if (properties.Control == "Bool")
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
                                "CaptionDecoration",
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "LookUpInputSize",
                                "PlaceHolder",
                                "Password",
                                "Max",
                                "Min",
                                "Step",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }
                    else if (properties.Control == "TextMask")
                    {
                        List<string> showProperties = new List<string>()
                        {
                            "ItemSize",
                            "BackgorundColorContainerItem",
                            "CaptionColor",
                            "CaptionItalic",
                            "CaptionWide",
                            "CaptionDecoration",
                            "PlaceHolder"
                        };

                        List<string> hideProperties = new List<string>()
                        {
                            "BackgorundColorGroup",
                            "ComponentFontSize",
                            "Visible",
                            "WidthSlave",
                            "HeightSlave",
                            "Icon",
                            "Password",
                            "LookUpInputSize",
                            "Max",
                            "Min",
                            "Step",
                            "Step",
                        };

                        if (
                            properties.BindingType == "string"
                            || properties.BindingType == "string?"
                            || properties.BindingType == "String"
                            || properties.BindingType == "String?"
                        )
                        {
                            showProperties.Add("InputString");

                            hideProperties.Add("InputNumeric");
                            hideProperties.Add("InputDate");
                        }
                        else if (
                            properties.BindingType == "DateTime"
                            || properties.BindingType == "DateTime?"
                        )
                        {
                            showProperties.Add("InputDate");

                            hideProperties.Add("InputString");
                            hideProperties.Add("InputNumeric");
                        }
                        else if (
                            properties.BindingType == "int"
                            || properties.BindingType == "int?"
                            || properties.BindingType == "Decimal"
                            || properties.BindingType == "Decimal?"
                            || properties.BindingType == "double"
                            || properties.BindingType == "double?"
                            || properties.BindingType == "long"
                            || properties.BindingType == "long?"
                        )
                        {
                            showProperties.Add("InputNumeric");

                            hideProperties.Add("InputString");
                            hideProperties.Add("InputDate");
                        }

                        if (
                            properties.InputDate == eInputDate.Custom
                            || properties.InputNumeric == eInputNumeric.Custom
                            || showProperties.Contains("InputString")
                        )
                        {
                            showProperties.Add("CustomMask");
                        }
                        else
                        {
                            hideProperties.Add("CustomMask");
                        }

                        designerFormOptions.SetComponentProperties(
                            selectedControl: $@"{properties.Id}",
                            componentProperties: properties,
                            showProperties: showProperties,
                            hideProperties: hideProperties
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
                                "CaptionDecoration",
                                "PlaceHolder",
                            },
                            hideProperties: new List<string>()
                            {
                                "BackgorundColorGroup",
                                "ComponentFontSize",
                                "Visible",
                                "WidthSlave",
                                "HeightSlave",
                                "Icon",
                                "Password",
                                "LookUpInputSize",
                                "Max",
                                "Min",
                                "Step",
                                "CustomMask",
                                "InputString",
                                "InputNumeric",
                                "InputDate"
                            }
                        );
                    }

                    designerFormOptions.ChangeVisibleProperties(
                        hide: properties.Id == "Alertas" || properties.Id == "Controles"
                    );

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

                    _tabGroupSelected = (TabbedControlGroup)_groupSelected.ParentTabbedGroup;
                }

                if (properties.IsSlavePooper)
                {
                    designerFormOptions.SetComponentProperties(
                        selectedControl: $@"{properties.Id}",
                        componentProperties: properties,
                        showProperties: new List<string>()
                        {
                            "BackgorundColorGroup",
                            "ComponentFontSize",
                            "Visible",
                            "WidthSlave",
                            "HeightSlave",
                            "Icon"
                        },
                        hideProperties: new List<string>()
                        {
                            "BackgorundColorContainerItem",
                            "ItemSize",
                            "CaptionColor",
                            "CaptionItalic",
                            "CaptionWide",
                            "CaptionDecoration",
                            "PlaceHolder",
                            "Password",
                            "LookUpInputSize"
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
                            "BackgorundColorGroup",
                            "ComponentFontSize",
                            "Visible",
                            "Icon"
                        },
                        hideProperties: new List<string>()
                        {
                            "BackgorundColorContainerItem",
                            "ItemSize",
                            "CaptionColor",
                            "CaptionItalic",
                            "CaptionWide",
                            "CaptionDecoration",
                            "WidthSlave",
                            "HeightSlave",
                            "PlaceHolder",
                            "Password",
                            "LookUpInputSize"
                        }
                    );
                }

                idSelectedComponent = properties.Id;

                designerFormOptions.ChangeVisibleProperties(hide: false);
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
                        "ComponentFontSize",
                        "Icon"
                    },
                    hideProperties: new List<string>()
                    {
                        "BackgorundColorContainerItem",
                        "ItemSize",
                        "CaptionColor",
                        "CaptionItalic",
                        "CaptionWide",
                        "CaptionDecoration",
                        "Visible",
                        "WidthSlave",
                        "HeightSlave",
                        "PlaceHolder",
                        "Password",
                        "LookUpInputSize"
                    }
                );

                idSelectedComponent = properties.Id;

                designerFormOptions.ChangeVisibleProperties(hide: false);
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
                if (_tabGroupSelected != null)
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
            }

            UpdateItemSelected(idSelectedComponent);
        }

        /// <summary>
        /// Actualiza el indicador de seleccionado de todos los items
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
                if (item.Tag == null)
                {
                    ///ToDo: Crear tags y asignar propiedades de los items que se cargar por default en el diseñador
                    item.Tag = new WoComponentProperties();
                    item.Text = "NewItem";
                    item.CustomDraw += ButtonCustomDraw;
                }

                WoComponentProperties properties = (WoComponentProperties)item.Tag;
                properties.Selected = false;
                item.Tag = properties;
            }
        }

        #endregion Cambio del foco del componente


        #region Carga del formulario desde Json o Raw

        /// <summary>
        /// Instancia de la clase de actualización de los componentes del formulario.
        /// </summary>
        private WoDesignerUpdateControls _woDesignerUpdateControls = new WoDesignerUpdateControls();

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
        private void ChargeFormToDesigner()
        {
            FormItemsCol.Clear();

            WoContainer woContainer = new WoContainer();

            if (!File.Exists(_pathLayout))
            {
                WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                    new WoDesignerRawSerializerHelper();

                woContainer = woDesignerRawSerializerHelper.BuildRawWoContainer(_modelName);
            }
            else
            {
                string json = File.ReadAllText(_pathLayout);
                woContainer = JsonConvert.DeserializeObject<WoContainer>(json);
            }

            _theme = woContainer.Theme;
            AnalizeTheme();

            woContainer = _woDesignerUpdateControls.UpdateControls(
                woContainer: woContainer,
                woSyntaxManagerUserCode: _woSyntaxManagerUserCode,
                woSyntaxManagerModelClass: _woSyntaxManagerModelClass
            );

            DeserializeJsonToForm(woContainer);

            _layoutDesigner.ItemAdded += ItemAdded;
            _layoutDesigner.ItemRemoved += ItemRemoved;
        }

        #endregion Carga del formulario desde Json o Raw

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
            ChargeJsonLayout(woContainer);
            ChargeHideItems();
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
            _noAssignedCol.Clear();
            if (group.Id == "Root")
            {
                _theme = group.Theme;
                AnalizeTheme();

                if (group.Row > 4)
                {
                    _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    _layoutDesigner.Root.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength =
                        10;
                    _layoutDesigner.Root.OptionsTableLayoutGroup.ShrinkEmptyAutoSizeDefinition =
                        DefaultBoolean.True;

                    for (int i = 0; i < group.Row; i++)
                    {
                        _layoutDesigner.Root.OptionsTableLayoutGroup.AddRow();
                        _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions[i].Height = 10;
                    }
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
            WoTabGroupFactory.ColumnSpan = grupo.ColSpan;
            WoTabGroupFactory.RowSpan = grupo.RowSpan;
            WoTabGroupFactory.ColumnIndex = grupo.ColumnIndex;
            WoTabGroupFactory.RowIndex = grupo.RowIndex;

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
            WoGroupFactory.ColumnSpan = group.ColSpan;
            WoGroupFactory.RowSpan = group.RowSpan;
            WoGroupFactory.ColumnIndex = group.ColumnIndex;
            WoGroupFactory.RowIndex = group.RowIndex;
            WoGroupFactory.InternalRows = group.Row;

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

        #region Cargar items ocultos

        /// <summary>
        /// Carga los items que no se encuentran ya pre cargados en el layout y los agrega a la lista de items ocultos
        /// adicionalmente genera métodos y atributos en las clases.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeHideItems()
        {
            if (
                (!_itemsAdded.Contains("Controles"))
                && (
                    _modelJson.TipoModelo != WoTypeModel.Request
                    && _modelJson.TipoModelo != WoTypeModel.Response
                    && _modelJson.TipoModelo != WoTypeModel.CatalogSlave
                    && _modelJson.TipoModelo != WoTypeModel.TransactionSlave
                )
            )
            {
                InstanceItem(
                    item: new WoItem()
                    {
                        Id = "Controles",
                        Etiqueta = "Controles",
                        MaskText = "Controls",
                        Parent = "Root",
                        Enable = eItemEnabled.Activo,
                        TypeItem = eTypeItem.FormItem,
                        ClassModelType = _modelName,
                        Control = "Label",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = 1,
                        ComponenteExtra = true,
                    },
                    isVisible: false
                );
                _itemsAdded.Add("Controles");
            }

            if (!_itemsAdded.Contains("Alertas"))
            {
                InstanceItem(
                    item: new WoItem()
                    {
                        Id = "Alertas",
                        Etiqueta = "Alertas",
                        MaskText = "Alertas",
                        Parent = "Root",
                        Enable = eItemEnabled.Activo,
                        TypeItem = eTypeItem.FormItem,
                        Control = "Label",
                        ColSpan = 12,
                        RowSpan = 1,
                        ColumnIndex = 0,
                        RowIndex = 1,
                        ComponenteExtra = true,
                    },
                    isVisible: false
                );
                _itemsAdded.Add("Alertas");
            }

            foreach (ModeloColumna column in _modelJson.Columnas)
            {
                BuildBaseItem(column);
            }
        }

        /// <summary>
        /// Crea los items que no se encuentran en el layout y los agrega a la lista de items ocultos.
        /// </summary>
        /// <param name="column"></param>
        [SupportedOSPlatform("windows")]
        private void BuildBaseItem(ModeloColumna column)
        {
            if (
                _layoutDesigner
                    .Items.Where(x => ((WoComponentProperties)x.Tag).BindedProperty == column.Id)
                    .FirstOrDefault() == null
            )
            {
                string control =
                    (column.TipoControl.ToString() == "CollectionEditor")
                        ? "Slave"
                        : column.TipoControl.ToString();
                string type = column.TipoColumna.ToString();
                type = _designerTypeHelper.DesignerTypeToCodeType(
                    designerType: type,
                    isNullable: column.Nulo,
                    attributeName: column.Id
                );

                WoItem newItem = new WoItem()
                {
                    Id = column.Id,
                    Etiqueta = Etiqueta.ToId(column.Formulario),
                    MaskText = column.Formulario,
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = type,
                    ClassModelType = column.ModeloId,
                    Nullable = column.Nulo,
                    Control = (control == "WoState") ? "EnumInt" : control,
                    Parent = "ModelForm",
                    ColSpan = 12,
                    RowSpan = (control == "Memo") ? 3 : 1,
                    ColumnIndex = 0,
                    RowIndex = 1,
                    BackgorundColorContainerItem = eContainerItemColor.Default,
                    CaptionColor = eTextColor.FontDefault,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default",
                    BindedProperty = column.Id
                };

                if (control == "Slave")
                {
                    newItem.TypeItem = eTypeItem.Slave;
                    newItem.BindedProperty = column.Id;
                    newItem.SlaveModelId = column.ModeloId;
                    newItem.CaptionColor = eTextColor.FontDefault;
                    newItem.ComponentFontColor = eTextColor.FontDefault;
                }

                CheckMethodsAndProperties(item: newItem);

                InstanceItem(item: newItem, isVisible: false);
            }
        }

        /// <summary>
        /// Se ocupa de realizar las instancias de atributos y creación de métodos de los nuevo controles creados desde el modelo.
        /// </summary>
        /// <param name="item"></param>
        [SupportedOSPlatform("windows")]
        private void CheckMethodsAndProperties(WoItem item)
        {
            if (
                File.Exists(
                    $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs"
                )
                && File.Exists(
                    $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Controls.cs"
                )
            )
            {
                /// Validar atributos
                if (
                    item.Control == "Text"
                    || item.Control == "Spin"
                    || item.Control == "Date"
                    || item.Control == "Memo"
                    || item.Control == "WoState"
                    || item.Control == "EnumInt"
                    || item.Control == "LookUpDialog"
                    || item.Control == "LookUp"
                    || item.Control == "Decimal"
                    || item.Control == "Bool"
                    || item.Control == "EnumString"
                )
                {
                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoEditView",
                        name: item.Id,
                        value: $@"new WoEditView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );
                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoFormItemView",
                        name: $"{item.Id}Container",
                        value: $@"new WoFormItemView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );
                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoInputAlertView",
                        name: $"{item.Id}Alert",
                        value: $@"new WoInputAlertView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );
                }

                if (
                    !_woSyntaxManagerUserCode.AlreadyExistMethod($@"{item.Id}_OnChange")
                    && !_woSyntaxManagerUserCode.AlreadyExistMethod($@"{item.Id}_OnClick")
                )
                {
                    if (item.Control == "Button")
                    {
                        _woSyntaxManagerUserCode.CreateNewMethod(
                            methodName: $@"{item.Id}_OnClick",
                            bodyMethod: "//Code",
                            typeMethod: "void"
                        );
                    }
                    else if (
                        item.Control == "Text"
                        || item.Control == "Date"
                        || item.Control == "Spin"
                        || item.Control == "Memo"
                        || item.Control == "WoState"
                        || item.Control == "EnumInt"
                        || item.Control == "EnumString"
                        || item.Control == "LookUpDialog"
                        || item.Control == "LookUp"
                    )
                    {
                        _woSyntaxManagerUserCode.CreateNewMethod(
                            methodName: $@"{item.Id}_OnChange",
                            bodyMethod: "//Code",
                            typeMethod: "void"
                        );

                        _woSyntaxManagerUserCode.CreateNewMethod(
                            methodName: $@"{item.Id}_OnFocus",
                            bodyMethod: "//Code",
                            typeMethod: "void"
                        );

                        _woSyntaxManagerUserCode.CreateNewMethod(
                            methodName: $@"{item.Id}_OnBlur",
                            bodyMethod: "//Code",
                            typeMethod: "void"
                        );
                    }
                }
            }
        }

        #endregion Cargar items ocultos

        #region Serializar formulario a json

        /// <summary>
        /// Grupo que se serializara del para guardar
        /// el formulario que diseña el usuario en json.
        /// </summary>
        private WoContainer _groupForSerialize = new WoContainer();

        /// <summary>
        /// Retorna lo que se encuentre en la variable del grupo.
        /// ToDo: De momento detono el método para serializar a json lo que hay en el layout todas las beses
        /// pero hay que agregar una bandera o hacerlo reactivo a cambios para que no se detone en caso de
        /// ser no ser necesario.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public WoContainer GetLastVersionGroup()
        {
            SerializeFormToJson();
            return _groupForSerialize;
        }

        /// <summary>
        /// Serializa el formulario que se guarda en la variable
        /// _groupForSerialize.
        /// En caso de no existir crea el archivo.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void SerializeFormToJson()
        {
            WoDesignerSerializerHelper designerSerializerHelper = new WoDesignerSerializerHelper(
                layoutDesigner: _layoutDesigner
            );

            _groupForSerialize = designerSerializerHelper.SerilizeFormToJson();

            if (File.Exists(_pathLayout))
                WoDirectory.DeleteFile(_pathLayout);

            WoDirectory.WriteFile(_pathLayout, JsonConvert.SerializeObject(_groupForSerialize));
        }

        #endregion Serializar formulario a json


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
                    _itemSelected.HideToCustomization();
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
                _itemSelected.HideToCustomization();
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
                    item.HideToCustomization();
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
                    System.Windows.Forms.MessageBox.Show("Seleccione un grupo!");
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


        #region Cambiar el modo del designer

        /// <summary>
        /// Guarda el estado en el que se encuentra el formulario.
        /// </summary>
        private eModeDesigner _modeDesigner = eModeDesigner.Block;

        /// <summary>
        /// Propiedades recuperadas desde el css del tema.
        /// </summary>
        private WoThemeOptions _woThemeOptions = WoThemeOptions.GetInstance();

        /// <summary>
        /// Opciones de diseño para los controles.
        /// </summary>
        private WoCommonDesignOptions _woCommonDesignOptions = new WoCommonDesignOptions();

        /// <summary>
        /// Controlador de eventos que se detonara cuando se cambie el modo del formulario.
        /// </summary>
        public event EventHandler<eModeDesigner> ModeChanged;

        /// <summary>
        /// Controlador de eventos que se detonara cuando se agregue un método o remueva alguno.
        /// </summary>
        public Action UpdateCodeEditor;

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

            designerFormOptions.Size = new Size(500, 1000);
            designerFormOptions.Location = new System.Drawing.Point(100, 100);

            designerFormOptions.ModelName = _modelName;
            if (_modelJson.TipoModelo == WoTypeModel.Request)
            {
                designerFormOptions.HideTabButtons();
            }
            else
            {
                designerFormOptions.ChargeButtonList();
            }

            designerFormOptions.UpdateCodeEditor += () =>
            {
                UpdateCodeEditor?.Invoke();
            };
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

        #region Modificación del formulario por drag and drop

        private List<(string parent, int colIndex, int labelSize)> _labelSizes =
            new List<(string parent, int colIndex, int labelSize)>();

        //Revisar
        [SupportedOSPlatform("windows")]
        public void DragEnter(object sender, ItemDraggingEventArgs e)
        {
            _labelSizes.Clear();
            if (EditingEvt != null)
            {
                EditingEvt.Invoke(this, false);
            }
        }

        #endregion Modificación del formulario por drag and drop


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
        /// Analiza el tema que se esta utilizando.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void AnalizeTheme()
        {
            _woThemeAnalizer.ReadCss(_theme);
        }

        /// <summary>
        /// Se detona cuando el diseñador de temas realiza una modificación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="theme"></param>
        [SupportedOSPlatform("windows")]
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
            if (
                File.Exists(
                    $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs"
                )
                && File.Exists(
                    $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Controls.cs"
                )
            )
            {
                _woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();

                _woSyntaxManagerUserCode.InitializeManager(
                    pathScript: $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs",
                    className: _modelName,
                    modelName: _modelName
                );

                _woSyntaxManagerModelClass = new WoSyntaxManagerModelClass();

                _woSyntaxManagerModelClass.InitializeManager(
                    pathScript: $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Controls.cs",
                    className: _modelName,
                    modelName: _modelName
                );
            }
            else
            {
                _woSyntaxManagerUserCode = null;
                _woSyntaxManagerModelClass = null;
            }
        }

        #endregion Gestión de clases


        #region Validación de las etiquetas

        /// <summary>
        /// Lista de etiquetas no encontradas.
        /// </summary>
        private List<string> _notFoundLabels = new List<string>();

        /// <summary>
        /// Valida que todos los controles tengan su etiqueta.
        /// </summary>
        private void ValidateLabels() { }

        #endregion Validación de las etiquetas


        //ToDo: quitar
        #region Basura, Solo para pruebas
        public void Prueba() { }
        #endregion Basura, Solo para pruebas

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
    }
}
