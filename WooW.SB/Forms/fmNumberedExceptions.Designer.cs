namespace WooW.SB.Forms
{
    partial class fmNumberedExceptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmNumberedExceptions));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buAceptarCambios = new DevExpress.XtraBars.BarButtonItem();
            buDescartarCambios = new DevExpress.XtraBars.BarButtonItem();
            txtIdioma = new DevExpress.XtraBars.BarEditItem();
            repIdioma = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            buRefrescar = new DevExpress.XtraBars.BarButtonItem();
            buEditar = new DevExpress.XtraBars.BarButtonItem();
            buTraducir = new DevExpress.XtraBars.BarButtonItem();
            txtElementos = new DevExpress.XtraBars.BarEditItem();
            repElementos = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            optRenglonesBlanco = new DevExpress.XtraBars.BarEditItem();
            repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            optSinTraducir = new DevExpress.XtraBars.BarEditItem();
            repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            buCopiarCodigoMuestra = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            grpTranslate = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            grdMensaje = new DevExpress.XtraGrid.GridControl();
            grdMensajeView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repIdioma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repElementos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdMensaje).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdMensajeView).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(60, 58, 60, 58);
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, ribbonControl1.SearchEditItem, buAceptarCambios, buDescartarCambios, txtIdioma, buRefrescar, buEditar, buTraducir, txtElementos, optRenglonesBlanco, optSinTraducir, buCopiarCodigoMuestra });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.Margin = new System.Windows.Forms.Padding(6);
            ribbonControl1.MaxItemId = 13;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.OptionsMenuMinWidth = 660;
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repIdioma, repElementos, repositoryItemCheckEdit1, repositoryItemCheckEdit2 });
            ribbonControl1.Size = new System.Drawing.Size(1852, 292);
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
            buDescartarCambios.ItemClick += buDescartarCambios_ItemClick;
            // 
            // txtIdioma
            // 
            txtIdioma.Caption = "Idioma";
            txtIdioma.Edit = repIdioma;
            txtIdioma.EditValue = "es-MX";
            txtIdioma.EditWidth = 120;
            txtIdioma.Id = 4;
            txtIdioma.Name = "txtIdioma";
            txtIdioma.EditValueChanged += txtIdioma_EditValueChanged;
            // 
            // repIdioma
            // 
            repIdioma.AutoHeight = false;
            repIdioma.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repIdioma.Name = "repIdioma";
            repIdioma.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // buRefrescar
            // 
            buRefrescar.Caption = "Refrescar";
            buRefrescar.Id = 5;
            buRefrescar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.Image");
            buRefrescar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.LargeImage");
            buRefrescar.Name = "buRefrescar";
            buRefrescar.ItemClick += buRefrescar_ItemClick;
            // 
            // buEditar
            // 
            buEditar.Caption = "Editar";
            buEditar.Id = 7;
            buEditar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buEditar.ImageOptions.Image");
            buEditar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buEditar.ImageOptions.LargeImage");
            buEditar.Name = "buEditar";
            buEditar.ItemClick += buEditar_ItemClick;
            // 
            // buTraducir
            // 
            buTraducir.Caption = "Traducir";
            buTraducir.Id = 8;
            buTraducir.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buTraducir.ImageOptions.Image");
            buTraducir.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buTraducir.ImageOptions.LargeImage");
            buTraducir.Name = "buTraducir";
            buTraducir.ItemClick += buTraducir_ItemClick;
            // 
            // txtElementos
            // 
            txtElementos.Caption = "Elementos";
            txtElementos.Edit = repElementos;
            txtElementos.EditWidth = 150;
            txtElementos.Id = 9;
            txtElementos.Name = "txtElementos";
            // 
            // repElementos
            // 
            repElementos.AutoHeight = false;
            repElementos.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repElementos.Name = "repElementos";
            repElementos.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // optRenglonesBlanco
            // 
            optRenglonesBlanco.Caption = "Traducir solo renglones en blanco";
            optRenglonesBlanco.Edit = repositoryItemCheckEdit1;
            optRenglonesBlanco.EditValue = true;
            optRenglonesBlanco.Id = 10;
            optRenglonesBlanco.Name = "optRenglonesBlanco";
            // 
            // repositoryItemCheckEdit1
            // 
            repositoryItemCheckEdit1.AutoHeight = false;
            repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // optSinTraducir
            // 
            optSinTraducir.Caption = "Mostrar Solo sin Traducir";
            optSinTraducir.Edit = repositoryItemCheckEdit2;
            optSinTraducir.EditValue = true;
            optSinTraducir.Id = 11;
            optSinTraducir.Name = "optSinTraducir";
            optSinTraducir.EditValueChanged += optSinTraducir_EditValueChanged;
            // 
            // repositoryItemCheckEdit2
            // 
            repositoryItemCheckEdit2.AutoHeight = false;
            repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // buCopiarCodigoMuestra
            // 
            buCopiarCodigoMuestra.Caption = "Copiar Código Muestra";
            buCopiarCodigoMuestra.Id = 12;
            buCopiarCodigoMuestra.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buCopiarCodigoMuestra.ImageOptions.Image");
            buCopiarCodigoMuestra.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buCopiarCodigoMuestra.ImageOptions.LargeImage");
            buCopiarCodigoMuestra.Name = "buCopiarCodigoMuestra";
            buCopiarCodigoMuestra.ItemClick += buCopiarCodigoMuestra_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, grpTranslate });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Mensajes de Error";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(buEditar);
            ribbonPageGroup1.ItemLinks.Add(buAceptarCambios);
            ribbonPageGroup1.ItemLinks.Add(buDescartarCambios, true);
            ribbonPageGroup1.ItemLinks.Add(buRefrescar, true);
            ribbonPageGroup1.ItemLinks.Add(buCopiarCodigoMuestra, true);
            ribbonPageGroup1.ItemLinks.Add(txtIdioma, true);
            ribbonPageGroup1.ItemLinks.Add(optSinTraducir);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Cambios";
            // 
            // grpTranslate
            // 
            grpTranslate.Enabled = false;
            grpTranslate.ItemLinks.Add(buTraducir);
            grpTranslate.ItemLinks.Add(txtElementos);
            grpTranslate.ItemLinks.Add(optRenglonesBlanco);
            grpTranslate.Name = "grpTranslate";
            grpTranslate.Text = "Traducir con Google";
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(panelControl2);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 292);
            panelControl1.Margin = new System.Windows.Forms.Padding(6);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1852, 862);
            panelControl1.TabIndex = 2;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(grdMensaje);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(3, 3);
            panelControl2.Margin = new System.Windows.Forms.Padding(6);
            panelControl2.Name = "panelControl2";
            panelControl2.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            panelControl2.Size = new System.Drawing.Size(1846, 856);
            panelControl2.TabIndex = 2;
            // 
            // grdMensaje
            // 
            grdMensaje.Dock = System.Windows.Forms.DockStyle.Fill;
            grdMensaje.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            grdMensaje.Location = new System.Drawing.Point(23, 22);
            grdMensaje.MainView = grdMensajeView;
            grdMensaje.Margin = new System.Windows.Forms.Padding(6);
            grdMensaje.MenuManager = ribbonControl1;
            grdMensaje.Name = "grdMensaje";
            grdMensaje.Size = new System.Drawing.Size(1800, 812);
            grdMensaje.TabIndex = 0;
            grdMensaje.UseEmbeddedNavigator = true;
            grdMensaje.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdMensajeView });
            // 
            // grdMensajeView
            // 
            grdMensajeView.DetailHeight = 673;
            grdMensajeView.GridControl = grdMensaje;
            grdMensajeView.LevelIndent = 0;
            grdMensajeView.Name = "grdMensajeView";
            grdMensajeView.OptionsBehavior.Editable = false;
            grdMensajeView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            grdMensajeView.OptionsView.ShowAutoFilterRow = true;
            grdMensajeView.OptionsView.ShowGroupPanel = false;
            grdMensajeView.PreviewIndent = 0;
            grdMensajeView.InitNewRow += grdMensajeView_InitNewRow;
            grdMensajeView.ValidateRow += grdMensajeView_ValidateRow;
            // 
            // fmNumberedExceptions
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Controls.Add(ribbonControl1);
            Margin = new System.Windows.Forms.Padding(6);
            Name = "fmNumberedExceptions";
            Size = new System.Drawing.Size(1852, 1154);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repIdioma).EndInit();
            ((System.ComponentModel.ISupportInitialize)repElementos).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdMensaje).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdMensajeView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.BarButtonItem buAceptarCambios;
        private DevExpress.XtraBars.BarButtonItem buDescartarCambios;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.GridControl grdMensaje;
        private DevExpress.XtraGrid.Views.Grid.GridView grdMensajeView;
        private DevExpress.XtraBars.BarEditItem txtIdioma;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repIdioma;
        private DevExpress.XtraBars.BarButtonItem buRefrescar;
        private DevExpress.XtraBars.BarButtonItem buEditar;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup grpTranslate;
        private DevExpress.XtraBars.BarButtonItem buTraducir;
        private DevExpress.XtraBars.BarEditItem txtElementos;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repElementos;
        private DevExpress.XtraBars.BarEditItem optRenglonesBlanco;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraBars.BarEditItem optSinTraducir;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraBars.BarButtonItem buCopiarCodigoMuestra;
    }
}
