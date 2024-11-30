using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Newtonsoft.Json;
//using System.Windows.Forms;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorGenerator.BlazorDialogs;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.SB.ManagerDirectory;
using WooW.SB.ManagerDirectory.OptionsDirectory;
using WooW.SB.Menus;
using WooW.SB.Menus.MenusComponents;
using WooW.SB.Themes;

namespace WooW.SB.Forms
{
    public partial class fmMenus : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        /// <summary>
        /// Lista para llenar la lista de menus contenidos en la carpeta de Menus
        /// </summary>
        public List<string> menuListGrid { get; set; }

        /// <summary>
        /// Variable para acceder a las
        /// </summary>
        public Proyecto proyecto { get; set; }

        /// <summary>
        /// Variable para suscribir funciones a los eventos del diseñador de menus
        /// </summary>
        private WoMenuDesigner _menu { get; set; }

        #region Instancias singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        private static WoDeleteDirectory _woDeleteDirectory = new WoDeleteDirectory();
        private RepositoryItemImageComboBox txtComboImage;
        private WoMenuDesigner _woMenuDesigner = new WoMenuDesigner();

        public fmMenus()
        {
            InitializeComponent();

            try
            {
                repositoryItemComboBox4.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;
                proyecto = Proyecto.getInstance();

                if (!Directory.Exists($@"{proyecto.DirMenusTemp}"))
                {
                    Directory.CreateDirectory($@"{proyecto.DirMenusTemp}");
                }
                if (!Directory.Exists($@"{proyecto.DirReportesTemp}"))
                {
                    Directory.CreateDirectory($@"{proyecto.DirReportesTemp}");
                }

                // Obtenemos la lista de archivos en el directorio de origen
                string[] menusFiles = Directory.GetFiles(proyecto.DirMenus);

                foreach (string archivo in menusFiles)
                {
                    string nombreArchivo = Path.GetFileName(archivo);
                    string destinoArchivo = Path.Combine(proyecto.DirMenusTemp, nombreArchivo);

                    // Verificamos si el archivo ya existe en el directorio de destino
                    if (!File.Exists(destinoArchivo))
                    {
                        // Copiamos el archivo al directorio de destino
                        File.Copy(archivo, destinoArchivo);
                    }
                }

                WoThemeSelector theme = new WoThemeSelector();
                theme.Dock = DockStyle.Fill;
                _menu = new WoMenuDesigner();
                if (_menu.grdMenusView.RowCount == 0)
                {
                    DisableBuEdit(false, false);
                }
                //Controladores de eventos
                //_woMenuDesigner.EventsSaveMenu += SaveMenu;
                _woMenuDesigner.EventsStopBlazor += StopBlazor;
            }
            catch (Exception ex)
            {
                throw new WoObserverException("Error: ", ex.Message);
            }
        }

        /// <summary>
        /// Función de carga del formulario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fmMenus_Load(object sender, EventArgs e)
        {
            List<string> listsub = new List<string>();
            List<string> listItem = new List<string>();
            listsub.Add("Nuevo submenu");
            listItem.Add("Página personalizada");
            LoadDataGrid();
            pnlMenusBase.Controls.Add(_woMenuDesigner);
            _woMenuDesigner.Dock = DockStyle.Fill;
            _woMenuDesigner.InitializeMode();
            ViewMode();
            //_observer.SetLog(_loadPanel);
        }

        public RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return false; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        /// <summary>
        /// Integración de la interfaz IForm
        /// </summary>
        public void Cargar() { }

        List<string> woListItem = new List<string>();

        /// <summary>
        /// Carga de información en el List Box de paginas
        // TODO: revisar la persistencia
        // Todo: Validar que la carpeta exista
        /// </summary>
        private void LoadDataGrid()
        {
            menuListGrid = new List<string>();
            if (Directory.Exists(proyecto.DirMenus))
            {
                List<string> menusNamesCol = WoDirectory.ReadDirectoryFiles(
                    proyecto.DirMenus,
                    onlyNames: true
                );
                foreach (string menuName in menusNamesCol)
                {
                    if (menuName != string.Empty)
                    {
                        menuListGrid.Add(menuName);
                    }
                }
                //grdModelos.DataSource = menuListGrid;
                //_observer.SetLog(_reloadMenus);
            }
        }

        /// <summary>
        /// Evento para eliminar un elemento del árbol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="WoObserverException"></exception>
        private void barButtonItem2_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            DialogResult result = _woMenuDesigner.Clearnode(true);
        }

        public void Refrescar()
        {
            throw new NotImplementedException();
        }

        public void DisableBuEdit(object sender, bool enabled)
        {
            btnEditMenu.Enabled = enabled;
        }

        /// <summary>
        /// Evento limpiar menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _woMenuDesigner.SupportTextSearchValue();
            _woMenuDesigner.ClearMenu(true);

            _woMenuDesigner.InitializeAllProcessList();

            _woMenuDesigner.InitializeSelectedProcessList();
            _woMenuDesigner.textSearchModels_ejec();
        }

        private void barButtonItem3_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            ClearListToTree();
        }

        private void ClearListToTree()
        {
            //_menu.processSelect.Nodes.Add(new TreeListNode());
            _observer.SetLog(_reloadPagesMenu);
        }

        private void btnDeletePage_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        ) { }

        private void btnNewMenu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            WoNewMenu form = new WoNewMenu();
            form._isEdit = false;
            form.ProcessGridChangeStatus(true);
            form.FormClosed += (object sender, FormClosedEventArgs e) =>
            {
                _woMenuDesigner.ChargeGrid();
            };
            form.ShowDialog();
            //_woMenuDesigner.ChargeGrid();
            //_woMenuDesigner.InitializeMode();

            //ViewMode();
            //_woMenuDesigner.SetFocusMenu(form.nameOfMenu);
            //_observer.SetLog(_newMenu);

            _woMenuDesigner.InitializeMode();
            ViewMode();
            _woMenuDesigner.InitializeTreeList();
            //_woMenuDesigner.grdMenusView.Focus();
            _woMenuDesigner.SetFocusMenu(form.nameOfMenu);
            _observer.SetLog(_newMenu);
        }

        private void btnEditMenu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _woMenuDesigner.EditMode();
            _woMenuDesigner.SetLabels();
            _woMenuDesigner.cmbAllProcess_TextChanged(null, null);
            _woMenuDesigner.cmbAllProcess_SelectedValueChanged(null, null);
            EditMode();
            //Se actualiza el arbol
            string json = JsonConvert.SerializeObject(_menu._woMenuDataSet);
            WoDirectory.WriteFile($@"{proyecto.DirMenus}/{_menu._woMenuDataSet.Id}.json", json);
            WoDirectory.WriteFile($@"{proyecto.DirMenusTemp}/{_menu._woMenuDataSet.Id}.json", json);
            //_woMenuDesigner.LoadDataListPages();
        }

        private void EditMode()
        {
            btnImportJson.Enabled = true;
            btnDeleteSubMenu.Enabled = true;
            btnClear.Enabled = true;
            btnRefreshMenu.Enabled = false;
            btnDeleteMenu.Enabled = false;
            btnExcecute.Enabled = false;
            //btnGenerate.Enabled = false;
            btnDeletePage.Enabled = true;
            btnSaveMenu.Enabled = true;
            btnNewMenu.Enabled = false;
            btnRefresh.Enabled = true;
            btnCancel.Enabled = true;
            btnEditMenu.Enabled = false;
            btnRename.Enabled = false;
            _observer.SetLog(_editMenuStart);
        }

        private void ViewMode()
        {
            btnImportJson.Enabled = false;
            btnDeleteSubMenu.Enabled = false;
            btnClear.Enabled = false;
            btnDeletePage.Enabled = false;
            btnSaveMenu.Enabled = false;
            btnCancel.Enabled = false;
            btnNewMenu.Enabled = true;
            btnRefresh.Enabled = false;
            btnRename.Enabled = true;
            if (
                _woMenuDesigner.grdMenus.DataSource != null
                && ((ICollection)_woMenuDesigner.grdMenus.DataSource).Count > 0
            )
            {
                btnEditMenu.Enabled = true;
                btnRename.Enabled = true;
                btnDeleteMenu.Enabled = true;
                btnExcecute.Enabled = true;
                //btnGenerate.Enabled = true;
                btnRefreshMenu.Enabled = true;
            }
            else
            {
                btnEditMenu.Enabled = false;
                btnRename.Enabled = false;
                btnDeleteMenu.Enabled = false;
                //btnGenerate.Enabled = false;
                btnExcecute.Enabled = false;
                btnRefreshMenu.Enabled = false;
            }
            //btnEditMenu.Enabled = true;
            _woMenuDesigner.SupportTextSearchValue(true);
            _woMenuDesigner.textSearchModels_ejec();
            _observer.SetLog(_viewMenuStart);
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (
                XtraMessageBox.Show("Deshacer cambios?", "Confirmación", MessageBoxButtons.YesNo)
                == DialogResult.Yes
            )
            {
                _woMenuDesigner.InitializeMode();
                ViewMode();
                _woMenuDesigner.InitializeTreeList();
                _woMenuDesigner.grdMenusView.Focus();
            }
        }

        private void btnDeleteMenu_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if (
                XtraMessageBox.Show(
                    "Desea eliminar el menu?",
                    "Confirmación",
                    MessageBoxButtons.YesNo
                ) == DialogResult.Yes
            )
            {
                try
                {
                    _woMenuDesigner.DeleteMenu();
                    //ViewMode();
                    //_woMenuDesigner.SetFocusMenu(_woMenuDesigner.nameOfMenu);
                    //_observer.SetLog(_deleteMenu);

                    _woMenuDesigner.InitializeMode();
                    ViewMode();
                    _woMenuDesigner.InitializeTreeList();
                    _woMenuDesigner.grdMenusView.Focus();
                }
                catch (Exception ex)
                {
                    throw new WoObserverException(_errorDeleteMenu, ex.Message);
                }
            }
        }

        /// <summary>
        /// Botón para refrescar la lista de modelos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPropMenu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _woMenuDesigner.ChargeGrid();
        }

        #region Save menus events

        /// <summary>
        /// Función para verificar que todos los nodos (menus) tengan etiquetas
        /// </summary>
        private void btnSaveMenu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();
            SaveMenu(sender, e);
            _woMenuDesigner.InitializeMode();
            ViewMode();
            _woMenuDesigner.InitializeTreeList();
            _woMenuDesigner.grdMenusView.Focus();
        }

        /// <summary>
        /// Función auxiliar para  guardar el menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMenu(object sender, object e)
        {
            _woMenuDesigner.SaveMenu();
        }
        #endregion Save menus events




        #region Generator Menus

        public void MenuServer()
        {
            //var menuFocus = grdModelosView.GetFocusedRow();
            //var menuSelect = Directory.GetFiles(proyecto.DirMenus, menuFocus + ".json");
            //string json = File.ReadAllText(menuSelect[0]);
            //WoMenuContainer menu = JsonConvert.DeserializeObject<WoMenuContainer>(json);

            //WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator(
            //    eTypeOfProject.ServerSideMenu
            //);
            //woBlazorGenerator.GenerateMenuServer(
            //    menuContainer: _menuCompleteSelected,
            //    projectName: $@"{txtProjectName.EditValue}_server"
            //);

            //WoExecute _executionBlazor = new WoExecute();

            //Task task2 = _executionBlazor.Iniciar(_menuCompleteSelected + "_proj", true);
        }

        public void MenuWasm()
        {
            //var menuFocus = grdModelosView.GetFocusedRow();
            //var menuSelect = Directory.GetFiles(proyecto.DirMenus, menuFocus + ".json");
            //string json = File.ReadAllText(menuSelect[0]);
            //WoMenuContainer menu = JsonConvert.DeserializeObject<WoMenuContainer>(json);

            //WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator(
            //    eTypeOfProject.WebAssemblyMenu
            //);
            //woBlazorGenerator.GenerateMenuWasm(
            //    menuContainer: menu,
            //    projectName: $@"{txtProjectName.EditValue}_wasm"
            //);

            //WoExecute _executionBlazor = new WoExecute();

            //Task task2 = _executionBlazor.Iniciar(menuFocus + "_proj", true);
        }

        public void BuildServerComplete()
        {
            //var menuFocus = grdModelosView.GetFocusedRow();
            //var menuSelect = Directory.GetFiles(proyecto.DirMenus, menuFocus + ".json");
            //string json = File.ReadAllText(menuSelect[0]);
            //WoMenuContainer menu = JsonConvert.DeserializeObject<WoMenuContainer>(json);

            //WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator(
            //    eTypeOfProject.ServerSideComplete
            //);
            //woBlazorGenerator.GenerateServerSideComplete(
            //    menuContainer: menu,
            //    projectName: $@"{txtProjectName.EditValue}_server"
            //);

            //WoExecute _executionBlazor = new WoExecute();

            //Task task2 = _executionBlazor.Iniciar(menuFocus + "_proj", true);
        }

        public void BuildWasmComplete()
        {
            //var menuFocus = grdModelosView.GetFocusedRow();
            //var menuSelect = Directory.GetFiles(proyecto.DirMenus, menuFocus + ".json");
            //string json = File.ReadAllText(menuSelect[0]);
            //WoMenuContainer menu = JsonConvert.DeserializeObject<WoMenuContainer>(json);

            //WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator(
            //    eTypeOfProject.WeAssemblyComplete
            //);
            //woBlazorGenerator.GenerateWasmComplete(
            //    menuContainer: _menuCompleteSelected,
            //    projectName: $@"{txtProjectName.EditValue}_wasm"
            //);

            //WoExecute _executionBlazor = new WoExecute();

            //Task task2 = _executionBlazor.Iniciar(_menuCompleteSelected + "_proj", true);
        }

        /// <summary>
        /// Botón para generar el menú en blazor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMenuGenerator_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            //_woMenuDesigner.GenerateBlazor();
        }
        #endregion Generator Menus


        #region Alertas

        private WoLog _loadPanel = new WoLog()
        {
            CodeLog = "000",
            Title = "Se cargaron los componentes base de los Menús.",
            Details =
                "Se cargan los paneles principales y la información para la visualización y edición de los menús.",
            UserMessage = "El panel base de los menús ah sido cargado correctamente.",
            LogType = eLogType.Common,
            FileDetails = new WoFileDetails() { Class = "fmMenus_Load", LineAprox = "71" }
        };

        private WoLog _newMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Se creo un nuevo menú.",
            Details = "Se realizo la creación de un nuevo menú.",
            UserMessage = "El menú se creo correctamente.",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails() { Class = "btnNewMenu_ItemClick", }
        };
        private WoLog _editMenuStart = new WoLog()
        {
            CodeLog = "000",
            Title = "Entrar en modo edición de un menú.",
            Details = "Se cargo el modo edición de un menú.",
            UserMessage = "La edición se realizo correctamente.",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails() { Class = "btnNewMenu_ItemClick", }
        };
        private WoLog _viewMenuStart = new WoLog()
        {
            CodeLog = "000",
            Title = "Entro en modo vista de un menú.",
            Details = "Se cargo el modo vista de un menú.",
            UserMessage = "La edición se realizo correctamente.",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails() { Class = "btnNewMenu_ItemClick", }
        };
        private WoLog _deleteMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Eliminación de un menú.",
            Details = "El menú fue borrado correctamente.",
            UserMessage = "La eliminación termino con éxito.",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnDeleteMenu_ItemClick",
                LineAprox = "923"
            }
        };
        private WoLog _errorDeleteMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Error en la eliminación de un menú.",
            Details = "Ocurrió un error mientras se intentaba eliminar el archivo del menú.",
            UserMessage = "La eliminación termino con errores.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnDeleteMenu_ItemClick",
                LineAprox = "941"
            }
        };
        private WoLog _reloadPagesMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Recarga de las paginas menú.",
            Details = "Se realizo la recarga de los modelos como paginas para un menú.",
            UserMessage = "La carga de paginas termino con éxito.",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "ClearListToTree",
                LineAprox = "787"
            }
        };
        private WoLog _reloadMenus = new WoLog()
        {
            CodeLog = "000",
            Title = "Recarga de la lista de menús.",
            Details = "Se realizo la recarga de los menus.",
            UserMessage = "La carga de paginas termino con éxito.",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "LoadDataGrid",
                LineAprox = "133"
            }
        };
        private WoLog _saveNewMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Guardar nuevo menú.",
            Details = "El archivo del menú fue escrito correctamente en la ruta correspondiente",
            UserMessage = "El menú se guardo correctamente",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnSaveMenu_ItemClick",
                LineAprox = "823"
            }
        };
        private WoLog _errorSaveNewMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Guardar nuevo menú.",
            Details = "Hubo errores al intentar guardar el archivo menú.",
            UserMessage = "Problemas al intentar guardar el menú.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnSaveMenu_ItemClick",
                LineAprox = "838"
            }
        };
        private WoLog _deletePage = new WoLog()
        {
            CodeLog = "000",
            Title = "Eliminar pagina.",
            Details = "El archivo del menú fue escrito correctamente en la ruta correspondiente",
            UserMessage = "La pagina se elimino correctamente",
            LogType = eLogType.Success,
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnSaveMenu_ItemClick",
                LineAprox = "823"
            }
        };
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
        private WoLog _clearMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Limpiar pagina.",
            Details = "Se realizo el borrado del diseño actual del menu.",
            UserMessage = "Limpiado del diseño del menu con exito",
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnClear_ItemClick",
                LineAprox = "770"
            }
        };
        private WoLog _ordersMenu = new WoLog()
        {
            CodeLog = "000",
            Title = "Vista de menus agrupados.",
            Details = "Se realizo la concatenación de menus por rol asignado",
            UserMessage = "Agrupación de menus",
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnClear_ItemClick",
                LineAprox = "770"
            }
        };
        private WoLog _dragAndDropPages = new WoLog()
        {
            CodeLog = "000",
            Title = "Vista de menus por rol.",
            Details = "Se realizo la concatenación de menus por rol asignado",
            UserMessage = "Agrupación de menus",
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnClear_ItemClick",
                LineAprox = "770"
            }
        };
        private WoLog _dragAndDropSubmenus = new WoLog()
        {
            CodeLog = "000",
            Title = "Vista de menus por rol.",
            Details = "Se realizo la concatenación de menus por rol asignado",
            UserMessage = "Agrupación de menus",
            FileDetails = new WoFileDetails()
            {
                MethodOrContext = "btnClear_ItemClick",
                LineAprox = "770"
            }
        };
        #endregion Alertas

        private void btnRename_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _woMenuDesigner.RenameMenu();
            _woMenuDesigner.ChargeGrid();
            _woMenuDesigner.InitializeMode();
            ViewMode();
            _woMenuDesigner.InitializeMode();
            _woMenuDesigner.InitializeTreeList();
            _woMenuDesigner.SetFocusMenu(_woMenuDesigner.nameOfMenu);
        }

        /// <summary>
        /// Función para ejecutar el proyecto de la prueba unitaria del menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcecute_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Directory.Exists(path: proyecto.DirApplication + "//Temp//MenusUnitTest"))
            {
                WoBlazorExecute _woExecute = WoBlazorExecute.GetInstance();
                _woExecute.StartBlazor("MenusUnitTest");
            }
            else
            {
                XtraMessageBox.Show(
                    "El menú no existe, favor de generarlo.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Boton para ejecutar el proyecto de la prueba unitaria del menu sin watch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem3_ItemClick_1(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            GenerateServer();
            GenerateWasm();
        }

        private void btnOpenDirectory_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            Process.Start("explorer.exe", $@"{proyecto.DirApplication}\Temp");
        }

        private void btnOpenSaveDirectory_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            Process.Start("explorer.exe", $@"{proyecto.DirLayOuts}\UserCode");
        }

        private void btnGenerateAll_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            WoModelSelector woModelSelector = new WoModelSelector();
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();
            woModelSelector.ShowDialog();
        }

        #region Tipo de ejecucion

        /// <summary>
        /// Actualiza el fichero con la persistencia para la ejecucion del formulario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void cmbExecute_EditValueChanged(object sender, EventArgs e)
        {
            string selectedOption = cmbEjex.EditValue.ToString();
            WoDirectory.WriteFile(
                $@"{proyecto.DirApplication}\Temp\SelectedOption.txt",
                selectedOption
            );
        }

        /// <summary>
        /// Selecciona la opcion que se habia seleccionado anteriormente.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SetSelectedOption()
        {
            if (File.Exists($@"{proyecto.DirApplication}\Temp\SelectedOption.txt"))
            {
                string selectedOption = WoDirectory.ReadFile(
                    $@"{proyecto.DirApplication}\Temp\SelectedOption.txt"
                );
                cmbEjex.EditValue = selectedOption;
            }
        }

        #endregion Tipo de ejecucion

        #region Generacion

        /// <summary>
        /// Genera el proyecto de server y el layout en el que se encuentra el usuario, solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void GenerateServer()
        {
            _woMenuDesigner.GenerateServer();
        }

        /// <summary>
        /// Genera el proyecto de wasm y el layout en el que se encuentra el usuario, solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void GenerateWasm()
        {
            _woMenuDesigner.GenerateWasm();
        }

        /// <summary>
        /// Elimina los ficheros de los proyectos generados.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void DeleteGeneratedProyects()
        {
            try
            {
                string server = $@"{proyecto.DirProyectTemp}/{_serverProjectName}";
                WoDirectory.DeleteDirectory(server);

                string wasm = $@"{proyecto.DirProyectTemp}/{_wasmProjectName}";
                WoDirectory.DeleteDirectory(wasm);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrio un error al eliminar el formulario: {ex.Message}",
                    $@"Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Generacion

        #region Abrir en visual studio

        private void OpenInVisualServer()
        {
            string commandLineArgs =
                $@"/run {proyecto.DirApplication}\Temp\{_serverProjectName}\{_serverProjectName}.csproj";
            Process processInVisual = woVisualStudio.AbreVisualStudio(commandLineArgs, false);
        }

        private void OpenInVisualWasm()
        {
            string commandLineArgs =
                $@"/run {proyecto.DirApplication}\Temp\{_wasmProjectName}\{_wasmProjectName}.csproj";
            Process processInVisual = woVisualStudio.AbreVisualStudio(commandLineArgs, false);
        }

        #endregion Abrir en visual studio


        #region Ejecucion Server

        /// <summary>
        /// Clase ocupada de generar la generacion del proyecto de blazor para server side.
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Utiliza la clase de ejecucion para levantar blazor server.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void ExecuteBlazorServer()
        {
            try
            {
                await _woBlazorServerExecute.StartSimpleServer(_serverProjectName);
                _woBlazorServerExecute.ProcessStatusChangeEvt += ExternalStop;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Se produjo un error al intentar ejecutar el proyecto de blazor. {ex.Message}",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Utiliza la clase de ejecucion para levantar blazor server con el watch.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void WatchBlazorServer()
        {
            try
            {
                await _woBlazorServerExecute.StartWatchServer(_serverProjectName);
                _woBlazorServerExecute.ProcessStatusChangeEvt += ExternalStop;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Se produjo un error al intentar ejecutar el proyecto de blazor. {ex.Message}",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Ejecucion Server

        #region Ejecucion Wasm

        /// <summary>
        /// Clase ocupada de generar la generacion del proyecto de blazor para server side.
        /// </summary>
        private WoBlazorWasmExecute _woBlazorWasmExecute = WoBlazorWasmExecute.GetInstance();

        /// <summary>
        /// Ejecuta el proyecto de blazor de wasm.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void ExecuteBlazorWasm()
        {
            try
            {
                await _woBlazorWasmExecute.StartSimpleWasm(_wasmProjectName);
                _woBlazorWasmExecute.ProcessStatusChangeEvt += ExternalStop;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Se produjo un error al intentar ejecutar el proyecto de blazor. {ex.Message}",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Ejecuta el proyecto wasm en el modo watch.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void WatchBlazorWasm()
        {
            try
            {
                await _woBlazorWasmExecute.StartWatchWasm(_wasmProjectName);
                _woBlazorWasmExecute.ProcessStatusChangeEvt += ExternalStop;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Se produjo un error al intentar ejecutar el proyecto de blazor. {ex.Message}",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Ejecucion Wasm

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
        #region Ejecucion y Stop de blazor

        /// <summary>
        /// Evento click del boton de ejecutar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnExecute_ItemClick(object sender, ItemClickEventArgs e)
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();

            string selectedOption = cmbEjex.EditValue.ToString();

            if (selectedOption != "Eliminar Generados")
            {
                GenerateServer();
                GenerateWasm();
            }

            if (selectedOption == "Solo Server")
            {
                ExecuteBlazorServer();
                btnExcecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Solo Wasm")
            {
                ExecuteBlazorWasm();
                btnExcecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Visual Server")
            {
                OpenInVisualServer();
            }
            else if (selectedOption == "Visual Wasm")
            {
                OpenInVisualWasm();
            }
            else if (selectedOption == "Watch Server")
            {
                WatchBlazorServer();
                btnExcecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Watch Wasm")
            {
                WatchBlazorWasm();
                btnExcecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Solo Server y Wasm")
            {
                ExecuteBlazorServer();
                ExecuteBlazorWasm();
                btnExcecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Watch Server y Wasm")
            {
                WatchBlazorServer();
                WatchBlazorWasm();
                btnExcecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Eliminar Generados")
            {
                DeleteGeneratedProyects();
            }
        }

        /// <summary>
        /// Detiene la ejecucion de cualquiera de los proyectos de blazor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();
            btnExcecute.Enabled = true;
            btnStop.Enabled = false;

            //consoleData.SelectionColor = Color.Green;
            //consoleData.AppendText("Proyecto detenido con exito.");

            //if (_woBlazorServerExecute.SendToConsoleEvt != null)
            //{
            //    _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
            //    _woBlazorServerExecute.SendToConsoleEvt = null;
            //}
            //if (_woBlazorWasmExecute.SendToConsoleEvt != null)
            //{
            //    _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
            //    _woBlazorWasmExecute.SendToConsoleEvt = null;
            //}
        }

        /// <summary>
        /// Función auxiliar para detener los procesos
        /// de blazor
        /// </summary>
        public void StopBlazor(object sender, string b)
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();
            btnExcecute.Enabled = true;
            btnStop.Enabled = false;
        }

        /// <summary>
        /// Metodo suscrito a los eventos de cierre de los proces que lebantan los proyectos
        /// de blazor en caso de un cierre externo al programa.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ExternalStop(object sender, bool e)
        {
            btnExcecute.Enabled = true;
            btnStop.Enabled = false;
        }

        #endregion Ejecucion y Stop de blazor

        private void cmbEjex_EditValueChanged(object sender, EventArgs e)
        {
            string selectedOption = cmbEjex.EditValue.ToString();
            _woMenuDesigner.selectedOption = selectedOption;
        }
    }
}
