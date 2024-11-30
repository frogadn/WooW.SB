using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Config.Class;
using WooW.SB.Config.Helpers;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

// ToDo cerrar servicio cuando se cierre el programa
// ToDo poner boton de cerrar servicio
// ToDo no ejectuar otra vez si ya esta el servicio
// ToDo Generar Proyecto completo
// ToDo

namespace WooW.SB.Forms
{
    public partial class fmModelMaker : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        int Errores = 0;
        public Proyecto proyecto { get; set; }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return false; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Refrescar() { }

        public void Cargar()
        {
            buCopiaPlantilla.Enabled = !wooWConfigParams.OrigenDiferenteSoloLectura;
            buFormateaTodo.Enabled = !wooWConfigParams.OrigenDiferenteSoloLectura;
        }

        public fmModelMaker()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();
        }

        public void LogTitulo(string message = "")
        {
            this.txtLog.SelectionStart = this.txtLog.TextLength;
            this.txtLog.SelectionLength = 0;

            this.txtLog.SelectionColor = Color.Yellow;
            this.txtLog.SelectionFont = new Font(this.txtLog.Font, FontStyle.Bold);
            this.txtLog.AppendText(message + "\r\n");
            this.txtLog.SelectionColor = this.txtLog.ForeColor;
            this.txtLog.ScrollToCaret();
        }

        public void LogParrafo(string message = "")
        {
            this.txtLog.SelectionStart = this.txtLog.TextLength;
            this.txtLog.SelectionLength = 0;

            this.txtLog.SelectionColor = Color.LightGray;
            this.txtLog.SelectionFont = new Font(this.txtLog.Font, FontStyle.Regular);
            this.txtLog.AppendText(message + "\r\n");
            this.txtLog.SelectionColor = this.txtLog.ForeColor;
            this.txtLog.ScrollToCaret();
        }

        public void LogError(string message = "")
        {
            this.txtLog.SelectionStart = this.txtLog.TextLength;
            this.txtLog.SelectionLength = 0;

            this.txtLog.SelectionColor = Color.Red;
            this.txtLog.SelectionFont = new Font(this.txtLog.Font, FontStyle.Regular);
            this.txtLog.AppendText(message + "\r\n");
            this.txtLog.SelectionColor = this.txtLog.ForeColor;
            this.txtLog.ScrollToCaret();
        }

        public void GenerarModelosYCompila()
        {
            MakeModels(true);
        }

        private void buGenerar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool Compilar = (e.Item != buGenerar);

            txtLog.Text = string.Empty;

            var assemblyVersion = typeof(fmModelMaker).Assembly.GetName().Version;
            var template = $"Templates.{assemblyVersion.Major}.{assemblyVersion.Minor}";

            if (template != Proyecto.getInstance().TemplateCompilacion)
            {
                XtraMessageBox.Show(
                    $"La plantilla fue cambiada, cópiela nuevamente, para generar servicio",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            if (Compilar)
            {
                MethodInfo methodInfo = this.Parent.GetType().GetMethod("CambiosPendientes");
                string sCambiosPendientes = methodInfo.Invoke(this.Parent, null).ToString();

                if (!string.IsNullOrEmpty(sCambiosPendientes))
                {
                    XtraMessageBox.Show(
                        $"Para Generar y Compilar debe grabar los cambios en {sCambiosPendientes}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
                    return;
                }

                proyecto.ParConexion.CierraServicio();
                fmMain.Restart(proyecto.ArchivoDeProyecto, "-m");
                return;
            }

            // ToDo Debería de cerrarse y lanzarse
            proyecto.ParConexion.CierraServicio();
            MakeModels(e.Item == buGenerarYCompilar);
        }

        private void MakeModels(bool bCompilar)
        {
            StringBuilder sb = new StringBuilder();
            proyecto.ValidaOrigenPropiedades(wooWConfigParams.Origen, sb, int.MaxValue);
            proyecto.ValidaOrigenModelos(wooWConfigParams.Origen, sb, int.MaxValue);

            if (!sb.ToString().IsNullOrStringEmpty())
            {
                LogError();
                LogError("Errores en Origen");
                LogError();
                LogError(sb.ToString());

                return;
            }

            try
            {
                string Repetidos = PaquetesVerifica.RecursivosYModelosRepetidos(
                    proyecto.ArchivoDeProyecto
                );

                if (!Repetidos.IsNullOrStringEmpty())
                {
                    LogError();
                    LogError("Modelos duplicados en:");
                    LogError();
                    LogError(Repetidos.ToString());
                    return;
                }
            }
            catch (Exception ex)
            {
                LogError();
                LogError(ex.Message);
                LogError();
                return;
            }

            ModelMaker modelMaker = new ModelMaker(proyecto.ArchivoDeProyecto, bCompilar);
            modelMaker.delLogError = LogError;
            modelMaker.delLogParrafo = LogParrafo;
            modelMaker.delLogtitulo = LogTitulo;

            modelMaker.Do();
        }

        private void buFormateaTodo_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            MethodInfo methodInfo = this.Parent.GetType().GetMethod("CambiosPendientes");
            string sCambiosPendientes = methodInfo.Invoke(this.Parent, null).ToString();

            if (!string.IsNullOrEmpty(sCambiosPendientes))
            {
                XtraMessageBox.Show(
                    $"Para formatear todo, debe grabar los cambios en {sCambiosPendientes}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            if (
                XtraMessageBox.Show(
                    "Esta seguro de formatear todo el proyecto?",
                    "Confirme...",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) != DialogResult.Yes
            )
                return;

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                FormateaTodo.Do();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                XtraMessageBox.Show(
                    "Formateo finalizado",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                fmMain.Restart(proyecto.ArchivoDeProyecto, "");
            }
        }

        private void fmModelMaker_Load(object sender, EventArgs e)
        {
            DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo =
                new DevExpress.XtraBars.LinkPersistInfo[proyecto.Apps.Count];

            DevExpress.XtraBars.BarItem[] barItem = new DevExpress.XtraBars.BarItem[
                proyecto.Apps.Count
            ];

            int index = 0;
            foreach (var app in proyecto.Apps)
            {
                DevExpress.XtraBars.BarButtonItem item = new DevExpress.XtraBars.BarButtonItem();
                item.Caption = EtiquetaCol.Get(app.EtiquetaId);
                item.Id = ribbonControl1.Manager.GetNewItemId();
                item.Name = app.Id + "btn";
                item.Tag = app;
                item.ItemClick += Item_ItemClick;
                barItem[index] = item;
                linkPersistInfo[index] = new DevExpress.XtraBars.LinkPersistInfo((item));
                index++;
            }

            ribbonControl1.Items.AddRange(barItem);
            buVisualStudio.AddItems(barItem); //LinksPersistInfo.AddRange(linkPersistInfo);
        }

        private void Item_ItemClick(object sender, ItemClickEventArgs e)
        {
            App app = e.Item.Tag as App;

            string commandLineArgs = $"{proyecto.DirApplication}\\{app.Id}\\WooW.{app.Id}.sln";

            woVisualStudio.AbreVisualStudio(commandLineArgs, false);
        }

        private void buCopiaPlantilla_ItemClick(object sender, ItemClickEventArgs e)
        {
            var assemblyVersion = typeof(fmModelMaker).Assembly.GetName().Version;
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var template = $"Templates.{assemblyVersion.Major}.{assemblyVersion.Minor}";

            // Repositorio donde se encuntran los archivos de plantilla
            var repoDir = Path.Combine(path, template);

            if (!Directory.Exists(repoDir))
            {
                XtraMessageBox.Show(
                    $"No se encontraró el directorio de plantilla {repoDir}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            ProyectNew.CopyPlantillas(
                repoDir,
                Path.GetDirectoryName(proyecto.ArchivoDeProyecto),
                "*.cs"
            );

            // Debe copiar a las diferentes apps excepto WebClient y WebService
            foreach (var app in proyecto.Apps)
            {
                if (app.Id == "WebClient" || app.Id == "WebService")
                    continue;

                var locRepDir = Path.Combine(repoDir, "Application\\WebService\\WooW.WebService");

                var locAppDir = Path.Combine(
                    Path.GetDirectoryName(proyecto.ArchivoDeProyecto),
                    $"Application\\{app.Id}\\WooW.{app.Id}"
                );

                ProyectNew.CopyPlantillas(locRepDir, locAppDir, "*.cs");
            }

            Proyecto.getInstance().TemplateCompilacion = template;
            Proyecto.getInstance().Save();

            DeleteVSDirectories(Path.GetDirectoryName(proyecto.ArchivoDeProyecto));

            XtraMessageBox.Show(
                "Archivos base copiados",
                "Verifique...",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void DeleteVSDirectories(string startDirectory)
        {
            try
            {
                // Get all directories including subdirectories
                var directories = Directory.GetDirectories(
                    startDirectory,
                    "*",
                    SearchOption.AllDirectories
                );

                foreach (var directory in directories)
                {
                    if (
                        Path.GetFileName(directory)
                            .Equals(".vs", StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        try
                        {
                            Directory.Delete(directory, true); // true means delete subdirectories and files
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"Error deleting .vs directory at {directory}: {ex.Message}"
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching directories: {ex.Message}");
            }
        }
    }
}
