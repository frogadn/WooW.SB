using WooW.SB.Config;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmEstiloLibre : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }
        public Proyecto proyecto { get; set; }

        public fmEstiloLibre()
        {
            InitializeComponent();
        }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return buAceptarCambios.Enabled; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Cargar()
        {
            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
        }

        public void Refrescar()
        {
            if (buRefrescar.Enabled)
                buRefrescar.PerformClick();
        }
    }
}
