namespace WooW.SB.Forms
{
    partial class fmModelMaker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmModelMaker));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buGenerar = new DevExpress.XtraBars.BarButtonItem();
            buGenerarYCompilar = new DevExpress.XtraBars.BarButtonItem();
            buFormateaTodo = new DevExpress.XtraBars.BarButtonItem();
            buVisualStudio = new DevExpress.XtraBars.BarSubItem();
            buCopiaPlantilla = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            txtLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, buGenerar, buGenerarYCompilar, buFormateaTodo, buVisualStudio, buCopiaPlantilla });
            ribbonControl1.Location = new System.Drawing.Point(10, 10);
            ribbonControl1.MaxItemId = 17;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.Size = new System.Drawing.Size(937, 150);
            // 
            // buGenerar
            // 
            buGenerar.Caption = "Generar Servicio";
            buGenerar.Id = 1;
            buGenerar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buGenerar.ImageOptions.Image");
            buGenerar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buGenerar.ImageOptions.LargeImage");
            buGenerar.Name = "buGenerar";
            buGenerar.ItemClick += buGenerar_ItemClick;
            // 
            // buGenerarYCompilar
            // 
            buGenerarYCompilar.Caption = "Generar Servicio y Compilar";
            buGenerarYCompilar.Id = 2;
            buGenerarYCompilar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buGenerarYCompilar.ImageOptions.Image");
            buGenerarYCompilar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buGenerarYCompilar.ImageOptions.LargeImage");
            buGenerarYCompilar.Name = "buGenerarYCompilar";
            buGenerarYCompilar.ItemClick += buGenerar_ItemClick;
            // 
            // buFormateaTodo
            // 
            buFormateaTodo.Caption = "Formatea Todos los Scripts";
            buFormateaTodo.Id = 8;
            buFormateaTodo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buFormateaTodo.ImageOptions.Image");
            buFormateaTodo.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buFormateaTodo.ImageOptions.LargeImage");
            buFormateaTodo.Name = "buFormateaTodo";
            buFormateaTodo.ItemClick += buFormateaTodo_ItemClick;
            // 
            // buVisualStudio
            // 
            buVisualStudio.Caption = "Revisar en Visual Studio";
            buVisualStudio.Id = 11;
            buVisualStudio.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buVisualStudio.ImageOptions.Image");
            buVisualStudio.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buVisualStudio.ImageOptions.LargeImage");
            buVisualStudio.Name = "buVisualStudio";
            // 
            // buCopiaPlantilla
            // 
            buCopiaPlantilla.Caption = "Copiar Plantilla";
            buCopiaPlantilla.Id = 15;
            buCopiaPlantilla.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buCopiaPlantilla.ImageOptions.Image");
            buCopiaPlantilla.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buCopiaPlantilla.ImageOptions.LargeImage");
            buCopiaPlantilla.Name = "buCopiaPlantilla";
            buCopiaPlantilla.ItemClick += buCopiaPlantilla_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Generar Modelos";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(buGenerar);
            ribbonPageGroup1.ItemLinks.Add(buGenerarYCompilar, true);
            ribbonPageGroup1.ItemLinks.Add(buVisualStudio, true);
            ribbonPageGroup1.ItemLinks.Add(buFormateaTodo, true);
            ribbonPageGroup1.ItemLinks.Add(buCopiaPlantilla);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Acciones";
            // 
            // txtLog
            // 
            txtLog.BackColor = System.Drawing.Color.Black;
            txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLog.Font = new System.Drawing.Font("Consolas", 11.25F);
            txtLog.ForeColor = System.Drawing.Color.FromArgb(224, 224, 224);
            txtLog.Location = new System.Drawing.Point(10, 160);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new System.Drawing.Size(937, 464);
            txtLog.TabIndex = 2;
            txtLog.Text = "";
            // 
            // fmModelMaker
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(txtLog);
            Controls.Add(ribbonControl1);
            Name = "fmModelMaker";
            Padding = new System.Windows.Forms.Padding(10);
            Size = new System.Drawing.Size(957, 634);
            Load += fmModelMaker_Load;
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem buGenerar;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private System.Windows.Forms.RichTextBox txtLog;
        private DevExpress.XtraBars.BarButtonItem buGenerarYCompilar;
        private DevExpress.XtraBars.BarButtonItem buFormateaTodo;
        private DevExpress.XtraBars.BarSubItem buVisualStudio;
        private DevExpress.XtraBars.BarButtonItem buCopiaPlantilla;
    }
}
