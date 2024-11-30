namespace WooW.SB.Forms
{
    partial class fmModelDTO
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.buRefrescarModelos = new DevExpress.XtraBars.BarButtonItem();
            this.buEditar = new DevExpress.XtraBars.BarButtonItem();
            this.buRegistrar = new DevExpress.XtraBars.BarButtonItem();
            this.buAnular = new DevExpress.XtraBars.BarButtonItem();
            this.buCompilar = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.grpCambios = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.splitControl = new DevExpress.XtraEditors.SplitContainerControl();
            this.grdModelos = new DevExpress.XtraGrid.GridControl();
            this.grdModelosView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tabCode = new DevExpress.XtraTab.XtraTabControl();
            this.tabPagePre = new DevExpress.XtraTab.XtraTabPage();
            this.txtScriptPre = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            this.contextScriptMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuDeshacerItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparador1Item = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCortarItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopiarItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPegarItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparador2Item = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSeleccionarTodoItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparador3Item = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFormatear = new System.Windows.Forms.ToolStripMenuItem();
            this.navigableSymbolSelector1 = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector();
            this.tabPagePost = new DevExpress.XtraTab.XtraTabPage();
            this.txtScriptPost = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            this.navigableSymbolSelector2 = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector();
            this.tabInferior = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.woGridModelo1 = new WooW.SB.Config.Editors.woGridModel();
            this.tabError = new DevExpress.XtraTab.XtraTabPage();
            this.lstErrors = new System.Windows.Forms.ListView();
            this.colNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colErrorNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLineNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colColNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.woDiagram1 = new WooW.SB.Config.woDiagram();
            this.buSeleccionaExcepcion = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel1)).BeginInit();
            this.splitControl.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel2)).BeginInit();
            this.splitControl.Panel2.SuspendLayout();
            this.splitControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdModelos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModelosView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2.Panel1)).BeginInit();
            this.splitContainerControl2.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2.Panel2)).BeginInit();
            this.splitContainerControl2.Panel2.SuspendLayout();
            this.splitContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabCode)).BeginInit();
            this.tabCode.SuspendLayout();
            this.tabPagePre.SuspendLayout();
            this.contextScriptMenu.SuspendLayout();
            this.tabPagePost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabInferior)).BeginInit();
            this.tabInferior.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.tabError.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.buRefrescarModelos,
            this.buEditar,
            this.buRegistrar,
            this.buAnular,
            this.buCompilar,
            this.buSeleccionaExcepcion});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 9;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1176, 150);
            // 
            // buRefrescarModelos
            // 
            this.buRefrescarModelos.Caption = "Refrescar Modelos";
            this.buRefrescarModelos.Id = 3;
            this.buRefrescarModelos.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Code_Snippet_Tick;
            this.buRefrescarModelos.Name = "buRefrescarModelos";
            this.buRefrescarModelos.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buRefrescarModelos_ItemClick);
            // 
            // buEditar
            // 
            this.buEditar.Caption = "Editar";
            this.buEditar.Enabled = false;
            this.buEditar.Id = 4;
            this.buEditar.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Query_Design;
            this.buEditar.Name = "buEditar";
            this.buEditar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buEditar_ItemClick);
            // 
            // buRegistrar
            // 
            this.buRegistrar.Caption = "Grabar";
            this.buRegistrar.Enabled = false;
            this.buRegistrar.Id = 5;
            this.buRegistrar.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.dbTable_Save;
            this.buRegistrar.Name = "buRegistrar";
            this.buRegistrar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buRegistrar_ItemClick);
            // 
            // buAnular
            // 
            this.buAnular.Caption = "Deshacer Cambios";
            this.buAnular.Enabled = false;
            this.buAnular.Id = 6;
            this.buAnular.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.dbTable_Refresh;
            this.buAnular.Name = "buAnular";
            this.buAnular.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buAnular_ItemClick);
            // 
            // buCompilar
            // 
            this.buCompilar.Caption = "Compilar";
            this.buCompilar.Enabled = false;
            this.buCompilar.Id = 7;
            this.buCompilar.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Dev_Build_Selection;
            this.buCompilar.Name = "buCompilar";
            this.buCompilar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buCompilar_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.grpCambios,
            this.ribbonPageGroup2});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Modelos DTO";
            // 
            // grpCambios
            // 
            this.grpCambios.ItemLinks.Add(this.buRefrescarModelos, true);
            this.grpCambios.Name = "grpCambios";
            this.grpCambios.Text = "Modelos";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.buEditar);
            this.ribbonPageGroup2.ItemLinks.Add(this.buRegistrar);
            this.ribbonPageGroup2.ItemLinks.Add(this.buAnular);
            this.ribbonPageGroup2.ItemLinks.Add(this.buCompilar, true);
            this.ribbonPageGroup2.ItemLinks.Add(this.buSeleccionaExcepcion, true);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Codigo";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.splitControl);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 150);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1176, 573);
            this.panelControl1.TabIndex = 2;
            // 
            // splitControl
            // 
            this.splitControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitControl.Location = new System.Drawing.Point(2, 2);
            this.splitControl.Name = "splitControl";
            // 
            // splitControl.Panel1
            // 
            this.splitControl.Panel1.Controls.Add(this.grdModelos);
            this.splitControl.Panel1.Padding = new System.Windows.Forms.Padding(10);
            this.splitControl.Panel1.Text = "Panel1";
            // 
            // splitControl.Panel2
            // 
            this.splitControl.Panel2.Controls.Add(this.splitContainerControl2);
            this.splitControl.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitControl.Panel2.Text = "Panel2";
            this.splitControl.Size = new System.Drawing.Size(1172, 569);
            this.splitControl.SplitterPosition = 308;
            this.splitControl.TabIndex = 0;
            // 
            // grdModelos
            // 
            this.grdModelos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdModelos.Location = new System.Drawing.Point(10, 10);
            this.grdModelos.MainView = this.grdModelosView;
            this.grdModelos.MenuManager = this.ribbonControl1;
            this.grdModelos.Name = "grdModelos";
            this.grdModelos.Size = new System.Drawing.Size(288, 549);
            this.grdModelos.TabIndex = 1;
            this.grdModelos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdModelosView});
            // 
            // grdModelosView
            // 
            this.grdModelosView.ColumnPanelRowHeight = 0;
            this.grdModelosView.FooterPanelHeight = 0;
            this.grdModelosView.GridControl = this.grdModelos;
            this.grdModelosView.GroupRowHeight = 0;
            this.grdModelosView.LevelIndent = 0;
            this.grdModelosView.Name = "grdModelosView";
            this.grdModelosView.OptionsBehavior.Editable = false;
            this.grdModelosView.OptionsView.ShowAutoFilterRow = true;
            this.grdModelosView.PreviewIndent = 0;
            this.grdModelosView.RowHeight = 0;
            this.grdModelosView.ViewCaptionHeight = 0;
            this.grdModelosView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grdModelosView_FocusedRowChanged);
            // 
            // splitContainerControl2
            // 
            this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl2.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl2.Horizontal = false;
            this.splitContainerControl2.Location = new System.Drawing.Point(10, 10);
            this.splitContainerControl2.Name = "splitContainerControl2";
            // 
            // splitContainerControl2.Panel1
            // 
            this.splitContainerControl2.Panel1.Controls.Add(this.tabCode);
            this.splitContainerControl2.Panel1.Text = "Panel1";
            // 
            // splitContainerControl2.Panel2
            // 
            this.splitContainerControl2.Panel2.Controls.Add(this.tabInferior);
            this.splitContainerControl2.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainerControl2.Panel2.Text = "Panel2";
            this.splitContainerControl2.Size = new System.Drawing.Size(834, 549);
            this.splitContainerControl2.SplitterPosition = 217;
            this.splitContainerControl2.TabIndex = 0;
            // 
            // tabCode
            // 
            this.tabCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCode.Location = new System.Drawing.Point(0, 0);
            this.tabCode.Name = "tabCode";
            this.tabCode.SelectedTabPage = this.tabPagePre;
            this.tabCode.Size = new System.Drawing.Size(834, 322);
            this.tabCode.TabIndex = 0;
            this.tabCode.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabPagePre,
            this.tabPagePost});
            this.tabCode.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabCode_SelectedPageChanged);
            // 
            // tabPagePre
            // 
            this.tabPagePre.Controls.Add(this.txtScriptPre);
            this.tabPagePre.Controls.Add(this.navigableSymbolSelector1);
            this.tabPagePre.Name = "tabPagePre";
            this.tabPagePre.Padding = new System.Windows.Forms.Padding(10);
            this.tabPagePre.Size = new System.Drawing.Size(832, 297);
            this.tabPagePre.Text = "Pre Condiciones";
            // 
            // txtScriptPre
            // 
            this.txtScriptPre.AllowDrop = true;
            this.txtScriptPre.ContextMenuStrip = this.contextScriptMenu;
            this.txtScriptPre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtScriptPre.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScriptPre.IsCurrentLineHighlightingEnabled = true;
            this.txtScriptPre.IsLineNumberMarginVisible = true;
            this.txtScriptPre.Location = new System.Drawing.Point(10, 31);
            this.txtScriptPre.Name = "txtScriptPre";
            this.txtScriptPre.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtScriptPre.Size = new System.Drawing.Size(812, 256);
            this.txtScriptPre.TabIndex = 0;
            this.txtScriptPre.Click += new System.EventHandler(this.txtScriptPre_Click);
            // 
            // contextScriptMenu
            // 
            this.contextScriptMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.contextScriptMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDeshacerItem,
            this.mnuSeparador1Item,
            this.mnuCortarItem,
            this.mnuCopiarItem,
            this.mnuPegarItem,
            this.mnuSeparador2Item,
            this.mnuSeleccionarTodoItem,
            this.mnuSeparador3Item,
            this.mnuFormatear});
            this.contextScriptMenu.Name = "contextMenuStrip1";
            this.contextScriptMenu.Size = new System.Drawing.Size(171, 154);
            // 
            // mnuDeshacerItem
            // 
            this.mnuDeshacerItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuDeshacerItem.Name = "mnuDeshacerItem";
            this.mnuDeshacerItem.Size = new System.Drawing.Size(170, 22);
            this.mnuDeshacerItem.Text = "Deshacer";
            this.mnuDeshacerItem.Click += new System.EventHandler(this.mnuDeshacerItem_Click);
            // 
            // mnuSeparador1Item
            // 
            this.mnuSeparador1Item.Name = "mnuSeparador1Item";
            this.mnuSeparador1Item.Size = new System.Drawing.Size(167, 6);
            // 
            // mnuCortarItem
            // 
            this.mnuCortarItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuCortarItem.Name = "mnuCortarItem";
            this.mnuCortarItem.Size = new System.Drawing.Size(170, 22);
            this.mnuCortarItem.Text = "Cortar";
            this.mnuCortarItem.Click += new System.EventHandler(this.mnuCortarItem_Click);
            // 
            // mnuCopiarItem
            // 
            this.mnuCopiarItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuCopiarItem.Name = "mnuCopiarItem";
            this.mnuCopiarItem.Size = new System.Drawing.Size(170, 22);
            this.mnuCopiarItem.Text = "Copiar";
            this.mnuCopiarItem.Click += new System.EventHandler(this.mnuCopiarItem_Click);
            // 
            // mnuPegarItem
            // 
            this.mnuPegarItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuPegarItem.Name = "mnuPegarItem";
            this.mnuPegarItem.Size = new System.Drawing.Size(170, 22);
            this.mnuPegarItem.Text = "Pegar";
            this.mnuPegarItem.Click += new System.EventHandler(this.mnuPegarItem_Click);
            // 
            // mnuSeparador2Item
            // 
            this.mnuSeparador2Item.Name = "mnuSeparador2Item";
            this.mnuSeparador2Item.Size = new System.Drawing.Size(167, 6);
            // 
            // mnuSeleccionarTodoItem
            // 
            this.mnuSeleccionarTodoItem.Name = "mnuSeleccionarTodoItem";
            this.mnuSeleccionarTodoItem.Size = new System.Drawing.Size(170, 22);
            this.mnuSeleccionarTodoItem.Text = "Seleccionar Todo";
            this.mnuSeleccionarTodoItem.Click += new System.EventHandler(this.mnuSeleccionarTodoItem_Click);
            // 
            // mnuSeparador3Item
            // 
            this.mnuSeparador3Item.Name = "mnuSeparador3Item";
            this.mnuSeparador3Item.Size = new System.Drawing.Size(167, 6);
            // 
            // mnuFormatear
            // 
            this.mnuFormatear.Name = "mnuFormatear";
            this.mnuFormatear.Size = new System.Drawing.Size(170, 22);
            this.mnuFormatear.Text = "Formatear Código";
            this.mnuFormatear.Click += new System.EventHandler(this.mnuFormatear_Click);
            // 
            // navigableSymbolSelector1
            // 
            this.navigableSymbolSelector1.Dock = System.Windows.Forms.DockStyle.Top;
            this.navigableSymbolSelector1.Location = new System.Drawing.Point(10, 10);
            this.navigableSymbolSelector1.Name = "navigableSymbolSelector1";
            this.navigableSymbolSelector1.Size = new System.Drawing.Size(812, 21);
            this.navigableSymbolSelector1.SyntaxEditor = this.txtScriptPre;
            this.navigableSymbolSelector1.TabIndex = 1;
            this.navigableSymbolSelector1.Text = "navigableSymbolSelector1";
            // 
            // tabPagePost
            // 
            this.tabPagePost.Controls.Add(this.txtScriptPost);
            this.tabPagePost.Controls.Add(this.navigableSymbolSelector2);
            this.tabPagePost.Name = "tabPagePost";
            this.tabPagePost.Padding = new System.Windows.Forms.Padding(10);
            this.tabPagePost.Size = new System.Drawing.Size(832, 297);
            this.tabPagePost.Text = "Post Condiciones";
            // 
            // txtScriptPost
            // 
            this.txtScriptPost.AllowDrop = true;
            this.txtScriptPost.ContextMenuStrip = this.contextScriptMenu;
            this.txtScriptPost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtScriptPost.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScriptPost.IsCurrentLineHighlightingEnabled = true;
            this.txtScriptPost.IsLineNumberMarginVisible = true;
            this.txtScriptPost.Location = new System.Drawing.Point(10, 31);
            this.txtScriptPost.Name = "txtScriptPost";
            this.txtScriptPost.Size = new System.Drawing.Size(812, 256);
            this.txtScriptPost.TabIndex = 1;
            // 
            // navigableSymbolSelector2
            // 
            this.navigableSymbolSelector2.Dock = System.Windows.Forms.DockStyle.Top;
            this.navigableSymbolSelector2.Location = new System.Drawing.Point(10, 10);
            this.navigableSymbolSelector2.Name = "navigableSymbolSelector2";
            this.navigableSymbolSelector2.Size = new System.Drawing.Size(812, 21);
            this.navigableSymbolSelector2.SyntaxEditor = this.txtScriptPost;
            this.navigableSymbolSelector2.TabIndex = 2;
            this.navigableSymbolSelector2.Text = "navigableSymbolSelector2";
            // 
            // tabInferior
            // 
            this.tabInferior.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabInferior.Location = new System.Drawing.Point(10, 10);
            this.tabInferior.Name = "tabInferior";
            this.tabInferior.SelectedTabPage = this.xtraTabPage1;
            this.tabInferior.Size = new System.Drawing.Size(814, 197);
            this.tabInferior.TabIndex = 0;
            this.tabInferior.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.tabError});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.woGridModelo1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Padding = new System.Windows.Forms.Padding(10);
            this.xtraTabPage1.Size = new System.Drawing.Size(812, 172);
            this.xtraTabPage1.Text = "Modelos";
            // 
            // woGridModelo1
            // 
            this.woGridModelo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.woGridModelo1.Location = new System.Drawing.Point(10, 10);
            this.woGridModelo1.Name = "woGridModelo1";
            this.woGridModelo1.SetSnippet = null;
            this.woGridModelo1.Size = new System.Drawing.Size(792, 152);
            this.woGridModelo1.Snippet = WooW.SB.Config.Editors.woGridModel.eSnippet.NoMostrar;
            this.woGridModelo1.TabIndex = 0;
            // 
            // tabError
            // 
            this.tabError.Controls.Add(this.lstErrors);
            this.tabError.Name = "tabError";
            this.tabError.Padding = new System.Windows.Forms.Padding(10);
            this.tabError.Size = new System.Drawing.Size(812, 172);
            this.tabError.Text = "Errores";
            // 
            // lstErrors
            // 
            this.lstErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNum,
            this.colErrorNum,
            this.colDescription,
            this.colLineNum,
            this.colColNum});
            this.lstErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstErrors.FullRowSelect = true;
            this.lstErrors.GridLines = true;
            this.lstErrors.HideSelection = false;
            this.lstErrors.Location = new System.Drawing.Point(10, 10);
            this.lstErrors.Name = "lstErrors";
            this.lstErrors.Size = new System.Drawing.Size(792, 152);
            this.lstErrors.TabIndex = 8;
            this.lstErrors.UseCompatibleStateImageBehavior = false;
            this.lstErrors.View = System.Windows.Forms.View.Details;
            this.lstErrors.DoubleClick += new System.EventHandler(this.lstErrors_DoubleClick);
            // 
            // colNum
            // 
            this.colNum.Text = "";
            this.colNum.Width = 34;
            // 
            // colErrorNum
            // 
            this.colErrorNum.Text = "Error";
            // 
            // colDescription
            // 
            this.colDescription.Text = "Descripción";
            this.colDescription.Width = 569;
            // 
            // colLineNum
            // 
            this.colLineNum.Text = "Linea";
            // 
            // colColNum
            // 
            this.colColNum.Text = "Columna";
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "treeListColumn2";
            this.treeListColumn1.FieldName = "treeListColumn2";
            this.treeListColumn1.MinWidth = 52;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsFilter.AutoFilterCondition = DevExpress.XtraTreeList.Columns.AutoFilterCondition.Greater;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 108;
            // 
            // woDiagram1
            // 
            this.woDiagram1.currModel = null;
            this.woDiagram1.Enabled = false;
            this.woDiagram1.Location = new System.Drawing.Point(22, 17);
            this.woDiagram1.MostrarPermisos = false;
            this.woDiagram1.Name = "woDiagram1";
            this.woDiagram1.Size = new System.Drawing.Size(909, 518);
            this.woDiagram1.TabIndex = 2;
            // 
            // buSeleccionaExcepcion
            // 
            this.buSeleccionaExcepcion.Caption = "Mensajes de Error";
            this.buSeleccionaExcepcion.Enabled = false;
            this.buSeleccionaExcepcion.Id = 8;
            this.buSeleccionaExcepcion.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Expenses_Error;
            this.buSeleccionaExcepcion.Name = "buSeleccionaExcepcion";
            this.buSeleccionaExcepcion.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buSeleccionaExcepcion_ItemClick);
            // 
            // fmModelDTO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "fmModelDTO";
            this.Size = new System.Drawing.Size(1176, 723);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel1)).EndInit();
            this.splitControl.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel2)).EndInit();
            this.splitControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitControl)).EndInit();
            this.splitControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdModelos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModelosView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2.Panel1)).EndInit();
            this.splitContainerControl2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2.Panel2)).EndInit();
            this.splitContainerControl2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).EndInit();
            this.splitContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabCode)).EndInit();
            this.tabCode.ResumeLayout(false);
            this.tabPagePre.ResumeLayout(false);
            this.contextScriptMenu.ResumeLayout(false);
            this.tabPagePost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabInferior)).EndInit();
            this.tabInferior.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.tabError.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup grpCambios;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.BarButtonItem buRefrescarModelos;
        private DevExpress.XtraEditors.SplitContainerControl splitControl;
        private DevExpress.XtraGrid.GridControl grdModelos;
        private DevExpress.XtraGrid.Views.Grid.GridView grdModelosView;
        private DevExpress.XtraTab.XtraTabControl tabCode;
        private DevExpress.XtraTab.XtraTabPage tabPagePre;
        private DevExpress.XtraTab.XtraTabPage tabPagePost;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        private System.Windows.Forms.ContextMenuStrip contextScriptMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuDeshacerItem;
        private System.Windows.Forms.ToolStripSeparator mnuSeparador1Item;
        private System.Windows.Forms.ToolStripMenuItem mnuCortarItem;
        private System.Windows.Forms.ToolStripMenuItem mnuCopiarItem;
        private System.Windows.Forms.ToolStripMenuItem mnuPegarItem;
        private System.Windows.Forms.ToolStripSeparator mnuSeparador2Item;
        private System.Windows.Forms.ToolStripMenuItem mnuSeleccionarTodoItem;
        private System.Windows.Forms.ToolStripSeparator mnuSeparador3Item;
        private System.Windows.Forms.ToolStripMenuItem mnuFormatear;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem buEditar;
        private DevExpress.XtraBars.BarButtonItem buRegistrar;
        private DevExpress.XtraBars.BarButtonItem buAnular;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtScriptPre;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtScriptPost;
        private DevExpress.XtraTab.XtraTabControl tabInferior;
        private DevExpress.XtraTab.XtraTabPage tabError;
        private System.Windows.Forms.ListView lstErrors;
        private System.Windows.Forms.ColumnHeader colNum;
        private System.Windows.Forms.ColumnHeader colErrorNum;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colLineNum;
        private System.Windows.Forms.ColumnHeader colColNum;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraBars.BarButtonItem buCompilar;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector navigableSymbolSelector1;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector navigableSymbolSelector2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private Config.Editors.woGridModel woGridModelo1;
        private Config.woDiagram woDiagram1;
        private DevExpress.XtraBars.BarButtonItem buSeleccionaExcepcion;
    }
}
