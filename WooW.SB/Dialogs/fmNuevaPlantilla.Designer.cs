namespace WooW.SB.Dialogs
{
    partial class fmNuevaPlantilla
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.buAceptar = new DevExpress.XtraEditors.SimpleButton();
            this.buCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.txtPlantilla = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlantilla.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtPlantilla);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1022, 141);
            this.panelControl1.TabIndex = 0;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.buCancelar);
            this.panelControl2.Controls.Add(this.buAceptar);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 141);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1022, 66);
            this.panelControl2.TabIndex = 1;
            // 
            // buAceptar
            // 
            this.buAceptar.Location = new System.Drawing.Point(27, 14);
            this.buAceptar.Name = "buAceptar";
            this.buAceptar.Size = new System.Drawing.Size(241, 40);
            this.buAceptar.TabIndex = 0;
            this.buAceptar.Text = "Aceptar";
            this.buAceptar.Click += new System.EventHandler(this.buAceptar_Click);
            // 
            // buCancelar
            // 
            this.buCancelar.Location = new System.Drawing.Point(299, 14);
            this.buCancelar.Name = "buCancelar";
            this.buCancelar.Size = new System.Drawing.Size(241, 40);
            this.buCancelar.TabIndex = 1;
            this.buCancelar.Text = "Cancelar";
            this.buCancelar.Click += new System.EventHandler(this.buCancelar_Click);
            // 
            // txtPlantilla
            // 
            this.txtPlantilla.Location = new System.Drawing.Point(308, 54);
            this.txtPlantilla.Name = "txtPlantilla";
            this.txtPlantilla.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegularMaskManager));
            this.txtPlantilla.Properties.MaskSettings.Set("MaskManagerSignature", "ignoreMaskBlank=True");
            this.txtPlantilla.Properties.MaskSettings.Set("mask", "[A-Z][a-zA-Z0-9]*");
            this.txtPlantilla.Size = new System.Drawing.Size(639, 40);
            this.txtPlantilla.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(67, 60);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(207, 25);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Nombre de la Plantilla";
            // 
            // fmNuevaPlantilla
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 207);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.panelControl2);
            this.Name = "fmNuevaPlantilla";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nueva Plantilla";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPlantilla.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtPlantilla;
    }
}