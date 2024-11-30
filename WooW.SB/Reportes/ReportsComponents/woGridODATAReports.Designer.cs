using WooW.SB.Config;

namespace WooW.SB.Reportes.ReportsComponents
{
    partial class woGridODATAReports
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
            grdModels = new DevExpress.XtraGrid.GridControl();
            grdModelsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            tabExplorador = new DevExpress.XtraTab.XtraTabControl();
            tabOData = new DevExpress.XtraTab.XtraTabPage();
            grdOdata = new DevExpress.XtraGrid.GridControl();
            grdOdataView = new DevExpress.XtraGrid.Views.Grid.GridView();
            tabODataPrevio = new DevExpress.XtraTab.XtraTabPage();
            documentViewerOdata = new DevExpress.XtraPrinting.Preview.DocumentViewer();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdModels).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdModelsView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabExplorador).BeginInit();
            tabExplorador.SuspendLayout();
            tabOData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdOdata).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdOdataView).BeginInit();
            tabODataPrevio.SuspendLayout();
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
            splitContainerControl1.Panel1.Controls.Add(grdModels);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(tabExplorador);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(1031, 273);
            splitContainerControl1.SplitterPosition = 399;
            splitContainerControl1.TabIndex = 1;
            // 
            // grdModels
            // 
            grdModels.Dock = System.Windows.Forms.DockStyle.Fill;
            grdModels.Location = new System.Drawing.Point(0, 0);
            grdModels.MainView = grdModelsView;
            grdModels.Name = "grdModels";
            grdModels.Size = new System.Drawing.Size(399, 273);
            grdModels.TabIndex = 1;
            grdModels.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdModelsView });
            grdModels.Load += grdModels_Load;
            // 
            // grdModelsView
            // 
            grdModelsView.GridControl = grdModels;
            grdModelsView.LevelIndent = 0;
            grdModelsView.Name = "grdModelsView";
            grdModelsView.OptionsBehavior.Editable = false;
            grdModelsView.OptionsView.ShowGroupPanel = false;
            grdModelsView.PreviewIndent = 0;
            grdModelsView.FocusedRowChanged += grdModelsView_FocusedRowChanged;
            // 
            // tabExplorador
            // 
            tabExplorador.Dock = System.Windows.Forms.DockStyle.Fill;
            tabExplorador.Location = new System.Drawing.Point(0, 0);
            tabExplorador.Name = "tabExplorador";
            tabExplorador.Padding = new System.Windows.Forms.Padding(10);
            tabExplorador.SelectedTabPage = tabOData;
            tabExplorador.Size = new System.Drawing.Size(622, 273);
            tabExplorador.TabIndex = 3;
            tabExplorador.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { tabOData, tabODataPrevio });
            // 
            // tabOData
            // 
            tabOData.Controls.Add(grdOdata);
            tabOData.Name = "tabOData";
            tabOData.Padding = new System.Windows.Forms.Padding(5);
            tabOData.Size = new System.Drawing.Size(620, 248);
            tabOData.Text = "OData";
            // 
            // grdOdata
            // 
            grdOdata.Dock = System.Windows.Forms.DockStyle.Fill;
            grdOdata.Location = new System.Drawing.Point(5, 5);
            grdOdata.MainView = grdOdataView;
            grdOdata.Name = "grdOdata";
            grdOdata.Size = new System.Drawing.Size(610, 238);
            grdOdata.TabIndex = 0;
            grdOdata.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdOdataView });
            // 
            // grdOdataView
            // 
            grdOdataView.GridControl = grdOdata;
            grdOdataView.LevelIndent = 0;
            grdOdataView.Name = "grdOdataView";
            grdOdataView.OptionsBehavior.Editable = false;
            grdOdataView.OptionsView.ColumnAutoWidth = false;
            grdOdataView.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
            grdOdataView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.ShowAlways;
            grdOdataView.OptionsView.ShowGroupPanel = false;
            grdOdataView.PreviewIndent = 0;
            // 
            // tabODataPrevio
            // 
            tabODataPrevio.Controls.Add(documentViewerOdata);
            tabODataPrevio.Name = "tabODataPrevio";
            tabODataPrevio.Size = new System.Drawing.Size(730, 248);
            tabODataPrevio.Text = "Previo";
            // 
            // documentViewerOdata
            // 
            documentViewerOdata.Dock = System.Windows.Forms.DockStyle.Fill;
            documentViewerOdata.IsMetric = true;
            documentViewerOdata.Location = new System.Drawing.Point(0, 0);
            documentViewerOdata.Margin = new System.Windows.Forms.Padding(6);
            documentViewerOdata.Name = "documentViewerOdata";
            documentViewerOdata.Size = new System.Drawing.Size(730, 248);
            documentViewerOdata.TabIndex = 0;
            // 
            // woGridODATAReports
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainerControl1);
            Name = "woGridODATAReports";
            Size = new System.Drawing.Size(1031, 273);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdModels).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdModelsView).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabExplorador).EndInit();
            tabExplorador.ResumeLayout(false);
            tabOData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdOdata).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdOdataView).EndInit();
            tabODataPrevio.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.Data.VirtualServerModeSource virtualServerModeSource1;
        private DevExpress.XtraGrid.GridControl grdModels;
        public DevExpress.XtraGrid.Views.Grid.GridView grdModelsView;
        public DevExpress.XtraGrid.GridControl grdOdata;
        public DevExpress.XtraGrid.Views.Grid.GridView grdOdataView;
        public DevExpress.XtraTab.XtraTabControl tabExplorador;
        public DevExpress.XtraTab.XtraTabPage tabOData;
        public DevExpress.XtraTab.XtraTabPage tabODataPrevio;
        public DevExpress.XtraPrinting.Preview.DocumentViewer documentViewerOdata;
    }
}
