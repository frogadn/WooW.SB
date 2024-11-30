using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Versioning;
using ServiceStack;
using ServiceStack.Auth;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Deploy.Models;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Deploy.MetaDataManager
{
    public class WoDeployMetaData
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
        /// Despliega la meta data en el servidor.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void DeployMetaData(WoDeployGenerationSettings woDeployGenerationSettings)
        {
            try
            {
                _client = new JsonApiClient(_basePathApi);
                GetAuthClient();

                CopyForDeploy();

                _pathZipData = $@"{_project.Dir}\Deploy\Proyecto.zip";
                string directoryPath = $@"{_project.Dir}\Deploy\Proyecto";

                System.IO.Compression.ZipFile.CreateFromDirectory(directoryPath, _pathZipData);

                dynamic result = ExistDeploy(
                    metaDataId: woDeployGenerationSettings.MetadataName,
                    generationName: woDeployGenerationSettings.IntegralDataName,
                    dockerName: woDeployGenerationSettings.Docker
                );
                if (result != null)
                {
                    string woState = result.WoState.ToString();
                    if (woState == "DockerDesplegado" || woState == "Error" || woState == "EnLinea")
                    {
                        if (woState != "Error" && woState != "EnLinea")
                        {
                            result = UpdateStatus(lastReg: result);
                        }

                        if (result != null)
                        {
                            result = ExistDeploy(
                                metaDataId: woDeployGenerationSettings.MetadataName,
                                generationName: woDeployGenerationSettings.IntegralDataName,
                                dockerName: woDeployGenerationSettings.Docker
                            );
                            woState = result.WoState.ToString();

                            if (woState != "Error" && woState != "EnLinea")
                            {
                                throw new Exception(
                                    "Hay un error en los status con la maquina de estados"
                                );
                            }

                            result = UpdateData(woDeployGenerationSettings, result);
                        }
                    }
                }
                else
                {
                    result = SendData(woDeployGenerationSettings);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(message: ex.Message, innerException: ex);
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


        #region Preparación de la meta data

        /// <summary>
        /// Genera una carpeta donde copia los ficheros para el deploy de un modelo.
        /// </summary>
        private void CopyForDeploy()
        {
            WoDirectory.CreateDirectory($@"{_project.Dir}\Deploy");
            WoDirectory.CreateDirectory($@"{_project.Dir}\Deploy\Proyecto");
            WoDirectory.CopyFile(
                oldPath: $@"{_project.Dir}\Prueba.wwsb",
                newPath: $@"{_project.Dir}\Deploy\Proyecto\Prueba.wwsb"
            );
            WoDirectory.CopyDirectory(
                oldPath: $@"{_project.Dir}\ProyectData",
                newPath: $@"{_project.Dir}\Deploy\Proyecto\ProyectData"
            );
        }

        #endregion Preparación de la meta data


        #region Validación de las publicaciones actuales

        /// <summary>
        /// Valida la existencia del deploy y en el caso de que ya exista un deploy
        /// retorna un true para indicar que se debe de actualizar.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private dynamic ExistDeploy(string metaDataId, string generationName, string dockerName)
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

                Type type = dll.GetType("WooW.Model.MetaUpload");
                object instanceType = Activator.CreateInstance(type);

                Type listType = dll.GetType("WooW.DTO.MetaUploadList");
                dynamic instanceListType = Activator.CreateInstance(listType);

                instanceListType.select =
                    "Id, MetaDataId, WWSBName, GenerationName, GenerationType, TaskId, CargaValidada, WoState, RowVersion";
                instanceListType.filter =
                    $@"MetaDataId eq '{metaDataId}' and DockerName eq '{dockerName}'";
                instanceListType.orderby = "";
                instanceListType.top = 10;
                instanceListType.skip = 0;

                Type[] types = { typeof(JsonApiClient), listType };
                MethodInfo metodo = type.GetMethod("List", types: types);

                // Invoca el método
                dynamic result = metodo.Invoke(null, new object[] { _client, instanceListType });

                if (result != null)
                {
                    if (result.Value.Count > 0)
                    {
                        return result.Value[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error en la validación del deploy existente: {ex.Message}"
                );
            }
        }

        #endregion Validación de las publicaciones actuales


        #region Actualización de la meta data a error

        /// <summary>
        /// Actualización de la data a error para actualizar
        /// </summary>
        [SupportedOSPlatform("windows")]
        private dynamic UpdateStatus(dynamic lastReg)
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

                Type type = dll.GetType("WooW.Model.MetaUpload");
                object instanceType = Activator.CreateInstance(type);

                Type enumType = dll.GetType("WooW.Model.eMetaUpload_GenerationType");
                Array valuesEnumType = Enum.GetValues(enumType);
                dynamic selectedType = valuesEnumType.GetValue(0);

                Type enumAction = dll.GetType("WooW.Model.eMetaUpload_Accion");
                Array actionValues = Enum.GetValues(enumAction);
                dynamic actionSelected = actionValues.GetValue(0);

                Type enumTypeType = dll.GetType("WooW.Model.eMetaUpload_Tipo");
                Array typeValues = Enum.GetValues(enumTypeType);
                dynamic typeSelected = typeValues.GetValue(0);

                Type enumorigen = dll.GetType("WooW.Model.eMetaUpload_Origen");
                Array typeOrigen = Enum.GetValues(enumorigen);
                dynamic origenSelected = typeOrigen.GetValue(0);

                Type enumWoState = dll.GetType("WooW.Model.eMetaUpload_WoState");
                Array woStateValues = Enum.GetValues(enumWoState);
                dynamic woStateSelected = woStateValues.GetValue(8);

                string typeOfUpdate = "WooW.DTO.MetaUploadIndicarConfigurarIISFallido";

                Type updateType = dll.GetType(typeOfUpdate);
                dynamic instanceUpdateType = Activator.CreateInstance(updateType);
                instanceUpdateType.Id = lastReg.Id;
                instanceUpdateType.RowVersion = lastReg.RowVersion;
                instanceUpdateType.WoState = woStateSelected;

                Type[] types = { typeof(JsonApiClient), updateType };
                MethodInfo metodo = type.GetMethod("Patch", types: types);

                // Invoca el método
                dynamic result = metodo.Invoke(null, new object[] { _client, instanceUpdateType });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error en la actualización del deploy existente: {ex.Message}"
                );
            }
        }

        #endregion Actualización de la meta data a error


        #region Envió de la data al servidor

        /// <summary>
        /// Envió de la meta data al servidor.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private dynamic SendData(WoDeployGenerationSettings woDeployGenerationSettings)
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

                Type type = dll.GetType("WooW.Model.MetaUpload");
                object instanceType = Activator.CreateInstance(type);

                Type enumType = dll.GetType("WooW.Model.eMetaUpload_GenerationType");
                Array valuesEnumType = Enum.GetValues(enumType);
                dynamic selectedType = null;

                if (woDeployGenerationSettings.ProyectType == eProyectType.Server)
                {
                    selectedType = valuesEnumType.GetValue(0);
                }
                else if (woDeployGenerationSettings.ProyectType == eProyectType.Wasm)
                {
                    selectedType = valuesEnumType.GetValue(1);
                }

                Type enumAction = dll.GetType("WooW.Model.eMetaUpload_Accion");
                Array actionValues = Enum.GetValues(enumAction);
                dynamic actionSelected = actionValues.GetValue(0);

                Type enumTypeType = dll.GetType("WooW.Model.eMetaUpload_Tipo");
                Array typeValues = Enum.GetValues(enumTypeType);
                dynamic typeSelected = typeValues.GetValue(0);

                Type enumorigen = dll.GetType("WooW.Model.eMetaUpload_Origen");
                Array typeOrigen = Enum.GetValues(enumorigen);
                dynamic origenSelected = typeOrigen.GetValue(0);

                Type enumWoState = dll.GetType("WooW.Model.eMetaUpload_WoState");
                Array woStateValues = Enum.GetValues(enumWoState);
                dynamic woStateSelected = woStateValues.GetValue(1);

                Type chargeType = dll.GetType("WooW.DTO.MetaUploadCargar");
                dynamic instanceChargeType = Activator.CreateInstance(chargeType);
                instanceChargeType.Id = "";
                instanceChargeType.SysUdnId = "INDU";
                instanceChargeType.Serie = "1";
                instanceChargeType.Folio = 1;
                instanceChargeType.Accion = actionSelected;
                instanceChargeType.Tipo = typeSelected;
                instanceChargeType.Fecha = DateTime.Today;
                instanceChargeType.Origen = origenSelected;
                instanceChargeType.Observacion = null;
                instanceChargeType.ConPeriodoId = "02";
                instanceChargeType.TaskId = "";
                instanceChargeType.DockerName = woDeployGenerationSettings.Docker;
                instanceChargeType.MetaDataId = woDeployGenerationSettings.MetadataName;
                instanceChargeType.WWSBName = woDeployGenerationSettings.MetadataFileName;
                instanceChargeType.GenerationName = woDeployGenerationSettings.IntegralDataName;
                instanceChargeType.GenerationType = selectedType;
                instanceChargeType._File = File.ReadAllBytes(_pathZipData);
                instanceChargeType.CargaValidada = false;
                instanceChargeType.WoState = woStateSelected;

                Type[] types = { typeof(JsonApiClient), chargeType };
                MethodInfo metodo = type.GetMethod("Post", types: types);

                // Invoca el método
                object result = metodo.Invoke(null, new object[] { _client, instanceChargeType });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar cargar el fichero: {ex.Message}"
                );
            }
        }

        #endregion Envió de la data al servidor


        #region Actualization de la meta data del servidor

        /// <summary>
        /// Actualiza la meta data del servidor.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private dynamic UpdateData(
            WoDeployGenerationSettings woDeployGenerationSettings,
            dynamic lastVersion
        )
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

                Type type = dll.GetType("WooW.Model.MetaUpload");
                object instanceType = Activator.CreateInstance(type);

                Type enumType = dll.GetType("WooW.Model.eMetaUpload_GenerationType");
                Array valuesEnumType = Enum.GetValues(enumType);
                dynamic selectedType = null;

                if (woDeployGenerationSettings.ProyectType == eProyectType.Server)
                {
                    selectedType = valuesEnumType.GetValue(0);
                }
                else if (woDeployGenerationSettings.ProyectType == eProyectType.Wasm)
                {
                    selectedType = valuesEnumType.GetValue(1);
                }

                Type enumAction = dll.GetType("WooW.Model.eMetaUpload_Accion");
                Array actionValues = Enum.GetValues(enumAction);
                dynamic actionSelected = actionValues.GetValue(0);

                Type enumTypeType = dll.GetType("WooW.Model.eMetaUpload_Tipo");
                Array typeValues = Enum.GetValues(enumTypeType);
                dynamic typeSelected = typeValues.GetValue(0);

                Type enumorigen = dll.GetType("WooW.Model.eMetaUpload_Origen");
                Array typeOrigen = Enum.GetValues(enumorigen);
                dynamic origenSelected = typeOrigen.GetValue(0);

                Type enumWoState = dll.GetType("WooW.Model.eMetaUpload_WoState");
                Array woStateValues = Enum.GetValues(enumWoState);
                dynamic woStateSelected = woStateValues.GetValue(1);

                string updateType =
                    (lastVersion.WoState.ToString() == "EnLinea")
                        ? "WooW.DTO.MetaUploadCargarNuevaVersion"
                        : "WooW.DTO.MetaUploadActualizarMetaData";

                Type chargeType = dll.GetType(updateType);
                dynamic instanceChargeType = Activator.CreateInstance(chargeType);
                instanceChargeType.Id = lastVersion.Id;
                instanceChargeType.TaskId = "";
                instanceChargeType.CargaValidada = false;
                if (lastVersion.WoState.ToString() != "EnLinea")
                {
                    instanceChargeType.DockerName = woDeployGenerationSettings.Docker;
                }
                instanceChargeType.MetaDataId = woDeployGenerationSettings.MetadataName;
                instanceChargeType.WWSBName = woDeployGenerationSettings.MetadataFileName;
                instanceChargeType.GenerationName = woDeployGenerationSettings.IntegralDataName;
                instanceChargeType.GenerationType = selectedType;
                instanceChargeType.Fecha = DateTime.Now;
                instanceChargeType._File = File.ReadAllBytes(_pathZipData);
                instanceChargeType.WoState = woStateSelected;
                instanceChargeType.RowVersion = lastVersion.RowVersion;

                Type[] types = { typeof(JsonApiClient), chargeType };
                MethodInfo metodo = type.GetMethod("Patch", types: types);

                // Invoca el método
                object result = metodo.Invoke(null, new object[] { _client, instanceChargeType });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al intentar cargar el fichero: {ex.Message}"
                );
            }
        }

        #endregion Actualization de la meta data del servidor
    }
}
