using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Designer.DesignerComponents
{
    public partial class WoSelectorCustomLayouts : UserControl
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

        #region Variables principales

        /// <summary>
        /// Path de la carpeta con los layouts libres.
        /// </summary>
        private string _pathFreeLayouts = string.Empty;

        /// <summary>
        /// Nombre del layout seleccionado.
        /// </summary>
        private string _layoutName = string.Empty;

        /// <summary>
        /// Get del layout seleccionado.
        /// </summary>
        /// <returns></returns>
        public string GetLayoutName()
        {
            return _layoutName;
        }

        #endregion Variables principales


        #region Constructor

        /// <summary>
        /// Constructor principal del contenedor
        /// </summary>
        public WoSelectorCustomLayouts()
        {
            InitializeComponent();

            ChargeFreeLayouts();
        }

        #endregion Constructor


        #region Carga de los modelos

        /// <summary>
        /// Lista de los layouts salvados.
        /// </summary>
        private List<string> _freeLayouts = new List<string>();

        /// <summary>
        /// Carga la grid con los layout en la carpeta.
        /// </summary>
        public void ChargeFreeLayouts()
        {
            _freeLayouts.Clear();
            _pathFreeLayouts = $@"{_project.DirLayOuts}\FreeStyles";

            List<string> layoutPaths = WoDirectory.ReadDirectoryFiles(_pathFreeLayouts);
            foreach (string layoutPath in layoutPaths)
            {
                string[] layoutPathCol = layoutPath.Split('\\');
                _freeLayouts.Add(layoutPathCol.Last().Replace(".json", ""));
            }

            grdFreeLayouts.DataSource = _freeLayouts;
            grdFreeLayouts.RefreshDataSource();
        }

        #endregion Carga de los modelos

        #region Selección del layout

        /// <summary>
        /// COntrolador de eventos que se detona cuando se selecciona un layout.
        /// </summary>
        public EventHandler<WoContainer> LayoutSelectedEvt;

        /// <summary>
        /// Selector manual para la primera carga del diseñador
        /// </summary>
        public void ReSelected()
        {
            grdFreeLayoutsView_FocusedRowChanged(null, null);
        }

        /// <summary>
        /// Selección del layout de las opciones en la grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdFreeLayoutsView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            if (_freeLayouts.Count > 0)
            {
                string layoutSelected = grdFreeLayoutsView.FocusedValue.ToString();
                _layoutName = layoutSelected;
                string json = WoDirectory.ReadFile($@"{_pathFreeLayouts}\{layoutSelected}.json");
                WoContainer container = JsonConvert.DeserializeObject<WoContainer>(json);
                LayoutSelectedEvt?.Invoke(this, container);
            }
        }

        #endregion Selección del layout
    }
}
