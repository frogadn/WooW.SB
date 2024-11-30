using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using DevExpress.XtraEditors;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoDesignerRawReportHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto.
        /// Para enviar alertas e información al log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Nombre del modelo de que se generara el formulario.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Contenedor principal de formulario que se diseñara.
        /// </summary>
        private WoContainer _root = new WoContainer();

        /// <summary>
        /// Contiene la row en la que se añadira el sigiente componente en agregarse al root.
        /// </summary>
        private int _indexRoot = 0;

        /// <summary>
        /// Indica si el reporte base es del tipo ODATA.
        /// </summary>
        private bool _oDataReport = false;

        /// <summary>
        /// Bandera que indica si el reporte es para una generacion completa de odata.
        /// </summary>
        private bool _oDataComplete = false;

        #endregion Atributos


        #region Metodo principal

        /// <summary>
        /// Metodo prinipal ocupado de generar un formulario base con reporte en base a un modelo.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public WoContainer BuildRawReportForm(
            string modelName,
            bool isOData = false,
            bool odataComplete = false,
            string labelId = "Root"
        )
        {
            _oDataReport = isOData;
            _oDataComplete = odataComplete;
            _modelName = modelName;

            _indexRoot = 0;

            _root = new WoContainer()
            {
                Id = "Root",
                Etiqueta = labelId,
                TypeContainer = eTypeContainer.FormRoot,
                Col = 12,
                Row = 6,
                ColSpan = 1,
                RowSpan = 1,
                BackgorundColorGroup = eGroupColor.Background,
                Theme = "Default",
                ModelId = _modelName,
                ItemsCol = new List<WoItem>(),
                ContainersCol = new List<WoContainer>(),
                HaveModelReference = false,
            };

            SearchModel();

            AddAlertsComponent();
            AddReport(labelId);

            return _root;
        }

        #endregion Metodo principal

        #region Modelo

        /// <summary>
        /// Modelo sobre el que se esta trabajando.
        /// </summary>
        private Modelo _baseModel = null;

        /// <summary>
        /// Busca el modelo y en caso de no poder encontrarlo en los que contiene
        /// project envía una exception.
        /// </summary>
        /// <exception cref="WoObserverException"></exception>
        [SupportedOSPlatform("windows")]
        private void SearchModel()
        {
            try
            {
                _baseModel = _project.ModeloCol.Modelos.Where(x => x.Id == _modelName).First();
                if (_baseModel == null)
                {
                    XtraMessageBox.Show("El modelo no existe");
                }
                else
                {
                    _root.Proceso = _baseModel.ProcesoId;
                    _root.ModelId = _baseModel.Id;

                    _root.ModelType = _baseModel.TipoModelo;
                    _root.SubType = _baseModel.SubTipoModelo;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error al encontrar el modelo");
            }
        }

        #endregion Modelo

        #region Control principal de alertas

        /// <summary>
        /// Agrega el componente de alertas al root.
        /// </summary>
        private void AddAlertsComponent()
        {
            _root.Row += 2;

            string text = EtiquetaCol.Get("Alertas");
            if (text == " ETIQUETA NO EXISTE")
            {
                _labelsNotFound.Add("Alertas");
            }

            _root.ItemsCol.Add(
                new WoItem()
                {
                    Id = "Alertas",
                    Etiqueta = "Alertas",
                    MaskText = EtiquetaCol.Get("Alertas"),
                    Parent = "Root",
                    Enable = eItemEnabled.Activo,
                    TypeItem = eTypeItem.FormItem,
                    Control = "Label",
                    ColSpan = 12,
                    RowSpan = 1,
                    ColumnIndex = 0,
                    RowIndex = _indexRoot,
                    ComponenteExtra = true,
                    BaseModelName = _modelName
                }
            );

            _indexRoot++;
        }

        #endregion Control principal de alertas

        #region Control principal del reporte

        /// <summary>
        /// Agrega el reporte al contenedor root.
        /// </summary>
        private void AddReport(string labelId = "Root")
        {
            _root.Row += 2;

            string text = EtiquetaCol.Get("Reporte");
            if (text == " ETIQUETA NO EXISTE")
            {
                _labelsNotFound.Add("Reporte");
            }

            WoItem reportItem = new WoItem()
            {
                Id = $@"{_modelName}Report",
                Etiqueta = labelId,
                MaskText = EtiquetaCol.Get("Reporte"),
                Parent = "Root",
                Enable = eItemEnabled.Activo,
                TypeItem = eTypeItem.ReportItem,
                Control = "Label",
                ColSpan = 12,
                RowSpan = 10,
                ColumnIndex = 0,
                RowIndex = _indexRoot,
                ComponenteExtra = true,
                BaseModelName = _modelName,
                ReportRequest = _modelName,
                ReportResponse = _modelName.Replace("Request", "Response"),
                ReportOdata = _oDataReport,
                InternalFrom = "Report0Layout"
            };

            if (_oDataComplete)
            {
                reportItem.InternalFrom = $@"{_modelName}ReportLayout";
            }

            _root.ItemsCol.Add(reportItem);

            _indexRoot += 10;
        }

        #endregion Control principal del reporte


        #region Validación de las etiquetas base

        /// <summary>
        /// Lista de las etiquetas que no se encontraron.
        /// </summary>
        private List<string> _labelsNotFound = new List<string>();

        /// <summary>
        /// Valida que todas las etiquetas base se encuentren en el archivo de recursos.
        /// </summary>
        private void AlertToLabels()
        {
            if (_labelsNotFound.Count > 0)
            {
                StringBuilder lostLabels = new StringBuilder();
                foreach (string label in _labelsNotFound)
                {
                    lostLabels.Append($"{label}\n");
                }

                //XtraMessageBox.Show(
                //    text: $@"Faltan las siguientes etiquetas: {lostLabels}",
                //    caption: "Alert",
                //    buttons: MessageBoxButtons.OK,
                //    icon: MessageBoxIcon.Information
                //);
            }
        }

        #endregion Validación de las etiquetas base


        #region Formulario raw Odata

        /// <summary>
        /// Genera el formulario para los filtros de un reporte Odata.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public WoContainer GetContainerOdataFilters(string modelName, string labelId = "Root")
        {
            WoContainer woContainer = new WoContainer()
            {
                Id = "Root",
                Etiqueta = labelId,
                TypeContainer = eTypeContainer.FormRoot,
                Col = 12,
                Row = 1,
                ColSpan = 1,
                RowSpan = 1,
                BackgorundColorGroup = eGroupColor.Background,
                Theme = "Default",
                HaveModelReference = false,
            };

            woContainer.ContainersCol = new List<WoContainer>();
            WoContainer baseForm = new WoContainer()
            {
                Id = "formModelform",
                Etiqueta = modelName,
                MaskText = modelName,
                Parent = "Root",
                TypeContainer = eTypeContainer.FormGroup,
                ColSpan = 12,
                RowSpan = 2,
                Row = 2,
                Col = 12,
                ColumnIndex = 0,
                RowIndex = 0,
                BackgorundColorGroup = eGroupColor.Default,
                ComponentFontColor = eTextColor.FontDefault,
                Theme = "Default",
                Visible = false,
                ItemsCol = new List<WoItem>()
            };

            baseForm.ItemsCol.Add(
                new WoItem()
                {
                    Id = "Alertas",
                    Etiqueta = "Alertas",
                    MaskText = EtiquetaCol.Get("Alertas"),
                    Parent = "Root",
                    Enable = eItemEnabled.Activo,
                    TypeItem = eTypeItem.FormItem,
                    Control = "Label",
                    ColSpan = 12,
                    RowSpan = 1,
                    ColumnIndex = 0,
                    RowIndex = 0,
                    ComponenteExtra = true,
                    BaseModelName = modelName,
                    NoModelComponent = true
                }
            );
            baseForm.ItemsCol.Add(
                new WoItem()
                {
                    Id = "Select",
                    Etiqueta = "Select",
                    MaskText = "Select",
                    BindedProperty = "Select",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    ClassModelType = "",
                    Nullable = false,
                    Control = "Text",
                    Parent = "Root",
                    ColSpan = 6, //Columnas
                    RowSpan = 1,
                    ColumnIndex = 6, //col de inicio
                    RowIndex = 1, // row en la que estara
                    BackgorundColorContainerItem = eContainerItemColor.Default,
                    CaptionColor = eTextColor.FontDefault,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default",
                    BaseModelName = modelName,
                    NoModelComponent = true
                }
            );
            baseForm.ItemsCol.Add(
                new WoItem()
                {
                    Id = "Filter",
                    Etiqueta = "Filter",
                    MaskText = "Filter",
                    BindedProperty = "Filter",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    ClassModelType = "",
                    Nullable = false,
                    Control = "Text",
                    Parent = "Root",
                    ColSpan = 6, //Columnas
                    RowSpan = 1,
                    ColumnIndex = 6, //col de inicio
                    RowIndex = 1, // row en la que estara
                    BackgorundColorContainerItem = eContainerItemColor.Default,
                    CaptionColor = eTextColor.FontDefault,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default",
                    BaseModelName = modelName,
                    NoModelComponent = true
                }
            );
            baseForm.ItemsCol.Add(
                new WoItem()
                {
                    Id = "OrderBy",
                    Etiqueta = "OrderBy",
                    MaskText = "OrderBy",
                    BindedProperty = "OrderBy",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    ClassModelType = "",
                    Nullable = false,
                    Control = "Text",
                    Parent = "Root",
                    ColSpan = 6, //Columnas
                    RowSpan = 1,
                    ColumnIndex = 6, //col de inicio
                    RowIndex = 1, // row en la que estara
                    BackgorundColorContainerItem = eContainerItemColor.Default,
                    CaptionColor = eTextColor.FontDefault,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default",
                    BaseModelName = modelName,
                    NoModelComponent = true
                }
            );
            baseForm.ItemsCol.Add(
                new WoItem()
                {
                    Id = "Top",
                    Etiqueta = "Top",
                    MaskText = "Top",
                    BindedProperty = "Top",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    ClassModelType = "",
                    Nullable = false,
                    Control = "Text",
                    Parent = "Root",
                    ColSpan = 6, //Columnas
                    RowSpan = 1,
                    ColumnIndex = 6, //col de inicio
                    RowIndex = 1, // row en la que estara
                    BackgorundColorContainerItem = eContainerItemColor.Default,
                    CaptionColor = eTextColor.FontDefault,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default",
                    BaseModelName = modelName,
                    NoModelComponent = true
                }
            );
            baseForm.ItemsCol.Add(
                new WoItem()
                {
                    Id = "Skip",
                    Etiqueta = "Skip",
                    MaskText = "Skip",
                    BindedProperty = "Skip",
                    TypeItem = eTypeItem.FormItem,
                    Enable = eItemEnabled.Activo,
                    BindingType = "string",
                    ClassModelType = "",
                    Nullable = false,
                    Control = "Text",
                    Parent = "Root",
                    ColSpan = 6, //Columnas
                    RowSpan = 1,
                    ColumnIndex = 6, //col de inicio
                    RowIndex = 1, // row en la que estara
                    BackgorundColorContainerItem = eContainerItemColor.Default,
                    CaptionColor = eTextColor.FontDefault,
                    ComponentFontColor = eTextColor.FontDefault,
                    Theme = "Default",
                    BaseModelName = modelName,
                    NoModelComponent = true
                }
            );

            woContainer.ContainersCol.Add(baseForm);

            return woContainer;
        }

        #endregion Formulario raw Odata
    }
}
