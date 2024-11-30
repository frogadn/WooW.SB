using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using Newtonsoft.Json;
using ServiceStack;
using WooW.Core;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.SB.BlazorGenerator;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Menus.MenuModels;
using WooW.SB.Menus.MenusComponents;
using WooW.SB.Menus.MenusHelper;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Menus
{
    public partial class WoMenuDesigner : UserControl
    {
        #region Instancias singleton
        /// <summary>
        /// instancia singleton
        /// </summary>
        private Proyecto _project { get; set; } = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Paths

        /// <summary>
        /// Variable usada para almacenar el path de los menus
        /// </summary>
        private string _pathMenus = string.Empty;

        /// <summary>
        /// Carpeta bin de donde toma el servicio los json con los menus.
        /// </summary>
        private string _pathTempMenus = string.Empty;

        /// <summary>
        /// Carpeta donde se encuentran los iconos
        /// </summary>
        private string _iconPath = "";

        #endregion Paths

        #region General attributes
        public EventHandler<string> EventsStopBlazor;

        /// <summary>
        /// Variable global para almacenar el valor del tipo de ejecución en blazor
        /// </summary>
        public string selectedOption = string.Empty;

        /// <summary>
        /// Variable global que almacena el árbol actual del menú
        /// </summary>
        public WoMenuProperties _woMenuDataSet = new WoMenuProperties();

        /// <summary>
        /// Contiene las propriedades del elemento seleccionado
        /// </summary>
        private WoMenuProperties _selectedNode = new WoMenuProperties();

        /// <summary>
        /// Contiene las propriedades del elemento seleccionado
        /// </summary>
        private TreeListNode _selectedNodeTreeList = new TreeListNode();

        #endregion General attributes

        #region Constructor general

        public WoMenuDesigner()
        {
            InitializeComponent();
            try
            {
                ///Verificacion de directorios
                _iconPath = _project.DirLayOuts + "\\icons";
                _pathMenus = _project.DirMenus;
                _pathTempMenus = _project.DirMenusTemp;

                if (!Directory.Exists(_pathMenus))
                    WoDirectory.CreateDirectory(_pathMenus);

                if (!Directory.Exists(_pathTempMenus))
                    WoDirectory.CreateDirectory(_pathTempMenus);

                dragDropEvents1.DragDrop += TreeDragDropEvents_DragDrop;
                dragDropEvents1.DragOver += TreeDragDropEvents_DragOver;
                //InstanceThemeSelector();

                ChargeGrid();

                InitializeCustomComponents();

                InstanceThemeSelector();

                ///Agrega el titulo a la grid de menus
                if (grdMenusView.Columns.Count > 0)
                {
                    grdMenusView.Columns[0].Caption = "Menu";
                    grdMenusView.Columns[1].Caption = "Roles";
                }
                lblHelp.Text = "Ayuda: ";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                //throw new Exception();
            }
        }

        private void TreeDragDropEvents_DragOver(object sender, dynamic e)
        {
            try
            {
                Cursor.Current = Cursors.Hand;
                //e.Effect = DragDropEffects.Move;
                // Cambiar el cursor durante el evento DragOver
                //e.Effect = DragDropEffects.Move;
                Cursor.Current = Cursors.Hand; // Cambia el cursor a una mano, por ejemplo

                // O puedes personalizar el cursor proporcionando un archivo de imagen
                // Cursor.Current = new Cursor("ruta_del_archivo_del_cursor.cur");
            }
            catch (Exception ex)
            {
                var a = ex;
                throw;
            }
        }
        #endregion Constructor general

        #region RowDataGrid
        /// <summary>
        /// Variable auxiliar para
        /// </summary>
        public void SetFocusMenu(string menuName)
        {
            grdMenusView.RefreshData();
            // Encuentra el índice de la fila basado en el valor del registro
            int filaIndice = grdMenusView.LocateByValue("Menu", menuName);

            if (filaIndice != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                // Asegúrate de que la fila esté visible
                grdMenusView.MakeRowVisible(filaIndice);

                // Establece la fila como enfocada
                grdMenusView.FocusedRowHandle = filaIndice;
            }
            else
            {
                Console.WriteLine($"No se encontró el registro con el nombre: {menuName}");
            }
        }
        #endregion RowDataGrid

        #region Cargar grid de menus

        /// <summary>
        /// Lista de menus recuperados desde la carpeta
        /// </summary>
        private List<string> _menuList = new List<string>();

        /// <summary>
        /// clase auxiliar para agregar los menus en el datagrid
        /// </summary>
        public class FileMenus
        {
            public string Menu { get; set; }
            public string Roles { get; set; }
        }

        /// <summary>
        /// Función para asignar el data source al data grid con los modelos permitidos
        /// </summary>
        public void ChargeGrid()
        {
            _menuList.Clear();
            List<FileMenus> listaArchivos = new List<FileMenus>();

            if (Directory.Exists(_pathMenus))
            {
                _menuList = WoDirectory.ReadDirectoryFiles(_pathMenus);

                //_menuList = _menuList
                //    .Select(x => x.Split("\\")[x.Split("\\").Length - 1].Replace(".json", ""))
                //    .ToList();

                foreach (string archivo in _menuList)
                {
                    string nombreArchivo = Path.GetFileName(archivo).Replace(".json", "");
                    string json = WoDirectory.ReadFile(archivo);
                    if (json.IsNullOrStringEmpty()) // ToDo: send log
                        return;

                    WoMenuProperties auxMenu = JsonConvert.DeserializeObject<WoMenuProperties>(
                        json
                    );
                    string rolesCollection = string.Empty;

                    foreach (Rol item in auxMenu.Roles)
                    {
                        // Dividir la cadena usando el espacio en blanco como delimitador
                        //string[] palabras = item.Id.Split(' ');

                        // Tomar la primera palabra (elemento en el índice 0)
                        //string clearRol = palabras[0];
                        if (item.Id != "Todos")
                        {
                            rolesCollection = rolesCollection + item.Id + ",";
                        }
                    }
                    listaArchivos.Add(
                        new FileMenus
                        {
                            Menu = nombreArchivo,
                            Roles =
                                auxMenu.Roles.Count > 0 ? rolesCollection.TrimEnd(',') : "Sin rol"
                        }
                    );
                }
                if (_menuList != null)
                {
                    grdMenus.DataSource = listaArchivos;
                }
            }
        }

        #endregion Cargar grid de menus

        #region Eventos de la grid

        /// <summary>
        /// Lista de menus base
        /// </summary>
        private List<string> _listMenuBase = new List<string>();

        /// <summary>
        /// Evento que se dispara al seleccionar un elemento de la grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdMenusView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            InitializeTreeList();

            #region Carga del tema definido

            _woThemeSelector.SetTheme(_woMenuDataSet.Theme);

            #endregion Carga del tema definido

            _menuView.MoveFirst();

            _selectedNode = _woMenuDataSet;
            if (selectedOption == "Watch Server" || selectedOption == "Watch Wasm")
            {
                WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();

                woBlazorGenerator.ReGenerateMenuTest(_serverProjectName, _woMenuDataSet);
            }
            EventsStopBlazor?.Invoke(null, null);
        }

        #endregion Eventos de la grid

        #region Comportamiento de los paneles
        /// <summary>
        ///
        /// </summary>
        private bool _isEditMode = false;

        /// <summary>
        /// Función para definir la vista de inicio
        /// </summary>
        public void InitializeMode()
        {
            _isEditMode = false;
            //spcMenusUpdate.Panel1Collapsed = false;
            spcMenusUpdate.PanelVisibility = SplitPanelVisibility.Panel1;

            //spcDesigner.Panel1Collapsed = false;
            spcDesigner.PanelVisibility = SplitPanelVisibility.Panel1;

            if (_menuView != null)
            {
                _menuView.Enabled = false;
                _menuView.ClearSelection();
            }
        }

        /// <summary>
        /// Función para definir la vista de edición
        /// </summary>
        public void EditMode()
        {
            _isEditMode = true;
            //spcMenusUpdate.Panel1Collapsed = true;
            spcMenusUpdate.PanelVisibility = SplitPanelVisibility.Panel2;

            //spcDesigner.Panel1Collapsed = false;
            spcDesigner.PanelVisibility = SplitPanelVisibility.Both;
            _menuView.Enabled = true;
            cmbAllProcess.SelectedIndex = 0;
            cmbSelectedProcess.SelectedIndex = 0;
        }

        #endregion Comportamiento de los paneles


        #region Carga de componentes custom

        /// <summary>
        /// Inicializa los componentes que se pueden agregar al menu
        /// </summary>
        private void InitializeCustomComponents()
        {
            List<string> listsub = new List<string>();
            List<string> listItem = new List<string>();

            listsub.Add("Nuevo SubMenu");
            listItem.Add("Página personalizada");

            SubMenusBox.DataSource = listsub;
            MenuCustom.DataSource = listItem;

            SubMenusBox.Enter += SelectedCustomMenu;
            SubMenusBox.Click += SelectedCustomMenu;
            MenuCustom.Enter += SelectedCustomItem;
            MenuCustom.Click += SelectedCustomItem;
        }

        #region Sub menus custom

        private void SelectedCustomMenu(object sender, EventArgs e)
        {
            _menuView.ClearSelection();
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode node in _menuView.Selection)
            {
                node.Selected = false;
            }
            int next = GetNextId("Nuevo SubMenu");
            string id = $@"Nuevo SubMenu";
            if (next > 0)
            {
                id = $@"Nuevo SubMenu{next}";
            }
            string validateLabel = EtiquetaCol.Get("NuevoSubmenu");
            if (
                validateLabel == " ETIQUETA NO EXISTE"
                || validateLabel == " ETIQUETA CON IDIOMA NO EXISTE"
            )
            {
                XtraMessageBox.Show(
                    "Hacen falta las etiqueta de 'Nuevo Submenu', favor de agregarla.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                //menuCustom.Focus();
                return;
            }
            _selectedNode = new WoMenuProperties()
            {
                Id = Etiqueta.ToId(id),
                Theme = _themeSelected,
                Label = validateLabel,
                MaskText = validateLabel,
                TypeContainer = eTypeContainer.SubMenu,
                TypeItem = eTypeItem.None,
                Enable = _woMenuDataSet.Enable,
                DropDown = _woMenuDataSet.DropDown,
                BackgroundColor = _woMenuDataSet.BackgroundColor,
                BeginRow = _woMenuDataSet.BeginRow,
                BorderColor = _woMenuDataSet.BorderColor,
                CustomDesignAplied = false,
                Decoration = _woMenuDataSet.Decoration,
                EndingRow = _woMenuDataSet.EndingRow,
                FontColor = _woMenuDataSet.FontColor,
                MenuSize = _woMenuDataSet.MenuSize,
                Icon = _woMenuDataSet.Icon,
                IsLogin = _woMenuDataSet.IsLogin,
                Italic = _woMenuDataSet.Italic,
                Orientation = _woMenuDataSet.Orientation,
                ExpandedNode = _woMenuDataSet.ExpandedNode,
                Wide = _woMenuDataSet.Wide,
                Roles = _woMenuDataSet.Roles,
                Reference = "/",
                BackgroundRoot = _woMenuDataSet.BackgroundRoot,
                BorderColorRoot = _woMenuDataSet.BorderColorRoot,
                NotFound = _woMenuDataSet.NotFound
            };
        }
        #endregion Sub menus custom

        #region Items custom

        private void SelectedCustomItem(object sender, EventArgs e)
        {
            _menuView.ClearSelection();
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode node in _menuView.Selection)
            {
                node.Selected = false;
            }
            int next = GetNextId("Nueva Página");
            string id = $@"Nueva Página";
            if (next > 0)
            {
                id = $@"Nueva Página{next}";
            }
            string validateLabel = EtiquetaCol.Get("NuevaPagina");
            if (
                validateLabel == " ETIQUETA NO EXISTE"
                || validateLabel == " ETIQUETA CON IDIOMA NO EXISTE"
            )
            {
                XtraMessageBox.Show(
                    "Hacen falta las etiqueta de 'Nueva Página', favor de agregarla.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                //SubMenusBox.Focus();
                return;
            }
            _selectedNode = new WoMenuProperties()
            {
                Id = Etiqueta.ToId(id),
                Theme = _themeSelected,
                Label = validateLabel,
                MaskText = validateLabel,
                TypeContainer = eTypeContainer.None,
                TypeItem = eTypeItem.MenuItem,
                Enable = _woMenuDataSet.Enable,
                DropDown = _woMenuDataSet.DropDown,
                BackgroundColor = _woMenuDataSet.BackgroundColor,
                BeginRow = _woMenuDataSet.BeginRow,
                BorderColor = _woMenuDataSet.BorderColor,
                CustomDesignAplied = false,
                Decoration = _woMenuDataSet.Decoration,
                EndingRow = _woMenuDataSet.EndingRow,
                FontColor = _woMenuDataSet.FontColor,
                MenuSize = _woMenuDataSet.MenuSize,
                Icon = _woMenuDataSet.Icon,
                IsLogin = _woMenuDataSet.IsLogin,
                Italic = _woMenuDataSet.Italic,
                Orientation = _woMenuDataSet.Orientation,
                ExpandedNode = _woMenuDataSet.ExpandedNode,
                Wide = _woMenuDataSet.Wide,
                Roles = _woMenuDataSet.Roles,
                Reference = "/",
                BackgroundRoot = _woMenuDataSet.BackgroundRoot,
                BorderColorRoot = _woMenuDataSet.BorderColorRoot,
                InNewTab = _woMenuDataSet.InNewTab,
                NotFound = _woMenuDataSet.NotFound
            };
        }

        #endregion Items custom

        #endregion Carga de componentes custom


        #region TreeList
        /// <summary>
        /// Función auxiliar para perder el foco de tree list de los menus
        /// </summary>
        public void LeaveTreeList()
        {
            _menuView.ClearSelection();
        }

        /// <summary>
        /// Instancia principal de la vista del menu
        /// </summary>
        private TreeList _menuView;

        [SupportedOSPlatform("windows")]
        public void InitializeTreeList(bool chargeToJson = true, bool chargeProcess = true)
        {
            pnlTreeList.Controls.Clear();

            TreeListColumn column = new TreeListColumn();
            column.Caption = "Menú";
            column.FieldName = "Menu";
            column.Name = "colNames2";
            column.Visible = true;
            column.VisibleIndex = 0;

            _menuView = null;
            _menuView = new TreeList();

            _menuView.Appearance.Row.Font = new System.Drawing.Font(
                "Tahoma",
                12F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point
            );
            _menuView.Appearance.Row.Options.UseFont = true;
            behaviorManager1.SetBehaviors(
                _menuView,
                new DevExpress.Utils.Behaviors.Behavior[]
                {
                    DragDropBehavior.Create(
                        typeof(DevExpress.XtraTreeList.TreeListDragDropSource),
                        true,
                        true,
                        true,
                        true,
                        dragDropEvents1
                    )
                }
            );
            _menuView.Columns.AddRange(
                new DevExpress.XtraTreeList.Columns.TreeListColumn[] { column }
            );
            _menuView.Dock = System.Windows.Forms.DockStyle.Fill;
            _menuView.Font = new System.Drawing.Font(
                "Tahoma",
                12F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point
            );
            _menuView.Location = new System.Drawing.Point(0, 0);
            _menuView.Name = "menusTree";
            _menuView.OptionsBehavior.Editable = false;
            _menuView.OptionsBehavior.ReadOnly = true;
            _menuView.Size = new System.Drawing.Size(544, 834);
            _menuView.TabIndex = 7;
            _menuView.Enabled = _isEditMode;
            //_menuView.Click += menusTree_Click;
            //_menuView.FocusedColumnChanged += menusTree_Click;
            _menuView.FocusedNodeChanged += menusTree_Click;
            //_menuView.FocusedNodeChanged += menusTree_FocusedNodeChanged;
            _menuView.CustomDrawNodeCell += menusTree_CustomDrawNodeCell;
            _menuView.OptionsView.ShowIndicator = false;
            _menuView.OptionsView.ShowColumns = false;
            _menuView.MouseHover += TreeDragDropEvents_DragOver;
            _menuView.MouseEnter += TreeDragDropEvents_DragOver;
            _menuView.MouseLeave += TreeDragDropEvents_DragOver;
            //_menuView.DragOver += TreeDragDropEvents_DragOver;
            _menuView.DragDrop += TreeDragDropEvents_DragDrop;

            _menuView.AfterExpand += AfterExpand;
            _menuView.AfterCollapse += AfterCollapse;

            pnlTreeList.Controls.Add(_menuView);
            _menuView.Dock = DockStyle.Fill;

            if (chargeToJson)
            {
                ChargeJsonToMenu();
            }
            else
            {
                CreateTreeList(_woMenuDataSet);
            }
            if (chargeProcess)
            {
                InitializeAllProcessList();

                InitializeSelectedProcessList();
            }
            _flagMenuValidator = false;
        }
        #endregion TreeList

        #region Eventos del tree list

        /// <summary>
        /// Evento que se dispara al expandir el nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterExpand(object sender, NodeEventArgs e)
        {
            dynamic properties = e;
            WoMenuProperties selectedNodeProperties = properties.Node.Data;
            selectedNodeProperties.ProcessList = _woMenuDataSet.ProcessList;
            selectedNodeProperties.ExpandedNode = true;
            WoMenuProperties newDataSet = RemplazeNode(_woMenuDataSet, selectedNodeProperties);
            _woMenuDataSet = newDataSet;
        }

        /// <summary>
        /// Evento que se dispara al contraer el nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterCollapse(object sender, NodeEventArgs e)
        {
            dynamic properties = e;
            WoMenuProperties selectedNodeProperties = properties.Node.Data;
            selectedNodeProperties.ProcessList = _woMenuDataSet.ProcessList;
            selectedNodeProperties.ExpandedNode = false;
            WoMenuProperties newDataSet = RemplazeNode(_woMenuDataSet, selectedNodeProperties);
            _woMenuDataSet = newDataSet;
        }

        #endregion Eventos del tree list

        #region remplazo de nodos

        /// <summary>
        /// Remplaza el nodo del data set por el ingresado mientras los identificadores coincidan
        /// </summary>
        /// <param name="container"></param>
        /// <param name="remplace"></param>
        private WoMenuProperties RemplazeNode(WoMenuProperties container, WoMenuProperties remplace)
        {
            WoMenuProperties newMenuContainer = new WoMenuProperties();
            if (container.Id == remplace.Id)
            {
                newMenuContainer = new WoMenuProperties()
                {
                    ProcessList = remplace.ProcessList,
                    Orientation = remplace.Orientation,
                    Order = remplace.Order,
                    Roles = remplace.Roles,
                    Reference = remplace.Reference,
                    ContentCol = new List<WoMenuProperties>(),
                    FontColor = remplace.FontColor,
                    MenuSize = remplace.MenuSize,
                    Italic = remplace.Italic,
                    Wide = remplace.Wide,
                    Decoration = remplace.Decoration,
                    Icon = remplace.Icon,
                    DropDown = remplace.DropDown,
                    BackgroundColor = remplace.BackgroundColor,
                    BorderColor = remplace.BorderColor,
                    TypeContainer = remplace.TypeContainer,
                    TypeItem = remplace.TypeItem,
                    Enable = remplace.Enable,
                    Parent = remplace.Parent,
                    Id = remplace.Id,
                    Label = remplace.Label,
                    MaskText = remplace.MaskText,
                    Theme = remplace.Theme,
                    ThemeSuperiorAplied = remplace.ThemeSuperiorAplied,
                    CustomDesignAplied = remplace.CustomDesignAplied,
                    ExpandedNode = remplace.ExpandedNode,
                    Added = remplace.Added,
                    BeginRow = remplace.BeginRow,
                    InNewTab = remplace.InNewTab,
                    IsExternalReference = remplace.IsExternalReference,
                    IsLogin = remplace.IsLogin,
                    Process = remplace.Process,
                    EndingRow = remplace.EndingRow,
                    BackgroundRoot = remplace.BackgroundRoot,
                    BorderColorRoot = remplace.BorderColorRoot,
                    NotFound = remplace.NotFound
                };
            }
            else
            {
                newMenuContainer = new WoMenuProperties()
                {
                    ProcessList = container.ProcessList,
                    Orientation = container.Orientation,
                    Order = container.Order,
                    Roles = container.Roles,
                    Reference = container.Reference,
                    ContentCol = new List<WoMenuProperties>(),
                    FontColor = container.FontColor,
                    MenuSize = container.MenuSize,
                    Italic = container.Italic,
                    Wide = container.Wide,
                    Decoration = container.Decoration,
                    Icon = container.Icon,
                    DropDown = container.DropDown,
                    BackgroundColor = container.BackgroundColor,
                    BorderColor = container.BorderColor,
                    TypeContainer = container.TypeContainer,
                    TypeItem = container.TypeItem,
                    Enable = container.Enable,
                    Parent = container.Parent,
                    Id = container.Id,
                    Label = container.Label,
                    MaskText = container.MaskText,
                    Theme = container.Theme,
                    ThemeSuperiorAplied = container.ThemeSuperiorAplied,
                    CustomDesignAplied = container.CustomDesignAplied,
                    ExpandedNode = container.ExpandedNode,
                    Added = container.Added,
                    Process = container.Process,
                    BeginRow = container.BeginRow,
                    IsLogin = container.IsLogin,
                    InNewTab = container.InNewTab,
                    IsExternalReference = container.IsExternalReference,
                    EndingRow = container.EndingRow,
                    BackgroundRoot = container.BackgroundRoot,
                    BorderColorRoot = container.BorderColorRoot,
                    NotFound = container.NotFound
                };
            }

            if (container.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties subNode in container.ContentCol)
                {
                    if (remplace.Id == subNode.Id)
                    {
                        newMenuContainer.ContentCol.Add(remplace);
                    }
                    else
                    {
                        newMenuContainer.ContentCol.Add(RemplazeNode(subNode, remplace));
                    }
                }
            }

            return newMenuContainer;
        }

        #endregion remplazo de nodos


        #region Cargar el menu

        /// <summary>
        /// Carga el árbol en función del menu seleccionado en la grid
        /// </summary>
        private void ChargeJsonToMenu()
        {
            if (grdMenusView.RowCount > 0)
            {
                FileMenus menuFocus = grdMenusView.GetFocusedRow() as FileMenus;

                string json = WoDirectory.ReadFile($@"{_pathMenus}\{menuFocus.Menu}.json");
                if (json.IsNullOrStringEmpty()) // ToDo: send log
                    return;

                WoMenuProperties _woMenu = JsonConvert.DeserializeObject<WoMenuProperties>(json);
                WoMenuVerifyHelper verify = new WoMenuVerifyHelper();
                WoMenuProperties MenuVerificated = verify.MenuVerificated(_woMenu);
                //se asigna el menu con la propiedad NotFound actualizada
                _woMenuDataSet = MenuVerificated;

                _themeSelected = _woMenuDataSet.Theme;
                CreateTreeList(_woMenuDataSet);
                //_flagMenuValidator = false;
            }
        }

        /// <summary>
        /// Función recursiva usada para crear el arbol y agregarlo a _menuView
        /// </summary>
        /// <param name="woMenu"></param>
        /// <param name="parent"></param>
        private void CreateTreeList(WoMenuProperties woMenu, TreeListNode parent = null)
        {
            woMenu.Order = parent == null ? 0 : parent.Level;
            dynamic newParent = new TreeListNode();
            TreeListNode newnodeFocus = new TreeListNode();

            newParent = _menuView.AppendNode(new WoMenuProperties[] { woMenu }, parent, woMenu);

            if (woMenu.ContentCol != null)
            {
                if (woMenu.ContentCol.Count > 0)
                {
                    foreach (var SubNode in woMenu.ContentCol)
                    {
                        CreateTreeList(SubNode, newParent);
                    }
                }
            }
            if (woMenu.ExpandedNode)
            {
                newParent.Expand();
            }
            if (newParent.Tag.Id == nodeFocused.Id)
            {
                _menuView.SetFocusedNode(newParent);
            }
        }

        #endregion Cargar el menu

        #region Eventos del menu
        /// <summary>
        /// Evento del Tree list de los menus que se detona cuando se cambia el foco de selección de un nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menusTree_Click(dynamic sender, dynamic e)
        {
            if (sender.Selection.Count > 0)
            {
                _selectedNode = (WoMenuProperties)sender.Selection[0].Tag;
                _selectedNodeTreeList = sender.Selection[0];
                WoMenuProperties woMenuProperties = new WoMenuProperties();
                dynamic woConvert = sender.Selection[0].Tag;
                if (sender.Selection[0].Tag is WoMenuProperties)
                {
                    woMenuProperties = (WoMenuProperties)sender.Selection[0].Tag;
                }
                else
                {
                    dynamic Woconvert = sender.Selection[0].Tag;
                    if (woConvert != null)
                        woMenuProperties = Woconvert.ConvertToComponentProperties();
                }
                if (woMenuProperties.Id.Contains("Nueva Página"))
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "Reference",
                            "IsLogin",
                            "BeginRow",
                            "EndingRow",
                            "Wide",
                            "InNewTab",
                            "Italic"
                        },
                        true
                    );
                }
                else
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "Reference", "IsExternalReference", "Wide", "Italic" },
                        false
                    );
                }
                if (woMenuProperties.Id.Contains("Nuevo Submenu"))
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "IsLogin" },
                        true
                    );
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "BeginRow,EndingRow" },
                        false
                    );
                }
                if (_selectedNode.TypeItem == eTypeItem.MenuItem)
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "InNewTab",
                            "IsLogin",
                            "BeginRow",
                            "EndingRow",
                            "Wide",
                            "Italic"
                        },
                        true
                    );
                    //HidePropertiesHelper.ModifyBrowsableAttribute(
                    //    woMenuProperties,
                    //    new List<string>() { "Italic" },
                    //    false
                    //);
                }
                else if (_selectedNode.TypeContainer == eTypeContainer.SubMenu)
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "InNewTab",
                            "Reference",
                            "Wide",
                            "IsLogin",
                            "DropDown",
                            "MenuSize",
                            "BeginRow",
                            "EndingRow"
                        },
                        false
                    );
                }
                if (_selectedNode.TypeContainer == eTypeContainer.Menu)
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "Label",
                            "CustomDesignAplied",
                            "Rol",
                            "InNewTab",
                            "Wide",
                            "BeginRow",
                            "EndingRow"
                        },
                        false
                    );
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "BackgroundRoot",
                            "BorderColorRoot",
                            "IsLogin",
                            "DropDown",
                            "MenuSize"
                        },
                        true
                    );
                }
                else
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "Label", "CustomDesignAplied" },
                        true
                    );
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "BackgroundRoot",
                            "BorderColorRoot",
                            "Rol",
                            "DropDown",
                            "MenuSize"
                        },
                        false
                    );
                }

                //Aqui se configura el property image
                propMenus.SelectedObject = woMenuProperties;
                if (
                    _selectedNode.TypeContainer is eTypeContainer.SubMenu
                    || _selectedNode.TypeContainer is eTypeContainer.Menu
                )
                {
                    lblModeloSelected.Text = "Proceso/Menú: " + woMenuProperties.Id;
                }
                else
                {
                    lblModeloSelected.Text = "Modelo/Página: " + woMenuProperties.Id;
                }
                if (woMenuProperties.Reference.ToString().EndsWith("GridList"))
                {
                    string url = woMenuProperties.Reference.ToString();

                    // Dividir la URL en partes usando el carácter '/'
                    string[] partes = url.Split('/');

                    // Obtener la primera y tercera palabra (índices 0 y 2)
                    string primeraPalabra = partes.Length > 0 ? partes[0] : string.Empty;
                    string terceraPalabra = partes.Length > 2 ? partes[2] : string.Empty;

                    lblReference.Text =
                        "Referencia: "
                        + primeraPalabra
                        + "/List/"
                        + terceraPalabra.Replace("GridList", "");
                }
                if (
                    woMenuProperties
                        .Reference.ToString()
                        .EndsWith("ReportOdata", StringComparison.OrdinalIgnoreCase)
                )
                {
                    string url = woMenuProperties.Reference.ToString();

                    // Dividir la URL en partes usando el carácter '/'
                    string[] partes = url.Split('/');

                    // Obtener la primera y tercera palabra (índices 0 y 2)
                    string primeraPalabra = partes.Length > 0 ? partes[0] : string.Empty;
                    string terceraPalabra = partes.Length > 2 ? partes[2] : string.Empty;

                    lblReference.Text =
                        "Referencia: "
                        + primeraPalabra
                        + "/report/"
                        + terceraPalabra.Replace(
                            "ReportOdata",
                            "",
                            StringComparison.OrdinalIgnoreCase
                        );
                }
                else
                {
                    lblReference.Text = "Referencia: " + woMenuProperties.Reference;
                }
                if (woMenuProperties.NotFound)
                {
                    lblNotFound.Text =
                        "Advertencia! el modelo/lista/reporte seleccionado ya no existe!";
                }
                else
                {
                    lblNotFound.Text = "";
                }
            }
        }

        /// <summary>
        /// Evento del Tree list de los menus que se detona cuando se cambia el foco de selección de un nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menusTree_FocusedNodeChanged(
            object sender,
            DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e
        )
        {
            if (e.Node != null)
            {
                _selectedNode = (WoMenuProperties)e.Node.Tag;
                _selectedNodeTreeList = e.Node;
                WoMenuProperties woMenuProperties = new WoMenuProperties();
                dynamic woConvert = e.Node.Tag;
                if (e.Node.Tag is WoMenuProperties)
                {
                    woMenuProperties = (WoMenuProperties)e.Node.Tag;
                }
                else
                {
                    dynamic Woconvert = e.Node.Tag;
                    if (woConvert != null)
                        woMenuProperties = Woconvert.ConvertToComponentProperties();
                }
                if (woMenuProperties.Id.Contains("Nueva Página"))
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "Reference",
                            "IsLogin",
                            "BeginRow",
                            "EndingRow",
                            "Wide",
                            "InNewTab",
                            "Italic"
                        },
                        true
                    );
                }
                else
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "Reference", "IsExternalReference", "Wide", "Italic" },
                        false
                    );
                }
                if (woMenuProperties.Id.Contains("Nuevo Submenu"))
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "IsLogin" },
                        true
                    );
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "BeginRow,EndingRow" },
                        false
                    );
                }
                if (_selectedNode.TypeItem == eTypeItem.MenuItem)
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "InNewTab",
                            "IsLogin",
                            "BeginRow",
                            "EndingRow",
                            "Wide",
                            "Italic"
                        },
                        true
                    );
                    //HidePropertiesHelper.ModifyBrowsableAttribute(
                    //    woMenuProperties,
                    //    new List<string>() { "Italic" },
                    //    false
                    //);
                }
                else if (_selectedNode.TypeContainer == eTypeContainer.SubMenu)
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "InNewTab",
                            "Reference",
                            "Wide",
                            "IsLogin",
                            "DropDown",
                            "MenuSize",
                            "BeginRow",
                            "EndingRow"
                        },
                        false
                    );
                }
                if (_selectedNode.TypeContainer == eTypeContainer.Menu)
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "Label",
                            "CustomDesignAplied",
                            "Rol",
                            "InNewTab",
                            "Wide",
                            "BeginRow",
                            "EndingRow"
                        },
                        false
                    );
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "BackgroundRoot",
                            "BorderColorRoot",
                            "IsLogin",
                            "DropDown",
                            "MenuSize"
                        },
                        true
                    );
                }
                else
                {
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>() { "Label", "CustomDesignAplied" },
                        true
                    );
                    HidePropertiesHelper.ModifyBrowsableAttribute(
                        woMenuProperties,
                        new List<string>()
                        {
                            "BackgroundRoot",
                            "BorderColorRoot",
                            "Roles",
                            "DropDown",
                            "MenuSize"
                        },
                        false
                    );
                }

                //Aqui se configura el property image
                propMenus.SelectedObject = woMenuProperties;
                if (
                    _selectedNode.TypeContainer is eTypeContainer.SubMenu
                    || _selectedNode.TypeContainer is eTypeContainer.Menu
                )
                {
                    lblModeloSelected.Text = "Proceso/Menú: " + woMenuProperties.Id;
                }
                else
                {
                    lblModeloSelected.Text = "Modelo/Página: " + woMenuProperties.Id;
                }
                if (
                    woMenuProperties
                        .Reference.ToString()
                        .EndsWith("GridList", StringComparison.OrdinalIgnoreCase)
                )
                {
                    string url = woMenuProperties.Reference.ToString();

                    // Dividir la URL en partes usando el carácter '/'
                    string[] partes = url.Split('/');

                    // Obtener la primera y tercera palabra (índices 0 y 2)
                    string primeraPalabra = partes.Length > 0 ? partes[0] : string.Empty;
                    string terceraPalabra = partes.Length > 2 ? partes[2] : string.Empty;

                    lblReference.Text =
                        "Referencia: "
                        + primeraPalabra
                        + "/List/"
                        + terceraPalabra.Replace("GridList", "");
                }
                else
                {
                    lblReference.Text = "Referencia: " + woMenuProperties.Reference;
                }
            }
        }

        /// <summary>
        /// Variable auxiliar para indicar que ya se mando el mensaje
        /// de que el color de letra y no repetir por cada nodo que se contenga
        /// </summary>
        private bool _flagMenuValidator = false;

        /// <summary>
        /// Función que se detona cuando se cambia el foco en el control de property  grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void propMenus_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            SupportTextSearchValue();
            if (
                e.Row.Name == "rowFontColor"
                || e.Row.Name == "rowBackgroundColor"
                || e.Row.Name == "rowBackgroundRoot"
            )
            {
                if (
                    _selectedNode.BackgroundColor.ToString() == _selectedNode.FontColor.ToString()
                    || _woMenuDataSet.BackgroundRoot.ToString()
                        == _selectedNode.FontColor.ToString()
                        && _selectedNode.BackgroundColor == eMenuColor.Default
                        && _selectedNode.Order == 0
                )
                {
                    if (!_flagMenuValidator)
                    {
                        if (
                            XtraMessageBox.Show(
                                "El color de fondo y el color de letra son el mismo. ¿Desea continuar?",
                                "Confirmación",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning
                            ) == DialogResult.Yes
                        )
                        {
                            _flagMenuValidator = true;
                        }
                        else
                        {
                            _flagMenuValidator = true;
                            _selectedNode = _selectedNodeOldVersion;
                        }
                    }
                }
            }
            if (
                e.Row.Name == "rowIsExternalReference"
                || e.Row.Name == "rowReference"
                || e.Row.Name == "rowIsLogin"
                || e.Row.Name == "rowCustomDesignAplied"
                || e.Row.Name == "rowLabel"
                || e.Row.Name == "rowInNewTab"
            )
            {
                if (e.Row.Name == "rowReference")
                {
                    //Validamos que la url sea valida
                    // Definir el patrón que solo permite cadenas tipo de URL
                    string patron = @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";

                    // Crear una instancia de Regex
                    Regex regex = new Regex(patron);

                    // Verificar si la cadena cumple con el patrón
                    dynamic auxValueIcon = sender;
                    bool esValida = regex.IsMatch(auxValueIcon.EditingValue.ToString());

                    if (!esValida)
                    {
                        XtraMessageBox.Show(
                            "La referencia no es valida.",
                            "Alerta",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );
                        _selectedNode.Reference = "/";
                        propMenus.Refresh();
                        return;
                    }
                }
            }
            else
            {
                dynamic auxValueIcon = sender;
                if (auxValueIcon.EditingValue.ToString() != "Sin Icono")
                {
                    _selectedNode.CustomDesignAplied = true;
                }
            }
            if (e.Row.Name == "rowCustomDesignAplied")
            {
                dynamic valueCustomAply = e.Value;
                if (!valueCustomAply)
                {
                    if (
                        XtraMessageBox.Show(
                            "¿Desea cambiar la propiedad del menú? se perdera el diseño customizado",
                            "Confirmación",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        ) == DialogResult.Yes
                    )
                    {
                        _selectedNode.Theme = _themeSelected;
                        _selectedNode.Enable = _woMenuDataSet.Enable;
                        _selectedNode.DropDown = _woMenuDataSet.DropDown;
                        _selectedNode.BackgroundColor = _woMenuDataSet.BackgroundColor;
                        _selectedNode.BeginRow = _woMenuDataSet.BeginRow;
                        _selectedNode.BorderColor = _woMenuDataSet.BorderColor;
                        _selectedNode.CustomDesignAplied = false;
                        _selectedNode.Decoration = _woMenuDataSet.Decoration;
                        _selectedNode.EndingRow = _woMenuDataSet.EndingRow;
                        _selectedNode.FontColor = _woMenuDataSet.FontColor;
                        _selectedNode.MenuSize = _woMenuDataSet.MenuSize;
                        _selectedNode.Icon = _woMenuDataSet.Icon;
                        _selectedNode.IsLogin = _woMenuDataSet.IsLogin;
                        _selectedNode.Italic = _woMenuDataSet.Italic;
                        _selectedNode.Orientation = _woMenuDataSet.Orientation;
                        _selectedNode.DropDown = _woMenuDataSet.DropDown;
                        _selectedNode.ExpandedNode = _woMenuDataSet.ExpandedNode;
                        _selectedNode.Wide = _woMenuDataSet.Wide;
                        _selectedNode.Roles = _woMenuDataSet.Roles;
                        _selectedNode.BackgroundRoot = _woMenuDataSet.BackgroundRoot;
                        _selectedNode.BorderColorRoot = _woMenuDataSet.BorderColorRoot;
                        _selectedNode.InNewTab = _woMenuDataSet.InNewTab;
                    }
                    else
                    {
                        _selectedNode.CustomDesignAplied = true;
                    }
                }
            }

            #region propiedades

            if (e.Row.Name == "rowLabel")
            {
                string newName = e.Value.ToString();

                _selectedNode.MaskText = EtiquetaCol.Get(newName);
                _selectedNode.Label = newName;
            }

            if (e.Row.Name == "rowIcon")
            {
                _selectedNode.Icon = e.Value.ToString();
            }

            if (e.Row.Name == "rowReference")
            {
                string valueReference = e.Value.ToString();
                string caracter = "Http";
                string caracter2 = "Https";
                if (
                    valueReference.ToLower().Contains(caracter.ToLower())
                    || valueReference.ToLower().Contains(caracter2.ToLower())
                )
                {
                    _selectedNode.IsExternalReference = true;
                }
                else
                {
                    _selectedNode.IsExternalReference = false;
                }
            }

            if (e.Row.Name == "rowId")
            {
                string idSelected = e.Value.ToString();
            }
            #endregion propiedades

            string id = _selectedNode.Id;

            if (_selectedNode.TypeContainer == eTypeContainer.Menu)
            {
                _woMenuDataSet = UpdatePropertyNodes(_selectedNode);
            }
            else
            {
                _woMenuDataSet = UpdateNodeProperty(_woMenuDataSet);
            }

            InitializeTreeList(chargeToJson: false);

            List<TreeListNode> nodesCol = _menuView.GetNodeList();
            TreeListNode node = nodesCol
                .Where(x => ((WoMenuProperties)x.Tag).Id == id)
                .FirstOrDefault();
            if (node != null)
            {
                _menuView.SetFocusedNode(node);
            }

            _menuView.Refresh();
            textSearchModels_ejec();
        }

        /// <summary>
        /// Función que borra un menu
        /// </summary>
        public void DeleteMenu()
        {
            FileMenus menu = grdMenusView.GetFocusedRow() as FileMenus;
            if (menu == null)
                return;
            try
            {
                if (File.Exists(_project.DirMenus + "\\" + menu.Menu + ".json"))
                {
                    WoDirectory.DeleteFile(_project.DirMenus + "\\" + menu.Menu + ".json");
                }

                if (File.Exists(_project.DirMenusTemp + "\\" + menu.Menu + ".json"))
                {
                    WoDirectory.DeleteFile(_project.DirMenusTemp + "\\" + menu.Menu + ".json");
                }

                //_observer.SetLog(_deleteMenu);
                _menuView.Nodes.Clear();
                ChargeGrid();
                grdMenusView.RefreshData();
            }
            catch (Exception ex)
            {
                //throw new WoObserverException(_errorDeleteMenu, ex.Message);
            }

            //LoadTree();
        }

        public string nameOfMenu = string.Empty;

        /// <summary>
        /// Función para renombrar y modificar un menu raiz
        /// </summary>
        public void RenameMenu()
        {
            nameOfMenu = string.Empty;
            WoNewMenu form = new WoNewMenu(_woMenuDataSet);
            form._isEdit = true;
            form.ProcessGridChangeStatus(false);
            form.FormClosed += (object sender, FormClosedEventArgs e) =>
            {
                ChargeGrid();
            };
            form.ShowDialog();
            nameOfMenu = form.nameOfMenu;
            SetFocusMenu(form.nameOfMenu);

            //_woMenuDesigner.ChargeGrid();
            //_woMenuDesigner.InitializeMode();

            //ViewMode();
            //_woMenuDesigner.SetFocusMenu(form.nameOfMenu);
            //_observer.SetLog(_newMenu);

            //_woMenuDesigner.InitializeMode();
            //ViewMode();
            //_woMenuDesigner.InitializeTreeList();
            ////_woMenuDesigner.grdMenusView.Focus();
            //_woMenuDesigner.SetFocusMenu(form.nameOfMenu);
            //_observer.SetLog(_newMenu);
        }

        /// <summary>
        /// Función para limpiar completamente el menu
        /// </summary>
        public void ClearMenu(bool reload = false)
        {
            if (
                XtraMessageBox.Show(
                    "Desea limpiar el menu?",
                    "Confirmación",
                    MessageBoxButtons.YesNo
                ) == DialogResult.Yes
            )
            {
                _woMenuDataSet = RemoveNode(_woMenuDataSet, cleanAll: true);
                if (reload)
                {
                    InitializeTreeList(chargeToJson: false);
                }
            }
        }

        /// <summary>
        /// Función para borrar un nodo del menu
        /// </summary>
        public DialogResult Clearnode(bool reload = false)
        {
            SupportTextSearchValue();
            if (
                _selectedNode.TypeContainer == eTypeContainer.Menu
                || _selectedNode.TypeContainer == eTypeContainer.None
            )
            {
                if (_selectedNode.TypeItem != eTypeItem.MenuItem)
                {
                    XtraMessageBox.Show(
                        "No se puede borrar el nodo principal del menú.",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            if (_selectedNode.TypeContainer is eTypeContainer.SubMenu)
            {
                if (
                    XtraMessageBox.Show(
                        "Se borraran todos los elementos contenidos dentro de este, desea eliminar?",
                        "Confirmación",
                        MessageBoxButtons.YesNo
                    ) == DialogResult.Yes
                )
                {
                    _woMenuDataSet = RemoveNode(container: _woMenuDataSet);

                    if (reload)
                    {
                        InitializeTreeList(chargeToJson: false);
                    }
                    _menuView.MoveFirst();
                    //_selectedNode = _woMenuDataSet;
                    textSearchModels_ejec();
                    return DialogResult.OK;
                }
            }
            else
            {
                _woMenuDataSet = RemoveNode(container: _woMenuDataSet);

                if (reload)
                {
                    InitializeTreeList(chargeToJson: false);
                }
                _menuView.MoveFirst();
                //_selectedNode = _woMenuDataSet;
                textSearchModels_ejec();
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }
        #endregion Eventos del menu


        #region Serializar menu
        /// <summary>
        /// Función para serializar y guardar el menu en formato Json
        /// </summary>
        public void SaveMenu()
        {
            _woMenuDataSet.Theme = _themeSelected;
            if (CheckLabels(_woMenuDataSet))
            {
                string json = JsonConvert.SerializeObject(_woMenuDataSet);
                WoDirectory.WriteFile($@"{_pathMenus}/{_woMenuDataSet.Id}.json", json);
                WoDirectory.WriteFile($@"{_pathTempMenus}/{_woMenuDataSet.Id}.json", json);
            }
            else
            {
                ///ToDo: send log
            }
        }

        #endregion Serializar menu


        #region Validaciones

        public bool CheckLabels(WoMenuProperties woContainer)
        {
            //Text, Name
            bool VerifyNodes = true;

            if (woContainer.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties subNode in woContainer.ContentCol)
                {
                    if (subNode.Label == string.Empty)
                    {
                        return false;
                    }
                    if (subNode.ContentCol.Count > 0)
                    {
                        CheckLabels(subNode);
                    }
                }
            }

            return VerifyNodes;
        }

        #endregion Validaciones


        #region Update
        /// <summary>
        /// Función para actualizar un nodo
        /// </summary>
        /// <param name="reload"></param>
        public void UpdateNode(bool reload = false)
        {
            if (_selectedNode.TypeContainer != eTypeContainer.Menu)
            {
                _woMenuDataSet = Update(_selectedNode);
            }
            else
            {
                ///Send log ToDo
            }

            if (reload)
            {
                InitializeTreeList(chargeToJson: false);
            }
        }

        /// <summary>
        /// Función para remover nodos del arbol
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public WoMenuProperties Update(WoMenuProperties container)
        {
            //GetCopy();
            WoMenuProperties menu = new WoMenuProperties()
            {
                ProcessList = container.ProcessList,
                Orientation = container.Orientation,
                Order = container.Order,
                Roles = container.Roles,
                ContentCol = new List<WoMenuProperties>(),
                FontColor = container.FontColor,
                MenuSize = container.MenuSize,
                Italic = container.Italic,
                Wide = container.Wide,
                Decoration = container.Decoration,
                Icon = container.Icon,
                DropDown = container.DropDown,
                BackgroundColor = container.BackgroundColor,
                BorderColor = container.BorderColor,
                TypeContainer = container.TypeContainer,
                TypeItem = container.TypeItem,
                Enable = container.Enable,
                Parent = container.Parent,
                Id = container.Id,
                Label = container.Label,
                MaskText = container.MaskText,
                Theme = container.Theme,
                IsLogin = container.IsLogin,
                ThemeSuperiorAplied = container.ThemeSuperiorAplied,
                CustomDesignAplied = container.CustomDesignAplied,
                ExpandedNode = container.ExpandedNode,
                Added = container.Added,
                Reference = container.Reference,
                BeginRow = container.BeginRow,
                InNewTab = container.InNewTab,
                IsExternalReference = container.IsExternalReference,
                Process = container.Process,
                EndingRow = container.EndingRow,
                BackgroundRoot = container.BackgroundRoot,
                BorderColorRoot = container.BorderColorRoot,
                NotFound = container.NotFound
            };

            if (container.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties subNode in container.ContentCol)
                {
                    if (_selectedNode.Id != subNode.Id)
                    {
                        menu.ContentCol.Add(Update(subNode));
                    }
                    else
                    {
                        menu.ContentCol.Add(subNode);
                    }
                }
            }

            return menu;
        }
        #endregion Update


        /// <summary>
        /// Función para actualizar nodos con las propiedades que se esten
        /// modificando
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public WoMenuProperties UpdateNodeProperty(WoMenuProperties container)
        {
            //GetCopy();
            WoMenuProperties menu = new WoMenuProperties()
            {
                ProcessList = container.ProcessList,
                Orientation = container.Orientation,
                Order = container.Order,
                Roles = container.Roles,
                ContentCol = new List<WoMenuProperties>(),
                FontColor = container.FontColor,
                MenuSize = container.MenuSize,
                Italic = container.Italic,
                Wide = container.Wide,
                Decoration = container.Decoration,
                Icon = container.Icon,
                DropDown = container.DropDown,
                BackgroundColor = container.BackgroundColor,
                BorderColor = container.BorderColor,
                TypeContainer = container.TypeContainer,
                TypeItem = container.TypeItem,
                Enable = container.Enable,
                Parent = container.Parent,
                Id = container.Id,
                IsLogin = container.IsLogin,
                Label = container.Label,
                MaskText = container.MaskText,
                Theme = container.Theme,
                ThemeSuperiorAplied = container.ThemeSuperiorAplied,
                CustomDesignAplied = container.CustomDesignAplied,
                ExpandedNode = container.ExpandedNode,
                Added = container.Added,
                Reference = container.Reference,
                BeginRow = container.BeginRow,
                InNewTab = container.InNewTab,
                IsExternalReference = container.IsExternalReference,
                Process = container.Process,
                EndingRow = container.EndingRow,
                BackgroundRoot = container.BackgroundRoot,
                BorderColorRoot = container.BorderColor,
                NotFound = container.NotFound
            };

            if (container.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties subNode in container.ContentCol)
                {
                    if (_selectedNode.Id == subNode.Id)
                    {
                        menu.ContentCol.Add(_selectedNode);
                    }
                    else
                    {
                        menu.ContentCol.Add(UpdateNodeProperty(subNode));
                    }
                }
            }

            return menu;
        }

        #region Delete
        /// <summary>
        /// Función para remover nodos del arbol
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public WoMenuProperties RemoveNode(WoMenuProperties container, bool cleanAll = false)
        {
            //GetCopy();
            WoMenuProperties menu = new WoMenuProperties()
            {
                ProcessList = container.ProcessList,
                Orientation = container.Orientation,
                Order = container.Order,
                Roles = container.Roles,
                ContentCol = new List<WoMenuProperties>(),
                FontColor = container.FontColor,
                MenuSize = container.MenuSize,
                Italic = container.Italic,
                Wide = container.Wide,
                Decoration = container.Decoration,
                Icon = container.Icon,
                DropDown = container.DropDown,
                BackgroundColor = container.BackgroundColor,
                BorderColor = container.BorderColor,
                TypeContainer = container.TypeContainer,
                TypeItem = container.TypeItem,
                Enable = container.Enable,
                Parent = container.Parent,
                Id = container.Id,
                Label = container.Label,
                MaskText = container.MaskText,
                Theme = container.Theme,
                ThemeSuperiorAplied = container.ThemeSuperiorAplied,
                CustomDesignAplied = container.CustomDesignAplied,
                ExpandedNode = container.ExpandedNode,
                Added = container.Added,
                Reference = container.Reference,
                BeginRow = container.BeginRow,
                InNewTab = container.InNewTab,
                IsExternalReference = container.IsExternalReference,
                IsLogin = container.IsLogin,
                Process = container.Process,
                EndingRow = container.EndingRow,
                BackgroundRoot = container.BackgroundRoot,
                BorderColorRoot = container.BorderColorRoot,
                NotFound = container.NotFound
            };

            if (cleanAll)
            {
                return menu;
            }
            else
            {
                if (container.ContentCol.Count > 0)
                {
                    foreach (WoMenuProperties subNode in container.ContentCol)
                    {
                        if (_selectedNode.Id != subNode.Id)
                        {
                            menu.ContentCol.Add(RemoveNode(subNode));
                        }
                    }
                }
            }

            return menu;
        }
        #endregion Delete

        #region Add Element

        private int GetNextId(string name)
        {
            int i = 0;
            bool result = false;

            do
            {
                result = CheckName(_woMenuDataSet, (i == 0) ? $@"{name}" : $@"{name}{i}");
                i = (result) ? i : i + 1;
            } while (!result);

            return i;
        }

        public bool CheckName(WoMenuProperties woContainer, string name)
        {
            bool VerifyNodes = true;

            if (woContainer.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties container in woContainer.ContentCol)
                {
                    if (container.Id == name)
                    {
                        VerifyNodes = false;
                        break;
                    }
                    else
                    {
                        VerifyNodes = CheckName(container, name);
                        if (!VerifyNodes)
                            break;
                    }
                }
            }

            return VerifyNodes;
        }

        #endregion Add Element


        #region Drag and Drop events

        /// <summary>
        /// indica si el re posicionamiento del nodo fue correcto
        /// </summary>
        private bool _correctMoveNode = false;

        /// <summary>
        /// Variable auxiliar para almacenar el valor del nodo
        /// que se esta agregando y poder hacerle focus
        /// </summary>
        WoMenuProperties nodeFocused { get; set; } = new WoMenuProperties();

        /// <summary>
        /// Función que orquesta los eventos de drag and drop de todos los componentes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TreeDragDropEvents_DragDrop(object sender, dynamic e)
        {
            SupportTextSearchValue();
            if (e == null || e.Data == null)
            {
                return;
            }
            if (
                e.InsertType == InsertType.After
                || e.InsertType == InsertType.Before
                || e.InsertType == InsertType.AsChild
            )
            {
                try
                {
                    TreeListNode destNode = GetDestNode(e.Location);

                    if (destNode == null)
                    {
                        e.Action = DragDropActions.None;
                        return;
                    }

                    WoMenuProperties destNodeProperties = (WoMenuProperties)destNode.Tag;
                    WoMenuProperties lastVersionSave = _woMenuDataSet.GetCopy();
                    WoMenuProperties lastVersionNodeDrag = _selectedNode;

                    try
                    {
                        if (_selectedNode.Id != _woMenuDataSet.Id)
                        {
                            if (_selectedNode.Id != destNodeProperties.Id)
                            {
                                if (
                                    !_selectedNode.Added
                                    && _selectedNode.TypeContainer != eTypeContainer.None
                                )
                                {
                                    int next = GetNextId(_selectedNode.Id);
                                    if (next > 0)
                                    {
                                        _selectedNode.Id = _selectedNode.Id + next;
                                    }
                                }

                                if (
                                    destNodeProperties.TypeItem == eTypeItem.MenuItem
                                    && e.InsertType is InsertType.AsChild
                                )
                                {
                                    e.Action = DragDropActions.None;
                                    return;
                                }
                                if (
                                    destNodeProperties.Id == lastVersionSave.Id
                                        && e.InsertType is InsertType.After
                                    || destNodeProperties.Id == lastVersionSave.Id
                                        && e.InsertType is InsertType.Before
                                )
                                {
                                    e.Action = DragDropActions.None;
                                    return;
                                }

                                _woMenuDataSet = RemoveNode(_woMenuDataSet);
                                nodeFocused = _selectedNode;
                                WoMenuProperties newMenu = MoveNode(
                                    container: _woMenuDataSet,
                                    selectedItem: _selectedNode,
                                    dest: destNodeProperties,
                                    type: e.InsertType
                                );
                                _woMenuDataSet = newMenu;
                            }
                        }
                        else
                        {
                            return;
                        }

                        if (!_correctMoveNode)
                        {
                            _woMenuDataSet = lastVersionSave;
                        }
                        else
                        {
                            _correctMoveNode = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _woMenuDataSet = lastVersionSave;
                        XtraMessageBox.Show(
                            ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                    if (
                        e.Source is TreeList
                        || e.Source.Name == "SubMenusBox"
                        || e.Source.Name == "menuCustom"
                    )
                    {
                        InitializeTreeList(chargeToJson: false, chargeProcess: false);
                    }
                    else
                    {
                        InitializeTreeList(chargeToJson: false);
                    }
                    //Se asigna de nuevo por el tema de los focus entre tree lists y demas focus que alteran su valor
                    //asi se mantiene persitencia del objeto recien agregado cuando viene de los listbox

                    textSearchModels_ejec();
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(_errorDeletePage, ex.Message);
                }
            }

            //_menuView.Focus();
        }

        private WoMenuProperties MoveNode(
            WoMenuProperties container,
            WoMenuProperties selectedItem,
            WoMenuProperties dest,
            InsertType type
        )
        {
            WoMenuProperties newMenuContainer = new WoMenuProperties()
            {
                ProcessList = container.ProcessList,
                Orientation = container.Orientation,
                Order = container.Order,
                Roles = container.Roles,
                ContentCol = new List<WoMenuProperties>(),
                FontColor = container.FontColor,
                MenuSize = container.MenuSize,
                Italic = container.Italic,
                Wide = container.Wide,
                Decoration = container.Decoration,
                Icon = container.Icon,
                DropDown = container.DropDown,
                BackgroundColor = container.BackgroundColor,
                BorderColor = container.BorderColor,
                TypeContainer = container.TypeContainer,
                TypeItem = container.TypeItem,
                Enable = container.Enable,
                Parent = container.Parent,
                Id = container.Id,
                Label = container.Label,
                MaskText = container.MaskText,
                Theme = container.Theme,
                Reference = container.Reference,
                ThemeSuperiorAplied = container.ThemeSuperiorAplied,
                CustomDesignAplied = container.CustomDesignAplied,
                ExpandedNode = container.ExpandedNode,
                Added = container.Added,
                BeginRow = container.BeginRow,
                InNewTab = container.InNewTab,
                IsExternalReference = container.IsExternalReference,
                IsLogin = container.IsLogin,
                Process = container.Process,
                EndingRow = container.EndingRow,
                BackgroundRoot = container.BackgroundRoot,
                BorderColorRoot = container.BorderColorRoot,
                NotFound = container.NotFound
            };
            if (newMenuContainer.Id == dest.Id && dest.Id != selectedItem.Id)
            {
                if (type is InsertType.AsChild)
                {
                    _correctMoveNode = true;
                    selectedItem.Added = true;
                    selectedItem.Parent = newMenuContainer.Id;

                    newMenuContainer.ContentCol.Add(selectedItem);
                }
            }

            if (dest.Id == selectedItem.Id)
                _correctMoveNode = false;
            if (container.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties subNode in container.ContentCol)
                {
                    if (subNode.Id == dest.Id)
                    {
                        if (
                            subNode.Id == selectedItem.Id
                            && selectedItem.TypeContainer != eTypeContainer.None
                        )
                        {
                            selectedItem.Parent = subNode.Id;
                            subNode.ContentCol.AddRange(selectedItem.ContentCol);
                            newMenuContainer.ContentCol.Add(subNode);
                        }
                        else
                        {
                            if (type is InsertType.After)
                            {
                                _correctMoveNode = true;
                                selectedItem.Added = true;
                                selectedItem.Parent = newMenuContainer.Id;
                                newMenuContainer.ContentCol.Add(subNode);

                                newMenuContainer.ContentCol.Add(selectedItem);
                            }
                            else if (type is InsertType.Before)
                            {
                                _correctMoveNode = true;
                                selectedItem.Added = true;
                                selectedItem.Parent = newMenuContainer.Id;

                                newMenuContainer.ContentCol.Add(selectedItem);

                                newMenuContainer.ContentCol.Add(subNode);
                            }
                            else if (type is InsertType.AsChild)
                            {
                                if (
                                    subNode.TypeContainer != eTypeContainer.None
                                    && subNode.TypeItem != eTypeItem.MenuItem
                                )
                                {
                                    _correctMoveNode = true;
                                    selectedItem.Added = true;
                                    selectedItem.Parent = subNode.Id;

                                    subNode.ContentCol.Add(selectedItem);
                                }

                                newMenuContainer.ContentCol.Add(subNode);
                            }
                            else
                            {
                                newMenuContainer.ContentCol.Add(subNode);
                            }
                        }
                    }
                    else
                    {
                        if (subNode != selectedItem)
                        {
                            newMenuContainer.ContentCol.Add(
                                MoveNode(
                                    container: subNode,
                                    selectedItem: selectedItem,
                                    dest: dest,
                                    type: type
                                )
                            );
                        }
                    }
                }
            }

            return newMenuContainer;
        }

        #endregion

        #region Recuperar nodo de destino

        /// <summary>
        /// Función para obtener el nodo destino al cual se va a integrar el o los nuevos nodos que se están arrastrando
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <returns></returns>
        TreeListNode GetDestNode(Point hitPoint)
        {
            try
            {
                Point pt = _menuView.PointToClient(hitPoint);
                TreeListHitInfo ht = _menuView.CalcHitInfo(pt);
                TreeListNode destNode = ht.Node;
                if (destNode is TreeListAutoFilterNode)
                    return null;

                return destNode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion Recuperar nodo de destino

        #region Funciones auxiliares
        /// <summary>
        /// Variable para almacenar las propiedades del nodo selecionado
        /// anteriores
        /// </summary>
        private WoMenuProperties _selectedNodeOldVersion { get; set; } = new WoMenuProperties();

        /// <summary>
        /// Función auxiliar que crea una copia del nodo seleccionado
        /// con finalidad de almacenar los valores anteriores a su edición
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propMenus_FocusedRowChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e
        )
        {
            _selectedNodeOldVersion = _selectedNode.CreateCopy();

            #region Ayuda Propiedades

            if (e.Row != null)
            {
                if (e.Row.Name == "rowEnable")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite habilitar o desabilitar el menú o página.";
                }
                if (e.Row.Name == "rowDropDown")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el despliegue de los menus,
sobre pasando el cursor o dando clic.";
                }
                if (e.Row.Name == "rowIsLogin")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar la visibilidad, en base si
esta logeado o no.";
                }
                if (e.Row.Name == "rowFontColor")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el color de fuente.";
                }
                if (e.Row.Name == "rowItalic")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el estilo de letra a cursiva.";
                }
                if (e.Row.Name == "rowDecoration")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el decorado de letra a tachado o
subrayado.";
                }
                if (e.Row.Name == "rowIcon")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite agregar un icono al menú.";
                }
                if (e.Row.Name == "rowBackgroundColor")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el color de fondo de un 
menú.";
                }
                if (e.Row.Name == "rowBorderColor")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el color de borde de un 
menú.";
                }
                if (e.Row.Name == "rowBeginRow")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite agregar un separador al inicio del 
grupo del menú.";
                }
                if (e.Row.Name == "rowEndingRow")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite agregar un separador al final del 
grupo del menú.";
                }
                if (e.Row.Name == "rowInNewTab")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite abrir en un una nueva pestaña la 
página del menú.";
                }
                if (e.Row.Name == "rowInNewTab")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite abrir en un una nueva pestaña la 
página del menú.";
                }
                if (e.Row.Name == "rowBackgroundRoot")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el color de fondo de la 
base del menú.";
                }
                if (e.Row.Name == "rowBorderColorRoot")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el color de borde de la 
base del menú.";
                }
                if (e.Row.Name == "rowMenuSize")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el tamaño del menú.";
                }
                if (e.Row.Name == "rowLabel")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Etiqueta del menú.";
                }
                if (e.Row.Name == "rowCustomDesignAplied")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Nos indica que al menú se le fue aplicado 
una configuración de diseño, los cambios 
generales no le afectaran hasta que se 
deshabilite esta propiedad.";
                }
                if (e.Row.Name == "rowReference")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite agregar una URL externa, debe
tener un formato valido,ya sea Http 
o Https.";
                }
                if (e.Row.Name == "rowIsExternalReference")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Nos indica si se trata de un referencia
externa o interna.";
                }
                if (e.Row.Name == "rowWide")
                {
                    lblHelp.Text =
                        @$"Ayuda:
Permite configurar el tipo de letra
en negritas.";
                }
            }

            #endregion Ayuda Propiedades
        }

        /// <summary>
        /// Función auxiliar para presetear el valor de la etiqueta de rol
        /// </summary>
        public void SetLabels()
        {
            if (
                _woMenuDataSet.Roles.Count > 0 /*&& _woMenuDataSet.Roles != "Sin Rol"*/
            )
            {
                string listRoles = string.Empty;
                string listRolesMore = string.Empty;

                foreach (Rol rol in _woMenuDataSet.Roles)
                {
                    if (listRoles.Length <= 80)
                    {
                        listRoles = listRoles + rol.Id + ",";
                    }
                    else
                    {
                        listRolesMore = listRolesMore + rol.Id + ",";
                    }

                    lblRol.Text = "Rol/Roles: " + listRoles.TrimEnd(',');
                    lblRol2.Text = listRolesMore.TrimEnd(',');
                }
            }
            else
            {
                lblRol.Text = "Rol: Sin rol";
            }
        }

        /// <summary>
        /// Función auxiliar para actualizar la variable global _woMenuDataSet
        /// con los cambios de las propiedades del nodo seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propMenus_RowChanged(
            dynamic sender,
            DevExpress.XtraVerticalGrid.Events.RowChangedEventArgs e
        )
        {
            _selectedNode = (WoMenuProperties)sender.SelectedObject;
            //_woMenuDataSet = UpdateNodeProperty(_woMenuDataSet);
        }

        #endregion Funciones auxiliares

        #region Actualizacion en cascada

        /// <summary>
        /// Función para actualizar las propiedades generales de todos los subnodos
        /// tomando de base el nodo root
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        private WoMenuProperties UpdatePropertyNodes(WoMenuProperties container)
        {
            WoMenuProperties menu = new WoMenuProperties();

            if (container.CustomDesignAplied)
            {
                menu = new WoMenuProperties()
                {
                    ProcessList = container.ProcessList,
                    Orientation = container.Orientation,
                    Order = container.Order,
                    Roles = container.Roles,
                    ContentCol = new List<WoMenuProperties>(),
                    FontColor = container.FontColor,
                    MenuSize = container.MenuSize,
                    Italic = container.Italic,
                    Wide = container.Wide,
                    Decoration = container.Decoration,
                    Icon = container.Icon,
                    DropDown = container.DropDown,
                    BackgroundColor = container.BackgroundColor,
                    BorderColor = container.BorderColor,
                    TypeContainer = container.TypeContainer,
                    TypeItem = container.TypeItem,
                    Enable = container.Enable,
                    Parent = container.Parent,
                    Id = container.Id,
                    IsLogin = container.IsLogin,
                    Label = container.Label,
                    MaskText = container.MaskText,
                    Theme = container.Theme,
                    ThemeSuperiorAplied = container.ThemeSuperiorAplied,
                    CustomDesignAplied = container.CustomDesignAplied,
                    ExpandedNode = container.ExpandedNode,
                    Added = container.Added,
                    Reference = container.Reference,
                    BeginRow = container.BeginRow,
                    InNewTab = container.InNewTab,
                    IsExternalReference = container.IsExternalReference,
                    Process = container.Process,
                    EndingRow = container.EndingRow,
                    BackgroundRoot = container.BackgroundRoot,
                    BorderColorRoot = container.BorderColorRoot,
                    NotFound = container.NotFound
                };
            }
            else
            {
                menu = new WoMenuProperties()
                {
                    ProcessList = container.ProcessList,
                    Orientation = container.Orientation,
                    Order = container.Order,
                    Roles = container.Roles,
                    ContentCol = new List<WoMenuProperties>(),
                    FontColor = _selectedNode.FontColor,
                    MenuSize = _selectedNode.MenuSize,
                    Italic = _selectedNode.Italic,
                    Wide = _selectedNode.Wide,
                    Decoration = _selectedNode.Decoration,
                    Icon = _selectedNode.Icon,
                    DropDown = _selectedNode.DropDown,
                    BackgroundColor = _selectedNode.BackgroundColor,
                    BorderColor = _selectedNode.BorderColor,
                    TypeContainer = container.TypeContainer,
                    TypeItem = container.TypeItem,
                    Enable = _selectedNode.Enable,
                    Parent = container.Parent,
                    Id = container.Id,
                    IsLogin = _selectedNode.IsLogin,
                    Label = container.Label,
                    MaskText = container.MaskText,
                    Theme = container.Theme,
                    ThemeSuperiorAplied = container.ThemeSuperiorAplied,
                    CustomDesignAplied = container.CustomDesignAplied,
                    ExpandedNode = container.ExpandedNode,
                    Added = container.Added,
                    Reference = container.Reference,
                    BeginRow = _selectedNode.BeginRow,
                    InNewTab = _selectedNode.InNewTab,
                    IsExternalReference = container.IsExternalReference,
                    Process = container.Process,
                    EndingRow = _selectedNode.EndingRow,
                    BackgroundRoot = container.BackgroundRoot,
                    BorderColorRoot = container.BorderColorRoot,
                    NotFound = container.NotFound
                };
            }

            if (container.ContentCol.Count > 0)
            {
                foreach (WoMenuProperties subNode in container.ContentCol)
                {
                    menu.ContentCol.Add(UpdatePropertyNodes(subNode));
                }
            }
            return menu;
        }

        #endregion Actualizacion en cascada

        #region Ejecutar Menú

        #region Variables principales del proyecto generado

        /// <summary>
        /// Nombre del proyecto de blazor server de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _serverProjectName = "ServerUnitMenu_proj";

        /// <summary>
        /// Nombre base de las clases que contendran las pruebas unitarias en claso de ser para server.
        /// </summary>
        private string _serverClassModelName = "ServerUnitMenu";

        /// <summary>
        /// Nombre del proyecto de blazor wasm de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _wasmProjectName = "WasmUnitMenu_proj";

        /// <summary>
        /// Nombre base de las clases que contendran las pruebas unitarias en claso de ser para wasm.
        /// </summary>
        private string _wasmClassModelName = "WasmUnitMenu";

        /// <summary>
        /// Define el nombre de la clase de la que se creara el formulario.
        /// </summary>
        private string _modelName = "";

        #endregion Variables principales del proyecto generado


        /// <summary>
        /// Genera el proyecto de server y el layout en el que se encuentra el usuario, solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void GenerateServer()
        {
            WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
            string pathProyectUnit = $@"{_project.DirProyectTemp}/{_serverProjectName}";
            if (!File.Exists($@"{pathProyectUnit}/{_serverProjectName}.csproj"))
            {
                woBlazorGenerator.GenerateBaseServer(_serverProjectName);

                woBlazorGenerator.GenerateMenuTest(_serverProjectName, _woMenuDataSet);
            }
            else
            {
                woBlazorGenerator.ReGenerateMenuTest(_serverProjectName, _woMenuDataSet);
            }
        }

        /// <summary>
        /// Genera el proyecto de wasm y el layout en el que se encuentra el usuario, solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void GenerateWasm()
        {
            WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
            string pathProyectUnit = $@"{_project.DirProyectTemp}/{_wasmProjectName}";
            if (!File.Exists($@"{pathProyectUnit}/{_wasmProjectName}.csproj"))
            {
                woBlazorGenerator.GenerateBaseWasm(_wasmProjectName);

                woBlazorGenerator.GenerateMenuTest(_wasmProjectName, _woMenuDataSet);
            }
            else
            {
                woBlazorGenerator.ReGenerateMenuTest(_wasmProjectName, _woMenuDataSet);
            }
        }

        #endregion Ejecutar Menú

        #region WoMessages
        private WoLog _errorDeletePage = new WoLog()
        {
            CodeLog = "000",
            Title = "Eliminar pagina.",
            Details = "Hubo errores al intentar eliminar la pagina del menú.",
            UserMessage = "Errores al borrar la pagina.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnSaveMenu_ItemClick",
                LineAprox = "838"
            }
        };
        #endregion WoMessages

        private void xtraTabControl1_SelectedPageChanged(
            object sender,
            DevExpress.XtraTab.TabPageChangedEventArgs e
        )
        {
            txtSearchModels.Text = "";
            string tabPageName = e.Page.Name;
            if (tabPageName == "tabAllProcess")
            {
                cmbAllProcess_TextChanged(null, null);
                cmbAllProcess_SelectedValueChanged(null, null);
            }
            else
            {
                lbcSelectedProcess.Items.Clear();
                cmbSelectedProcess_TextChanged(null, null);
            }
        }

        /// <summary>
        /// Función axiliar para ejecutar el cambio en el propertygrid
        /// si se cambia mas de una vez la misma propiedad ya no se ejecuta
        /// es por eso que se creo esta función para forzar la ejecución
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propMenus_CellValueChanging(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            if (e.Row.Name == "rowIcon")
            {
                propMenus_CellValueChanged(sender, e);
            }
        }

        string ValueBeforeChange = "";

        #region Funciones auxiliares para mantener la persistencia de la busqueda
        /// <summary>
        /// Funcion auxiliar para respaldar el valor actual de la busqueda en el
        ///  txtSearchModels
        /// </summary>
        public void SupportTextSearchValue(bool setNull = false)
        {
            if (!setNull)
            {
                ValueBeforeChange = txtSearchModels.Text;
                cmbAllProcess.SelectedIndex = cmbAllProcess.SelectedIndex;
                cmbSelectedProcess.SelectedIndex = cmbSelectedProcess.SelectedIndex;
            }
            else
            {
                txtSearchModels.Text = string.Empty;
            }
        }

        /// <summary>
        /// Funcion auxiliar para mantener la percistencia de la busqueda despues de
        /// modificar el arbol de nodos
        /// </summary>
        public void textSearchModels_ejec()
        {
            if (txtSearchModels.Text != string.Empty)
            {
                txtSearchModels.Text = ValueBeforeChange;
                textEdit1_EditValueChanged(null, null);
            }
            else
            {
                cmbAllProcess_SelectedValueChanged(null, null);
                cmbSelectedProcess_SelectedValueChanged(null, null);
            }
        }
        #endregion Funciones auxiliares para mantener la persistencia de la busqueda
        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (!isComboChanged)
            {
                cmbSelectedProcess.SelectedIndex = 0;
                cmbAllProcess.SelectedIndex = 0;
            }

            lbcAllProcess.Items.Clear();
            lbcSelectedProcess.Items.Clear();
            if (txtSearchModels.Text == "" || txtSearchModels.Text == "Todos")
            {
                #region Valores default Todos los procesos
                foreach (List<Modelo> process in _allProcessModelsCol)
                {
                    foreach (Modelo model in process)
                    {
                        lbcAllProcess.Items.Add(model);
                    }
                }
                #endregion Valores default Todos los procesos

                #region Valores default Procesos seleccionados

                foreach (List<Modelo> process in _selectedProcessModelsCol)
                {
                    foreach (Modelo model in process)
                    {
                        lbcSelectedProcess.Items.Add(model);
                    }
                }
                #endregion Valores default Procesos seleccionados
            }
            else
            {
                try
                {
                    #region Busqueda en todos los procesos
                    foreach (var modelList in _allProcessModelsCol)
                    {
                        //List<Modelo> modelsCol = _allProcessModelsCol
                        //    .Where(
                        //        x =>
                        //            x[i].Id.Contains(
                        //                txtSearchModels.Text,
                        //                StringComparison.OrdinalIgnoreCase
                        //            ) || x[i].EtiquetaId.Contains(txtSearchModels.Text)
                        //    )
                        //    .ToList()
                        //    .FirstNonDefault();

                        List<Modelo> modelsCol = modelList
                            .Where(x =>
                                x.Id.Contains(
                                    txtSearchModels.Text,
                                    StringComparison.OrdinalIgnoreCase
                                )
                                || x.EtiquetaId.Contains(
                                    txtSearchModels.Text,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            .ToList();
                        foreach (var model in modelsCol)
                        {
                            if (model != null)
                            {
                                lbcAllProcess.Items.Add(model);
                            }
                        }
                    }

                    #endregion Busqueda en todos los procesos

                    #region Busqueda en proceso seleccionado
                    foreach (var modelList in _selectedProcessModelsCol)
                    {
                        List<Modelo> modelsCol = modelList
                            .Where(x =>
                                x.Id.Contains(
                                    txtSearchModels.Text,
                                    StringComparison.OrdinalIgnoreCase
                                )
                                || x.EtiquetaId.Contains(
                                    txtSearchModels.Text,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            .ToList();
                        foreach (var model in modelsCol)
                        {
                            if (model != null)
                            {
                                lbcSelectedProcess.Items.Add(model);
                            }
                        }
                    }
                    #endregion Busqueda en proceso seleccionado
                }
                catch (Exception EX)
                {
                    var A = EX;
                    throw;
                }
            }
            isComboChanged = false;
        }

        /// <summary>
        /// Función para regenerar los valores de las etiquetas
        /// del nodo seleccionado tomandolas de los modelos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegenerate_Click(object sender, EventArgs e)
        {
            Modelo modelo = new Modelo();
            if (_selectedNode.Id.Contains("GridList", StringComparison.OrdinalIgnoreCase))
            {
                modelo = _project
                    .ModeloCol.Modelos.Where(e => e.Id == _selectedNode.Id.Replace("GridList", ""))
                    .FirstOrDefault();
            }
            if (_selectedNode.Id.Contains("ReportOdata", StringComparison.OrdinalIgnoreCase))
            {
                modelo = _project
                    .ModeloCol.Modelos.Where(e =>
                        e.Id
                        == _selectedNode.Id.Replace(
                            "ReportOdata",
                            "",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .FirstOrDefault();
            }
            if (
                !_selectedNode.Id.Contains("ReportOdata", StringComparison.OrdinalIgnoreCase)
                && !_selectedNode.Id.Contains("GridList", StringComparison.OrdinalIgnoreCase)
            )
            {
                modelo = _project
                    .ModeloCol.Modelos.Where(e => e.Id == _selectedNode.Id)
                    .FirstOrDefault();
            }
            if (modelo != null)
            {
                _selectedNode.MaskText = modelo.EtiquetaId;
                _selectedNode.Label = modelo.EtiquetaId;

                string id = _selectedNode.Id;

                if (_selectedNode.TypeContainer == eTypeContainer.Menu)
                {
                    _woMenuDataSet = UpdatePropertyNodes(_selectedNode);
                }
                else
                {
                    _woMenuDataSet = UpdateNodeProperty(_woMenuDataSet);
                }

                InitializeTreeList(chargeToJson: false);

                List<TreeListNode> nodesCol = _menuView.GetNodeList();
                TreeListNode node = nodesCol
                    .Where(x => ((WoMenuProperties)x.Tag).Id == id)
                    .FirstOrDefault();
                if (node != null)
                {
                    _menuView.SetFocusedNode(node);
                }

                _menuView.Refresh();
            }
        }
    }
}
