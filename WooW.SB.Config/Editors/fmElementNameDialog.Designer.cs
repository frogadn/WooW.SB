namespace WooW.SB.Config
{
    partial class fmElementNameDialog
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
            this.txtNombre = new DevExpress.XtraEditors.TextEdit();
            this.buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(36, 22);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegularMaskManager));
            this.txtNombre.Properties.MaskSettings.Set("mask", "[A-Z][a-zA-Z0-9]*");
            this.txtNombre.Size = new System.Drawing.Size(305, 20);
            this.txtNombre.TabIndex = 0;
            // 
            // buAceptar
            // 
            this.buAceptar.Location = new System.Drawing.Point(135, 59);
            this.buAceptar.Name = "buAceptar";
            this.buAceptar.Size = new System.Drawing.Size(98, 23);
            this.buAceptar.TabIndex = 1;
            this.buAceptar.Text = "Aceptar";
            this.buAceptar.Click += new System.EventHandler(this.buAceptar_Click);
            // 
            // AdminTransacNameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 102);
            this.ControlBox = false;
            this.Controls.Add(this.buAceptar);
            this.Controls.Add(this.txtNombre);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdminTransacNameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Escriba un nombre para el elemento";
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtNombre;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
    }
}