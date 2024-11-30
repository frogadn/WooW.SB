namespace WooW.SB.Reportes.ReportsComponents
{
    partial class fmPropReports
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            propReports = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            ((System.ComponentModel.ISupportInitialize)propReports).BeginInit();
            SuspendLayout();
            // 
            // propReports
            // 
            propReports.BandsInterval = 4;
            propReports.Dock = System.Windows.Forms.DockStyle.Fill;
            propReports.Location = new System.Drawing.Point(0, 0);
            propReports.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            propReports.Name = "propReports";
            propReports.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            propReports.OptionsView.FixedLineWidth = 4;
            propReports.OptionsView.MinRowAutoHeight = 21;
            propReports.Size = new System.Drawing.Size(815, 768);
            propReports.TabIndex = 0;
            propReports.CellValueChanging += propReports_CellValueChanging;
            propReports.CellValueChanged += propReports_CellValueChanged;
            // 
            // fmPropReports
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(815, 768);
            Controls.Add(propReports);
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "fmPropReports";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Propiedades de los Reportes";
            FormClosing += fmPropReports_FormClosing;
            ((System.ComponentModel.ISupportInitialize)propReports).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraVerticalGrid.PropertyGridControl propReports;
    }
}