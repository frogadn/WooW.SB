namespace WooW.SB.Config
{
    partial class fmLabelsSelector
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
            this.grdEtiqueta = new DevExpress.XtraGrid.GridControl();
            this.grdEtiquetaView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.buCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.buAceptar = new DevExpress.XtraEditors.SimpleButton();
            this.buActualizarEtiquetas = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdEtiqueta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdEtiquetaView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grdEtiqueta);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl1.Size = new System.Drawing.Size(1322, 517);
            this.panelControl1.TabIndex = 3;
            // 
            // grdEtiqueta
            // 
            this.grdEtiqueta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdEtiqueta.Location = new System.Drawing.Point(12, 12);
            this.grdEtiqueta.MainView = this.grdEtiquetaView;
            this.grdEtiqueta.Name = "grdEtiqueta";
            this.grdEtiqueta.Size = new System.Drawing.Size(1298, 493);
            this.grdEtiqueta.TabIndex = 0;
            this.grdEtiqueta.UseEmbeddedNavigator = true;
            this.grdEtiqueta.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdEtiquetaView});
            // 
            // grdEtiquetaView
            // 
            this.grdEtiquetaView.GridControl = this.grdEtiqueta;
            this.grdEtiquetaView.Name = "grdEtiquetaView";
            this.grdEtiquetaView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.grdEtiquetaView.OptionsView.ShowAutoFilterRow = true;
            this.grdEtiquetaView.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.grdEtiquetaView_InitNewRow);
            this.grdEtiquetaView.RowDeleted += new DevExpress.Data.RowDeletedEventHandler(this.grdEtiquetaView_RowDeleted);
            this.grdEtiquetaView.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.grdEtiquetaView_ValidateRow);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.buCancelar);
            this.panelControl2.Controls.Add(this.panelControl3);
            this.panelControl2.Controls.Add(this.buActualizarEtiquetas);
            this.panelControl2.Controls.Add(this.buAceptar);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 517);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl2.Size = new System.Drawing.Size(1322, 50);
            this.panelControl2.TabIndex = 4;
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
            // buActualizarEtiquetas
            // 
            this.buActualizarEtiquetas.Dock = System.Windows.Forms.DockStyle.Right;
            this.buActualizarEtiquetas.Location = new System.Drawing.Point(1191, 12);
            this.buActualizarEtiquetas.Name = "buActualizarEtiquetas";
            this.buActualizarEtiquetas.Size = new System.Drawing.Size(119, 26);
            this.buActualizarEtiquetas.TabIndex = 2;
            this.buActualizarEtiquetas.Text = "Actualizar Etiquetas";
            this.buActualizarEtiquetas.Click += new System.EventHandler(this.buActualizarEtiquetas_Click);
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
            // fmLabelsSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1322, 567);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.panelControl2);
            this.MinimizeBox = false;
            this.Name = "fmLabelsSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Etiquetas";
            this.Load += new System.EventHandler(this.EtiquetaSimple_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdEtiqueta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdEtiquetaView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl grdEtiqueta;
        private DevExpress.XtraGrid.Views.Grid.GridView grdEtiquetaView;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.SimpleButton buActualizarEtiquetas;
        private DevExpress.XtraEditors.PanelControl panelControl3;
    }
}