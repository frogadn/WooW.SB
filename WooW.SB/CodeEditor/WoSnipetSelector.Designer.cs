namespace WooW.SB.CodeEditor
{
    partial class WoSnipetSelector
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
            grdSnipets = new DevExpress.XtraGrid.GridControl();
            grdViewSnipets = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)grdSnipets).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdViewSnipets).BeginInit();
            SuspendLayout();
            // 
            // grdSnipets
            // 
            grdSnipets.Dock = System.Windows.Forms.DockStyle.Fill;
            grdSnipets.Location = new System.Drawing.Point(0, 0);
            grdSnipets.MainView = grdViewSnipets;
            grdSnipets.Name = "grdSnipets";
            grdSnipets.Size = new System.Drawing.Size(621, 684);
            grdSnipets.TabIndex = 0;
            grdSnipets.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdViewSnipets });
            // 
            // grdViewSnipets
            // 
            grdViewSnipets.GridControl = grdSnipets;
            grdViewSnipets.Name = "grdViewSnipets";
            grdViewSnipets.OptionsBehavior.Editable = false;
            grdViewSnipets.OptionsView.ShowAutoFilterRow = true;
            grdViewSnipets.OptionsView.ShowGroupPanel = false;
            // 
            // WoSnipetSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(grdSnipets);
            Name = "WoSnipetSelector";
            Size = new System.Drawing.Size(621, 684);
            ((System.ComponentModel.ISupportInitialize)grdSnipets).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdViewSnipets).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraGrid.GridControl grdSnipets;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewSnipets;
    }
}
