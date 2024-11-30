namespace WooW.SB.Dialogs
{
    partial class fmConfiguracion
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
            propertyGridControl1 = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            propertyDescription = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)propertyGridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(propertyGridControl1);
            panelControl1.Controls.Add(propertyDescription);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(5);
            panelControl1.Size = new System.Drawing.Size(726, 535);
            panelControl1.TabIndex = 0;
            // 
            // propertyGridControl1
            // 
            propertyGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGridControl1.Location = new System.Drawing.Point(7, 7);
            propertyGridControl1.Name = "propertyGridControl1";
            propertyGridControl1.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            propertyGridControl1.Size = new System.Drawing.Size(712, 384);
            propertyGridControl1.TabIndex = 1;
            // 
            // propertyDescription
            // 
            propertyDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            propertyDescription.Location = new System.Drawing.Point(7, 391);
            propertyDescription.Name = "propertyDescription";
            propertyDescription.Size = new System.Drawing.Size(712, 137);
            propertyDescription.TabIndex = 9;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(buCancelar);
            panelControl2.Controls.Add(buAceptar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl2.Location = new System.Drawing.Point(0, 535);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(726, 49);
            panelControl2.TabIndex = 1;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(164, 9);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(125, 27);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "&Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(23, 9);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(125, 27);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "&Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // fmConfiguracion
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(726, 584);
            ControlBox = false;
            Controls.Add(panelControl1);
            Controls.Add(panelControl2);
            Name = "fmConfiguracion";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Configuración";
            Load += fmConfiguracion_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)propertyGridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraVerticalGrid.PropertyGridControl propertyGridControl1;
        private DevExpress.XtraVerticalGrid.PropertyDescriptionControl propertyDescription;
    }
}