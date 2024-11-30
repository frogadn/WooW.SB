namespace WooW.SB.CodeEditor.CodeDialogs
{
    partial class fmNewMethodParams
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
            btnDelete = new System.Windows.Forms.Button();
            pnlControlValue = new DevExpress.XtraEditors.PanelControl();
            btnAdd = new System.Windows.Forms.Button();
            lbcParams = new DevExpress.XtraEditors.ListBoxControl();
            lblValue = new System.Windows.Forms.Label();
            cmbTypes = new System.Windows.Forms.ComboBox();
            lblType = new System.Windows.Forms.Label();
            lblParameter = new System.Windows.Forms.Label();
            txtParametro = new System.Windows.Forms.TextBox();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            btnCancel = new System.Windows.Forms.Button();
            btnContinue = new System.Windows.Forms.Button();
            btnAceptar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlControlValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lbcParams).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(btnDelete);
            panelControl1.Controls.Add(pnlControlValue);
            panelControl1.Controls.Add(btnAdd);
            panelControl1.Controls.Add(lbcParams);
            panelControl1.Controls.Add(lblValue);
            panelControl1.Controls.Add(cmbTypes);
            panelControl1.Controls.Add(lblType);
            panelControl1.Controls.Add(lblParameter);
            panelControl1.Controls.Add(txtParametro);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1704, 429);
            panelControl1.TabIndex = 0;
            // 
            // btnDelete
            // 
            btnDelete.Location = new System.Drawing.Point(1624, 10);
            btnDelete.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(56, 42);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "X";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // pnlControlValue
            // 
            pnlControlValue.Appearance.BackColor = System.Drawing.Color.Transparent;
            pnlControlValue.Appearance.Options.UseBackColor = true;
            pnlControlValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlControlValue.Location = new System.Drawing.Point(1046, 12);
            pnlControlValue.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            pnlControlValue.Name = "pnlControlValue";
            pnlControlValue.Size = new System.Drawing.Size(408, 40);
            pnlControlValue.TabIndex = 8;
            // 
            // btnAdd
            // 
            btnAdd.Location = new System.Drawing.Point(1466, 10);
            btnAdd.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(146, 42);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "Agregar";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // lbcParams
            // 
            lbcParams.Location = new System.Drawing.Point(30, 63);
            lbcParams.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            lbcParams.Name = "lbcParams";
            lbcParams.Size = new System.Drawing.Size(1650, 344);
            lbcParams.TabIndex = 6;
            lbcParams.SelectedValueChanged += lbcParams_SelectedValueChanged;
            // 
            // lblValue
            // 
            lblValue.AutoSize = true;
            lblValue.Location = new System.Drawing.Point(972, 17);
            lblValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblValue.Name = "lblValue";
            lblValue.Size = new System.Drawing.Size(60, 25);
            lblValue.TabIndex = 4;
            lblValue.Text = "Valor";
            // 
            // cmbTypes
            // 
            cmbTypes.FormattingEnabled = true;
            cmbTypes.Location = new System.Drawing.Point(90, 12);
            cmbTypes.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            cmbTypes.Name = "cmbTypes";
            cmbTypes.Size = new System.Drawing.Size(312, 33);
            cmbTypes.TabIndex = 3;
            cmbTypes.SelectedValueChanged += cmbTypes_SelectedValueChanged;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(24, 17);
            lblType.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(52, 25);
            lblType.TabIndex = 2;
            lblType.Text = "Tipo";
            // 
            // lblParameter
            // 
            lblParameter.AutoSize = true;
            lblParameter.Location = new System.Drawing.Point(418, 17);
            lblParameter.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblParameter.Name = "lblParameter";
            lblParameter.Size = new System.Drawing.Size(109, 25);
            lblParameter.TabIndex = 1;
            lblParameter.Text = "Parametro";
            // 
            // txtParametro
            // 
            txtParametro.Location = new System.Drawing.Point(544, 12);
            txtParametro.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            txtParametro.Name = "txtParametro";
            txtParametro.Size = new System.Drawing.Size(412, 33);
            txtParametro.TabIndex = 0;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(btnCancel);
            panelControl2.Controls.Add(btnContinue);
            panelControl2.Controls.Add(btnAceptar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(0, 429);
            panelControl2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(1704, 92);
            panelControl2.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(593, 14);
            btnCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(212, 63);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnContinue
            // 
            btnContinue.Location = new System.Drawing.Point(279, 14);
            btnContinue.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnContinue.Name = "btnContinue";
            btnContinue.Size = new System.Drawing.Size(302, 63);
            btnContinue.TabIndex = 1;
            btnContinue.Text = "Continuar Sin Parámetros";
            btnContinue.UseVisualStyleBackColor = true;
            btnContinue.Click += btnContinue_Click;
            // 
            // btnAceptar
            // 
            btnAceptar.Location = new System.Drawing.Point(15, 14);
            btnAceptar.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new System.Drawing.Size(252, 63);
            btnAceptar.TabIndex = 0;
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = true;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // fmNewMethodParams
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1704, 521);
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Name = "fmNewMethodParams";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "fmNewMethodParams";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pnlControlValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)lbcParams).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblParameter;
        private System.Windows.Forms.TextBox txtParametro;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.ComboBox cmbTypes;
        private DevExpress.XtraEditors.ListBoxControl lbcParams;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnContinue;
        private DevExpress.XtraEditors.PanelControl pnlControlValue;
        private System.Windows.Forms.Button btnDelete;
    }
}