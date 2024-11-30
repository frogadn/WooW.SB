using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorAlerts.WoAlertList;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormGroup;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormTab;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormTabs;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoDetailGrid;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoFormToolbar;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoListGrid;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoReports;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.CheckBoxEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoComboEnumEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoDateEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoFileEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpDialogEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoLookUpEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMaskedEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoMemoEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoSpinEdit;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorInputs.WoTextEdit;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.Slaves
{
    public class WoSlaveRazorGenerator
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con las configuraciones e información del proyecto con el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        #endregion Variables globales


        #region Instancias de las clases que generan los razor

        private WoFormRazor _woFormRazor = new WoFormRazor();
        private WoFormGroupRazor _woFormGroupRazor = new WoFormGroupRazor();
        private WoFormTabRazor _woFormTabRazor = new WoFormTabRazor();
        private WoFormTabsRazor _woFormTabsRazor = new WoFormTabsRazor();

        private WoAlertListRazor _woAlertListRazor = null;
        private WoFormToolbarRazor _woFormToolbarRazor = null;
        private WoTextEditRazor _woTextEditRazor = null;
        private WoMaskedEditRazor _woMaskedEditRazor = null;
        private WoMemoEditRazor _woMemoEditRazor = null;
        private WoSpinEditRazor _woSpinEditRazor = null;
        private WoLookUpEditRazor _woLookUpEditRazor = null;
        private WoDateEditRazor _woDateEditRazor = null;
        private WoComboEnumEditRazor _woComboEnumEditRazor = null;
        private WoLookUpDialogEditRazor _woLookUpDialogEditRazor = null;
        private WoDetailGridRazor _woDetailGridRazor = null;
        private WoCheckBoxEditRazor _woCheckBoxEditRazor = null;
        private WoFileEditRazor _woFileEditRazor = null;

        private WoReportViewerRazor _woReportViewerRazor = null;

        private WoListGridRazor _woListGridRazor = null;

        #endregion Instancias de las clases que generan los razor

        #region Constructor

        /// <summary>
        /// Asigna el nombre de a clase e instancia las clases que generan ell código razor.
        /// </summary>
        /// <param name="classModelName"></param>
        public WoSlaveRazorGenerator()
        {
            _woAlertListRazor = new WoAlertListRazor();
            _woFormToolbarRazor = new WoFormToolbarRazor();
            _woTextEditRazor = new WoTextEditRazor();
            _woMaskedEditRazor = new WoMaskedEditRazor();
            _woMemoEditRazor = new WoMemoEditRazor();
            _woSpinEditRazor = new WoSpinEditRazor();
            _woDateEditRazor = new WoDateEditRazor();
            _woLookUpEditRazor = new WoLookUpEditRazor();
            _woComboEnumEditRazor = new WoComboEnumEditRazor();
            _woLookUpDialogEditRazor = new WoLookUpDialogEditRazor();
            _woDetailGridRazor = new WoDetailGridRazor();
            _woCheckBoxEditRazor = new WoCheckBoxEditRazor();
            _woFileEditRazor = new WoFileEditRazor();

            _woReportViewerRazor = new WoReportViewerRazor();

            _woListGridRazor = new WoListGridRazor();
        }

        #endregion Constructor

        #region Identación

        /// <summary>
        /// Nivel de identación para grupos internos y controles.
        /// </summary>
        private int _identLevel = -1;

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de la clase;
        /// </summary>
        private String _identGroups = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de las propiedades;
        /// </summary>
        private String _identItems = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado del código dentro de los métodos;
        /// </summary>
        private String _identItemProperties = "";

        /// <summary>
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identGroups = FormatClassHelper.Ident(_identLevel);
            _identItems = FormatClassHelper.Ident(_identLevel + 1);
            _identItemProperties = FormatClassHelper.Ident(_identLevel + 2);
        }

        #endregion Identación

        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar el resto de métodos de la clase
        /// para poder generar la clase final.
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public string GetControlModelClass(
            WoContainer baseContainer,
            bool isReport,
            bool isList,
            bool blazorIntegral
        )
        {
            string modelType =
                (isReport)
                    ? "Report"
                    : (isList)
                        ? "List"
                        : baseContainer.ModelType.ToString();

            string page = string.Empty;

            if (baseContainer.IsExtension)
            {
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                Modelo findModel = woProjectDataHelper.GetMainModel(baseContainer.ModelId);

                page = $@"@page ""/{findModel.ProcesoId}/{modelType}/{baseContainer.ModelId}""";
            }

            if (baseContainer.Proceso != string.Empty)
            {
                page = $@"@page ""/{baseContainer.Proceso}/{modelType}/{baseContainer.ModelId}""";
            }

            StringBuilder urlPage = new StringBuilder();
            urlPage.AppendLine(page);

            if (
                (
                    baseContainer.IsUnit
                    && (
                        baseContainer.ModelType == Core.WoTypeModel.Kardex
                        || baseContainer.ModelType == Core.WoTypeModel.Control
                        || baseContainer.ModelType == Core.WoTypeModel.Configuration
                        || baseContainer.ModelType == Core.WoTypeModel.View
                    )
                )
                || (
                    baseContainer.IsUnit
                    && baseContainer.ModelType == Core.WoTypeModel.Request
                    && baseContainer.SubType == Core.WoSubTypeModel.Report
                )
                || (baseContainer.IsUnit && (!baseContainer.HaveModelReference))
            )
            {
                urlPage.AppendLine($@"@page ""/Index""");
            }

            _finalClass.AppendLine(
                $@"@*Url de acceso*@
{urlPage}

@*Atributo de autorización (solo se puede accesar si esta autenticado)*@
@attribute [Authorize]

@*Herencia de la clase base de componentes (Permite gestionar la autenticación y el cliente)*@
@inherits AWoComponentBase

@*Injection de la dependencia del localizer (recuperación de etiquetas y formatos en función de la cultura)*@
@inject IStringLocalizer<App> Localizer

@* Interop con JavaScript *@
@inject IJSRuntime JSRuntime
"
            );

            if (isReport)
            {
                _finalClass.AppendLine($@"<h5>@Localizer[""{baseContainer.Etiqueta}""]</h5>");
            }

            _finalClass.AppendLine(BuildBodyClass(baseContainer, blazorIntegral));

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Body

        /// <summary>
        /// Método principal para la construcción del cuerpo del razor
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private string BuildBodyClass(WoContainer container, bool blazorIntegral)
        {
            try
            {
                string body = BuildContainer(container, blazorIntegral);
                return body;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al construir el cuerpo del razor. {ex.Message}");
            }
        }

        #endregion Body

        #region Build Containers

        /// <summary>
        /// Recorre el diseño del contenedor que se recibe por parámetro y va construyendo el codigo razor
        /// </summary>
        [SupportedOSPlatform("windows")]
        private string BuildContainer(WoContainer container, bool blazorIntegral)
        {
            try
            {
                // Calcula la identación del razor
                CalculateIdentSpaces();

                // Codigo del contenedor
                StringBuilder strContainerCode = new StringBuilder();

                // Resultado del código del contenedor
                (string open, string close) resultContainer = (string.Empty, string.Empty);

                // En función del tipo del contenedor, generamos el codigo.
                if (container.Id == "Root")
                {
                    // Recuperamos el código para el contenedor principal
                    resultContainer = _woFormRazor.GetCode(_identGroups, container);
                }
                else if (container.TypeContainer == eTypeContainer.FormTabGroup)
                {
                    // Recuperamos el código para el contenedor de tabs
                    resultContainer = _woFormTabsRazor.GetCode(_identGroups, container);
                }
                else if (container.TypeContainer == eTypeContainer.FormTab)
                {
                    // Recuperamos el código para una tab
                    resultContainer = _woFormTabRazor.GetCode(_identGroups, container);
                }
                else if (container.TypeContainer == eTypeContainer.FormGroup)
                {
                    // Recuperamos el código para un formulario convencional
                    resultContainer = _woFormGroupRazor.GetCode(_identGroups, container);
                }
                else
                {
                    // Validamos que no rea la base del diseño de la grid
                    if (container.Id != "RootNoIndexed")
                    {
                        // En caso de que el contenedor no cumpla con ninguna de las condiciones
                        string rawJsonItem = JsonConvert.SerializeObject(container);
                        XtraMessageBox.Show(
                            text: $"El contenedor no entro en ninguna generación:\n {rawJsonItem}",
                            caption: $@"Container not found {container.Id}",
                            icon: System.Windows.Forms.MessageBoxIcon.Error,
                            buttons: System.Windows.Forms.MessageBoxButtons.OK
                        );
                    }
                }

                // Agregamos el código de apertura del contenedor
                strContainerCode.AppendLine(resultContainer.open);

                strContainerCode.AppendLine(AnalizeContainer(container, blazorIntegral));

                // Agregamos el código del cierre del contenedor
                strContainerCode.AppendLine(resultContainer.close);

                return strContainerCode.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recorrer en codigo razor en el contenedor: {container.Id}. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Realiza un análisis del diseño del contenedor y realiza su generación
        /// de codigo en función del contenido
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private string AnalizeContainer(WoContainer container, bool blazorIntegral)
        {
            try
            {
                StringBuilder strContainerCode = new StringBuilder();

                // En caso de ser un grupo de tab solo recorremos las tabs que contiene
                // Un grupo de tabs solo puede contener tabs
                if (container.TypeContainer == eTypeContainer.FormTabGroup)
                {
                    // Recorremos las tabs del grupo de tabs
                    foreach (WoContainer tab in container.ContainersCol)
                    {
                        // Usamos el método para recuperar el codigo de la tab
                        strContainerCode.AppendLine(BuildContainer(tab, blazorIntegral));
                    }
                }
                else
                {
                    // En caso de ser un grupo normal o una tab
                    // Recorremos las filas del container
                    for (int row = 0; row <= container.Row; row++)
                    {
                        // Recuperamos los contenedores en la fila ordenados por la columna en
                        // la que se encuentran
                        List<WoContainer> findContainers = container
                            .ContainersCol.Where(container => container.RowIndex == row)
                            .OrderBy(container => container.ColumnIndex)
                            .ToList();

                        // Lista para los items que puede contener el contenedor
                        // Recuperamos la lista de items en la row actual
                        // Ordenadas por la columna en la que se encuentran
                        List<WoItem> findItems = container
                            .ItemsCol.Where(item => item.RowIndex == row)
                            .OrderBy(item => item.ColumnIndex)
                            .ToList();

                        // Recorremos las 12 columnas de la row en el contenedor para ir generando el codigo
                        for (int column = 0; column <= 12; column++)
                        {
                            // Item en la posición actual
                            WoItem findItem = null;

                            // Contenedor en la posición actual
                            WoContainer findContainer = null;

                            if (findItems.Count > 0)
                            {
                                findItem = findItems.FirstOrDefault(item =>
                                    item.ColumnIndex == column
                                );
                            }
                            if (findContainers.Count > 0)
                            {
                                findContainer = findContainers.FirstOrDefault(container =>
                                    container.ColumnIndex == column
                                );
                            }

                            if (findItem != null)
                            {
                                strContainerCode.AppendLine(
                                    GetItemRazorCode(findItem, blazorIntegral)
                                );
                            }
                            else if (findContainer != null)
                            {
                                strContainerCode.AppendLine(
                                    BuildContainer(findContainer, blazorIntegral)
                                );
                            }
                        }
                    }
                }

                return strContainerCode.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al analizar el contenedor {container.Id}. {ex.Message}"
                );
            }
        }

        #endregion Build Containers

        #region Build Items

        /// <summary>
        /// Recupera el código para blazor en función del item que se recibe por parámetro
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        private string GetItemRazorCode(WoItem item, bool blazorIntegral)
        {
            try
            {
                string razorCode = string.Empty;

                if (item.Control == "Label")
                {
                    if (item.Id == "Alertas")
                    {
                        razorCode = _woAlertListRazor.GetCode(_identItems, item);
                    }
                    else if (item.Id == "Controles")
                    {
                        razorCode = _woFormToolbarRazor.GetCode(_identItems, item);
                    }
                    else if (item.Id.Contains("Report"))
                    {
                        razorCode = _woReportViewerRazor.GetCode(_identItems, item);
                    }
                    else if (item.TypeItem == eTypeItem.ReportItem)
                    {
                        razorCode = _woReportViewerRazor.GetCode(_identItems, item);
                    }
                    else if (item.TypeItem == eTypeItem.List)
                    {
                        razorCode = _woListGridRazor.GetCode(_identItems, item, blazorIntegral);
                    }
                }
                else if (
                    item.Control == "Decimal"
                    || item.Control == "TextMask"
                    || item.BindingType.ToLower().Contains("decimal")
                    || item.BindingType.ToLower().Contains("double")
                )
                {
                    razorCode = _woMaskedEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "Text" && !item.BindingType.Contains("byte[]"))
                {
                    if (
                        item.BindingType.ToLower() == "string"
                        || item.BindingType.ToLower() == "string?"
                    )
                    {
                        razorCode = _woTextEditRazor.GetCode(_identItems, item);
                    }
                    else
                    {
                        razorCode = _woMaskedEditRazor.GetCode(_identItems, item);
                    }
                }
                else if (item.Control == "Urn")
                {
                    razorCode = _woTextEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "Memo")
                {
                    razorCode = _woMemoEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "Spin")
                {
                    razorCode = _woSpinEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "Bool")
                {
                    razorCode = _woCheckBoxEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "Date")
                {
                    razorCode = _woDateEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "LookUp")
                {
                    razorCode = _woLookUpEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "LookUpDialog")
                {
                    razorCode = _woLookUpDialogEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "EnumString" || item.Control == "EnumInt")
                {
                    razorCode = _woComboEnumEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "WoState")
                {
                    razorCode = _woComboEnumEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "Decimal" || item.Control == "Custom")
                {
                    razorCode = _woMaskedEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "File" || item.BindingType.Contains("byte[]"))
                {
                    razorCode = _woFileEditRazor.GetCode(_identItems, item);
                }
                else if (item.Control == "MaskText")
                {
                    razorCode = _woMaskedEditRazor.GetCode(_identItems, item);
                }
                else
                {
                    // Validamos que no sea un campo extra del dialog
                    if (!item.Id.Contains("__"))
                    {
                        string rawJsonItem = JsonConvert.SerializeObject(item);
                        XtraMessageBox.Show(
                            text: $"El item no entro en ninguna generación:\n {rawJsonItem}",
                            caption: $@"Item not found {item.Id}",
                            icon: System.Windows.Forms.MessageBoxIcon.Error,
                            buttons: System.Windows.Forms.MessageBoxButtons.OK
                        );
                    }
                }

                return razorCode;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar el codigo del item {item.Id}. {ex.Message}"
                );
            }
        }

        #endregion Build Items
    }
}
