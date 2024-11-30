using DevExpress.XtraEditors;
using Svg;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WooW.Core;

namespace WooW.SB.Config.Editors
{
    public partial class fmIconoSelector : Form
    {
        Proyecto project;
        public fmIconoSelector()
        {
            InitializeComponent();
            project = Proyecto.getInstance();
            LoadImage();
        }
        public void LoadImage()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Imagen", typeof(Image));
            List<IconsCatalog> itemList = new List<IconsCatalog>();
            System.Drawing.Image icon = null;


            foreach (var pathIcon in Directory.GetFiles(project.DirLayOuts + "\\icons"))
            {
                var name = QuitarEspaciosYGuiones(Path.GetFileName(pathIcon).Replace(".svg", ""));
                var svgDoc = SvgDocument.Open(
               pathIcon
           );
                icon = new Bitmap(svgDoc.Draw());
                itemList.Add(new IconsCatalog { Id = name, Icon = icon });
            }



            foreach (IconsCatalog iconos in itemList)
            {
                DataRow drRow = dt.NewRow();
                drRow["Imagen"] = iconos.Icon;
                drRow["Id"] = iconos.Id;
                dt.Rows.Add(drRow);
            }


            // Enlaza la lista al GridControl.
            grdIcon.DataSource = dt;
        }
        public string Id { get; set; }
        public class IconsCatalog
        {
            public string Id { get; set; }
            public Image Icon { get; set; }
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            DataRow drLoc = grdIconoView.GetFocusedDataRow();

            if (drLoc.IsNull())
            {
                XtraMessageBox.Show(
                    $"Seleccione un icono",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            Id = drLoc["Id"].ToString();

            DialogResult = DialogResult.OK;
        }
        /// <summary>
        /// Evento de cancelar la edición de agregar un icono
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        private void btnDeleteIcon_Click(object sender, EventArgs e)
        {
            Id = "Sin Icono";

            DialogResult = DialogResult.OK;
        }
        static string QuitarEspaciosYGuiones(string input)
        {
            // Crear un diccionario para mapear números a nombres
            Dictionary<char, string> numerosNombres = new Dictionary<char, string>
        {
            {'0', "cero"},
            {'1', "uno"},
            {'2', "dos"},
            {'3', "tres"},
            {'4', "cuatro"},
            {'5', "cinco"},
            {'6', "seis"},
            {'7', "siete"},
            {'8', "ocho"},
            {'9', "nueve"}
        };

            // Reemplaza los espacios con cadena vacía ""
            string sinEspacios = input.Replace(" ", "");

            // Reemplaza los guiones "-" con cadena vacía ""
            string sinGuiones = sinEspacios.Replace("-", "");

            // Inicializa una cadena para el resultado
            string resultado = "";

            // Itera a través de cada carácter de la cadena sin guiones
            foreach (char caracter in sinGuiones)
            {
                // Si el carácter es un número, reemplázalo con su nombre
                if (numerosNombres.ContainsKey(caracter))
                {
                    resultado += numerosNombres[caracter];
                }
                else
                {
                    resultado += caracter; // Mantén otros caracteres sin cambios
                }
            }

            return resultado;
        }

        private void fmIconoSelector_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
