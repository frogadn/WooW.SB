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
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.TransitionSettings
{
    public class WoSlaveTransitionSettingsGenerator
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
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        /// <summary>
        /// Formulario final recuperado de alguno de los diseñadores.
        /// </summary>
        private WoContainer _woContainer = null;

        /// <summary>
        /// Instancia del modelo del que se generara el código.
        /// </summary>
        private Modelo _model = null;

        /// <summary>
        /// Maquina de estados del formulario.
        /// </summary>
        private ModeloDiagrama _woModelDiagram = null;

        /// <summary>
        /// Instancia con herramientas de búsquedas y operaciones de modelos.
        /// </summary>
        private WoToolModelsHelper _woToolModelsHelper = new WoToolModelsHelper();

        #endregion Variables globales


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase.
        /// ClassModelName: nombre base de la clase que se va a generar (Slave0Slave).
        /// </summary>
        /// <param name="classModelName"></param>
        /// <param name="masterModelName"></param>
        public WoSlaveTransitionSettingsGenerator(
            string classModelName,
            string masterModelName,
            WoContainer slaveContainer
        )
        {
            _classModelName = classModelName;
            _woContainer = slaveContainer;

            //_model = _woToolModelsHelper.SearchModel(masterModelName);

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            _model = woProjectDataHelper.GetMainModel(masterModelName);
            _woModelDiagram = _model.Diagrama;

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


        #region Metodo principal

        /// <summary>
        /// Metod principal que retorna la clase generada con la configuracion de bloqueo de los controles
        /// en funcion de la trancicion.
        /// </summary>
        /// <returns></returns>
        public string GetCode()
        {
            BuildHeaderClass();

            BuildOptionMethod();

            _finalClass.AppendLine(_mainMethod.ToString());

            _finalClass.AppendLine(_optionMethods.ToString());

            BuilFooterClass();

            return _finalClass.ToString();
        }

        #endregion Metodo principal


        #region Header

        /// <summary>
        /// Construye el Header de la clase.
        /// </summary>
        private void BuildHeaderClass()
        {
            _finalClass.AppendLine(
                $@"
{_identClass}public class {_classModelName}TransitionSettings
{_identClass}{{

{_identClass}    #region Constructor

{_identClass}    /// <summary>
{_identClass}    /// Asigna las instancias inyectadas desde el despachador de dependencias de blazor.
{_identClass}    /// <summary>
{_identClass}    public {_classModelName}TransitionSettings({_classModelName}Controls _{_classModelName}Controls)
{_identClass}    {{
{_identClass}        this._{_classModelName}Controls = _{_classModelName}Controls;
{_identClass}    }}

{_identClass}   #endregion Constructor

{_identClass}   #region Atributos

{_identClass}   /// <summary>
{_identClass}   /// Inyeccion de la clase que contiene las vistas de los controles de la vista.
{_identClass}   /// </summary>
{_identClass}   public {_classModelName}Controls _{_classModelName}Controls {{ get; set; }}

{_identClass}   #endregion Atributos"
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

        /// <summary>
        /// Contienen el método con las posibles opciones de navegación en función del diagrama.
        /// </summary>
        private StringBuilder _mainMethod = new StringBuilder();

        /// <summary>
        /// Contiene los métodos con las configuraciones para los controles del formulario en
        /// función de las transiciones del diagrama.
        /// </summary>
        private StringBuilder _optionMethods = new StringBuilder();

        /// <summary>
        /// Construye el cuerpo del método
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

            /// Primera opción, valida que la transición se pase correctamente.
            _mainMethod.AppendLine(
                $@"
{_identMethodsAndProperties}    if (transition == string.Empty)
{_identMethodsAndProperties}    {{
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
{_identMethodsAndProperties}    }}
"
                    );
                }

                BuildMethod(transicion);
            }

            /// Cierre del método.
            _mainMethod.AppendLine($@"{_identMethodsAndProperties}}}");
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

            List<ModeloDiagramaTransicionDTOColeccion> slaveDtos = transicion.DTO.Colleccion;
            ModeloDiagramaTransicionDTOColeccion dto = slaveDtos
                .Where(dto => dto.ModeloId == _woContainer.ModelId)
                .FirstOrDefault();

            //WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

            //Modelo slaveModel = woProjectDataHelper.GetMainModel(dto.ModeloId);



            if (dto != null)
            {
                Modelo slaveModel = _project
                    .ModeloCol.Modelos.Where(m => m.Id == dto.ModeloId)
                    .FirstOrDefault();

                List<string> assignedColumns = new List<string>();

                // Recorremos las columnas a des habilitar
                foreach (string columna in dto.ColumnasNoEditar)
                {
                    string strDisableColumn =
                        $@"{_identMethodsAndProperties}    _{_classModelName}Controls.{columna}.EstadoHabilitado";

                    if (
                        ContainerContains(columna)
                        && (!strOptionMethod.ToString().Contains(strDisableColumn))
                    )
                    {
                        strOptionMethod.AppendLine($@"{strDisableColumn}(false);");
                    }

                    assignedColumns.Add(columna);
                }

                // Recorremos las columnas a habilitar
                foreach (string columna in dto.Columnas)
                {
                    string strEnableColumn =
                        $@"{_identMethodsAndProperties}    _{_classModelName}Controls.{columna}.EstadoHabilitado";

                    if (
                        ContainerContains(columna)
                        && (!strOptionMethod.ToString().Contains(strEnableColumn))
                    )
                    {
                        strOptionMethod.AppendLine($@"{strEnableColumn}(true);");
                    }

                    assignedColumns.Add(columna);
                }

                // Recorremos las columnas no asignadas
                foreach (ModeloColumna columna in slaveModel.Columnas)
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
                                strOptionMethod.AppendLine($@"{strDisableColumn}(false);");
                            }
                        }
                    }
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
