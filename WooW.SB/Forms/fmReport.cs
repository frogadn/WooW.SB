using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using ServiceStack;
using WooW.Core;
using WooW.Core.Common.Exceptions;
using WooW.SB.BlazorGenerator;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.SB.ManagerDirectory;
using WooW.SB.Reportes;
using WooW.SB.Reportes.ReportModels;
using WooW.SB.Reportes.ReportsComponents;

namespace WooW.SB.Forms
{
    public partial class fmReport : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        #region Variables
        public WooWConfigParams wooWConfigParams { get; set; }

        public Proyecto proyecto { get; set; }
        private WoReportsDesigner _report { get; set; }
        private string _templateNew = "Seleccione..";
        private string _viewNew = "Seleccione..";

        public bool Excecute = false;
        #endregion Variables

        public fmReport()
        {
            InitializeComponent();
            try
            {
                repositoryItemComboBox4.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;
                repositoryItemComboBox5.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;
                repositoryItemComboBox6.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;

                proyecto = Proyecto.getInstance();

                //Verifica si la carpeta contenedora de los reportes existe
                if (!Directory.Exists($"{proyecto.DirReportesTemp}"))
                {
                    Directory.CreateDirectory($"{proyecto.DirReportesTemp}");
                }
                if (!Directory.Exists($@"{proyecto.DirMenusTemp}"))
                {
                    Directory.CreateDirectory($@"{proyecto.DirMenusTemp}");
                }
                //Verifica si la carpeta contenedora de los plantillas para los reportes existe
                if (!Directory.Exists($"{proyecto.DirPlantillasReportes}"))
                {
                    Directory.CreateDirectory($"{proyecto.DirPlantillasReportes}");
                }
                //Verifica si la carpeta contenedora de los plantillas para los reportes existe
                if (!Directory.Exists($"{proyecto.DirVistasReports}"))
                {
                    Directory.CreateDirectory($"{proyecto.DirVistasReports}");
                }
                // Copia los reportes
                CopyReportsTemp();
                // Obtenemos la lista de archivos en el directorio de origen
                string[] reportsFiles = Directory.GetFiles(proyecto.DirVistasReports, "*.xml");

                foreach (string archivo in reportsFiles)
                {
                    string nombreArchivo = Path.GetFileName(archivo);
                    string destinoArchivo = Path.Combine(proyecto.DirReportesTemp, nombreArchivo);

                    // Verificamos si el archivo ya existe en el directorio de destino
                    if (!File.Exists(destinoArchivo))
                    {
                        // Copiamos el archivo al directorio de destino
                        File.Copy(archivo, destinoArchivo);
                    }
                }

                //buPrevio.Enabled = false;
                _report = new WoReportsDesigner();
                pnlReportes.AddControl(_report);
                _report.Dock = DockStyle.Fill;
                //Controladores de eventos
                _report.buttonStopProcess += StateStopExeButtons;
                _report.EventsLoadTemplate += LoadTemplate;
                _report.DisableReports += DisableReports;
                _report.DisableBuRun += DisableBuRunReports;
                _report.EnableReports += EnableReports;
                _report.EventsChangeAuth += ChangeAuth;
                _report.EventsBuRun += ButtonRun;
                _report.EventsbuEditar += StateBuEdit;
                _report.buGenerateBlazor += StatebuGenerateBlazor;
                _report.EventsSetStatusBtnsRowsChanged += SetStatusBtnsRowsChanged;
                _report.EventSetTemplateValue += SetValueTemplate;
                _report.EventSetViewValue += SetValueView;
                _report.buEditView += StatebuEditView;
                _report.EventsViews += LoadViews;
                _report.BuExcecute += ButtonExcWatch;
                _report.SetValuCBTemplates += SetValueTemplateCB;
                _report.woGridodataReports1.EventsViews += LoadViews;
                _report.woGridodataReports1.StopProccess += StopProcess;
                _report.woGridodataReports1.DisableReports += DisableReports;
                _report.woGridodataReports1.EnableReports += EnableReports;
                _report.woGridodataReports1.BuExcecute += ButtonExcWatch;
                _report.EnablePrevio(false);

                StateStopExeButtons(null, false);

                SetSelectedOption();

                var a = _report.grdReportesView.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                throw new WoObserverException("Error: ", ex.Message);
            }
        }

        private static volatile fmReport uniqueInstance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// Singleton para la instancia de la clase
        /// </summary>
        /// <returns></returns>
        public static fmReport getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (padlock)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new fmReport();
                    }
                }
            }
            return uniqueInstance;
        }

        #region Carga de datos


        ///// <summary>
        ///// Función para cargar el combobox con las vistas existentes
        ///// </summary>
        public void LoadViews(object sender, WoReport reportFocus)
        {
            RepositoryItemComboBox ComboBoxEdit = cmbVistas.Edit as RepositoryItemComboBox;
            if (reportFocus != null)
            {
                ComboBoxEdit.Items.Clear();
                ComboBoxEdit.Items.Add(_viewNew);

                List<string> repViews1 = new List<string>();

                dynamic views = Directory
                    .GetFiles(proyecto.DirVistasReports, "*.xml")
                    .Where(x => x.Contains(reportFocus.idReport));
                foreach (string file in views)
                {
                    var _reportPath = proyecto.DirVistasReports + "\\" + reportFocus.idReport;
                    if (file.Equals(_reportPath))
                    {
                        repViews1.Add(Path.GetFileNameWithoutExtension(file));
                    }
                    var reportPathViewExtra =
                        proyecto.DirVistasReports + "\\" + reportFocus.idReport + ".";
                    if (file.StartsWith(reportPathViewExtra))
                    {
                        repViews1.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }
                ComboBoxEdit.Items.AddRange(repViews1);

                cmbVistas.EditValue = _report._viewNew;
                buEditarVista.Enabled = false;
                if (cmbTemplates.EditValue != _templateNew)
                {
                    btnNewView.Enabled = true;
                }
            }
            else
            {
                ComboBoxEdit.Items.Clear();
                ComboBoxEdit.Items.Add(_viewNew);
                cmbVistas.EditValue = _report._viewNew;
                buEditarVista.Enabled = false;
                btnNewView.Enabled = false;
            }
        }

        #endregion Carga de datos



        /// <summary>
        /// Función para desactivar o activar los botos al seleccionar un nuevo row
        /// </summary>
        public void SetStatusBtnsRowsChanged(object sender, bool status)
        {
            //buEditar.Enabled = status;
            //buPrevio.Enabled = status;
        }

        /// <summary>
        /// Implementación de la interfaz IForm
        /// </summary>
        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        /// <summary>
        /// Implementación de la interfaz IForm
        /// </summary>
        public bool CambiosPendientes
        {
            get { return false; }
        }

        /// <summary>
        /// Implementación de la interfaz IForm
        /// </summary>
        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        Proyecto IForm.proyecto { get; set; }

        public void Refrescar()
        {
            // ToDo Refrescar
            //if (buRefrescar.Enabled)
            //    buRefrescar.PerformClick();
        }

        /// <summary>
        /// Implementación de la interfaz IForm
        /// </summary>
        public void Cargar() { }

        private async Task DeleteView()
        {
            try
            {
                string Archivo =
                    $"{proyecto.DirVistasReports}\\{cmbVistas.EditValue.ToSafeString()}.xml";
                File.Delete(Archivo);
                Archivo = $"{proyecto.DirReportesTemp}\\{cmbVistas.EditValue.ToSafeString()}.xml";
                File.Delete(Archivo);

                if (_report.xtraTabSelected == "ODATA")
                {
                    dynamic data = _report.woGridodataReports1.grdModelsView.DataSource;
                    List<string> ReportsCreated = new List<string>();
                    string[] archivos = Directory.GetFiles(proyecto.DirVistasReports, "*.xml");
                    // Obtén solo los nombres de archivo
                    string[] nombresArchivos = archivos
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                    foreach (var files in nombresArchivos)
                    {
                        // Dividir la cadena por el punto
                        string[] partes = files.Split('.');
                        ReportsCreated.Add(partes[0]);
                    }
                    foreach (WoReport row in data)
                    {
                        dynamic viewsCreate = ReportsCreated
                            .Where(report => report == (row.idReport))
                            .ToList();

                        if (viewsCreate.Count > 0)
                        {
                            row.reportDesign = true;
                        }
                        else
                        {
                            row.reportDesign = false;
                        }
                    }
                    _report.woGridodataReports1.grdModelsView.RefreshData();
                }
                if (_report.xtraTabSelected == "Servicio")
                {
                    dynamic data = _report.grdReportesView.DataSource;
                    List<string> ReportsCreated = new List<string>();
                    string[] archivos = Directory.GetFiles(proyecto.DirVistasReports, "*.xml");
                    // Obtén solo los nombres de archivo
                    string[] nombresArchivos = archivos
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                    foreach (var files in nombresArchivos)
                    {
                        // Dividir la cadena por el punto
                        string[] partes = files.Split('.');
                        ReportsCreated.Add(partes[0]);
                    }
                    foreach (WoReport row in data)
                    {
                        dynamic viewsCreate = ReportsCreated
                            .Where(report => report == (row.idReport))
                            .ToList();

                        if (viewsCreate.Count > 0)
                        {
                            row.reportDesign = true;
                        }
                        else
                        {
                            row.reportDesign = false;
                        }
                    }
                    _report.grdReportesView.RefreshData();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region vistas

        #region Edición de vistas
        /// <summary>
        /// vista actual
        /// </summary>
        private string _currentView;

        /// <summary>
        /// Botón de edicion de una vista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buEditarVista_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if ((cmbVistas.EditValue.ToSafeString() != _viewNew))
            {
                _report.SetReportFm_txtVista(cmbVistas.EditValue.ToString());
                string auxView = cmbVistas.EditValue.ToString();
                _report.BuEditView(sender, e);
                SetFocusRefreshComboViews();
                _report.SetValues();
                cmbVistas.EditValue = (auxView);
            }
        }

        /// <summary>
        /// función auxiliar para actualizar el combobox de vistas
        /// y seleccionar la que se creo o edito
        /// </summary>
        public async Task SetFocusRefreshComboViews()
        {
            WoReport reportFocus = new WoReport();
            if (_report.xtraTabSelected == "ODATA")
            {
                reportFocus = _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
            }
            else
            {
                reportFocus = _report.grdReportesView.GetFocusedRow() as WoReport;
            }
            LoadViews(reportFocus.idReport, reportFocus);
            if (_report.currentView != string.Empty)
            {
                cmbVistas.EditValue = _report.currentView;
            }
        }
        #endregion Edición de vistas

        #region Creación de vistas

        /// <summary>
        /// Función para crear una nueva vista del reporte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem1_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            DialogResult dialog = _report.BuEditView(sender, e, true);
            if (dialog == DialogResult.OK)
            {
                WoReport reportFocused = new WoReport();
                if (_report.xtraTabSelected == "ODATA")
                {
                    reportFocused =
                        _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
                    _report.woGridodataReports1.SetValues();
                    LoadViews(sender, reportFocused);
                    cmbVistas.EditValue = _report.currentView;
                    SetFocusRefreshComboViews();
                }
                if (_report.xtraTabSelected == "Servicio")
                {
                    reportFocused = _report.grdReportesView.GetFocusedRow() as WoReport;
                    LoadViews(sender, reportFocused);
                    cmbVistas.EditValue = _report.currentView;
                    SetFocusRefreshComboViews();
                    _report.buRun(sender, e);
                }

                //Actualizamos el checkbox de vistas
                if (_report.xtraTabSelected == "ODATA")
                {
                    dynamic data = _report.woGridodataReports1.grdModelsView.DataSource;
                    List<string> ReportsCreated = new List<string>();
                    string[] archivos = Directory.GetFiles(proyecto.DirVistasReports, "*.xml");
                    // Obtén solo los nombres de archivo
                    string[] nombresArchivos = archivos
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                    foreach (var files in nombresArchivos)
                    {
                        // Dividir la cadena por el punto
                        string[] partes = files.Split('.');
                        ReportsCreated.Add(partes[0]);
                    }
                    foreach (WoReport row in data)
                    {
                        dynamic viewsCreate = ReportsCreated
                            .Where(report => report == (row.idReport))
                            .ToList();

                        if (viewsCreate.Count > 0)
                        {
                            row.reportDesign = true;
                        }
                        else
                        {
                            row.reportDesign = false;
                        }
                    }
                    _report.woGridodataReports1.grdModelsView.RefreshData();
                    RepositoryItemComboBox ComboBoxEdit = cmbVistas.Edit as RepositoryItemComboBox;
                    if (ComboBoxEdit.Items.Count > 1)
                    {
                        cmbVistas_EditValueChanged(e, sender);
                    }
                }
                if (_report.xtraTabSelected == "Servicio")
                {
                    dynamic data = _report.grdReportesView.DataSource;
                    List<string> ReportsCreated = new List<string>();
                    string[] archivos = Directory.GetFiles(proyecto.DirVistasReports, "*.xml");
                    // Obtén solo los nombres de archivo
                    string[] nombresArchivos = archivos
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                    foreach (var files in nombresArchivos)
                    {
                        // Dividir la cadena por el punto
                        string[] partes = files.Split('.');
                        ReportsCreated.Add(partes[0]);
                    }
                    foreach (WoReport row in data)
                    {
                        dynamic viewsCreate = ReportsCreated
                            .Where(report => report == (row.idReport))
                            .ToList();

                        if (viewsCreate.Count > 0)
                        {
                            row.reportDesign = true;
                        }
                        else
                        {
                            row.reportDesign = false;
                        }
                    }
                    _report.grdReportesView.RefreshData();
                }
            }
        }
        #endregion Creación de vistas

        #endregion vistas

        #region Plantillas

        #region Edición de plantillas

        ///// <summary>
        ///// Función para la creación o edición de la plantilla del reporte
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        private void buPlantillaEditar_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            _report.SetReportFm_txtPlantilla(cmbTemplates.EditValue.ToString());

            _report.BuEditTemplate(sender, e);
        }

        /// <summary>
        /// Funcion auxiliar para actualizar el valor seleccionado del combo de plantillas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void SetValueTemplateCB(object sender, string value)
        {
            cmbTemplates.EditValue = value; //_report.currentTemplate;
        }

        #endregion Edición de plantillas

        #region Creación de plantillas
        /// <summary>
        /// Función para crear una nueva plantilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem2_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            _report.SetReportFm_txtPlantilla(
                cmbTemplates.EditValue == null ? "" : cmbTemplates.EditValue.ToString()
            );
            WoReport reportFocused = new WoReport();
            if (_report.xtraTabSelected == "ODATA")
            {
                reportFocused =
                    _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
            }
            else
            {
                reportFocused = _report.grdReportesView.GetFocusedRow() as WoReport;
            }
            cmbVistas.Refresh();
            LoadTemplate(sender, reportFocused);
            _report.BuEditTemplate(sender, e, true);
        }

        /// <summary>
        /// función auxiliar para actualizar el combobox de plantillas
        /// y seleccionar la que se creo o edito
        /// </summary>
        public void SetFocusRefreshComboTemplates()
        {
            LoadTemplate(_report.currentTemplate, null);
            if (_report.currentTemplate != string.Empty)
            {
                cmbTemplates.EditValue = _report.currentTemplate;
            }
        }
        #endregion Creación de plantillas

        #endregion Plantillas


        #region Funciones Auxiliares

        private void ButtonExcWatch()
        {
            string selectedOption = cmbEjex.EditValue.ToString();
            if (selectedOption == "Watch Server" || selectedOption == "Watch Wasm")
            {
                if (Excecute == true)
                {
                    btnExecute.PerformClick();
                }
            }
        }

        /// <summary>
        /// Funcion auxiliar para ejecutar el boton Run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRun(object sender, object e)
        {
            buRun.PerformClick();
        }

        /// <summary>
        /// Función para ejecutar el blazor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem1_ItemClick_1(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            WoBlazorExecute _woExecute = WoBlazorExecute.GetInstance();
            WoReport reportView = new WoReport();
            if (_report.xtraTabSelected == "ODATA")
            {
                //reportView = _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
            }
            else
            {
                reportView = _report.grdReportesView.GetFocusedRow() as WoReport;
            }
            _woExecute.StartBlazor(reportView.Etiqueta);
        }

        /// <summary>
        /// bandera para no volver a ejecutar la función de cambio de vista
        /// </summary>
        private bool _firstOpen = true;

        /// <summary>
        /// Función para gestionar el estatus del botón de edición de plantillas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        public void StateBuEdit(object sender, bool state)
        {
            buEditar.Enabled = state;
            buRun.Enabled = state;
        }

        /// <summary>
        /// Función para gestionar el estatus del botón de edición de plantillas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        public void StateStopExeButtons(object sender, bool state)
        {
            btnExecute.Enabled = true;
            btnStop.Enabled = false;
        }

        /// <summary>
        /// Función para gestionar el estatus del botón de "Generar proyecto blazor"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        public void StatebuGenerateBlazor(object sender, bool state)
        {
            //buPrevio.Enabled = state;
        }

        /// <summary>
        /// Función para gestionar el estatus del botón de "Editar" de la vista previa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        public void StatebuEditView(object sender, bool state)
        {
            buEditarVista.Enabled = state;
            btnNewView.Enabled = state;
        }

        /// <summary>
        /// Función auxiliar para setear el valor de la plantilla seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetValueTemplate(object sender, dynamic e)
        {
            _report.SetReportFm_txtPlantilla(
                cmbTemplates.EditValue == null ? "" : cmbTemplates.EditValue.ToString()
            );
        }

        /// <summary>
        /// Función auxiliar para setear el valor de la plantilla seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetValueView(object sender, dynamic e)
        {
            //_report.SetReportFm_txtVista(
            //    cmbVistas.EditValue == null ? "" : cmbVistas.EditValue.ToString()
            //);
        }

        /// <summary>
        /// Funcion auxiliar para asignar el valor de la plantilla seleccionada a
        /// la variable usada en woReportDesigner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPlantilla_EditValueChanged(object sender, EventArgs e)
        {
            SetValueTemplate(sender, e);
            if (cmbTemplates.EditValue != _templateNew)
            {
                if (cmbVistas.EditValue == _viewNew)
                {
                    buEditarVista.Enabled = false;
                }
                btnNewView.Enabled = true;
                buPlantillaEditar.Enabled = true;
                if (_report.xtraTabSelected == "ODATA")
                {
                    buEditar.Enabled = false;
                }
                else
                {
                    buEditar.Enabled = true;
                }
            }
            else
            {
                cmbVistas.EditValue = _viewNew;
                buEditarVista.Enabled = false;
                btnNewView.Enabled = false;
                buPlantillaEditar.Enabled = false;
            }
        }

        /// <summary>
        /// Funcion auxiliar para asignar el valor de la vista seleccionada a la variable usada en woReportDesigner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtVistasReports_EditValueChanged(dynamic sender, EventArgs e)
        {
            if (cmbVistas.EditValue == _viewNew)
            {
                buEditarVista.Enabled = false;
                _report.woGridodataReports1.tabODataPrevio.PageEnabled = false;
                _report.woGridodataReports1.tabExplorador.SelectedTabPage = _report
                    .woGridodataReports1
                    .tabOData;
            }
            else if (cmbVistas.EditValue != _viewNew)
            {
                buEditarVista.Enabled = true;
                _report.woGridodataReports1.tabODataPrevio.PageEnabled = true;
            }

            //SetValueView(sender, e);
        }
        #endregion Funciones Auxiliares

        #region Funciones que utilizados por el controlador de eventos
        /// <summary>
        /// Función para cargar el combo box con las plantillas existentes
        /// </summary>
        public void LoadTemplate(object sender, dynamic e)
        {
            RepositoryItemComboBox ComboBoxEdit = cmbTemplates.Edit as RepositoryItemComboBox;
            ComboBoxEdit.Items.Clear();
            ComboBoxEdit.Items.Add(_templateNew);

            foreach (string file in Directory.GetFiles(proyecto.DirPlantillasReportes, "*.xml"))
            {
                ComboBoxEdit.Items.Add(Path.GetFileNameWithoutExtension(file));
            }

            cmbTemplates.EditValue = _templateNew;
        }

        /// <summary>
        /// función auxiliar para deshabilitar los controles en caso de no existir reportes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="estatus"></param>
        public void DisableReports(object sender, dynamic e)
        {
            LoadTemplate(null, null);
            LoadViews(null, null);
            buNewTemplate.Enabled = false;
            buRun.Enabled = false;
            btnExecute.Enabled = false;
            buRun.Enabled = false;
            cmbTemplates.Enabled = false;
            cmbVistas.Enabled = false;
            cmbEjex.Enabled = false;
            btnPropReports.Enabled = false;
        }

        /// <summary>
        /// función auxiliar para deshabilitar el boton de obtener datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="estatus"></param>
        public void DisableBuRunReports()
        {
            buRun.Enabled = false;
        }

        /// <summary>
        /// función auxiliar para habilitar los controles en caso de no existir reportes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="estatus"></param>
        public void EnableReports(object sender, dynamic e)
        {
            buNewTemplate.Enabled = true;
            buRun.Enabled = true;
            btnExecute.Enabled = true;
            cmbTemplates.Enabled = true;
            cmbVistas.Enabled = true;
            cmbEjex.Enabled = true;
            btnPropReports.Enabled = true;
        }

        /// <summary>
        /// Función que nos habilita algunos botones al autentiocarse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status"></param>
        private void ChangeAuth(object sender, bool status)
        {
            if (status)
            {
                //controlador de eventos
                if (_report.xtraTabSelected == "Servicio")
                {
                    buRun.Enabled = true;
                }
            }
            else
            {
                buRun.Enabled = false; //Controlador!
                XtraMessageBox.Show(
                    "Ocurrió un error al intentar autenticarse, revise que el servicio se encuentre activo",
                    "Error de Autenticación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        #endregion Funciones que utilizados por el controlador de eventos

        #region Eventos

        /// <summary>
        /// Función para ejecutar y consumir el servicio del modelos seleccionado
        /// mandando los parámetros del request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buRun_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _report.SetReportFm_txtVista(cmbVistas.EditValue.ToString());
            _report.buRun(sender, e);
        }

        ///// <summary>
        ///// Función que nos permite realizar la edición y asociacion del datasource
        ///// del servicio antes consumido al reporte.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ///Editar
            WoReport reportFocused = new WoReport();
            _report.buEditar(sender, e);
            if (_report.xtraTabSelected == "ODATA")
            {
                //reportFocused =
                //    _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
            }
            else
            {
                reportFocused = _report.grdReportesView.GetFocusedRow() as WoReport;
            }

            LoadViews(sender, reportFocused);
            LoadTemplate(sender, reportFocused);
        }
        #endregion Eventos


        #region Variables principales del proyecto generado

        /// <summary>
        /// Nombre del proyecto de blazor server de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _serverProjectName = "ServerUnitReport_proj";

        ///// <summary>
        ///// Nombre base de las clases que contendran las pruebas unitarias en claso de ser para server.
        ///// </summary>
        private string _serverClassModelName = "ServerUnitReport";

        ///// <summary>
        ///// Nombre base de las clases que contendran las pruebas unitarias en claso de ser para server.
        ///// </summary>
        private string _serverClassModelNameOData = "ServerUnitReport";

        /// <summary>
        /// Nombre del proyecto de blazor wasm de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _wasmProjectName = "WasmUnitReport_proj";

        /// <summary>
        /// Nombre del proyecto de blazor wasm de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _wasmProjectNameOdata = "WasmUnitReport_proj";

        ///// <summary>
        ///// Nombre base de las clases que contendran las pruebas unitarias en claso de ser para wasm.
        ///// </summary>
        private string _wasmClassModelName = "WasmUnitReport";

        ///// <summary>
        ///// Nombre base de las clases que contendran las pruebas unitarias en claso de ser para wasm.
        ///// </summary>
        private string _wasmClassModelNameOData = "WasmUnitReport";

        ///// <summary>
        ///// Define el nombre de la clase de la que se creara el formulario.
        ///// </summary>
        //private string _modelName = "";

        #endregion Variables principales del proyecto generado

        #region Ejecucion y Stop de blazor
        /// <summary>
        /// Ejecuta el blazor en caso de ya existir.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void barButtonItem8_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            Excecute = true;
            _report.consoleData.Text = string.Empty;
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();

            string pathProyectUnit = string.Empty;
            WoReport reportFocused = new WoReport();
            if (_report.xtraTabSelected == "ODATA")
            {
                reportFocused =
                    _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
            }
            else
            {
                reportFocused = _report.grdReportesView.GetFocusedRow() as WoReport;
            }

            string[] archivos = Directory.GetFiles(proyecto.DirVistasReports, "*.xml");

            var reports = archivos
                .Where(archivo => Path.GetFileName(archivo).Contains(reportFocused.idReport))
                .ToList();
            if (reports.Count == 0)
            {
                XtraMessageBox.Show(
                    $@"El modelo seleccionado no contiene reportes, favor de generar alguno.",
                    $@"Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            // Copia los reportes
            CopyReportsTemp();

            pathProyectUnit =
                $@"{proyecto.DirProyectTemp}/{_serverProjectName}/{_serverProjectName}.csproj";

            string selectedOption = cmbEjex.EditValue.ToString();

            if (selectedOption != "Eliminar Generados")
            {
                if (_report.xtraTabSelected == "ODATA")
                {
                    GenerateServer("ODATA");
                    GenerateWasm("ODATA");
                }
                if (_report.xtraTabSelected == "Servicio")
                {
                    GenerateServer("Servicio");
                    GenerateWasm("Servicio");
                }
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
            else if (selectedOption == "Solo Server y Wasm")
            {
                ExecuteBlazorServer();
                ExecuteBlazorWasm();
                btnExecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Watch Server y Wasm")
            {
                WatchBlazorServer();
                WatchBlazorWasm();
                btnExecute.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (selectedOption == "Eliminar Generados")
            {
                DeleteGeneratedProyects();
            }
        }

        /// <summary>
        /// Metodo que detiene la ejecucion de blazor server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem10_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            Excecute = false;
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();
            btnExecute.Enabled = true;
            btnStop.Enabled = false;

            _report.consoleData.SelectionColor = Color.Green;
            _report.consoleData.AppendText("Proyecto detenido con exito.");

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
        /// Función para parar los procesos de blazor server y wasm.
        /// </summary>
        public void StopProcess(object sender, EventArgs e)
        {
            _woBlazorWasmExecute.GeneralStop();
            _woBlazorServerExecute.GeneralStop();
            btnExecute.Enabled = true;
            btnStop.Enabled = false;
        }

        /// <summary>
        /// Función auxiliar para copiar los archivos de los reportes a la carpeta temporal del proyecto.
        /// </summary>
        public void CopyReportsTemp()
        {
            // Copia los reportes
            foreach (string Fuente in Directory.GetFiles($"{proyecto.DirVistasReports}", "*.xml"))
            {
                string Destino =
                    $"{proyecto.DirApplication_WebService_WebService_Reports}\\{Path.GetFileName(Fuente)}";
                if (!File.Exists(Destino))
                {
                    File.Copy(Fuente, Destino);
                }
            }
        }
        #endregion Ejecucion y Stop de blazor

        #region Envio de la data a la consola

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
            _report.consoleData.Invoke(
                new Action(() =>
                {
                    if (Data.Contains("Building") || Data.Contains("building"))
                    {
                        _report.consoleData.SelectionColor = Color.LightBlue;
                    }
                    else if (Data.Contains("Warning") || Data.Contains("warning"))
                    {
                        _report.consoleData.SelectionColor = Color.Yellow;
                    }
                    else if (
                        Data.Contains("Error")
                        || Data.Contains("error")
                        || Data.Contains("Stop")
                    )
                    {
                        _report.consoleData.SelectionColor = Color.Red;

                        if (!_sendError)
                        {
                            XtraMessageBox.Show(
                                $"Se produjo un error al intentar ejecutar el proyecto de blazor.\n\r Revise la pestaña de ejecuión para mas información",
                                "Alerta",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            _sendError = true;
                        }
                    }
                    else if (Data.Contains("Info") || Data.Contains("info"))
                    {
                        _report.consoleData.SelectionColor = Color.Green;
                    }
                    else
                    {
                        _report.consoleData.SelectionColor = Color.White;
                    }

                    _report.consoleData.AppendText($"{Data}\n\r");
                    _report.consoleData.ScrollToCaret();

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

        #endregion Envio de la data a la consola

        #region Abrir directorios

        /// <summary>
        /// Abre el explorador de archovos con la ruta de los ficheros de codigo salvados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buSaveFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process.Start("explorer.exe", $@"{proyecto.DirLayOuts}\UserCode");
        }

        /// <summary>
        /// Abre en el explorador de archivos la ruta con los proyectos generados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buOpenDirectory_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process.Start("explorer.exe", $@"{proyecto.DirApplication}\Temp");
        }

        #endregion Abrir directorios

        #region Generacion

        /// <summary>
        /// Genera el proyecto de server y el layout en el que se encuentra el usuario, solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void GenerateServer(string TabSelected)
        {
            try
            {
                string pathProyectUnit = $@"{proyecto.DirProyectTemp}/{_serverProjectName}";
                if (!File.Exists($@"{pathProyectUnit}/{_serverProjectName}.csproj"))
                {
                    WoDesignerRawReportHelper woDesignerRawReportHelper =
                        new WoDesignerRawReportHelper();
                    WoReport reportView = new WoReport();
                    if (TabSelected == "ODATA")
                    {
                        //Generación Odata
                        WoReport reportViewServerOData = new WoReport();
                        reportView =
                            _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;

                        WoBlazorGenerator woBlazorGeneratorOdata = new WoBlazorGenerator();
                        woBlazorGeneratorOdata.SendToConsole += _report.SendDataToConsole;
                        woBlazorGeneratorOdata.GenerateBaseServer(_serverProjectName, isUnit: true);
                        woBlazorGeneratorOdata.GenerateODataReport(
                            reportView.idReport,
                            _serverClassModelNameOData,
                            isServer: true,
                            labelId: _report._repSelected.LabelId
                        );
                    }
                    if (TabSelected == "Servicio")
                    {
                        reportView = _report.grdReportesView.GetFocusedRow() as WoReport;
                        // Generación Server
                        WoContainer woContainer = woDesignerRawReportHelper.BuildRawReportForm(
                            reportView.idReport,
                            labelId: _report._repSelected.LabelId
                        );
                        WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
                        woBlazorGenerator.SendToConsole += _report.SendDataToConsole;
                        pathProyectUnit = $@"{proyecto.DirProyectTemp}/{_serverProjectName}";
                        //Generamos la base del proyecto primero
                        woBlazorGenerator.GenerateBaseServer(_serverProjectName, isUnit: true);
                        woBlazorGenerator.GenerateUnitLayout(
                            container: woContainer,
                            _serverClassModelName,
                            isServer: true,
                            isReport: true
                        );
                    }
                }
                else
                {
                    WoDesignerRawReportHelper woDesignerRawReportHelper =
                        new WoDesignerRawReportHelper();
                    WoReport reportView = new WoReport();

                    if (TabSelected == "ODATA")
                    {
                        //Generación Odata
                        reportView =
                            _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;

                        WoBlazorGenerator woBlazorGeneratorOdata = new WoBlazorGenerator();
                        woBlazorGeneratorOdata.SendToConsole += _report.SendDataToConsole;
                        woBlazorGeneratorOdata.GenerateODataReport(
                            reportView.idReport,
                            _serverClassModelNameOData,
                            isServer: true,
                            labelId: _report._repSelected.LabelId
                        );
                    }
                    if (TabSelected == "Servicio")
                    {
                        // Generación Server
                        reportView = _report.grdReportesView.GetFocusedRow() as WoReport;
                        WoContainer woContainer = woDesignerRawReportHelper.BuildRawReportForm(
                            reportView.idReport,
                            labelId: _report._repSelected.LabelId
                        );
                        WoBlazorGenerator woBlazorGenerator = new WoBlazorGenerator();
                        woBlazorGenerator.SendToConsole += _report.SendDataToConsole;

                        woBlazorGenerator.GenerateUnitLayout(
                            container: woContainer,
                            _serverClassModelName,
                            isServer: true,
                            isReport: true
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                var excep = ex;
                throw;
            }
        }

        /// <summary>
        /// Genera el proyecto de wasm y el layout en el que se encuentra el usuario, solo en caso de que no exista.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void GenerateWasm(string TabSelected)
        {
            try
            {
                string pathProyectUnit = $@"{proyecto.DirProyectTemp}/{_wasmProjectName}";
                if (!File.Exists($@"{pathProyectUnit}/{_wasmProjectName}.csproj"))
                {
                    WoDesignerRawReportHelper woDesignerRawReportHelperWasm =
                        new WoDesignerRawReportHelper();
                    WoReport reportViewWasm = new WoReport();
                    if (TabSelected == "ODATA")
                    {
                        //Generación Odata
                        reportViewWasm =
                            _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;

                        WoBlazorGenerator woBlazorGeneratorOdata = new WoBlazorGenerator();
                        woBlazorGeneratorOdata.SendToConsole += _report.SendDataToConsole;
                        woBlazorGeneratorOdata.GenerateBaseWasm(
                            _wasmProjectNameOdata,
                            isUnit: true
                        );
                        woBlazorGeneratorOdata.GenerateODataReport(
                            reportViewWasm.idReport,
                            _wasmClassModelNameOData,
                            isServer: false,
                            labelId: reportViewWasm.LabelId
                        );
                    }
                    if (TabSelected == "Servicio")
                    {
                        // Generación usando Servicios
                        reportViewWasm = _report.grdReportesView.GetFocusedRow() as WoReport;
                        WoContainer woContainerWasm =
                            woDesignerRawReportHelperWasm.BuildRawReportForm(
                                reportViewWasm.idReport,
                                labelId: reportViewWasm.LabelId
                            );
                        //Generamos la base del proyecto primero
                        WoBlazorGenerator woBlazorGeneratorWasm = new WoBlazorGenerator();
                        woBlazorGeneratorWasm.SendToConsole += _report.SendDataToConsole;
                        woBlazorGeneratorWasm.GenerateBaseWasm(_wasmProjectNameOdata, isUnit: true);

                        woBlazorGeneratorWasm.SendToConsole += _report.SendDataToConsole;
                        woBlazorGeneratorWasm.GenerateUnitLayout(
                            container: woContainerWasm,
                            _wasmClassModelName,
                            isServer: false,
                            isReport: true
                        );
                    }
                }
                else
                {
                    WoDesignerRawReportHelper woDesignerRawReportHelperWasm =
                        new WoDesignerRawReportHelper();
                    WoReport reportViewWasm = new WoReport();
                    if (TabSelected == "ODATA")
                    {
                        //Generación Odata
                        reportViewWasm =
                            _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;

                        WoBlazorGenerator woBlazorGeneratorOdata = new WoBlazorGenerator();
                        woBlazorGeneratorOdata.SendToConsole += _report.SendDataToConsole;
                        woBlazorGeneratorOdata.GenerateODataReport(
                            reportViewWasm.idReport,
                            _wasmClassModelNameOData,
                            isServer: false,
                            labelId: reportViewWasm.LabelId
                        );
                    }
                    if (TabSelected == "Servicio")
                    {
                        // Generación usando Servicios
                        reportViewWasm = _report.grdReportesView.GetFocusedRow() as WoReport;
                        WoContainer woContainerWasm =
                            woDesignerRawReportHelperWasm.BuildRawReportForm(
                                reportViewWasm.idReport,
                                labelId: reportViewWasm.LabelId
                            );

                        WoBlazorGenerator woBlazorGeneratorWasm = new WoBlazorGenerator();
                        woBlazorGeneratorWasm.SendToConsole += _report.SendDataToConsole;
                        woBlazorGeneratorWasm.GenerateUnitLayout(
                            container: woContainerWasm,
                            _wasmClassModelName,
                            isServer: false,
                            isReport: true
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                var excep = ex;
                throw;
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
                _report._sendError = false;
                if (_woBlazorServerExecute.SendToConsoleEvt == null)
                {
                    _woBlazorServerExecute.SendToConsoleEvt += _report.SendDataToConsole;
                }
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
        /// Metodo suscrito a los eventos de cierre de los proces que lebantan los proyectos
        /// de blazor en caso de un cierre externo al programa.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ExternalStop(object sender, bool e)
        {
            btnExecute.Enabled = true;
            btnStop.Enabled = false;
        }

        /// <summary>
        /// Utiliza la clase de ejecucion para levantar blazor server con el watch.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private async void WatchBlazorServer()
        {
            try
            {
                _report._sendError = false;
                _woBlazorServerExecute.SendToConsoleEvt += _report.SendDataToConsole;
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
                _report._sendError = false;
                if (_woBlazorWasmExecute.SendToConsoleEvt == null)
                {
                    _woBlazorWasmExecute.SendToConsoleEvt += _report.SendDataToConsole;
                }
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
                _report._sendError = false;
                _woBlazorWasmExecute.SendToConsoleEvt += _report.SendDataToConsole;
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

        #region Tipo de ejecucion

        /// <summary>
        /// Actualiza el fichero con la persistencia para la ejecucion del formulario.
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
                cmbExecute.EditValue = selectedOption;
            }
        }

        #endregion Tipo de ejecucion

        #region Eventos del combobox de vistas

        /// <summary>
        /// Función que se ejecuta cuando el valor del combobox de vistas cambia
        /// </summary>
        private void cmbVistas_EditValueChanged(object sender, dynamic e)
        {
            RepositoryItemComboBox ComboBoxEdit = cmbVistas.Edit as RepositoryItemComboBox;
            if (cmbVistas.EditValue.ToString() == _viewNew)
            {
                buEditarVista.Enabled = false;
                _report.woGridodataReports1.EnablePrevio(false);
                _report.tabPrincipal.SelectedTabPage = _report.tabDatos;
            }
            else
            {
                buEditarVista.Enabled = true;
                if (_report.xtraTabSelected == "ODATA")
                {
                    if (ComboBoxEdit.Items.Count > 1)
                    {
                        _report.woGridodataReports1.EnablePrevio(true);
                        _report.SetReportFm_txtVista(cmbVistas.EditValue.ToString());
                        _report.SetValues();
                    }
                }
            }
        }

        #endregion Eventos del combobox de vistas

        private void barButtonItem1_ItemClick_2(object sender, ItemClickEventArgs e)
        {
            Process.Start("explorer.exe", $@"{proyecto.DirApplication}\Temp");
        }

        private void btnPropReports_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_report.xtraTabSelected == "ODATA")
            {
                var auxResportSelect = _report.woGridodataReports1.grdModelsView.FocusedRowHandle;
                fmPropReports fmprop = new fmPropReports(_report.woGridodataReports1._repSelected);
                fmprop.ShowDialog();
                _report.woGridodataReports1.woGridODATAReports_Load();
                _report.woGridodataReports1.grdModelsView.FocusedRowHandle = auxResportSelect;
            }
            if (_report.xtraTabSelected == "Servicio")
            {
                var auxResportSelect = _report.grdReportesView.FocusedRowHandle;
                fmPropReports fmprop = new fmPropReports(_report._repSelected);
                fmprop.ShowDialog();
                _report.LoadData();
                _report.grdReportesView.FocusedRowHandle = auxResportSelect;
            }
        }

        private void btnUpdateClient_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string pathOld =
                    $@"{proyecto.DirApplication}\WebClient\WooW.WebClient\bin\Debug\net6.0\WooW.WebClient.dll";

                if (File.Exists(pathOld))
                {
                    string pathNew = $@"{proyecto.DirProyectData}\Assembly\WooW.WebClient.dll";

                    WoDirectory.CopyFile(pathOld, pathNew, true);
                }
                else
                {
                    XtraMessageBox.Show(
                        $"Primero compile el cliente.\n\r No se encontro el ensa,blado de la bibliotraca del cliente en la ruta: {pathOld}",
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

        /// <summary>
        /// Botón para clonar una vista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloneView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //verificamos que no intente clonar una vista no seleccionada
            if ((cmbVistas.EditValue.ToSafeString() != _viewNew))
            {
                _report.SetReportFm_txtVista(cmbVistas.EditValue.ToSafeString());
                _report.BuEditView(sender, e, false, true);
                SetFocusRefreshComboViews();
            }
        }

        /// <summary>
        /// Botón para borrar una vista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteView_ItemClick(object sender, ItemClickEventArgs e)
        {
            WoReport reportFocused = new WoReport();
            if (_report.xtraTabSelected == "ODATA")
            {
                reportFocused =
                    _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
            }
            else
            {
                reportFocused = _report.grdReportesView.GetFocusedRow() as WoReport;
            }
            if (
                cmbVistas.EditValue.ToSafeString() == _report._viewNew
                || cmbVistas.EditValue.ToSafeString() == reportFocused.idReport
            )
                return;
            if (
                XtraMessageBox.Show(
                    $"Borrar la vista {cmbVistas.EditValue.ToSafeString()}?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) == DialogResult.Yes
            )
            {
                try
                {
                    DeleteView();
                    _report.currentView = string.Empty;
                    SetFocusRefreshComboViews();
                    cmbVistas.EditValue = _viewNew;

                    if (_report.xtraTabSelected == "ODATA")
                    {
                        _report.woGridodataReports1.SetValues();
                    }
                    if (_report.xtraTabSelected == "Servicio")
                    {
                        _report.buRun(sender, e);
                        _report.tabPrincipal.SelectedTabPage = _report.tabDatos;
                    }
                }
                catch (Exception ex)
                {
                    var a = ex.Message;
                }
            }
        }

        /// <summary>
        /// Función para la eliminación de una plantilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteTemplate_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (cmbTemplates.EditValue.ToSafeString() == _report.templateNew)
                return;

            if (
                XtraMessageBox.Show(
                    $"Borrar la plantilla {cmbTemplates.EditValue.ToSafeString()}?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) == DialogResult.Yes
            )
            {
                string Archivo =
                    $"{proyecto.DirPlantillasReportes}\\{cmbTemplates.EditValue.ToSafeString()}.xml";
                File.Delete(Archivo);
                //_report.buRun(sender, e);
                _report.tabPrincipal.SelectedTabPage = _report.tabDatos;
                cmbTemplates.EditValue = _templateNew;
                WoReport reportFocused = new WoReport();
                if (_report.xtraTabSelected == "ODATA")
                {
                    reportFocused =
                        _report.woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
                }
                else
                {
                    reportFocused = _report.grdReportesView.GetFocusedRow() as WoReport;
                }
                cmbVistas.Refresh();
                LoadTemplate(sender, reportFocused);
            }
        }

        /// <summary>
        /// Función para la clonación de una plantilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloneTemplate_ItemClick(object sender, ItemClickEventArgs e)
        {
            //verificamos que no intente clonar una plantilla no seleccionada
            if ((cmbTemplates.EditValue.ToSafeString() != _templateNew))
            {
                buPlantillaEditar.Enabled = true;

                var cancel = _report.BuEditTemplate(sender, e, false, true);
                if (cancel)
                {
                    cmbTemplates.EditValue = cmbTemplates.EditValue;
                    return;
                }
                SetFocusRefreshComboTemplates();
            }
            cmbTemplates.Refresh();
        }

        private void cmbTemplates_EditValueChanged(object sender, EventArgs e)
        {
            //RepositoryItemComboBox ComboBoxEdit = cmbTemplates.Edit as RepositoryItemComboBox; Probar correctamente que no se habilite cuando este seleccionado el valor default de los templates
            if (cmbTemplates.EditValue.ToString() == _templateNew)
            {
                buPlantillaEditar.Enabled = false;
                btnNewView.Enabled = false;
                buEditarVista.Enabled = false;
            }
            else
            {
                buPlantillaEditar.Enabled = true;
                btnNewView.Enabled = true;
            }
            _report.SetReportFm_txtPlantilla(cmbTemplates.EditValue.ToString());
        }
    }
}
