using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using ActiproSoftware.Text.Languages.DotNet.Reflection;
using ActiproSoftware.Text.Searching;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.EditActions;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.Implementation;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.CodeAnalysis;
using ServiceStack;
using WooW.Core;
using WooW.Core.Server;
using WooW.SB.BlazorTestGenerator.Components;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Config.Templates;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;
using WooW.Socket;

namespace WooW.SB.Forms
{
    // ToDo Drag Drop para mover casos prueba

    public partial class fmTestCasesOld : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }
        private int IContadorBach = 0;
        private object LockContadorBach = new object();

        private int iExitCode = 0;

        private ServSock _ServSock = null;

        private bool CriticalError = false;

        public Proyecto proyecto { get; set; }

        private RichTextBox txtLog = null;
        private bool bNuevo = false;

        private string AssemblyPath;

        private IProjectAssembly projectAssembly;
        private IProjectAssembly projectAssemblyApp;

        private BackgroundWorker _worker = new BackgroundWorker();
        private BackgroundWorker _workerApp = new BackgroundWorker();

        private SyntaxEditorHelper syntaxEditorHelper;
        private SyntaxEditorHelper syntaxEditorHelperApp;

        private JsonApiClient woTarget = null;
        private fmTestCasesSelector TestCasesSelector = null;
        private fmTestIntegralSelector TestIntegralSelector = null;

        DevExpress.XtraTreeList.TreeList currentPopTreeList = null;

        #region Ensamblados Syntax Editor

        private List<string> Referencias = new List<string>()
        {
            "Newtonsoft.Json.dll",
            "ServiceStack.Client.dll",
            "ServiceStack.Common.dll",
            "ServiceStack.Interfaces.dll",
            "ServiceStack.Text.dll",
            "GemBox.Document.dll",
            "MimeKit.dll",
            "WooW.CFDI.dll",
            "WooW.Resources.dll",
            "WooW.Core.Common.dll",
            "WooW.WebClient.dll"
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
            "System.ComponentModel.Primitives",
            "System.Diagnostics.Process",
            "System.Text.RegularExpressions",
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
            "System.Xml.ReaderWriter",
            "System.Private.Xml"
        };

        private List<string> ReferenciasApp = new List<string>()
        {
            "DynamicODataToSQL.dll",
            "FastMember.dll",
            "Microsoft.OData.Core.dll",
            "Microsoft.OData.Edm.dll",
            "Microsoft.Spatial.dll",
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
            "WooW.Resources.dll",
            "WooW.Core.Common.dll",
            "WooW.Core.Server.dll"
        };

        private List<string> SystemReferenciasApp = new List<string>()
        {
            "System.RunTime",
            "mscorlib",
            "System",
            "System.Core",
            "System.Data",
            "System.Data.Common",
            "System.Drawing",
            "System.Text.RegularExpressions",
            "System.Linq",
            "System.Linq.Expressions",
            "System.Private.CoreLib",
            "System.Reflection",
            "System.Collections",
            "System.IO.Compression.FileSystem",
            "System.Numerics",
            "System.RunTime.Serialization",
            "System.Xml",
            "System.Xml.Linq"
        };

        #endregion Ensamblados Syntax Editor


        int iConsoleErrorsNumLog = 0;
        int iConsoleErrorsNumWarningLog = 0;
        int iConsoleErrorsNumErrorLog = 0;
        int iConsoleErrorsNumTitleLog = 0;
        int iConsoleErrorsNumCriticalLog = 0;
        int iConsoleErrorsNumSuccessLog = 0;

        string CurrentApp = string.Empty;

        public fmTestCasesOld()
        {
            InitializeComponent();

            tabScript.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;

            Disposed += OnDispose;

            proyecto = Proyecto.getInstance();

            //
            // Llena el combo con la base de datos disponibles
            //
            string DataBase = WoLib.GetDBName(
                proyecto.ParConexion.Instance,
                proyecto.ParConexion.InstanceType,
                proyecto.ParConexion.Year
            );

            string result = EjecutarOperacionDB(
                new WoDataBaseOperations()
                {
                    Accion = WooW.Core.Common.tWoDataBaseAccion.ListBackup,
                    DataBaseName = DataBase,
                },
                false
            );

            repDBaRecuperar.Items.Clear();

            repDBaRecuperar.Items.AddRange(
                result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            );

            if (repDBaRecuperar.Items.IndexOf(proyecto.ParConexion.DBRestore) != -1)
                txtDBaRecuperar.EditValue = proyecto.ParConexion.DBRestore;

            // Recupera una persistencia
            txtEjecutarAccion.EditValue = proyecto.ParConexion.DBAccion;
            txtEjecutarAccionApp.EditValue = proyecto.ParConexion.DBAccionApp;

            CurrentApp = proyecto.ParConexion.UnitTestApp;
            if (!CurrentApp.IsNullOrStringEmpty())
                ReferenciasApp[ReferenciasApp.Count - 1] = $"WooW.{CurrentApp}.dll";

            lblAppActual.Caption = "App actual: " + CurrentApp;

            //
            // Configura el SyntaxEditor
            //
            AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            syntaxEditorHelper = new SyntaxEditorHelper(
                SystemReferencias,
                AssemblyPath,
                Referencias
            );

            syntaxEditorHelperApp = new SyntaxEditorHelper(
                SystemReferenciasApp,
                AssemblyPath,
                ReferenciasApp
            );

            tabPrincipal.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;

            txtProceso.Properties.Items.Clear();

            foreach (var Proceso in proyecto.Procesos)
                txtProceso.Properties.Items.Add(Proceso.Id);

            woGridModelo1.MostraSoloDiagrama();
            woGridModelo1.SetSnippet = setSnippet;

            txtAño.EditValue = DateTime.Today.Year;

            //
            // Configura la comunicacion entre una aplicacion externa y la consola
            //

            _ServSock = new ServSock();
            _ServSock.EventGetData += GetData;
            Thread thread = new Thread(new ThreadStart(_ServSock.Run));
            thread.Start();

            try
            {
                Directory.Delete($"{proyecto.Dir}\\Temp", true);
            }
            catch { }

            txtModoEjecucion.EditValue = repModoEjecucion.Items[0];

            txtScript.ContextMenuStrip = contextScriptMenu;
            txtScript.IsDefaultContextMenuEnabled = false;
        }

        private void GetData(object sender, string data)
        {
            EscribeEnLaConsola(data);
        }

        public void setSnippet(string snippet)
        {
            if (GetCurrentScriptEditor().Document.IsReadOnly)
                return;

            try
            {
                Clipboard.SetDataObject(snippet, true);
                GetCurrentScriptEditor().ActiveView.PasteFromClipboard();
                Clipboard.Clear();
            }
            catch { }
        }

        private void OnDispose(object sender, EventArgs e)
        {
            _ServSock.Stop();
            projectAssembly.AssemblyReferences.Clear();
        }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return (buAceptarCambios.Enabled || buRegistrarPruebaIntegral.Enabled); }
        }

        public string Nombre
        {
            get { return ribbonPageUnitarias.Text; }
        }

        public void Refrescar()
        {
            if (buRefrescar.Enabled)
                buRefrescar.PerformClick();
        }

        public void Cargar()
        {
            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;

            DataSetPruebasIntegrales();

            if (!CargarDirectorioUnitaria())
                return;

            if (!CargarDirectorioIntegral())
                return;

            ExecuteResolver();
            ExecuteResolverApp();
        }

        private void DataSetPruebasIntegrales()
        {
            //grdElementoPrueba
            DataTable dtDet = new DataTable();

            dtDet.Columns.Add(@"Orden", typeof(int));
            dtDet.Columns.Add(@"Tipo", typeof(string));
            dtDet.Columns.Add(@"PruebaUnitaria", typeof(string));
            dtDet.Columns.Add(@"Diagnostico", typeof(string));

            grdElementoPrueba.DataSource = dtDet;

            GridColumn col = grdElementoPruebaView.Columns[@"Orden"];
            col.Width = 50;
            grdElementoPruebaView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdElementoPruebaView.Columns[@"PruebaUnitaria"];
            col.Width = 200;
            col.Caption = "Prueba Unitaria";

            RepositoryItemButtonEdit txtCasoPrueba = new RepositoryItemButtonEdit();
            txtCasoPrueba.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            //txtCasoPrueba.Buttons.Add(
            //    new DevExpress.XtraEditors.Controls.EditorButton(
            //        DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis
            //    )
            //);
            txtCasoPrueba.ButtonClick += TxtCasoPrueba_ButtonClick;
            col.ColumnEdit = txtCasoPrueba;

            col = grdElementoPruebaView.Columns[@"Diagnostico"];
            col.Width = 200;
            col.OptionsColumn.AllowEdit = false;

            col = grdElementoPruebaView.Columns[@"Tipo"];

            RepositoryItemComboBox txtTipo = new RepositoryItemComboBox();
            txtTipo.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;
            txtTipo.ImmediatePopup = true;
            txtTipo.Items.Clear();
            foreach (var tipo in Enum.GetValues(typeof(eTypePruebaUnitaria)))
                txtTipo.Items.Add(tipo.ToString());
            txtTipo.EditValueChanged += TxtTipo_EditValueChanged;
            col.ColumnEdit = txtTipo;
        }

        private void TxtTipo_EditValueChanged(object sender, EventArgs e)
        {
            DataRow drRow = grdElementoPruebaView.GetFocusedDataRow();

            if (drRow == null)
                return;

            drRow[@"PruebaUnitaria"] = null;
        }

        // LLena las pruebas integrales y coloca un editor de acuerdo al tipo

        private void TxtCasoPrueba_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            DataRow drRow = grdElementoPruebaView.GetFocusedDataRow();
            if (drRow.IsNull())
                return;

            var Tipo = (eTypePruebaUnitaria)
                Enum.Parse(typeof(eTypePruebaUnitaria), drRow[@"Tipo"].ToString());

            if (Tipo == eTypePruebaUnitaria.PruebaUnitaria)
            {
                if (TestCasesSelector == null)
                    TestCasesSelector = new fmTestCasesSelector();

                if (TestCasesSelector.ShowDialog() == DialogResult.OK)
                {
                    grdElementoPruebaView.SetFocusedRowCellValue(
                        "PruebaUnitaria",
                        TestCasesSelector.PruebaUnitaria
                    );
                    grdElementoPruebaView.SetFocusedValue(TestCasesSelector.PruebaUnitaria);

                    grdElementoPruebaView.RefreshData();
                }
            }
            else if (Tipo == eTypePruebaUnitaria.PruebaIntegral)
            {
                //   if (TestIntegralSelector == null)
                TestIntegralSelector = new fmTestIntegralSelector();

                if (TestIntegralSelector.ShowDialog() == DialogResult.OK)
                {
                    grdElementoPruebaView.SetFocusedRowCellValue(
                        "PruebaUnitaria",
                        TestIntegralSelector.PruebaIntegral
                    );
                    grdElementoPruebaView.SetFocusedValue(TestIntegralSelector.PruebaIntegral);

                    grdElementoPruebaView.RefreshData();
                }
            }
            else if (
                (Tipo == eTypePruebaUnitaria.RecuperarDB)
                || (Tipo == eTypePruebaUnitaria.RespaldarDB)
            )
            {
                string DataBase = WoLib.GetDBName(
                    proyecto.ParConexion.Instance,
                    proyecto.ParConexion.InstanceType,
                    proyecto.ParConexion.Year
                );

                var fmText = new fmComboSelect();
                fmText.EditStyle = TextEditStyles.Standard;
                fmText.Text = "Seleccione o escriba el nombre del backup...";
                string result = EjecutarOperacionDB(
                    new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.ListBackup,
                        DataBaseName = DataBase,
                    },
                    false
                );

                fmText.Items = result.Split(
                    new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries
                );

                if (fmText.ShowDialog() == DialogResult.OK)
                {
                    grdElementoPruebaView.SetFocusedRowCellValue(
                        "PruebaUnitaria",
                        fmText.SelectedItem
                    );
                }
            }
            else if (Tipo == eTypePruebaUnitaria.CambiaUdn)
            {
                var fmText = new fmNormalizedText();
                fmText.ExpresionRegular = @"[A-Z0-9]*";
                fmText.Text = "Nombre de la Udn";
                fmText.NormalizedText = grdElementoPruebaView
                    .GetFocusedRowCellValue("PruebaUnitaria")
                    .ToSafeString();

                if (fmText.ShowDialog() == DialogResult.OK)
                {
                    grdElementoPruebaView.SetFocusedRowCellValue(
                        "PruebaUnitaria",
                        fmText.NormalizedText
                    );
                }
            }

            grdElementoPruebaView.RefreshData();
        }

        [SupportedOSPlatform("windows")]
        private bool CargarDirectorioUnitaria()
        {
            treeUnitaria.BeginUpdate();
            treeUnitaria.BeginUnboundLoad();
            treeUnitaria.Nodes.Clear();
            CreaArbolDirectorios(
                treeUnitaria,
                -1,
                proyecto.DirProyectData_Test_UnitTest,
                new List<string>() { "*.cs", "*.js" }
            );
            treeUnitaria.EndUnboundLoad();
            treeUnitaria.EndUpdate();

            return true;
        }

        private bool CargarDirectorioIntegral()
        {
            treeIntegral.BeginUpdate();
            treeIntegral.BeginUnboundLoad();
            treeIntegral.Nodes.Clear();
            CreaArbolDirectorios(
                treeIntegral,
                -1,
                proyecto.DirProyectData_Test_IntegralTest,
                new List<string>() { "*.json" }
            );
            treeIntegral.EndUnboundLoad();
            treeIntegral.EndUpdate();

            treeIntegral.Refresh();
            return true;
        }

        /// <summary>
        /// Recuperamos todos los directorios y ficheros dentro de la carpeta principal de forma recursiva
        /// y cargamos los nodos al tree list
        /// </summary>
        /// <param name="treeList"></param>
        /// <param name="NodeId"></param>
        /// <param name="locDirectorio"></param>
        /// <param name="FilesPatternCol"></param>
        [SupportedOSPlatform("windows")]
        private void CreaArbolDirectorios(
            DevExpress.XtraTreeList.TreeList treeList,
            int NodeId,
            string locDirectorio,
            List<string> FilesPatternCol
        )
        {
            foreach (var FullSubDir in Directory.GetDirectories(locDirectorio))
            {
                var SubDir = Path.GetFileName(FullSubDir);

                TreeListNode newNode = treeList.AppendNode(
                    new object[] { SubDir },
                    NodeId,
                    FullSubDir
                );

                newNode.ImageIndex = 0;
                newNode.SelectImageIndex = 1;
                newNode.StateImageIndex = 2;

                CreaArbolDirectorios(treeList, newNode.Id, FullSubDir, FilesPatternCol);
            }

            foreach (string filePattern in FilesPatternCol)
            {
                foreach (var File in Directory.GetFiles(locDirectorio, filePattern))
                {
                    string OnlyNameFile = Path.GetFileName(File);

                    TreeListNode newNode = treeList.AppendNode(
                        new object[] { OnlyNameFile },
                        NodeId,
                        File
                    );

                    newNode.ImageIndex = 3;
                    newNode.SelectImageIndex = 3;
                    newNode.StateImageIndex = 3;
                }
            }
        }

        #region Codigo para Configurar Syntax Editor
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
            txtScriptIntegral.Document.Language = language;
        }

        internal void ExecuteResolverApp()
        {
            var language = new CSharpSyntaxLanguage();
            projectAssemblyApp = language.GetService<IProjectAssembly>();
            _workerApp.WorkerSupportsCancellation = true;
            _workerApp.DoWork += ChargeDllsApp;
            _workerApp.RunWorkerAsync();

            txtScriptApp.Document.Language = language;
            var formatterCreacion = (CSharpTextFormatter)
                txtScriptApp.Document.Language.GetTextFormatter();
            formatterCreacion.IsOpeningBraceOnNewLine = true;
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

        private void ChargeDllsApp(object sender, EventArgs e)
        {
            var references = new List<string>();

            string corePath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            foreach (var Referencia in SystemReferenciasApp)
                references.Add(Path.Combine(corePath, Referencia) + ".dll");

            foreach (var Referencia in ReferenciasApp)
                references.Add(Path.Combine(AssemblyPath, Referencia));

            projectAssemblyApp.AssemblyReferences.AddMsCorLib();
            /// System
            foreach (var dll in references)
            {
                projectAssemblyApp.AssemblyReferences.AddFrom(dll);
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

                GetCurrentScriptEditor().ActiveView.Selection.CaretPosition = new TextPosition(
                    iLine,
                    iColumn
                );
                GetCurrentScriptEditor().ActiveView.Scroller.ScrollLineToVisibleMiddle();
                GetCurrentScriptEditor().Focus();
            }
        }

        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor GetCurrentScriptEditor()
        {
            if (tabScript.SelectedTabPage == tabScriptPage)
                return txtScript;
            else
                return txtScriptApp;
        }
        #endregion Codigo para Configurar Syntax Editor


        #region Compilacion del Archivo
        private void buCompilar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                tabInferior.SelectedTabPage = tabError;
                txtLog = txtLogPruebaUnitaria;
                txtLog.Text = String.Empty;

                List<ScriptErrorDescriptor> alErrors;

                if (tabScript.SelectedTabPage == tabScriptPage)
                    syntaxEditorHelper.Validar(GetCurrentScriptEditor().Text, out alErrors);
                else
                    syntaxEditorHelperApp.Validar(GetCurrentScriptEditor().Text, out alErrors);

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
        #endregion Compilacion del Archivo


        #region Ejecucion del Archivo
        private void buEjecutar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtLog = txtLogPruebaUnitaria;
            tabPrincipal.SelectedTabPage = tabPruebasUnitarias2;
            tabInferior.SelectedTabPage = tabConsola;
            txtLog.Text = String.Empty;

            /*
                0 Solo Ejecturar
                1 Borrar DB y Ejecutar
                2 Recuperar DB y Ejecutar
             */

            // Borrar DB
            if (txtEjecutarAccion.EditValue.ToString() == repEjecutarAccion.Items[1].ToString())
            {
                ConsoleLog("Borrando db");
                buBorrarInstancia_ItemClick(null, null);
            }

            // Borrar DB App
            if (txtEjecutarAccionApp.EditValue.ToString() == repEjecutarAccion.Items[1].ToString())
            {
                ConsoleLog("Borrando db");
                buBorrarDBHibrida_ItemClick(null, null);
            }

            // Recuperar DB
            if (txtEjecutarAccion.EditValue.ToString() == repEjecutarAccion.Items[2].ToString())
            {
                if (txtDBaRecuperar.EditValue.IsNullOrStringEmpty())
                {
                    XtraMessageBox.Show(
                        "Debe seleccionar una base de datos para recuperar",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                string DataBase = WoLib.GetDBName(
                    proyecto.ParConexion.Instance,
                    proyecto.ParConexion.InstanceType,
                    proyecto.ParConexion.Year
                );

                string result = EjecutarOperacionDB(
                    new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.Restore,
                        DataBaseName = DataBase,
                        BackupName = txtDBaRecuperar.EditValue.ToString()
                    },
                    true
                );

                if (result.StartsWith("Error"))
                {
                    XtraMessageBox.Show(
                        $"Error al tratar de restaurar de {txtDBaRecuperar.EditValue.ToString()}\r\n{result}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
                    return;
                }

                ConsoleLog($"Recupeando db {txtDBaRecuperar.EditValue.ToString()}");

                buLogOut.PerformClick();
            }

            // ToDo
            // Recuperar DB App

            if (txtEjecutarAccionApp.EditValue.ToString() == repEjecutarAccion.Items[2].ToString())
            {
                XtraMessageBox.Show(
                    "Falta Implementar",
                    "Error...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Determina como ejectuar si como un ensamblado interno o como aplicacion externa

            if (txtModoEjecucion.EditValue.ToString() == repModoEjecucion.Items[0])
            {
                string Name = Path.GetFileNameWithoutExtension(
                    treeUnitaria.FocusedNode.Tag.ToString()
                );

                if (Name.EndsWith(".Hybrid"))
                {
                    XtraMessageBox.Show(
                        "Las pruebas para Apps solo se pueden ejecutar como 'Ejecutable Externo'",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                try
                {
                    if (!EjecutarPruebaEnsambladoInterno(treeUnitaria.FocusedNode.Tag.ToString()))
                        tabInferior.SelectedTabPage = tabError;
                }
                catch (Exception ex)
                {
                    ConsoleCriticalLog(ex.Message);
                    //XtraMessageBox.Show(
                    //    ex.Message,
                    //    this.Text,
                    //    MessageBoxButtons.OK,
                    //    MessageBoxIcon.Error
                    //);
                    //tabInferior.SelectedTabPage = tabError;
                }
            }
            else
            {
                EjecutarPruebaExecutableExterno(treeUnitaria.FocusedNode.Tag.ToString(), false);
            }
        }
        #endregion Ejecucion del Archivo



        public void CopiarDirectorio(DirectoryInfo origen, DirectoryInfo destino)
        {
            // Comprueba que el destino exista:
            if (!destino.Exists)
            {
                destino.Create();
            }

            // Copia todos los archivos del directorio actual:
            foreach (FileInfo archivo in origen.EnumerateFiles())
            {
                archivo.CopyTo(Path.Combine(destino.FullName, archivo.Name));
            }

            // Procesamiento recursivo de subdirectorios:
            foreach (DirectoryInfo directorio in origen.EnumerateDirectories())
            {
                // Obtención de directorio de destino:
                string directorioDestino = Path.Combine(destino.FullName, directorio.Name);

                // Invocación recursiva del método `CopiarDirectorio`:
                CopiarDirectorio(directorio, new DirectoryInfo(directorioDestino));
            }
        }

        // Todo Net6 error en EjecutarAppDomain
        public async void EjecutarAppDomain(
            string csProgPath,
            string AppDomainName,
            string ExecFile,
            string args
        )
        {
            Process process = new Process();

            process.StartInfo.WorkingDirectory = $"{csProgPath}\\WooW.UnitTest\\bin\\Debug\\net8.0";
            process.StartInfo.FileName = ExecFile;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.Arguments = args;
            process.Start();
        }

        private bool EjecutarPruebaEnsambladoInterno(string ArchivoPrueba)
        {
            Cursor.Current = Cursors.WaitCursor;

            bool EjecucionExitosa = true;

            try
            {
                proyecto.AssemblyModelCargado = true;

                Cursor.Current = Cursors.WaitCursor;
                lstErrors.Items.Clear();

                string UnitTest = File.ReadAllText(ArchivoPrueba);

                List<ScriptErrorDescriptor> alErrors;
                Assembly ass = syntaxEditorHelper.CreateInstanceForRun(UnitTest, out alErrors);

                if (ass == null)
                {
                    RefreshErrors(alErrors);
                    throw new Exception("Error en el ensamblado");
                }

                IWoUnitTest unit = null;

                var types = ass.GetTypes()
                    .Where(mytype => mytype.GetInterfaces().Contains(typeof(IWoUnitTest)));
                if (types.Count() == 0)
                    throw new Exception("No se encontró prueba unitaria");

                unit = (IWoUnitTest)ass.CreateInstance(types.FirstOrDefault().FullName);

                if (unit == null)
                    throw new Exception("No se encontró prueba unitaria");

                unit.Uth = new WoUthHelper()
                {
                    dLog = ConsoleLog,
                    dLogWarning = ConsoleWarningLog,
                    dLogError = ConsoleErrorLog,
                    dLogSuccess = ConsoleSuccessLog
                };

                unit.User = proyecto.ParConexion.Usuario;
                unit.Instance = proyecto.ParConexion.Instance;
                unit.Udn = proyecto.ParConexion.Udn;
                unit.Year = proyecto.ParConexion.Year;
                unit.InstanceType = proyecto.ParConexion.InstanceType;
                unit.DataBase = WoLib.GetDBName(
                    proyecto.ParConexion.Instance,
                    proyecto.ParConexion.InstanceType,
                    proyecto.ParConexion.Year
                );

                unit.ParamsDecimal = new decimal[5];
                unit.ParamsDecimal[0] = System.Convert.ToDecimal(txtNumber1.EditValue);
                unit.ParamsDecimal[1] = System.Convert.ToDecimal(txtNumber2.EditValue);
                unit.ParamsDecimal[2] = System.Convert.ToDecimal(txtNumber3.EditValue);
                unit.ParamsDecimal[3] = System.Convert.ToDecimal(txtNumber4.EditValue);
                unit.ParamsDecimal[4] = System.Convert.ToDecimal(txtNumber5.EditValue);

                unit.ParamsString = new string[5];
                unit.ParamsString[0] = txtString1.EditValue.ToString();
                unit.ParamsString[1] = txtString2.EditValue.ToString();
                unit.ParamsString[2] = txtString3.EditValue.ToString();
                unit.ParamsString[3] = txtString4.EditValue.ToString();
                unit.ParamsString[4] = txtString5.EditValue.ToString();

                DateTime now = DateTime.Now;

                if (woTarget == null)
                    InitWooWTarget();

                for (int i = 0; i < System.Convert.ToInt32(txtVecesPrueba.EditValue.ToInt16()); i++)
                {
                    unit.ExecuteNumber = i + 1;
                    if (!unit.Run(woTarget))
                        EjecucionExitosa = false;
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return EjecucionExitosa;
        }

        // Login
        private void InitWooWTarget()
        {
            woTarget = new JsonApiClient("https://localhost:5101");

            var AuthenticateResponse = woTarget.Post<AuthenticateResponse>(
                new Authenticate
                {
                    provider = "credentials", // CredentialsAuthProvider.Name, //= credentials
                    UserName = proyecto.ParConexion.Usuario,
                    Password = proyecto.ParConexion.Password,
                    RememberMe = true,
                }
            );

            woTarget.BearerToken = AuthenticateResponse.BearerToken;
            woTarget.GetHttpClient().DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AuthenticateResponse.BearerToken);

            var WoInstanciaUdnResponse = woTarget.Post(
                new WoInstanciaUdnAsignar
                {
                    Instance = proyecto.ParConexion.Instance,
                    Udn = proyecto.ParConexion.Udn,
                    Year = proyecto.ParConexion.Year,
                    InstanceType = proyecto.ParConexion.InstanceType
                }
            );
        }

        private string EjecutarOperacionDB(WoDataBaseOperations dbOperation, bool OutConsole)
        {
            Cursor.Current = Cursors.WaitCursor;

            bool EjecucionExitosa = true;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (woTarget == null)
                {
                    try
                    {
                        InitWooWTarget();
                    }
                    catch (Exception ex)
                    {
                        if (woTarget == null)
                        {
                            if (OutConsole)
                                ConsoleCriticalLog("Error al dar login\r\n" + ex.Message);
                            else
                                XtraMessageBox.Show(
                                    "Error al dar login\r\n" + ex.Message,
                                    "Verifique...",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            return "Error";
                        }
                    }
                }

                return woTarget.Send<string>(dbOperation);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool EjecutarPruebaExecutableExterno(string ArchivoPrueba, bool VisualStudio)
        {
            Cursor.Current = Cursors.WaitCursor;

            #region Argumentos

            string[] args = new string[19];

            // Construye argumentos
            args[0] = "Socket";
            args[1] =
                $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\{proyecto.ParConexion.DbApp}";
            args[2] = "https://localhost:5101";
            //args[0] = "http://192.168.1.161:5100";
            args[3] = txtVecesPrueba.EditValue.ToString();
            args[4] = proyecto.ParConexion.Usuario;
            args[5] = proyecto.ParConexion.Password;
            args[6] = proyecto.ParConexion.Instance;
            args[7] = proyecto.ParConexion.Year.ToString();
            args[8] = proyecto.ParConexion.Udn;
            args[9] = txtNumber1.EditValue.ToString();
            args[10] = txtNumber2.EditValue.ToString();
            args[11] = txtNumber3.EditValue.ToString();
            args[12] = txtNumber4.EditValue.ToString();
            args[13] = txtNumber5.EditValue.ToString();
            args[14] = txtString1.EditValue.ToString();
            args[15] = txtString2.EditValue.ToString();
            args[16] = txtString3.EditValue.ToString();
            args[17] = txtString4.EditValue.ToString();
            args[18] = txtString5.EditValue.ToString();

            #endregion Argumentos

            // Crea un directorio temporal
            string csProgPath = $"{proyecto.Dir}\\Temp";

            if (!Directory.Exists(csProgPath))
                Directory.CreateDirectory(csProgPath);

            string AppDomainName = string.Empty;

            for (; ; )
            {
                AppDomainName = Path.GetRandomFileName();
                string tempDirectory = Path.Combine(csProgPath, AppDomainName);
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                    csProgPath = tempDirectory;
                    break;
                }
            }

            string Name = Path.GetFileNameWithoutExtension(ArchivoPrueba);

            bool Hybrid = Name.EndsWith(".Hybrid");

            string SourceDir = (
                Hybrid
                    ? proyecto.DirApplication_UnitTest_UnitTestHybrid
                    : proyecto.DirApplication_UnitTest_UnitTest
            );

            string UnitTestFile = $"{SourceDir}\\WooW.UnitTest\\UnitTest.cs";
            string UnitTest = File.ReadAllText(ArchivoPrueba);
            File.WriteAllText(UnitTestFile, UnitTest);

            DirectoryInfo directorioOrigen = new DirectoryInfo(SourceDir);
            DirectoryInfo directorioDestino = new DirectoryInfo(csProgPath);

            CopiarDirectorio(directorioOrigen, directorioDestino);

            // Todo ver como meter parametros
            if (VisualStudio)
            {
                string arg = string.Empty;
                for (int i = 0; i < args.Length; i++)
                {
                    if (!arg.IsNullOrStringEmpty())
                        arg += " ";

                    arg += $"\\\"{args[i].Replace("\\", "\\\\")}\\\"";
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("{");
                sb.AppendLine("  \"profiles\": {");
                sb.AppendLine("    \"WooW.UnitTest\": {");
                sb.AppendLine("       \"commandName\": \"Project\",");
                sb.Append("      \"commandLineArgs\": \"");
                sb.AppendLine(arg + "\"");
                sb.AppendLine("    }");
                sb.AppendLine("  }");
                sb.AppendLine("}");

                string launchSettings =
                    $"{directorioDestino}\\WooW.UnitTest\\Properties\\launchSettings.json";

                File.WriteAllText(launchSettings, sb.ToString());

                /*
{
  "profiles": {
    "WooW.UnitTest": {
      "commandName": "Project",
      "commandLineArgs": "Socket DBHandHeld.sqlite https://localhost:5101 1 employee@email.com p FRO941024IHA 2023 INDU 0 0 0 0 0 \"\" \"\" \"\" \"\" \"\""
    }
  }
}                 */


                string commandLineArgs =
                    csProgPath + (Hybrid ? @"\WooW.UnitTest.Hybrid.sln" : @"\WooW.UnitTest.sln");

                Process process = woVisualStudio.AbreVisualStudio(commandLineArgs, true);

                string VSUnit = $"{directorioDestino}\\WooW.UnitTest\\UnitTest.cs";

                string NewUnitTest = File.ReadAllText(VSUnit);
                if (NewUnitTest != File.ReadAllText(ArchivoPrueba))
                {
                    XtraMessageBox.Show(
                        "El caso prueba fue cambiado en Visual Studio",
                        "Verifique ...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    buEditar.PerformClick();
                    GetCurrentScriptEditor().Text = NewUnitTest;
                }

                return false;
            }
            else
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    string arg = string.Empty;
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (!arg.IsNullOrStringEmpty())
                            arg += " ";
                        arg += $"\"{args[i]}\"";
                    }

                    CriticalError = false;

                    ConsoleLogClear();

                    string BatFile = (Hybrid ? "UnitTest.Hybrid" : "UnitTest");

                    string sResult = Proceso(
                        $"{csProgPath}\\Build.WooW.{BatFile}.bat",
                        string.Empty
                    );

                    if (sResult.IsNullOrStringEmpty())
                    {
                        string ExecFile =
                            $"{csProgPath}\\WooW.UnitTest\\bin\\Debug\\net8.0\\WooW.UnitTest.exe";

                        // Solucion patito para que no mande error entre hilos
                        Control.CheckForIllegalCrossThreadCalls = false;
                        iExitCode = -1;

                        // Todo Net6
                        Task tarea = new Task(
                            () => EjecutarAppDomain(csProgPath, AppDomainName, ExecFile, arg)
                        );
                        tarea.Start();
                        tarea.Wait();

                        lock (LockContadorBach)
                        {
                            IContadorBach = 0;
                        }

                        for (; ; )
                        {
                            if (iExitCode == -1)
                            {
                                Application.DoEvents();
                                System.Threading.Thread.Sleep(30);
                            }
                            else
                                break;

                            lock (LockContadorBach)
                            {
                                IContadorBach++;

                                if (IContadorBach > 2000)
                                    break;
                            }
                        }

                        // Por alguna razon no llega el exitcode, en micros ryzen 7950
                        //if (iExitCode == -1)
                        //    iExitCode = 0;

                        switch (iExitCode)
                        {
                            case 2:
                                ConsoleCriticalLog($"Exit Code {iExitCode}");
                                break;
                            case 1:
                                ConsoleErrorLog($"Exit Code {iExitCode}");
                                break;
                            case -1:
                                ConsoleErrorLog($"Sin Exit Code {iExitCode}");
                                break;
                            case 0:
                                ConsoleSuccessLog($"Success Exit Code {iExitCode}");
                                break;
                        }
                        return !CriticalError && (iExitCode == 0);
                    }
                    else
                    {
                        ConsoleCriticalLog($"Error al generar Build a {AppDomainName}");
                        ConsoleCriticalLog(string.Empty);
                        ConsoleCriticalLog(sResult);
                    }
                    Cursor.Current = Cursors.Default;

                    return (iConsoleErrorsNumCriticalLog == 0) && (iExitCode == 0);
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return false;
                }
            }
        }

        private string Proceso(string Proceso, string Parametros)
        {
            string sDir = Path.GetDirectoryName(Proceso);

            string LogErrors = $"{sDir}\\WooW.Errors.Log";
            if (File.Exists(LogErrors))
                File.Delete(LogErrors);

            string LogModelErrors = $"{sDir}\\WooW.Model.Errors.Log";
            if (File.Exists(LogModelErrors))
                File.Delete(LogModelErrors);

            if (!File.Exists(Proceso))
                throw new Exception($"No existe {Proceso}");

            ProcessStartInfo startInfo = new ProcessStartInfo(Proceso, Parametros);
            startInfo.WorkingDirectory = sDir;
            startInfo.WindowStyle = ProcessWindowStyle.Minimized; // .Hidden;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true; //No utiliza RunDLL32 para lanzarlo   //Opcional: establecer la carpeta de trabajo en la que se ejecutará el proceso   //startInfo.WorkingDirectory = "C:\\MiCarpeta\\";
            //Redirige las salidas y los errores
            //startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            Process proc = Process.Start(startInfo); //Ejecuta el proceso
            proc.WaitForExit(); // Espera a que termine el proceso

            string error = string.Empty;
            //string error = proc.StandardError.ReadToEnd();
            //if (error == null || error == "") //Error
            //{
            if (File.Exists(LogErrors))
                error = File.ReadAllText(LogErrors);
            if (File.Exists(LogModelErrors))
            {
                if (!error.IsEmpty())
                    error += "\r\n\r\n";
                error += File.ReadAllText(LogModelErrors);
            }
            //}

            return error;
        }

        #region Consola

        public void EscribeEnLaConsola(string MensajeRecibido)
        {
            string original = MensajeRecibido;
            string mensaje = original;

            if (original.IndexOf("> ") != -1)
                mensaje = mensaje.Substring(original.IndexOf("> ") + 2);

            Action accionConsola = new Action(() =>
            {
                try
                {
                    if (original.StartsWith("ExitCode>"))
                        iExitCode = System.Convert.ToInt16(original.Substring("ExitCode>".Length));
                    else if (original.StartsWith("Console>"))
                        ConsoleLog(mensaje);
                    else if (original.StartsWith("ConsoleWarning>"))
                        ConsoleWarningLog(mensaje);
                    else if (original.StartsWith("ConsoleError>"))
                        ConsoleErrorLog(mensaje);
                    else if (original.StartsWith("ConsoleTitle>"))
                    {
                        if (mensaje.StartsWith("Código de Prueba Terminada "))
                            int.TryParse(
                                mensaje.Substring("Código de Prueba Terminada ".Length).Trim(),
                                out iExitCode
                            );
                        ConsoleTitleLog(mensaje);
                    }
                    else if (original.StartsWith("ConsoleCritical>"))
                        ConsoleCriticalLog(mensaje);
                    else if (original.StartsWith("ConsoleSuccess>"))
                        ConsoleSuccessLog(mensaje);
                    else
                        ConsoleLog(mensaje);
                }
                catch (Exception ex) { }
                ;
            });

            Dispatcher.CurrentDispatcher.Invoke(accionConsola, DispatcherPriority.ApplicationIdle);
        }

        private void ConsoleLogClear()
        {
            iConsoleErrorsNumLog = 0;
            iConsoleErrorsNumWarningLog = 0;
            iConsoleErrorsNumErrorLog = 0;
            iConsoleErrorsNumTitleLog = 0;
            iConsoleErrorsNumCriticalLog = 0;
            iConsoleErrorsNumSuccessLog = 0;
        }

        private void ConsoleLog(string message)
        {
            lock (LockContadorBach)
            {
                IContadorBach = 0;
            }

            iConsoleErrorsNumLog++;

            if (!mnuLog.Checked)
                return;

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            txtLog.SelectionColor = Color.LightGray;
            txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Regular);
            txtLog.AppendText(message + "\r\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            txtLog.Refresh();
            //txtLog.ScrollToCaret();
        }

        private void ConsoleWarningLog(string message)
        {
            lock (LockContadorBach)
            {
                IContadorBach = 0;
            }

            iConsoleErrorsNumWarningLog++;

            if (!mnuLogWarning.Checked)
                return;

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            txtLog.SelectionColor = Color.Yellow;
            txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Regular);
            txtLog.AppendText(message + "\r\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            //txtLog.ScrollToCaret();
        }

        private void ConsoleErrorLog(string message)
        {
            lock (LockContadorBach)
            {
                IContadorBach = 0;
            }

            iConsoleErrorsNumErrorLog++;

            if (!mnuLogError.Checked)
                return;

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            txtLog.SelectionColor = Color.OrangeRed;
            txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Regular);
            txtLog.AppendText(message + "\r\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            //txtLog.ScrollToCaret();
        }

        private void ConsoleTitleLog(string message)
        {
            lock (LockContadorBach)
            {
                IContadorBach = 0;
            }

            iConsoleErrorsNumTitleLog++;

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            txtLog.SelectionColor = Color.White;
            txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Bold);
            txtLog.AppendText(message + "\r\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            //txtLog.ScrollToCaret();
        }

        private void ConsoleCriticalLog(string message)
        {
            lock (LockContadorBach)
            {
                IContadorBach = 0;
            }

            iConsoleErrorsNumCriticalLog++;

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            txtLog.SelectionColor = Color.Red;
            txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Regular);
            txtLog.AppendText(message + "\r\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            //txtLog.ScrollToCaret();
            CriticalError = true;
        }

        private void ConsoleSuccessLog(string message)
        {
            lock (LockContadorBach)
            {
                IContadorBach = 0;
            }

            iConsoleErrorsNumSuccessLog++;

            if (!mnuLogSuccess.Checked)
                return;

            txtLog.SuspendLayout();
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            txtLog.SelectionColor = Color.LimeGreen;
            txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Bold);
            txtLog.AppendText(message + "\r\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            txtLog.Refresh();
            txtLog.ResumeLayout();
        }
        #endregion Console


        /// <summary>
        /// Actualización de la pantalla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void buRefrescar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestCasesSelector = null;
            Application.DoEvents();
            CargarDirectorioUnitaria();
            CargarDirectorioIntegral();
        }

        // Valida que el archivo o carpeta este bien escrito
        private void treeObjetos_ValidatingEditor(
            object sender,
            DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e
        )
        {
            TestCasesSelector = null;

            DevExpress.XtraTreeList.TreeList treeList = (
                sender as DevExpress.XtraTreeList.TreeList
            );

            string Terminador = (treeList == treeIntegral ? ".json" : ".cs");

            var ex = e.Value;

            if ((treeList.FocusedNode.IsNull()) || (treeList.FocusedNode.Tag.IsNull()))
            {
                e.Valid = false;
                return;
            }

            string iTem = treeList.FocusedNode.Tag.ToString();

            if (
                (e.Value.ToString().IndexOfAny(Path.GetInvalidPathChars()) != -1)
                || (e.Value.ToString().IndexOfAny(new char[] { '\\', '/' }) != -1)
            )
            {
                e.Valid = false;
                e.ErrorText = "Nombre tiene caracteres inválidos";
                return;
            }

            if (Directory.Exists(iTem))
            {
                string sDestino = Path.GetDirectoryName(iTem) + "\\" + e.Value.ToString().Trim();
                try
                {
                    Directory.Move(iTem, sDestino);
                    treeList.FocusedNode.Tag = sDestino;
                    //treeList.FocusedNode = treeList.Nodes[0];

                    ModificarPruebaIntegralDirectorio(iTem, sDestino);
                    treeList.OptionsBehavior.Editable = false;
                    e.Valid = true;
                    return;

                    //buRefrescar.PerformClick();
                }
                catch
                {
                    e.ErrorText = "Error al renombrar la carpeta";
                    e.Valid = false;
                    return;
                }
            }
            else
            {
                string Archivo = e.Value.ToString().Trim();
                // Existe ya un archivo con ese nombre?
                foreach (
                    string CompArchivo in Directory.EnumerateFiles(
                        proyecto.DirProyectData_Test_UnitTest,
                        "*" + Terminador,
                        SearchOption.AllDirectories
                    )
                )
                {
                    if (iTem.ToUpper() == CompArchivo.ToUpper())
                        continue;

                    if (
                        Path.GetFileNameWithoutExtension(Archivo).ToUpper()
                        == Path.GetFileNameWithoutExtension(
                            Path.GetFileNameWithoutExtension(CompArchivo).ToUpper()
                        )
                    )
                    {
                        e.ErrorText = "Nombre del caso prueba ya existe, renombre lo por otro";
                        e.Valid = false;
                        return;
                    }
                }

                if (!e.Value.ToString().ToLower().EndsWith(Terminador))
                {
                    e.ErrorText = "Debe terminar con extensión " + Terminador;
                    e.Valid = false;
                    return;
                }

                Regex regex = new Regex(@"[A-Z][a-zA-Z0-9]*");

                string Name = Path.GetFileNameWithoutExtension(e.Value.ToString());

                if (Name.EndsWith(".Hybrid"))
                    Name = Path.GetFileNameWithoutExtension(Name);

                // Step 3: test for Success.
                if (regex.Match(Name).Value.ToString() != Name)
                {
                    e.ErrorText =
                        "Nombre del archivo debe comenzar con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números";
                    e.Valid = false;
                    return;
                }

                string sDestino = Path.GetDirectoryName(iTem) + "\\" + e.Value.ToString().Trim();

                try
                {
                    File.Move(iTem, sDestino);
                    treeList.OptionsBehavior.Editable = false;

                    string sUnit = "IWoUnitTest";
                    if (Path.GetFileNameWithoutExtension(sDestino).EndsWith(".Hybrid"))
                        sUnit = "IWoUnitHybridTest";

                    // Cambia el nombre de la clase en el archivo
                    string[] lineas = File.ReadAllLines(sDestino);
                    for (int i = 0; i < lineas.Length; i++)
                    {
                        if (lineas[i].Trim().EndsWith(sUnit))
                        {
                            lineas[i] =
                                $"    public class {Path.GetFileNameWithoutExtension(sDestino).Replace('.', '_')}UnitTest : {sUnit}";
                            break;
                        }
                    }
                    File.WriteAllLines(sDestino, lineas);
                    treeList.FocusedNode.Tag = sDestino;
                    treeObjetos_FocusedNodeChanged(null, null);

                    ModificarPruebaIntegral(iTem, sDestino);
                }
                catch
                {
                    e.Valid = false;
                    e.ErrorText = "Error al renombrar el archivo";
                    return;
                }
            }
        }

        private void mnuRenombrarCarpeta_Click(object sender, EventArgs e)
        {
            currentPopTreeList.OptionsBehavior.Editable = true;
            currentPopTreeList.ShowEditor();
        }

        // Detecta cuando se selecciona una Prueba

        private void treeObjetos_FocusedNodeChanged(
            object sender,
            DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e
        )
        {
            treeUnitaria.OptionsBehavior.Editable = false;

            if ((treeUnitaria.FocusedNode.IsNull()) || (treeUnitaria.FocusedNode.Tag.IsNull()))
                return;

            string iTem = treeUnitaria.FocusedNode.Tag.ToString();

            string Name = Path.GetFileNameWithoutExtension(iTem);

            if (Name.EndsWith(".Hybrid"))
                tabScript.SelectedTabPage = tabScriptPageApp;
            else
                tabScript.SelectedTabPage = tabScriptPage;

            buEjecutar.Enabled = false;
            buParametrosEjecucion.Enabled = buEjecutar.Enabled;
            buVisual.Enabled = buEjecutar.Enabled;
            buCompilar.Enabled = false;
            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            txtScript.Document.IsReadOnly = true;
            txtScriptApp.Document.IsReadOnly = true;
            grpEditor.Enabled = false;
            lstErrors.Items.Clear();

            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.NoMostrar;

            if (File.Exists(iTem))
            {
                buEditar.Enabled = true;
                string Source = File.ReadAllText(iTem);

                GetCurrentScriptEditor().Text = Source;
                buEjecutar.Enabled = !Source.IsNullOrStringEmpty();
                buParametrosEjecucion.Enabled = buEjecutar.Enabled;
                buVisual.Enabled = buEjecutar.Enabled;
                buCompilar.Enabled = buEjecutar.Enabled;
            }
            else
            {
                GetCurrentScriptEditor().Text = string.Empty;
            }
        }

        private void mnuCrearCarpeta_Click(object sender, EventArgs e)
        {
            string sDir = string.Empty;

            if (currentPopTreeList.FocusedNode == null)
                sDir = (
                    currentPopTreeList == treeIntegral
                        ? proyecto.DirProyectData_Test_IntegralTest
                        : proyecto.DirProyectData_Test_UnitTest
                );
            else
                sDir = Path.GetDirectoryName(currentPopTreeList.FocusedNode.Tag.ToString());

            //Nueva carpeta (2)

            string sCarpeta = "Nueva Carpeta";

            string FullSubDir = $"{sDir}\\{sCarpeta}";

            if (Directory.Exists(FullSubDir))
                for (int i = 2; ; i++)
                {
                    sCarpeta = $"Nueva Carpeta ({i})";
                    FullSubDir = $"{sDir}\\{sCarpeta}";
                    if (!Directory.Exists(FullSubDir))
                        break;
                    ;
                }

            Directory.CreateDirectory(FullSubDir);

            int NodeId = -1;
            if (currentPopTreeList.FocusedNode != null)
                if (currentPopTreeList.FocusedNode.ParentNode != null)
                    NodeId = currentPopTreeList.FocusedNode.ParentNode.Id;

            TreeListNode newNode = currentPopTreeList.AppendNode(
                new object[] { sCarpeta },
                NodeId,
                FullSubDir
            );

            newNode.ImageIndex = 0;
            newNode.SelectImageIndex = 1;
            newNode.StateImageIndex = 2;

            currentPopTreeList.SetFocusedNode(newNode);
            currentPopTreeList.OptionsBehavior.Editable = true;
            currentPopTreeList.ShowEditor();
        }

        private void mnuCrearSubCarpeta_Click(object sender, EventArgs e)
        {
            string sDir = currentPopTreeList.FocusedNode.Tag.ToString();

            string sCarpeta = "Nueva Carpeta";

            string FullSubDir = $"{sDir}\\{sCarpeta}";

            if (Directory.Exists(FullSubDir))
                for (int i = 2; ; i++)
                {
                    sCarpeta = $"Nueva Carpeta ({i})";
                    FullSubDir = $"{sDir}\\{sCarpeta}";
                    if (!Directory.Exists(FullSubDir))
                        break;
                    ;
                }

            Directory.CreateDirectory(FullSubDir);

            int NodeId = currentPopTreeList.FocusedNode.Id;

            TreeListNode newNode = currentPopTreeList.AppendNode(
                new object[] { sCarpeta },
                NodeId,
                FullSubDir
            );

            newNode.ImageIndex = 0;
            newNode.SelectImageIndex = 1;
            newNode.StateImageIndex = 2;

            currentPopTreeList.SetFocusedNode(newNode);
            currentPopTreeList.OptionsBehavior.Editable = true;
            currentPopTreeList.ShowEditor();
        }

        #region Construye el MenuContextual de acuerdo al elemento seleccionado en arbol
        private void treeObjetos_PopupMenuShowing(
            object sender,
            DevExpress.XtraTreeList.PopupMenuShowingEventArgs e
        )
        {
            DevExpress.XtraTreeList.TreeList treeList = (sender as TreeList);
            currentPopTreeList = treeList;

            if ((treeList.FocusedNode.IsNull()) || (treeList.FocusedNode.Tag.IsNull()))
            {
                e.Menu.Items.Add(new DXMenuItem("Crear Carpeta", mnuCrearCarpeta_Click));
                return;
            }

            string iTem = treeList.FocusedNode.Tag.ToString();

            if (File.Exists(iTem))
            {
                e.Menu.Items.Add(
                    new DXMenuItem("Renombrar Caso Prueba", RenombrarCasoPrueba_Click)
                );

                if (treeList == treeIntegral)
                {
                    e.Menu.Items.Add(
                        new DXMenuItem("Crear Prueba Integral", CrearCasoPrueba_Click)
                    );
                    e.Menu.Items.Add(
                        new DXMenuItem("Clonar Prueba Integral", ClonarPruebaIntegral_Click)
                    );
                }
                else
                {
                    e.Menu.Items.Add(new DXMenuItem("Crear Caso Prueba", CrearCasoPrueba_Click));
                    e.Menu.Items.Add(new DXMenuItem("Clonar Caso Prueba", Clonar_Click));
                    e.Menu.Items.Add(
                        new DXMenuItem("Crear Caso Prueba Híbrido", CrearCasoPrueba_Click)
                    );
                }
                e.Menu.Items.Add(new DXMenuItem("Borrar Caso Prueba", BorrarCasoPrueba_Click));
            }
            else
            {
                e.Menu.Items.Add(new DXMenuItem("Crear Carpeta", mnuCrearCarpeta_Click));
                e.Menu.Items.Add(new DXMenuItem("Crear Sub Carpeta", mnuCrearSubCarpeta_Click));
                e.Menu.Items.Add(new DXMenuItem("Renombrar Carpeta", mnuRenombrarCarpeta_Click));

                if (
                    (Directory.GetDirectories(iTem).Length == 0)
                    && (Directory.GetFiles(iTem).Length == 0)
                )
                    e.Menu.Items.Add(new DXMenuItem("Borrar Carpeta", BorrarCarpeta_Click));

                e.Menu.Items.Add(new DXMenuItem("Crear Caso Prueba", CrearCasoPrueba_Click));
                e.Menu.Items.Add(
                    new DXMenuItem("Crear Caso Prueba Híbrido", CrearCasoPrueba_Click)
                );
            }
        }

        private void RenombrarCasoPrueba_Click(object sender, EventArgs e)
        {
            currentPopTreeList.OptionsBehavior.Editable = true;
            currentPopTreeList.ShowEditor();
        }

        private void BorrarCarpeta_Click(object sender, EventArgs e)
        {
            Directory.Delete(currentPopTreeList.FocusedNode.Tag.ToString());
            currentPopTreeList.FocusedNode.Remove();
        }

        private void BorrarCasoPrueba_Click(object sender, EventArgs e)
        {
            string sFile = currentPopTreeList.FocusedNode.Tag.ToString();

            if (File.Exists(sFile))
            {
                if (
                    XtraMessageBox.Show(
                        $"Borrar el caso prueba {Path.GetFileName(sFile)}?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) == DialogResult.Yes
                )
                {
                    File.Delete(sFile);
                    BorrarPruebaIntegral(sFile);
                    buRefrescar.PerformClick();
                }
            }
        }

        private void CrearCasoPrueba_Click(object sender, EventArgs e)
        {
            bool bDir = true;

            string sDir = currentPopTreeList.FocusedNode.Tag.ToString();

            if (File.Exists(sDir))
            {
                sDir = Path.GetDirectoryName(sDir);
                bDir = false;
            }

            if (!Directory.Exists(sDir))
                return;

            string sCasoPrueba = String.Empty;

            string sCasoPruebaFull = $"{sDir}\\{sCasoPrueba}";

            for (int i = 1; ; i++)
            {
                if (currentPopTreeList == treeIntegral)
                    sCasoPrueba = $"PruebaIntegral{i}.json";
                else
                {
                    if ((sender as DXMenuItem).Caption.EndsWith("Híbrido"))
                        sCasoPrueba = $"CasoPrueba{i}.Hybrid.cs";
                    else
                        sCasoPrueba = $"CasoPrueba{i}.cs";
                }

                sCasoPruebaFull = $"{sDir}\\{sCasoPrueba}";
                if (!File.Exists(sCasoPruebaFull))
                {
                    bool bFound = false;
                    // Existe ya un archivo con ese nombre?
                    foreach (
                        string CompArchivo in Directory.EnumerateFiles(
                            proyecto.DirProyectData_Test_UnitTest,
                            "*.cs",
                            SearchOption.AllDirectories
                        )
                    )
                    {
                        if (
                            Path.GetFileNameWithoutExtension(sCasoPrueba).ToUpper()
                            == Path.GetFileNameWithoutExtension(CompArchivo).ToUpper()
                        )
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                        break;
                }
            }

            File.WriteAllText(sCasoPruebaFull, String.Empty);

            int NodeId = currentPopTreeList.FocusedNode.Id;
            if (!bDir)
            {
                if (currentPopTreeList.FocusedNode.ParentNode.IsNull())
                    NodeId = -1;
                else
                    NodeId = currentPopTreeList.FocusedNode.ParentNode.Id;
            }

            TreeListNode newNode = currentPopTreeList.AppendNode(
                new object[] { sCasoPrueba },
                NodeId,
                sCasoPruebaFull
            );

            newNode.ImageIndex = 3;
            newNode.SelectImageIndex = 3;
            newNode.StateImageIndex = 3;

            currentPopTreeList.SetFocusedNode(newNode);
            currentPopTreeList.OptionsBehavior.Editable = true;
            currentPopTreeList.ShowEditor();
        }

        private void ClonarPruebaIntegral_Click(object sender, EventArgs e)
        {
            string file = currentPopTreeList.FocusedNode.Tag.ToString();

            if (!File.Exists(file))
                return;

            bool bDir = true;

            string sDir = currentPopTreeList.FocusedNode.Tag.ToString();

            if (File.Exists(sDir))
            {
                sDir = Path.GetDirectoryName(sDir);
                bDir = false;
            }

            if (!Directory.Exists(sDir))
                return;

            string sCasoPrueba = String.Empty;

            string sCasoPruebaFull = $"{sDir}\\{sCasoPrueba}";

            for (int i = 1; ; i++)
            {
                if (currentPopTreeList == treeIntegral)
                    sCasoPrueba = $"PruebaIntegral{i}.json";
                else
                {
                    if ((sender as DXMenuItem).Caption.EndsWith("Híbrido"))
                        sCasoPrueba = $"CasoPrueba{i}.Hybrid.cs";
                    else
                        sCasoPrueba = $"CasoPrueba{i}.cs";
                }

                sCasoPruebaFull = $"{sDir}\\{sCasoPrueba}";
                if (!File.Exists(sCasoPruebaFull))
                {
                    bool bFound = false;
                    // Existe ya un archivo con ese nombre?
                    foreach (
                        string CompArchivo in Directory.EnumerateFiles(
                            proyecto.DirProyectData_Test_UnitTest,
                            "*.cs",
                            SearchOption.AllDirectories
                        )
                    )
                    {
                        if (
                            Path.GetFileNameWithoutExtension(sCasoPrueba).ToUpper()
                            == Path.GetFileNameWithoutExtension(CompArchivo).ToUpper()
                        )
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                        break;
                }
            }

            File.WriteAllText(sCasoPruebaFull, File.ReadAllText(file));

            int NodeId = currentPopTreeList.FocusedNode.Id;
            if (!bDir)
            {
                if (currentPopTreeList.FocusedNode.ParentNode.IsNull())
                    NodeId = -1;
                else
                    NodeId = currentPopTreeList.FocusedNode.ParentNode.Id;
            }

            TreeListNode newNode = currentPopTreeList.AppendNode(
                new object[] { sCasoPrueba },
                NodeId,
                sCasoPruebaFull
            );

            newNode.ImageIndex = 3;
            newNode.SelectImageIndex = 3;
            newNode.StateImageIndex = 3;

            currentPopTreeList.SetFocusedNode(newNode);
            currentPopTreeList.OptionsBehavior.Editable = true;
            currentPopTreeList.ShowEditor();
        }

        private void Clonar_Click(object sender, EventArgs e)
        {
            string file = currentPopTreeList.FocusedNode.Tag.ToString();

            if (!File.Exists(file))
                return;

            string sDir = Path.GetDirectoryName(file);

            string sCasoPrueba = string.Empty;
            string sCasoPruebaFull = string.Empty;

            for (int i = 1; ; i++)
            {
                if ((sender as DXMenuItem).Caption.EndsWith("Híbrido"))
                    sCasoPrueba = $"CasoPrueba{i}.Hybrid.cs";
                else
                    sCasoPrueba = $"CasoPrueba{i}.cs";

                sCasoPruebaFull = $"{sDir}\\{sCasoPrueba}";
                if (!File.Exists(sCasoPruebaFull))
                {
                    bool bFound = false;
                    // Existe ya un archivo con ese nombre?
                    foreach (
                        string CompArchivo in Directory.EnumerateFiles(
                            proyecto.DirProyectData_Test_UnitTest,
                            "*.cs",
                            SearchOption.AllDirectories
                        )
                    )
                    {
                        if (
                            Path.GetFileNameWithoutExtension(sCasoPrueba).ToUpper()
                            == Path.GetFileNameWithoutExtension(CompArchivo).ToUpper()
                        )
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                        break;
                }
            }

            File.Copy(file, sCasoPruebaFull);

            string sUnit = "IWoUnitTest";
            if (Path.GetFileNameWithoutExtension(sCasoPruebaFull).EndsWith(".Hybrid"))
                sUnit = "IWoUnitHybridTest";

            // Cambia el nombre de la clase en el archivo
            string[] lineas = File.ReadAllLines(sCasoPruebaFull);
            for (int i = 0; i < lineas.Length; i++)
            {
                if (lineas[i].Trim().EndsWith(sUnit))
                {
                    lineas[i] =
                        $"    public class {Path.GetFileNameWithoutExtension(sCasoPruebaFull).Replace('.', '_')}UnitTest : {sUnit}";
                    break;
                }
            }
            File.WriteAllLines(sCasoPruebaFull, lineas);

            TreeListNode newNode = currentPopTreeList.AppendNode(
                new object[] { sCasoPrueba },
                currentPopTreeList.FocusedNode.ParentNode.Id,
                sCasoPruebaFull
            );

            newNode.ImageIndex = 3;
            newNode.SelectImageIndex = 3;
            newNode.StateImageIndex = 3;

            currentPopTreeList.SetFocusedNode(newNode);
        }

        #endregion Construye el MenuContextual de acuerdo al elemento seleccionado en arbol

        #region movimiento de archivos en el arbol

        private void treeObjetos_BeforeDragNode(
            object sender,
            DevExpress.XtraTreeList.BeforeDragNodeEventArgs e
        )
        {
            e.CanDrag = File.Exists(e.Node.Tag.ToString());
        }

        private void treeObjetos_BeforeDropNode(
            object sender,
            DevExpress.XtraTreeList.BeforeDropNodeEventArgs e
        )
        {
            if (e.DestinationNode.IsNull())
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = !Directory.Exists(e.DestinationNode.Tag.ToString());

            if (e.Cancel)
                return;

            string sSource = e.SourceNode.Tag.ToString();
            string sFileName = Path.GetFileName(sSource);
            string sDestination = $"{e.DestinationNode.Tag.ToString()}\\{sFileName}";

            e.Cancel = File.Exists(sDestination);
        }

        private void treeObjetos_AfterDropNode(
            object sender,
            DevExpress.XtraTreeList.AfterDropNodeEventArgs e
        )
        {
            TestCasesSelector = null;

            string sSource = e.Node.Tag.ToString();
            string sFileName = Path.GetFileName(sSource);

            string sDestination = e.DestinationNode.Tag.ToString();
            if (File.Exists(sDestination)) // es un archivo
                sDestination = Path.GetDirectoryName(sDestination);

            sDestination = $"{sDestination}\\{sFileName}";

            if (sSource == sDestination)
                return;

            e.Node.Tag = sDestination;

            File.Move(sSource, sDestination);
            ModificarPruebaIntegral(sSource, sDestination);
        }

        #endregion movimiento de archivos en el arbol

        [SupportedOSPlatform("windows")]
        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tabInferior.SelectedTabPage = tabModelos;

            string sFile = treeUnitaria.FocusedNode.Tag.ToString();

            if (!File.Exists(sFile))
                return;

            string sCodigo = File.ReadAllText(sFile);

            string sNombreCasoPrueba = Path.GetFileNameWithoutExtension(sFile);

            string sExtencion = Path.GetExtension(sFile);

            bool Hybrid = false;
            if (sNombreCasoPrueba.EndsWith(".Hybrid"))
            {
                sNombreCasoPrueba = sNombreCasoPrueba.Replace('.', '_');
                Hybrid = true;
            }

            Regex regex = new Regex(@"[A-Z][a-zA-Z0-9]*");
            if (regex.Match(sNombreCasoPrueba).Value.ToString() != sNombreCasoPrueba)
            {
                XtraMessageBox.Show(
                    "Nombre del archivo debe comenzar con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (!File.Exists(sFile))
                return;

            string Prefijo = "CasoPrueba";

            if (sNombreCasoPrueba.ToUpper().StartsWith(Prefijo.ToUpper()))
            {
                XtraMessageBox.Show(
                    "Renombre el archivo, no puede comenzar con " + Prefijo,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (sCodigo.IsNullOrStringEmpty())
            {
                string sBuffer = String.Empty;

                try
                {
                    CreateBaseTestDefault(pathTest: sFile, Hybrid);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $"Error al generar Script {sNombreCasoPrueba} {ex.Message}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }

            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
            txtScript.Document.IsReadOnly = false;
            txtScriptApp.Document.IsReadOnly = false;

            buEditar.Enabled = false;
            buCompilar.Enabled = true;
            buEjecutar.Enabled = false;
            buParametrosEjecucion.Enabled = buEjecutar.Enabled;
            buVisual.Enabled = buEjecutar.Enabled;
            buAceptarCambios.Enabled = true;
            buDescartarCambios.Enabled = true;
            grpEditor.Enabled = true;

            woGridModelo1.Snippet = Config.Editors.woGridModel.eSnippet.SnippetCliente;
        }

        #region Creación de la base de los scripts

        [SupportedOSPlatform("windows")]
        private void CreateBaseTestDefault(string pathTest, bool Hybrid)
        {
            try
            {
                string sNombreCasoPrueba = Path.GetFileNameWithoutExtension(pathTest);

                string extencion = Path.GetExtension(pathTest);

                if (extencion == ".cs")
                {
                    if (Hybrid)
                    {
                        ttCasoPruebaApp ttprueba = new ttCasoPruebaApp();
                        ttprueba.NombreCasoPrueba = sNombreCasoPrueba;
                        txtScriptApp.Text = SyntaxEditorHelper.PrettyPrint(
                            ttprueba.TransformText()
                        );
                    }
                    else
                    {
                        ttCasoPrueba ttprueba = new ttCasoPrueba();
                        ttprueba.NombreCasoPrueba = sNombreCasoPrueba;
                        txtScript.Text = SyntaxEditorHelper.PrettyPrint(ttprueba.TransformText());
                    }
                }
                else if (extencion == ".js")
                {
                    string testPath = $@"{proyecto.DirProyectData}\";
                    WoNewTest woNewTest = new WoNewTest(pathTest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la generación base del script. {ex.Message}");
            }
        }

        #endregion Creación de la base de los scripts


        private void buAceptarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            buCompilar.PerformClick();

            if (lstErrors.Items.Count > 0)
            {
                if (
                    XtraMessageBox.Show(
                        "El script tiene errores, grabar de cualquier forma?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes
                )
                    return;
            }

            string sFile = treeUnitaria.FocusedNode.Tag.ToString();

            if (!File.Exists(sFile))
                return;

            File.WriteAllText(sFile, GetCurrentScriptEditor().Text);
            splitControl.PanelVisibility = SplitPanelVisibility.Both;

            treeObjetos_FocusedNodeChanged(null, null);
        }

        private void buDescartarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if (
                XtraMessageBox.Show(
                    "Cancelar los cambios al script?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            treeObjetos_FocusedNodeChanged(null, null);

            splitControl.PanelVisibility = SplitPanelVisibility.Both;
        }

        private void xtraTabControl1_SelectedPageChanged(
            object sender,
            DevExpress.XtraTab.TabPageChangedEventArgs e
        )
        {
            if (xtraTabControl1.SelectedTabPage == tabPruebasUnitarias)
            {
                ribbonPageUnitarias.Visible = true;
                ribbonControl1.SelectPage(ribbonPageUnitarias);
                ribbonPageIntegrales.Visible = false;
                tabInferior.SelectedTabPage = tabModelos;
                tabPrincipal.SelectedTabPage = tabPruebasUnitarias2;
                grpPruebaUnitaria.Enabled = true;
                grpPruebaIntegral.Enabled = false;
                treeObjetos_FocusedNodeChanged(null, null);
            }
            else
            {
                ribbonPageIntegrales.Visible = true;
                ribbonControl1.SelectPage(ribbonPageIntegrales);
                ribbonPageUnitarias.Visible = false;
                tabIntegral.SelectedTabPage = tabIntegralScript;
                tabPrincipal.SelectedTabPage = tabPruebasIntegrales2;
                grpPruebaUnitaria.Enabled = false;
                grpPruebaIntegral.Enabled = true;
            }
        }

        private void buEditarPruebaIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string iTem = treeIntegral.FocusedNode.Tag.ToString();

            if (!File.Exists(iTem))
                return;

            string sNombreCasoPrueba = Path.GetFileNameWithoutExtension(iTem);

            Regex regex = new Regex(@"[A-Z][a-zA-Z0-9]*");
            if (regex.Match(sNombreCasoPrueba).Value.ToString() != sNombreCasoPrueba)
            {
                XtraMessageBox.Show(
                    "Nombre del archivo debe comenzar con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }
            string Prefijo = "PruebaIntegral";

            if (sNombreCasoPrueba.ToUpper().StartsWith(Prefijo.ToUpper()))
            {
                XtraMessageBox.Show(
                    "Renombre el archivo, no puede comenzar con " + Prefijo,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            tabIntegral.SelectedTabPage = tabIntegralScript;

            txtWorkItem.Enabled = true;
            txtProceso.Enabled = true;
            txtDescripcion.Enabled = true;

            buEditarPruebaIntegral.Enabled = false;
            buRegistrarPruebaIntegral.Enabled = true;
            buAnularPruebaIntegral.Enabled = true;
            buBorrarPruebaIntegral.Enabled = false;
            buEjecutarPruebaIntegral.Enabled = false;

            txtPrueba.Enabled = false;
            pnlDatos.Enabled = true;

            grdElementoPruebaView.OptionsBehavior.Editable = true;
            splitControl.PanelVisibility = SplitPanelVisibility.Panel2;
        }

        private void buVerificarPruebaIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            tabIntegral.SelectedTabPage = tabIntegralConsola;
            txtLog = txtLogPruebaIntegral;
            txtLog.Text = String.Empty;

            foreach (DataRow drRow in (grdElementoPrueba.DataSource as DataTable).Rows)
            {
                var Tipo = (eTypePruebaUnitaria)
                    Enum.Parse(typeof(eTypePruebaUnitaria), drRow[@"Tipo"].ToString());

                if (Tipo == eTypePruebaUnitaria.PruebaUnitaria)
                    drRow[@"Diagnostico"] = VerificaPruebaUnitaria(
                        proyecto,
                        drRow[@"PruebaUnitaria"].ToString()
                    );
                else if (Tipo == eTypePruebaUnitaria.PruebaIntegral)
                {
                    Stack<string> Paquetes = new Stack<string>();
                    drRow[@"Diagnostico"] = VerificaPruebaIntegral(
                        proyecto,
                        drRow[@"PruebaUnitaria"].ToString(),
                        Paquetes
                    );
                }
            }
        }

        private string VerificaPruebaUnitaria(Proyecto locProyecto, string NombrePrueba)
        {
            string sFile = locProyecto.DirProyectData_Test_UnitTest + NombrePrueba;

            if (!File.Exists(sFile))
                return $"Archivo no existe {sFile}";

            string sCodigo = File.ReadAllText(sFile);

            List<ScriptErrorDescriptor> alErrors = new List<ScriptErrorDescriptor>();
            var Instancia = syntaxEditorHelper.Validar(sCodigo, out alErrors);

            if (alErrors != null && alErrors.Count > 0)
                return "Error en la compilación";

            return "Ok";
        }

        private string VerificaPruebaIntegral(
            Proyecto locProyecto,
            string NombrePrueba,
            Stack<string> stackPaquetes
        )
        {
            string[] PruebaElementos = NombrePrueba
                .ToString()
                .Split('|')
                .Select(s => s.Trim())
                .ToArray();

            if (PruebaElementos.Length != 2)
                return $"La prueba integral {NombrePrueba} está mal configurada. Se esperaba el Paquete y la ruta.";

            if (stackPaquetes.Contains(NombrePrueba))
                return $"La prueba integral {NombrePrueba} se llama recursivamente";

            stackPaquetes.Push(NombrePrueba);

            try
            {
                string Paquete = PruebaElementos[0];

                Proyecto paqProyecto = BuscarPaqueteRecursivamente(Paquete, locProyecto);

                if (paqProyecto.IsNull())
                    return $"El proyecto no existe {Paquete}";

                string ArchivoDePruebaIntegral = Path.Combine(
                    paqProyecto.DirProyectData_Test_IntegralTest,
                    PruebaElementos[1]
                );

                if (!File.Exists(ArchivoDePruebaIntegral))
                    return $"Archivo no existe {ArchivoDePruebaIntegral}";

                string Source = File.ReadAllText(ArchivoDePruebaIntegral);

                if (Source.IsNullOrStringEmpty())
                    return $"Archivo esta vacío  {ArchivoDePruebaIntegral}";

                var Prueba = Config.PruebaIntegral.FromJson(Source);

                Stack<string> Paquetes = new Stack<string>();

                var Pruebas = new List<string>();
                foreach (var PU in Prueba.Pruebas.OrderBy(j => j.Orden))
                {
                    string result = "Ok";

                    if (PU.Tipo == eTypePruebaUnitaria.PruebaUnitaria)
                        result = VerificaPruebaUnitaria(paqProyecto, PU.ArchivoPruebaUnitaria);
                    else if (PU.Tipo == eTypePruebaUnitaria.PruebaIntegral)
                        result = VerificaPruebaIntegral(
                            paqProyecto,
                            PU.ArchivoPruebaUnitaria,
                            stackPaquetes
                        );

                    if (result != "Ok")
                        return result;
                }
            }
            finally
            {
                stackPaquetes.Pop();
            }

            return "Ok";
        }

        //private List<Tuple<string, string>> VerificarPruebaIntegral(
        //    Proyecto locProyecto,
        //    List<string> Pruebas
        //)
        //{
        //    var Resultados = new List<Tuple<string, string>>();

        //    Cursor.Current = Cursors.WaitCursor;

        //    try
        //    {
        //        foreach (var Prueba in Pruebas)
        //        {
        //            string sFile = locProyecto.DirProyectData_Test_UnitTest + Prueba;

        //            if (!File.Exists(sFile))
        //            {
        //                Resultados.Add(new Tuple<string, string>(Prueba, "Archivo no existe"));
        //                continue;
        //            }
        //            string sCodigo = File.ReadAllText(sFile);

        //            List<ScriptErrorDescriptor> alErrors = new List<ScriptErrorDescriptor>();
        //            var Instancia = syntaxEditorHelper.Validar(sCodigo, out alErrors);

        //            if (alErrors != null && alErrors.Count > 0)
        //                Resultados.Add(
        //                    new Tuple<string, string>(Prueba, "Error en la compilación")
        //                );
        //            else
        //                Resultados.Add(new Tuple<string, string>(Prueba, "Ok"));
        //        }
        //        Cursor.Current = Cursors.Default;
        //    }
        //    finally
        //    {
        //        Cursor.Current = Cursors.Default;
        //    }

        //    return Resultados;
        //}

        private void buEjecutarPruebaIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            tabPrincipal.SelectedTabPage = tabPruebasIntegrales2;
            tabIntegral.SelectedTabPage = tabIntegralConsola;
            txtLog = txtLogPruebaIntegral;
            txtLog.Text = String.Empty;

            EjecutaIntegral();
        }

        private Proyecto BuscarPaqueteRecursivamente(string nombrePaquete, Proyecto locProyecto)
        {
            string nombreSinExtension = Path.GetFileNameWithoutExtension(
                locProyecto.ArchivoDeProyecto
            );
            if (nombreSinExtension.Equals(nombrePaquete, StringComparison.OrdinalIgnoreCase))
            {
                return locProyecto;
            }

            Proyecto paqueteEncontrado = null;

            foreach (var paquete in locProyecto.Paquetes)
            {
                string pathReal = Proyecto.ArchivoPaquete(
                    locProyecto.ArchivoDeProyecto,
                    paquete.Archivo
                );
                Proyecto proyecto = new Proyecto();
                proyecto.Load(pathReal);

                nombreSinExtension = Path.GetFileNameWithoutExtension(paquete.Archivo);
                if (nombreSinExtension.Equals(nombrePaquete, StringComparison.OrdinalIgnoreCase))
                {
                    return proyecto;
                }

                // Si no se encuentra en el nivel actual, buscar recursivamente en los subpaquetes
                paqueteEncontrado = BuscarPaqueteRecursivamente(nombrePaquete, proyecto);
                if (paqueteEncontrado != null)
                {
                    return paqueteEncontrado;
                }
            }

            return null; // Retornar null si no se encontró el paquete
        }

        /*
        private void EjecutaIntegralDeIntegrales()
        {
            DateTime Inicio = DateTime.Now;

            string ArchivoDePruebaIntegralDeIntegrales = treeIntegral.FocusedNode.Tag.ToString();

            var PruebaIntegralDeIntegrales = Path.GetFileNameWithoutExtension(
                ArchivoDePruebaIntegralDeIntegrales
            );

            foreach (
                DataRow drRow in (grdElementoPrueba.DataSource as DataTable).Select("", @"Orden")
            )
            {
                string[] PruebaElementos = drRow[@"PruebaUnitaria"]
                    .ToString()
                    .Split('|')
                    .Select(s => s.Trim())
                    .ToArray();

                if (PruebaElementos.Length != 2)
                {
                    ConsoleErrorLog(
                        $"\r\nLa prueba unitaria {drRow[@"PruebaUnitaria"].ToString()} está mal configurada. Se esperaba el Paquete y la ruta.\r\n"
                    );
                    return;
                }

                string Paquete = PruebaElementos[0];

                Proyecto paqProyecto = BuscarPaqueteRecursivamente(Paquete, proyecto);

                string ArchivoDePruebaIntegral = Path.Combine(
                    paqProyecto.DirProyectData_Test_IntegralTest,
                    PruebaElementos[1]
                );

                if (!File.Exists(ArchivoDePruebaIntegral))
                {
                    ConsoleErrorLog($"\r\nArchivo no existe {ArchivoDePruebaIntegral}\r\n");
                    return;
                }

                string Source = File.ReadAllText(ArchivoDePruebaIntegral);

                if (Source.IsNullOrStringEmpty())
                {
                    ConsoleErrorLog($"\r\nArchivo esta vacío  {ArchivoDePruebaIntegral}\r\n");
                    return;
                }

                var Prueba = Config.PruebaIntegral.FromJson(Source);

                var Pruebas = new List<string>();
                foreach (var PU in Prueba.Pruebas.OrderBy(j => j.Orden))
                {
                    if (PU.Tipo == eTypePruebaUnitaria.PruebaUnitaria)
                        Pruebas.Add(PU.ArchivoPruebaUnitaria);
                    else if (PU.Tipo == eTypePruebaUnitaria.PruebaIntegral)
                    {
                        ConsoleCriticalLog(
                            $"\r\nNo se puede ejecutar la prueba {drRow[@"PruebaUnitaria"].ToString()} integral de integrales, {PruebaIntegralDeIntegrales} solo puede llevar pruebas integrales\r\n"
                        );
                        return;
                    }
                }

                //var Resultados = VerificarPruebaIntegral(paqProyecto, Pruebas);

                //foreach (var res in Resultados)
                //{
                //    if (res.Item2 != "Ok")
                //    {
                //        ConsoleCriticalLog(
                //            $"\r\nNo se puede ejecutar la prueba {drRow[@"PruebaUnitaria"].ToString()} tiene errores, verifique {res.Item1}\r\n"
                //        );
                //        return;
                //    }
                //}

                ConsoleTitleLog(
                    $"\r\nCompilación correcta {drRow[@"PruebaUnitaria"].ToString()}\r\n"
                );
                Application.DoEvents();
            }

            foreach (
                DataRow drRow in (grdElementoPrueba.DataSource as DataTable).Select("", @"Orden")
            )
            {
                string[] PruebaElementos = drRow[@"PruebaUnitaria"]
                    .ToString()
                    .Split('|')
                    .Select(s => s.Trim())
                    .ToArray();

                string Paquete = PruebaElementos[0];

                Proyecto paqProyecto = BuscarPaqueteRecursivamente(Paquete, proyecto);

                string ArchivoDePruebaIntegral = Path.Combine(
                    paqProyecto.DirProyectData_Test_IntegralTest,
                    PruebaElementos[1]
                );

                if (!ResuelvePruebaIntegral(paqProyecto, ArchivoDePruebaIntegral))
                    return;
                Application.DoEvents();
            }

            TimeSpan transcurrido = DateTime.Now - Inicio;

            int Segundos = System.Convert.ToInt32(transcurrido.TotalSeconds);

            ConsoleTitleLog(
                $"\r\nPrueba Integral de Integrales terminada {PruebaIntegralDeIntegrales}, {Segundos} segundos\r\n"
            );
        }
        */

        private void EjecutaIntegral()
        {
            buVerificarPruebaIntegral.PerformClick();

            foreach (DataRow drRow in (grdElementoPrueba.DataSource as DataTable).Rows)
            {
                var Tipo = (eTypePruebaUnitaria)
                    Enum.Parse(typeof(eTypePruebaUnitaria), drRow[@"Tipo"].ToString());

                if (
                    (Tipo == eTypePruebaUnitaria.PruebaUnitaria)
                    || (Tipo == eTypePruebaUnitaria.PruebaIntegral)
                )
                {
                    if (drRow[@"Diagnostico"].ToString() != "Ok")
                    {
                        ConsoleCriticalLog(
                            "\r\nNo se puede ejecutar la prueba tiene errores, verifique"
                        );
                        return;
                    }
                }
            }

            string ArchivoDePruebaIntegral = treeIntegral.FocusedNode.Tag.ToString();

            ResuelvePruebaIntegral(proyecto, ArchivoDePruebaIntegral);
        }

        private bool ResuelvePruebaIntegral(Proyecto paqProyecto, string ArchivoDePruebaIntegral)
        {
            if (!File.Exists(ArchivoDePruebaIntegral))
                return false;

            string Source = File.ReadAllText(ArchivoDePruebaIntegral);

            if (Source.IsNullOrStringEmpty())
                return false;

            var Prueba = Config.PruebaIntegral.FromJson(Source);

            Cursor.Current = Cursors.WaitCursor;

            DateTime Inicio = DateTime.Now;

            int iNumeroPrueba = -1;

            var PruebaIntegral = Path.GetFileNameWithoutExtension(ArchivoDePruebaIntegral);

            bool bCambioUdn = false;

            try
            {
                ConsoleTitleLog($"\r\nEjecutando Prueba Integral {PruebaIntegral}\r\n");
                ConsoleTitleLog("\r\nAbriendo el Servicio\r\n");

                proyecto.ParConexion.CierraServicio();
                woGridModelo1.ClearLogin();
                woTarget = null;
                proyecto.ParConexion.IniciaServicio();

                Thread.Sleep(3000);

                foreach (var PU in Prueba.Pruebas.OrderBy(j => j.Orden))
                {
                    iNumeroPrueba = PU.Orden;

                    if (PU.Tipo == eTypePruebaUnitaria.PruebaUnitaria)
                    {
                        Application.DoEvents();
                        ConsoleTitleLog(
                            $"\r\n{PU.Orden}.- Ejecutando Prueba {PU.ArchivoPruebaUnitaria}\r\n"
                        );

                        string sFile =
                            paqProyecto.DirProyectData_Test_UnitTest + PU.ArchivoPruebaUnitaria;

                        DateTime now = DateTime.Now;

                        if (txtModoEjecucion.EditValue.ToString() == repModoEjecucion.Items[0])
                        {
                            if (!EjecutarPruebaEnsambladoInterno(sFile))
                                return false;
                        }
                        else
                        {
                            if (!EjecutarPruebaExecutableExterno(sFile, false))
                                return false;
                        }
                    }
                    else if (PU.Tipo == eTypePruebaUnitaria.PruebaIntegral)
                    {
                        string[] PruebaElementos = PU
                            .ArchivoPruebaUnitaria.ToString()
                            .Split('|')
                            .Select(s => s.Trim())
                            .ToArray();

                        string Paquete = PruebaElementos[0];
                        Proyecto locProyecto = BuscarPaqueteRecursivamente(Paquete, paqProyecto);

                        string locArchivoDePruebaIntegral = Path.Combine(
                            locProyecto.DirProyectData_Test_IntegralTest,
                            PruebaElementos[1]
                        );

                        if (!ResuelvePruebaIntegral(locProyecto, locArchivoDePruebaIntegral))
                            return false;
                    }
                    else if (PU.Tipo == eTypePruebaUnitaria.InicializaDB)
                    {
                        //proyecto.ParConexion.CierraServicio();
                        woGridModelo1.ClearLogin();
                        woTarget = null;
                        Thread.Sleep(2000);

                        string dbName = Path.GetFileNameWithoutExtension(
                            proyecto.ParConexion.DbName()
                        );
                        //$"{proyecto.DirApplication_WebService_WooWServer_DirDBService}\\{proyecto.ParConexion.DbName()}";

                        ConsoleTitleLog($"\r\n{PU.Orden}.- Inicializando DB {dbName} \r\n");

                        string result = EjecutarOperacionDB(
                            new WoDataBaseOperations()
                            {
                                Accion = WooW.Core.Common.tWoDataBaseAccion.Drop,
                                DataBaseName = dbName
                            },
                            true
                        );

                        woTarget = null;

                        if (result.StartsWith("Error"))
                        {
                            throw new Exception(result);
                            break;
                        }
                        else
                        {
                            Application.DoEvents();
                        }

                        //proyecto.ParConexion.IniciaServicio();
                        Thread.Sleep(3000);
                    }
                    else if (PU.Tipo == eTypePruebaUnitaria.RespaldarDB)
                    {
                        woGridModelo1.ClearLogin();
                        woTarget = null;
                        Thread.Sleep(2000);

                        string dbName = Path.GetFileNameWithoutExtension(
                            proyecto.ParConexion.DbName()
                        );
                        //$"{proyecto.DirApplication_WebService_WooWServer_DirDBService}\\{proyecto.ParConexion.DbName()}";

                        ConsoleTitleLog(
                            $"\r\n{PU.Orden}.- Respaldando DB {PU.ArchivoPruebaUnitaria}\r\n"
                        );

                        string result = EjecutarOperacionDB(
                            new WoDataBaseOperations()
                            {
                                Accion = WooW.Core.Common.tWoDataBaseAccion.Backup,
                                DataBaseName = dbName,
                                BackupName = PU.ArchivoPruebaUnitaria
                            },
                            true
                        );

                        woTarget = null;

                        if (result.StartsWith("Error"))
                        {
                            throw new Exception(result);
                            break;
                        }
                        else
                        {
                            Application.DoEvents();
                        }
                    }
                    else if (PU.Tipo == eTypePruebaUnitaria.RecuperarDB)
                    {
                        woGridModelo1.ClearLogin();
                        woTarget = null;
                        Thread.Sleep(2000);

                        string dbName = Path.GetFileNameWithoutExtension(
                            proyecto.ParConexion.DbName()
                        );
                        //$"{proyecto.DirApplication_WebService_WooWServer_DirDBService}\\{proyecto.ParConexion.DbName()}";

                        ConsoleTitleLog(
                            $"\r\n{PU.Orden}.- Recuperando DB {PU.ArchivoPruebaUnitaria}\r\n"
                        );

                        string result = EjecutarOperacionDB(
                            new WoDataBaseOperations()
                            {
                                Accion = WooW.Core.Common.tWoDataBaseAccion.Restore,
                                DataBaseName = dbName,
                                BackupName = PU.ArchivoPruebaUnitaria
                            },
                            true
                        );

                        woTarget = null;

                        if (result.StartsWith("Error"))
                        {
                            throw new Exception(result);
                            break;
                        }
                        else
                        {
                            Application.DoEvents();
                        }
                    }
                    else if (PU.Tipo == eTypePruebaUnitaria.CambiaUdn)
                    {
                        if (woTarget == null)
                            InitWooWTarget();

                        var WoInstanciaUdnResponse = woTarget.Post(
                            new WoInstanciaUdnAsignar
                            {
                                Instance = proyecto.ParConexion.Instance,
                                Udn = PU.ArchivoPruebaUnitaria,
                                Year = proyecto.ParConexion.Year,
                                InstanceType = proyecto.ParConexion.InstanceType
                            }
                        );

                        bCambioUdn = true;

                        Application.DoEvents();
                    }
                }

                TimeSpan transcurrido = DateTime.Now - Inicio;

                int Segundos = System.Convert.ToInt32(transcurrido.TotalSeconds);

                ConsoleTitleLog($"\r\nTiempo de {PruebaIntegral}: {Segundos} segundos \r\n");

                return true;
            }
            catch (Exception Ex)
            {
                if (Ex.InnerException != null)
                    ConsoleCriticalLog(
                        "\r\nExcepción no controlada "
                            + PruebaIntegral
                            + " en : \r\n\r\n"
                            + iNumeroPrueba.ToString()
                            + " : \r\n\r\n"
                            + Ex.Message
                            + "\r\n\r\n"
                            + Ex.InnerException.Message
                    );
                else
                    ConsoleCriticalLog(
                        "\r\nExcepción no controlada "
                            + PruebaIntegral
                            + " en : \r\n\r\n"
                            + iNumeroPrueba.ToString()
                            + " : \r\n\r\n"
                            + Ex.Message
                    );
                return false;
            }
            finally
            {
                if (bCambioUdn)
                {
                    try
                    {
                        var WoInstanciaUdnResponse = woTarget.Post(
                            new WoInstanciaUdnAsignar
                            {
                                Instance = proyecto.ParConexion.Instance,
                                Udn = proyecto.ParConexion.Udn,
                                Year = proyecto.ParConexion.Year,
                                InstanceType = proyecto.ParConexion.InstanceType
                            }
                        );
                    }
                    catch { }
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void grdElementoPruebaView_InitNewRow(
            object sender,
            DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e
        )
        {
            DataTable dt = grdElementoPrueba.DataSource as DataTable;

            int Next = 1;
            foreach (DataRow drRow in dt.Rows)
            {
                if (drRow[@"Orden"].ToInt32() >= Next)
                    Next = drRow[@"Orden"].ToInt32() + 10;
            }

            DataRow dr = (DataRow)grdElementoPruebaView.GetDataRow(e.RowHandle);

            dr[@"Orden"] = Next;
            dr[@"Tipo"] = eTypePruebaUnitaria.PruebaUnitaria.ToString();
        }

        private void buAnularPruebaIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
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

            treeIntegral_FocusedNodeChanged(null, null);
            splitControl.PanelVisibility = SplitPanelVisibility.Both;
        }

        private void buRegistrarPruebaIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            DataTable dtDet = grdElementoPrueba.DataSource as DataTable;

            if (dtDet.Rows.Count == 0)
            {
                XtraMessageBox.Show(
                    "Registre Pruebas Unitarias",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var pruebaIntegral = new PruebaIntegral();

            pruebaIntegral.WorkItem = txtWorkItem.EditValue.ToSafeString();
            pruebaIntegral.ProcesoId = txtProceso.EditValue.ToSafeString();
            pruebaIntegral.Descripcion = txtDescripcion.EditValue.ToSafeString();

            foreach (DataRow drDet in dtDet.Select("", "Orden"))
            {
                var Det = new PruebaUnitaria();

                Det.Orden = drDet[@"Orden"].ToInt32();
                Det.Tipo = (eTypePruebaUnitaria)
                    Enum.Parse(typeof(eTypePruebaUnitaria), drDet[@"Tipo"].ToString());
                Det.ArchivoPruebaUnitaria = drDet[@"PruebaUnitaria"].ToString();
                pruebaIntegral.Pruebas.Add(Det);
            }

            string FileName = treeIntegral.FocusedNode.Tag.ToString();

            File.WriteAllText(FileName, pruebaIntegral.ToJson());
            treeIntegral_FocusedNodeChanged(null, null);
            splitControl.PanelVisibility = SplitPanelVisibility.Both;
        }

        private void grdElementoPruebaView_ValidateRow(
            object sender,
            DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e
        )
        {
            DataRow dr = (DataRow)grdElementoPruebaView.GetDataRow(e.RowHandle);

            if (dr == null)
                return;

            var Tipo = (eTypePruebaUnitaria)
                Enum.Parse(typeof(eTypePruebaUnitaria), dr[@"Tipo"].ToString());

            if (Tipo == eTypePruebaUnitaria.PruebaIntegral)
            {
                if (
                    treeIntegral
                        .FocusedNode.Tag.ToString()
                        .ToLower()
                        .EndsWith(dr[@"PruebaUnitaria"].ToString().ToLower())
                )
                {
                    e.ErrorText = "La Prueba Integral no puede ser ella misma";
                    e.Valid = false;
                    return;
                }
            }

            if (Tipo == eTypePruebaUnitaria.InicializaDB)
            {
                dr[@"PruebaUnitaria"] = string.Empty;
                e.Valid = true;
                return;
            }

            if ((dr[@"PruebaUnitaria"].IsNull()) || (dr[@"PruebaUnitaria"].IsNullOrStringEmpty()))
            {
                if (Tipo == eTypePruebaUnitaria.PruebaUnitaria)
                    e.ErrorText = "Registre una Prueba Unitaria";
                else
                    e.ErrorText = "Registre una Base de Datos";

                e.Valid = false;
                return;
            }
        }

        private void buBorrarPruebaIntegral_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string iTem = treeIntegral.FocusedNode.Tag.ToString();

            if (!File.Exists(iTem))
                return;

            if (
                XtraMessageBox.Show(
                    "Borrar la prueba integral?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            File.Delete(iTem);
            CargarDirectorioIntegral();
        }

        private void grdElementoPruebaView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            txtScriptIntegral.Text = string.Empty;

            DataRow dr = grdElementoPruebaView.GetDataRow(e.FocusedRowHandle);

            if (dr == null)
                return;

            string sFile = proyecto.DirProyectData_Test_UnitTest + dr[@"PruebaUnitaria"].ToString();
            if (File.Exists(sFile))
            {
                txtScriptIntegral.Text = File.ReadAllText(sFile);
            }
        }

        private void buZoomIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            woGridModelo1.ZoomIn();
        }

        private void buZoomOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            woGridModelo1.ZoomOut();
        }

        private void buBorrarInstancia_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string DataBase = WoLib.GetDBName(
                proyecto.ParConexion.Instance,
                proyecto.ParConexion.InstanceType,
                proyecto.ParConexion.Year
            );

            if (sender != null)
            {
                if (
                    XtraMessageBox.Show(
                        $"Borrar la base de datos {DataBase}?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes
                )
                    return;
            }

            string result = EjecutarOperacionDB(
                new WoDataBaseOperations()
                {
                    Accion = WooW.Core.Common.tWoDataBaseAccion.Drop,
                    DataBaseName = DataBase
                },
                false
            );

            woTarget = null;

            if (sender != null)
            {
                XtraMessageBox.Show(
                    result,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void buAbrirInstancia_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if (proyecto.ParConexion.InstanceType != tWoIntanciaType.DEV)
            {
                XtraMessageBox.Show(
                    "Utilice el SQL Server managment studio para explorar las Instancias QAS o PRO",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            string sFile =
                $"{proyecto.DirApplication_WebService_WooWServer_DirDBService}\\{proyecto.ParConexion.DbName()}";
            if (!File.Exists(sFile))
            {
                XtraMessageBox.Show(
                    $"Archivo no existe {sFile}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(sFile) { UseShellExecute = true };
            p.Start();

            //System.Diagnostics.Process.Start(sFile);
        }

        private void buRespaldarInstancia_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            var fmText = new fmNormalizedText();
            fmText.Text = "Nombre del Backup";

            if (fmText.ShowDialog() == DialogResult.OK)
            {
                string DataBase = WoLib.GetDBName(
                    proyecto.ParConexion.Instance,
                    proyecto.ParConexion.InstanceType,
                    proyecto.ParConexion.Year
                );

                string result = EjecutarOperacionDB(
                    new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.Backup,
                        DataBaseName = DataBase,
                        BackupName = fmText.NormalizedText
                    },
                    false
                );

                woTarget = null;

                XtraMessageBox.Show(
                    result,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void buRestaurarInstancia_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string DataBase = WoLib.GetDBName(
                proyecto.ParConexion.Instance,
                proyecto.ParConexion.InstanceType,
                proyecto.ParConexion.Year
            );

            var fmText = new fmComboSelect();
            fmText.Text = "Nombre del Backup";
            string result = EjecutarOperacionDB(
                new WoDataBaseOperations()
                {
                    Accion = WooW.Core.Common.tWoDataBaseAccion.ListBackup,
                    DataBaseName = DataBase,
                },
                false
            );

            fmText.Items = result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (fmText.ShowDialog() == DialogResult.OK)
            {
                result = EjecutarOperacionDB(
                    new WoDataBaseOperations()
                    {
                        Accion = WooW.Core.Common.tWoDataBaseAccion.Restore,
                        DataBaseName = DataBase,
                        BackupName = fmText.SelectedItem
                    },
                    false
                );

                woTarget = null;

                XtraMessageBox.Show(
                    result,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        // TODO Corregir
        private void ModificarPruebaIntegral(string PruebaFuente, string PruebaDestino)
        {
            //string UnitTest = "\\UnitTest";
            //PruebaFuente = PruebaFuente.Substring(PruebaFuente.IndexOf(UnitTest) + UnitTest.Length);
            //PruebaDestino = PruebaDestino.Substring(
            //    PruebaDestino.IndexOf(UnitTest) + UnitTest.Length
            //);

            //DataTable dt = (grdPruebas.DataSource as DataTable);

            //var bCambios = false;

            //foreach (var pruebaIntegral in proyecto.Pruebas)
            //{
            //    var bCambioPrueba = false;
            //    foreach (
            //        var item in pruebaIntegral.Pruebas.Where(
            //            e => e.Tipo == eTypePruebaUnitaria.PruebaUnitaria
            //        )
            //    )
            //    {
            //        if (item.ArchivoPruebaUnitaria == PruebaFuente)
            //        {
            //            item.ArchivoPruebaUnitaria = PruebaDestino;
            //            bCambios = true;
            //            bCambioPrueba = true;
            //        }
            //    }

            //    if (bCambioPrueba)
            //    {
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            if (dr[@"Nombre"].ToString() == pruebaIntegral.Nombre)
            //            {
            //                dr[@"Json"] = pruebaIntegral.ToJson();
            //                grdPruebasView_FocusedRowChanged(null, null);
            //                break;
            //            }
            //        }
            //    }
            //}

            //if (bCambios)
            //    proyecto.Save();
        }

        private void ModificarPruebaIntegralDirectorio(string PruebaFuente, string PruebaDestino)
        {
            //string UnitTest = "\\UnitTest";
            //PruebaFuente =
            //    PruebaFuente.Substring(PruebaFuente.IndexOf(UnitTest) + UnitTest.Length) + "\\";
            //PruebaDestino =
            //    PruebaDestino.Substring(PruebaDestino.IndexOf(UnitTest) + UnitTest.Length) + "\\";

            //DataTable dt = (grdPruebas.DataSource as DataTable);

            //var bCambios = false;

            //foreach (var pruebaIntegral in proyecto.Pruebas)
            //{
            //    var bCambioPrueba = false;
            //    foreach (
            //        var item in pruebaIntegral.Pruebas.Where(
            //            e => e.Tipo == eTypePruebaUnitaria.PruebaUnitaria
            //        )
            //    )
            //    {
            //        if (item.ArchivoPruebaUnitaria.StartsWith(PruebaFuente))
            //        {
            //            item.ArchivoPruebaUnitaria =
            //                PruebaDestino
            //                + item.ArchivoPruebaUnitaria.Substring(PruebaFuente.Length);
            //            bCambios = true;
            //            bCambioPrueba = true;
            //        }
            //    }

            //    if (bCambioPrueba)
            //    {
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            if (dr[@"Nombre"].ToString() == pruebaIntegral.Nombre)
            //            {
            //                dr[@"Json"] = pruebaIntegral.ToJson();
            //                grdPruebasView_FocusedRowChanged(null, null);
            //                break;
            //            }
            //        }
            //    }
            //}

            //if (bCambios)
            //    proyecto.Save();
        }

        private void BorrarPruebaIntegral(string PruebaFuente)
        {
            //string UnitTest = "\\UnitTest";
            //PruebaFuente = PruebaFuente.Substring(PruebaFuente.IndexOf(UnitTest) + UnitTest.Length);

            //DataTable dt = (grdPruebas.DataSource as DataTable);

            //var bCambios = false;

            //foreach (var pruebaIntegral in proyecto.Pruebas)
            //{
            //    var bCambioPrueba = false;
            //    var slBorrar = new List<PruebaUnitaria>();

            //    foreach (
            //        var item in pruebaIntegral.Pruebas.Where(
            //            e => e.Tipo == eTypePruebaUnitaria.PruebaUnitaria
            //        )
            //    )
            //    {
            //        if (item.ArchivoPruebaUnitaria == PruebaFuente)
            //        {
            //            slBorrar.Add(item);
            //            bCambios = true;
            //            bCambioPrueba = true;
            //        }
            //    }

            //    if (bCambioPrueba)
            //    {
            //        foreach (var borrar in slBorrar)
            //            pruebaIntegral.Pruebas.Remove(borrar);

            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            if (dr[@"Nombre"].ToString() == pruebaIntegral.Nombre)
            //            {
            //                dr[@"Json"] = pruebaIntegral.ToJson();
            //                grdPruebasView_FocusedRowChanged(null, null);
            //                break;
            //            }
            //        }
            //    }
            //}

            //if (bCambios)
            //    proyecto.Save();
        }

        private void grdElementoPrueba_Click(object sender, EventArgs e) { }

        private void buComentar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetCurrentScriptEditor().ActiveView.TextChangeActions.CommentLines();
        }

        private void buDescomentar_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            GetCurrentScriptEditor().ActiveView.TextChangeActions.UncommentLines();
        }

        private void buFormatear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetCurrentScriptEditor().ActiveView.TextChangeActions.FormatDocument();
            GetCurrentScriptEditor().Text = SyntaxEditorHelper.PrettyPrint(
                GetCurrentScriptEditor().Text
            );
        }

        private void buAutocomplete_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            GetCurrentScriptEditor().ActiveView.IntelliPrompt.RequestAutoComplete();
        }

        private void buRequestComplete_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            GetCurrentScriptEditor().ActiveView.IntelliPrompt.RequestCompletionSession();
        }

        private void buParameter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetCurrentScriptEditor().ActiveView.IntelliPrompt.RequestParameterInfoSession();
        }

        private void buQuickInfo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetCurrentScriptEditor().ActiveView.IntelliPrompt.RequestQuickInfoSession();
        }

        private void buLogOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            woTarget = null;
        }

        private void txtScript_DoubleClick(object sender, EventArgs e)
        {
            woGridModelo1.Filtra(GetCurrentScriptEditor().ActiveView.SelectedText);
        }

        private void buVisual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EjecutarPruebaExecutableExterno(treeUnitaria.FocusedNode.Tag.ToString(), true);
        }

        private void tabPrincipal_Click(object sender, EventArgs e) { }

        private void treeIntegral_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            buEditarPruebaIntegral.Enabled = false;
            buRegistrarPruebaIntegral.Enabled = false;
            buAnularPruebaIntegral.Enabled = false;
            buVerificarPruebaIntegral.Enabled = false;
            buEjecutarPruebaIntegral.Enabled = false;
            buBorrarPruebaIntegral.Enabled = false;

            treeIntegral.OptionsBehavior.Editable = false;

            txtPrueba.EditValue = null;
            txtWorkItem.EditValue = null;
            txtProceso.EditValue = null;
            txtDescripcion.EditValue = null;

            txtPrueba.Enabled = false;
            txtWorkItem.Enabled = false;
            txtProceso.Enabled = false;
            txtDescripcion.Enabled = false;

            grdElementoPruebaView.OptionsBehavior.Editable = false;
            (grdElementoPrueba.DataSource as DataTable).Rows.Clear();

            if ((treeIntegral.FocusedNode.IsNull()) || (treeIntegral.FocusedNode.Tag.IsNull()))
                return;

            string iTem = treeIntegral.FocusedNode.Tag.ToString();

            if (File.Exists(iTem))
            {
                buEditarPruebaIntegral.Enabled = true;
                string Source = File.ReadAllText(iTem);

                if (!Source.IsNullOrStringEmpty())
                {
                    try
                    {
                        var pruebaIntegral = PruebaIntegral.FromJson(Source);

                        txtPrueba.EditValue = Path.GetFileNameWithoutExtension(iTem);
                        txtWorkItem.EditValue = pruebaIntegral.WorkItem;
                        txtProceso.EditValue = pruebaIntegral.ProcesoId;
                        txtDescripcion.EditValue = pruebaIntegral.Descripcion;

                        foreach (var Det in pruebaIntegral.Pruebas)
                        {
                            DataTable dtDet = grdElementoPrueba.DataSource as DataTable;
                            DataRow drDet = dtDet.NewRow();
                            drDet[@"Orden"] = Det.Orden;
                            drDet[@"Tipo"] = Det.Tipo.ToString();
                            drDet[@"PruebaUnitaria"] = Det.ArchivoPruebaUnitaria;
                            dtDet.Rows.Add(drDet);
                        }
                    }
                    catch { }
                }

                buVerificarPruebaIntegral.Enabled = true;
                buEjecutarPruebaIntegral.Enabled = true;
                buVerificarPruebaIntegral.Enabled = true;
                buBorrarPruebaIntegral.Enabled = true;
            }
        }

        private void buAbrirDBHibrida_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string sFile =
                $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\{proyecto.ParConexion.DbApp}";
            if (!File.Exists(sFile))
            {
                XtraMessageBox.Show(
                    $"Archivo no existe {sFile}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(sFile) { UseShellExecute = true };
            p.Start();
        }

        private void buBorrarDBHibrida_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string sFile =
                $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\{proyecto.ParConexion.DbApp}";
            if (File.Exists(sFile))
            {
                if (sender != null)
                {
                    if (
                        XtraMessageBox.Show(
                            $"Borrar la base de datos {sFile}?",
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                        ) != DialogResult.Yes
                    )
                        return;
                }

                try
                {
                    File.Delete(sFile);
                }
                catch
                {
                    string Message = $"Error al tratar de borrar {sFile} el archivo esta en uso";
                    if (sender != null)
                        XtraMessageBox.Show(
                            $"Error al tratar de borrar {sFile} el archivo esta en uso",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );
                    else
                        throw new Exception(Message);
                }
            }
        }

        private void buRespaldarDBHibrida_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string ArchivoFuente =
                $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\{proyecto.ParConexion.DbApp}";
            if (!File.Exists(ArchivoFuente))
            {
                XtraMessageBox.Show(
                    $"No existe la base de datos {ArchivoFuente}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            string ArchivoDestino = string.Empty;

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Base de datos SQL Lite (*.sqlite)|*.sqlite";
                saveDialog.FileName = string.Empty;
                saveDialog.RestoreDirectory = true;

                string sDirFijo =
                    $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\DataBaseBackup";
                if (Directory.Exists(sDirFijo))
                    saveDialog.InitialDirectory = sDirFijo;

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                ArchivoDestino = saveDialog.FileName;
            }

            try
            {
                File.Copy(ArchivoFuente, ArchivoDestino, true);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Error al tratar de copiar de {ArchivoFuente} a {ArchivoDestino}  {ex.Message}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
        }

        private void buRestaurarHibrida_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            string ArchivoDestino =
                $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\{proyecto.ParConexion.DbApp}";

            if (File.Exists(ArchivoDestino))
            {
                if (
                    XtraMessageBox.Show(
                        $"La base de datos {ArchivoDestino} existe, sobre escribirla?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    ) != DialogResult.Yes
                )
                    return;
            }

            string ArchivoFuente = string.Empty;

            using (OpenFileDialog saveDialog = new OpenFileDialog())
            {
                saveDialog.Filter = "Base de datos SQL Lite (*.sqlite)|*.sqlite";
                saveDialog.FileName = string.Empty;
                saveDialog.RestoreDirectory = true;

                string sDirFijo =
                    $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\DataBaseBackup";
                if (Directory.Exists(sDirFijo))
                    saveDialog.InitialDirectory = sDirFijo;

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                ArchivoFuente = saveDialog.FileName;
            }

            try
            {
                File.Copy(ArchivoFuente, ArchivoDestino, true);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Error al tratar de copiar de {ArchivoFuente} a {ArchivoDestino} {ex.Message}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
        }

        private void repDBaRecuperar_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            string ArchivoFuente = string.Empty;

            using (OpenFileDialog saveDialog = new OpenFileDialog())
            {
                saveDialog.Filter = "Base de datos SQL Lite (*.sqlite)|*.sqlite";
                saveDialog.FileName = string.Empty;
                saveDialog.RestoreDirectory = true;

                string sDirFijo =
                    $"{proyecto.DirApplication_WebService_WooWServer_DirDBService}\\DataBaseBackup";
                if (Directory.Exists(sDirFijo))
                    saveDialog.InitialDirectory = sDirFijo;

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                ArchivoFuente = saveDialog.FileName;

                txtDBaRecuperar.EditValue = Path.GetFileName(ArchivoFuente);
            }
        }

        private void txtDBaRecuperar_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.DBRestore = txtDBaRecuperar.EditValue.ToSafeString();
            proyecto.Save();
        }

        private void repDBAppaRecuperar_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            string ArchivoFuente = string.Empty;

            using (OpenFileDialog saveDialog = new OpenFileDialog())
            {
                saveDialog.Filter = "Base de datos SQL Lite (*.sqlite)|*.sqlite";
                saveDialog.FileName = string.Empty;
                saveDialog.RestoreDirectory = true;

                string sDirFijo =
                    $"{proyecto.DirApplication_WebService_WooWServer_DBApp}\\DataBaseBackup";
                if (Directory.Exists(sDirFijo))
                    saveDialog.InitialDirectory = sDirFijo;

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                ArchivoFuente = saveDialog.FileName;

                txtDBAppaRecuperar.EditValue = Path.GetFileName(ArchivoFuente);
            }
        }

        private void txtDBAppaRecuperar_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.DBRestoreApp = txtDBAppaRecuperar.EditValue.ToSafeString();
            proyecto.Save();
        }

        private void txtEjecutarAccion_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.DBAccion = txtEjecutarAccion.EditValue.ToSafeString();
            proyecto.Save();
        }

        private void txtEjecutarAccionApp_EditValueChanged(object sender, EventArgs e)
        {
            proyecto.ParConexion.DBAccionApp = txtEjecutarAccionApp.EditValue.ToSafeString();
            proyecto.Save();
        }

        private void buSeleccionaExcepcion_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            var fmNE = new fmNumberedExceptionsSelector(false);

            if (fmNE.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Clipboard.SetDataObject(fmNE.NumeredExcepcion, true);
                    GetCurrentScriptEditor().ActiveView.PasteFromClipboard();
                    Clipboard.Clear();
                }
                catch { }
            }
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

        private void txtBuscarTexto_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            if (txtBuscarTexto.EditValue.IsNullOrStringEmpty())
                return;

            bool currentNodeFound = false;
            TreeListNode currentNode = null;
            if (
                (treeUnitaria.FocusedNode != null)
                && (!Directory.Exists((treeUnitaria.FocusedNode as TreeListNode).Tag.ToString()))
            )
            {
                currentNode = treeUnitaria.FocusedNode;
                currentNodeFound = true;
            }

            bool Found = false;
            foreach (TreeListNode node in treeUnitaria.Nodes)
                if (ScanNode(node, txtBuscarTexto.EditValue.ToString(), ref currentNode))
                {
                    Found = true;
                    break;
                }

            if ((!Found) && (currentNodeFound))
            {
                currentNode = null;
                foreach (TreeListNode node in treeUnitaria.Nodes)
                    if (ScanNode(node, txtBuscarTexto.EditValue.ToString(), ref currentNode))
                        break;
            }
        }

        private bool ScanNode(
            TreeListNode rootNode,
            string SearchText,
            ref TreeListNode currentNode
        )
        {
            foreach (var node in rootNode.Nodes)
            {
                if ((node as TreeListNode).Tag.IsNull())
                    continue;

                string DirOrFile = (node as TreeListNode).Tag.ToString();

                if (Directory.Exists(DirOrFile))
                {
                    if (ScanNode(node as TreeListNode, SearchText, ref currentNode))
                        return true;
                }
                else
                {
                    if ((currentNode != null) && (currentNode == node))
                    {
                        currentNode = null;
                        continue;
                    }

                    if (currentNode != null)
                        continue;

                    if (File.Exists(DirOrFile) == false)
                        continue;

                    string[] Lineas = File.ReadAllLines(DirOrFile);
                    foreach (string Linea in Lineas)
                    {
                        if (Linea.ToUpper().Contains(SearchText.ToUpper()))
                        {
                            txtScript.Focus();
                            treeUnitaria.FocusedNode = node as TreeListNode;
                            treeUnitaria.MakeNodeVisible(node as TreeListNode);

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
            }

            return false;
        }

        private void buDBControl_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sFile =
                $"{proyecto.DirApplication_WebService_WooWServer_DirDBService}\\WooW.DB.Config.sqlite";
            if (!File.Exists(sFile))
            {
                XtraMessageBox.Show(
                    $"Archivo no existe {sFile}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(sFile) { UseShellExecute = true };
            p.Start();
        }

        private void mnuSnippetException_Click(object sender, EventArgs e)
        {
            setSnippet(@"throw new Exception(string.Format(""""));");
        }

        private void mnuFormatear_Click(object sender, EventArgs e)
        {
            if (GetCurrentScriptEditor().Document.IsReadOnly)
                return;
            buFormatear.PerformClick();
        }

        private void fmTestCases_Load(object sender, EventArgs e)
        {
            //string DataBase = WoLib.GetDBName(
            //    proyecto.ParConexion.Instance,
            //    proyecto.ParConexion.InstanceType,
            //    proyecto.ParConexion.Year
            //);

            //string result = EjecutarOperacionDB(
            //    new WoDataBaseOperations()
            //    {
            //        Accion = WooW.Core.Common.tWoDataBaseAccion.ListBackup,
            //        DataBaseName = DataBase,
            //    }
            //);

            //repDBaRecuperar.Items.Clear();

            //repDBaRecuperar.Items.AddRange(
            //    result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            //);

            //if (repDBaRecuperar.Items.IndexOf(proyecto.ParConexion.DBRestore) != -1)
            //    txtDBaRecuperar.EditValue = proyecto.ParConexion.DBRestore;
        }
    }

    public class ServSock
    {
        //escucha y estatus
        private WoSocketListener _Listener;
        private bool _IsRuning = false;

        //controlador de eventos para devolver lo que llego
        public event EventHandler<string> EventGetData;

        public async void Run()
        {
            _IsRuning = true;
            await InicializarServidor();
        }

        public void Stop()
        {
            _IsRuning = false;
            if (_Listener != null)
                _Listener.Dispose();
        }

        private async Task InicializarServidor()
        {
            _Listener = new WoSocketListener(1337); // configuracion del listener la ip esta dentro de a clase y sera 127.0.0.1 sobre el protocolo tcp

            while (_IsRuning)
            {
                using (var remote = _Listener.Accept()) // Accepts a connection (blocks execution)
                {
                    if (remote == null)
                        break;
                    var data = remote.Receive(); // Receives data (blocks execution)
                    EventGetData.Invoke(this, data.ToString());
                    //remote.Send(data); // Sends the received data back
                }
            }
        }
    }
}
