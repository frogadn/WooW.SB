namespace WooW.SB.BlazorTestGenerator.Components
{
    partial class WoNewIntegralGroup
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
            pnlNewData = new DevExpress.XtraEditors.PanelControl();
            txtGroupName = new DevExpress.XtraEditors.TextEdit();
            lblGroupName = new DevExpress.XtraEditors.LabelControl();
            pnlControls = new DevExpress.XtraEditors.PanelControl();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnCreate = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)pnlNewData).BeginInit();
            pnlNewData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtGroupName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlControls).BeginInit();
            pnlControls.SuspendLayout();
            SuspendLayout();
            // 
            // pnlNewData
            // 
            pnlNewData.Controls.Add(txtGroupName);
            pnlNewData.Controls.Add(lblGroupName);
            pnlNewData.Dock = System.Windows.Forms.DockStyle.Top;
            pnlNewData.Location = new System.Drawing.Point(0, 0);
            pnlNewData.Name = "pnlNewData";
            pnlNewData.Size = new System.Drawing.Size(1150, 92);
            pnlNewData.TabIndex = 0;
            // 
            // txtGroupName
            // 
            txtGroupName.Dock = System.Windows.Forms.DockStyle.Top;
            txtGroupName.Location = new System.Drawing.Point(3, 28);
            txtGroupName.Name = "txtGroupName";
            txtGroupName.Size = new System.Drawing.Size(1144, 40);
            txtGroupName.TabIndex = 1;
            // 
            // lblGroupName
            // 
            lblGroupName.Dock = System.Windows.Forms.DockStyle.Top;
            lblGroupName.Location = new System.Drawing.Point(3, 3);
            lblGroupName.Name = "lblGroupName";
            lblGroupName.Size = new System.Drawing.Size(171, 25);
            lblGroupName.TabIndex = 0;
            lblGroupName.Text = "Nombre del grupo";
            // 
            // pnlControls
            // 
            pnlControls.Controls.Add(btnCancel);
            pnlControls.Controls.Add(btnCreate);
            pnlControls.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlControls.Location = new System.Drawing.Point(0, 92);
            pnlControls.Name = "pnlControls";
            pnlControls.Size = new System.Drawing.Size(1150, 70);
            pnlControls.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            btnCancel.Location = new System.Drawing.Point(253, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(250, 64);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancelar";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnCreate
            // 
            btnCreate.Dock = System.Windows.Forms.DockStyle.Left;
            btnCreate.Location = new System.Drawing.Point(3, 3);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new System.Drawing.Size(250, 64);
            btnCreate.TabIndex = 0;
            btnCreate.Text = "Crear";
            btnCreate.Click += btnCreate_Click;
            // 
            // WoNewIntegralGroup
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1150, 162);
            Controls.Add(pnlControls);
            Controls.Add(pnlNewData);
            Name = "WoNewIntegralGroup";
            ((System.ComponentModel.ISupportInitialize)pnlNewData).EndInit();
            pnlNewData.ResumeLayout(false);
            pnlNewData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtGroupName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlControls).EndInit();
            pnlControls.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlNewData;
        private DevExpress.XtraEditors.TextEdit txtGroupName;
        private DevExpress.XtraEditors.LabelControl lblGroupName;
        private DevExpress.XtraEditors.PanelControl pnlControls;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnCreate;
    }
}
