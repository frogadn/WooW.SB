namespace WooW.SB.Forms
{
    partial class fmGuardarVista
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmGuardarVista));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.bnGuardar = new System.Windows.Forms.Button();
            this.bnCancelar = new System.Windows.Forms.Button();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.rbPlantillaUsuario = new System.Windows.Forms.RadioButton();
            this.rbPlantillaRoll = new System.Windows.Forms.RadioButton();
            this.rbPlantillaDefault = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.bnGuardar);
            this.panelControl1.Controls.Add(this.bnCancelar);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 190);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(433, 47);
            this.panelControl1.TabIndex = 8;
            // 
            // bnGuardar
            // 
            this.bnGuardar.Location = new System.Drawing.Point(55, 17);
            this.bnGuardar.Name = "bnGuardar";
            this.bnGuardar.Size = new System.Drawing.Size(131, 23);
            this.bnGuardar.TabIndex = 9;
            this.bnGuardar.Text = "Guardar";
            this.bnGuardar.UseVisualStyleBackColor = true;
            this.bnGuardar.Click += new System.EventHandler(this.bnGuardar_Click);
            // 
            // bnCancelar
            // 
            this.bnCancelar.Location = new System.Drawing.Point(238, 17);
            this.bnCancelar.Name = "bnCancelar";
            this.bnCancelar.Size = new System.Drawing.Size(131, 23);
            this.bnCancelar.TabIndex = 8;
            this.bnCancelar.Text = "Cancelar";
            this.bnCancelar.UseVisualStyleBackColor = true;
            this.bnCancelar.Click += new System.EventHandler(this.bnCancelar_Click_1);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.labelControl1);
            this.panelControl2.Controls.Add(this.txtNombre);
            this.panelControl2.Controls.Add(this.rbPlantillaUsuario);
            this.panelControl2.Controls.Add(this.rbPlantillaRoll);
            this.panelControl2.Controls.Add(this.rbPlantillaDefault);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(10);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(3);
            this.panelControl2.Size = new System.Drawing.Size(433, 190);
            this.panelControl2.TabIndex = 9;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(35, 28);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(160, 13);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "Registre un nombre para la vista:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(35, 47);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(362, 21);
            this.txtNombre.TabIndex = 3;
            this.txtNombre.Text = "DEFAULT";
            // 
            // rbPlantillaUsuario
            // 
            this.rbPlantillaUsuario.AutoSize = true;
            this.rbPlantillaUsuario.Location = new System.Drawing.Point(35, 134);
            this.rbPlantillaUsuario.Name = "rbPlantillaUsuario";
            this.rbPlantillaUsuario.Size = new System.Drawing.Size(136, 17);
            this.rbPlantillaUsuario.TabIndex = 2;
            this.rbPlantillaUsuario.TabStop = true;
            this.rbPlantillaUsuario.Text = "Plantilla para el Usuario";
            this.rbPlantillaUsuario.UseVisualStyleBackColor = true;
            // 
            // rbPlantillaRoll
            // 
            this.rbPlantillaRoll.AutoSize = true;
            this.rbPlantillaRoll.Location = new System.Drawing.Point(35, 111);
            this.rbPlantillaRoll.Name = "rbPlantillaRoll";
            this.rbPlantillaRoll.Size = new System.Drawing.Size(115, 17);
            this.rbPlantillaRoll.TabIndex = 1;
            this.rbPlantillaRoll.TabStop = true;
            this.rbPlantillaRoll.Text = "Plantilla para el Rol";
            this.rbPlantillaRoll.UseVisualStyleBackColor = true;
            // 
            // rbPlantillaDefault
            // 
            this.rbPlantillaDefault.AutoSize = true;
            this.rbPlantillaDefault.Location = new System.Drawing.Point(35, 88);
            this.rbPlantillaDefault.Name = "rbPlantillaDefault";
            this.rbPlantillaDefault.Size = new System.Drawing.Size(99, 17);
            this.rbPlantillaDefault.TabIndex = 0;
            this.rbPlantillaDefault.TabStop = true;
            this.rbPlantillaDefault.Text = "Plantilla Default";
            this.rbPlantillaDefault.UseVisualStyleBackColor = true;
            // 
            // fmGuardarVista
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 237);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fmGuardarVista";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Guardar Vista";
            this.Load += new System.EventHandler(this.fmGuardarVista_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Button bnGuardar;
        private System.Windows.Forms.Button bnCancelar;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.RadioButton rbPlantillaUsuario;
        private System.Windows.Forms.RadioButton rbPlantillaRoll;
        private System.Windows.Forms.RadioButton rbPlantillaDefault;
    }
}