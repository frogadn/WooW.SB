using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace WooW.SB.Config.Editors
{
    public partial class woConsole : DevExpress.XtraEditors.XtraUserControl
    {

        #region Propiedades

        /// <summary>
        /// Nombre del grupo de opciones.
        /// Para cuando se instancian multiples consolas
        /// </summary>
        public string RibbonGroupName = "Consola";

        #endregion Propiedades


        #region Constructor

        /// <summary>
        /// Constructor principal de la consola
        /// </summary>
        public woConsole()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Ribbon

        /// <summary>
        /// Retornamos el ribbon para que se haga merge y lo oculta en el componente
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public RibbonControl GetRibbon()
        {
            try
            {
                ribbonControl1.Hide();
                return ribbonControl1;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al intentar recuperar el ribbon. {ex.Message}");
            }
        }

        #endregion Ribbon


        #region Envió de información

        /// <summary>
        /// Envió de data a la consola
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public void SendData(string data)
        {
            try
            {
                consoleData.Invoke(
                    new Action(() =>
                    {
                        consoleData.Font = new Font("Consolas", 12, System.Drawing.FontStyle.Regular);

                        if (
                            (data.Contains("Building") || data.Contains("building"))
                            && ShowMessages)
                        {
                            consoleData.SelectionColor = System.Drawing.Color.LightBlue;

                            consoleData.AppendText($"{data}\n\r");
                            consoleData.ScrollToCaret();
                        }
                        else if (
                            (data.Contains("Warning") || data.Contains("warning"))
                            && ShowWarnings)
                        {
                            consoleData.SelectionColor = System.Drawing.Color.Yellow;

                            consoleData.AppendText($"{data}\n\r");
                            consoleData.ScrollToCaret();
                        }
                        else if (
                            (data.Contains("Error")
                            || data.Contains("error")
                            || data.Contains("Stop"))
                            && ShowErrors
                        )
                        {
                            consoleData.SelectionColor = System.Drawing.Color.Red;

                            consoleData.AppendText($"{data}\n\r");
                            consoleData.ScrollToCaret();
                        }
                        else if (
                            (data.Contains("Info") || data.Contains("info"))
                            && ShowMessages)
                        {
                            consoleData.SelectionColor = System.Drawing.Color.Green;

                            consoleData.AppendText($"{data}\n\r");
                            consoleData.ScrollToCaret();
                        }
                        else if (
                            (data.Contains("Success") || data.Contains("success"))
                            && ShowSuccess)
                        {
                            consoleData.SelectionColor = System.Drawing.Color.Green;

                            consoleData.AppendText($"{data}\n\r");
                            consoleData.ScrollToCaret();
                        }
                        else if (ShowMessages)
                        {
                            consoleData.SelectionColor = System.Drawing.Color.White;

                            consoleData.AppendText($"{data}\n\r");
                            consoleData.ScrollToCaret();
                        }
                    }
                ));
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al enviar data a la consola. {ex.Message}");
            }
        }

        #endregion Envió de información


        #region Clean console

        /// <summary>
        /// Limpiamos la consola desde el ribbon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnClearConsole_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                CleanConsole();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Error al limpiar la consola. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Limpia la consola
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public void CleanConsole()
        {
            try
            {
                consoleData.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al limpiar la consola. {ex.Message}");
            }
        }

        #endregion Clean console


        #region Gestión de los mensajes mostrados

        /// <summary>
        /// Bandera para indicar si se muestran los warnings
        /// </summary>
        private bool ShowWarnings = true;

        /// <summary>
        /// Cambio del valor de la bandera para mostrar los warnings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void bciShowWarnings_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ShowWarnings = !ShowWarnings;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Error al cambiar la configuración de la consola para las alertas. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private bool ShowErrors = true;

        [SupportedOSPlatform("windows")]
        private void bciErrores_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ShowErrors = !ShowErrors;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Error al cambiar la configuración de la consola para los errores. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private bool ShowMessages = true;

        [SupportedOSPlatform("windows")]
        private void bciShowMessages_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ShowMessages = !ShowMessages;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Error al cambiar la configuración de la consola para los mensajes. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private bool ShowSuccess = true;

        [SupportedOSPlatform("windows")]
        private void bciShowSuccess_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ShowSuccess = !ShowSuccess;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Error al cambiar la configuración de la consola para los mensajes de éxito. {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Gestión de los mensajes mostrados


    }
}

