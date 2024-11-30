using System;
using System.Collections.Generic;
using System.Text;
using WooW.Core;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.BlazorGenerator.BlazorGeneratorTools;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.TransitionSettings
{
    public class WoTransitionSettingsGenerator
    {
        #region Instancias singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Indica el nombre de la clase y del fichero.
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Nombre del modelo
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        /// <summary>
        /// Formulario final recuperado de alguno de los diseñadores.
        /// </summary>
        private WoContainer _woContainer = null;

        /// <summary>
        /// Instancia del modelo principal.
        /// </summary>
        private Modelo _mainModel = null;

        /// <summary>
        /// Modelo en el que nos encontramos actualmente puede ser una extension
        /// si nos encontramos en una
        /// </summary>
        private Modelo _actualModel = null;

        /// <summary>
        /// Lista de las extensiones del modelo
        /// </summary>
        private List<Modelo> _modelExtencionsCol = new List<Modelo>();

        /// <summary>
        /// Lista de las columnas del modelo con las columnas de las extensiones.
        /// </summary>
        private List<ModeloColumna> _modelColumnsCol = new List<ModeloColumna>();

        /// <summary>
        /// Maquina de estados del formulario.
        /// </summary>
        private ModeloDiagrama _woModelDiagram = null;

        #endregion Variables globales

        #region Helpers

        /// <summary>
        /// Helper para trabajar con la información del modelo y sus extensiones
        /// </summary>
        private WoProjectDataHelper _woProjectDataHelper = new WoProjectDataHelper();

        /// <summary>
        /// Helper para el manejo de los diagramas en multiples niveles de extension.
        /// </summary>
        private WoProjectDiagramHelper _woProjectDiagramHelper = new WoProjectDiagramHelper();

        #endregion Helpers


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="woContainer"></param>
        public WoTransitionSettingsGenerator(WoContainer woContainer, string classModelName)
        {
            _modelName = woContainer.ModelId;

            _mainModel = _woProjectDataHelper.GetMainModel(woContainer.ModelId);

            try
            {
                _actualModel = _woProjectDataHelper.GetActualModel(woContainer.ModelId);
                _woModelDiagram = _actualModel.Diagrama;
            }
            catch
            {
                _woModelDiagram = _mainModel.Diagrama;
            }
            _modelExtencionsCol = _woProjectDataHelper.GetExtensions(woContainer.ModelId);

            _modelColumnsCol = _woProjectDataHelper.GetFullColumns(woContainer.ModelId);

            //_model = _woToolModelsHelper.SearchModel(woContainer.ModelId);

            //_woContainer = woContainer;

            _woContainer = _woProjectDataHelper.GetFullDesing(woContainer.ModelId);
            _woContainer.IsUnit = woContainer.IsUnit;

            _classModelName = classModelName;

            CalculateIdentSpaces();
        }

        #endregion Constructor

        #region Identación

        /// <summary>
        /// Nivel de identación para grupos internos y controles.
        /// </summary>
        private int _identLevel = 0;

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de la clase;
        /// </summary>
        private String _identClass = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de las propiedades;
        /// </summary>
        private String _identMethodsAndProperties = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado del código dentro de los métodos;
        /// </summary>
        private String _identCode = "";

        /// <summary>
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identClass = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identMethodsAndProperties = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identCode = FormatClassHelper.Ident(_identLevel);
        }

        #endregion Identación


        #region Método principal

        /// <summary>
        /// Método principal que retorna la clase ya construida, igual se ocupa de orquestar
        /// el resto de métodos.
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        public string GetTransitionSettingsClass()
        {
            BuildHeaderClass();

            BuildBaseOptionMethods();

            BuildOptionMethod();

            _finalClass.Append(_mainMethod);
            _finalClass.Append(_optionMethods);

            BuilFooterClass();

            _observer.SetLog(
                woLog: _classCreated,
                details: $@"La clase ""{_classModelName}TransitionSettings"" fue creada correctamente."
            );

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Header

        private StringBuilder _strSetBlockSettings = new StringBuilder();

        /// <summary>
        /// Construye el Header de la clase.
        /// </summary>
        private void BuildHeaderClass()
        {
            _strSetBlockSettings.Clear();

            StringBuilder strTransitionSlaves = new StringBuilder();

            StringBuilder strTransitionSlavesParameters = new StringBuilder();
            StringBuilder strTransitionSlavesSetParams = new StringBuilder();

            WoBlazorAnalize woBlazorAnalize = new WoBlazorAnalize();
            List<string> nameSlaves = woBlazorAnalize.GetSlaveNames(_woContainer);

            int slaveCount = 0;

            if (_woContainer.IsUnit)
            {
                foreach (string slaveModelName in nameSlaves)
                {
                    strTransitionSlaves.AppendLine(
                        $@"{_identClass}    private Slave{slaveCount}SlaveTransitionSettings _slave{slaveCount}SlaveTransitionSettings {{ get; set; }}"
                    );

                    strTransitionSlavesParameters.AppendLine(
                        $@"Slave{slaveCount}SlaveTransitionSettings _slave{slaveCount}SlaveTransitionSettings, "
                    );

                    strTransitionSlavesSetParams.AppendLine(
                        $@"this._slave{slaveCount}SlaveTransitionSettings = _slave{slaveCount}SlaveTransitionSettings;"
                    );

                    _strSetBlockSettings.AppendLine(
                        $@"{_identClass}        _slave{slaveCount}SlaveTransitionSettings.SetBlockSettings(transition);"
                    );

                    slaveCount++;
                }
            }
            else
            {
                foreach (string slaveModelName in nameSlaves)
                {
                    strTransitionSlaves.AppendLine(
                        $@"{_identClass}    private {slaveModelName}SlaveTransitionSettings _slave{slaveCount}SlaveTransitionSettings {{ get; set; }}"
                    );

                    strTransitionSlavesParameters.AppendLine(
                        $@"{slaveModelName}SlaveTransitionSettings _slave{slaveCount}SlaveTransitionSettings, "
                    );

                    strTransitionSlavesSetParams.AppendLine(
                        $@"this._slave{slaveCount}SlaveTransitionSettings = _slave{slaveCount}SlaveTransitionSettings;"
                    );

                    _strSetBlockSettings.AppendLine(
                        $@"{_identClass}        _slave{slaveCount}SlaveTransitionSettings.SetBlockSettings(transition);"
                    );

                    slaveCount++;
                }
            }

            _finalClass.AppendLine(
                $@"
{_identClass}public class {_classModelName}TransitionSettings
{_identClass}{{

{_identClass}    #region Constructor

{_identClass}    /// <summary>
{_identClass}    /// Asigna las instancias inyectadas desde el despachador de dependencias de blazor.
{_identClass}    /// <summary>
{_identClass}    public {_classModelName}TransitionSettings({strTransitionSlavesParameters} {_classModelName}Controls _{_classModelName}Controls)
{_identClass}    {{
{_identClass}        this._{_classModelName}Controls = _{_classModelName}Controls;
{_identClass}        {strTransitionSlavesSetParams}
{_identClass}    }}

{_identClass}    #endregion Constructor

{_identClass}    #region Atributos

{_identClass}    /// <summary>
{_identClass}    /// Inyección de la clase que contiene las vistas de los controles de la vista.
{_identClass}    /// </summary>
{_identClass}    public {_classModelName}Controls _{_classModelName}Controls {{ get; set; }}

{_identClass}    {strTransitionSlaves}

{_identClass}    #endregion Atributos"
            );
        }

        #endregion Header

        #region Footer

        /// <summary>
        /// Construye el footer de la clase.
        /// </summary>
        private void BuilFooterClass()
        {
            _finalClass.AppendLine($@"{_identClass}}}");
        }

        #endregion Footer

        #region Body

        ///-------------------------------------------------------------------------------------------------
        /// Codigo temporal solo en lo que se termina de realizar el componente del Fichero
        ///
        ///
        private List<string> noView = new List<string>();

        ///-------------------------------------------------------------------------------------------------

        /// <summary>
        /// Contienen el método con las posibles opciones de navegación en función del diagrama.
        /// </summary>
        private StringBuilder _mainMethod = new StringBuilder();

        /// <summary>
        /// Contiene los métodos con las configuraciones para los controles del formulario en
        /// función de las transacciones del diagrama.
        /// </summary>
        private StringBuilder _optionMethods = new StringBuilder();

        /// <summary>
        /// Construye el cuerpo de l
        /// </summary>
        private void BuildOptionMethod()
        {
            /// Header del método principal.
            _mainMethod.AppendLine(
                $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Detona el método con la configuración de bloqueo de los controles del formulario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}/// <param name=""transitionName""></param>
{_identMethodsAndProperties}public void SetBlockSettings(string transition)
{_identMethodsAndProperties}{{"
            );

            _mainMethod.AppendLine($@"{_strSetBlockSettings}");

            /// Primera opción, se encuentra en cualquier tipo de formulario con navegación.
            _mainMethod.AppendLine(
                $@"
{_identMethodsAndProperties}    if (transition == ""NavigationMode"")
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        NavigationMode();
{_identMethodsAndProperties}    }}
{_identMethodsAndProperties}    else if (transition == ""NavigationModeNoReg"")
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        NavigationModeNoReg();
{_identMethodsAndProperties}    }}
"
            );

            /// Opciones generadas en función de las transiciones del diagrama.

            foreach (Transicion transicion in _woModelDiagram.Transiciones)
            {
                if (transicion.Tipo == eTransicionTipo.Local)
                {
                    _mainMethod.AppendLine(
                        $@"
{_identMethodsAndProperties}    else if (transition == ""{transicion.Id}"")
{_identMethodsAndProperties}    {{
{_identMethodsAndProperties}        {transicion.Id}();
{_identMethodsAndProperties}    }}"
                    );
                }

                BuildMethod(transicion);
            }

            /// Cierre del método.
            _mainMethod.AppendLine($@"{_identMethodsAndProperties}}}");
        }

        /// <summary>
        /// Construye el método de navegación.
        /// </summary>
        private void BuildBaseOptionMethods()
        {
            _optionMethods.AppendLine(
                $@"
{_identMethodsAndProperties}public void NavigationMode()
{_identMethodsAndProperties}{{"
            );

            foreach (ModeloColumna column in _modelColumnsCol)
            {
                if (ContainerContains(column.Id) && !column.Id.Contains("__"))
                {
                    if (column.TipoColumna != WoTypeColumn.Blob)
                    {
                        _optionMethods.AppendLine(
                            $@"
{_identMethodsAndProperties}    _{_classModelName}Controls.{column.Id}.EstadoHabilitado(false);
"
                        );
                    }
                    else if (column.TipoColumna == WoTypeColumn.Blob)
                    {
                        noView.Add(column.Id);
                    }
                }
            }

            _optionMethods.AppendLine(
                $@"
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoNavegacion(true);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBotonesDeDelete(true);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBarraDeTransicion(true);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBotonesDeTransicion(false);
{_identMethodsAndProperties}    _{_classModelName}Controls.Compartir.EstadoHabilitado(true);"
            );

            _optionMethods.AppendLine(
                $@"
{_identMethodsAndProperties}}}"
            );

            _optionMethods.AppendLine(
                $@"
{_identMethodsAndProperties}private void NavigationModeNoReg()
{_identMethodsAndProperties}{{
"
            );

            foreach (ModeloColumna column in _modelColumnsCol)
            {
                if (ContainerContains(column.Id) && !column.Id.Contains("__"))
                {
                    if (column.TipoColumna != WoTypeColumn.Blob)
                    {
                        _optionMethods.AppendLine(
                            $@"
{_identMethodsAndProperties}    _{_classModelName}Controls.{column.Id}.EstadoHabilitado(false);
"
                        );
                    }
                    else if (column.TipoColumna == WoTypeColumn.Blob)
                    {
                        noView.Add(column.Id);
                    }
                }
            }

            _optionMethods.AppendLine(
                $@"
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoNavegacion(false);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBotonesDeDelete(false);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBarraDeTransicion(true);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBotonesDeTransicion(false);
{_identMethodsAndProperties}    _{_classModelName}Controls.Compartir.EstadoHabilitado(true);"
            );

            _optionMethods.AppendLine(
                $@"
{_identMethodsAndProperties}}}"
            );
        }

        /// <summary>
        /// Construye el método de navegación en función de la transición que se recibe por parámetro.
        /// </summary>
        private void BuildMethod(Transicion transicion)
        {
            StringBuilder strOptionMethod = new StringBuilder();

            strOptionMethod.AppendLine(
                $@"
{_identMethodsAndProperties}public void {transicion.Id}()
{_identMethodsAndProperties}{{"
            );

            List<string> assignedColumns = new List<string>();

            // Recorremos las columnas que se ban a des habilitar

            List<string> columnsNoEditCol = _woProjectDiagramHelper.GetFullTransitionNoEditColumns(
                _modelName,
                transicion.Id
            );
            foreach (string columna in columnsNoEditCol)
            {
                string strDisableColumn =
                    $@"{_identMethodsAndProperties}    _{_classModelName}Controls.{columna}.EstadoHabilitado";

                if (
                    ContainerContains(columna)
                    && (!strOptionMethod.ToString().Contains(strDisableColumn))
                )
                {
                    if (!noView.Contains(columna))
                    {
                        strOptionMethod.AppendLine($@"{strDisableColumn}(false);");
                    }
                }

                assignedColumns.Add(columna);
            }

            // Recorremos las columnas que se ban a habilitar
            List<string> columnsEditCol = _woProjectDiagramHelper.GetFullTransitionEditColumns(
                _modelName,
                transicion.Id
            );
            foreach (string columna in columnsEditCol)
            {
                string strEnableColumn =
                    $@"{_identMethodsAndProperties}    _{_classModelName}Controls.{columna}.EstadoHabilitado";

                if (
                    ContainerContains(columna)
                    && (!strOptionMethod.ToString().Contains(strEnableColumn))
                )
                {
                    if (!noView.Contains(columna))
                    {
                        strOptionMethod.AppendLine($@"{strEnableColumn}(true);");
                    }
                }

                assignedColumns.Add(columna);
            }

            // Recorremos todas las columnas y recorremos las que no se encuentren
            foreach (ModeloColumna columna in _modelColumnsCol)
            {
                if (!columna.Id.Contains("__"))
                {
                    string strDisableColumn =
                        $@"{_identMethodsAndProperties}    _{_classModelName}Controls.{columna.Id}.EstadoHabilitado";

                    if (!assignedColumns.Contains(columna.Id))
                    {
                        if (
                            ContainerContains(columna.Id)
                            && (!strOptionMethod.ToString().Contains(strDisableColumn))
                        )
                        {
                            if (!noView.Contains(columna.Id))
                            {
                                strOptionMethod.AppendLine($@"{strDisableColumn}(false);");
                            }
                        }
                    }
                }
            }

            strOptionMethod.AppendLine(
                $@"
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoNavegacion(false);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBotonesDeDelete(false);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBarraDeTransicion(false);
{_identMethodsAndProperties}    _{_classModelName}Controls.Controles.EstadoBotonesDeTransicion(true);
{_identMethodsAndProperties}    _{_classModelName}Controls.Compartir.EstadoHabilitado(false);"
            );
            if (transicion.DTO.Colleccion.Count > 0)
            {
                foreach (var collection in transicion.DTO.Colleccion)
                {
                    strOptionMethod.AppendLine(
                        $@"{_identMethodsAndProperties}    _{_classModelName}Controls.{collection.ModeloId}Col.EstadoHabilitado(true);"
                    );
                }
            }

            strOptionMethod.AppendLine($@"{_identMethodsAndProperties}}}");

            _optionMethods.AppendLine(strOptionMethod.ToString());
        }

        #endregion Body

        #region Tools

        /// <summary>
        /// Valida si el contenedor principal contiene el id de la columna.
        /// </summary>
        /// <param name="columnId"></param>
        /// <returns></returns>
        private bool ContainerContains(string columnId)
        {
            return Search(_woContainer, columnId);
        }

        /// <summary>
        /// Busca dentro de el contenedor principal si existe el id de la columna.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="columnId"></param>
        /// <returns></returns>
        private bool Search(WoContainer container, string columnId)
        {
            if (container.ContainersCol != null && container.ContainersCol.Count > 0)
            {
                foreach (WoContainer subContainer in container.ContainersCol)
                {
                    if (Search(subContainer, columnId))
                    {
                        return true;
                    }
                }
            }

            if (container.ItemsCol != null && container.ItemsCol.Count > 0)
            {
                foreach (WoItem item in container.ItemsCol)
                {
                    if (item.BindedProperty == columnId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion Tools

        #region Log

        private WoLog _classCreated = new WoLog()
        {
            CodeLog = "000",
            Title = "La clase de transition settings se creo correctamente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoTransitionSettingsGenerator",
                MethodOrContext = "GetObserverModelClass"
            }
        };

        #endregion Log
    }
}
