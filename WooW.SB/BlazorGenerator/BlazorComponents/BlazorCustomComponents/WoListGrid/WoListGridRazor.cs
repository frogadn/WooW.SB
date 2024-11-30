using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoListGrid
{
    public class WoListGridRazor
    {
        #region Instancias singleton

        /// <summary>
        /// Observador principal.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Instancia con las configuraciones e infirmacion del proyecto con el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Attributes

        /// <summary>
        /// Resultado final del razor del List.
        /// </summary>
        private StringBuilder _strGridList = new StringBuilder();

        /// <summary>
        /// Indica el item con la data para la generación del razor.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Nivel de identación
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Modelo del grid que se esta generando.
        /// </summary>
        private Modelo _model;

        /// <summary>
        /// Propiedades de la grid
        /// </summary>
        private WoGridProperties _gridProperties = new WoGridProperties();

        #endregion Attributes


        #region Método principal

        /// <summary>
        /// Recupera el código del razor del report.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        /// <param name="woItem"></param>
        /// <returns></returns>
        public string GetCode(
            string identMethodsAndProperties,
            WoItem woItem,
            bool blazorIntegral = false
        )
        {
            _item = woItem;
            _identMethodsAndProperties = identMethodsAndProperties;

            _strGridList.Clear();

            //_model = _project.ModeloCol.Modelos.FirstOrDefault(model =>
            //    model.Id == _item.BaseModelName
            //);
            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            _model = woProjectDataHelper.GetMainModel(_item.BaseModelName);

            string toolBar = GetToolBar(blazorIntegral);

            _strGridList.AppendLine(toolBar);

            string razor = GetRazor();

            _strGridList.AppendLine(razor);

            return _strGridList.ToString();
        }

        #endregion Método principal

        #region Build ToolBar

        /// <summary>
        /// Razor de la toolbar
        /// </summary>
        /// <returns></returns>
        private string GetToolBar(bool blazorIntegral = false)
        {
            //string pathJson =
            //    $@"{_project.DirLayOuts}\ListDesign\{_item.BaseModelName}GridList.json";

            //if (!File.Exists(pathJson))
            //{
            //    WoGridDesignerRawHelper rawHelper = new WoGridDesignerRawHelper();
            //    WoGridProperties rawDesign = rawHelper.BuildRawActualModelo(
            //        _item.BaseModelName,
            //        writeFile: true
            //    );
            //}

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

            //-----
            //string json = WoDirectory.ReadFile(pathJson);
            //_gridProperties = JsonConvert.DeserializeObject<WoGridProperties>(json);
            _gridProperties = woProjectDataHelper.GetGridFullDesign(
                _item.BaseModelName,
                blazorIntegral
            );

            StringBuilder strGridRazorCode = new StringBuilder();

            strGridRazorCode.AppendLine($@"<h5>@Localizer[""{_gridProperties.LabelId}""]</h5>");

            strGridRazorCode.AppendLine($@"{_identMethodsAndProperties}@*List*@");
            strGridRazorCode.AppendLine(
                $@"{_identMethodsAndProperties}<WoListToolbar TApp=""App"""
            );

            if (_model.TipoModelo != Core.WoTypeModel.View)
            {
                strGridRazorCode.AppendLine(
                    $@"{_identMethodsAndProperties}               OnRedirectEvc=""@{_item.Id}_RedirectTo"""
                );
            }

            strGridRazorCode.AppendLine(
                $@"{_identMethodsAndProperties}               ShowRedirectionButton=""@false"">"
            );
            strGridRazorCode.AppendLine($@"{_identMethodsAndProperties}    <ItemsFragment>");

            strGridRazorCode.AppendLine(GetCustomButtons());

            strGridRazorCode.AppendLine($@"{_identMethodsAndProperties}    </ItemsFragment>");
            strGridRazorCode.AppendLine($@"{_identMethodsAndProperties}</WoListToolbar>");
            strGridRazorCode.AppendLine($@"{_identMethodsAndProperties}<hr/>");

            return strGridRazorCode.ToString();
        }

        #endregion Build ToolBar

        #region Botones custom

        /// <summary>
        /// Lee el fichero de los botones custom para la grid y genera los componentes en función del json
        /// </summary>
        /// <returns></returns>
        private string GetCustomButtons()
        {
            try
            {
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                List<string> paths = woProjectDataHelper.GetExtencionsProjectPath(
                    _item.BaseModelName
                );
                string actualPath = _project.Dir;
                if (!paths.Contains(actualPath))
                {
                    paths.Add(actualPath);
                }

                StringBuilder strCustomButtons = new StringBuilder();

                foreach (string path in paths)
                {
                    string pathJson =
                        $"{path}\\ProyectData\\LayOuts\\UserCode\\{_item.BaseModelName}GridList_proj\\{_item.BaseModelName}GridListCustomButtons.json";

                    if (File.Exists(pathJson))
                    {
                        string json = WoDirectory.ReadFile(pathJson);
                        List<WoCustomButtonProperties> customButtonCol =
                            Newtonsoft.Json.JsonConvert.DeserializeObject<
                                List<WoCustomButtonProperties>
                            >(json);

                        foreach (
                            WoCustomButtonProperties customButton in customButtonCol.OrderBy(
                                button => button.Index
                            )
                        )
                        {
                            strCustomButtons.AppendLine(
                                $@"
{_identMethodsAndProperties}        <WoSimpleToolbarItem TApp=""App""
{_identMethodsAndProperties}                             Icon=""eBoostrapIcons.{customButton.Icon}""
{_identMethodsAndProperties}                             Text=""{customButton.Label}""
{_identMethodsAndProperties}                             OnClickEvc=""@_scriptsUser.{customButton.ButtonId}_OnClick"" />
"
                            );
                        }
                    }
                }

                return strCustomButtons.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar los botones custom. {ex.Message}");
            }
        }

        #endregion Botones custom

        #region Build razor

        /// <summary>
        /// Construye el razor del report.
        /// </summary>
        /// <returns></returns>
        private string GetRazor()
        {
            StringBuilder strGridComponent = new StringBuilder();

            strGridComponent.AppendLine(
                $@"{_identMethodsAndProperties}<WoODataGrid TData=""{_item.BaseModelName}"""
            );
            strGridComponent.AppendLine($@"{_identMethodsAndProperties}             TApp=""App""");

            if (_model.TipoModelo != Core.WoTypeModel.View)
            {
                strGridComponent.AppendLine(
                    $@"{_identMethodsAndProperties}             CheckSelectionColumnEnabled=""@true"""
                );
                strGridComponent.AppendLine(
                    $@"{_identMethodsAndProperties}             ShowColumnChooser=""@true"""
                );

                strGridComponent.AppendLine(
                    $@"{_identMethodsAndProperties}             IdProperty=""@nameof(_scriptsUser.{_item.BaseModelName}.Id)"""
                );
                strGridComponent.AppendLine(
                    $@"{_identMethodsAndProperties}             SelectedItemsChangedEvc=""@_scriptsUser.Selected{_item.BaseModelName}ItemsChanged"""
                );
            }
            else if (_model.TipoModelo == Core.WoTypeModel.View && _gridProperties.GridSelect)
            {
                WoColumnProperties idCol = _gridProperties.WoColumnPropertiesCol.FirstOrDefault(
                    col => col.IsId
                );
                if (idCol != null)
                {
                    strGridComponent.AppendLine(
                        $@"{_identMethodsAndProperties}             CheckSelectionColumnEnabled=""@true"""
                    );
                    strGridComponent.AppendLine(
                        $@"{_identMethodsAndProperties}             ShowColumnChooser=""@true"""
                    );

                    strGridComponent.AppendLine(
                        $@"{_identMethodsAndProperties}             IdProperty=""@nameof(_scriptsUser.{_item.BaseModelName}.{idCol.Id})"""
                    );
                    strGridComponent.AppendLine(
                        $@"{_identMethodsAndProperties}             SelectedItemsChangedEvc=""@_scriptsUser.Selected{_item.BaseModelName}ItemsChanged"""
                    );
                }
            }
            else
            {
                strGridComponent.AppendLine(
                    $@"{_identMethodsAndProperties}             CheckSelectionColumnEnabled=""@false"""
                );
                strGridComponent.AppendLine(
                    $@"{_identMethodsAndProperties}             ShowColumnChooser=""@false"""
                );
            }

            strGridComponent.AppendLine(
                $@"{_identMethodsAndProperties}             SelectedrowChangedEvc=""@_scriptsUser.OnSelectedRowChanged"""
            );
            strGridComponent.AppendLine(
                $@"{_identMethodsAndProperties}             ColumnsCol=""@_{_item.Id}Columns"""
            );
            strGridComponent.AppendLine(
                $@"{_identMethodsAndProperties}             CellNavigationEvc=""@{_item.Id}_CellNavigation"""
            );
            strGridComponent.AppendLine(
                $@"{_identMethodsAndProperties}             SetAlertEvc=""@{_item.Id}_SetGridAlert"""
            );
            strGridComponent.AppendLine(
                $@"{_identMethodsAndProperties}             @ref=""@_{_item.Id}ListGrid""/>"
            );

            return strGridComponent.ToString();
        }

        #endregion Build razor
    }
}
