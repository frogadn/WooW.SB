using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Versioning;
using ServiceStack;
using ServiceStack.Auth;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Deploy.Models;

namespace WooW.SB.Deploy.MetaDataManager
{
    public class WoNetworkManager
    {
        #region Instancias singleton

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Ruta base de la api a la que se cargara la meta data y el fichero.
        /// </summary>
        private string _basePathApi = "https://192.168.1.36";

        /// <summary>
        /// Cliente del servicio
        /// </summary>
        private JsonApiClient _client;

        #endregion Atributos


        #region Servicio

        #region Recuperación de un cliente autenticado

        /// <summary>
        /// Método principal para recuperar un cliente autenticado.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public JsonApiClient GetAuthClient()
        {
            try
            {
                _client = new JsonApiClient(_basePathApi);

                //Autenticate(user: "employee@email.com", password: "p");
                Autenticate();
                SetInstanceUdn();

                return _client;
            }
            catch (Exception ex)
            {
                throw new Exception(message: ex.Message, innerException: ex);
            }
        }

        #endregion Recuperación de un cliente autenticado

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
        /// Indicador de autenticación
        /// </summary>
        private bool _isAuth = false;

        /// <summary>
        /// Autentica al usuario en el servidor.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        [SupportedOSPlatform("windows")]
        private void Autenticate(string user = "admin@email.com", string password = "mypass")
        {
            try
            {
                if (!_isAuth)
                {
                    _authenticateResponse = _client.Post<AuthenticateResponse>(
                        new Authenticate
                        {
                            provider = CredentialsAuthProvider.Name, //= credenciales
                            UserName =
                                (string.IsNullOrEmpty(user)) ? _project.ParConexion.Usuario : user,
                            Password =
                                (string.IsNullOrEmpty(password))
                                    ? _project.ParConexion.Password
                                    : password,
                            RememberMe = true,
                        }
                    );

                    _isAuth = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error al autenticar al usuario: {ex.Message}",
                    innerException: ex
                );
            }
        }

        /// <summary>
        /// Envía la instancia y la UDN del usuario.
        /// </summary>
        /// <param name="instancia"></param>
        /// <param name="udn"></param>
        [SupportedOSPlatform("windows")]
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
                        Instance = (instance == "") ? _project.ParConexion.Instance : instance,
                        Udn = (udn == "") ? _project.ParConexion.Udn : udn,
                        Year = (year == 0) ? _project.ParConexion.Year : year,
                        InstanceType = _project.ParConexion.InstanceType
                    }
                );
            }
            catch (Exception ex)
            {
                _isAuth = false;

                throw new Exception(
                    message: $@"Ocurrió un error al autenticar al usuario: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Autenticación

        #endregion Servicio


        #region Creación de redes

        #region Método principal

        /// <summary>
        /// Método principal para la creación de una nueva red
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CreateNetwork(WoNetworkSettings woNetworkSettings)
        {
            try
            {
                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                SendNewNetwork(woNetworkSettings);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error en la creación de la red: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Método principal

        #region Creación de nuevas redes

        /// <summary>
        /// Se ocupa de enviar la información de la red al servicio
        /// </summary>
        /// <param name="woNetworkSettings"></param>
        private void SendNewNetwork(WoNetworkSettings woNetworkSettings)
        {
            try
            {
                string assemblyPath = $@"C:\Frog\WooW.Assemblies\WooW.WebClient.dll";
                if (!File.Exists(assemblyPath))
                {
                    throw new Exception(
                        $@"Error: No se encontró el dll del servicio: {assemblyPath}"
                    );
                }

                Assembly dll = Assembly.LoadFile(assemblyPath);

                Type type = dll.GetType("WooW.Model.NetworkManager");
                object instanceType = Activator.CreateInstance(type);

                Type enumAction = dll.GetType("WooW.Model.eNetworkManager_Accion");
                Array actionValues = Enum.GetValues(enumAction);
                dynamic actionSelected = actionValues.GetValue(0);

                Type enumTypeType = dll.GetType("WooW.Model.eNetworkManager_Tipo");
                Array typeValues = Enum.GetValues(enumTypeType);
                dynamic typeSelected = typeValues.GetValue(0);

                Type enumorigen = dll.GetType("WooW.Model.eNetworkManager_Origen");
                Array typeOrigen = Enum.GetValues(enumorigen);
                dynamic origenSelected = typeOrigen.GetValue(0);

                Type enumWoState = dll.GetType("WooW.Model.eNetworkManager_WoState");
                Array woStateValues = Enum.GetValues(enumWoState);
                dynamic woStateSelected = woStateValues.GetValue(1);

                Type networkManagerCrearType = dll.GetType("WooW.DTO.NetworkManagerCrear");
                dynamic networkManagerCrear = Activator.CreateInstance(networkManagerCrearType);

                networkManagerCrear.Id = "";
                networkManagerCrear.SysUdnId = "INDU";
                networkManagerCrear.Serie = "1";
                networkManagerCrear.Folio = 0;
                networkManagerCrear.Accion = actionSelected;
                networkManagerCrear.Tipo = typeSelected;
                networkManagerCrear.Fecha = DateTime.Now;
                networkManagerCrear.Origen = origenSelected;
                networkManagerCrear.Observacion = "";
                networkManagerCrear.ConPeriodoId = "02";
                networkManagerCrear.NetworkName = woNetworkSettings.NetworkName;
                networkManagerCrear.Dockers = 0;
                networkManagerCrear.WoState = woStateSelected;
                networkManagerCrear.NetworkId = "";

                Type[] types = { typeof(JsonApiClient), networkManagerCrearType };
                MethodInfo metodo = type.GetMethod("Post", types: types);

                // Invoca el método
                object result = metodo.Invoke(null, new object[] { _client, networkManagerCrear });
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error al enviar los datos de la nueva red: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Creación de nuevas redes

        #endregion Creación de redes


        #region Edición de redes

        #region Método principal

        /// <summary>
        /// Método principal para la edición de una red
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void EditNetwork(string oldNetworkName, string newNetworkName)
        {
            try
            {
                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                EditNetworkSettings(oldNetworkName, newNetworkName);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error en la creación de la red: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Método principal

        #region Edición de la red

        /// <summary>
        /// Se ocupa de enviar la información de la red al servicio
        /// </summary>
        /// <param name="woNetworkSettings"></param>
        private void EditNetworkSettings(string oldNetworkName, string newNetworkName)
        {
            try
            {
                (string id, string networkName, int dockers, ulong rowVersion) network = GetNetwork(
                    oldNetworkName
                );

                string assemblyPath = $@"C:\Frog\WooW.Assemblies\WooW.WebClient.dll";
                if (!File.Exists(assemblyPath))
                {
                    throw new Exception(
                        $@"Error: No se encontró el dll del servicio: {assemblyPath}"
                    );
                }

                Assembly dll = Assembly.LoadFile(assemblyPath);

                Type type = dll.GetType("WooW.Model.NetworkManager");
                object instanceType = Activator.CreateInstance(type);

                Type enumWoState = dll.GetType("WooW.Model.eNetworkManager_WoState");
                Array woStateValues = Enum.GetValues(enumWoState);
                dynamic woStateSelected = woStateValues.GetValue(3);

                Type networkManagerSolicitarCambioType = dll.GetType(
                    "WooW.DTO.NetworkManagerSolicitarCambio"
                );
                dynamic networkManagerSolicitarCambio = Activator.CreateInstance(
                    networkManagerSolicitarCambioType
                );

                networkManagerSolicitarCambio.Id = network.id;
                networkManagerSolicitarCambio.Fecha = DateTime.Now;
                networkManagerSolicitarCambio.NetworkName = newNetworkName;
                networkManagerSolicitarCambio.WoState = woStateSelected;
                networkManagerSolicitarCambio.RowVersion = network.rowVersion;

                Type[] types = { typeof(JsonApiClient), networkManagerSolicitarCambioType };
                MethodInfo metodo = type.GetMethod("Patch", types: types);

                // Invoca el método
                object result = metodo.Invoke(
                    null,
                    new object[] { _client, networkManagerSolicitarCambio }
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error al enviar los datos de la red: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Edición de la red

        #endregion Edición de redes


        #region Eliminación de redes

        #region Método principal

        /// <summary>
        /// Método principal para la eliminación de la red
        /// </summary>
        /// <param name="networkName"></param>
        [SupportedOSPlatform("windows")]
        public void DeleteNetwork(string networkName)
        {
            try
            {
                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                SendDelete(networkName);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error en la eliminación de la red: {networkName}: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Método principal

        #region Eliminar redes

        /// <summary>
        /// Realizamos las peticiones para eliminar una red de docker
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private void SendDelete(string networkName)
        {
            try
            {
                (string id, string networkName, int dockers, ulong rowVersion) network = GetNetwork(
                    networkName
                );

                string assemblyPath = $@"C:\Frog\WooW.Assemblies\WooW.WebClient.dll";
                if (!File.Exists(assemblyPath))
                {
                    throw new Exception(
                        $@"Error: No se encontró el dll del servicio: {assemblyPath}"
                    );
                }

                Assembly dll = Assembly.LoadFile(assemblyPath);

                Type type = dll.GetType("WooW.Model.NetworkManager");
                object instanceType = Activator.CreateInstance(type);

                Type enumWoState = dll.GetType("WooW.Model.eNetworkManager_WoState");
                Array woStateValues = Enum.GetValues(enumWoState);
                dynamic woStateSelected = woStateValues.GetValue(4);

                Type networkManagerEliminarType = dll.GetType("WooW.DTO.NetworkManagerEliminar");
                dynamic networkManagerEliminar = Activator.CreateInstance(
                    networkManagerEliminarType
                );

                networkManagerEliminar.Id = network.id;
                networkManagerEliminar.Fecha = DateTime.Now;
                networkManagerEliminar.WoState = woStateSelected;
                networkManagerEliminar.NetworkName = network.networkName;
                networkManagerEliminar.RowVersion = network.rowVersion;

                Type[] types = { typeof(JsonApiClient), networkManagerEliminarType };
                MethodInfo metodo = type.GetMethod("Patch", types: types);

                // Invoca el método
                object result = metodo.Invoke(
                    null,
                    new object[] { _client, networkManagerEliminar }
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Error al intentar realizar la petition de eliminado: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Eliminar redes

        #endregion Eliminación de redes


        #region Recuperación de las redes

        /// <summary>new Json
        /// Recupera la lista de redes del usuario
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public List<WoNetworkSettings> GetNetworks()
        {
            try
            {
                List<WoNetworkSettings> networks = new List<WoNetworkSettings>();

                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                string assemblyPath = $@"C:\Frog\WooW.Assemblies\WooW.WebClient.dll";
                if (!File.Exists(assemblyPath))
                {
                    throw new Exception(
                        $@"Error: no se encontró el dll del servicio: {assemblyPath}"
                    );
                }

                Assembly dll = Assembly.LoadFile(assemblyPath);

                Type type = dll.GetType("WooW.Model.NetworkDockers");
                object instanceType = Activator.CreateInstance(type);

                Type typeList = dll.GetType("WooW.DTO.NetworkDockersList");
                dynamic instanceList = Activator.CreateInstance(typeList);

                instanceList.select = "Id, NetworkName, Dockers, NetworkId";
                instanceList.filter = "";
                instanceList.orderby = "";
                instanceList.count = false;
                instanceList.top = 100;
                instanceList.skip = 0;

                Type[] types = { typeof(JsonApiClient), typeList };
                MethodInfo metodo = type.GetMethod("List", types: types);

                // Invoca el método
                dynamic result = metodo.Invoke(null, new object[] { _client, instanceList });

                if (result != null && result.Value.Count > 0)
                {
                    foreach (dynamic network in result.Value)
                    {
                        WoNetworkSettings networkSettings = new WoNetworkSettings()
                        {
                            NetworkName = network.NetworkName,
                            Dockers = network.Dockers,
                            NetworkId = network.NetworkId,
                        };

                        networks.Add(networkSettings);
                    }
                }

                return networks;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error en la consulta de los dockers: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Recuperación de las redes

        #region Recuperación de una red en especifico

        /// <summary>new Json
        /// Recupera la red con el nombre que se recibe por parámetro
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public (string id, string networkName, int dockers, ulong rowVersion) GetNetwork(
            string networkName
        )
        {
            try
            {
                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                string assemblyPath = $@"C:\Frog\WooW.Assemblies\WooW.WebClient.dll";
                if (!File.Exists(assemblyPath))
                {
                    throw new Exception(
                        $@"Error: no se encontró el dll del servicio: {assemblyPath}"
                    );
                }

                Assembly dll = Assembly.LoadFile(assemblyPath);

                Type type = dll.GetType("WooW.Model.NetworkManager");
                object instanceType = Activator.CreateInstance(type);

                Type typeList = dll.GetType("WooW.DTO.NetworkManagerList");
                dynamic instanceList = Activator.CreateInstance(typeList);

                instanceList.select = "Id, NetworkName, Dockers, RowVersion";
                instanceList.filter = $@"NetworkName eq '{networkName}'";
                instanceList.orderby = "";
                instanceList.count = false;
                instanceList.top = 100;
                instanceList.skip = 0;

                Type[] types = { typeof(JsonApiClient), typeList };
                MethodInfo metodo = type.GetMethod("List", types: types);

                // Invoca el método
                dynamic result = metodo.Invoke(null, new object[] { _client, instanceList });

                if (result != null && result.Value.Count > 0)
                {
                    dynamic network = result.Value[0];

                    return (
                        network.Id.ToString(),
                        network.NetworkName,
                        network.Dockers,
                        network.RowVersion
                    );
                }
                else
                {
                    throw new Exception($@"La red {networkName} no fue encontrada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error en la consulta de los dockers: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Recuperación de una red en especifico
    }
}
