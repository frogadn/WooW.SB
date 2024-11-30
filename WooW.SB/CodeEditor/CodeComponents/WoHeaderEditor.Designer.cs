namespace WooW.SB.CodeEditor.CodeComponents
{
    partial class WoHeaderEditor
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
            syeHeader = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            navigableSymbolSelector1 = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            SuspendLayout();
            // 
            // syeHeader
            // 
            syeHeader.AllowDrop = true;
            syeHeader.CanSplitHorizontally = false;
            syeHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            syeHeader.Document.IsReadOnly = true;
            syeHeader.IsLineNumberMarginVisible = true;
            syeHeader.Location = new System.Drawing.Point(2, 2);
            syeHeader.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            syeHeader.Name = "syeHeader";
            syeHeader.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            syeHeader.PrintSettings.AreCollapsedOutliningNodesAllowed = true;
            syeHeader.PrintSettings.AreColumnGuidesVisible = true;
            syeHeader.Size = new System.Drawing.Size(842, 349);
            syeHeader.TabIndex = 0;
            // 
            // navigableSymbolSelector1
            // 
            navigableSymbolSelector1.BackColor = System.Drawing.SystemColors.Control;
            navigableSymbolSelector1.Dock = System.Windows.Forms.DockStyle.Top;
            navigableSymbolSelector1.Location = new System.Drawing.Point(0, 0);
            navigableSymbolSelector1.Name = "navigableSymbolSelector1";
            navigableSymbolSelector1.Size = new System.Drawing.Size(846, 23);
            navigableSymbolSelector1.SyntaxEditor = syeHeader;
            navigableSymbolSelector1.TabIndex = 1;
            navigableSymbolSelector1.Text = "navigableSymbolSelector1";
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(syeHeader);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControl1.Location = new System.Drawing.Point(0, 23);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(846, 353);
            panelControl1.TabIndex = 2;
            // 
            // WoHeaderEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Controls.Add(navigableSymbolSelector1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "WoHeaderEditor";
            Size = new System.Drawing.Size(846, 376);
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor syeHeader;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.NavigableSymbolSelector navigableSymbolSelector1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}
