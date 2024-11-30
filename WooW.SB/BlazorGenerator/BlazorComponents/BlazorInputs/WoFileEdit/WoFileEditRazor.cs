using System.Linq;
using System.Text;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoFileEdit
{
    public class WoFileEditRazor
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia singleton con la data del proyecto
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Contiene el item que se recibe como parámetro en el método principal,
        /// contiene toda la información requerida para la generación del código
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Contiene la definición del componente en código, contiene
        /// todo el código necesario para construir un file edit
        /// </summary>
        private StringBuilder _strFileEdit = new StringBuilder();

        /// <summary>
        /// Contiene strings para poder identar el código
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Fil";

        /// <summary>
        /// Prefijo del componente en minúsculas
        /// </summary>
        private string _lowShortComponent = "fil";

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

            _strFileEdit.Clear();

            _strFileEdit.AppendLine(BuildTag());
            _strFileEdit.AppendLine(BuildItem());
            _strFileEdit.AppendLine(BuildComponent());
            _strFileEdit.AppendLine(BuildAlert());
            _strFileEdit.AppendLine(BuildEndComponentes());

            return _strFileEdit.ToString();
        }

        #endregion Método principal


        #region Tag

        /// <summary>
        /// Tag con la data de la generation a realizar
        /// </summary>
        /// <returns></returns>
        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoFileEditRazor.cs *@
{_identMethodsAndProperties}@*WoWSB por el generador a día 02-04-2024*@";
        }

        #endregion Tag

        #region Contrucción del codigo

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
        /// Construction del componente
        /// </summary>
        private string BuildComponent()
        {
            string length = string.Empty;

            string isRequired = "false";

            if (!_item.NoModelComponent)
            {
                ModeloColumna column = _project
                    .ModeloCol.Modelos.Where(x => x.Id == _item.BaseModelName)
                    .FirstOrDefault()
                    .Columnas.Where(x => x.Id == _item.BindedProperty)
                    .FirstOrDefault();

                if (column.Longitud > 0)
                {
                    length =
                        $"\n{_identMethodsAndProperties}                    Length=\"{column.Longitud}\"";

                    isRequired = (!column.Nulo).ToString().ToLower();
                }
            }

            StringBuilder strComponent = new StringBuilder();
            string modelName = string.Empty;

            if (!_item.NoModelComponent)
            {
                modelName = $@"{_item.BaseModelName}.";
            }

            strComponent.AppendLine(
                $@"
{_identMethodsAndProperties}        <WoFileInput Value=""@_scriptsUser.{modelName}{_item.BindedProperty}"" 
{_identMethodsAndProperties}                     ValueChangedEvc=""@{_item.Id}_OnChange"" />"
            );

            return strComponent.ToString();
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
        #endregion Construction del código
    }
}
