namespace WooW.SB.Config
{
    partial class woDTOColeccion
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
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            optBorrar = new DevExpress.XtraEditors.CheckEdit();
            optActualizar = new DevExpress.XtraEditors.CheckEdit();
            optInsertar = new DevExpress.XtraEditors.CheckEdit();
            grdDTO = new DevExpress.XtraGrid.GridControl();
            grdDTOView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)optBorrar.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optActualizar.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)optInsertar.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdDTO).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdDTOView).BeginInit();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(optBorrar);
            panelControl1.Controls.Add(optActualizar);
            panelControl1.Controls.Add(optInsertar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(20, 19);
            panelControl1.Margin = new System.Windows.Forms.Padding(6);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(547, 160);
            panelControl1.TabIndex = 0;
            // 
            // optBorrar
            // 
            optBorrar.Location = new System.Drawing.Point(40, 110);
            optBorrar.Margin = new System.Windows.Forms.Padding(6);
            optBorrar.Name = "optBorrar";
            optBorrar.Properties.Caption = "Permite Borrar Renglones";
            optBorrar.Size = new System.Drawing.Size(328, 40);
            optBorrar.TabIndex = 2;
            optBorrar.CheckedChanged += optBorrar_CheckedChanged;
            // 
            // optActualizar
            // 
            optActualizar.Location = new System.Drawing.Point(40, 60);
            optActualizar.Margin = new System.Windows.Forms.Padding(6);
            optActualizar.Name = "optActualizar";
            optActualizar.Properties.Caption = "Permite Actualizar Renglones";
            optActualizar.Size = new System.Drawing.Size(328, 40);
            optActualizar.TabIndex = 1;
            optActualizar.CheckedChanged += optActualizar_CheckedChanged;
            // 
            // optInsertar
            // 
            optInsertar.Location = new System.Drawing.Point(40, 10);
            optInsertar.Margin = new System.Windows.Forms.Padding(6);
            optInsertar.Name = "optInsertar";
            optInsertar.Properties.Caption = "Permite Insertar Renglones";
            optInsertar.Size = new System.Drawing.Size(328, 40);
            optInsertar.TabIndex = 0;
            optInsertar.CheckedChanged += optInsertar_CheckedChanged;
            // 
            // grdDTO
            // 
            grdDTO.Dock = System.Windows.Forms.DockStyle.Fill;
            grdDTO.Location = new System.Drawing.Point(20, 179);
            grdDTO.MainView = grdDTOView;
            grdDTO.Name = "grdDTO";
            grdDTO.Size = new System.Drawing.Size(547, 340);
            grdDTO.TabIndex = 2;
            grdDTO.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdDTOView });
            // 
            // grdDTOView
            // 
            grdDTOView.GridControl = grdDTO;
            grdDTOView.Name = "grdDTOView";
            grdDTOView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            grdDTOView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            grdDTOView.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full;
            grdDTOView.OptionsView.ShowGroupPanel = false;
            grdDTOView.OptionsView.ShowIndicator = false;
            // 
            // woDTOColeccion
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(grdDTO);
            Controls.Add(panelControl1);
            Margin = new System.Windows.Forms.Padding(6);
            Name = "woDTOColeccion";
            Padding = new System.Windows.Forms.Padding(20, 19, 20, 19);
            Size = new System.Drawing.Size(587, 538);
            Load += woDTOColeccion_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)optBorrar.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optActualizar.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)optInsertar.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdDTO).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdDTOView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckEdit optActualizar;
        private DevExpress.XtraEditors.CheckEdit optInsertar;
        private DevExpress.XtraEditors.CheckEdit optBorrar;
        private DevExpress.XtraGrid.GridControl grdDTO;
        private DevExpress.XtraGrid.Views.Grid.GridView grdDTOView;
    }
}
