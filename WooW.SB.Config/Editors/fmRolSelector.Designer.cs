namespace WooW.SB.Config
{
    partial class fmRolSelector
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
            this.grdRol = new DevExpress.XtraGrid.GridControl();
            this.grdRolView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.buActualizarDatos = new DevExpress.XtraEditors.SimpleButton();
            this.buCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.buAceptar = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdRol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdRolView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grdRol);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl1.Size = new System.Drawing.Size(1058, 531);
            this.panelControl1.TabIndex = 3;
            // 
            // grdRol
            // 
            this.grdRol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdRol.Location = new System.Drawing.Point(12, 12);
            this.grdRol.MainView = this.grdRolView;
            this.grdRol.Name = "grdRol";
            this.grdRol.Size = new System.Drawing.Size(1034, 507);
            this.grdRol.TabIndex = 0;
            this.grdRol.UseEmbeddedNavigator = true;
            this.grdRol.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdRolView});
            // 
            // grdRolView
            // 
            this.grdRolView.GridControl = this.grdRol;
            this.grdRolView.Name = "grdRolView";
            this.grdRolView.OptionsBehavior.Editable = false;
            this.grdRolView.OptionsView.ShowAutoFilterRow = true;
            this.grdRolView.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.grdEtiquetaView_InitNewRow);
            this.grdRolView.RowDeleted += new DevExpress.Data.RowDeletedEventHandler(this.grdEtiquetaView_RowDeleted);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.buCancelar);
            this.panelControl2.Controls.Add(this.panelControl3);
            this.panelControl2.Controls.Add(this.buActualizarDatos);
            this.panelControl2.Controls.Add(this.buAceptar);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 531);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl2.Size = new System.Drawing.Size(1058, 50);
            this.panelControl2.TabIndex = 4;
            // 
            // buActualizarDatos
            // 
            this.buActualizarDatos.Dock = System.Windows.Forms.DockStyle.Right;
            this.buActualizarDatos.Location = new System.Drawing.Point(927, 12);
            this.buActualizarDatos.Name = "buActualizarDatos";
            this.buActualizarDatos.Size = new System.Drawing.Size(119, 26);
            this.buActualizarDatos.TabIndex = 2;
            this.buActualizarDatos.Text = "Actualizar Roles";
            this.buActualizarDatos.Click += new System.EventHandler(this.buActualizarDatos_Click);
            // 
            // buCancelar
            // 
            this.buCancelar.Dock = System.Windows.Forms.DockStyle.Left;
            this.buCancelar.Location = new System.Drawing.Point(145, 12);
            this.buCancelar.Name = "buCancelar";
            this.buCancelar.Size = new System.Drawing.Size(119, 26);
            this.buCancelar.TabIndex = 1;
            this.buCancelar.Text = "Cancelar";
            this.buCancelar.Click += new System.EventHandler(this.buCancelar_Click);
            // 
            // buAceptar
            // 
            this.buAceptar.Dock = System.Windows.Forms.DockStyle.Left;
            this.buAceptar.Location = new System.Drawing.Point(12, 12);
            this.buAceptar.Name = "buAceptar";
            this.buAceptar.Size = new System.Drawing.Size(119, 26);
            this.buAceptar.TabIndex = 0;
            this.buAceptar.Text = "Aceptar";
            this.buAceptar.Click += new System.EventHandler(this.buAceptar_Click);
            // 
            // panelControl3
            // 
            this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl3.Location = new System.Drawing.Point(131, 12);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(14, 26);
            this.panelControl3.TabIndex = 7;
            // 
            // fmRolSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1058, 581);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.panelControl2);
            this.MinimizeBox = false;
            this.Name = "fmRolSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rol";
            this.Load += new System.EventHandler(this.EtiquetaSimple_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdRol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdRolView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl grdRol;
        private DevExpress.XtraGrid.Views.Grid.GridView grdRolView;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.SimpleButton buActualizarDatos;
        private DevExpress.XtraEditors.PanelControl panelControl3;
    }
}