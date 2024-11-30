using DevExpress.XtraEditors;

namespace WooW.SB.Forms
{
    partial class fmModelStateDiagram
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmModelStateDiagram));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buEditar = new DevExpress.XtraBars.BarButtonItem();
            buRegistrar = new DevExpress.XtraBars.BarButtonItem();
            buAnular = new DevExpress.XtraBars.BarButtonItem();
            buZoomMas = new DevExpress.XtraBars.BarButtonItem();
            buZoomMenos = new DevExpress.XtraBars.BarButtonItem();
            buRedibujar = new DevExpress.XtraBars.BarButtonItem();
            buRefrescar = new DevExpress.XtraBars.BarButtonItem();
            optMostrarPermisos = new DevExpress.XtraBars.BarEditItem();
            repMostrarPermisos = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            buMuestraDTO = new DevExpress.XtraBars.BarButtonItem();
            txtFiltroApps = new DevExpress.XtraBars.BarEditItem();
            repFiltroApps = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            buBorrar = new DevExpress.XtraBars.BarButtonItem();
            buCopiar = new DevExpress.XtraBars.BarButtonItem();
            buAnalizaRolesDeLectura = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            grpCambios = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            panelControl1 = new PanelControl();
            splitControl = new SplitContainerControl();
            grdModelos = new DevExpress.XtraGrid.GridControl();
            grdModelosView = new DevExpress.XtraGrid.Views.Grid.GridView();
            woDiagram1 = new Config.woDiagram();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repMostrarPermisos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repFiltroApps).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitControl.Panel1).BeginInit();
            splitControl.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitControl.Panel2).BeginInit();
            splitControl.Panel2.SuspendLayout();
            splitControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdModelos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdModelosView).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, buEditar, buRegistrar, buAnular, buZoomMas, buZoomMenos, buRedibujar, buRefrescar, optMostrarPermisos, buMuestraDTO, txtFiltroApps, buBorrar, buCopiar, buAnalizaRolesDeLectura });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.MaxItemId = 18;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repMostrarPermisos, repFiltroApps });
            ribbonControl1.Size = new System.Drawing.Size(1209, 150);
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
            // buRegistrar
            // 
            buRegistrar.Caption = "Grabar";
            buRegistrar.Enabled = false;
            buRegistrar.Id = 5;
            buRegistrar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buRegistrar.ImageOptions.Image");
            buRegistrar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buRegistrar.ImageOptions.LargeImage");
            buRegistrar.Name = "buRegistrar";
            buRegistrar.ItemClick += buGrabar_ItemClick;
            // 
            // buAnular
            // 
            buAnular.Caption = "Deshacer Cambios";
            buAnular.Enabled = false;
            buAnular.Id = 6;
            buAnular.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buAnular.ImageOptions.Image");
            buAnular.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buAnular.ImageOptions.LargeImage");
            buAnular.Name = "buAnular";
            buAnular.ItemClick += buDeshacer_ItemClick;
            // 
            // buZoomMas
            // 
            buZoomMas.Caption = "Zoom +";
            buZoomMas.Enabled = false;
            buZoomMas.Id = 7;
            buZoomMas.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buZoomMas.ImageOptions.Image");
            buZoomMas.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buZoomMas.ImageOptions.LargeImage");
            buZoomMas.Name = "buZoomMas";
            buZoomMas.ItemClick += buZoomMas_ItemClick;
            // 
            // buZoomMenos
            // 
            buZoomMenos.Caption = "Zoom -";
            buZoomMenos.Enabled = false;
            buZoomMenos.Id = 8;
            buZoomMenos.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buZoomMenos.ImageOptions.Image");
            buZoomMenos.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buZoomMenos.ImageOptions.LargeImage");
            buZoomMenos.Name = "buZoomMenos";
            buZoomMenos.ItemClick += buZoomMenos_ItemClick;
            // 
            // buRedibujar
            // 
            buRedibujar.Caption = "Re dibujar";
            buRedibujar.Enabled = false;
            buRedibujar.Id = 9;
            buRedibujar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buRedibujar.ImageOptions.Image");
            buRedibujar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buRedibujar.ImageOptions.LargeImage");
            buRedibujar.Name = "buRedibujar";
            buRedibujar.ItemClick += buRedibujar_ItemClick;
            // 
            // buRefrescar
            // 
            buRefrescar.Caption = "Refrescar";
            buRefrescar.Id = 10;
            buRefrescar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.Image");
            buRefrescar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.LargeImage");
            buRefrescar.Name = "buRefrescar";
            buRefrescar.ItemClick += buRefrescarModelos_ItemClick;
            // 
            // optMostrarPermisos
            // 
            optMostrarPermisos.Caption = "Mostrar Permisos";
            optMostrarPermisos.Edit = repMostrarPermisos;
            optMostrarPermisos.EditValue = false;
            optMostrarPermisos.Enabled = false;
            optMostrarPermisos.Id = 11;
            optMostrarPermisos.Name = "optMostrarPermisos";
            optMostrarPermisos.EditValueChanged += optMostrarPermisos_EditValueChanged;
            // 
            // repMostrarPermisos
            // 
            repMostrarPermisos.AutoHeight = false;
            repMostrarPermisos.Name = "repMostrarPermisos";
            // 
            // buMuestraDTO
            // 
            buMuestraDTO.Caption = "Muestra DTO";
            buMuestraDTO.Enabled = false;
            buMuestraDTO.Id = 12;
            buMuestraDTO.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buMuestraDTO.ImageOptions.Image");
            buMuestraDTO.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buMuestraDTO.ImageOptions.LargeImage");
            buMuestraDTO.Name = "buMuestraDTO";
            buMuestraDTO.ItemClick += buMuestraDTO_ItemClick;
            // 
            // txtFiltroApps
            // 
            txtFiltroApps.Caption = "Filtro Apps";
            txtFiltroApps.Edit = repFiltroApps;
            txtFiltroApps.EditWidth = 160;
            txtFiltroApps.Id = 14;
            txtFiltroApps.Name = "txtFiltroApps";
            txtFiltroApps.HiddenEditor += txtFiltroApps_HiddenEditor;
            // 
            // repFiltroApps
            // 
            repFiltroApps.AutoHeight = false;
            repFiltroApps.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repFiltroApps.Name = "repFiltroApps";
            // 
            // buBorrar
            // 
            buBorrar.Caption = "Borrar Diagrama";
            buBorrar.Id = 15;
            buBorrar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buBorrar.ImageOptions.Image");
            buBorrar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buBorrar.ImageOptions.LargeImage");
            buBorrar.Name = "buBorrar";
            buBorrar.ItemClick += buBorrar_ItemClick;
            // 
            // buCopiar
            // 
            buCopiar.Caption = "Copiar Diagrama de ...";
            buCopiar.Id = 16;
            buCopiar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buCopiar.ImageOptions.Image");
            buCopiar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buCopiar.ImageOptions.LargeImage");
            buCopiar.Name = "buCopiar";
            buCopiar.ItemClick += buCopiar_ItemClick;
            // 
            // buAnalizaRolesDeLectura
            // 
            buAnalizaRolesDeLectura.Caption = "Analíza Roles de Lectura";
            buAnalizaRolesDeLectura.Enabled = false;
            buAnalizaRolesDeLectura.Id = 17;
            buAnalizaRolesDeLectura.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buAnalizaRolesDeLectura.ImageOptions.Image");
            buAnalizaRolesDeLectura.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buAnalizaRolesDeLectura.ImageOptions.LargeImage");
            buAnalizaRolesDeLectura.Name = "buAnalizaRolesDeLectura";
            buAnalizaRolesDeLectura.ItemClick += buAnalizaRolesDeLectura_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { grpCambios, ribbonPageGroup2 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Diagrama de Estados";
            // 
            // grpCambios
            // 
            grpCambios.ItemLinks.Add(buRefrescar);
            grpCambios.ItemLinks.Add(txtFiltroApps, true);
            grpCambios.Name = "grpCambios";
            grpCambios.Text = "Cambios";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(buEditar);
            ribbonPageGroup2.ItemLinks.Add(buRegistrar);
            ribbonPageGroup2.ItemLinks.Add(buAnular);
            ribbonPageGroup2.ItemLinks.Add(buBorrar);
            ribbonPageGroup2.ItemLinks.Add(buCopiar);
            ribbonPageGroup2.ItemLinks.Add(buZoomMas, true);
            ribbonPageGroup2.ItemLinks.Add(buZoomMenos);
            ribbonPageGroup2.ItemLinks.Add(buRedibujar);
            ribbonPageGroup2.ItemLinks.Add(optMostrarPermisos, true);
            ribbonPageGroup2.ItemLinks.Add(buMuestraDTO);
            ribbonPageGroup2.ItemLinks.Add(buAnalizaRolesDeLectura, true);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = "Diagrama de Estados";
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(splitControl);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 150);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1209, 551);
            panelControl1.TabIndex = 2;
            // 
            // splitControl
            // 
            splitControl.Dock = System.Windows.Forms.DockStyle.Fill;
            splitControl.Location = new System.Drawing.Point(2, 2);
            splitControl.Name = "splitControl";
            // 
            // splitControl.Panel1
            // 
            splitControl.Panel1.Controls.Add(grdModelos);
            splitControl.Panel1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            splitControl.Panel1.Text = "Panel1";
            // 
            // splitControl.Panel2
            // 
            splitControl.Panel2.Controls.Add(woDiagram1);
            splitControl.Panel2.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            splitControl.Panel2.Text = "Panel2";
            splitControl.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitControl.Size = new System.Drawing.Size(1205, 547);
            splitControl.SplitterPosition = 444;
            splitControl.TabIndex = 1;
            // 
            // grdModelos
            // 
            grdModelos.Dock = System.Windows.Forms.DockStyle.Fill;
            grdModelos.Location = new System.Drawing.Point(10, 10);
            grdModelos.MainView = grdModelosView;
            grdModelos.MenuManager = ribbonControl1;
            grdModelos.Name = "grdModelos";
            grdModelos.Size = new System.Drawing.Size(424, 527);
            grdModelos.TabIndex = 0;
            grdModelos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdModelosView });
            // 
            // grdModelosView
            // 
            grdModelosView.ColumnPanelRowHeight = 0;
            grdModelosView.FooterPanelHeight = 0;
            grdModelosView.GridControl = grdModelos;
            grdModelosView.GroupRowHeight = 0;
            grdModelosView.LevelIndent = 0;
            grdModelosView.Name = "grdModelosView";
            grdModelosView.OptionsBehavior.Editable = false;
            grdModelosView.OptionsEditForm.PopupEditFormWidth = 400;
            grdModelosView.OptionsView.ShowAutoFilterRow = true;
            grdModelosView.PreviewIndent = 0;
            grdModelosView.RowHeight = 0;
            grdModelosView.ViewCaptionHeight = 0;
            grdModelosView.FocusedRowChanged += grdModelosView_FocusedRowChanged;
            // 
            // woDiagram1
            // 
            woDiagram1.currDiagrama = null;
            woDiagram1.currModelo = null;
            woDiagram1.Dock = System.Windows.Forms.DockStyle.Fill;
            woDiagram1.Enabled = false;
            woDiagram1.Location = new System.Drawing.Point(10, 10);
            woDiagram1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            woDiagram1.MostrarPermisos = false;
            woDiagram1.Name = "woDiagram1";
            woDiagram1.Size = new System.Drawing.Size(731, 527);
            woDiagram1.TabIndex = 0;
            woDiagram1.Load += woDiagram1_Load;
            // 
            // fmModelStateDiagram
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Controls.Add(ribbonControl1);
            Name = "fmModelStateDiagram";
            Size = new System.Drawing.Size(1209, 701);
            Load += fmModelosLogica_Load;
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repMostrarPermisos).EndInit();
            ((System.ComponentModel.ISupportInitialize)repFiltroApps).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitControl.Panel1).EndInit();
            splitControl.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitControl.Panel2).EndInit();
            splitControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitControl).EndInit();
            splitControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdModelos).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdModelosView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup grpCambios;
        private PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private SplitContainerControl splitControl;
        private DevExpress.XtraGrid.GridControl grdModelos;
        private DevExpress.XtraGrid.Views.Grid.GridView grdModelosView;
        private DevExpress.XtraBars.BarButtonItem buEditar;
        private DevExpress.XtraBars.BarButtonItem buRegistrar;
        private DevExpress.XtraBars.BarButtonItem buAnular;
        private DevExpress.XtraBars.BarButtonItem buZoomMas;
        private DevExpress.XtraBars.BarButtonItem buZoomMenos;
        private DevExpress.XtraBars.BarButtonItem buRedibujar;
        private DevExpress.XtraBars.BarButtonItem buRefrescar;
        private DevExpress.XtraBars.BarEditItem optMostrarPermisos;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repMostrarPermisos;
        private Config.woDiagram woDiagram1;
        private DevExpress.XtraBars.BarButtonItem buMuestraDTO;
        private DevExpress.XtraBars.BarEditItem txtFiltroApps;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repFiltroApps;
        private DevExpress.XtraBars.BarButtonItem buBorrar;
        private DevExpress.XtraBars.BarButtonItem buCopiar;
        private DevExpress.XtraBars.BarButtonItem buAnalizaRolesDeLectura;
    }
}
