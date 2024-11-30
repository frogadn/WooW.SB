namespace WooW.SB.Dialogs
{
    partial class fmSummaryDTO
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.grdDTO = new DevExpress.XtraGrid.GridControl();
            this.grdDTOView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDTO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDTOView)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grdDTO);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl1.Size = new System.Drawing.Size(1073, 529);
            this.panelControl1.TabIndex = 0;
            // 
            // grdDTO
            // 
            this.grdDTO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdDTO.Location = new System.Drawing.Point(12, 12);
            this.grdDTO.MainView = this.grdDTOView;
            this.grdDTO.Name = "grdDTO";
            this.grdDTO.Size = new System.Drawing.Size(1049, 505);
            this.grdDTO.TabIndex = 0;
            this.grdDTO.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdDTOView});
            // 
            // grdDTOView
            // 
            this.grdDTOView.GridControl = this.grdDTO;
            this.grdDTOView.Name = "grdDTOView";
            this.grdDTOView.OptionsBehavior.Editable = false;
            this.grdDTOView.OptionsView.ShowGroupPanel = false;
            // 
            // fmMostrarDTO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 529);
            this.Controls.Add(this.panelControl1);
            this.MinimizeBox = false;
            this.Name = "fmMostrarDTO";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DTO";
            this.Load += new System.EventHandler(this.fmMostrarDTO_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDTO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDTOView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl grdDTO;
        private DevExpress.XtraGrid.Views.Grid.GridView grdDTOView;
    }
}