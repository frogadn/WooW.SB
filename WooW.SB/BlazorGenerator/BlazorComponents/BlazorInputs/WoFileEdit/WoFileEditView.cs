using System.Text;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoFileEdit
{
    public class WoFileEditView
    {
        #region Atributos

        /// <summary>
        /// Contiene la definición del componente en codigo, contiene
        /// todo el codigo necesario para construir un file edit
        /// </summary>
        private StringBuilder _strFileEdit = new StringBuilder();

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
        public WoFileEditView(string _identMethodsAndProperties)
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

            _strFileEdit.Clear();

            _strFileEdit.AppendLine($@"{_identMethodsAndProperties}#region {item.BindedProperty}");

            _strFileEdit.AppendLine(BuildTag());

            _strFileEdit.AppendLine(BuildComponent());

            _strFileEdit.AppendLine(BuildContainerComponent());

            _strFileEdit.AppendLine(BuildAlertComponent());

            _strFileEdit.AppendLine(
                $@"{_identMethodsAndProperties}#endregion {item.BindedProperty}"
            );

            return _strFileEdit.ToString();
        }
        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoFileEditView.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 02-04-2024";
        }

        #endregion Tag

        #region Construcción del código

        /// <summary>
        /// Instancia del componente
        /// </summary>
        /// <returns></returns>
        private string BuildComponent()
        {
            return $@"";
        }

        /// <summary>
        /// Contenedor del componente
        /// </summary>
        /// <returns></returns>
        private string BuildContainerComponent()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del input de texto Usuario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoFormItemView {_item.BindedProperty}Container {{ get; set; }} = new WoFormItemView();";
        }

        /// <summary>
        /// Alerta del componente
        /// </summary>
        /// <returns></returns>
        private string BuildAlertComponent()
        {
            return $@"
{_identMethodsAndProperties}/// <summary>
{_identMethodsAndProperties}/// Instancia de la vista del input de texto Usuario.
{_identMethodsAndProperties}/// </summary>
{_identMethodsAndProperties}public WoInputAlertView {_item.BindedProperty}Alert {{ get; set; }} = new WoInputAlertView();";
        }

        #endregion Construcción del código
    }
}
