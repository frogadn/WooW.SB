using System.Collections.Generic;
using System.Text;
using WooW.Core;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports
{
    public class WoReportViewerPartial
    {
        #region Instancias singleton

        /// <summary>
        /// Es el observador que se encarga de registrar los eventos en el log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Identaci�n a nivel de la declaraci�n de los m�todos y las propiedades.
        /// </summary>
        private string _identMethodsAndProperties = "";

        /// <summary>
        /// Item del que se esta generando la parcial.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Instancia del modelo del que se generara el c�digo.
        /// </summary>
        private Modelo _model = null;

        /// <summary>
        /// Funcionalidades para trabajar con los modelos.
        /// </summary>
        private ModelHelper _modelHelper = new ModelHelper();

        /// <summary>
        /// Instancia con herramientas de b�squedas y operaciones de modelos.
        /// </summary>
        private WoToolModelsHelper _woToolModelsHelper = new WoToolModelsHelper();

        #endregion Atributos


        #region C�digo en generaci�n

        /// <summary>
        /// C�digo que se va generando para el componente.
        /// </summary>
        private StringBuilder _strComponentCode = new StringBuilder();

        #endregion C�digo en generaci�n


        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        public WoReportViewerPartial(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase


        #region M�todo principal

        /// <summary>
        /// Recupera el apartado de la parcial para el componente.
        /// </summary>
        /// <param name="woItem"></param>
        /// <returns></returns>
        public string GetCode(WoItem woItem)
        {
            _item = woItem;

            //_model = _woToolModelsHelper.SearchModel(_item.BaseModelName);

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            _model = woProjectDataHelper.GetMainModel(_item.BaseModelName);

            _strComponentCode.Clear();

            _strComponentCode.AppendLine($@"{_identMethodsAndProperties}#region {_item.Id}");

            _strComponentCode.AppendLine(BuildTag());

            _strComponentCode.AppendLine(BuildAttributes());

            _strComponentCode.AppendLine(BuildInitMethod());

            _strComponentCode.AppendLine(BuildAlerts());

            _strComponentCode.AppendLine(BuildLoadReports());

            _strComponentCode.AppendLine(BuildGetParams());

            _strComponentCode.AppendLine(BuildSelectReport());

            _strComponentCode.AppendLine(BuildShareBtn());

            _strComponentCode.AppendLine(BuildCalculateReport());

            _strComponentCode.AppendLine($@"{_identMethodsAndProperties}#endregion {_item.Id}");

            _partialReady.Details = $@"Se creo el c�digo parcial para el componente: {_item.Id}.";
            _observer.SetLog(_partialReady);

            return _strComponentCode.ToString();
        }

        #endregion M�todo principal


        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este c�digo fue generado por el fichero WoReportViewerPartial.cs en el Path: WooW.SB\WooW.SB\BlazorGenerator\BlazorComponents\BlazorCustomComponents\WoReports\WoReportViewerPartial.cs.
{_identMethodsAndProperties}// WoWSB por el generador a d�a 5-10-2023";
        }

        #endregion Tag


        #region Generaci�n de atributos

        /// <summary>
        /// Construye el apartado de los atributos del reporte.
        /// </summary>
        /// <returns></returns>
        private string BuildAttributes()
        {
            StringBuilder strAtributes = new StringBuilder();

            strAtributes.AppendLine(
                $@"
{_identMethodsAndProperties}#region Atributos

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Reporte que se visualizara en el visor.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private XtraReport _report{_item.Id} = new XtraReport();"
            );

            if (!_item.ReportOdata)
            {
                strAtributes.AppendLine(
                    $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio que permite obtener los par�metros del reporte.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoRequestService<{_item.ReportRequest}, {_item.ReportResponse}>? _sParams{_item.Id};"
                );
            }

            strAtributes.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio que permite obtener la date de la compa��a para el reporte.
{_identMethodsAndProperties}/// (Requerido en la mayor�a de reportes.)
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoRequestService<SysDatosEmpresaReq, SysDatosEmpresaRes>? _sCompany{_item.Id};

{_identMethodsAndProperties}#endregion Atributos"
            );

            return strAtributes.ToString();
        }

        #endregion Generaci�n de atributos

        #region Generaci�n del m�todo de inicio

        /// <summary>
        /// Construye el m�todo de inicio del reporte,
        /// (inicializaci�n de servicios y peticiones iniciales)
        /// </summary>
        /// <returns></returns>
        private string BuildInitMethod()
        {
            StringBuilder strInitializeReport = new StringBuilder();

            strInitializeReport.AppendLine(
                $@"
{_identMethodsAndProperties}#region Initialize 

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// M�todo que inicia los servicios y componentes necesarios para el visor de reportes.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private async Task {_item.Id}_InitializeReport()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (Client != null)
{_identMethodsAndProperties}        {{"
            );

            strInitializeReport.AppendLine(
                $@"
{_identMethodsAndProperties}            /// Servicios"
            );

            if (!_item.ReportOdata)
            {
                strInitializeReport.AppendLine(
                    $@"
{_identMethodsAndProperties}            _sParams{_item.Id} = new WoRequestService<{_item.ReportRequest}, {_item.ReportResponse}>(Client);"
                );
            }

            strInitializeReport.AppendLine(
                $@"
{_identMethodsAndProperties}            _sCompany{_item.Id} = new WoRequestService<SysDatosEmpresaReq, SysDatosEmpresaRes>(Client);
{_identMethodsAndProperties}            _sReports{_item.Id} = new WoRequestService<SysReportListReq, SysReportListRes>(Client);
{_identMethodsAndProperties}            _sReport{_item.Id} = new WoRequestService<SysReportReq, SysReportRes>(Client);

{_identMethodsAndProperties}            await {_item.Id}_LoadReports();

{_identMethodsAndProperties}            {_item.Id}_CalculateReport();"
            );

            strInitializeReport.AppendLine(
                $@"
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            throw new Exception($@""Error en InitializeReport, es necesario que el cliente se encuentre inicializado"");
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        throw new Exception($@""Ocurri� un error al iniciar alguno de los servicios InitializeReport: {{ex.Message}}"");
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Initialize "
            );

            return strInitializeReport.ToString();
        }

        #endregion Generaci�n del m�todo de inicio

        #region Generaci�n de las alertas

        /// <summary>
        /// Construye el m�todo para enviar alertas desde el componente interno.
        /// </summary>
        /// <returns></returns>
        private string BuildAlerts()
        {
            return $@"
{_identMethodsAndProperties}#region Alertas internas

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// M�todo que se puede pasar a trav�s de la propiedad de send alert de los componentes
{_identMethodsAndProperties}/// para que estos puedan enviar alertas al control principal, desde si c�digo interno.
{_identMethodsAndProperties}/// Requiere que se encuentre a instancia de _modelControls y la de la vista de las alertas internamente.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""alert""></param>
{_identMethodsAndProperties}/// <param name=""type""></param>
{_identMethodsAndProperties}/// <param name=""alwaysOnDisplay""></param>
{_identMethodsAndProperties}private void {_item.Id}_AddAlert(string alert, eTipoDeAlerta type = eTipoDeAlerta.Error, bool alwaysOnDisplay = true)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _modelControls.Alertas.AgregarAlerta(alert, type, alwaysOnDisplay);
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Alertas internas
";
        }

        #endregion Generaci�n de las alertas

        #region Generaci�n de la carga de los reportes

        /// <summary>
        /// Construye el m�todo principal para la carga de los reportes y un primer calculo de estos.
        /// </summary>
        /// <returns></returns>
        private string BuildLoadReports()
        {
            StringBuilder strLoadReports = new StringBuilder();

            strLoadReports.AppendLine(
                $@"
{_identMethodsAndProperties}#region Carga de reportes

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio que recupera un reporte en especifico.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoRequestService<SysReportReq, SysReportRes>? _sReport{_item.Id};

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia del servicio que recupera el listado de reportes.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoRequestService<SysReportListReq, SysReportListRes>? _sReports{_item.Id};

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Colecci�n de reportes que se pueden visualizar con el request actual.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private List<string> _reportsCol{_item.Id} = new List<string>();

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Repuesta del servicio en funci�n del reporte seccionado.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private SysReportRes? _strReport{_item.Id};

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Template o reporte seleccionado, puede ser nulo y mostrar la vista default.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string? _selectedReport{_item.Id} = null;"
            );

            strLoadReports.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// M�todo que se ejecuta al iniciar el formulario y consulta la lista de posibles reportes (templates)
{_identMethodsAndProperties}/// que se pueden visualizar con la data y el visor de reportes.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private async Task {_item.Id}_LoadReports()
{_identMethodsAndProperties}{{"
            );

            strLoadReports.AppendLine(
                $@"
{_identMethodsAndProperties}    if (_sReports{_item.Id} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        try
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _reportsCol{_item.Id}.Clear();
{_identMethodsAndProperties}            SysReportListReq? claseReportList = new SysReportListReq() {{ Reporte = typeof({_item.ReportRequest}).Name }};
{_identMethodsAndProperties}            SysReportReq claseReportReq = new SysReportReq() {{ Reporte = typeof({_item.ReportRequest}).Name }};
{_identMethodsAndProperties}
{_identMethodsAndProperties}            if (claseReportList != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                IList<SysReportListRes>? ReportList = await _sReports{_item.Id}.GetListResponse(claseReportList);
{_identMethodsAndProperties}
{_identMethodsAndProperties}                if (ReportList != null)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    foreach (SysReportListRes reportList in ReportList)
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        if (reportList.ReporteCol.Count > 0)
{_identMethodsAndProperties}                        {{
{_identMethodsAndProperties}                            foreach (var file in reportList.ReporteCol)
{_identMethodsAndProperties}                            {{
{_identMethodsAndProperties}                                _reportsCol{_item.Id}.Add(file);
{_identMethodsAndProperties}                            }}
{_identMethodsAndProperties}                        }}
{_identMethodsAndProperties}                    }}
{_identMethodsAndProperties}                    _selectedReport{_item.Id} = (_selectedReport{_item.Id} != null && _reportsCol{_item.Id}.Contains(_selectedReport{_item.Id})) ? _selectedReport{_item.Id} : _reportsCol{_item.Id}.FirstOrDefault();
{_identMethodsAndProperties}                    claseReportReq.Reporte = _selectedReport{_item.Id};
{_identMethodsAndProperties}
{_identMethodsAndProperties}                    if (_sReport{_item.Id} != null)
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        _strReport{_item.Id} = await _sReport{_item.Id}.GetResponse(claseReportReq);
{_identMethodsAndProperties}
{_identMethodsAndProperties}                        if (_strReport{_item.Id} != null)
{_identMethodsAndProperties}                        {{
{_identMethodsAndProperties}                            byte[] byteArray = Encoding.UTF8.GetBytes(_strReport{_item.Id}.Reporte);
{_identMethodsAndProperties}
{_identMethodsAndProperties}                            using (MemoryStream ms = new MemoryStream(byteArray))
{_identMethodsAndProperties}                            {{
{_identMethodsAndProperties}                                DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
{_identMethodsAndProperties}                                {{
{_identMethodsAndProperties}                                    _report{_item.Id}.LoadLayoutFromXml(ms, true);
{_identMethodsAndProperties}                                }});
{_identMethodsAndProperties}                                _report{_item.Id}.CreateDocument();
{_identMethodsAndProperties}                            }}
{_identMethodsAndProperties}                        }}
{_identMethodsAndProperties}                        else
{_identMethodsAndProperties}                        {{
{_identMethodsAndProperties}                            throw new Exception(""El servicio no recupero nada _strReport"");
{_identMethodsAndProperties}                        }}
{_identMethodsAndProperties}                    }}
{_identMethodsAndProperties}                    else
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        throw new Exception(""No se pudo obtener el servicio de reportes _sReport (LoadReports {_item.Id}Report)."");
{_identMethodsAndProperties}                    }}
{_identMethodsAndProperties}
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                else
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    throw new Exception(""No regreso nada el servicio de reportes (LoadReports {_item.Id}Report)."");
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}            else
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                throw new Exception(""No se pudo obtener la clase de listado de reportes claseReportList is null (LoadReports {_item.Id}Report)."");
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        catch (Exception ex)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta($""Error al cargar los reportes: {{ex.Message}}"", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    else
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta(""No se pudo obtener el servicio de listado de reportes (LoadReports {_item.Id}Report)."", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}"
            );

            strLoadReports.AppendLine(
                $@"
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Carga de reportes"
            );

            return strLoadReports.ToString();
        }

        #endregion Generaci�n de la carga de los reportes

        #region Generaci�n de la recuperaci�n de par�metros (link)
        /// <summary>
        /// Construye el m�todo para enviar alertas desde el componente interno.
        /// </summary>
        /// <returns></returns>
        private string BuildGetParams()
        {
            StringBuilder strGetParams = new StringBuilder();

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}#region Recuperaci�n de par�metros (link)"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Url que se recupera desde el navegador en la barra de b�squeda.
{_identMethodsAndProperties}/// Permite recuperar los par�metros y la base.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string _listUri = string.Empty;"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Recupera los par�metros de la url y los asigna al modelo
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private async Task {_item.Id}_GetParameters()
{_identMethodsAndProperties}{{"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_scriptsUser != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            /// Recuperaci�n del par�metro del identificador para la navegaci�n.
{_identMethodsAndProperties}            Uri? uri = NavigationManager?.ToAbsoluteUri(NavigationManager.Uri);"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}            if (uri != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                var queryStrings = QueryHelpers.ParseQuery(uri.Query);"
            );

            if (_item.ReportOdata)
            {
                strGetParams.AppendLine(
                    $@"
{_identMethodsAndProperties}                if (queryStrings.TryGetValue(""Select"", out var Select))
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _scriptsUser.Select = Select;
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                if (queryStrings.TryGetValue(""Filter"", out var Filter))
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _scriptsUser.Filter = Filter;
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                if (queryStrings.TryGetValue(""OrderBy"", out var OrderBy))
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _scriptsUser.OrderBy = OrderBy;
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                if (queryStrings.TryGetValue(""Top"", out var Top))
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _scriptsUser.Top = Top;
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                if (queryStrings.TryGetValue(""Skip"", out var Skip))
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _scriptsUser.Skip = Skip;
{_identMethodsAndProperties}                }}"
                );
            }
            else
            {
                strGetParams.AppendLine(
                    $@"
{_identMethodsAndProperties}                _scriptsUser.{_item.BaseModelName} = new WooW.Model.{_item.BaseModelName}();
{BuildSaveParams()}"
                );
            }

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}               if (queryStrings.TryGetValue(""Reporte"", out var reporte))
{_identMethodsAndProperties}               {{
{_identMethodsAndProperties}                   _selectedReport{_item.Id} = reporte;
{_identMethodsAndProperties}               }}
{_identMethodsAndProperties}               if (NavigationManager != null)
{_identMethodsAndProperties}               {{
{_identMethodsAndProperties}                   _listUri = NavigationManager.Uri;
{_identMethodsAndProperties}               }}"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}            }}"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            throw new Exception($@""No se pudo recuperar el par�metro {_item.Id}"");
{_identMethodsAndProperties}        }}"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta($""Error al recuperar los par�metros: {{ex.Message}}"", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}"
            );

            strGetParams.AppendLine(
                $@"
{_identMethodsAndProperties}}}
{_identMethodsAndProperties}#endregion  Recuperaci�n de par�metros (link)"
            );

            return strGetParams.ToString();
        }

        private string BuildSaveParams()
        {
            StringBuilder strListParams = new StringBuilder();

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            List<ModeloColumna> columnsCol = woProjectDataHelper.GetFullColumns(_model.Id);

            foreach (ModeloColumna column in columnsCol)
            {
                strListParams.AppendLine(
                    $@"{_identMethodsAndProperties} if (queryStrings.TryGetValue(""{column.Id}"", out var {column.Id}))"
                );

                if (
                    column.TipoColumna == WoTypeColumn.EnumInt
                    || column.TipoColumna == WoTypeColumn.EnumString
                )
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = (e{_item.BaseModelName}_{column.Id})Enum.Parse(typeof(e{_item.BaseModelName}_{column.Id}), {column.Id});
{_identMethodsAndProperties} }} "
                    );
                }
                else if (column.TipoColumna == WoTypeColumn.Date)
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = DateTime.Parse({column.Id});
{_identMethodsAndProperties} }}"
                    );
                }
                else if (column.TipoColumna == WoTypeColumn.Integer)
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = int.Parse({column.Id});
{_identMethodsAndProperties} }}"
                    );
                }
                else if (column.TipoColumna == WoTypeColumn.Smallint)
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = short.Parse({column.Id});
{_identMethodsAndProperties} }}"
                    );
                }
                else if (column.TipoColumna == WoTypeColumn.Decimal)
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = decimal.Parse({column.Id});
{_identMethodsAndProperties} }}"
                    );
                }
                else if (column.TipoColumna == WoTypeColumn.Double)
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = double.Parse({column.Id});
{_identMethodsAndProperties} }}"
                    );
                }
                else if (
                    column.TipoColumna == WoTypeColumn.DateTime
                    || column.TipoColumna == WoTypeColumn.Long
                    || column.TipoColumna == WoTypeColumn.Autoincrement
                )
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = long.Parse({column.Id});
{_identMethodsAndProperties} }}"
                    );
                }
                else if (column.TipoColumna == WoTypeColumn.Boolean)
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = bool.Parse({column.Id});
{_identMethodsAndProperties} }}"
                    );
                }
                else
                {
                    strListParams.AppendLine(
                        $@"{_identMethodsAndProperties} {{
{_identMethodsAndProperties}     _scriptsUser.{_item.BaseModelName}.{column.Id} = {column.Id};
{_identMethodsAndProperties} }} "
                    );
                }
            }
            return strListParams.ToString();
        }

        #endregion Generaci�n de la recuperaci�n de par�metros (link)

        #region Generaci�n de la selecci�n de un reporte

        /// <summary>
        /// Construye el m�todo para la selecci�n de reportes
        /// </summary>
        /// <returns></returns>
        private string BuildSelectReport()
        {
            return $@"
{_identMethodsAndProperties}#region Selecci�n de un nuevo reporte

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// M�todo que se dispara desde dentro del componente cuando el usuario modifica un nuevo reporte.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""newSelectedReport""></param>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private async Task {_item.Id}_SelectedReportChanged(string newSelectedReport)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    _selectedReport{_item.Id} = newSelectedReport;
{_identMethodsAndProperties}    SysReportReq? reportGetReq = new SysReportReq();
{_identMethodsAndProperties}    reportGetReq.Reporte = newSelectedReport;
{_identMethodsAndProperties}
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_sReport{_item.Id} != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _strReport{_item.Id} = await _sReport{_item.Id}.GetResponse(reportGetReq);
{_identMethodsAndProperties}
{_identMethodsAndProperties}            if (_strReport{_item.Id} != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                byte[] byteArray = Encoding.UTF8.GetBytes(_strReport{_item.Id}.Reporte);
{_identMethodsAndProperties}
{_identMethodsAndProperties}                using (MemoryStream ms = new MemoryStream(byteArray))
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    DevExpress.Utils.DeserializationSettings.InvokeTrusted(() =>
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        _report{_item.Id}.LoadLayoutFromXml(ms);
{_identMethodsAndProperties}                    }});
{_identMethodsAndProperties}                    _report{_item.Id}.CreateDocument();
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}            else
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                throw new Exception($@""No se recupero nada del servicio, report : {{newSelectedReport}}"");
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            throw new Exception($@""El cliente no se encuentra inicializado"");
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta($""Error al cargar el reporte seleccionado: {{ex.Message}}"", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}
{_identMethodsAndProperties}
{_identMethodsAndProperties}#endregion Selecci�n de un nuevo reporte";
        }

        #endregion Generaci�n de la selecci�n de un reporte

        #region Generaci�n de la region del bot�n compartir

        /// <summary>
        /// Construye el m�todo para enviar alertas desde el componente interno.
        /// </summary>
        /// <returns></returns>
        private string BuildShareBtn()
        {
            StringBuilder strBtnShare = new StringBuilder();

            strBtnShare.AppendLine(
                $@"
{_identMethodsAndProperties}#region bot�n Compartir"
            );

            strBtnShare.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Url que se visualizara en el componente de compartir para compartir el reporte en su estado actual.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}protected string _urlForShare = string.Empty;
{_identMethodsAndProperties}
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio de acceso al porta papeles para copiar la data al porta papeles.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoReportShareButton<App>? _wsbCopiar;"
            );

            strBtnShare.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Evento que se dispara desde el bot�n de compartir para copiar la url al porta papeles.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private void SetStatusWsbCopiar()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_wsbCopiar != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Compartir.ChangeShareEnabledEvt += _wsbCopiar.SetShareEnabled;
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}"
            );

            strBtnShare.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Recupera la lista con la informaci�n de los par�metros que recibe el componente para generar la url.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private List<WoShareParameter> GetListParams()
{_identMethodsAndProperties}{{"
            );

            strBtnShare.AppendLine(
                $@"
{_identMethodsAndProperties}    List<WoShareParameter> listParams = new List<WoShareParameter>();"
            );

            if (_item.ReportOdata)
            {
                strBtnShare.AppendLine(
                    $@"
///Pendiente"
                );
            }
            else
            {
                strBtnShare.AppendLine(BuildListParams());
            }

            strBtnShare.AppendLine(
                $@"
{_identMethodsAndProperties}    return listParams;
{_identMethodsAndProperties}}}"
            );

            strBtnShare.AppendLine(
                $@"
{_identMethodsAndProperties}#endregion bot�n Compartir"
            );

            return strBtnShare.ToString();
        }

        private string BuildListParams()
        {
            StringBuilder strListParams = new StringBuilder();

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            List<ModeloColumna> columnsCol = woProjectDataHelper.GetFullColumns(_model.Id);

            foreach (ModeloColumna column in columnsCol)
            {
                if (column.TipoColumna == WoTypeColumn.Date)
                {
                    strListParams.AppendLine(
                        $@"
{_identMethodsAndProperties}    if (_scriptsUser.{_item.BaseModelName}.{column.Id} != null && _scriptsUser.{_item.BaseModelName}.{column.Id}.ToString() != ""01/01/0001 12:00:00 a. m."")
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        listParams.Add(
{_identMethodsAndProperties}            new WoShareParameter 
{_identMethodsAndProperties}            {{ 
{_identMethodsAndProperties}                Name = ""{column.Id}"", 
{_identMethodsAndProperties}                Value = _scriptsUser.{_item.BaseModelName}.{column.Id}.ToString() 
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        );
{_identMethodsAndProperties}    }}"
                    );
                }
                else
                {
                    strListParams.AppendLine(
                        $@"
{_identMethodsAndProperties}    if (_scriptsUser.{_item.BaseModelName}.{column.Id} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        listParams.Add(new WoShareParameter {{ Name = ""{column.Id}"", Value = _scriptsUser.{_item.BaseModelName}.{column.Id}.ToString() }});
{_identMethodsAndProperties}    }}"
                    );
                }
            }
            return strListParams.ToString();
        }

        #endregion Generaci�n de la region del bot�n compartir

        #region Generaci�n del calculo del reporte

        /// <summary>
        /// Construye el m�todo que calcula el reporte.
        /// </summary>
        /// <returns></returns>
        private string BuildCalculateReport()
        {
            StringBuilder strCalculateReport = new StringBuilder();

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}#region Calculo del reporte"
            );

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Datos del reporte en funci�n de los par�metros seleccionados.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private List<{_item.ReportResponse}>? _paramsData{_item.Id};

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Datos de la empresa.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private SysDatosEmpresaRes? _companyData{_item.Id};

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Dataset del reporte.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private DataSet _mainDataSet{_item.Id} = new DataSet();"
            );

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Permite calcular el reporte en funci�n de los par�metros seleccionados.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private async void {_item.Id}_CalculateReport()
{_identMethodsAndProperties}{{"
            );

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{"
            );

            if (_item.ReportOdata)
            {
                strCalculateReport.AppendLine(
                    $@"
{_identMethodsAndProperties}        WoLookupDialogService<{_item.BaseModelName}, {_item.BaseModelName}Get, {_item.BaseModelName}List>? {_item.BaseModelName}ListGridService = null;
{_identMethodsAndProperties}        (IEnumerable<{_item.BaseModelName}>, int) resultCol;

{_identMethodsAndProperties}        if (Client != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            {_item.BaseModelName}ListGridService = new WoLookupDialogService<{_item.BaseModelName}, {_item.BaseModelName}Get, {_item.BaseModelName}List>(Client);
{_identMethodsAndProperties}            if ({_item.BaseModelName}ListGridService != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                resultCol = await {_item.BaseModelName}ListGridService.GetDataList(new WoODataModel
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    Select = _scriptsUser.Select,
{_identMethodsAndProperties}                    Filter = _scriptsUser.Filter,
{_identMethodsAndProperties}                    OrderBy = _scriptsUser.OrderBy,
{_identMethodsAndProperties}                    Top = int.Parse(_scriptsUser.Top),
{_identMethodsAndProperties}                    Skip = int.Parse(_scriptsUser.Skip)
{_identMethodsAndProperties}                }});

{_identMethodsAndProperties}                _paramsData{_item.BaseModelName}Report = new List<{_item.BaseModelName}>();
{_identMethodsAndProperties}                _paramsData{_item.BaseModelName}Report.AddRange(resultCol.Item1);
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}"
                );
            }
            else
            {
                strCalculateReport.AppendLine(
                    $@"
{_identMethodsAndProperties}        if (_sParams{_item.Id} != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _paramsData{_item.Id} = (List<{_item.ReportResponse}>)await _sParams{_item.Id}.GetListResponse(_scriptsUser.{_item.ReportRequest});
{_identMethodsAndProperties}            if (_paramsData{_item.Id} == null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                throw new Exception($@""El servicio no retorno nada Calculate report _sParams"");
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            throw new Exception($@""El servicio _sParams no se ah inicializado en CalculateReport"");
{_identMethodsAndProperties}        }}"
                );
            }

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}        if (_sCompany{_item.Id} != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _companyData{_item.Id} = await _sCompany{_item.Id}.GetResponse(new SysDatosEmpresaReq());
{_identMethodsAndProperties}            if (_companyData{_item.Id} == null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                throw new Exception($@""El servicio no retorno nada Calculate report _sCompany"");
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            throw new Exception($@""El servicio _sCompany no se ah inicializado en CalculateReport"");
{_identMethodsAndProperties}        }}"
            );

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}        List<SysEmpresa> companyList = new List<SysEmpresa>();
{_identMethodsAndProperties}        List<SysRegion> regionList = new List<SysRegion>();
{_identMethodsAndProperties}        List<SysUdn> udnList = new List<SysUdn>();"
            );

            if (!_item.ReportOdata)
            {
                strCalculateReport.AppendLine(
                    $@"
{_identMethodsAndProperties}        List<{_item.ReportRequest}> paramsList = new List<{_item.ReportRequest}>()
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _scriptsUser.{_item.ReportRequest}
{_identMethodsAndProperties}        }};"
                );
            }

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}        companyList.Add(_companyData{_item.Id}.SysEmpresa);
{_identMethodsAndProperties}        regionList.Add(_companyData{_item.Id}.SysRegion);
{_identMethodsAndProperties}        udnList.Add(_companyData{_item.Id}.SysUdn);"
            );

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}        DataTable principal = WoReportsHelper.ConvertToDataTable(_paramsData{_item.Id});
{_identMethodsAndProperties}        principal.TableName = ""Principal"";
{_identMethodsAndProperties}        DataTable dtEmpresa = WoReportsHelper.ConvertToDataTable(companyList);
{_identMethodsAndProperties}        dtEmpresa.TableName = ""Datos de la empresa"";
{_identMethodsAndProperties}        DataTable dtRegion = WoReportsHelper.ConvertToDataTable(regionList);
{_identMethodsAndProperties}        dtRegion.TableName = ""Regi�n"";
{_identMethodsAndProperties}        DataTable dtUdn = WoReportsHelper.ConvertToDataTable(udnList);
{_identMethodsAndProperties}        dtUdn.TableName = ""UDN"";"
            );

            if (!_item.ReportOdata)
            {
                strCalculateReport.AppendLine(
                    $@"
{_identMethodsAndProperties}        DataTable dtParameters = WoReportsHelper.ConvertToDataTable(paramsList);
{_identMethodsAndProperties}        dtParameters.TableName = ""Parametros"";"
                );

                strCalculateReport.AppendLine(
                    $@"
{_identMethodsAndProperties}        if (dtParameters.Columns.Count > 0)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            dtParameters.TableName = ""Parametros"";
{_identMethodsAndProperties}            dtParameters.Columns.Add(""Usuario"");
{_identMethodsAndProperties}            dtParameters.Columns.Add(""Rol"");
{_identMethodsAndProperties}            dtParameters.Columns.Add(""ListaDeParametrosPorNombre"");
{_identMethodsAndProperties}
{_identMethodsAndProperties}            DataRow dtRoleUser = dtParameters.Rows[0];
{_identMethodsAndProperties}            dtRoleUser[""Usuario""] = Client.UserName ?? ""No user name"";
{_identMethodsAndProperties}            dtRoleUser[""Rol""] = ""Admnistrador"";
{_identMethodsAndProperties}
{_identMethodsAndProperties}            string woContainer = JsonConvert.SerializeObject(_scriptsUser.{_item.ReportRequest});
{_identMethodsAndProperties}            var listParameter = woContainer.Replace(""{{"", """").Replace(""}}"", """");
{_identMethodsAndProperties}            dtRoleUser[""ListaDeParametrosPorNombre""] = listParameter;
{_identMethodsAndProperties}        }}"
                );
            }

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}        _mainDataSet{_item.Id}.Tables.Clear();
{_identMethodsAndProperties}        _mainDataSet{_item.Id}.Tables.Add(principal);
{_identMethodsAndProperties}        _mainDataSet{_item.Id}.Tables.Add(dtEmpresa);
{_identMethodsAndProperties}        _mainDataSet{_item.Id}.Tables.Add(dtRegion);
{_identMethodsAndProperties}        _mainDataSet{_item.Id}.Tables.Add(dtUdn);"
            );

            if (!_item.ReportOdata)
            {
                strCalculateReport.AppendLine(
                    $@"
{_identMethodsAndProperties}        _mainDataSet{_item.Id}.Tables.Add(dtParameters);"
                );
            }

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}        _report{_item.Id}.DataSource = _mainDataSet{_item.Id};
{_identMethodsAndProperties}        _report{_item.Id}.CreateDocument();"
            );

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta($""Error al calcular el reporte: {{ex.Message}}"", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}"
            );

            strCalculateReport.AppendLine(
                $@"
{_identMethodsAndProperties}}}
{_identMethodsAndProperties}
{_identMethodsAndProperties}#endregion Calculo del reporte"
            );

            return strCalculateReport.ToString();
        }

        #endregion Generaci�n del calculo del reporte


        #region Logs

        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el c�digo interno de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoTextEditPartial",
                MethodOrContext = "GetTextEditPartial"
            }
        };

        #endregion Logs
    }
}
