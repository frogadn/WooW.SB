namespace WooW.SB.Dialogs
{
    partial class fmNumberedExceptionsSelector
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
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            grdMensaje = new DevExpress.XtraGrid.GridControl();
            grdMensajeView = new DevExpress.XtraGrid.Views.Grid.GridView();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buSeleccionar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdMensaje).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdMensajeView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(grdMensaje);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(6);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            panelControl1.Size = new System.Drawing.Size(1996, 1015);
            panelControl1.TabIndex = 0;
            // 
            // grdMensaje
            // 
            grdMensaje.Dock = System.Windows.Forms.DockStyle.Fill;
            grdMensaje.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            grdMensaje.Location = new System.Drawing.Point(23, 22);
            grdMensaje.MainView = grdMensajeView;
            grdMensaje.Margin = new System.Windows.Forms.Padding(6);
            grdMensaje.Name = "grdMensaje";
            grdMensaje.Size = new System.Drawing.Size(1950, 971);
            grdMensaje.TabIndex = 1;
            grdMensaje.UseEmbeddedNavigator = true;
            grdMensaje.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdMensajeView });
            // 
            // grdMensajeView
            // 
            grdMensajeView.DetailHeight = 673;
            grdMensajeView.GridControl = grdMensaje;
            grdMensajeView.Name = "grdMensajeView";
            grdMensajeView.OptionsBehavior.Editable = false;
            grdMensajeView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            grdMensajeView.OptionsView.ShowAutoFilterRow = true;
            grdMensajeView.OptionsView.ShowGroupPanel = false;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(buCancelar);
            panelControl2.Controls.Add(buSeleccionar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl2.Location = new System.Drawing.Point(0, 1015);
            panelControl2.Margin = new System.Windows.Forms.Padding(6);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(1996, 77);
            panelControl2.TabIndex = 1;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(376, 17);
            buCancelar.Margin = new System.Windows.Forms.Padding(6);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(321, 44);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buSeleccionar
            // 
            buSeleccionar.Location = new System.Drawing.Point(40, 17);
            buSeleccionar.Margin = new System.Windows.Forms.Padding(6);
            buSeleccionar.Name = "buSeleccionar";
            buSeleccionar.Size = new System.Drawing.Size(303, 44);
            buSeleccionar.TabIndex = 0;
            buSeleccionar.Text = "Seleccionar";
            buSeleccionar.Click += buSeleccionar_Click;
            // 
            // fmNumberedExceptionsSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1996, 1092);
            Controls.Add(panelControl1);
            Controls.Add(panelControl2);
            Margin = new System.Windows.Forms.Padding(6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "fmNumberedExceptionsSelector";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Selector de Excepciones Numeradas";
            Load += fmErrrorExceptionSelector_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdMensaje).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdMensajeView).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buSeleccionar;
        private DevExpress.XtraGrid.GridControl grdMensaje;
        private DevExpress.XtraGrid.Views.Grid.GridView grdMensajeView;
    }
}