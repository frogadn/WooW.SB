using System;
using System.Linq;
using System.Reflection;
using WooW.Core;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Client;
using WooW.SB.Config;
using WooW.SB.Helpers.DesignerHelpers;

namespace WooW.SB.Reportes.Client
{
    public class InstanceModelManager
    {
        #region Instancias singleton

        /// <summary>
        /// Variable con la información global del proyecto
        /// Se puede obtener toda la información general del proyecto desde esta.
        /// </summary>
        public Proyecto proyecto { get; set; }

        /// <summary>
        /// Log general del proyecto.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Representa una instancia de uno de los modelo recuperados desde
        /// la dll generada.
        /// </summary>
        private dynamic _instanceModelDyn = null;

        /// <summary>
        /// Es el tipo de la instancia dinámica.
        /// </summary>
        private Type _instanceModelType = null;

        /// <summary>
        /// Nombre del modelo del que se tiene la instancia.
        /// Igual a Type.Name.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Nombre completo de la clase que se esta buscando.
        /// WooW.Model.ModelName
        /// </summary>
        private string _fullModelName = string.Empty;

        /// <summary>
        /// Assembly que se recupera desde la ruta proporcionada.
        /// </summary>
        private Assembly _findAssembly = null;

        /// <summary>
        /// Tiene las funciones para:
        ///     convertir de string al tipo que se defina en la función genérica
        /// </summary>
        private ControlItemHelper _controlHelper = new ControlItemHelper();

        /// <summary>
        /// Permite que se recupere información del modelo a través del nombre.
        /// </summary>
        private WooW.SB.Helpers.ModelHelper _modelHelper = new WooW.SB.Helpers.ModelHelper();

        /// <summary>
        /// Nombre del modelo de la instancia
        /// </summary>
        private Modelo _modelOfInstance = new Modelo();

        #endregion Variables globales

        #region Constructor

        /// <summary>
        /// Constructor principal, ocupado de inicializar los atributos
        /// y de cargar la dll, así como de orquestar los métodos principales.
        /// </summary>
        /// <param name="dirModel"></param>
        /// <param name="fullAssemblyName"></param>
        public InstanceModelManager(string modelName, string dirModel, string fullAssemblyName)
        {
            _modelOfInstance = _modelHelper.SearchModel(modelName);
            _fullModelName = fullAssemblyName;
            string fullAssemblyRoute = dirModel;
            ChargeAssembly(fullAssemblyRoute);
        }

        #endregion Constructor

        #region Creación de la instancia

        /// <summary>
        /// Carga la assembly desde la ruta proporcionada.
        /// </summary>
        private void ChargeAssembly(string fullAssemblyRoute)
        {
            _findAssembly = Assembly.LoadFile(fullAssemblyRoute);
            if (_findAssembly.IsNull())
            {
                /// ToDo, enviar a un repositorio de alertas o especificar bien.
                _observer.SetLog(_nullAssembly);
                return;
            }

            CreateInstance();
        }

        /// <summary>
        /// Crea la instancia del modelo con la assembly que se busco desde el fichero.
        /// </summary>
        private void CreateInstance()
        {
            _instanceModelType = _findAssembly.GetType(_fullModelName);
            _modelName = _instanceModelType.Name;
            _instanceModelDyn = Activator.CreateInstance(_instanceModelType);
        }

        #endregion Creación de la instancia

        #region Recuperar el tipo del enum

        /// <summary>
        /// Recupera el tipo del enum mientras coincida con el estándar de los enum
        /// eModelName_EnumName. (Pasar en esa estructura).
        /// </summary>
        /// <param name="enumName"></param>
        /// <returns></returns>
        public dynamic GetEnumType(string enumName)
        {
            dynamic enumType = _findAssembly.GetType($@"WooW.Model.{enumName}");

            if (enumType == null)
            {
                _observer.SetLog(_cantFindEnum);
            }

            return enumType;
        }

        #endregion Recuperar el tipo del enum

        #region Asignar valores a la instancia

        /// <summary>
        /// Asigna el valor que se recibe por parámetro a la propiedad con el nombre
        /// que se recibe por parámetro en el tipo de la propiedad.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void AssignValue(string propertyName, string value)
        {
            PropertyInfo findProperty = SearchProperty(propertyName);
            if (findProperty.IsNull())
                return;

            Type type = findProperty.PropertyType;

            var variable = Activator.CreateInstance(type);

            findProperty.SetValue(
                _instanceModelDyn,
                (type != "".GetType()) ? _controlHelper.CastStringToGeneric(variable, value) : value
            );
        }

        /// <summary>
        /// Permite asignar un valor a una propiedad relacionada a un spin.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void AssingnValueSpin(string propertyName, string value)
        {
            ModeloColumna column = _modelOfInstance
                .Columnas.Where(x => x.Id == propertyName)
                .FirstOrDefault();
            if (column.IsNull())
                return;

            PropertyInfo findProperty = SearchProperty(propertyName);
            if (findProperty.IsNull())
                return;

            if (column.TipoColumna == WoTypeColumn.Smallint)
            {
                findProperty.SetValue(_instanceModelDyn, short.Parse(value));
            }
            else if (column.TipoColumna == WoTypeColumn.Long)
            {
                findProperty.SetValue(_instanceModelDyn, long.Parse(value));
            }
            else if (column.TipoColumna == WoTypeColumn.Integer)
            {
                findProperty.SetValue(_instanceModelDyn, int.Parse(value));
            }
        }

        /// <summary>
        /// Asigna el valor que se recibe por el parámetro a la propiedad con el nombre
        /// que se recibe por el parámetro.
        /// Contiene una validación adicional para componentes de texto.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void AssignFromTextComponent(string propertyName, string value)
        {
            PropertyInfo findProperty = SearchProperty(propertyName);
            if (findProperty.IsNull())
                return;

            Type type = findProperty.PropertyType;

            var variable = (type != "".GetType()) ? Activator.CreateInstance(type) : "";
            findProperty.SetValue(
                _instanceModelDyn,
                (type != "".GetType()) ? _controlHelper.CastStringToGeneric(variable, value) : value
            );
        }

        /// <summary>
        /// Asigna el valor desde un combo enum a la instancia del modelo.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="selectedValue"></param>
        public void AssignFromComboEnum(string propertyName, string selectedValue)
        {
            PropertyInfo property = SearchProperty(propertyName);
            if (property.IsNull())
                return;

            var enumType = GetEnumType($@"e{_modelName}_{propertyName}");
            if (enumType == null)
                return;

            int newValue = -1;
            var valuesEnum = System.Enum.GetValues(enumType);
            foreach (var value in valuesEnum)
            {
                newValue++;
                string rawValue = selectedValue;
                if (value.ToString() == rawValue)
                    break;
            }

            property.SetValue(_instanceModelDyn, valuesEnum.GetValue(newValue));
        }

        /// <summary>
        /// Asigna el valor desde un combo enum a la instancia del modelo.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="selectedValue"></param>
        public void AssignComboEnumFromLookUp(string propertyName, string selectedValue)
        {
            PropertyInfo findProperty = SearchProperty(propertyName);
            if (findProperty.IsNull())
                return;

            Type type = findProperty.PropertyType;

            var variable = (type != "".GetType()) ? Activator.CreateInstance(type) : "";
            int indiceGuion = selectedValue.IndexOf('-');
            findProperty.SetValue(
                _instanceModelDyn,
                (type != "".GetType())
                    ? _controlHelper.CastStringToGeneric(
                        variable,
                        selectedValue.Substring(0, indiceGuion)
                    )
                    : selectedValue.Substring(0, indiceGuion)
            );
        }

        /// <summary>
        /// Recupera alguna de las propiedades de la instancia que se tiene del modelo.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private PropertyInfo SearchProperty(string propertyName)
        {
            PropertyInfo findProperty = _instanceModelType.GetProperty(propertyName);
            if (findProperty.IsNull())
                _observer.SetLog(_nullProperty);

            return findProperty;
        }

        #endregion Asignar valores a la instancia

        #region Recuperar Dto

        /// <summary>
        /// Recupera el Dto que coincida con el nombre que se recibe.
        /// Puede retornar nulo si no se encuentra.
        /// </summary>
        /// <param name="fullDtoName"></param>
        /// <returns></returns>
        public dynamic GetDto(string fullDtoName)
        {
            Type findDtoType = SearchDtoType(fullDtoName);
            if (findDtoType.IsNull())
                return null;

            dynamic dtoInstance = Activator.CreateInstance(findDtoType);

            foreach (var property in findDtoType.GetProperties())
            {
                PropertyInfo formProperty = SearchProperty(property.Name);
                if (formProperty.IsNull())
                {
                    _observer.SetLog(_inconsistencyInDto);
                }
                else
                {
                    dynamic formValue = formProperty.GetValue(_instanceModelDyn);
                    property.SetValue(dtoInstance, formValue);
                }
            }

            return dtoInstance;
        }

        /// <summary>
        /// Busca el tipo del dto en función del nombre en el ensamblado.
        /// </summary>
        /// <param name="fullDtoName"></param>
        /// <returns></returns>
        private Type SearchDtoType(string fullDtoName)
        {
            Type findDtoType = _findAssembly.GetType(fullDtoName);
            if (findDtoType == null)
            {
                _observer.SetLog(_cantFindDto);
            }

            return findDtoType;
        }

        #endregion Recuperar Dto

        #region Get y Set

        /// <summary>
        /// Retorna la instancia del modelo.
        /// Valida que la instancia no sea nula, en ese caso se envía una
        /// error y retorna un nulo.
        /// </summary>
        /// <returns></returns>
        public dynamic GetInstanceModelDyn()
        {
            if (_instanceModelDyn == null)
                _observer.SetLog(_nullInstance);

            return _instanceModelDyn;
        }

        /// <summary>
        /// Retorna el tipo de la instancia.
        /// </summary>
        /// <returns></returns>
        public Type GetInstanceModelType()
        {
            if (_instanceModelType.IsNull())
                _observer.SetLog(_nullInstance);

            return _instanceModelType;
        }

        #endregion Get y Set

        #region Consumo del servicio

        private dynamic _responseService;

        public dynamic GetResponseService()
        {
            return _responseService;
        }

        /// <summary>
        /// Consume el servicio y regresa los datos que retorna.
        /// Principalmente para el uso de los formularios de reportes.
        /// Usa datos por defecto, si se requiere pasarlos por parámetro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void Carcular(object sender, EventArgs eventArgs)
        {
            FormClient client = new FormClient();
            client.AutoAuth();
            client.SetModelType(_instanceModelType);
            client.SearchMethod(_instanceModelType);
            _responseService = client.ExecuteMethod(_instanceModelDyn);
            var x = 0;
        }

        /// <summary>
        /// ToDo: De momento solo es para probar.
        /// Evento del botón registrar, de momento para pruebas
        /// el botón se genera en el apartado de los elementos ocultos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void Nuevo(object sender, EventArgs eventArgs)
        {
            ///// Instancia del cliente.
            //FormClient client = new FormClient();
            ///// Autenticación.
            //client.AutoAuth();

            //dynamic dtoInstance = GetDto($@"WooW.DTO.{_modelName}Nuevo");
            //if (dtoInstance == null)
            //    return;

            ///// Le paso en que tipo va a buscar el método.
            //client.SetModelType(_instanceModelType);
            ///// Busco el método Post que reciba un parámetro del tipo DTO nuevo en el tipo que definí arriba.
            //client.SearchMethod(paramType: dtoInstance.GetType(), method: "Post");

            ///// Ejecuta el método.
            //client.ExecuteMethod(dtoInstance);
        }

        #endregion Consumo del servicio

        #region Alertas

        private WoLog _nullAssembly = new WoLog()
        {
            CodeLog = "000",
            Title = "La dll es nula",
            Details = "La dll que se esta intentando cargar no se encuentra.",
            UserMessage = "La dll que se esta intentando cargar no se encuentra.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails() { Class = "InstanceModelManager", LineAprox = "80" }
        };

        private WoLog _nullInstance = new WoLog()
        {
            CodeLog = "000",
            Title = "La instancia del modelo es nula",
            Details = "La instancia del modelo que esta intentando recuperar es nula.",
            UserMessage = "La instancia del modelo que esta intentando recuperar es nula.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails() { Class = "InstanceModelManager", }
        };

        private WoLog _incorrectInstance = new WoLog()
        {
            CodeLog = "000",
            Title = "La instancia que esta intentando pasa es incorrecta",
            Details =
                "La instancia que se esta recibiendo por parámetro no coincide con el tipo de instancia que se tiene",
            UserMessage =
                "La instancia que se esta recibiendo por parámetro no coincide con el tipo de instancia que se tiene",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "InstanceModelManager",
                MethodOrContext = "SetInstanceModelDyn"
            }
        };

        private WoLog _nullProperty = new WoLog()
        {
            CodeLog = "000",
            Title = "La propiedad que se esta intentando buscar no existe.",
            Details =
                "La propiedad a la que esta intentando asignar un valor no exist en el modelo seleccionado.",
            UserMessage =
                "La propiedad a la que esta intentando asignar un valor no exist en el modelo seleccionado.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "InstanceModelManager",
                MethodOrContext = "SearchProperty",
                LineAprox = "201"
            }
        };

        private WoLog _cantFindEnum = new WoLog()
        {
            CodeLog = "000",
            Title = "El enum no existe.",
            Details =
                "El enum del que esta intentando obtener sus valores no se encuentra en la dll cargada.",
            UserMessage =
                "El enum del que esta intentando obtener sus valores no se encuentra en la dll cargada.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "InstanceModelManager",
                MethodOrContext = "GetEnumValues"
            }
        };

        private WoLog _cantFindDto = new WoLog()
        {
            CodeLog = "000",
            Title = "El dto no existe.",
            Details = "El dto con el nombre que esta intentando buscar no existe en el ensamblado.",
            UserMessage =
                "El dto con el nombre que esta intentando buscar no existe en el ensamblado.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "InstanceModelManager",
                MethodOrContext = "SearchDtoType"
            }
        };

        private WoLog _inconsistencyInDto = new WoLog()
        {
            CodeLog = "000",
            Title = "La propiedad no existe",
            Details =
                "Ocurrió un error: existe una propiedad en el DTO nuevo que no existe en el modelo base.",
            UserMessage =
                "Ocurrió un error: existe una propiedad en el DTO nuevo que no existe en el modelo base.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "InstanceModelManager",
                MethodOrContext = "GetDto"
            }
        };

        #endregion Alertas
    }
}
