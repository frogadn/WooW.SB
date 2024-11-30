using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;

namespace WooW.SB.CodeEditor
{
    public partial class WoTreeMethods : UserControl
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


        #region Atributos de la clase

        /// <summary>
        /// Table que funge de modelo para la grid que representara el árbol con los métodos.
        /// </summary>
        public DataTable _dtMethodsTree;

        #endregion Atributos de la clase


        #region Constructor y métodos principales

        /// <summary>
        /// Constructor por defecto.
        /// y principal de la clase.
        /// </summary>
        public WoTreeMethods(string className)
        {
            InitializeComponent();

            InitializeGridMethods();
        }

        #endregion Constructor y métodos principales


        #region Inicialización de la grid

        /// <summary>
        /// Carga las columnas y configura la grid de los métodos del formulario.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeGridMethods()
        {
            _dtMethodsTree = new DataTable();

            _dtMethodsTree.Columns.Add($@"Controles", typeof(string));
            _dtMethodsTree.Columns.Add($@"Tipo", typeof(string));
            _dtMethodsTree.Columns.Add($@"Eventos", typeof(string));
            _dtMethodsTree.Columns.Add($@"Código", typeof(bool));
            _dtMethodsTree.Columns.Add($@"Activo", typeof(bool));

            grdMethodsBase.DataSource = _dtMethodsTree;
            grdMethods.Columns[@"Controles"].Group();

            grdMethods.Columns[@"Tipo"].Resize(10);
            grdMethods.Columns[@"Código"].Resize(5);
            grdMethods.Columns[@"Activo"].Resize(5);

            grdMethods.ClearSorting();

            grdMethods.Columns[@"Tipo"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
        }

        #endregion Inicialización de la grid


        #region Agregar métodos

        /// <summary>
        /// Agrega métodos a la grid.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void AddMethod(
            string control,
            string type,
            string method,
            bool haveCode,
            bool status
        )
        {
            grdMethodsBase.DataSource = null;
            grdMethods.RefreshData();
            grdMethodsBase.Refresh();

            DataRow drForm = _dtMethodsTree.NewRow();
            drForm[@"Controles"] = control;
            drForm[@"Tipo"] = type;
            drForm[@"Eventos"] = method;
            drForm[@"Código"] = haveCode;
            drForm[@"Activo"] = status;

            if (_dtMethodsTree != null)
            {
                if (drForm != null)
                {
                    _dtMethodsTree.Rows.Add(drForm);

                    //grdMethods.RefreshData();
                    //grdMethodsBase.Refresh();

                    //if (control == "Métodos Custom")
                    //{
                    //    grdMethods.FocusedRowHandle = grdMethods.LocateByValue(
                    //        "Eventos",
                    //        method
                    //    );
                    //}
                }
            }
        }

        [SupportedOSPlatform("windows")]
        public void MethodsReady()
        {
            if (grdMethodsBase != null)
            {
                grdMethodsBase.DataSource = null;
                grdMethodsBase.DataSource = _dtMethodsTree;
                grdMethods.RefreshData();
                grdMethodsBase.Refresh();
            }
        }

        #endregion Agregar métodos


        #region Validación de los métodos

        /// <summary>
        /// Recibe una lista con una tupla, con el nombre del método y un indicador si esta en uso.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void UpdateMethodsUsed(List<(string method, bool used)> updateMethods)
        {
            foreach (var method in updateMethods)
            {
                int row = grdMethods.LocateByValue(@"Eventos", method.method);
                DataRow auxDataRow = grdMethods.GetDataRow(row);
                if (!auxDataRow.IsNull())
                    auxDataRow[@"Activo"] = method.used;
            }
        }

        #endregion Validación de los métodos

        #region Cambiar selección del método

        /// <summary>
        /// Action ocupado de enviar la información del método seleccionado y de
        /// informar del cambio a la clase padre.
        /// </summary>
        public Action<string, string, string> ChangeSeleccionEvt;

        /// <summary>
        /// Row Seleccionada en la grid de métodos.
        /// </summary>
        private DataRow _datarowSelected = null;

        /// <summary>
        /// Row seleccionada anteriormente en la grid de métodos.
        /// </summary>
        private int _prevRowSelected = 0;

        /// <summary>
        /// Se dispara al realizar el cambio del método seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdMethods_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            _prevRowSelected = e.PrevFocusedRowHandle;

            if (grdMethods != null)
            {
                if (grdMethods?.GetFocusedDataRow() != null)
                {
                    _datarowSelected = grdMethods.GetFocusedDataRow();

                    if (_datarowSelected == null)
                        return;

                    ChangeSeleccionEvt?.Invoke(
                        _datarowSelected[@"Eventos"].ToString(),
                        _datarowSelected[@"Controles"].ToString(),
                        _datarowSelected[@"Tipo"].ToString()
                    );
                }
            }
        }

        #endregion Cambiar selección del método


        #region Gestión de los métodos

        /// <summary>
        /// Elimina de la row el método seleccionado y retorna el nombre del método.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public string DeleteMethod()
        {
            string methodDeleted = string.Empty;

            if (_datarowSelected != null)
            {
                if (_datarowSelected[@"Controles"].ToString() == $@"Métodos Custom")
                {
                    if ((bool)_datarowSelected[@"Activo"])
                    {
                        XtraMessageBox.Show(
                            $@"No se puede eliminar un método que esta en uso.",
                            $@"Eliminar método",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    else
                    {
                        methodDeleted = _datarowSelected[@"Eventos"].ToString();
                        _dtMethodsTree.Rows.Remove(_datarowSelected);

                        grdMethods.FocusedRowHandle = 0;
                    }
                }
                else
                {
                    ///Send log cant send base methods or control events
                }
            }

            return methodDeleted;
        }

        #endregion Gestión de los métodos


        #region Update información de la row

        /// <summary>
        /// Actualiza el indicador de si un método tiene código con el parámetro.
        /// </summary>
        /// <param name="haveCode"></param>
        public void UpdateHaveCode(bool haveCode)
        {
            _datarowSelected[@"Código"] = haveCode;
        }

        /// <summary>
        /// Actualiza el status en la grid de los métodos.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="status"></param>
        [SupportedOSPlatform("windows")]
        public void UpdateStatus(string control, bool status)
        {
            int row = grdMethods.LocateByValue(@"Eventos", $@"{control}_OnChange");
            _datarowSelected = grdMethods.GetDataRow(row);
            if (_datarowSelected != null)
                _datarowSelected[@"Activo"] = status;

            row = grdMethods.LocateByValue(@"Eventos", $@"{control}_OnFocus");
            _datarowSelected = grdMethods.GetDataRow(row);
            if (_datarowSelected != null)
                _datarowSelected[@"Activo"] = status;

            row = grdMethods.LocateByValue(@"Eventos", $@"{control}_OnBlur");
            _datarowSelected = grdMethods.GetDataRow(row);
            if (_datarowSelected != null)
                _datarowSelected[@"Activo"] = status;
        }

        /// <summary>
        /// Actualiza el nombre de los métodos en la grid.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public List<(string oldName, string newName)> UpdateEvents(
            WoComponentProperties woComponentProperties
        )
        {
            List<(string oldName, string newName)> methodsChanged =
                new List<(string oldName, string newName)>();

            if (woComponentProperties.ChangedProperty == "Id")
            {
                bool rename = true;
                while (rename)
                {
                    int row = grdMethods.LocateByValue("Controles", woComponentProperties.OldValue);
                    DataRow dataRow = grdMethods.GetDataRow(row);

                    if (dataRow != null)
                    {
                        (string oldName, string newName) changedMethod = (
                            oldName: dataRow[@"Eventos"].ToString(),
                            newName: dataRow[@"Eventos"]
                                .ToString()
                                .Replace(
                                    $@"{woComponentProperties.OldValue}",
                                    woComponentProperties.Id
                                )
                        );

                        methodsChanged.Add(changedMethod);

                        dataRow[@"Controles"] = woComponentProperties.Id;
                        dataRow[@"Eventos"] = changedMethod.newName;
                    }
                    else
                    {
                        rename = false;
                    }
                }
            }

            return methodsChanged;
        }

        #endregion Update información de la row


        #region Mover el metodo selecionado

        /// <summary>
        /// Mueve la seleccion al metodo previo que se habia seleccionado.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void SetLastMethod()
        {
            grdMethods.FocusedRowHandle = _prevRowSelected;
        }

        #endregion Mover el metodo selecionado
    }
}
