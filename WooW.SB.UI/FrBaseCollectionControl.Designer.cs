namespace WooW.SB.UI
{
    partial class FrBaseCollectionControl
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrBaseCollectionControl));
            imgButtons = new DevExpress.Utils.ImageCollection(components);
            splitPanel = new DevExpress.XtraEditors.SplitContainerControl();
            buAgregar = new DevExpress.XtraEditors.DropDownButton();
            buCargar = new DevExpress.XtraEditors.SimpleButton();
            buGuardar = new DevExpress.XtraEditors.SimpleButton();
            buCopiar = new DevExpress.XtraEditors.SimpleButton();
            buDown = new DevExpress.XtraEditors.SimpleButton();
            buUp = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            buEliminar = new DevExpress.XtraEditors.SimpleButton();
            lstItems = new DevExpress.XtraEditors.ListBoxControl();
            panPropiedades = new DevExpress.XtraEditors.PanelControl();
            pgItem = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            pdItem = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)imgButtons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitPanel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitPanel.Panel1).BeginInit();
            splitPanel.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitPanel.Panel2).BeginInit();
            splitPanel.Panel2.SuspendLayout();
            splitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lstItems).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panPropiedades).BeginInit();
            panPropiedades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pgItem).BeginInit();
            SuspendLayout();
            // 
            // imgButtons
            // 
            imgButtons.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imgButtons.ImageStream");
            // 
            // splitPanel
            // 
            splitPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            splitPanel.Location = new System.Drawing.Point(0, 0);
            splitPanel.Name = "splitPanel";
            // 
            // splitPanel.Panel1
            // 
            splitPanel.Panel1.Controls.Add(buAgregar);
            splitPanel.Panel1.Controls.Add(buCargar);
            splitPanel.Panel1.Controls.Add(buGuardar);
            splitPanel.Panel1.Controls.Add(buCopiar);
            splitPanel.Panel1.Controls.Add(buDown);
            splitPanel.Panel1.Controls.Add(buUp);
            splitPanel.Panel1.Controls.Add(labelControl1);
            splitPanel.Panel1.Controls.Add(buEliminar);
            splitPanel.Panel1.Controls.Add(lstItems);
            splitPanel.Panel1.MinSize = 288;
            splitPanel.Panel1.Text = "Panel1";
            // 
            // splitPanel.Panel2
            // 
            splitPanel.Panel2.Controls.Add(panPropiedades);
            splitPanel.Panel2.Controls.Add(labelControl2);
            splitPanel.Panel2.Text = "Panel2";
            splitPanel.Size = new System.Drawing.Size(808, 436);
            splitPanel.SplitterPosition = 334;
            splitPanel.TabIndex = 11;
            splitPanel.Text = "splitContainerControl1";
            // 
            // buAgregar
            // 
            buAgregar.ActAsDropDown = false;
            buAgregar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buAgregar.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            buAgregar.ImageOptions.Image = Properties.Resources.Document_New;
            buAgregar.ImageOptions.ImageIndex = 2;
            buAgregar.ImageOptions.ImageList = imgButtons;
            buAgregar.Location = new System.Drawing.Point(12, 368);
            buAgregar.Name = "buAgregar";
            buAgregar.Size = new System.Drawing.Size(86, 26);
            buAgregar.TabIndex = 5;
            buAgregar.Text = "Agregar";
            buAgregar.Click += buAgregar_Click;
            // 
            // buCargar
            // 
            buCargar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buCargar.ImageOptions.Image = Properties.Resources.Document_Add;
            buCargar.ImageOptions.ImageIndex = 6;
            buCargar.ImageOptions.ImageList = imgButtons;
            buCargar.Location = new System.Drawing.Point(224, 400);
            buCargar.Name = "buCargar";
            buCargar.Size = new System.Drawing.Size(68, 26);
            buCargar.TabIndex = 13;
            buCargar.Text = "Cargar";
            buCargar.Click += buCargar_Click;
            // 
            // buGuardar
            // 
            buGuardar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buGuardar.ImageOptions.Image = Properties.Resources.Document_Save;
            buGuardar.ImageOptions.ImageIndex = 5;
            buGuardar.ImageOptions.ImageList = imgButtons;
            buGuardar.Location = new System.Drawing.Point(150, 400);
            buGuardar.Name = "buGuardar";
            buGuardar.Size = new System.Drawing.Size(68, 26);
            buGuardar.TabIndex = 12;
            buGuardar.Text = "Guardar";
            buGuardar.Click += buGuardar_Click;
            // 
            // buCopiar
            // 
            buCopiar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buCopiar.ImageOptions.Image = Properties.Resources.Document_Details;
            buCopiar.ImageOptions.ImageIndex = 4;
            buCopiar.ImageOptions.ImageList = imgButtons;
            buCopiar.Location = new System.Drawing.Point(224, 368);
            buCopiar.Name = "buCopiar";
            buCopiar.Size = new System.Drawing.Size(68, 26);
            buCopiar.TabIndex = 11;
            buCopiar.Text = "Copiar";
            buCopiar.Click += buCopiar_Click;
            // 
            // buDown
            // 
            buDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buDown.ImageOptions.Image = Properties.Resources.mm_Down;
            buDown.ImageOptions.ImageIndex = 1;
            buDown.ImageOptions.ImageList = imgButtons;
            buDown.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            buDown.Location = new System.Drawing.Point(297, 188);
            buDown.Name = "buDown";
            buDown.Size = new System.Drawing.Size(30, 30);
            buDown.TabIndex = 10;
            buDown.Click += buDown_Click;
            // 
            // buUp
            // 
            buUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buUp.ImageOptions.Image = Properties.Resources.mm_Up;
            buUp.ImageOptions.ImageIndex = 0;
            buUp.ImageOptions.ImageList = imgButtons;
            buUp.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            buUp.Location = new System.Drawing.Point(297, 152);
            buUp.Name = "buUp";
            buUp.Size = new System.Drawing.Size(30, 30);
            buUp.TabIndex = 9;
            buUp.Click += buUp_Click;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(12, 5);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(49, 13);
            labelControl1.TabIndex = 8;
            labelControl1.Text = "Miembros:";
            // 
            // buEliminar
            // 
            buEliminar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buEliminar.ImageOptions.Image = Properties.Resources.Document_Remove;
            buEliminar.ImageOptions.ImageIndex = 3;
            buEliminar.ImageOptions.ImageList = imgButtons;
            buEliminar.Location = new System.Drawing.Point(150, 368);
            buEliminar.Name = "buEliminar";
            buEliminar.Size = new System.Drawing.Size(68, 26);
            buEliminar.TabIndex = 6;
            buEliminar.Text = "Eliminar";
            buEliminar.Click += buEliminar_Click;
            // 
            // lstItems
            // 
            lstItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lstItems.Location = new System.Drawing.Point(6, 24);
            lstItems.Name = "lstItems";
            lstItems.Size = new System.Drawing.Size(286, 338);
            lstItems.TabIndex = 4;
            lstItems.SelectedIndexChanged += lstItems_SelectedIndexChanged;
            lstItems.DrawItem += lstItems_DrawItem;
            // 
            // panPropiedades
            // 
            panPropiedades.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panPropiedades.Controls.Add(pgItem);
            panPropiedades.Controls.Add(pdItem);
            panPropiedades.Location = new System.Drawing.Point(17, 24);
            panPropiedades.Name = "panPropiedades";
            panPropiedades.Size = new System.Drawing.Size(431, 398);
            panPropiedades.TabIndex = 10;
            // 
            // pgItem
            // 
            pgItem.Dock = System.Windows.Forms.DockStyle.Fill;
            pgItem.Location = new System.Drawing.Point(2, 2);
            pgItem.Name = "pgItem";
            pgItem.OptionsBehavior.SmartExpand = false;
            pgItem.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            pgItem.Size = new System.Drawing.Size(427, 315);
            pgItem.TabIndex = 7;
            pgItem.EditorKeyPress += pgItem_EditorKeyPress;
            // 
            // pdItem
            // 
            pdItem.Dock = System.Windows.Forms.DockStyle.Bottom;
            pdItem.Location = new System.Drawing.Point(2, 317);
            pdItem.Name = "pdItem";
            pdItem.PropertyGrid = pgItem;
            pdItem.Size = new System.Drawing.Size(427, 79);
            pdItem.TabIndex = 0;
            pdItem.TabStop = false;
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(17, 5);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(63, 13);
            labelControl2.TabIndex = 9;
            labelControl2.Text = "Propiedades:";
            // 
            // FrBaseCollectionControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitPanel);
            Name = "FrBaseCollectionControl";
            Size = new System.Drawing.Size(808, 436);
            ((System.ComponentModel.ISupportInitialize)imgButtons).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitPanel.Panel1).EndInit();
            splitPanel.Panel1.ResumeLayout(false);
            splitPanel.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitPanel.Panel2).EndInit();
            splitPanel.Panel2.ResumeLayout(false);
            splitPanel.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitPanel).EndInit();
            splitPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lstItems).EndInit();
            ((System.ComponentModel.ISupportInitialize)panPropiedades).EndInit();
            panPropiedades.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pgItem).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.Utils.ImageCollection imgButtons;
        private DevExpress.XtraEditors.SplitContainerControl splitPanel;
        private DevExpress.XtraEditors.SimpleButton buCargar;
        private DevExpress.XtraEditors.SimpleButton buGuardar;
        private DevExpress.XtraEditors.SimpleButton buCopiar;
        private DevExpress.XtraEditors.SimpleButton buDown;
        private DevExpress.XtraEditors.SimpleButton buUp;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton buEliminar;
        private DevExpress.XtraEditors.ListBoxControl lstItems;
        private DevExpress.XtraEditors.PanelControl panPropiedades;
        private DevExpress.XtraVerticalGrid.PropertyGridControl pgItem;
        private DevExpress.XtraVerticalGrid.PropertyDescriptionControl pdItem;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.DropDownButton buAgregar;

    }
}
