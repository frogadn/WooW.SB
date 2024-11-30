namespace WooW.SB.Config.Editors
{
    partial class fmIconoSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmIconoSelector));
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            grdIcon = new DevExpress.XtraGrid.GridControl();
            grdIconoView = new DevExpress.XtraGrid.Views.Grid.GridView();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            btnDeleteIcon = new DevExpress.XtraEditors.SimpleButton();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            panelControl3 = new DevExpress.XtraEditors.PanelControl();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdIconoView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl3).BeginInit();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(grdIcon);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 0);
            panelControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(12);
            panelControl1.Size = new System.Drawing.Size(628, 596);
            panelControl1.TabIndex = 3;
            // 
            // grdIcon
            // 
            grdIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            grdIcon.EmbeddedNavigator.Buttons.Append.Enabled = false;
            grdIcon.EmbeddedNavigator.Buttons.CancelEdit.Enabled = false;
            grdIcon.EmbeddedNavigator.Buttons.Edit.Enabled = false;
            grdIcon.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
            grdIcon.EmbeddedNavigator.Buttons.Remove.Enabled = false;
            grdIcon.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grdIcon.Location = new System.Drawing.Point(14, 14);
            grdIcon.MainView = grdIconoView;
            grdIcon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grdIcon.Name = "grdIcon";
            grdIcon.Size = new System.Drawing.Size(600, 568);
            grdIcon.TabIndex = 0;
            grdIcon.UseEmbeddedNavigator = true;
            grdIcon.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdIconoView });
            // 
            // grdIconoView
            // 
            grdIconoView.DetailHeight = 404;
            grdIconoView.GridControl = grdIcon;
            grdIconoView.Name = "grdIconoView";
            grdIconoView.OptionsBehavior.ReadOnly = true;
            grdIconoView.OptionsEditForm.PopupEditFormWidth = 933;
            grdIconoView.OptionsView.ShowAutoFilterRow = true;
            grdIconoView.OptionsView.ShowColumnHeaders = false;
            grdIconoView.OptionsView.ShowGroupPanel = false;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(btnDeleteIcon);
            panelControl2.Controls.Add(buCancelar);
            panelControl2.Controls.Add(panelControl3);
            panelControl2.Controls.Add(buAceptar);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl2.Location = new System.Drawing.Point(0, 596);
            panelControl2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelControl2.Name = "panelControl2";
            panelControl2.Padding = new System.Windows.Forms.Padding(12);
            panelControl2.Size = new System.Drawing.Size(628, 58);
            panelControl2.TabIndex = 4;
            // 
            // btnDeleteIcon
            // 
            btnDeleteIcon.Dock = System.Windows.Forms.DockStyle.Left;
            btnDeleteIcon.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            btnDeleteIcon.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnDeleteIcon.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnDeleteIcon.ImageOptions.SvgImage");
            btnDeleteIcon.Location = new System.Drawing.Point(308, 14);
            btnDeleteIcon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDeleteIcon.Name = "btnDeleteIcon";
            btnDeleteIcon.Size = new System.Drawing.Size(50, 30);
            btnDeleteIcon.TabIndex = 8;
            btnDeleteIcon.Click += btnDeleteIcon_Click;
            // 
            // buCancelar
            // 
            buCancelar.Dock = System.Windows.Forms.DockStyle.Left;
            buCancelar.Location = new System.Drawing.Point(169, 14);
            buCancelar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(139, 30);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // panelControl3
            // 
            panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panelControl3.Dock = System.Windows.Forms.DockStyle.Left;
            panelControl3.Location = new System.Drawing.Point(153, 14);
            panelControl3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelControl3.Name = "panelControl3";
            panelControl3.Size = new System.Drawing.Size(16, 30);
            panelControl3.TabIndex = 7;
            // 
            // buAceptar
            // 
            buAceptar.Dock = System.Windows.Forms.DockStyle.Left;
            buAceptar.Location = new System.Drawing.Point(14, 14);
            buAceptar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(139, 30);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // fmIconoSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(628, 654);
            Controls.Add(panelControl1);
            Controls.Add(panelControl2);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimizeBox = false;
            Name = "fmIconoSelector";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Iconos";
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdIconoView).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl grdIcon;
        private DevExpress.XtraGrid.Views.Grid.GridView grdIconoView;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.SimpleButton btnDeleteIcon;
    }
}