namespace WooW.SB.Reportes.ReportsComponents
{
    partial class WoSimpleDesigner
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
            pnlBaseDesigner.Name = "pnlBaseDesigner";
            pnlBaseDesigner.Size = new System.Drawing.Size(829, 554);
            pnlBaseDesigner.TabIndex = 0;
            // 
            // WoSimpleDesigner
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(pnlBaseDesigner);
            Name = "WoSimpleDesigner";
            Size = new System.Drawing.Size(829, 554);
            ((System.ComponentModel.ISupportInitialize)pnlBaseDesigner).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlBaseDesigner;
    }
}
