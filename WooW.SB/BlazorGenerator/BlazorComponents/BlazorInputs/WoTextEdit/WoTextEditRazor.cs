using System;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoTextEdit
{
    public class WoTextEditRazor
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
        /// todo el codigo necesario para construir un text edit
        /// </summary>
        private StringBuilder _strTextEdit = new StringBuilder();

        /// <summary>
        /// Contiene strings para poder identar el codigo
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente
        /// </summary>
        private string _shortComponent = "Txt";

        /// <summary>
        /// Prefijo del componente en minúsculas
        /// </summary>
        private string _lowShortComponent = "txt";

        /// <summary>
        /// Columna de la que se realizara el control
        /// </summary>
        private ModeloColumna _column;

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
            try
            {
                this._identMethodsAndProperties = identItemProperty;
                _item = item;

                //GetColumn();
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                _column = woProjectDataHelper.GetColumn(
                    modelName: _item.BaseModelName,
                    property: _item.BindedProperty
                );

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
            catch (Exception ex)
            {
                throw new Exception($@"Error al eliminar la recuperar el código. {ex.Message}");
            }
        }

        #endregion Método principal

        #region Recuperación de la columna

        ///// <summary>
        ///// Recupera la columna buscando en el modelo y los paquetes
        ///// </summary>
        ///// <returns></returns>
        //private void GetColumn()
        //{
        //    try
        //    {
        //        Modelo findModel = _project.ModeloCol.Modelos.FirstOrDefault(x =>
        //            x.Id == _item.BaseModelName
        //        );
        //        if (findModel != null)
        //        {
        //            _column = findModel.Columnas.FirstOrDefault(x => x.Id == _item.BindedProperty);
        //            if (_column == null)
        //            {
        //                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
        //                List<Modelo> models = woProjectDataHelper.GetModelWithExtencions(
        //                    _item.BaseModelName
        //                );
        //                foreach (Modelo model in models)
        //                {
        //                    _column = model.Columnas.FirstOrDefault(x =>
        //                        x.Id == _item.BindedProperty
        //                    );
        //                    if (_column != null)
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
        //            List<Modelo> models = woProjectDataHelper.GetModelWithExtencions(
        //                _item.BaseModelName
        //            );
        //            foreach (Modelo model in models)
        //            {
        //                _column = model.Columnas.FirstOrDefault(x => x.Id == _item.BindedProperty);
        //                if (_column != null)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($@"Error al recuperar la columna. {ex.Message}");
        //    }
        //}

        #endregion Recuperación de la columna

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoTextEditRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorInputs\WoTextEdit\WoTextEditRazor.cs.*@
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
                etiqueta = Etiqueta.ToId(_column.Formulario);
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
            string length = string.Empty;

            string isRequired = "false";

            if (!_item.NoModelComponent)
            {
                if (_column.Longitud > 0)
                {
                    length =
                        $"\n{_identMethodsAndProperties}                    Length=\"{_column.Longitud}\"";

                    isRequired = (!_column.Nulo).ToString().ToLower();
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
{_identMethodsAndProperties}        <WoTextEdit Id=""{_lowShortComponent}{_item.Id}""
{_identMethodsAndProperties}                    TApp=""App""
{_identMethodsAndProperties}                    Value=""@_scriptsUser.{modelName}{_item.BindedProperty}""
{_identMethodsAndProperties}                    IsRequired=@{isRequired}{length}
{_identMethodsAndProperties}                    Password=""@{_item.Password.ToString().ToLower()}""
{_identMethodsAndProperties}                    PlaceHolder=""{_item.PlaceHolder}""
{_identMethodsAndProperties}                    SetStatus=""@SetStatus{_shortComponent}{_item.Id}""
{_identMethodsAndProperties}                    ValueChangedEvc=""@{_item.Id}_OnChange""
{_identMethodsAndProperties}                    OnFocusEvc=""@_scriptsUser.{_item.Id}_OnFocus""
{_identMethodsAndProperties}                    OnBlurEvc=""@_scriptsUser.{_item.Id}_OnBlur""
{_identMethodsAndProperties}                    @ref=""@_{_lowShortComponent}{_item.Id}"" />"
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
        #endregion Contrucción del codigo

        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoTextEditRazor",
                MethodOrContext = "GetTextEditRazor"
            }
        };

        #endregion Logs
    }
}
