using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using ActiproSoftware.Text.Languages.DotNet;
using ActiproSoftware.Text.Languages.DotNet.Reflection;
using ActiproSoftware.Text.Tagging.Implementation;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using WooW.Core;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Enum;
using WooW.SB.Config.Helpers;
using WooW.SB.Config.Templates;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using static WooW.SB.Config.Editors.woGridModel;

// ToDo falta parte de consulta de modelos y propiedades y generacion de snippets

namespace WooW.SB.Forms
{
    public partial class fmModelDTO : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public Proyecto proyecto { get; set; }

        private string AssemblyPath = @"\Service\WooW\WooW.ServiceModel\bin\Debug\net48";

        private List<string> Referencias = new List<string>()
        {
            "DynamicODataToSQL.dll",
            "FastMember.dll",
            "Microsoft.Bcl.AsyncInterfaces.dll",
            "Microsoft.OData.Core.dll",
            "Microsoft.OData.Edm.dll",
            "Microsoft.Spatial.dll",
            "Newtonsoft.Json.dll",
            "ServiceStack.Client.dll",
            "ServiceStack.Common.dll",
            "ServiceStack.dll",
            "ServiceStack.Interfaces.dll",
            "ServiceStack.OrmLite.dll",
            "ServiceStack.Redis.dll",
            "ServiceStack.Server.dll",
            "ServiceStack.Text.dll",
            "SqlKata.dll",
            "System.Buffers.dll",
            "System.Diagnostics.DiagnosticSource.dll",
            "System.Memory.dll",
            "System.Numerics.Vectors.dll",
            "System.Runtime.CompilerServices.Unsafe.dll",
            "System.Threading.Tasks.Extensions.dll",
            "WooW.Resources.dll",
            "WooW.Core.dll",
            "WooW.Core.Common.dll",
            "WooW.ServiceModel.dll"
        };

        private List<string> SystemReferencias = new List<string>()
        {
            "mscorlib",
            "System",
            "System.Core",
            "System.Drawing",
            "System.Data",
            "System.Xml",
            "System.Xml.Linq",
            "System.Data.Entity",
            "System.Threading.Tasks.Extensions"
        };

        private System.Threading.Timer refreshReferencesTimer;

        //private IProjectAssembly projectAssembly;

        private List<string> m_References;

        public fmModelDTO()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();

            m_References = new List<string>();

            Disposed += OnDispose;

            woGridModelo1.SetSnippet = setSnippet;
        }

        public void setSnippet(string snippet)
        {
            SyntaxEditor editor = (
                tabCode.SelectedTabPage == tabPagePre ? txtScriptPre : txtScriptPost
            );

            try
            {
                Clipboard.SetDataObject(snippet, true);
                editor.ActiveView.PasteFromClipboard();
                Clipboard.Clear();
            }
            catch { }
        }

        private void OnDispose(object sender, EventArgs e)
        {
            //projectAssembly.AssemblyReferences.Clear();
        }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return buRegistrar.Enabled; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Cargar()
        {
            ExecuteResolver();
            CargarModelo();
        }

        public void CargarModelo()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Tipo", typeof(string));
            dt.Columns.Add("Modelo", typeof(string));

            foreach (var Modelo in proyecto.ModeloCol.Modelos)
            {
                if (
                    (Modelo.TipoModelo == TypeModel.Configuration)
                    || (Modelo.TipoModelo == TypeModel.CatalogType)
                    || (Modelo.TipoModelo == TypeModel.Catalog)
                    || (Modelo.TipoModelo == TypeModel.TransactionContable)
                    || (Modelo.TipoModelo == TypeModel.TransactionNoContable)
                    || (Modelo.TipoModelo == TypeModel.Control)
                    || (Modelo.TipoModelo == TypeModel.Kardex)
                    || (Modelo.TipoModelo == TypeModel.DataMart)
                    || (Modelo.TipoModelo == TypeModel.Parameter)
                )
                {
                    DataRow drRow = dt.NewRow();
                    drRow["Proceso"] = Modelo.ProcesoId;
                    drRow["Tipo"] = Modelo.TipoModelo.ToString();
                    drRow["Modelo"] = Modelo.Id;
                    dt.Rows.Add(drRow);
                }
            }

            grdModelos.DataSource = dt;

            GridColumn col = grdModelosView.Columns["Proceso"];
            col.Width = 100;

            col = grdModelosView.Columns["Tipo"];
            col.Width = 100;

            col = grdModelosView.Columns["Modelo"];
            col.Width = 300;

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void grdModelosView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            buEditar.Enabled = false;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buSeleccionaExcepcion.Enabled = false;

            txtScriptPre.Document.IsReadOnly = true;
            txtScriptPost.Document.IsReadOnly = true;
            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.NoMostrar;
            txtScriptPre.Text = String.Empty;
            txtScriptPost.Text = String.Empty;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;

            if (drRow.IsNull())
            {
                return;
            }

            //"\\PostConditions"
            //string Model = drRow["Modelo"].ToString();

            var Model = this
                .proyecto.ModeloCol.Modelos.Where(x => x.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            txtScriptPre.Text = Model.GetPreCondiciones();
            txtScriptPost.Text = Model.GetPostCondiciones();

            buEditar.Enabled = true;
        }

        private void buAnular_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (
                XtraMessageBox.Show(
                    "Anular los cambios al modelo?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            grdModelosView_FocusedRowChanged(null, null);

            buEditar.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buSeleccionaExcepcion.Enabled = false;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;
        }

        private void RefreshReferenceListCallback(object stateInfo)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)(() => this.RefreshReferenceListCallback(stateInfo)));
                return;
            }

            //referencesListBox.Items.Clear();
            //foreach (var assemblyRef in projectAssembly.AssemblyReferences.ToArray())
            //    referencesListBox.Items.Add(assemblyRef.Assembly.Name);
        }

        private void OnAssemblyReferencesChanged(
            object sender,
            ActiproSoftware.Text.Utility.CollectionChangeEventArgs<IProjectAssemblyReference> e
        )
        {
            // Assessmblies can be added/removed quickly, especially during initial discovery.
            // Throttle UI refreshing until no "change" events have been received for a given time.
            if (refreshReferencesTimer is null)
                refreshReferencesTimer = new System.Threading.Timer(RefreshReferenceListCallback);

            // Reset the timer each time a new event is raised (without auto-restart)
            refreshReferencesTimer.Change(dueTime: 250, period: System.Threading.Timeout.Infinite);
        }

        private void DotNetProjectAssemblyReferenceLoader(object sender, DoWorkEventArgs e)
        {
            // Add some common assemblies for reflection (any custom assemblies could be added using various Add overloads instead)
            SyntaxEditorHelper.AddCommonDotNetSystemAssemblyReferences(
                AssemblyHelper.getInstance().projectAssembly,
                SystemReferencias,
                AssemblyPath,
                Referencias
            );
        }

        internal void ExecuteResolver()
        {
            if (AssemblyHelper.getInstance().projectAssembly == null)
            {
                AssemblyHelper.getInstance().projectAssembly = new CSharpProjectAssembly(
                    "ModelDTO"
                );

                AssemblyHelper.getInstance().projectAssembly.AssemblyReferences.ItemAdded +=
                    OnAssemblyReferencesChanged;
                AssemblyHelper.getInstance().projectAssembly.AssemblyReferences.ItemRemoved +=
                    OnAssemblyReferencesChanged;

                var assemblyLoader = new BackgroundWorker();
                assemblyLoader.DoWork += DotNetProjectAssemblyReferenceLoader;
                assemblyLoader.RunWorkerAsync();
            }

            // Load the .NET Languages Add-on C# language and register the project assembly on it
            var language = new CSharpSyntaxLanguage();
            language.RegisterService(
                new TextViewTaggerProvider<DelimiterHighlightTagger>(
                    typeof(DelimiterHighlightTagger)
                )
            );
            language.RegisterProjectAssembly(AssemblyHelper.getInstance().projectAssembly);
            txtScriptPre.Document.Language = language;
            txtScriptPost.Document.Language = language;

            /*

            if (m_ProjectResolver.IsNull())
            {
                m_ProjectResolver = new DotNetProjectResolver();
                m_ProjectResolver.CachePath = string.Format("{0}Cache\\", Path.GetTempPath());

                m_ProjectResolver.AddExternalReferenceForMSCorLib();
                m_ProjectResolver.AddExternalReferenceForSystemAssembly("System");
                m_ProjectResolver.AddExternalReferenceForSystemAssembly("System.Core");
                m_ProjectResolver.AddExternalReferenceForSystemAssembly("System.Drawing");
                m_ProjectResolver.AddExternalReferenceForSystemAssembly("System.Data");
                m_ProjectResolver.AddExternalReferenceForSystemAssembly("System.Xml");
                m_ProjectResolver.AddExternalReferenceForSystemAssembly("System.Xml.Linq");
                m_ProjectResolver.AddExternalReferenceForSystemAssembly("System.Windows.Forms");

                m_SyntaxLanguage = new CSharpSyntaxLanguage();
                txtScriptPre.Document.Language = m_SyntaxLanguage;
                txtScriptPre.Document.LanguageData = m_ProjectResolver;

                txtScriptPost.Document.Language = m_SyntaxLanguage;
                txtScriptPost.Document.LanguageData = m_ProjectResolver;

                m_SyntaxLanguage.SyntaxEditorIntelliPromptMemberListPreFilter += (
                    object sender,
                    IntelliPromptMemberListPreFilterEventArgs e
                ) =>
                {
                    CSharpContext Contexto = (CSharpContext)e.Context;
                    if (Contexto.Type == DotNetContextType.ThisAccess)
                        e.Items.Clear();
                    else if (Contexto.Type == DotNetContextType.UsingDeclaration)
                        e.Items.Clear();
                    else if (Contexto.Type == DotNetContextType.NamespaceTypeOrMember)
                    {
                        e.Items.Remove("GetType");
                        e.Items.Remove("GetHashCode");
                    }
                };
            }

            if (parReferences.IsNull())
                return;

            string filePath =
                string.Format(
                    "{0}\\",
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ) + "{0}";
            string filePathReferncias =
                string.Format(
                    "{0}\\{1}\\",
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    sPrivatePath
                ) + "{0}";

            // se checa si las referencias ya fueron agregadas previamente
            foreach (string itemReference in parReferences)
            {
                string sReferenciaReal = string.Format(filePath, itemReference);

                if (!File.Exists(sReferenciaReal))
                {
                    sReferenciaReal = string.Format(filePathReferncias, itemReference);
                    if (!File.Exists(sReferenciaReal))
                        sReferenciaReal = itemReference;
                }

                if (m_References.IndexOf(sReferenciaReal) >= 0)
                    continue;

                m_References.Add(sReferenciaReal);

                // se agrega referencia nueva al resolver
                try
                {
                    m_ProjectResolver.AddExternalReference(sReferenciaReal);
                }
#pragma warning disable CS0168
                catch (Exception Ex)
#pragma warning restore CS0168
                {
                    if (Ex.InnerException != null)
                        MessageBox.Show(Ex.Message + "\r\n\r\n" + Ex.InnerException.Message);
                    else
                        MessageBox.Show(Ex.Message);

                    //Debug.Write(string.Format("Error al agregar referencia externa -> {0}", ex.Message));
                }
            }
            */
        }

        #region Edición

        private SyntaxEditor currEditor()
        {
            return (tabCode.SelectedTabPage == tabPagePre ? txtScriptPre : txtScriptPost);
        }

        public void EditCut()
        {
            try
            {
                currEditor().ActiveView.CutToClipboard();
            }
            catch { }
        }

        public void EditCopy()
        {
            try
            {
                currEditor().ActiveView.CopyToClipboard();
            }
            catch { }
        }

        public void EditPaste()
        {
            try
            {
                currEditor().ActiveView.PasteFromClipboard();
            }
            catch { }
        }

        public void EditUndo()
        {
            try
            {
                currEditor().Document.UndoHistory.Undo();
            }
            catch { }
        }

        public void EditRedo()
        {
            try
            {
                currEditor().Document.UndoHistory.Redo();
            }
            catch { }
        }

        public void EditSelectAll()
        {
            try
            {
                currEditor().ActiveView.Selection.SelectAll();
            }
            catch { }
        }

        public void SelectLine(int parLine)
        {
            try
            {
                //currEditor().ActiveView.Selection.TextRange = currEditor().Document..Lines[
                //    parLine
                //].TextRange;
            }
            catch { }
        }

        private void contextScriptMenu_Opening(object sender, CancelEventArgs e)
        {
            mnuDeshacerItem.Enabled = currEditor().Document.UndoHistory.CanUndo;
            //mnuPegarItem.Enabled = currEditor().ActiveView.CanPaste;
            //mnuEliminarItem.Enabled = currEditor().ActiveView.CanDelete;
            //mnuInfoErrorItem.Enabled = currEditor().ActiveView.CurrentDocumentLine.Text.Contains(
            //    "ScriptingException"
            //);
        }

        private void mnuDeshacerItem_Click(object sender, EventArgs e)
        {
            this.EditUndo();
        }

        private void mnuCortarItem_Click(object sender, EventArgs e)
        {
            this.EditCut();
        }

        private void mnuCopiarItem_Click(object sender, EventArgs e)
        {
            this.EditCopy();
        }

        private void mnuPegarItem_Click(object sender, EventArgs e)
        {
            this.EditPaste();
        }

        private void mnuSeleccionarTodoItem_Click(object sender, EventArgs e)
        {
            this.EditSelectAll();
        }

        private void mnuFormatear_Click(object sender, EventArgs e)
        {
            PrettyPrint();
        }

        public bool PrettyPrint()
        {
            return true;

            /*
            string sTempFile = string.Format(
                "{0}\\astyle.cs",
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            );
            string sFile = string.Format(
                "{0}\\astyle.exe",
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            );

            if (File.Exists(sFile))
            {
                File.WriteAllText(sTempFile, txtScript.Document.Text);

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo =
                    new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "astyle.exe";
                startInfo.Arguments =
                    "--style=allman --indent-namespaces --break-blocks --delete-empty-lines \""
                    + sTempFile
                    + "\"";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                txtScript.Document.Text = File.ReadAllText(sTempFile);
                return true;
            }
            else
            {
                XtraMessageBox.Show(
                    "Falta astyle.exe",
                    "WooW",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return false;
            }
            */
        }

        #endregion Edición

        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();
            if (drRow.IsNull())
                return;

            var model = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            if (model == null)
            {
                XtraMessageBox.Show(
                    "Al parecer el modelo fue borrado, no es posible continuar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (
                (model.Diagrama == null)
                || (model.Diagrama.Estados == null)
                || (model.Diagrama.Estados.Count == 0)
            )
            {
                XtraMessageBox.Show(
                    "El modelo no tiene diagrama de estados",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            buEditar.Enabled = false;
            buRefrescarModelos.Enabled = false;
            buRegistrar.Enabled = true;
            buAnular.Enabled = true;
            buCompilar.Enabled = true;
            buSeleccionaExcepcion.Enabled = true;

            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            grpCambios.Enabled = false;

            if (txtScriptPre.Text.IsNullOrStringEmpty())
            {
                ttModeloScriptPre ttmodelo = new ttModeloScriptPre();
                ttmodelo.modelo = model;

                string sBuffer = String.Empty;

                try
                {
                    txtScriptPre.Text = ttmodelo.TransformText();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $"Error al generar Modelo Script {model.Id} {ex.Message}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }

            if (txtScriptPost.Text.IsNullOrStringEmpty())
            {
                ttModeloScriptPost ttmodelo = new ttModeloScriptPost();
                ttmodelo.modelo = model;

                string sBuffer = String.Empty;

                try
                {
                    txtScriptPost.Text = ttmodelo.TransformText();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $"Error al generar Modelo Script {model.Id} {ex.Message}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }

            txtScriptPre.Document.IsReadOnly = false;
            txtScriptPost.Document.IsReadOnly = false;
            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.SnippetServidor;
        }

        private void buRegistrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                grdModelosView_FocusedRowChanged(null, null);
                return;
            }

            if (!ValidarScript())
                return;

            var model = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            if (model == null)
            {
                XtraMessageBox.Show(
                    "Al parecer el modelo fue borrado, no es posible continuar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            model.SavePreCondiciones(txtScriptPre.Text);
            model.SavePostCondiciones(txtScriptPost.Text);

            buEditar.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buSeleccionaExcepcion.Enabled = false;

            buRefrescarModelos.Enabled = true;
            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;

            txtScriptPre.Document.IsReadOnly = true;
            txtScriptPost.Document.IsReadOnly = true;
            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.NoMostrar;
        }

        private bool ValidarScript()
        {
            //currModel.PreCondiciones = txtScriptPre.Text;
            //currModel.PostCondiciones = txtScriptPost.Text;

            return true;
        }

        private void buDescartarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if (
                XtraMessageBox.Show(
                    "Descartar los cambios?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            Cargar();
        }

        private void buRefrescarModelos_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            CargarModelo();
        }

        private void RefreshErrors(List<ScriptErrorDescriptor> parErrors)
        {
            if (parErrors.IsNull())
                return;

            lstErrors.Items.Clear();
            int iCount = 1;

            foreach (var itemError in parErrors)
            {
                ListViewItem viewItem = new ListViewItem(iCount.ToString());
                viewItem.SubItems.Add(itemError.Code);
                viewItem.SubItems.Add(itemError.Description);
                viewItem.SubItems.Add(itemError.Line == -1 ? "-" : itemError.Line.ToString());
                viewItem.SubItems.Add(itemError.Column.ToString());

                lstErrors.Items.Add(viewItem);
                iCount++;
            }

            tabInferior.SelectedTabPage = tabError;
        }

        private void buCompilar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<ScriptErrorDescriptor> alErrors;
            SyntaxEditorHelper.Validar(
                currEditor().Text,
                out alErrors,
                SystemReferencias,
                AssemblyPath,
                Referencias
            );

            if (alErrors != null && alErrors.Count > 0)
                RefreshErrors(alErrors);
            else
                lstErrors.Items.Clear();
        }

        private void lstErrors_DoubleClick(object sender, EventArgs e)
        {
            if (lstErrors.FocusedItem != null)
            {
                string sLine = lstErrors.FocusedItem.SubItems[3].Text;
                if (sLine.Equals("-"))
                    return;

                int iLine = Convert.ToInt32(sLine) - 1;
                if (iLine < 0)
                    return;

                currEditor().ActiveView.Selection.CaretPosition = new TextPosition(iLine, 0);
                currEditor().ActiveView.Selection.MoveToVisibleTop();
                currEditor().ActiveView.Selection.CaretPosition = new TextPosition(iLine, 0);
            }
        }

        private void tabCode_SelectedPageChanged(
            object sender,
            DevExpress.XtraTab.TabPageChangedEventArgs e
        )
        {
            lstErrors.Items.Clear();
        }

        private void txtScriptPre_Click(object sender, EventArgs e) { }

        private void buSeleccionaExcepcion_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            var fmNE = new fmNumberedExceptionsSelector();

            if (fmNE.ShowDialog() == DialogResult.OK)
            {
                SyntaxEditor editor = (
                    tabCode.SelectedTabPage == tabPagePre ? txtScriptPre : txtScriptPost
                );

                try
                {
                    Clipboard.SetDataObject(fmNE.NumeredExcepcion, true);
                    editor.ActiveView.PasteFromClipboard();
                    Clipboard.Clear();
                }
                catch { }
            }
        }
    }
}
