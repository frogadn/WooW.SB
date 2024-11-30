namespace WooW.SB.BlazorTestGenerator.Components
{
    partial class WoNewIntegralTest
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            cmbGroup = new DevExpress.XtraEditors.ComboBoxEdit();
            lblGroup = new DevExpress.XtraEditors.LabelControl();
            txtDescripcion = new DevExpress.XtraEditors.MemoEdit();
            lblDescripcion = new DevExpress.XtraEditors.LabelControl();
            txtName = new DevExpress.XtraEditors.TextEdit();
            lblName = new System.Windows.Forms.Label();
            pnlControls = new DevExpress.XtraEditors.PanelControl();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnCreate = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbGroup.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtDescripcion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlControls).BeginInit();
            pnlControls.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(cmbGroup);
            panelControl1.Controls.Add(lblGroup);
            panelControl1.Controls.Add(txtDescripcion);
            panelControl1.Controls.Add(lblDescripcion);
            panelControl1.Controls.Add(txtName);
            panelControl1.Controls.Add(lblName);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1166, 284);
            panelControl1.TabIndex = 0;
            // 
            // cmbGroup
            // 
            cmbGroup.Dock = System.Windows.Forms.DockStyle.Top;
            cmbGroup.Location = new System.Drawing.Point(3, 224);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbGroup.Size = new System.Drawing.Size(1160, 40);
            cmbGroup.TabIndex = 5;
            // 
            // lblGroup
            // 
            lblGroup.Dock = System.Windows.Forms.DockStyle.Top;
            lblGroup.Location = new System.Drawing.Point(3, 199);
            lblGroup.Name = "lblGroup";
            lblGroup.Size = new System.Drawing.Size(169, 25);
            lblGroup.TabIndex = 4;
            lblGroup.Text = "Grupo de pruebas";
            // 
            // txtDescripcion
            // 
            txtDescripcion.Dock = System.Windows.Forms.DockStyle.Top;
            txtDescripcion.Location = new System.Drawing.Point(3, 93);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new System.Drawing.Size(1160, 106);
            txtDescripcion.TabIndex = 3;
            // 
            // lblDescripcion
            // 
            lblDescripcion.Dock = System.Windows.Forms.DockStyle.Top;
            lblDescripcion.Location = new System.Drawing.Point(3, 68);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new System.Drawing.Size(233, 25);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Descripción de la prueba";
            // 
            // txtName
            // 
            txtName.Dock = System.Windows.Forms.DockStyle.Top;
            txtName.Location = new System.Drawing.Point(3, 28);
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(1160, 40);
            txtName.TabIndex = 1;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Dock = System.Windows.Forms.DockStyle.Top;
            lblName.Location = new System.Drawing.Point(3, 3);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(212, 25);
            lblName.TabIndex = 0;
            lblName.Text = "Nombre de la prueba";
            // 
            // pnlControls
            // 
            pnlControls.Controls.Add(btnCancel);
            pnlControls.Controls.Add(btnCreate);
            pnlControls.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlControls.Location = new System.Drawing.Point(0, 284);
            pnlControls.Name = "pnlControls";
            pnlControls.Size = new System.Drawing.Size(1166, 77);
            pnlControls.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            btnCancel.Location = new System.Drawing.Point(253, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(250, 71);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancelar";
            // 
            // btnCreate
            // 
            btnCreate.Dock = System.Windows.Forms.DockStyle.Left;
            btnCreate.Location = new System.Drawing.Point(3, 3);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new System.Drawing.Size(250, 71);
            btnCreate.TabIndex = 0;
            btnCreate.Text = "Crear";
            btnCreate.Click += btnCreate_Click;
            // 
            // WoNewIntegralTest
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1166, 361);
            Controls.Add(pnlControls);
            Controls.Add(panelControl1);
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Name = "WoNewIntegralTest";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbGroup.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtDescripcion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlControls).EndInit();
            pnlControls.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblDescripcion;
        private DevExpress.XtraEditors.TextEdit txtName;
        private System.Windows.Forms.Label lblName;
        private DevExpress.XtraEditors.ComboBoxEdit cmbGroup;
        private DevExpress.XtraEditors.LabelControl lblGroup;
        private DevExpress.XtraEditors.MemoEdit txtDescripcion;
        private DevExpress.XtraEditors.PanelControl pnlControls;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnCreate;
    }
}
