namespace WooW.SB.Designer.DesignerComponents
{
    partial class WoGenericSelector
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
            grdSelector = new DevExpress.XtraGrid.GridControl();
            grdViewSelector = new DevExpress.XtraGrid.Views.Grid.GridView();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            btnAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)grdSelector).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdViewSelector).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            SuspendLayout();
            // 
            // grdSelector
            // 
            grdSelector.Dock = System.Windows.Forms.DockStyle.Top;
            grdSelector.Location = new System.Drawing.Point(0, 0);
            grdSelector.MainView = grdViewSelector;
            grdSelector.Name = "grdSelector";
            grdSelector.Size = new System.Drawing.Size(630, 447);
            grdSelector.TabIndex = 0;
            grdSelector.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdViewSelector });
            // 
            // grdViewSelector
            // 
            grdViewSelector.DetailHeight = 303;
            grdViewSelector.GridControl = grdSelector;
            grdViewSelector.Name = "grdViewSelector";
            grdViewSelector.FocusedRowChanged += grdViewSelector_FocusedRowChanged;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(btnAceptar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 447);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(630, 41);
            panelControl1.TabIndex = 1;
            // 
            // btnAceptar
            // 
            btnAceptar.Dock = System.Windows.Forms.DockStyle.Left;
            btnAceptar.Location = new System.Drawing.Point(2, 2);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new System.Drawing.Size(112, 37);
            btnAceptar.TabIndex = 0;
            btnAceptar.Text = "Aceptar";
            btnAceptar.Click += btnAceptar_Click;
            // 
            // WoGenericSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(630, 488);
            Controls.Add(panelControl1);
            Controls.Add(grdSelector);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "WoGenericSelector";
            ((System.ComponentModel.ISupportInitialize)grdSelector).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdViewSelector).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraGrid.GridControl grdSelector;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewSelector;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnAceptar;
    }
}
