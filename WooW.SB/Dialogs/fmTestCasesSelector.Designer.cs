namespace WooW.SB.Dialogs
{
    partial class fmTestCasesSelector
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.buCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.buAceptar = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.treeObjetos = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeObjetos)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.buCancelar);
            this.panelControl1.Controls.Add(this.buAceptar);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 442);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(785, 44);
            this.panelControl1.TabIndex = 0;
            // 
            // buCancelar
            // 
            this.buCancelar.Location = new System.Drawing.Point(145, 9);
            this.buCancelar.Name = "buCancelar";
            this.buCancelar.Size = new System.Drawing.Size(120, 23);
            this.buCancelar.TabIndex = 1;
            this.buCancelar.Text = "Cancelar";
            this.buCancelar.Click += new System.EventHandler(this.buCancelar_Click);
            // 
            // buAceptar
            // 
            this.buAceptar.Location = new System.Drawing.Point(12, 9);
            this.buAceptar.Name = "buAceptar";
            this.buAceptar.Size = new System.Drawing.Size(120, 23);
            this.buAceptar.TabIndex = 0;
            this.buAceptar.Text = "Aceptar";
            this.buAceptar.Click += new System.EventHandler(this.buAceptar_Click);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.treeObjetos);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl2.Size = new System.Drawing.Size(785, 442);
            this.panelControl2.TabIndex = 1;
            // 
            // treeObjetos
            // 
            this.treeObjetos.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn2});
            this.treeObjetos.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeObjetos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeObjetos.Location = new System.Drawing.Point(12, 12);
            this.treeObjetos.Name = "treeObjetos";
            this.treeObjetos.OptionsBehavior.Editable = false;
            this.treeObjetos.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
            this.treeObjetos.OptionsMenu.EnableColumnMenu = false;
            this.treeObjetos.OptionsMenu.EnableFooterMenu = false;
            this.treeObjetos.OptionsSelection.InvertSelection = true;
            this.treeObjetos.OptionsView.ShowColumns = false;
            this.treeObjetos.OptionsView.ShowHorzLines = false;
            this.treeObjetos.OptionsView.ShowIndicator = false;
            this.treeObjetos.OptionsView.ShowVertLines = false;
            this.treeObjetos.Size = new System.Drawing.Size(761, 418);
            this.treeObjetos.TabIndex = 17;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "treeListColumn2";
            this.treeListColumn2.FieldName = "treeListColumn2";
            this.treeListColumn2.MinWidth = 52;
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 0;
            this.treeListColumn2.Width = 108;
            // 
            // fmCasosPruebaPruebaUnitaria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 486);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.MinimizeBox = false;
            this.Name = "fmCasosPruebaPruebaUnitaria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Seleccione una Prueba Unitaria...";
            this.Load += new System.EventHandler(this.fmCasosPruebaPruebaUnitaria_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeObjetos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraTreeList.TreeList treeObjetos;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
    }
}