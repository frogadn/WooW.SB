namespace WooW.SB.Dialogs
{
    partial class fmModelRequestNew
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
            optBackGround = new DevExpress.XtraEditors.CheckEdit();
            txtResponse = new DevExpress.XtraEditors.ComboBoxEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            optColeccion = new DevExpress.XtraEditors.CheckEdit();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)optBackGround.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtResponse.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optColeccion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(optBackGround);
            panelControl1.Controls.Add(txtResponse);
            panelControl1.Controls.Add(labelControl1);
            panelControl1.Controls.Add(optColeccion);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(439, 148);
            panelControl1.TabIndex = 0;
            // 
            // optBackGround
            // 
            optBackGround.Location = new System.Drawing.Point(109, 70);
            optBackGround.Name = "optBackGround";
            optBackGround.Properties.Caption = "Ejecuta en BackGround";
            optBackGround.Size = new System.Drawing.Size(191, 20);
            optBackGround.TabIndex = 5;
            // 
            // txtResponse
            // 
            txtResponse.Location = new System.Drawing.Point(109, 18);
            txtResponse.Name = "txtResponse";
            txtResponse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            txtResponse.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtResponse.Size = new System.Drawing.Size(268, 20);
            txtResponse.TabIndex = 4;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(25, 21);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(78, 13);
            labelControl1.TabIndex = 2;
            labelControl1.Text = "Model Response";
            // 
            // optColeccion
            // 
            optColeccion.EditValue = true;
            optColeccion.Location = new System.Drawing.Point(109, 44);
            optColeccion.Name = "optColeccion";
            optColeccion.Properties.Caption = "Regresa una colección del Modelo";
            optColeccion.Size = new System.Drawing.Size(268, 20);
            optColeccion.TabIndex = 1;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(buCancelar);
            panelControl2.Controls.Add(buAceptar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl2.Location = new System.Drawing.Point(0, 109);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(439, 39);
            panelControl2.TabIndex = 1;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(161, 8);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(130, 23);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(12, 8);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(130, 23);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // fmModelRequestNew
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(439, 148);
            ControlBox = false;
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            Name = "fmModelRequestNew";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Nuevo Request";
            Load += fmModelRequestNew_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)optBackGround.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtResponse.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optColeccion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit txtResponse;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit optColeccion;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.CheckEdit optBackGround;
    }
}