using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using DevExpress.XtraEditors;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoDesignerRawListHelper
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
        /// Contiene la row en la que se añadirá el siguiente componente en agregarse al root.
        /// </summary>
        private int _indexRoot = 0;

        #endregion Atributos


        #region Método principal

        /// <summary>
        /// Método principal ocupado de generar un formulario base con reporte en base a un modelo.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public WoContainer BuildRawListForm(string modelName)
        {
            _modelName = modelName;

            _indexRoot = 0;

            _root = new WoContainer()
            {
                Id = "RootNoIndexed",
                Etiqueta = "RootNoIndexed",
                TypeContainer = eTypeContainer.None,
                Parent = "Root",
                Col = 12,
                Row = 6,
                ColSpan = 1,
                RowSpan = 1,
                BackgorundColorGroup = eGroupColor.Background,
                Theme = "Default",
                ModelId = _modelName,
                ItemsCol = new List<WoItem>(),
                ContainersCol = new List<WoContainer>(),
            };

            SearchModel();

            AddAlertsComponent();
            AddList();

            return _root;
        }

        #endregion Método principal


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
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

                _baseModel = woProjectDataHelper.GetMainModel(_modelName);
                //_baseModel = _project.ModeloCol.Modelos.FirstOrDefault(x => x.Id == _modelName);

                _root.IsExtension = (_baseModel.ProcesoId == string.Empty);

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
                throw new Exception(
                    $"Error al encontrar el modelo: {ex.Message} en WoDesignerRawListHelper"
                );
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
                    AddFormItem = false,
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

        #region Control principal del grid

        /// <summary>
        /// Agrega el list al contenedor root.
        /// </summary>
        private void AddList()
        {
            _root.Row += 2;

            string text = EtiquetaCol.Get("List");
            if (text == " ETIQUETA NO EXISTE")
            {
                _labelsNotFound.Add("List");
            }

            _root.ItemsCol.Add(
                new WoItem()
                {
                    Id = $@"{_modelName}ListGrid",
                    Etiqueta = "List",
                    MaskText = EtiquetaCol.Get("List"),
                    Parent = "RootNoIndexed",
                    Enable = eItemEnabled.Activo,
                    TypeItem = eTypeItem.List,
                    Control = "Label",
                    ColSpan = 12,
                    RowSpan = 5,
                    ColumnIndex = 0,
                    RowIndex = _indexRoot,
                    ComponenteExtra = true,
                    BaseModelName = _modelName,
                    ModelType = _baseModel.TipoModelo,
                    SubType = _baseModel.SubTipoModelo
                }
            );

            _indexRoot += 10;
        }

        #endregion Control principal del grid


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
    }
}
