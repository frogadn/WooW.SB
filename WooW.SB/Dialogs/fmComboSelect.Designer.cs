namespace WooW.SB.Dialogs
{
    partial class fmComboSelect
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
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            txtCombo = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtCombo.Properties).BeginInit();
            SuspendLayout();
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(buCancelar);
            panelControl2.Controls.Add(buAceptar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl2.Location = new System.Drawing.Point(0, 125);
            panelControl2.Margin = new System.Windows.Forms.Padding(6);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(1048, 75);
            panelControl2.TabIndex = 3;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(322, 15);
            buCancelar.Margin = new System.Windows.Forms.Padding(6);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(260, 44);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(24, 15);
            buAceptar.Margin = new System.Windows.Forms.Padding(6);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(260, 44);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // txtCombo
            // 
            txtCombo.Location = new System.Drawing.Point(143, 39);
            txtCombo.Name = "txtCombo";
            txtCombo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            txtCombo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtCombo.Size = new System.Drawing.Size(715, 40);
            txtCombo.TabIndex = 4;
            // 
            // fmComboSelect
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1048, 200);
            ControlBox = false;
            Controls.Add(txtCombo);
            Controls.Add(panelControl2);
            Name = "fmComboSelect";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "fmComboSelect";
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtCombo.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.ComboBoxEdit txtCombo;
    }
}