namespace WooW.SB.Dialogs
{
    partial class fmDiagramSelect
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
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            txtDiagrama = new DevExpress.XtraEditors.ComboBoxEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtDiagrama.Properties).BeginInit();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(buCancelar);
            panelControl1.Controls.Add(buAceptar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(0, 70);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(384, 35);
            panelControl1.TabIndex = 0;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(119, 7);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(97, 23);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(12, 7);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(97, 23);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // txtDiagrama
            // 
            txtDiagrama.Location = new System.Drawing.Point(78, 25);
            txtDiagrama.Name = "txtDiagrama";
            txtDiagrama.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            txtDiagrama.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtDiagrama.Size = new System.Drawing.Size(282, 20);
            txtDiagrama.TabIndex = 1;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(23, 28);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(49, 13);
            labelControl1.TabIndex = 2;
            labelControl1.Text = "Diagrama:";
            // 
            // fmDiagramSelect
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(384, 105);
            ControlBox = false;
            Controls.Add(labelControl1);
            Controls.Add(txtDiagrama);
            Controls.Add(panelControl1);
            Name = "fmDiagramSelect";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Seleccione Diagrama a Copiar ...";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtDiagrama.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.ComboBoxEdit txtDiagrama;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}