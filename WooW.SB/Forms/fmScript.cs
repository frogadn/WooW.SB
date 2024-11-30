using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using ActiproSoftware.Text.Languages.DotNet;
using ActiproSoftware.Text.Languages.DotNet.Reflection;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using ServiceStack;
using WooW.Core;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Editors;
using WooW.SB.Config.Enum;
using WooW.SB.Config.Helpers;
using WooW.SB.Config.Templates;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmScript : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        private string AssemblyPath = @"\Service\WooW\WooW.ServiceModel\bin\Debug\net48";

        public Proyecto proyecto { get; set; }

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
            "System.Data",
            "System.Drawing",
            "System.IO.Compression.FileSystem",
            "System.Numerics",
            "System.RunTime.Serialization",
            "System.Xml",
            "System.Xml.Linq",
            "System.Data.Entity"
        };

        private System.Threading.Timer refreshReferencesTimer;

        //private IProjectAssembly projectAssembly;

        private List<string> m_References;

        public fmScript()
        {
            InitializeComponent();

            m_References = new List<string>();

            Disposed += OnDispose;

            woGridModelo1.SetSnippet = setSnippet;
        }

        public void setSnippet(string snippet)
        {
            SyntaxEditor editor = txtScript;

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

        internal void ExecuteResolver()
        {
            if (AssemblyHelper.getInstance().projectAssembly == null)
            {
                AssemblyHelper.getInstance().projectAssembly = new CSharpProjectAssembly("Script");

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
            language.RegisterProjectAssembly(AssemblyHelper.getInstance().projectAssembly);

            txtScript.Document.Language = language;

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
            txtResponse.Properties.Items.Clear();

            foreach (
                var Modelo in this.proyecto.ModeloCol.Modelos.Where(e =>
                    e.TipoModelo == TypeModel.Response
                )
            )
                txtResponse.Properties.Items.Add(Modelo.Id);

            foreach (var Rol in proyecto.Roles)
                txtRoles.Properties.Items.Add(Rol.Id);

            foreach (var Permiso in proyecto.Permisos)
                txtPermisos.Properties.Items.Add(Permiso.Id);

            DataTable dt = new DataTable();

            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Tipo", typeof(string));
            dt.Columns.Add("Modelo", typeof(string));

            foreach (
                var Modelo in this.proyecto.ModeloCol.Modelos.Where(e =>
                    ((e.TipoModelo == TypeModel.Request) || (e.TipoModelo == TypeModel.Interface))
                )
            )
            {
                DataRow drRow = dt.NewRow();
                drRow["Proceso"] = Modelo.ProcesoId;
                drRow["Tipo"] = Modelo.TipoModelo.ToString();
                drRow["Modelo"] = Modelo.Id;
                dt.Rows.Add(drRow);
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

        private void buRefrescarModelos_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            CargarModelo();
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
            pnlPropiedades.Enabled = false;

            txtScript.Document.IsReadOnly = true;
            txtScript.Text = String.Empty;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;

            if (drRow.IsNull())
            {
                return;
            }

            var Model = this
                .proyecto.ModeloCol.Modelos.Where(x => x.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            txtScript.Text = Model.GetScript();

            foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                i.CheckState = CheckState.Unchecked;

            foreach (string rol in Model.ScriptRequest.Roles)
            {
                foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                {
                    if (i.Value.ToString() == rol)
                    {
                        i.CheckState = CheckState.Checked;
                        break;
                    }
                }
            }

            foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                i.CheckState = CheckState.Unchecked;

            foreach (string permiso in Model.ScriptRequest.Permisos)
            {
                foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                {
                    if (i.Value.ToString() == permiso)
                    {
                        i.CheckState = CheckState.Checked;
                        break;
                    }
                }
            }

            txtResponse.EditValue = Model.ScriptRequest.ResponseId;
            optColeccion.Checked = Model.ScriptRequest.Coleccion;
            optBackGround.Checked = Model.ScriptRequest.EjecutarEnBackGround;

            buEditar.Enabled = true;
        }

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

            if (txtScript.Text.IsNullOrStringEmpty())
            {
                fmModelRequestNew fm = new fmModelRequestNew(model);

                if (model.TipoModelo != TypeModel.Interface)
                {
                    if (fm.ShowDialog() != DialogResult.OK)
                        return;
                }

                try
                {
                    if (model.TipoModelo == TypeModel.Interface)
                    {
                        ttInterfaceScript ttinterfaceScript = new ttInterfaceScript();
                        ttinterfaceScript.request = model;
                        txtScript.Text = ttinterfaceScript.TransformText();
                    }
                    else if (fm.BackGround)
                    {
                        ttModeloScriptTarea ttmodeloscript = new ttModeloScriptTarea();
                        ttmodeloscript.request = model;
                        ttmodeloscript.response = proyecto
                            .ModeloCol.Modelos.Where(j => j.Id == fm.Response)
                            .FirstOrDefault();
                        ttmodeloscript.coleccion = fm.Coleccion;
                        txtScript.Text = ttmodeloscript.TransformText();
                    }
                    else
                    {
                        ttModeloScript ttmodeloscript = new ttModeloScript();
                        ttmodeloscript.request = model;
                        ttmodeloscript.response = proyecto
                            .ModeloCol.Modelos.Where(j => j.Id == fm.Response)
                            .FirstOrDefault();
                        ttmodeloscript.coleccion = fm.Coleccion;
                        txtScript.Text = ttmodeloscript.TransformText();
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $"Error al generar Modelo Script {model.Id} {ex.Message}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                txtResponse.EditValue = fm.Response;
                optColeccion.Checked = fm.Coleccion;
                optBackGround.Checked = fm.BackGround;
            }

            buEditar.Enabled = false;
            buRefrescarModelos.Enabled = false;
            buRegistrar.Enabled = true;
            buAnular.Enabled = true;
            buCompilar.Enabled = true;

            optColeccion.Enabled = false;
            optBackGround.Enabled = false;
            txtResponse.Enabled = false;

            pnlPropiedades.Enabled = (model.TipoModelo != TypeModel.Interface);

            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            grpCambios.Enabled = false;

            txtScript.Document.IsReadOnly = false;
            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.SnippetServidor;
        }

        private bool ValidarScript()
        {
            return true;
        }

        private void buRegistrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                grdModelosView_FocusedRowChanged(null, null);
                return;
            }

            //if (!ValidarScript())
            //    return;

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

            if (model.TipoModelo != TypeModel.Interface)
            {
                if (txtResponse.EditValue.ToSafeString().IsNullOrEmpty())
                {
                    XtraMessageBox.Show(
                        "Registre el modelo para la respuesta",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                model.ScriptRequest.ResponseId = txtResponse.EditValue.ToSafeString();
                model.ScriptRequest.Roles.Clear();
                model.ScriptRequest.Permisos.Clear();

                foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                {
                    if (i.CheckState == CheckState.Checked)
                        model.ScriptRequest.Roles.Add(i.Value.ToString());
                }

                foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                {
                    if (i.CheckState == CheckState.Checked)
                        model.ScriptRequest.Permisos.Add(i.Value.ToString());
                }

                model.ScriptRequest.ResponseId = txtResponse.EditValue.ToSafeString();
                model.ScriptRequest.Coleccion = optColeccion.Checked;
                model.ScriptRequest.EjecutarEnBackGround = optBackGround.Checked;

                model.ScriptRequest.Roles.Clear();
                model.ScriptRequest.Permisos.Clear();

                foreach (CheckedListBoxItem i in txtRoles.Properties.Items)
                {
                    if (i.CheckState == CheckState.Checked)
                        model.ScriptRequest.Roles.Add(i.Value.ToString());
                }

                foreach (CheckedListBoxItem i in txtPermisos.Properties.Items)
                {
                    if (i.CheckState == CheckState.Checked)
                        model.ScriptRequest.Permisos.Add(i.Value.ToString());
                }

                if (
                    (model.ScriptRequest.Roles.Count == 0)
                    || (model.ScriptRequest.Permisos.Count == 0)
                )
                {
                    XtraMessageBox.Show(
                        "Registre roles y permisos",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }
            model.SaveScript(txtScript.Text);

            proyecto.SaveModel(model);

            buEditar.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;

            buRefrescarModelos.Enabled = true;
            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;

            txtScript.Document.IsReadOnly = true;
            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.NoMostrar;
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

            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;
        }

        private void buCompilar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<ScriptErrorDescriptor> alErrors;
            SyntaxEditorHelper.Validar(
                txtScript.Text,
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

        public void EditCut()
        {
            try
            {
                this.txtScript.ActiveView.CutToClipboard();
            }
            catch { }
        }

        public void EditCopy()
        {
            try
            {
                this.txtScript.ActiveView.CopyToClipboard();
            }
            catch { }
        }

        public void EditPaste()
        {
            try
            {
                this.txtScript.ActiveView.PasteFromClipboard();
            }
            catch { }
        }

        public void EditUndo()
        {
            try
            {
                this.txtScript.Document.UndoHistory.Undo();
            }
            catch { }
        }

        public void EditRedo()
        {
            try
            {
                this.txtScript.Document.UndoHistory.Redo();
            }
            catch { }
        }

        public void EditSelectAll()
        {
            try
            {
                this.txtScript.ActiveView.Selection.SelectAll();
            }
            catch { }
        }

        private void contextScriptMenu_Opening(object sender, CancelEventArgs e)
        {
            mnuDeshacerItem.Enabled = this.txtScript.Document.UndoHistory.CanUndo;
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

                txtScript.ActiveView.Selection.CaretPosition = new TextPosition(iLine, 0);
                txtScript.ActiveView.Selection.MoveToVisibleTop();
                txtScript.ActiveView.Selection.CaretPosition = new TextPosition(iLine, 0);
            }
        }
    }
}
