namespace WooW.SB.Config
{
    partial class fmPermissionSelector
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
            this.grdPermiso = new DevExpress.XtraGrid.GridControl();
            this.grdPermisoView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.buCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.buActualizarPermisos = new DevExpress.XtraEditors.SimpleButton();
            this.buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPermiso)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdPermisoView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grdPermiso);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.panelControl1.Size = new System.Drawing.Size(1234, 617);
            this.panelControl1.TabIndex = 3;
            // 
            // grdPermiso
            // 
            this.grdPermiso.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdPermiso.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grdPermiso.Location = new System.Drawing.Point(14, 14);
            this.grdPermiso.MainView = this.grdPermisoView;
            this.grdPermiso.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grdPermiso.Name = "grdPermiso";
            this.grdPermiso.Size = new System.Drawing.Size(1206, 589);
            this.grdPermiso.TabIndex = 0;
            this.grdPermiso.UseEmbeddedNavigator = true;
            this.grdPermiso.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdPermisoView});
            // 
            // grdPermisoView
            // 
            this.grdPermisoView.DetailHeight = 431;
            this.grdPermisoView.GridControl = this.grdPermiso;
            this.grdPermisoView.Name = "grdPermisoView";
            this.grdPermisoView.OptionsBehavior.Editable = false;
            this.grdPermisoView.OptionsView.ShowAutoFilterRow = true;
            this.grdPermisoView.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.grdEtiquetaView_InitNewRow);
            this.grdPermisoView.RowDeleted += new DevExpress.Data.RowDeletedEventHandler(this.grdEtiquetaView_RowDeleted);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.buCancelar);
            this.panelControl2.Controls.Add(this.panelControl3);
            this.panelControl2.Controls.Add(this.buActualizarPermisos);
            this.panelControl2.Controls.Add(this.buAceptar);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 617);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.panelControl2.Size = new System.Drawing.Size(1234, 98);
            this.panelControl2.TabIndex = 4;
            // 
            // buCancelar
            // 
            this.buCancelar.Dock = System.Windows.Forms.DockStyle.Left;
            this.buCancelar.Location = new System.Drawing.Point(169, 14);
            this.buCancelar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buCancelar.Name = "buCancelar";
            this.buCancelar.Size = new System.Drawing.Size(139, 70);
            this.buCancelar.TabIndex = 1;
            this.buCancelar.Text = "Cancelar";
            this.buCancelar.Click += new System.EventHandler(this.buCancelar_Click);
            // 
            // panelControl3
            // 
            this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl3.Location = new System.Drawing.Point(153, 14);
            this.panelControl3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(16, 70);
            this.panelControl3.TabIndex = 7;
            // 
            // buActualizarPermisos
            // 
            this.buActualizarPermisos.Dock = System.Windows.Forms.DockStyle.Right;
            this.buActualizarPermisos.Location = new System.Drawing.Point(1081, 14);
            this.buActualizarPermisos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buActualizarPermisos.Name = "buActualizarPermisos";
            this.buActualizarPermisos.Size = new System.Drawing.Size(139, 70);
            this.buActualizarPermisos.TabIndex = 2;
            this.buActualizarPermisos.Text = "Actualizar Permisos";
            this.buActualizarPermisos.Click += new System.EventHandler(this.buActualizarPermisos_Click);
            // 
            // buAceptar
            // 
            this.buAceptar.Dock = System.Windows.Forms.DockStyle.Left;
            this.buAceptar.Location = new System.Drawing.Point(14, 14);
            this.buAceptar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buAceptar.Name = "buAceptar";
            this.buAceptar.Size = new System.Drawing.Size(139, 70);
            this.buAceptar.TabIndex = 0;
            this.buAceptar.Text = "Aceptar";
            this.buAceptar.Click += new System.EventHandler(this.buAceptar_Click);
            // 
            // fmPermissionSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 715);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.panelControl2);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.Name = "fmPermissionSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Permiso";
            this.Load += new System.EventHandler(this.EtiquetaSimple_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdPermiso)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdPermisoView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl grdPermiso;
        private DevExpress.XtraGrid.Views.Grid.GridView grdPermisoView;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.SimpleButton buActualizarPermisos;
        private DevExpress.XtraEditors.PanelControl panelControl3;
    }
}