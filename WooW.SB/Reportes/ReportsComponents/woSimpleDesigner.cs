using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using WooW.Core;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Client;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerFactory;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.DesignerHelpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Reportes.Client;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Reportes.ReportsComponents
{
    public partial class WoSimpleDesigner : UserControl
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

        #region Atributos

        /// <summary>
        /// Nombre del modelo del que se realiza el componente.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Contenedor base de los componentes.
        /// </summary>
        private WoContainer _woContainer = new WoContainer();

        /// <summary>
        /// Se inicializa en un método dedicado a inicializar el diseñador que se detona desde el constructor.
        /// Representa el diseñador como tal y desde este se puede acceder a Root.
        /// Root se utiliza para poder asignar componentes desde código al diseñador.
        /// </summary>
        private LayoutControl _layoutDesigner = null;

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

        #endregion Atributos

        #region Componentes

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

        #endregion Componentes

        #region Initialize y constructor

        /// <summary>
        /// Constructor principal del componente.
        /// </summary>
        /// <param name="modelName"></param>
        public WoSimpleDesigner(string modelName)
        {
            _modelName = modelName;

            _instanceModelManager = new InstanceModelManager(
                modelName: _modelName,
                dirModel: $@"{_project.DirApplication}\WebClient\WooW.WebClient\bin\Debug\net8.0\WooW.WebClient.dll",
                fullAssemblyName: $@"WooW.Model.{_modelName}"
            );

            InitializeComponent();

            _woContainer = GetBaseJsonLayout();

            CleanBaseControls();
            InitializeGridBase();

            DeserializeJsonToForm();
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
            _layoutDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            _layoutDesigner.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            _layoutDesigner.Root.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
            _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions.Clear();
            _layoutDesigner.Root.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 30;

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
        }

        #endregion Initialize y constructor

        #region Creación del modelo

        /// <summary>
        /// Valida la existencia de un formulario diseñado, si no existe lo crea.
        /// Retorna el contenedor base del formulario.
        /// </summary>
        /// <returns></returns>
        private WoContainer GetBaseJsonLayout()
        {
            string pathLayout = $@"{_project.DirLayOuts}\FormDesign\{_modelName}.json";
            if (!File.Exists(pathLayout))
            {
                WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                    new WoDesignerRawSerializerHelper();
                return woDesignerRawSerializerHelper.BuildRawWoContainer(_modelName);
            }
            else
            {
                return JsonConvert.DeserializeObject<WoContainer>(WoDirectory.ReadFile(pathLayout));
            }
        }

        #endregion Creación del modelo

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
        private void DeserializeJsonToForm()
        {
            ChargeJsonLayout(_woContainer);
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
            if (group.Id == "Root")
            {
                if (group.Row > 4)
                {
                    List<RowDefinition> rows = new List<RowDefinition>();
                    for (int i = 0; i < (group.Row - 4); i++)
                    {
                        rows.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    }

                    _layoutDesigner.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(
                        rows.ToArray()
                    );
                }

                if (group.ContainersCol != null)
                {
                    foreach (var subGroup in group.ContainersCol)
                    {
                        if (subGroup.TypeContainer == eTypeContainer.FormTabGroup)
                        {
                            _layoutDesigner.Root.AddTabbedGroup(GetTab(subGroup));
                        }
                        else
                        {
                            _layoutDesigner.Root.AddGroup(GetGrupo(subGroup));
                        }
                    }
                }

                if (group.ItemsCol != null)
                {
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
                groupText: group.MaskText,
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

        #region Creación de los grupos, tabs y grupos de tabs

        /// <summary>
        /// Indica el numero de grupos agregados en el formulario.
        /// Se añade al nombre cuando se crea un grupo nuevo, para evitar que se repitan los nombres
        /// (no se puede agregar un componente del diseñador con un id duplicado, independientemente de si es un tab, grupo o item).
        /// Vuelve a 0 cuando se limpia toda la pantalla
        /// </summary>
        private int _groupIndex = 0;

        /// <summary>
        /// Instancia y configura el grupo de tabs.
        /// Con una tab por defecto, salvo que se modifique el parámetro with tab.
        /// Utiliza el helper para generar las instancias
        /// tanto del grupo de tabs como de la tab.
        /// pero no lo agrega al layout.
        /// </summary>
        /// <param name="columnSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="internalRows"></param>
        /// <param name="tabGroupName"></param>
        /// <param name="tabGroupText"></param>
        /// <param name="tabName"></param>
        /// <param name="tabText"></param>
        /// <param name="tab"></param>
        [SupportedOSPlatform("windows")]
        public TabbedControlGroup CreateTabGroup(
            int internalRows = 1,
            string tabGroupName = "",
            string tabGroupText = "",
            string tabName = "",
            string tabText = "",
            bool add = true,
            bool withTab = true
        )
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            TabbedControlGroup tcgNew = WoTabGroupFactory.GenerateTabGroup(
                indexGroup: _groupIndex,
                text: tabGroupText,
                name: tabGroupName
            );
            _groupIndex++;

            if (withTab)
            {
                LayoutControlGroup tabNew = CreateTab(
                    internalRows: internalRows,
                    tabText: tabText,
                    tabName: tabName,
                    add: false
                );

                WoComponentProperties properties = (WoComponentProperties)tabNew.Tag;
                properties.Parent = tabGroupName;
                tabNew.Tag = properties;

                tcgNew.AddTabPage(tabNew);
            }

            if (add)
            {
                if (_groupSelected.IsNull())
                {
                    _groupSelected = _layoutDesigner.Root;
                    //ToDo: enviar a un log, si es nulo hay error
                }
                tcgNew = _groupSelected.AddTabbedGroup(tcgNew);

                tcgNew.Tag = new WoComponentProperties()
                {
                    Id = tcgNew.Name,
                    Etiqueta = tcgNew.Text,
                    MaskText = tcgNew.Text,
                    TypeContainer = eTypeContainer.FormTabGroup,
                    Enable = eItemEnabled.Activo,
                    Parent = _groupSelected.Name,
                    Col = 12,
                    Row = WoGroupFactory.InternalRows,
                    ColSpan = WoGroupFactory.ColumnSpan,
                    RowSpan = WoGroupFactory.RowSpan,
                    ColumnIndex = WoGroupFactory.ColumnIndex,
                    RowIndex = WoGroupFactory.RowIndex,
                    BackgorundColorGroup = eGroupColor.Primary,
                    ComponentFontColor = eTextColor.White
                };

                tcgNew.Update();
            }

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            return tcgNew;
        }

        /// <summary>
        /// Crea una tab en caso de definir el add como true, se
        /// agregara en _selectedGroup.
        /// Utiliza el helper de los grupos para generar
        /// la instancia del grupo que funge como tab
        /// y para definir las columnas a este.
        /// Retorna la tab para evitar la búsqueda posterior.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public LayoutControlGroup CreateTab(
            int internalRows,
            string tabText = "",
            string tabName = "",
            bool add = true
        )
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            WoGroupFactory.InternalRows = internalRows;

            LayoutControlGroup tabNew = WoGroupFactory.GenerateGroupControl(
                indexGroup: _groupIndex,
                text: tabText,
                name: tabName
            );
            _groupIndex++;

            WoComponentProperties tabProperties = new WoComponentProperties()
            {
                Id = tabNew.Name,
                Etiqueta = tabNew.Text,
                MaskText = tabNew.Text,
                TypeContainer = eTypeContainer.FormTab,
                Enable = eItemEnabled.Activo,
                Parent = "",
                Col = 12,
                Row = WoGroupFactory.InternalRows,
                ColSpan = WoGroupFactory.ColumnSpan,
                RowSpan = WoGroupFactory.RowSpan,
                ColumnIndex = WoGroupFactory.ColumnIndex,
                RowIndex = WoGroupFactory.RowIndex,
                BackgorundColorGroup = eGroupColor.Primary,
                ComponentFontColor = eTextColor.White
            };

            if (add)
            {
                if (_tabGroupSelected.IsNull())
                {
                    MessageBox.Show("No existe un grupo de tabs seleccionado donde agregarlo");
                    //ToDo: enviar a un log, si es nulo hay error
                }
                tabNew = _tabGroupSelected.AddTabPage(tabNew);

                tabProperties.Parent = _tabGroupSelected.Name;
            }

            tabNew.Tag = tabProperties;

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            return tabNew;
        }

        /// <summary>
        /// Crea el un grupo con la ayuda de la fabrica.
        /// Siempre retorna la instancia que se crea para evitar la búsqueda posterior de esta.
        /// En el caso de que se quiera auto agregar primero valida que el grupo seleccionado no sea
        /// nulo aun que este nunca puede ser nulo, se valida para controlar y detectar errores.
        /// </summary>
        /// <param name="columnSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="internalRows"></param>
        /// <param name="groupText"></param>
        /// <param name="groupName"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public LayoutControlGroup CreateGroup(
            string groupText = "",
            string groupName = "",
            bool add = true
        )
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            LayoutControlGroup lcgNew = WoGroupFactory.GenerateGroupControl(
                _groupIndex,
                groupText,
                groupName
            );
            _groupIndex++;

            if (add)
            {
                if (_groupSelected.IsNull())
                {
                    _groupSelected = _layoutDesigner.Root;
                }
                lcgNew = _groupSelected.AddGroup(lcgNew);

                lcgNew.Tag = new WoComponentProperties()
                {
                    Id = lcgNew.Name,
                    Etiqueta = lcgNew.Text,
                    MaskText = lcgNew.Text,
                    TypeContainer = eTypeContainer.FormGroup,
                    Enable = eItemEnabled.Activo,
                    Parent = _groupSelected.Name,
                    Col = 12,
                    Row = WoGroupFactory.InternalRows,
                    ColSpan = WoGroupFactory.ColumnSpan,
                    RowSpan = WoGroupFactory.RowSpan,
                    ColumnIndex = WoGroupFactory.ColumnIndex,
                    RowIndex = WoGroupFactory.RowIndex,
                    BackgorundColorGroup = eGroupColor.Primary,
                    ComponentFontColor = eTextColor.White
                };
            }

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            return lcgNew;
        }

        #endregion Creación de los grupos, tabs y grupos de tabs

        #region Definir el tipo de items

        /// <summary>
        /// En función de una instancia de la clase item determinamos el tipo de control
        /// y utilizamos los métodos de creación dedicados para generar la instancia y agregarlo
        /// a la pantalla.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isVisible"></param>
        private void InstanceItem(WoItem item, bool isVisible)
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            if (item.Control == "Label")
            {
                CreateLabelControl(item: item, visible: isVisible);
            }
            else if (item.Control == "Button")
            {
                CreateButtonControl(item: item, visible: isVisible);
            }
            else if (item.Control == "Text")
            {
                CreateTextControl(item: item, visible: isVisible);
            }
            else if (item.Control == "Date")
            {
                CreateDateControl(item: item, visible: isVisible);
            }
            else if (item.Control == "Spin" || item.Control == "Decimal")
            {
                CreateSpinControl(item: item, visible: isVisible);
            }
            else if (item.Control == "Bool")
            {
                CreateCheckControl(item: item, visible: isVisible);
            }
            else if (item.Control == "Memo")
            {
                CreateMemoControl(item: item, visible: isVisible);
            }
            else if (
                item.Control == "EnumInt"
                || item.Control == "EnumString"
                || item.Control == "VxState"
            )
            {
                CreateComboBoxControl(item: item, visible: isVisible);
            }
            else if (item.Control == "LookUp" || item.Control == "LookUpDialog")
            {
                CreateLookUpControl(item: item, visible: isVisible);
            }

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Definir el tipo de items

        #region Creación de los controles
        /// <summary>
        /// Tiene las funciones para:
        ///     convertir de string al tipo que se defina en la función genérica
        /// </summary>
        private ControlItemHelper _controlHelper = new ControlItemHelper();

        /// <summary>
        /// Guarda el item añadido al formulario.
        /// Permite manejar el item una ves añadido para modificar temas del diseño.
        /// </summary>
        private LayoutControlItem _itemAdded;

        /// <summary>
        /// Permite generar la instancia de un control del tipo label
        /// y lo agrega al grupo que este en la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void CreateLabelControl(WoItem item, bool visible)
        {
            try
            {
                DevExpress.XtraEditors.LabelControl label =
                    new()
                    {
                        Name = item.Id,
                        AllowHtmlString = true,
                        Appearance =
                        {
                            TextOptions = { WordWrap = DevExpress.Utils.WordWrap.Wrap }
                        },
                    };

                _itemAdded = _groupSelected.AddItem(item.Id, label);

                _itemAdded.Name = item.Id;
                _itemAdded.OptionsTableLayoutItem.ColumnSpan = item.ColSpan;
                _itemAdded.OptionsTableLayoutItem.RowSpan = item.RowSpan;
                _itemAdded.OptionsTableLayoutItem.ColumnIndex = item.ColumnIndex;
                _itemAdded.OptionsTableLayoutItem.RowIndex = item.RowIndex;

                _itemAdded.Tag = item.ConvertToComponentProperties();

                if (!visible)
                    _itemAdded.HideToCustomization();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Permite generar un control de texto y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void CreateTextControl(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.TextEdit textEdit = new DevExpress.XtraEditors.TextEdit()
            {
                Name = item.Id,
            };

            textEdit.EditValueChanged += (sender, EventArgs) =>
            {
                DevExpress.XtraEditors.TextEdit component =
                    sender as DevExpress.XtraEditors.TextEdit;

                _instanceModelManager.AssignFromTextComponent(
                    item.BindedProperty,
                    component.EditValue.ToString()
                );
            };

            _itemAdded = _groupSelected.AddItem(item.Id, textEdit);

            SetBasicProperties(item: item, visible: visible);
        }

        /// <summary>
        /// Permite generar un date control y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void CreateDateControl(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.DateEdit dateEdit = new DevExpress.XtraEditors.DateEdit()
            {
                Name = item.Id,
                Properties = { TextEditStyle = TextEditStyles.DisableTextEditor, ShowClear = false }
            };

            dateEdit.EditValueChanged += (sender, EventArgs) =>
            {
                DevExpress.XtraEditors.DateEdit component =
                    sender as DevExpress.XtraEditors.DateEdit;

                _instanceModelManager.AssignValue(item.Id, component.EditValue.ToString());
            };
            if (item.MaskText == null)
            {
                item.MaskText = string.Empty;
            }
            _itemAdded = _groupSelected.AddItem(item.MaskText, dateEdit);

            SetBasicProperties(item: item, visible: visible);
        }

        /// <summary>
        /// Permite generar un spin control y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void CreateSpinControl(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.SpinEdit spinEdit = new DevExpress.XtraEditors.SpinEdit()
            {
                Name = item.Id,
                Properties = { TextEditStyle = TextEditStyles.DisableTextEditor }
            };

            spinEdit.EditValueChanged += (sender, EventArgs) =>
            {
                DevExpress.XtraEditors.SpinEdit component =
                    sender as DevExpress.XtraEditors.SpinEdit;

                string rawValue = component.EditValue.ToString();
                if (rawValue.Contains('.'))
                    return;

                _instanceModelManager.AssingnValueSpin(item.Id, rawValue);
            };
            if (item.MaskText == null)
            {
                item.MaskText = string.Empty;
            }
            _itemAdded = _groupSelected.AddItem(item.MaskText, spinEdit);

            SetBasicProperties(item: item, visible: visible);
        }

        /// <summary>
        /// Permite generar un check control y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void CreateCheckControl(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit()
            {
                Name = item.Id
            };

            checkEdit.EditValueChanged += (sender, EventArgs) =>
            {
                DevExpress.XtraEditors.CheckEdit component =
                    sender as DevExpress.XtraEditors.CheckEdit;

                _instanceModelManager.AssignValue(item.Id, component.EditValue.ToString());
            };
            if (item.MaskText == null)
            {
                item.MaskText = string.Empty;
            }
            _itemAdded = _groupSelected.AddItem(item.MaskText, checkEdit);

            SetBasicProperties(item: item, visible: visible);
        }

        /// <summary>
        /// Permite generar un memo control y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void CreateMemoControl(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.MemoEdit memoEdit = new DevExpress.XtraEditors.MemoEdit()
            {
                Name = item.Id,
                ReadOnly = true
            };

            memoEdit.EditValueChanged += (sender, EventArgs) =>
            {
                DevExpress.XtraEditors.MemoEdit component =
                    sender as DevExpress.XtraEditors.MemoEdit;

                _instanceModelManager.AssignFromTextComponent(
                    item.BindedProperty,
                    component.EditValue.ToString()
                );
            };
            if (item.MaskText == null)
            {
                item.MaskText = string.Empty;
            }
            _itemAdded = _groupSelected.AddItem(item.MaskText, memoEdit);

            SetBasicProperties(item: item, visible: visible);
        }

        /// <summary>
        /// Permite generar un combo control y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void CreateComboBoxControl(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit =
                new DevExpress.XtraEditors.ComboBoxEdit()
                {
                    Name = item.Id,
                    Properties = { TextEditStyle = TextEditStyles.DisableTextEditor }
                };

            if (item.BindingType.StartsWith("e") || item.BindingType.StartsWith("WoState"))
            {
                var enumType = _instanceModelManager.GetEnumType(item.BindingType);
                if (enumType == null)
                    return;

                var valuesEnum = System.Enum.GetValues(enumType);
                foreach (var value in valuesEnum)
                {
                    comboBoxEdit.Properties.Items.Add(value);
                }
            }
            comboBoxEdit.SelectedIndex = 0;
            comboBoxEdit.EditValueChanged += (sender, EventArgs) =>
            {
                DevExpress.XtraEditors.ComboBoxEdit component =
                    sender as DevExpress.XtraEditors.ComboBoxEdit;

                string selectedValue = component.EditValue.ToString();

                _instanceModelManager.AssignFromComboEnum(item.Id, selectedValue);
            };
            if (item.MaskText == null)
            {
                item.MaskText = string.Empty;
            }
            _itemAdded = _groupSelected.AddItem(item.MaskText, comboBoxEdit);

            SetBasicProperties(item: item, visible: visible);
        }

        /// <summary>
        /// Representa una instancia de uno de los modelo recuperados desde
        /// la dll generada.
        /// </summary>
        private dynamic _instanceModelDyn = null;

        /// <summary>
        /// Es el tipo de la instancia dinámica.
        /// </summary>
        private Type _instanceModelType = null;

        /// <summary>
        /// Permite generar un combo control y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        [SupportedOSPlatform("windows")]
        private void CreateLookUpControl(WoItem item, bool visible)
        {
            try
            {
                DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit =
                    new DevExpress.XtraEditors.ComboBoxEdit()
                    {
                        Name = item.Id,
                        Properties = { TextEditStyle = TextEditStyles.DisableTextEditor }
                    };

                string AssemblyFile = $"{_project.FileWooWWebClientdll}";
                Assembly _assembly = Assembly.LoadFrom(AssemblyFile);

                Type _instanceClass = _assembly
                    .GetTypes()
                    .Where(x => x.Name == item.ClassModelType)
                    .FirstOrDefault();
                if (_instanceClass == null)
                {
                    return;
                }
                dynamic clase = Activator.CreateInstance(_instanceClass);
                dynamic _instanceList = _assembly
                    .GetTypes()
                    .Where(x => x.Name == @$"{item.ClassModelType}List")
                    .FirstOrDefault();

                if (_instanceList == null)
                {
                    return;
                }
                WoLookUpConfigAttribute attr = (WoLookUpConfigAttribute)
                    _instanceClass.GetCustomAttribute(typeof(WoLookUpConfigAttribute), false);
                dynamic claseList = Activator.CreateInstance(_instanceList);
                claseList.filter = attr.Filter;
                claseList.select = attr.Select;
                /// Instancia del cliente.
                FormClient client = new FormClient();
                /// Autenticación.
                client.AutoAuth();
                /// Le paso en que tipo va a buscar el método.
                client.SetModelType(_instanceClass);
                /// Busco el método Post que reciba un parámetro del tipo.
                client.SearchMethod(clase.GetType(), "List");
                //XMateriaList xMateriaList = new XMateriaList();


                /// Ejecuta el método.
                dynamic adata = client.ExecuteMethod(claseList);
                int countRegisters = adata.Value.Count;
                if (adata == null)
                {
                    comboBoxEdit.Properties.Items.Add("Seleccione...");
                }
                else
                {
                    for (int i = 0; i < countRegisters; i++)
                    {
                        var itemKey = adata
                            .Value[i]
                            .GetType()
                            .GetProperty(attr.KeyField)
                            .GetValue(adata.Value[i])
                            .ToString();
                        var itemDesc = adata
                            .Value[i]
                            .GetType()
                            .GetProperty(attr.DescriptionField)
                            .GetValue(adata.Value[i])
                            .ToString();
                        comboBoxEdit.Properties.Items.Add(itemKey + " - " + itemDesc);
                    }
                }
                comboBoxEdit.SelectedIndex = 0;
                comboBoxEdit.EditValueChanged += (sender, EventArgs) =>
                {
                    DevExpress.XtraEditors.ComboBoxEdit component =
                        sender as DevExpress.XtraEditors.ComboBoxEdit;

                    string selectedValue = component.EditValue.ToString();

                    _instanceModelManager.AssignComboEnumFromLookUp(item.Id, selectedValue);
                };
                if (item.MaskText == null)
                {
                    item.MaskText = string.Empty;
                }
                _itemAdded = _groupSelected.AddItem(item.MaskText, comboBoxEdit);

                SetBasicProperties(item: item, visible: visible);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Recupera el Dto que coincida con el nombre que se recibe.
        /// Puede retornar nulo si no se encuentra.
        /// </summary>
        /// <param name="fullDtoName"></param>
        /// <returns></returns>
        public dynamic GetDto(string fullDtoName)
        {
            Type findDtoType = SearchDtoType(fullDtoName);
            if (findDtoType.IsNull())
                return null;

            dynamic dtoInstance = Activator.CreateInstance(findDtoType);

            foreach (var property in findDtoType.GetProperties())
            {
                PropertyInfo formProperty = SearchProperty(property.Name);
                if (formProperty.IsNull())
                {
                    //_observer.SetLog(_inconsistencyInDto);
                }
                else
                {
                    dynamic formValue = formProperty.GetValue(_instanceModelDyn);
                    property.SetValue(dtoInstance, formValue);
                }
            }

            return dtoInstance;
        }

        /// <summary>
        /// Busca el tipo del dto en función del nombre en el ensamblado.
        /// </summary>
        /// <param name="fullDtoName"></param>
        /// <returns></returns>
        private Type SearchDtoType(string fullDtoName)
        {
            string AssemblyFile = $"{_project.FileWooWWebClientdll}";
            Assembly _assembly = Assembly.LoadFrom(AssemblyFile);
            Type findDtoType = _assembly.GetType(fullDtoName);
            if (findDtoType == null)
            {
                //_observer.SetLog(_cantFindDto);
            }

            return findDtoType;
        }

        /// <summary>
        /// Recupera alguna de las propiedades de la instancia que se tiene del modelo.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private PropertyInfo SearchProperty(string propertyName)
        {
            PropertyInfo findProperty = _instanceModelType.GetProperty(propertyName);
            if (findProperty.IsNull())
                //_observer.SetLog(_nullProperty);
                return null;

            return findProperty;
        }

        /// <summary>
        /// Permite generar un button control y agregarlo a la variable de _groupSelected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        [SupportedOSPlatform("windows")]
        private void CreateButtonControl(WoItem item, bool visible)
        {
            if (item.MaskText == null)
            {
                item.MaskText = string.Empty;
            }
            DevExpress.XtraEditors.SimpleButton simpleButton =
                new DevExpress.XtraEditors.SimpleButton() { Name = item.Id, Text = item.MaskText };

            if (item.Id == "btnCalcular")
            {
                simpleButton.Click += _instanceModelManager.Carcular;
            }
            else if (item.Id == "btnNuevo")
            {
                simpleButton.Click += _instanceModelManager.Nuevo;
            }

            _itemAdded = _groupSelected.AddItem(item.Id, simpleButton);

            SetBasicProperties(item: item, visible: visible);
        }

        /// <summary>
        /// Añade a el item añadido la configuración general para todos los items.
        /// </summary>
        /// <param name="visible"></param>
        /// /// <param name="item"></param>
        private void SetBasicProperties(WoItem item, bool visible)
        {
            if (item.MaskText == null)
            {
                item.MaskText = string.Empty;
            }
            _itemAdded.Name = item.Id;
            _itemAdded.Text = item.MaskText;
            _itemAdded.OptionsTableLayoutItem.ColumnSpan = item.ColSpan;
            _itemAdded.OptionsTableLayoutItem.RowSpan = item.RowSpan;
            _itemAdded.OptionsTableLayoutItem.ColumnIndex = item.ColumnIndex;
            _itemAdded.OptionsTableLayoutItem.RowIndex = item.RowIndex;

            if (!visible)
                _itemAdded.HideToCustomization();
        }
        #endregion Creación de los controles

        #region Eventos de consumo de servicio

        /// <summary>
        /// Instancia de la clase que permite la interacción con una instancia del
        /// modelo con el que se esta trabajando.
        /// </summary>
        public InstanceModelManager _instanceModelManager;

        /// <summary>
        /// Fachada del evento calcular de la clase que gestiona el
        /// servicio.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public dynamic Calcular(object sender, EventArgs eventArgs)
        {
            _instanceModelManager.Carcular(sender, eventArgs);
            //return _instanceModelManager.GetInstanceModelDyn();
            return _instanceModelManager.GetResponseService();
        }

        #endregion Eventos de consumo de servicio
    }
}
