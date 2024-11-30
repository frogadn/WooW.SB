namespace WooW.SB.Designer
{
    partial class WoFormDesigner
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
            ((System.ComponentModel.ISupportInitialize)pnlBaseDesigner).BeginInit();
            SuspendLayout();
            // 
            // pnlBaseDesigner
            // 
            pnlBaseDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlBaseDesigner.Location = new System.Drawing.Point(0, 0);
            pnlBaseDesigner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pnlBaseDesigner.Name = "pnlBaseDesigner";
            pnlBaseDesigner.Size = new System.Drawing.Size(1325, 700);
            pnlBaseDesigner.TabIndex = 0;
            // 
            // WoFormDesigner
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(pnlBaseDesigner);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "WoFormDesigner";
            Size = new System.Drawing.Size(1325, 700);
            ((System.ComponentModel.ISupportInitialize)pnlBaseDesigner).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlBaseDesigner;
    }
}
