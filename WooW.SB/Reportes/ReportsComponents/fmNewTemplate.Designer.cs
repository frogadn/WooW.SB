namespace WooW.SB.Reportes.ReportsComponents
{
    partial class fmNewTemplate
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
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            txtPlantilla = new DevExpress.XtraEditors.TextEdit();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtPlantilla.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(labelControl1);
            panelControl1.Controls.Add(txtPlantilla);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(2);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(511, 74);
            panelControl1.TabIndex = 0;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(34, 31);
            labelControl1.Margin = new System.Windows.Forms.Padding(2);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(102, 13);
            labelControl1.TabIndex = 1;
            labelControl1.Text = "Nombre de la Plantilla";
            // 
            // txtPlantilla
            // 
            txtPlantilla.Location = new System.Drawing.Point(154, 28);
            txtPlantilla.Margin = new System.Windows.Forms.Padding(2);
            txtPlantilla.Name = "txtPlantilla";
            txtPlantilla.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegularMaskManager));
            txtPlantilla.Properties.MaskSettings.Set("MaskManagerSignature", "ignoreMaskBlank=True");
            txtPlantilla.Properties.MaskSettings.Set("mask", "[A-Z][a-zA-Z0-9]*");
            txtPlantilla.Size = new System.Drawing.Size(320, 20);
            txtPlantilla.TabIndex = 0;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(buCancelar);
            panelControl2.Controls.Add(buAceptar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl2.Location = new System.Drawing.Point(0, 74);
            panelControl2.Margin = new System.Windows.Forms.Padding(2);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(511, 34);
            panelControl2.TabIndex = 1;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(150, 7);
            buCancelar.Margin = new System.Windows.Forms.Padding(2);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(120, 21);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click_1;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(14, 7);
            buAceptar.Margin = new System.Windows.Forms.Padding(2);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(120, 21);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click_1;
            // 
            // fmNewTemplate
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(511, 108);
            ControlBox = false;
            Controls.Add(panelControl1);
            Controls.Add(panelControl2);
            IconOptions.ShowIcon = false;
            Margin = new System.Windows.Forms.Padding(2);
            Name = "fmNewTemplate";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Nueva Plantilla";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtPlantilla.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ResumeLayout(false);
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