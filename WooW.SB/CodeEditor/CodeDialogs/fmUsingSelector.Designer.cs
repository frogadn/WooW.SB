namespace WooW.SB.CodeEditor.CodeDialogs
{
    partial class fmUsingSelector
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
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            pnlCode = new DevExpress.XtraEditors.PanelControl();
            grdUsings = new DevExpress.XtraGrid.GridControl();
            grdUsingsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlCode).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdUsings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdUsingsView).BeginInit();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Horizontal = false;
            splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            splitContainerControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(pnlCode);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(grdUsings);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(1144, 1067);
            splitContainerControl1.SplitterPosition = 354;
            splitContainerControl1.TabIndex = 0;
            // 
            // pnlCode
            // 
            pnlCode.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlCode.Location = new System.Drawing.Point(0, 0);
            pnlCode.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            pnlCode.Name = "pnlCode";
            pnlCode.Size = new System.Drawing.Size(1144, 354);
            pnlCode.TabIndex = 0;
            // 
            // grdUsings
            // 
            grdUsings.Dock = System.Windows.Forms.DockStyle.Fill;
            grdUsings.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            grdUsings.Location = new System.Drawing.Point(0, 0);
            grdUsings.MainView = grdUsingsView;
            grdUsings.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            grdUsings.Name = "grdUsings";
            grdUsings.Size = new System.Drawing.Size(1144, 693);
            grdUsings.TabIndex = 0;
            grdUsings.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdUsingsView });
            // 
            // grdUsingsView
            // 
            grdUsingsView.DetailHeight = 673;
            grdUsingsView.GridControl = grdUsings;
            grdUsingsView.Name = "grdUsingsView";
            grdUsingsView.OptionsEditForm.PopupEditFormWidth = 1600;
            grdUsingsView.OptionsMenu.EnableGroupPanelMenu = false;
            grdUsingsView.OptionsMenu.EnableGroupRowMenu = true;
            grdUsingsView.OptionsView.ShowAutoFilterRow = true;
            grdUsingsView.OptionsView.ShowGroupPanel = false;
            grdUsingsView.CellValueChanging += grdUsingsView_CellValueChanging;
            // 
            // fmUsingSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1144, 1067);
            Controls.Add(splitContainerControl1);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "fmUsingSelector";
            Text = "fmUsingSelector";
            FormClosed += fmUsingSelector_FormClosed;
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlCode).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdUsings).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdUsingsView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.PanelControl pnlCode;
        private DevExpress.XtraGrid.GridControl grdUsings;
        private DevExpress.XtraGrid.Views.Grid.GridView grdUsingsView;
    }
}