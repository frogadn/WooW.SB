using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using ServiceStack;
using WooW.Core;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.BlazorGenerator;
using WooW.SB.BlazorGenerator.BlazorDialogs;
using WooW.SB.BlazorGenerator.BlazorDialogs.BlazorDialogsModels;
using WooW.SB.BlazorGenerator.BlazorGeneration.BlazorSave;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.BlazorGenerator.BlazorProjectFiles.UserCode;
using WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.UserCode;
using WooW.SB.CodeEditor;
using WooW.SB.CodeEditor.CodeDialogs;
using WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorFiles;
using WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorSettings;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerHelpers;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Designer;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Forms
{
    public partial class fmModelLayOut : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        #region Instancias singleton

        /// <summary>
        /// Variable con la información global del proyecto
        /// Se pueden obtener los modelos desde esta
        /// </summary>
        public Proyecto proyecto { get; set; }

        /// <summary>
        /// Instancia del observador del proyecto.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton


        #region Constructor y métodos base del formulario
        private void fmModelLayOut_Load(object sender, EventArgs e) { }

        public fmModelLayOut()
        {
            InitializeComponent();
        }

        [SupportedOSPlatform("windows")]
        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        [SupportedOSPlatform("windows")]
        public bool CambiosPendientes
        {
            get { return buAceptarCambios.Enabled; }
        }

        [SupportedOSPlatform("windows")]
        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        #endregion Constructor y métodos base del formulario

        #region Carga de la pantalla

        /// <summary>
        /// Se ejecuta al cargar todo el layout
        /// deshabilita algunos botones que no se usan
        /// y llama el método que inicializa el resto
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void Cargar()
        {
            try
            {
                WoDesignerLabels woDesignerLabels = new WoDesignerLabels();
                woDesignerLabels.AddLabels();

                buAceptarCambios.Enabled = false;
                buDescartarCambios.Enabled = false;
                CargarModelo();

                DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbExecuteControl =
                    cmbExecute.Edit as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                cmbExecuteControl.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error en el método principal de cargar {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Carga de la pantalla


        #region Variables de control del diseñador

        /// <summary>
        ///  Bandera que indica si la version actual del diseñador se encuentra salvada.
        /// </summary>
        private bool _isSaved = true;

        /// <summary>
        /// Bandera para indicar el restablecimiento del diseño.
        /// </summary>
        private bool _restart = false;

        /// <summary>
        /// Actualiza el estado de la bandera de salvado
        /// y el indicador de la etiqueta del tab.
        /// </summary>
        /// <param name="newStatus"></param>
        [SupportedOSPlatform("windows")]
        private void ChangeSavedStatus(bool newStatus)
        {
            _isSaved = newStatus;
            if (_isSaved)
            {
                tabBaseDesigner.TabPages[0].Text = "Diseñador";
            }
            else
            {
                tabBaseDesigner.TabPages[0].Text = "*Diseñador";
            }
        }

        #endregion Variables de control del diseñador


        #region Variables principales del proyecto generado

        /// <summary>
        /// Nombre del proyecto de blazor server de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _serverProjectName = "ServerUnitModel_proj";

        /// <summary>
        /// Nombre base de las clases que contendrán las pruebas unitarias en caso de ser para server.
        /// </summary>
        private string _serverClassModelName = "ServerUnitModel";

        /// <summary>
        /// Nombre del proyecto de blazor wasm de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _wasmProjectName = "WasmUnitModel_proj";

        /// <summary>
        /// Nombre base de las clases que contendrán las pruebas unitarias en caso de ser para wasm.
        /// </summary>
        private string _wasmClassModelName = "WasmUnitModel";

        /// <summary>
        /// Define el nombre de la clase de la que se creara el formulario.
        /// </summary>
        private string _modelName = "";

        #endregion Variables principales del proyecto generado


        #region Propiedades de la grid

        /// <summary>
        /// Fila de la grid seleccionada
        /// </summary>
        private DataRow _dataRowSelected;

        /// <summary>
        /// Modelo seleccionado por el usuario en la grid de la izquierda.
        /// es el campo oculto en la grid con el json.
        /// Contiene todos los campos del modelo y sus definiciones.
        /// </summary>
        private Modelo _modelSelected;

        /// <summary>
        /// Indicador de si es o no una extension de un modelo.
        /// </summary>
        private bool _isExtencion = false;

        #endregion Propiedades de la grid

        #region Carga de los modelos en la grid

        /// <summary>
        /// carga la grid lateral e inicializa el diseñador correspondiente
        ///
        /// usa un data table para cargarlo a la grid
        ///
        /// recorre los modelos que se encuentran en la variable program
        /// para agregarlos al data table se valida que sea alguno de los tipos de modelo coincidentes
        /// en caso de ser una esclava se agrega otro formulario ademas del de la grid
        ///
        /// carga el data table a la grid y esconde el campo del json en la grid lateral
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CargarModelo()
        {
            DataTable modelosCol = new DataTable();

            modelosCol.Columns.Add(@"Proceso", typeof(string));
            modelosCol.Columns.Add(@"Tipo", typeof(string));
            modelosCol.Columns.Add(@"Repositorio", typeof(string));
            modelosCol.Columns.Add(@"Modelo", typeof(string));
            modelosCol.Columns.Add(@"Diseñado", typeof(bool));
            modelosCol.Columns.Add(@"Json", typeof(string));

            foreach (var modelo in proyecto.ModeloCol.Modelos)
            {
                if (
                    (
                        !(
                            modelo.TipoModelo == WoTypeModel.Request
                            && modelo.SubTipoModelo != WoSubTypeModel.Report
                        )
                    )
                    && modelo.TipoModelo != WoTypeModel.Response
                    && modelo.TipoModelo != WoTypeModel.Complex
                    && modelo.TipoModelo != WoTypeModel.Interface
                    && modelo.TipoModelo != WoTypeModel.Class
                    && modelo.TipoModelo != WoTypeModel.Parameter
                    && modelo.TipoModelo.ToString() != "CollectionComplex"
                )
                {
                    bool diseño = (
                        File.Exists($@"{proyecto.DirLayOuts}\FormDesign\{modelo.Id}.json")
                        || File.Exists(
                            $@"{proyecto.DirLayOuts}\ListDesign\{modelo.Id}GridList.json"
                        )
                    );

                    DataRow drRow = modelosCol.NewRow();
                    drRow[@"Proceso"] = modelo.ProcesoId;
                    drRow[@"Tipo"] = modelo.TipoModelo.ToString();
                    drRow[@"Repositorio"] = modelo.Repositorio.ToString();
                    drRow[@"Modelo"] = modelo.Id;
                    drRow[@"Diseñado"] = diseño;
                    drRow[@"Json"] = modelo.ToJson();
                    modelosCol.Rows.Add(drRow);
                }
            }

            grdModelos.DataSource = modelosCol;

            GridColumn column = grdModelosView.Columns[@"Proceso"];
            column.Width = 100;

            column = grdModelosView.Columns[@"Tipo"];
            column.Width = 100;

            column = grdModelosView.Columns[@"Repositorio"];
            column.Width = 100;

            column = grdModelosView.Columns[@"Modelo"];
            column.Width = 300;

            grdModelosView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        column,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            column = grdModelosView.Columns[@"Json"];
            column.Visible = false;
            HandleButtonsInit();
        }

        /// <summary>
        /// Control de la botonera de generación
        /// Habilita o deshabilita de acuerdo al estado de la existencia de los proyectos generados
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void HandleButtonsInit()
        {
            ///Control de la botonera de generación
            string pathProyectServer =
                $@"{proyecto.DirProyectTemp}/{_serverProjectName}/{_serverProjectName}.csproj";
            if (File.Exists(pathProyectServer))
            {
                btnDeleteBlazor.Enabled = true;

                btnGenerateServer.Caption = "Regenerar Server";
                btnGenerateServer.Enabled = true;
                btnExecuteServer.Enabled = true;
                btnAbrirServer.Enabled = true;
                btnWatchServer.Enabled = true;
                btnDeleteServer.Enabled = true;
                btnStopServer.Enabled = false;
            }
            else
            {
                btnDeleteBlazor.Enabled = false;

                btnGenerateServer.Caption = "Generar Server";
                btnGenerateServer.Enabled = true;
                btnExecuteServer.Enabled = false;
                btnAbrirServer.Enabled = false;
                btnWatchServer.Enabled = false;
                btnDeleteServer.Enabled = false;
                btnStopServer.Enabled = false;
            }

            string pathProyectWasm =
                $@"{proyecto.DirProyectTemp}/{_wasmProjectName}/{_wasmProjectName}.csproj";
            if (File.Exists(pathProyectWasm))
            {
                btnDeleteBlazor.Enabled = true;

                btnGenerateWasm.Caption = "Regenerar Wasm";
                btnGenerateWasm.Enabled = true;
                btnExecuteWasm.Enabled = true;
                btnAbrirWasm.Enabled = true;
                btnWatchWasm.Enabled = true;
                btnDeleteWasm.Enabled = true;
                btnStopWasm.Enabled = false;
            }
            else
            {
                btnDeleteBlazor.Enabled = false;

                btnGenerateWasm.Caption = "Generar Wasm";
                btnGenerateWasm.Enabled = true;
                btnExecuteWasm.Enabled = false;
                btnAbrirWasm.Enabled = false;
                btnWatchWasm.Enabled = false;
                btnDeleteWasm.Enabled = false;
                btnStopWasm.Enabled = false;
            }
        }

        #endregion Carga de los modelos en la grid

        #region Selección de un modelo en la grid

        private int _lastSelectedIndex = 0;

        private bool _noNewSelect = false;

        /// <summary>
        /// Evento que se detona cuando el usuario selecciona otro modelo en la grid
        /// detona el método para cargar el diseñador e inicializar variables globales.
        /// Si hay una ejecución de blazor en proceso la detiene.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdModelosView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            try
            {
                object focusedRow = grdModelosView.GetFocusedRow();
                if (focusedRow == null)
                    return;

                DialogResult result = DialogResult.Yes;
                if (!_isSaved && (!_noNewSelect))
                {
                    result = XtraMessageBox.Show(
                        "El proyecto aun no se a guardado, ¿Seguro que desea salir y descartar los cambios?",
                        "Alert",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                }

                if (!_isSavedCode && (!_noNewSelect))
                {
                    result = XtraMessageBox.Show(
                        "El código aun no se a guardado, ¿Seguro que desea salir y descartar los cambios?",
                        "Alert",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                }

                bool stop = false;

                if (
                    (!cmbExecute.EditValue.ToString().Contains("Watch"))
                    && (_woBlazorServerExecute.IsRuning() || _woBlazorWasmExecute.IsRuning())
                    && _lastSelectedIndex != grdModelosView.GetFocusedDataSourceRowIndex()
                )
                {
                    result = XtraMessageBox.Show(
                        "El proyecto en ejecución no se puede actualizar mientras se encuentra en ejecución, ¿Desea cerrarlo?",
                        "Alert",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    stop = result == DialogResult.Yes;
                }

                if (result == DialogResult.Yes && (!_noNewSelect))
                {
                    consoleData.Text = string.Empty;

                    if (stop)
                    {
                        btnStop_ItemClick(null, null);
                        //generate = true;
                    }

                    _isSavedCode = true;
                    _isSaved = true;

                    TabCodeEditor.PageVisible = false;

                    BlockCodeGroup();

                    ModelHelper modelHelper = new ModelHelper();
                    DataRow dataRow = grdModelosView.GetFocusedDataRow();
                    _modelSelected = modelHelper.SearchModel(dataRow[@"Modelo"].ToString());
                    _isExtencion = (_modelSelected.ProcesoId == string.Empty);

                    if (
                        _modelSelected.TipoModelo == WoTypeModel.CatalogSlave
                        || _modelSelected.TipoModelo == WoTypeModel.TransactionSlave
                    )
                    {
                        _isSlave = true;
                    }
                    else
                    {
                        _isSlave = false;
                    }

                    UpdateUsings();

                    if (
                        _modelSelected.TipoModelo != WoTypeModel.Kardex
                        && _modelSelected.TipoModelo != WoTypeModel.Control
                        && _modelSelected.TipoModelo != WoTypeModel.Configuration
                        && _modelSelected.TipoModelo != WoTypeModel.View
                    )
                    {
                        InitializeDesigner();
                    }

                    if (
                        _modelSelected.TipoModelo == WoTypeModel.Kardex
                        || _modelSelected.TipoModelo == WoTypeModel.Control
                        || _modelSelected.TipoModelo == WoTypeModel.Configuration
                        || _modelSelected.TipoModelo == WoTypeModel.View
                    )
                    {
                        _modelName = _modelSelected.Id;
                        TabFormDesigner.PageVisible = false;
                    }

                    if (
                        File.Exists(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs"
                        )
                    )
                    {
                        UpdateStartForm();
                        EnableCodeGroup();
                    }

                    bool generate = !_noNewSelect;

                    if (
                        _modelSelected.TipoModelo == WoTypeModel.TransactionSlave
                        || _modelSelected.TipoModelo == WoTypeModel.CatalogSlave
                    )
                    {
                        TabListDesigner.PageVisible = false;

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "Código de lista"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Código de esclava";

                            tab.PageEnabled = false;
                            tab.PageVisible = true;
                        }
                    }
                    else
                    {
                        TabListDesigner.PageVisible = true;

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "Código de esclava"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Código de lista";

                            tab.PageEnabled = true;
                        }

                        InitializeListDesigner();
                    }

                    if (generate)
                    {
                        _lastSelectedIndex = grdModelosView.GetFocusedDataSourceRowIndex();

                        SaveUnitProjects();
                    }
                    else
                    {
                        if (_lastSelectedIndex != grdModelosView.GetFocusedDataSourceRowIndex())
                        {
                            _noNewSelect = true;
                            grdModelosView.FocusedRowHandle = _lastSelectedIndex;
                        }
                        else
                        {
                            _noNewSelect = false;
                        }
                    }

                    ChangeSavedStatus(true);

                    if (
                        File.Exists(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs"
                        )
                        && (
                            _modelSelected.TipoModelo != WoTypeModel.Configuration
                            && _modelSelected.TipoModelo != WoTypeModel.Kardex
                            && _modelSelected.TipoModelo != WoTypeModel.Control
                            && _modelSelected.TipoModelo != WoTypeModel.View
                        )
                    )
                    {
                        InitializeWoSyntaxEditor();
                        _isSavedCode = true;
                    }

                    if (
                        (
                            _modelSelected.TipoModelo == WoTypeModel.Request
                            && _modelSelected.SubTipoModelo == WoSubTypeModel.Report
                        )
                        || _modelSelected.TipoModelo == WoTypeModel.TransactionSlave
                        || _modelSelected.TipoModelo == WoTypeModel.CatalogSlave
                    )
                    {
                        btnExecute.Enabled = false;
                    }
                    else
                    {
                        btnExecute.Enabled = true;
                    }

                    if (
                        _modelSelected.TipoModelo != WoTypeModel.CatalogSlave
                        && _modelSelected.TipoModelo != WoTypeModel.TransactionSlave
                        && _modelSelected.TipoModelo != WoTypeModel.Request
                    )
                    {
                        InitializeWoGridSyntaxEditor();
                    }
                    else if (
                        _modelSelected.TipoModelo == WoTypeModel.Request
                        && _modelSelected.SubTipoModelo == WoSubTypeModel.Report
                    )
                    {
                        TabListCodeEditor.PageVisible = false;
                        TabGridDesigner.PageVisible = false;
                        TabListDesigner.PageVisible = false;
                    }

                    _noNewSelect = false;

                    if (
                        _modelSelected.TipoModelo == WoTypeModel.Configuration
                        || _modelSelected.TipoModelo == WoTypeModel.Kardex
                        || _modelSelected.TipoModelo == WoTypeModel.Control
                        || _modelSelected.TipoModelo == WoTypeModel.View
                    )
                    {
                        TabCodeEditor.PageVisible = false;
                        TabGridDesigner.PageVisible = false;
                    }

                    if (!(bool)dataRow["Diseñado"])
                    {
                        if (
                            _modelSelected.TipoModelo != WoTypeModel.CatalogSlave
                            && _modelSelected.TipoModelo != WoTypeModel.TransactionSlave
                            && _modelSelected.TipoModelo != WoTypeModel.Request
                        )
                        {
                            if (
                                _modelSelected.TipoModelo != WoTypeModel.Configuration
                                && _modelSelected.TipoModelo != WoTypeModel.Kardex
                                && _modelSelected.TipoModelo != WoTypeModel.Control
                                && _modelSelected.TipoModelo != WoTypeModel.View
                            )
                            {
                                if (!_isExtencion)
                                {
                                    WoCustomButtonsRawHelper woCustomButtonsRawHelper =
                                        new WoCustomButtonsRawHelper();
                                    woCustomButtonsRawHelper.BuildRawGridList(_modelSelected.Id);
                                }
                            }

                            InitializeWoGridSyntaxEditor();
                            InitializeListDesigner();
                        }
                    }
                }
                else if (_noNewSelect)
                {
                    _noNewSelect = false;
                }
                else
                {
                    _noNewSelect = true;
                    grdModelosView.FocusedRowHandle = _lastSelectedIndex;
                }

                UpdateForSlaves();

                _woBlazorServerExecute.GeneralStop();
                _woBlazorWasmExecute.GeneralStop();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al cambiar el foco del modelo. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Selección de un modelo en la grid


        #region Instancias de los diseñadores

        /// <summary>
        /// Diseñador para diseñar formularios
        /// Se inicializa cuando se define si el modelo seleccionado es un modelo normal o una esclava.
        /// </summary>
        private WoFormDesigner _woFormDesginer;

        /// <summary>
        /// Diseñador para las grids.
        /// Se inicializa cuando se define si el modelo seleccionado es un modelo normal o una esclava.
        /// </summary>
        private WoGridDesigner _woGridDesigner;

        #endregion Instancias de los diseñadores

        #region Inicio del designer

        /// <summary>
        /// Ejecuta los métodos para inicializar el diseñador
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeDesigner()
        {
            ((System.ComponentModel.ISupportInitialize)(this.pnlFormDesigner)).BeginInit();
            this.SuspendLayout();

            TabFormDesigner.PageVisible = true;
            ValidateDesignerExist();
            ChargeDesigner();
            SetSelectedOption();

            ((System.ComponentModel.ISupportInitialize)(this.pnlFormDesigner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Inicio del designer

        #region Validación del diseñador

        /// <summary>
        /// Valida si ya hay un diseñador inicializado y en ese caso
        /// desecha el diseñador actual e iguala la variable a nulo
        /// lo hace tanto para el diseñador del formulario como el de la grid
        /// y actualiza la variable de la grid seleccionada
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ValidateDesignerExist()
        {
            if (_woFormDesginer != null)
            {
                _woFormDesginer.BlockMode();
                _woFormDesginer.Dispose();
                _woFormDesginer = null;
            }

            if (_woGridDesigner != null)
            {
                _woGridDesigner.BlockMode();
                TabGridDesigner.PageVisible = false;
                _woGridDesigner.Dispose();
                _woGridDesigner = null;
            }

            _dataRowSelected = grdModelosView.GetFocusedDataRow();
        }

        #endregion Validación del diseñador

        #region Carga principal del diseñador

        /// <summary>
        /// Variable que indica si el modelo seleccionado es una esclava.
        /// </summary>
        private bool _isSlave = false;

        /// <summary>
        /// Carga el diseñador en el panel del formulario
        /// Valida que la variable que guarda la row seleccionada no sea nula
        /// obtiene una instancia de Modelo desde el json de la columna de json de la row seleccionada
        /// obtiene el nombre del modelo que viene en la columna Modelo, limpia el nombre y lo guarda en la variable assembly
        /// la variable assembly indica el nombre la clase, como si fuese un type.name
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeDesigner()
        {
            proyecto.AssemblyModelCargado = true;

            if (_dataRowSelected != null)
            {
                TabGridDesigner.PageVisible = false;

                ModelHelper modelHelper = new ModelHelper();
                _modelSelected = modelHelper.SearchModel(_dataRowSelected[@"Modelo"].ToString());

                string modelSelected = _dataRowSelected[@"Modelo"].ToString();
                _modelName = modelSelected;

                string modelType = _dataRowSelected[@"Tipo"].ToString();
                if (modelType == "TransactionSlave" || modelType == "CatalogSlave")
                {
                    TabGridDesigner.PageVisible = true;

                    _woGridDesigner = new WoGridDesigner(
                        saveFolder: "Grids",
                        modelName: _modelName
                    );
                    _woGridDesigner.Dock = DockStyle.Fill;
                    pnlGridDesigner.Controls.Add(_woGridDesigner);
                    _woGridDesigner.ModeChanged += ChangeDesignMode;
                    _woGridDesigner.AddButtonEvt += InitializeWoGridSyntaxEditor;
                    _woGridDesigner.DeleteButtonEvt += InitializeWoGridSyntaxEditor;

                    _woGridDesigner.GridUpdatingEvt += UpdatingGrid;

                    _isSlave = true;
                }
                else
                {
                    _isSlave = false;
                }

                _woFormDesginer = new WoFormDesigner(
                    typeModel: _dataRowSelected[@"Tipo"].ToString(),
                    modelName: _modelName,
                    modelJson: _modelSelected
                );
                _woFormDesginer.Dock = DockStyle.Fill;
                pnlFormDesigner.Controls.Add(_woFormDesginer);

                _woFormDesginer.ModeChanged += ChangeDesignMode;
                _woFormDesginer.BlockMode();

                _woFormDesginer.EditingEvt += EditingForm;

                _woFormDesginer.UpdateCodeEditor += UpdateChangesControlsCustom;

                UpdateScripts();
            }
            else
            {
                XtraMessageBox.Show(
                    $@"No hay ningún modelo seleccionado",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Cambia el estatus en el que se encuentra el formulario.
        /// Cambios guardados o no.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="saved"></param>
        [SupportedOSPlatform("windows")]
        private void EditingForm(object sender, bool saved)
        {
            ChangeSavedStatus(saved);
        }

        #endregion Carga principal del diseñador

        #region Actualización del editor y generador para agregación de botones

        [SupportedOSPlatform("windows")]
        private void UpdateChangesControlsCustom()
        {
            InitializeWoSyntaxEditor();
            SaveLayout();

            UpdateScripts();

            SaveUnitProjects();
        }

        [SupportedOSPlatform("windows")]
        private void UpdateScripts()
        {
            string serverPath =
                $@"{proyecto.DirProyectTemp}\{_serverProjectName}\{_serverProjectName}.csproj";

            string wasmPath =
                $@"{proyecto.DirProyectTemp}\{_wasmProjectName}\{_wasmProjectName}.csproj";

            if (File.Exists(serverPath))
            {
                UpdateGenerated(
                    pathProject: $@"{proyecto.DirProyectTemp}\{_serverProjectName}",
                    projectName: _serverProjectName,
                    classModelName: _serverClassModelName
                );
            }

            if (File.Exists(wasmPath))
            {
                UpdateGenerated(
                    pathProject: $@"{proyecto.DirProyectTemp}\{_wasmProjectName}",
                    projectName: _wasmProjectName,
                    classModelName: _wasmClassModelName
                );
            }
        }

        [SupportedOSPlatform("windows")]
        private void UpdateGenerated(string pathProject, string projectName, string classModelName)
        {
            try
            {
                #region Update Form

                string modelName = _modelName;

                string pathDirectoryCodeSave =
                    $@"{proyecto.DirProyectData}\LayOuts\UserCode\{modelName}_proj";

                //WoContainer woContainer = _woFormDesginer.GetLastVersionGroup();
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                WoContainer woContainer = woProjectDataHelper.GetFullDesing(_modelName);

                if (!File.Exists($@"{pathDirectoryCodeSave}\{modelName}ScriptsUser.cs"))
                {
                    WoBlazorSave woBlazorSave = new WoBlazorSave(classModelName: modelName);

                    string pathContainer =
                        $@"{proyecto.DirProyectData}\LayOuts\FormDesign\{modelName}.json";

                    woBlazorSave.BuildBaseSave(woContainer);
                }

                WoTemplateScriptsUserBlazor woTemplateScriptsUserBlazor =
                    new WoTemplateScriptsUserBlazor();
                woTemplateScriptsUserBlazor.Project = projectName;

                WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                woSyntaxManagerUserCode.InitializeManager(
                    pathScript: $@"{pathDirectoryCodeSave}/{modelName}ScriptsUser.cs",
                    className: $@"{modelName}ScriptsUser",
                    modelName: modelName
                );

                List<(string type, string name)> parametersAux =
                    new List<(string type, string name)>();

                if (_isSlave) { }
                else
                {
                    woSyntaxManagerUserCode.DeleteConstructor($@"{modelName}ScriptsUser");

                    (List<(string type, string name)> parameters, string body) constructor =
                        BuildNewConstructorBody(modelName, classModelName);

                    woSyntaxManagerUserCode.CreateConstructor(
                        constructorName: $@"{modelName}ScriptsUser",
                        constructorBody: constructor.body,
                        constructorParams: constructor.parameters
                    );

                    StringBuilder strAtributes = new StringBuilder();

                    foreach ((string type, string name) parameter in constructor.parameters)
                    {
                        strAtributes.AppendLine(
                            $@"    public {parameter.type} {parameter.name} {{ get; set; }}"
                        );

                        woSyntaxManagerUserCode.DeleteBaseAttribute(name: $@"{parameter.name}", "");

                        if (
                            parameter.type != "IJSRuntime"
                            && parameter.type != $@"{modelName}Controls"
                        )
                        {
                            parametersAux.Add(parameter);
                        }
                    }

                    woSyntaxManagerUserCode.AddRawCode(strAtributes.ToString());
                }

                string rawClass = woSyntaxManagerUserCode
                    .GetClassCodeWithNewName(
                        oldName: $@"{modelName}ScriptsUser",
                        newName: $@"{classModelName}ScriptsUser"
                    )
                    .Replace($@"public {modelName}Controls", $@"public {classModelName}Controls")
                    .Replace(
                        $@"= {modelName}Controls.GetInstance();",
                        $@"= {classModelName}Controls.GetInstance();"
                    )
                    .Replace($@"{modelName}ScriptsUser", $@"{classModelName}ScriptsUser")
                    .Replace(
                        $@"public {modelName} {modelName} = new {modelName}();",
                        $@"public {woContainer.ModelId} {woContainer.ModelId} = new {woContainer.ModelId}();"
                    )
                    .Replace($@"({modelName}Controls", $@"({classModelName}Controls");

                int slaveCount = 0;
                foreach ((string type, string name) param in parametersAux)
                {
                    rawClass = rawClass.Replace(param.type, $@"Slave{slaveCount}SlaveControls");
                    rawClass = rawClass.Replace(param.name, $@"Slave{slaveCount}SlaveControls");
                }

                rawClass = rawClass.Replace($@"{modelName}Controls", $@"{classModelName}Controls");

                woTemplateScriptsUserBlazor.Code = WoSyntaxManagerHelper.ValidateRegions(rawClass);

                string usingsPath =
                    $@"{proyecto.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}Usings.cs";

                string usings = string.Empty;

                if (!File.Exists(usingsPath))
                {
                    fmUsingSelector fmUsingSelector = new fmUsingSelector(_modelName);
                }

                usings = WoDirectory
                    .ReadFile(usingsPath)
                    .Replace(
                        "using System.Private.CoreLib;",
                        $@"using {projectName}.ControlModels;"
                    );

                woTemplateScriptsUserBlazor.Usings = usings;

                string pathFile = $@"{pathProject}/UserCode/{classModelName}ScriptsUser.cs";

                WoDirectory.WriteTemplate(
                    pathTemplate: pathFile,
                    data: woTemplateScriptsUserBlazor.TransformText()
                );

                #endregion Update Form

                #region Update Grid

                string gridModelName = $@"{_modelName}GridList";

                string gridPathDirectoryCodeSave =
                    $@"{proyecto.DirProyectData}\LayOuts\UserCode\{gridModelName}_proj";

                #endregion Update Grid
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"No se pudieron escribir las plantillas en _proj/UserCode {ex.Message}"
                );
            }

            SaveUnitProjects();
        }

        private void UpdateForSlaves()
        {
            string modelName = _modelName;

            string pathDirectoryCodeSave =
                $@"{proyecto.DirProyectData}\LayOuts\UserCode\{modelName}_proj";

            if (File.Exists($@"{pathDirectoryCodeSave}\{modelName}ScriptsUser.cs"))
            {
                WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                woSyntaxManagerUserCode.InitializeManager(
                    pathScript: $@"{pathDirectoryCodeSave}/{modelName}ScriptsUser.cs",
                    className: $@"{modelName}ScriptsUser",
                    modelName: modelName
                );

                List<(string type, string name)> parametersAux =
                    new List<(string type, string name)>();

                if (!_isSlave)
                {
                    woSyntaxManagerUserCode.DeleteConstructor($@"{modelName}ScriptsUser");

                    (List<(string type, string name)> parameters, string body) constructor =
                        BuildNewConstructorBody(modelName, "");

                    woSyntaxManagerUserCode.CreateConstructor(
                        constructorName: $@"{modelName}ScriptsUser",
                        constructorBody: constructor.body,
                        constructorParams: constructor.parameters
                    );

                    StringBuilder strAtributes = new StringBuilder();

                    foreach ((string type, string name) parameter in constructor.parameters)
                    {
                        strAtributes.AppendLine(
                            $@"    public {parameter.type} {parameter.name} {{ get; set; }}"
                        );

                        woSyntaxManagerUserCode.DeleteBaseAttribute(name: $@"{parameter.name}", "");

                        if (
                            parameter.type != "IJSRuntime"
                            && parameter.type != $@"{modelName}Controls"
                        )
                        {
                            parametersAux.Add(parameter);
                        }
                    }

                    woSyntaxManagerUserCode.AddRawCode(strAtributes.ToString());

                    string rawClass = WoDirectory.ReadFile(
                        $@"{pathDirectoryCodeSave}\{modelName}ScriptsUser.cs"
                    );
                    //rawClass = rawClass
                    //    .Replace($@"#region Constructor", "")
                    //    .Replace($@"#endregion Constructor", "")
                    //    .Replace($@"#region", $@"//")
                    //    .Replace($@"#endregion", $@"//");

                    if (File.Exists($@"{pathDirectoryCodeSave}\{modelName}ScriptsUser.cs"))
                    {
                        WoDirectory.DeleteFile(
                            $@"{pathDirectoryCodeSave}\{modelName}ScriptsUser.cs"
                        );
                    }

                    WoDirectory.WriteFile(
                        $@"{pathDirectoryCodeSave}\{modelName}ScriptsUser.cs",
                        rawClass
                    );
                }
            }
        }

        /// <summary>
        /// Construye el nuevo constructor con todas las propiedades de controles
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        private (List<(string type, string name)> parameters, string body) BuildNewConstructorBody(
            string modelName,
            string clasModelName
        )
        {
            List<(string type, string name)> parameters = new List<(string type, string name)>();
            StringBuilder constructor = new StringBuilder();

            parameters.Add(("IJSRuntime", "JS"));
            parameters.Add(($"{modelName}Controls", $"{modelName}Controles"));

            constructor.AppendLine(
                $@"
        this.{modelName}Controles = {modelName}Controles;
        this.JS = JS;"
            );

            Modelo findModel = proyecto.ModeloCol.Modelos.FirstOrDefault(m => m.Id == modelName);
            if (findModel != null)
            {
                foreach (ModeloColumna column in findModel.Columnas)
                {
                    if (column.TipoControl == WoTypeControl.CollectionEditor)
                    {
                        constructor.Append(
                            $@"this.{column.Id.Replace("Col", "")}Controles = {column.Id.Replace("Col", "")}Controles;"
                        );
                        parameters.Add(
                            (
                                $"{column.Id.Replace("Col", "")}SlaveControls",
                                $"{column.Id.Replace("Col", "")}Controles"
                            )
                        );
                    }
                }
            }

            return (parameters, constructor.ToString());
        }

        #endregion Actualización del editor y generador para agregación de botones


        #region Cambio a modo de edición del formulario

        /// <summary>
        /// Cambia a modo de edición el diseñador en la pantalla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnEditar_ItemClick(object sender, ItemClickEventArgs e)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabFormDesigner")
            {
                _woFormDesginer.DesingMode();
            }
            else if (selectedTabName == "TabGridDesigner")
            {
                _woGridDesigner.DesingMode();
                _woGridDesigner.DisableForSlaveOptions();
            }
            else if (selectedTabName == "TabListDesigner")
            {
                _listDesigner.DesingMode();
            }
        }

        #endregion Cambio a modo de edición del formulario

        #region Salvado del formulario

        /// <summary>
        /// Salva el formulario del diseñador y pone el diseñador en modo view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnGuardarSalir_ItemClick(object sender, ItemClickEventArgs e)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabFormDesigner")
            {
                SaveLayout();
                _woFormDesginer.BlockMode();
                EnableCodeGroup();

                //SaveUnitProjects();
            }
            else if (selectedTabName == "TabGridDesigner")
            {
                SaveGrid();
                _woGridDesigner.BlockMode();
            }
            else if (selectedTabName == "TabListDesigner")
            {
                _isSaved = true;
                _listDesigner.SerializeGridToJson();

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Lista");
                if (tab != null)
                {
                    tab.Text = "Lista";
                }

                //SaveLayout();
                _listDesigner.BlockMode();
            }

            SaveUnitProjects();

            if (!CanExecute())
            {
                StringBuilder nesesaryControls = new StringBuilder();
                foreach (string item in _listOfNesesaryItems)
                {
                    nesesaryControls.AppendLine(item);
                }

                if (_listOfNesesaryItems.Count > 0)
                {
                    XtraMessageBox.Show(
                        text: $@"Faltan los siguientes controles para el correcto funcionamiento del formulario: {nesesaryControls}",
                        caption: "No es posible ejecutar el proyecto sin los componentes base",
                        buttons: MessageBoxButtons.OK,
                        icon: MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Salva el formulario del diseñador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnSalvarLayout_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (
                (!cmbExecute.EditValue.ToString().Contains("Watch"))
                && (_woBlazorServerExecute.IsRuning() || _woBlazorWasmExecute.IsRuning())
            )
            {
                btnStop_ItemClick(null, null);
            }

            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabFormDesigner")
            {
                SaveLayout();

                EnableCodeGroup();

                SaveUnitProjects();
            }
            else if (selectedTabName == "TabGridDesigner")
            {
                SaveGrid();
            }
            else if (selectedTabName == "TabListDesigner")
            {
                _listDesigner.SerializeGridToJson();
            }

            SaveUnitProjects();
        }

        /// <summary>
        /// Seria liza la grid
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SaveGrid()
        {
            _woGridDesigner.SerializeGridToJson();

            var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Esclava");
            if (tab != null)
            {
                tab.Text = "Esclava";
            }

            _isSaved = true;
        }

        /// <summary>
        /// Método base de salvado del diseñador.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SaveLayout()
        {
            if (
                _modelSelected.TipoModelo != WoTypeModel.Kardex
                || _modelSelected.TipoModelo != WoTypeModel.Control
                || _modelSelected.TipoModelo != WoTypeModel.Configuration
                || _modelSelected.TipoModelo != WoTypeModel.View
            )
            {
                BuildBaseSave();

                InitializeWoSyntaxEditor();

                UpdateStartForm();

                SaveSlaves();
            }

            /// Genera la actualización del formulario desde el botón de salvado
            string serverPath =
                $@"{proyecto.DirProyectTemp}\{_serverProjectName}\{_serverProjectName}.csproj";
            string wasmPath =
                $@"{proyecto.DirProyectTemp}\{_wasmProjectName}\{_wasmProjectName}.csproj";

            WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();

            if (File.Exists(serverPath) && !_isSlave)
            {
                if (
                    _modelSelected.TipoModelo == WoTypeModel.Kardex
                    || _modelSelected.TipoModelo == WoTypeModel.Control
                    || _modelSelected.TipoModelo == WoTypeModel.Configuration
                    || _modelSelected.TipoModelo == WoTypeModel.View
                )
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        container: new WoContainer()
                        {
                            Proceso = _modelSelected.ProcesoId,
                            ModelId = _modelSelected.Id,
                            ModelType = _modelSelected.TipoModelo,
                            SubType = _modelSelected.SubTipoModelo,
                            Id = _modelSelected.Id,
                            TypeContainer = eTypeContainer.None
                        },
                        _serverClassModelName,
                        isServer: true
                    );
                }
                else
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        container: _woFormDesginer.GetLastVersionGroup(),
                        _serverClassModelName,
                        isServer: true
                    );
                }
            }
            if (File.Exists(wasmPath) && !_isSlave)
            {
                if (
                    _modelSelected.TipoModelo == WoTypeModel.Kardex
                    || _modelSelected.TipoModelo == WoTypeModel.Control
                    || _modelSelected.TipoModelo == WoTypeModel.Configuration
                    || _modelSelected.TipoModelo == WoTypeModel.View
                )
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        container: new WoContainer()
                        {
                            Proceso = _modelSelected.ProcesoId,
                            ModelId = _modelSelected.Id,
                            ModelType = _modelSelected.TipoModelo,
                            SubType = _modelSelected.SubTipoModelo,
                            Id = _modelSelected.Id,
                            TypeContainer = eTypeContainer.None
                        },
                        _wasmClassModelName,
                        isServer: false
                    );
                }
                else
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        container: _woFormDesginer.GetLastVersionGroup(),
                        _wasmClassModelName,
                        isServer: false
                    );
                }
            }

            _woFormDesginer.SerializeFormToJson();

            DataRow drModelSelected = grdModelosView.GetFocusedDataRow();
            if (drModelSelected != null)
            {
                drModelSelected[@"Diseñado"] = true;
            }

            ChangeSavedStatus(true);
        }

        /// <summary>
        /// Instancia del generador de todo el fichero de save el layouts.
        /// </summary>
        private WoBlazorSave _woBlazorSave = null;

        /// <summary>
        /// Genera los ficheros base para el código del usuario en caso de no existir.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void BuildBaseSave()
        {
            _woBlazorSave = new WoBlazorSave(classModelName: _modelName);
            _woBlazorSave.BuildBaseSave(_woFormDesginer.GetLastVersionGroup());
        }

        #endregion Salvado del formulario

        #region Salvado de la slaves para cuando se guarda una maestra

        /// <summary>
        /// Guarda el diseño en raw de las slaves para que las modificaciones funcionen correctamente.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SaveSlaves()
        {
            Modelo findModel = proyecto.ModeloCol.Modelos.FirstOrDefault(m => m.Id == _modelName);
            if (findModel != null)
            {
                WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                    new WoDesignerRawSerializerHelper();

                foreach (ModeloColumna column in findModel.Columnas)
                {
                    if (column.TipoControl == WoTypeControl.CollectionEditor)
                    {
                        string pathLayout =
                            $@"{proyecto.DirProyectData}\LayOuts\FormDesign\{column.ModeloId}.json";

                        if (!File.Exists(pathLayout))
                        {
                            WoContainer container =
                                woDesignerRawSerializerHelper.BuildRawWoContainer(column.ModeloId);

                            WoBlazorSave woBlazorSave = new WoBlazorSave(
                                classModelName: column.ModeloId
                            );
                            woBlazorSave.BuildBaseSave(container);

                            WoDirectory.WriteFile(
                                pathLayout,
                                JsonConvert.SerializeObject(container)
                            );

                            int row = grdModelosView.LocateByValue(
                                @"Modelo",
                                column.Id.Replace("Col", "")
                            );

                            DataRow drModelSelected = grdModelosView.GetDataRow(row);
                            if (drModelSelected != null)
                            {
                                drModelSelected[@"Diseñado"] = true;
                                grdModelos.Refresh();
                            }
                        }
                    }
                }
            }
        }

        #endregion Salvado de la slaves para cuando se guarda una maestra

        #region Actualizar unitaria

        /// <summary>
        /// Actualiza los formularios de ambos approach de unitarias
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SaveUnitProjects()
        {
            try
            {
                if (_modelSelected.ProcesoId == "")
                {
                    UpdateProjectExtencion();
                }
                else
                {
                    UpdateProjectNoExtencion();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Actualizar unitaria

        #region Actualización de los proyectos de blazor, cuando hay extensiones

        /// <summary>
        /// Actualiza los proyectos de unitarias con el modelo base y las exenciones.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void UpdateProjectExtencion()
        {
            try
            {
                string serverPath =
                    $@"{proyecto.DirProyectTemp}\{_serverProjectName}\{_serverProjectName}.csproj";
                string wasmPath =
                    $@"{proyecto.DirProyectTemp}\{_wasmProjectName}\{_wasmProjectName}.csproj";

                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

                WoContainer mainContainer = woProjectDataHelper.GetFullDesing(_modelName);

                WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();

                if (File.Exists(serverPath) && !_isSlave)
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        isServer: true,
                        container: mainContainer,
                        classModelName: _serverClassModelName
                    );
                }

                if (File.Exists(wasmPath) && !_isSlave)
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        isServer: false,
                        container: mainContainer,
                        classModelName: _wasmClassModelName
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al actualizar las generaciones unitarias. {ex.Message}"
                );
            }
        }

        #endregion Actualización de los proyectos de blazor, cuando hay extensiones

        #region Actualización de los proyectos de blazor unitarios

        /// <summary>
        /// Actualiza los proyectos de blazor para cuando no se utilizan exenciones
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void UpdateProjectNoExtencion()
        {
            try
            {
                string serverPath =
                    $@"{proyecto.DirProyectTemp}\{_serverProjectName}\{_serverProjectName}.csproj";
                string wasmPath =
                    $@"{proyecto.DirProyectTemp}\{_wasmProjectName}\{_wasmProjectName}.csproj";

                WoContainer woContainer = new WoContainer();
                if (
                    _modelSelected.TipoModelo == WoTypeModel.Kardex
                    || _modelSelected.TipoModelo == WoTypeModel.Control
                    || _modelSelected.TipoModelo == WoTypeModel.Configuration
                    || _modelSelected.TipoModelo == WoTypeModel.View
                )
                {
                    WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
                    woContainer = woDesignerRawListHelper.BuildRawListForm(_modelName);
                }
                else
                {
                    woContainer = _woFormDesginer.GetLastVersionGroup();
                }

                WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();

                if (File.Exists(serverPath) && !_isSlave)
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        isServer: true,
                        container: woContainer,
                        classModelName: _serverClassModelName
                    );
                }

                if (File.Exists(wasmPath) && !_isSlave)
                {
                    woBlazorGenerator.GenerateUnitLayout(
                        isServer: false,
                        container: woContainer,
                        classModelName: _wasmClassModelName
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al actualizar los proyectos de blazor. {ex.Message}");
            }
        }

        #endregion Actualización de los proyectos de blazor unitarios


        #region Restauración del layout

        /// <summary>
        /// Restaura el layout y deja el diseñador en modo de diseño.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnRestaurarLayout_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            _isSaved = false;
            _restart = true;

            //if (_woGridDesigner != null)
            //{
            //    _woGridDesigner.BlockMode();
            //    TabGridDesigner.PageVisible = false;
            //    _woGridDesigner.Dispose();
            //    _woGridDesigner = null;

            //    TabGridDesigner.PageVisible = true;

            //    _woGridDesigner = new WoGridDesigner(saveFolder: "Grids", modelName: _modelName);
            //    _woGridDesigner.Dock = DockStyle.Fill;
            //    pnlGridDesigner.Controls.Add(_woGridDesigner);
            //    _woGridDesigner.ModeChanged += ChangeDesignMode;
            //    _woGridDesigner.AddButtonEvt += InitializeWoGridSyntaxEditor;
            //    _woGridDesigner.DeleteButtonEvt += InitializeWoGridSyntaxEditor;

            //    _woGridDesigner.GridUpdatingEvt += UpdatingGrid;
            //}

            //_woFormDesginer.InitializeDesigner();
            //_woFormDesginer.DesingMode();

            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabFormDesigner")
            {
                _isSaved = true;
                _woFormDesginer.InitializeDesigner();
                _woFormDesginer.DesingMode();

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Diseñador");
                if (tab != null)
                {
                    tab.Text = "Diseñador";
                }
            }
            else if (selectedTabName == "TabGridDesigner")
            {
                _isSaved = true;
                _woGridDesigner.InitializeGrid();
                _woGridDesigner.DesingMode();

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Esclava");
                if (tab != null)
                {
                    tab.Text = "Esclava";
                }
            }
            else if (selectedTabName == "TabListDesigner")
            {
                _isSaved = true;
                _listDesigner.InitializeGrid();
                _listDesigner.DesingMode();

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Lista");
                if (tab != null)
                {
                    tab.Text = "Lista";
                }
            }
        }

        /// <summary>
        /// Restaura el diseñador y cambia el diseñador a modo de view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnSalirSinGuardar_ItemClick(object sender, ItemClickEventArgs e)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabFormDesigner")
            {
                _isSaved = true;
                _woFormDesginer.InitializeDesigner();
                _woFormDesginer.BlockMode();

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Diseñador");
                if (tab != null)
                {
                    tab.Text = "Diseñador";
                }
            }
            else if (selectedTabName == "TabGridDesigner")
            {
                _isSaved = true;
                _woGridDesigner.InitializeGrid();
                _woGridDesigner.BlockMode();

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Esclava");
                if (tab != null)
                {
                    tab.Text = "Esclava";
                }
            }
            else if (selectedTabName == "TabListDesigner")
            {
                _isSaved = true;
                _listDesigner.InitializeGrid();
                _listDesigner.BlockMode();

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Lista");
                if (tab != null)
                {
                    tab.Text = "Lista";
                }
            }
        }

        /// <summary>
        /// Modifica la bandera de salvado para cuando se modifica algo en la grid
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void UpdatingGrid()
        {
            string selectedPage = tabBaseDesigner.SelectedTabPage.Text;

            if (selectedPage == "Lista")
            {
                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "Lista");
                if (tab != null)
                {
                    tab.Text = "*Lista";
                }
            }
            else if (selectedPage == "Esclava")
            {
                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "Esclava");
                if (tab != null)
                {
                    tab.Text = "*Esclava";
                }
            }

            _isSaved = false;
        }

        #endregion Restauración del layout

        #region Eventos de los botones del grupo Grupos
        [SupportedOSPlatform("windows")]
        private void btnAgregarGrupo_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            _woFormDesginer.WoGroupFactory.ColumnSpan = 4;
            _woFormDesginer.WoGroupFactory.RowSpan = 2;
            _woFormDesginer.WoGroupFactory.ColumnIndex = 0;
            _woFormDesginer.WoGroupFactory.RowIndex = 0;
            _woFormDesginer.WoGroupFactory.InternalRows = 2;
            _woFormDesginer.CreateGroup();

            ChangeSavedStatus(false);
        }

        [SupportedOSPlatform("windows")]
        private void btnAgregarGrupoTabs_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            _woFormDesginer.WoTabGroupFactory.ColumnSpan = 4;
            _woFormDesginer.WoTabGroupFactory.RowSpan = 2;
            _woFormDesginer.WoTabGroupFactory.ColumnIndex = 0;
            _woFormDesginer.WoTabGroupFactory.RowIndex = 0;
            _woFormDesginer.CreateTabGroup(internalRows: 2);

            ChangeSavedStatus(false);
        }

        #endregion Eventos de los botones del grupo Grupos

        #region Actualización del método de inicio

        /// <summary>
        /// Permite actualizar el método de inicio del formulario con las
        /// ultimas modificaciones del diseño del formulario para el inicio.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void UpdateStartForm()
        {
            WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();

            woSyntaxManagerUserCode.InitializeManager(
                pathScript: $@"{proyecto.DirProyectData}/LayOuts/UserCode/{_modelName}_proj/{_modelName}ScriptsUser.cs",
                className: _modelName,
                modelName: _modelName
            );

            WoScriptsUserGenerator _woScriptsUserGenerator = new WoScriptsUserGenerator(
                classModelName: _modelName
            );

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

            //woSyntaxManagerUserCode.UpdateBodyMethod(
            //    methodName: "FormSettings",
            //    bodyMethod: _woScriptsUserGenerator.GetMainConfig(
            //        baseContainer: _woFormDesginer.GetLastVersionGroup()
            //    )
            //);
            woSyntaxManagerUserCode.UpdateBodyMethod(
                methodName: "FormSettings",
                bodyMethod: _woScriptsUserGenerator.GetMainConfig(
                    baseContainer: woProjectDataHelper.GetFullDesing(_modelName)
                )
            );
        }

        #endregion Actualización del método de inicio


        #region Syntax Editor

        #region Estatus de salvado del código

        /// <summary>
        /// Bandera indicadora para cuando el código del usuario no se a salvado.
        /// </summary>
        private bool _isSavedCode = true;

        /// <summary>
        /// Método suscrito al controlador de eventos del syntax para cuando se
        /// cambia el código.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void EditCode()
        {
            _isSavedCode = false;

            string page = tabBaseDesigner.SelectedTabPage.Name;

            if (page == "TabCodeEditor")
            {
                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "Código");
                if (tab != null)
                {
                    tab.Text = "*Código";
                }
            }
            else if (page == "TabListCodeEditor")
            {
                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                    tp.Text == "Código de lista"
                );
                if (tab != null)
                {
                    tab.Text = "*Código de lista";
                }
            }
        }

        #endregion Estatus de salvado del código

        #region Inicializar SyntaxEditor

        /// <summary>
        /// Instancia del syntax editor encapsulado en un componente de WinForms.
        /// </summary>
        private WoSyntaxEditor _woSyntaxEditor = null;

        /// <summary>
        /// Instancia de la grid de métodos.
        /// </summary>
        private WoTreeMethods _treeMethods = null;

        /// <summary>
        /// Nombre del evento seleccionado.
        /// </summary>
        private string _selectedEventName = string.Empty;

        /// <summary>
        /// Control seleccionado del evento.
        /// </summary>
        private string _selectedControlName = string.Empty;

        /// <summary>
        /// Tipo de evento seleccionado.
        /// </summary>
        private string _selectedType = string.Empty;

        /// <summary>
        /// Inicializa el syntax editor para el modelo seleccionado.
        /// Verifica si ya hay una instancia y la reemplaza.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeWoSyntaxEditor()
        {
            string pathProject = $@"{proyecto.DirProyectData}\LayOuts\UserCode\{_modelName}_proj";
            if (File.Exists($@"{pathProject}\{_modelName}ScriptsUser.cs"))
            {
                if (!_woSyntaxEditor.IsNull())
                {
                    //_woSyntaxEditor.Dispose();
                    pnlSyntaxEditor.Controls.Clear();
                    TabCodeEditor.PageVisible = false;
                }

                TabCodeEditor.PageVisible = true;
                _woSyntaxEditor = new WoSyntaxEditor(
                    pathDirSaveProject: pathProject,
                    modelName: _modelName,
                    classModelName: _modelName,
                    methodParamsCol: _methodParamsCol
                );

                _woSyntaxEditor.CodeChangedEvt += EditCode;

                _woSyntaxEditor.Parent = pnlSyntaxEditor;
                _woSyntaxEditor.Dock = DockStyle.Fill;

                _woFormDesginer.AddItemEvt += ItemAdded;
                _woFormDesginer.RemoveItemEvt += ItemRemoved;

                InitializeTreeListMethods();

                InitializeSnippetPanel();
            }
            else
            {
                ///todo: Send trow: base files not ready
            }
        }

        /// <summary>
        /// Método que se ocupa de inicializar el árbol de métodos y cargarlo.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeTreeListMethods()
        {
            if (_treeMethods != null)
            {
                pnlTreeMethods.Controls.Clear();
                _treeMethods = null;
            }

            _treeMethods = new WoTreeMethods(_modelName);
            _treeMethods.Parent = pnlTreeMethods;
            _treeMethods.Dock = DockStyle.Fill;

            _treeMethods.ChangeSeleccionEvt += ChangeFocusedMethod;

            _woFormDesginer.UpdateIdEvt += _treeMethods.UpdateEvents;

            InitializeMethods();
        }

        #endregion Inicializar SyntaxEditor

        #region Inicializar grid de métodos

        /// <summary>
        /// Cargo los métodos al árbol de métodos.
        /// ToDo: cambiar el tipo de método por un enum.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeMethods()
        {
            bool correctCharge = false;

            do
            {
                try
                {
                    _treeMethods.AddMethod(
                        control: "Formulario",
                        type: "Constructor",
                        method: "Validaciones Fluent",
                        haveCode: _woSyntaxEditor.HaveCode(
                            methodName: $@"{_modelName}Validator",
                            codeType: eCodeType.Constructor
                        ),
                        status: true
                    );

                    correctCharge = true;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $@"Carga incorrecta de métodos Formulario. {ex.Message}",
                        "Alert"
                    );
                }
            } while (!correctCharge);

            ChargeNormalMethods();

            ChargeCustomMethods();

            _treeMethods.MethodsReady();

            ValidateUseCustomMethods();
        }

        [SupportedOSPlatform("windows")]
        private void ChargeNormalMethods()
        {
            List<(string type, string method, bool haveCode)> methods = _woSyntaxEditor
                .GetMethodsInfo(eCodeType.Method)
                .ToList();

            for (int i = 0; i < methods.Count; i++)
            {
                string[] methodNameCol = methods[i].method.ToString().Split('_');
                string controlName = string.Empty;
                if (methodNameCol.Length <= 2)
                {
                    controlName = methodNameCol[0];
                }
                else
                {
                    for (int j = 0; j < methodNameCol.Length - 1; j++)
                    {
                        controlName += methodNameCol[j] + "_";
                    }
                    controlName = controlName.Substring(0, controlName.Length - 1);
                }

                bool active =
                    (controlName == "FormularioIniciado")
                        ? true
                        : _woFormDesginer.FormItemsCol.Contains(controlName);

                bool correctCharge = false;

                do
                {
                    try
                    {
                        if (_treeMethods != null)
                        {
                            if (methods[i].method.ToLower() != "getinstance")
                            {
                                string type = methods[i].type;
                                string method = methods[i].method;
                                bool haveCode = methods[i].haveCode;

                                _treeMethods.AddMethod(
                                    control: controlName,
                                    type: type,
                                    method: method,
                                    haveCode: haveCode,
                                    status: active
                                );
                            }

                            correctCharge = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(
                            $@"Carga incorrecta de métodos {controlName}, {methods[i].type}, {methods[i].method}, {methods[i].haveCode}, {active}",
                            "Alert"
                        );
                    }
                } while (!correctCharge);
            }
        }

        [SupportedOSPlatform("windows")]
        private void ChargeCustomMethods()
        {
            List<(string type, string method, bool haveCode)> customMethods =
                _woSyntaxEditor.GetMethodsInfo(eCodeType.CustomMethod);

            foreach ((string type, string method, bool haveCode) method in customMethods)
            {
                _treeMethods.AddMethod(
                    control: "Métodos Custom",
                    type: method.type,
                    method: method.method,
                    haveCode: method.haveCode,
                    status: false
                );
            }
        }

        /// <summary>
        /// Valida los métodos usados a través del syntax y lo pasa al árbol de métodos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ValidateUseCustomMethods()
        {
            List<(string method, bool used)> methodsUsed =
                _woSyntaxEditor.ValidateUseCustomMethods();
            _treeMethods.UpdateMethodsUsed(methodsUsed);
        }

        #endregion Inicializar grid de métodos

        #region Initialize snippets panel

        /// <summary>
        /// Instancia del panel de selección de snippets para el syntax editor.
        /// </summary>
        private WoSnipetSelector _woSnipetSelector = null;

        /// <summary>
        /// Inicializa el panel de sección de snippets.
        /// Limpia el panel donde se encontrara en caso de que ya exista una instancia anterior
        /// la instancia y la agrega al panel.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeSnippetPanel()
        {
            pnlSnipets.Controls.Clear();

            _woSnipetSelector = new WoSnipetSelector(_modelName);
            _woSnipetSelector.Dock = DockStyle.Fill;
            _woSnipetSelector.SetSnipetEvt += SetSelectedSnipet;

            pnlSnipets.Controls.Add(_woSnipetSelector);
        }

        /// <summary>
        /// Pasa el snippet recuperado desde el panel a través del action.
        /// </summary>
        /// <param name="snipet"></param>
        [SupportedOSPlatform("windows")]
        private void SetSelectedSnipet(string snipet)
        {
            _woSyntaxEditor.SetSnipet(snipet);
        }

        #endregion Initialize snippets panel

        #region Items añadidos de los controles ocultos

        /// <summary>
        /// Método suscrito al evento cuando se agrega un item nuevo al formulario.
        /// Actualiza el estatus de activo en la grid con los métodos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        [SupportedOSPlatform("windows")]
        private void ItemAdded(object sender, string control)
        {
            if (control != "Item ")
            {
                AddMehods(control);
                _treeMethods.UpdateStatus(control, true);
            }
        }

        private void AddMehods(string control)
        {
            WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
            woSyntaxManagerUserCode.InitializeManager(
                pathScript: $@"{proyecto.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs",
                className: $@"{_modelName}",
                modelName: _modelName
            );

            string code = WoDirectory.ReadFile(
                $@"{proyecto.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs"
            );

            if (!code.Contains($@"{control}_OnChange()"))
            {
                woSyntaxManagerUserCode.CreateNewMethod(
                    methodName: $@"{control}_OnChange",
                    bodyMethod: $@"",
                    typeMethod: $@"void"
                );
            }

            if (!code.Contains($@"{control}_OnFocus()"))
            {
                woSyntaxManagerUserCode.CreateNewMethod(
                    methodName: $@"{control}_OnFocus",
                    bodyMethod: $@"",
                    typeMethod: $@"void"
                );
            }

            if (!code.Contains($@"{control}_OnBlur()"))
            {
                woSyntaxManagerUserCode.CreateNewMethod(
                    methodName: $@"{control}_OnBlur",
                    bodyMethod: $@"",
                    typeMethod: $@"void"
                );
            }
        }

        /// <summary>
        /// Método suscrito al evento cuando se agrega un item nuevo al formulario.
        /// actualiza el estatus de activo en la grid con los métodos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="control"></param>
        [SupportedOSPlatform("windows")]
        private void ItemRemoved(object sender, string control)
        {
            _treeMethods.UpdateStatus(control, false);
        }

        #endregion Items añadidos de los controles ocultos

        #region Selección de método

        /// <summary>
        /// Bandera para cuando se regresa al ultimo método seleccionado.
        /// </summary>
        private bool _reloadMethod = false;

        /// <summary>
        /// Método suscrito al evento de
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="control"></param>
        [SupportedOSPlatform("windows")]
        private void ChangeFocusedMethod(string eventName, string control, string type)
        {
            if (!_reloadMethod)
            {
                DialogResult result = DialogResult.Yes;

                if (!_isSavedCode)
                {
                    result = XtraMessageBox.Show(
                        "Se perderá el código no salvado. ¿Está seguro de querer salir?",
                        "Alerta",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                }

                if (result == DialogResult.Yes)
                {
                    var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Código");
                    if (tab != null)
                    {
                        tab.Text = "Código";
                    }

                    _selectedEventName = eventName;
                    _selectedControlName = control;
                    _selectedType = type;

                    if (_woSyntaxEditor != null)
                    {
                        if (eventName.IsNull())
                        {
                            _woSyntaxEditor.CleanEditor();
                        }
                        else
                        {
                            if (eventName == "Validaciones Fluent")
                            {
                                _woSyntaxEditor.ChargeCode(
                                    methodType: string.Empty,
                                    methodName: $@"{_modelName}Validator",
                                    typeOfCode: eCodeType.Constructor
                                );
                            }
                            else if (control == @"Métodos Custom")
                            {
                                _woSyntaxEditor.ChargeCode(
                                    methodType: type,
                                    methodName: eventName,
                                    typeOfCode: eCodeType.CustomMethod
                                );
                            }
                            else
                            {
                                _woSyntaxEditor.ChargeCode(
                                    methodType: type,
                                    methodName: eventName,
                                    typeOfCode: eCodeType.Method
                                );
                            }
                        }
                    }
                }
                else
                {
                    _reloadMethod = true;
                    _treeMethods.SetLastMethod();
                }
            }
            else
            {
                _reloadMethod = false;
            }
        }

        #endregion Selección de método

        #region Salvar código

        /// <summary>
        /// Instancia de la clase que permite actualizar los ficheros de código al aver modificaciones.
        /// </summary>
        private WoSyntaxEditorFiles _woSyntaxEditorFiles;

        /// <summary>
        /// Salva el código.
        ///     - Valida si es un nuevo método para agregarlo a la grid con el resto de métodos.
        ///     - En caso de no se un método nuevo, lo escribe con un método del syntax.
        ///     - Si existe el proyecto generado utiliza la clase WoSyntaxEditorFiles para actualizar el proyecto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnSalvarCodigo_ItemClick(object sender, ItemClickEventArgs e)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabCodeEditor")
            {
                SaveCode();
            }
            else if (selectedTabName == "TabListCodeEditor")
            {
                SaveCustomCode();
            }
        }

        /// <summary>
        /// Salva el código.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SaveCode()
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();

            if (_woSyntaxEditor.SaveCode())
            {
                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Código");
                if (tab != null)
                {
                    tab.Text = "Código";
                }
            }

            _treeMethods.UpdateHaveCode(_woSyntaxEditor.HaveEditorCode());

            List<(string method, bool used)> methodsUsed =
                _woSyntaxEditor.ValidateUseCustomMethods();
            _treeMethods.UpdateMethodsUsed(methodsUsed);

            _woSyntaxEditorFiles = new WoSyntaxEditorFiles(
                modelName: _modelName,
                unitClassModelNameServer: _serverClassModelName,
                unitClassModelNameWasm: _wasmClassModelName
            );
            _woSyntaxEditorFiles.UpdateBlazorProject();

            _isSavedCode = true;

            if (
                _modelSelected.TipoModelo == WoTypeModel.CatalogSlave
                || _modelSelected.TipoModelo == WoTypeModel.TransactionSlave
                || _modelSelected.TipoModelo == WoTypeModel.Request
            )
            {
                btnExecute.Enabled = false;
            }
        }

        #endregion Salvar código


        #region Métodos custom

        #region Base nuevo método

        /// <summary>
        /// Tipo del método que se esta creando.
        /// </summary>
        private string _newMethodType = string.Empty;

        /// <summary>
        /// Nombre del método que ingresa el usuario desde el dialog.
        /// </summary>
        private string _newMethodName = string.Empty;

        /// <summary>
        /// Instancia el dialog para que el usuario ingrese información del nuevo método y
        /// suscribe un método para cuando el formulario se cierre y obtener la información que
        /// el usuario ingreso e igual dispara el dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAddMethod_ItemClick(object sender, ItemClickEventArgs e)
        {
            _newMethodDialog = new fmNewMethod();
            _newMethodParamsDialog = new fmNewMethodParams();

            _newMethodDialog.MethodReadyEvt += MethodReady;
            _newMethodDialog.AddParamsEvt += AddParams;
            _newMethodDialog.CancelEvt += CancelMethod;

            _newMethodParamsDialog.ParamsReadyEvt += ParamsReady;
            _newMethodParamsDialog.ContinueEvt += ContinueCreatingMethod;
            _newMethodParamsDialog.CancelEvt += CancelMethod;

            _newMethodDialog.ShowDialog();
        }

        #endregion Base nuevo método

        #region Método

        /// <summary>
        /// Instancia del dialog para que el usuario ingrese el nombre del método que busca crear.
        /// </summary>
        private fmNewMethod _newMethodDialog;

        /// <summary>
        /// Método que se suscribe al controlador de eventos del dialog para detonarse desde este cuando se
        /// da click en la opción de aceptar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void MethodReady(object sender, (string methodType, string methodName) newMethod)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabCodeEditor")
            {
                if (
                    _woSyntaxEditor.AlreadyExistMethod(newMethod.methodName)
                    || _woSyntaxEditor.AlreadyExistAttribute(newMethod.methodName)
                )
                {
                    XtraMessageBox.Show(
                        "El método que esta intentando crear ya existe",
                        "Alerta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
                }
                else
                {
                    _newMethodType = newMethod.methodType;
                    _newMethodName = newMethod.methodName;

                    _woSyntaxEditor.CreateNewMethod(_newMethodType, _newMethodName);

                    InitializeTreeListMethods();
                }
            }
            else if (selectedTabName == "TabListCodeEditor")
            {
                if (
                    _woGridSyntaxEditor.AlreadyExistMethod(newMethod.methodName)
                    || _woGridSyntaxEditor.AlreadyExistAttribute(newMethod.methodName)
                )
                {
                    XtraMessageBox.Show(
                        "El método que esta intentando crear ya existe",
                        "Alerta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
                }
                else
                {
                    _newMethodType = newMethod.methodType;
                    _newMethodName = newMethod.methodName;

                    _woGridSyntaxEditor.CreateNewMethod(_newMethodType, _newMethodName);

                    InitializeGridListMethods();
                }
            }
        }

        /// <summary>
        /// Método que se suscribe al controlador de eventos del dialog para detonarse cuando se
        /// da click en la opción para agregar parámetros al método que se esta creando.
        /// </summary>
        /// <param name="sender"></param>
        [SupportedOSPlatform("windows")]
        private void AddParams(object sender, (string methodType, string methodName) newMethod)
        {
            if (_woSyntaxEditor.AlreadyExistMethod(newMethod.methodName))
            {
                XtraMessageBox.Show(
                    "El método que esta intentando crear ya existe",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            else
            {
                _newMethodType = newMethod.methodType;
                _newMethodName = newMethod.methodName;

                _newMethodParamsDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Método que se suscribe al controlador de eventos del dialog para detonarse cuando se
        /// da click en la opción para cancelar la creación, reinicia todas las variables.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void CancelMethod(object sender, EventArgs e)
        {
            _newMethodType = string.Empty;
            _newMethodName = string.Empty;
        }

        #endregion Método

        #region Parámetros

        /// <summary>
        /// Instancia del dialog que permite agregar parámetros al nuevo método.
        /// </summary>
        private fmNewMethodParams _newMethodParamsDialog;

        /// <summary>
        /// Lista de parámetros del nuevo método.
        /// </summary>
        private List<(string type, string name, string value)> _methodParamsCol = null;

        /// <summary>
        /// Método que se suscribe al controlador de eventos para cuando se an terminado de agregar parámetros
        /// y se va a crear el método con esos parámetros.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="methodParamsCol"></param>
        [SupportedOSPlatform("windows")]
        private void ParamsReady(
            object sender,
            List<(string type, string name, string value)> methodParamsCol
        )
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabCodeEditor")
            {
                _methodParamsCol = methodParamsCol;

                _woSyntaxEditor.CreateNewMethod(
                    type: _newMethodType,
                    name: _newMethodName,
                    methodParamsCol: methodParamsCol
                );

                InitializeTreeListMethods();
            }
            else if (selectedTabName == "TabListCodeEditor")
            {
                _methodParamsCol = methodParamsCol;

                _woGridSyntaxEditor.CreateNewMethod(
                    type: _newMethodType,
                    name: _newMethodName,
                    methodParamsCol: methodParamsCol
                );

                InitializeGridListMethods();
            }
        }

        /// <summary>
        /// Método suscrito al controlador de eventos para cuando se busca crear el método ya definido pero
        /// sin parámetros aun que se le dio en la opción de agregar parámetros.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ContinueCreatingMethod(object sender, EventArgs e)
        {
            _woSyntaxEditor.CreateNewMethod(_newMethodType, _newMethodName);
            AddNewMethodToRow();
        }
        #endregion Parámetros

        #region Agregar método a la row

        /// <summary>
        /// Agrega el método en la row de la tabla de métodos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void AddNewMethodToRow()
        {
            _treeMethods.AddMethod(
                control: "Métodos Custom",
                type: _newMethodType,
                method: _newMethodName,
                haveCode: (_newMethodType != "void"),
                status: false
            );
        }

        #endregion Agregar método a la row

        #region Eliminar método

        /// <summary>
        /// Elimina el método seleccionado mientras que sea un método custom.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteMethod_ItemClick(object sender, ItemClickEventArgs e)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabCodeEditor")
            {
                string methodName = _treeMethods.DeleteMethod();
                if (methodName != string.Empty)
                {
                    _woSyntaxEditor.DeleteMethod(methodName, alerted: false);

                    InitializeTreeListMethods();
                }
                else
                {
                    ///send log or trow
                    XtraMessageBox.Show(
                        "No hay método que eliminar",
                        "Alert",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            else if (selectedTabName == "TabListCodeEditor")
            {
                string methodName = _treeGridMethods.DeleteMethod();
                if (methodName != string.Empty)
                {
                    _woGridSyntaxEditor.DeleteMethod(methodName, alerted: false);

                    InitializeGridListMethods();
                }
                else
                {
                    ///send log or trow
                    XtraMessageBox.Show(
                        "No hay método que eliminar",
                        "Alert",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        #endregion Eliminar método

        #endregion Métodos custom


        #region Gestión de atributos custom

        /// <summary>
        /// Instancia del dialog que permite la creación de atributos custom.
        /// </summary>
        private fmNewAttribute _fmNewAttributeDialog = null;

        /// <summary>
        /// Instancia del dialog que permite eliminar atributos custom.
        /// </summary>
        private fmDeleteAttribute _fmDeleteAttributeDialog = null;

        /// <summary>
        /// Inicializa la instancia del dialog, suscribe el método para recuperar la información
        /// y los muestra.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAddAttribute_ItemClick(object sender, ItemClickEventArgs e)
        {
            _fmNewAttributeDialog = new fmNewAttribute();
            _fmNewAttributeDialog.DataReadyEvt += AddAttribute;
            _fmNewAttributeDialog.ShowDialog();
        }

        /// <summary>
        /// Método suscrito al controlador de eventos del dialog que permite recibir
        /// la información desde el dialog a este formulario para la creación del atributo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newAttribute"></param>
        [SupportedOSPlatform("windows")]
        private void AddAttribute(
            object sender,
            (string type, string name, string value) newAttribute
        )
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabCodeEditor")
            {
                if (
                    _woSyntaxEditor.AlreadyExistAttribute(newAttribute.name)
                    || _woSyntaxEditor.AlreadyExistMethod(newAttribute.name)
                )
                {
                    XtraMessageBox.Show(
                        "El atributo que esta intentando crear ya existe",
                        "Alerta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    _woSyntaxEditor.CreateNewAttribute(
                        newAttribute.type,
                        newAttribute.name,
                        newAttribute.value
                    );
                }
            }
            else if (selectedTabName == "TabListCodeEditor")
            {
                if (
                    _woGridSyntaxEditor.AlreadyExistAttribute(newAttribute.name)
                    || _woGridSyntaxEditor.AlreadyExistMethod(newAttribute.name)
                )
                {
                    XtraMessageBox.Show(
                        "El atributo que esta intentando crear ya existe",
                        "Alerta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    _woGridSyntaxEditor.CreateNewAttribute(
                        newAttribute.type,
                        newAttribute.name,
                        newAttribute.value
                    );
                }
            }
        }

        /// <summary>
        /// Opción de eliminar atributo del formulario general.
        /// Inicializa el dialog con los datos para que el usuario pueda eliminar alguno de los atributos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteAttribute_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<string> customAttributesCol = new List<string>();

            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabCodeEditor")
            {
                customAttributesCol = _woSyntaxEditor.GetCustomAttributes();
            }
            else if (selectedTabName == "TabListCodeEditor")
            {
                customAttributesCol = _woGridSyntaxEditor.GetCustomAttributes();
            }

            if (customAttributesCol.Count == 0)
            {
                XtraMessageBox.Show(
                    "No hay atributos",
                    "Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            else
            {
                _fmDeleteAttributeDialog = new fmDeleteAttribute(customAttributesCol);
                _fmDeleteAttributeDialog.DeleteAttributeEvt += DeleteCustomAttribute;
                _fmDeleteAttributeDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Método suscrito al controlador de eventos del dialog para eliminar el método.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="attribute"></param>
        private void DeleteCustomAttribute(object sender, string attribute)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabCodeEditor")
            {
                _woSyntaxEditor.DeleteCustomAttribute(attribute);
            }
            else if (selectedTabName == "TabListCodeEditor")
            {
                _woGridSyntaxEditor.DeleteCustomAttribute(attribute);
            }
        }

        #endregion Gestión de atributos custom


        #region Gestión de usings

        /// <summary>
        /// Se ocupa de abrir el dialog para modificar los usings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUsings_ItemClick(object sender, ItemClickEventArgs e)
        {
            fmUsingSelector fmUsingSelector = new fmUsingSelector(_modelName);
            fmUsingSelector.ShowDialog();
        }

        #endregion Gestión de usings

        #endregion Syntax Editor


        #region Syntax editor de la grid

        #region Inicializar SyntaxEditor

        /// <summary>
        /// Instancia del syntax editor encapsulado en un componente de WinForms.
        /// </summary>
        private WoSyntaxEditor _woGridSyntaxEditor = null;

        /// <summary>
        /// Instancia de la grid de métodos.
        /// </summary>
        private WoTreeMethods _treeGridMethods = null;

        /// <summary>
        /// Inicializa el syntax editor para el modelo seleccionado.
        /// Verifica si ya hay una instancia y la reemplaza.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeWoGridSyntaxEditor()
        {
            string pathProject =
                $@"{proyecto.DirProyectData}\LayOuts\UserCode\{_modelName}GridList_proj";

            if (!File.Exists($@"{pathProject}\{_modelName}GridListScriptsUser.cs"))
            {
                WoBlazorSave woBlazorSave = new WoBlazorSave(
                    classModelName: $@"{_modelName}GridList"
                );
                WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
                WoContainer listRawContainer = woDesignerRawListHelper.BuildRawListForm(_modelName);
                woBlazorSave.BuildBaseSave(listRawContainer);
            }

            if (File.Exists($@"{pathProject}\{_modelName}GridListScriptsUser.cs"))
            {
                if (!_woGridSyntaxEditor.IsNull())
                {
                    pnlSyntaxForList.Controls.Clear();
                    TabListCodeEditor.PageVisible = false;
                }

                TabListCodeEditor.PageVisible = true;
                _woGridSyntaxEditor = new WoSyntaxEditor(
                    pathDirSaveProject: pathProject,
                    modelName: _modelName,
                    classModelName: $@"{_modelName}GridList",
                    methodParamsCol: _methodParamsCol,
                    isGrid: true
                );

                _woGridSyntaxEditor.CodeChangedEvt += EditCode;

                _woGridSyntaxEditor.Parent = pnlSyntaxForList;
                _woGridSyntaxEditor.Dock = DockStyle.Fill;

                InitializeGridListMethods();

                InitializeListGridSnippetPanel();
            }
            else
            {
                ///todo: Send trow: base files not ready
            }
        }

        #endregion Inicializar SyntaxEditor

        #region Inicializar grid de métodos

        /// <summary>
        /// Cargo los métodos al árbol de métodos.
        /// ToDo: cambiar el tipo de método por un enum.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeGridListMethods()
        {
            if (_treeGridMethods != null)
            {
                pnlListGridMethods.Controls.Clear();
                _treeGridMethods = null;
            }

            _treeGridMethods = new WoTreeMethods(_modelName);
            _treeGridMethods.Parent = pnlListGridMethods;
            _treeGridMethods.Dock = DockStyle.Fill;

            _treeGridMethods.ChangeSeleccionEvt += ChangeFocusedMethodGridList;

            bool correctCharge = false;

            do
            {
                try
                {
                    correctCharge = true;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($@"Carga incorrecta de métodos Formulario", "Alert");
                }
            } while (!correctCharge);

            ChargeNormalMethodsGridList();

            ChargeCustomMethodsGridList();

            _treeGridMethods.MethodsReady();

            ValidateUseCustomMethodsGridList();
        }

        [SupportedOSPlatform("windows")]
        private void ChargeNormalMethodsGridList()
        {
            IEnumerable<(string type, string method, bool haveCode)> baseMethods =
                _woGridSyntaxEditor.GetMethodsInfo(eCodeType.Method);

            List<(string type, string method, bool haveCode)> methods = baseMethods.ToList();

            for (int i = 0; i < methods.Count; i++)
            {
                string controlName = methods[i].method.ToString().Split('_')[0];

                bool correctCharge = false;

                do
                {
                    try
                    {
                        if (
                            methods[i].method.ToLower() != "getinstance"
                            && methods[i].method.ToLower() != "formsettings"
                        )
                        {
                            _treeGridMethods.AddMethod(
                                control: controlName,
                                type: methods[i].type,
                                method: methods[i].method,
                                haveCode: methods[i].haveCode,
                                status: true
                            );
                        }

                        correctCharge = true;
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(
                            $@"Carga incorrecta de métodos {controlName}, {methods[i].type}, {methods[i].method}, {methods[i].haveCode}, {true}",
                            "Alert"
                        );
                    }
                } while (!correctCharge);
            }
        }

        [SupportedOSPlatform("windows")]
        private void ChargeCustomMethodsGridList()
        {
            List<(string type, string method, bool haveCode)> customMethods =
                _woGridSyntaxEditor.GetMethodsInfo(eCodeType.CustomMethod);

            foreach ((string type, string method, bool haveCode) method in customMethods)
            {
                _treeGridMethods.AddMethod(
                    control: "Métodos Custom",
                    type: method.type,
                    method: method.method,
                    haveCode: method.haveCode,
                    status: false
                );
            }
        }

        /// <summary>
        /// Valida los métodos usados a través del syntax y lo pasa al árbol de métodos.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ValidateUseCustomMethodsGridList()
        {
            List<(string method, bool used)> methodsUsed =
                _woGridSyntaxEditor.ValidateUseCustomMethods();
            _treeGridMethods.UpdateMethodsUsed(methodsUsed);
        }

        #endregion Inicializar grid de métodos

        #region Initialize snipets panel

        /// <summary>
        /// Instancia del panel de seleccion de senipets para el syntax editor.
        /// </summary>
        private WoSnipetSelector _woListGridSnipetSelector = null;

        /// <summary>
        /// Inicializa el panel de seccion de snipets.
        /// Limpia el panel donde se encontrara en caso de que ya exista una instancia anterior
        /// la instancia y la agrega al panel.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeListGridSnippetPanel()
        {
            pnlListGridSnippets.Controls.Clear();

            _woListGridSnipetSelector = new WoSnipetSelector(_modelName, isForm: false);
            _woListGridSnipetSelector.Dock = DockStyle.Fill;
            _woListGridSnipetSelector.SetSnipetEvt += SetListGridSelectedSnipet;

            pnlListGridSnippets.Controls.Add(_woListGridSnipetSelector);
        }

        /// <summary>
        /// Pasa el snipet recuerado desde el panel a travez del action.
        /// </summary>
        /// <param name="snipet"></param>
        [SupportedOSPlatform("windows")]
        private void SetListGridSelectedSnipet(string snipet)
        {
            _woGridSyntaxEditor.SetSnipet(snipet);
        }

        #endregion Initialize snipets panel

        #region Selección de método

        /// <summary>
        /// Bandera para cuando se regresa al ultimo método seleccionado.
        /// </summary>
        private bool _reloadGridMethod = false;

        /// <summary>
        /// Método suscrito al evento de
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="control"></param>
        [SupportedOSPlatform("windows")]
        private void ChangeFocusedMethodGridList(string eventName, string control, string type)
        {
            if (!_reloadGridMethod)
            {
                DialogResult result = DialogResult.Yes;

                if (!_isSavedCode)
                {
                    result = XtraMessageBox.Show(
                        "Se perdera el código no salvado. ¿Está seguro de querer salir?",
                        "Alerta",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                }

                if (result == DialogResult.Yes)
                {
                    if (_woGridSyntaxEditor != null)
                    {
                        if (eventName.IsNull())
                        {
                            _woGridSyntaxEditor.CleanEditor();
                        }
                        else
                        {
                            if (control == @"Métodos Custom")
                            {
                                _woGridSyntaxEditor.ChargeCode(
                                    methodType: type,
                                    methodName: eventName,
                                    typeOfCode: eCodeType.CustomMethod
                                );
                            }
                            else
                            {
                                _woGridSyntaxEditor.ChargeCode(
                                    methodType: type,
                                    methodName: eventName,
                                    typeOfCode: eCodeType.Method
                                );
                            }
                        }
                    }
                }
                else
                {
                    _reloadGridMethod = true;
                    _treeGridMethods.SetLastMethod();
                }
            }
            else
            {
                _reloadGridMethod = false;
            }
        }

        #endregion Selección de método

        #region Salvar código

        /// <summary>
        /// Instancia de la clase que permite actualizar los ficheros de código al aver modificaciones.
        /// </summary>
        private WoSyntaxEditorFiles _woSyntaxEditorFilesGridList;

        /// <summary>
        /// Salva el código.
        ///     - Valida si es un nuevo método para agregarlo a la grid con el resto de métodos.
        ///     - En caso de no se un método nuevo, lo escribe con un método del syntax.
        ///     - Si existe el proyecto generado utiliza la clase WoSyntaxEditorFiles para actualizar el proyecto.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SaveCustomCode()
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();

            if (_woGridSyntaxEditor.SaveCode())
            {
                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                    tp.Text == "*Código de lista"
                );
                if (tab != null)
                {
                    tab.Text = "Código de lista";
                    _isSavedCode = true;
                }
            }

            _treeGridMethods.UpdateHaveCode(_woGridSyntaxEditor.HaveEditorCode());

            List<(string method, bool used)> methodsUsed =
                _woGridSyntaxEditor.ValidateUseCustomMethods();
            _treeGridMethods.UpdateMethodsUsed(methodsUsed);

            _woSyntaxEditorFilesGridList = new WoSyntaxEditorFiles(
                modelName: $@"{_modelName}GridList",
                unitClassModelNameServer: _serverClassModelName,
                unitClassModelNameWasm: _wasmClassModelName,
                isGridCode: true
            );
            _woSyntaxEditorFilesGridList.UpdateBlazorProject();
        }

        #endregion Salvar código


        #endregion Syntax editor de la grid


        #region Eventos de los botones del grupo de Edición

        /// <summary>
        /// Limpia todo el panel de diseño.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnLimpiarTodo_ItemClick(object sender, ItemClickEventArgs e)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabFormDesigner")
            {
                DialogResult result = XtraMessageBox.Show(
                    text: "¿Esta seguro de limpiar todo el formulario?\n Todo se enviara al panel de items",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    _woFormDesginer.CleanAll();
                    ChangeSavedStatus(false);
                }
            }
            else if (selectedTabName == "TabGridDesigner")
            {
                DialogResult result = XtraMessageBox.Show(
                    text: "¿Esta seguro de limpiar toda la grid?\n Todo se enviara al panel de items",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    _woGridDesigner.CleanAll();
                }
            }
            else if (selectedTabName == "TabListDesigner")
            {
                DialogResult result = XtraMessageBox.Show(
                    text: "¿Esta seguro de limpiar toda la lista?\n Todo se enviara al panel de items",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    _listDesigner.CleanAll();
                }
            }
        }

        #endregion Eventos de los botones del grupo de Edición


        #region Diseñador de la lista para cada modelo

        /// <summary>
        /// Instancia del diseñador de grids.
        /// </summary>
        private WoGridDesigner _listDesigner;

        /// <summary>
        /// Permite la inicialización y dibujado del diseñador de la lista para cada modelo.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeListDesigner()
        {
            if (TabListDesigner != null)
            {
                TabListDesigner.Controls.Clear();

                _listDesigner?.Dispose();

                _listDesigner = new WoGridDesigner(saveFolder: "ListDesign", modelName: _modelName);
                _listDesigner.ModeChanged += ChangeDesignMode;
                _listDesigner.AddButtonEvt += InitializeWoGridSyntaxEditor;
                _listDesigner.DeleteButtonEvt += InitializeWoGridSyntaxEditor;
                _listDesigner.UpdateProjectsEvt += SaveUnitProjects;

                XtraScrollableControl scrollableControl = new XtraScrollableControl();
                scrollableControl.Dock = DockStyle.Fill;
                scrollableControl.Controls.Add(_listDesigner);

                TabListDesigner.Controls.Add(scrollableControl);
                _listDesigner.Dock = DockStyle.Fill;

                _listDesigner.GridUpdatingEvt += UpdatingGrid;

                var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Lista");
                if (tab != null)
                {
                    tab.Text = "Lista";
                }

                tab = tabBaseDesigner.TabPages.FirstOrDefault(tp => tp.Text == "*Esclava");
                if (tab != null)
                {
                    tab.Text = "Esclava";
                }

                UpdateScripts();
            }
        }

        #endregion Diseñador de la lista para cada modelo


        #region Abrir directorios

        /// <summary>
        /// Abre en el explorador de archivos la ruta con los proyectos generados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenDirectory_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process.Start("explorer.exe", $@"{proyecto.DirApplication}\Temp");
        }

        /// <summary>
        /// Abre el explorador de archivos con la ruta de los ficheros de código salvados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenSaveDirectory_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process.Start("explorer.exe", $@"{proyecto.DirLayOuts}\UserCode");
        }

        #endregion Abrir directorios


        #region Ejecución y Stop de blazor

        /// <summary>
        /// Evento click del botón de ejecutar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnExecute_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveUnitProjects();

                consoleData.Text = string.Empty;

                _woBlazorServerExecute.GeneralStop();
                _woBlazorWasmExecute.GeneralStop();

                if (CanExecute())
                {
                    string selectedOption = cmbExecute.EditValue.ToString();
                    if (selectedOption != "Eliminar Generados")
                    {
                        GenerateServer();
                        GenerateWasm();
                    }

                    if (selectedOption == "Solo Server")
                    {
                        btnExecute.Enabled = false;
                        btnStop.Enabled = true;
                        ExecuteBlazorServer();
                    }
                    else if (selectedOption == "Solo Wasm")
                    {
                        btnExecute.Enabled = false;
                        btnStop.Enabled = true;
                        ExecuteBlazorWasm();
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
                        btnExecute.Enabled = false;
                        btnStop.Enabled = true;
                        WatchBlazorServer();
                    }
                    else if (selectedOption == "Watch Wasm")
                    {
                        btnExecute.Enabled = false;
                        btnStop.Enabled = true;
                        WatchBlazorWasm();
                    }
                    else if (selectedOption == "Eliminar Generados")
                    {
                        DeleteGeneratedProyects();
                    }
                }
                else
                {
                    StringBuilder nesesaryControls = new StringBuilder();
                    foreach (string item in _listOfNesesaryItems)
                    {
                        nesesaryControls.AppendLine(item);
                    }

                    if (_listOfNesesaryItems.Count > 0)
                    {
                        XtraMessageBox.Show(
                            text: $@"Faltan los siguientes controles para el correcto funcionamiento del formulario: {nesesaryControls}",
                            caption: "No es posible ejecutar el proyecto sin los componentes base",
                            buttons: MessageBoxButtons.OK,
                            icon: MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al ejecutar el proyecto de blazor. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Detiene la ejecución de cualquiera de los proyectos de blazor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();
            btnExecute.Enabled = true;
            btnStop.Enabled = false;

            consoleData.SelectionColor = Color.Green;
            consoleData.AppendText("Proyecto detenido con éxito.");

            if (_woBlazorServerExecute.SendToConsoleEvt != null)
            {
                _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
                _woBlazorServerExecute.SendToConsoleEvt = null;
            }
            if (_woBlazorWasmExecute.SendToConsoleEvt != null)
            {
                _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
                _woBlazorWasmExecute.SendToConsoleEvt = null;
            }
        }

        /// <summary>
        /// Método suscrito a los eventos de cierre de los proceso que levantan los proyectos
        /// de blazor en caso de un cierre externo al programa.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ExternalStop(object sender, bool e)
        {
            btnExecute.Enabled = true;
            btnStop.Enabled = false;
        }

        #endregion Ejecución y Stop de blazor

        #region Validar ejecución

        private List<string> _listOfNesesaryItems = new List<string>();

        /// <summary>
        /// Valida que existan los componentes inter relacionados necesarios para que la ejecución funcione correctamente.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private bool CanExecute()
        {
            bool canExecute = false;

            //WoContainer lastVersionContainer = _woFormDesginer.GetLastVersionGroup();

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            WoContainer lastVersionContainer = woProjectDataHelper.GetFullDesing(_modelName);

            _listOfNesesaryItems.Clear();

            if (
                _modelSelected.TipoModelo == WoTypeModel.Kardex
                || _modelSelected.TipoModelo == WoTypeModel.Control
                || _modelSelected.TipoModelo == WoTypeModel.Configuration
                || _modelSelected.TipoModelo == WoTypeModel.View
                || _modelSelected.ProcesoId == ""
            )
            {
                return true;
            }
            else if (
                _modelSelected.TipoModelo == WoTypeModel.Request
                || _modelSelected.TipoModelo == WoTypeModel.CatalogSlave
                || _modelSelected.TipoModelo == WoTypeModel.TransactionSlave
            )
            {
                _listOfNesesaryItems.Add("Alertas");
            }
            else
            {
                _listOfNesesaryItems.Add("Alertas");
                _listOfNesesaryItems.Add("Controles");
            }

            SearchNesesaryItems(lastVersionContainer);
            canExecute = (_listOfNesesaryItems.Count == 0);

            if (!ValidateControlReferences(_modelSelected.Id))
            {
                return false;
            }

            return canExecute;
        }

        /// <summary>
        /// Busca sobre del wo container si se encuentran los campos necesarios
        /// los cuales están en la lista de necesarios.
        /// </summary>
        /// <param name="container"></param>
        private void SearchNesesaryItems(WoContainer container)
        {
            if (container.ContainersCol != null)
            {
                foreach (WoContainer subContainer in container.ContainersCol)
                {
                    SearchNesesaryItems(subContainer);
                }
            }

            if (container.ItemsCol != null)
            {
                foreach (WoItem item in container.ItemsCol)
                {
                    var findItem = _listOfNesesaryItems.Where(x => x == item.Id).FirstOrDefault();
                    if (findItem != null)
                    {
                        _listOfNesesaryItems.Remove(findItem);
                    }
                }
            }
        }

        /// <summary>
        /// Busca sobre el modelo que los campos de tipo referencia que requieren de un campo
        /// de descripción lo contenga correctamente
        /// </summary>
        [SupportedOSPlatform("windows")]
        private bool ValidateControlReferences(string modelName)
        {
            bool result = true;

            List<string> nesesaryItems = new List<string>();

            Modelo findModel = proyecto.ModeloCol.Modelos.FirstOrDefault(model =>
                model.Id == modelName
            );
            if (findModel != null)
            {
                /// Validación de lookups
                List<ModeloColumna> lookUpCol = findModel
                    .Columnas.Where(column => column.TipoControl == WoTypeControl.LookUp)
                    .ToList();

                foreach (ModeloColumna col in lookUpCol)
                {
                    ModeloColumna findCol = findModel.Columnas.FirstOrDefault(column =>
                        column.Id == $@"__{col.Id}"
                    );
                    if (findCol == null)
                    {
                        nesesaryItems.Add(col.Id);
                    }
                }

                /// Validación de lookups dialogs
                List<ModeloColumna> lookUpDialogCol = findModel
                    .Columnas.Where(column => column.TipoControl == WoTypeControl.LookUpDialog)
                    .ToList();

                foreach (ModeloColumna col in lookUpDialogCol)
                {
                    ModeloColumna findCol = findModel.Columnas.FirstOrDefault(column =>
                        column.Id == $@"__{col.Id}"
                    );
                    if (findCol == null)
                    {
                        nesesaryItems.Add(col.Id);
                    }
                }

                if (nesesaryItems.Count > 0)
                {
                    StringBuilder strNesesaryCols = new StringBuilder();
                    foreach (string col in nesesaryItems)
                    {
                        strNesesaryCols.AppendLine(col);
                    }
                    XtraMessageBox.Show(
                        text: $@"Los siguientes campos de tipo lookup requieren un campo de descripción: {strNesesaryCols}",
                        caption: "No es posible ejecutar el proyecto sin los componentes base",
                        buttons: MessageBoxButtons.OK,
                        icon: MessageBoxIcon.Error
                    );

                    result = false;
                }
            }

            return result;
        }

        #endregion Validar ejecución

        #region Tipo de ejecución

        /// <summary>
        /// Actualiza el fichero con la persistencia para la ejecución del formulario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void cmbExecute_EditValueChanged(object sender, EventArgs e)
        {
            string selectedOption = cmbExecute.EditValue.ToString();
            WoDirectory.WriteFile(
                $@"{proyecto.DirApplication}\Temp\SelectedOption.txt",
                selectedOption
            );
        }

        /// <summary>
        /// Selecciona la opción que se había seleccionado anteriormente.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void SetSelectedOption()
        {
            if (File.Exists($@"{proyecto.DirApplication}\Temp\SelectedOption.txt"))
            {
                string selectedOption = WoDirectory.ReadFile(
                    $@"{proyecto.DirApplication}\Temp\SelectedOption.txt"
                );
                cmbExecute.EditValue = selectedOption;
            }
        }

        #endregion Tipo de ejecución

        #region Generación

        /// <summary>
        /// Genera el proyecto de server y el layout en el que se encuentra el usuario, solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void GenerateServer()
        {
            string pathProyectUnit = $@"{proyecto.DirProyectTemp}/{_serverProjectName}";
            if (!File.Exists($@"{pathProyectUnit}/{_serverProjectName}.csproj"))
            {
                WoContainer woContainer = new WoContainer();

                if (
                    _modelSelected.TipoModelo == WoTypeModel.Kardex
                    || _modelSelected.TipoModelo == WoTypeModel.Control
                    || _modelSelected.TipoModelo == WoTypeModel.Configuration
                    || _modelSelected.TipoModelo == WoTypeModel.View
                )
                {
                    WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
                    woContainer = woDesignerRawListHelper.BuildRawListForm(_modelName);
                }
                else
                {
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    woContainer = woProjectDataHelper.GetFullDesing(_modelName);
                    //woContainer = _woFormDesginer.GetLastVersionGroup();
                }

                WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
                woBlazorGenerator.SendToConsole += SendDataToConsole;
                woBlazorGenerator.GenerateBaseServer(_serverProjectName, isUnit: true);

                woBlazorGenerator.GenerateUnitLayout(
                    container: woContainer,
                    _serverClassModelName,
                    isServer: true
                );
            }
        }

        /// <summary>
        /// Genera el proyecto de wasm y el layout en el que se encuentra el usuario,
        /// solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void GenerateWasm()
        {
            string pathProyectUnit = $@"{proyecto.DirProyectTemp}/{_wasmProjectName}";
            if (!File.Exists($@"{pathProyectUnit}/{_wasmProjectName}.csproj"))
            {
                WoContainer woContainer = new WoContainer();

                if (
                    _modelSelected.TipoModelo == WoTypeModel.Kardex
                    || _modelSelected.TipoModelo == WoTypeModel.Control
                    || _modelSelected.TipoModelo == WoTypeModel.Configuration
                    || _modelSelected.TipoModelo == WoTypeModel.View
                )
                {
                    WoDesignerRawListHelper woDesignerRawListHelper = new WoDesignerRawListHelper();
                    woContainer = woDesignerRawListHelper.BuildRawListForm(_modelName);
                }
                else
                {
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    woContainer = woProjectDataHelper.GetFullDesing(_modelName);
                    //woContainer = _woFormDesginer.GetLastVersionGroup();
                }

                WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
                woBlazorGenerator.SendToConsole += SendDataToConsole;
                woBlazorGenerator.GenerateBaseWasm(_wasmProjectName, isUnit: true);

                woBlazorGenerator.GenerateUnitLayout(
                    container: woContainer,
                    _wasmClassModelName,
                    isServer: false
                );
            }
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

                XtraMessageBox.Show(
                    $@"Se eliminaron las pruebas unitarias exitosamente.",
                    $@"Alerta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $@"Ocurrió un error al eliminar el formulario: {ex.Message}",
                    $@"Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Generación

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


        #region Ejecución Server

        /// <summary>
        /// Clase ocupada de generar la generación del proyecto de blazor para server side.
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Utiliza la clase de ejecución para levantar blazor server.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void ExecuteBlazorServer()
        {
            try
            {
                _sendError = false;
                _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
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
        /// Utiliza la clase de ejecución para levantar blazor server con el watch.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void WatchBlazorServer()
        {
            try
            {
                _sendError = false;
                _woBlazorServerExecute.SendToConsoleEvt += SendDataToConsole;
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

        #endregion Ejecución Server

        #region Ejecución Wasm

        /// <summary>
        /// Clase ocupada de generar la generación del proyecto de blazor para server side.
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
                _sendError = false;
                _woBlazorWasmExecute.SendToConsoleEvt += SendDataToConsole;
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
                _sendError = false;
                _woBlazorWasmExecute.SendToConsoleEvt += SendDataToConsole;
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

        #endregion Ejecución Wasm


        #region Envió de la data a la consola

        /// <summary>
        /// Bandera que indica si ya fue enviada una alerta de error para la ejecución actual.
        /// </summary>
        private bool _sendError = false;

        /// <summary>
        /// Envío de la data a la consola.
        /// </summary>
        /// <param name="Data"></param>
        [SupportedOSPlatform("windows")]
        private void SendDataToConsole(string Data)
        {
            consoleData.Invoke(
                new Action(() =>
                {
                    if (Data.Contains("Building") || Data.Contains("building"))
                    {
                        consoleData.SelectionColor = Color.LightBlue;
                    }
                    else if (Data.Contains("Warning") || Data.Contains("warning"))
                    {
                        consoleData.SelectionColor = Color.Yellow;
                    }
                    else if (Data == "CLSCLEAR")
                    {
                        consoleData.Clear();
                    }
                    else if (
                        Data.Contains("Error")
                        || Data.Contains("error")
                        || Data.Contains("Stop")
                    )
                    {
                        consoleData.SelectionColor = Color.Red;

                        if (!_sendError)
                        {
                            XtraMessageBox.Show(
                                $"Se produjo un error al intentar ejecutar el proyecto de blazor.\n\r Revise la pestaña de ejecución para mas información",
                                "Alerta",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            _sendError = true;
                        }
                    }
                    else if (Data.Contains("Info") || Data.Contains("info"))
                    {
                        consoleData.SelectionColor = Color.Green;
                    }
                    else
                    {
                        consoleData.SelectionColor = Color.White;
                    }

                    consoleData.AppendText($"{Data}\n\r");
                    consoleData.ScrollToCaret();

                    if (_sendError)
                    {
                        if (_woBlazorServerExecute.SendToConsoleEvt != null)
                        {
                            _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
                            _woBlazorServerExecute.SendToConsoleEvt = null;
                        }
                        if (_woBlazorWasmExecute.SendToConsoleEvt != null)
                        {
                            _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
                            _woBlazorWasmExecute.SendToConsoleEvt = null;
                        }
                    }
                })
            );
        }

        #endregion Envió de la data a la consola


        #region Blazor general

        /// <summary>
        /// Instancia y muestra el componente para seleccionar los modelos a generar el en blazor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnGenerateAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();

            if (_woBlazorServerExecute.SendToConsoleEvt != null)
            {
                _woBlazorServerExecute.SendToConsoleEvt -= SendDataToConsole;
            }
            if (_woBlazorWasmExecute.SendToConsoleEvt != null)
            {
                _woBlazorWasmExecute.SendToConsoleEvt -= SendDataToConsole;
            }

            WoModelSelector woModelSelector = new WoModelSelector();
            woModelSelector.ShowDialog();
        }

        #endregion Blazor general



        #region Enable and disable botones

        #region Bloqueo de grupos de botones

        [SupportedOSPlatform("windows")]
        public void BlockLayoutGroup()
        {
            btnSaveAndExit.Enabled = false;
            btnSaveLayout.Enabled = false;
            btnRestoreLayout.Enabled = false;
            btnExitOnly.Enabled = false;
        }

        [SupportedOSPlatform("windows")]
        public void BlockGroupsGroup()
        {
            //btnAddGroup.Enabled = false;
            //btnAddTabGroup.Enabled = false;
        }

        [SupportedOSPlatform("windows")]
        public void BlockEdicionGroup()
        {
            btnDeleteAll.Enabled = false;
        }

        [SupportedOSPlatform("windows")]
        public void BlockBlazorGroup()
        {
            btnStopBlazor.Enabled = false;
            btnDeleteBlazor.Enabled = false;

            btnGenerateServer.Enabled = false;
            btnExecuteServer.Enabled = false;
            btnAbrirServer.Enabled = false;

            btnGenerateWasm.Enabled = false;
            btnExecuteWasm.Enabled = false;
            btnAbrirWasm.Enabled = false;
        }

        [SupportedOSPlatform("windows")]
        public void BlockCodeGroup()
        {
            btnSaveCode.Enabled = false;
            btnAddMethod.Enabled = false;
            btnDeleteMethod.Enabled = false;
            btnAddAttribute.Enabled = false;
            btnDeleteAttribute.Enabled = false;
        }

        #endregion Bloqueo de grupos de botones

        #region Desbloqueo de grupos de botones

        [SupportedOSPlatform("windows")]
        public void EnableLayoutGroup()
        {
            btnSaveAndExit.Enabled = true;
            btnSaveLayout.Enabled = true;
            btnRestoreLayout.Enabled = true;
            btnExitOnly.Enabled = true;
        }

        [SupportedOSPlatform("windows")]
        public void EnableGroupsGroup()
        {
            //btnAddGroup.Enabled = true;
            //btnAddTabGroup.Enabled = true;
        }

        [SupportedOSPlatform("windows")]
        public void EnableEdicionGroup()
        {
            btnDeleteAll.Enabled = true;
        }

        [SupportedOSPlatform("windows")]
        public void EnableBlazorGroup()
        {
            btnStopBlazor.Enabled = true;
            btnDeleteBlazor.Enabled = true;

            btnGenerateServer.Enabled = true;
            btnExecuteServer.Enabled = true;
            btnAbrirServer.Enabled = true;

            btnGenerateWasm.Enabled = true;
            btnExecuteWasm.Enabled = true;
            btnAbrirWasm.Enabled = true;
        }

        [SupportedOSPlatform("windows")]
        public void EnableCodeGroup()
        {
            btnSaveCode.Enabled = true;
            btnAddMethod.Enabled = true;
            btnDeleteMethod.Enabled = true;
            btnAddAttribute.Enabled = true;
            btnDeleteAttribute.Enabled = true;
        }

        #endregion Desbloqueo de grupos de botones

        #region Comportamiento con base al estado del formulario

        /// <summary>
        /// Orquesta los métodos de bloqueo y desbloqueo de controles en función del modo
        /// al que se pasa.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="modeDesigner"></param>
        [SupportedOSPlatform("windows")]
        public void ChangeDesignMode(object sender, eModeDesigner modeDesigner)
        {
            switch (modeDesigner)
            {
                case eModeDesigner.Block:
                    btnEdit.Enabled = true;
                    BlockLayoutGroup();
                    BlockGroupsGroup();
                    BlockEdicionGroup();

                    if (!_isSaved)
                    {
                        SaveFromDesigner();
                    }

                    break;
                case eModeDesigner.Design:
                    btnEdit.Enabled = false;
                    EnableLayoutGroup();
                    EnableGroupsGroup();
                    EnableEdicionGroup();
                    break;
            }
        }

        [SupportedOSPlatform("windows")]
        public void SaveFromDesigner()
        {
            if (!_restart && !_isSaved)
            {
                DialogResult result = XtraMessageBox.Show(
                    text: "Desea guardar las modificaciones realizadas",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

                    if (selectedTabName == "TabFormDesigner")
                    {
                        SaveLayout();

                        EnableCodeGroup();
                    }
                    else if (selectedTabName == "TabGridDesigner")
                    {
                        SaveGrid();
                    }
                    else if (selectedTabName == "TabListDesigner")
                    {
                        _listDesigner.SerializeGridToJson();

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Lista"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Lista";
                        }
                    }

                    SaveUnitProjects();
                }
                else
                {
                    if (tabBaseDesigner.SelectedTabPage.Name == "TabFormDesigner")
                    {
                        _woFormDesginer.InitializeDesigner();
                    }
                    else if (tabBaseDesigner.SelectedTabPage.Name == "TabListDesigner")
                    {
                        _listDesigner.InitializeGrid();

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Lista"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Lista";
                        }
                    }
                    else if (tabBaseDesigner.SelectedTabPage.Name == "TabGridDesigner")
                    {
                        _woGridDesigner.InitializeGrid();

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Esclava"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Esclava";
                        }
                    }
                }

                _isSaved = true;
                ChangeSavedStatus(_isSaved);
            }
            else
            {
                _restart = false;
            }
        }

        #endregion Comportamiento con base al estado del formulario

        #endregion Enable and disable botones

        public void Refrescar()
        {
            throw new NotImplementedException();
        }

        #region Alertas

        private WoLog _dontHaveModels = new WoLog()
        {
            CodeLog = "000",
            Title = "No se encuentra seleccionado ningún modelo.",
            Details =
                "Por lo general se auto selecciona algún modelo por defecto en caso de no hacerlo, es probable que no existan modelos a los que se pueda generar un formulario, primero cree algún modelo",
            UserMessage =
                "Por lo general se auto selecciona algún modelo por defecto en caso de no hacerlo, es probable que no existan modelos a los que se pueda generar un formulario, primero cree algún modelo",
            LogType = eLogType.Warning,
            FileDetails = new WoFileDetails()
            {
                Class = "fmModelLayOut",
                MethodOrContext = "ReduceWidth"
            }
        };

        #endregion Alertas


        #region Cambio de la selección de la tab

        /// <summary>
        /// Bandera que indica cuando se anula el cambio de la selección de la tab.
        /// </summary>
        private bool _noValidChange = false;

        /// <summary>
        /// Método que se detona cuando se modifica la selección de la tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void tabBaseDesigner_SelectedPageChanged(
            object sender,
            DevExpress.XtraTab.TabPageChangedEventArgs e
        )
        {
            string selectedTabName = e.PrevPage.Name;

            if (!_isSaved && (!_noValidChange))
            {
                DialogResult result = XtraMessageBox.Show(
                    text: "Se perderán las modificaciones, Desea continuar?",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Information
                );

                if (selectedTabName == "TabFormDesigner")
                {
                    if (result == DialogResult.Yes)
                    {
                        _isSaved = true;
                        _woFormDesginer?.BlockMode();
                        _woFormDesginer?.InitializeDesigner();

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Diseñador"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Diseñador";
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        _noValidChange = true;
                        tabBaseDesigner.SelectedTabPage = e.PrevPage;
                    }
                }
                else if (selectedTabName == "TabGridDesigner")
                {
                    if (result == DialogResult.Yes)
                    {
                        _isSaved = true;
                        _woGridDesigner?.BlockMode();
                        _woGridDesigner?.InitializeGrid();

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Esclava"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Esclava";
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        _noValidChange = true;
                        tabBaseDesigner.SelectedTabPage = e.PrevPage;
                    }
                }
                else if (selectedTabName == "TabListDesigner")
                {
                    if (result == DialogResult.Yes)
                    {
                        _isSaved = true;
                        _listDesigner?.BlockMode();
                        _listDesigner.InitializeGrid();

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Lista"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Lista";
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        _noValidChange = true;
                        tabBaseDesigner.SelectedTabPage = e.PrevPage;
                    }
                }
            }
            else if (_isSaved)
            {
                if (e.PrevPage.Name == "TabFormDesigner" && e.Page.Name != "TabFormDesigner")
                {
                    _woFormDesginer?.BlockMode();
                }
                else if (e.PrevPage.Name == "TabGridDesigner" && e.Page.Name != "TabGridDesigner")
                {
                    _woGridDesigner?.BlockMode();
                }
                else if (e.PrevPage.Name == "TabListDesigner" && e.Page.Name != "TabListDesigner")
                {
                    _listDesigner?.BlockMode();
                }
            }
            else
            {
                _noValidChange = false;
            }

            if (!_isSavedCode && (!_noValidChange))
            {
                DialogResult result = XtraMessageBox.Show(
                    text: "Se perderá el código que no se haya guardado, ¿Desea continuar?",
                    caption: "Alerta",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Information
                );

                if (selectedTabName == "TabCodeEditor")
                {
                    if (result == DialogResult.Yes)
                    {
                        _isSavedCode = true;
                        _woSyntaxEditor.InitializeCodeEditors(
                            methodType: "void",
                            methodName: "FormularioIniciado",
                            typeOfCode: eCodeType.Method
                        );

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Código"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Código";
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        _noValidChange = true;
                        tabBaseDesigner.SelectedTabPage = e.PrevPage;
                    }
                }
                else if (selectedTabName == "TabListCodeEditor")
                {
                    if (result == DialogResult.Yes)
                    {
                        _isSavedCode = true;
                        _woGridSyntaxEditor.InitializeCodeEditors(
                            methodType: "void",
                            methodName: "FormularioIniciado",
                            typeOfCode: eCodeType.Method
                        );

                        var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                            tp.Text == "*Código de lista"
                        );
                        if (tab != null)
                        {
                            tab.Text = "Código de lista";
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        _noValidChange = true;
                        tabBaseDesigner.SelectedTabPage = e.PrevPage;
                    }
                }
            }
            else
            {
                _noValidChange = false;
            }

            if (e.Page.Name == "TabCodeEditor")
            {
                btnEdit.Enabled = false;

                btnCleanAll.Enabled = false;
            }
            else if (e.Page.Name == "TabListCodeEditor")
            {
                btnEdit.Enabled = false;

                btnCleanAll.Enabled = false;
            }
            else
            {
                if (_isSaved)
                {
                    btnEdit.Enabled = true;
                }
                else
                {
                    btnEdit.Enabled = false;
                }

                btnCleanAll.Enabled = true;
            }
        }

        #endregion Cambio de la selección de la tab


        [SupportedOSPlatform("windows")]
        private void buAceptarCambios_ItemClick(object sender, ItemClickEventArgs e) { }

        #region Limpieza completa

        /// <summary>
        /// Restaura el formulario a la version base de creación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCleanAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            string selectedTabName = tabBaseDesigner.SelectedTabPage.Name;

            if (selectedTabName == "TabFormDesigner")
            {
                if (
                    XtraMessageBox.Show(
                        "Se perderán todos los cambios realizados en el formulario hasta ahora. ¿Está seguro de querer restaurar el formulario a su estado inicial?",
                        "Restaurar formulario",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.Yes
                )
                {
                    if (File.Exists($@"{proyecto.DirLayOuts}\FormDesign\{_modelSelected.Id}.json"))
                    {
                        WoDirectory.DeleteFile(
                            $@"{proyecto.DirLayOuts}\FormDesign\{_modelSelected.Id}.json"
                        );
                    }

                    WoDesignerRawSerializerHelper woDesignerRawSerializerHelper =
                        new WoDesignerRawSerializerHelper();
                    WoContainer baseDesing = woDesignerRawSerializerHelper.BuildRawWoContainer(
                        _modelSelected.Id
                    );

                    WoDirectory.WriteFile(
                        $@"{proyecto.DirLayOuts}\FormDesign\{_modelSelected.Id}.json",
                        JsonConvert.SerializeObject(baseDesing)
                    );

                    btnRestaurarLayout_ItemClick(null, null);

                    SaveLayout();
                    _woFormDesginer.BlockMode();
                    EnableCodeGroup();

                    SaveUnitProjects();
                }
            }
            else if (selectedTabName == "TabGridDesigner")
            {
                if (
                    XtraMessageBox.Show(
                        "Se perderán todos los cambios realizados en la slave hasta ahora. ¿Está seguro de querer restaurar la grid a su estado inicial?",
                        "Restaurar lista",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.Yes
                )
                {
                    if (
                        File.Exists(
                            $@"{proyecto.DirLayOuts}\Grids\{_modelSelected.Id}GridList.json"
                        )
                    )
                    {
                        WoDirectory.DeleteFile(
                            $@"{proyecto.DirLayOuts}\Grids\{_modelSelected.Id}GridList.json"
                        );
                    }
                    if (
                        File.Exists(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        )
                    )
                    {
                        string json = File.ReadAllText(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        );
                        List<WoCustomButtonProperties> customButtons =
                            JsonConvert.DeserializeObject<List<WoCustomButtonProperties>>(json);

                        WoSyntaxManagerUserCode userCode = new WoSyntaxManagerUserCode();
                        userCode.InitializeManager(
                            pathScript: $@"{proyecto.DirLayOuts}\UserCode\{_modelName}GridList_proj\{_modelName}GridListScriptsUser.cs",
                            className: $@"{_modelName}GridList",
                            modelName: _modelName
                        );

                        foreach (WoCustomButtonProperties customButton in customButtons)
                        {
                            if (
                                customButton.ButtonId != "BtnEdit"
                                && customButton.ButtonId != "BtnCopyToNew"
                            )
                            {
                                userCode.DeleteMethod($@"{customButton.ButtonId}_OnClick");
                            }
                            else if (
                                customButton.ButtonId == "BtnEdit"
                                || customButton.ButtonId == "BtnCopyToNew"
                            )
                            {
                                userCode.DeleteMethod($@"{customButton.ButtonId}_OnClick");
                            }
                        }

                        WoDirectory.DeleteFile(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        );
                    }
                    if (
                        !File.Exists(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        )
                    )
                    {
                        if (!_isExtencion)
                        {
                            WoCustomButtonsRawHelper woCustomButtonsRawHelper =
                                new WoCustomButtonsRawHelper();
                            woCustomButtonsRawHelper.BuildRawGridList(_modelSelected.Id);
                        }

                        InitializeWoGridSyntaxEditor();
                    }

                    WoGridDesignerRawHelper woGridDesignerRawHelper = new WoGridDesignerRawHelper();
                    WoGridProperties woGridProperties = woGridDesignerRawHelper.GetRawGrid(
                        _modelSelected.Id,
                        isSlave: false
                    );

                    WoDirectory.WriteFile(
                        $@"{proyecto.DirLayOuts}\Grids\{_modelSelected.Id}GridList.json",
                        JsonConvert.SerializeObject(woGridProperties)
                    );

                    if (_woGridDesigner != null)
                    {
                        _woGridDesigner.BlockMode();
                        TabGridDesigner.PageVisible = false;
                        _woGridDesigner.Dispose();
                        _woGridDesigner = null;
                    }
                    TabGridDesigner.PageVisible = true;

                    _woGridDesigner = new WoGridDesigner(
                        saveFolder: "Grids",
                        modelName: _modelName
                    );
                    _woGridDesigner.Dock = DockStyle.Fill;
                    pnlGridDesigner.Controls.Add(_woGridDesigner);
                    _woGridDesigner.ModeChanged += ChangeDesignMode;
                    _woGridDesigner.AddButtonEvt += InitializeWoGridSyntaxEditor;
                    _woGridDesigner.DeleteButtonEvt += InitializeWoGridSyntaxEditor;

                    _woGridDesigner.GridUpdatingEvt += UpdatingGrid;

                    _isSlave = true;

                    var tab = tabBaseDesigner.TabPages.FirstOrDefault(tp =>
                        tp.Text == "*Esclava" || tp.Text == "Esclava"
                    );
                    if (tab != null)
                    {
                        tabBaseDesigner.SelectedTabPage = tab;
                    }
                }
            }
            else if (selectedTabName == "TabListDesigner")
            {
                if (
                    XtraMessageBox.Show(
                        "Se perderán todos los cambios realizados en la lista hasta ahora. ¿Está seguro de querer restaurar la lista a su estado inicial?",
                        "Restaurar lista",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.Yes
                )
                {
                    if (
                        File.Exists(
                            $@"{proyecto.DirLayOuts}\ListDesign\{_modelSelected.Id}GridList.json"
                        )
                    )
                    {
                        WoDirectory.DeleteFile(
                            $@"{proyecto.DirLayOuts}\ListDesign\{_modelSelected.Id}GridList.json"
                        );
                    }

                    if (
                        File.Exists(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        )
                    )
                    {
                        string json = File.ReadAllText(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        );
                        List<WoCustomButtonProperties> customButtons =
                            JsonConvert.DeserializeObject<List<WoCustomButtonProperties>>(json);

                        WoSyntaxManagerUserCode userCode = new WoSyntaxManagerUserCode();
                        userCode.InitializeManager(
                            pathScript: $@"{proyecto.DirLayOuts}\UserCode\{_modelName}GridList_proj\{_modelName}GridListScriptsUser.cs",
                            className: $@"{_modelName}GridList",
                            modelName: _modelName
                        );

                        foreach (WoCustomButtonProperties customButton in customButtons)
                        {
                            if (
                                customButton.ButtonId != "BtnEdit"
                                && customButton.ButtonId != "BtnCopyToNew"
                            )
                            {
                                userCode.DeleteMethod($@"{customButton.ButtonId}_OnClick");
                            }
                            else if (
                                customButton.ButtonId == "BtnEdit"
                                || customButton.ButtonId == "BtnCopyToNew"
                            )
                            {
                                userCode.DeleteMethod($@"{customButton.ButtonId}_OnClick");
                            }
                        }

                        WoDirectory.DeleteFile(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        );
                    }

                    if (
                        !File.Exists(
                            $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListCustomButtons.json"
                        )
                    )
                    {
                        if (!_isExtencion)
                        {
                            WoCustomButtonsRawHelper woCustomButtonsRawHelper =
                                new WoCustomButtonsRawHelper();
                            woCustomButtonsRawHelper.BuildRawGridList(_modelSelected.Id);
                        }

                        InitializeWoGridSyntaxEditor();
                    }

                    WoGridDesignerRawHelper woGridDesignerRawHelper = new WoGridDesignerRawHelper();
                    WoGridProperties woGridProperties = woGridDesignerRawHelper.GetRawGrid(
                        _modelSelected.Id,
                        isSlave: false
                    );

                    WoDirectory.WriteFile(
                        $@"{proyecto.DirLayOuts}\ListDesign\{_modelSelected.Id}GridList.json",
                        JsonConvert.SerializeObject(woGridProperties)
                    );

                    InitializeListDesigner();
                }
            }
        }

        #endregion Limpieza completa

        #region Copy de los ficheros

        /// <summary>
        /// Copiar el dll del fichero de generación a la carpeta de assemblies
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void btnUpdateClient_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string pathOld =
                    $@"{proyecto.DirApplication}\WebClient\WooW.WebClient\bin\Debug\net8.0\WooW.WebClient.dll";

                if (File.Exists(pathOld))
                {
                    string pathNew = $@"{proyecto.DirProyectData}\Assembly\WooW.WebClient.dll";

                    WoDirectory.CopyFile(pathOld, pathNew, true);
                }
                else
                {
                    XtraMessageBox.Show(
                        $"Primero compile el cliente.\n\r No se encontró el ensamblado de la bibliotecario del cliente en la ruta: {pathOld}",
                        $@"Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Primero compile el cliente.",
                    $@"Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Copy de los ficheros

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
            //woBlazorGenerator.GenerateBaseServer("ServerUnitReport_proj", isUnit: true);
            //woBlazorGenerator.GenerateODataReport("Banco", "ServerUnitReport", isServer: true);

            //woBlazorGenerator.GenerateBaseWasm("WasmUnitReport_proj", isUnit: true);
            //woBlazorGenerator.GenerateODataReport("Banco", "WasmUnitReport", isServer: false);
        }

        #region Eliminar diseño

        /// <summary>
        /// Elimina diseño y código del modelo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDeleteDesign_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult result = DialogResult.Yes;
            result = XtraMessageBox.Show(
                $@"Se eliminara el diseño del modelo, lista, el código del proyecto y ya no se mostrara en el diseñador de menus.
¿Seguro que desea continuar?",
                "Alert",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string pathForm = $@"{proyecto.DirLayOuts}\FormDesign\{_modelName}.json";
                    if (File.Exists(pathForm))
                    {
                        WoDirectory.DeleteFile(pathForm);
                    }

                    string pathGrid = $@"{proyecto.DirLayOuts}\Grids\{_modelName}GridList.json";
                    if (File.Exists(pathGrid))
                    {
                        WoDirectory.DeleteFile(pathGrid);
                    }

                    string pathList =
                        $@"{proyecto.DirLayOuts}\ListDesign\{_modelName}GridList.json";
                    if (File.Exists(pathList))
                    {
                        WoDirectory.DeleteFile(pathList);
                    }

                    string pathListAuxTemp = $@"{proyecto.DirLayOuts}\ListDesign\{_modelName}.json";
                    if (File.Exists(pathListAuxTemp))
                    {
                        WoDirectory.DeleteFile(pathListAuxTemp);
                    }

                    string pathCode = $@"{proyecto.DirLayOuts}\UserCode\{_modelName}_proj";
                    if (Directory.Exists(pathCode))
                    {
                        WoDirectory.DeleteDirectory(pathCode);
                    }

                    string pathCodeGrid =
                        $@"{proyecto.DirLayOuts}\UserCode\{_modelName}GridList_proj";
                    if (Directory.Exists(pathCodeGrid))
                    {
                        WoDirectory.DeleteDirectory(pathCodeGrid);
                    }

                    List<string> integralGenerations = WoDirectory.ReadDirectoryFiles(
                        $@"{proyecto.DirLayOuts}\IntegralGenerations"
                    );
                    foreach (string generation in integralGenerations)
                    {
                        List<WoModelSelected> modelsFromJsonCol = JsonConvert.DeserializeObject<
                            List<WoModelSelected>
                        >(WoDirectory.ReadFile(generation));
                        if (
                            modelsFromJsonCol.FirstOrDefault(model => model.ModelName == _modelName)
                            != null
                        )
                        {
                            List<WoModelSelected> newList = modelsFromJsonCol
                                .Where(model => model.ModelName != _modelName)
                                .ToList();
                            WoDirectory.WriteFile(generation, JsonConvert.SerializeObject(newList));
                        }
                    }

                    DataRow dataRow = grdModelosView.GetFocusedDataRow();
                    dataRow["Diseñado"] = false;

                    grdModelosView.FocusedRowHandle = 0;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $@"Ocurrió un error al eliminar el diseño: {ex.Message}",
                        $@"Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        #endregion Eliminar diseño

        #region CODIGO TEMPORAL

        /// <summary>
        /// Código que se ocupa de actualizar los using de el modelo seleccionado
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void UpdateUsings()
        {
            string pathForm =
                $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}_proj\{_modelSelected.Id}Usings.cs";
            UpdateUsignsFile(pathForm);

            string pathGrid =
                $@"{proyecto.DirLayOuts}\UserCode\{_modelSelected.Id}GridList_proj\{_modelSelected.Id}GridListUsings.cs";
            UpdateUsignsFile(pathGrid);

            List<ModeloColumna> slaveModels = _modelSelected
                .Columnas.Where(col => col.TipoControl == WoTypeControl.CollectionEditor)
                .ToList();
            foreach (ModeloColumna slave in slaveModels)
            {
                string slavePath =
                    $@"{proyecto.DirLayOuts}\UserCode\{slave.ModeloId}_proj\{slave.ModeloId}Usings.cs";
                UpdateUsignsFile(slavePath);

                string slavePathGrid =
                    $@"{proyecto.DirLayOuts}\UserCode\{slave.ModeloId}GridList_proj\{slave.ModeloId}GridListUsings.cs";
                UpdateUsignsFile(slavePathGrid);
            }
        }

        private void UpdateUsignsFile(string pathUsings)
        {
            if (File.Exists(pathUsings))
            {
                string usingText = WoDirectory.ReadFile(pathUsings);
                if (usingText.Contains("using WooW.Blazor.UI."))
                {
                    string newUsing = usingText
                        .Replace("using WooW.Blazor.UI.WoResources;", "")
                        .Replace("using WooW.Blazor.UI.WoForm.Form;", "")
                        .Replace("using WooW.Blazor.UI.WoForm.FormGroup;", "")
                        .Replace("using WooW.Blazor.UI.WoForm.FormTabs;", "")
                        .Replace("using WooW.Blazor.UI.WoForm.FormTab;", "")
                        .Replace("using WooW.Blazor.UI.WoForm.FormItem;", "")
                        .Replace("using WooW.Blazor.UI.WoForm.CommonForm;", "")
                        .Replace("using WooW.Blazor.UI.WoButtons.Button;", "")
                        .Replace("using WooW.Blazor.UI.WoButtons.ShareButton;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.CheckBox;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.TextEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.MemoEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.MaskedEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.SpinEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.DateEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.ComboEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.ComboEnumEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.LookupEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoInputs.LookupDialogEdit;", "")
                        .Replace("using WooW.Blazor.UI.WoAlerts.InputAlert;", "")
                        .Replace("using WooW.Blazor.UI.WoAlerts.FormAlert;", "")
                        .Replace("using WooW.Blazor.UI.WoToolbars.ListToolbar;", "")
                        .Replace(
                            "using WooW.Blazor.UI.WoToolbars.ToolbarItem.SimpleToolbarItem;",
                            ""
                        )
                        .Replace(
                            "using WooW.Blazor.UI.WoToolbars.ToolbarItem.CustomToolbarItem;",
                            ""
                        )
                        .Replace("using WooW.Blazor.UI.WoGrids.DetailGrid;", "")
                        .Replace("using WooW.Blazor.UI.WoGrids.ListGrid;", "")
                        .Replace("using WooW.Blazor.UI.WoGrids.GridPager;", "")
                        .Replace("using WooW.Blazor.UI.WoReportViewer;", "")
                        .Replace("using WooW.Blazor.UI.WoButtons.ShareButtonForReports;", "")
                        .Replace("using WooW.Blazor.Services.WoLookUpServices;", "");

                    newUsing += "using WooW.Blazor.Resources;";

                    WoDirectory.WriteFile(pathUsings, newUsing);
                }
            }
        }

        #endregion CODIGO TEMPORAL
    }
}
