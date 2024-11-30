namespace WooW.SB.FormsAux
{
    partial class fmLibroMayor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmLibroMayor));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            buRefrescar = new DevExpress.XtraBars.BarButtonItem();
            buPeriodos = new DevExpress.XtraBars.BarEditItem();
            repPeriodos = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            txtDato = new DevExpress.XtraBars.BarEditItem();
            repDato = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            panel1 = new System.Windows.Forms.Panel();
            xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            treeObjetos = new DevExpress.XtraTreeList.TreeList();
            grdOData = new DevExpress.XtraGrid.GridControl();
            grdODataView = new DevExpress.XtraGrid.Views.Grid.GridView();
            barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repPeriodos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repDato).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).BeginInit();
            xtraTabControl1.SuspendLayout();
            xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treeObjetos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdOData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdODataView).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, ribbonControl1.SearchEditItem, buRefrescar, buPeriodos, txtDato, barButtonItem1 });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.MaxItemId = 5;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repPeriodos, repDato });
            ribbonControl1.Size = new System.Drawing.Size(2023, 292);
            // 
            // buRefrescar
            // 
            buRefrescar.Caption = "Refrescar";
            buRefrescar.Id = 1;
            buRefrescar.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.Image");
            buRefrescar.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("buRefrescar.ImageOptions.LargeImage");
            buRefrescar.Name = "buRefrescar";
            buRefrescar.ItemClick += buRefrescar_ItemClick;
            // 
            // buPeriodos
            // 
            buPeriodos.Caption = "Periodos";
            buPeriodos.Edit = repPeriodos;
            buPeriodos.EditWidth = 150;
            buPeriodos.Id = 2;
            buPeriodos.Name = "buPeriodos";
            buPeriodos.EditValueChanged += buPeriodos_EditValueChanged;
            // 
            // repPeriodos
            // 
            repPeriodos.AutoHeight = false;
            repPeriodos.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repPeriodos.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] { new DevExpress.XtraEditors.Controls.CheckedListBoxItem("00", "Apertura", System.Windows.Forms.CheckState.Checked, "00"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("01", "Enero", System.Windows.Forms.CheckState.Checked, "01"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("02", "Febrero", System.Windows.Forms.CheckState.Checked, "02"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("03", "Marzo", System.Windows.Forms.CheckState.Checked, "03"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("04", "Abril", System.Windows.Forms.CheckState.Checked, "04"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("05", "Mayo", System.Windows.Forms.CheckState.Checked, "05"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("06", "Junio", System.Windows.Forms.CheckState.Checked, "06"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("07", "Julio", System.Windows.Forms.CheckState.Checked, "07"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("08", "Agosto", System.Windows.Forms.CheckState.Checked, "08"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("09", "Septiembre", System.Windows.Forms.CheckState.Checked, "09"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("10", "Octubre", System.Windows.Forms.CheckState.Checked, "10"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("11", "Noviembre", System.Windows.Forms.CheckState.Checked, "11"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("12", "Diciembre", System.Windows.Forms.CheckState.Checked, "12"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("13", "Cierre", System.Windows.Forms.CheckState.Checked, "13") });
            repPeriodos.Name = "repPeriodos";
            // 
            // txtDato
            // 
            txtDato.Caption = "    Datos";
            txtDato.Edit = repDato;
            txtDato.EditWidth = 150;
            txtDato.Id = 3;
            txtDato.Name = "txtDato";
            txtDato.EditValueChanged += txtDato_EditValueChanged;
            // 
            // repDato
            // 
            repDato.AutoHeight = false;
            repDato.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repDato.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] { new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Inicial", "Inicial", System.Windows.Forms.CheckState.Checked, "Inicial"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Cargos", "Cargos", System.Windows.Forms.CheckState.Checked, "Cargos"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Abonos", "Abonos", System.Windows.Forms.CheckState.Checked, "Abonos"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Neto", "Neto", System.Windows.Forms.CheckState.Checked, "Neto"), new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Final", "Final", System.Windows.Forms.CheckState.Checked, "Final") });
            repDato.Name = "repDato";
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Libro Mayor";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(buRefrescar);
            ribbonPageGroup1.ItemLinks.Add(buPeriodos, true);
            ribbonPageGroup1.ItemLinks.Add(txtDato);
            ribbonPageGroup1.ItemLinks.Add(barButtonItem1);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Acciones";
            // 
            // panel1
            // 
            panel1.Controls.Add(xtraTabControl1);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 292);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(10);
            panel1.Size = new System.Drawing.Size(2023, 992);
            panel1.TabIndex = 2;
            // 
            // xtraTabControl1
            // 
            xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            xtraTabControl1.Location = new System.Drawing.Point(10, 10);
            xtraTabControl1.Name = "xtraTabControl1";
            xtraTabControl1.Padding = new System.Windows.Forms.Padding(10);
            xtraTabControl1.SelectedTabPage = xtraTabPage1;
            xtraTabControl1.Size = new System.Drawing.Size(2003, 972);
            xtraTabControl1.TabIndex = 2;
            xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { xtraTabPage1 });
            // 
            // xtraTabPage1
            // 
            xtraTabPage1.Controls.Add(splitContainerControl1);
            xtraTabPage1.Name = "xtraTabPage1";
            xtraTabPage1.Padding = new System.Windows.Forms.Padding(10);
            xtraTabPage1.Size = new System.Drawing.Size(1999, 923);
            xtraTabPage1.Text = "Tree";
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            splitContainerControl1.Horizontal = false;
            splitContainerControl1.Location = new System.Drawing.Point(10, 10);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(treeObjetos);
            splitContainerControl1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(grdOData);
            splitContainerControl1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(1979, 903);
            splitContainerControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitContainerControl1.SplitterPosition = 505;
            splitContainerControl1.TabIndex = 1;
            // 
            // treeObjetos
            // 
            treeObjetos.ColumnPanelRowHeight = 76;
            treeObjetos.Dock = System.Windows.Forms.DockStyle.Fill;
            treeObjetos.Location = new System.Drawing.Point(10, 10);
            treeObjetos.MenuManager = ribbonControl1;
            treeObjetos.Name = "treeObjetos";
            treeObjetos.OptionsBehavior.Editable = false;
            treeObjetos.OptionsMenu.EnableColumnMenu = false;
            treeObjetos.OptionsMenu.EnableFooterMenu = false;
            treeObjetos.OptionsSelection.InvertSelection = true;
            treeObjetos.OptionsView.AutoWidth = false;
            treeObjetos.OptionsView.EnableAppearanceEvenRow = true;
            treeObjetos.OptionsView.EnableAppearanceOddRow = true;
            treeObjetos.Size = new System.Drawing.Size(1959, 358);
            treeObjetos.TabIndex = 0;
            treeObjetos.FocusedNodeChanged += treeObjetos_FocusedNodeChanged;
            treeObjetos.FocusedColumnChanged += treeObjetos_FocusedColumnChanged;
            treeObjetos.CustomDrawNodeCell += treeObjetos_CustomDrawNodeCell;
            // 
            // grdOData
            // 
            grdOData.Dock = System.Windows.Forms.DockStyle.Fill;
            grdOData.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            grdOData.Location = new System.Drawing.Point(10, 10);
            grdOData.MainView = grdODataView;
            grdOData.Margin = new System.Windows.Forms.Padding(6);
            grdOData.Name = "grdOData";
            grdOData.Size = new System.Drawing.Size(1959, 485);
            grdOData.TabIndex = 4;
            grdOData.UseEmbeddedNavigator = true;
            grdOData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdODataView });
            // 
            // grdODataView
            // 
            grdODataView.DetailHeight = 673;
            grdODataView.GridControl = grdOData;
            grdODataView.Name = "grdODataView";
            grdODataView.OptionsBehavior.Editable = false;
            grdODataView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.Inplace;
            grdODataView.OptionsView.BestFitMaxRowCount = 50;
            grdODataView.OptionsView.EnableAppearanceOddRow = true;
            grdODataView.OptionsView.ShowAutoFilterRow = true;
            grdODataView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.ShowAlways;
            grdODataView.OptionsView.ShowGroupPanel = false;
            // 
            // barButtonItem1
            // 
            barButtonItem1.Caption = "barButtonItem1";
            barButtonItem1.Id = 4;
            barButtonItem1.Name = "barButtonItem1";
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;
            // 
            // fmLibroMayor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(ribbonControl1);
            Name = "fmLibroMayor";
            Size = new System.Drawing.Size(2023, 1284);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repPeriodos).EndInit();
            ((System.ComponentModel.ISupportInitialize)repDato).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).EndInit();
            xtraTabControl1.ResumeLayout(false);
            xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)treeObjetos).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdOData).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdODataView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem buRefrescar;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTreeList.TreeList treeObjetos;
        private DevExpress.XtraBars.BarEditItem buPeriodos;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repPeriodos;
        private DevExpress.XtraBars.BarEditItem txtDato;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repDato;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl grdOData;
        private DevExpress.XtraGrid.Views.Grid.GridView grdODataView;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
    }
}
