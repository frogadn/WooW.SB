namespace WooW.SB.Dialogs
{
    partial class fmDialogRename
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
            txtModelo = new DevExpress.XtraEditors.TextEdit();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            txtAnterior = new DevExpress.XtraEditors.TextEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtModelo.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtAnterior.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(txtModelo);
            panelControl1.Controls.Add(labelControl2);
            panelControl1.Controls.Add(txtAnterior);
            panelControl1.Controls.Add(labelControl1);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(515, 141);
            panelControl1.TabIndex = 0;
            // 
            // txtModelo
            // 
            txtModelo.Location = new System.Drawing.Point(116, 60);
            txtModelo.Name = "txtModelo";
            txtModelo.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegularMaskManager));
            txtModelo.Properties.MaskSettings.Set("mask", "[A-Z][a-zA-Z0-9]*");
            txtModelo.Size = new System.Drawing.Size(358, 20);
            txtModelo.TabIndex = 4;
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(30, 24);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(79, 13);
            labelControl2.TabIndex = 3;
            labelControl2.Text = "Nombre Anterior";
            // 
            // txtAnterior
            // 
            txtAnterior.Enabled = false;
            txtAnterior.Location = new System.Drawing.Point(115, 21);
            txtAnterior.Name = "txtAnterior";
            txtAnterior.Size = new System.Drawing.Size(359, 20);
            txtAnterior.TabIndex = 2;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(38, 59);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(71, 13);
            labelControl1.TabIndex = 1;
            labelControl1.Text = "Nuevo Nombre";
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(buCancelar);
            panelControl2.Controls.Add(buAceptar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl2.Location = new System.Drawing.Point(0, 99);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(515, 42);
            panelControl2.TabIndex = 1;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(166, 12);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(127, 23);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(23, 12);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(127, 23);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // fmModelRename
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(515, 141);
            ControlBox = false;
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            Name = "fmModelRename";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Renombrar";
            Load += fmModelRename_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtModelo.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtAnterior.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtAnterior;
        private DevExpress.XtraEditors.TextEdit txtModelo;
    }
}