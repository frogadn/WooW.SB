using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using ActiproSoftware.Text.Languages.DotNet.Reflection;
using ActiproSoftware.Text.Searching;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.EditActions;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.Implementation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTab;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Utilities;
using WooW.Core;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Config.Templates;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

// ToDo falta parte de consulta de modelos y propiedades y generacion de snippets

namespace WooW.SB.Forms
{
    public partial class fmModelPrePost : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        bool bInitPre = false;
        bool bInitPost = false;

        bool MarcandoTransicones = false;
        public Proyecto proyecto { get; set; }

        private string AssemblyPath;

        private IProjectAssembly projectAssembly;
        private BackgroundWorker _worker = new BackgroundWorker();
        private SyntaxEditorHelper syntaxEditorHelper;

        private List<string> Referencias = new List<string>()
        {
            "DynamicODataToSQL.dll",
            "FastMember.dll",
            //"Flurl.dll",
            "Microsoft.OData.Core.dll",
            "Microsoft.OData.Edm.dll",
            "Microsoft.Spatial.dll",
            //"Microsoft.Win32.SystemEvents.dll",
            "Newtonsoft.Json.dll",
            //"ServiceStack.Api.OpenApi.dll",
            "ServiceStack.Client.dll",
            "ServiceStack.Common.dll",
            "ServiceStack.dll",
            "ServiceStack.Interfaces.dll",
            "ServiceStack.OrmLite.dll",
            "ServiceStack.OrmLite.Sqlite.dll",
            "ServiceStack.Redis.dll",
            "ServiceStack.Server.dll",
            "ServiceStack.Text.dll",
            "GemBox.Document.dll",
            "MimeKit.dll",
            "SqlKata.dll",
            //"System.Data.SQLite.dll",
            //"System.Drawing.Common.dll",
            "WooW.Resources.dll",
            "WooW.Core.Common.dll",
            "WooW.Core.Server.dll",
            "WooW.WebService.dll",
            "WooW.CFDI.dll"
        };

        private List<string> SystemReferencias = new List<string>()
        {
            "System.RunTime",
            "mscorlib",
            "System",
            "System.Core",
            "System.Data",
            "System.Data.Common",
            "System.Drawing",
            "System.Text.RegularExpressions",
            //"System.Diagnostics",
            "System.Console",
            "System.Linq",
            "System.Linq.Expressions",
            "System.Private.CoreLib",
            "System.Reflection",
            "System.Collections",
            "System.IO.Compression.FileSystem",
            "System.Numerics",
            "System.RunTime.Serialization",
            "System.Xml",
            "System.Xml.Linq",
            "System.Xml.Serialization",
            "System.Private.Xml",
            "System.Xml.ReaderWriter"
        };

        public fmModelPrePost()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();

            AssemblyPath = Path.Combine(
                proyecto.DirApplication_WebService_WooWServer,
                @"bin\Debug\net8.0"
            );

            Disposed += OnDispose;

            woGridModelo1.SetSnippet = setSnippet;

            syntaxEditorHelper = new SyntaxEditorHelper(
                SystemReferencias,
                AssemblyPath,
                Referencias
            );

            woDiagram2.MostraSoloDiagrama();
            woDiagram2.OcultaReglas();
            woDiagram2.Enabled = true;
            woDiagram2.SetReadOnly(true);

            woDiagram2.selectTransicion = selectTransicion;

            woGridModelo1.MostraSoloDiagrama();
            woGridModelo1.OcultaReglas();

            navigablePre.Controls[1].TextChanged += navigableSymbolSelector1_TextChanged;
            navigablePost.Controls[1].TextChanged += navigableSymbolSelector1_TextChanged;

            txtScriptPre.ContextMenuStrip = contextScriptMenu;
            txtScriptPre.IsDefaultContextMenuEnabled = false;
            txtScriptPost.ContextMenuStrip = contextScriptMenu;
            txtScriptPost.IsDefaultContextMenuEnabled = false;
            txtModelCreation.ContextMenuStrip = contextScriptMenu;
            txtModelCreation.IsDefaultContextMenuEnabled = false;
            txtModelPoliza.ContextMenuStrip = contextScriptMenu;
            txtModelPoliza.IsDefaultContextMenuEnabled = false;
        }

        private void navigableSymbolSelector1_TextChanged(object sender, EventArgs e)
        {
            if (optMarcarTransiciones.EditValue.ToBoolean())
                MostrarMetodoSeleccionado();
        }

        private void selectTransicion(string Name)
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                grdModelosView_FocusedRowChanged(null, null);
                return;
            }

            var modelo = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            SyntaxEditor editor = currEditor();

            string Texto = currEditor().Text.Replace("\r\n", "\n");

            string[] Lines = Texto.Split(new char[] { '\n' });

            string Busqueda = $"#region {modelo.Id}{Name}";

            bool Found = false;
            int Line = 0;
            foreach (string line in Lines)
            {
                if (line.Contains(Busqueda))
                {
                    Found = true;
                    break;
                }
                Line++;
            }

            if (!Found)
                return;

            currEditor().ActiveView.ExecuteEditAction(new ExpandAllOutliningAction());
            currEditor().ActiveView.Selection.CaretPosition = new TextPosition(Line, 1);
            currEditor().ActiveView.Scroller.ScrollLineToVisibleMiddle();
            currEditor().Focus();
        }

        public void setSnippet(string snippet)
        {
            SyntaxEditor editor = currEditor();

            if (editor.Document.IsReadOnly)
                return;

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

        public void Refrescar()
        {
            if (buRefrescar.Enabled)
                buRefrescar.PerformClick();
        }

        public void Cargar()
        {
            repFiltroApps.Items.Clear();
            repFiltroApps.Items.Add(@" Todos ");
            repFiltroApps.Items[0].CheckState = CheckState.Checked;
            foreach (var app in proyecto.Apps)
            {
                repFiltroApps.Items.Add(app.Id);
            }
            txtFiltroApps.EditValue = " Todos ";

            ExecuteResolver();
            CargarModelo();
        }

        public void CargarModelo()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Proceso", typeof(string));
            dt.Columns.Add(@"Tipo", typeof(string));
            dt.Columns.Add(@"Hecho", typeof(bool));
            dt.Columns.Add(@"Modelo", typeof(string));

            foreach (var modelo in proyecto.ModeloCol.Modelos.OrderBy(e => e.Id).ToList())
            {
                if (
                    (modelo.TipoModelo == WoTypeModel.Configuration)
                    || (modelo.TipoModelo == WoTypeModel.CatalogType)
                    || (modelo.TipoModelo == WoTypeModel.Catalog)
                    || (modelo.TipoModelo == WoTypeModel.TransactionContable)
                    || (modelo.TipoModelo == WoTypeModel.TransactionNoContable)
                    || (modelo.TipoModelo == WoTypeModel.Control)
                    || (modelo.TipoModelo == WoTypeModel.Kardex)
                    || (modelo.TipoModelo == WoTypeModel.DataMart)
                    || (modelo.TipoModelo == WoTypeModel.Parameter)
                    || (modelo.TipoModelo == WoTypeModel.View)
                )
                {
                    bool bFound = false;
                    foreach (CheckedListBoxItem item in repFiltroApps.Items)
                    {
                        if (item.CheckState != CheckState.Checked)
                            continue;

                        if (
                            (item.Value.ToString() == " Todos ")
                            || (
                                modelo
                                    .Apps.Where(x => x.AppId == item.Value.ToString())
                                    .FirstOrDefault() != null
                            )
                        )
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                        continue;

                    DataRow drRow = dt.NewRow();
                    drRow["Proceso"] = modelo.ProcesoId;
                    drRow["Tipo"] = modelo.TipoModelo.ToString();
                    drRow["Modelo"] = modelo.Id;
                    drRow["Hecho"] =
                        modelo.bPreCondicionesExist
                        || modelo.bPostCondicionesExist
                        || modelo.bModelPolizaExist
                        || modelo.bModelCreationExist;
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
            buEditar.Enabled = false;
            buVisual.Enabled = false;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buBorrar.Enabled = false;
            buRefrescar.Enabled = true;
            buSeleccionaExcepcion.Enabled = false;
            txtFiltroApps.Enabled = true;

            txtScriptPre.Document.IsReadOnly = true;
            txtScriptPost.Document.IsReadOnly = true;
            txtModelPoliza.Document.IsReadOnly = true;
            txtModelCreation.Document.IsReadOnly = true;

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow == null)
            {
                txtScriptPre.Text = String.Empty;
                txtScriptPost.Text = String.Empty;
                txtModelPoliza.Text = String.Empty;
                txtModelCreation.Text = String.Empty;
                return;
            }

            if (drRow["Tipo"].ToString() == WoTypeModel.View.ToString())
            {
                tabPagePre.PageVisible = false;
                tabPagePost.PageVisible = false;
                tabModelPoliza.PageVisible = false;
            }
            else
            {
                tabPagePre.PageVisible = true;
                tabPagePost.PageVisible = true;
            }

            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.NoMostrar;
            txtScriptPre.Text = String.Empty;
            txtScriptPost.Text = String.Empty;
            txtModelPoliza.Text = String.Empty;
            txtModelCreation.Text = String.Empty;

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

            woDiagram2.New();
            woDiagram2.currDiagrama = Model.Diagrama;
            woDiagram2.CreaDiagrama();

            tabModelPoliza.PageVisible = (Model.TipoModelo == WoTypeModel.TransactionContable);

            if (tabPagePre.PageVisible)
                txtScriptPre.Text = Model.GetPreCondiciones();

            if (tabPagePost.PageVisible)
                txtScriptPost.Text = Model.GetPostCondiciones();

            if (tabModelPoliza.PageVisible)
                txtModelPoliza.Text = Model.GetModelPoliza();

            txtModelCreation.Text = Model.GetModelCreation();

            buEditar.Enabled = true;
            buVisual.Enabled = true;
            buBorrar.Enabled = true;
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
            buVisual.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buBorrar.Enabled = true;
            buSeleccionaExcepcion.Enabled = false;
            grpEditor.Enabled = false;
            txtFiltroApps.Enabled = true;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;
        }

        //private void RefreshReferenceListCallback(object stateInfo)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        this.BeginInvoke((Action)(() => this.RefreshReferenceListCallback(stateInfo)));
        //        return;
        //    }

        //    //referencesListBox.Items.Clear();
        //    //foreach (var assemblyRef in projectAssembly.AssemblyReferences.ToArray())
        //    //    referencesListBox.Items.Add(assemblyRef.Assembly.Name);
        //}

        //private void OnAssemblyReferencesChanged(
        //    object sender,
        //    ActiproSoftware.Text.Utility.CollectionChangeEventArgs<IProjectAssemblyReference> e
        //)
        //{
        //    // Assessmblies can be added/removed quickly, especially during initial discovery.
        //    // Throttle UI refreshing until no "change" events have been received for a given time.
        //    if (refreshReferencesTimer is null)
        //        refreshReferencesTimer = new System.Threading.Timer(RefreshReferenceListCallback);

        //    // Reset the timer each time a new event is raised (without auto-restart)
        //    refreshReferencesTimer.Change(dueTime: 250, period: System.Threading.Timeout.Infinite);
        //}

        //private void DotNetProjectAssemblyReferenceLoader(object sender, DoWorkEventArgs e)
        //{
        //    // Add some common assemblies for reflection (any custom assemblies could be added using various Add overloads instead)
        //    SyntaxEditorHelper.AddCommonDotNetSystemAssemblyReferences(
        //        AssemblyHelper.getInstance().projectAssembly,
        //        SystemReferencias,
        //        AssemblyPath,
        //        Referencias
        //    );
        //}

        internal void ExecuteResolver()
        {
            var language = new CSharpSyntaxLanguage();
            projectAssembly = language.GetService<IProjectAssembly>();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += ChargeDlls;
            _worker.RunWorkerAsync();

            txtScriptPre.Document.Language = language;
            var formatterPre = (CSharpTextFormatter)
                txtScriptPre.Document.Language.GetTextFormatter();
            formatterPre.IsOpeningBraceOnNewLine = true;

            txtScriptPost.Document.Language = language;
            var formatterPost = (CSharpTextFormatter)
                txtScriptPre.Document.Language.GetTextFormatter();
            formatterPost.IsOpeningBraceOnNewLine = true;

            txtModelPoliza.Document.Language = language;
            var formatterPoliza = (CSharpTextFormatter)
                txtModelPoliza.Document.Language.GetTextFormatter();
            formatterPoliza.IsOpeningBraceOnNewLine = true;

            txtModelCreation.Document.Language = language;
            var formatterCreacion = (CSharpTextFormatter)
                txtModelCreation.Document.Language.GetTextFormatter();
            formatterCreacion.IsOpeningBraceOnNewLine = true;

            //var formatter = (CSharpTextFormatter)txtScript.Document.Language.GetTextFormatter();
            //formatter.IsOpeningBraceOnNewLine = true;
        }

        private void ChargeDlls(object sender, EventArgs e)
        {
            var references = new List<string>();

            string corePath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            foreach (var Referencia in SystemReferencias)
                references.Add(Path.Combine(corePath, Referencia) + ".dll");

            foreach (var Referencia in Referencias)
                references.Add(Path.Combine(AssemblyPath, Referencia));

            projectAssembly.AssemblyReferences.AddMsCorLib();
            /// System
            foreach (var dll in references)
            {
                projectAssembly.AssemblyReferences.AddFrom(dll);
            }
        }

        //internal void ExecuteResolver()
        //{
        //    if (AssemblyHelper.getInstance().projectAssembly == null)
        //    {
        //        AssemblyHelper.getInstance().projectAssembly = new CSharpProjectAssembly(
        //            "ModelDTO"
        //        );

        //        AssemblyHelper.getInstance().projectAssembly.AssemblyReferences.ItemAdded +=
        //            OnAssemblyReferencesChanged;
        //        AssemblyHelper.getInstance().projectAssembly.AssemblyReferences.ItemRemoved +=
        //            OnAssemblyReferencesChanged;

        //        var assemblyLoader = new BackgroundWorker();
        //        assemblyLoader.DoWork += DotNetProjectAssemblyReferenceLoader;
        //        assemblyLoader.RunWorkerAsync();
        //    }

        //    // Load the .NET Languages Add-on C# language and register the project assembly on it
        //    var language = new CSharpSyntaxLanguage();
        //    language.RegisterService(
        //        new TextViewTaggerProvider<DelimiterHighlightTagger>(
        //            typeof(DelimiterHighlightTagger)
        //        )
        //    );
        //    language.RegisterProjectAssembly(AssemblyHelper.getInstance().projectAssembly);

        //    txtScriptPre.Document.Language = language;
        //    var formatterPre = (CSharpTextFormatter)
        //        txtScriptPre.Document.Language.GetTextFormatter();
        //    formatterPre.IsOpeningBraceOnNewLine = true;

        //    txtScriptPost.Document.Language = language;
        //    var formatterPost = (CSharpTextFormatter)
        //        txtScriptPost.Document.Language.GetTextFormatter();
        //    formatterPost.IsOpeningBraceOnNewLine = true;

        //    txtCreacion.Document.Language = language;
        //    var formatterCreacion = (CSharpTextFormatter)
        //        txtCreacion.Document.Language.GetTextFormatter();
        //    formatterCreacion.IsOpeningBraceOnNewLine = true;
        //}

        #region Edición

        private SyntaxEditor currEditor()
        {
            if (tabCode.SelectedTabPage == tabPagePre)
                return txtScriptPre;
            else if (tabCode.SelectedTabPage == tabPagePost)
                return txtScriptPost;
            else if (tabCode.SelectedTabPage == tabModelPoliza)
                return txtModelPoliza;
            else
                return txtModelCreation;
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
            //    "WoScriptingException"
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
            if (currEditor().Document.IsReadOnly)
                return;
            buFormatear.PerformClick();
        }

        #endregion Edición

        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string CambiosPendientes = string.Empty;
            if ((this.Parent as fmMain).CambiosPendientesLogicaOScript(out CambiosPendientes))
            {
                XtraMessageBox.Show(
                    "No se puede editar si tiene cambios por aplicar en " + CambiosPendientes,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

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

            if (tabPagePre.PageVisible)
            {
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
            }

            buEditar.Enabled = false;
            buVisual.Enabled = false;
            buRefrescar.Enabled = false;
            buRegistrar.Enabled = (
                (drRow[@"Modelo"].ToSafeString().StartsWith(wooWConfigParams.Origen))
                || (model.SubTipoModelo == WoSubTypeModel.Extension)
                || (model.SubTipoModelo == WoSubTypeModel.Override)
            );
            buAnular.Enabled = true;
            buCompilar.Enabled = true;
            buBorrar.Enabled = false;
            buSeleccionaExcepcion.Enabled = true;
            grpEditor.Enabled = true;
            txtFiltroApps.Enabled = false;

            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            grpCambios.Enabled = false;

            if ((tabPagePre.PageVisible) && (txtScriptPre.Text.IsNullOrStringEmpty()))
            {
                ttModeloScriptPre ttmodelo = new ttModeloScriptPre();
                ttmodelo.modelo = model;
                ttmodelo.extension = model.GetModelNameExtensionOverride();

                string sBuffer = String.Empty;

                try
                {
                    txtScriptPre.Text = SyntaxEditorHelper.PrettyPrint(ttmodelo.TransformText());
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

            if ((tabPagePost.PageVisible) && (txtScriptPost.Text.IsNullOrStringEmpty()))
            {
                try
                {
                    if (model.TipoModelo == WoTypeModel.TransactionContable)
                    {
                        ttModeloScriptContablePost ttmodelo = new ttModeloScriptContablePost();
                        ttmodelo.modelo = model;
                        ttmodelo.extension = model.GetModelNameExtensionOverride();
                        txtScriptPost.Text = SyntaxEditorHelper.PrettyPrint(
                            ttmodelo.TransformText()
                        );
                    }
                    else if (model.TipoModelo == WoTypeModel.TransactionNoContable)
                    {
                        ttModeloScriptNoContablePost ttmodelo = new ttModeloScriptNoContablePost();
                        ttmodelo.modelo = model;
                        ttmodelo.extension = model.GetModelNameExtensionOverride();
                        txtScriptPost.Text = SyntaxEditorHelper.PrettyPrint(
                            ttmodelo.TransformText()
                        );
                    }
                    else
                    {
                        ttModeloScriptPost ttmodelo = new ttModeloScriptPost();
                        ttmodelo.modelo = model;
                        ttmodelo.extension = model.GetModelNameExtensionOverride();
                        txtScriptPost.Text = SyntaxEditorHelper.PrettyPrint(
                            ttmodelo.TransformText()
                        );
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
                }
            }

            if (
                (txtModelPoliza.Text.IsNullOrStringEmpty())
                && (model.TipoModelo == WoTypeModel.TransactionContable)
            )
            {
                bool bCrearPoliza = true;
                if (
                    (model.SubTipoModelo == WoSubTypeModel.Extension)
                    || (model.SubTipoModelo == WoSubTypeModel.Override)
                )
                    if (
                        XtraMessageBox.Show(
                            "Sobre escribir la póliza Base?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) != DialogResult.Yes
                    )
                        bCrearPoliza = false;

                if (bCrearPoliza)
                {
                    ttModeloScriptContablePoliza ttmodelo = new ttModeloScriptContablePoliza();
                    ttmodelo.modelo = model;

                    try
                    {
                        txtModelPoliza.Text = SyntaxEditorHelper.PrettyPrint(
                            ttmodelo.TransformText()
                        );
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(
                            $"Error al generar Modelo Póliza {model.Id} {ex.Message}",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }

            if (txtModelCreation.Text.IsNullOrStringEmpty())
            {
                ttModeloScriptCreacion ttmodelo = new ttModeloScriptCreacion();
                ttmodelo.modelo = model;
                ttmodelo.extension = model.GetModelNameExtensionOverride();

                try
                {
                    txtModelCreation.Text = SyntaxEditorHelper.PrettyPrint(
                        ttmodelo.TransformText()
                    );
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
            txtModelPoliza.Document.IsReadOnly = false;
            txtModelCreation.Document.IsReadOnly = false;
            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.SnippetServidor;

            bInitPre = true;
            bInitPost = true;

            if ((tabCode.SelectedTabPage == tabPagePre) || (tabCode.SelectedTabPage == tabPagePost))
                buAnalisisTransiciones.PerformClick();
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

            if (tabPagePost.PageVisible)
                model.SavePreCondiciones(txtScriptPre.Text);

            if (tabPagePost.PageVisible)
                model.SavePostCondiciones(txtScriptPost.Text);

            if (tabModelPoliza.PageVisible)
                if (!txtModelPoliza.Text.IsNullOrStringEmpty())
                    model.SaveModelPoliza(txtModelPoliza.Text);

            if (tabModelCreation.PageVisible)
                model.SaveModelCreation(txtModelCreation.Text);

            drRow["Hecho"] =
                model.bPreCondicionesExist
                || model.bPostCondicionesExist
                || model.bModelPolizaExist
                || model.bModelCreationExist;

            buEditar.Enabled = true;
            buVisual.Enabled = true;
            buRegistrar.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buBorrar.Enabled = true;
            buRefrescar.Enabled = true;
            buSeleccionaExcepcion.Enabled = false;
            grpEditor.Enabled = false;
            txtFiltroApps.Enabled = true;

            buRefrescar.Enabled = true;
            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;

            txtScriptPre.Document.IsReadOnly = true;
            txtScriptPost.Document.IsReadOnly = true;
            txtModelPoliza.Document.IsReadOnly = true;
            txtModelCreation.Document.IsReadOnly = true;

            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.NoMostrar;
        }

        private bool ValidarScript()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                List<ScriptErrorDescriptor> alErrors;

                if (tabPagePre.PageVisible)
                {
                    syntaxEditorHelper.Validar(txtScriptPre.Text, out alErrors);

                    if (alErrors != null && alErrors.Count > 0)
                    {
                        tabCode.SelectedTabPage = tabPagePre;
                        RefreshErrors(alErrors);
                        throw new Exception("PreCondición tiene errores");
                    }
                }

                if (tabPagePost.PageVisible)
                {
                    syntaxEditorHelper.Validar(txtScriptPost.Text, out alErrors);

                    if (alErrors != null && alErrors.Count > 0)
                    {
                        var Error = alErrors
                            .Where(x =>
                                x.Description.Contains(
                                    "El nombre 'CreaPoliza' no existe en el contexto actual"
                                )
                            )
                            .ToList();
                        foreach (var item in Error)
                            alErrors.Remove(item);
                    }

                    if (alErrors != null && alErrors.Count > 0)
                    {
                        tabCode.SelectedTabPage = tabPagePost;
                        RefreshErrors(alErrors);
                        throw new Exception("PostCondición tiene errores");
                    }
                }

                if ((tabModelPoliza.PageVisible) && (!txtModelPoliza.Text.IsNullOrStringEmpty()))
                {
                    syntaxEditorHelper.Validar(txtModelPoliza.Text, out alErrors);

                    if (alErrors != null && alErrors.Count > 0)
                    {
                        tabCode.SelectedTabPage = tabModelPoliza;
                        RefreshErrors(alErrors);
                        throw new Exception("Póliza tiene errores");
                    }
                }

                if (tabModelCreation.PageVisible)
                {
                    syntaxEditorHelper.Validar(txtModelCreation.Text, out alErrors);

                    if (alErrors != null && alErrors.Count > 0)
                    {
                        tabCode.SelectedTabPage = tabModelCreation;
                        RefreshErrors(alErrors);
                        throw new Exception("Creación tiene errores");
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;

                if (
                    XtraMessageBox.Show(
                        ex.Message + "\r\n\r\nGrabar de cualquier manera?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes
                )
                    return false;
            }

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
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                List<ScriptErrorDescriptor> alErrors;
                syntaxEditorHelper.Validar(currEditor().Text, out alErrors);

                if (alErrors != null && alErrors.Count > 0)
                {
                    var Error = alErrors
                        .Where(x =>
                            x.Description.Contains(
                                "El nombre 'CreaPoliza' no existe en el contexto actual"
                            )
                        )
                        .ToList();
                    foreach (var item in Error)
                        alErrors.Remove(item);
                }

                if (alErrors != null && alErrors.Count > 0)
                    RefreshErrors(alErrors);
                else
                    lstErrors.Items.Clear();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void lstErrors_DoubleClick(object sender, EventArgs e)
        {
            if (lstErrors.FocusedItem != null)
            {
                string sLine = lstErrors.FocusedItem.SubItems[3].Text;
                if (sLine.Equals("-"))
                    return;

                string sColumn = lstErrors.FocusedItem.SubItems[4].Text;

                int iLine = Convert.ToInt32(sLine) - 1;
                if (iLine < 0)
                    return;

                int iColumn = Convert.ToInt32(sColumn) - 1;
                if (iColumn < 0)
                    iColumn = 1;

                currEditor().ActiveView.Selection.CaretPosition = new TextPosition(iLine, iColumn);
                currEditor().ActiveView.Scroller.ScrollLineToVisibleMiddle();
                currEditor().Focus();
            }
        }

        private void tabCode_SelectedPageChanged(
            object sender,
            DevExpress.XtraTab.TabPageChangedEventArgs e
        )
        {
            lstErrors.Items.Clear();

            if (
                ((tabCode.SelectedTabPage == tabPagePre) && bInitPre)
                || ((tabCode.SelectedTabPage == tabPagePost) && bInitPost)
            )
                buAnalisisTransiciones.PerformClick();
        }

        private void txtScriptPre_Click(object sender, EventArgs e) { }

        private void buSeleccionaExcepcion_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            var fmNE = new fmNumberedExceptionsSelector(true);

            if (fmNE.ShowDialog() == DialogResult.OK)
            {
                SyntaxEditor editor = currEditor();

                try
                {
                    Clipboard.SetDataObject(fmNE.NumeredExcepcion, true);
                    editor.ActiveView.PasteFromClipboard();
                    Clipboard.Clear();
                }
                catch { }
            }
        }

        private void buBorrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = grdModelosView.GetFocusedDataRow();

            if (dr.IsNull())
                return;

            if (
                XtraMessageBox.Show(
                    $"Borrar el script del modelo {dr["Modelo"].ToString()}?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            var Model = this
                .proyecto.ModeloCol.Modelos.Where(x => x.Id == dr["Modelo"].ToString())
                .FirstOrDefault();

            if (Model != null)
            {
                Model.DeletePreCondiciones();
                Model.DeletePostCondiciones();
                Model.DeleteModelCreation();
                Model.DeleteModelPoliza();
            }

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buComentar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currEditor().ActiveView.TextChangeActions.CommentLines();
        }

        private void buDescomentar_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            currEditor().ActiveView.TextChangeActions.UncommentLines();
        }

        private void buFormatear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currEditor().ActiveView.TextChangeActions.FormatDocument();
            currEditor().Text = SyntaxEditorHelper.PrettyPrint(currEditor().Text);
        }

        private void buAutocomplete_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            currEditor().ActiveView.IntelliPrompt.RequestAutoComplete();
        }

        private void barButtonItem1_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            currEditor().ActiveView.IntelliPrompt.RequestCompletionSession();
        }

        private void buParameter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currEditor().ActiveView.IntelliPrompt.RequestParameterInfoSession();
        }

        private void buQuickInfo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currEditor().ActiveView.IntelliPrompt.RequestQuickInfoSession();
        }

        private void txtScriptPre_DoubleClick(object sender, EventArgs e)
        {
            woGridModelo1.Filtra(txtScriptPre.ActiveView.SelectedText);
        }

        private void txtScriptPost_DoubleClick(object sender, EventArgs e)
        {
            woGridModelo1.Filtra(txtScriptPost.ActiveView.SelectedText);
        }

        private void txtCreacion_DoubleClick(object sender, EventArgs e)
        {
            woGridModelo1.Filtra(txtModelCreation.ActiveView.SelectedText);
        }

        private void txtFiltroApps_HiddenEditor(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            CargarModelo();
        }

        private void txtModelPoliza_DoubleClick(object sender, EventArgs e)
        {
            woGridModelo1.Filtra(txtModelPoliza.ActiveView.SelectedText);
        }

        private void buAnalisisTransiciones_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            bool bPrecondicion = true;
            string sCodigo = string.Empty;

            if (tabCode.SelectedTabPage == tabPagePre)
                sCodigo = txtScriptPre.Text;
            else if (tabCode.SelectedTabPage == tabPagePost)
            {
                sCodigo = txtScriptPost.Text;
                bPrecondicion = false;
            }
            else
                return;

            DataRow drRow = grdModelosView.GetFocusedDataRow();
            if (drRow.IsNull())
                return;

            var model = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            string Generado = string.Empty;

            if (bPrecondicion)
            {
                ttModeloScriptPre ttmodelo = new ttModeloScriptPre();
                ttmodelo.modelo = model;
                ttmodelo.extension = model.GetModelNameExtensionOverride();
                Generado = SyntaxEditorHelper.PrettyPrint(ttmodelo.TransformText());
                bInitPre = false;
            }
            else
            {
                ttModeloScriptPost ttmodelo = new ttModeloScriptPost();
                ttmodelo.modelo = model;
                ttmodelo.extension = model.GetModelNameExtensionOverride();
                Generado = SyntaxEditorHelper.PrettyPrint(ttmodelo.TransformText());
                bInitPost = false;
            }

            List<string> TransicionesFaltantes = new List<string>();

            StringBuilder sbFaltante = new StringBuilder();

            // Busca las regiones falantes
            foreach (var transicion in model.Diagrama.Transiciones)
            {
                string sRegion = $"#region {model.Id}{transicion.Id}";
                string sEndRegion = $"#endregion {model.Id}{transicion.Id}";

                int iPos = sCodigo.IndexOf(sRegion);
                if (iPos >= 0)
                    continue;

                TransicionesFaltantes.Add(transicion.Id);

                // Buscar pedazo de codigo
                sbFaltante.AppendLine(RecuperarRegion(Generado, sRegion, sEndRegion));
                sbFaltante.AppendLine();
            }

            // Agrega Borrar
            if (
                (bPrecondicion)
                && (
                    (model.TipoModelo == WoTypeModel.CatalogType)
                    || (model.TipoModelo == WoTypeModel.Catalog)
                    || (model.TipoModelo == WoTypeModel.Control)
                    || (model.TipoModelo == WoTypeModel.DataMart)
                )
            )
            {
                string sRegion = $"#region {model.Id}Borrar";
                string sEndRegion = $"#endregion {model.Id}Borrar";

                int iPos = sCodigo.IndexOf(sRegion);
                if (iPos < 0)
                {
                    TransicionesFaltantes.Add(@"Borrar");
                    // Buscar pedazo de codigo
                    sbFaltante.AppendLine(RecuperarRegion(Generado, sRegion, sEndRegion));
                    sbFaltante.AppendLine();
                }
            }

            if (TransicionesFaltantes.Count == 0)
            {
                // Busca las regiones sobrantes
                string[] Lineas = sCodigo.Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.None
                );

                List<string> TransicionesSobrantes = new List<string>();

                foreach (string linea in Lineas)
                {
                    string sRegion = $"#region {model.Id}";
                    int index = linea.IndexOf(sRegion);
                    if (index >= 0)
                    {
                        string Transicion = linea.Substring(index + sRegion.Length).Trim();
                        if (Transicion.IndexOf(" ") > 0)
                            Transicion = Transicion.Substring(0, Transicion.IndexOf(" "));

                        if (
                            model
                                .Diagrama.Transiciones.Where(x => x.Id == Transicion)
                                .FirstOrDefault() == null
                        )
                            if ((bPrecondicion) && (Transicion == "Borrar"))
                            {
                                if (
                                    !(
                                        (model.TipoModelo == WoTypeModel.CatalogType)
                                        || (model.TipoModelo == WoTypeModel.Catalog)
                                        || (model.TipoModelo == WoTypeModel.Control)
                                        || (model.TipoModelo == WoTypeModel.DataMart)
                                    )
                                )
                                    TransicionesSobrantes.Add(Transicion);
                            }
                            else
                            {
                                TransicionesSobrantes.Add(Transicion);
                            }
                    }
                }

                if (TransicionesSobrantes.Count > 0)
                {
                    XtraMessageBox.Show(
                        $"Sobran las transiciones {string.Join(",", TransicionesSobrantes)}, debe de quitarlas",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                return;
            }

            XtraMessageBox.Show(
                $"Faltan las transiciones {string.Join(",", TransicionesFaltantes)}, el código fue pegado en el portapapeles",
                "Verifique...",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            Clipboard.SetDataObject(sbFaltante.ToString(), true);
        }

        private string RecuperarRegion(string Generado, string Region, string EndRegion)
        {
            string[] Lineas = Generado.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None
            );

            StringBuilder sb = new StringBuilder();

            bool Inicio = false;
            foreach (string linea in Lineas)
            {
                if (Inicio)
                {
                    sb.AppendLine(linea);
                    if (linea.Contains(EndRegion))
                    {
                        return sb.ToString();
                    }
                }
                else if (linea.Contains(Region))
                {
                    sb.AppendLine(linea);
                    Inicio = true;
                }
            }

            return string.Empty;
        }

        private void woGridModelo1_Load(object sender, EventArgs e) { }

        private void buVisual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                grdModelosView_FocusedRowChanged(null, null);
                return;
            }

            if (!ValidarScript())
                return;

            var modelo = this
                .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            if (modelo == null)
            {
                XtraMessageBox.Show(
                    "Al parecer el modelo fue borrado, no es posible continuar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            string commandLineArgs = string.Empty;

            if (tabCode.SelectedTabPage == tabPagePre)
                commandLineArgs = modelo.PreConditionsFile;
            else if (tabCode.SelectedTabPage == tabPagePost)
                commandLineArgs = modelo.PostConditionsFile;
            else if (tabCode.SelectedTabPage == tabModelPoliza)
                commandLineArgs = modelo.ModelPolizaFile;
            else
                commandLineArgs = modelo.ModelCreationFile;

            Process process = woVisualStudio.AbreVisualStudio(commandLineArgs, true);

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buExpandir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currEditor().ActiveView.ExecuteEditAction(new MoveUpAction());
            currEditor().ActiveView.ExecuteEditAction(new ExpandAllOutliningAction());
            currEditor().ActiveView.Scroller.ScrollLineToVisibleMiddle();
            currEditor().ActiveView.ExecuteEditAction(new MoveDownAction());
        }

        private void buColapsar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currEditor().ActiveView.ExecuteEditAction(new MoveUpAction());
            currEditor().ActiveView.ExecuteEditAction(new ApplyDefaultOutliningExpansionAction());
            currEditor().ActiveView.Scroller.ScrollLineToVisibleMiddle();
            currEditor().ActiveView.ExecuteEditAction(new MoveDownAction());
        }

        private void txtBuscarTexto_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (txtBuscarTexto.EditValue.IsNullOrStringEmpty())
                return;

            string SearchText = txtBuscarTexto.EditValue.ToString();

            var drRow = grdModelosView.GetFocusedDataRow();
            var currentTab = tabCode.SelectedTabPage;

            bool Found = SearchTextInScript(SearchText, ref drRow, ref currentTab);

            drRow = null;
            currentTab = null;
            if (!Found)
                SearchTextInScript(SearchText, ref drRow, ref currentTab);
        }

        private bool SearchTextInScript(
            string SearchText,
            ref DataRow currentRow,
            ref XtraTabPage currentTab
        )
        {
            for (int i = 0; i < grdModelosView.DataRowCount; i++)
            {
                DataRow drRow = grdModelosView.GetDataRow(i);

                if (currentRow == drRow)
                {
                    currentRow = null;
                }
                else
                {
                    if (currentRow != null)
                        continue;
                }

                if (drRow == null)
                    continue;

                var Model = this
                    .proyecto.ModeloCol.Modelos.Where(x => x.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                if (Model == null)
                    continue;

                string[] Lineas;
                bool Found = false;
                if (Model.bPostCondicionesExist)
                {
                    if (currentTab == null)
                    {
                        Lineas = Model.GetPreCondiciones().Split('\n');
                        foreach (string Linea in Lineas)
                        {
                            if (Model.bPreCondicionesExist)
                            {
                                if (Linea.ToUpper().Contains(SearchText.ToUpper()))
                                {
                                    grdModelosView.FocusedRowHandle = i;
                                    tabCode.SelectedTabPage = tabPagePre;
                                    Found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (currentTab == tabPagePre)
                        currentTab = null;
                }

                if (!Found)
                {
                    if (Model.bPostCondicionesExist)
                    {
                        if (currentTab == null)
                        {
                            Lineas = Model.GetPostCondiciones().Split('\n');
                            foreach (string Linea in Lineas)
                            {
                                if (Linea.ToUpper().Contains(SearchText.ToUpper()))
                                {
                                    grdModelosView.FocusedRowHandle = i;
                                    tabCode.SelectedTabPage = tabPagePost;
                                    Found = true;
                                    break;
                                }
                            }
                        }
                        if (currentTab == tabPagePost)
                            currentTab = null;
                    }
                }

                if (!Found)
                {
                    if (Model.bModelPolizaExist)
                    {
                        if (currentTab == null)
                        {
                            Lineas = Model.GetModelPoliza().Split('\n');
                            foreach (string Linea in Lineas)
                            {
                                if (Linea.ToUpper().Contains(SearchText.ToUpper()))
                                {
                                    grdModelosView.FocusedRowHandle = i;
                                    tabCode.SelectedTabPage = tabModelPoliza;
                                    Found = true;
                                    break;
                                }
                            }
                        }
                        if (currentTab == tabModelPoliza)
                            currentTab = null;
                    }
                }

                if (!Found)
                {
                    if (Model.bModelCreationExist)
                    {
                        if (currentTab == null)
                        {
                            Lineas = Model.GetModelCreation().Split('\n');
                            foreach (string Linea in Lineas)
                            {
                                if (Linea.ToUpper().Contains(SearchText.ToUpper()))
                                {
                                    grdModelosView.FocusedRowHandle = i;
                                    tabCode.SelectedTabPage = tabModelCreation;
                                    Found = true;
                                    break;
                                }
                            }
                        }
                        if (currentTab == tabModelCreation)
                            currentTab = null;
                    }
                }

                if (Found)
                {
                    currEditor().Focus();
                    currEditor().SearchOptions.FindText = SearchText;

                    EditorSearchOptions options = new EditorSearchOptions();
                    options.FindText = SearchText;
                    options.PatternProvider = SearchPatternProviders.Normal;
                    ISearchResultSet resultSet = currEditor().ActiveView.Searcher.FindNext(options);

                    currEditor().ActiveView.Scroller.ScrollLineToVisibleMiddle();
                    return true;
                }
            }
            return false;
        }

        private void MostrarMetodoSeleccionado()
        {
            if (MarcandoTransicones)
                return;
            try
            {
                MarcandoTransicones = true;

                DataRow drRow = grdModelosView.GetFocusedDataRow();
                if (drRow.IsNull())
                    return;

                var modelo = this
                    .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                if (modelo == null)
                    return;

                var slTransiciones = new List<string>();
                woDiagram2.EngruesaTransicion(slTransiciones);

                try
                {
                    string source = string.Empty;
                    if (tabCode.SelectedTabPage == tabPagePre)
                    {
                        if (navigablePre.SelectedMemberSymbol == null)
                            return;
                        source = navigablePre.SelectedMemberSymbol.SnapshotRange.Text;
                    }
                    else if (tabCode.SelectedTabPage == tabPagePost)
                    {
                        if (navigablePost.SelectedMemberSymbol == null)
                            return;
                        source = navigablePost.SelectedMemberSymbol.SnapshotRange.Text;
                    }
                    if (source.IsNullOrStringEmpty())
                        return;

                    SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

                    var root = (CompilationUnitSyntax)tree.GetRoot();
                    var globalStatement = root.Members.FirstOrDefault();
                    if (globalStatement == null)
                        return;
                    if (!(globalStatement is GlobalStatementSyntax))
                        return;
                    var localFunction = (globalStatement as GlobalStatementSyntax)
                        .ChildNodes()
                        .FirstOrDefault();
                    if (localFunction == null)
                        return;
                    if (!(localFunction is LocalFunctionStatementSyntax))
                        return;
                    var functionStatament = (localFunction as LocalFunctionStatementSyntax);

                    if (!(localFunction is LocalFunctionStatementSyntax))
                        return;

                    var functionName = functionStatament.Identifier.Text;

                    if (functionName.IsNullOrStringEmpty())
                        return;

                    if (functionName.StartsWith(modelo.Id))
                    {
                        slTransiciones.Add(functionName.Substring(modelo.Id.Length));
                        woDiagram2.EngruesaTransicion(slTransiciones);
                    }
                    else
                    {
                        slTransiciones = ResuelveDondeOcupanMetodos(
                            functionName,
                            modelo.Id,
                            slTransiciones
                        );
                        woDiagram2.EngruesaTransicion(slTransiciones);
                    }
                }
                catch { }
            }
            finally
            {
                MarcandoTransicones = false;
            }
        }

        private List<string> ResuelveDondeOcupanMetodos(
            string functionName,
            string modeloId,
            List<string> slTransiciones
        )
        {
            SyntaxTree treeFull = CSharpSyntaxTree.ParseText(currEditor().Text);

            var root = (CompilationUnitSyntax)treeFull.GetRoot();
            var firstMember = root.Members.FirstOrDefault();
            if (!(firstMember is NamespaceDeclarationSyntax))
                return slTransiciones;

            var declarations = (firstMember as NamespaceDeclarationSyntax).ChildNodes();

            foreach (var declaration in declarations)
            {
                if (declaration is ClassDeclarationSyntax)
                {
                    foreach (
                        var declarationItem in (declaration as ClassDeclarationSyntax).ChildNodes()
                    )
                    {
                        if (!(declarationItem is MethodDeclarationSyntax))
                            continue;

                        var declarationToken = (declarationItem as MethodDeclarationSyntax)
                            .Identifier
                            .Text;

                        if (declarationToken.StartsWith(modeloId))
                        {
                            var declarationMethods = (
                                declarationItem as MethodDeclarationSyntax
                            ).ChildNodes();

                            foreach (var declarationMethod in declarationMethods)
                            {
                                if (!(declarationMethod is BlockSyntax))
                                    continue;

                                foreach (
                                    var blockItem in (declarationMethod as BlockSyntax).Statements
                                )
                                {
                                    if (!(blockItem is ExpressionStatementSyntax))
                                        continue;

                                    var blockStatament = (
                                        blockItem as ExpressionStatementSyntax
                                    ).Expression;

                                    if (!(blockStatament is InvocationExpressionSyntax))
                                        continue;

                                    var expresssionName = (
                                        blockStatament as InvocationExpressionSyntax
                                    );

                                    if (expresssionName.Expression is IdentifierNameSyntax)
                                    {
                                        var member = (
                                            expresssionName.Expression as IdentifierNameSyntax
                                        );

                                        // Es el metodo buscado
                                        if (functionName == member.Identifier.Text)
                                            slTransiciones.Add(
                                                declarationToken.Substring(modeloId.Length)
                                            );
                                    }
                                    else if (expresssionName.Expression is GenericNameSyntax)
                                    {
                                        var member = (
                                            expresssionName.Expression as GenericNameSyntax
                                        );

                                        // Es el metodo buscado
                                        if (functionName == member.Identifier.Text)
                                            slTransiciones.Add(
                                                declarationToken.Substring(modeloId.Length)
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return slTransiciones;
        }

        private void optMarcarTransiciones_EditValueChanged(object sender, EventArgs e)
        {
            if (optMarcarTransiciones.EditValue.ToBoolean())
                MostrarMetodoSeleccionado();
            else
                woDiagram2.EngruesaTransicion(new List<string>());
        }

        private void mnuSnippetException_Click(object sender, EventArgs e)
        {
            setSnippet(@"throw new Exception(string.Format(""""));");
        }
    }
}
