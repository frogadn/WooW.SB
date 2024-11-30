using System;

namespace WooW.SB.Helpers.DesignerHelpers
{
    public class ControlItemHelper
    {
        /// <summary>
        /// Permite convertir el dato que se obtiene de un control en
        /// windows forms y convierte al tipo de la propiedad del modelo relacionado.
        ///
        /// Obtienen el tipo de value.
        /// Valida que no sea null.
        /// Si el dato de entrada es nulo, retorna el default del tipo.
        /// Retorna el resultado del parseo.
        /// Si la conversion falla retorna el default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="dataRaw"></param>
        /// <returns></returns>
        public T CastStringToGeneric<T>(T value, string dataRaw)
        {
            Type type = value.GetType();

            var ReturnType = Nullable.GetUnderlyingType(type) ?? type;
            try
            {
                if (ReturnType != type && dataRaw == null)
                    return default(T);

                return (T)Convert.ChangeType(dataRaw, ReturnType);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
