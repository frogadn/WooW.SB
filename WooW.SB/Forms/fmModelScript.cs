using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using ServiceStack;
using WooW.Core;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Config.Templates;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmModelScript : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        private string AssemblyPath;

        public Proyecto proyecto { get; set; }

        private IProjectAssembly projectAssembly;
        private BackgroundWorker _worker = new BackgroundWorker();
        private SyntaxEditorHelper syntaxEditorHelper;

        private List<string> ReferenciasScript = new List<string>()
        {
            "DynamicODataToSQL.dll",
            "FastMember.dll",
            "Microsoft.OData.Core.dll",
            "Microsoft.OData.Edm.dll",
            "Microsoft.Spatial.dll",
            "Microsoft.IO.RecyclableMemoryStream.dll",
            "Newtonsoft.Json.dll",
            "ServiceStack.Client.dll",
            "ServiceStack.Common.dll",
            "ServiceStack.dll",
            "ServiceStack.Interfaces.dll",
            "ServiceStack.OrmLite.dll",
            "ServiceStack.OrmLite.Sqlite.dll",
            "ServiceStack.Redis.dll",
            "ServiceStack.Server.dll",
            "ServiceStack.Text.dll",
            "SqlKata.dll",
            "GemBox.Document.dll",
            "MimeKit.dll",
            "NPOI.Core.dll",
            "WooW.Resources.dll",
            "WooW.Core.Common.dll",
            "WooW.Core.Server.dll",
            "WooW.WebService.dll",
            "WooW.CFDI.dll"
        };

        private List<string> SystemReferenciasScript = new List<string>()
        {
            "System.RunTime",
            "mscorlib",
            "System",
            "System.Core",
            "System.Data",
            "System.Data.Common",
            "System.Drawing",
            "System.Text.RegularExpressions",
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

        public fmModelScript()
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
                SystemReferenciasScript,
                AssemblyPath,
                ReferenciasScript
            );

            txtScript.ContextMenuStrip = contextScriptMenu;
            txtScript.IsDefaultContextMenuEnabled = false;
        }

        public void setSnippet(string snippet)
        {
            SyntaxEditor editor = txtScript;

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

        internal void ExecuteResolver()
        {
            var language = new CSharpSyntaxLanguage();
            projectAssembly = language.GetService<IProjectAssembly>();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += ChargeDlls;
            _worker.RunWorkerAsync();

            txtScript.Document.Language = language;
            var formatterCreacion = (CSharpTextFormatter)
                txtScript.Document.Language.GetTextFormatter();
            formatterCreacion.IsOpeningBraceOnNewLine = true;
        }

        private void ChargeDlls(object sender, EventArgs e)
        {
            var references = new List<string>();

            string corePath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            foreach (var Referencia in SystemReferenciasScript)
                references.Add(Path.Combine(corePath, Referencia) + ".dll");

            foreach (var Referencia in ReferenciasScript)
                references.Add(Path.Combine(AssemblyPath, Referencia));

            projectAssembly.AssemblyReferences.AddMsCorLib();
            /// System
            foreach (var dll in references)
            {
                projectAssembly.AssemblyReferences.AddFrom(dll);
            }
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
            txtResponse.Properties.Items.Clear();

            foreach (
                var Modelo in this.proyecto.ModeloCol.Modelos.Where(e =>
                    e.TipoModelo == WoTypeModel.Response
                )
            )
                txtResponse.Properties.Items.Add(Modelo.Id);

            DataTable dt = new DataTable();

            dt.Columns.Add(@"Proceso", typeof(string));
            dt.Columns.Add(@"Tipo", typeof(string));
            dt.Columns.Add(@"Hecho", typeof(bool));
            dt.Columns.Add(@"Modelo", typeof(string));

            foreach (
                var modelo in this
                    .proyecto.ModeloCol.Modelos.Where(e =>
                        (
                            (e.TipoModelo == WoTypeModel.Request)
                            || (e.TipoModelo == WoTypeModel.Interface)
                            || (e.TipoModelo == WoTypeModel.Class)
                        )
                    )
                    .OrderBy(e => e.Id)
                    .ToList()
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
                drRow["Hecho"] = modelo.bScriptExist;
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
            buEditar.Enabled = false;
            buVisual.Enabled = false;
            buRegistrar.Enabled = false;
            buSeleccionaExcepcion.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buBorrar.Enabled = false;
            buRefrescar.Enabled = true;

            grpEditor.Enabled = false;
            pnlPropiedades.Enabled = false;
            txtFiltroApps.Enabled = true;

            txtScript.Document.IsReadOnly = true;
            txtScript.Text = String.Empty;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            if (drRow.IsNull())
            {
                return;
            }

            var Model = this
                .proyecto.ModeloCol.Modelos.Where(x => x.Id == drRow["Modelo"].ToString())
                .FirstOrDefault();

            txtScript.Text = Model.GetScript();

            txtResponse.EditValue = Model.ScriptRequest.ResponseId;
            optColeccion.Checked = Model.ScriptRequest.Coleccion;
            optBackGround.Checked = Model.ScriptRequest.EjecutarEnBackGround;

            if (
                (Model.TipoModelo == WoTypeModel.Request)
                && (
                    (Model.SubTipoModelo == WoSubTypeModel.DataService)
                    || (Model.SubTipoModelo == WoSubTypeModel.MicroService)
                )
                && (!Model.ScriptRequest.EjecutarEnBackGround)
            )
            {
                optUsarGet.Checked = Model.ScriptRequest.UsaGetEnRequest;
                optUsarPost.Checked = Model.ScriptRequest.UsaPostEnRequest;
                optUsarPut.Checked = Model.ScriptRequest.UsaPutEnRequest;
                optUsarPatch.Checked = Model.ScriptRequest.UsaPatchEnRequest;
                optUsarDelete.Checked = Model.ScriptRequest.UsaDeleteEnRequest;
            }
            else
            {
                optUsarGet.Checked = false;
                optUsarPost.Checked = false;
                optUsarPut.Checked = false;
                optUsarPatch.Checked = false;
                optUsarDelete.Checked = false;
            }

            buEditar.Enabled = true;
            txtBuscarTexto.Enabled = true;
            buVisual.Enabled = true;
            buBorrar.Enabled = true;
            grpEditor.Enabled = false;
        }

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

            if (txtScript.Text.IsNullOrStringEmpty())
            {
                fmModelRequestNew fm = new fmModelRequestNew(model);

                if (
                    (model.TipoModelo != WoTypeModel.Interface)
                    && (model.TipoModelo != WoTypeModel.Class)
                )
                {
                    if (fm.ShowDialog() != DialogResult.OK)
                        return;
                }

                try
                {
                    if (model.TipoModelo == WoTypeModel.Interface)
                    {
                        ttInterfaceScript ttinterfaceScript = new ttInterfaceScript();
                        ttinterfaceScript.request = model;
                        txtScript.Text = SyntaxEditorHelper.PrettyPrint(
                            ttinterfaceScript.TransformText()
                        );
                    }
                    else if (fm.BackGround)
                    {
                        ttModeloScriptTarea ttmodeloscript = new ttModeloScriptTarea();
                        ttmodeloscript.request = model;
                        ttmodeloscript.responseId = fm.Response;
                        ttmodeloscript.coleccion = fm.Coleccion;
                        txtScript.Text = SyntaxEditorHelper.PrettyPrint(
                            ttmodeloscript.TransformText()
                        );
                    }
                    else if (model.TipoModelo == WoTypeModel.Class)
                    {
                        string ParentClass = string.Empty;
                        if (!model.Interface1.IsNullOrStringEmpty())
                            if (ParentClass.IsNullOrStringEmpty())
                                ParentClass = $": {model.Interface1}";
                            else
                                ParentClass = $"{ParentClass}, {model.Interface1}";

                        if (!model.Interface2.IsNullOrStringEmpty())
                            if (ParentClass.IsNullOrStringEmpty())
                                ParentClass = $": {model.Interface2}";
                            else
                                ParentClass = $"{ParentClass}, {model.Interface2}";

                        if (!model.Interface3.IsNullOrStringEmpty())
                            if (ParentClass.IsNullOrStringEmpty())
                                ParentClass = $": {model.Interface3}";
                            else
                                ParentClass = $"{ParentClass}, {model.Interface3}";

                        if (
                            (model.SubTipoModelo == WoSubTypeModel.Standard)
                            || (model.SubTipoModelo == WoSubTypeModel.Extension)
                            || (model.SubTipoModelo == WoSubTypeModel.Override)
                        )
                        {
                            ttClassStandard ttclassStandard = new ttClassStandard();
                            ttclassStandard.classScript = model;
                            ttclassStandard.ParentClass = ParentClass;
                            txtScript.Text = SyntaxEditorHelper.PrettyPrint(
                                ttclassStandard.TransformText()
                            );
                        }
                        else if (model.SubTipoModelo == WoSubTypeModel.Static)
                        {
                            ttClassStatic ttclassStatic = new ttClassStatic();
                            ttclassStatic.classScript = model;
                            txtScript.Text = SyntaxEditorHelper.PrettyPrint(
                                ttclassStatic.TransformText()
                            );
                        }
                        else if (model.SubTipoModelo == WoSubTypeModel.Singleton)
                        {
                            ttClassSingleton ttclassSingleton = new ttClassSingleton();
                            ttclassSingleton.classScript = model;
                            ttclassSingleton.ParentClass = ParentClass;
                            txtScript.Text = SyntaxEditorHelper.PrettyPrint(
                                ttclassSingleton.TransformText()
                            );
                        }
                    }
                    else
                    {
                        ttModeloScript ttmodeloscript = new ttModeloScript();
                        ttmodeloscript.request = model;
                        ttmodeloscript.responseId = fm.Response;

                        ttmodeloscript.coleccion = fm.Coleccion;
                        txtScript.Text = SyntaxEditorHelper.PrettyPrint(
                            ttmodeloscript.TransformText()
                        );
                        optUsarGet.Checked = true;
                        optUsarPost.Checked = false;
                        optUsarPut.Checked = false;
                        optUsarPatch.Checked = false;
                        optUsarDelete.Checked = false;
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
            txtBuscarTexto.Enabled = false;
            buVisual.Enabled = false;
            buRefrescar.Enabled = false;
            buRegistrar.Enabled =
                (drRow[@"Modelo"].ToSafeString().StartsWith(wooWConfigParams.Origen))
                || (
                    (drRow[@"Tipo"].ToSafeString() == WoTypeModel.Interface.GetDescription())
                    && (
                        drRow[@"Modelo"]
                            .ToSafeString()
                            .Substring(1)
                            .StartsWith(wooWConfigParams.Origen)
                    )
                )
                || (model.SubTipoModelo == WoSubTypeModel.Extension)
                || (model.SubTipoModelo == WoSubTypeModel.Override);

            buSeleccionaExcepcion.Enabled = true;
            buAnular.Enabled = true;
            buBorrar.Enabled = false;
            buCompilar.Enabled = true;
            grpEditor.Enabled = true;
            txtFiltroApps.Enabled = false;

            optColeccion.Enabled = false;
            optBackGround.Enabled = false;

            if (
                (model.TipoModelo == WoTypeModel.Request)
                && (
                    (model.SubTipoModelo == WoSubTypeModel.DataService)
                    || (model.SubTipoModelo == WoSubTypeModel.MicroService)
                )
                && (!model.ScriptRequest.EjecutarEnBackGround)
            )
            {
                optUsarGet.Enabled = true;
                optUsarPost.Enabled = true;
                optUsarPut.Enabled = true;
                optUsarPatch.Enabled = true;
                optUsarDelete.Enabled = true;
            }
            else
            {
                optUsarGet.Checked = false;
                optUsarPost.Checked = false;
                optUsarPut.Checked = false;
                optUsarPatch.Checked = false;
                optUsarDelete.Checked = false;
                optUsarGet.Enabled = false;
                optUsarPost.Enabled = false;
                optUsarPut.Enabled = false;
                optUsarPatch.Enabled = false;
                optUsarDelete.Enabled = false;
            }

            txtResponse.Enabled = false;

            pnlPropiedades.Enabled =
                (model.TipoModelo != WoTypeModel.Interface)
                && (model.TipoModelo != WoTypeModel.Class);

            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            grpCambios.Enabled = false;

            txtScript.Document.IsReadOnly = false;
            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.SnippetServidor;
        }

        private bool ValidarScript(Modelo model)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (optUsarGet.Checked)
                    if (!txtScript.Text.Contains($"{model.Id}Get<REQ, RES>"))
                        throw new Exception("No se encontró el método para el verbo Get");
                if (optUsarPost.Checked)
                    if (!txtScript.Text.Contains($"{model.Id}Post<REQ, RES>"))
                        throw new Exception("No se encontró el método para el verbo Post");
                if (optUsarPut.Checked)
                    if (!txtScript.Text.Contains($"{model.Id}Put<REQ, RES>"))
                        throw new Exception("No se encontró el método para el verbo Put");
                if (optUsarPatch.Checked)
                    if (!txtScript.Text.Contains($"{model.Id}Patch<REQ, RES>"))
                        throw new Exception("No se encontró el método para el verbo Patch");
                if (optUsarDelete.Checked)
                    if (!txtScript.Text.Contains($"{model.Id}Delete<REQ, RES>"))
                        throw new Exception("No se encontró el método para el verbo Delete");

                List<ScriptErrorDescriptor> alErrors;
                syntaxEditorHelper.Validar(txtScript.Text, out alErrors);

                if (alErrors != null && alErrors.Count > 0)
                {
                    RefreshErrors(alErrors);
                    throw new Exception("Script tiene errores");
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
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

        private void buRegistrar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

            if (!ValidarScript(modelo))
                return;

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

            if (
                (modelo.TipoModelo != WoTypeModel.Interface)
                && (modelo.TipoModelo != WoTypeModel.Class)
            )
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

                modelo.ScriptRequest.ResponseId = txtResponse.EditValue.ToSafeString();

                modelo.ScriptRequest.ResponseId = txtResponse.EditValue.ToSafeString();
                modelo.ScriptRequest.Coleccion = optColeccion.Checked;
                modelo.ScriptRequest.EjecutarEnBackGround = optBackGround.Checked;
                modelo.ScriptRequest.UsaGetEnRequest = optUsarGet.Checked;
                modelo.ScriptRequest.UsaPostEnRequest = optUsarPost.Checked;
                modelo.ScriptRequest.UsaPutEnRequest = optUsarPut.Checked;
                modelo.ScriptRequest.UsaPatchEnRequest = optUsarPatch.Checked;
                modelo.ScriptRequest.UsaDeleteEnRequest = optUsarDelete.Checked;
            }
            modelo.SaveScript(txtScript.Text);

            drRow["Hecho"] = modelo.bScriptExist;

            proyecto.SaveModel(modelo);

            buEditar.Enabled = true;
            txtBuscarTexto.Enabled = true;
            buVisual.Enabled = true;
            buRegistrar.Enabled = false;
            buSeleccionaExcepcion.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buRefrescar.Enabled = true;
            buBorrar.Enabled = true;
            grpEditor.Enabled = false;
            txtFiltroApps.Enabled = true;

            optUsarGet.Enabled = false;
            optUsarPost.Enabled = false;
            optUsarPut.Enabled = false;
            optUsarPatch.Enabled = false;
            optUsarDelete.Enabled = false;

            buRefrescar.Enabled = true;
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
            txtBuscarTexto.Enabled = true;
            buVisual.Enabled = true;
            buRegistrar.Enabled = false;
            buSeleccionaExcepcion.Enabled = false;
            buAnular.Enabled = false;
            buCompilar.Enabled = false;
            buBorrar.Enabled = true;
            grpEditor.Enabled = false;
            txtFiltroApps.Enabled = true;
            optUsarGet.Enabled = false;
            optUsarPost.Enabled = false;
            optUsarPut.Enabled = false;
            optUsarPatch.Enabled = false;
            optUsarDelete.Enabled = false;

            splitControl.PanelVisibility = SplitPanelVisibility.Both;
            grpCambios.Enabled = true;
        }

        private void buCompilar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                List<ScriptErrorDescriptor> alErrors;
                syntaxEditorHelper.Validar(txtScript.Text, out alErrors);

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

                string sColumn = lstErrors.FocusedItem.SubItems[4].Text;

                int iLine = Convert.ToInt32(sLine) - 1;
                if (iLine < 0)
                    return;

                int iColumn = Convert.ToInt32(sColumn) - 1;
                if (iColumn < 0)
                    iColumn = 1;

                txtScript.ActiveView.Selection.CaretPosition = new TextPosition(iLine, iColumn);
                txtScript.ActiveView.Scroller.ScrollLineToVisibleMiddle();
                txtScript.Focus();
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
                Model.DeleteScript();
            }

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buSeleccionaExcepcion_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            var fmNE = new fmNumberedExceptionsSelector(true);

            if (fmNE.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Clipboard.SetDataObject(fmNE.NumeredExcepcion, true);
                    txtScript.ActiveView.PasteFromClipboard();
                    Clipboard.Clear();
                }
                catch { }
            }
        }

        private void buComentar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtScript.ActiveView.TextChangeActions.CommentLines();
        }

        private void buDescomentar_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            txtScript.ActiveView.TextChangeActions.UncommentLines();
        }

        private void buFormatear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int iLine = txtScript.ActiveView.CurrentViewLine.StartPosition.DisplayLine;
            int iColumn = txtScript.ActiveView.Selection.CaretCharacterColumn;

            txtScript.ActiveView.TextChangeActions.FormatDocument();
            txtScript.Text = SyntaxEditorHelper.PrettyPrint(txtScript.Text);

            txtScript.ActiveView.Selection.CaretPosition = new TextPosition(iLine - 1, iColumn);
            txtScript.ActiveView.Scroller.ScrollLineToVisibleMiddle();
            txtScript.Focus();
        }

        private void buAutocomplete_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            txtScript.ActiveView.IntelliPrompt.RequestAutoComplete();
        }

        private void buRequestComplete_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            txtScript.ActiveView.IntelliPrompt.RequestCompletionSession();
        }

        private void buParameter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtScript.ActiveView.IntelliPrompt.RequestParameterInfoSession();
        }

        private void buQuickInfo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtScript.ActiveView.IntelliPrompt.RequestQuickInfoSession();
        }

        private void txtScript_DoubleClick(object sender, EventArgs e)
        {
            woGridModelo1.Filtra(txtScript.ActiveView.SelectedText);
        }

        private void txtFiltroApps_HiddenEditor(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            CargarModelo();
        }

        private void buVisual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

            if (!ValidarScript(modelo))
                return;

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

            string commandLineArgs = modelo.ScriptFile;

            Process process = woVisualStudio.AbreVisualStudio(commandLineArgs, true);

            grdModelosView_FocusedRowChanged(null, null);
        }

        private void buExpandir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtScript.ActiveView.ExecuteEditAction(new MoveUpAction());
            txtScript.ActiveView.ExecuteEditAction(new ExpandAllOutliningAction());
            txtScript.ActiveView.Scroller.ScrollLineToVisibleMiddle();
            txtScript.ActiveView.ExecuteEditAction(new MoveDownAction());
        }

        private void buColapsar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtScript.ActiveView.ExecuteEditAction(new MoveUpAction());
            txtScript.ActiveView.ExecuteEditAction(new ApplyDefaultOutliningExpansionAction());
            txtScript.ActiveView.Scroller.ScrollLineToVisibleMiddle();
            txtScript.ActiveView.ExecuteEditAction(new MoveDownAction());
        }

        private void txtBuscarTexto_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (txtBuscarTexto.EditValue.IsNullOrStringEmpty())
                return;

            string SearchText = txtBuscarTexto.EditValue.ToString();

            DataRow drRow = grdModelosView.GetFocusedDataRow();

            bool Found = SearchTextInScript(SearchText, ref drRow);

            drRow = null;
            if (!Found)
                SearchTextInScript(SearchText, ref drRow);
        }

        private bool SearchTextInScript(string SearchText, ref DataRow currentRow)
        {
            for (int i = 0; i < grdModelosView.DataRowCount; i++)
            {
                DataRow drRow = grdModelosView.GetDataRow(i);

                if (currentRow == drRow)
                {
                    currentRow = null;
                    continue;
                }

                if (currentRow != null)
                    continue;

                if (drRow == null)
                    continue;

                var Model = this
                    .proyecto.ModeloCol.Modelos.Where(x => x.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                if (Model == null)
                    continue;

                string[] Lineas = Model.GetScript().Split('\n');
                foreach (string Linea in Lineas)
                {
                    if (Linea.ToUpper().Contains(SearchText.ToUpper()))
                    {
                        grdModelosView.FocusedRowHandle = i;

                        txtScript.Focus();
                        txtScript.SearchOptions.FindText = SearchText;

                        EditorSearchOptions options = new EditorSearchOptions();
                        options.FindText = SearchText;
                        options.PatternProvider = SearchPatternProviders.Normal;
                        ISearchResultSet resultSet = txtScript.ActiveView.Searcher.FindNext(
                            options
                        );

                        txtScript.ActiveView.Scroller.ScrollLineToVisibleMiddle();
                        return true;
                    }
                }
            }
            return false;
        }

        private void mnuSnippetException_Click(object sender, EventArgs e)
        {
            setSnippet(@"throw new Exception(string.Format(""""));");
        }

        private void mnuFormatear_Click(object sender, EventArgs e)
        {
            if (txtScript.Document.IsReadOnly)
                return;
            buFormatear.PerformClick();
        }

        private void optUsarGet_CheckedChanged(object sender, EventArgs e)
        {
            if (txtScript.Document.IsReadOnly)
                return;

            CheckEdit checkButton = (sender as CheckEdit);

            if (checkButton.Checked)
            {
                DataRow drRow = grdModelosView.GetFocusedDataRow();

                if (drRow.IsNull())
                    return;

                var model = this
                    .proyecto.ModeloCol.Modelos.Where(j => j.Id == drRow["Modelo"].ToString())
                    .FirstOrDefault();

                if (model == null)
                    return;

                string Verb = string.Empty;
                if (checkButton == optUsarGet)
                    Verb = "Get";
                else if (checkButton == optUsarPost)
                    Verb = "Post";
                else if (checkButton == optUsarPut)
                    Verb = "Put";
                else if (checkButton == optUsarPatch)
                    Verb = "Patch";
                else if (checkButton == optUsarDelete)
                    Verb = "Delete";
                else
                    return;

                bool bFound = txtScript.Text.Contains($"{model.Id}{Verb}<REQ, RES>");

                if (bFound)
                    return;

                string coleccion = model.ScriptRequest.Coleccion ? "IList<RES>" : "RES";
                string coleccionRes = model.ScriptRequest.Coleccion
                    ? $"IList<{model.ScriptRequest.ResponseId}>"
                    : model.ScriptRequest.ResponseId;

                string Snippet =
                    $@"		//
		// Método para el request {model.Id} verbo {Verb}
		//
		public {coleccion} {model.Id}{Verb}<REQ, RES>(WoTargetConnection woTarget, REQ req{model.Id} )
		    where REQ : {model.Id}, new()
		    where RES : {coleccionRes}, new()
		{{
		    return null;
		}}";

                Clipboard.SetDataObject(Snippet, true);
                XtraMessageBox.Show(
                    text: $@"El snippet para el verbo {Verb} fue copiado al portapapeles, peguelo en el editor",
                    caption: "Verifique",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Information
                );
            }
        }
    }
}
