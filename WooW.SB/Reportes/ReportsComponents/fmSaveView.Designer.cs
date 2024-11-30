namespace WooW.SB.Reportes.ReportsComponents
{
    partial class fmSaveView
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
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            bnGuardar = new System.Windows.Forms.Button();
            bnCancelar = new System.Windows.Forms.Button();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            txtNombre = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(bnGuardar);
            panelControl1.Controls.Add(bnCancelar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(0, 98);
            panelControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(505, 54);
            panelControl1.TabIndex = 8;
            // 
            // bnGuardar
            // 
            bnGuardar.Location = new System.Drawing.Point(64, 20);
            bnGuardar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnGuardar.Name = "bnGuardar";
            bnGuardar.Size = new System.Drawing.Size(153, 27);
            bnGuardar.TabIndex = 9;
            bnGuardar.Text = "Guardar";
            bnGuardar.UseVisualStyleBackColor = true;
            bnGuardar.Click += bnGuardar_Click;
            // 
            // bnCancelar
            // 
            bnCancelar.Location = new System.Drawing.Point(278, 20);
            bnCancelar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnCancelar.Name = "bnCancelar";
            bnCancelar.Size = new System.Drawing.Size(153, 27);
            bnCancelar.TabIndex = 8;
            bnCancelar.Text = "Cancelar";
            bnCancelar.UseVisualStyleBackColor = true;
            bnCancelar.Click += bnCancelar_Click_1;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(labelControl1);
            panelControl2.Controls.Add(txtNombre);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(0, 0);
            panelControl2.Margin = new System.Windows.Forms.Padding(12);
            panelControl2.Name = "panelControl2";
            panelControl2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelControl2.Size = new System.Drawing.Size(505, 98);
            panelControl2.TabIndex = 9;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(41, 32);
            labelControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(160, 13);
            labelControl1.TabIndex = 4;
            labelControl1.Text = "Registre un nombre para la vista:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new System.Drawing.Point(41, 54);
            txtNombre.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(422, 21);
            txtNombre.TabIndex = 3;
            txtNombre.Text = "DEFAULT";
            // 
            // fmSaveView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(505, 152);
            ControlBox = false;
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "fmSaveView";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Guardar Vista";
            Load += fmGuardarVista_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            panelControl2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Button bnGuardar;
        private System.Windows.Forms.Button bnCancelar;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.TextBox txtNombre;
    }
}