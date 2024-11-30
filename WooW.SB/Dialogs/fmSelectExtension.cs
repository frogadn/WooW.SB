using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs
{
    public partial class fmSelectExtension : DevExpress.XtraEditors.XtraForm
    {
        public string ExtOvr { get; set; }
        public string ModeloExtension { get; set; }

        public WoTypeModel TipoModelo { get; set; }

        private Proyecto proyecto;

        private string _PruebaUnitaria;

        private bool FirstLoad = true;

        public string PruebaUnitaria
        {
            get { return _PruebaUnitaria; }
        }

        public fmSelectExtension()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            DataRow dtRow = grdExtensionView.GetDataRow(grdExtensionView.FocusedRowHandle);

            if (dtRow == null)
            {
                XtraMessageBox.Show(
                    "Debe seleccionar un modelo",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            ModeloExtension = dtRow["Modelo"].ToString();
            TipoModelo = (WoTypeModel)Enum.Parse(typeof(WoTypeModel), dtRow["Tipo"].ToString());
            ExtOvr = dtRow["ExtOvr"].ToString();

            this.DialogResult = DialogResult.OK;
        }

        private void fmCasosPruebaPruebaUnitaria_Load(object sender, EventArgs e)
        {
            if (FirstLoad)
                CargarModelos();
            FirstLoad = false;
        }

        private bool CargarModelos()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"ExtOvr", typeof(string));
            dt.Columns.Add(@"Paquete", typeof(string));
            dt.Columns.Add(@"Modelo", typeof(string));
            dt.Columns.Add(@"Tipo", typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[@"Paquete"], dt.Columns[@"Modelo"] };

            foreach (Paquete paquete in proyecto.Paquetes)
            {
                string pathReal = Proyecto.ArchivoPaquete(
                    proyecto.ArchivoDeProyecto,
                    paquete.Archivo
                );
                var locProyecto = new Proyecto();
                locProyecto.Load(pathReal);
                ResuelvePaqutesModelos(locProyecto, dt);
            }

            grdExtension.DataSource = dt;

            return true;
        }

        private void ResuelvePaqutesModelos(Proyecto principal, DataTable dt)
        {
            var nombreDelPaquete = Path.GetFileNameWithoutExtension(principal.ArchivoDeProyecto);

            foreach (var modelo in principal.ModeloCol.Modelos)
            {
                if (
                    (modelo.TipoModelo == WoTypeModel.Configuration)
                    || (modelo.TipoModelo == WoTypeModel.CatalogType)
                    || (modelo.TipoModelo == WoTypeModel.Catalog)
                    || (modelo.TipoModelo == WoTypeModel.TransactionContable)
                    || (modelo.TipoModelo == WoTypeModel.TransactionNoContable)
                    || (modelo.TipoModelo == WoTypeModel.TransactionFreeStyle)
                    || (modelo.TipoModelo == WoTypeModel.Control)
                    || (modelo.TipoModelo == WoTypeModel.Kardex)
                    || (modelo.TipoModelo == WoTypeModel.Parameter)
                    || (modelo.TipoModelo == WoTypeModel.View)
                    || (
                        (modelo.TipoModelo == WoTypeModel.Class)
                    //&& // Clase con interfaces
                    //(
                    //    (!modelo.Interface1.IsNullOrStringEmpty())
                    //    || (!modelo.Interface2.IsNullOrStringEmpty())
                    //    || (!modelo.Interface3.IsNullOrStringEmpty())
                    //)
                    )
                )
                {
                    if (dt.Rows.Find(new object[] { nombreDelPaquete, modelo.Id }) == null)
                    {
                        DataRow row = dt.NewRow();
                        if (modelo.TipoModelo == WoTypeModel.Class)
                            row["ExtOvr"] = "Override";
                        else
                            row["ExtOvr"] = "Extension";
                        row["Paquete"] = nombreDelPaquete;
                        row["Modelo"] = modelo.Id;
                        row["Tipo"] = modelo.TipoModelo.ToString();
                        dt.Rows.Add(row);
                    }
                }
            }

            foreach (Paquete paquete in principal.Paquetes)
            {
                string pathReal = Proyecto.ArchivoPaquete(
                    principal.ArchivoDeProyecto,
                    paquete.Archivo
                );
                var locProyecto = new Proyecto();
                locProyecto.Load(pathReal);
                ResuelvePaqutesModelos(locProyecto, dt);
            }
        }
    }
}
