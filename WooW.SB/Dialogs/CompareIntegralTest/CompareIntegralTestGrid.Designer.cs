namespace WooW.SB.Dialogs.CompareIntegralTest
{
    partial class CompareIntegralTestGrid
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
            grdElementoPrueba = new DevExpress.XtraGrid.GridControl();
            grdElementoPruebaView = new DevExpress.XtraGrid.Views.Grid.GridView();
            pnlDatos = new DevExpress.XtraEditors.PanelControl();
            txtOrdenDeEjecucion = new DevExpress.XtraEditors.SpinEdit();
            labelControl9 = new DevExpress.XtraEditors.LabelControl();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            txtDescripcion = new DevExpress.XtraEditors.MemoEdit();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            txtProceso = new DevExpress.XtraEditors.ComboBoxEdit();
            txtWorkItem = new DevExpress.XtraEditors.TextEdit();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            txtPrueba = new DevExpress.XtraEditors.TextEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)grdElementoPrueba).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdElementoPruebaView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlDatos).BeginInit();
            pnlDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtOrdenDeEjecucion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtDescripcion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtProceso.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtWorkItem.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtPrueba.Properties).BeginInit();
            SuspendLayout();
            // 
            // grdElementoPrueba
            // 
            grdElementoPrueba.Dock = System.Windows.Forms.DockStyle.Fill;
            grdElementoPrueba.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(6);
            grdElementoPrueba.Location = new System.Drawing.Point(0, 185);
            grdElementoPrueba.MainView = grdElementoPruebaView;
            grdElementoPrueba.Margin = new System.Windows.Forms.Padding(6);
            grdElementoPrueba.Name = "grdElementoPrueba";
            grdElementoPrueba.Size = new System.Drawing.Size(2172, 1048);
            grdElementoPrueba.TabIndex = 4;
            grdElementoPrueba.UseEmbeddedNavigator = true;
            grdElementoPrueba.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdElementoPruebaView });
            // 
            // grdElementoPruebaView
            // 
            grdElementoPruebaView.DetailHeight = 673;
            grdElementoPruebaView.GridControl = grdElementoPrueba;
            grdElementoPruebaView.LevelIndent = 0;
            grdElementoPruebaView.Name = "grdElementoPruebaView";
            grdElementoPruebaView.OptionsBehavior.Editable = false;
            grdElementoPruebaView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            grdElementoPruebaView.OptionsView.ShowGroupPanel = false;
            grdElementoPruebaView.PreviewIndent = 0;
            grdElementoPruebaView.CustomDrawCell += grdElementoPruebaView_CustomDrawCell;
            // 
            // pnlDatos
            // 
            pnlDatos.Controls.Add(txtOrdenDeEjecucion);
            pnlDatos.Controls.Add(labelControl9);
            pnlDatos.Controls.Add(labelControl3);
            pnlDatos.Controls.Add(txtDescripcion);
            pnlDatos.Controls.Add(labelControl4);
            pnlDatos.Controls.Add(txtProceso);
            pnlDatos.Controls.Add(txtWorkItem);
            pnlDatos.Controls.Add(labelControl2);
            pnlDatos.Controls.Add(txtPrueba);
            pnlDatos.Controls.Add(labelControl1);
            pnlDatos.Dock = System.Windows.Forms.DockStyle.Top;
            pnlDatos.Location = new System.Drawing.Point(0, 0);
            pnlDatos.Margin = new System.Windows.Forms.Padding(6);
            pnlDatos.Name = "pnlDatos";
            pnlDatos.Size = new System.Drawing.Size(2172, 185);
            pnlDatos.TabIndex = 3;
            // 
            // txtOrdenDeEjecucion
            // 
            txtOrdenDeEjecucion.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            txtOrdenDeEjecucion.Location = new System.Drawing.Point(522, 127);
            txtOrdenDeEjecucion.Margin = new System.Windows.Forms.Padding(6);
            txtOrdenDeEjecucion.Name = "txtOrdenDeEjecucion";
            txtOrdenDeEjecucion.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            txtOrdenDeEjecucion.Properties.ReadOnly = true;
            txtOrdenDeEjecucion.Properties.UseReadOnlyAppearance = false;
            txtOrdenDeEjecucion.Size = new System.Drawing.Size(156, 40);
            txtOrdenDeEjecucion.TabIndex = 25;
            // 
            // labelControl9
            // 
            labelControl9.Location = new System.Drawing.Point(350, 135);
            labelControl9.Margin = new System.Windows.Forms.Padding(6);
            labelControl9.Name = "labelControl9";
            labelControl9.Size = new System.Drawing.Size(155, 25);
            labelControl9.TabIndex = 24;
            labelControl9.Text = "Orden en Batch:";
            // 
            // labelControl3
            // 
            labelControl3.Location = new System.Drawing.Point(708, 33);
            labelControl3.Margin = new System.Windows.Forms.Padding(6);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(114, 25);
            labelControl3.TabIndex = 13;
            labelControl3.Text = "Descripción:";
            // 
            // txtDescripcion
            // 
            txtDescripcion.Location = new System.Drawing.Point(836, 27);
            txtDescripcion.Margin = new System.Windows.Forms.Padding(6);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Properties.ReadOnly = true;
            txtDescripcion.Properties.UseReadOnlyAppearance = false;
            txtDescripcion.Size = new System.Drawing.Size(742, 138);
            txtDescripcion.TabIndex = 12;
            // 
            // labelControl4
            // 
            labelControl4.Location = new System.Drawing.Point(64, 133);
            labelControl4.Margin = new System.Windows.Forms.Padding(6);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(79, 25);
            labelControl4.TabIndex = 11;
            labelControl4.Text = "Proceso:";
            // 
            // txtProceso
            // 
            txtProceso.Location = new System.Drawing.Point(160, 127);
            txtProceso.Margin = new System.Windows.Forms.Padding(6);
            txtProceso.Name = "txtProceso";
            txtProceso.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            txtProceso.Properties.ReadOnly = true;
            txtProceso.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtProceso.Properties.UseReadOnlyAppearance = false;
            txtProceso.Size = new System.Drawing.Size(178, 40);
            txtProceso.TabIndex = 10;
            // 
            // txtWorkItem
            // 
            txtWorkItem.Location = new System.Drawing.Point(160, 77);
            txtWorkItem.Margin = new System.Windows.Forms.Padding(6);
            txtWorkItem.Name = "txtWorkItem";
            txtWorkItem.Properties.ReadOnly = true;
            txtWorkItem.Properties.UseReadOnlyAppearance = false;
            txtWorkItem.Size = new System.Drawing.Size(518, 40);
            txtWorkItem.TabIndex = 5;
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(46, 83);
            labelControl2.Margin = new System.Windows.Forms.Padding(6);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(99, 25);
            labelControl2.TabIndex = 6;
            labelControl2.Text = "WorkItem:";
            // 
            // txtPrueba
            // 
            txtPrueba.Location = new System.Drawing.Point(160, 27);
            txtPrueba.Margin = new System.Windows.Forms.Padding(6);
            txtPrueba.Name = "txtPrueba";
            txtPrueba.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegularMaskManager));
            txtPrueba.Properties.MaskSettings.Set("mask", "[A-Z][a-zA-Z0-9]*");
            txtPrueba.Properties.ReadOnly = true;
            txtPrueba.Properties.UseReadOnlyAppearance = false;
            txtPrueba.Size = new System.Drawing.Size(518, 40);
            txtPrueba.TabIndex = 3;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(66, 33);
            labelControl1.Margin = new System.Windows.Forms.Padding(6);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(81, 25);
            labelControl1.TabIndex = 4;
            labelControl1.Text = "Nombre:";
            // 
            // CompareIntegralTestGrid
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(grdElementoPrueba);
            Controls.Add(pnlDatos);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "CompareIntegralTestGrid";
            Size = new System.Drawing.Size(2172, 1233);
            ((System.ComponentModel.ISupportInitialize)grdElementoPrueba).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdElementoPruebaView).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlDatos).EndInit();
            pnlDatos.ResumeLayout(false);
            pnlDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtOrdenDeEjecucion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtDescripcion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtProceso.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtWorkItem.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtPrueba.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraGrid.GridControl grdElementoPrueba;
        private DevExpress.XtraGrid.Views.Grid.GridView grdElementoPruebaView;
        private DevExpress.XtraEditors.PanelControl pnlDatos;
        private DevExpress.XtraEditors.SpinEdit txtOrdenDeEjecucion;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.MemoEdit txtDescripcion;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.ComboBoxEdit txtProceso;
        private DevExpress.XtraEditors.TextEdit txtWorkItem;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtPrueba;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}
