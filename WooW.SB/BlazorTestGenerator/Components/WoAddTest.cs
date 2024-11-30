using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
    public partial class WoAddTest : DevExpress.XtraEditors.XtraForm
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
        /// Path del test integral en edición
        /// </summary>
        private string _pathIntegralTest = string.Empty;

        /// <summary>
        /// Settings del test integral actual
        /// </summary>
        private WoTestIntegral _testIntegral = new WoTestIntegral();

        #endregion Atributos


        #region Constructor principal de la clase

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        [SupportedOSPlatform("windows")]
        public WoAddTest(string pathIntegralTest)
        {
            InitializeComponent();

            try
            {
                _pathIntegralTest = pathIntegralTest;

                if (File.Exists(_pathIntegralTest))
                {
                    string rawJson = WoDirectory.ReadFile(_pathIntegralTest);
                    _testIntegral = JsonConvert.DeserializeObject<WoTestIntegral>(rawJson);

                    ChargeTest();
                }
                else
                {
                    throw new Exception($@"No existe el path con la prueba {_pathIntegralTest}");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al cargar el formulario. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Constructor principal de la clase


        #region Carga de pruebas

        /// <summary>
        /// Data source del grid de tests
        /// </summary>
        private DataTable _testCol = new DataTable();

        /// <summary>
        /// Cargamos la grid de test para selección
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeTest()
        {
            try
            {
                _unitTest.Clear();

                _testCol = new DataTable();

                _testCol.Columns.Add($@"Prueba", typeof(string));
                _testCol.Columns.Add($@"Modelo", typeof(string));
                _testCol.Columns.Add($@"Descripción", typeof(string));
                _testCol.Columns.Add($@"Agregar", typeof(bool));

                string unitTestPath = $@"{_project.DirProyectData}\Test\TestCafe\Unit";

                if (Directory.Exists(unitTestPath))
                {
                    List<(string path, string name)> testDirectories =
                        WoDirectory.ReadDirectoryDirectoriesPathName(unitTestPath);
                    foreach ((string path, string name) directory in testDirectories)
                    {
                        List<string> fileTest = WoDirectory.ReadDirectoryFiles(directory.path);
                        foreach (string file in fileTest)
                        {
                            string rawData = WoDirectory.ReadFile(file);
                            string[] rawDataCol = rawData.Split("///");

                            DataRow drTest = _testCol.NewRow();

                            string testName = rawDataCol[2]
                                .Replace("\n", "")
                                .Replace("\r", "")
                                .Replace("Prueba: ", "");

                            drTest["Prueba"] = testName;
                            drTest["Modelo"] = directory.name;
                            drTest["Descripción"] = rawDataCol[3]
                                .Replace("\n", "")
                                .Replace("\r", "")
                                .Replace("Descripción: ", "");

                            IEnumerable<WoTestEjecutionProperties> testResultCol =
                                _testIntegral.TestCol.Where(test => test.TestName == testName);
                            if (testResultCol != null && testResultCol.Count() > 0)
                            {
                                drTest["Agregar"] = true;
                                _unitTest.Add(testResultCol.First());
                            }
                            else
                            {
                                drTest["Agregar"] = false;
                            }

                            _testCol.Rows.Add(drTest);
                        }
                    }

                    grdTest.DataSource = _testCol;

                    grdTestView.Columns[@"Prueba"].OptionsColumn.AllowEdit = false;
                    grdTestView.Columns[@"Modelo"].OptionsColumn.AllowEdit = false;
                    grdTestView.Columns[@"Descripción"].OptionsColumn.AllowEdit = false;
                }
                else
                {
                    WoDirectory.CreateDirectory(unitTestPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la carga de las pruebas. {ex.Message}");
            }
        }

        #endregion Carga de pruebas


        #region Gestión de la pruebas unitarias internas

        /// <summary>
        /// Lista de las pruebas integrales que contiene la prueba unitaria
        /// </summary>
        private List<WoTestEjecutionProperties> _unitTest = new List<WoTestEjecutionProperties>();

        /// <summary>
        /// Agregamos las pruebas seleccionadas a la lista de test de la configuración de la integral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                dynamic x = grdTestView.DataSource;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al intentar actualizar el fichero con la data de la integral. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Agrega a la lista las pruebas que se ban seleccionando
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdTestView_CellValueChanging(
            object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e
        )
        {
            try { }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Ocurrió un error al agregar la prueba. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Gestión de la pruebas unitarias internas


        #region Cierre del formulario

        /// <summary>
        /// Cierre del formulario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion Cierre del formulario
    }
}
