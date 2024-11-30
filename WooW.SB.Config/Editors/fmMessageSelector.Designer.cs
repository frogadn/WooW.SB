namespace WooW.SB.Config
{
    partial class fmMessageSelector
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
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.buCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.buAceptar = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.grdMensaje = new DevExpress.XtraGrid.GridControl();
            this.grdMensajeView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.buActualizarMensajes = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdMensaje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMensajeView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.buCancelar);
            this.panelControl2.Controls.Add(this.panelControl3);
            this.panelControl2.Controls.Add(this.buActualizarMensajes);
            this.panelControl2.Controls.Add(this.buAceptar);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 454);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl2.Size = new System.Drawing.Size(1035, 50);
            this.panelControl2.TabIndex = 5;
            // 
            // buCancelar
            // 
            this.buCancelar.Dock = System.Windows.Forms.DockStyle.Left;
            this.buCancelar.Location = new System.Drawing.Point(145, 12);
            this.buCancelar.Name = "buCancelar";
            this.buCancelar.Size = new System.Drawing.Size(119, 26);
            this.buCancelar.TabIndex = 1;
            this.buCancelar.Text = "Cancelar";
            // 
            // buAceptar
            // 
            this.buAceptar.Dock = System.Windows.Forms.DockStyle.Left;
            this.buAceptar.Location = new System.Drawing.Point(12, 12);
            this.buAceptar.Name = "buAceptar";
            this.buAceptar.Size = new System.Drawing.Size(119, 26);
            this.buAceptar.TabIndex = 0;
            this.buAceptar.Text = "Aceptar";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grdMensaje);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl1.Size = new System.Drawing.Size(1035, 454);
            this.panelControl1.TabIndex = 6;
            // 
            // grdMensaje
            // 
            this.grdMensaje.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdMensaje.Location = new System.Drawing.Point(12, 12);
            this.grdMensaje.MainView = this.grdMensajeView;
            this.grdMensaje.Name = "grdMensaje";
            this.grdMensaje.Size = new System.Drawing.Size(1011, 430);
            this.grdMensaje.TabIndex = 0;
            this.grdMensaje.UseEmbeddedNavigator = true;
            this.grdMensaje.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdMensajeView});
            // 
            // grdMensajeView
            // 
            this.grdMensajeView.GridControl = this.grdMensaje;
            this.grdMensajeView.Name = "grdMensajeView";
            this.grdMensajeView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.grdMensajeView.OptionsView.ShowAutoFilterRow = true;
            // 
            // buActualizarMensajes
            // 
            this.buActualizarMensajes.Dock = System.Windows.Forms.DockStyle.Right;
            this.buActualizarMensajes.Location = new System.Drawing.Point(904, 12);
            this.buActualizarMensajes.Name = "buActualizarMensajes";
            this.buActualizarMensajes.Size = new System.Drawing.Size(119, 26);
            this.buActualizarMensajes.TabIndex = 2;
            this.buActualizarMensajes.Text = "Actualizar Mensajes";
            this.buActualizarMensajes.Click += new System.EventHandler(this.buActualizarMensajes_Click);
            // 
            // panelControl3
            // 
            this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl3.Location = new System.Drawing.Point(131, 12);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(14, 26);
            this.panelControl3.TabIndex = 6;
            // 
            // fmMessageSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1035, 504);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.panelControl2);
            this.Name = "fmMessageSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selector de Mensajes";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdMensaje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMensajeView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl grdMensaje;
        private DevExpress.XtraGrid.Views.Grid.GridView grdMensajeView;
        private DevExpress.XtraEditors.SimpleButton buActualizarMensajes;
        private DevExpress.XtraEditors.PanelControl panelControl3;
    }
}