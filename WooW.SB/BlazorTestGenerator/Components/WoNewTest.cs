using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorTestGenerator.BuildTest;
using WooW.SB.Config;

namespace WooW.SB.BlazorTestGenerator.Components
{
    public partial class WoNewTest : DevExpress.XtraEditors.XtraForm
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        /// <summary>
        /// Observador de logs.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Path del test a crear
        /// </summary>
        private string _testPath;

        #endregion Atributos

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        [SupportedOSPlatform("windows")]
        public WoNewTest(string testPath)
        {
            InitializeComponent();

            try
            {
                _testPath = testPath;

                ChargeModels();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al iniciar el componente. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Constructor


        #region Carga de los modelos

        /// <summary>
        /// Carga de los modelos al combo
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeModels()
        {
            try
            {
                if (_project != null)
                {
                    foreach (Modelo model in _project.ModeloCol.Modelos)
                    {
                        string pathDesign = $@"{_project.DirLayOuts}\FormDesign\{model.Id}.json";

                        if (
                            (
                                !(
                                    model.TipoModelo == WoTypeModel.Request
                                    && model.SubTipoModelo != WoSubTypeModel.Report
                                )
                            )
                            && model.TipoModelo != WoTypeModel.Response
                            && model.TipoModelo != WoTypeModel.Complex
                            && model.TipoModelo != WoTypeModel.Interface
                            && model.TipoModelo != WoTypeModel.Class
                            && model.TipoModelo != WoTypeModel.Parameter
                            && model.TipoModelo != WoTypeModel.TransactionSlave
                            && model.TipoModelo != WoTypeModel.CatalogSlave
                            && model.TipoModelo.ToString() != "CollectionComplex"
                            && File.Exists(pathDesign)
                        )
                        {
                            cmbModel.Properties.Items.Add(model.Id);
                        }
                    }

                    cmbModel.SelectedIndex = 0;

                    ChargeTypeTest(_project.ModeloCol.Modelos.First().Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al cargar el combo de modelos. {ex.Message}");
            }
        }

        #endregion Carga de los modelos

        #region Selección de un modelo

        /// <summary>
        /// Selección de uno de los modelos desde el combo para la creación de la prueba
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void cmbModel_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbModel.SelectedText != "")
                {
                    ChargeTypeTest(cmbModel.SelectedText);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al iniciar el componente. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Selección de un modelo

        #region Carga de los tipos de test

        /// <summary>
        /// Recupera el diagrama del modelo y carga el combo con las transiciones
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeTypeTest(string modelName)
        {
            try
            {
                Modelo model = _project.ModeloCol.Modelos.Where(m => m.Id == modelName).First();

                if (model != null)
                {
                    ModeloDiagrama diagrama = model.Diagrama;

                    foreach (Transicion transicion in diagrama.Transiciones)
                    {
                        cmbTestType.Properties.Items.Add(transicion.Id);
                    }

                    cmbTestType.Properties.Items.Add("Custom");

                    cmbTestType.SelectedText = "Custom";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al cargar el combo con las transiciones. {ex.Message}"
                );
            }
        }

        #endregion Carga de los tipos de test

        #region Creación del test

        /// <summary>
        /// Action que se detona al momento de crear un nuevo test
        /// </summary>
        public Action<string> NewTestCreated;

        /// <summary>
        /// Crea un layout default de prueba en función del tipo de test seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCreateTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbTestType.SelectedItem == "")
                {
                    XtraMessageBox.Show(
                        text: $@"Seleccione algún tipo de prueba valido",
                        caption: "Alerta",
                        icon: MessageBoxIcon.Information,
                        buttons: MessageBoxButtons.OK
                    );

                    return;
                }

                WoBuildTest woBuildTest = new WoBuildTest();
                woBuildTest.BuildNewTest(pathTest: _testPath);

                NewTestCreated?.Invoke(_testPath);

                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error durante la creación de la prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
                this.Close();
            }
        }

        #endregion Creación del test


        #region Cancelar creación

        /// <summary>
        /// Action que se detona cuando se cancela la creación del test
        /// </summary>
        public Action CreateTestCancelEvt;

        /// <summary>
        /// Cancelado de la creación del test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CreateTestCancelEvt?.Invoke();
            this.Close();
        }

        #endregion Cancelar creación
    }
}
