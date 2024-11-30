namespace WooW.SB.Designer.DesignerComponents
{
    partial class WoNewFreeLayout
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
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            btnCreate = new DevExpress.XtraEditors.SimpleButton();
            txtLayoutName = new DevExpress.XtraEditors.TextEdit();
            lblNewLayout = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtLayoutName.Properties).BeginInit();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(panelControl2);
            panelControl1.Controls.Add(txtLayoutName);
            panelControl1.Controls.Add(lblNewLayout);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(479, 75);
            panelControl1.TabIndex = 0;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(btnCreate);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(2, 35);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(475, 38);
            panelControl2.TabIndex = 2;
            // 
            // btnCreate
            // 
            btnCreate.Dock = System.Windows.Forms.DockStyle.Left;
            btnCreate.Location = new System.Drawing.Point(2, 2);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new System.Drawing.Size(101, 34);
            btnCreate.TabIndex = 0;
            btnCreate.Text = "Crear";
            btnCreate.Click += btnCreate_Click;
            // 
            // txtLayoutName
            // 
            txtLayoutName.Dock = System.Windows.Forms.DockStyle.Top;
            txtLayoutName.Location = new System.Drawing.Point(2, 15);
            txtLayoutName.Name = "txtLayoutName";
            txtLayoutName.Size = new System.Drawing.Size(475, 20);
            txtLayoutName.TabIndex = 1;
            // 
            // lblNewLayout
            // 
            lblNewLayout.Dock = System.Windows.Forms.DockStyle.Top;
            lblNewLayout.Location = new System.Drawing.Point(2, 2);
            lblNewLayout.Name = "lblNewLayout";
            lblNewLayout.Size = new System.Drawing.Size(87, 13);
            lblNewLayout.TabIndex = 0;
            lblNewLayout.Text = "Nombre del layout";
            // 
            // WoNewFreeLayout
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(479, 75);
            Controls.Add(panelControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "WoNewFreeLayout";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtLayoutName.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnCreate;
        private DevExpress.XtraEditors.TextEdit txtLayoutName;
        private DevExpress.XtraEditors.LabelControl lblNewLayout;
    }
}
