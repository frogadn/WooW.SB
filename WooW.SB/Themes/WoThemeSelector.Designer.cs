namespace WooW.SB.Themes
{
    partial class WoThemeSelector
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
            pnlBase = new DevExpress.XtraEditors.PanelControl();
            pnlDropTemplate = new DevExpress.XtraEditors.PanelControl();
            btnDelete = new DevExpress.XtraEditors.SimpleButton();
            btnLoadTemplate = new DevExpress.XtraEditors.SimpleButton();
            btnDefault = new DevExpress.XtraEditors.SimpleButton();
            cmbThemes = new DevExpress.XtraEditors.ComboBoxEdit();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            wvwThemePreview = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)pnlBase).BeginInit();
            pnlBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlDropTemplate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbThemes.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)wvwThemePreview).BeginInit();
            SuspendLayout();
            // 
            // pnlBase
            // 
            pnlBase.Controls.Add(pnlDropTemplate);
            pnlBase.Controls.Add(btnDelete);
            pnlBase.Controls.Add(btnLoadTemplate);
            pnlBase.Controls.Add(btnDefault);
            pnlBase.Controls.Add(cmbThemes);
            pnlBase.Dock = System.Windows.Forms.DockStyle.Top;
            pnlBase.Location = new System.Drawing.Point(0, 0);
            pnlBase.Margin = new System.Windows.Forms.Padding(6);
            pnlBase.Name = "pnlBase";
            pnlBase.Size = new System.Drawing.Size(1033, 160);
            pnlBase.TabIndex = 0;
            // 
            // pnlDropTemplate
            // 
            pnlDropTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlDropTemplate.Location = new System.Drawing.Point(422, 43);
            pnlDropTemplate.Margin = new System.Windows.Forms.Padding(6);
            pnlDropTemplate.Name = "pnlDropTemplate";
            pnlDropTemplate.Size = new System.Drawing.Size(608, 114);
            pnlDropTemplate.TabIndex = 5;
            // 
            // btnDelete
            // 
            btnDelete.Dock = System.Windows.Forms.DockStyle.Left;
            btnDelete.Location = new System.Drawing.Point(283, 43);
            btnDelete.Margin = new System.Windows.Forms.Padding(6);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(139, 114);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Delete";
            btnDelete.Click += btnDelete_Click;
            // 
            // btnLoadTemplate
            // 
            btnLoadTemplate.Dock = System.Windows.Forms.DockStyle.Left;
            btnLoadTemplate.Location = new System.Drawing.Point(144, 43);
            btnLoadTemplate.Margin = new System.Windows.Forms.Padding(6);
            btnLoadTemplate.Name = "btnLoadTemplate";
            btnLoadTemplate.Size = new System.Drawing.Size(139, 114);
            btnLoadTemplate.TabIndex = 3;
            btnLoadTemplate.Text = "Cargar";
            btnLoadTemplate.Click += btnLoadTemplate_Click;
            // 
            // btnDefault
            // 
            btnDefault.Dock = System.Windows.Forms.DockStyle.Left;
            btnDefault.Location = new System.Drawing.Point(3, 43);
            btnDefault.Margin = new System.Windows.Forms.Padding(6);
            btnDefault.Name = "btnDefault";
            btnDefault.Size = new System.Drawing.Size(141, 114);
            btnDefault.TabIndex = 2;
            btnDefault.Text = "Default";
            // 
            // cmbThemes
            // 
            cmbThemes.Dock = System.Windows.Forms.DockStyle.Top;
            cmbThemes.Location = new System.Drawing.Point(3, 3);
            cmbThemes.Margin = new System.Windows.Forms.Padding(6);
            cmbThemes.Name = "cmbThemes";
            cmbThemes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbThemes.Size = new System.Drawing.Size(1027, 40);
            cmbThemes.TabIndex = 0;
            cmbThemes.SelectedValueChanged += cmbThemes_SelectedValueChanged;
            cmbThemes.TextChanged += cmbThemes_TextChanged;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(wvwThemePreview);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 160);
            panelControl1.Margin = new System.Windows.Forms.Padding(6);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1033, 1293);
            panelControl1.TabIndex = 1;
            // 
            // wvwThemePreview
            // 
            wvwThemePreview.AllowExternalDrop = true;
            wvwThemePreview.CreationProperties = null;
            wvwThemePreview.DefaultBackgroundColor = System.Drawing.Color.White;
            wvwThemePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            wvwThemePreview.Location = new System.Drawing.Point(3, 3);
            wvwThemePreview.Margin = new System.Windows.Forms.Padding(6);
            wvwThemePreview.Name = "wvwThemePreview";
            wvwThemePreview.Size = new System.Drawing.Size(1027, 1287);
            wvwThemePreview.TabIndex = 0;
            wvwThemePreview.ZoomFactor = 1D;
            // 
            // WoThemeSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Controls.Add(pnlBase);
            Margin = new System.Windows.Forms.Padding(6);
            Name = "WoThemeSelector";
            Size = new System.Drawing.Size(1033, 1453);
            ((System.ComponentModel.ISupportInitialize)pnlBase).EndInit();
            pnlBase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlDropTemplate).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbThemes.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)wvwThemePreview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlBase;
        private DevExpress.XtraEditors.SimpleButton btnDefault;
        private DevExpress.XtraEditors.ComboBoxEdit cmbThemes;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private Microsoft.Web.WebView2.WinForms.WebView2 wvwThemePreview;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnLoadTemplate;
        private DevExpress.XtraEditors.PanelControl pnlDropTemplate;
    }
}
