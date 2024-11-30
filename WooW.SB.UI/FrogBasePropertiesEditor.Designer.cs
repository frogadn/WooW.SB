namespace WooW.SB.UI
{
    partial class FrogBasePropertiesEditor
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
            this.panBotones = new DevExpress.XtraEditors.PanelControl();
            this.btnCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.btnAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panBotones)).BeginInit();
            this.panBotones.SuspendLayout();
            this.SuspendLayout();
            // 
            // panBotones
            // 
            this.panBotones.Controls.Add(this.btnCancelar);
            this.panBotones.Controls.Add(this.btnAceptar);
            this.panBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panBotones.Location = new System.Drawing.Point(0, 423);
            this.panBotones.Name = "panBotones";
            this.panBotones.Size = new System.Drawing.Size(421, 56);
            this.panBotones.TabIndex = 2;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(266, 9);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 41);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnAceptar
            // 
            this.btnAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAceptar.Location = new System.Drawing.Point(130, 9);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(130, 41);
            this.btnAceptar.TabIndex = 0;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // FrogBasePropertiesEditor
            // 
            this.ClientSize = new System.Drawing.Size(421, 479);
            this.ControlBox = false;
            this.Controls.Add(this.panBotones);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrogBasePropertiesEditor";
            this.ShowInTaskbar = false;
            this.Text = "Editor de Propiedades ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrogBasePropertiesEditor_FormClosed);
            this.Load += new System.EventHandler(this.FrogBasePropertiesEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrogBasePropertiesEditor_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.panBotones)).EndInit();
            this.panBotones.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panBotones;
        private DevExpress.XtraEditors.SimpleButton btnCancelar;
        private DevExpress.XtraEditors.SimpleButton btnAceptar;
    }
}
