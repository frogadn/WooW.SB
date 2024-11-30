using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoFormToolbar
{
    public class WoFormToolbarPartial
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia del observador de logs.
        /// </summary>
        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton

        #region Propiedades base

        /// <summary>
        /// Instancia del item del que se generara el código.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Instancia del modelo del que se generara el código.
        /// </summary>
        private Modelo _model = null;

        /// <summary>
        /// Maquina de estados del formulario.
        /// </summary>
        private ModeloDiagrama _woModelDiagram = null;

        /// <summary>
        /// Identación de los métodos y propiedades.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Instancia con herramientas de búsquedas y operaciones de modelos.
        /// </summary>
        private WoToolModelsHelper _woToolModelsHelper = new WoToolModelsHelper();

        /// <summary>
        /// Resultado final del código generado.
        /// </summary>
        private StringBuilder _strRecordToolBar = new StringBuilder();

        /// <summary>
        /// Indica el prefijo del componente.
        /// </summary>
        private string _shortComponent = "Ftb";

        /// <summary>
        /// Indica el prefijo del componente pero en minúsculas.
        /// </summary>
        private string _lowShortComponent = "ftb";

        #endregion Propiedades base


        #region Código desde otros métodos

        /// <summary>
        /// Código de los métodos getData de los lookups.
        /// </summary>
        private string _methodsGetDataLookUp = string.Empty;

        #endregion Código desde otros métodos

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        public WoFormToolbarPartial(string identMethodsAndProperties)
        {
            this._identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor

        #region Método principal del generador

        /// <summary>
        /// Método principal del generador del componente record.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item, string methodsGetDataLookUps)
        {
            _item = item;

            //_model = _woToolModelsHelper.SearchModel(_item.ClassModelType);

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            _model = woProjectDataHelper.GetMainModel(_item.ClassModelType);
            _woModelDiagram = _model.Diagrama;
            _methodsGetDataLookUp = methodsGetDataLookUps;

            SearchMainTrancitions();

            _strRecordToolBar.Clear();

            _strRecordToolBar.AppendLine($@"{_identMethodsAndProperties}#region {item.Id}");

            _strRecordToolBar.AppendLine(BuildTag());

            _strRecordToolBar.AppendLine(GetServices());

            _strRecordToolBar.AppendLine(GetControls());

            _strRecordToolBar.AppendLine(GetStatus());

            _strRecordToolBar.AppendLine(GetTransitions());

            _strRecordToolBar.AppendLine(GetComboOnChange());

            _strRecordToolBar.AppendLine(GetBtnExecute());

            _strRecordToolBar.AppendLine(GetBtnCancel());

            _strRecordToolBar.AppendLine(GetBtnShared());

            _strRecordToolBar.AppendLine(GetBtnDelete());

            _strRecordToolBar.AppendLine($@"{_identMethodsAndProperties}#endregion {item.Id}");

            _partialReady.Details = $@"Se creo el código parcial para el componente: {item.Id}.";
            _observer.SetLog(_partialReady);

            return (_strRecordToolBar.ToString());
        }

        #endregion Método principal del generador

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoRecordToolBarPartial.cs.";
        }

        #endregion Tag


        #region Charge transitions

        /// <summary>
        /// Lista con el nombre y etiqueta de las tracciones principales
        /// las cuales se puede ejecutar desde cualquier otra.
        /// </summary>
        private List<(string id, string label)> _mainTransitions = new List<(string, string)>();

        /// <summary>
        /// Busca las transiciones principales del diagrama de estados,
        /// las que parten del estado 0 y se podrán ejecutar desde cualquier otra.
        /// </summary>
        private void SearchMainTrancitions()
        {
            foreach (Transicion transicion in _woModelDiagram.Transiciones)
            {
                if (transicion.EstadoInicial == 0)
                {
                    _mainTransitions.Add((transicion.Id, transicion.EtiquetaId));
                }
            }
        }

        #endregion Charge transitions


        #region servicios

        /// <summary>
        /// Genera el código de las instancias de los servicios.
        /// </summary>
        /// <returns></returns>
        public string GetServices()
        {
            StringBuilder strServices = new StringBuilder();

            strServices.AppendLine(
                $@"
{_identMethodsAndProperties}#region Servicios

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio de navegación del record
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoNavigationService<{_item.BaseModelName}, {_item.BaseModelName}Get, {_item.BaseModelName}List>? _navService;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio de transiciones del record.
{_identMethodsAndProperties}/// (Dispara la transición y envía el DTO)
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoTransitionService<{_item.BaseModelName}>? _transitionService;"
            );

            if (
                _item.ModelType == Core.WoTypeModel.Catalog
                || _item.ModelType == Core.WoTypeModel.CatalogType
            )
            {
                strServices.AppendLine(
                    $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio del delete de los registros.
{_identMethodsAndProperties}/// Solo aplicable a alguno de los formularios (Catálogos, mientras no aya referencias).
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoDeleteService<{_item.BaseModelName}, {_item.BaseModelName}Borrar>? _deleteService;
"
                );
            }

            strServices.AppendLine($@"{_identMethodsAndProperties}#endregion Servicios");

            return strServices.ToString();
        }

        #endregion servicios


        #region Controles

        /// <summary>
        /// Genera el código de los controles de navegación.
        /// </summary>
        /// <returns></returns>
        public string GetControls()
        {
            return $@"
{_identMethodsAndProperties}#region Controles

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Botones de navegación entre registros.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private async Task First_OnClick()
{_identMethodsAndProperties}{{
            
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_navService != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            {_item.BaseModelName} result = await _navService.FirstAsync();
{_identMethodsAndProperties}            if (result != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _scriptsUser.{_item.BaseModelName} = result;
{_identMethodsAndProperties}                _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationMode"");

{_identMethodsAndProperties}                _currentTransition = null;
{_identMethodsAndProperties}                _transitionsCol = _dataTransitions{_item.Id}.Where(x => x.TargetWoState.ToString() == result.WoState.ToString()).FirstOrDefault()?.AvailableTransitions ?? new List<WoTransitionWrapper>();

{_methodsGetDataLookUp}

{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnPrimeroHabilitado(false);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnAnteriorHabilitado(false);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnSiguienteHabilitado(true);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnUltimoHabilitado(true);
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (ex.Message == ""Error Last click: No se encontraron datos"")
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Mensaje, true);
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
            
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}private async Task Prior_OnClick()
{_identMethodsAndProperties}{{
            
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_navService != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            ({_item.BaseModelName} Model, bool IsFirst) result = await _navService.PriorAsync();
{_identMethodsAndProperties}            if (result.Model != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _scriptsUser.{_item.BaseModelName} = result.Model;
{_identMethodsAndProperties}                _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationMode"");

{_identMethodsAndProperties}                _currentTransition = null;
{_identMethodsAndProperties}                _transitionsCol = _dataTransitions{_item.Id}.Where(x => x.TargetWoState.ToString() == result.Model.WoState.ToString()).FirstOrDefault()?.AvailableTransitions ?? new List<WoTransitionWrapper>();

{_methodsGetDataLookUp}

{_identMethodsAndProperties}                if (result.IsFirst)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnPrimeroHabilitado(false);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnAnteriorHabilitado(false);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnSiguienteHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnUltimoHabilitado(true);
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                else
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnPrimeroHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnAnteriorHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnSiguienteHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnUltimoHabilitado(true);
{_identMethodsAndProperties}                }}

{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (ex.Message == ""Error Last click: No se encontraron datos"")
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Mensaje, true);
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
            
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}private async Task Next_OnClick()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_navService != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            ({_item.BaseModelName} Model, bool IsLast) result = await _navService.NextAsync();
{_identMethodsAndProperties}            if (result.Model != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _scriptsUser.{_item.BaseModelName} = result.Model;
{_identMethodsAndProperties}                _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationMode"");

{_identMethodsAndProperties}                _currentTransition = null;
{_identMethodsAndProperties}                _transitionsCol = _dataTransitions{_item.Id}.Where(x => x.TargetWoState.ToString() == result.Model.WoState.ToString()).FirstOrDefault()?.AvailableTransitions ?? new List<WoTransitionWrapper>();

{_methodsGetDataLookUp}

{_identMethodsAndProperties}                if (result.IsLast)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnPrimeroHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnAnteriorHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnSiguienteHabilitado(false);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnUltimoHabilitado(false);
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                else
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnPrimeroHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnAnteriorHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnSiguienteHabilitado(true);
{_identMethodsAndProperties}                    _modelControls.{_item.Id}.EstadoBtnUltimoHabilitado(true);
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (ex.Message == ""Error Last click: No se encontraron datos"")
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Mensaje, true);
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}   
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}private async Task Last_OnClick()
{_identMethodsAndProperties}{{
            
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_navService != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            {_item.BaseModelName} result = await _navService.LastAsync();
{_identMethodsAndProperties}            if (result != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _scriptsUser.{_item.BaseModelName} = result;
{_identMethodsAndProperties}                _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationMode"");

{_identMethodsAndProperties}                _currentTransition = null;
{_identMethodsAndProperties}                _transitionsCol = _dataTransitions{_item.Id}.Where(x => x.TargetWoState.ToString() == result.WoState.ToString()).FirstOrDefault()?.AvailableTransitions ?? new List<WoTransitionWrapper>();

{_methodsGetDataLookUp}

{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnPrimeroHabilitado(true);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnAnteriorHabilitado(true);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnSiguienteHabilitado(false);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnUltimoHabilitado(false);

{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationModeNoReg"");
{_identMethodsAndProperties}        _scriptsUser.{_item.BaseModelName} = new {_item.BaseModelName}();

{_identMethodsAndProperties}        if (ex.Message == ""Error Last click: No se encontraron datos"")
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Mensaje, true);
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(ex.Message, eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
            
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}private async Task MoveTo()
{_identMethodsAndProperties}{{
            
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_navService != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            ({_item.BaseModelName}? model, bool isFirst, bool isLast) result = await _navService.GetModelByParameter(_idParameter);
{_identMethodsAndProperties}            if (result.model != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _scriptsUser.{_item.BaseModelName} = result.model;
{_identMethodsAndProperties}                _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationMode"");

{_methodsGetDataLookUp}

{_identMethodsAndProperties}                _currentTransition = null;
{_identMethodsAndProperties}                _transitionsCol = _dataTransitions{_item.Id}.Where(x => x.TargetWoState.ToString() == result.model.WoState.ToString()).FirstOrDefault()?.AvailableTransitions ?? new List<WoTransitionWrapper>();

{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnPrimeroHabilitado(!result.isFirst);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnAnteriorHabilitado(!result.isFirst);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnSiguienteHabilitado(!result.isLast);
{_identMethodsAndProperties}                _modelControls.{_item.Id}.EstadoBtnUltimoHabilitado(!result.isLast);
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        throw new Exception($@""El identificador {{_idParameter}} que esta intentando buscar no se encontró: {{ex.Message}}"");
{_identMethodsAndProperties}    }}
            
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Controles
";
        }

        #endregion Controles


        #region Status

        /// <summary>
        /// Genera el método que suscribe la vista al control de la barra de navegación.
        /// </summary>
        /// <returns></returns>
        private string GetStatus()
        {
            return $@"
{_identMethodsAndProperties}#region Status

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia del control de la barra de navegación (tool bar).
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoFormToolbar<App>? _{_lowShortComponent}{_item.Id};

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Suscribe los eventos de la vista a la barra de navegación.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void SetStatus{_item.Id}()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_{_lowShortComponent}{_item.Id} != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeNavigationEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledNavigation;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeTransitionEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledTransition;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeTransitionButtonsEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledBtnTransition;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeBtnFirstEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledFirstBtn;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeBtnPriorEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledPriorBtn;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeBtnNextEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledNextBtn;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeBtnLastEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledLastBtn;
{_identMethodsAndProperties}        _modelControls.{_item.Id}.ChangeDeleteButtonsEnabledEvt += _{_lowShortComponent}{_item.Id}.SetEnabledDeleteBtn;
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Status
";
        }

        #endregion Status


        #region Transiciones

        /// <summary>
        /// Instancia los atributos necesarios para que funcione el control de selección de Transiciones.
        /// Se apoya en métodos específicos para realizar la lista con el mapa de Transiciones.
        /// </summary>
        /// <returns></returns>
        private string GetTransitions()
        {
            StringBuilder strTransitions = new StringBuilder();

            strTransitions.AppendLine(
                $@"
{_identMethodsAndProperties}#region Transiciones

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Transición seleccionada en el combo
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private string? _currentTransition;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Lista con las Transiciones interna posibles a partir de una Transición.
{_identMethodsAndProperties}/// </summary>"
            );

            List<Transicion> baseTrancitions = _woModelDiagram
                .Transiciones.Where(x => x.EstadoInicial == 0)
                .ToList();

            StringBuilder strBaseTransitions = new StringBuilder();
            strBaseTransitions.Append(
                $@"{_identMethodsAndProperties}private IEnumerable<WoTransitionWrapper> _transitionsCol = new List<WoTransitionWrapper>() {{"
            );

            foreach (Transicion transicion in baseTrancitions)
            {
                strBaseTransitions.Append(
                    $@"{_identMethodsAndProperties}            new WoTransitionWrapper {{ Id = ""{transicion.Id}"", Label = ""{transicion.EtiquetaId}"" }},"
                );
            }

            strBaseTransitions.Append($@"}};");

            strTransitions.AppendLine(strBaseTransitions.ToString());

            strTransitions.AppendLine(
                $@"{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Lista con las Transiciones que se pueden realizar y las que se cargan en el control.
{_identMethodsAndProperties}/// Todas las Transiciones del diagrama de estados.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private IEnumerable<WoTransitionModel> _dataTransitions{_item.Id} = new List<WoTransitionModel>()
{_identMethodsAndProperties}{{

{BuildTransitions()}

{_identMethodsAndProperties}}};

{_identMethodsAndProperties}#endregion Transiciones"
            );

            return strTransitions.ToString();
        }

        /// <summary>
        /// Construye la lista de traicionases que se cargan en el control.
        /// Se apoya en un método para generar las posibles Transiciones a partir del estado actual.
        /// </summary>
        /// <returns></returns>
        private string BuildTransitions()
        {
            StringBuilder trnasitions = new StringBuilder();

            foreach (Transicion transition in _woModelDiagram.Transiciones)
            {
                if (transition.Tipo == eTransicionTipo.Local)
                {
                    string isNew = string.Empty;
                    if (transition.EstadoInicial == 0)
                    {
                        isNew =
                            $@",
{_identMethodsAndProperties}        IsNew = true";
                    }

                    trnasitions.AppendLine(
                        $@"
{_identMethodsAndProperties}    new WoTransitionModel() {{ 
{_identMethodsAndProperties}        TransitionName = ""{transition.Id}"", 
{_identMethodsAndProperties}        TriggersPopup = false, 
{_identMethodsAndProperties}        TargetWoState = (e{_item.BaseModelName}_WoState){transition.EstadoFinal}, 
{_identMethodsAndProperties}        DtoType = typeof({_item.BaseModelName}{transition.Id}),
{_identMethodsAndProperties}        AvailableTransitions = new List<WoTransitionWrapper> 
{_identMethodsAndProperties}        {{

{BuildTransitionWrapers(transition)}

{_identMethodsAndProperties}        }}
{isNew}
{_identMethodsAndProperties}    }},
"
                    );
                }
            }

            return trnasitions.ToString();
        }

        /// <summary>
        /// Construye las posibles transiciones a las que se puede navegar en función de la transición que se
        /// recibe por parámetro en el wraper.
        /// </summary>
        /// <param name="transicion"></param>
        /// <returns></returns>
        private string BuildTransitionWrapers(Transicion transicion)
        {
            StringBuilder wrapersTrancition = new StringBuilder();

            List<Transicion> baseTrancitions = _woModelDiagram
                .Transiciones.Where(x => x.EstadoInicial == 0)
                .ToList();

            foreach (Transicion transicionPosible in baseTrancitions)
            {
                wrapersTrancition.AppendLine(
                    $@"{_identMethodsAndProperties}            new WoTransitionWrapper {{ Id = ""{transicionPosible.Id}"", Label = ""{transicionPosible.EtiquetaId}"" }},"
                );
            }

            IEnumerable<Transicion> posibleTransitions = _woModelDiagram.Transiciones.Where(x =>
                x.EstadoInicial == transicion.EstadoFinal && x.Tipo == eTransicionTipo.Local
            );

            if (posibleTransitions != null && posibleTransitions.Count() > 0)
            {
                foreach (Transicion transition in posibleTransitions)
                {
                    wrapersTrancition.AppendLine(
                        $@"{_identMethodsAndProperties}            new WoTransitionWrapper {{Id = ""{transition.Id}"", Label = ""{transition.EtiquetaId}""}}, "
                    );
                }
            }

            return wrapersTrancition.ToString();
        }

        #endregion Transiciones


        #region Cambio del valor del combo

        /// <summary>
        /// Construye el evento principal del combo de Transiciones.
        /// </summary>
        /// <returns></returns>
        public string GetComboOnChange()
        {
            List<Transicion> initialTransitions = _woModelDiagram
                .Transiciones.Where(x => x.EstadoInicial == 0)
                .ToList();

            return $@"
{_identMethodsAndProperties}#region Cambio de transición

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Método que se detona al seleccionar una nueva transición del toolbar.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""newValue""></param>
{_identMethodsAndProperties}private void {_item.Id}Transition_OnChange(string newValue)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    WoTransitionModel? transitionSelected = null;
{_identMethodsAndProperties}    if (_dataTransitionsControles != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        transitionSelected = _dataTransitionsControles.Where(x => x.TransitionName == newValue).FirstOrDefault();
{_identMethodsAndProperties}    }}

{_identMethodsAndProperties}    if (transitionSelected != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _currentTransition = newValue;

{_identMethodsAndProperties}        if (newValue != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _{_item.BaseModelName}TransitionSettings.SetBlockSettings(_currentTransition);
{_identMethodsAndProperties}            if (transitionSelected.IsNew)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _scriptsUser.{_item.BaseModelName} = new {_item.BaseModelName}();
{BuildInitializeDefaultValues()}

{_identMethodsAndProperties}            }}

{_identMethodsAndProperties}            _scriptsUser.{_item.BaseModelName}.WoState = (e{_item.BaseModelName}_WoState)transitionSelected.TargetWoState;

{_identMethodsAndProperties}            _scriptsUser.Transicion = _currentTransition;

{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{BuildGenerateIdMethod()}

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Envía alertas al control de alertas en función de los resultados y el poseso interno del record.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""alert""></param>
{_identMethodsAndProperties}private void SendTransitionAlert(string alert)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_alerts != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (alert == ""ok"")
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta($@""La transición se realizo correctamente."", eTipoDeAlerta.Ok);
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else if (alert == ""No Existen mas registros disponibles."")
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(alert, eTipoDeAlerta.Mensaje, siempreEnPantalla: false, tiempo: 3);
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(alert, eTipoDeAlerta.Error, siempreEnPantalla: true);
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Cambio de transición
";
        }

        private string BuildGenerateIdMethod()
        {
            if (
                _item.ModelType == Core.WoTypeModel.TransactionContable
                || _item.ModelType == Core.WoTypeModel.TransactionNoContable
            )
            {
                return $@"

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Genera el Id para una transacción, concatenando udn, serie y folio.
{_identMethodsAndProperties}/// Generando serie por año y mes.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <returns></returns>
{_identMethodsAndProperties}private string GenerateID()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    string udn = UdnResult.Result.Split("" "")[1];
{_identMethodsAndProperties}    string serie = _scriptsUser.{_item.BaseModelName}.Serie ?? $""AA{{DateTime.Now.Year - 2000}}{{(DateTime.Now.Month < 10 ? $""0{{DateTime.Now.Month}}"" : $""{{DateTime.Now.Month}}"")}}"";
{_identMethodsAndProperties}    string folio = _scriptsUser.{_item.BaseModelName}.Folio.ToString() ?? ""1"";
{_identMethodsAndProperties}
{_identMethodsAndProperties}    return $""{{udn}} {{serie}}-{{folio}}"";
{_identMethodsAndProperties}}}
";
            }
            else
            {
                return string.Empty;
            }
        }

        private string BuildInitializeDefaultValues()
        {
            if (
                _item.ModelType == Core.WoTypeModel.TransactionContable
                || _item.ModelType == Core.WoTypeModel.TransactionNoContable
            )
            {
                return $@"
{_identMethodsAndProperties}            _scriptsUser.{_item.BaseModelName}.SysUdnId = UdnResult.Result.Split("" "")[1];
{_identMethodsAndProperties}            _scriptsUser.{_item.BaseModelName}.Id = GenerateID();
{_identMethodsAndProperties}            //_scriptsUser.{_item.BaseModelName}.ConPeriodoId = (DateTime.Now.Month < 10 ? $""0{{DateTime.Now.Month}}"" : $""{{DateTime.Now.Month}}"");
{_identMethodsAndProperties}            //_scriptsUser.{_item.BaseModelName}.Serie = $""AA{{DateTime.Now.Year - 2000}}{{_scriptsUser.{_item.BaseModelName}.ConPeriodoId}}"";
";
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Cambio del valor del combo


        #region Botón de ejecución

        /// <summary>
        /// Recupera el método principal del botón de ejecución.
        /// </summary>
        /// <returns></returns>
        private string GetBtnExecute()
        {
            return $@"
{_identMethodsAndProperties}#region Botón de ejecución;

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Botón que ejecuta el comando de la transición seleccionada.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""model""></param>
{_identMethodsAndProperties}private async Task {_item.Id}Execute_OnClick()
{_identMethodsAndProperties}{{
            
{_identMethodsAndProperties}    if (_transitionService != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_currentTransition != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            Type? dtoType = _dataTransitions{_item.Id}.Where(x => x.TransitionName == _currentTransition).FirstOrDefault()?.DtoType;
{_identMethodsAndProperties}            bool isNew = _dataTransitions{_item.Id}.Where(x => x.TransitionName == _currentTransition).FirstOrDefault()?.IsNew ?? false;

{_identMethodsAndProperties}            if (dtoType != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                try
{_identMethodsAndProperties}                {{      

{(_item.ModelType == Core.WoTypeModel.TransactionContable || _item.ModelType == Core.WoTypeModel.TransactionNoContable ? $"{_identMethodsAndProperties}                    if (isNew)\r\n{_identMethodsAndProperties}                    {{\r\n{_identMethodsAndProperties}                        _scriptsUser.{_item.BaseModelName}.Id = \"\";\r\n{_identMethodsAndProperties}                    }}" : "")}

{_identMethodsAndProperties}                    {_item.BaseModelName}? result = await _transitionService.CommitTransition(dtoType, _scriptsUser.{_item.BaseModelName}, isNew);

{_identMethodsAndProperties}                    if (_navService != null)
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        if (result != null)
{_identMethodsAndProperties}                        {{
{_identMethodsAndProperties}                            _scriptsUser.{_item.BaseModelName} = result;
{_identMethodsAndProperties}                            _navService.LastValidId = result.Id;
{_identMethodsAndProperties}                            _currentTransition = null;
{_identMethodsAndProperties}                            _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationMode"");
{_identMethodsAndProperties}                            _modelControls.Alertas.AgregarAlerta($@""Transicion {{_currentTransition}} se realizo con exito"", eTipoDeAlerta.Ok);

{_identMethodsAndProperties}                            _transitionsCol = _dataTransitions{_item.Id}.Where(x => x.TargetWoState.ToString() == result.WoState.ToString()).FirstOrDefault()?.AvailableTransitions ?? new List<WoTransitionWrapper>();

{_identMethodsAndProperties}                            _scriptsUser.Transicion = ""Navegacion"";
{_identMethodsAndProperties}                        }}
{_identMethodsAndProperties}                        else
{_identMethodsAndProperties}                        {{
{_identMethodsAndProperties}                            _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error al intentar recuperar el result"");
{_identMethodsAndProperties}                        }}
{_identMethodsAndProperties}                    }}
{_identMethodsAndProperties}                    else
{_identMethodsAndProperties}                    {{
{_identMethodsAndProperties}                        _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error al intentar recuperar el Servicio de Navegación"");
{_identMethodsAndProperties}                    }}
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}                catch (Exception ex)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error: {{ex.Message}}"", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}            else
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _modelControls.Alertas.AgregarAlerta(""No se ha encontrado el tipo de DTO."", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta(""No se ha seleccionado una transición."", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    else
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta(""No se ha inicializado el servicio de transiciones."", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}

{_identMethodsAndProperties}}}
{_identMethodsAndProperties}#endregion Botón de ejecución;
";
        }

        #endregion Botón de ejecución


        #region Botón de cancel

        /// <summary>
        /// Recupera el método principal del botón de cancel.
        /// </summary>
        /// <returns></returns>
        private string GetBtnCancel()
        {
            return $@"
{_identMethodsAndProperties}#region Botón de cancel

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Botón que cancela la transición seleccionada.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""model""></param>
{_identMethodsAndProperties}private async Task {_item.Id}Cancel_OnClick()
{_identMethodsAndProperties}{{
            
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_navService != null)
{_identMethodsAndProperties}        {{
                   
{_identMethodsAndProperties}            if (_navService.LastValidId == null || _navService.LastValidId == string.Empty)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationModeNoReg"");
{_identMethodsAndProperties}                _currentTransition = null;
{_identMethodsAndProperties}                _modelControls.Alertas.AgregarAlerta($@""No hay registros"", eTipoDeAlerta.Mensaje);
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}            else
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                {_item.BaseModelName}? result = await _navService?.GetModelById(_navService.LastValidId);

{_identMethodsAndProperties}                if (result != null)
{_identMethodsAndProperties}                {{
{_identMethodsAndProperties}                    _scriptsUser.{_item.BaseModelName} = result;
{_identMethodsAndProperties}                    _{_item.BaseModelName}TransitionSettings.SetBlockSettings(""NavigationMode"");
{_identMethodsAndProperties}                    _currentTransition = null;
{_identMethodsAndProperties}                }}
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}        else
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error al intentar recuperar el Servicio de Navegación"");
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error en el CancelOnClick (Blazor): {{ex.Message}}"");
{_identMethodsAndProperties}    }}

{_identMethodsAndProperties}    _scriptsUser.Transicion = ""Navegacion"";

{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion Botón de cancel
";
        }

        #endregion Botón de cancel


        #region Botón de compartir

        /// <summary>
        /// Recupera el método principal del botón de compartir.
        /// </summary>
        /// <returns></returns>
        private string GetBtnShared()
        {
            return $@"
{_identMethodsAndProperties}#region botón Compartir

{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Servicio de acceso al porta papeles para copiar la data al porta papeles.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}private WoShareButton<App>? _wsbCopiar;

{_identMethodsAndProperties}private void SetStatusWsbCopiar()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    if (_wsbCopiar != null)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Compartir.ChangeShareEnabledEvt += _wsbCopiar.SetShareEnabled;
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion botón Compartir
";
        }

        #endregion Botón de compartir


        #region Botón de eliminar

        public string GetBtnDelete()
        {
            if (
                _item.ModelType == Core.WoTypeModel.Catalog
                || _item.ModelType == Core.WoTypeModel.CatalogType
            )
            {
                return $@"
{_identMethodsAndProperties}#region botón Eliminar

{_identMethodsAndProperties}private async Task {_item.Id}Delete_OnClick()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    try
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        if (_deleteService != null)
{_identMethodsAndProperties}        {{
{_identMethodsAndProperties}            {_item.BaseModelName}? deleteResult = await _deleteService.DeleteTransition(_scriptsUser.{_item.BaseModelName});
{_identMethodsAndProperties}            if (deleteResult != null)
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                _modelControls.Alertas.AgregarAlerta($@""Se elimino el registro con el id {{deleteResult.Id}} con exito"", eTipoDeAlerta.Ok);
{_identMethodsAndProperties}                await Last_OnClick();
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}            else
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                throw new Exception($@""No se pudo eliminar el registro con el id {{_scriptsUser.{_item.BaseModelName}.Id}}"");
{_identMethodsAndProperties}            }}
{_identMethodsAndProperties}        }}
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    catch (Exception ex)
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        _modelControls.Alertas.AgregarAlerta($@""Ocurrió un error en el ControlsDelete_OnClick (Blazor): {{ex.Message}}"", eTipoDeAlerta.Error, true);
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}}}

{_identMethodsAndProperties}#endregion botón Eliminar
";
            }

            return $@"";
        }

        #endregion Botón de eliminar


        #region Logs

        private WoLog _partialReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el código interno de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoFormToolbarPartial",
                MethodOrContext = "GetFormToolbarPartial"
            }
        };

        #endregion Logs
    }
}
