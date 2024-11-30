namespace WooW.SB.Forms
{
    partial class fmProyectProperties
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmProyectProperties));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buAceptarCambios = new DevExpress.XtraBars.BarButtonItem();
            buDescartarCambios = new DevExpress.XtraBars.BarButtonItem();
            buEditar = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            propertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            propertyDescription = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
            groupControl1 = new DevExpress.XtraEditors.GroupControl();
            mstHtmlEditor1 = new BaiqiSoft.HtmlEditorControl.MstHtmlEditor();
            errorProvider1 = new System.Windows.Forms.ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)propertyGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)groupControl1).BeginInit();
            groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, buAceptarCambios, buDescartarCambios, buEditar });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.MaxItemId = 6;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.Size = new System.Drawing.Size(1000, 150);
            // 
            // buAceptarCambios
            // 
            buAceptarCambios.Caption = "&Aceptar";
            buAceptarCambios.Enabled = false;
            buAceptarCambios.Id = 1;
            buAceptarCambios.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buAceptarCambios.ImageOptions.Image");
            buAceptarCambios.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buAceptarCambios.ImageOptions.LargeImage");
            buAceptarCambios.Name = "buAceptarCambios";
            buAceptarCambios.ItemClick += buAceptarCambios_ItemClick;
            // 
            // buDescartarCambios
            // 
            buDescartarCambios.Caption = "&Descartar";
            buDescartarCambios.Enabled = false;
            buDescartarCambios.Id = 2;
            buDescartarCambios.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buDescartarCambios.ImageOptions.Image");
            buDescartarCambios.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buDescartarCambios.ImageOptions.LargeImage");
            buDescartarCambios.Name = "buDescartarCambios";
            buDescartarCambios.ItemClick += buDesartarCambios_ItemClick;
            // 
            // buEditar
            // 
            buEditar.Caption = "Editar";
            buEditar.Id = 4;
            buEditar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buEditar.ImageOptions.Image");
            buEditar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buEditar.ImageOptions.LargeImage");
            buEditar.Name = "buEditar";
            buEditar.ItemClick += buEditar_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Propiedades del Proyecto";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(buEditar);
            ribbonPageGroup1.ItemLinks.Add(buAceptarCambios, true);
            ribbonPageGroup1.ItemLinks.Add(buDescartarCambios, true);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Cambios";
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(splitContainerControl1);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 150);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(10);
            panelControl1.Size = new System.Drawing.Size(1000, 463);
            panelControl1.TabIndex = 6;
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Location = new System.Drawing.Point(12, 12);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(propertyGrid);
            splitContainerControl1.Panel1.Controls.Add(propertyDescription);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(groupControl1);
            splitContainerControl1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitContainerControl1.Size = new System.Drawing.Size(976, 439);
            splitContainerControl1.SplitterPosition = 481;
            splitContainerControl1.TabIndex = 9;
            // 
            // propertyGrid
            // 
            propertyGrid.ActiveViewType = DevExpress.XtraVerticalGrid.PropertyGridView.Office;
            propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGrid.Enabled = false;
            propertyGrid.Location = new System.Drawing.Point(0, 0);
            propertyGrid.MenuManager = ribbonControl1;
            propertyGrid.Name = "propertyGrid";
            propertyGrid.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            propertyGrid.Size = new System.Drawing.Size(481, 302);
            propertyGrid.TabIndex = 7;
            propertyGrid.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.True;
            // 
            // propertyDescription
            // 
            propertyDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            propertyDescription.Location = new System.Drawing.Point(0, 302);
            propertyDescription.Name = "propertyDescription";
            propertyDescription.PropertyGrid = propertyGrid;
            propertyDescription.Size = new System.Drawing.Size(481, 137);
            propertyDescription.TabIndex = 8;
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(mstHtmlEditor1);
            groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupControl1.Location = new System.Drawing.Point(5, 5);
            groupControl1.Margin = new System.Windows.Forms.Padding(2);
            groupControl1.Name = "groupControl1";
            groupControl1.Padding = new System.Windows.Forms.Padding(5);
            groupControl1.Size = new System.Drawing.Size(475, 429);
            groupControl1.TabIndex = 1;
            groupControl1.Text = "Historial de Versiones";
            // 
            // mstHtmlEditor1
            // 
            mstHtmlEditor1.DataSource = null;
            mstHtmlEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            mstHtmlEditor1.Enabled = false;
            mstHtmlEditor1.Location = new System.Drawing.Point(7, 28);
            mstHtmlEditor1.Name = "mstHtmlEditor1";
            mstHtmlEditor1.Size = new System.Drawing.Size(461, 394);
            mstHtmlEditor1.TabIndex = 0;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // fmProyectProperties
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(panelControl1);
            Controls.Add(ribbonControl1);
            Name = "fmProyectProperties";
            Size = new System.Drawing.Size(1000, 613);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)propertyGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)groupControl1).EndInit();
            groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.BarButtonItem buAceptarCambios;
        private DevExpress.XtraBars.BarButtonItem buDescartarCambios;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraVerticalGrid.PropertyDescriptionControl propertyDescription;
        private DevExpress.XtraVerticalGrid.PropertyGridControl propertyGrid;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraBars.BarButtonItem buEditar;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private BaiqiSoft.HtmlEditorControl.MstHtmlEditor mstHtmlEditor1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
