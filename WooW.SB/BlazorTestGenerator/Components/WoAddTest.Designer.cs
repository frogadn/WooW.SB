namespace WooW.SB.BlazorTestGenerator.Components
{
    partial class WoAddTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WoAddTest));
            pnlMain = new DevExpress.XtraEditors.PanelControl();
            grdTest = new DevExpress.XtraGrid.GridControl();
            grdTestView = new DevExpress.XtraGrid.Views.Grid.GridView();
            pnlControls = new DevExpress.XtraEditors.PanelControl();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnAdd = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdTest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdTestView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlControls).BeginInit();
            pnlControls.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(grdTest);
            pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            pnlMain.Location = new System.Drawing.Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new System.Drawing.Size(1258, 402);
            pnlMain.TabIndex = 0;
            // 
            // grdTest
            // 
            grdTest.Dock = System.Windows.Forms.DockStyle.Fill;
            grdTest.Location = new System.Drawing.Point(3, 3);
            grdTest.MainView = grdTestView;
            grdTest.Name = "grdTest";
            grdTest.Size = new System.Drawing.Size(1252, 396);
            grdTest.TabIndex = 0;
            grdTest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdTestView });
            // 
            // grdTestView
            // 
            grdTestView.GridControl = grdTest;
            grdTestView.Name = "grdTestView";
            grdTestView.OptionsView.ShowAutoFilterRow = true;
            grdTestView.OptionsView.ShowGroupPanel = false;
            grdTestView.CellValueChanging += grdTestView_CellValueChanging;
            // 
            // pnlControls
            // 
            pnlControls.Controls.Add(btnCancel);
            pnlControls.Controls.Add(btnAdd);
            pnlControls.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlControls.Location = new System.Drawing.Point(0, 402);
            pnlControls.Name = "pnlControls";
            pnlControls.Size = new System.Drawing.Size(1258, 96);
            pnlControls.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
            btnCancel.Location = new System.Drawing.Point(250, 22);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(200, 50);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancelar";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAdd
            // 
            btnAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAdd.ImageOptions.Image");
            btnAdd.Location = new System.Drawing.Point(24, 22);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(200, 50);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Agrergar";
            btnAdd.Click += btnAdd_Click;
            // 
            // WoAddTest
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1258, 498);
            Controls.Add(pnlControls);
            Controls.Add(pnlMain);
            Name = "WoAddTest";
            ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
            pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdTest).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdTestView).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlControls).EndInit();
            pnlControls.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlMain;
        private DevExpress.XtraGrid.GridControl grdTest;
        private DevExpress.XtraGrid.Views.Grid.GridView grdTestView;
        private DevExpress.XtraEditors.PanelControl pnlControls;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
    }
}
