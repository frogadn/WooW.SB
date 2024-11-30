using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList.Nodes;
using FastMember;
using Newtonsoft.Json;
using ServiceStack;
using WooW.Core;
using WooW.Core.Common;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.FormsAux
{
    public partial class fmLibroMayor : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        Assembly assemblyModelData = null;
        Type ODataType = null;
        Type ODataTypeList = null;
        int ODataPageSize = 20;
        int ODataCurrentTop = 0;
        bool ODataNextPage = true;
        bool ODataBestFit = false;
        string ODataOrderBy = string.Empty;
        string ODataFilter = string.Empty;
        string ODataFilterCustom = string.Empty;
        string ODataSelect = string.Empty;
        string ODataLastModel = string.Empty;

        private DevExpress.Data.VirtualServerModeSource virtualServerModeSource;

        JsonApiClient woTarget = null;

        object objectList = null;

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
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

        public Proyecto proyecto { get; set; }

        public void ClearLogin()
        {
            woTarget = null;
        }

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

        public void Cargar()
        {
            if (proyecto.ParConexion.proc == null)
                return;

            if (assemblyModelData == null)
            {
                assemblyModelData = Assembly.LoadFile(proyecto.FileWooWWebClientdll);
                proyecto.AssemblyModelCargado = true;
            }

            LogIn();

            var ConSaldoRequestType = assemblyModelData.GetType("WooW.Model.ConSaldoRequest");
            var ConSaldoRequestBackGroundType = assemblyModelData.GetType(
                "WooW.Model.ConSaldoRequestBackGround"
            );

            var oConSaldoRequest = Activator.CreateInstance(ConSaldoRequestType);

            MethodInfo MetSaldoRequest = ConSaldoRequestType.GetMethod("DoBackGround");
            MethodInfo MetBackGroundResult = ConSaldoRequestType.GetMethod("BackGroundResult");

            if ((MetSaldoRequest == null) || (MetBackGroundResult == null))
                return;

            string BackGroundId = string.Empty;

            try
            {
                BackGroundId = MetSaldoRequest
                    .Invoke(null, new object[] { woTarget, oConSaldoRequest })
                    .ToString();
            }
            catch (Exception ex)
            {
                ClearLogin();
            }

            if (string.IsNullOrEmpty(BackGroundId))
            {
                try
                {
                    LogIn();
                    BackGroundId = MetSaldoRequest
                        .Invoke(null, new object[] { woTarget, oConSaldoRequest })
                        .ToString();
                }
                catch (Exception ex)
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(BackGroundId))
                return;

            var ConSaldoRequestAccesor = TypeAccessor.Create(ConSaldoRequestBackGroundType);
            var oConSaldoRequestBackGround = Activator.CreateInstance(
                ConSaldoRequestBackGroundType
            );
            ConSaldoRequestAccesor[oConSaldoRequestBackGround, "BackGroundId"] = BackGroundId;

            for (; ; )
            {
                string s = woTarget.Send<string>(oConSaldoRequestBackGround);
                WoBackGround task = JsonConvert.DeserializeObject<WoBackGround>(s);

                if (!task.IsCompleted)
                    continue;

                if (task.IsError)
                {
                    XtraMessageBox.Show(
                        task.Error,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                try
                {
                    objectList = MetBackGroundResult.Invoke(
                        null,
                        new object[] { woTarget, oConSaldoRequestBackGround }
                    );

                    Refrescar();

                    break;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }

            ClearLogin();
        }

        public void Refrescar()
        {
            if (objectList == null)
                return;

            var ConSaldoResponseType = assemblyModelData.GetType("WooW.Model.ConSaldoResponse");

            Type[] MethodTypes = new Type[] { ConSaldoResponseType };

            MethodInfo Make = typeof(fmLibroMayor).GetMethod(nameof(fmLibroMayor.CreaTreeView));
            MethodInfo Generic = Make.MakeGenericMethod(MethodTypes);

            Generic.Invoke(this, new object[] { objectList });
        }

        public void CreaTreeView<T>(List<T> list)
        {
            var Orden = new List<string>();
            Orden.Add(@"ConCuentaId");

            for (int j = 0; j <= 13; j++)
            {
                Orden.Add(@"Inicial" + j.ToString("D2"));
                Orden.Add(@"Cargos" + j.ToString("D2"));
                Orden.Add(@"Abonos" + j.ToString("D2"));
                Orden.Add(@"Neto" + j.ToString("D2"));
                Orden.Add(@"Final" + j.ToString("D2"));
            }

            var dicCatalogo = new Dictionary<string, int>();

            treeObjetos.BeginUpdate();
            treeObjetos.Columns.Clear();
            treeObjetos.Nodes.Clear();

            TypeAccessor accessor = TypeAccessor.Create(typeof(T));

            int i = 0;
            foreach (var ordered in Orden)
            {
                treeObjetos.Columns.Add();
                treeObjetos.Columns[i].Tag = ordered;

                if (ordered != "ConCuentaId")
                {
                    treeObjetos.Columns[i].UnboundType = DevExpress
                        .XtraTreeList
                        .Data
                        .UnboundColumnType
                        .Decimal;
                    treeObjetos.Columns[i].Format.FormatType = DevExpress.Utils.FormatType.Numeric;
                    treeObjetos.Columns[i].Format.FormatString = "n2";
                    treeObjetos.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress
                        .Utils
                        .HorzAlignment
                        .Far;
                    string sCaption = RegresaMesColumna(ordered);
                    treeObjetos.Columns[i].Caption = sCaption;

                    treeObjetos.Columns[i].Visible = IsVisible(ordered);
                    if (treeObjetos.Columns[i].Visible)
                    {
                        treeObjetos.Columns[i].VisibleIndex = i;
                        i++;
                    }
                }
                else
                {
                    treeObjetos.Columns[i].Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
                    treeObjetos.Columns[i].Caption = ordered;
                    treeObjetos.Columns[i].VisibleIndex = i;
                    i++;
                }
            }

            treeObjetos.EndUpdate();
            treeObjetos.BeginUnboundLoad();

            TreeListNode NodeCuenta = null;

            foreach (var item in list)
            {
                string ConCuentaId = accessor[item, "ConCuentaId"].ToString();
                string Descripcion = accessor[item, "Descripcion"].ToString();
                bool Contabiliza = accessor[item, "Contabiliza"].ToBoolean();
                int Nivel = accessor[item, "Nivel"].ToInt16();

                //DataRow drCatalogo = dtCatalogo.Rows.Find(Cuenta);

                //object[] oArray = drRow.ItemArray;
                object[] oObjetos = new object[Orden.Count];

                oObjetos[0] = string.Format("{0} {1}", ConCuentaId, Descripcion);

                int k = 0;
                bool Skip = true;
                foreach (var ordered in Orden)
                {
                    if (k != 0)
                    {
                        if (!IsVisible(ordered))
                            continue;

                        decimal dValor = accessor[item, ordered].ToDecimal();
                        oObjetos[k] = dValor;
                        if (dValor != 0.0m)
                            Skip = false;
                    }
                    k++;
                }

                if (Skip)
                    continue;

                if (Nivel == 1)
                {
                    NodeCuenta = treeObjetos.AppendNode(oObjetos, -1, null);
                    NodeCuenta.ImageIndex = 72;
                    NodeCuenta.SelectImageIndex = 71;
                    NodeCuenta.StateImageIndex = -1;

                    dicCatalogo.Add(ConCuentaId, NodeCuenta.Id);
                }
                else
                {
                    int Id = 0;
                    string sCuentaPadre = GetCuentaPadre(ConCuentaId);

                    if (!dicCatalogo.TryGetValue(sCuentaPadre, out Id))
                        continue;

                    NodeCuenta = treeObjetos.AppendNode(oObjetos, Id, null);

                    if (Contabiliza)
                    {
                        NodeCuenta.ImageIndex = 72;
                        NodeCuenta.SelectImageIndex = 71;
                        NodeCuenta.StateImageIndex = -1;
                    }
                    else
                    {
                        NodeCuenta.ImageIndex = 17;
                        NodeCuenta.SelectImageIndex = 17;
                        NodeCuenta.StateImageIndex = -1;
                    }

                    if (!dicCatalogo.TryGetValue(ConCuentaId, out Id))
                        dicCatalogo.Add(ConCuentaId, NodeCuenta.Id);
                }
            }

            for (int j = 0; j < treeObjetos.Columns.Count; j++)
            {
                treeObjetos.Columns[j].BestFit();
            }

            treeObjetos.Columns[0].Width = 300;
            treeObjetos.EndUnboundLoad();
            treeObjetos.BestFitColumns();
        }

        private string GetCuentaPadre(string sParCuenta)
        {
            int iPos = 0;
            for (int i = sParCuenta.Length - 1; i > 0; i--)
            {
                iPos = i;
                if (sParCuenta[i] == '.')
                    break;
            }

            return sParCuenta.Substring(0, iPos);
        }

        private string RegresaMesColumna(string ordered)
        {
            string Prefijo = ordered.Substring(0, ordered.Length - 2);
            string SubFijo = ordered.Substring(ordered.Length - 2);

            string Final = string.Empty;
            switch (SubFijo)
            {
                case "00":
                    Final = "Apertura";
                    break;
                case "01":
                    Final = "Enero";
                    break;
                case "02":
                    Final = "Febrero";
                    break;
                case "03":
                    Final = "Marzo";
                    break;
                case "04":
                    Final = "Abril";
                    break;
                case "05":
                    Final = "Mayo";
                    break;
                case "06":
                    Final = "Junio";
                    break;
                case "07":
                    Final = "Julio";
                    break;
                case "08":
                    Final = "Agosto";
                    break;
                case "09":
                    Final = "Septiembre";
                    break;
                case "10":
                    Final = "Octubre";
                    break;
                case "11":
                    Final = "Noviembre";
                    break;
                case "12":
                    Final = "Diciembre";
                    break;
                case "13":
                    Final = "Cierre";
                    break;
            }

            return Prefijo + "\r\n" + Final;
        }

        public bool IsVisible(string ordered)
        {
            string Prefijo = ordered.Substring(0, ordered.Length - 2);
            string SubFijo = ordered.Substring(ordered.Length - 2);

            bool bDato = false;
            bool bPeriodo = false;

            foreach (CheckedListBoxItem item in repDato.Items)
            {
                if (item.CheckState == CheckState.Checked)
                {
                    if (Prefijo.StartsWith(item.Value.ToString()))
                    {
                        bDato = true;
                        break;
                    }
                }
            }

            foreach (CheckedListBoxItem item in repPeriodos.Items)
            {
                if (item.CheckState == CheckState.Checked)
                {
                    if (SubFijo.StartsWith(item.Tag.ToString()))
                    {
                        bPeriodo = true;
                        break;
                    }
                }
            }

            return bDato && bPeriodo;
        }

        public fmLibroMayor()
        {
            InitializeComponent();
        }

        private void buRefrescar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Cargar();
        }

        private void treeObjetos_CustomDrawNodeCell(
            object sender,
            DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e
        )
        {
            try
            {
                if (e.Column.Tag == null)
                    return;

                if ((e.Column.Tag == null) || (e.Node.Focused))
                {
                    e.Appearance.Font = new System.Drawing.Font(
                        e.Appearance.Font.FontFamily.Name,
                        e.Appearance.Font.Size,
                        System.Drawing.FontStyle.Bold
                    );
                    return;
                }

                if (e.Column.Tag.ToString().StartsWith("I"))
                {
                    System.Drawing.Color b = e.Appearance.BackColor;
                    System.Drawing.Color n = System.Drawing.Color.FromArgb(
                        b.A,
                        (b.R + 0 > 255 ? 255 : b.R + 0),
                        (b.G + 0 > 255 ? 255 : b.G + 0),
                        (b.B - 15 < 0 ? 0 : b.B - 15)
                    );
                    e.Appearance.BackColor = n;
                }
                else if (e.Column.Tag.ToString().StartsWith("F"))
                {
                    System.Drawing.Color b = e.Appearance.BackColor;
                    System.Drawing.Color n = System.Drawing.Color.FromArgb(
                        b.A,
                        (b.R - 15 < 0 ? 0 : b.R - 15),
                        (b.G - 0 < 0 ? 0 : b.G - 0),
                        (b.B + 15 > 255 ? 255 : b.B + 15)
                    );

                    e.Appearance.BackColor = n;
                }
                else if (e.Column.Tag.ToString().StartsWith("Ca"))
                {
                    System.Drawing.Color b = e.Appearance.BackColor;
                    System.Drawing.Color n = System.Drawing.Color.FromArgb(
                        b.A,
                        (b.R - 15 < 0 ? 0 : b.R - 15),
                        (b.G + 15 > 255 ? 255 : b.G + 15),
                        (b.B - 15 < 0 ? 0 : b.B - 15)
                    );

                    e.Appearance.BackColor = n;
                }
                else if (e.Column.Tag.ToString().StartsWith("A"))
                {
                    System.Drawing.Color b = e.Appearance.BackColor;
                    System.Drawing.Color n = System.Drawing.Color.FromArgb(
                        b.A,
                        (b.R - 0 < 0 ? 0 : b.R - 0),
                        (b.G - 15 < 0 ? 0 : b.G - 15),
                        (b.B - 15 < 0 ? 0 : b.B - 15)
                    );

                    e.Appearance.BackColor = n;
                }
            }
            catch { }
        }

        private void txtDato_EditValueChanged(object sender, EventArgs e)
        {
            Refrescar();
        }

        private void buPeriodos_EditValueChanged(object sender, EventArgs e)
        {
            Refrescar();
        }

        private void treeObjetos_FocusedColumnChanged(
            object sender,
            DevExpress.XtraTreeList.FocusedColumnChangedEventArgs e
        )
        {
            MostrarDetalle();
        }

        private void treeObjetos_FocusedNodeChanged(
            object sender,
            DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e
        )
        {
            MostrarDetalle();
        }

        private void MostrarDetalle()
        {
            if (treeObjetos.FocusedColumn == null)
                return;

            string sCampo = treeObjetos.FocusedColumn.Tag.ToSafeString();

            if (sCampo == "ConCuentaId")
                return;

            string Parte = sCampo.Substring(0, sCampo.Length - 2);
            string Periodo = sCampo.Substring(sCampo.Length - 2);

            string Cuenta = (treeObjetos.GetFocusedRow() as ArrayList)[0].ToString();
            Cuenta = Cuenta.Substring(0, Cuenta.IndexOf(" "));

            ODataSelect =
                "WoConPolizaDetConCuentaId,WoConPolizaDetDescripcion,WoConPolizaUrnTransaccion,WoConPolizaDetDebeMN,WoConPolizaDetHaberMN";

            ODataFilterCustom =
                $"contains(WoConPolizaDetConCuentaId, '{Cuenta}%') and WoConPoliza{WoConst.WOPERIODOID} eq '{Periodo}' ";
            //$"startswith(WoConPolizaDetConCuentaId,'{Cuenta}') and WoConPoliza{WoConst.WOPERIODOID} eq '{Periodo}' ";

            if (Parte == "Cargos")
                ODataFilterCustom += $" and WoConPolizaDetDebeMN ne 0.0";
            else if (Parte == "Abonos")
                ODataFilterCustom += $" and WoConPolizaDetHaberMN ne 0.0";
            DoODataModel();

            GridColumn col = grdODataView.Columns.ColumnByFieldName("WoConPolizaDetDebeMN");

            if (col != null)
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = "n4";
                col.SummaryItem.SummaryType = SummaryItemType.Sum;
                col.SummaryItem.DisplayFormat = "{0:n4}";
                col.Width = 150;
            }

            col = grdODataView.Columns.ColumnByFieldName("WoConPolizaDetHaberMN");

            if (col != null)
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = "n4";
                col.SummaryItem.SummaryType = SummaryItemType.Sum;
                col.SummaryItem.DisplayFormat = "{0:n4}";
                col.Width = 150;
            }

            col = grdODataView.Columns.ColumnByFieldName("WoConPolizaDetDescripcion");
            col.Width = 1200;
            col = grdODataView.Columns.ColumnByFieldName("WoConPolizaUrnTransaccion");
            col.Width = 600;
            col = grdODataView.Columns.ColumnByFieldName("WoConPolizaDetConCuentaId");
            col.Width = 600;
            col = grdODataView.Columns.ColumnByFieldName("WoConPolizaDetDebeMN");
            col.Width = 350;
            col = grdODataView.Columns.ColumnByFieldName("WoConPolizaDetDebeMN");
            col.Width = 350;

            grdODataView.OptionsView.ShowFooter = true;

            grdOData.ForceInitialize();
            grdODataView.BestFitColumns();
        }

        private void DoODataModel()
        {
            grdOData.DataSource = null;
            grdODataView.Columns.Clear();
            ODataCurrentTop = 0;
            ODataBestFit = false;
            ODataNextPage = true;
            ODataOrderBy = string.Empty;
            ODataFilter = string.Empty;

            if (proyecto.ParConexion.proc == null)
                return;

            if (assemblyModelData == null)
            {
                assemblyModelData = Assembly.LoadFile(proyecto.FileWooWWebClientdll);
                proyecto.AssemblyModelCargado = true;
            }

            string modelo = "WoConPolizaDetView";

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

            grdOData.DataSource = virtualServerModeSource;
            if (grdODataView.Columns.Count > 0)
            {
                grdODataView.Columns[0].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                foreach (GridColumn col in grdODataView.Columns)
                    col.Visible = (
                        ODataSelect.Split(',').Where(x => x == col.FieldName).FirstOrDefault()
                        != null
                    );
                grdODataView.RefreshData();
                grdODataView.BestFitColumns();
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
                ODataFilter =
                    ODataFilterCustom
                    + " and "
                    + ODataHelpers.FormatODataQueryString(e.ConfigurationInfo.Filter);
            else
                ODataFilter = ODataFilterCustom;

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

        private void virtualServerModeSource_GetUniqueValues(
            object sender,
            VirtualServerModeGetUniqueValuesEventArgs e
        )
        {
            grdODataView.BestFitColumns();
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
            ODataRequest.select = ODataSelect;

            MethodInfo ODataList = ODataType.GetMethod("List");

            if (ODataList == null)
                return null;

            var ODataResponse = ODataList.Invoke(null, new object[] { woTarget, modelList });

            var ODataResponseAccesor = TypeAccessor.Create(ODataResponse.GetType());

            ICollection col = (ICollection)ODataResponseAccesor[ODataResponse, "Value"];
            ODataCurrentTop += col.Count;
            ODataNextPage = col.Count >= ODataPageSize;

            return col;
        }

        private void barButtonItem1_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            grdOData.ForceInitialize();
            grdODataView.BestFitColumns();
        }
    }
}
