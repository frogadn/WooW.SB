using System;
using System.IO;
using System.Windows.Forms;
using WooW.SB.Config;

namespace WooW.SB.Designer.DesignerComponents
{
    public partial class WoNewFreeLayout : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Instancia inyectada de singleton con toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        public WoNewFreeLayout()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Controlador de evento para crear un nuevo layout.
        /// </summary>
        public EventHandler<string> NewLayoutEvt;

        /// <summary>
        /// Controlador de evento para crear un nuevo layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (txtLayoutName.Text != string.Empty)
            {
                string pathLayout = $@"{_project.DirLayOuts}\FreeStyles\{txtLayoutName.Text}.json";
                if (!File.Exists(pathLayout))
                {
                    NewLayoutEvt?.Invoke(this, txtLayoutName.Text);
                    this.Close();
                }
                else
                {
                    ///ToDo: send alerts to log
                    MessageBox.Show("El layout ya existe");
                }
            }
            else
            {
                ///ToDo: send alerts to log
                MessageBox.Show("Ingrese un nombre valido");
            }
        }
    }
}
