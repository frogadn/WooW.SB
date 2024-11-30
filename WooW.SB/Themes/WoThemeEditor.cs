using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;
using WooW.SB.Themes.ThemeModels;

namespace WooW.SB.Themes
{
    public partial class WoThemeEditor : UserControl
    {
        #region Instancias singleton

        /// <summary>
        /// Datos de todo el proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        /// <summary>
        /// Observador principal.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Elemento bindeado al property.
        /// </summary>
        private WoStyleProperties _woStyleProperties = new WoStyleProperties();

        /// <summary>
        /// Ruta de salvado de los temas.
        /// </summary>
        private string _themesRoute = string.Empty;

        #endregion Atributos

        #region Constructor

        public WoThemeEditor()
        {
            InitializeComponent();

            propThemeEditor.SelectedObject = _woStyleProperties;
            _themesRoute = $@"{_project.DirProyectData}/Themes";

            ChargeCombo();
        }

        #endregion Constructor

        #region Datos del combo

        private List<string> _themesCol = new List<string>();

        private void ChargeCombo()
        {
            _themesCol.Clear();
            _themesCol.Add("Seleccione...");

            List<string> filesCol = WoDirectory.ReadDirectoryFiles(_themesRoute);
            foreach (string file in filesCol)
            {
                string[] fileCol = file.Split('\\');
                string fileName = fileCol[fileCol.Length - 1];
                _themesCol.Add(fileName.Substring(0, fileName.Length - 5));
            }
            cmbThemes.Items.Clear();
            cmbThemes.Items.AddRange(_themesCol.ToArray());
            cmbThemes.SelectedItem = "Seleccione...";
        }

        #endregion Datos del combo

        #region Salvar y actualizar json

        private void btnSave_Click(object sender, EventArgs e)
        {
            string themeName =
                (cmbThemes.SelectedItem == null)
                    ? cmbThemes.Text
                    : cmbThemes.SelectedItem.ToString();

            if (themeName != null && themeName != string.Empty)
            {
                if (File.Exists($@"{_themesRoute}/{themeName}.json"))
                {
                    WoDirectory.DeleteFile($@"{_themesRoute}/{themeName}.json");
                }
                string themeJson = JsonConvert.SerializeObject(_woStyleProperties);
                WoDirectory.WriteFile($@"{_themesRoute}/{themeName}.json", themeJson);
            }

            _themesCol.Add(themeName);
            ChargeCombo();
            cmbThemes.SelectedItem = themeName;
        }

        public EventHandler<WoStyleProperties> ApliThemeEvt { get; set; }

        private void btnAply_Click(object sender, EventArgs e)
        {
            if (ApliThemeEvt != null)
            {
                ApliThemeEvt.Invoke(this, _woStyleProperties);
            }
        }

        #endregion Salvar y actualizar json

        #region Asignación al property

        public void SetSelectedObject(WoStyleProperties woStyleProperties)
        {
            _woStyleProperties = woStyleProperties;
        }

        #endregion Asignación al property

        #region Eventos del combo

        /// <summary>
        /// Para cuando se cambia el valor del combo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbThemes_SelectedValueChanged(object sender, EventArgs e)
        {
            string themeSelected = cmbThemes.SelectedItem.ToString();
            if (File.Exists($@"{_themesRoute}/{themeSelected}.json"))
            {
                string json = WoDirectory.ReadFile($@"{_themesRoute}/{themeSelected}.json");
                _woStyleProperties = JsonConvert.DeserializeObject<WoStyleProperties>(json);
            }
            else
            {
                _woStyleProperties = new WoStyleProperties();
            }
            propThemeEditor.SelectedObject = _woStyleProperties;
        }

        /// <summary>
        /// Para cuando se le da el foco al combo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbThemes_Enter(object sender, EventArgs e)
        {
            if (cmbThemes.Text == "Seleccione...")
            {
                cmbThemes.Text = string.Empty;
            }
        }

        /// <summary>
        /// Para cuando se le quita el foco al combo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbThemes_Leave(object sender, EventArgs e)
        {
            if (cmbThemes.Text == string.Empty)
            {
                cmbThemes.SelectedItem = "Seleccione...";
            }
        }

        #endregion Eventos del combo
    }
}
