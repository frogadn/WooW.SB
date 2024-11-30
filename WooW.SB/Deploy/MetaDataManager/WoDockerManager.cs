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
    public class WoDockerManager
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

        /// <summary>
        /// Path del zip que se genera con la meta data del proyecto.
        /// </summary>
        private string _pathZipData;

        #endregion Atributos


        #region Método principal

        /// <summary>
        /// Método principal para la creación de un nuevo docker en la bd
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CreateDocker(WoDockerSettings woDockerSettings)
        {
            try
            {
                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                SendNewDocker(woDockerSettings);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error en la creación del docker: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Método principal


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


        #region Recuperación de los dockers

        /// <summary>
        /// Recupera la lista de los dockers ya deserializada para mostrar en la vista.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public List<WoDockerSettings> GetDockers()
        {
            try
            {
                List<WoDockerSettings> dockers = new List<WoDockerSettings>();

                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                string assemblyPath = $@"C:\Frog\WooW.Assemblies\WooW.WebClient.dll";
                if (!File.Exists(assemblyPath))
                {
                    throw new Exception(
                        $@"Error: No se encontró el dll del servicio: {assemblyPath}"
                    );
                }

                Assembly dll = Assembly.LoadFile(assemblyPath);

                Type type = dll.GetType("WooW.Model.DockerSettigs");
                object instanceType = Activator.CreateInstance(type);

                Type typeList = dll.GetType("WooW.DTO.DockerSettigsList");
                dynamic instanceList = Activator.CreateInstance(typeList);

                instanceList.select =
                    "Id, DockerName, ImageName, Almacenamiento, NetWorkName, Port, CPU, RAM, WoState";
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
                    foreach (dynamic docker in result.Value)
                    {
                        WoDockerSettings dockerSettings = new WoDockerSettings()
                        {
                            DockerName = docker.DockerName,
                            ImageName = docker.ImageName,
                            NetworkName = docker.NetWorkName,
                            Size = docker.Almacenamiento,
                            Port = docker.Port,
                            CPU = docker.CPU,
                            RAM = docker.RAM,
                        };

                        dockers.Add(dockerSettings);
                    }
                }

                return dockers;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error al recuperar la lista de dockers: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Recuperación de los dockers


        #region Send data for edit docker

        private dynamic EditDocker(WoDockerSettings woDockerSettings)
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

                Type type = dll.GetType("WooW.Model.DockerManager");
                object instanceType = Activator.CreateInstance(type);

                Type dockerManagerEditDockerType = dll.GetType("WooW.DTO.DockerManagerModificar");
                dynamic dockerManagerEditDocker = Activator.CreateInstance(
                    dockerManagerEditDockerType
                );

                Type[] types = { typeof(JsonApiClient), dockerManagerEditDocker };
                MethodInfo metodo = type.GetMethod("Post", types: types);

                // Invoca el método
                object result = metodo.Invoke(
                    null,
                    new object[] { _client, dockerManagerEditDocker }
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error al enviar los datos con la nueva información del docker: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Send data for edit docker


        #region Send data for new docker

        /// <summary>
        /// Se ocupa de enviar la information al servidor para la creación de un nuevo docker.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private dynamic SendNewDocker(WoDockerSettings woDockerSettings)
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

                Type type = dll.GetType("WooW.Model.DockerManager");
                object instanceType = Activator.CreateInstance(type);

                Type enumAction = dll.GetType("WooW.Model.eDockerManager_Accion");
                Array actionValues = Enum.GetValues(enumAction);
                dynamic actionSelected = actionValues.GetValue(0);

                Type enumTypeType = dll.GetType("WooW.Model.eDockerManager_Tipo");
                Array typeValues = Enum.GetValues(enumTypeType);
                dynamic typeSelected = typeValues.GetValue(0);

                Type enumorigen = dll.GetType("WooW.Model.eDockerManager_Origen");
                Array typeOrigen = Enum.GetValues(enumorigen);
                dynamic origenSelected = typeOrigen.GetValue(0);

                Type enumWoState = dll.GetType("WooW.Model.eDockerManager_WoState");
                Array woStateValues = Enum.GetValues(enumWoState);
                dynamic woStateSelected = woStateValues.GetValue(1);

                Type dockerManagerCrearDockerType = dll.GetType(
                    "WooW.DTO.DockerManagerCrearDocker"
                );
                dynamic dockerManagerCrearDocker = Activator.CreateInstance(
                    dockerManagerCrearDockerType
                );

                dockerManagerCrearDocker.Id = "";
                dockerManagerCrearDocker.SysUdnId = "INDU";
                dockerManagerCrearDocker.Serie = "1";
                dockerManagerCrearDocker.Folio = 0;
                dockerManagerCrearDocker.Accion = actionSelected;
                dockerManagerCrearDocker.Tipo = typeSelected;
                dockerManagerCrearDocker.Fecha = DateTime.Now;
                dockerManagerCrearDocker.Origen = origenSelected;
                dockerManagerCrearDocker.Observacion = null;
                dockerManagerCrearDocker.ConPeriodoId = "02";
                dockerManagerCrearDocker.DockerName = woDockerSettings.DockerName;
                dockerManagerCrearDocker.ImageName = woDockerSettings.ImageName;
                dockerManagerCrearDocker.NetWorkName = woDockerSettings.NetworkName;
                dockerManagerCrearDocker.Almacenamiento = woDockerSettings.Size;
                dockerManagerCrearDocker.Port = woDockerSettings.Port;
                dockerManagerCrearDocker.CPU = woDockerSettings.CPU;
                dockerManagerCrearDocker.RAM = woDockerSettings.RAM;
                dockerManagerCrearDocker.WoState = woStateSelected;

                Type[] types = { typeof(JsonApiClient), dockerManagerCrearDockerType };
                MethodInfo metodo = type.GetMethod("Post", types: types);

                // Invoca el método
                object result = metodo.Invoke(
                    null,
                    new object[] { _client, dockerManagerCrearDocker }
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $@"Ocurrió un error al enviar los datos del nuevo docker: {ex.Message}",
                    innerException: ex
                );
            }
        }

        #endregion Send data for new docker
    }
}
