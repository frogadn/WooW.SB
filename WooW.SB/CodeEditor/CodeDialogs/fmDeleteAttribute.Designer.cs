namespace WooW.SB.CodeEditor.CodeDialogs
{
    partial class fmDeleteAttribute
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
            pnlMain = new DevExpress.XtraEditors.PanelControl();
            cmbAttrbutes = new DevExpress.XtraEditors.ComboBoxEdit();
            lblAttribute = new System.Windows.Forms.Label();
            pnlButtons = new DevExpress.XtraEditors.PanelControl();
            btnCancel = new System.Windows.Forms.Button();
            btnAcept = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbAttrbutes.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(cmbAttrbutes);
            pnlMain.Controls.Add(lblAttribute);
            pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            pnlMain.Location = new System.Drawing.Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new System.Drawing.Size(550, 48);
            pnlMain.TabIndex = 0;
            // 
            // cmbAttrbutes
            // 
            cmbAttrbutes.Location = new System.Drawing.Point(65, 10);
            cmbAttrbutes.Name = "cmbAttrbutes";
            cmbAttrbutes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbAttrbutes.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbAttrbutes.Size = new System.Drawing.Size(473, 20);
            cmbAttrbutes.TabIndex = 1;
            // 
            // lblAttribute
            // 
            lblAttribute.AutoSize = true;
            lblAttribute.Location = new System.Drawing.Point(13, 13);
            lblAttribute.Name = "lblAttribute";
            lblAttribute.Size = new System.Drawing.Size(46, 13);
            lblAttribute.TabIndex = 0;
            lblAttribute.Text = "Atributo";
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnAcept);
            pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlButtons.Location = new System.Drawing.Point(0, 48);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new System.Drawing.Size(550, 46);
            pnlButtons.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(297, 6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(116, 35);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAcept
            // 
            btnAcept.Location = new System.Drawing.Point(419, 6);
            btnAcept.Name = "btnAcept";
            btnAcept.Size = new System.Drawing.Size(119, 35);
            btnAcept.TabIndex = 0;
            btnAcept.Text = "Aceptar";
            btnAcept.UseVisualStyleBackColor = true;
            btnAcept.Click += btnAcept_Click;
            // 
            // fmDeleteAttribute
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(550, 94);
            Controls.Add(pnlButtons);
            Controls.Add(pnlMain);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "fmDeleteAttribute";
            Text = "fmDeleteAttribute";
            ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbAttrbutes.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlMain;
        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblAttribute;
        private System.Windows.Forms.Button btnAcept;
        private System.Windows.Forms.Button btnCancel;
        private DevExpress.XtraEditors.ComboBoxEdit cmbAttrbutes;
    }
}