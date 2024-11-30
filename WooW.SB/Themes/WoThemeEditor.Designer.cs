namespace WooW.SB.Themes
{
    partial class WoThemeEditor
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
            this.pnlControls = new DevExpress.XtraEditors.PanelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnAply = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.cmbThemes = new System.Windows.Forms.ComboBox();
            this.pnlGrid = new DevExpress.XtraEditors.PanelControl();
            this.propThemeEditor = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
            this.pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGrid)).BeginInit();
            this.pnlGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.panelControl1);
            this.pnlControls.Controls.Add(this.cmbThemes);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(521, 55);
            this.pnlControls.TabIndex = 0;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnAply);
            this.panelControl1.Controls.Add(this.btnSave);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(2, 23);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(517, 30);
            this.panelControl1.TabIndex = 4;
            // 
            // btnAply
            // 
            this.btnAply.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAply.Location = new System.Drawing.Point(92, 2);
            this.btnAply.Name = "btnAply";
            this.btnAply.Size = new System.Drawing.Size(91, 26);
            this.btnAply.TabIndex = 2;
            this.btnAply.Text = "Aplicar";
            this.btnAply.Click += new System.EventHandler(this.btnAply_Click);
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSave.Location = new System.Drawing.Point(2, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 26);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Guardar";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbThemes
            // 
            this.cmbThemes.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbThemes.FormattingEnabled = true;
            this.cmbThemes.Location = new System.Drawing.Point(2, 2);
            this.cmbThemes.Name = "cmbThemes";
            this.cmbThemes.Size = new System.Drawing.Size(517, 21);
            this.cmbThemes.TabIndex = 3;
            this.cmbThemes.SelectedValueChanged += new System.EventHandler(this.cmbThemes_SelectedValueChanged);
            this.cmbThemes.Enter += new System.EventHandler(this.cmbThemes_Enter);
            this.cmbThemes.Leave += new System.EventHandler(this.cmbThemes_Leave);
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.propThemeEditor);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 55);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(521, 476);
            this.pnlGrid.TabIndex = 1;
            // 
            // propThemeEditor
            // 
            this.propThemeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propThemeEditor.Location = new System.Drawing.Point(2, 2);
            this.propThemeEditor.Name = "propThemeEditor";
            this.propThemeEditor.Size = new System.Drawing.Size(517, 472);
            this.propThemeEditor.TabIndex = 0;
            // 
            // WoThemeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlGrid);
            this.Controls.Add(this.pnlControls);
            this.Name = "WoThemeEditor";
            this.Size = new System.Drawing.Size(521, 531);
            ((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
            this.pnlControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlGrid)).EndInit();
            this.pnlGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlControls;
        private DevExpress.XtraEditors.SimpleButton btnAply;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.PanelControl pnlGrid;
        private System.Windows.Forms.PropertyGrid propThemeEditor;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.ComboBox cmbThemes;
    }
}
