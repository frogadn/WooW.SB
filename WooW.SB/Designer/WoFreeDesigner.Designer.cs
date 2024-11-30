namespace WooW.SB.Designer
{
    partial class WoFreeDesigner
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
            pnlBaseDesigner = new DevExpress.XtraEditors.PanelControl();
            sccBase = new DevExpress.XtraEditors.SplitContainerControl();
            pnlGridLayouts = new DevExpress.XtraEditors.PanelControl();
            grdMethods = new DevExpress.XtraTab.XtraTabControl();
            tabDesigner = new DevExpress.XtraTab.XtraTabPage();
            tabCode = new DevExpress.XtraTab.XtraTabPage();
            sccSyntaxBase = new DevExpress.XtraEditors.SplitContainerControl();
            pnlSyntaxEditor = new DevExpress.XtraEditors.PanelControl();
            grdMethod = new DevExpress.XtraGrid.GridControl();
            grdViewMethods = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)pnlBaseDesigner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sccBase).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sccBase.Panel1).BeginInit();
            sccBase.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sccBase.Panel2).BeginInit();
            sccBase.Panel2.SuspendLayout();
            sccBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlGridLayouts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdMethods).BeginInit();
            grdMethods.SuspendLayout();
            tabDesigner.SuspendLayout();
            tabCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxBase).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxBase.Panel1).BeginInit();
            sccSyntaxBase.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxBase.Panel2).BeginInit();
            sccSyntaxBase.Panel2.SuspendLayout();
            sccSyntaxBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlSyntaxEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdMethod).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdViewMethods).BeginInit();
            SuspendLayout();
            // 
            // pnlBaseDesigner
            // 
            pnlBaseDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlBaseDesigner.Location = new System.Drawing.Point(0, 0);
            pnlBaseDesigner.Name = "pnlBaseDesigner";
            pnlBaseDesigner.Size = new System.Drawing.Size(1104, 654);
            pnlBaseDesigner.TabIndex = 0;
            // 
            // sccBase
            // 
            sccBase.Dock = System.Windows.Forms.DockStyle.Fill;
            sccBase.Location = new System.Drawing.Point(0, 0);
            sccBase.Name = "sccBase";
            // 
            // sccBase.Panel1
            // 
            sccBase.Panel1.Controls.Add(pnlGridLayouts);
            sccBase.Panel1.Text = "Panel1";
            // 
            // sccBase.Panel2
            // 
            sccBase.Panel2.Controls.Add(grdMethods);
            sccBase.Panel2.Text = "Panel2";
            sccBase.Size = new System.Drawing.Size(1433, 679);
            sccBase.SplitterPosition = 317;
            sccBase.TabIndex = 1;
            // 
            // pnlGridLayouts
            // 
            pnlGridLayouts.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlGridLayouts.Location = new System.Drawing.Point(0, 0);
            pnlGridLayouts.Name = "pnlGridLayouts";
            pnlGridLayouts.Size = new System.Drawing.Size(317, 679);
            pnlGridLayouts.TabIndex = 0;
            // 
            // grdMethods
            // 
            grdMethods.Dock = System.Windows.Forms.DockStyle.Fill;
            grdMethods.Location = new System.Drawing.Point(0, 0);
            grdMethods.Name = "grdMethods";
            grdMethods.SelectedTabPage = tabDesigner;
            grdMethods.Size = new System.Drawing.Size(1106, 679);
            grdMethods.TabIndex = 1;
            grdMethods.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { tabDesigner, tabCode });
            // 
            // tabDesigner
            // 
            tabDesigner.Controls.Add(pnlBaseDesigner);
            tabDesigner.Name = "tabDesigner";
            tabDesigner.Size = new System.Drawing.Size(1104, 654);
            tabDesigner.Text = "Diseñador";
            // 
            // tabCode
            // 
            tabCode.Controls.Add(sccSyntaxBase);
            tabCode.Name = "tabCode";
            tabCode.Size = new System.Drawing.Size(1104, 654);
            tabCode.Text = "Codigo";
            // 
            // sccSyntaxBase
            // 
            sccSyntaxBase.Dock = System.Windows.Forms.DockStyle.Fill;
            sccSyntaxBase.Location = new System.Drawing.Point(0, 0);
            sccSyntaxBase.Name = "sccSyntaxBase";
            // 
            // sccSyntaxBase.Panel1
            // 
            sccSyntaxBase.Panel1.Controls.Add(pnlSyntaxEditor);
            sccSyntaxBase.Panel1.Text = "Panel1";
            // 
            // sccSyntaxBase.Panel2
            // 
            sccSyntaxBase.Panel2.Controls.Add(grdMethod);
            sccSyntaxBase.Panel2.Text = "Panel2";
            sccSyntaxBase.Size = new System.Drawing.Size(1104, 654);
            sccSyntaxBase.SplitterPosition = 751;
            sccSyntaxBase.TabIndex = 0;
            // 
            // pnlSyntaxEditor
            // 
            pnlSyntaxEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlSyntaxEditor.Location = new System.Drawing.Point(0, 0);
            pnlSyntaxEditor.Name = "pnlSyntaxEditor";
            pnlSyntaxEditor.Size = new System.Drawing.Size(751, 654);
            pnlSyntaxEditor.TabIndex = 0;
            // 
            // grdMethod
            // 
            grdMethod.Dock = System.Windows.Forms.DockStyle.Fill;
            grdMethod.Location = new System.Drawing.Point(0, 0);
            grdMethod.MainView = grdViewMethods;
            grdMethod.Name = "grdMethod";
            grdMethod.Size = new System.Drawing.Size(343, 654);
            grdMethod.TabIndex = 0;
            grdMethod.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdViewMethods });
            // 
            // grdViewMethods
            // 
            grdViewMethods.GridControl = grdMethod;
            grdViewMethods.Name = "grdViewMethods";
            grdViewMethods.FocusedRowChanged += grdViewMethods_FocusedRowChanged;
            // 
            // WoFreeDesigner
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(sccBase);
            Name = "WoFreeDesigner";
            Size = new System.Drawing.Size(1433, 679);
            ((System.ComponentModel.ISupportInitialize)pnlBaseDesigner).EndInit();
            ((System.ComponentModel.ISupportInitialize)sccBase.Panel1).EndInit();
            sccBase.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)sccBase.Panel2).EndInit();
            sccBase.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)sccBase).EndInit();
            sccBase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlGridLayouts).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdMethods).EndInit();
            grdMethods.ResumeLayout(false);
            tabDesigner.ResumeLayout(false);
            tabCode.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)sccSyntaxBase.Panel1).EndInit();
            sccSyntaxBase.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)sccSyntaxBase.Panel2).EndInit();
            sccSyntaxBase.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)sccSyntaxBase).EndInit();
            sccSyntaxBase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlSyntaxEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdMethod).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdViewMethods).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlBaseDesigner;
        private DevExpress.XtraEditors.SplitContainerControl sccBase;
        private DevExpress.XtraEditors.PanelControl pnlGridLayouts;
        private DevExpress.XtraTab.XtraTabControl grdMethods;
        private DevExpress.XtraTab.XtraTabPage tabDesigner;
        private DevExpress.XtraTab.XtraTabPage tabCode;
        private DevExpress.XtraEditors.SplitContainerControl sccSyntaxBase;
        private DevExpress.XtraEditors.PanelControl pnlSyntaxEditor;
        private DevExpress.XtraGrid.GridControl grdMethod;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewMethods;
    }
}
