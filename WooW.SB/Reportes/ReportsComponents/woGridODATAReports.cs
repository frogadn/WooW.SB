using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using FastMember;
using Newtonsoft.Json;
using ServiceStack;
using WooW.Core;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Helpers.GeneratorsHelpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Reportes.Helpers;
using WooW.SB.Reportes.ReportModels;

namespace WooW.SB.Reportes.ReportsComponents
{
    public delegate void setSnippet(string snippet);

    public partial class woGridODATAReports : DevExpress.XtraEditors.XtraUserControl, IDisposable
    {
        public EventHandler<object> RunOdata;
        public Modelo _selectedModel { get; set; } = null;

        public EventHandler<WoReport> EventsViews;
        public EventHandler StopProccess;
        public EventHandler<string> CreateInstances;
        public EventHandler<Type> EventsLoadData;
        public EventHandler<string> DisableReports;
        public EventHandler<string> EnableReports;
        public Action BuExcecute;

        /// <summary>
        /// Clase ocupada de generar la generacion del proyecto de blazor para server side.
        /// </summary>
        private WoBlazorServerExecute _woBlazorServerExecute = WoBlazorServerExecute.GetInstance();

        /// <summary>
        /// Clase ocupada de generar la generacion del proyecto de blazor para server side.
        /// </summary>
        private WoBlazorWasmExecute _woBlazorWasmExecute = WoBlazorWasmExecute.GetInstance();

        private Proyecto proyecto;

        public setSnippet SetSnippet { get; set; } = null;

        public woGridODATAReports(Proyecto _project)
        {
            InitializeComponent();
            EnablePrevio(false);
            proyecto = _project;
            //proyecto = Proyecto.getInstance();
            _selectedModel = new Modelo();
        }

        /// <summary>
        /// Función auxiliar para habiliatar o deshabiliatar el tab de previo
        /// </summary>
        /// <param name="status"></param>
        public void EnablePrevio(bool status)
        {
            tabODataPrevio.PageEnabled = status;
        }

        /// <summary>
        /// Variable que almacenará el reporte seleccionado
        /// </summary>
        public WoReport _repSelected = new WoReport();

        /// <summary>
        /// Función que se ejecuta al hacer click sobre un registro del grid de modelos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdModelsView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            try
            {
                _repSelected = grdModelsView.GetFocusedRow() as WoReport;
                CreateInstances(false, _repSelected.idReport);
                EventsViews?.Invoke(false, _repSelected);
                grdOdata.DataSource = null;
                grdOdataView.Columns.Clear();
                DoODataModel(true);
                tabExplorador.SelectedTabPage = tabOData;
                StopProccess?.Invoke(false, null);

                _woBlazorServerExecute.GeneralStop();
                _woBlazorWasmExecute.GeneralStop();
                //Integramos la funcion del watch
                if (_repSelected != null)
                {
                    BuExcecute?.Invoke();
                }
            }
            catch (Exception ex)
            {
                var exception = ex;
                throw;
            }
        }

        public void SetValues()
        {
            try
            {
                DoODataModel(false);

                tabExplorador.SelectedTabPage = tabODataPrevio;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Carga de Data ODATA
        Assembly assemblyModelData = null;
        Type ODataType = null;
        Type ODataTypeList = null;
        int ODataPageSize = 20;
        int ODataCurrentTop = 0;
        bool ODataNextPage = true;
        bool ODataBestFit = false;
        string ODataOrderBy = string.Empty;
        string ODataFilter = string.Empty;
        string ODataLastModel = string.Empty;
        private DevExpress.Data.VirtualServerModeSource virtualServerModeSource;

        public void DoODataModel(bool IsChanged = true)
        {
            if (IsChanged && grdOdataView.RowCount == 0)
            {
                grdOdata.DataSource = null;
                grdOdataView.Columns.Clear();
            }
            ODataCurrentTop = 0;
            ODataBestFit = false;
            ODataNextPage = true;
            ODataOrderBy = string.Empty;
            ODataFilter = string.Empty;

            if (ModeloSeleccionado() == null)
                return;

            var TipoModelo = ModeloSeleccionado().TipoModelo;

            if (
                (TipoModelo == WoTypeModel.Response)
                || (TipoModelo == WoTypeModel.Request)
                || (TipoModelo == WoTypeModel.Complex)
                || (TipoModelo == WoTypeModel.Interface)
                || (TipoModelo == WoTypeModel.Class)
            )
            {
                ODataLastModel = string.Empty;
                return;
            }

            string modelo = ModeloSeleccionado().Id;
            ODataLastModel = ModeloSeleccionado().Id;

            if (proyecto.ParConexion.proc == null)
                throw new Exception("Error en la conexión");

            if (assemblyModelData == null)
            {
                assemblyModelData = Assembly.LoadFile(
                    $"{proyecto.DirProyectData}\\Assembly\\WooW.WebClient.dll"
                );
                if (assemblyModelData == null)
                {
                    throw new Exception("No se pudo cargar correctamente la Dll");
                }
                proyecto.AssemblyModelCargado = true;
            }

            ODataType = assemblyModelData.GetType($"WooW.Model.{modelo}");
            ODataTypeList = assemblyModelData.GetType($"WooW.DTO.{modelo}List");

            LogIn();

            virtualServerModeSource = new DevExpress.Data.VirtualServerModeSource(this.components);
            virtualServerModeSource.RowType = ODataType;
            virtualServerModeSource.ConfigurationChanged +=
                new System.EventHandler<DevExpress.Data.VirtualServerModeRowsEventArgs>(
                    this.virtualServerModeSource_ConfigurationChanged
                );
            virtualServerModeSource.MoreRows +=
                new System.EventHandler<DevExpress.Data.VirtualServerModeRowsEventArgs>(
                    this.virtualServerModeSource_MoreRows
                );
            virtualServerModeSource.GetUniqueValues +=
                new System.EventHandler<DevExpress.Data.VirtualServerModeGetUniqueValuesEventArgs>(
                    this.virtualServerModeSource_GetUniqueValues
                );
            virtualServerModeSource.ConfigurationChanged +=
                new System.EventHandler<DevExpress.Data.VirtualServerModeRowsEventArgs>(
                    this.ServerDataSource_ConfigurationChanged
                );

            if (!IsChanged)
            {
                RunOdata?.Invoke(dataSelected, dataSelected);
            }
            if (IsChanged || grdOdataView.RowCount == 0)
            {
                grdOdata.DataSource = virtualServerModeSource;
                //grdOdata.DataSource = dataSelected;
            }
            if (grdOdataView.Columns.Count > 0)
            {
                grdOdataView.Columns[0].Fixed = FixedStyle.Left;
                grdOdataView.BestFitColumns();
            }
        }

        object dataSelected = null;

        async void ServerDataSource_ConfigurationChanged(
            object sender,
            VirtualServerModeRowsEventArgs e
        )
        {
            try
            {
                string sOData = string.Empty;
                string sNextURL = string.Empty;
                string url = string.Empty;

                if (e.ConfigurationInfo.SortInfo != null)
                {
                    if (string.IsNullOrEmpty(sOData))
                        sOData += @"?";
                    else
                        sOData += @"&";

                    string sSort = string.Empty;

                    foreach (ServerModeOrderDescriptor order in e.ConfigurationInfo.SortInfo)
                    {
                        if (!string.IsNullOrEmpty(sSort))
                            sSort += ", ";

                        sSort += order.SortPropertyName;

                        if (order.IsDesc)
                            sSort += @" DESC";
                    }

                    sOData += @"%24orderby=" + sSort;
                }

                if (e.ConfigurationInfo.Filter != null)
                {
                    if (string.IsNullOrEmpty(sOData))
                        sOData += "?";
                    else
                        sOData += "&";

                    string sWhere = string.Empty;

                    sOData +=
                        $"$filter={WoCriteriaOperatorHelper.FormatODataQueryString(e.ConfigurationInfo.Filter)}";
                }

                if (string.IsNullOrEmpty(sOData))
                    sOData += "?%24top=80";
                else
                    sOData += "&%24top=80";

                sNextURL = woTarget.BaseUri + sOData;
                var modelSelected = ModeloSeleccionado();
                string sFilter = string.Empty;
                if (e.ConfigurationInfo.Filter != null)
                {
                    sFilter = WoCriteriaOperatorHelper.FormatODataQueryString(
                        e.ConfigurationInfo.Filter
                    );
                }

                object result = await WoGetListOData.GetDataList(
                    ODataType,
                    ODataTypeList,
                    string.Empty,
                    sFilter,
                    string.Empty,
                    true,
                    40,
                    0,
                    woTarget
                );
                Type ListType = typeof(List<>).MakeGenericType(ODataType);

                var data = Convert.ChangeType(result, ListType);

                dataSelected = data;
                RunOdata?.Invoke(data, data);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void virtualServerModeSource_ConfigurationChanged(
            object sender,
            VirtualServerModeRowsEventArgs e
        )
        {
            if (e.ConfigurationInfo.SortInfo != null)
            {
                string sSort = string.Empty;

                foreach (ServerModeOrderDescriptor order in e.ConfigurationInfo.SortInfo)
                {
                    if (!string.IsNullOrEmpty(sSort))
                        sSort += ", ";

                    sSort += order.SortPropertyName;

                    if (order.IsDesc)
                        sSort += " DESC";
                }

                ODataOrderBy = sSort;
            }

            if (e.ConfigurationInfo.Filter != null)
                ODataFilter = ODataHelpers.FormatODataQueryString(e.ConfigurationInfo.Filter);

            ODataCurrentTop = 0;
            ODataNextPage = true;
        }

        private void virtualServerModeSource_MoreRows(
            object sender,
            VirtualServerModeRowsEventArgs e
        )
        {
            e.RowsTask = System.Threading.Tasks.Task.Factory.StartNew(
                () =>
                {
                    bool moreRows;
                    var enumerator = e.UserData as IEnumerator;
                    ICollection nextBatch = GetItems();
                    if (nextBatch == null)
                    {
                        nextBatch = null; // new List();
                        moreRows = false;
                    }
                    else
                    {
                        moreRows = true;
                    }

                    return new VirtualServerModeRowsTaskResult(nextBatch, moreRows, enumerator);
                },
                e.CancellationToken
            );
        }

        private ICollection GetItems()
        {
            if ((ODataType == null) || (ODataTypeList == null))
                return null;

            if (!ODataNextPage)
                return null;

            LogIn();

            var modelList = Activator.CreateInstance(ODataTypeList);

            if (modelList == null)
                return null;

            var ODataRequest = (modelList as IWoODataRequest);

            ODataRequest.top = ODataPageSize;
            ODataRequest.skip = ODataCurrentTop;
            ODataRequest.orderby = ODataOrderBy;
            ODataRequest.filter = ODataFilter;

            MethodInfo ODataList = ODataType.GetMethod("List");

            if (ODataList == null)
                return null;

            object ODataResponse;

            try
            {
                ODataResponse = ODataList.Invoke(null, new object[] { woTarget, modelList });
            }
            catch (Exception ex)
            {
                if (ex.InnerException.IsNullOrStringEmpty())
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                else
                    XtraMessageBox.Show(
                        ex.InnerException.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                return null;
            }

            var ODataResponseAccesor = TypeAccessor.Create(ODataResponse.GetType());

            ICollection col = (ICollection)ODataResponseAccesor[ODataResponse, "Value"];
            ODataCurrentTop += col.Count;
            ODataNextPage = col.Count >= ODataPageSize;

            return col;
        }

        private void virtualServerModeSource_GetUniqueValues(
            object sender,
            VirtualServerModeGetUniqueValuesEventArgs e
        )
        {
            //e.UniqueValuesTask = new System.Threading.Tasks.Task<object[]>(() =>
            //{
            //    switch (e.ValuesPropertyName)
            //    {
            //        case "ModelPrice":
            //            return new object[] { 15000m, 150000m };
            //        case "Discount":
            //            return new object[] { 0.00, 0.05, 0.10, 0.15 };
            //        case "SalesDate":
            //            DateTime today = TutorialConstants.Today;
            //            DateTime sevenYearsAgo = new DateTime(today.Year - 7, 1, 1);
            //            int totalDays = (int)Math.Ceiling(today.Subtract(sevenYearsAgo).TotalDays);
            //            object[] days = new object[totalDays];
            //            for (int i = 0; i < days.Length; i++)
            //                days[i] = sevenYearsAgo.AddDays(i);
            //            return days;
            //        case "Trademark":
            //            return models.Select(m => m.Trademark).Distinct().Cast<object>().ToArray();
            //        case "Name":
            //            return models.Select(m => m.Name).Distinct().Cast<object>().ToArray();
            //        case "Modification":
            //            return models.Select(m => m.Modification).Distinct().Cast<object>().ToArray();
            //        default:
            //            return null;
            //    }
            //}, e.CancellationToken);
        }

        JsonApiClient woTarget = null;

        private void LogIn()
        {
            if (woTarget == null)
            {
                woTarget = new JsonApiClient("https://localhost:5101");

                var AuthenticateResponse = woTarget.Post<AuthenticateResponse>(
                    new Authenticate
                    {
                        provider = "credentials", // CredentialsAuthProvider.Name, //= credentials
                        UserName = proyecto.ParConexion.Usuario,
                        Password = proyecto.ParConexion.Password,
                        RememberMe = true,
                    }
                );

                woTarget.BearerToken = AuthenticateResponse.BearerToken;
                woTarget.GetHttpClient().DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AuthenticateResponse.BearerToken);

                var WoInstanciaUdnResponse = woTarget.Post(
                    new WoInstanciaUdnAsignar
                    {
                        Instance = proyecto.ParConexion.Instance,
                        Udn = proyecto.ParConexion.Udn,
                        Year = proyecto.ParConexion.Year,
                        InstanceType = proyecto.ParConexion.InstanceType
                    }
                );
            }
        }

        private Modelo ModeloSeleccionado()
        {
            Modelo modelo = new Modelo();

            if ((grdModelsView.GetFocusedRow().IsNull()))
                return null;

            if (grdModelsView.GetFocusedRow() is WoReport)
            {
                dynamic aux = (grdModelsView.GetFocusedRow());
                modelo.Id = aux.idReport;
                modelo.TipoModelo = aux.modelType;
                modelo.ProcesoId = aux.proccessId;
                modelo.EtiquetaId = aux.Etiqueta;
            }

            return modelo;
        }
        #endregion Carga de Data ODATA

        #region Carga de Data Models


        public void woGridODATAReports_Load()
        {
            List<WoReport> reportList = new List<WoReport>();
            try
            {
                foreach (
                    var models in proyecto
                        .ModeloCol.Modelos.Where(f =>
                            f.TipoModelo != WoTypeModel.Response
                            && (f.TipoModelo != WoTypeModel.Request)
                            && (f.TipoModelo != WoTypeModel.Complex)
                            && (f.TipoModelo != WoTypeModel.Interface)
                            && (f.TipoModelo != WoTypeModel.Class)
                            && (f.TipoModelo != WoTypeModel.TransactionSlave)
                            && (f.TipoModelo != WoTypeModel.CatalogSlave)
                            && (f.TipoModelo != WoTypeModel.Parameter)
                        )
                        .ToList()
                )
                {
                    if (File.Exists($"{proyecto.DirVistasReports}\\{models.Id}.json"))
                    {
                        string json = WoDirectory.ReadFile(
                            $@"{proyecto.DirReportesVistas}\{models.Id}.json"
                        );
                        var reports = JsonConvert.DeserializeObject<WoReport>(json);
                        reportList.Add(reports);
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
                        reportList.Add(reports);
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
                grdModels.DataSource = reportList;

                // Obtén la vista (GridView) asociada al GridControl
                GridView gridView = grdModels.MainView as GridView;

                gridView.Columns[0].Caption = "Modelo";
                gridView.Columns[1].Caption = "Proceso";
                gridView.Columns[2].Caption = "Con Vistas";

                //Dejamos los valores default bloqueados en caso de no tener modelos
                if (reportList.Count == 0)
                {
                    DisableReports?.Invoke(null, null);
                }
                else
                {
                    EnableReports?.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdModels_Load(object sender, EventArgs e)
        {
            woGridODATAReports_Load();
        }

        #endregion Carga de Data Models

        //public void Dispose()
        //{
        //    virtualServerModeSource.Dispose();
        //    virtualServerModeSource = null;
        //}
    }
}
