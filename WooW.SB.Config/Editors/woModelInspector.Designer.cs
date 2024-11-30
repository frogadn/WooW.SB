namespace WooW.SB.Config.Editors
{
    partial class woModelInspector
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
            // tabExplorador
            // 
            tabExplorador.Dock = System.Windows.Forms.DockStyle.Fill;
            tabExplorador.Location = new System.Drawing.Point(0, 0);
            tabExplorador.Margin = new System.Windows.Forms.Padding(6);
            tabExplorador.Name = "tabExplorador";
            tabExplorador.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            tabExplorador.SelectedTabPage = tabColumnas;
            tabExplorador.Size = new System.Drawing.Size(1449, 820);
            tabExplorador.TabIndex = 4;
            tabExplorador.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { tabColumnas, tabDiagrama, tabPreCondiciones, tabPostCodiciones, tabOData });
            // 
            // tabColumnas
            // 
            tabColumnas.Controls.Add(grdColumna);
            tabColumnas.Margin = new System.Windows.Forms.Padding(6);
            tabColumnas.Name = "tabColumnas";
            tabColumnas.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            tabColumnas.Size = new System.Drawing.Size(1445, 771);
            tabColumnas.Text = "Columnas";
            // 
            // grdColumna
            // 
            grdColumna.Dock = System.Windows.Forms.DockStyle.Fill;
            grdColumna.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            grdColumna.Location = new System.Drawing.Point(20, 19);
            grdColumna.MainView = grdColumnaView;
            grdColumna.Margin = new System.Windows.Forms.Padding(6);
            grdColumna.Name = "grdColumna";
            grdColumna.Size = new System.Drawing.Size(1405, 733);
            grdColumna.TabIndex = 2;
            grdColumna.UseEmbeddedNavigator = true;
            grdColumna.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdColumnaView });
            // 
            // grdColumnaView
            // 
            grdColumnaView.DetailHeight = 673;
            grdColumnaView.GridControl = grdColumna;
            grdColumnaView.Name = "grdColumnaView";
            grdColumnaView.OptionsBehavior.Editable = false;
            grdColumnaView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.Inplace;
            grdColumnaView.OptionsEditForm.PopupEditFormWidth = 1600;
            grdColumnaView.OptionsLayout.Columns.AddNewColumns = false;
            grdColumnaView.OptionsLayout.Columns.RemoveOldColumns = false;
            grdColumnaView.OptionsLayout.Columns.StoreLayout = false;
            grdColumnaView.OptionsLayout.StoreVisualOptions = false;
            grdColumnaView.OptionsView.BestFitMaxRowCount = 50;
            grdColumnaView.OptionsView.EnableAppearanceOddRow = true;
            grdColumnaView.OptionsView.ShowAutoFilterRow = true;
            grdColumnaView.OptionsView.ShowGroupPanel = false;
            // 
            // tabDiagrama
            // 
            tabDiagrama.Controls.Add(woDiagram1);
            tabDiagrama.Margin = new System.Windows.Forms.Padding(6);
            tabDiagrama.Name = "tabDiagrama";
            tabDiagrama.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            tabDiagrama.Size = new System.Drawing.Size(1460, 476);
            tabDiagrama.Text = "Diagrama";
            // 
            // woDiagram1
            // 
            woDiagram1.currDiagrama = null;
            woDiagram1.currModelo = null;
            woDiagram1.Dock = System.Windows.Forms.DockStyle.Fill;
            woDiagram1.Enabled = false;
            woDiagram1.Location = new System.Drawing.Point(20, 19);
            woDiagram1.Margin = new System.Windows.Forms.Padding(12);
            woDiagram1.MostrarPermisos = false;
            woDiagram1.Name = "woDiagram1";
            woDiagram1.Size = new System.Drawing.Size(1420, 438);
            woDiagram1.TabIndex = 0;
            // 
            // tabPreCondiciones
            // 
            tabPreCondiciones.Controls.Add(txtScriptPre);
            tabPreCondiciones.Controls.Add(navigableSymbolSelector1);
            tabPreCondiciones.Margin = new System.Windows.Forms.Padding(6);
            tabPreCondiciones.Name = "tabPreCondiciones";
            tabPreCondiciones.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            tabPreCondiciones.Size = new System.Drawing.Size(1460, 476);
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
            txtScriptPre.Location = new System.Drawing.Point(20, 52);
            txtScriptPre.Margin = new System.Windows.Forms.Padding(6);
            txtScriptPre.Name = "txtScriptPre";
            txtScriptPre.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
            txtScriptPre.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            txtScriptPre.Size = new System.Drawing.Size(1420, 405);
            txtScriptPre.TabIndex = 1;
            // 
            // navigableSymbolSelector1
            // 
            navigableSymbolSelector1.BackColor = System.Drawing.SystemColors.Control;
            navigableSymbolSelector1.Dock = System.Windows.Forms.DockStyle.Top;
            navigableSymbolSelector1.Location = new System.Drawing.Point(20, 19);
            navigableSymbolSelector1.Margin = new System.Windows.Forms.Padding(6);
            navigableSymbolSelector1.Name = "navigableSymbolSelector1";
            navigableSymbolSelector1.Size = new System.Drawing.Size(1420, 33);
            navigableSymbolSelector1.SyntaxEditor = txtScriptPre;
            navigableSymbolSelector1.TabIndex = 2;
            navigableSymbolSelector1.Text = "navigableSymbolSelector1";
            // 
            // tabPostCodiciones
            // 
            tabPostCodiciones.Controls.Add(txtScriptPost);
            tabPostCodiciones.Controls.Add(navigableSymbolSelector2);
            tabPostCodiciones.Margin = new System.Windows.Forms.Padding(6);
            tabPostCodiciones.Name = "tabPostCodiciones";
            tabPostCodiciones.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            tabPostCodiciones.Size = new System.Drawing.Size(1460, 476);
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
            txtScriptPost.Location = new System.Drawing.Point(20, 52);
            txtScriptPost.Margin = new System.Windows.Forms.Padding(6);
            txtScriptPost.Name = "txtScriptPost";
            txtScriptPost.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
            txtScriptPost.Size = new System.Drawing.Size(1420, 405);
            txtScriptPost.TabIndex = 2;
            // 
            // navigableSymbolSelector2
            // 
            navigableSymbolSelector2.BackColor = System.Drawing.SystemColors.Control;
            navigableSymbolSelector2.Dock = System.Windows.Forms.DockStyle.Top;
            navigableSymbolSelector2.Location = new System.Drawing.Point(20, 19);
            navigableSymbolSelector2.Margin = new System.Windows.Forms.Padding(6);
            navigableSymbolSelector2.Name = "navigableSymbolSelector2";
            navigableSymbolSelector2.Size = new System.Drawing.Size(1420, 33);
            navigableSymbolSelector2.SyntaxEditor = txtScriptPost;
            navigableSymbolSelector2.TabIndex = 3;
            navigableSymbolSelector2.Text = "navigableSymbolSelector2";
            // 
            // tabOData
            // 
            tabOData.Controls.Add(grdOData);
            tabOData.Margin = new System.Windows.Forms.Padding(6);
            tabOData.Name = "tabOData";
            tabOData.Padding = new System.Windows.Forms.Padding(10);
            tabOData.Size = new System.Drawing.Size(1460, 476);
            tabOData.Text = "OData";
            // 
            // grdOData
            // 
            grdOData.Dock = System.Windows.Forms.DockStyle.Fill;
            grdOData.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            grdOData.Location = new System.Drawing.Point(10, 10);
            grdOData.MainView = grdODataView;
            grdOData.Margin = new System.Windows.Forms.Padding(6);
            grdOData.Name = "grdOData";
            grdOData.Size = new System.Drawing.Size(1440, 456);
            grdOData.TabIndex = 3;
            grdOData.UseEmbeddedNavigator = true;
            grdOData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdODataView });
            // 
            // grdODataView
            // 
            grdODataView.DetailHeight = 673;
            grdODataView.GridControl = grdOData;
            grdODataView.Name = "grdODataView";
            grdODataView.OptionsBehavior.Editable = false;
            grdODataView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.Inplace;
            grdODataView.OptionsEditForm.PopupEditFormWidth = 1600;
            grdODataView.OptionsView.BestFitMaxRowCount = 50;
            grdODataView.OptionsView.ColumnAutoWidth = false;
            grdODataView.OptionsView.EnableAppearanceOddRow = true;
            grdODataView.OptionsView.ShowAutoFilterRow = true;
            grdODataView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.ShowAlways;
            grdODataView.OptionsView.ShowGroupPanel = false;
            // 
            // woModelInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tabExplorador);
            Name = "woModelInspector";
            Size = new System.Drawing.Size(1449, 820);
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

        private DevExpress.XtraTab.XtraTabControl tabExplorador;
        private DevExpress.XtraTab.XtraTabPage tabColumnas;
        private DevExpress.XtraGrid.GridControl grdColumna;
        private DevExpress.XtraGrid.Views.Grid.GridView grdColumnaView;
        private DevExpress.XtraTab.XtraTabPage tabDiagrama;
        private woDiagram woDiagram1;
        private DevExpress.XtraTab.XtraTabPage tabPreCondiciones;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtScriptPre;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector navigableSymbolSelector1;
        private DevExpress.XtraTab.XtraTabPage tabPostCodiciones;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtScriptPost;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector navigableSymbolSelector2;
        private DevExpress.XtraTab.XtraTabPage tabOData;
        private DevExpress.XtraGrid.GridControl grdOData;
        private DevExpress.XtraGrid.Views.Grid.GridView grdODataView;
    }
}
