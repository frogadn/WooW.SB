using System;
using System.Reflection;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Text;

namespace WooW.SB.Reportes.Helpers
{
    public static class WoGetListOData
    {
        #region Metodo principal

        /// <summary>
        /// Atributo con los valores necesarios para las propiedades del dto.
        /// </summary>
        //private (string select, string filter, string orderBy)? _dtoSettings;

        public static async Task<object> GetDataList(
            Type baseType,
            Type listType,
            string select,
            string filter,
            string orderby,
            bool count,
            int? top,
            int skip,
            JsonApiClient _client
        )
        {
            JsConfig.AllowRuntimeType = _ => true;

            dynamic? modelInstance = Activator.CreateInstance(baseType);

            if (modelInstance != null)
            {
                dynamic? dtoInstance = Activator.CreateInstance(listType);

                if (dtoInstance != null)
                {
                    if (_client != null)
                    {
                        dtoInstance.select = select;
                        dtoInstance.filter = filter;
                        dtoInstance.orderby = orderby;
                        dtoInstance.count = count;
                        dtoInstance.top = top;
                        dtoInstance.skip = null;

                        MethodInfo? listMethod = baseType.GetMethod("ListAsync");

                        if (listMethod != null)
                        {
                            //Type rType = typeof(WoODataResponse<>).MakeGenericType(baseType);
                            dynamic result = await listMethod.Invoke(
                                modelInstance,
                                new object[] { _client, dtoInstance }
                            );

                            return result.Value;
                        }
                        else
                        {
                            throw new Exception(
                                $@"No se encontró el método ListAsync en el modelo {baseType.Name}"
                            );
                        }
                    }
                    else
                    {
                        throw new Exception($@"No se encontró el cliente JsonApiClient");
                    }
                }
                else
                {
                    throw new Exception($@"No se realizo la instancia del dto correctamente");
                }
            }
            else
            {
                throw new Exception($@"No se pudo realizar la instancia del modelo");
            }
        }
        #endregion Metodo principal
    }
}
