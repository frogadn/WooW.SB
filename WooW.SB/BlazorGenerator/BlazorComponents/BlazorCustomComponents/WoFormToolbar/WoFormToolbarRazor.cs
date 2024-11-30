using System.Collections.Generic;
using System.Text;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoFormToolbar
{
    public class WoFormToolbarRazor
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia del observador de logs.
        /// </summary>
        private WoLogObserver _observer = new WoLogObserver();

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Salva la informacion razor del componente.
        /// </summary>
        private StringBuilder _strFormToolbarRazor = new StringBuilder();

        /// <summary>
        /// Identacion del componente de razor.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Identacion del componente de razor.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        #endregion Atributos


        #region Metodo principal

        /// <summary>
        /// Metodo principal que genera el codigo razor del componente.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetCode(string identMethodsAndProperties, WoItem item)
        {
            _item = item;
            _identMethodsAndProperties = identMethodsAndProperties;

            _strFormToolbarRazor.Clear();

            _strFormToolbarRazor.AppendLine(BuildTag());

            _strFormToolbarRazor.Append(BuildComponent());

            _razorReady.Details =
                $@"Se a generado con éxito el código razor para el componente: {item.Id}";
            _observer.SetLog(_razorReady);

            return _strFormToolbarRazor.ToString();
        }

        #endregion Metodo principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoFormToolbarRazor.cs *@";
        }

        #endregion Tag


        #region Componente

        /// <summary>
        /// Construlle el componente de razor.
        /// </summary>
        /// <returns></returns>
        private string BuildComponent()
        {
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            Modelo model = woProjectDataHelper.GetMainModel(_item.BaseModelName);

            string strCatalog =
                (
                    _item.ModelType == Core.WoTypeModel.Catalog
                    || _item.ModelType == Core.WoTypeModel.CatalogType
                )
                    ? "true"
                    : "false";
            string strOnDelete =
                (
                    _item.ModelType == Core.WoTypeModel.Catalog
                    || _item.ModelType == Core.WoTypeModel.CatalogType
                )
                    ? $@"{_identMethodsAndProperties}                 DeleteOnClickEvc=""@{_item.Id}Delete_OnClick"""
                    : string.Empty;

            return $@"
{_identMethodsAndProperties}@*Toolbar de navegación y transiciones*@
{_identMethodsAndProperties}<WoFormToolbar Id=""{_item.Id}""
{_identMethodsAndProperties}               TApp=""App""
{_identMethodsAndProperties}               SetStatus=""@SetStatus{_item.Id}""
{_identMethodsAndProperties}               OnFirstClick=""@First_OnClick""
{_identMethodsAndProperties}               OnPriorClick=""@Prior_OnClick""
{_identMethodsAndProperties}               OnNextClick=""@Next_OnClick""
{_identMethodsAndProperties}               OnLastClick=""@Last_OnClick""
{_identMethodsAndProperties}               Value=""@_currentTransition""
{_identMethodsAndProperties}               OnTransitionChangeEvc=""@(newValue => {_item.Id}Transition_OnChange(newValue))""
{_identMethodsAndProperties}               TransitionsCol=""@_transitionsCol""
{_identMethodsAndProperties}               ExecuteOnClickEvc=""@{_item.Id}Execute_OnClick""
{_identMethodsAndProperties}               CancelOnClickEvc=""@{_item.Id}Cancel_OnClick""
{strOnDelete}
{_identMethodsAndProperties}               IsCatalog=""@{strCatalog}""
{_identMethodsAndProperties}               SendAlertEvc=""@SendTransitionAlert""
{_identMethodsAndProperties}               ShowRedirectButton=""@true""
{_identMethodsAndProperties}               ListUrl=""{model.ProcesoId}/List/{_item.BaseModelName}""
{_identMethodsAndProperties}               @ref=""_ftbControles"">
{_identMethodsAndProperties}    <ItemsFragment>

{_identMethodsAndProperties}        <WoShareButton Id=""btn{_item.Id}"" Url=""@(NavigationManager.Uri)"" ParameterCol=""@(new List<WoShareParameter>()
{_identMethodsAndProperties}            {{
{_identMethodsAndProperties}                new WoShareParameter {{Name = ""Id"", Value = _scriptsUser.{_item.BaseModelName}.Id}}
{_identMethodsAndProperties}            }})""
{_identMethodsAndProperties}            TApp=""App""
{_identMethodsAndProperties}            SetStatus=""@SetStatusWsbCopiar""
{_identMethodsAndProperties}            @ref=""_wsbCopiar"" />

{GetCustomButonsCode()}

{_identMethodsAndProperties}        </ItemsFragment>

{_identMethodsAndProperties}    </WoFormToolbar>
{_identMethodsAndProperties}    <div class=""dxbl-fl-row-break"" style=""height:1px""></div>
";
        }

        private string GetCustomButonsCode()
        {
            StringBuilder strCustomButtons = new StringBuilder();

            //if (
            //    File.Exists(
            //        $@"{_project.DirLayOuts}\UserCode\{_item.BaseModelName}_proj\{_item.BaseModelName}CustomButtons.json"
            //    )
            //)
            //{
            //Modelo model = _project
            //    .ModeloCol.Modelos.Where(m => m.Id == _item.BaseModelName)
            //    .FirstOrDefault();

            //string json = WoDirectory.ReadFile(
            //    $@"{_project.DirLayOuts}\UserCode\{_item.BaseModelName}_proj\{_item.BaseModelName}CustomButtons.json"
            //);
            //List<WoCustomButtonProperties> customButtonCol =
            //    Newtonsoft.Json.JsonConvert.DeserializeObject<List<WoCustomButtonProperties>>(
            //        json
            //    );

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            List<WoCustomButtonProperties> customButtonCol = woProjectDataHelper.GetCustomButtons(
                _item.BaseModelName
            );

            //foreach (
            //    WoCustomButtonProperties customButton in customButtonCol.OrderBy(button =>
            //        button.Index
            //    )
            //)

            if (customButtonCol.Count > 0)
            {
                foreach (WoCustomButtonProperties customButton in customButtonCol)
                {
                    strCustomButtons.AppendLine(
                        $@"
{_identMethodsAndProperties}        <WoSimpleToolbarItem TApp=""App""
{_identMethodsAndProperties}            Id=""{customButton.ButtonId}""
{_identMethodsAndProperties}            Icon=""eBoostrapIcons.{customButton.Icon}""
{_identMethodsAndProperties}            Text=""{customButton.Label}""
{_identMethodsAndProperties}            OnClickEvc=""@_scriptsUser.{customButton.ButtonId}_OnClick""/>
"
                    );
                }
            }

            //}

            return strCustomButtons.ToString();
        }

        #endregion Componente


        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoRecordToolBarRazor",
                MethodOrContext = "GetRecordToolBarRazor"
            }
        };

        #endregion Logs
    }
}
