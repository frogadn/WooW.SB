namespace WooW.SB.Dialogs
{
    partial class fmSelectExtension
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
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            grdExtension = new DevExpress.XtraGrid.GridControl();
            grdExtensionView = new DevExpress.XtraGrid.Views.Grid.GridView();
            stackPanel1 = new DevExpress.Utils.Layout.StackPanel();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdExtension).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdExtensionView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)stackPanel1).BeginInit();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(buCancelar);
            panelControl1.Controls.Add(buAceptar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(0, 442);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(785, 44);
            panelControl1.TabIndex = 0;
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
            // panelControl2
            // 
            panelControl2.Controls.Add(grdExtension);
            panelControl2.Controls.Add(stackPanel1);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(0, 0);
            panelControl2.Name = "panelControl2";
            panelControl2.Padding = new System.Windows.Forms.Padding(5);
            panelControl2.Size = new System.Drawing.Size(785, 442);
            panelControl2.TabIndex = 1;
            // 
            // grdExtension
            // 
            grdExtension.Dock = System.Windows.Forms.DockStyle.Fill;
            grdExtension.Location = new System.Drawing.Point(7, 7);
            grdExtension.MainView = grdExtensionView;
            grdExtension.Name = "grdExtension";
            grdExtension.Size = new System.Drawing.Size(771, 428);
            grdExtension.TabIndex = 0;
            grdExtension.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdExtensionView });
            // 
            // grdExtensionView
            // 
            grdExtensionView.GridControl = grdExtension;
            grdExtensionView.Name = "grdExtensionView";
            grdExtensionView.OptionsBehavior.Editable = false;
            grdExtensionView.OptionsView.ShowAutoFilterRow = true;
            grdExtensionView.OptionsView.ShowGroupPanel = false;
            // 
            // stackPanel1
            // 
            stackPanel1.Location = new System.Drawing.Point(316, 368);
            stackPanel1.Name = "stackPanel1";
            stackPanel1.Size = new System.Drawing.Size(8, 8);
            stackPanel1.TabIndex = 1;
            stackPanel1.UseSkinIndents = true;
            // 
            // fmSelectExtension
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(785, 486);
            Controls.Add(panelControl2);
            Controls.Add(panelControl1);
            MinimizeBox = false;
            Name = "fmSelectExtension";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Seleccione una Modelo a Extender";
            Load += fmCasosPruebaPruebaUnitaria_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdExtension).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdExtensionView).EndInit();
            ((System.ComponentModel.ISupportInitialize)stackPanel1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.GridControl grdExtension;
        private DevExpress.XtraGrid.Views.Grid.GridView grdExtensionView;
        private DevExpress.Utils.Layout.StackPanel stackPanel1;
    }
}