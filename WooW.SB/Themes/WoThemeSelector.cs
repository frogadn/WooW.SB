using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;
using WooW.SB.Themes.ThemeHelpers;

namespace WooW.SB.Themes
{
    public partial class WoThemeSelector : UserControl
    {
        #region Instancias singleton

        /// <summary>
        /// Datos del proyecto
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        private WoLogObserver _log = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Ruta de los temas.
        /// </summary>
        private string _pathThemes = string.Empty;

        private WoThemeAnalizer _woThemeAnalizer = new WoThemeAnalizer();

        #endregion Atributos

        public WoThemeSelector()
        {
            InitializeComponent();

            _pathThemes = $@"{_project.DirProyectData}/LayOuts/Themes";

            InstanceLabel();

            ChargeCombo();
        }

        #region Theme selected

        private string _themeSelected = "Default";

        public void SetTheme(string theme)
        {
            _themeSelected = theme;

            if (_themesCol.Contains(_themeSelected))
            {
                cmbThemes.SelectedItem = _themeSelected;
            }
            else
            {
                //Todo: send exception de que el tema no se encontró.
                cmbThemes.SelectedItem = "Seleccione...";
            }
        }

        #endregion Theme selected

        #region Combo

        public EventHandler<string> ApliedThemeEvt;

        private List<string> _themesCol = new List<string>();

        private void ChargeCombo(string themeSelected = "Seleccione...")
        {
            _themesCol.Clear();
            _themesCol.Add("Seleccione...");

            List<string> filesCol = WoDirectory.ReadDirectoryFiles(_pathThemes);
            foreach (string file in filesCol)
            {
                string[] fileCol = file.Split('\\');
                string fileName = fileCol[fileCol.Length - 1];
                string fileExtension = fileName.Substring(fileName.Length - 3);

                if (fileExtension.ToLower() == "css")
                {
                    _themesCol.Add(fileName.Substring(0, fileName.Length - 4));
                }
            }

            cmbThemes.Properties.Items.Clear();
            cmbThemes.Properties.Items.AddRange(_themesCol);

            if (_themesCol.Contains(_themeSelected))
            {
                cmbThemes.SelectedItem = _themeSelected;
            }
            else
            {
                //Todo: send exception de que el tema no se encontró.
                cmbThemes.SelectedItem = "Seleccione...";
            }
            ChargeHtml();
        }

        #endregion Combo

        #region Carga del css

        bool webRenderComplete = false;

        private void ChargeHtml()
        {
            string pathPreview = $@"{_pathThemes}/PreviewWithTheme.html";

            string htmlPreview = WoDirectory.ReadFile($@"{_pathThemes}/Preview.html");
            string[] htmlPreviewCol = htmlPreview.Split($@"<Referencia al tema seleccionado>");

            string[] htmlBodyPreview = htmlPreviewCol[1].Split("<Nombre del tema>");

            string themeReference = $@"<link href=""{_themeSelected}.css"" rel=""stylesheet"" />";
            WoDirectory.WriteFile(
                pathPreview,
                $@"{htmlPreviewCol[0]} {themeReference} {htmlBodyPreview[0]} {_themeSelected} {htmlBodyPreview[1]}"
            );

            wvwThemePreview.Source = new Uri(pathPreview);
            wvwThemePreview.EnsureCoreWebView2Async();
            wvwThemePreview.CoreWebView2InitializationCompleted += (s, a) =>
            {
                webRenderComplete = true;
            };
            if (webRenderComplete)
            {
                wvwThemePreview.Reload();
            }
        }

        private void cmbThemes_SelectedValueChanged(object sender, EventArgs e)
        {
            _themeSelected = cmbThemes.Text;
            if (
                _themeSelected == null
                || _themeSelected == string.Empty
                || _themeSelected == "Seleccione..."
            )
                _themeSelected = "Default";

            _woThemeAnalizer.ReadCss(_themeSelected);
            ChargeHtml();

            ApliedThemeEvt?.Invoke(this, _themeSelected);
        }

        private void cmbThemes_TextChanged(object sender, EventArgs e)
        {
            _themeSelected = cmbThemes.Text;
            if (
                _themeSelected == null
                || _themeSelected == string.Empty
                || _themeSelected == "Seleccione..."
            )
                _themeSelected = "Default";

            _woThemeAnalizer.ReadCss(_themeSelected);
            ChargeHtml();

            ApliedThemeEvt?.Invoke(this, _themeSelected);
        }

        #endregion Carga del css

        #region Cargar template

        private WoThemeValidator _themeValidator = new WoThemeValidator();

        private void btnLoadTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Configurar el diálogo
            openFileDialog.Title = "Seleccionar archivo";
            openFileDialog.Filter =
                "Plantillas de bootstrap (*.css)|*.css|Todos los archivos (*.*)|*.*";
            openFileDialog.Multiselect = true;
            // Mostrar el diálogo y comprobar si el usuario hizo clic en el botón "Aceptar"
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Obtener la ruta del archivo seleccionado
                string[] files = openFileDialog.FileNames;

                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string fileData = WoDirectory.ReadFile(file);
                    string extension = Path.GetExtension(file);

                    if (extension != ".css")
                    {
                        MessageBox.Show("No es un archivo de bootstrap");
                        return;
                    }
                    if (!_themeValidator.VerifyBoostrap(fileData))
                    {
                        MessageBox.Show("No es un archivo de bootstrap");
                        return;
                    }

                    if (!File.Exists($@"{_pathThemes}\{fileName}"))
                    {
                        WoDirectory.WriteFile($@"{_pathThemes}/{fileName}", fileData);
                    }
                    else
                    {
                        MessageBox.Show("ya existe");
                        if (
                            XtraMessageBox.Show(
                                " El tema ya existe,¿desea remplazarlo? ",
                                this.Text,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1
                            ) == DialogResult.Yes
                        )
                        {
                            WoDirectory.WriteFile($@"{_pathThemes}/{fileName}", fileData);
                        }
                    }
                }

                ChargeCombo(Path.GetFileName(files[0]).Replace(".css", ""));
            }
            else
            {
                Console.WriteLine("Ningún archivo seleccionado");
            }
        }

        #endregion Cargar template

        #region label de carga del css

        private void InstanceLabel()
        {
            this.Text = "Arrastrar y almacenar archivo";
            this.AllowDrop = true;
            Label dropPictureBox = new Label();
            dropPictureBox.Text = "Arrastre aqui";
            dropPictureBox.ForeColor = System.Drawing.Color.White;
            dropPictureBox.TextAlign = ContentAlignment.MiddleCenter;
            dropPictureBox.Dock = DockStyle.Fill;
            dropPictureBox.AllowDrop = true;
            dropPictureBox.DragEnter += DropLabel_DragEnter;
            dropPictureBox.DragDrop += DropLabel_DragDrop;

            pnlDropTemplate.BackColor = System.Drawing.Color.Gray;
            pnlDropTemplate.Controls.Add(dropPictureBox);
        }

        private void DropLabel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void DropLabel_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string fileData = WoDirectory.ReadFile(file);

                //BootstrapValidator validator = new BootstrapValidator();
                string extension = Path.GetExtension(file);
                if (extension != ".css")
                {
                    MessageBox.Show("No es un archivo de bootstrap");
                    return;
                }
                if (!_themeValidator.VerifyBoostrap(fileData))
                {
                    MessageBox.Show("No es un archivo de bootstrap");
                    return;
                }
                if (!File.Exists($@"{_pathThemes}\{fileName}"))
                {
                    WoDirectory.WriteFile($@"{_pathThemes}/{fileName}", fileData);
                }
                else
                {
                    MessageBox.Show("ya existe");
                    if (
                        XtraMessageBox.Show(
                            " El tema ya existe,¿desea remplazarlo? ",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes
                    )
                    {
                        WoDirectory.WriteFile($@"{_pathThemes}/{fileName}", fileData);
                    }
                }
            }

            ChargeCombo(Path.GetFileName(files[0]).Replace(".css", ""));
        }

        #endregion label de carga del css

        #region Eliminar tema

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cmbThemes.Text.ToLower() != "default" && cmbThemes.Text != "Seleccione...")
            {
                if (
                    XtraMessageBox.Show(
                        "¿Desea eliminar el tema?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) == DialogResult.Yes
                )
                {
                    WoDirectory.DeleteFile(_pathThemes + "\\" + cmbThemes.Text + ".css");
                    MessageBox.Show(
                        "El tema fue eliminado.",
                        "Eliminar tema.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
                    ChargeCombo();
                }
            }
            else
            {
                return;
            }
        }

        #endregion Eliminar tema
    }
}
