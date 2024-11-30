namespace WooW.SB.Forms
{
    partial class fmModelosLogica
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
            this.buEditar = new DevExpress.XtraBars.BarButtonItem();
            this.buRegistrar = new DevExpress.XtraBars.BarButtonItem();
            this.buAnular = new DevExpress.XtraBars.BarButtonItem();
            this.buZoomMas = new DevExpress.XtraBars.BarButtonItem();
            this.buZoomMenos = new DevExpress.XtraBars.BarButtonItem();
            this.buRedibujar = new DevExpress.XtraBars.BarButtonItem();
            this.buRefrescarModelos = new DevExpress.XtraBars.BarButtonItem();
            this.optMostrarPermisos = new DevExpress.XtraBars.BarEditItem();
            this.repMostrarPermisos = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.buMuestraDTO = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.grpCambios = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.splitControl = new DevExpress.XtraEditors.SplitContainerControl();
            this.grdModelos = new DevExpress.XtraGrid.GridControl();
            this.grdModelosView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.woDiagram1 = new WooW.SB.Config.woDiagram();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repMostrarPermisos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel1)).BeginInit();
            this.splitControl.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel2)).BeginInit();
            this.splitControl.Panel2.SuspendLayout();
            this.splitControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdModelos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModelosView)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.buEditar,
            this.buRegistrar,
            this.buAnular,
            this.buZoomMas,
            this.buZoomMenos,
            this.buRedibujar,
            this.buRefrescarModelos,
            this.optMostrarPermisos,
            this.buMuestraDTO});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 14;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repMostrarPermisos});
            this.ribbonControl1.Size = new System.Drawing.Size(1209, 150);
            // 
            // buEditar
            // 
            this.buEditar.Caption = "Editar";
            this.buEditar.Id = 4;
            this.buEditar.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Query_Design;
            this.buEditar.Name = "buEditar";
            this.buEditar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buEditar_ItemClick);
            // 
            // buRegistrar
            // 
            this.buRegistrar.Caption = "Grabar";
            this.buRegistrar.Enabled = false;
            this.buRegistrar.Id = 5;
            this.buRegistrar.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.dbTable_Save;
            this.buRegistrar.Name = "buRegistrar";
            this.buRegistrar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buGrabar_ItemClick);
            // 
            // buAnular
            // 
            this.buAnular.Caption = "Deshacer Cambios";
            this.buAnular.Enabled = false;
            this.buAnular.Id = 6;
            this.buAnular.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.dbTable_Refresh;
            this.buAnular.Name = "buAnular";
            this.buAnular.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buDeshacer_ItemClick);
            // 
            // buZoomMas
            // 
            this.buZoomMas.Caption = "Zoom +";
            this.buZoomMas.Enabled = false;
            this.buZoomMas.Id = 7;
            this.buZoomMas.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Zoom_In_More;
            this.buZoomMas.Name = "buZoomMas";
            this.buZoomMas.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buZoomMas_ItemClick);
            // 
            // buZoomMenos
            // 
            this.buZoomMenos.Caption = "Zoom -";
            this.buZoomMenos.Enabled = false;
            this.buZoomMenos.Id = 8;
            this.buZoomMenos.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Zoom_Out;
            this.buZoomMenos.Name = "buZoomMenos";
            this.buZoomMenos.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buZoomMenos_ItemClick);
            // 
            // buRedibujar
            // 
            this.buRedibujar.Caption = "Redibujar";
            this.buRedibujar.Enabled = false;
            this.buRedibujar.Id = 9;
            this.buRedibujar.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Repeat;
            this.buRedibujar.Name = "buRedibujar";
            this.buRedibujar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buRedibujar_ItemClick);
            // 
            // buRefrescarModelos
            // 
            this.buRefrescarModelos.Caption = "Refrescar Modelos";
            this.buRefrescarModelos.Id = 10;
            this.buRefrescarModelos.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Code_Snippet_Tick;
            this.buRefrescarModelos.Name = "buRefrescarModelos";
            this.buRefrescarModelos.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buRefrescarModelos_ItemClick);
            // 
            // optMostrarPermisos
            // 
            this.optMostrarPermisos.Caption = "Mostrar Permisos";
            this.optMostrarPermisos.Edit = this.repMostrarPermisos;
            this.optMostrarPermisos.EditValue = false;
            this.optMostrarPermisos.Enabled = false;
            this.optMostrarPermisos.Id = 11;
            this.optMostrarPermisos.Name = "optMostrarPermisos";
            this.optMostrarPermisos.EditValueChanged += new System.EventHandler(this.optMostrarPermisos_EditValueChanged);
            // 
            // repMostrarPermisos
            // 
            this.repMostrarPermisos.AutoHeight = false;
            this.repMostrarPermisos.Name = "repMostrarPermisos";
            // 
            // buMuestraDTO
            // 
            this.buMuestraDTO.Caption = "Muestra DTO";
            this.buMuestraDTO.Enabled = false;
            this.buMuestraDTO.Id = 12;
            this.buMuestraDTO.ImageOptions.LargeImage = global::WooW.SB.Properties.Resources.Site_Map;
            this.buMuestraDTO.Name = "buMuestraDTO";
            this.buMuestraDTO.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.buMuestraDTO_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.grpCambios,
            this.ribbonPageGroup2});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Modelos Logica";
            // 
            // grpCambios
            // 
            this.grpCambios.ItemLinks.Add(this.buRefrescarModelos);
            this.grpCambios.Name = "grpCambios";
            this.grpCambios.Text = "Cambios";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.buEditar);
            this.ribbonPageGroup2.ItemLinks.Add(this.buRegistrar);
            this.ribbonPageGroup2.ItemLinks.Add(this.buAnular);
            this.ribbonPageGroup2.ItemLinks.Add(this.buZoomMas, true);
            this.ribbonPageGroup2.ItemLinks.Add(this.buZoomMenos);
            this.ribbonPageGroup2.ItemLinks.Add(this.buRedibujar);
            this.ribbonPageGroup2.ItemLinks.Add(this.optMostrarPermisos, true);
            this.ribbonPageGroup2.ItemLinks.Add(this.buMuestraDTO);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Diagrama de Estados";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.splitControl);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 150);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1209, 551);
            this.panelControl1.TabIndex = 2;
            // 
            // splitControl
            // 
            this.splitControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitControl.Location = new System.Drawing.Point(2, 2);
            this.splitControl.Name = "splitControl";
            // 
            // splitControl.Panel1
            // 
            this.splitControl.Panel1.Controls.Add(this.grdModelos);
            this.splitControl.Panel1.Padding = new System.Windows.Forms.Padding(10);
            this.splitControl.Panel1.Text = "Panel1";
            // 
            // splitControl.Panel2
            // 
            this.splitControl.Panel2.Controls.Add(this.woDiagram1);
            this.splitControl.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitControl.Panel2.Text = "Panel2";
            this.splitControl.Size = new System.Drawing.Size(1205, 547);
            this.splitControl.SplitterPosition = 311;
            this.splitControl.TabIndex = 1;
            // 
            // grdModelos
            // 
            this.grdModelos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdModelos.Location = new System.Drawing.Point(10, 10);
            this.grdModelos.MainView = this.grdModelosView;
            this.grdModelos.MenuManager = this.ribbonControl1;
            this.grdModelos.Name = "grdModelos";
            this.grdModelos.Size = new System.Drawing.Size(291, 527);
            this.grdModelos.TabIndex = 0;
            this.grdModelos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdModelosView});
            // 
            // grdModelosView
            // 
            this.grdModelosView.ColumnPanelRowHeight = 0;
            this.grdModelosView.FooterPanelHeight = 0;
            this.grdModelosView.GridControl = this.grdModelos;
            this.grdModelosView.GroupRowHeight = 0;
            this.grdModelosView.LevelIndent = 0;
            this.grdModelosView.Name = "grdModelosView";
            this.grdModelosView.OptionsBehavior.Editable = false;
            this.grdModelosView.OptionsView.ShowAutoFilterRow = true;
            this.grdModelosView.PreviewIndent = 0;
            this.grdModelosView.RowHeight = 0;
            this.grdModelosView.ViewCaptionHeight = 0;
            this.grdModelosView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grdModelosView_FocusedRowChanged);
            // 
            // woDiagram1
            // 
            this.woDiagram1.currModel = null;
            this.woDiagram1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.woDiagram1.Enabled = false;
            this.woDiagram1.Location = new System.Drawing.Point(10, 10);
            this.woDiagram1.Margin = new System.Windows.Forms.Padding(8);
            this.woDiagram1.MostrarPermisos = false;
            this.woDiagram1.Name = "woDiagram1";
            this.woDiagram1.Size = new System.Drawing.Size(864, 527);
            this.woDiagram1.TabIndex = 0;
            // 
            // fmModelosLogica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "fmModelosLogica";
            this.Size = new System.Drawing.Size(1209, 701);
            this.Load += new System.EventHandler(this.fmModelosLogica_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repMostrarPermisos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel1)).EndInit();
            this.splitControl.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitControl.Panel2)).EndInit();
            this.splitControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitControl)).EndInit();
            this.splitControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdModelos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdModelosView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup grpCambios;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraEditors.SplitContainerControl splitControl;
        private DevExpress.XtraGrid.GridControl grdModelos;
        private DevExpress.XtraGrid.Views.Grid.GridView grdModelosView;
        private DevExpress.XtraBars.BarButtonItem buEditar;
        private DevExpress.XtraBars.BarButtonItem buRegistrar;
        private DevExpress.XtraBars.BarButtonItem buAnular;
        private DevExpress.XtraBars.BarButtonItem buZoomMas;
        private DevExpress.XtraBars.BarButtonItem buZoomMenos;
        private DevExpress.XtraBars.BarButtonItem buRedibujar;
        private DevExpress.XtraBars.BarButtonItem buRefrescarModelos;
        private DevExpress.XtraBars.BarEditItem optMostrarPermisos;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repMostrarPermisos;
        private Config.woDiagram woDiagram1;
        private DevExpress.XtraBars.BarButtonItem buMuestraDTO;
    }
}
