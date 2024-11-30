namespace WooW.SB.Config.Editors
{
    partial class woGridModel
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
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            treeModelos = new DevExpress.XtraTreeList.TreeList();
            treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            searchControl2 = new DevExpress.XtraEditors.SearchControl();
            buAutoFiltro = new DevExpress.XtraEditors.CheckButton();
            tabExplorador = new DevExpress.XtraTab.XtraTabControl();
            tabColumnas = new DevExpress.XtraTab.XtraTabPage();
            grdColumna = new DevExpress.XtraGrid.GridControl();
            grdColumnaView = new DevExpress.XtraGrid.Views.Grid.GridView();
            tabDiagrama = new DevExpress.XtraTab.XtraTabPage();
            woDiagram1 = new woDiagram();
            tabPreCondiciones = new DevExpress.XtraTab.XtraTabPage();
            txtScriptPre = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            navigableSymbolSelector1 = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector();
            tabPostCodiciones = new DevExpress.XtraTab.XtraTabPage();
            txtScriptPost = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            navigableSymbolSelector2 = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector();
            tabOData = new DevExpress.XtraTab.XtraTabPage();
            grdOData = new DevExpress.XtraGrid.GridControl();
            grdODataView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treeModelos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)searchControl2.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabExplorador).BeginInit();
            tabExplorador.SuspendLayout();
            tabColumnas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdColumna).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdColumnaView).BeginInit();
            tabDiagrama.SuspendLayout();
            tabPreCondiciones.SuspendLayout();
            tabPostCodiciones.SuspendLayout();
            tabOData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdOData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdODataView).BeginInit();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(treeModelos);
            splitContainerControl1.Panel1.Controls.Add(panelControl1);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(tabExplorador);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(1031, 273);
            splitContainerControl1.SplitterPosition = 289;
            splitContainerControl1.TabIndex = 1;
            // 
            // treeModelos
            // 
            treeModelos.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { treeListColumn1 });
            treeModelos.Dock = System.Windows.Forms.DockStyle.Fill;
            treeModelos.Location = new System.Drawing.Point(0, 26);
            treeModelos.Name = "treeModelos";
            treeModelos.OptionsBehavior.Editable = false;
            treeModelos.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = true;
            treeModelos.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Matches;
            treeModelos.OptionsFind.AllowIncrementalSearch = true;
            treeModelos.OptionsMenu.EnableColumnMenu = false;
            treeModelos.OptionsMenu.EnableFooterMenu = false;
            treeModelos.OptionsSelection.InvertSelection = true;
            treeModelos.OptionsView.ShowColumns = false;
            treeModelos.OptionsView.ShowHorzLines = false;
            treeModelos.OptionsView.ShowIndicator = false;
            treeModelos.OptionsView.ShowVertLines = false;
            treeModelos.Size = new System.Drawing.Size(289, 247);
            treeModelos.TabIndex = 20;
            treeModelos.FocusedNodeChanged += treeModelos_FocusedNodeChanged;
            treeModelos.PopupMenuShowing += treeModelos_PopupMenuShowing;
            treeModelos.DoubleClick += treeModelos_DoubleClick;
            // 
            // treeListColumn1
            // 
            treeListColumn1.Caption = "treeListColumn2";
            treeListColumn1.FieldName = "treeListColumn2";
            treeListColumn1.MinWidth = 52;
            treeListColumn1.Name = "treeListColumn1";
            treeListColumn1.OptionsFilter.AutoFilterCondition = DevExpress.XtraTreeList.Columns.AutoFilterCondition.Greater;
            treeListColumn1.Visible = true;
            treeListColumn1.VisibleIndex = 0;
            treeListColumn1.Width = 108;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(searchControl2);
            panelControl1.Controls.Add(buAutoFiltro);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(1, 1, 1, 1);
            panelControl1.Size = new System.Drawing.Size(289, 26);
            panelControl1.TabIndex = 21;
            // 
            // searchControl2
            // 
            searchControl2.Client = treeModelos;
            searchControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            searchControl2.Location = new System.Drawing.Point(3, 3);
            searchControl2.Name = "searchControl2";
            searchControl2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Repository.ClearButton(), new DevExpress.XtraEditors.Repository.SearchButton() });
            searchControl2.Properties.Client = treeModelos;
            searchControl2.Properties.FindDelay = 500;
            searchControl2.Size = new System.Drawing.Size(222, 20);
            searchControl2.TabIndex = 18;
            // 
            // buAutoFiltro
            // 
            buAutoFiltro.Checked = true;
            buAutoFiltro.Dock = System.Windows.Forms.DockStyle.Right;
            buAutoFiltro.Location = new System.Drawing.Point(225, 3);
            buAutoFiltro.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            buAutoFiltro.Name = "buAutoFiltro";
            buAutoFiltro.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            buAutoFiltro.Size = new System.Drawing.Size(61, 20);
            buAutoFiltro.TabIndex = 19;
            buAutoFiltro.Text = "Auto Filtro";
            // 
            // tabExplorador
            // 
            tabExplorador.Dock = System.Windows.Forms.DockStyle.Fill;
            tabExplorador.Location = new System.Drawing.Point(0, 0);
            tabExplorador.Name = "tabExplorador";
            tabExplorador.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            tabExplorador.SelectedTabPage = tabColumnas;
            tabExplorador.Size = new System.Drawing.Size(732, 273);
            tabExplorador.TabIndex = 3;
            tabExplorador.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { tabColumnas, tabDiagrama, tabPreCondiciones, tabPostCodiciones, tabOData });
            tabExplorador.SelectedPageChanged += tabExplorador_SelectedPageChanged;
            // 
            // tabColumnas
            // 
            tabColumnas.Controls.Add(grdColumna);
            tabColumnas.Name = "tabColumnas";
            tabColumnas.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            tabColumnas.Size = new System.Drawing.Size(730, 248);
            tabColumnas.Text = "Columnas";
            // 
            // grdColumna
            // 
            grdColumna.Dock = System.Windows.Forms.DockStyle.Fill;
            grdColumna.Location = new System.Drawing.Point(10, 10);
            grdColumna.MainView = grdColumnaView;
            grdColumna.Name = "grdColumna";
            grdColumna.Size = new System.Drawing.Size(710, 228);
            grdColumna.TabIndex = 2;
            grdColumna.UseEmbeddedNavigator = true;
            grdColumna.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdColumnaView });
            // 
            // grdColumnaView
            // 
            grdColumnaView.GridControl = grdColumna;
            grdColumnaView.LevelIndent = 0;
            grdColumnaView.Name = "grdColumnaView";
            grdColumnaView.OptionsBehavior.Editable = false;
            grdColumnaView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.Inplace;
            grdColumnaView.OptionsLayout.Columns.AddNewColumns = false;
            grdColumnaView.OptionsLayout.Columns.RemoveOldColumns = false;
            grdColumnaView.OptionsLayout.Columns.StoreLayout = false;
            grdColumnaView.OptionsLayout.StoreVisualOptions = false;
            grdColumnaView.OptionsView.BestFitMaxRowCount = 50;
            grdColumnaView.OptionsView.EnableAppearanceOddRow = true;
            grdColumnaView.OptionsView.ShowAutoFilterRow = true;
            grdColumnaView.OptionsView.ShowGroupPanel = false;
            grdColumnaView.PreviewIndent = 0;
            grdColumnaView.ShowingEditor += grdColumnaView_ShowingEditor;
            grdColumnaView.DoubleClick += grdColumnaView_DoubleClick;
            // 
            // tabDiagrama
            // 
            tabDiagrama.Controls.Add(woDiagram1);
            tabDiagrama.Name = "tabDiagrama";
            tabDiagrama.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            tabDiagrama.Size = new System.Drawing.Size(730, 248);
            tabDiagrama.Text = "Diagrama";
            // 
            // woDiagram1
            // 
            woDiagram1.currDiagrama = null;
            woDiagram1.currModelo = null;
            woDiagram1.Dock = System.Windows.Forms.DockStyle.Fill;
            woDiagram1.Enabled = false;
            woDiagram1.Location = new System.Drawing.Point(10, 10);
            woDiagram1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            woDiagram1.MostrarPermisos = false;
            woDiagram1.Name = "woDiagram1";
            woDiagram1.proyecto = null;
            woDiagram1.Size = new System.Drawing.Size(710, 228);
            woDiagram1.TabIndex = 0;
            // 
            // tabPreCondiciones
            // 
            tabPreCondiciones.Controls.Add(txtScriptPre);
            tabPreCondiciones.Controls.Add(navigableSymbolSelector1);
            tabPreCondiciones.Name = "tabPreCondiciones";
            tabPreCondiciones.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            tabPreCondiciones.Size = new System.Drawing.Size(735, 248);
            tabPreCondiciones.Text = "PreCondiciones";
            // 
            // txtScriptPre
            // 
            txtScriptPre.AllowDrop = true;
            txtScriptPre.Dock = System.Windows.Forms.DockStyle.Fill;
            txtScriptPre.Document.IsReadOnly = true;
            txtScriptPre.Font = new System.Drawing.Font("Courier New", 11.25F);
            txtScriptPre.IsCurrentLineHighlightingEnabled = true;
            txtScriptPre.IsLineNumberMarginVisible = true;
            txtScriptPre.Location = new System.Drawing.Point(10, 35);
            txtScriptPre.Name = "txtScriptPre";
            txtScriptPre.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            txtScriptPre.Size = new System.Drawing.Size(715, 203);
            txtScriptPre.TabIndex = 1;
            // 
            // navigableSymbolSelector1
            // 
            navigableSymbolSelector1.BackColor = System.Drawing.SystemColors.Control;
            navigableSymbolSelector1.Dock = System.Windows.Forms.DockStyle.Top;
            navigableSymbolSelector1.Location = new System.Drawing.Point(10, 10);
            navigableSymbolSelector1.Name = "navigableSymbolSelector1";
            navigableSymbolSelector1.Size = new System.Drawing.Size(715, 25);
            navigableSymbolSelector1.SyntaxEditor = txtScriptPre;
            navigableSymbolSelector1.TabIndex = 2;
            navigableSymbolSelector1.Text = "navigableSymbolSelector1";
            // 
            // tabPostCodiciones
            // 
            tabPostCodiciones.Controls.Add(txtScriptPost);
            tabPostCodiciones.Controls.Add(navigableSymbolSelector2);
            tabPostCodiciones.Name = "tabPostCodiciones";
            tabPostCodiciones.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            tabPostCodiciones.Size = new System.Drawing.Size(735, 248);
            tabPostCodiciones.Text = "PostCondiciones";
            // 
            // txtScriptPost
            // 
            txtScriptPost.AllowDrop = true;
            txtScriptPost.Dock = System.Windows.Forms.DockStyle.Fill;
            txtScriptPost.Document.IsReadOnly = true;
            txtScriptPost.Font = new System.Drawing.Font("Courier New", 11.25F);
            txtScriptPost.IsCurrentLineHighlightingEnabled = true;
            txtScriptPost.IsLineNumberMarginVisible = true;
            txtScriptPost.Location = new System.Drawing.Point(10, 35);
            txtScriptPost.Name = "txtScriptPost";
            txtScriptPost.Size = new System.Drawing.Size(715, 203);
            txtScriptPost.TabIndex = 2;
            // 
            // navigableSymbolSelector2
            // 
            navigableSymbolSelector2.BackColor = System.Drawing.SystemColors.Control;
            navigableSymbolSelector2.Dock = System.Windows.Forms.DockStyle.Top;
            navigableSymbolSelector2.Location = new System.Drawing.Point(10, 10);
            navigableSymbolSelector2.Name = "navigableSymbolSelector2";
            navigableSymbolSelector2.Size = new System.Drawing.Size(715, 25);
            navigableSymbolSelector2.SyntaxEditor = txtScriptPost;
            navigableSymbolSelector2.TabIndex = 3;
            navigableSymbolSelector2.Text = "navigableSymbolSelector2";
            // 
            // tabOData
            // 
            tabOData.Controls.Add(grdOData);
            tabOData.Name = "tabOData";
            tabOData.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            tabOData.Size = new System.Drawing.Size(735, 248);
            tabOData.Text = "OData";
            // 
            // grdOData
            // 
            grdOData.Dock = System.Windows.Forms.DockStyle.Fill;
            grdOData.Location = new System.Drawing.Point(5, 5);
            grdOData.MainView = grdODataView;
            grdOData.Name = "grdOData";
            grdOData.Size = new System.Drawing.Size(725, 238);
            grdOData.TabIndex = 3;
            grdOData.UseEmbeddedNavigator = true;
            grdOData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdODataView });
            // 
            // grdODataView
            // 
            grdODataView.GridControl = grdOData;
            grdODataView.LevelIndent = 0;
            grdODataView.Name = "grdODataView";
            grdODataView.OptionsBehavior.Editable = false;
            grdODataView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.Inplace;
            grdODataView.OptionsView.BestFitMaxRowCount = 50;
            grdODataView.OptionsView.ColumnAutoWidth = false;
            grdODataView.OptionsView.EnableAppearanceOddRow = true;
            grdODataView.OptionsView.ShowAutoFilterRow = true;
            grdODataView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.ShowAlways;
            grdODataView.OptionsView.ShowGroupPanel = false;
            grdODataView.PreviewIndent = 0;
            grdODataView.PopupMenuShowing += grdODataView_PopupMenuShowing;
            grdODataView.DoubleClick += grdODataView_DoubleClick;
            // 
            // woGridModel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainerControl1);
            Name = "woGridModel";
            Size = new System.Drawing.Size(1031, 273);
            Load += woGridModelo_Load;
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)treeModelos).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)searchControl2.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabExplorador).EndInit();
            tabExplorador.ResumeLayout(false);
            tabColumnas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdColumna).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdColumnaView).EndInit();
            tabDiagrama.ResumeLayout(false);
            tabPreCondiciones.ResumeLayout(false);
            tabPostCodiciones.ResumeLayout(false);
            tabOData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdOData).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdODataView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.SearchControl searchControl2;
        private DevExpress.XtraTab.XtraTabControl tabExplorador;
        private DevExpress.XtraTab.XtraTabPage tabColumnas;
        private DevExpress.XtraGrid.GridControl grdColumna;
        private DevExpress.XtraGrid.Views.Grid.GridView grdColumnaView;
        private DevExpress.XtraTab.XtraTabPage tabDiagrama;
        private woDiagram woDiagram1;
        private DevExpress.XtraTab.XtraTabPage tabPreCondiciones;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtScriptPre;
        private DevExpress.XtraTab.XtraTabPage tabPostCodiciones;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtScriptPost;
        private DevExpress.XtraTreeList.TreeList treeModelos;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTab.XtraTabPage tabOData;
        private DevExpress.XtraGrid.GridControl grdOData;
        private DevExpress.XtraGrid.Views.Grid.GridView grdODataView;
        private DevExpress.Data.VirtualServerModeSource virtualServerModeSource1;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector navigableSymbolSelector1;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector navigableSymbolSelector2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckButton buAutoFiltro;
    }
}
