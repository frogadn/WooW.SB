namespace WooW.SB.Dialogs
{
    partial class fmTestIntegralSelector
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
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            treeObjetos = new DevExpress.XtraTreeList.TreeList();
            treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treeObjetos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(treeObjetos);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(0, 0);
            panelControl2.Name = "panelControl2";
            panelControl2.Padding = new System.Windows.Forms.Padding(10);
            panelControl2.Size = new System.Drawing.Size(769, 436);
            panelControl2.TabIndex = 3;
            // 
            // treeObjetos
            // 
            treeObjetos.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { treeListColumn2 });
            treeObjetos.Dock = System.Windows.Forms.DockStyle.Fill;
            treeObjetos.Location = new System.Drawing.Point(12, 12);
            treeObjetos.Name = "treeObjetos";
            treeObjetos.OptionsBehavior.Editable = false;
            treeObjetos.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
            treeObjetos.OptionsMenu.EnableColumnMenu = false;
            treeObjetos.OptionsMenu.EnableFooterMenu = false;
            treeObjetos.OptionsSelection.InvertSelection = true;
            treeObjetos.OptionsView.ShowColumns = false;
            treeObjetos.OptionsView.ShowHorzLines = false;
            treeObjetos.OptionsView.ShowIndicator = false;
            treeObjetos.OptionsView.ShowVertLines = false;
            treeObjetos.Size = new System.Drawing.Size(745, 412);
            treeObjetos.TabIndex = 17;
            // 
            // treeListColumn2
            // 
            treeListColumn2.Caption = "treeListColumn2";
            treeListColumn2.FieldName = "treeListColumn2";
            treeListColumn2.MinWidth = 52;
            treeListColumn2.Name = "treeListColumn2";
            treeListColumn2.Visible = true;
            treeListColumn2.VisibleIndex = 0;
            treeListColumn2.Width = 108;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(buCancelar);
            panelControl1.Controls.Add(buAceptar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(0, 436);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(769, 44);
            panelControl1.TabIndex = 2;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(145, 9);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(120, 23);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Location = new System.Drawing.Point(12, 9);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(120, 23);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // fmTestIntegralSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(769, 480);
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            MinimizeBox = false;
            Name = "fmTestIntegralSelector";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Seleccione una Prueba Integral...";
            Load += fmTestIntegralSelector_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)treeObjetos).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraTreeList.TreeList treeObjetos;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
    }
}