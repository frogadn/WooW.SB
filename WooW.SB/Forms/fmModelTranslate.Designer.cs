namespace WooW.SB.Forms
{
    partial class fmModelTranslate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmModelTranslate));
            RibbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buAceptarCambios = new DevExpress.XtraBars.BarButtonItem();
            buDescartarCambios = new DevExpress.XtraBars.BarButtonItem();
            txtIdioma = new DevExpress.XtraBars.BarEditItem();
            repIdioma = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            buEditar = new DevExpress.XtraBars.BarButtonItem();
            buTraducir = new DevExpress.XtraBars.BarButtonItem();
            txtElementos = new DevExpress.XtraBars.BarEditItem();
            repElementos = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            optRenglonesBlanco = new DevExpress.XtraBars.BarEditItem();
            repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            txtSeleccion = new DevExpress.XtraBars.BarEditItem();
            repSeleccion = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            optSinTraducir = new DevExpress.XtraBars.BarEditItem();
            repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            grpTranslate = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            grdEtiqueta = new DevExpress.XtraGrid.GridControl();
            grdEtiquetaView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)RibbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repIdioma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repElementos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repSeleccion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdEtiqueta).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdEtiquetaView).BeginInit();
            SuspendLayout();
            // 
            // RibbonControl1
            // 
            RibbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(60, 58, 60, 58);
            RibbonControl1.ExpandCollapseItem.Id = 0;
            RibbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { RibbonControl1.ExpandCollapseItem, RibbonControl1.SearchEditItem, buAceptarCambios, buDescartarCambios, txtIdioma, buEditar, buTraducir, txtElementos, optRenglonesBlanco, txtSeleccion, optSinTraducir });
            RibbonControl1.Location = new System.Drawing.Point(0, 0);
            RibbonControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            RibbonControl1.MaxItemId = 12;
            RibbonControl1.Name = "RibbonControl1";
            RibbonControl1.OptionsMenuMinWidth = 660;
            RibbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            RibbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repIdioma, repElementos, repositoryItemCheckEdit1, repSeleccion, repositoryItemCheckEdit2 });
            RibbonControl1.Size = new System.Drawing.Size(1946, 292);
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
            txtIdioma.Caption = "      Idioma";
            txtIdioma.Edit = repIdioma;
            txtIdioma.EditValue = "";
            txtIdioma.EditWidth = 200;
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
            // buEditar
            // 
            buEditar.Caption = "Editar";
            buEditar.Enabled = false;
            buEditar.Id = 6;
            buEditar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buEditar.ImageOptions.Image");
            buEditar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buEditar.ImageOptions.LargeImage");
            buEditar.Name = "buEditar";
            buEditar.ItemClick += buEditar_ItemClick;
            // 
            // buTraducir
            // 
            buTraducir.Caption = "Traducir";
            buTraducir.Id = 7;
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
            txtElementos.Id = 8;
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
            optRenglonesBlanco.Id = 9;
            optRenglonesBlanco.Name = "optRenglonesBlanco";
            // 
            // repositoryItemCheckEdit1
            // 
            repositoryItemCheckEdit1.AutoHeight = false;
            repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // txtSeleccion
            // 
            txtSeleccion.Caption = "Seleccione";
            txtSeleccion.Edit = repSeleccion;
            txtSeleccion.EditWidth = 200;
            txtSeleccion.Id = 10;
            txtSeleccion.Name = "txtSeleccion";
            txtSeleccion.EditValueChanged += txtSeleccion_EditValueChanged;
            // 
            // repSeleccion
            // 
            repSeleccion.AutoHeight = false;
            repSeleccion.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repSeleccion.Items.AddRange(new object[] { "Etiqueta Grid y Formulario", "Descripción y Ayuda" });
            repSeleccion.Name = "repSeleccion";
            repSeleccion.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
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
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, grpTranslate });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Traducción de Modelos";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(buEditar);
            ribbonPageGroup1.ItemLinks.Add(buAceptarCambios);
            ribbonPageGroup1.ItemLinks.Add(buDescartarCambios, true);
            ribbonPageGroup1.ItemLinks.Add(txtSeleccion, true);
            ribbonPageGroup1.ItemLinks.Add(txtIdioma);
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
            panelControl1.Controls.Add(grdEtiqueta);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 292);
            panelControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            panelControl1.Name = "panelControl1";
            panelControl1.Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            panelControl1.Size = new System.Drawing.Size(1946, 1039);
            panelControl1.TabIndex = 1;
            // 
            // grdEtiqueta
            // 
            grdEtiqueta.Dock = System.Windows.Forms.DockStyle.Fill;
            grdEtiqueta.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            grdEtiqueta.Location = new System.Drawing.Point(23, 22);
            grdEtiqueta.MainView = grdEtiquetaView;
            grdEtiqueta.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            grdEtiqueta.MenuManager = RibbonControl1;
            grdEtiqueta.Name = "grdEtiqueta";
            grdEtiqueta.Size = new System.Drawing.Size(1900, 995);
            grdEtiqueta.TabIndex = 0;
            grdEtiqueta.UseEmbeddedNavigator = true;
            grdEtiqueta.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdEtiquetaView });
            // 
            // grdEtiquetaView
            // 
            grdEtiquetaView.DetailHeight = 673;
            grdEtiquetaView.GridControl = grdEtiqueta;
            grdEtiquetaView.Name = "grdEtiquetaView";
            grdEtiquetaView.OptionsBehavior.Editable = false;
            grdEtiquetaView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            grdEtiquetaView.OptionsView.ShowAutoFilterRow = true;
            grdEtiquetaView.OptionsView.ShowGroupPanel = false;
            grdEtiquetaView.InitNewRow += grdEtiquetaView_InitNewRow;
            grdEtiquetaView.ValidateRow += grdEtiquetaView_ValidateRow;
            // 
            // fmModelTranslate
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Controls.Add(RibbonControl1);
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Name = "fmModelTranslate";
            Size = new System.Drawing.Size(1946, 1331);
            ((System.ComponentModel.ISupportInitialize)RibbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repIdioma).EndInit();
            ((System.ComponentModel.ISupportInitialize)repElementos).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repSeleccion).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdEtiqueta).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdEtiquetaView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl RibbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem buAceptarCambios;
        private DevExpress.XtraBars.BarButtonItem buDescartarCambios;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl grdEtiqueta;
        private DevExpress.XtraGrid.Views.Grid.GridView grdEtiquetaView;
        private DevExpress.XtraBars.BarEditItem txtIdioma;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repIdioma;
        private DevExpress.XtraBars.BarButtonItem buEditar;
        private DevExpress.XtraBars.BarButtonItem buTraducir;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup grpTranslate;
        private DevExpress.XtraBars.BarEditItem txtElementos;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repElementos;
        private DevExpress.XtraBars.BarEditItem optRenglonesBlanco;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraBars.BarEditItem txtSeleccion;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repSeleccion;
        private DevExpress.XtraBars.BarEditItem optSinTraducir;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
    }
}
