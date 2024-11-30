using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.SB.Config;

namespace WooW.SB.Forms
{
    public partial class fmGuardarVista : Form
    {
        private Proyecto proyecto;
        public string _NombreVista { get; set; }
        private string vista;
        public string Vista
        {
            get => vista;
        }

        public fmGuardarVista(string NombreVista)
        {
            _NombreVista = NombreVista;
            proyecto = Proyecto.getInstance();
            InitializeComponent();
        }

        private void bnCancelar_Click_1(object sender, EventArgs e)
        {
            vista = "";
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void bnGuardar_Click(object sender, EventArgs e)
        {
            string Archivo = $"{_NombreVista}.xml";

            if (File.Exists(Archivo))
            {
                XtraMessageBox.Show(
                    "Vista ya existe",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            vista = txtNombre.Text.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void fmGuardarVista_Load(object sender, EventArgs e)
        {
            rbPlantillaDefault.Checked = true;
        }
    }
}
