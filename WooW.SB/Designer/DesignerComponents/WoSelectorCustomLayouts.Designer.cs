namespace WooW.SB.Designer.DesignerComponents
{
    partial class WoSelectorCustomLayouts
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
            pnlGrid = new DevExpress.XtraEditors.PanelControl();
            grdFreeLayouts = new DevExpress.XtraGrid.GridControl();
            grdFreeLayoutsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)pnlGrid).BeginInit();
            pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdFreeLayouts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdFreeLayoutsView).BeginInit();
            SuspendLayout();
            // 
            // pnlGrid
            // 
            pnlGrid.Controls.Add(grdFreeLayouts);
            pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlGrid.Location = new System.Drawing.Point(0, 0);
            pnlGrid.Name = "pnlGrid";
            pnlGrid.Size = new System.Drawing.Size(568, 806);
            pnlGrid.TabIndex = 1;
            // 
            // grdFreeLayouts
            // 
            grdFreeLayouts.Dock = System.Windows.Forms.DockStyle.Fill;
            grdFreeLayouts.Location = new System.Drawing.Point(2, 2);
            grdFreeLayouts.MainView = grdFreeLayoutsView;
            grdFreeLayouts.Name = "grdFreeLayouts";
            grdFreeLayouts.Size = new System.Drawing.Size(564, 802);
            grdFreeLayouts.TabIndex = 0;
            grdFreeLayouts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdFreeLayoutsView });
            // 
            // grdFreeLayoutsView
            // 
            grdFreeLayoutsView.GridControl = grdFreeLayouts;
            grdFreeLayoutsView.Name = "grdFreeLayoutsView";
            grdFreeLayoutsView.FocusedRowChanged += grdFreeLayoutsView_FocusedRowChanged;
            // 
            // WoSelectorCustomLayouts
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(pnlGrid);
            Name = "WoSelectorCustomLayouts";
            Size = new System.Drawing.Size(568, 806);
            ((System.ComponentModel.ISupportInitialize)pnlGrid).EndInit();
            pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdFreeLayouts).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdFreeLayoutsView).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraEditors.PanelControl pnlGrid;
        private DevExpress.XtraGrid.GridControl grdFreeLayouts;
        private DevExpress.XtraGrid.Views.Grid.GridView grdFreeLayoutsView;
    }
}
