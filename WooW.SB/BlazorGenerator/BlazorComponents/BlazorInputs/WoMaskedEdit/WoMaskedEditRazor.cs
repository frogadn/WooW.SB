using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMaskedEdit
{
    public class WoMaskedEditRazor
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
        /// todo el codigo necesario para construir un textedit
        /// </summary>
        private StringBuilder _strTextEdit = new StringBuilder();

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Mkd";

        /// <summary>
        /// Prefijo del componente en minusculas
        /// </summary>
        private string _lowShortComponent = "mkd";

        #endregion Atributos

        #region Método principal

        /// <summary>
        /// Método prinicpla ocupado de orquestar la generación del codigo del componente.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(string identItemProperty, WoItem item)
        {
            this._identMethodsAndProperties = identItemProperty;

            _item = item;

            _strTextEdit.Clear();

            _strTextEdit.AppendLine(BuildTag());
            _strTextEdit.AppendLine(BuildItem());
            _strTextEdit.AppendLine(BuildComponent());
            _strTextEdit.AppendLine(BuildAlert());
            _strTextEdit.AppendLine(BuildEndComponentes());
            _razorReady.Details =
                $@"Se a generado con éxito el código razor para el componente: {item.Id}";
            _observer.SetLog(_razorReady);

            return _strTextEdit.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoMaskedEditRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoMaskedEdit\WoMaskedEditRazor.cs.*@
{_identMethodsAndProperties}@*WoWSB por el generador a día 5-10-2023*@";
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
        /// Contrucción del componente
        /// </summary>
        private string BuildComponent()
        {
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

            ModeloColumna column = woProjectDataHelper.GetColumn(
                modelName: _item.BaseModelName,
                property: _item.BindedProperty
            );

            StringBuilder strMasked = new StringBuilder();

            strMasked.Append($@"{_identMethodsAndProperties}        ");
            strMasked.Append($"<WoMaskedEdit Id=\"{_lowShortComponent}{_item.Id}\"\n");
            strMasked.Append($"                       TApp=\"App\"\n");

            if (_item.Control == "Custom")
            {
                strMasked.Append(
                    $"              Value=\"@JsonConvert.SerializeObject(_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty})\"\n"
                );
                strMasked.Append($"                       TValue=\"string\"\n");
            }
            else
            {
                strMasked.Append(
                    $"              Value=\"@_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}\"\n"
                );
                strMasked.Append($"                       TValue=\"{_item.BindingType}\"\n");
            }

            if (_item.CustomMask != string.Empty || _item.InputString != eInputString.None)
            {
                strMasked.Append($"                       Mask=\"{_item.CustomMask}\"\n");
            }
            else
            {
                if (_item.InputNumeric != eInputNumeric.None)
                {
                    strMasked.Append(
                        $"                       Mask=\"@NumericMask.{_item.InputNumeric.ToString()}\"\n"
                    );
                }
                else if (
                    _item.InputDate != eInputDate.None
                    && (
                        _item.InputDate == eInputDate.GeneralLongFormat
                        || _item.InputDate == eInputDate.GeneralShortFormat
                        || _item.InputDate == eInputDate.ConstantFormat
                    )
                )
                {
                    strMasked.Append(
                        $"                       Mask=\"@TimeSpanMask.{_item.InputDate.ToString()}\"\n"
                    );
                }
                else if (_item.InputDate != eInputDate.None)
                {
                    strMasked.Append(
                        $"                       Mask=\"@DateTimeMask.{_item.InputDate.ToString()}\"\n"
                    );
                }
            }

            strMasked.Append(
                $"                       IsRequired=@{(!_item.Nullable).ToString().ToLower()}\n"
            );
            strMasked.Append($"                       PlaceHolder=\"{_item.PlaceHolder}\"\n");
            strMasked.Append($"                       Length=\"{column.Longitud}\"\n");
            strMasked.Append(
                $"                       SetStatus=\"@SetStatus{_shortComponent}{_item.Id}\"\n"
            );
            strMasked.Append($"                       ValueChangedEvc=\"@{_item.Id}_OnChange\"\n");
            strMasked.Append(
                $"                       OnFocusEvc=\"@_scriptsUser.{_item.Id}_OnFocus\"\n"
            );
            strMasked.Append(
                $"                       OnBlurEvc=\"@_scriptsUser.{_item.Id}_OnBlur\"\n"
            );
            strMasked.Append(
                $"                       @ref=\"@_{_lowShortComponent}{_item.Id}\" />\n"
            );

            return strMasked.ToString();
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
        #endregion Contrucción del codigo

        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoMaskedEditRazor",
                MethodOrContext = "GetMaskedEditRazor"
            }
        };

        #endregion Logs
    }
}
