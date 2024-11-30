namespace WooW.SB.CodeEditor
{
    partial class WoTreeMethods
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
            grdMethodsBase = new DevExpress.XtraGrid.GridControl();
            grdMethods = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)grdMethodsBase).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdMethods).BeginInit();
            SuspendLayout();
            // 
            // grdMethodsBase
            // 
            grdMethodsBase.Dock = System.Windows.Forms.DockStyle.Fill;
            grdMethodsBase.Location = new System.Drawing.Point(0, 0);
            grdMethodsBase.MainView = grdMethods;
            grdMethodsBase.Name = "grdMethodsBase";
            grdMethodsBase.Size = new System.Drawing.Size(1056, 640);
            grdMethodsBase.TabIndex = 0;
            grdMethodsBase.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdMethods });
            // 
            // grdMethods
            // 
            grdMethods.GridControl = grdMethodsBase;
            grdMethods.Name = "grdMethods";
            grdMethods.OptionsBehavior.Editable = false;
            grdMethods.OptionsMenu.EnableGroupRowMenu = true;
            grdMethods.OptionsView.ShowAutoFilterRow = true;
            grdMethods.OptionsView.ShowGroupPanel = false;
            grdMethods.FocusedRowChanged += grdMethods_FocusedRowChanged;
            // 
            // WoTreeMethods
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(grdMethodsBase);
            Name = "WoTreeMethods";
            Size = new System.Drawing.Size(1056, 640);
            ((System.ComponentModel.ISupportInitialize)grdMethodsBase).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdMethods).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public DevExpress.XtraGrid.GridControl grdMethodsBase;
        public DevExpress.XtraGrid.Views.Grid.GridView grdMethods;
    }
}
