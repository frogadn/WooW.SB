namespace WooW.SB.Config.Editors
{
    partial class woConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(woConsole));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bsiConsoleSettings = new DevExpress.XtraBars.BarSubItem();
            bciShowWarnings = new DevExpress.XtraBars.BarCheckItem();
            bciErrores = new DevExpress.XtraBars.BarCheckItem();
            bciShowMessages = new DevExpress.XtraBars.BarCheckItem();
            bciShowSuccess = new DevExpress.XtraBars.BarCheckItem();
            btnClearConsole = new DevExpress.XtraBars.BarButtonItem();
            rbpConsole = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgConsole = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            consoleData = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, bsiConsoleSettings, bciShowWarnings, bciErrores, bciShowMessages, bciShowSuccess, btnClearConsole });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.MaxItemId = 7;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { rbpConsole });
            ribbonControl1.Size = new System.Drawing.Size(1459, 292);
            // 
            // bsiConsoleSettings
            // 
            bsiConsoleSettings.Caption = "Configuración De Consola";
            bsiConsoleSettings.Id = 1;
            bsiConsoleSettings.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bsiConsoleSettings.ImageOptions.Image");
            bsiConsoleSettings.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bsiConsoleSettings.ImageOptions.LargeImage");
            bsiConsoleSettings.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(bciShowWarnings), new DevExpress.XtraBars.LinkPersistInfo(bciErrores), new DevExpress.XtraBars.LinkPersistInfo(bciShowMessages), new DevExpress.XtraBars.LinkPersistInfo(bciShowSuccess) });
            bsiConsoleSettings.Name = "bsiConsoleSettings";
            // 
            // bciShowWarnings
            // 
            bciShowWarnings.BindableChecked = true;
            bciShowWarnings.Caption = "Mostrar Warnings";
            bciShowWarnings.Checked = true;
            bciShowWarnings.Id = 2;
            bciShowWarnings.Name = "bciShowWarnings";
            bciShowWarnings.CheckedChanged += bciShowWarnings_CheckedChanged;
            // 
            // bciErrores
            // 
            bciErrores.BindableChecked = true;
            bciErrores.Caption = "Mostrar Errores";
            bciErrores.Checked = true;
            bciErrores.Id = 3;
            bciErrores.Name = "bciErrores";
            bciErrores.CheckedChanged += bciErrores_CheckedChanged;
            // 
            // bciShowMessages
            // 
            bciShowMessages.BindableChecked = true;
            bciShowMessages.Caption = "Mostrar Mensajes";
            bciShowMessages.Checked = true;
            bciShowMessages.Id = 4;
            bciShowMessages.Name = "bciShowMessages";
            bciShowMessages.CheckedChanged += bciShowMessages_CheckedChanged;
            // 
            // bciShowSuccess
            // 
            bciShowSuccess.BindableChecked = true;
            bciShowSuccess.Caption = "Mostrar Exitosos";
            bciShowSuccess.Checked = true;
            bciShowSuccess.Id = 5;
            bciShowSuccess.Name = "bciShowSuccess";
            bciShowSuccess.CheckedChanged += bciShowSuccess_CheckedChanged;
            // 
            // btnClearConsole
            // 
            btnClearConsole.Caption = "Limpiar Consola";
            btnClearConsole.Id = 6;
            btnClearConsole.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnClearConsole.ImageOptions.Image");
            btnClearConsole.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnClearConsole.ImageOptions.LargeImage");
            btnClearConsole.Name = "btnClearConsole";
            btnClearConsole.ItemClick += btnClearConsole_ItemClick;
            // 
            // rbpConsole
            // 
            rbpConsole.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgConsole });
            rbpConsole.Name = "rbpConsole";
            rbpConsole.Text = "Consola";
            // 
            // rpgConsole
            // 
            rpgConsole.ItemLinks.Add(bsiConsoleSettings);
            rpgConsole.ItemLinks.Add(btnClearConsole);
            rpgConsole.Name = "rpgConsole";
            rpgConsole.Text = "Consola";
            // 
            // consoleData
            // 
            consoleData.BackColor = System.Drawing.SystemColors.WindowText;
            consoleData.Dock = System.Windows.Forms.DockStyle.Fill;
            consoleData.ForeColor = System.Drawing.SystemColors.Window;
            consoleData.Location = new System.Drawing.Point(0, 292);
            consoleData.Name = "consoleData";
            consoleData.Size = new System.Drawing.Size(1459, 527);
            consoleData.TabIndex = 1;
            consoleData.Text = "";
            // 
            // woConsole
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(consoleData);
            Controls.Add(ribbonControl1);
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Name = "woConsole";
            Size = new System.Drawing.Size(1459, 819);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage rbpConsole;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgConsole;
        private System.Windows.Forms.RichTextBox consoleData;
        private DevExpress.XtraBars.BarSubItem bsiConsoleSettings;
        private DevExpress.XtraBars.BarCheckItem bciShowWarnings;
        private DevExpress.XtraBars.BarCheckItem bciErrores;
        private DevExpress.XtraBars.BarCheckItem bciShowMessages;
        private DevExpress.XtraBars.BarCheckItem bciShowSuccess;
        private DevExpress.XtraBars.BarButtonItem btnClearConsole;
    }
}
