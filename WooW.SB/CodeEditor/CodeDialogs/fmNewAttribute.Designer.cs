namespace WooW.SB.CodeEditor.CodeDialogs
{
    partial class fmNewAttribute
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
            pnlButtons = new DevExpress.XtraEditors.PanelControl();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnAcept = new DevExpress.XtraEditors.SimpleButton();
            pnlMain = new DevExpress.XtraEditors.PanelControl();
            cmbTypes = new System.Windows.Forms.ComboBox();
            pnlControlValue = new DevExpress.XtraEditors.PanelControl();
            lblValue = new System.Windows.Forms.Label();
            lblName = new DevExpress.XtraEditors.LabelControl();
            txtAttribute = new DevExpress.XtraEditors.TextEdit();
            lblType = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlControlValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtAttribute.Properties).BeginInit();
            SuspendLayout();
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnAcept);
            pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlButtons.Location = new System.Drawing.Point(0, 85);
            pnlButtons.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new System.Drawing.Size(1388, 94);
            pnlButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(308, 12);
            btnCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(246, 60);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAcept
            // 
            btnAcept.Location = new System.Drawing.Point(15, 12);
            btnAcept.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnAcept.Name = "btnAcept";
            btnAcept.Size = new System.Drawing.Size(272, 60);
            btnAcept.TabIndex = 0;
            btnAcept.Text = "Aceptar";
            btnAcept.Click += btnAcept_Click;
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(cmbTypes);
            pnlMain.Controls.Add(pnlControlValue);
            pnlMain.Controls.Add(lblValue);
            pnlMain.Controls.Add(lblName);
            pnlMain.Controls.Add(txtAttribute);
            pnlMain.Controls.Add(lblType);
            pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlMain.Location = new System.Drawing.Point(0, 0);
            pnlMain.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new System.Drawing.Size(1388, 85);
            pnlMain.TabIndex = 1;
            // 
            // cmbTypes
            // 
            cmbTypes.FormattingEnabled = true;
            cmbTypes.Location = new System.Drawing.Point(90, 10);
            cmbTypes.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            cmbTypes.Name = "cmbTypes";
            cmbTypes.Size = new System.Drawing.Size(300, 33);
            cmbTypes.TabIndex = 10;
            cmbTypes.SelectedValueChanged += cmbTypes_SelectedValueChanged;
            // 
            // pnlControlValue
            // 
            pnlControlValue.Appearance.BackColor = System.Drawing.Color.Transparent;
            pnlControlValue.Appearance.Options.UseBackColor = true;
            pnlControlValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlControlValue.Location = new System.Drawing.Point(950, 10);
            pnlControlValue.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            pnlControlValue.Name = "pnlControlValue";
            pnlControlValue.Size = new System.Drawing.Size(408, 40);
            pnlControlValue.TabIndex = 9;
            // 
            // lblValue
            // 
            lblValue.AutoSize = true;
            lblValue.Location = new System.Drawing.Point(876, 17);
            lblValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblValue.Name = "lblValue";
            lblValue.Size = new System.Drawing.Size(60, 25);
            lblValue.TabIndex = 4;
            lblValue.Text = "Valor";
            // 
            // lblName
            // 
            lblName.Location = new System.Drawing.Point(406, 17);
            lblName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(75, 25);
            lblName.TabIndex = 3;
            lblName.Text = "Atributo";
            // 
            // txtAttribute
            // 
            txtAttribute.Location = new System.Drawing.Point(496, 12);
            txtAttribute.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            txtAttribute.Name = "txtAttribute";
            txtAttribute.Size = new System.Drawing.Size(368, 40);
            txtAttribute.TabIndex = 1;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(24, 17);
            lblType.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(52, 25);
            lblType.TabIndex = 0;
            lblType.Text = "Tipo";
            // 
            // fmNewAttribute
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1388, 179);
            Controls.Add(pnlMain);
            Controls.Add(pnlButtons);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Name = "fmNewAttribute";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "fmNewAttribute";
            ((System.ComponentModel.ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pnlControlValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtAttribute.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private DevExpress.XtraEditors.PanelControl pnlMain;
        private DevExpress.XtraEditors.TextEdit txtAttribute;
        private System.Windows.Forms.Label lblType;
        private DevExpress.XtraEditors.LabelControl lblName;
        private System.Windows.Forms.Label lblValue;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnAcept;
        private DevExpress.XtraEditors.PanelControl pnlControlValue;
        private System.Windows.Forms.ComboBox cmbTypes;
    }
}