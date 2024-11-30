using System;
using System.Collections.Generic;
using System.Data;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Designer.DesignerComponents
{
    public enum eOptionSelector
    {
        Forms
    }

    public partial class WoGenericSelector : DevExpress.XtraEditors.XtraForm
    {
        #region Instancias singleton

        /// <summary>
        /// Información general del proyecto abierto.
        /// </summary>
        public Proyecto proyecto { get; set; } = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Propiedades principales

        /// <summary>
        /// Lista de las posibles opciones del selector.
        /// se reciben en el constructor.
        /// </summary>
        private List<string> _options = new List<string>();

        /// <summary>
        /// Opción seleccionada por el usuario.
        /// </summary>
        public string SelectedOption = string.Empty;

        #endregion Propiedades principales


        #region Constructor principal

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        public WoGenericSelector(List<string> options)
        {
            InitializeComponent();

            _options = options;

            ChargeGrid();
        }

        /// <summary>
        /// Segundo constructor orientado a cargar por default las opciones
        /// desde la opción que se indique.
        /// </summary>
        public WoGenericSelector(eOptionSelector selector)
        {
            InitializeComponent();

            ChargeOptions(selector);
            ChargeGrid();
        }

        #endregion Constructor principal


        #region Carga de opciones

        /// <summary>
        /// En función del tipo de selector que se recibe por parámetro carga al selector
        /// las opciones recuperadas en función de la opción indicada.
        /// </summary>
        /// <param name="selector"></param>
        private void ChargeOptions(eOptionSelector selector)
        {
            List<string> options = new List<string>();

            if (selector == eOptionSelector.Forms)
            {
                string pathOptions = $@"{proyecto.DirLayOuts}\FormDesign";
                _options = WoDirectory.ReadDirectoryFiles(path: pathOptions, onlyNames: true);
            }
        }

        #endregion Carga de opciones


        #region Carga a la grid

        /// <summary>
        /// Carga las opciones del combo en la grid.
        /// </summary>
        private void ChargeGrid()
        {
            DataTable dtOptions = new DataTable();
            dtOptions.Columns.Add("Opción", typeof(string));
            foreach (string option in _options)
            {
                DataRow dr = dtOptions.NewRow();
                dr["Opción"] = option;
            }
            grdSelector.DataSource = _options;
            grdSelector.Refresh();
        }

        #endregion Carga a la grid

        #region Selección del valor

        /// <summary>
        /// Controlador de eventos para cuando se selecciona una opción.
        /// </summary>
        public Action<string> AceptClickEvt { get; set; }

        /// <summary>
        /// Evento de la grid que va actualizando la opción seleccionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdViewSelector_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            dynamic grid = sender;
            SelectedOption = grid.FocusedValue;
        }

        /// <summary>
        /// Evento del botón aceptar que dispara el evento de selección de la opción.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            AceptClickEvt?.Invoke(SelectedOption);
        }

        #endregion Selección del valor
    }
}
