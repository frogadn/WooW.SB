using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;
using WooW.SB.Menus.MenuModels;
using WooW.SB.Menus.MenusHelper;

namespace WooW.SB.Menus.MenusComponents
{
    public partial class WoNewMenu : DevExpress.XtraEditors.XtraForm
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        private Proyecto _proyect = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Constructor
        /// <summary>
        /// Variable para almacenar el nombre del menu que se esta guardando o editando
        /// </summary>
        public string nameOfMenu = string.Empty;

        /// <summary>
        /// Variable para almacenar el valor de un menu para que sea posible su
        /// edición
        /// </summary>
        private WoMenuProperties _menuProperties = new WoMenuProperties();

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        public WoNewMenu(WoMenuProperties menu = null)
        {
            InitializeComponent();
            _menuProperties = menu;

            txtMenuName.Focus();
            nameOfMenu = string.Empty;

            //ChargeRoles();
            ChargeRol();
            ChargeProcess();
            SetValues();
        }

        #endregion Constructor


        #region Roles

        ///// <summary>
        ///// Cargo los roles en el combo box.
        ///// </summary>
        //private void ChargeRoles()
        //{
        //    cmbRol.Properties.Items.Add("Sin Rol");

        //    foreach (var rol in _proyect.Roles)
        //    {
        //        cmbRol.Properties.Items.Add(rol.Id);
        //    }

        //    cmbRol.SelectedIndex = 0;
        //}

        #endregion Roles


        #region Cargar Roles
        /// <summary>
        /// DataTable que contiene los roles.
        /// Funge de dataset de la grid.
        /// </summary>
        private DataTable _rolesCol;

        /// <summary>
        /// Indicador de todos los procesos seleccionados.
        /// </summary>
        private bool _rolSelected = false;

        /// <summary>
        /// Permite cargar la lista de los roles al grid view
        /// </summary>
        private void ChargeRol(bool generalStatus = false)
        {
            try
            {
                _rolesCol = new DataTable();
                _rolesCol.Columns.Add(@"Rol", typeof(Rol));
                _rolesCol.Columns.Add(@"Etiqueta", typeof(string));
                _rolesCol.Columns.Add(@"Añadir", typeof(bool));

                List<Rol> listRolesCol = new List<Rol>();

                DataRow rowAllRoles = _rolesCol.NewRow();
                foreach (Rol rol in _proyect.Roles)
                {
                    // Dividir la cadena usando el espacio en blanco como delimitador
                    string[] palabras = rol.Id.Split('-');

                    // Tomar la primera palabra (elemento en el índice 0)
                    string clearRol = palabras[0];

                    if (!listRolesCol.Contains(rol))
                    {
                        listRolesCol.Add(rol);
                        string labelRol = EtiquetaCol.Get(rol.EtiquetaId);
                        DataRow rowRoles = _rolesCol.NewRow();
                        rowRoles[@"Rol"] = rol;
                        rowRoles[@"Etiqueta"] = labelRol;
                        rowRoles[@"Añadir"] = generalStatus;

                        _rolesCol.Rows.Add(rowRoles);
                    }
                }

                //_rolesCol.ColumnChanged += ColumnRolesChange;

                grdRoles.DataSource = _rolesCol;

                GridColumn column = grdRolesView.Columns[@"Rol"];
                column.Width = 50;
                column.OptionsColumn.AllowEdit = false;
                column = grdRolesView.Columns[@"Etiqueta"];
                column.Width = 50;
                column.Visible = false;
                // Deshabilitar la edición para la columna seleccionada
                column.OptionsColumn.AllowEdit = false;
                column = grdRolesView.Columns[@"Añadir"];
                column.Width = 20;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Permite cargar la lista de los roles al grid view a partir de
        /// los roles seleccionados de un menu
        /// </summary>
        private void ChargeRolSelected(List<Rol> roles)
        {
            _rolesCol = new DataTable();
            _rolesCol.Columns.Add(@"Rol", typeof(Rol));
            _rolesCol.Columns.Add(@"Etiqueta", typeof(string));
            _rolesCol.Columns.Add(@"Añadir", typeof(bool));

            List<string> listRolesCol = new List<string>();

            DataRow rowAllRoles = _rolesCol.NewRow();

            foreach (Rol modelo in _proyect.Roles)
            {
                if (!listRolesCol.Contains(modelo.Id))
                {
                    listRolesCol.Add(modelo.Id);

                    DataRow rowRoles = _rolesCol.NewRow();
                    string labelRol = EtiquetaCol.Get(modelo.Id);
                    rowRoles[@"Rol"] = modelo;
                    rowRoles[@"Etiqueta"] = labelRol;
                    List<string> rolesColAux = new List<string>();
                    foreach (Rol rol in roles)
                    {
                        rolesColAux.Add(rol.Id);
                    }
                    if (rolesColAux.Contains(modelo.Id))
                    {
                        rowRoles[@"Añadir"] = true;
                    }
                    else
                    {
                        rowRoles[@"Añadir"] = false;
                    }
                    _rolesCol.Rows.Add(rowRoles);
                }
            }

            grdRoles.DataSource = _rolesCol;

            GridColumn column = grdRolesView.Columns[@"Rol"];
            column.Width = 50;
            column.OptionsColumn.AllowEdit = false;
            column = grdRolesView.Columns[@"Etiqueta"];
            column.Width = 50;
            column.OptionsColumn.AllowEdit = false;
            column = grdRolesView.Columns[@"Añadir"];
            column.Width = 20;
        }

        private void grdRolesView_PopupMenuShowing(
            object sender,
            DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e
        )
        {
            DevExpress.XtraGrid.Menu.GridViewMenu menu = new DevExpress.XtraGrid.Menu.GridViewMenu(
                grdRolesView
            );

            menu.Items.Add(
                new DevExpress.Utils.Menu.DXMenuItem(
                    "Seleccionar Todo",
                    new EventHandler(
                        delegate(object s, EventArgs args)
                        {
                            SelectAllRoles();
                        }
                    )
                )
            );

            menu.Items.Add(
                new DevExpress.Utils.Menu.DXMenuItem(
                    "Deseleccionar Todo",
                    new EventHandler(
                        delegate(object s, EventArgs args)
                        {
                            UnSelectAllRoles();
                        }
                    )
                )
            );

            menu.Show(e.Point);
        }

        private void SelectAllRoles()
        {
            foreach (DataRow row in _rolesCol.Rows)
            {
                row[@"Añadir"] = true;
            }
        }

        private void UnSelectAllRoles()
        {
            foreach (DataRow row in _rolesCol.Rows)
            {
                row[@"Añadir"] = false;
            }
        }
        #endregion Cargar roles


        #region Cargar procesos

        /// <summary>
        /// DataTable que contiene los procesos.
        /// Funge de dataset de la grid.
        /// </summary>
        private DataTable _processCol;

        /// <summary>
        /// Indicador de todos los procesos seleccionados.
        /// </summary>
        private bool _allSelected = false;

        /// <summary>
        /// Permite cargar la lista de los procesos al grid view
        /// </summary>
        private void ChargeProcess(bool generalStatus = false)
        {
            _processCol = new DataTable();
            _processCol.Columns.Add(@"Proceso", typeof(string));
            _processCol.Columns.Add(@"Añadir", typeof(bool));

            List<string> listProcessCol = new List<string>();

            DataRow rowAllProcess = _processCol.NewRow();

            foreach (Modelo modelo in _proyect.ModeloCol.Modelos)
            {
                if (!listProcessCol.Contains(modelo.ProcesoId))
                {
                    listProcessCol.Add(modelo.ProcesoId);

                    DataRow rowProcess = _processCol.NewRow();
                    rowProcess[@"Proceso"] = modelo.ProcesoId;
                    rowProcess[@"Añadir"] = generalStatus;

                    _processCol.Rows.Add(rowProcess);
                }
            }

            _processCol.ColumnChanged += ColumnChange;

            grdProcess.DataSource = _processCol;

            GridColumn column = grdProcessView.Columns[@"Proceso"];
            column.Width = 100;
            // Deshabilitar la edición para la columna seleccionada
            column.OptionsColumn.AllowEdit = false;
            column = grdProcessView.Columns[@"Añadir"];
            column.Width = 20;
        }

        /// <summary>
        /// Permite cargar la lista de los procesos al grid view a partir de
        /// los procesos seleccionados de un menu
        /// </summary>
        private void ChargeProcessSelected(List<string> process)
        {
            _processCol = new DataTable();
            _processCol.Columns.Add(@"Proceso", typeof(string));
            _processCol.Columns.Add(@"Añadir", typeof(bool));

            List<string> listProcessCol = new List<string>();

            DataRow rowAllProcess = _processCol.NewRow();

            foreach (Modelo modelo in _proyect.ModeloCol.Modelos)
            {
                if (
                    modelo.TipoModelo == WoTypeModel.Request
                    || modelo.SubTipoModelo == WoSubTypeModel.Report
                    || modelo.TipoModelo == WoTypeModel.Configuration
                )
                {
                    if (!listProcessCol.Contains(modelo.ProcesoId))
                    {
                        listProcessCol.Add(modelo.ProcesoId);

                        DataRow rowProcess = _processCol.NewRow();
                        rowProcess[@"Proceso"] = modelo.ProcesoId;
                        if (process.Contains(modelo.ProcesoId))
                        {
                            rowProcess[@"Añadir"] = true;
                        }
                        else
                        {
                            rowProcess[@"Añadir"] = false;
                        }
                        _processCol.Rows.Add(rowProcess);
                    }
                }
            }
            _processCol.ColumnChanged += ColumnChange;

            grdProcess.DataSource = _processCol;

            GridColumn column = grdProcessView.Columns[@"Proceso"];
            column.Width = 100;
            column = grdProcessView.Columns[@"Añadir"];
            column.Width = 20;
        }

        /// <summary>
        /// Se detona al realizar un cambio en la columna de procesos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnChange(object sender, DataColumnChangeEventArgs e)
        {
            int row = grdProcessView.LocateByValue(@"Proceso", "Todos");
            DataRow findDataRow = grdProcessView.GetDataRow(row);

            if ((string)e.Row.ItemArray[0] == "Todos")
            {
                if ((bool)findDataRow[@"Añadir"])
                {
                    _allSelected = true;
                    ChargeProcess(true);
                }
            }
            else if (_allSelected)
            {
                _allSelected = false;

                if (findDataRow != null)
                {
                    findDataRow[@"Añadir"] = false;
                }
            }
        }

        #endregion Cargar procesos

        #region Actualizar menu
        /// <summary>
        /// Función para Rellenar los datos del formulario si se trata
        /// de una edición
        /// </summary>
        public void SetValues()
        {
            if (_menuProperties != null)
            {
                txtMenuName.EditValue = _menuProperties.Id;
                //cmbRoles.Text = _menuProperties.Roles[0];
                ChargeProcessSelected(_menuProperties.ProcessList);
                ChargeRolSelected(_menuProperties.Roles);
            }
        }
        #endregion Actualizar menu

        #region Cancel

        /// <summary>
        /// cierra el formulario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            nameOfMenu = txtMenuName.Text;
        }
        #endregion Cancel

        #region Creación del menu
        /// <summary>
        /// Variable auxiliar para indicar si se trata de una edición o un
        /// nuevo registro
        /// </summary>
        public bool _isEdit;

        /// <summary>
        /// LIsta de los procesos seleccionados por el usuario.
        /// </summary>
        private List<string> _selectedProcess = new List<string>();

        /// <summary>
        /// LIsta de los roles seleccionados por el usuario.
        /// </summary>
        private List<Rol> _selectedRoles = new List<Rol>();

        /// <summary>
        /// Evento del botón de aceptar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAcept_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in _processCol.Rows)
            {
                if ((bool)row[1])
                {
                    _selectedProcess.Add(row[0].ToString());
                }
            }
            foreach (DataRow row in _rolesCol.Rows)
            {
                if ((bool)row[2])
                {
                    var aux = row[0];
                    _selectedRoles.Add((Rol)aux); //error here
                }
            }
            if (ValidateNewMenu())
            {
                if (_menuProperties != null)
                {
                    if (File.Exists($@"{_proyect.DirMenus}\{_menuProperties.Id}.json"))
                    {
                        File.Delete($@"{_proyect.DirMenus}\{_menuProperties.Id}.json");
                    }
                }
                if (_menuProperties != null)
                {
                    if (File.Exists($@"{_proyect.DirMenusTemp}\{_menuProperties.Id}.json"))
                    {
                        File.Delete($@"{_proyect.DirMenusTemp}\{_menuProperties.Id}.json");
                    }
                }
                WoMenuProperties menu = new WoMenuProperties();
                if (_isEdit)
                {
                    menu = _menuProperties;
                    menu.Id = txtMenuName.Text;
                    menu.ProcessList = _selectedProcess;
                    menu.Roles = _selectedRoles;
                }
                else
                {
                    WoBuildMenuHelper woBuildMenuHelper = new WoBuildMenuHelper(
                        Nombre: txtMenuName.Text,
                        process: _selectedProcess,
                        roles: _selectedRoles
                    );

                    menu = woBuildMenuHelper.GetRawMenu();
                }

                WoDirectory.WriteFile(
                    path: $@"{_proyect.DirMenus}\{txtMenuName.Text}.json",
                    JsonConvert.SerializeObject(menu)
                );
                WoDirectory.WriteFile(
                    path: $@"{_proyect.DirMenusTemp}\{txtMenuName.Text}.json",
                    JsonConvert.SerializeObject(menu)
                );

                nameOfMenu = txtMenuName.Text;
                this.Close();
            }
        }

        /// <summary>
        /// Valida las configuraciones seleccionadas por el usuario para la creación del menú.
        /// </summary>
        /// <returns></returns>
        private bool ValidateNewMenu()
        {
            bool result = true;

            // Definir el patrón que solo permite letras y números
            string patron = @"^[a-zA-Z0-9\s]*$";

            // Crear una instancia de Regex
            Regex regex = new Regex(patron);

            // Verificar si la cadena cumple con el patrón
            bool esValida = regex.IsMatch(txtMenuName.Text);

            if (!esValida)
            {
                XtraMessageBox.Show(
                    "La cadena no debe contener caracteres especiales.",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                txtMenuName.Text = "";
                _selectedProcess.Clear();
                return false;
            }

            //Nombre existe
            if (txtMenuName.Text == "")
            {
                XtraMessageBox.Show(
                    "Debe ingresar un nombre para el menú.",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                _selectedProcess.Clear();
                return false;
            }

            //El nombre no se repite
            string menuName = txtMenuName.Text;
            List<string> menus = WoDirectory.ReadDirectoryFiles(_proyect.DirMenus, onlyNames: true);
            if (menus.Contains(menuName))
            {
                if (!_isEdit && _menuProperties == null)
                {
                    XtraMessageBox.Show(
                        "Ya existe un menu con ese nombre.",
                        "Alerta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
                    _selectedProcess.Clear();
                    return false;
                }
            }
            return result;
        }

        #endregion Creación del menu

        #region Comportamiento del grid de procesos

        public void ProcessGridChangeStatus(bool status)
        {
            grdProcessView.OptionsBehavior.Editable = status;
            //grdRolesView.OptionsBehavior.Editable = status;
        }
        #endregion Comportamiento del grid de procesos

        private void panelControl3_Paint(object sender, PaintEventArgs e) { }

        private void grdProcessView_PopupMenuShowing(
            object sender,
            DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e
        )
        {
            DevExpress.XtraGrid.Menu.GridViewMenu menu = new DevExpress.XtraGrid.Menu.GridViewMenu(
                grdProcessView
            );
            if (!_isEdit)
            {
                menu.Items.Add(
                    new DevExpress.Utils.Menu.DXMenuItem(
                        "Seleccionar Todo",
                        new EventHandler(
                            delegate(object s, EventArgs args)
                            {
                                SelectAllProcess();
                            }
                        )
                    )
                );

                menu.Items.Add(
                    new DevExpress.Utils.Menu.DXMenuItem(
                        "Deseleccionar Todo",
                        new EventHandler(
                            delegate(object s, EventArgs args)
                            {
                                UnSelectAllProcess();
                            }
                        )
                    )
                );

                menu.Show(e.Point);
            }
        }

        private void SelectAllProcess()
        {
            foreach (DataRow row in _processCol.Rows)
            {
                row[@"Añadir"] = true;
            }
        }

        private void UnSelectAllProcess()
        {
            foreach (DataRow row in _processCol.Rows)
            {
                row[@"Añadir"] = false;
            }
        }

        //private void grdRolesView_CellValueChanged(
        //    object sender,
        //    DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e
        //)
        //{
        //    try
        //    {
        //        int row = grdRolesView.LocateByValue(@"Rol", "Todos");
        //        DataRow findDataRow = grdRolesView.GetDataRow(row);
        //        var a = grdRolesView.GetRowCellValue(e.RowHandle, grdRolesView.Columns[0]);
        //        bool check = (bool)
        //            grdRolesView.GetRowCellValue(e.RowHandle, grdRolesView.Columns[2]);
        //        if (a == "Todos")
        //        {
        //            _rolSelected = true;
        //            ChargeRol(check);
        //        }
        //        else if (_rolSelected)
        //        {
        //            _rolSelected = false;

        //            if (findDataRow != null)
        //            {
        //                findDataRow[@"Añadir"] = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}
