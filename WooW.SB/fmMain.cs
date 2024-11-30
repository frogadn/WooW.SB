using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
//using Microsoft.Win32;
using ServiceStack;
using ServiceStack.OrmLite;
using WooW.Core;
using WooW.Core.Common;
using WooW.Core.Common.Observer.LogFiles;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.AlertsAndLog.Alerts;
using WooW.SB.Config;
using WooW.SB.Config.Class;
using WooW.SB.Config.Helpers;
using WooW.SB.Dialogs;
using WooW.SB.Forms;
using WooW.SB.FormsAux;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

// ToDo Control de versiones
// ToDo Control trabajar con Git

namespace WooW.SB
{
    internal delegate void AsignarPropiedad(object Propiedad);

    public partial class fmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Proyecto proyecto;
        public WooWConfigParams wooWConfigParams { get; set; }

        private List<IForm> Forms = new List<IForm>();

        private string ArchivoProyecto = string.Empty;

        private UserControl userBringToFront = null;

        private string[] SavedArgs;

        public Process processVisualStudio = null;

        private bool NoSaveProyect = false;

        private string keyName = @"HKEY_CURRENT_USER\SOFTWARE\FROGadn\WooW\";

        /// <summary>
        /// Observador del log.
        /// </summary>
        private WoLogObserver observer = WoLogObserver.GetInstance();

        public fmMain(string[] args)
        {
            WoLicence.Set();

            SavedArgs = args;

            UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXICompact);
            //UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.WXICompact.Darkness);

            InitializeComponent();

            InitializeSyntaxEditor();

            proyecto = Proyecto.getInstance();
            wooWConfigParams = WooWConfigParams.getInstance();

            /// Clase con el funcionamiento para generar los ficheros en la ruta establecida con
            /// con la información que se envié en los logs.
            WoFilesLog woFilesLog = new WoFilesLog();
            observer.SetNewLogEvt += woFilesLog.Write;

            /// Clase ocupada de gestionar las alertas.
            WoAlertManager woAlertManager = new WoAlertManager();
            observer.SetNewLogEvt += woAlertManager.ShowAlert;
        }

        private static void InitializeSyntaxEditor()
        {
            //FileStream fs = File.OpenRead(
            //    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            //        + "\\Dark.vssettings"
            //);
            //AmbientHighlightingStyleRegistry.Instance.ImportHighlightingStyles(fs);

            //logger?.LogInformation("Configuring SyntaxEditor ...");

            // If using SyntaxEditor with languages that support syntax/semantic parsing, use this line at
            //   app startup to ensure that worker threads are used to perform the parsing
            ActiproSoftware
                .Text
                .Parsing
                .AmbientParseRequestDispatcherProvider
                .Dispatcher =
                new ActiproSoftware.Text.Parsing.Implementation.ThreadedParseRequestDispatcher();

            //logger?.LogDebug("AmbientParseRequestDispatcherProvider.Dispatcher = {0}", ActiproSoftware.Text.Parsing.AmbientParseRequestDispatcherProvider.Dispatcher?.GetType().FullName ?? "NULL");

            // Create SyntaxEditor .NET Languages Add-on ambient assembly repository, which supports caching of
            //   reflection data and improves performance for the add-on...
            //   Be sure to replace the appDataPath with a proper path for your own application (if file access is allowed)
            var appDataPath = Path.Combine(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Actipro Software"
                ),
                "WinForms SampleBrowser Assembly Repository"
            );
            ActiproSoftware
                .Text
                .Languages
                .DotNet
                .Reflection
                .AmbientAssemblyRepositoryProvider
                .Repository =
                new ActiproSoftware.Text.Languages.DotNet.Reflection.Implementation.FileBasedAssemblyRepository(
                    appDataPath
                );

            //logger?.LogDebug("Python Package Repository Path = {0}", appDataPath);
        }

        //###CODIGO MODIFICADO --------------------------------------------
        UserControl userForm = null;

        //###CODIGO MODIFICADO --------------------------------------------

        private UserControl InitUserForm(Type TypeOfForm)
        {
            Cursor.Current = Cursors.WaitCursor;

            //###CODIGO MODIFICADO --------------------------------------------
            userForm = null;
            //###CODIGO MODIFICADO --------------------------------------------

            try
            {
                foreach (Control ctrl in this.Controls)
                    if (ctrl.GetType() == TypeOfForm)
                        userForm = (UserControl)ctrl;

                this.SuspendLayout();

                if (userForm.IsNull())
                {
                    userForm = (UserControl)Activator.CreateInstance(TypeOfForm);
                    userForm.Dock = DockStyle.Fill;
                    this.Controls.Add(userForm);
                    Forms.Add((IForm)userForm);

                    //###CODIGO MODIFICADO --------------------------------------------

                    if (TypeOfForm == typeof(fmTestCases))
                    {
                        ((fmTestCases)userForm).UpdateRibbonEvt += UpdateRibbon;
                    }

                    //###CODIGO MODIFICADO --------------------------------------------

                    if (userForm is IForm)
                        (userForm as IForm).proyecto = this.proyecto;

                    ((IForm)userForm).wooWConfigParams = wooWConfigParams;

                    ((IForm)userForm).Cargar();

                    DevExpress.XtraBars.BarButtonItem barItem =
                        new DevExpress.XtraBars.BarButtonItem();
                    barItem.Caption = ((IForm)userForm).Nombre;
                    barItem.Tag = userForm;
                    barItem.ItemClick += BarItem_ItemClick;
                    mnuVentanas.AddItem(barItem);
                    mnuVentanas2.AddItem(barItem);
                }
                userForm.SuspendLayout();
                this.ribbonControl1.MergeRibbon(((IForm)userForm).CurrentRibbon);
                this.ribbonControl1.SelectedPage = this.ribbonControl1.MergedPages[0];

                userForm.BringToFront();
                userBringToFront = userForm;

                userForm.ResumeLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return userForm;
        }

        //###CODIGO MODIFICADO --------------------------------------------
        [SupportedOSPlatform("windows")]
        private void UpdateRibbon()
        {
            this.SuspendLayout();

            this.ribbonControl1.UnMergeRibbon();
            this.ribbonControl1.MergeRibbon(((IForm)userForm).CurrentRibbon);
            this.ribbonControl1.SelectedPage = this.ribbonControl1.MergedPages[0];

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        //###CODIGO MODIFICADO --------------------------------------------

        private void BarItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (e.Item is DevExpress.XtraBars.BarButtonItem)
            {
                DevExpress.XtraBars.BarButtonItem barItem = (DevExpress.XtraBars.BarButtonItem)
                    e.Item;
                if (barItem.Tag is UserControl)
                {
                    InitUserForm(barItem.Tag.GetType());
                }
            }
        }

        private void CloseUserForm(Type TypeOfForm)
        {
            UserControl userForm = null;

            foreach (Control ctrl in this.Controls)
                if (ctrl.GetType() == TypeOfForm)
                    userForm = (UserControl)ctrl;

            if (!userForm.IsNull())
            {
                userForm.Dispose();
                this.Controls.Remove(userForm);
                userForm = null;
            }
        }

        private bool bCambiosAlProyecto()
        {
            string sCambiosPendientes = CambiosPendientes();

            if (!string.IsNullOrEmpty(sCambiosPendientes))
            {
                XtraMessageBox.Show(
                    $"Tiene cambios por aplicar en: {sCambiosPendientes}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return true;
            }

            return false;
        }

        private void buPropiedades_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmProyectProperties));
        }

        private void buEtiquetas_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmLabels));
        }

        private void buErrores_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmNumberedExceptions));
        }

        private void buPruebas_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmTestCasesOld));
        }

        private void buMenus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmMenus));
        }

        private void buModelos_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmModel));
        }

        private void buModelDTO_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!VerificaUltimaCompilacion())
                return;

            InitUserForm(typeof(fmModelPrePost));
        }

        private void buModelosLogic_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmModelStateDiagram));
        }

        private void buModeloLayOut_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmModelLayOut));
        }

        // ToDo Revisar lo de crear nuevo (parte de directorio etc.)
        private void buNuevo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var fmNuevo = new fmProyectoNuevo();
            fmNuevo.Origen = wooWConfigParams.Origen;
            if (fmNuevo.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                wooWConfigParams.OrigenDiferenteSoloLectura =
                    PackageHelper.ValidaNombreOrigenYSoloLectura(
                        fmNuevo.ArchivoProyecto,
                        wooWConfigParams.Origen
                    );

                if (wooWConfigParams.OrigenDiferenteSoloLectura)
                    throw new Exception(
                        $"El nombre del proyecto {fmNuevo.ArchivoProyecto} debe comenzar con {wooWConfigParams.Origen}"
                    );
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            ArchivoProyecto = fmNuevo.ArchivoProyecto;

            Proyecto.Clear();
            proyecto = Proyecto.getInstance();
            CargarProyecto(ArchivoProyecto);
        }

        private void buAbrir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bCambiosAlProyecto())
            {
                return;
            }

            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Proyecto WooW Service Builder (*.wwsb)|*.wwsb";
                openDialog.FileName = string.Empty;
                openDialog.RestoreDirectory = true;

                string valueName = @"LastOpenDir";
                object LastDir;
                if ((LastDir = Microsoft.Win32.Registry.GetValue(keyName, valueName, null)) != null)
                {
                    if (Directory.Exists(LastDir.ToSafeString()))
                        openDialog.InitialDirectory = LastDir.ToSafeString();
                }

                if (openDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    wooWConfigParams.OrigenDiferenteSoloLectura =
                        PackageHelper.ValidaNombreOrigenYSoloLectura(
                            openDialog.FileName,
                            wooWConfigParams.Origen
                        );
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
                {
                    var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                        @"SOFTWARE\FROGadn\WooW"
                    );
                    key.SetValue(valueName, Path.GetDirectoryName(openDialog.FileName));
                }
                else
                {
                    var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        @"SOFTWARE\FROGadn\WooW",
                        true
                    );
                    key.SetValue(valueName, Path.GetDirectoryName(openDialog.FileName));
                }

                ArchivoProyecto = openDialog.FileName;

                // Procesa ultimos archivos abiertos
                valueName = @"ListFiles";
                string ListFiles = Microsoft
                    .Win32.Registry.GetValue(keyName, valueName, null)
                    .ToSafeString();

                string[] Files = ListFiles.Split(';');

                // El arreglo debe ser menor a 20 elementos y se agrega al principio ArchivoProyecto
                ListFiles = ArchivoProyecto + ";";
                int Elementos = (Files.Length > 26 ? 26 : Files.Length);
                for (int i = 0; i < Elementos; i++)
                {
                    if (Files[i] == ArchivoProyecto)
                        continue;
                    if (File.Exists(Files[i]))
                        ListFiles += Files[i] + ";";
                }

                if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
                {
                    var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                        @"SOFTWARE\FROGadn\WooW"
                    );
                    key.SetValue(valueName, ListFiles);
                }
                else
                {
                    var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        @"SOFTWARE\FROGadn\WooW",
                        true
                    );
                    key.SetValue(valueName, ListFiles);
                }
            }

            Proyecto.Clear();
            proyecto = Proyecto.getInstance();
            CargarProyecto(ArchivoProyecto);
        }

        private void CopiaEnsamblados(Proyecto principal)
        {
            string Drive = Path.GetPathRoot(principal.DirApplication).Substring(0, 2);

            string Proceso = $"{principal.DirApplication}\\Copy.Build.WooW.bat";

            if (!File.Exists(Proceso))
            {
                XtraMessageBox.Show(
                    "Copie nuevamente la plantilla",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            string Parametros =
                $"{Drive} \"{principal.DirApplication}\" \"{Application.StartupPath}\"";
            ProcessStartInfo startInfo = new ProcessStartInfo(Proceso, Parametros);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true; //No utiliza RunDLL32 para lanzarlo   //Opcional: establecer la carpeta de trabajo en la que se ejecutará el proceso   //startInfo.WorkingDirectory = "C:\\MiCarpeta\\";
            //Redirige las salidas y los errores
            //startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            Process proc = Process.Start(startInfo); //Ejecuta el proceso
            proc.WaitForExit(); // Espera a que termine el proceso
        }

        private string GetEtiqueta(string EtiquetaId)
        {
            if (EtiquetaId.IsNullOrStringEmpty())
                return string.Empty;

            var Etiquetas = proyecto
                .EtiquetaCol.Etiquetas.Where(x => x.Id == EtiquetaId)
                .FirstOrDefault();

            if (Etiquetas.IsNull())
                return EtiquetaId;

            var Idiomas = Etiquetas
                .Idiomas.Where(x => x.IdiomaId == proyecto.esMX)
                .FirstOrDefault();

            if (Idiomas.IsNull())
                return EtiquetaId;

            return Idiomas.Texto;
        }

        private void CargarProyecto(string ArchivoProyecto)
        {
            try
            {
                PaquetesVerifica.RecursivosYModelosRepetidos(ArchivoProyecto);
                proyecto.Load(ArchivoProyecto);
                this.ArchivoProyecto = ArchivoProyecto;
                MensajeSoloLectura();
            }
            catch (Exception ex)
            {
                Proyecto.Clear();
                proyecto = Proyecto.getInstance();

                XtraMessageBox.Show(
                    ex.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Valida el origen tomando los primeros 2 caracteres del archivo
            string locOrigen = Path.GetFileName(proyecto.ArchivoDeProyecto).Substring(0, 2);
            string mensajes = proyecto.ValidaOrigen(locOrigen);
            if (!mensajes.IsNullOrEmpty())
                XtraMessageBox.Show(
                    mensajes,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

            NoSaveProyect = true;
            try
            {
                txtUsuario.EditValue = proyecto.ParConexion.Usuario;
                txtPassword.EditValue = proyecto.ParConexion.Password;
                txtUDN.EditValue = proyecto.ParConexion.Udn;
                txtInstancia.EditValue = proyecto.ParConexion.Instance;
                txtTipoInstancia.EditValue = proyecto.ParConexion.InstanceType.ToString();
                txtBaseDatos.EditValue = proyecto.ParConexion.InstanceDataBase;
                txtAño.EditValue = proyecto.ParConexion.Year;
                txtBaseDatosHibrida.EditValue = proyecto.ParConexion.DbApp;

                repTestApp.Items.Clear();
                repTestApp.Items.Add(string.Empty);
                foreach (var app in proyecto.Apps)
                {
                    if ((app.Id == "WebService") || (app.Id == "WebClient"))
                        continue;
                    repTestApp.Items.Add(app.Id);
                }

                txtTestApp.EditValue = proyecto.ParConexion.UnitTestApp;

                buBuild.Enabled = true;
                ribbonElementos.Enabled = true;
                ribbonMensajes.Enabled = true;
                ribDiseno.Enabled = true;
                buPropiedades.Enabled = true;
                buPackageManager.Enabled = true;
                buComparar.Enabled = true;
                buParametros.Enabled = true;
                buClose.Enabled = true;

                buArchivo.Enabled = false;

                if (proyecto.FechaCompilacion != DateTime.MinValue)
                    CopiaEnsamblados(proyecto);
            }
            finally
            {
                NoSaveProyect = false;
            }
        }

        public string CambiosPendientes()
        {
            string sCambiosPendientes = string.Empty;

            foreach (var Form in Forms)
            {
                if (Form.CambiosPendientes)
                {
                    if (!string.IsNullOrEmpty(sCambiosPendientes))
                        sCambiosPendientes += ", ";
                    sCambiosPendientes += Form.Nombre;
                }
            }

            return sCambiosPendientes;
        }

        private void fmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            string sCambiosPendientes = CambiosPendientes();

            if (!string.IsNullOrEmpty(sCambiosPendientes))
            {
                if (
                    XtraMessageBox.Show(
                        $"Tiene cambios pendientes en {sCambiosPendientes}\r\n\r\nDescartar los cambios y cerrar la aplicación?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes
                )
                {
                    e.Cancel = true;
                    return;
                }
            }

            //var dispatcher = ActiproSoftware
            //    .Text
            //    .Parsing
            //    .AmbientParseRequestDispatcherProvider
            //    .Dispatcher;
            //if (dispatcher != null)
            //{
            //    ActiproSoftware.Text.Parsing.AmbientParseRequestDispatcherProvider.Dispatcher =
            //        null;
            //    dispatcher.Dispose();
            //}

            //// Prune any SyntaxEditor .NET Languages Add-on cache data that is no longer valid
            //var repository = ActiproSoftware
            //    .Text
            //    .Languages
            //    .DotNet
            //    .Reflection
            //    .AmbientAssemblyRepositoryProvider
            //    .Repository;
            //if (repository != null)
            //    repository.PruneCache();



            proyecto.ParConexion.CierraServicio();
        }

        private void buGenerarModelos_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmModelMaker));
        }

        private void buScripts_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!VerificaUltimaCompilacion())
                return;

            InitUserForm(typeof(fmModelScript));
        }

        private void buReportesLayOut_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmReport));
        }

        private void buEstiloLibreLayOut_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmEstiloLibre));
        }

        public void AbreServicio()
        {
            buServicio.PerformClick();
        }

        private void InvokeInModelMaker(string Method)
        {
            UserControl userFront = userBringToFront;

            if (!buBuild.Enabled)
                return;

            UserControl userForm = null;

            foreach (Control ctrl in this.Controls)
                if (ctrl.GetType() == typeof(fmModelMaker))
                    userForm = (UserControl)ctrl;

            if (userForm == null)
                userForm = InitUserForm(typeof(fmModelMaker));

            Application.DoEvents();

            MethodInfo methodInfo = userForm.GetType().GetMethod(Method);
            methodInfo.Invoke(userForm, null);

            Application.DoEvents();

            if (userFront != null)
                InitUserForm(userFront.GetType());
        }

        private void buTraduccion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmModelTranslate));
        }

        public bool CambiosPendientesLogicaOScript(out string sCambiosPendientes)
        {
            sCambiosPendientes = string.Empty;

            foreach (var Form in Forms)
            {
                if (Form is fmModel)
                {
                    if (Form.CambiosPendientes)
                    {
                        sCambiosPendientes = "Modelos";
                        return true;
                    }
                }
                if (Form is fmModelStateDiagram)
                {
                    if (Form.CambiosPendientes)
                    {
                        sCambiosPendientes = "Diagramas de Estado";
                        return true;
                    }
                }
                if (Form is fmModelPrePost)
                {
                    if (Form.CambiosPendientes)
                    {
                        sCambiosPendientes = "PreCondiciones/PostCondiciones";
                        return true;
                    }
                }
                if (Form is fmModelScript)
                {
                    if (Form.CambiosPendientes)
                    {
                        sCambiosPendientes = "Scripts";
                        return true;
                    }
                }
            }

            return false;
        }

        public bool RefrescarLogicaOScript()
        {
            string sCambiosPendientes = string.Empty;

            foreach (var Form in Forms)
            {
                if (
                    (Form is fmModel)
                    || (Form is fmModelPrePost)
                    || (Form is fmModelStateDiagram)
                    || (Form is fmModelScript)
                    || (Form is fmLabels)
                    || (Form is fmNumberedExceptions)
                    || (Form is fmModelTranslate)
                )
                    Form.Refrescar();
            }

            return false;
        }

        private void buCierraServicio_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string sCambiosPendientes = string.Empty;

            foreach (var Form in Forms)
            {
                if (
                    (Form is fmModelLayOut)
                    || (Form is fmReport)
                    || (Form is fmEstiloLibre)
                    || (Form is fmTestCasesOld)
                )
                {
                    if (Form.CambiosPendientes)
                    {
                        if (!string.IsNullOrEmpty(sCambiosPendientes))
                            sCambiosPendientes += ", ";
                        sCambiosPendientes += Form.Nombre;
                    }
                }
            }

            if (!string.IsNullOrEmpty(sCambiosPendientes))
            {
                XtraMessageBox.Show(
                    $"Tiene cambios por aplicar en: {sCambiosPendientes}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            proyecto.ParConexion.CierraServicio();

            if (processVisualStudio != null)
            {
                processVisualStudio.Kill(true);
                processVisualStudio = null;
            }

            buFormulariosLayOut.Enabled = false;
            buReportesLayOut.Enabled = false;
            buEstiloLibreLayOut.Enabled = false;
            buPruebas.Enabled = false;
            buConsultasAuxiliares.Enabled = false;
            buServicio.Enabled = true;
            buVisualStudio.Enabled = true;
            buCierraServicio.Enabled = false;
            buMenus.Enabled = false;
            buIntegrales.Enabled = false;

            txtUsuario.Enabled = true;
            txtPassword.Enabled = true;
            txtInstancia.Enabled = true;
            txtUDN.Enabled = true;
            txtAño.Enabled = true;
            txtBaseDatosHibrida.Enabled = true;
            txtTestApp.Enabled = true;
            txtTipoInstancia.Enabled = true;
            txtBaseDatos.Enabled = true;
            buPropiedadesServicio.Enabled = true;

            CloseUserForm(typeof(fmModelLayOut));
            CloseUserForm(typeof(fmReport));
            CloseUserForm(typeof(fmEstiloLibre));
            CloseUserForm(typeof(fmTestCasesOld));

            foreach (var Form in Forms)
            {
                if (
                    !(Form is fmModelLayOut)
                    && !(Form is fmModelLayOut)
                    && !(Form is fmReport)
                    && !(Form is fmEstiloLibre)
                    && !(Form is fmTestCasesOld)
                )
                {
                    this.ribbonControl1.MergeRibbon(((IForm)Form).CurrentRibbon);
                    this.ribbonControl1.SelectedPage = this.ribbonControl1.MergedPages[0];
                }
            }
        }

        private bool VerificaUltimaCompilacion()
        {
            // Lee el archivo LastComile.txt y lo compara con el archivo de proyecto
            // si es diferente debe compilarse de nuevo
            if (proyecto.RequiereCompilacion())
            {
                // Indica que ha cambiado el proyecto que si desea compilarlo
                if (
                    XtraMessageBox.Show(
                        "El proyecto ha cambiado, debe compilarlo de nuevo\r\n\r\rDesea hacerlo ahora?",
                        this.Text,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Information
                    ) == DialogResult.OK
                )
                {
                    string sCambiosPendientes = CambiosPendientes();
                    if (!string.IsNullOrEmpty(sCambiosPendientes))
                    {
                        XtraMessageBox.Show(
                            $"Para Generar y Compilar debe grabar los cambios en {sCambiosPendientes}",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );
                        return false;
                    }

                    proyecto.ParConexion.CierraServicio();
                    fmMain.Restart(proyecto.ArchivoDeProyecto, "-m");
                    return false;
                }
            }
            return true;
        }

        private void buServicio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!VerificaUltimaCompilacion())
                return;

            if (
                txtUsuario.EditValue.IsNullOrStringEmpty()
                || txtPassword.EditValue.IsNullOrStringEmpty()
                || txtInstancia.EditValue.IsNullOrStringEmpty()
                //|| txtUDN.EditValue.IsNullOrStringEmpty()
                || txtAño.EditValue.IsNullOrStringEmpty()
                //|| txtBaseDatosHibrida.EditValue.IsNullOrStringEmpty()
                //|| txtTestApp.EditValue.IsNullOrStringEmpty()
                || txtTipoInstancia.EditValue.IsNullOrStringEmpty()
                || txtBaseDatos.EditValue.IsNullOrStringEmpty()
            )
            {
                XtraMessageBox.Show(
                    "Faltan parámetros que registrar",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            string valueName = @"CadenaConexionSQL";
            string CadenaConexionSQL = Microsoft
                .Win32.Registry.GetValue(keyName, valueName, null)
                .ToSafeString();
            valueName = @"CadenaConexionMySQL";
            string CadenaConexionMySQL = Microsoft
                .Win32.Registry.GetValue(keyName, valueName, null)
                .ToSafeString();
            valueName = @"CadenaConexionPostgre";
            string CadenaConexionPostgre = Microsoft
                .Win32.Registry.GetValue(keyName, valueName, null)
                .ToSafeString();
            valueName = @"EncryptedSQLPassword";
            string EncryptedSQLPassword = Microsoft
                .Win32.Registry.GetValue(keyName, valueName, null)
                .ToSafeString();

            if (txtTipoInstancia.EditValue.ToSafeString() == "QAS")
            {
                string SqlDatabase = string.Empty;
                string ConnString = string.Empty;

                if (txtBaseDatos.EditValue.ToSafeString() == "QAS MySQL")
                {
                    ConnString = CadenaConexionMySQL;
                    SqlDatabase = "MySql";
                }
                else if (txtBaseDatos.EditValue.ToSafeString() == "QAS SQLServer")
                {
                    ConnString = CadenaConexionSQL;
                    SqlDatabase = "SqlServer";
                }
                else if (txtBaseDatos.EditValue.ToSafeString() == "QAS Postgre")
                {
                    ConnString = CadenaConexionPostgre;
                    SqlDatabase = "Postgres";
                }

                string jsonFile = Path.Combine(
                    Path.GetFullPath(proyecto.ArchivoDeProyecto),
                    proyecto.DirApplication_WebService_WebService,
                    "appsettings.json"
                );

                var jsonText = File.ReadAllText(jsonFile);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText);
                jsonObj["WooWSettings"]["SqlDatabase"] = SqlDatabase;
                jsonObj["WooWSettings"]["ConnString"] = ConnString;
                jsonObj["WooWSettings"]["PasswordDataBase"] = EncryptedSQLPassword;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(
                    jsonObj,
                    Newtonsoft.Json.Formatting.Indented
                );
                File.WriteAllText(jsonFile, output);
            }

            try
            {
                proyecto.ParConexion.CierraServicio();
                proyecto.ParConexion.IniciaServicio();

                buFormulariosLayOut.Enabled = true;
                buReportesLayOut.Enabled = true;
                buEstiloLibreLayOut.Enabled = true;
                buPruebas.Enabled = true;
                buConsultasAuxiliares.Enabled = true;
                buMenus.Enabled = true;
                buIntegrales.Enabled = true;

                buServicio.Enabled = false;
                buVisualStudio.Enabled = false;
                buCierraServicio.Enabled = true;

                txtUsuario.Enabled = false;
                txtPassword.Enabled = false;
                txtInstancia.Enabled = false;
                txtUDN.Enabled = false;
                txtAño.Enabled = false;
                txtBaseDatosHibrida.Enabled = false;
                txtTestApp.Enabled = false;
                txtTipoInstancia.Enabled = false;
                txtBaseDatos.Enabled = false;
                buPropiedadesServicio.Enabled = false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void txtUsuario_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.Usuario = txtUsuario.EditValue.ToSafeString();
            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void txtPassword_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.Password = txtPassword.EditValue.ToSafeString();
            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void txtInstancia_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.Instance = txtInstancia.EditValue.ToSafeString();
            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void txtBaseDatosHibrida_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.DbApp = txtBaseDatosHibrida.EditValue.ToSafeString();
            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void txtTipoInstancia_EditValueChanged(object sender, EventArgs e)
        {
            if (txtTipoInstancia.EditValue.ToSafeString() == "DEV")
                txtBaseDatos.EditValue = "DEV Sqlite";
            else
            {
                if (txtBaseDatos.EditValue.ToSafeString() == "DEV Sqlite")
                    txtBaseDatos.EditValue = "QAS MySQL";
            }

            proyecto.ParConexion.InstanceType = (
                txtTipoInstancia.EditValue.ToSafeString() == "QAS"
                    ? tWoIntanciaType.QAS
                    : tWoIntanciaType.DEV
            );

            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void txtAño_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.Year = txtAño.EditValue.ToInt16();
            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void txtUDN_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.Udn = txtUDN.EditValue.ToSafeString();
            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void fmMain_Shown(object sender, EventArgs e)
        {
            var valueName = @"CadenaConexionSQL";
            object CadenaConexionSQL;
            if (
                (CadenaConexionSQL = Microsoft.Win32.Registry.GetValue(keyName, valueName, null))
                != null
            )
                txtCadenaConexionSQL.EditValue = CadenaConexionSQL;
            else
                txtCadenaConexionSQL.EditValue =
                    "Server=localhost;Database={0};Trusted_Connection=True;";

            valueName = @"CadenaConexionMySQL";
            object CadenaConexionMySQL;
            if (
                (CadenaConexionMySQL = Microsoft.Win32.Registry.GetValue(keyName, valueName, null))
                != null
            )
                txtCadenaConexionMySQL.EditValue = CadenaConexionMySQL;
            else
                txtCadenaConexionMySQL.EditValue =
                    "Server=localhost;Database={0};Uid=root;Pwd=%Password%;";

            valueName = @"CadenaConexionPostgre";
            object CadenaConexionPostgre;
            if (
                (
                    CadenaConexionPostgre = Microsoft.Win32.Registry.GetValue(
                        keyName,
                        valueName,
                        null
                    )
                ) != null
            )
                txtCadenaConexionPostgre.EditValue = CadenaConexionPostgre;
            else
                txtCadenaConexionPostgre.EditValue =
                    "Server=localhost;Database={0};Port=5432;Uid=postgres;Pwd=%Password%;";

            valueName = @"EncryptedSQLPassword";
            object EncryptedSQLPassword;
            if (
                (EncryptedSQLPassword = Microsoft.Win32.Registry.GetValue(keyName, valueName, null))
                != null
            )
                txtDBPassword.EditValue = EncryptedSQLPassword;
            else
                txtDBPassword.EditValue = string.Empty;

            if (SavedArgs.Length > 0)
            {
                string ArchivoProyecto = SavedArgs[0];

                if (
                    (!File.Exists(ArchivoProyecto))
                    || (Path.GetExtension(ArchivoProyecto) != ".wwsb")
                )
                {
                    XtraMessageBox.Show(
                        $"Error en parámetro {ArchivoProyecto} el proyecto no existe",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                try
                {
                    wooWConfigParams.OrigenDiferenteSoloLectura =
                        PackageHelper.ValidaNombreOrigenYSoloLectura(
                            ArchivoProyecto,
                            wooWConfigParams.Origen
                        );
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                Proyecto.Clear();
                proyecto = Proyecto.getInstance();
                CargarProyecto(ArchivoProyecto);

                if (SavedArgs.Length > 1)
                {
                    if (SavedArgs[1].ToLower() == "-m")
                    {
                        InitUserForm(typeof(fmModelMaker));
                        InvokeInModelMaker("GenerarModelosYCompila");
                    }
                }
            }
        }

        public void OpenProyect(string ArchivoProyecto)
        {
            buClose_ItemClick(null, null);

            try
            {
                wooWConfigParams.OrigenDiferenteSoloLectura =
                    PackageHelper.ValidaNombreOrigenYSoloLectura(
                        ArchivoProyecto,
                        wooWConfigParams.Origen
                    );
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            Proyecto.Clear();
            proyecto = Proyecto.getInstance();
            CargarProyecto(ArchivoProyecto);
            InitUserForm(typeof(fmPackageManager));
        }

        private void MensajeSoloLectura()
        {
            if (wooWConfigParams.OrigenDiferenteSoloLectura)
                XtraMessageBox.Show(
                    $"El proyecto es diferente al origen {wooWConfigParams.Origen}, se abre como solo lectura",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

            string SoloLectura = (
                wooWConfigParams.OrigenDiferenteSoloLectura ? " (Solo Lectura)" : string.Empty
            );
            this.Text = $"WooW Service Builder - {ArchivoProyecto}{SoloLectura}";
        }

        public static void Restart(string ArchivoDeProyecto = "", string Arguments = "")
        {
            //ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
            //startInfo.FileName = Application.ExecutablePath;
            //var exit = typeof(Application).GetMethod(
            //    "ExitInternal",
            //    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            //);
            //exit.Invoke(null, null);
            //startInfo.Arguments = ArchivoDeProyecto + " " + Arguments;
            //Process.Start(startInfo);


            string exePath = Application.ExecutablePath;
            try
            {
                Application.Exit();
                wait_allowingEvents(1000);
            }
            catch (ArgumentException ex)
            {
                throw;
            }

            string commandLineArgs = ArchivoDeProyecto + " " + Arguments;

            Process.Start(exePath, commandLineArgs);
        }

        static void wait_allowingEvents(int durationMS)
        {
            DateTime start = DateTime.Now;
            do
            {
                Application.DoEvents();
            } while (start.Subtract(DateTime.Now).TotalMilliseconds > durationMS);
        }

        private void buVisualStudio_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string commandLineArgs =
                $"/run {proyecto.DirApplication_WebService}\\WooW.WebService.sln";

            processVisualStudio = woVisualStudio.AbreVisualStudio(commandLineArgs, false);

            buFormulariosLayOut.Enabled = true;
            buReportesLayOut.Enabled = true;
            buEstiloLibreLayOut.Enabled = true;
            buPruebas.Enabled = true;
            buConsultasAuxiliares.Enabled = true;
            buMenus.Enabled = true;
            buIntegrales.Enabled = true;

            buServicio.Enabled = false;
            buVisualStudio.Enabled = false;
            buCierraServicio.Enabled = true;

            txtUsuario.Enabled = false;
            txtPassword.Enabled = false;
            txtInstancia.Enabled = false;
            txtUDN.Enabled = false;
            txtAño.Enabled = false;
            txtBaseDatosHibrida.Enabled = false;
            txtTestApp.Enabled = false;
            txtTipoInstancia.Enabled = false;
            txtBaseDatos.Enabled = false;
            buPropiedadesServicio.Enabled = false;
        }

        private void repBaseDatosLocal_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                if (!Directory.Exists(proyecto.DirApplication_WebService_WooWServer_DBApp))
                    Directory.CreateDirectory(proyecto.DirApplication_WebService_WooWServer_DBApp);

                saveDialog.Filter = "Base datos sqlite (*.sqlite)|*.sqlite";
                saveDialog.FileName = string.Empty;
                saveDialog.InitialDirectory = proyecto.DirApplication_WebService_WooWServer_DBApp;
                saveDialog.RestoreDirectory = true;

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                if (
                    Path.GetDirectoryName(saveDialog.FileName).ToUpper()
                    != proyecto.DirApplication_WebService_WooWServer_DBApp.ToUpper()
                )
                {
                    XtraMessageBox.Show(
                        $"Base de datos deben de estar en {proyecto.DirApplication_WebService_WooWServer_DBApp}",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                proyecto.ParConexion.DbApp = Path.GetFileName(saveDialog.FileName);
                txtBaseDatosHibrida.EditValue = proyecto.ParConexion.DbApp;

                if (!NoSaveProyect)
                    proyecto.Save();
            }
        }

        private void txtTestApp_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.UnitTestApp = txtTestApp.EditValue.ToSafeString();
            if (!NoSaveProyect)
                proyecto.Save();
        }

        private void buLibroMayor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmLibroMayor));
        }

        private void buControlVersiones_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmCompareVersion));
        }

        private void buClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fmMain.Restart();
            //if (bCambiosAlProyecto())
            //{
            //    return;
            //}

            //if (buCierraServicio.Enabled)
            //    buCierraServicio.PerformClick();

            //if (processVisualStudio != null)
            //{
            //    processVisualStudio.Kill(true);
            //    processVisualStudio = null;
            //}

            //buBuild.Enabled = false;
            //ribbonElementos.Enabled = false;
            //ribbonMensajes.Enabled = false;
            //ribDiseno.Enabled = false;
            //buPropiedades.Enabled = false;
            //buPackageManager.Enabled = false;
            //buComparar.Enabled = false;
            //buParametros.Enabled = false;
            //buClose.Enabled = false;

            //buArchivo.Enabled = true;

            //List<UserControl> UserToDelete = new List<UserControl>();

            //foreach (Control ctrl in this.Controls)
            //    if (ctrl is IForm)
            //        UserToDelete.Add(ctrl as UserControl);

            //foreach (var Form in UserToDelete)
            //{
            //    Form.Dispose();
            //    this.Controls.Remove(Form);
            //}
            //Forms.Clear();

            //Proyecto.Clear();
            //proyecto = Proyecto.getInstance();

            //this.Text = $"WooW Service Builder";
        }

        private void buIntegrales_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmIntegralGenerations));
        }

        private void buPackageManager_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            InitUserForm(typeof(fmPackageManager));
        }

        private void buArchivo_Popup(object sender, EventArgs e)
        {
            buArchivo.ItemLinks.Clear();
            buArchivo.ItemLinks.Add(buAbrir);
            buArchivo.ItemLinks.Add(buNuevo);
            buArchivo.ItemLinks.Add(buSalir);
            var locMenu = buArchivo.ItemLinks.Add(buConfiguracion);
            locMenu.BeginGroup = true;

            string valueName = @"ListFiles";
            string ListFiles = Microsoft
                .Win32.Registry.GetValue(keyName, valueName, null)
                .ToSafeString();

            string[] Files = ListFiles.Split(';');

            bool bFirst = true;
            BarItemLink buInicio = null;
            // El arreglo debe ser menor a 20 elementos y se agrega al principio ArchivoProyecto
            ListFiles = ArchivoProyecto + ";";
            int Elementos = (Files.Length > 20 ? 20 : Files.Length);
            foreach (string file in Files)
            {
                if (!File.Exists(file))
                    continue;
                BarButtonItem button = new DevExpress.XtraBars.BarButtonItem() { Caption = file };

                button.ItemClick += (object sender, ItemClickEventArgs e) =>
                {
                    if (bCambiosAlProyecto())
                    {
                        return;
                    }

                    var thisButton = e.Item as BarButtonItem;

                    string valueName = @"ListFiles";
                    string ListFiles = Microsoft
                        .Win32.Registry.GetValue(keyName, valueName, null)
                        .ToSafeString();

                    // El arreglo debe ser menor a 20 elementos y se agrega al principio ArchivoProyecto
                    ListFiles = thisButton.Caption + ";";
                    int Elementos = (Files.Length > 20 ? 20 : Files.Length);
                    for (int i = 0; i < Elementos; i++)
                    {
                        if (Files[i] == thisButton.Caption)
                            continue;
                        if (File.Exists(Files[i]))
                            ListFiles += Files[i] + ";";
                    }

                    if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
                    {
                        var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                            @"SOFTWARE\FROGadn\WooW"
                        );
                        key.SetValue(valueName, ListFiles);
                    }
                    else
                    {
                        var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                            @"SOFTWARE\FROGadn\WooW",
                            true
                        );
                        key.SetValue(valueName, ListFiles);
                    }
                    ArchivoProyecto = file;

                    try
                    {
                        wooWConfigParams.OrigenDiferenteSoloLectura =
                            PackageHelper.ValidaNombreOrigenYSoloLectura(
                                ArchivoProyecto,
                                wooWConfigParams.Origen
                            );
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(
                            ex.Message,
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }

                    Proyecto.Clear();
                    proyecto = Proyecto.getInstance();
                    CargarProyecto(ArchivoProyecto);
                };

                BarItemLink buActual = null;
                if (bFirst)
                    buActual = buInicio = buArchivo.ItemLinks.Add(button);
                else
                    buActual = buArchivo.ItemLinks.Add(button);

                bFirst = false;
            }

            if (buInicio != null)
                buInicio.BeginGroup = true;
        }

        private void btnViewTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitUserForm(typeof(fmTestCases));
        }

        private void buAbreAppSettings_ItemClick(object sender, ItemClickEventArgs e)
        {
            string Archivo = Path.Combine(
                Path.GetFullPath(proyecto.ArchivoDeProyecto),
                proyecto.DirApplication_WebService_WebService,
                "appsettings.json"
            );

            // Abre el archivo usando el block de notas
            Process.Start("notepad.exe", Archivo);
        }

        private void buEncriptarPassword_ItemClick(object sender, ItemClickEventArgs e)
        {
            fmNormalizedText fm = new fmNormalizedText();
            fm.ExpresionRegular = string.Empty;
            if (fm.ShowDialog() == DialogResult.OK)
            {
                var protectedString = WoCrypto.ProtectCrypt(fm.NormalizedText);
                Clipboard.SetDataObject(protectedString, true);
                MessageBox.Show("Texto encriptado copiado al portapapeles");
            }
        }

        private void txtCadenaConexionMySQL_EditValueChanged(object sender, EventArgs e)
        {
            string valueName = @"CadenaConexionMySQL";

            if (txtCadenaConexionMySQL.EditValue.IsNullOrStringEmpty())
                return;

            if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\FROGadn\WooW"
                );
                key.SetValue(valueName, txtCadenaConexionMySQL.EditValue.ToSafeString());
            }
            else
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\FROGadn\WooW",
                    true
                );
                key.SetValue(valueName, txtCadenaConexionMySQL.EditValue.ToSafeString());
            }
        }

        private void txtCadenaConexionSQL_EditValueChanged(object sender, EventArgs e)
        {
            string valueName = @"CadenaConexionSQL";

            if (txtCadenaConexionSQL.EditValue.IsNullOrStringEmpty())
                return;

            if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\FROGadn\WooW"
                );
                key.SetValue(valueName, txtCadenaConexionSQL.EditValue.ToSafeString());
            }
            else
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\FROGadn\WooW",
                    true
                );
                key.SetValue(valueName, txtCadenaConexionSQL.EditValue.ToSafeString());
            }
        }

        private void txtCadenaConexionPostgre_EditValueChanged(object sender, EventArgs e)
        {
            string valueName = @"CadenaConexionPostgre";

            if (txtCadenaConexionPostgre.EditValue.IsNullOrStringEmpty())
                return;

            if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\FROGadn\WooW"
                );
                key.SetValue(valueName, txtCadenaConexionPostgre.EditValue.ToSafeString());
            }
            else
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\FROGadn\WooW",
                    true
                );
                key.SetValue(valueName, txtCadenaConexionPostgre.EditValue.ToSafeString());
            }
        }

        private void txtDBPassword_EditValueChanged(object sender, EventArgs e)
        {
            string valueName = @"EncryptedSQLPassword";

            if (txtDBPassword.EditValue.IsNullOrStringEmpty())
                return;

            if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\FROGadn\WooW"
                );
                key.SetValue(valueName, txtDBPassword.EditValue.ToSafeString());
            }
            else
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\FROGadn\WooW",
                    true
                );
                key.SetValue(valueName, txtDBPassword.EditValue.ToSafeString());
            }
        }

        private void txtBaseDatos_EditValueChanged(object sender, EventArgs e)
        {
            if (txtTipoInstancia.EditValue.ToSafeString() == "DEV")
                txtBaseDatos.EditValue = "DEV Sqlite";
            else
            {
                if (txtBaseDatos.EditValue.ToSafeString() == "DEV Sqlite")
                    txtBaseDatos.EditValue = "QAS MySQL";
            }

            proyecto.ParConexion.InstanceDataBase = txtBaseDatos.EditValue.ToSafeString();
        }

        private void buConfiguracion_ItemClick(object sender, ItemClickEventArgs e)
        {
            fmConfiguracion fm = new fmConfiguracion(keyName);
            fm.ShowDialog();
        }

        private void buSalir_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}
