using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using WooW.SB.Config;
using WooW.SB.Helpers.GeneratorsHelpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Reportes.ReportModels;

namespace WooW.SB.Reportes.ReportsComponents
{
    public partial class fmPropReports : Form
    {
        #region Variables
        /// <summary>
        /// Variable con la instancia del proyecto
        /// </summary>
        public Proyecto proyecto { get; set; }

        /// <summary>
        /// Variable para almacenar el id del reporte
        /// </summary>
        string idReport = string.Empty;

        /// <summary>
        /// Instancia del diseño de la grid.
        /// </summary>
        WoReport woReport = new WoReport();

        #endregion Variables

        public fmPropReports(dynamic objectSource)
        {
            woReport = objectSource;
            proyecto = Proyecto.getInstance();
            InitializeComponent();
            //Asignacion de propiedades
            HidePropertiesHelper.ModifyBrowsableAttribute(
                objectSource,
                new List<string>()
                {
                    "idReport",
                    "Etiqueta",
                    "proccessId",
                    "modelType",
                    "jsonModel",
                    "reportDesign",
                    "MaskText",
                },
                false
            );
            HidePropertiesHelper.ModifyBrowsableAttribute(
                objectSource,
                new List<string>()
                {
                    "LabelId",
                    //"AutoPrint",
                    //"NumPrint",
                    //"Export",
                    //"TittleType",
                    //"PrintClose"
                },
                true
            );
            propReports.SelectedObject = objectSource;
            idReport = objectSource.idReport;
        }

        /// <summary>
        /// Evento que se dispara al hacer alguna modificación en las propiedades del reporte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propReports_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            #region Creación del archivo de configuración

            #region Serializar el reporte

            if (!Directory.Exists(proyecto.DirVistasReports))
            {
                Directory.CreateDirectory(proyecto.DirVistasReports);
            }
            string json = JsonConvert.SerializeObject(propReports.SelectedObject);
            WoDirectory.WriteFile($@"{proyecto.DirVistasReports}/{idReport}.json", json);

            #endregion Serializar el reporte


            #endregion Creación del archivo de configuración
        }

        private void fmPropReports_FormClosing(object sender, FormClosingEventArgs e)
        {
            #region Creación del archivo de configuración

            #region Serializar el reporte

            if (!Directory.Exists(proyecto.DirVistasReports))
            {
                Directory.CreateDirectory(proyecto.DirVistasReports);
            }
            string json = JsonConvert.SerializeObject(propReports.SelectedObject);
            WoDirectory.WriteFile($@"{proyecto.DirVistasReports}/{idReport}.json", json);

            #endregion Serializar el reporte


            #endregion Creación del archivo de configuración
        }

        private void propReports_CellValueChanging(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            if (e.Row.Properties.FieldName == "LabelId")
            {
                string text = EtiquetaCol.Get(e.Value.ToString());
                woReport.LabelId = e.Value.ToString();
                woReport.MaskText = text;
            }
        }
    }
}
