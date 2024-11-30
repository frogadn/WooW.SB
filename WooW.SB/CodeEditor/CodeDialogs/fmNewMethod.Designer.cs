namespace WooW.SB.CodeEditor.CodeDialogs
{
    partial class fmNewMethod
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
            cmbTypes = new System.Windows.Forms.ComboBox();
            lblType = new System.Windows.Forms.Label();
            lblName = new System.Windows.Forms.Label();
            txtName = new DevExpress.XtraEditors.TextEdit();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            btnAddParams = new DevExpress.XtraEditors.SimpleButton();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnAcept = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(cmbTypes);
            panelControl1.Controls.Add(lblType);
            panelControl1.Controls.Add(lblName);
            panelControl1.Controls.Add(txtName);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1190, 148);
            panelControl1.TabIndex = 0;
            // 
            // cmbTypes
            // 
            cmbTypes.FormattingEnabled = true;
            cmbTypes.Location = new System.Drawing.Point(134, 12);
            cmbTypes.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            cmbTypes.Name = "cmbTypes";
            cmbTypes.Size = new System.Drawing.Size(1028, 33);
            cmbTypes.TabIndex = 3;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(10, 17);
            lblType.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(52, 25);
            lblType.TabIndex = 2;
            lblType.Text = "Tipo";
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(10, 77);
            lblName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(86, 25);
            lblName.TabIndex = 1;
            lblName.Text = "Nombre";
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(134, 71);
            txtName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(1032, 40);
            txtName.TabIndex = 0;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(btnAddParams);
            panelControl2.Controls.Add(btnCancel);
            panelControl2.Controls.Add(btnAcept);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(0, 148);
            panelControl2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(1190, 77);
            panelControl2.TabIndex = 1;
            // 
            // btnAddParams
            // 
            btnAddParams.Location = new System.Drawing.Point(656, 10);
            btnAddParams.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnAddParams.Name = "btnAddParams";
            btnAddParams.Size = new System.Drawing.Size(256, 44);
            btnAddParams.TabIndex = 2;
            btnAddParams.Text = "Agregar Parametros";
            btnAddParams.Click += btnAddParams_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(388, 10);
            btnCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(256, 44);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancelar";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAcept
            // 
            btnAcept.Location = new System.Drawing.Point(924, 10);
            btnAcept.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            btnAcept.Name = "btnAcept";
            btnAcept.Size = new System.Drawing.Size(256, 44);
            btnAcept.TabIndex = 0;
            btnAcept.Text = "Aceptar";
            btnAcept.Click += btnAcept_Click;
            // 
            // fmNewMethod
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1190, 225);
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Name = "fmNewMethod";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "fmNewMethod";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnAcept;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbTypes;
        private DevExpress.XtraEditors.SimpleButton btnAddParams;
    }
}