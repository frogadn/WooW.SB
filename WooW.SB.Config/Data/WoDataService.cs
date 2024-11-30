using ServiceStack;
using System;
using System.Net.Http.Headers;
using WooW.Core;

namespace WooW.SB.Config.Data
{
    public class WoDataService
    {

        #region Atributos

        /// <summary>
        /// Dirección del servicio local sobre del que se esta probando
        /// </summary>
        private string _baseUrlService = "https://localhost:5101";

        /// <summary>
        /// Instancia del cliente del servicio
        /// </summary>
        private JsonApiClient? _client;

        #endregion Atributos


        #region Método principal

        /// <summary>
        /// Autenticamos y retornamos un cliente autenticado y con la udn asignada
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="instnce"></param>
        /// <param name="udn"></param>
        /// <param name="year"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public JsonApiClient GetAuthClient(
            string userName,
            string password,
            string instance,
            string udn,
            int year,
            tWoIntanciaType instanceType
        )
        {
            try
            {
                if (_client != null)
                {
                    _client = null;
                }

                _client = new JsonApiClient(_baseUrlService);

                var AuthenticateResponse = _client.Post<AuthenticateResponse>(
                    new Authenticate
                    {
                        provider = "credentials", // CredentialsAuthProvider.Name, //= credentials
                        UserName = userName,
                        Password = password,
                        RememberMe = true,
                    }
                );

                _client.BearerToken = AuthenticateResponse.BearerToken;
                _client.GetHttpClient().DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AuthenticateResponse.BearerToken);

                var WoInstanciaUdnResponse = _client.Post(
                    new WoInstanciaUdnAsignar
                    {
                        Instance = instance,
                        Udn = udn,
                        Year = year,
                        InstanceType = instanceType
                    }
                );

                return _client;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al autenticar el cliente. {ex.Message}");
            }
        }

        #endregion Método principal
    }
}
