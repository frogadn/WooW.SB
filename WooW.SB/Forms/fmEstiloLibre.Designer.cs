﻿namespace WooW.SB.Forms
{
    partial class fmEstiloLibre
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.buAceptarCambios = new DevExpress.XtraBars.BarButtonItem();
            this.buDescartarCambios = new DevExpress.XtraBars.BarButtonItem();
            this.buRefrescar = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(60, 58, 60, 58);
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.buAceptarCambios,
            this.buDescartarCambios,
            this.buRefrescar});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ribbonControl1.MaxItemId = 4;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.OptionsMenuMinWidth = 660;
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1120, 292);
            // 
            // buAceptarCambios
            // 
            this.buAceptarCambios.Caption = "&Aceptar";
            this.buAceptarCambios.Enabled = false;
            this.buAceptarCambios.Id = 1;
            this.buAceptarCambios.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Code_Snippet_Save;
            this.buAceptarCambios.Name = "buAceptarCambios";
            // 
            // buDescartarCambios
            // 
            this.buDescartarCambios.Caption = "&Descartar";
            this.buDescartarCambios.Enabled = false;
            this.buDescartarCambios.Id = 2;
            this.buDescartarCambios.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Code_Snippet_Delete;
            this.buDescartarCambios.Name = "buDescartarCambios";
            // 
            // buRefrescar
            // 
            this.buRefrescar.Caption = "Refrescar";
            this.buRefrescar.Id = 3;
            this.buRefrescar.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Refresh_All;
            this.buRefrescar.Name = "buRefrescar";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Roles";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.buRefrescar);
            this.ribbonPageGroup1.ItemLinks.Add(this.buAceptarCambios, true);
            this.ribbonPageGroup1.ItemLinks.Add(this.buDescartarCambios, true);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Cambios";
            // 
            // panelControl1
            // 
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 292);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1120, 627);
            this.panelControl1.TabIndex = 2;
            // 
            // fmEstiloLibre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonControl1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "fmEstiloLibre";
            this.Size = new System.Drawing.Size(1120, 919);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem buAceptarCambios;
        private DevExpress.XtraBars.BarButtonItem buDescartarCambios;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.BarButtonItem buRefrescar;
    }
}
