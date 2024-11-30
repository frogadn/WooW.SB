namespace WooW.SB.Forms
{
    partial class fmCompareVersion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmCompareVersion));
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            grdLista = new DevExpress.XtraEditors.GroupControl();
            grdComparacion = new DevExpress.XtraGrid.GridControl();
            grdComparacionView = new DevExpress.XtraGrid.Views.Grid.GridView();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buAbrir = new DevExpress.XtraBars.BarButtonItem();
            txtElemento = new DevExpress.XtraBars.BarEditItem();
            repElemento = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            optSinDiferencias = new DevExpress.XtraBars.BarEditItem();
            repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            buCopiar = new DevExpress.XtraBars.BarButtonItem();
            buComparaCodigo = new DevExpress.XtraBars.BarButtonItem();
            buActualizar = new DevExpress.XtraBars.BarButtonItem();
            buComparaModelo = new DevExpress.XtraBars.BarButtonItem();
            buComparaDiagrama = new DevExpress.XtraBars.BarButtonItem();
            buNormalizar = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            lblFuente = new DevExpress.XtraEditors.LabelControl();
            lblDestino = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdLista).BeginInit();
            grdLista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdComparacion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdComparacionView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repElemento).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Location = new System.Drawing.Point(13, 13);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(grdLista);
            splitContainerControl1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitContainerControl1.Size = new System.Drawing.Size(1921, 982);
            splitContainerControl1.SplitterPosition = 1483;
            splitContainerControl1.TabIndex = 3;
            // 
            // grdLista
            // 
            grdLista.Controls.Add(grdComparacion);
            grdLista.Controls.Add(panelControl2);
            grdLista.Dock = System.Windows.Forms.DockStyle.Fill;
            grdLista.Location = new System.Drawing.Point(10, 10);
            grdLista.Name = "grdLista";
            grdLista.Padding = new System.Windows.Forms.Padding(10);
            grdLista.Size = new System.Drawing.Size(1463, 962);
            grdLista.TabIndex = 0;
            grdLista.Text = "Comparación";
            // 
            // grdComparacion
            // 
            grdComparacion.Dock = System.Windows.Forms.DockStyle.Fill;
            grdComparacion.Location = new System.Drawing.Point(13, 157);
            grdComparacion.MainView = grdComparacionView;
            grdComparacion.Name = "grdComparacion";
            grdComparacion.Size = new System.Drawing.Size(1437, 792);
            grdComparacion.TabIndex = 0;
            grdComparacion.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdComparacionView });
            // 
            // grdComparacionView
            // 
            grdComparacionView.GridControl = grdComparacion;
            grdComparacionView.Name = "grdComparacionView";
            grdComparacionView.OptionsBehavior.Editable = false;
            grdComparacionView.OptionsView.ShowAutoFilterRow = true;
            grdComparacionView.CustomDrawCell += grdComparacionView_CustomDrawCell;
            grdComparacionView.FocusedRowChanged += grdComparacionView_FocusedRowChanged;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(splitContainerControl1);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 292);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(10);
            panelControl1.Size = new System.Drawing.Size(1947, 1008);
            panelControl1.TabIndex = 4;
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, buAbrir, txtElemento, optSinDiferencias, buCopiar, buComparaCodigo, buActualizar, buComparaModelo, buComparaDiagrama, buNormalizar });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.MaxItemId = 10;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repElemento, repositoryItemCheckEdit1 });
            ribbonControl1.Size = new System.Drawing.Size(1947, 292);
            // 
            // buAbrir
            // 
            buAbrir.Caption = "Abrir Proyecto";
            buAbrir.Id = 1;
            buAbrir.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buAbrir.ImageOptions.Image");
            buAbrir.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buAbrir.ImageOptions.LargeImage");
            buAbrir.Name = "buAbrir";
            buAbrir.ItemClick += buAbrir_ItemClick;
            // 
            // txtElemento
            // 
            txtElemento.Caption = "Elemento";
            txtElemento.Edit = repElemento;
            txtElemento.EditWidth = 150;
            txtElemento.Enabled = false;
            txtElemento.Id = 2;
            txtElemento.Name = "txtElemento";
            txtElemento.EditValueChanged += txtElemento_EditValueChanged;
            // 
            // repElemento
            // 
            repElemento.AutoHeight = false;
            repElemento.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repElemento.Name = "repElemento";
            repElemento.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // optSinDiferencias
            // 
            optSinDiferencias.Caption = "No mostrar sin diferencias";
            optSinDiferencias.Edit = repositoryItemCheckEdit1;
            optSinDiferencias.EditValue = false;
            optSinDiferencias.Enabled = false;
            optSinDiferencias.Id = 3;
            optSinDiferencias.Name = "optSinDiferencias";
            optSinDiferencias.EditValueChanged += txtElemento_EditValueChanged;
            // 
            // repositoryItemCheckEdit1
            // 
            repositoryItemCheckEdit1.AutoHeight = false;
            repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // buCopiar
            // 
            buCopiar.Caption = "Copiar a Destino";
            buCopiar.Enabled = false;
            buCopiar.Id = 4;
            buCopiar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buCopiar.ImageOptions.Image");
            buCopiar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buCopiar.ImageOptions.LargeImage");
            buCopiar.Name = "buCopiar";
            buCopiar.ItemClick += buCopiar_ItemClick;
            // 
            // buComparaCodigo
            // 
            buComparaCodigo.Caption = "Compara Código";
            buComparaCodigo.Enabled = false;
            buComparaCodigo.Id = 5;
            buComparaCodigo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buComparaCodigo.ImageOptions.Image");
            buComparaCodigo.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buComparaCodigo.ImageOptions.LargeImage");
            buComparaCodigo.Name = "buComparaCodigo";
            buComparaCodigo.ItemClick += buComparaCodigo_ItemClick;
            // 
            // buActualizar
            // 
            buActualizar.Caption = "Actualizar";
            buActualizar.Enabled = false;
            buActualizar.Id = 6;
            buActualizar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buActualizar.ImageOptions.Image");
            buActualizar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buActualizar.ImageOptions.LargeImage");
            buActualizar.Name = "buActualizar";
            buActualizar.ItemClick += buActualizar_ItemClick;
            // 
            // buComparaModelo
            // 
            buComparaModelo.Caption = "Compara Propiedades";
            buComparaModelo.Enabled = false;
            buComparaModelo.Id = 7;
            buComparaModelo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buComparaModelo.ImageOptions.Image");
            buComparaModelo.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buComparaModelo.ImageOptions.LargeImage");
            buComparaModelo.Name = "buComparaModelo";
            buComparaModelo.ItemClick += buComparaModelo_ItemClick;
            // 
            // buComparaDiagrama
            // 
            buComparaDiagrama.Caption = "Compara Diagrama de Estados";
            buComparaDiagrama.Enabled = false;
            buComparaDiagrama.Id = 8;
            buComparaDiagrama.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buComparaDiagrama.ImageOptions.Image");
            buComparaDiagrama.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buComparaDiagrama.ImageOptions.LargeImage");
            buComparaDiagrama.Name = "buComparaDiagrama";
            buComparaDiagrama.ItemClick += buComparaDiagrama_ItemClick;
            // 
            // buNormalizar
            // 
            buNormalizar.Caption = "Normaliza Código";
            buNormalizar.Enabled = false;
            buNormalizar.Id = 9;
            buNormalizar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buNormalizar.ImageOptions.Image");
            buNormalizar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buNormalizar.ImageOptions.LargeImage");
            buNormalizar.Name = "buNormalizar";
            buNormalizar.ItemClick += buNormalizar_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Compara Versiones";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(buAbrir);
            ribbonPageGroup1.ItemLinks.Add(txtElemento, true);
            ribbonPageGroup1.ItemLinks.Add(optSinDiferencias);
            ribbonPageGroup1.ItemLinks.Add(buActualizar);
            ribbonPageGroup1.ItemLinks.Add(buCopiar, true);
            ribbonPageGroup1.ItemLinks.Add(buComparaModelo, true);
            ribbonPageGroup1.ItemLinks.Add(buComparaDiagrama);
            ribbonPageGroup1.ItemLinks.Add(buComparaCodigo);
            ribbonPageGroup1.ItemLinks.Add(buNormalizar);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Acciones";
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(lblDestino);
            panelControl2.Controls.Add(lblFuente);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl2.Location = new System.Drawing.Point(13, 55);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(1437, 102);
            panelControl2.TabIndex = 1;
            // 
            // lblFuente
            // 
            lblFuente.Appearance.Font = new System.Drawing.Font("Tahoma", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblFuente.Appearance.Options.UseFont = true;
            lblFuente.Location = new System.Drawing.Point(24, 19);
            lblFuente.Name = "lblFuente";
            lblFuente.Size = new System.Drawing.Size(21, 25);
            lblFuente.TabIndex = 0;
            lblFuente.Text = "...";
            // 
            // lblDestino
            // 
            lblDestino.Appearance.Font = new System.Drawing.Font("Tahoma", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblDestino.Appearance.Options.UseFont = true;
            lblDestino.Location = new System.Drawing.Point(24, 60);
            lblDestino.Name = "lblDestino";
            lblDestino.Size = new System.Drawing.Size(21, 25);
            lblDestino.TabIndex = 1;
            lblDestino.Text = "...";
            // 
            // fmCompareVersion
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Controls.Add(ribbonControl1);
            Margin = new System.Windows.Forms.Padding(6);
            Name = "fmCompareVersion";
            Size = new System.Drawing.Size(1947, 1300);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdLista).EndInit();
            grdLista.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdComparacion).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdComparacionView).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repElemento).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            panelControl2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.BarEditItem txtMostrar;
        private DevExpress.XtraEditors.GroupControl grdLista;
        private DevExpress.XtraGrid.GridControl grdComparacion;
        private DevExpress.XtraGrid.Views.Grid.GridView grdComparacionView;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem buAbrir;
        private DevExpress.XtraBars.BarEditItem txtElemento;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repElemento;
        private DevExpress.XtraBars.BarEditItem optSinDiferencias;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraBars.BarButtonItem buCopiar;
        private DevExpress.XtraBars.BarButtonItem buComparaCodigo;
        private DevExpress.XtraBars.BarButtonItem buActualizar;
        private DevExpress.XtraBars.BarButtonItem buComparaModelo;
        private DevExpress.XtraBars.BarButtonItem buComparaDiagrama;
        private DevExpress.XtraBars.BarButtonItem buNormalizar;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl lblDestino;
        private DevExpress.XtraEditors.LabelControl lblFuente;
    }
}
