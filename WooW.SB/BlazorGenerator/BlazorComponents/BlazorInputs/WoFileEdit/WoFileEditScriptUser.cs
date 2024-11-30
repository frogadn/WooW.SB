using System.Text;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoFileEdit
{
    public class WoFileEditScriptUser
    {
        #region Atributos

        /// <summary>
        /// Contiene la definición del componente en codigo, contiene
        /// todo el codigo necesario para construir un TextEdit
        /// </summary>
        private StringBuilder _filEditStr = new StringBuilder();

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos

        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="modelName"></param>
        public WoFileEditScriptUser(string _identMethodsAndProperties)
        {
            this._identMethodsAndProperties = _identMethodsAndProperties;
        }

        #endregion Constructor de la clase

        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar la generación del codigo del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(WoItem item)
        {
            _item = item;

            _filEditStr.Clear();

            _filEditStr.AppendLine($@"{_identMethodsAndProperties}#region {item.BindedProperty}");

            _filEditStr.AppendLine(BuildTag());

            _filEditStr.AppendLine(BuildOnChangeUserCode());

            _filEditStr.AppendLine(
                $@"{_identMethodsAndProperties}#endregion {item.BindedProperty}"
            );

            return _filEditStr.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoFileEditScriptUser.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 02-04-2024";
        }

        #endregion Tag

        #region Construcción del código

        /// <summary>
        /// Instancia del componente
        /// </summary>
        /// <returns></returns>
        private string BuildOnChangeUserCode()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Evento que se cacha cuando existe un cambio.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public void {_item.BindedProperty}_OnChange()
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}   //Coloque su código aquí.
{_identMethodsAndProperties}}}";
        }

        #endregion Construcción del código
    }
}
