namespace WooW.SB.BlazorTestGenerator.Components
{
    partial class WoNewTest
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
            lblModel = new DevExpress.XtraEditors.LabelControl();
            lblTypeTest = new DevExpress.XtraEditors.LabelControl();
            cmbTestType = new DevExpress.XtraEditors.ComboBoxEdit();
            btnCreateTest = new DevExpress.XtraEditors.SimpleButton();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            cmbModel = new DevExpress.XtraEditors.ComboBoxEdit();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            panelControl4 = new DevExpress.XtraEditors.PanelControl();
            txtDescripcion = new DevExpress.XtraEditors.MemoEdit();
            lblDescripcion = new System.Windows.Forms.Label();
            panelControl5 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)cmbTestType.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbModel.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl4).BeginInit();
            panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtDescripcion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl5).BeginInit();
            panelControl5.SuspendLayout();
            SuspendLayout();
            // 
            // lblModel
            // 
            lblModel.Dock = System.Windows.Forms.DockStyle.Top;
            lblModel.Location = new System.Drawing.Point(2, 2);
            lblModel.Margin = new System.Windows.Forms.Padding(2);
            lblModel.Name = "lblModel";
            lblModel.Size = new System.Drawing.Size(34, 13);
            lblModel.TabIndex = 2;
            lblModel.Text = "Modelo";
            // 
            // lblTypeTest
            // 
            lblTypeTest.Dock = System.Windows.Forms.DockStyle.Top;
            lblTypeTest.Location = new System.Drawing.Point(2, 2);
            lblTypeTest.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            lblTypeTest.Name = "lblTypeTest";
            lblTypeTest.Size = new System.Drawing.Size(72, 13);
            lblTypeTest.TabIndex = 1;
            lblTypeTest.Text = "Tipo de prueba";
            // 
            // cmbTestType
            // 
            cmbTestType.Dock = System.Windows.Forms.DockStyle.Top;
            cmbTestType.Location = new System.Drawing.Point(2, 15);
            cmbTestType.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            cmbTestType.Name = "cmbTestType";
            cmbTestType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbTestType.Size = new System.Drawing.Size(614, 20);
            cmbTestType.TabIndex = 0;
            // 
            // btnCreateTest
            // 
            btnCreateTest.Dock = System.Windows.Forms.DockStyle.Left;
            btnCreateTest.Location = new System.Drawing.Point(2, 2);
            btnCreateTest.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnCreateTest.Name = "btnCreateTest";
            btnCreateTest.Size = new System.Drawing.Size(110, 35);
            btnCreateTest.TabIndex = 1;
            btnCreateTest.Text = "Crear Prueba";
            btnCreateTest.Click += btnCreateTest_Click;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(cmbModel);
            panelControl1.Controls.Add(lblModel);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(2);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(618, 45);
            panelControl1.TabIndex = 2;
            // 
            // cmbModel
            // 
            cmbModel.Dock = System.Windows.Forms.DockStyle.Top;
            cmbModel.Location = new System.Drawing.Point(2, 15);
            cmbModel.Margin = new System.Windows.Forms.Padding(2);
            cmbModel.Name = "cmbModel";
            cmbModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbModel.Size = new System.Drawing.Size(614, 20);
            cmbModel.TabIndex = 3;
            cmbModel.SelectedValueChanged += cmbModel_SelectedValueChanged;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(cmbTestType);
            panelControl2.Controls.Add(lblTypeTest);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl2.Location = new System.Drawing.Point(0, 45);
            panelControl2.Margin = new System.Windows.Forms.Padding(2);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(618, 45);
            panelControl2.TabIndex = 3;
            // 
            // btnCancel
            // 
            btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            btnCancel.Location = new System.Drawing.Point(112, 2);
            btnCancel.Margin = new System.Windows.Forms.Padding(2);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(110, 35);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancelar";
            btnCancel.Click += btnCancel_Click;
            // 
            // panelControl4
            // 
            panelControl4.Controls.Add(txtDescripcion);
            panelControl4.Controls.Add(lblDescripcion);
            panelControl4.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl4.Location = new System.Drawing.Point(0, 90);
            panelControl4.Margin = new System.Windows.Forms.Padding(2);
            panelControl4.Name = "panelControl4";
            panelControl4.Size = new System.Drawing.Size(618, 75);
            panelControl4.TabIndex = 5;
            // 
            // txtDescripcion
            // 
            txtDescripcion.Dock = System.Windows.Forms.DockStyle.Fill;
            txtDescripcion.Location = new System.Drawing.Point(2, 15);
            txtDescripcion.Margin = new System.Windows.Forms.Padding(2);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new System.Drawing.Size(614, 58);
            txtDescripcion.TabIndex = 1;
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Dock = System.Windows.Forms.DockStyle.Top;
            lblDescripcion.Location = new System.Drawing.Point(2, 2);
            lblDescripcion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new System.Drawing.Size(113, 13);
            lblDescripcion.TabIndex = 0;
            lblDescripcion.Text = "Descripción (Opcional)";
            // 
            // panelControl5
            // 
            panelControl5.Controls.Add(btnCancel);
            panelControl5.Controls.Add(btnCreateTest);
            panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl5.Location = new System.Drawing.Point(0, 165);
            panelControl5.Margin = new System.Windows.Forms.Padding(2);
            panelControl5.Name = "panelControl5";
            panelControl5.Size = new System.Drawing.Size(618, 39);
            panelControl5.TabIndex = 6;
            // 
            // WoNewTest
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(618, 204);
            Controls.Add(panelControl5);
            Controls.Add(panelControl4);
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            MinimumSize = new System.Drawing.Size(224, 112);
            Name = "WoNewTest";
            Text = "Nueva Prueba";
            ((System.ComponentModel.ISupportInitialize)cmbTestType.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbModel.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl4).EndInit();
            panelControl4.ResumeLayout(false);
            panelControl4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtDescripcion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl5).EndInit();
            panelControl5.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraEditors.LabelControl lblTypeTest;
        private DevExpress.XtraEditors.ComboBoxEdit cmbTestType;
        private DevExpress.XtraEditors.SimpleButton btnCreateTest;
        private DevExpress.XtraEditors.LabelControl lblModel;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private DevExpress.XtraEditors.MemoEdit txtDescripcion;
        private System.Windows.Forms.Label lblDescripcion;
        private DevExpress.XtraEditors.PanelControl panelControl5;
        private DevExpress.XtraEditors.ComboBoxEdit cmbModel;
    }
}
