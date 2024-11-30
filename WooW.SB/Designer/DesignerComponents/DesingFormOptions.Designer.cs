namespace WooW.SB.Designer.DesignerComponents
{
    partial class DesingFormOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesingFormOptions));
            hiddenItemsList1 = new DevExpress.XtraLayout.Customization.Controls.HiddenItemsList();
            tbgOptions = new System.Windows.Forms.TabControl();
            tabHideItems = new System.Windows.Forms.TabPage();
            tabProperties = new System.Windows.Forms.TabPage();
            propDesigner = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            tabPageStyle = new System.Windows.Forms.TabPage();
            pnlThemes = new DevExpress.XtraEditors.PanelControl();
            tabPageCustomButtons = new System.Windows.Forms.TabPage();
            pnlControlButtons = new DevExpress.XtraEditors.PanelControl();
            propButton = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            pnlButtonsAddRemove = new DevExpress.XtraEditors.PanelControl();
            btnDownButton = new DevExpress.XtraEditors.SimpleButton();
            btnUpButton = new DevExpress.XtraEditors.SimpleButton();
            btnDeleteButton = new DevExpress.XtraEditors.SimpleButton();
            btnAddButtons = new DevExpress.XtraEditors.SimpleButton();
            pnlButtonsContainer = new DevExpress.XtraEditors.PanelControl();
            pnlButtons = new DevExpress.XtraEditors.XtraScrollableControl();
            tabNoAssignedControls = new System.Windows.Forms.TabPage();
            grdNotAssignedControls = new DevExpress.XtraGrid.GridControl();
            grdViewNotAssignedControls = new DevExpress.XtraGrid.Views.Grid.GridView();
            lblSelectedControl = new System.Windows.Forms.Label();
            pnlControl = new System.Windows.Forms.Panel();
            pnlBody = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)hiddenItemsList1).BeginInit();
            tbgOptions.SuspendLayout();
            tabHideItems.SuspendLayout();
            tabProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)propDesigner).BeginInit();
            tabPageStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlThemes).BeginInit();
            tabPageCustomButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlControlButtons).BeginInit();
            pnlControlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)propButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlButtonsAddRemove).BeginInit();
            pnlButtonsAddRemove.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlButtonsContainer).BeginInit();
            pnlButtonsContainer.SuspendLayout();
            tabNoAssignedControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdNotAssignedControls).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdViewNotAssignedControls).BeginInit();
            pnlControl.SuspendLayout();
            pnlBody.SuspendLayout();
            SuspendLayout();
            // 
            // hiddenItemsList1
            // 
            hiddenItemsList1.Dock = System.Windows.Forms.DockStyle.Fill;
            hiddenItemsList1.Location = new System.Drawing.Point(6, 6);
            hiddenItemsList1.Margin = new System.Windows.Forms.Padding(6);
            hiddenItemsList1.Name = "hiddenItemsList1";
            hiddenItemsList1.Size = new System.Drawing.Size(800, 972);
            // 
            // tbgOptions
            // 
            tbgOptions.Controls.Add(tabHideItems);
            tbgOptions.Controls.Add(tabProperties);
            tbgOptions.Controls.Add(tabPageStyle);
            tbgOptions.Controls.Add(tabPageCustomButtons);
            tbgOptions.Controls.Add(tabNoAssignedControls);
            tbgOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            tbgOptions.Location = new System.Drawing.Point(0, 0);
            tbgOptions.Margin = new System.Windows.Forms.Padding(6);
            tbgOptions.Name = "tbgOptions";
            tbgOptions.SelectedIndex = 0;
            tbgOptions.Size = new System.Drawing.Size(828, 1031);
            tbgOptions.TabIndex = 4;
            // 
            // tabHideItems
            // 
            tabHideItems.Controls.Add(hiddenItemsList1);
            tabHideItems.Location = new System.Drawing.Point(8, 39);
            tabHideItems.Margin = new System.Windows.Forms.Padding(6);
            tabHideItems.Name = "tabHideItems";
            tabHideItems.Padding = new System.Windows.Forms.Padding(6);
            tabHideItems.Size = new System.Drawing.Size(812, 984);
            tabHideItems.TabIndex = 0;
            tabHideItems.Text = "Items";
            tabHideItems.UseVisualStyleBackColor = true;
            // 
            // tabProperties
            // 
            tabProperties.Controls.Add(propDesigner);
            tabProperties.Location = new System.Drawing.Point(8, 39);
            tabProperties.Margin = new System.Windows.Forms.Padding(6);
            tabProperties.Name = "tabProperties";
            tabProperties.Size = new System.Drawing.Size(812, 984);
            tabProperties.TabIndex = 2;
            tabProperties.Text = "Propiedades";
            tabProperties.UseVisualStyleBackColor = true;
            // 
            // propDesigner
            // 
            propDesigner.BandsInterval = 4;
            propDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            propDesigner.Location = new System.Drawing.Point(0, 0);
            propDesigner.Margin = new System.Windows.Forms.Padding(6);
            propDesigner.Name = "propDesigner";
            propDesigner.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            propDesigner.OptionsView.FixedLineWidth = 4;
            propDesigner.OptionsView.MinRowAutoHeight = 19;
            propDesigner.Size = new System.Drawing.Size(812, 984);
            propDesigner.TabIndex = 0;
            propDesigner.CellValueChanging += propDesigner_CellValueChanging;
            propDesigner.CellValueChanged += propDesigner_CellValueChanged;
            // 
            // tabPageStyle
            // 
            tabPageStyle.Controls.Add(pnlThemes);
            tabPageStyle.Location = new System.Drawing.Point(8, 39);
            tabPageStyle.Margin = new System.Windows.Forms.Padding(6);
            tabPageStyle.Name = "tabPageStyle";
            tabPageStyle.Padding = new System.Windows.Forms.Padding(6);
            tabPageStyle.Size = new System.Drawing.Size(812, 984);
            tabPageStyle.TabIndex = 3;
            tabPageStyle.Text = "Temas";
            tabPageStyle.UseVisualStyleBackColor = true;
            // 
            // pnlThemes
            // 
            pnlThemes.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlThemes.Location = new System.Drawing.Point(6, 6);
            pnlThemes.Margin = new System.Windows.Forms.Padding(6);
            pnlThemes.Name = "pnlThemes";
            pnlThemes.Size = new System.Drawing.Size(800, 972);
            pnlThemes.TabIndex = 0;
            // 
            // tabPageCustomButtons
            // 
            tabPageCustomButtons.Controls.Add(pnlControlButtons);
            tabPageCustomButtons.Controls.Add(pnlButtonsContainer);
            tabPageCustomButtons.Location = new System.Drawing.Point(8, 39);
            tabPageCustomButtons.Margin = new System.Windows.Forms.Padding(4);
            tabPageCustomButtons.Name = "tabPageCustomButtons";
            tabPageCustomButtons.Size = new System.Drawing.Size(812, 984);
            tabPageCustomButtons.TabIndex = 4;
            tabPageCustomButtons.Text = "Botones";
            tabPageCustomButtons.UseVisualStyleBackColor = true;
            // 
            // pnlControlButtons
            // 
            pnlControlButtons.Controls.Add(propButton);
            pnlControlButtons.Controls.Add(pnlButtonsAddRemove);
            pnlControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlControlButtons.Location = new System.Drawing.Point(0, 421);
            pnlControlButtons.Margin = new System.Windows.Forms.Padding(4);
            pnlControlButtons.Name = "pnlControlButtons";
            pnlControlButtons.Size = new System.Drawing.Size(812, 563);
            pnlControlButtons.TabIndex = 1;
            // 
            // propButton
            // 
            propButton.Dock = System.Windows.Forms.DockStyle.Fill;
            propButton.Location = new System.Drawing.Point(3, 72);
            propButton.Margin = new System.Windows.Forms.Padding(4);
            propButton.Name = "propButton";
            propButton.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            propButton.OptionsView.MinRowAutoHeight = 19;
            propButton.Size = new System.Drawing.Size(806, 488);
            propButton.TabIndex = 1;
            propButton.CellValueChanged += propButton_CellValueChanged;
            // 
            // pnlButtonsAddRemove
            // 
            pnlButtonsAddRemove.Controls.Add(btnDownButton);
            pnlButtonsAddRemove.Controls.Add(btnUpButton);
            pnlButtonsAddRemove.Controls.Add(btnDeleteButton);
            pnlButtonsAddRemove.Controls.Add(btnAddButtons);
            pnlButtonsAddRemove.Dock = System.Windows.Forms.DockStyle.Top;
            pnlButtonsAddRemove.Location = new System.Drawing.Point(3, 3);
            pnlButtonsAddRemove.Margin = new System.Windows.Forms.Padding(4);
            pnlButtonsAddRemove.Name = "pnlButtonsAddRemove";
            pnlButtonsAddRemove.Size = new System.Drawing.Size(806, 69);
            pnlButtonsAddRemove.TabIndex = 0;
            // 
            // btnDownButton
            // 
            btnDownButton.Dock = System.Windows.Forms.DockStyle.Left;
            btnDownButton.Enabled = false;
            btnDownButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnDownButton.ImageOptions.Image");
            btnDownButton.Location = new System.Drawing.Point(563, 3);
            btnDownButton.Margin = new System.Windows.Forms.Padding(4);
            btnDownButton.Name = "btnDownButton";
            btnDownButton.Size = new System.Drawing.Size(160, 63);
            btnDownButton.TabIndex = 3;
            btnDownButton.Text = "Bajar";
            btnDownButton.Click += btnDownButton_Click;
            // 
            // btnUpButton
            // 
            btnUpButton.Dock = System.Windows.Forms.DockStyle.Left;
            btnUpButton.Enabled = false;
            btnUpButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnUpButton.ImageOptions.Image");
            btnUpButton.Location = new System.Drawing.Point(403, 3);
            btnUpButton.Margin = new System.Windows.Forms.Padding(4);
            btnUpButton.Name = "btnUpButton";
            btnUpButton.Size = new System.Drawing.Size(160, 63);
            btnUpButton.TabIndex = 2;
            btnUpButton.Text = "Subir";
            btnUpButton.Click += btnUpButton_Click;
            // 
            // btnDeleteButton
            // 
            btnDeleteButton.Dock = System.Windows.Forms.DockStyle.Left;
            btnDeleteButton.Enabled = false;
            btnDeleteButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnDeleteButton.ImageOptions.Image");
            btnDeleteButton.Location = new System.Drawing.Point(203, 3);
            btnDeleteButton.Margin = new System.Windows.Forms.Padding(4);
            btnDeleteButton.Name = "btnDeleteButton";
            btnDeleteButton.Size = new System.Drawing.Size(200, 63);
            btnDeleteButton.TabIndex = 1;
            btnDeleteButton.Text = "Eliminar Botón";
            btnDeleteButton.Click += simpleButton1_Click;
            // 
            // btnAddButtons
            // 
            btnAddButtons.Dock = System.Windows.Forms.DockStyle.Left;
            btnAddButtons.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAddButtons.ImageOptions.Image");
            btnAddButtons.Location = new System.Drawing.Point(3, 3);
            btnAddButtons.Margin = new System.Windows.Forms.Padding(4);
            btnAddButtons.Name = "btnAddButtons";
            btnAddButtons.Size = new System.Drawing.Size(200, 63);
            btnAddButtons.TabIndex = 0;
            btnAddButtons.Text = "Agregar Botón";
            btnAddButtons.Click += btnAddButtons_Click;
            // 
            // pnlButtonsContainer
            // 
            pnlButtonsContainer.Controls.Add(pnlButtons);
            pnlButtonsContainer.Dock = System.Windows.Forms.DockStyle.Top;
            pnlButtonsContainer.Location = new System.Drawing.Point(0, 0);
            pnlButtonsContainer.Margin = new System.Windows.Forms.Padding(4);
            pnlButtonsContainer.Name = "pnlButtonsContainer";
            pnlButtonsContainer.Size = new System.Drawing.Size(812, 421);
            pnlButtonsContainer.TabIndex = 0;
            // 
            // pnlButtons
            // 
            pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlButtons.Location = new System.Drawing.Point(3, 3);
            pnlButtons.Margin = new System.Windows.Forms.Padding(6);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new System.Drawing.Size(806, 415);
            pnlButtons.TabIndex = 0;
            // 
            // tabNoAssignedControls
            // 
            tabNoAssignedControls.Controls.Add(grdNotAssignedControls);
            tabNoAssignedControls.Location = new System.Drawing.Point(8, 39);
            tabNoAssignedControls.Name = "tabNoAssignedControls";
            tabNoAssignedControls.Padding = new System.Windows.Forms.Padding(3);
            tabNoAssignedControls.Size = new System.Drawing.Size(812, 984);
            tabNoAssignedControls.TabIndex = 5;
            tabNoAssignedControls.Text = "No asignado";
            tabNoAssignedControls.UseVisualStyleBackColor = true;
            // 
            // grdNotAssignedControls
            // 
            grdNotAssignedControls.Dock = System.Windows.Forms.DockStyle.Fill;
            grdNotAssignedControls.Location = new System.Drawing.Point(3, 3);
            grdNotAssignedControls.MainView = grdViewNotAssignedControls;
            grdNotAssignedControls.Name = "grdNotAssignedControls";
            grdNotAssignedControls.Size = new System.Drawing.Size(806, 978);
            grdNotAssignedControls.TabIndex = 0;
            grdNotAssignedControls.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdViewNotAssignedControls });
            // 
            // grdViewNotAssignedControls
            // 
            grdViewNotAssignedControls.GridControl = grdNotAssignedControls;
            grdViewNotAssignedControls.Name = "grdViewNotAssignedControls";
            grdViewNotAssignedControls.OptionsMenu.EnableGroupRowMenu = true;
            grdViewNotAssignedControls.OptionsView.ShowGroupPanel = false;
            // 
            // lblSelectedControl
            // 
            lblSelectedControl.AutoSize = true;
            lblSelectedControl.Dock = System.Windows.Forms.DockStyle.Fill;
            lblSelectedControl.Location = new System.Drawing.Point(0, 0);
            lblSelectedControl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblSelectedControl.Name = "lblSelectedControl";
            lblSelectedControl.Size = new System.Drawing.Size(266, 25);
            lblSelectedControl.TabIndex = 0;
            lblSelectedControl.Text = "Control seleccionado: None";
            // 
            // pnlControl
            // 
            pnlControl.Controls.Add(lblSelectedControl);
            pnlControl.Dock = System.Windows.Forms.DockStyle.Top;
            pnlControl.Location = new System.Drawing.Point(0, 0);
            pnlControl.Margin = new System.Windows.Forms.Padding(6);
            pnlControl.Name = "pnlControl";
            pnlControl.Size = new System.Drawing.Size(828, 31);
            pnlControl.TabIndex = 8;
            // 
            // pnlBody
            // 
            pnlBody.Controls.Add(tbgOptions);
            pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlBody.Location = new System.Drawing.Point(0, 31);
            pnlBody.Margin = new System.Windows.Forms.Padding(6);
            pnlBody.Name = "pnlBody";
            pnlBody.Size = new System.Drawing.Size(828, 1031);
            pnlBody.TabIndex = 5;
            // 
            // DesingFormOptions
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(828, 1062);
            Controls.Add(pnlBody);
            Controls.Add(pnlControl);
            Margin = new System.Windows.Forms.Padding(6);
            MinimumSize = new System.Drawing.Size(224, 112);
            Name = "DesingFormOptions";
            Text = "DesingFormOptions";
            ((System.ComponentModel.ISupportInitialize)hiddenItemsList1).EndInit();
            tbgOptions.ResumeLayout(false);
            tabHideItems.ResumeLayout(false);
            tabProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)propDesigner).EndInit();
            tabPageStyle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlThemes).EndInit();
            tabPageCustomButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlControlButtons).EndInit();
            pnlControlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)propButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlButtonsAddRemove).EndInit();
            pnlButtonsAddRemove.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlButtonsContainer).EndInit();
            pnlButtonsContainer.ResumeLayout(false);
            tabNoAssignedControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdNotAssignedControls).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdViewNotAssignedControls).EndInit();
            pnlControl.ResumeLayout(false);
            pnlControl.PerformLayout();
            pnlBody.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraLayout.Customization.Controls.HiddenItemsList hiddenItemsList1;
        private System.Windows.Forms.TabControl tbgOptions;
        private System.Windows.Forms.TabPage tabHideItems;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.TabPage tabPageStyle;
        private DevExpress.XtraEditors.PanelControl pnlThemes;
        private System.Windows.Forms.Label lblSelectedControl;
        private System.Windows.Forms.Panel pnlControl;
        private System.Windows.Forms.Panel pnlBody;
        private DevExpress.XtraVerticalGrid.PropertyGridControl propDesigner;
        private System.Windows.Forms.TabPage tabPageCustomButtons;
        private DevExpress.XtraEditors.PanelControl pnlControlButtons;
        private DevExpress.XtraEditors.PanelControl pnlButtonsAddRemove;
        private DevExpress.XtraEditors.SimpleButton btnDeleteButton;
        private DevExpress.XtraEditors.SimpleButton btnAddButtons;
        private DevExpress.XtraEditors.PanelControl pnlButtonsContainer;
        private DevExpress.XtraVerticalGrid.PropertyGridControl propButton;
        private DevExpress.XtraEditors.XtraScrollableControl pnlButtons;
        private DevExpress.XtraEditors.SimpleButton btnDownButton;
        private DevExpress.XtraEditors.SimpleButton btnUpButton;
        private System.Windows.Forms.TabPage tabNoAssignedControls;
        private DevExpress.XtraGrid.GridControl grdNotAssignedControls;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewNotAssignedControls;
    }
}