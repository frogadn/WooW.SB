namespace WooW.SB.Forms
{
    partial class fmPackageManager
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmPackageManager));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buRefrescar = new DevExpress.XtraBars.BarButtonItem();
            buAgregar = new DevExpress.XtraBars.BarButtonItem();
            buQuitar = new DevExpress.XtraBars.BarButtonItem();
            buAbrir = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            treePackages = new DevExpress.XtraTreeList.TreeList();
            treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            imgTree = new System.Windows.Forms.ImageList(components);
            groupControl1 = new DevExpress.XtraEditors.GroupControl();
            xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            grdModelos = new DevExpress.XtraGrid.GridControl();
            grdModelosView = new DevExpress.XtraGrid.Views.Grid.GridView();
            groupControl2 = new DevExpress.XtraEditors.GroupControl();
            woModelInspector1 = new Config.Editors.woModelInspector();
            xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
            propertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            groupControl3 = new DevExpress.XtraEditors.GroupControl();
            mstHtmlEditor1 = new BaiqiSoft.HtmlEditorControl.MstHtmlEditor();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treePackages).BeginInit();
            ((System.ComponentModel.ISupportInitialize)groupControl1).BeginInit();
            groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).BeginInit();
            xtraTabControl1.SuspendLayout();
            xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel1).BeginInit();
            splitContainerControl2.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel2).BeginInit();
            splitContainerControl2.Panel2.SuspendLayout();
            splitContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdModelos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdModelosView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)groupControl2).BeginInit();
            groupControl2.SuspendLayout();
            xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel1).BeginInit();
            splitContainerControl3.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel2).BeginInit();
            splitContainerControl3.Panel2.SuspendLayout();
            splitContainerControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)propertyGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)groupControl3).BeginInit();
            groupControl3.SuspendLayout();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(15, 16, 15, 16);
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, buRefrescar, buAgregar, buQuitar, buAbrir });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.Margin = new System.Windows.Forms.Padding(2);
            ribbonControl1.MaxItemId = 5;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.OptionsMenuMinWidth = 165;
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.Size = new System.Drawing.Size(989, 150);
            // 
            // buRefrescar
            // 
            buRefrescar.Caption = "Refrescar";
            buRefrescar.Id = 1;
            buRefrescar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.Image");
            buRefrescar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.LargeImage");
            buRefrescar.Name = "buRefrescar";
            buRefrescar.ItemClick += buRefrescar_ItemClick;
            // 
            // buAgregar
            // 
            buAgregar.Caption = "Agregar";
            buAgregar.Enabled = false;
            buAgregar.Id = 2;
            buAgregar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buAgregar.ImageOptions.Image");
            buAgregar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buAgregar.ImageOptions.LargeImage");
            buAgregar.Name = "buAgregar";
            buAgregar.ItemClick += buAgregar_ItemClick;
            // 
            // buQuitar
            // 
            buQuitar.Caption = "Quitar";
            buQuitar.Enabled = false;
            buQuitar.Id = 3;
            buQuitar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buQuitar.ImageOptions.Image");
            buQuitar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buQuitar.ImageOptions.LargeImage");
            buQuitar.Name = "buQuitar";
            buQuitar.ItemClick += buQuitar_ItemClick;
            // 
            // buAbrir
            // 
            buAbrir.Caption = "Abrir";
            buAbrir.Id = 4;
            buAbrir.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buAbrir.ImageOptions.Image");
            buAbrir.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buAbrir.ImageOptions.LargeImage");
            buAbrir.Name = "buAbrir";
            buAbrir.ItemClick += buAbrir_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Administrador de Paquetes";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(buRefrescar);
            ribbonPageGroup1.ItemLinks.Add(buAgregar, true);
            ribbonPageGroup1.ItemLinks.Add(buQuitar);
            ribbonPageGroup1.ItemLinks.Add(buAbrir, true);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Paquetes";
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(splitContainerControl1);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 150);
            panelControl1.Margin = new System.Windows.Forms.Padding(2);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(5);
            panelControl1.Size = new System.Drawing.Size(989, 550);
            panelControl1.TabIndex = 1;
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Location = new System.Drawing.Point(7, 7);
            splitContainerControl1.Margin = new System.Windows.Forms.Padding(2);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(treePackages);
            splitContainerControl1.Panel1.Padding = new System.Windows.Forms.Padding(5);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(groupControl1);
            splitContainerControl1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(975, 536);
            splitContainerControl1.SplitterPosition = 539;
            splitContainerControl1.TabIndex = 1;
            // 
            // treePackages
            // 
            treePackages.Appearance.FocusedCell.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            treePackages.Appearance.FocusedCell.Options.UseFont = true;
            treePackages.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            treePackages.Appearance.Row.Options.UseFont = true;
            treePackages.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { treeListColumn2 });
            treePackages.Dock = System.Windows.Forms.DockStyle.Fill;
            treePackages.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            treePackages.Location = new System.Drawing.Point(5, 5);
            treePackages.Name = "treePackages";
            treePackages.OptionsBehavior.Editable = false;
            treePackages.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
            treePackages.OptionsMenu.EnableColumnMenu = false;
            treePackages.OptionsMenu.EnableFooterMenu = false;
            treePackages.OptionsSelection.InvertSelection = true;
            treePackages.OptionsView.ShowColumns = false;
            treePackages.OptionsView.ShowHorzLines = false;
            treePackages.OptionsView.ShowIndicator = false;
            treePackages.OptionsView.ShowVertLines = false;
            treePackages.SelectImageList = imgTree;
            treePackages.Size = new System.Drawing.Size(529, 526);
            treePackages.TabIndex = 18;
            treePackages.FocusedNodeChanged += treePackages_FocusedNodeChanged;
            // 
            // treeListColumn2
            // 
            treeListColumn2.Caption = "treeListColumn2";
            treeListColumn2.FieldName = "treeListColumn2";
            treeListColumn2.MinWidth = 52;
            treeListColumn2.Name = "treeListColumn2";
            treeListColumn2.Visible = true;
            treeListColumn2.VisibleIndex = 0;
            treeListColumn2.Width = 108;
            // 
            // imgTree
            // 
            imgTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            imgTree.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imgTree.ImageStream");
            imgTree.TransparentColor = System.Drawing.Color.Transparent;
            imgTree.Images.SetKeyName(0, "Flag-Green.png");
            imgTree.Images.SetKeyName(1, "Flag-Yellow.png");
            imgTree.Images.SetKeyName(2, "Flag-Red.png");
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(xtraTabControl1);
            groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupControl1.Location = new System.Drawing.Point(5, 5);
            groupControl1.Margin = new System.Windows.Forms.Padding(2);
            groupControl1.Name = "groupControl1";
            groupControl1.Padding = new System.Windows.Forms.Padding(5);
            groupControl1.Size = new System.Drawing.Size(416, 526);
            groupControl1.TabIndex = 0;
            groupControl1.Text = "Propiedades";
            // 
            // xtraTabControl1
            // 
            xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            xtraTabControl1.Location = new System.Drawing.Point(7, 28);
            xtraTabControl1.Margin = new System.Windows.Forms.Padding(2);
            xtraTabControl1.Name = "xtraTabControl1";
            xtraTabControl1.SelectedTabPage = xtraTabPage1;
            xtraTabControl1.Size = new System.Drawing.Size(402, 491);
            xtraTabControl1.TabIndex = 0;
            xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { xtraTabPage1, xtraTabPage2 });
            // 
            // xtraTabPage1
            // 
            xtraTabPage1.Controls.Add(splitContainerControl2);
            xtraTabPage1.Margin = new System.Windows.Forms.Padding(2);
            xtraTabPage1.Name = "xtraTabPage1";
            xtraTabPage1.Padding = new System.Windows.Forms.Padding(5);
            xtraTabPage1.Size = new System.Drawing.Size(400, 466);
            xtraTabPage1.Text = "Modelos";
            // 
            // splitContainerControl2
            // 
            splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl2.Horizontal = false;
            splitContainerControl2.Location = new System.Drawing.Point(5, 5);
            splitContainerControl2.Margin = new System.Windows.Forms.Padding(2);
            splitContainerControl2.Name = "splitContainerControl2";
            // 
            // splitContainerControl2.Panel1
            // 
            splitContainerControl2.Panel1.Controls.Add(grdModelos);
            splitContainerControl2.Panel1.Padding = new System.Windows.Forms.Padding(5);
            splitContainerControl2.Panel1.Text = "Panel1";
            // 
            // splitContainerControl2.Panel2
            // 
            splitContainerControl2.Panel2.Controls.Add(groupControl2);
            splitContainerControl2.Panel2.Padding = new System.Windows.Forms.Padding(5);
            splitContainerControl2.Panel2.Text = "Panel2";
            splitContainerControl2.Size = new System.Drawing.Size(390, 456);
            splitContainerControl2.SplitterPosition = 281;
            splitContainerControl2.TabIndex = 2;
            // 
            // grdModelos
            // 
            grdModelos.Dock = System.Windows.Forms.DockStyle.Fill;
            grdModelos.Location = new System.Drawing.Point(5, 5);
            grdModelos.MainView = grdModelosView;
            grdModelos.MenuManager = ribbonControl1;
            grdModelos.Name = "grdModelos";
            grdModelos.Size = new System.Drawing.Size(380, 271);
            grdModelos.TabIndex = 1;
            grdModelos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdModelosView });
            // 
            // grdModelosView
            // 
            grdModelosView.GridControl = grdModelos;
            grdModelosView.LevelIndent = 0;
            grdModelosView.Name = "grdModelosView";
            grdModelosView.OptionsBehavior.Editable = false;
            grdModelosView.OptionsEditForm.PopupEditFormWidth = 400;
            grdModelosView.OptionsView.ShowAutoFilterRow = true;
            grdModelosView.PreviewIndent = 0;
            grdModelosView.FocusedRowChanged += grdModelosView_FocusedRowChanged;
            // 
            // groupControl2
            // 
            groupControl2.Controls.Add(woModelInspector1);
            groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupControl2.Location = new System.Drawing.Point(5, 5);
            groupControl2.Margin = new System.Windows.Forms.Padding(2);
            groupControl2.Name = "groupControl2";
            groupControl2.Padding = new System.Windows.Forms.Padding(5);
            groupControl2.Size = new System.Drawing.Size(380, 155);
            groupControl2.TabIndex = 0;
            groupControl2.Text = "Columnas del Modelo";
            // 
            // woModelInspector1
            // 
            woModelInspector1.Dock = System.Windows.Forms.DockStyle.Fill;
            woModelInspector1.Location = new System.Drawing.Point(7, 28);
            woModelInspector1.Margin = new System.Windows.Forms.Padding(1);
            woModelInspector1.Name = "woModelInspector1";
            woModelInspector1.Size = new System.Drawing.Size(366, 120);
            woModelInspector1.TabIndex = 0;
            // 
            // xtraTabPage2
            // 
            xtraTabPage2.Controls.Add(splitContainerControl3);
            xtraTabPage2.Name = "xtraTabPage2";
            xtraTabPage2.Padding = new System.Windows.Forms.Padding(10);
            xtraTabPage2.Size = new System.Drawing.Size(400, 466);
            xtraTabPage2.Text = "Propiedades";
            // 
            // splitContainerControl3
            // 
            splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl3.Horizontal = false;
            splitContainerControl3.Location = new System.Drawing.Point(10, 10);
            splitContainerControl3.Name = "splitContainerControl3";
            // 
            // splitContainerControl3.Panel1
            // 
            splitContainerControl3.Panel1.Controls.Add(propertyGrid);
            splitContainerControl3.Panel1.Padding = new System.Windows.Forms.Padding(5);
            splitContainerControl3.Panel1.Text = "Panel1";
            // 
            // splitContainerControl3.Panel2
            // 
            splitContainerControl3.Panel2.Controls.Add(groupControl3);
            splitContainerControl3.Panel2.Padding = new System.Windows.Forms.Padding(5);
            splitContainerControl3.Panel2.Text = "Panel2";
            splitContainerControl3.Size = new System.Drawing.Size(380, 446);
            splitContainerControl3.SplitterPosition = 239;
            splitContainerControl3.TabIndex = 0;
            // 
            // propertyGrid
            // 
            propertyGrid.ActiveViewType = DevExpress.XtraVerticalGrid.PropertyGridView.Office;
            propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGrid.Location = new System.Drawing.Point(5, 5);
            propertyGrid.MenuManager = ribbonControl1;
            propertyGrid.Name = "propertyGrid";
            propertyGrid.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            propertyGrid.Size = new System.Drawing.Size(370, 229);
            propertyGrid.TabIndex = 8;
            propertyGrid.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.True;
            propertyGrid.EditorKeyPress += propertyGrid_EditorKeyPress;
            // 
            // groupControl3
            // 
            groupControl3.Controls.Add(mstHtmlEditor1);
            groupControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            groupControl3.Location = new System.Drawing.Point(5, 5);
            groupControl3.Margin = new System.Windows.Forms.Padding(2);
            groupControl3.Name = "groupControl3";
            groupControl3.Padding = new System.Windows.Forms.Padding(5);
            groupControl3.Size = new System.Drawing.Size(370, 187);
            groupControl3.TabIndex = 2;
            groupControl3.Text = "Historial de Versiones";
            // 
            // mstHtmlEditor1
            // 
            mstHtmlEditor1.DataSource = null;
            mstHtmlEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            mstHtmlEditor1.EditorMode = BaiqiSoft.HtmlEditorControl.EditorMode.Preview;
            mstHtmlEditor1.Location = new System.Drawing.Point(7, 28);
            mstHtmlEditor1.Name = "mstHtmlEditor1";
            mstHtmlEditor1.ShortcutsEnabled = false;
            mstHtmlEditor1.ShowEditorModeToolbar = false;
            mstHtmlEditor1.ShowFormattingToolbar = false;
            mstHtmlEditor1.ShowStandardToolbar = false;
            mstHtmlEditor1.Size = new System.Drawing.Size(356, 152);
            mstHtmlEditor1.TabIndex = 0;
            // 
            // fmPackageManager
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Controls.Add(ribbonControl1);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "fmPackageManager";
            Size = new System.Drawing.Size(989, 700);
            Load += fmPackageManager_Load;
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)treePackages).EndInit();
            ((System.ComponentModel.ISupportInitialize)groupControl1).EndInit();
            groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).EndInit();
            xtraTabControl1.ResumeLayout(false);
            xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel1).EndInit();
            splitContainerControl2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel2).EndInit();
            splitContainerControl2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2).EndInit();
            splitContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdModelos).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdModelosView).EndInit();
            ((System.ComponentModel.ISupportInitialize)groupControl2).EndInit();
            groupControl2.ResumeLayout(false);
            xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel1).EndInit();
            splitContainerControl3.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel2).EndInit();
            splitContainerControl3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3).EndInit();
            splitContainerControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)propertyGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)groupControl3).EndInit();
            groupControl3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraBars.BarButtonItem buRefrescar;
        private DevExpress.XtraBars.BarButtonItem buAgregar;
        private DevExpress.XtraBars.BarButtonItem buQuitar;
        private DevExpress.XtraTreeList.TreeList treePackages;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private System.Windows.Forms.ImageList imgTree;
        private DevExpress.XtraBars.BarButtonItem buAbrir;
        private DevExpress.XtraGrid.GridControl grdModelos;
        private DevExpress.XtraGrid.Views.Grid.GridView grdModelosView;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private Config.Editors.woModelInspector woModelInspector1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl3;
        private DevExpress.XtraVerticalGrid.PropertyGridControl propertyGrid;
        private DevExpress.XtraEditors.GroupControl groupControl3;
        private BaiqiSoft.HtmlEditorControl.MstHtmlEditor mstHtmlEditor1;
    }
}
