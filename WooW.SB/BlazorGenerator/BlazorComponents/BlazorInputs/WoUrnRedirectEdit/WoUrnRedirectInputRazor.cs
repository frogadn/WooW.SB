using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoUrnRedirectEdit
{
    public class WoUrnRedirectInputRazor
    {
        #region Instancias singleton

        private WoLogObserver _observer = new WoLogObserver();

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Contiene la definición del componente en codigo, contiene
        /// todo el codigo necesario para construir un urn redirect input
        /// </summary>
        private StringBuilder _strUrnRedirectInput = new StringBuilder();

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Urn";

        /// <summary>
        /// Prefijo del componente en minúsculas
        /// </summary>
        private string _lowShortComponent = "urn";

        #endregion Atributos

        #region Método principal

        /// <summary>
        /// Construye el codigo razor del componente
        /// </summary>
        /// <param name="identItemProperty"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(string identItemProperty, WoItem item)
        {
            this._identMethodsAndProperties = identItemProperty;
            _item = item;

            _strUrnRedirectInput.Clear();

            _strUrnRedirectInput.AppendLine(BuildTag());
            _strUrnRedirectInput.AppendLine(BuildItem());
            _strUrnRedirectInput.AppendLine(BuildComponent());
            _strUrnRedirectInput.AppendLine(BuildAlert());
            _strUrnRedirectInput.AppendLine(BuildEndComponentes());

            return _strUrnRedirectInput.ToString();
        }

        #endregion Método principal

        #region Tag

        /// <summary>
        /// Construye el tag con la información del componente que se genera
        /// </summary>
        /// <returns></returns>
        private string BuildTag()
        {
            StringBuilder strTag = new StringBuilder();

            strTag.Append($@"{_identMethodsAndProperties}@* ");

            strTag.Append("Este código fue generado por el fichero WoUrnRedirectInputRazor.cs ");
            strTag.Append("en el Path: ");
            strTag.Append(
                "WooW.SB\\BlazorGenerator\\BlazorComponents\\BlazorInputs\\WoTextEdit\\WoTextEditRazor.cs"
            );

            strTag.Append("*@");

            return strTag.ToString();
        }

        #endregion Tag

        #region Construcción del código

        /// <summary>
        /// Contrucción de WoFormItem
        /// </summary>
        /// <returns></returns>
        private string BuildItem()
        {
            string etiqueta = string.Empty;
            if (_item.Etiqueta != null)
            {
                etiqueta = _item.Etiqueta;
            }
            else
            {
                ModeloColumna column = _project
                    .ModeloCol.Modelos.Where(x => x.Id == _item.BaseModelName)
                    .FirstOrDefault()
                    .Columnas.Where(x => x.Id == _item.BindedProperty)
                    .FirstOrDefault();
                etiqueta = Etiqueta.ToId(column.Formulario);
            }

            return $@"
{_identMethodsAndProperties}@*{_item.Id}*@
{_identMethodsAndProperties}<WoFormItem ColSpan=""{_item.ColSpan}""
{_identMethodsAndProperties}            BeginRow=""{_item.BeginRow.ToString().ToLower()}""
{_identMethodsAndProperties}            Caption=""@Localizer[""{etiqueta}""]""
{_identMethodsAndProperties}            SetStatus=""@SetStatusFli{_item.Id}""
{_identMethodsAndProperties}            @ref=""@_fli{_item.Id}"">
{_identMethodsAndProperties}    <TemplateFragment>";
        }

        /// <summary>
        /// Construye el codigo para el input
        /// </summary>
        /// <returns></returns>
        private string BuildComponent()
        {
            return "";
        }

        /// <summary>
        /// Contrucción de la alerta
        /// </summary>
        private string BuildAlert()
        {
            return $@"
{_identMethodsAndProperties}        <WoInputAlert SetStatus=""@SetStatusWia{_item.Id}"" @ref=""@_wia{_item.Id}"" />";
        }

        /// <summary>
        /// Construcción de los cierres de etiquetas
        /// </summary>
        private string BuildEndComponentes()
        {
            return $@"
{_identMethodsAndProperties}   </TemplateFragment>
{_identMethodsAndProperties}</WoFormItem>";
        }

        #endregion Construcción del código
    }
}
