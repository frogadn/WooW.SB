using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoDetailGrid
{
    public class WoDetailGridRazor
    {
        #region Instancias singleton

        private Proyecto _project = Proyecto.getInstance();

        private WoLogObserver _observer = new WoLogObserver();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Salva la información razor del componente.
        /// </summary>
        private StringBuilder _strGridComponent = new StringBuilder();

        /// <summary>
        /// Instancia con la meta data del componente.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Instancia con la meta data del componente de la slave.
        /// </summary>
        private WoComponentProperties _itemSlave = new WoComponentProperties();

        /// <summary>
        /// Identación de los componentes.
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Prefijo del componente.
        /// </summary>
        private string _shortComponent = "Grd";

        /// <summary>
        /// Prefijo del componente en minúsculas.
        /// </summary>
        private string _lowShortComponent = "grd";

        #endregion Atributos


        #region Método principal

        /// <summary>
        /// Método principal que construye el componente.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="identItemProperty"></param>
        /// <returns></returns>
        public string GetCode(WoItem item, string identMethodsAndProperties)
        {
            _item = item;
            this._identMethodsAndProperties = identMethodsAndProperties;

            try
            {
                string json = WoDirectory.ReadFile(
                    $@"{_project.DirLayOuts}\FormDesign\{_item.SlaveModelId}.json"
                );
                _itemSlave = JsonConvert.DeserializeObject<WoComponentProperties>(json);
            }
            catch (Exception ex)
            {
                WoDesignerRawSerializerHelper serializerHelper =
                    new WoDesignerRawSerializerHelper();

                WoContainer itemSlaveContainer = serializerHelper.BuildRawWoContainer(
                    _item.SlaveModelId
                );
                _itemSlave = itemSlaveContainer.ConvertToComponentProperties();
                _itemSlave.ContainersCol = itemSlaveContainer.ContainersCol;
                _itemSlave.ItemsCol = itemSlaveContainer.ItemsCol;
            }

            _strGridComponent.Clear();
            _strGridComponent.AppendLine(BuildTag());
            _strGridComponent.AppendLine(BuildBaseComponent());

            _razorReady.Details =
                $@"Se a generado con éxito el código razor para el componente: {item.Id}";
            _observer.SetLog(_razorReady);

            return _strGridComponent.ToString();
        }

        #endregion Método principal

        #region Tag

        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}@*Este código fue generado por el fichero WoDetailGridRazor.cs en el Path: WooW.SB\BlazorGenerator\BlazorComponents\BlazorCustomComponents\WoDetailGrid\WoDetailGridRazor.cs*@
{_identMethodsAndProperties}@*WoWSB por el generador a día 5-10-2023*@";
        }

        #endregion Tag

        #region Construcción del codigo

        /// <summary>
        /// Construye el razor principal del componente.
        /// </summary>
        /// <returns></returns>
        private string BuildBaseComponent()
        {
            string widthSlave = "50%";
            string heightSlave = "80%";
            string title = _item.SlaveModelId;

            if (_itemSlave != null)
            {
                WoContainer modal = _itemSlave.ContainersCol[0];

                widthSlave =
                    (modal.WidthSlave == null || modal.WidthSlave == string.Empty)
                        ? "auto"
                        : modal.WidthSlave;
                heightSlave =
                    (modal.HeightSlave == null || modal.HeightSlave == string.Empty)
                        ? "auto"
                        : modal.HeightSlave;

                title = modal.Etiqueta;
            }

            return $@"
<WoDetailGrid TData=""{_item.ClassModelType}""
                          Id=""{_item.SlaveModelId}""
                          TApp=""App""
                          ColumnsCol=""@_detailColumnsCol{_item.BindedProperty}""
                          Data=""@_scriptsUser.{_item.BaseModelName}.{_item.BindedProperty}""
                          FormPopupTitle =""@Localizer[""{title}""]""
                          FormPopupWidth=""{widthSlave}""                          
                          FormPopupHeight=""{heightSlave}""
                          SetStatus=""@SetStatus{_shortComponent}{_item.BindedProperty}""
                          OpenNewPopupEvc=""@{_item.BindedProperty}OpenNewPopup""
                          SaveNewDetailEvc=""@{_item.BindedProperty}_SaveNewDetail""
                          OpenEditPopupEvc=""@{_item.BindedProperty}_OpenEditPopup""
                          DeleteDetailEvc=""@{_item.BindedProperty}_DeleteDetail""
                          @ref=""@_{_lowShortComponent}{_item.BindedProperty}"">

                <EditFormFragment>
                    @{{
                        <{_item.InternalFrom} />
                    }}
                </EditFormFragment>

                <CustomButtonsFragment>

{BuildCustomButton()}

                </CustomButtonsFragment>

            </WoDetailGrid>
";
        }

        /// <summary>
        /// Construye el razor de los botones custom.
        /// </summary>
        /// <returns></returns>
        private string BuildCustomButton()
        {
            StringBuilder strCustomButtons = new StringBuilder();

            string pathCustomButtons =
                $@"{_project.DirLayOuts}\UserCode\{_item.SlaveModelId}_proj\{_item.SlaveModelId}CustomButtons.json";

            if (File.Exists(pathCustomButtons))
            {
                string json = WoDirectory.ReadFile(pathCustomButtons);
                List<WoCustomButtonProperties> customButtonsProperties =
                    JsonConvert.DeserializeObject<List<WoCustomButtonProperties>>(json);

                foreach (
                    WoCustomButtonProperties customButton in customButtonsProperties.OrderBy(
                        button => button.Index
                    )
                )
                {
                    strCustomButtons.AppendLine(
                        $@"
                    <WoButton Caption=""@Localizer[""{customButton.Label}""]""
                              ButtonType=""eButtonColor.Secondary""
                              Icon=""eBoostrapIcons.{customButton.Icon}""
                              OnClickEvc=""@_{_item.SlaveModelId}ScriptsUser.{customButton.ButtonId}_OnClick"" /> "
                    );
                }
            }

            return strCustomButtons.ToString();
        }

        #endregion Construcción del codigo


        #region Logs

        private WoLog _razorReady = new WoLog()
        {
            CodeLog = "000",
            Title = "Se ha generado el razor de un componente.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDetailGridRazor",
                MethodOrContext = "GetDetailGridRazor"
            }
        };

        #endregion Logs
    }
}
