using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraSplashScreen;
using MyClosingReport;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Auth;
using WooW.Core;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.Client;
using WooW.SB.Config;
using WooW.SB.Forms;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;
using WooW.SB.Helpers.GeneratorsHelpers;
using WooW.SB.Interfaces;
using WooW.SB.ManagerDirectory;
using WooW.SB.Reportes.Helpers;
using WooW.SB.Reportes.ReportBase;
using WooW.SB.Reportes.ReportModels;
using WooW.SB.Reportes.ReportsComponents;

namespace WooW.SB.Reportes
{
    public partial class WoReportsDesigner : UserControl, IForm
    {
        #region Variables

        public WooWConfigParams wooWConfigParams { get; set; }
        private Type _companyInstanceClass;
        private Type _reportListInstance;
        private Type _reportInstance;
        private Type _sysCompanyInstance;
        private Type _sysUdnInstance;
        private Type _sysRegionInstance;
        private Type _companyInstancerResponseClass;
        private Type _instanceClass;
        private Assembly _assembly;
        private Type _reportType;
        public DataSet dataSet = new DataSet();

        //private MethodInfo _methodInfoModelSelected;
        private MethodInfo _methodInfoCompany;
        private MethodInfo _methodInfoReport;

        /// <summary>
        /// Diseñador de formularios
        /// </summary>
        private WoSimpleDesigner _designerComponent;
        private DataTable _dtCompany;
        private DataTable _dtRegion;
        private DataTable _dtUdn;

        /// <summary>
        /// ruta donde se encuentra almacenado el reporte
        /// </summary>
        private string _reportPath;

        /// <summary>
        /// Instancia del cliente
        /// </summary>
        private FormClient _formClient = new FormClient();

        /// <summary>
        /// Variable auxiliar para la creación de reportes
        /// </summary>
        private XtraReport report = new XtraReport();

        /// <summary>
        /// Variable que almacenará el reporte seleccionado
        /// </summary>
        public WoReport _repSelected = new WoReport();

        /// <summary>
        /// Valor default de el combo de plantillas
        /// </summary>
        public string templateNew = "Seleccione..";

        /// <summary>
        /// variable para almacenar el valor del TxtdePlantillas
        /// </summary>
        private string _reportFm_txtPlantilla = string.Empty;

        /// <summary>
        /// variable para almacenar el valor de la vista seleccionada
        /// </summary>
        private string _reportFm_txtVista = string.Empty;

        /// <summary>
        /// Funcion para seterar el valor de txtTemplate en tiempo real
        /// </summary>
        /// <param name="value"></param>
        public void SetReportFm_txtPlantilla(string value)
        {
            _reportFm_txtPlantilla = value;
        }

        /// <summary>
        /// Función para seterar el valor de txtTemplate en tiempo real.
        /// </summary>
        /// <param name="value"></param>
        public void SetReportFm_txtVista(string value)
        {
            _reportFm_txtVista = value;
        }

        /// <summary>
        /// Valor default de el combo de vistas
        /// </summary>
        public string _viewNew = "Seleccione..";

        public Proyecto proyecto { get; set; }

        public RibbonControl CurrentRibbon => throw new NotImplementedException();

        public bool CambiosPendientes => throw new NotImplementedException();

        public string Nombre => throw new NotImplementedException();

        private List<WoReport> reportListData;
        private List<WoReport> reportListDto;

        /// <summary>
        ///  grupo base para que la función recursiva guarde el layout y se serialize en json
        /// </summary>
        private WoContainer _group = new WoContainer();

        /// <summary>
        /// Variable ustilizada pra inicializar un objeto de tipo fmMenus
        /// </summary>
        //private fmReport _reportFm { get; set; } = new fmReport();

        /// <summary>
        /// bandera para no volver a ejecutar la función de cambio de vista
        /// </summary>
        private bool _firstOpen = true;

        /// <summary>
        /// Path de la carpeta de reportes
        /// </summary>
        private string _pathReports = $"";

        /// <summary>
        /// Ruta donde se encuentran los modelos de reportes
        /// </summary>
        private string _pathReportsModels = $"";

        /// <summary>
        /// nombre de la clase del modelo seleccionado
        /// </summary>
        public string _modelSelected = "";

        /// <summary>
        /// clase ejecución para ejecutar el blazor generado
        /// </summary>
        //private WoExecute _executionBlazor;

        public EventHandler<string> EventsLoadTemplate;
        public EventHandler<string> DisableReports;
        public Action DisableBuRun;
        public EventHandler<string> EnableReports;
        public EventHandler<bool> EventsChangeAuth;
        public EventHandler<object> EventsBuRun;
        public EventHandler<bool> EventsSetStatusBtnsRowsChanged;
        public EventHandler<bool> EventsbuEditar;
        public EventHandler<bool> buGenerateBlazor;
        public EventHandler<bool> buEditView;
        public EventHandler<bool> buttonStopProcess;
        public EventHandler<WoReport> EventsViews;
        public EventHandler<string> SetValuCBTemplates;
        public Action BuExcecute;

        /// <summary>
        /// Controlador de eventos para actualziar el cambio de valor de la plantilla
        /// </summary>
        public EventHandler<dynamic> EventSetTemplateValue;

        /// <summary>
        /// Controlador de eventos para actualziar el cambio de valor de la vista
        /// </summary>
        public EventHandler<dynamic> EventSetViewValue;

        /// <summary>
        /// Variable para almacenar el valor default de la vista
        /// </summary>
        private string _templateNew = "";

        #endregion Variables

        public WoReportsDesigner()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();
            proyecto.AssemblyModelCargado = true;
            //Copia todas vistas y reportes a la carpeta temporal

            EnablePrevio(false);
            _formClient.AutoAuth();
            _formClient.ChangeStateClient += ChangeAuth;
            dataSet.DataSetName = "Datos";
            string AssemblyFile = $"{proyecto.FileWooWWebClientdll}";
            _assembly = Assembly.LoadFrom(AssemblyFile);
            _pathReports = $"{proyecto.DirReportes}";
            _pathReportsModels = $"{proyecto.DirReportesVistas}";
            proyecto.AssemblyModelCargado = true;

            if (_modelSelected != "")
            {
                if (xtraTabSelected == "ODATA")
                {
                    _repSelected = woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
                }
                else
                {
                    _repSelected = grdReportesView.GetFocusedRow() as WoReport;
                }
                LoadTemplate();
            }
            else
            {
                LoadTemplate();
            }

            #region Add gridODATA component


            woGridodataReports1 = new ReportsComponents.woGridODATAReports(_project: proyecto);
            //
            // woGridodataReports1
            //
            woGridodataReports1.Dock = System.Windows.Forms.DockStyle.Fill;
            woGridodataReports1.Location = new System.Drawing.Point(0, 0);
            woGridodataReports1.Name = "woGridodataReports1";
            woGridodataReports1.SetSnippet = null;
            woGridodataReports1.Size = new System.Drawing.Size(1390, 741);
            woGridodataReports1.TabIndex = 2;
            woGridodataReports1.RunOdata += RunOdata;
            woGridodataReports1.CreateInstances += FillPropertyGrid;
            //
            // xtraTabPageReportsDTO
            //
            xtraTabPageReportsDTO.Controls.Add(woGridodataReports1);
            xtraTabPageReportsDTO.Name = "xtraTabPageReportsDTO";
            xtraTabPageReportsDTO.Size = new System.Drawing.Size(1390, 741);
            xtraTabPageReportsDTO.Text = "ODATA";

            #endregion Add gridODATA component
        }

        /// <summary>
        /// Creacion del objeto de diseñador de Odata de reportes
        /// </summary>
        public woGridODATAReports woGridodataReports1 { get; set; }

        #region Eventos

        #region Servicio de autenticación
        private void ChangeAuth(object sender, bool status)
        {
            EventsChangeAuth?.Invoke(sender, status);
        }
        #endregion Servicio de autenticación


        #region Carga de datos reportes por servicio
        private void WoReports_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Función para cargar el combo box con las plantillas existentes
        /// </summary>
        public void LoadTemplate()
        {
            EventsLoadTemplate?.Invoke(_templateNew, _templateNew);
        }

        /// <summary>
        /// Carga de información en el datagrid de datos
        /// </summary>
        public void LoadData()
        {
            reportListData = new List<WoReport>();
            foreach (
                var models in proyecto
                    .ModeloCol.Modelos.Where(f =>
                        f.TipoModelo == WoTypeModel.Request
                        && f.SubTipoModelo == WoSubTypeModel.Report
                    )
                    .ToList()
            )
            {
                if (File.Exists($"{proyecto.DirVistasReports}\\{models.Id}.json"))
                {
                    string json = WoDirectory.ReadFile($@"{_pathReportsModels}\{models.Id}.json");
                    var reports = JsonConvert.DeserializeObject<WoReport>(json);
                    reportListData.Add(reports);
                }
                else
                {
                    var reports = new WoReport()
                    {
                        idReport = models.Id,
                        Etiqueta = models.EtiquetaId,
                        MaskText = EtiquetaCol.Get(models.EtiquetaId),
                        proccessId = models.ProcesoId,
                        modelType = models.TipoModelo,
                        jsonModel = models.ToJson(),
                        LabelId = models.EtiquetaId,
                        reportDesign = WoGetReportFilesHelper.GetReportFiles(models.Id)
                    };
                    reportListData.Add(reports);
                }
            }

            //Asignacion de propiedades
            HidePropertiesHelper.ModifyBrowsableAttribute(
                _repSelected,
                new List<string>() { "LabelId", "proccessId", "idReport", "reportDesign" },
                true
            );
            //HidePropertiesHelper.ModifyBrowsableAttribute(
            //    _repSelected,
            //    new List<string>()
            //    {
            //        "LabelId",
            //        //"AutoPrint",
            //        //"NumPrint",
            //        //"Export",
            //        //"TittleType",
            //        //"PrintClose"
            //    },
            //    false
            //);

            grdReportes.DataSource = reportListData;

            // Obtén la vista (GridView) asociada al GridControl
            GridView gridView = grdReportes.MainView as GridView;

            gridView.Columns[0].Caption = "Modelo";
            gridView.Columns[1].Caption = "Proceso";
            gridView.Columns[2].Caption = "Con Vistas";

            //Dejamos los valores default bloqueados en caso de no tener modelos
            if (reportListData.Count == 0)
            {
                DisableReports?.Invoke(null, null);
            }
            else
            {
                EnableReports?.Invoke(null, null);
            }
        }

        /// <summary>
        /// Función para incializar las instancias con el modelos seleccionado
        /// </summary>
        /// <param name="Request"></param>
        public void FillPropertyGrid(dynamic e, string Request)
        {
            try
            {
                _instanceClass = _assembly
                    .GetTypes()
                    .Where(x => x.Name == Request)
                    .FirstOrDefault();

                _companyInstanceClass = _assembly
                    .GetTypes()
                    .Where(x => x.Name == "SysDatosEmpresaReq")
                    .FirstOrDefault();

                _reportListInstance = _assembly
                    .GetTypes()
                    .Where(x => x.Name == "SysReportListReq")
                    .FirstOrDefault();

                _reportInstance = _assembly
                    .GetTypes()
                    .Where(x => x.Name == "SysReportReq")
                    .FirstOrDefault();

                _sysUdnInstance = _assembly
                    .GetTypes()
                    .Where(x => x.Name == "SysUdn")
                    .FirstOrDefault();

                _sysRegionInstance = _assembly
                    .GetTypes()
                    .Where(x => x.Name == "SysRegion")
                    .FirstOrDefault();

                _sysCompanyInstance = _assembly
                    .GetTypes()
                    .Where(x => x.Name == "SysEmpresa")
                    .FirstOrDefault();

                _companyInstancerResponseClass = _assembly
                    .GetTypes()
                    .Where(x => x.Name == "SysDatosEmpresaRes")
                    .FirstOrDefault();

                if (_instanceClass == null)
                    return;
                var obj = Activator.CreateInstance(_instanceClass);
                _reportType = obj.GetType();
                _modelSelected = _reportType.Name;
            }
            catch (Exception ex)
            {
                var exception = ex;
            }
        }

        #endregion Carga de datos reportes por servicio



        #region Botón de obtener datos

        /// <summary>
        /// Función para consumir el servicio request y recibir el response
        /// a traves de los parámetros dados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void buRun(object sender, EventArgs e)
        {
            try
            {
                if (xtraTabSelected == "ODATA")
                {
                    _repSelected = woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
                }
                if (xtraTabSelected == "Servicio")
                {
                    EnablePrevio(true);
                    _repSelected = grdReportesView.GetFocusedRow() as WoReport;
                }
                FillPropertyGrid(null, _repSelected.idReport);

                dynamic company = null;
                dynamic region = null;
                dynamic udn = null;
                object DatosEmpresaResponse = null;

                if (_sysCompanyInstance != null)
                {
                    company = Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(_sysCompanyInstance)
                    );
                    ;
                }
                if (_sysRegionInstance != null)
                {
                    region = Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(_sysRegionInstance)
                    );
                }
                if (_sysUdnInstance != null)
                {
                    udn = Activator.CreateInstance(typeof(List<>).MakeGenericType(_sysUdnInstance));
                }
                if (_companyInstancerResponseClass != null)
                {
                    DatosEmpresaResponse = Activator.CreateInstance(_companyInstancerResponseClass);
                }
                report = new XtraReport();

                if (_repSelected == null)
                    return;
                if (_instanceClass == null)
                {
                    return;
                }
                if (_companyInstanceClass != null)
                {
                    _methodInfoCompany = _companyInstanceClass.GetMethod("Get");
                }

                //if (_methodInfoModelSelected != null)
                //{
                dataSet.Tables.Clear();
                dynamic claseDatEmp = Activator.CreateInstance(_companyInstanceClass);
                List<object> listReq = new List<object>();
                //A través del cliente generado consultamos el servicio
                _formClient.SetModelType(_companyInstanceClass);
                _formClient.SearchMethod(claseDatEmp.GetType());
                object adata2 = _formClient.ExecuteMethod(claseDatEmp);
                //Ejecuta el evento calcular
                dynamic adata = _designerComponent.Calcular(sender, e);
                //Recibir una lista como un objeto y convertirlo a un tipo
                dynamic listDatEmp = adata2.ConvertTo(DatosEmpresaResponse.GetType());
                listReq.Add(_designerComponent._instanceModelManager.GetInstanceModelDyn());
                var listclassReqT = Activator.CreateInstance(
                    typeof(List<>).MakeGenericType(_instanceClass)
                );
                if (listDatEmp == null)
                {
                    XtraMessageBox.Show(
                        "Hacen falta datos del sistema, verifique porfavor.",
                        "Error falta de data.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                dynamic listParam = listReq.ConvertTo(listclassReqT.GetType());
                if (_sysCompanyInstance != null)
                {
                    company.Add(listDatEmp.SysEmpresa);
                }
                if (_sysRegionInstance != null)
                {
                    region.Add(listDatEmp.SysRegion);
                }
                if (_sysUdnInstance != null)
                {
                    udn.Add(listDatEmp.SysUdn);
                }
                if (adata != null)
                {
                    //Identifica si es una lista o una lista de listas
                    if ((adata.GetType().GetInterface(nameof(IEnumerable)) != null))
                    {
                        DataTable principal = FilesHelper.ConvertToDataTable(adata);
                        principal.TableName = "Principal";

                        if (_sysCompanyInstance != null)
                        {
                            _dtCompany = FilesHelper.ConvertToDataTable(company);
                            _dtCompany.TableName = "Datos de la empresa";
                        }
                        if (_sysRegionInstance != null)
                        {
                            _dtRegion = FilesHelper.ConvertToDataTable(region);
                            _dtRegion.TableName = "Región";
                        }
                        if (_sysUdnInstance != null)
                        {
                            _dtUdn = FilesHelper.ConvertToDataTable(udn);
                            _dtUdn.TableName = "UDN";
                        }
                        DataTable parameters = FilesHelper.ConvertToDataTable(listParam);
                        if (parameters.Columns.Count > 0)
                        {
                            parameters.TableName = "Parametros";

                            parameters.Columns.Add("Usuario");
                            parameters.Columns.Add("Rol");
                            parameters.Columns.Add("ListaDeParametrosPorNombre");
                            DataRow _ravi = parameters.Rows[0];
                            _ravi["Usuario"] = CredentialsAuthProvider.Name;
                            _ravi["Rol"] = "Admnistrador";
                            String _listaParam = "";
                            //var accessor = FastMember.TypeAccessor.Create(_instanceClass);

                            Modelo modelo = proyecto
                                .ModeloCol.Modelos.Where(m => m.Id == _instanceClass.Name)
                                .FirstOrDefault();

                            foreach (dynamic param in listParam)
                            {
                                ///obtenemos la colección PropertyInfo con todas las propiedades del objeto
                                //foreach (var property in accessor.GetMembers())

                                foreach (var property in param.GetType().GetProperties())
                                {
                                    var col = modelo
                                        .Columnas.Where(c => c.Id == property.Name)
                                        .FirstOrDefault();
                                    if (col != null)
                                    {
                                        var InstanceClass = listParam[0];
                                        string etiqueta = col.Formulario;
                                        var value = string.Empty;
                                        try
                                        {
                                            //value = accessor[InstanceClass, property.Name];
                                            var x = property.GetValue(param);
                                            if (x != null)
                                            {
                                                value = x.ToString();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            var exception = ex;
                                        }
                                        //var value = "";
                                        _listaParam += etiqueta + ": " + value + ", ";
                                    }
                                }
                            }
                            var listP = _listaParam.Substring(0, _listaParam.Length - 2);
                            _ravi["ListaDeParametrosPorNombre"] = listP;
                        }
                        if (_sysCompanyInstance != null)
                        {
                            dataSet.Tables.Add(_dtCompany);
                        }
                        if (_sysRegionInstance != null)
                        {
                            dataSet.Tables.Add(_dtRegion);
                        }
                        if (_sysUdnInstance != null)
                        {
                            dataSet.Tables.Add(_dtUdn);
                        }
                        dataSet.Tables.Add(principal);
                        dataSet.Tables.Add(parameters);
                        grdResultado.DataSource = principal;
                    }
                    else
                    {
                        XtraMessageBox.Show(
                            "Complejidad alta en el servicio response",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );
                        return;
                    }
                }
                //}
                string Archivo = string.Empty;
                if (_reportFm_txtVista != _viewNew)
                {
                    Archivo = $"{proyecto.DirReportes}\\Vistas\\{_reportFm_txtVista}.xml";
                }
                else if (_reportFm_txtPlantilla != _templateNew)
                {
                    Archivo = $"{proyecto.DirReportes}\\Plantillas\\{_reportFm_txtPlantilla}.xml";
                }
                else
                {
                    WoReportBase rep = new WoReportBase();
                    woGridodataReports1.documentViewerOdata.DocumentSource = rep;
                    rep.CreateDocument();
                }

                if (File.Exists(Archivo))
                {
                    XtraReport rep = new XtraReport();
                    DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                    {
                        rep.LoadLayoutFromXml(Archivo);
                    });
                    rep.DataSource = dataSet;
                    rep.CreateDocument();

                    repPreview.DocumentSource = rep;

                    tabPrincipal.SelectedTabPage = tabPrevio;
                    buGenerateBlazor.Invoke(true, true);
                    report = rep;
                    _reportPath = Archivo;
                }
                else
                {
                    Archivo = $"{proyecto.DirReportes}\\{_repSelected.idReport}.xml";
                    if (File.Exists(Archivo))
                    {
                        XtraReport rep = new XtraReport();
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            rep.LoadLayoutFromXml(Archivo);
                        });
                        rep.DataSource = dataSet;
                        rep.CreateDocument();

                        repPreview.DocumentSource = rep;

                        tabPrincipal.SelectedTabPage = tabPrevio;
                        buGenerateBlazor.Invoke(true, true);
                        report = rep;
                        _reportPath = Archivo;
                    }
                    else
                    {
                        woGridodataReports1.documentViewerOdata.DocumentSource = null;
                    }
                }

                //EventsbuEditar.Invoke(true, true);
            }
            catch (Exception ex)
            {
                string m =
                    ex.Message
                    + " "
                    + (ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                XtraMessageBox.Show(
                    m,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
        }

        #region Botón de obtener datos

        /// <summary>
        /// Función para consumir el servicio request y recibir el response
        /// a traves de los parámetros dados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RunOdata(object data, dynamic e)
        {
            try
            {
                //Definición de variables
                dynamic company = null;
                dynamic region = null;
                dynamic udn = null;
                object DatosEmpresaResponse = null;

                _repSelected = GetModelSelectedOdata();

                if (_sysCompanyInstance != null)
                {
                    company = Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(_sysCompanyInstance)
                    );
                }
                if (_sysRegionInstance != null)
                {
                    region = Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(_sysRegionInstance)
                    );
                }
                if (_sysUdnInstance != null)
                {
                    udn = Activator.CreateInstance(typeof(List<>).MakeGenericType(_sysUdnInstance));
                }
                if (_companyInstancerResponseClass != null)
                {
                    DatosEmpresaResponse = Activator.CreateInstance(_companyInstancerResponseClass);
                }

                report = new XtraReport();
                //_methodInfoModelSelected = null;
                if (_instanceClass == null)
                {
                    return;
                }
                if (_companyInstanceClass != null)
                {
                    _methodInfoCompany = _companyInstanceClass.GetMethod("Get");
                }

                //if (_methodInfoModelSelected != null)
                //{
                dataSet.Tables.Clear();
                dynamic claseDatEmp = null;
                object adata2 = null;
                if (_companyInstanceClass != null)
                {
                    claseDatEmp = Activator.CreateInstance(_companyInstanceClass);
                    //A través del cliente generado consultamos el servicio
                    _formClient.SetModelType(_companyInstanceClass);
                    _formClient.SearchMethod(claseDatEmp.GetType());
                    adata2 = _formClient.ExecuteMethod(claseDatEmp);
                }
                //Ejecuta el evento calcular
                dynamic adata = data;
                //Recibir una lista como un objeto y convertirlo a un tipo
                dynamic listDatEmp = null;
                if (DatosEmpresaResponse != null && adata2 != null)
                {
                    listDatEmp = adata2.ConvertTo(DatosEmpresaResponse.GetType());
                }
                var listclassReqT = Activator.CreateInstance(
                    typeof(List<>).MakeGenericType(_instanceClass)
                );
                if (listDatEmp == null)
                {
                    XtraMessageBox.Show(
                        "Hacen falta datos del sistema, verifique porfavor.",
                        "Error falta de data.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                if (_sysCompanyInstance != null)
                {
                    company.Add(listDatEmp.SysEmpresa);
                }
                if (_sysRegionInstance != null)
                {
                    region.Add(listDatEmp.SysRegion);
                }
                if (_sysUdnInstance != null)
                {
                    udn.Add(listDatEmp.SysUdn);
                }

                if (adata != null)
                {
                    //Identifica si es una lista o una lista de listas
                    if ((adata.GetType().GetInterface(nameof(IEnumerable)) != null))
                    {
                        DataTable principal = FilesHelper.ConvertToDataTable(adata);
                        principal.TableName = "Principal";
                        dataSet.Tables.Add(principal);

                        if (_sysCompanyInstance != null)
                        {
                            _dtCompany = FilesHelper.ConvertToDataTable(company);
                            _dtCompany.TableName = "Datos de la empresa";
                            dataSet.Tables.Add(_dtCompany);
                        }
                        if (_sysRegionInstance != null)
                        {
                            _dtRegion = FilesHelper.ConvertToDataTable(region);
                            _dtRegion.TableName = "Región";
                            dataSet.Tables.Add(_dtRegion);
                        }
                        if (_sysUdnInstance != null)
                        {
                            _dtUdn = FilesHelper.ConvertToDataTable(udn);
                            _dtUdn.TableName = "UDN";
                            dataSet.Tables.Add(_dtUdn);
                        }
                        var accessor = FastMember.TypeAccessor.Create(_instanceClass);

                        Modelo modelo = proyecto
                            .ModeloCol.Modelos.Where(m => m.Id == _instanceClass.Name)
                            .FirstOrDefault();
                    }
                    else
                    {
                        XtraMessageBox.Show(
                            "Complejidad alta en el servicio response",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );
                        return;
                    }
                }
                //}
                string Archivo = string.Empty;
                if (_reportFm_txtVista != _viewNew)
                {
                    Archivo = $"{proyecto.DirReportes}\\Vistas\\{_reportFm_txtVista}.xml";
                    if (File.Exists(Archivo))
                    {
                        XtraReport rep = new XtraReport();
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            rep.LoadLayoutFromXml(Archivo);
                        });
                        rep.DataSource = dataSet;
                        rep.CreateDocument();

                        woGridodataReports1.documentViewerOdata.DocumentSource = rep;
                        report = rep;
                        _reportPath = Archivo;
                    }
                }
                else
                {
                    Archivo = $"{proyecto.DirReportes}\\{_repSelected.idReport}.xml";
                    if (File.Exists(Archivo))
                    {
                        XtraReport rep = new XtraReport();
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            rep.LoadLayoutFromXml(Archivo);
                        });
                        rep.DataSource = dataSet;
                        rep.CreateDocument();

                        woGridodataReports1.documentViewerOdata.DocumentSource = rep;

                        woGridodataReports1.tabExplorador.SelectedTabPage = tabPrevio;
                        //buGenerateBlazor.Invoke(true, true);
                        report = rep;
                        _reportPath = Archivo;
                    }
                    else
                    {
                        woGridodataReports1.documentViewerOdata.DocumentSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string m =
                    ex.Message
                    + " "
                    + (ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                XtraMessageBox.Show(
                    m,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
        }
        #endregion Botón de obtener datos

        #endregion Botón de obtener datos

        #region Botón de editar o crear vistas o plantillas
        /// <summary>
        /// Variable auxiliar para almacenar el nombre de la nueva plantilla
        /// </summary>
        public string currentTemplate;

        /// <summary>
        /// Función para editar o crear una plantilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool BuEditTemplate(
            object sender,
            EventArgs e,
            bool NewTemplate = false,
            bool Clone = false
        )
        {
            try
            {
                dynamic company = null;
                dynamic region = null;
                dynamic udn = null;
                object DatosEmpresaResponse = null;

                _repSelected = GetModelSelectedOdata();
                object adata2 = null;
                if (_sysCompanyInstance != null)
                {
                    company = Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(_sysCompanyInstance)
                    );
                    dynamic claseDatEmp = Activator.CreateInstance(_companyInstanceClass);
                    _formClient.AutoAuth();
                    _formClient.SetModelType(_companyInstanceClass);
                    _formClient.SearchMethod(_companyInstanceClass);
                    adata2 = _formClient.ExecuteMethod(claseDatEmp);
                }
                if (_sysRegionInstance != null)
                {
                    region = Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(_sysRegionInstance)
                    );
                }
                if (_sysUdnInstance != null)
                {
                    udn = Activator.CreateInstance(typeof(List<>).MakeGenericType(_sysUdnInstance));
                }
                if (_companyInstancerResponseClass != null)
                {
                    DatosEmpresaResponse = Activator.CreateInstance(_companyInstancerResponseClass);
                }

                dynamic listDatEmp = null;
                if (DatosEmpresaResponse != null)
                {
                    listDatEmp = adata2.ConvertTo(DatosEmpresaResponse.GetType());
                }
                if (listDatEmp == null)
                {
                    XtraMessageBox.Show(
                        "Hacen falta datos del sistema, verifique porfavor.",
                        "Error falta de data.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return true;
                }
                DataSet _dataSet = new DataSet();
                if (_sysCompanyInstance != null)
                {
                    company.Add(listDatEmp.SysEmpresa);
                    _dtCompany = FilesHelper.ConvertToDataTable(company);
                    _dtCompany.TableName = "Datos de la empresa";
                    _dataSet.Tables.Add(_dtCompany);
                }
                if (_sysRegionInstance != null)
                {
                    region.Add(listDatEmp.SysRegion);
                    _dtRegion = FilesHelper.ConvertToDataTable(region);
                    _dtRegion.TableName = "Región";
                    _dataSet.Tables.Add(_dtRegion);
                }
                if (_sysUdnInstance != null)
                {
                    udn.Add(listDatEmp.SysUdn);
                    _dtUdn = FilesHelper.ConvertToDataTable(udn);
                    _dtUdn.TableName = "UDN";
                    _dataSet.Tables.Add(_dtUdn);
                }

                XtraReport rep = new XtraReport();

                if (!Clone)
                {
                    if (NewTemplate)
                    {
                        fmNewTemplate fm = new fmNewTemplate();

                        if (fm.ShowDialog() != DialogResult.OK)
                            return true;

                        currentTemplate = fm.Plantilla;
                        if (File.Exists($"{proyecto.DirPlantillasReportes}\\{currentTemplate}.xml"))
                        {
                            XtraMessageBox.Show(
                                "Ya existe una plantilla con el mismo nombre.",
                                "Verifique...",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            BuEditTemplate(sender, e, NewTemplate: true, Clone: false);
                            return true; // no cerrar permitir cambiar nombre
                        }
                        rep = new WoReportBase();
                    }
                    else
                    {
                        if (_reportFm_txtPlantilla != _templateNew)
                        {
                            currentTemplate = _reportFm_txtPlantilla;
                            string ArchivoLoad =
                                $"{proyecto.DirPlantillasReportes}\\{_reportFm_txtPlantilla}.xml";
                            DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                            {
                                if (_reportFm_txtPlantilla != "Seleccione..")
                                {
                                    rep.LoadLayoutFromXml(ArchivoLoad);
                                }
                            });
                        }
                    }

                    rep.DataSource = _dataSet;

                    ReportDesignTool designTool = CreateDesigner(rep);
                    designTool.DesignRibbonForm.DesignMdiController.AddCommandHandler(
                        new MyClosingReportCommandHandler(
                            designTool.DesignRibbonForm.DesignMdiController
                        )
                    );
                    (designTool.DesignRibbonForm as Form).FormClosing += Form_FormClosingEvt;
                    SplashScreenManager.ShowForm(typeof(fmWait));
                    designTool.ShowRibbonDesignerDialog();
                    SplashScreenManager.CloseForm();
                    if (
                        XtraMessageBox.Show(
                            $"Guardar la plantilla?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes
                    )
                    {
                        string Archivo = $"{proyecto.DirPlantillasReportes}\\{currentTemplate}.xml";
                        rep.SaveLayoutToXml(Archivo);
                        //string ArchivoTemp = $"{proyecto.DirReportesTemp}\\{currentTemplate}.xml";
                        //rep.SaveLayoutToXml(ArchivoTemp);
                        LoadTemplate();
                        _reportFm_txtPlantilla = currentTemplate;
                        if (xtraTabSelected == "Servicio")
                        {
                            buRun(sender, e);
                        }
                        SetValuCBTemplates?.Invoke(currentTemplate, currentTemplate);
                    }
                }
                else
                {
                    fmNewTemplate fm = new fmNewTemplate();

                    if (fm.ShowDialog() != DialogResult.OK)
                        return true;

                    currentTemplate = fm.Plantilla;
                    if (File.Exists($"{proyecto.DirPlantillasReportes}\\{currentTemplate}.xml"))
                    {
                        XtraMessageBox.Show(
                            "Ya existe una plantilla con el mismo nombre.",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        BuEditTemplate(sender, e, NewTemplate: false, Clone: true);
                        return true; // no cerrar permitir cambiar nombre
                    }
                    //validamos que la plantilla seleccionada no sea la default
                    if (_reportFm_txtPlantilla != _templateNew)
                    {
                        string ArchivoLoad =
                            $"{proyecto.DirPlantillasReportes}\\{_reportFm_txtPlantilla}.xml";
                        rep = new WoReportBase();
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            if (_reportFm_txtPlantilla != "Seleccione..")
                            {
                                rep.LoadLayoutFromXml(ArchivoLoad);
                            }
                        });
                    }
                    rep.DataSource = dataSet;

                    if (
                        XtraMessageBox.Show(
                            $"Desea abrir el editor de reportes?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes
                    )
                    {
                        ReportDesignTool designTool = CreateDesigner(rep);
                        designTool.DesignRibbonForm.DesignMdiController.AddCommandHandler(
                            new MyClosingReportCommandHandler(
                                designTool.DesignRibbonForm.DesignMdiController
                            )
                        );
                        (designTool.DesignRibbonForm as Form).FormClosing += Form_FormClosingEvt;
                        SplashScreenManager.ShowForm(typeof(fmWait));
                        designTool.ShowRibbonDesignerDialog();
                        SplashScreenManager.CloseForm();

                        if (
                            XtraMessageBox.Show(
                                $"Guardar la Plantilla?",
                                this.Text,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1
                            ) == DialogResult.Yes
                        )
                        {
                            string fileReport =
                                $"{proyecto.DirPlantillasReportes}\\{currentTemplate}.xml";
                            rep.SaveLayoutToXml(fileReport);
                            string fileReportTemp =
                                $"{proyecto.DirApplication_WebService_WebService_Reports}\\{currentTemplate}.xml";
                            rep.SaveLayoutToXml(fileReportTemp);

                            if (xtraTabSelected == "Servicio")
                            {
                                buRun(sender, e);
                            }
                        }
                        else
                        {
                            currentTemplate = "";
                        }
                    }
                    else
                    {
                        string fileReport =
                            $"{proyecto.DirPlantillasReportes}\\{currentTemplate}.xml";
                        rep.SaveLayoutToXml(fileReport);
                        string fileReportTemp =
                            $"{proyecto.DirApplication_WebService_WebService_Reports}\\{currentTemplate}.xml";
                        rep.SaveLayoutToXml(fileReportTemp);
                        if (xtraTabSelected == "Servicio")
                        {
                            buRun(sender, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            return false;
        }

        /// <summary>
        /// Variable auxiliar para almacenar el nombre de la nueva vista
        /// se utiliza concatenar el nombre del modelo deguido de un punto
        /// y del nombre que se le dio
        /// </summary>
        public string currentView = string.Empty;

        /// <summary>
        /// Función para crear o editar una vista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="newView"></param>
        public DialogResult BuEditView(
            object sender,
            EventArgs e,
            bool newView = false,
            bool Clone = false
        )
        {
            DialogResult dialog = new DialogResult();

            XtraReport rep = new XtraReport();

            #region Manipulación de Vistas ODATA

            if (xtraTabSelected == "ODATA")
            {
                //Vista de reporte por OData
                _repSelected = woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;

                if (_repSelected == null)
                {
                    return dialog;
                }

                if (woGridodataReports1.grdOdataView.RowCount == 0)
                {
                    MessageBox.Show(
                        "No hay data",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dialog;
                }
                //checar boton Editar
                if (!Clone)
                {
                    if (newView)
                    {
                        fmSaveView saveView = new fmSaveView(_repSelected.idReport);
                        dialog = saveView.ShowDialog(this);
                        if (_reportFm_txtPlantilla == _templateNew)
                        {
                            XtraMessageBox.Show(
                                "No hay data para cargar en el reporte, verifique.",
                                "Verifique...",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            return dialog;
                        }

                        if (dialog == DialogResult.Cancel)
                            return dialog;
                        if (saveView.Vista == "")
                            return dialog;
                        if (xtraTabSelected == "ODATA")
                        {
                            currentView =
                                _repSelected.idReport.ToString() + "." + saveView.Vista + "ODATA";
                        }
                        if (xtraTabSelected == "Servicio")
                        {
                            currentView = _repSelected.idReport.ToString() + "." + saveView.Vista;
                        }
                        string ArchivoLoad = $"{proyecto.DirVistasReports}\\{currentView}.xml";
                        if (File.Exists($"{proyecto.DirVistasReports}\\{currentView}.xml"))
                        {
                            XtraMessageBox.Show(
                                "Ya existe una vista con el mismo nombre.",
                                "Verifique...",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            BuEditView(sender, e, newView: true, Clone: false);
                            return dialog; // no cerrar permitir cambiar nombre
                        }
                        ArchivoLoad = string.Empty;
                        ArchivoLoad =
                            $"{proyecto.DirPlantillasReportes}\\{_reportFm_txtPlantilla}.xml";
                        rep = new XtraReport();
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            if (_reportFm_txtPlantilla != "Seleccione..")
                            {
                                rep.LoadLayoutFromXml(ArchivoLoad);
                            }
                        });
                    }
                    else
                    {
                        //Edicion
                        string ArchivoLoad =
                            $"{proyecto.DirVistasReports}\\{_reportFm_txtVista}.xml";
                        //rep = new XtraReport();
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            rep.LoadLayoutFromXml(ArchivoLoad);
                        });
                    }
                    rep.DataSource = dataSet;
                    ReportDesignTool designTool = CreateDesigner(rep);
                    designTool.DesignRibbonForm.DesignMdiController.AddCommandHandler(
                        new MyClosingReportCommandHandler(
                            designTool.DesignRibbonForm.DesignMdiController
                        )
                    );
                    (designTool.DesignRibbonForm as Form).FormClosing += Form_FormClosingEvt;
                    SplashScreenManager.ShowForm(typeof(fmWait));
                    designTool.ShowRibbonDesignerDialog();
                    SplashScreenManager.CloseForm();

                    if (
                        XtraMessageBox.Show(
                            $"Guardar la vista?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes
                    )
                    {
                        if (newView)
                        {
                            //Nueva vista
                            SetReportFm_txtVista(currentView);
                            if (xtraTabSelected == "ODATA")
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirApplication_WebService_WebService_Reports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                                currentView = currentView;
                            }
                            if (xtraTabSelected == "Servicio")
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirApplication_WebService_WebService_Reports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                            }
                        }
                        else
                        {
                            //Edicion
                            SetReportFm_txtVista(_reportFm_txtVista);
                            string fileReport =
                                $"{proyecto.DirVistasReports}\\{_reportFm_txtVista}.xml";
                            rep.SaveLayoutToXml(fileReport);
                            string fileReportTemp =
                                $"{proyecto.DirApplication_WebService_WebService_Reports}\\{_reportFm_txtVista}.xml";
                            rep.SaveLayoutToXml(fileReportTemp);
                        }
                    }
                    else
                    {
                        currentView = "";
                    }
                }
                else
                {
                    fmSaveView saveView = new fmSaveView(_repSelected.idReport);
                    dialog = saveView.ShowDialog(this);
                    if (saveView.Vista == "")
                        return dialog;

                    currentView = _repSelected.idReport.ToString() + "." + saveView.Vista;
                    string ArchivoLoad = $"{proyecto.DirVistasReports}\\{_reportFm_txtVista}.xml";
                    if (File.Exists($"{proyecto.DirVistasReports}\\{currentView}ODATA.xml"))
                    {
                        XtraMessageBox.Show(
                            "Ya existe una vista con el mismo nombre.",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        BuEditView(sender, e, false, true);
                        return dialog; // no cerrar permitir cambiar nombre
                    }
                    rep = new XtraReport();
                    DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                    {
                        rep.LoadLayoutFromXml(ArchivoLoad);
                    });
                    rep.DataSource = dataSet;
                    if (
                        XtraMessageBox.Show(
                            $"Desea abrir el editor de reportes?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes
                    )
                    {
                        ReportDesignTool designTool = CreateDesigner(rep);
                        designTool.DesignRibbonForm.DesignMdiController.AddCommandHandler(
                            new MyClosingReportCommandHandler(
                                designTool.DesignRibbonForm.DesignMdiController
                            )
                        );
                        (designTool.DesignRibbonForm as Form).FormClosing += Form_FormClosingEvt;
                        SplashScreenManager.ShowForm(typeof(fmWait));
                        designTool.ShowRibbonDesignerDialog();
                        SplashScreenManager.CloseForm();

                        if (
                            XtraMessageBox.Show(
                                $"Guardar la vista?",
                                this.Text,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1
                            ) == DialogResult.Yes
                        )
                        {
                            //SetReportFm_txtVista(_reportFm_txtVista);
                            if (xtraTabSelected == "ODATA")
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                                currentView = currentView + "ODATA";
                            }
                            else
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirVistasReports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                            }

                            if (xtraTabSelected == "Servicio")
                            {
                                buRun(sender, e);
                            }
                        }
                        else
                        {
                            currentView = "";
                        }
                    }
                    else
                    {
                        SetReportFm_txtVista(currentView);
                        if (xtraTabSelected == "ODATA")
                        {
                            string fileReport =
                                $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                            rep.SaveLayoutToXml(fileReport);
                            string fileReportTemp =
                                $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                            rep.SaveLayoutToXml(fileReportTemp);
                            currentView = currentView + "ODATA";
                        }
                        if (xtraTabSelected == "Servicio")
                        {
                            string fileReport = $"{proyecto.DirVistasReports}\\{currentView}.xml";
                            rep.SaveLayoutToXml(fileReport);
                            string fileReportTemp =
                                $"{proyecto.DirVistasReports}\\{currentView}.xml";
                            rep.SaveLayoutToXml(fileReportTemp);
                        }
                    }
                }
                return dialog;
            }

            #endregion Manipulación de Vistas ODATA

            #region Manipulación de Vistas por servicio

            if (xtraTabSelected == "Servicio")
            {
                //Vista de reportes por servicio
                _repSelected = grdReportesView.GetFocusedRow() as WoReport;

                if (_repSelected == null)
                {
                    return dialog;
                }

                if (grdResultadoView.RowCount == 0)
                {
                    MessageBox.Show(
                        "No hay data para cargar en el reporte, verifique.",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dialog;
                }

                fmSaveView saveView = new fmSaveView(_repSelected.idReport);
                if (!Clone)
                {
                    if (newView)
                    {
                        if (_reportFm_txtPlantilla == _templateNew)
                        {
                            XtraMessageBox.Show(
                                "Seleccione una plantilla.",
                                "Verifique...",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            return dialog;
                        }

                        dialog = saveView.ShowDialog(this);
                        if (dialog == DialogResult.Cancel)
                            return dialog;
                        if (saveView.Vista == "")
                            return dialog;

                        currentView = _repSelected.idReport.ToString() + "." + saveView.Vista;
                        string ArchivoLoad = $"{proyecto.DirVistasReports}\\{currentView}.xml";
                        if (File.Exists($"{proyecto.DirVistasReports}\\{currentView}.xml"))
                        {
                            XtraMessageBox.Show(
                                "Ya existe una vista con el mismo nombre.",
                                "Verifique...",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            BuEditView(sender, e, true, false);
                            return dialog; // no cerrar permitir cambiar nombre
                        }
                        ArchivoLoad = string.Empty;
                        ArchivoLoad =
                            $"{proyecto.DirPlantillasReportes}\\{_reportFm_txtPlantilla}.xml";
                        rep = new XtraReport();
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            if (_reportFm_txtPlantilla != "Seleccione..")
                            {
                                rep.LoadLayoutFromXml(ArchivoLoad);
                            }
                        });
                    }
                    else
                    {
                        //Edicion
                        string ArchivoLoad =
                            $"{proyecto.DirVistasReports}\\{_reportFm_txtVista}.xml";
                        DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                        {
                            rep.LoadLayoutFromXml(ArchivoLoad);
                        });
                    }
                    rep.DataSource = dataSet;
                    ReportDesignTool designTool = CreateDesigner(rep);
                    designTool.DesignRibbonForm.DesignMdiController.AddCommandHandler(
                        new MyClosingReportCommandHandler(
                            designTool.DesignRibbonForm.DesignMdiController
                        )
                    );
                    (designTool.DesignRibbonForm as Form).FormClosing += Form_FormClosingEvt;
                    SplashScreenManager.ShowForm(typeof(fmWait));
                    designTool.ShowRibbonDesignerDialog();
                    SplashScreenManager.CloseForm();

                    if (
                        XtraMessageBox.Show(
                            $"Guardar la vista?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes
                    )
                    {
                        if (newView)
                        {
                            //Nueva Vista
                            SetReportFm_txtVista(currentView.ToString());
                            if (xtraTabSelected == "ODATA")
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirApplication_WebService_WebService_Reports}\\{currentView}ODATA.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                                currentView = currentView + "ODATA";
                            }
                            if (xtraTabSelected == "Servicio")
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirApplication_WebService_WebService_Reports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                                buRun(sender, e);
                            }
                        }
                        else
                        {
                            //Edición
                            string fileReport =
                                $"{proyecto.DirVistasReports}\\{_reportFm_txtVista}.xml";
                            rep.SaveLayoutToXml(fileReport);
                            string fileReportTemp =
                                $"{proyecto.DirApplication_WebService_WebService_Reports}\\{_reportFm_txtVista}.xml";
                            rep.SaveLayoutToXml(fileReportTemp);
                            if (xtraTabSelected == "Servicio")
                            {
                                buRun(sender, e);
                            }
                            SetReportFm_txtVista(_reportFm_txtVista);
                        }
                    }
                    else
                    {
                        currentView = "";
                    }
                }
                else
                {
                    saveView.ShowDialog(this);
                    if (saveView.Vista == "")
                        return dialog;

                    currentView = _repSelected.idReport.ToString() + "." + saveView.Vista;
                    string ArchivoLoad = $"{proyecto.DirVistasReports}\\{_reportFm_txtVista}.xml";
                    if (File.Exists($"{proyecto.DirVistasReports}\\{currentView}.xml"))
                    {
                        XtraMessageBox.Show(
                            "Ya existe una vista con el mismo nombre.",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        BuEditView(sender, e, false, true);
                        return dialog; // no cerrar permitir cambiar nombre
                    }
                    rep = new XtraReport();
                    DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                    {
                        rep.LoadLayoutFromXml(ArchivoLoad);
                    });
                    rep.DataSource = dataSet;
                    if (
                        XtraMessageBox.Show(
                            $"Desea abrir el editor de reportes?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes
                    )
                    {
                        ReportDesignTool designTool = CreateDesigner(rep);
                        designTool.DesignRibbonForm.DesignMdiController.AddCommandHandler(
                            new MyClosingReportCommandHandler(
                                designTool.DesignRibbonForm.DesignMdiController
                            )
                        );
                        (designTool.DesignRibbonForm as Form).FormClosing += Form_FormClosingEvt;
                        SplashScreenManager.ShowForm(typeof(fmWait));
                        designTool.ShowRibbonDesignerDialog();
                        SplashScreenManager.CloseForm();

                        if (
                            XtraMessageBox.Show(
                                $"Guardar la vista?",
                                this.Text,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1
                            ) == DialogResult.Yes
                        )
                        {
                            SetReportFm_txtVista(currentView);
                            if (xtraTabSelected == "ODATA")
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                                currentView = currentView + "ODATA";
                            }

                            if (xtraTabSelected == "Servicio")
                            {
                                string fileReport =
                                    $"{proyecto.DirVistasReports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReport);
                                string fileReportTemp =
                                    $"{proyecto.DirVistasReports}\\{currentView}.xml";
                                rep.SaveLayoutToXml(fileReportTemp);
                                buRun(sender, e);
                            }
                        }
                        else
                        {
                            currentView = "";
                        }
                    }
                    else
                    {
                        SetReportFm_txtVista(currentView);
                        if (xtraTabSelected == "ODATA")
                        {
                            string fileReport =
                                $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                            rep.SaveLayoutToXml(fileReport);
                            string fileReportTemp =
                                $"{proyecto.DirVistasReports}\\{currentView}ODATA.xml";
                            rep.SaveLayoutToXml(fileReportTemp);
                            SetReportFm_txtVista(_reportFm_txtVista);
                            currentView = currentView + "ODATA";
                        }
                        if (xtraTabSelected == "Servicio")
                        {
                            string fileReport = $"{proyecto.DirVistasReports}\\{currentView}.xml";
                            rep.SaveLayoutToXml(fileReport);
                            string fileReportTemp =
                                $"{proyecto.DirVistasReports}\\{currentView}.xml";
                            rep.SaveLayoutToXml(fileReportTemp);
                            SetReportFm_txtVista(_reportFm_txtVista);
                            buRun(sender, e);
                        }
                    }
                }
                return dialog;
            }

            #endregion Manipulación de Vistas por servicio

            return dialog;
        }

        #endregion Botón de editar o crear vistas o plantillas

        #region Eventos de los reportes
        public void Form_FormClosingEvt(object sender, FormClosingEventArgs e)
        {
            XRDesignRibbonForm form = sender as XRDesignRibbonForm;
            for (
                int i = 0;
                i < form.DesignMdiController.XtraTabbedMdiManager.View.Documents.Count;
                i++
            )
            {
                BaseDocument document = form.DesignMdiController
                    .XtraTabbedMdiManager
                    .View
                    .Documents[i];

                XRDesignPanelForm designForm = document.Form as XRDesignPanelForm;
                if (designForm != null)
                {
                    XRDesignPanel panel = designForm.DesignPanel;
                    panel.ExecCommand(ReportCommand.Close);
                }
            }
        }
        #endregion Eventos de los reportes

        #region Cambio de reportes

        /// <summary>
        /// Clase ocupada de generar la generacion del proyecto de blazor para server side.
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Clase ocupada de generar la generacion del proyecto de blazor para server side.
        /// </summary>
        private WoBlazorWasmExecute _woBlazorWasmExecute = WoBlazorWasmExecute.GetInstance();

        private void grdReportesView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            if (xtraTabSelected == "Servicio")
            {
                _repSelected = grdReportesView.GetFocusedRow() as WoReport;
                if (File.Exists($@"{_pathReportsModels}/{_repSelected.idReport}.json"))
                {
                    string json = WoDirectory.ReadFile(
                        $@"{_pathReportsModels}\{_repSelected.idReport}.json"
                    );
                    if (json.IsNullOrStringEmpty()) // ToDo: send log
                        return;

                    _repSelected = JsonConvert.DeserializeObject<WoReport>(json);
                }
            }
            if (_repSelected == null)
                return;
            EventsViews?.Invoke(false, _repSelected);
            EventsSetStatusBtnsRowsChanged?.Invoke(false, false);
            FillPropertyGrid(null, _repSelected.idReport);
            LoadLayout(_repSelected);
            LoadTemplate();
            grdResultado.DataSource = null;
            grdResultadoView.Columns.Clear();
            repPreview.DocumentSource = null;
            tabPrincipal.SelectedTabPage = tabDatos;
            _firstOpen = false;
            _designerComponent = new WoSimpleDesigner(_repSelected.idReport);
            gcFiltros.AddControl(_designerComponent);
            _designerComponent.Dock = DockStyle.Fill;
            EventSetTemplateValue?.Invoke(false, false);
            _woBlazorServerExecute.GeneralStop();
            _woBlazorWasmExecute.GeneralStop();
            if (_repSelected != null)
            {
                BuExcecute?.Invoke();
            }
            EnablePrevio(false);
        }
        #endregion Cambio de reportes

        #region Generar unitLayout

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

        /// <summary>
        /// Nombre del proyecto de blazor wasm de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _wasmProjectName = "WasmUnitReport_proj";

        ///// <summary>
        ///// Nombre base de las clases que contendran las pruebas unitarias en claso de ser para wasm.
        ///// </summary>
        private string _wasmClassModelName = "WasmUnitReport";

        ///// <summary>
        ///// Define el nombre de la clase de la que se creara el formulario.
        ///// </summary>
        //private string _modelName = "";

        #endregion Variables principales del proyecto generado



        #endregion Generar unitLayout

        #endregion Eventos

        #region Singelton

        private volatile static WoReportsDesigner uniqueInstance = null;
        private static readonly object padlock = new object();

        public static WoReportsDesigner getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (padlock)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new WoReportsDesigner();
                    }
                }
            }
            return uniqueInstance;
        }
        #endregion Singelton

        #region Diseñador de filtros
        /// <summary>
        /// Carga el layout donde se cargara el formulario de parámetros
        /// </summary>
        /// <param name="rowSelected"></param>
        private void LoadLayout(WoReport rowSelected)
        {
            gcFiltros.Controls.Clear();
            if (_modelSelected == "")
                return;

            string _assembly =
                (_modelSelected.Contains("Formulario"))
                    ? _modelSelected.Substring(0, _modelSelected.Length - 10)
                    : _modelSelected;
            Modelo modelo = Modelo.FromJson(rowSelected.jsonModel.ToString(), proyecto);
            //TODO: Acompletar con el diseñador de reportes
            //_designerComponent = new WoFormDesigner(
            //    proyecto.DirWooWModel,
            //    rowSelected.modelType.ToString(),
            //    _assembly,
            //    modelo
            //);
            //_designerComponent.Dock = DockStyle.Fill;
            //gcFiltros.Controls.Add(_designerComponent);

            //_designerComponent.FormMode();
            //_group = _designerComponent.GetLastVersionGroup();
            //_designerComponent.HideItemComponent("Calcular");
        }
        #endregion Diseñador de filtros

        #region Funciones Auxiliares

        /// <summary>
        /// Función auxiliar para setear el valor de Odata en el reporte
        /// </summary>
        public void SetValues()
        {
            woGridodataReports1.SetValues();
        }

        /// <summary>
        /// Función para ejecutar la plantila del previo
        /// </summary>
        public void SelectedTabPag(dynamic sender, dynamic e)
        {
            if (e.Page.Name == "tabPrevio")
            {
                if (_reportFm_txtVista == _viewNew)
                {
                    WoReportBase rep = new WoReportBase();
                    repPreview.DocumentSource = rep;
                    rep.CreateDocument();
                }
                else
                {
                    buRun(sender, e);
                }
            }
        }

        /// <summary>
        /// Función que nos permite generar y administrar el diseñador del reporte, oculta algunos elementos
        /// </summary>
        /// <param name="rep"></param>
        /// <returns></returns>
        public ReportDesignTool CreateDesigner(XtraReport rep)
        {
            var designTool = new ReportDesignTool(rep);
            IDesignForm designForm = designTool.DesignRibbonForm;

            XRDesignMdiController mdiController = designForm.DesignMdiController;

            // Hide the "New with Wizard..." item on the File menu,
            // and the "Design in Report Wizard..." item in the report's smart tag.
            mdiController.SetCommandVisibility(
                ReportCommand.NewReport,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.SaveFile,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.SaveFileAs,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.NewReport,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.OpenFile,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.SaveAll,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.SaveGalleryAs,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.ChartSave,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );
            mdiController.SetCommandVisibility(
                ReportCommand.VerbRtfSaveFile,
                DevExpress.XtraReports.UserDesigner.CommandVisibility.None
            );

            return designTool;
        }

        /// <summary>
        /// Función auxiliar para habiliatar o deshabiliatar el tab de previo
        /// </summary>
        /// <param name="status"></param>
        public void EnablePrevio(bool status)
        {
            tabPrevio.PageEnabled = status;
        }

        public WoReport GetModelSelected()
        {
            WoReport reportViewGrid = grdReportesView.GetFocusedRow() as WoReport;
            return reportViewGrid;
        }

        public WoReport GetModelSelectedOdata()
        {
            WoReport reportViewGrid = woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
            return reportViewGrid;
        }

        ///// <summary>
        ///// Función que nos permite realizar la edición y asociacion del datasource
        ///// del servicio antes consumido al reporte.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        public void buEditar(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var reportTemplate = _reportFm_txtPlantilla; //Controlador!

            if (xtraTabSelected == "ODATA")
            {
                _repSelected = woGridodataReports1.grdModelsView.GetFocusedRow() as WoReport;
                EnablePrevio(false);
            }
            if (xtraTabSelected == "Servicio")
            {
                _repSelected = grdReportesView.GetFocusedRow() as WoReport;
            }
            if (reportTemplate == null || _repSelected == null)
                return;

            string Archivo = $"{proyecto.DirVistasReports}\\{_repSelected.idReport}.xml";
            string ArchivoTemp = $"{proyecto.DirReportesTemp}\\{_repSelected.idReport}.xml";

            XtraReport reportLoad;

            if (File.Exists(Archivo))
            {
                reportLoad = new XtraReport();
                DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                {
                    reportLoad.LoadLayoutFromXml(Archivo);
                });
            }
            else
            {
                if (_reportFm_txtPlantilla == templateNew)
                {
                    XtraMessageBox.Show(
                        "Seleccione una plantilla.",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
                if (grdResultadoView.RowCount == 0)
                {
                    XtraMessageBox.Show(
                        "No hay data para cargar en el reporte, verifique.",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
                string fileLoaded =
                    $"{proyecto.DirPlantillasReportes}\\{_reportFm_txtPlantilla}.xml";
                reportLoad = new XtraReport();
                DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
                {
                    reportLoad.LoadLayoutFromXml(fileLoaded);
                });
            }

            reportLoad.DataSource = dataSet;

            ReportDesignTool designTool = CreateDesigner(reportLoad);
            designTool.DesignRibbonForm.DesignMdiController.AddCommandHandler(
                new MyClosingReportCommandHandler(designTool.DesignRibbonForm.DesignMdiController)
            );
            (designTool.DesignRibbonForm as Form).FormClosing += Form_FormClosingEvt;
            (designTool.DesignForm as Form).WindowState = FormWindowState.Maximized;
            // Show a splashscreen.
            SplashScreenManager.ShowForm(typeof(fmWait));
            designTool.ShowRibbonDesignerDialog();
            SplashScreenManager.CloseForm();

            if (
                XtraMessageBox.Show(
                    $"Guardar el reporte?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) == DialogResult.Yes
            )
            {
                reportLoad.SaveLayoutToXml(Archivo);
                reportLoad.SaveLayoutToXml(ArchivoTemp);
                LoadTemplate();
                EventsBuRun?.Invoke(reportTemplate, reportTemplate);
                tabPrincipal.SelectedTabPage = tabPrevio;
            }
        }

        #endregion Funciones Auxiliares


        public void Cargar() { }

        public void Refrescar() { }

        /// <summary>
        /// Variable auxiliar para almacenar el valor del tab actual
        /// </summary>
        public string xtraTabSelected = "Servicio";

        private void xtraTabControl1_SelectedPageChanged(
            object sender,
            DevExpress.XtraTab.TabPageChangedEventArgs e
        )
        {
            if (e.Page.Text == "ODATA")
            {
                if (woGridodataReports1.grdModelsView.RowCount > 0)
                {
                    woGridodataReports1.CreateInstances(
                        false,
                        woGridodataReports1._repSelected.idReport
                    );
                    EventsbuEditar.Invoke(false, false);
                    xtraTabSelected = e.Page.Text;
                    EventsViews?.Invoke(false, woGridodataReports1._repSelected);
                    EnableReports?.Invoke(null, null);
                    DisableBuRun?.Invoke();
                    if (_repSelected != null)
                    {
                        BuExcecute?.Invoke();
                    }
                }
                else
                {
                    DisableReports?.Invoke(null, null);
                }
            }
            if (e.Page.Text == "Servicio")
            {
                if (grdReportesView.RowCount > 0)
                {
                    EventsbuEditar.Invoke(true, true);
                    xtraTabSelected = e.Page.Text;
                    //grdReportesView.FocusedRowHandle = 0;
                    _repSelected = grdReportesView.GetFocusedRow() as WoReport;
                    EventsViews?.Invoke(false, _repSelected);
                    EnableReports?.Invoke(null, null);
                    if (_repSelected != null)
                    {
                        BuExcecute?.Invoke();
                    }
                }
                else
                {
                    DisableReports?.Invoke(null, null);
                }
            }
        }

        #region Envio de la data a la consola

        /// <summary>
        /// Bandera que indica si ya fue enviada una alerta de error para la ejecución actual.
        /// </summary>
        public bool _sendError = false;

        /// <summary>
        /// Envío de la data a la consola.
        /// </summary>
        /// <param name="Data"></param>
        [SupportedOSPlatform("windows")]
        public void SendDataToConsole(string Data)
        {
            //consoleData.Clear();
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
                        consoleData.SelectionColor = Color.Green;
                    }
                    else
                    {
                        consoleData.SelectionColor = Color.White;
                    }

                    consoleData.AppendText($"{Data}\n\r");
                    consoleData.ScrollToCaret();
                })
            );
        }

        #endregion Envio de la data a la consola
    }
}
