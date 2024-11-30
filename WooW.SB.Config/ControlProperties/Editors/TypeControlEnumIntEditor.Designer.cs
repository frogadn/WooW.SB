namespace WooW.SB.Config.ControlProperties.Editors
{
    partial class TypeControlEnumIntEditor<T>
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
            this.grdEnum = new DevExpress.XtraGrid.GridControl();
            this.grdEnumView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdEnum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdEnumView)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.buCancelar);
            this.panelControl1.Controls.Add(this.buAceptar);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 385);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1232, 42);
            this.panelControl1.TabIndex = 0;
            // 
            // buCancelar
            // 
            this.buCancelar.Location = new System.Drawing.Point(134, 9);
            this.buCancelar.Name = "buCancelar";
            this.buCancelar.Size = new System.Drawing.Size(106, 24);
            this.buCancelar.TabIndex = 1;
            this.buCancelar.Text = "Cancelar";
            this.buCancelar.Click += new System.EventHandler(this.buCancelar_Click);
            // 
            // buAceptar
            // 
            this.buAceptar.Location = new System.Drawing.Point(9, 9);
            this.buAceptar.Name = "buAceptar";
            this.buAceptar.Size = new System.Drawing.Size(106, 24);
            this.buAceptar.TabIndex = 0;
            this.buAceptar.Text = "Aceptar";
            this.buAceptar.Click += new System.EventHandler(this.buAceptar_Click);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.grdEnum);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl2.Size = new System.Drawing.Size(1232, 385);
            this.panelControl2.TabIndex = 1;
            // 
            // grdEnum
            // 
            this.grdEnum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdEnum.Location = new System.Drawing.Point(12, 12);
            this.grdEnum.MainView = this.grdEnumView;
            this.grdEnum.Name = "grdEnum";
            this.grdEnum.Size = new System.Drawing.Size(1208, 361);
            this.grdEnum.TabIndex = 2;
            this.grdEnum.UseEmbeddedNavigator = true;
            this.grdEnum.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdEnumView});
            // 
            // grdEnumView
            // 
            this.grdEnumView.GridControl = this.grdEnum;
            this.grdEnumView.Name = "grdEnumView";
            this.grdEnumView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.Inplace;
            this.grdEnumView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.grdEnumView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this.grdEnumView.OptionsView.ShowGroupPanel = false;
            this.grdEnumView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grdEnumView_FocusedRowChanged);
            // 
            // TypeControlEnumIntEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1232, 427);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TypeControlEnumIntEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Propiedades del Control";
            this.Load += new System.EventHandler(this.TypeControlEnumEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdEnum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdEnumView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.GridControl grdEnum;
        private DevExpress.XtraGrid.Views.Grid.GridView grdEnumView;
    }
}