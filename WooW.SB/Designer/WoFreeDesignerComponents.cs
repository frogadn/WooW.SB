using System;
using System.Data;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using Microsoft.CodeAnalysis;
using WooW.Core;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.DesignerHelpers;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer
{
    public partial class WoFreeDesigner : UserControl
    {
        #region Numeración de los nuevos controles

        /// <summary>
        /// Permite verificar si hay componentes con el mismo nombre al momento de
        /// crearlo y en dicho caso retorna el numero consiguiente al numero de copias que
        /// existan para reducir el numero de la numeración.
        /// </summary>
        /// <param name="componeneType"></param>
        /// <returns></returns>
        private int GetComponentNumber(string testName)
        {
            int nextId = 1;
            bool recheck = true;

            while (recheck)
            {
                var result = _layoutDesigner
                    .Items.Where(x => x.Name == $@"{testName}{nextId}")
                    .FirstOrDefault();
                recheck = (result != null);
                nextId = (result != null) ? nextId + 1 : nextId;
            }

            return nextId;
        }

        #endregion Numeración de los nuevos controles


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

            CreateTextBase();
            CreateDate();
            CreateSpin();
            CreateMemo();
            CreateButtonBase();
            CreateFormularioBase();
        }

        /// <summary>
        /// Bandera que indica si se agregó el grupo base al formulario.
        /// </summary>
        private bool _tabAdedd = false;

        /// <summary>
        /// Crea un grupo oculto que siempre se puede regenerar como componente base
        /// para que una vez agregado se regenere y se puedan generar en este caso N grupos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateGroupBase()
        {
            _woGroupFactory.ColumnSpan = 4;
            _woGroupFactory.RowSpan = 2;
            _woGroupFactory.ColumnIndex = 0;
            _woGroupFactory.RowIndex = 0;
            _woGroupFactory.InternalRows = 2;
            LayoutControlGroup newGroup = CreateGroup(
                groupText: "Grupo",
                groupName: "Grupo",
                add: false
            );
            _layoutDesigner.Root.AddGroup(newGroup);
            newGroup.HideToCustomization();
        }

        /// <summary>
        /// Crea el grupo de tabs base que siempre se puede regenerar como componente base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateTabGroupBase()
        {
            _woTabGroupFactory.ColumnSpan = 4;
            _woTabGroupFactory.RowSpan = 2;
            _woTabGroupFactory.ColumnIndex = 0;
            _woTabGroupFactory.RowIndex = 0;

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

        /// <summary>
        /// Crea el texto base que se puede regenerar como componente base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateTextBase()
        {
            CreateControl(
                item: new WoItem()
                {
                    Id = $@"Input Text{_groupIndex}",
                    Etiqueta = $@"Input Text",
                    MaskText = $@"Input Text",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    Nullable = true,
                    Control = "Text",
                    Parent = "ModelForm",
                    ColSpan = 12,
                    RowSpan = 1,
                    ColumnIndex = 0,
                    RowIndex = 0,
                    BackgorundColorContainerItem = eContainerItemColor.Primary,
                    CaptionColor = eTextColor.White,
                    ComponentFontColor = eTextColor.White,
                    Theme = "Default"
                },
                visible: false
            );

            _groupIndex++;
        }

        /// <summary>
        /// Crea el date base que se puede regenerar como componente base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateDate()
        {
            CreateControl(
                item: new WoItem()
                {
                    Id = $@"Input Date{_groupIndex}",
                    Etiqueta = $@"Input Date",
                    MaskText = $@"Input Date",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    Nullable = true,
                    Control = "Date",
                    Parent = "ModelForm",
                    ColSpan = 12,
                    RowSpan = 1,
                    ColumnIndex = 0,
                    RowIndex = 0,
                    BackgorundColorContainerItem = eContainerItemColor.Primary,
                    CaptionColor = eTextColor.White,
                    ComponentFontColor = eTextColor.White,
                    Theme = "Default"
                },
                visible: false
            );

            _groupIndex++;
        }

        /// <summary>
        /// Crea el spin base que se puede regenerar como componente base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateSpin()
        {
            CreateControl(
                item: new WoItem()
                {
                    Id = $@"Input Spin{_groupIndex}",
                    Etiqueta = $@"Input Spin",
                    MaskText = $@"Input Spin",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    Nullable = true,
                    Control = "Spin",
                    Parent = "ModelForm",
                    ColSpan = 12,
                    RowSpan = 1,
                    ColumnIndex = 0,
                    RowIndex = 0,
                    BackgorundColorContainerItem = eContainerItemColor.Primary,
                    CaptionColor = eTextColor.White,
                    ComponentFontColor = eTextColor.White,
                    Theme = "Default"
                },
                visible: false
            );

            _groupIndex++;
        }

        /// <summary>
        /// Crea el spin base que se puede regenerar como componente base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateMemo()
        {
            CreateControl(
                item: new WoItem()
                {
                    Id = $@"Input Memo{_groupIndex}",
                    Etiqueta = $@"Input Memo",
                    MaskText = $@"Input Memo",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    Nullable = true,
                    Control = "Memo",
                    Parent = "ModelForm",
                    ColSpan = 12,
                    RowSpan = 1,
                    ColumnIndex = 0,
                    RowIndex = 0,
                    BackgorundColorContainerItem = eContainerItemColor.Primary,
                    CaptionColor = eTextColor.White,
                    ComponentFontColor = eTextColor.White,
                    Theme = "Default"
                },
                visible: false
            );

            _groupIndex++;
        }

        /// <summary>
        /// Crea el spin base que se puede regenerar como componente base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateButtonBase()
        {
            CreateControl(
                item: new WoItem()
                {
                    Id = $@"Button{_groupIndex}",
                    Etiqueta = $@"Button",
                    MaskText = $@"Button",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = string.Empty,
                    Nullable = true,
                    Control = "Button",
                    Parent = "ModelForm",
                    ColSpan = 4,
                    RowSpan = 1,
                    ColumnIndex = 0,
                    RowIndex = 0,
                    BackgorundColorContainerItem = eContainerItemColor.Primary,
                    CaptionColor = eTextColor.White,
                    ComponentFontColor = eTextColor.White,
                    Theme = "Default"
                },
                visible: false
            );

            _groupIndex++;
        }

        /// <summary>
        /// Crea el spin base que se puede regenerar como componente base.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateFormularioBase()
        {
            CreateControl(
                item: new WoItem()
                {
                    Id = $@"Form{_groupIndex}",
                    Etiqueta = $@"Form",
                    MaskText = $@"Form",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = string.Empty,
                    Nullable = true,
                    Control = "Form",
                    Parent = "ModelForm",
                    ColSpan = 4,
                    RowSpan = 4,
                    ColumnIndex = 0,
                    RowIndex = 0,
                    BackgorundColorContainerItem = eContainerItemColor.Primary,
                    CaptionColor = eTextColor.White,
                    ComponentFontColor = eTextColor.White,
                    Theme = "Default"
                },
                visible: false
            );

            _groupIndex++;
        }

        #endregion Componentes base

        #region Agregar componente al formulario

        /// <summary>
        /// Método que se dispara cuando se agrega un componente X al formulario
        /// que se va a crear.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutDesigner_ItemAdded(object sender, EventArgs e)
        {
            Type controlType = sender.GetType();

            if (controlType == typeof(LayoutControlGroup))
            {
                LayoutControlGroup groupBase;

                groupBase = (LayoutControlGroup)sender;

                if (groupBase.Name == "Grupo" && groupBase.IsHidden)
                {
                    int index = GetComponentNumber("Grupo");

                    groupBase.Name = $@"Grupo{index}";
                    groupBase.Text = $@"Grupo{index}";
                    WoComponentProperties properties = (WoComponentProperties)groupBase.Tag;
                    properties.Id = $@"Grupo{index}";
                    properties.Etiqueta = $@"Grupo{index}";
                    properties.MaskText = $@"Grupo{index}";
                    groupBase.Tag = properties;

                    CreateGroupBase();
                }
            }
            else if (controlType == typeof(TabbedControlGroup))
            {
                TabbedControlGroup tabbedControlGroup;

                tabbedControlGroup = (TabbedControlGroup)sender;

                if (tabbedControlGroup.Name == "Grupo de tabs" && tabbedControlGroup.IsHidden)
                {
                    int index = GetComponentNumber("TabGroup");

                    tabbedControlGroup.Name = $@"TabGroup{index}";
                    tabbedControlGroup.Text = $@"TabGroup{index}";
                    WoComponentProperties properties = (WoComponentProperties)
                        tabbedControlGroup.Tag;
                    properties.Id = $@"TabGroup{index}";
                    properties.Etiqueta = $@"TabGroup{index}";
                    properties.MaskText = $@"TabGroup{index}";
                    tabbedControlGroup.Tag = properties;

                    index = GetComponentNumber("Tab");

                    LayoutControlGroup tabbedControl = (LayoutControlGroup)
                        tabbedControlGroup.TabPages.First();
                    tabbedControl.Name = $@"Tab{index}";
                    tabbedControl.Text = $@"Tab{index}";
                    WoComponentProperties propertiesInternal = (WoComponentProperties)
                        tabbedControl.Tag;
                    propertiesInternal.Id = $@"Tab{index}";
                    propertiesInternal.Etiqueta = $@"Tab{index}";
                    propertiesInternal.MaskText = $@"Tab{index}";
                    tabbedControl.Tag = propertiesInternal;

                    CreateTabGroupBase();

                    _tabAdedd = true;
                }
            }
            else if (controlType == typeof(LayoutControlItem))
            {
                LayoutControlItem item;

                item = (LayoutControlItem)sender;
                if (item.Text == "Form" && item.IsHidden)
                {
                    int index = GetComponentNumber("Form");

                    item.Name = $@"Form{index}";
                    item.Text = $@"Form{index}";
                    WoComponentProperties properties = (WoComponentProperties)item.Tag;
                    properties.Id = $@"Form{index}";
                    properties.Etiqueta = $@"Form{index}";
                    properties.MaskText = $@"Form{index}";
                    item.Tag = properties;

                    _itemSelected = item;

                    ///Crear los métodos para el formulario

                    CreateFormularioBase();
                }
                else if (item.Text == "Input Text" && item.IsHidden)
                {
                    int index = GetComponentNumber("Text");

                    item.Name = $@"Text{index}";
                    item.Text = $@"Text{index}";
                    WoComponentProperties properties = (WoComponentProperties)item.Tag;
                    properties.Id = $@"Text{index}";
                    properties.Etiqueta = $@"Text{index}";
                    properties.MaskText = $@"Text{index}";
                    item.Tag = properties;

                    _itemSelected = item;

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnChange",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnFocus",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnBlur",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoTextEditView",
                        name: item.Name,
                        value: $@"new WoTextEditView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );

                    CreateTextBase();
                }
                else if (item.Text == "Button" && item.IsHidden)
                {
                    int index = GetComponentNumber("Button");

                    item.Name = $@"Button{index}";
                    item.Text = $@"Button{index}";
                    WoComponentProperties properties = (WoComponentProperties)item.Tag;
                    properties.Id = $@"Button{index}";
                    properties.Etiqueta = $@"Button{index}";
                    properties.MaskText = $@"Button{index}";
                    item.Tag = properties;

                    _itemSelected = item;

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnClick",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoButtonView",
                        name: item.Name,
                        value: $@"new WoButtonView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );

                    CreateButtonBase();
                }
                else if (item.Text == "Input Date" && item.IsHidden)
                {
                    int index = GetComponentNumber("Date");

                    item.Name = $@"Date{index}";
                    item.Text = $@"Date{index}";
                    WoComponentProperties properties = (WoComponentProperties)item.Tag;
                    properties.Id = $@"Date{index}";
                    properties.Etiqueta = $@"Date{index}";
                    properties.MaskText = $@"Date{index}";
                    item.Tag = properties;

                    _itemSelected = item;

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnChange",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnFocus",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnBlur",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoDateEditView",
                        name: item.Name,
                        value: $@"new WoDateEditView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );

                    CreateDate();
                }
                else if (item.Text == "Input Spin" && item.IsHidden)
                {
                    int index = GetComponentNumber("Spin");

                    item.Name = $@"Spin{index}";
                    item.Text = $@"Spin{index}";
                    WoComponentProperties properties = (WoComponentProperties)item.Tag;
                    properties.Id = $@"Spin{index}";
                    properties.Etiqueta = $@"Spin{index}";
                    properties.MaskText = $@"Spin{index}";
                    item.Tag = properties;

                    _itemSelected = item;

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnChange",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnFocus",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnBlur",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoSpinEditView",
                        name: item.Name,
                        value: $@"new WoSpinEditView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );

                    CreateSpin();
                }
                else if (item.Text == "Input Memo" && item.IsHidden)
                {
                    int index = GetComponentNumber("Memo");

                    item.Name = $@"Memo{index}";
                    item.Text = $@"Memo{index}";
                    WoComponentProperties properties = (WoComponentProperties)item.Tag;
                    properties.Id = $@"Memo{index}";
                    properties.Etiqueta = $@"Memo{index}";
                    properties.MaskText = $@"Memo{index}";
                    item.Tag = properties;

                    _itemSelected = item;

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnChange",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnFocus",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerUserCode.CreateNewMethod(
                        methodName: $@"{item.Name}_OnBlur",
                        bodyMethod: "//Code",
                        typeMethod: "void"
                    );

                    _woSyntaxManagerModelClass.CreateAttribute(
                        type: "WoMemoEditView",
                        name: item.Name,
                        value: $@"new WoMemoEditView()",
                        accessModifier: "public",
                        classType: "Controls"
                    );

                    CreateMemo();
                }
            }
        }

        #endregion Agregar componente al formulario


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

            TabbedControlGroup tcgNew = _woTabGroupFactory.GenerateTabGroup(
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
                Row = _woGroupFactory.InternalRows,
                ColSpan = _woGroupFactory.ColumnSpan,
                RowSpan = _woGroupFactory.RowSpan,
                ColumnIndex = _woGroupFactory.ColumnIndex,
                RowIndex = _woGroupFactory.RowIndex,
                BackgorundColorGroup = eGroupColor.Primary,
                ComponentFontColor = eTextColor.White
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

            _woGroupFactory.InternalRows = internalRows;

            LayoutControlGroup tabNew = _woGroupFactory.GenerateGroupControl(
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
                Row = _woGroupFactory.InternalRows,
                ColSpan = _woGroupFactory.ColumnSpan,
                RowSpan = _woGroupFactory.RowSpan,
                ColumnIndex = _woGroupFactory.ColumnIndex,
                RowIndex = _woGroupFactory.RowIndex,
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

            LayoutControlGroup lcgNew = _woGroupFactory.GenerateGroupControl(
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
                Row = _woGroupFactory.InternalRows,
                ColSpan = _woGroupFactory.ColumnSpan,
                RowSpan = _woGroupFactory.RowSpan,
                ColumnIndex = _woGroupFactory.ColumnIndex,
                RowIndex = _woGroupFactory.RowIndex,
                BackgorundColorGroup = eGroupColor.Primary,
                ComponentFontColor = eTextColor.White
            };

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
        [SupportedOSPlatform("windows")]
        private void InstanceItem(WoItem item, bool isVisible)
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaseDesigner)).BeginInit();
            this.SuspendLayout();

            if (
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
                || item.Control == "Form"
            )
            {
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

        /// <summary>
        /// Instancia el control a través de una instancia de woItem
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
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
            else if (item.Control == "Form")
            {
                _itemAdded.CustomDraw += FormCustomDraw;
            }
            else
            {
                _itemAdded.CustomDraw += ItemCustomDraw;
            }

            if (!visible)
                _itemAdded.HideToCustomization();
        }

        #endregion Creación de los controles
    }
}
