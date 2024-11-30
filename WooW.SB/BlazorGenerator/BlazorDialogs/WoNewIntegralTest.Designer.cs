namespace WooW.SB.BlazorGenerator.BlazorDialogs
{
    partial class WoNewIntegralTest
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
            txtNameProyect = new DevExpress.XtraEditors.TextEdit();
            btnAddProyectSettings = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)txtNameProyect.Properties).BeginInit();
            SuspendLayout();
            // 
            // txtNameProyect
            // 
            txtNameProyect.Dock = System.Windows.Forms.DockStyle.Top;
            txtNameProyect.Location = new System.Drawing.Point(0, 0);
            txtNameProyect.Name = "txtNameProyect";
            txtNameProyect.Size = new System.Drawing.Size(800, 40);
            txtNameProyect.TabIndex = 0;
            // 
            // btnAddProyectSettings
            // 
            btnAddProyectSettings.Location = new System.Drawing.Point(12, 46);
            btnAddProyectSettings.Name = "btnAddProyectSettings";
            btnAddProyectSettings.Size = new System.Drawing.Size(231, 53);
            btnAddProyectSettings.TabIndex = 1;
            btnAddProyectSettings.Text = "Crear";
            btnAddProyectSettings.Click += btnAddProyectSettings_Click;
            // 
            // WoNewIntegralTest
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 119);
            Controls.Add(btnAddProyectSettings);
            Controls.Add(txtNameProyect);
            Name = "WoNewIntegralTest";
            Text = "Nueva prueba integral";
            ((System.ComponentModel.ISupportInitialize)txtNameProyect.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtNameProyect;
        private DevExpress.XtraEditors.SimpleButton btnAddProyectSettings;
    }
}