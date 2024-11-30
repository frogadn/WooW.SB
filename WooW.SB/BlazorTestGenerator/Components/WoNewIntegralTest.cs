using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorTestGenerator.Models;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.Components
{
    public partial class WoNewIntegralTest : DevExpress.XtraEditors.XtraForm
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
        public Action IntegralCreatedEvt;

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        [SupportedOSPlatform("windows")]
        public WoNewIntegralTest()
        {
            InitializeComponent();

            try
            {
                ChargeGroups();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al cargar los grupos. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Constructor


        #region Cargar Grupos

        /// <summary>
        /// Cargamos los grupos al combo
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeGroups()
        {
            try
            {
                string pathIntegralTests = $@"{_project.DirProyectData}\Test\TestCafe\Integral";
                List<string> groups = WoDirectory.ReadDirectoryDirectories(
                    pathIntegralTests,
                    onlyNames: true
                );

                cmbGroup.Properties.Items.AddRange(groups);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la carga de los grupos. {ex.Message}");
            }
        }

        #endregion Cargar Grupos


        #region Creación de prueba integral

        /// <summary>
        /// Creamos una nueva integral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                WoTestIntegral woTestIntegral = new WoTestIntegral();
                woTestIntegral.IntegralName = txtName.Text;
                woTestIntegral.IntegralDescription = txtDescripcion.Text;
                woTestIntegral.Group = cmbGroup.Text;

                string pathIntegral =
                    $@"{_project.DirProyectData}\Test\TestCafe\Integral\{cmbGroup.Text}\{txtName.Text}.json";
                string pathGroup =
                    $@"{_project.DirProyectData}\Test\TestCafe\Integral\{cmbGroup.Text}";

                if (File.Exists(pathIntegral))
                {
                    throw new Exception("Ya existe una prueba integral con ese nombre");
                }
                else
                {
                    if (!Directory.Exists(pathGroup))
                    {
                        Directory.CreateDirectory(pathGroup);
                    }

                    string json = JsonConvert.SerializeObject(woTestIntegral);
                    WoDirectory.WriteFile(pathIntegral, json);
                    if (File.Exists(pathIntegral))
                    {
                        IntegralCreatedEvt.Invoke();
                        this.Close();
                    }
                    else
                    {
                        throw new Exception("Ocurrió un error al crear la prueba");
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

        #endregion Creación de prueba integral
    }
}
