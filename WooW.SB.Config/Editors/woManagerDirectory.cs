using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WooW.Core;

namespace WooW.SB.Config.Editors
{
    #region Enumeradores

    /// <summary>
    /// Enumerador para agregar opciones custom al menu
    /// </summary>
    public enum eOptionMenuType
    {
        General,
        Directories,
        Files,
        FileTypes
    }

    /// <summary>
    /// Indica el estatus del nodo
    /// </summary>
    public enum eStatusNode
    {
        Expanded,
        Collapsed,
        NoAply
    }

    #endregion Enumeradores

    public partial class woManagerDirectory : DevExpress.XtraEditors.XtraUserControl
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Path del directorio sobre el que esta trabajando el componente
        /// </summary>
        public string MainPath { get; set; } = string.Empty;

        /// <summary>
        /// Lista de los tipos de fichero a leer por el manager directory.
        /// Default *.cs y *.js
        /// </summary>
        public List<string> FileTypeCol { get; set; } = new List<string>();

        /// <summary>
        /// Nodo que se busca copiar;
        /// </summary>
        private TreeListNode _copyNode;

        /// <summary>
        /// Nodo que se busca mover;
        /// </summary>
        private TreeListNode _cutNode;

        #endregion Atributos


        #region Constructor principal

        /// <summary>
        /// Constructor principal del componente
        /// </summary>
        [SupportedOSPlatform("windows")]
        public woManagerDirectory()
        {
            MainPath = (MainPath == string.Empty) ? _project.DirProyectData_Test : MainPath;
            FileTypeCol = (FileTypeCol.Count == 0)
                ? new List<string>() { "cs", "js" }
                : FileTypeCol;

            InitializeComponent();

            ChargeFilesTree();
        }

        #endregion Constructor principal

        #region Ribbon

        /// <summary>
        /// Retornamos el ribbon para que se haga merge y lo oculta en el componente
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public RibbonControl GetRibbon()
        {
            try
            {
                ribbonControl1.Hide();
                return ribbonControl1;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al intentar recuperar el ribbon. {ex.Message}");
            }
        }

        #endregion Ribbon


        #region Carga del árbol de ficheros

        /// <summary>
        /// Lista de los nodos anteriores al refresh
        /// </summary>
        private List<TreeListNode> _statusListNode = null;

        /// <summary>
        /// Ultimo nodo con el foco, previo al refresh
        /// </summary>
        private TreeListNode _lastFocusedNode = null;

        /// <summary>
        /// Método publico del componente para actualizar la carga de nodos
        /// desde el directorio principal
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public void RefreshComponent()
        {
            try
            {
                _statusListNode = treeDirectories1.GetNodeList();
                _lastFocusedNode = treeDirectories1.FocusedNode;

                ChargeFilesTree();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al intentar actualizar el componente. {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private void RefreshTree(object sender, EventArgs e)
        {
            try
            {
                ChargeFilesTree();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al cargar el árbol de ficheros. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Método principal para orquestar el funcionamiento de carga de los nodos al árbol
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeFilesTree()
        {
            try
            {
                if (Directory.Exists(MainPath))
                {
                    treeDirectories1.BeginUpdate();
                    treeDirectories1.BeginUnboundLoad();
                    treeDirectories1.Nodes.Clear();

                    ChargeFilesRec(directoryPath: MainPath, nodeId: -1);

                    treeDirectories1.EndUnboundLoad();
                    treeDirectories1.EndUpdate();

                    //Persistencia de los status de los nodos
                    if (_statusListNode != null && _statusListNode.Count > 0 && treeDirectories1 != null)
                    {
                        List<TreeListNode> expandedNodes = _statusListNode.Where(node => node.Expanded).ToList();
                        foreach (TreeListNode oldNode in expandedNodes)
                        {
                            TreeListNode findNode = treeDirectories1.Nodes
                                .FirstOrDefault(node => node.Tag.ToString().Replace("\\", "") == oldNode.Tag.ToString().Replace("\\", ""));

                            if (findNode != null)
                            {
                                int indexNode = treeDirectories1.Nodes.IndexOf(findNode);
                                treeDirectories1.Nodes[indexNode].Expanded = true;
                            }
                        }
                    }

                    // Persistencia de selección
                    if (_lastFocusedNode != null && treeDirectories1 != null)
                    {
                        TreeListNode findNode = treeDirectories1.Nodes
                            .FirstOrDefault(node => node.Tag.ToString().Replace("\\", "") == _lastFocusedNode.Tag.ToString().Replace("\\", ""));

                        if (findNode != null)
                        {
                            treeDirectories1.SetFocusedNode(findNode);
                        }
                    }

                    treeDirectories1.Refresh();
                }
                else
                {
                    throw new Exception($@"El path: {MainPath} no existe.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al cargar los ficheros al árbol. {ex.Message}");
            }
        }

        /// <summary>
        /// Recorre de forma recursiva los directorios mientras va agregando los nodos
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void ChargeFilesRec(string directoryPath, int nodeId)
        {
            try
            {
                foreach (string pathDirectory in Directory.GetDirectories(directoryPath))
                {
                    string directoryName = Path.GetFileName(pathDirectory);

                    TreeListNode newNode = treeDirectories1.AppendNode(
                        nodeData: new object[] { directoryName },
                        parentNodeId: nodeId,
                        tag: pathDirectory
                    );

                    newNode.ImageIndex = 0;
                    newNode.SelectImageIndex = 1;
                    newNode.StateImageIndex = 2;

                    ChargeFilesRec(pathDirectory, newNode.Id);
                }

                foreach (string pathFile in Directory.GetFiles(directoryPath))
                {
                    string fileName = Path.GetFileName(pathFile);

                    string extencion = fileName.Split('.').Last().ToLower();

                    if (FileTypeCol.Contains(extencion))
                    {
                        TreeListNode newNode = treeDirectories1.AppendNode(
                            nodeData: new object[] { fileName },
                            parentNodeId: nodeId,
                            tag: pathFile
                        );

                        newNode.ImageIndex = 3;
                        newNode.SelectImageIndex = 3;
                        newNode.StateImageIndex = 3;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"error en el path: {directoryPath}. {ex.Message}");
            }
        }

        #endregion Carga del árbol de ficheros

        #region Selección de un componente

        /// <summary>
        /// Action que se detona cuando cambia el foco del nodo y retorna el tag del nodo
        /// </summary>
        public Action<string> FocusNodeChangeEvt { get; set; }

        /// <summary>
        /// Cambio de elemento seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void treeDirectories_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            try
            {
                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                    return;

                treeDirectories1.OptionsBehavior.Editable = false;

                FocusNodeChangeEvt?.Invoke(treeDirectories1.FocusedNode.Tag.ToString());
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al seleccionar el elemento en la grid. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Recupera el path del elemento seleccionado en el componente
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public string GetSelectedPath()
        {
            try
            {
                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    throw new Exception("No hay nodos seleccionados");
                }
                else
                {
                    return treeDirectories1.FocusedNode.Tag.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar el path seleccionado. {ex.Message}");
            }
        }

        #endregion Selección de un componente


        #region Creación del menu contextual

        /// <summary>
        /// Propiedad del componente para determinar si esta o no activado el menu por default.
        /// </summary>
        public bool ActiveDefaultMenu { get; set; } = true;

        /// <summary>
        /// Lista de las opciones del menu para opciones custom
        /// </summary>
        private List<(string title, Action<string> actionEvt, eOptionMenuType optionMenuType, List<string> fileTypes)> _customContexMenu = new List<(string title, Action<string> actionEvt, eOptionMenuType optionMenuType, List<string> fileTypes)>();

        /// <summary>
        /// Agregamos a la lista de eventos
        /// </summary>
        /// <param name="title"></param>
        /// <param name="actionEvt"></param>
        /// <param name="optionMenuType"></param>
        /// <param name="fileTypes"></param>
        public void AddCustomMenuOption(
            string title,
            Action<string> actionEvt,
            eOptionMenuType optionMenuType = eOptionMenuType.General,
            List<string> fileTypes = null
        )
        {
            try
            {
                _customContexMenu.Add((title, actionEvt, optionMenuType, fileTypes));
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al agregar el elemento {title} al menu custom. {ex.Message}");
            }
        }

        /// <summary>
        /// Extiende el menu contextual del árbol con el resto de opciones para el manejo de ficheros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void treeDirectories_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Actualizar", RefreshTree));

                if (!treeDirectories1.FocusedNode.IsNull() && !treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    string fullPathNode = treeDirectories1.FocusedNode.Tag.ToString();

                    if (_copyNode != null || _cutNode != null)
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Pegar", PasteNode));
                    }

                    if (Directory.Exists(fullPathNode))
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Copiar Carpeta", CopyNode));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Cortar Carpeta", CutNode));
                    }
                    else if (File.Exists(fullPathNode))
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Copiar Fichero", CopyNode));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Cortar Fichero", CutNode));
                    }
                }

                if (ActiveDefaultMenu)
                {
                    if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Nueva Carpeta", CreateNewFolder));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Crear Fichero", CreateNewFile));

                        return;
                    }

                    string fullPathNode = treeDirectories1.FocusedNode.Tag.ToString();

                    if (Directory.Exists(fullPathNode))
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Nueva Carpeta", CreateNewFolder));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Renombrar Carpeta", RenameFolder));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Eliminar Carpeta", DeleteFolder));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Crear Fichero", CreateNewFile));
                    }

                    if (File.Exists(fullPathNode))
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Nueva Carpeta", CreateNewFolder));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Editar Fichero", EditFile));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Renombrar Fichero", RenameFile));

                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem("Eliminar Fichero", DeleteFile));
                    }
                }

                string pathNode = treeDirectories1.FocusedNode.Tag.ToString();
                string extencion = Path.GetExtension(pathNode).Replace(".", "");

                foreach ((string title, Action<string> actionEvt, eOptionMenuType optionMenuType, List<string> fileTypes) method in _customContexMenu)
                {
                    if (method.optionMenuType == eOptionMenuType.General)
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem(
                                caption: method.title,
                                click: ((object sender, EventArgs e) => method.actionEvt.Invoke(pathNode))));
                    }
                    else if (method.optionMenuType == eOptionMenuType.Directories && Directory.Exists(pathNode))
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem(
                                caption: method.title,
                                click: ((object sender, EventArgs e) => method.actionEvt.Invoke(pathNode))));
                    }
                    else if (method.optionMenuType == eOptionMenuType.Files && File.Exists(pathNode))
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem(
                                caption: method.title,
                                click: ((object sender, EventArgs e) => method.actionEvt.Invoke(pathNode))));
                    }
                    else if (method.optionMenuType == eOptionMenuType.FileTypes
                        && File.Exists(pathNode)
                        && method.fileTypes != null
                        && method.fileTypes.Contains(extencion)
                    )
                    {
                        e.Menu.Items.Add(
                            new DevExpress.Utils.Menu.DXMenuItem(
                                caption: method.title,
                                click: ((object sender, EventArgs e) => method.actionEvt.Invoke(pathNode))));
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al construir el menu contextual. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Creación del menu contextual


        #region Gestión de directorios

        /// <summary>
        /// Crea un nuevo directorio
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateNewFolder(object sender, EventArgs e)
        {
            try
            {
                string focusedPath = string.Empty;
                int nodeId = 0;

                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    focusedPath = MainPath;
                }
                else
                {
                    focusedPath = treeDirectories1.FocusedNode.Tag.ToString();
                    if (File.Exists(focusedPath))
                    {
                        string[] focusedPathCol = focusedPath.Split("\\");
                        focusedPath = focusedPath.Replace($"\\{focusedPathCol.Last()}", "");
                        TreeListNode findNode = treeDirectories1.Nodes
                            .FirstOrDefault(node => node.Tag.ToString().Replace("\\", "") == focusedPath.Replace("\\", ""));

                        nodeId = findNode.Id;
                    }
                    else
                    {
                        nodeId = treeDirectories1.FocusedNode.Id;
                    }
                }

                int count = 0;
                string newFolder = "Nueva Carpeta";

                do
                {
                    newFolder = "Nueva Carpeta";
                    newFolder = (count < 1) ? newFolder : $@"{newFolder} ({count})";
                    count++;
                } while (ExistDirectory(focusedPath, newFolder));

                Directory.CreateDirectory($@"{focusedPath}\{newFolder}");

                TreeListNode newNode = treeDirectories1.AppendNode(
                    nodeData: new object[] { newFolder },
                    parentNodeId: nodeId,
                    tag: $@"{focusedPath}\{newFolder}"
                );

                newNode.ImageIndex = 0;
                newNode.SelectImageIndex = 1;
                newNode.StateImageIndex = 2;

                treeDirectories1.SetFocusedNode(newNode);
                treeDirectories1.OptionsBehavior.Editable = true;
                treeDirectories1.ShowEditor();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error en la creación del fichero {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Eliminado del directorio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void DeleteFolder(object sender, EventArgs e)
        {
            try
            {
                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    return;
                }

                DialogResult result = DialogResult.Yes;

                if (Directory.GetDirectories(treeDirectories1.FocusedNode.Tag.ToString()).ToList().Count > 0)
                {
                    result = XtraMessageBox.Show(
                        text: $@"El directorio contiene ficheros, ¿Seguro que desea eliminar el directorio?",
                        caption: "Alerta",
                        icon: MessageBoxIcon.Question,
                        buttons: MessageBoxButtons.YesNo
                    );
                }

                if (result == DialogResult.Yes)
                {
                    Directory.Delete(treeDirectories1.FocusedNode.Tag.ToString(), true);

                    RefreshComponent();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al eliminar directorio {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Verificamos que en el path actual exista un directorio con ese nombre
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool ExistDirectory(string directoryBase, string directoryName)
        {
            try
            {
                bool result = false;

                List<string> subDirectories = Directory.GetDirectories(directoryBase).ToList();
                foreach (string subDirectory in subDirectories)
                {
                    if (Path.GetFileName(subDirectory) == directoryName)
                    {
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($@"error en la validación del directorio. {ex.Message}");
            }
        }

        #endregion Gestión de directorios

        #region Re nombrar directorios

        /// <summary>
        /// Permite poner el nodo en modo de edición para usar en la capa superior
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public void RenameFolder()
        {
            try
            {
                RenameFolder(null, null);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al poner el nodo del directorio en modo edición. {ex.Message}");
            }
        }

        /// <summary>
        /// Renombrado del directorio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void RenameFolder(object sender, EventArgs e)
        {
            try
            {
                treeDirectories1.OptionsBehavior.Editable = true;
                treeDirectories1.ShowEditor();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al renombrar directorio. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Re nombrar directorios


        #region Gestión de ficheros

        /// <summary>
        /// Creación de un nuevo fichero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void CreateNewFile(object sender, EventArgs e)
        {
            try
            {
                string focusedPath = string.Empty;
                int nodeId = 0;

                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    focusedPath = MainPath;
                }
                else
                {
                    focusedPath = treeDirectories1.FocusedNode.Tag.ToString();
                    nodeId = treeDirectories1.FocusedNode.Id;
                }

                int count = 0;
                string newFile = "Nuevo Documento";

                do
                {
                    newFile = "Nuevo Documento";
                    newFile = (count < 1) ? newFile : $@"{newFile} ({count})";
                    count++;
                } while (File.Exists($@"{focusedPath}\{newFile}.{FileTypeCol.First()}"));

                File.Create($@"{focusedPath}\{newFile}.{FileTypeCol.First()}");

                TreeListNode newNode = treeDirectories1.AppendNode(
                    nodeData: new object[] { $@"{newFile}.{FileTypeCol.First()}" },
                    parentNodeId: nodeId,
                    tag: $@"{focusedPath}\{newFile}.{FileTypeCol.First()}"
                );

                newNode.ImageIndex = 3;
                newNode.SelectImageIndex = 3;
                newNode.StateImageIndex = 3;

                treeDirectories1.SetFocusedNode(newNode);
                treeDirectories1.OptionsBehavior.Editable = true;
                treeDirectories1.ShowEditor();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error crear un nuevo fichero. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Eliminado de los ficheros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void DeleteFile(object sender, EventArgs e)
        {
            try
            {
                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    return;
                }

                DialogResult result = DialogResult.Yes;

                if (File.ReadAllText(treeDirectories1.FocusedNode.Tag.ToString()) != string.Empty)
                {
                    result = XtraMessageBox.Show(
                        text: $@"El directorio contiene información, ¿Seguro que desea eliminar el fichero?",
                        caption: "Alerta",
                        icon: MessageBoxIcon.Question,
                        buttons: MessageBoxButtons.YesNo
                    );
                }

                if (result == DialogResult.Yes)
                {
                    File.Delete(treeDirectories1.FocusedNode.Tag.ToString());

                    RefreshComponent();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al eliminar fichero {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Gestión de ficheros

        #region Re nombrar ficheros

        /// <summary>
        /// Método para accesar a la edición del nodo desde una capa superior
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void RenameFile()
        {
            try
            {
                RenameFile(null, null);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al poner el nodo del fichero en modo edición. {ex.Message}");
            }
        }

        /// <summary>
        /// Renombrado del fichero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void RenameFile(object sender, EventArgs e)
        {
            try
            {
                treeDirectories1.OptionsBehavior.Editable = true;
                treeDirectories1.ShowEditor();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al renombrar fichero. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Re nombrar ficheros

        #region Edición de ficheros

        /// <summary>
        /// Action que se detona cuando le damos a la opción de editar el fichero
        /// </summary>
        public Action<string> EditSelectedFileEvt { get; set; }

        /// <summary>
        /// Invocación del action para editar el fichero seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void EditFile(object sender, EventArgs e)
        {
            try
            {
                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    throw new Exception("No hay nodos seleccionados");
                }
                else
                {
                    EditSelectedFileEvt?.Invoke(treeDirectories1.FocusedNode.Tag.ToString());
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al editar el fichero. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Invocación del action para editar el directorio seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (treeDirectories1.FocusedNode.IsNull() || treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    throw new Exception("No hay nodos seleccionados");
                }
                else
                {
                    EditSelectedFileEvt?.Invoke(treeDirectories1.FocusedNode.Tag.ToString());
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al editar el fichero. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Edición de ficheros


        #region Movimiento de un nodo

        /// <summary>
        /// Copia el nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void CopyNode(object sender, EventArgs e)
        {
            try
            {
                _cutNode = null;
                _copyNode = treeDirectories1.FocusedNode;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al copiar nodo. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Corta el nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void CutNode(object sender, EventArgs e)
        {
            try
            {
                _copyNode = null;
                _cutNode = treeDirectories1.FocusedNode;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al cortar nodo. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }


        /// <summary>
        /// Mueve el nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void PasteNode(object sender, EventArgs e)
        {
            try
            {
                if (_copyNode != null)
                {
                    string nodePath = _copyNode.Tag.ToString();
                    if (Directory.Exists(nodePath))
                    {
                        string focusedPath = treeDirectories1.FocusedNode.Tag.ToString();
                        if (File.Exists(focusedPath))
                        {
                            string[] focusedPathCol = focusedPath.Split("\\");
                            focusedPath = focusedPath.Replace($"\\{focusedPathCol.Last()}", "");
                        }

                        string directoryName = Path.GetFileName(nodePath);
                        int count = 0;
                        string newDirectoryName = string.Empty;

                        if (Directory.Exists($"{focusedPath}\\{directoryName}"))
                        {
                            do
                            {
                                count++;
                                newDirectoryName = $@"{directoryName} ({count})";
                            }
                            while (Directory.Exists($"{focusedPath}\\{newDirectoryName}"));

                            DialogResult result = XtraMessageBox.Show(
                                text: $@"Ya existe un directorio {directoryName} en la ruta seleccionada. ¿Desea pegarlo con el nombre {newDirectoryName}?",
                                caption: "Alerta",
                                icon: MessageBoxIcon.Information,
                                buttons: MessageBoxButtons.YesNo
                            );

                            if (result == DialogResult.Yes)
                            {
                                CopyDirectory(nodePath, focusedPath, newDirectoryName);
                            }
                        }
                        else
                        {
                            CopyDirectory(nodePath, focusedPath, directoryName);
                        }
                    }
                    else if (File.Exists(nodePath))
                    {
                        PasteFile(copy: true);
                    }
                    else
                    {
                        throw new Exception($@"El fichero en el path: {nodePath}. No existe.");
                    }

                    RefreshComponent();
                }

                if (_cutNode != null)
                {
                    string nodePath = _cutNode.Tag.ToString();
                    if (Directory.Exists(nodePath))
                    {
                        string focusedPath = treeDirectories1.FocusedNode.Tag.ToString();
                        if (File.Exists(focusedPath))
                        {
                            string[] focusedPathCol = focusedPath.Split("\\");
                            focusedPath = focusedPath.Replace($"\\{focusedPathCol.Last()}", "");
                        }

                        string directoryName = Path.GetFileName(nodePath);
                        int count = 0;
                        string newDirectoryName = string.Empty;

                        if (Directory.Exists($"{focusedPath}\\{directoryName}"))
                        {
                            do
                            {
                                count++;
                                newDirectoryName = $@"{directoryName} ({count})";
                            }
                            while (Directory.Exists($"{focusedPath}\\{newDirectoryName}"));

                            DialogResult result = XtraMessageBox.Show(
                                text: $@"Ya existe un directorio {directoryName} en la ruta seleccionada. ¿Desea pegarlo con el nombre {newDirectoryName}?",
                                caption: "Alerta",
                                icon: MessageBoxIcon.Information,
                                buttons: MessageBoxButtons.YesNo
                            );

                            if (result == DialogResult.Yes)
                            {
                                Directory.Move(nodePath, $"{focusedPath}\\{newDirectoryName}");
                                _cutNode = null;
                            }
                        }
                        else
                        {
                            Directory.Move(nodePath, $"{focusedPath}\\{directoryName}");
                            _cutNode = null;
                        }

                        RefreshComponent();
                    }
                    else if (File.Exists(nodePath))
                    {
                        PasteFile(copy: false);

                        RefreshComponent();

                        //ToDo: Revisar por que no recupera el nodo
                        _copyNode = treeDirectories1.Nodes
                            .FirstOrDefault(node => node.Tag.ToString().Replace("\\", "") == _newNodePath.Replace("\\", ""));
                    }
                    else
                    {
                        throw new Exception($@"El fichero en el path: {nodePath}. No existe.");
                    }
                }


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al pegar nodo. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Copia el directorio en el nodo al directorio de destino
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CopyDirectory(string directoryPath, string destinyPath, string directoryName)
        {
            try
            {
                string newPathDirectory = $"{destinyPath}\\{directoryName}";
                Directory.CreateDirectory(newPathDirectory);

                List<string> subDirectories = Directory.GetDirectories(directoryPath).ToList();

                foreach (string subDirectory in subDirectories)
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    CopyDirectory(subDirectory, newPathDirectory, subDirectoryName);
                }

                List<string> directoryFiles = Directory.GetFiles(directoryPath).ToList();

                foreach (string file in directoryFiles)
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, $"{newPathDirectory}\\{fileName}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al copiar el directorio. {ex.Message}");
            }
        }

        /// <summary>
        /// Path al que se movió el nodo
        /// </summary>
        private string _newNodePath = string.Empty;

        /// <summary>
        /// Pega el fichero en el nodo seleccionado
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void PasteFile(bool copy)
        {
            try
            {
                string nodePath = string.Empty;
                if (copy)
                {
                    nodePath = _copyNode.Tag.ToString();
                }
                else
                {
                    nodePath = _cutNode.Tag.ToString();
                }

                string focusedPath = treeDirectories1.FocusedNode.Tag.ToString();
                if (File.Exists(focusedPath))
                {
                    string[] focusedPathCol = focusedPath.Split("\\");
                    focusedPath = focusedPath.Replace($"\\{focusedPathCol.Last()}", "");
                }

                string fileName = Path.GetFileName(nodePath);
                string newFilePath = $"{focusedPath}\\{fileName}";

                if (!Directory.GetFiles(focusedPath).Contains(newFilePath))
                {
                    if (copy)
                    {
                        File.Copy(nodePath, newFilePath);
                    }
                    else
                    {
                        File.Move(nodePath, newFilePath);
                        _newNodePath = newFilePath;
                        _cutNode = null;
                    }
                }
                else
                {
                    string fileNameBase = Path.GetFileNameWithoutExtension(nodePath);
                    string extencion = Path.GetExtension(nodePath);
                    int count = 0;
                    string newFileName = string.Empty;

                    do
                    {
                        count++;
                        newFileName = $@"{fileNameBase} ({count}){extencion}";
                    }
                    while (Directory.GetFiles(focusedPath).Contains($"{focusedPath}\\{newFileName}"));

                    DialogResult result = XtraMessageBox.Show(
                        text: $@"Ya existe un fichero {fileName} en la ruta seleccionada. ¿Desea pegarlo con el nombre {newFileName}?",
                        caption: "Alerta",
                        icon: MessageBoxIcon.Information,
                        buttons: MessageBoxButtons.YesNo
                    );

                    if (result == DialogResult.Yes)
                    {
                        if (copy)
                        {
                            File.Copy(nodePath, $"{focusedPath}\\{newFileName}");
                        }
                        else
                        {
                            File.Move(nodePath, $"{focusedPath}\\{newFileName}");
                            _newNodePath = newFilePath;
                            _cutNode = null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al copiar el fichero. {ex.Message}");
            }
        }

        #endregion Movimiento de un nodo

        #region Validaciones del nombre del nodo

        /// <summary>
        /// Function que se detona previo a la creación de un fichero para validarlo y en función
        /// del retorno se crea o no el fichero
        /// </summary>
        public Func<bool> ValidateFileNameEvt { get; set; }

        /// <summary>
        /// Re nombra el directorio o el fichero seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void treeDirectories_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                if (treeDirectories1.FocusedNode.Tag.IsNull())
                {
                    throw new Exception("EL path a renombrar no existe");
                }

                string newName = e.Value.ToString();
                string oldPath = treeDirectories1.FocusedNode.Tag.ToString();

                if (Directory.Exists(oldPath))
                {
                    string oldName = Path.GetFileName(oldPath);
                    string basePath = oldPath.Substring(0, (oldPath.Length - oldName.Length - 1));
                    string newPath = $@"{basePath}\{newName}";

                    if (ExistDirectory(directoryBase: basePath, directoryName: newName))
                    {
                        throw new Exception("Ya existe un directorio con ese nombre");
                    }
                    else
                    {
                        Directory.Move(oldPath, newPath);
                    }
                }
                else if (File.Exists(oldPath))
                {
                    string oldName = Path.GetFileName(oldPath);
                    string basePath = oldPath.Substring(0, (oldPath.Length - oldName.Length - 1));
                    string newPath = $@"{basePath}\{newName}";

                    if (File.Exists(newPath))
                    {
                        throw new Exception("Ya existe un fichero con ese nombre");
                    }
                    else
                    {
                        if (ValidateFileNameEvt != null)
                        {
                            if (ValidateFileNameEvt.Invoke())
                            {
                                File.Move(oldPath, newPath);
                            }
                        }
                        else
                        {
                            File.Move(oldPath, newPath);
                        }
                    }
                }
                else
                {
                    throw new Exception("El fichero a renombrar no existe");
                }

                treeDirectories1.Refresh();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al renombrar el fichero {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Validaciones del nombre del nodo
    }
}