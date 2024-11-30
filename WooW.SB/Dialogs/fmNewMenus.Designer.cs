namespace WooW.SB.Dialogs
{
    partial class fmNewMenu
    {


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmNewMenu));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.checkListProc = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.cmbRoles = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txtMenuName = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkListProc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRoles.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.checkListProc);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.cmbRoles);
            this.panelControl1.Controls.Add(this.txtMenuName);
            this.panelControl1.Controls.Add(this.label1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(502, 281);
            this.panelControl1.TabIndex = 0;
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "prueba.png");
            this.imageCollection1.Images.SetKeyName(1, "Construction.png");
            this.imageCollection1.Images.SetKeyName(2, "Construction-Add.png");
            this.imageCollection1.Images.SetKeyName(3, "Construction-Cog.png");
            this.imageCollection1.Images.SetKeyName(4, "Construction-Information.png");
            this.imageCollection1.Images.SetKeyName(5, "Construction-Internet.png");
            this.imageCollection1.Images.SetKeyName(6, "Construction-Lock.png");
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(78, 98);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(43, 13);
            this.labelControl2.TabIndex = 10;
            this.labelControl2.Text = "Procesos";
            // 
            // checkListProc
            // 
            this.checkListProc.Location = new System.Drawing.Point(129, 97);
            this.checkListProc.Name = "checkListProc";
            this.checkListProc.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.checkListProc.Size = new System.Drawing.Size(337, 136);
            this.checkListProc.TabIndex = 9;
            this.checkListProc.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkListProc_ItemCheck_1);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(106, 69);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(15, 13);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Rol";
            // 
            // cmbRoles
            // 
            this.cmbRoles.Location = new System.Drawing.Point(129, 66);
            this.cmbRoles.Name = "cmbRoles";
            this.cmbRoles.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbRoles.Size = new System.Drawing.Size(178, 20);
            this.cmbRoles.TabIndex = 6;
            // 
            // txtMenuName
            // 
            this.txtMenuName.Location = new System.Drawing.Point(129, 24);
            this.txtMenuName.Name = "txtMenuName";
            this.txtMenuName.Size = new System.Drawing.Size(337, 20);
            this.txtMenuName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre del Menú";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.simpleButton2);
            this.panelControl2.Controls.Add(this.simpleButton1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 239);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(502, 42);
            this.panelControl2.TabIndex = 0;
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(157, 8);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(125, 23);
            this.simpleButton2.TabIndex = 1;
            this.simpleButton2.Text = "Cancelar";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(12, 8);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(125, 23);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "Aceptar";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // fmNewMenu
            // 
            this.ClientSize = new System.Drawing.Size(502, 281);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Name = "fmNewMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nuevo Menú";
            this.Activated += new System.EventHandler(this.fmNewMenu_Activated);
            this.Load += new System.EventHandler(this.fmNewMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkListProc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRoles.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit txtMenuName;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.ComboBoxEdit cmbRoles;
		private DevExpress.XtraEditors.CheckedListBoxControl checkListProc;
		private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private System.ComponentModel.IContainer components;
    }
}