namespace WooW.SB.Config.Editors
{
    partial class woSyntaxEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(woSyntaxEditor));
            syeCode = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            btnComent = new DevExpress.XtraBars.BarButtonItem();
            btnUncomment = new DevExpress.XtraBars.BarButtonItem();
            btnFormatCode = new DevExpress.XtraBars.BarButtonItem();
            btnExpandRegions = new DevExpress.XtraBars.BarButtonItem();
            btnCollapseRegions = new DevExpress.XtraBars.BarButtonItem();
            btnCompile = new DevExpress.XtraBars.BarButtonItem();
            btnOpenFileVisualCode = new DevExpress.XtraBars.BarButtonItem();
            btnSave = new DevExpress.XtraBars.BarButtonItem();
            btnIncreaseIdent = new DevExpress.XtraBars.BarButtonItem();
            btnDecreaseIdent = new DevExpress.XtraBars.BarButtonItem();
            rbpSyntaxEditor = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            pnlBody = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlBody).BeginInit();
            pnlBody.SuspendLayout();
            SuspendLayout();
            // 
            // syeCode
            // 
            syeCode.AllowDrop = true;
            syeCode.Dock = System.Windows.Forms.DockStyle.Fill;
            syeCode.IsIndicatorMarginVisible = true;
            syeCode.IsLineNumberMarginVisible = true;
            syeCode.Location = new System.Drawing.Point(3, 3);
            syeCode.Name = "syeCode";
            syeCode.Size = new System.Drawing.Size(1612, 836);
            syeCode.TabIndex = 0;
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, btnComent, btnUncomment, btnFormatCode, btnExpandRegions, btnCollapseRegions, btnCompile, btnOpenFileVisualCode, btnSave, btnIncreaseIdent, btnDecreaseIdent });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.MaxItemId = 14;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { rbpSyntaxEditor });
            ribbonControl1.Size = new System.Drawing.Size(1618, 292);
            // 
            // btnComent
            // 
            btnComent.Caption = "Comentar";
            btnComent.Id = 4;
            btnComent.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnComent.ImageOptions.Image");
            btnComent.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnComent.ImageOptions.LargeImage");
            btnComent.Name = "btnComent";
            btnComent.ItemClick += btnComent_ItemClick;
            // 
            // btnUncomment
            // 
            btnUncomment.Caption = "Des Comentar";
            btnUncomment.Id = 5;
            btnUncomment.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnUncomment.ImageOptions.Image");
            btnUncomment.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnUncomment.ImageOptions.LargeImage");
            btnUncomment.Name = "btnUncomment";
            btnUncomment.ItemClick += btnUncomment_ItemClick;
            // 
            // btnFormatCode
            // 
            btnFormatCode.Caption = "Formatear";
            btnFormatCode.Id = 6;
            btnFormatCode.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnFormatCode.ImageOptions.Image");
            btnFormatCode.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnFormatCode.ImageOptions.LargeImage");
            btnFormatCode.Name = "btnFormatCode";
            btnFormatCode.ItemClick += btnFormatCode_ItemClick;
            // 
            // btnExpandRegions
            // 
            btnExpandRegions.Caption = "Expandir Regiones";
            btnExpandRegions.Id = 7;
            btnExpandRegions.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnExpandRegions.ImageOptions.Image");
            btnExpandRegions.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnExpandRegions.ImageOptions.LargeImage");
            btnExpandRegions.Name = "btnExpandRegions";
            btnExpandRegions.ItemClick += btnExpandRegions_ItemClick;
            // 
            // btnCollapseRegions
            // 
            btnCollapseRegions.Caption = "Colapsar Regiones";
            btnCollapseRegions.Id = 8;
            btnCollapseRegions.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCollapseRegions.ImageOptions.Image");
            btnCollapseRegions.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnCollapseRegions.ImageOptions.LargeImage");
            btnCollapseRegions.Name = "btnCollapseRegions";
            btnCollapseRegions.ItemClick += btnCollapseRegions_ItemClick;
            // 
            // btnCompile
            // 
            btnCompile.Caption = "Compilar";
            btnCompile.Id = 9;
            btnCompile.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCompile.ImageOptions.Image");
            btnCompile.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnCompile.ImageOptions.LargeImage");
            btnCompile.Name = "btnCompile";
            btnCompile.ItemClick += btnCompile_ItemClick;
            // 
            // btnOpenFileVisualCode
            // 
            btnOpenFileVisualCode.Caption = "Abrir Fichero En Visual Code";
            btnOpenFileVisualCode.Id = 10;
            btnOpenFileVisualCode.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOpenFileVisualCode.ImageOptions.Image");
            btnOpenFileVisualCode.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnOpenFileVisualCode.ImageOptions.LargeImage");
            btnOpenFileVisualCode.Name = "btnOpenFileVisualCode";
            btnOpenFileVisualCode.ItemClick += btnOpenFileVisualCode_ItemClick;
            // 
            // btnSave
            // 
            btnSave.Caption = "Guardar";
            btnSave.Id = 11;
            btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
            btnSave.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.LargeImage");
            btnSave.Name = "btnSave";
            btnSave.ItemClick += btnSave_ItemClick;
            // 
            // btnIncreaseIdent
            // 
            btnIncreaseIdent.Caption = "Incrementar Identado";
            btnIncreaseIdent.Id = 12;
            btnIncreaseIdent.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnIncreaseIdent.ImageOptions.Image");
            btnIncreaseIdent.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnIncreaseIdent.ImageOptions.LargeImage");
            btnIncreaseIdent.Name = "btnIncreaseIdent";
            btnIncreaseIdent.ItemClick += btnIncreaseIdent_ItemClick;
            // 
            // btnDecreaseIdent
            // 
            btnDecreaseIdent.Caption = "Reducir Identado";
            btnDecreaseIdent.Id = 13;
            btnDecreaseIdent.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnDecreaseIdent.ImageOptions.Image");
            btnDecreaseIdent.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnDecreaseIdent.ImageOptions.LargeImage");
            btnDecreaseIdent.Name = "btnDecreaseIdent";
            btnDecreaseIdent.ItemClick += btnDecreaseIdent_ItemClick;
            // 
            // rbpSyntaxEditor
            // 
            rbpSyntaxEditor.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2, ribbonPageGroup1 });
            rbpSyntaxEditor.Name = "rbpSyntaxEditor";
            rbpSyntaxEditor.Text = "Editor De Código";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(btnSave);
            ribbonPageGroup2.ItemLinks.Add(btnCompile);
            ribbonPageGroup2.ItemLinks.Add(btnOpenFileVisualCode);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = "Código";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(btnFormatCode);
            ribbonPageGroup1.ItemLinks.Add(btnDecreaseIdent);
            ribbonPageGroup1.ItemLinks.Add(btnIncreaseIdent);
            ribbonPageGroup1.ItemLinks.Add(btnComent);
            ribbonPageGroup1.ItemLinks.Add(btnUncomment);
            ribbonPageGroup1.ItemLinks.Add(btnExpandRegions);
            ribbonPageGroup1.ItemLinks.Add(btnCollapseRegions);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Editor";
            // 
            // pnlBody
            // 
            pnlBody.Controls.Add(syeCode);
            pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlBody.Location = new System.Drawing.Point(0, 292);
            pnlBody.Name = "pnlBody";
            pnlBody.Size = new System.Drawing.Size(1618, 842);
            pnlBody.TabIndex = 3;
            // 
            // woSyntaxEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(pnlBody);
            Controls.Add(ribbonControl1);
            Name = "woSyntaxEditor";
            Size = new System.Drawing.Size(1618, 1134);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlBody).EndInit();
            pnlBody.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor syeCode;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage rbpSyntaxEditor;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.PanelControl pnlBody;
        private DevExpress.XtraBars.BarButtonItem btnComent;
        private DevExpress.XtraBars.BarButtonItem btnUncomment;
        private DevExpress.XtraBars.BarButtonItem btnFormatCode;
        private DevExpress.XtraBars.BarButtonItem btnExpandRegions;
        private DevExpress.XtraBars.BarButtonItem btnCollapseRegions;
        private DevExpress.XtraBars.BarButtonItem btnCompile;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem btnOpenFileVisualCode;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraBars.BarButtonItem btnIncreaseIdent;
        private DevExpress.XtraBars.BarButtonItem btnDecreaseIdent;
    }
}
