namespace WooW.SB
{
    partial class fmProyectoNuevo
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
            txtDirectorio = new DevExpress.XtraEditors.ButtonEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            txtNombre = new DevExpress.XtraEditors.TextEdit();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            cmbTipoProyecto = new DevExpress.XtraEditors.ComboBoxEdit();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            txtVersion = new DevExpress.XtraEditors.TextEdit();
            labelControl5 = new DevExpress.XtraEditors.LabelControl();
            txtRevision = new DevExpress.XtraEditors.TextEdit();
            labelControl6 = new DevExpress.XtraEditors.LabelControl();
            txtFix = new DevExpress.XtraEditors.TextEdit();
            optIdiomasDefault = new DevExpress.XtraEditors.CheckEdit();
            optProcesosDefault = new DevExpress.XtraEditors.CheckEdit();
            optRolesDefault = new DevExpress.XtraEditors.CheckEdit();
            optPermisosDefault = new DevExpress.XtraEditors.CheckEdit();
            optMultiTenancy = new DevExpress.XtraEditors.CheckEdit();
            optERP = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)txtDirectorio.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbTipoProyecto.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtVersion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtRevision.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtFix.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optIdiomasDefault.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optProcesosDefault.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optRolesDefault.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optPermisosDefault.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optMultiTenancy.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optERP.Properties).BeginInit();
            SuspendLayout();
            // 
            // txtDirectorio
            // 
            txtDirectorio.Location = new System.Drawing.Point(274, 37);
            txtDirectorio.Name = "txtDirectorio";
            txtDirectorio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            txtDirectorio.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtDirectorio.Size = new System.Drawing.Size(986, 40);
            txtDirectorio.TabIndex = 0;
            txtDirectorio.ButtonClick += txtDirectorio_ButtonClick;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(32, 43);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(219, 25);
            labelControl1.TabIndex = 1;
            labelControl1.Text = "Directorio del Proyecto:";
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(buCancelar);
            panelControl1.Controls.Add(buAceptar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(0, 829);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1314, 95);
            panelControl1.TabIndex = 2;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(375, 18);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(260, 53);
            buCancelar.TabIndex = 11;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(56, 18);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(260, 53);
            buAceptar.TabIndex = 10;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // txtNombre
            // 
            txtNombre.Location = new System.Drawing.Point(274, 108);
            txtNombre.Name = "txtNombre";
            txtNombre.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegularMaskManager));
            txtNombre.Properties.MaskSettings.Set("mask", "[A-Z][a-zA-Z0-9]*");
            txtNombre.Size = new System.Drawing.Size(400, 40);
            txtNombre.TabIndex = 1;
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(48, 114);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(203, 25);
            labelControl2.TabIndex = 4;
            labelControl2.Text = "Nombre del Proyecto:";
            // 
            // cmbTipoProyecto
            // 
            cmbTipoProyecto.Location = new System.Drawing.Point(274, 179);
            cmbTipoProyecto.Name = "cmbTipoProyecto";
            cmbTipoProyecto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbTipoProyecto.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbTipoProyecto.Size = new System.Drawing.Size(986, 40);
            cmbTipoProyecto.TabIndex = 2;
            // 
            // labelControl3
            // 
            labelControl3.Location = new System.Drawing.Point(87, 185);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(164, 25);
            labelControl3.TabIndex = 6;
            labelControl3.Text = "Tipo de Proyecto:";
            // 
            // labelControl4
            // 
            labelControl4.Location = new System.Drawing.Point(175, 256);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(76, 25);
            labelControl4.TabIndex = 8;
            labelControl4.Text = "Versión:";
            // 
            // txtVersion
            // 
            txtVersion.EditValue = "01";
            txtVersion.Location = new System.Drawing.Point(274, 250);
            txtVersion.Name = "txtVersion";
            txtVersion.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegularMaskManager));
            txtVersion.Properties.MaskSettings.Set("mask", "[0-9][0-9]");
            txtVersion.Size = new System.Drawing.Size(120, 40);
            txtVersion.TabIndex = 3;
            // 
            // labelControl5
            // 
            labelControl5.Location = new System.Drawing.Point(168, 327);
            labelControl5.Name = "labelControl5";
            labelControl5.Size = new System.Drawing.Size(83, 25);
            labelControl5.TabIndex = 10;
            labelControl5.Text = "Revision:";
            // 
            // txtRevision
            // 
            txtRevision.EditValue = "00";
            txtRevision.Location = new System.Drawing.Point(274, 321);
            txtRevision.Name = "txtRevision";
            txtRevision.Size = new System.Drawing.Size(120, 40);
            txtRevision.TabIndex = 4;
            // 
            // labelControl6
            // 
            labelControl6.Location = new System.Drawing.Point(218, 398);
            labelControl6.Name = "labelControl6";
            labelControl6.Size = new System.Drawing.Size(33, 25);
            labelControl6.TabIndex = 12;
            labelControl6.Text = "Fix:";
            // 
            // txtFix
            // 
            txtFix.EditValue = "00";
            txtFix.Location = new System.Drawing.Point(274, 392);
            txtFix.Name = "txtFix";
            txtFix.Size = new System.Drawing.Size(120, 40);
            txtFix.TabIndex = 5;
            // 
            // optIdiomasDefault
            // 
            optIdiomasDefault.EditValue = true;
            optIdiomasDefault.Location = new System.Drawing.Point(274, 583);
            optIdiomasDefault.Name = "optIdiomasDefault";
            optIdiomasDefault.Properties.Caption = "Agrega Idiomas Default";
            optIdiomasDefault.Size = new System.Drawing.Size(300, 40);
            optIdiomasDefault.TabIndex = 6;
            // 
            // optProcesosDefault
            // 
            optProcesosDefault.EditValue = true;
            optProcesosDefault.Location = new System.Drawing.Point(274, 641);
            optProcesosDefault.Name = "optProcesosDefault";
            optProcesosDefault.Properties.Caption = "Agrega Procesos Default";
            optProcesosDefault.Size = new System.Drawing.Size(300, 40);
            optProcesosDefault.TabIndex = 7;
            // 
            // optRolesDefault
            // 
            optRolesDefault.EditValue = true;
            optRolesDefault.Location = new System.Drawing.Point(274, 700);
            optRolesDefault.Name = "optRolesDefault";
            optRolesDefault.Properties.Caption = "Agrega Roles Default";
            optRolesDefault.Size = new System.Drawing.Size(300, 40);
            optRolesDefault.TabIndex = 8;
            // 
            // optPermisosDefault
            // 
            optPermisosDefault.EditValue = true;
            optPermisosDefault.Location = new System.Drawing.Point(274, 759);
            optPermisosDefault.Name = "optPermisosDefault";
            optPermisosDefault.Properties.Caption = "Agrega Permisos Default";
            optPermisosDefault.Size = new System.Drawing.Size(300, 40);
            optPermisosDefault.TabIndex = 9;
            // 
            // optMultiTenancy
            // 
            optMultiTenancy.EditValue = true;
            optMultiTenancy.Enabled = false;
            optMultiTenancy.Location = new System.Drawing.Point(274, 464);
            optMultiTenancy.Name = "optMultiTenancy";
            optMultiTenancy.Properties.Caption = "MultiTenancy";
            optMultiTenancy.Size = new System.Drawing.Size(300, 40);
            optMultiTenancy.TabIndex = 13;
            // 
            // optERP
            // 
            optERP.EditValue = true;
            optERP.Enabled = false;
            optERP.Location = new System.Drawing.Point(274, 524);
            optERP.Name = "optERP";
            optERP.Properties.Caption = "Modelo ERP (Unides de Negocio, Polizas, Cierres Anuales y Periodos Mensuales)";
            optERP.Size = new System.Drawing.Size(894, 40);
            optERP.TabIndex = 14;
            // 
            // fmProyectoNuevo
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1314, 924);
            ControlBox = false;
            Controls.Add(optERP);
            Controls.Add(optMultiTenancy);
            Controls.Add(optPermisosDefault);
            Controls.Add(optRolesDefault);
            Controls.Add(optProcesosDefault);
            Controls.Add(optIdiomasDefault);
            Controls.Add(labelControl6);
            Controls.Add(txtFix);
            Controls.Add(labelControl5);
            Controls.Add(txtRevision);
            Controls.Add(labelControl4);
            Controls.Add(txtVersion);
            Controls.Add(labelControl3);
            Controls.Add(cmbTipoProyecto);
            Controls.Add(labelControl2);
            Controls.Add(txtNombre);
            Controls.Add(panelControl1);
            Controls.Add(labelControl1);
            Controls.Add(txtDirectorio);
            Name = "fmProyectoNuevo";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Proyecto Nuevo";
            Load += fmProyectoNuevo_Load;
            ((System.ComponentModel.ISupportInitialize)txtDirectorio.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbTipoProyecto.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtVersion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtRevision.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtFix.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optIdiomasDefault.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optProcesosDefault.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optRolesDefault.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optPermisosDefault.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optMultiTenancy.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optERP.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.ButtonEdit txtDirectorio;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.TextEdit txtNombre;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.ComboBoxEdit cmbTipoProyecto;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtVersion;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtRevision;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtFix;
        private DevExpress.XtraEditors.CheckEdit optIdiomasDefault;
        private DevExpress.XtraEditors.CheckEdit optProcesosDefault;
        private DevExpress.XtraEditors.CheckEdit optRolesDefault;
        private DevExpress.XtraEditors.CheckEdit optPermisosDefault;
        private DevExpress.XtraEditors.CheckEdit optMultiTenancy;
        private DevExpress.XtraEditors.CheckEdit optERP;
    }
}