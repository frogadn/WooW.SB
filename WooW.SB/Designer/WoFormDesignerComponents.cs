using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraLayout;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using WooW.Core;
using WooW.SB.Designer.DesignerFactory;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.DesignerHelpers;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer
{
    public partial class WoFormDesigner : UserControl
    {
        #region Componentes base

        /// <summary>
        /// Orquesta una serie de métodos que se ocupan de generar componentes base a
        /// aparir de los cuales se pueden crear otros para construir formularios.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void BuildBaseComponents()
        {
            CreateGroupBase();
            CreateTabGroupBase();
        }

        /// <summary>
        /// Bandera que indica si se agregó el grupo base al formulario.
        /// </summary>
        private bool _tabAdedd = false;

        /// <summary>
        /// Método que se dispara cuando se agrega un componente X al formulario
        /// que se va a crear.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void LayoutDesigner_ItemAdded(object sender, EventArgs e)
        {
            Type controlType = sender.GetType();

            if (controlType == typeof(LayoutControlGroup))
            {
                LayoutControlGroup groupBase;

                groupBase = (LayoutControlGroup)sender;

                if (groupBase.Name == "Grupo" && groupBase.IsHidden)
                {
                    groupBase.Name = $@"Grupo{_groupIndex}";
                    groupBase.Text = $@"Grupo{_groupIndex}";
                    WoComponentProperties properties = (WoComponentProperties)groupBase.Tag;
                    properties.Id = $@"Grupo{_groupIndex}";
                    properties.MaskText = $@"Grupo{_groupIndex}";
                    groupBase.Tag = properties;
                    _groupIndex++;

                    CreateGroupBase();
                }
            }
            else if (controlType == typeof(TabbedControlGroup))
            {
                TabbedControlGroup tabbedControlGroup;

                tabbedControlGroup = (TabbedControlGroup)sender;

                if (tabbedControlGroup.Name == "Grupo de tabs" && tabbedControlGroup.IsHidden)
                {
                    tabbedControlGroup.Name = $@"TabGroup{_groupIndex}";
                    tabbedControlGroup.Text = $@"TabGroup{_groupIndex}";
                    WoComponentProperties properties = (WoComponentProperties)
                        tabbedControlGroup.Tag;
                    properties.Id = $@"TabGroup{_groupIndex}";
                    properties.MaskText = $@"TabGroup{_groupIndex}";
                    tabbedControlGroup.Tag = properties;

                    LayoutControlGroup tabbedControl = (LayoutControlGroup)
                        tabbedControlGroup.TabPages.First();
                    tabbedControl.Name = $@"Tab{_groupIndex}";
                    tabbedControl.Text = $@"Tab{_groupIndex}";
                    WoComponentProperties propertiesInternal = (WoComponentProperties)
                        tabbedControl.Tag;
                    propertiesInternal.Id = $@"Tab{_groupIndex}";
                    propertiesInternal.MaskText = $@"Tab{_groupIndex}";
                    tabbedControl.Tag = propertiesInternal;

                    _groupIndex++;

                    CreateTabGroupBase();

                    _tabAdedd = true;
                }
            }
        }

        /// <summary>
        /// Crea un grupo oculto que siempre se puede regenerar como componente base
        /// para que una vez agregado se regenere y se puedan generar en este caso N grupos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateGroupBase()
        {
            WoGroupFactory.ColumnSpan = 4;
            WoGroupFactory.RowSpan = 2;
            WoGroupFactory.ColumnIndex = 0;
            WoGroupFactory.RowIndex = 0;
            WoGroupFactory.InternalRows = 2;
            LayoutControlGroup newGroup = CreateGroup(
                groupText: "Grupo",
                groupName: "Grupo",
                add: false
            );
            _layoutDesigner.Root.AddGroup(newGroup);
            newGroup.HideToCustomization();
        }

        /// <summary>
        /// Crea una tab base que se puede regenerar para fungir de generador de tabs.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateTabGroupBase()
        {
            WoTabGroupFactory.ColumnSpan = 4;
            WoTabGroupFactory.RowSpan = 2;
            WoTabGroupFactory.ColumnIndex = 0;
            WoTabGroupFactory.RowIndex = 0;

            TabbedControlGroup newTabGroup = CreateTabGroup(
                internalRows: 2,
                tabGroupName: "Grupo de tabs",
                tabGroupText: "Grupo de tabs",
                tabName: "Tab",
                tabText: "Tab",
                add: false,
                withTab: true
            );
            _layoutDesigner.Root.AddTabbedGroup(newTabGroup);
            newTabGroup.HideToCustomization();
        }

        #endregion Componentes base

        #region Items en el formulario

        /// <summary>
        /// Controlador de eventos que se detona cuando se agrega un nuevo item.
        /// </summary>
        public event EventHandler<string> AddItemEvt;

        /// <summary>
        /// Controlador de eventos que se detona cuando se retira un item.
        /// </summary>
        public event EventHandler<string> RemoveItemEvt;

        /// <summary>
        /// Lista con los nombre de los formularios que están agregados en el diseñador.
        /// </summary>
        public List<string> FormItemsCol = new List<string>();

        /// <summary>
        /// Se suscribe al método del layout para cuando se agrega al formulario un control nuevo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ItemAdded(object sender, EventArgs e)
        {
            if (!_itemSelected.IsNull())
            {
                LayoutControlItem item = sender.ConvertTo<LayoutControlItem>();
                if (!FormItemsCol.Contains(item.Text))
                {
                    FormItemsCol.Add(item.Text);
                    AddItemEvt?.Invoke(this, item.Text);
                }
            }
        }

        /// <summary>InstanceItem
        /// Se suscribe al método del layout para cuando se remueve un item del formulario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ItemRemoved(object sender, EventArgs e)
        {
            if (!_itemSelected.IsNull())
            {
                LayoutControlItem item = sender.ConvertTo<LayoutControlItem>();
                if (FormItemsCol.Contains(item.Text))
                {
                    FormItemsCol.Remove(item.Text);
                    RemoveItemEvt?.Invoke(this, item.Text);
                }
            }
        }

        #endregion Items en el formulario


        #region Creación de los grupos, tabs y grupos de tabs

        /// <summary>
        /// Indica el numero de grupos agregados en el formulario.
        /// Se añade al nombre cuando se crea un grupo nuevo, para evitar que se repitan los nombres
        /// (no se puede agregar un componente del diseñador con un id duplicado, independientemente de si es un tab, grupo o item).
        /// Vuelve a 0 cuando se limpia toda la pantalla
        /// Siempre tendrá un index no usado, recordar sumarle 1 tras usarlo.
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
            }

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
                BackgorundColorGroup = eGroupColor.Background,
                ComponentFontColor = eTextColor.FontDefault
            };

            tcgNew.CustomDraw += TabGroupCustomDraw;
            tcgNew.Update();

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
                BackgorundColorGroup = eGroupColor.Background,
                ComponentFontColor = eTextColor.FontDefault
            };

            if (add)
            {
                if (_tabGroupSelected.IsNull())
                {
                    MessageBox.Show("No existe un grupo de tabs seleccionado donde agregarlo");
                    //ToDo: enviar a un log, si es nulo hay error
                }
                else
                {
                    tabProperties.Parent = _tabGroupSelected.Name;
                    tabNew.Tag = tabProperties;

                    _tabGroupSelected.AddTabPage(tabNew);
                }
            }
            else
            {
                tabNew.Tag = tabProperties;
            }

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
            }

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
                BackgorundColorGroup = eGroupColor.Background,
                ComponentFontColor = eTextColor.FontDefault
            };

            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            return lcgNew;
        }

        #endregion Creación de los grupos, tabs y grupos de tabs

        #region Cambiar el orden de las tabs

        /// <summary>
        /// Mueve la tab seleccionada a la derecha.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void MoveTabRight()
        {
            WoComponentProperties selectedTabProperties = (WoComponentProperties)_groupSelected.Tag;

            List<LayoutControlGroup> newTabsInTab = new List<LayoutControlGroup>();

            LayoutControlGroup findGroup = null;

            foreach (var tab in _tabGroupSelected.TabPages)
            {
                LayoutControlGroup lastGroup = (LayoutControlGroup)tab;
                WoComponentProperties properties = (WoComponentProperties)lastGroup.Tag;

                bool added = false;

                if (findGroup != null)
                {
                    newTabsInTab.Add((LayoutControlGroup)tab);
                    newTabsInTab.Add(findGroup);
                    findGroup = null;
                    added = true;
                }

                if (properties.Id == selectedTabProperties.Id)
                {
                    findGroup = (LayoutControlGroup)tab;
                }
                else
                {
                    if (!added)
                    {
                        newTabsInTab.Add((LayoutControlGroup)tab);
                    }
                }
            }

            foreach (var tab in newTabsInTab)
            {
                _tabGroupSelected.RemoveTabPage(tab);
                _tabGroupSelected.AddTabPage(tab);
            }
        }

        /// <summary>
        /// Mueve la tab seleccionada a la izquierda.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void MoveTabLeft()
        {
            WoComponentProperties selectedTabProperties = (WoComponentProperties)_groupSelected.Tag;
            string selectedId = selectedTabProperties.Id;

            List<LayoutControlGroup> newTabsInTab = new List<LayoutControlGroup>();

            int count = 0;
            bool nextAdded = false;

            foreach (var tab in _tabGroupSelected.TabPages)
            {
                if (count < _tabGroupSelected.TabPages.Count - 1)
                {
                    LayoutControlGroup nextLayoutControlGroup = (LayoutControlGroup)
                        _tabGroupSelected.TabPages[count + 1];
                    WoComponentProperties nextProperties = (WoComponentProperties)
                        nextLayoutControlGroup.Tag;

                    if (nextProperties.Id == selectedId)
                    {
                        newTabsInTab.Add(nextLayoutControlGroup);
                        newTabsInTab.Add((LayoutControlGroup)tab);
                        nextAdded = true;
                    }
                    else if (
                        ((WoComponentProperties)((LayoutControlGroup)tab).Tag).Id == selectedId
                    )
                    {
                        nextAdded = false;
                    }
                    else
                    {
                        newTabsInTab.Add((LayoutControlGroup)tab);
                        nextAdded = false;
                    }
                }
                else if (((WoComponentProperties)((LayoutControlGroup)tab).Tag).Id != selectedId)
                {
                    newTabsInTab.Add((LayoutControlGroup)tab);
                }

                count++;
            }

            foreach (var tab in newTabsInTab)
            {
                _tabGroupSelected.RemoveTabPage(tab);
                _tabGroupSelected.AddTabPage(tab);
            }
        }

        #endregion Cambiar el orden de las tabs

        #region Definir el tipo de items

        /// <summary>
        /// En función de una instancia de la clase item determinamos el tipo de control
        /// y utilizamos los métodos de creación dedicados para generar la instancia y agregarlo
        /// a la pantalla.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isVisible"></param>
        [SupportedOSPlatform("windows")]
        private void InstanceItem(WoItem item, bool isVisible)
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            if (item.Control == "UnKnown" || item.Control == "NA")
            {
                if (!_noAssignedCol.Contains(item.Id))
                {
                    _noAssignedCol.Add(item.Id);
                }
            }

            if (isVisible)
                FormItemsCol.Add(item.Id);

            if (
                item.BindingType.ToLower().Contains("decimal")
                || item.BindingType.ToLower().Contains("double")
            )
            {
                item.Control = "TextMask";
            }

            if (item.Control == "Urn")
            {
                item.Control = "Text";
            }

            if (item.Control == "Slave")
            {
                CreateSlave(item: item, visible: isVisible);
            }
            else if (
                item.Control == "Label"
                || item.Control == "Button"
                || item.Control == "Text"
                || item.Control == "Date"
                || item.Control == "Spin"
                || item.Control == "Decimal"
                || item.Control == "Bool"
                || item.Control == "Memo"
                || item.Control == "WoState"
                || item.Control == "EnumInt"
                || item.Control == "EnumString"
                || item.Control == "LookUp"
                || item.Control == "LookUpDialog"
                || item.Control == "Custom"
                || item.Control == "File"
                || item.Control == "TextMask"
            )
            {
                if (item.BindingType.ToLower().Contains("byte[]"))
                {
                    item.Control = "File";
                }
                CreateControl(item: item, visible: isVisible);
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

        [SupportedOSPlatform("windows")]
        private void CreateControl(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.LabelControl label =
                new()
                {
                    Name = item.Id,
                    AllowHtmlString = true,
                    AutoSizeInLayoutControl = false,
                    Appearance = { TextOptions = { WordWrap = DevExpress.Utils.WordWrap.Wrap } },
                };

            _itemAdded = _groupSelected.AddItem(item.Id, label);

            _itemAdded.Name = item.Id;
            _itemAdded.Text = item.Etiqueta;
            _itemAdded.SizeConstraintsType = SizeConstraintsType.Default;
            _itemAdded.OptionsTableLayoutItem.ColumnSpan = item.ColSpan;
            _itemAdded.OptionsTableLayoutItem.RowSpan = item.RowSpan;
            _itemAdded.OptionsTableLayoutItem.ColumnIndex = item.ColumnIndex;
            _itemAdded.OptionsTableLayoutItem.RowIndex = item.RowIndex;

            _itemAdded.Tag = item.ConvertToComponentProperties();

            if (item.Control == "Button")
            {
                _itemAdded.CustomDraw += ButtonCustomDraw;
            }
            else
            {
                _itemAdded.CustomDraw += ItemCustomDraw;
            }

            if (!visible)
                _itemAdded.HideToCustomization();
        }

        [SupportedOSPlatform("windows")]
        private void CreateSlave(WoItem item, bool visible)
        {
            DevExpress.XtraEditors.LabelControl label =
                new()
                {
                    Name = item.Id,
                    AllowHtmlString = true,
                    Appearance = { TextOptions = { WordWrap = DevExpress.Utils.WordWrap.Wrap } },
                };

            _itemAdded = _groupSelected.AddItem(item.Id, label);

            _itemAdded.Name = item.Id;
            _itemAdded.Text = item.Etiqueta;
            _itemAdded.OptionsTableLayoutItem.ColumnSpan = item.ColSpan;
            _itemAdded.OptionsTableLayoutItem.RowSpan = item.RowSpan;
            _itemAdded.OptionsTableLayoutItem.ColumnIndex = item.ColumnIndex;
            _itemAdded.OptionsTableLayoutItem.RowIndex = item.RowIndex;

            _itemAdded.Tag = item.ConvertToComponentProperties();

            _itemAdded.CustomDraw += SlaveCustomDraw;

            if (!visible)
                _itemAdded.HideToCustomization();
        }

        #endregion Creación de los controles


        #region Campos no asignados a un control

        /// <summary>
        /// Lista de los campos que no se han asignado a un control en el modelo.
        /// </summary>
        private List<string> _noAssignedCol = new List<string>();

        #endregion Campos no asignados a un control
    }
}
