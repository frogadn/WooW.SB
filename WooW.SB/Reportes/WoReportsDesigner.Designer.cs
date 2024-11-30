namespace WooW.SB.Reportes
{
    partial class WoReportsDesigner
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
            grdReportes = new DevExpress.XtraGrid.GridControl();
            grdReportesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            tabPrincipal = new DevExpress.XtraTab.XtraTabControl();
            tabDatos = new DevExpress.XtraTab.XtraTabPage();
            splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            gcFiltros = new DevExpress.XtraEditors.GroupControl();
            grdResultado = new DevExpress.XtraGrid.GridControl();
            grdResultadoView = new DevExpress.XtraGrid.Views.Grid.GridView();
            tabPrevio = new DevExpress.XtraTab.XtraTabPage();
            repPreview = new DevExpress.XtraPrinting.Preview.DocumentViewer();
            splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
            xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            xtraTabPageReportsService = new DevExpress.XtraTab.XtraTabPage();
            xtraTabPageReportsDTO = new DevExpress.XtraTab.XtraTabPage();
            xtraTabPageConsole = new DevExpress.XtraTab.XtraTabPage();
            consoleData = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)grdReportes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdReportesView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabPrincipal).BeginInit();
            tabPrincipal.SuspendLayout();
            tabDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel1).BeginInit();
            splitContainerControl2.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel2).BeginInit();
            splitContainerControl2.Panel2.SuspendLayout();
            splitContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gcFiltros).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdResultado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdResultadoView).BeginInit();
            tabPrevio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel1).BeginInit();
            splitContainerControl3.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel2).BeginInit();
            splitContainerControl3.Panel2.SuspendLayout();
            splitContainerControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).BeginInit();
            xtraTabControl1.SuspendLayout();
            xtraTabPageReportsService.SuspendLayout();
            xtraTabPageConsole.SuspendLayout();
            SuspendLayout();
            // 
            // grdReportes
            // 
            grdReportes.Dock = System.Windows.Forms.DockStyle.Fill;
            grdReportes.Location = new System.Drawing.Point(0, 0);
            grdReportes.MainView = grdReportesView;
            grdReportes.Name = "grdReportes";
            grdReportes.Size = new System.Drawing.Size(481, 741);
            grdReportes.TabIndex = 0;
            grdReportes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdReportesView });
            // 
            // grdReportesView
            // 
            grdReportesView.GridControl = grdReportes;
            grdReportesView.Name = "grdReportesView";
            grdReportesView.OptionsBehavior.Editable = false;
            grdReportesView.OptionsBehavior.ReadOnly = true;
            grdReportesView.OptionsView.ShowGroupPanel = false;
            grdReportesView.FocusedRowChanged += grdReportesView_FocusedRowChanged;
            // 
            // tabPrincipal
            // 
            tabPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPrincipal.Location = new System.Drawing.Point(0, 0);
            tabPrincipal.Name = "tabPrincipal";
            tabPrincipal.SelectedTabPage = tabDatos;
            tabPrincipal.Size = new System.Drawing.Size(899, 741);
            tabPrincipal.TabIndex = 0;
            tabPrincipal.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { tabDatos, tabPrevio });
            tabPrincipal.SelectedPageChanged += SelectedTabPag;
            // 
            // tabDatos
            // 
            tabDatos.Controls.Add(splitContainerControl2);
            tabDatos.Name = "tabDatos";
            tabDatos.Size = new System.Drawing.Size(897, 716);
            tabDatos.Text = "Fuente de Datos";
            // 
            // splitContainerControl2
            // 
            splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl2.Location = new System.Drawing.Point(0, 0);
            splitContainerControl2.Name = "splitContainerControl2";
            // 
            // splitContainerControl2.Panel1
            // 
            splitContainerControl2.Panel1.Controls.Add(gcFiltros);
            splitContainerControl2.Panel1.Text = "Panel1";
            // 
            // splitContainerControl2.Panel2
            // 
            splitContainerControl2.Panel2.Controls.Add(grdResultado);
            splitContainerControl2.Panel2.Text = "Panel2";
            splitContainerControl2.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitContainerControl2.Size = new System.Drawing.Size(897, 716);
            splitContainerControl2.SplitterPosition = 473;
            splitContainerControl2.TabIndex = 0;
            // 
            // gcFiltros
            // 
            gcFiltros.Dock = System.Windows.Forms.DockStyle.Fill;
            gcFiltros.Location = new System.Drawing.Point(0, 0);
            gcFiltros.Name = "gcFiltros";
            gcFiltros.Size = new System.Drawing.Size(473, 716);
            gcFiltros.TabIndex = 0;
            gcFiltros.Text = "Filtros";
            // 
            // grdResultado
            // 
            grdResultado.Dock = System.Windows.Forms.DockStyle.Fill;
            grdResultado.Location = new System.Drawing.Point(0, 0);
            grdResultado.MainView = grdResultadoView;
            grdResultado.Name = "grdResultado";
            grdResultado.Size = new System.Drawing.Size(414, 716);
            grdResultado.TabIndex = 0;
            grdResultado.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdResultadoView });
            // 
            // grdResultadoView
            // 
            grdResultadoView.GridControl = grdResultado;
            grdResultadoView.Name = "grdResultadoView";
            grdResultadoView.OptionsBehavior.Editable = false;
            grdResultadoView.OptionsBehavior.ReadOnly = true;
            grdResultadoView.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full;
            grdResultadoView.OptionsView.ShowGroupPanel = false;
            // 
            // tabPrevio
            // 
            tabPrevio.Controls.Add(repPreview);
            tabPrevio.Name = "tabPrevio";
            tabPrevio.Size = new System.Drawing.Size(897, 716);
            tabPrevio.Text = "Previo";
            // 
            // repPreview
            // 
            repPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            repPreview.IsMetric = true;
            repPreview.Location = new System.Drawing.Point(0, 0);
            repPreview.Name = "repPreview";
            repPreview.Size = new System.Drawing.Size(897, 716);
            repPreview.TabIndex = 0;
            // 
            // splitContainerControl3
            // 
            splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl3.Location = new System.Drawing.Point(0, 0);
            splitContainerControl3.Name = "splitContainerControl3";
            // 
            // splitContainerControl3.Panel1
            // 
            splitContainerControl3.Panel1.Controls.Add(grdReportes);
            splitContainerControl3.Panel1.Text = "Panel1";
            // 
            // splitContainerControl3.Panel2
            // 
            splitContainerControl3.Panel2.Controls.Add(tabPrincipal);
            splitContainerControl3.Panel2.Text = "Panel2";
            splitContainerControl3.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitContainerControl3.Size = new System.Drawing.Size(1390, 741);
            splitContainerControl3.SplitterPosition = 481;
            splitContainerControl3.TabIndex = 1;
            // 
            // xtraTabControl1
            // 
            xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            xtraTabControl1.Name = "xtraTabControl1";
            xtraTabControl1.SelectedTabPage = xtraTabPageReportsService;
            xtraTabControl1.Size = new System.Drawing.Size(1392, 766);
            xtraTabControl1.TabIndex = 1;
            xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { xtraTabPageReportsService, xtraTabPageReportsDTO, xtraTabPageConsole });
            xtraTabControl1.SelectedPageChanged += xtraTabControl1_SelectedPageChanged;
            // 
            // xtraTabPageReportsService
            // 
            xtraTabPageReportsService.Controls.Add(splitContainerControl3);
            xtraTabPageReportsService.Name = "xtraTabPageReportsService";
            xtraTabPageReportsService.Size = new System.Drawing.Size(1390, 741);
            xtraTabPageReportsService.Text = "Servicio";
            // 
            // xtraTabPageReportsDTO
            // 
            xtraTabPageReportsDTO.Name = "xtraTabPageReportsDTO";
            xtraTabPageReportsDTO.Size = new System.Drawing.Size(1390, 741);
            xtraTabPageReportsDTO.Text = "ODATA";
            // 
            // xtraTabPageConsole
            // 
            xtraTabPageConsole.Controls.Add(consoleData);
            xtraTabPageConsole.Name = "xtraTabPageConsole";
            xtraTabPageConsole.Size = new System.Drawing.Size(1390, 741);
            xtraTabPageConsole.Text = "Consola";
            // 
            // consoleData
            // 
            consoleData.BackColor = System.Drawing.SystemColors.InfoText;
            consoleData.Dock = System.Windows.Forms.DockStyle.Fill;
            consoleData.Location = new System.Drawing.Point(0, 0);
            consoleData.Name = "consoleData";
            consoleData.ReadOnly = true;
            consoleData.Size = new System.Drawing.Size(1390, 741);
            consoleData.TabIndex = 0;
            consoleData.Text = "";
            // 
            // WoReportsDesigner
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(xtraTabControl1);
            Name = "WoReportsDesigner";
            Size = new System.Drawing.Size(1392, 766);
            Load += WoReports_Load;
            ((System.ComponentModel.ISupportInitialize)grdReportes).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdReportesView).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabPrincipal).EndInit();
            tabPrincipal.ResumeLayout(false);
            tabDatos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel1).EndInit();
            splitContainerControl2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel2).EndInit();
            splitContainerControl2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2).EndInit();
            splitContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gcFiltros).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdResultado).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdResultadoView).EndInit();
            tabPrevio.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel1).EndInit();
            splitContainerControl3.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3.Panel2).EndInit();
            splitContainerControl3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl3).EndInit();
            splitContainerControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).EndInit();
            xtraTabControl1.ResumeLayout(false);
            xtraTabPageReportsService.ResumeLayout(false);
            xtraTabPageConsole.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraGrid.GridControl grdReportes;
        public DevExpress.XtraTab.XtraTabPage tabDatos;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        private DevExpress.XtraGrid.Views.Grid.GridView grdResultadoView;
        private DevExpress.XtraEditors.GroupControl gcFiltros;
        public DevExpress.XtraGrid.Views.Grid.GridView grdReportesView;
        public DevExpress.XtraGrid.GridControl grdResultado;
        public DevExpress.XtraPrinting.Preview.DocumentViewer repPreview;
        public DevExpress.XtraTab.XtraTabControl tabPrincipal;
        public DevExpress.XtraTab.XtraTabPage tabPrevio;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl3;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageReportsService;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageReportsDTO;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageConsole;
        public System.Windows.Forms.RichTextBox consoleData;
    }
}
