using System;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows.Forms;
using ServiceStack;
using ServiceStack.Auth;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Client
{
    public class FormClient
    {
        #region Variables globales

        /// <summary>
        /// Cliente del servicio
        /// </summary>
        private JsonApiClient _client;

        /// <summary>
        /// Guarda el endpoint del servicio.
        /// Se busca en un método desde una clase con los métodos para ese modelo en función de su
        /// modelo de estados.
        /// </summary>
        private MethodInfo _method;

        /// <summary>
        /// Tipo del modelo desde donde se buscaran los métodos.
        /// </summary>
        private Type _modelType;

        /// <summary>
        /// Indica si el usuario esta autenticado o no.
        /// </summary>
        private bool _isAuth = false;

        /// <summary>
        /// Permite avisar si el cliente se encuentra conectado o no.
        /// </summary>
        public event EventHandler<bool> ChangeStateClient;

        /// <summary>
        /// Variable al proyecto
        /// </summary>
        private Proyecto proyecto;

        #endregion Variables globales

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase, permite crear la instancia del cliente.
        /// </summary>
        public FormClient()
        {
            proyecto = Proyecto.getInstance();
            _client = new JsonApiClient(@"https://localhost:5101");
        }

        #endregion Constructor

        #region Métodos

        #region Autenticación

        /// <summary>
        /// Guarda la respuesta del servicio de autenticación.
        /// </summary>
        private AuthenticateResponse _authenticateResponse;

        /// <summary>
        /// Luego de la autenticación hay que definir la instancia y la UDN.
        /// Guarda la respuesta del servicio que recibe la instancia y la UDN.
        /// </summary>
        private WoInstanciaUdnResponse _instanceUdnResponse;

        /// <summary>
        /// En caso de que solo se requiera una auto autenticación con los valores por defecto
        /// se puede utilizar para autenticarse y asignar la UDN con los valores por defecto.
        /// </summary>
        public void AutoAuth()
        {
            Autenticate();
            SetInstanceUdn();
        }

        /// <summary>
        /// Autentica al usuario con los datos del usuario.
        /// ToDo: Borrar los datos por defecto que pasan por parámetro, solo son para pruebas.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void Autenticate(string user = "", string password = "")
        {
            try
            {
                _authenticateResponse = _client.Post<AuthenticateResponse>(
                    new Authenticate
                    {
                        provider = CredentialsAuthProvider.Name, //= credenciales
                        UserName =
                            (string.IsNullOrEmpty(user)) ? proyecto.ParConexion.Usuario : user,
                        Password =
                            (string.IsNullOrEmpty(password))
                                ? proyecto.ParConexion.Password
                                : password,
                        RememberMe = true,
                    }
                );

                //MessageBox.Show("Usuario autenticado correctamente.");

                _isAuth = true;

                if (ChangeStateClient != null)
                    ChangeStateClient(this, _isAuth);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(
                //    "Ocurrió un error al intentar autenticarse, revise que el servicio se encuentre activo"
                //);

                _isAuth = false;

                if (ChangeStateClient != null)
                    ChangeStateClient(this, _isAuth);
            }
        }

        /// <summary>
        /// Envía la instancia y la UDN del usuario.
        /// </summary>
        /// <param name="instancia"></param>
        /// <param name="udn"></param>
        public void SetInstanceUdn(
            string instance = "FRO941024IHA",
            string udn = "INDU",
            int year = 0
        )
        {
            try
            {
                _client.BearerToken = _authenticateResponse.BearerToken;
                _client.GetHttpClient().DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _authenticateResponse.BearerToken);
                _instanceUdnResponse = _client.Post(
                    new WoInstanciaUdnAsignar
                    {
                        Instance = (instance == "") ? proyecto.ParConexion.Instance : instance,
                        Udn = (udn == "") ? proyecto.ParConexion.Udn : udn,
                        Year = (year == 0) ? proyecto.ParConexion.Year : year,
                        InstanceType = proyecto.ParConexion.InstanceType
                    }
                );

                //MessageBox.Show("UDN asignada correctamente.");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(
                //    "Ocurrió un error al intentar asignar la UDN, revise que el servicio se encuentre activo"
                //);

                _isAuth = false;

                if (ChangeStateClient != null)
                    ChangeStateClient(this, _isAuth);
            }
        }

        #endregion Autenticación

        #region Configuración de la petición

        /// <summary>
        /// Asigna el tipo sobre el que se buscara el método del servicio.
        /// </summary>
        /// <param name="typeParam"></param>
        public void SetModelType(Type modelType)
        {
            _modelType = modelType;
        }

        /// <summary>
        /// Busca el método del servicio sobre el tipo que se asigno en "SetModelType".
        /// </summary>
        /// <param name="method"></param>
        public void SearchMethod(Type paramType, string method = "Get")
        {
            if (_modelType.IsNull())
            {
                MessageBox.Show(
                    @"No se a configurado un tipo sobre el que buscar un método, primero configure uno con el método ""SetModelType"""
                );
            }
            else
            {
                try
                {
                    _method = _modelType.GetMethod(
                        method,
                        new Type[] { _client.GetType(), paramType }
                    );

                    if (_method == null)
                    {
                        _method = _modelType.GetMethod(method);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Ocurrió un error al intentar buscar el método verifique el nombre o parámetro de este."
                    );
                }
            }
        }

        #endregion Configuración de la petición

        #region Ejecuta el método

        /// <summary>
        /// Detona el método que se busco, en caso de requerir pasar información
        /// se pasa en el parámetro.
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public dynamic ExecuteMethod(dynamic sendData)
        {
            dynamic response = null;

            if (_method != null)
            {
                try
                {
                    response = _method.Invoke(_method, new object[] { _client, sendData });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al ejecutar el método");
                }
            }
            return response;
        }

        #endregion Ejecuta el método

        #endregion Métodos
    }
}
