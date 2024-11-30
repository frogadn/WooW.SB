using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ServiceStack;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.Menus.MenuModels;
using WooW.SB.Menus.MenusHelper;

namespace WooW.SB.Menus
{
    public partial class WoMenuDesigner : UserControl
    {
        #region Variables globales
        /// <summary>
        /// Variable para indicar que el combo box ya fue abierto y evitar que se vuelva
        /// a ejecutar el evento de SelectedValueChanged
        /// </summary>
        public bool isComboChanged { get; set; } = false;

        #endregion Variables globales


        #region Helpers

        /// <summary>
        /// Helper model para recupera el modelo a través del nombre
        /// </summary>
        private ModelHelper _modelHelper = new ModelHelper();

        #endregion Helpers

        #region AllProcess

        #region Cargar lista de todo procesos
        /// <summary>
        /// Variable que nos permite setearle un valor default al combo box
        /// </summary>
        private string valueCmbAllProcess = "Todos";

        /// <summary>
        /// Dataset de todos los procesos.
        /// </summary>
        private List<string> _menusAllProcesCol = new List<string>();

        /// <summary>
        /// Dataset de procesos con modelos.
        /// </summary>
        List<List<Modelo>> _allProcessModelsCol = new List<List<Modelo>>();

        /// <summary>
        /// Inicializa el menu con todos los procesos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void InitializeAllProcessList()
        {
            WoBuildProcessHelper allProcessHelper = new WoBuildProcessHelper(_woMenuDataSet);
            _allProcessModelsCol.Clear();
            _allProcessModelsCol = allProcessHelper.BuildTree();

            InitializeTreeListAllProcess();
        }

        #endregion Cargar lista de todo procesos

        #region Cargar el árbol con todos los procesos
        /// <summary>
        /// Función para incializar los valores de todos los procesos
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeTreeListAllProcess()
        {
            lbcAllProcess.Items.Clear();
            cmbAllProcess.Properties.Items.Clear();

            cmbAllProcess.Properties.Items.Add("Todos");

            foreach (List<Modelo> processModels in _allProcessModelsCol)
            {
                if (!cmbAllProcess.Properties.Items.Contains(processModels[0].ProcesoId))
                {
                    cmbAllProcess.Properties.Items.Add(processModels[0].ProcesoId);
                }
            }

            cmbAllProcess.SelectedItem = valueCmbAllProcess;

            lbcAllProcess.SelectedValueChanged += SelectedProcessInAllProcess;
            lbcAllProcess.Click += SelectedProcessInAllProcess;
            lbcAllProcess.SelectedIndexChanged += SelectedProcessInAllProcess;
            lbcAllProcess.MouseDown += SelectedProcessInAllProcess;

            //cmbAllProcess_SelectedValueChanged(null, null);

            if (lbcAllProcess.Items.Count == 0)
            {
                cmbAllProcess_TextChanged(null, null);
            }
        }
        #endregion Cargar el árbol con todos los procesos

        #region Cargar nodos del proceso
        /// <summary>
        /// Evento que se detona cuando la selección del combobox a sido cambiada,
        /// Carga lo elementos dependiendo de la selección
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void cmbAllProcess_SelectedValueChanged(object sender, EventArgs e)
        {
            lbcAllProcess.Items.Clear();
            valueCmbAllProcess = cmbAllProcess.Text;
            bool first = false;

            if (cmbAllProcess.Text == "Todos" || cmbAllProcess.Text == "")
            {
                foreach (List<Modelo> process in _allProcessModelsCol)
                {
                    foreach (Modelo model in process)
                    {
                        if (!first)
                        {
                            first = true;
                        }
                        lbcAllProcess.Items.Add(model);
                    }
                }
            }
            else
            {
                List<List<Modelo>> modelsColList = _allProcessModelsCol
                    .Where(x =>
                        x[0]
                            .ProcesoId.Contains(
                                cmbAllProcess.Text,
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    .ToList();
                foreach (var modelsCol in modelsColList)
                {
                    if (modelsCol != null)
                    {
                        foreach (Modelo model in modelsCol)
                        {
                            lbcAllProcess.Items.Add(model);
                        }
                    }
                }
            }
        }

        #endregion Cargar nodos del proceso

        #region Búsqueda in all nodes
        /// <summary>
        /// Evento para ejecutar la función de filtrado de de procesos dentro del listbox
        /// de todos los procesos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void cmbAllProcess_TextChanged(object sender, EventArgs e)
        {
            isComboChanged = true;

            txtSearchModels.Text = string.Empty;
            lbcAllProcess.Items.Clear();
            valueCmbAllProcess = cmbAllProcess.Text;
            bool first = false;
            if (cmbAllProcess.Text == "Todos" || cmbAllProcess.Text == "")
            {
                foreach (List<Modelo> process in _allProcessModelsCol)
                {
                    foreach (Modelo model in process)
                    {
                        if (model.Id.Contains(cmbAllProcess.Text))
                        {
                            lbcAllProcess.Items.Add(model);
                        }
                    }
                }
            }
            else
            {
                List<Modelo> modelsCol = _allProcessModelsCol
                    .Where(x =>
                        x[0]
                            .ProcesoId.Contains(
                                cmbAllProcess.Text,
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    .ToList()
                    .FirstNonDefault();
                ;
                if (modelsCol != null)
                {
                    foreach (Modelo model in modelsCol)
                    {
                        lbcAllProcess.Items.Add(model);
                    }
                }
            }
        }

        #endregion Búsqueda in all nodes

        #region Instanciar el nuevo nodo
        /// <summary>
        /// Función para instanciar el nodo seleccionado dentro del listbox
        /// de todos los procesos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedProcessInAllProcess(object sender, EventArgs e)
        {
            if (!lbcAllProcess.Focused)
            {
                return;
            }
            _menuView.ClearSelection();
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode node in _menuView.Selection)
            {
                node.Selected = false;
            }
            ListBoxControl control = (ListBoxControl)sender;
            if (control.SelectedValue == null)
            {
                return;
            }
            Modelo model = (Modelo)control.SelectedValue;

            _selectedNode = new WoMenuProperties()
            {
                Process = model.ProcesoId,
                Id = model.Id,
                Theme = _themeSelected,
                TypeItem = eTypeItem.MenuItem,
                TypeContainer = eTypeContainer.None,
                Label = EtiquetaCol.Get(model.EtiquetaId),
                MaskText = EtiquetaCol.Get(model.EtiquetaId),
                Enable = _woMenuDataSet.Enable,
                DropDown = _woMenuDataSet.DropDown,
                Reference = model.ProcesoId + "/" + model.TipoModelo + "/" + model.Id,
                BackgroundColor = _woMenuDataSet.BackgroundColor,
                BeginRow = _woMenuDataSet.BeginRow,
                BorderColor = _woMenuDataSet.BorderColor,
                CustomDesignAplied = false,
                Decoration = _woMenuDataSet.Decoration,
                EndingRow = _woMenuDataSet.EndingRow,
                FontColor = _woMenuDataSet.FontColor,
                //FontSize = _woMenuDataSet.FontSize,
                Icon = _woMenuDataSet.Icon,
                IsLogin = _woMenuDataSet.IsLogin,
                Italic = _woMenuDataSet.Italic,
                Orientation = _woMenuDataSet.Orientation,
                ExpandedNode = _woMenuDataSet.ExpandedNode,
                Wide = _woMenuDataSet.Wide,
                Roles = _woMenuDataSet.Roles
            };
            if (model.Id.EndsWith("GridList"))
            {
                _selectedNode.Reference =
                    model.ProcesoId + "/List/" + model.Id.Replace("GridList", "");
            }
            if (model.Id.EndsWith("ReportOdata"))
            {
                _selectedNode.Reference =
                    model.ProcesoId + "/Report/" + model.Id.Replace("ReportOdata", "");
            }
            if (model.SubTipoModelo == WoSubTypeModel.Report)
            {
                _selectedNode.Reference =
                    model.ProcesoId + "/" + model.SubTipoModelo + "/" + model.Id;
            }
        }

        #endregion Instanciar el nuevo nodo

        #endregion AllProcess

        #region SelectedProcess

        #region Cargar lista de los procesos seleccionados

        /// <summary>
        /// Variable para iniciarlizar el valor default del combobox de procesos seleccionados
        /// </summary>
        private string valueCmbSelectedProcess = "Todos";

        /// <summary>
        /// Dataset de los procesos seleccionados.
        /// </summary>
        private List<string> _menusSelectedProcesCol = new List<string>();

        /// <summary>
        /// Dataset de procesos con modelos.
        /// </summary>
        List<List<Modelo>> _selectedProcessModelsCol = new List<List<Modelo>>();

        /// <summary>
        /// Inicializa el menu con los procesos seleccionados.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void InitializeSelectedProcessList()
        {
            WoBuildProcessHelper allProcessHelper = new WoBuildProcessHelper(
                dataSetMenu: _woMenuDataSet,
                allProcess: false
            );
            _selectedProcessModelsCol.Clear();
            _selectedProcessModelsCol = allProcessHelper.BuildTree();

            InitializeTreeListSelectedProcess();
        }

        #endregion Cargar lista de los procesos seleccionados

        #region Cargar el árbol con los procesos seleccionados

        /// <summary>
        /// Función para incializar los valores de los procesos seleccionados
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeTreeListSelectedProcess()
        {
            lbcSelectedProcess.Items.Clear();
            cmbSelectedProcess.Properties.Items.Clear();

            cmbSelectedProcess.Properties.Items.Add("Todos");

            foreach (List<Modelo> processModels in _selectedProcessModelsCol)
            {
                if (!cmbSelectedProcess.Properties.Items.Contains(processModels[0].ProcesoId))
                {
                    cmbSelectedProcess.Properties.Items.Add(processModels[0].ProcesoId);
                }
            }

            cmbSelectedProcess.SelectedItem = valueCmbSelectedProcess;

            lbcSelectedProcess.SelectedValueChanged += SelectedProcessInSelectedProcess;
            lbcSelectedProcess.Click += SelectedProcessInSelectedProcess;
            lbcSelectedProcess.SelectedIndexChanged += SelectedProcessInSelectedProcess;
            lbcSelectedProcess.MouseDown += SelectedProcessInSelectedProcess;

            cmbSelectedProcess_SelectedValueChanged(null, null);

            if (lbcSelectedProcess.Items.Count == 0)
            {
                cmbSelectedProcess_TextChanged(null, null);
            }
        }

        #endregion Cargar el árbol con los procesos seleccionados

        #region Cargar nodos del proceso
        /// <summary>
        /// Evento para ejecutar la función de filtrado de de procesos dentro del listbox
        /// de los procesos seleccionados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSelectedProcess_SelectedValueChanged(object sender, EventArgs e)
        {
            lbcSelectedProcess.Items.Clear();
            valueCmbSelectedProcess = cmbSelectedProcess.Text;
            bool first = false;

            if (cmbSelectedProcess.Text == "Todos" || cmbSelectedProcess.Text == "")
            {
                foreach (List<Modelo> process in _selectedProcessModelsCol)
                {
                    foreach (Modelo model in process)
                    {
                        if (!first)
                        {
                            first = true;
                        }
                        lbcSelectedProcess.Items.Add(model); //se esta agregando dos veces
                    }
                }
            }
            else
            {
                List<Modelo> modelsCol = _selectedProcessModelsCol
                    .Where(x =>
                        x[0]
                            .ProcesoId.Contains(
                                cmbSelectedProcess.Text,
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    .FirstOrDefault();
                if (modelsCol != null)
                {
                    foreach (Modelo model in modelsCol)
                    {
                        if (!first)
                        {
                            first = true;
                        }
                        lbcSelectedProcess.Items.Add(model);
                    }
                }
            }
        }

        #endregion Cargar nodos del proceso

        #region Instanciar el nuevo nodo
        /// <summary>
        /// Función para instanciar el nodo seleccionado dentro del listbox
        /// de los procesos seleccionados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedProcessInSelectedProcess(object sender, EventArgs e)
        {
            if (!lbcSelectedProcess.Focused)
            {
                return;
            }
            _menuView.ClearSelection();
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode node in _menuView.Selection)
            {
                node.Selected = false;
            }
            ListBoxControl control = (ListBoxControl)sender;
            if (control.SelectedValue == null)
            {
                return;
            }
            Modelo model = (Modelo)control.SelectedValue;

            _selectedNode = new WoMenuProperties()
            {
                Process = model.ProcesoId,
                Id = model.Id,
                Theme = _themeSelected,
                TypeItem = eTypeItem.MenuItem,
                TypeContainer = eTypeContainer.None,
                Label = EtiquetaCol.Get(model.EtiquetaId),
                MaskText = EtiquetaCol.Get(model.EtiquetaId),
                Enable = _woMenuDataSet.Enable,
                DropDown = _woMenuDataSet.DropDown,
                Reference = model.ProcesoId + "/" + model.TipoModelo + "/" + model.Id,
                BackgroundColor = _woMenuDataSet.BackgroundColor,
                BeginRow = _woMenuDataSet.BeginRow,
                BorderColor = _woMenuDataSet.BorderColor,
                CustomDesignAplied = false,
                Decoration = _woMenuDataSet.Decoration,
                EndingRow = _woMenuDataSet.EndingRow,
                FontColor = _woMenuDataSet.FontColor,
                //FontSize = _woMenuDataSet.FontSize,
                Icon = _woMenuDataSet.Icon,
                IsLogin = _woMenuDataSet.IsLogin,
                Italic = _woMenuDataSet.Italic,
                Orientation = _woMenuDataSet.Orientation,
                ExpandedNode = _woMenuDataSet.ExpandedNode,
                Wide = _woMenuDataSet.Wide,
                Roles = _woMenuDataSet.Roles
            };
            if (model.Id.EndsWith("GridList"))
            {
                _selectedNode.Reference =
                    model.ProcesoId + "/List/" + model.Id.Replace("GridList", "");
            }
            if (model.Id.EndsWith("ReportOdata"))
            {
                _selectedNode.Reference =
                    model.ProcesoId + "/Report/" + model.Id.Replace("ReportOdata", "");
            }
            if (model.SubTipoModelo == WoSubTypeModel.Report)
            {
                _selectedNode.Reference =
                    model.ProcesoId + "/" + model.SubTipoModelo + "/" + model.Id;
            }
        }

        #endregion Instanciar el nuevo nodo

        #region Búsqueda en los procesos seleccionados
        /// <summary>
        /// Evento que se detona cuando la selección del combobox a sido cambiada,
        /// Carga lo elementos dependiendo de la selección
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSelectedProcess_TextChanged(object sender, EventArgs e)
        {
            isComboChanged = true;

            txtSearchModels.Text = string.Empty;

            lbcSelectedProcess.Items.Clear();
            valueCmbSelectedProcess = cmbSelectedProcess.Text;
            bool first = false;

            if (cmbSelectedProcess.Text == "Todos" || cmbSelectedProcess.Text == "")
            {
                foreach (List<Modelo> process in _selectedProcessModelsCol)
                {
                    foreach (Modelo model in process)
                    {
                        if (!first)
                        {
                            first = true;
                        }
                        lbcSelectedProcess.Items.Add(model);
                    }
                }
            }
            else
            {
                List<List<Modelo>> modelsCol = _selectedProcessModelsCol
                    .Where(x =>
                        x[0]
                            .ProcesoId.Contains(
                                cmbSelectedProcess.Text,
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    .ToList();
                foreach (var listModel in modelsCol)
                {
                    Modelo primerModeloCumpliendoCondicion = listModel.FirstOrDefault();

                    if (listModel != null)
                    {
                        foreach (Modelo model in listModel)
                        {
                            if (!first)
                            {
                                first = true;
                            }
                            lbcSelectedProcess.Items.Add(model);
                        }
                    }
                }
            }
        }

        #endregion Búsqueda en los procesos seleccionados

        #endregion SelectedProcess
    }
}
