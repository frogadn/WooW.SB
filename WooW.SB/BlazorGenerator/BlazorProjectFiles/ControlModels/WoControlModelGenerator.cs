using System;
using System.Text;
using WooW.Core;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorAlerts.WoAlertList;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoForm;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormGroup;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormTab;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorContainers.WoFormTabs;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoDetailGrid;
using WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoFormToolbar;
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
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.ControlModels
{
    public class WoControlModelGenerator
    {
        #region Variables globales

        /// <summary>
        /// Nombre de la clase y fichero
        /// </summary>
        private string _classModelName = string.Empty;

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        #endregion Variables globales


        #region Instancias de las clases que generan las views

        private WoAlertListView _woAlertListView = null;

        private WoFormToolbarView _woFormToolbarView = null;

        private WoDetailGridView _woDetailGridView = null;

        private WoTextEditView _woTextEditView = null;
        private WoMaskedEditView _woMaskedEditView = null;
        private WoMemoEditView _woMemoEditView = null;
        private WoSpinEditView _woSpinEditView = null;
        private WoDateEditView _woDateEditView = null;
        private WoLookUpEditView _woLookUpEditView = null;
        private WoComboEnumEditView _woComboEnumView = null;
        private WoLookUpDialogEditView _woLookUpDialogEditView = null;
        private WoCheckBoxEditView _woCheckBoxEditView = null;
        private WoReportViewerView _woReportViewerView = null;
        private WoFileEditView _woFileEditView = null;

        private WoFormView _woFormView = null;
        private WoFormGroupView _woFormGroupView = null;
        private WoFormTabView _woFormTabView = null;
        private WoFormTabsView _woFormTabsView = null;

        #endregion Instancias de las clases que generan las views


        #region Constructor

        /// <summary>
        /// Asigna el nombre de a clase, calcula la identación del codigo a generar e instancia las clases que generan las views.
        /// </summary>
        /// <param name="className"></param>
        public WoControlModelGenerator(string classModelName)
        {
            _classModelName = classModelName;

            CalculateIdentSpaces();

            _woAlertListView = new WoAlertListView(_identMethodsAndProperties);

            _woFormToolbarView = new WoFormToolbarView(_identMethodsAndProperties);
            _woDetailGridView = new WoDetailGridView(_identMethodsAndProperties);
            _woTextEditView = new WoTextEditView(_identMethodsAndProperties);
            _woMaskedEditView = new WoMaskedEditView(_identMethodsAndProperties);
            _woMemoEditView = new WoMemoEditView(_identMethodsAndProperties);
            _woSpinEditView = new WoSpinEditView(_identMethodsAndProperties);
            _woDateEditView = new WoDateEditView(_identMethodsAndProperties);
            _woLookUpEditView = new WoLookUpEditView(_identMethodsAndProperties);
            _woLookUpDialogEditView = new WoLookUpDialogEditView(_identMethodsAndProperties);
            _woComboEnumView = new WoComboEnumEditView(_identMethodsAndProperties);
            _woCheckBoxEditView = new WoCheckBoxEditView(_identMethodsAndProperties);
            _woReportViewerView = new WoReportViewerView(_identMethodsAndProperties);
            _woFileEditView = new WoFileEditView(_identMethodsAndProperties);

            _woFormView = new WoFormView(_identMethodsAndProperties);
            _woFormGroupView = new WoFormGroupView(_identMethodsAndProperties);
            _woFormTabView = new WoFormTabView(_identMethodsAndProperties);
            _woFormTabsView = new WoFormTabsView(_identMethodsAndProperties);
        }

        #endregion Constructor


        #region Identación

        /// <summary>
        /// Nivel de identación para grupos internos y controles.
        /// </summary>
        private int _identLevel = 0;

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de la clase;
        /// </summary>
        private String _identClass = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de las propiedades;
        /// </summary>
        private String _identMethodsAndProperties = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado del código dentro de los métodos;
        /// </summary>
        private String _identCode = "";

        /// <summary>
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identClass = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identMethodsAndProperties = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identCode = FormatClassHelper.Ident(_identLevel);
        }

        #endregion Identación


        #region Método principal

        /// <summary>
        /// Método principal ocupado de orquestar el resto de métodos de la clase
        /// para poder generar la clase final.
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        public string GetControlModelClass(WoContainer baseContainer)
        {
            _finalClass.Clear();

            BuildHeaderClass();

            BuildBodyClass(baseContainer);

            BuilFooterClass();

            return _finalClass.ToString();
        }

        #endregion Método principal

        #region Header

        /// <summary>
        /// Construye el header de la clase.
        /// </summary>
        private void BuildHeaderClass()
        {
            _finalClass.AppendLine(
                $@"
{_identClass}public class {_classModelName}Controls
{_identClass}{{
{_identClass}   #region Constructores

{_identClass}   /// <summary>
{_identClass}   /// En caso de requerir instancias desde el despachador de blazor se inicializan aquí.
{_identClass}   /// </summary>
{_identClass}   public {_classModelName}Controls() {{ }}

{_identClass}   #endregion Constructores"
            );
        }

        #endregion Header

        #region Footer

        /// <summary>
        /// Construye el footer de la clase.
        /// </summary>
        private void BuilFooterClass()
        {
            _finalClass.AppendLine($@"{_identClass}}}");
        }

        #endregion Footer

        #region Body

        /// <summary>
        /// Construye las instancias de las vies de los componentes en el objeto que se recibe por parametro.
        /// </summary>
        /// <param name="container"></param>
        private void BuildBodyClass(WoContainer container)
        {
            if (container.TypeContainer == eTypeContainer.FormRoot)
            {
                _finalClass.AppendLine(_woFormView.GetCode(container));
            }
            else if (container.TypeContainer == eTypeContainer.FormTabGroup)
            {
                _finalClass.AppendLine(_woFormTabsView.GetCode(container));
            }
            else if (container.TypeContainer == eTypeContainer.FormTab)
            {
                _finalClass.AppendLine(_woFormTabView.GetCode(container));
            }
            else if (container.TypeContainer == eTypeContainer.FormGroup)
            {
                _finalClass.AppendLine(_woFormGroupView.GetCode(container));
            }

            if (!container.ContainersCol.IsNull())
            {
                foreach (var subContainer in container.ContainersCol)
                {
                    BuildBodyClass(subContainer);
                }
            }

            if (!container.ItemsCol.IsNull())
            {
                foreach (WoItem item in container.ItemsCol)
                {
                    if (item.Control == "Label")
                    {
                        if (item.Id == "Alertas")
                        {
                            _finalClass.AppendLine(_woAlertListView.GetCode(item));
                        }
                        else if (item.Id == "Controles")
                        {
                            _finalClass.AppendLine(_woFormToolbarView.GetCode(item));
                        }
                        else if (item.TypeItem == eTypeItem.ReportItem)
                        {
                            _finalClass.AppendLine(_woReportViewerView.GetCode(item));
                        }
                    }
                    else if (
                        item.Control == "Decimal"
                        || item.BindingType.ToLower().Contains("decimal")
                        || item.BindingType.ToLower().Contains("double")
                    )
                    {
                        _finalClass.AppendLine(_woMaskedEditView.GetCode(item));
                    }
                    else if (item.Control == "TextMask")
                    {
                        _finalClass.AppendLine(_woMaskedEditView.GetCode(item));
                    }
                    else if (item.Control == "Text" || item.Control == "Urn")
                    {
                        if (
                            item.BindingType.ToLower() == "string"
                            || item.BindingType.ToLower() == "string?"
                        )
                        {
                            _finalClass.AppendLine(_woTextEditView.GetCode(item));
                        }
                        else
                        {
                            _finalClass.AppendLine(_woMaskedEditView.GetCode(item));
                        }
                    }
                    else if (item.Control == "Memo")
                    {
                        _finalClass.AppendLine(_woMemoEditView.GetCode(item));
                    }
                    else if (item.Control == "Spin")
                    {
                        _finalClass.AppendLine(_woSpinEditView.GetCode(item));
                    }
                    else if (item.Control == "Date")
                    {
                        _finalClass.AppendLine(_woDateEditView.GetCode(item));
                    }
                    else if (item.Control == "LookUp")
                    {
                        _finalClass.AppendLine(_woLookUpEditView.GetCode(item));
                    }
                    else if (item.Control == "LookUpDialog")
                    {
                        _finalClass.AppendLine(_woLookUpDialogEditView.GetCode(item));
                    }
                    else if (item.Control == "Bool")
                    {
                        _finalClass.AppendLine(_woCheckBoxEditView.GetCode(item));
                    }
                    else if (item.Control == "EnumString" || item.Control == "EnumInt")
                    {
                        _finalClass.AppendLine(_woComboEnumView.GetCode(item));
                    }
                    else if (item.Control == "WoState")
                    {
                        _finalClass.AppendLine(_woComboEnumView.GetCode(item));
                    }
                    else if (item.Control == "Slave")
                    {
                        _finalClass.AppendLine(_woDetailGridView.GetCode(item));
                    }
                    else if (item.Control == "Decimal" || item.Control == "Custom")
                    {
                        _finalClass.AppendLine(_woMaskedEditView.GetCode(item));
                    }
                    else if (item.Control == "File")
                    {
                        _finalClass.AppendLine(_woFileEditView.GetCode(item));
                    }
                }
            }
        }

        #endregion Body
    }
}
