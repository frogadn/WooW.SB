using System;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.Components
{
    public partial class WoNewIntegralGroup : DevExpress.XtraEditors.XtraForm
    {
        #region Instancias singleton

        /// <summary>
        /// Observador principal.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Instancia singleton de project que contiene toda la información del proyecto base sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Action que se detona cuando se crea un nuevo grupo.
        /// </summary>
        public Action GroupCreatedEvt;

        #endregion Atributos

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        public WoNewIntegralGroup()
        {
            InitializeComponent();
        }

        #endregion Constructor


        #region Creación del grupo

        /// <summary>
        /// Creación del grupo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                string newGorupPath =
                    $@"{_project.DirProyectData}\Test\TestCafe\Integral\{txtGroupName.Text}";
                if (Directory.Exists(newGorupPath))
                {
                    throw new Exception($@"EL grupo ya existe");
                }
                else
                {
                    WoDirectory.CreateDirectory(newGorupPath);
                    if (Directory.Exists(newGorupPath))
                    {
                        GroupCreatedEvt.Invoke();
                        this.Close();
                    }
                    else
                    {
                        throw new Exception(
                            $@"No se pudo crear correctamente el grupo. {newGorupPath}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar crear el grupo de pruebas. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
                this.Close();
            }
        }

        #endregion Creación del grupo


        [SupportedOSPlatform("windows")]
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
