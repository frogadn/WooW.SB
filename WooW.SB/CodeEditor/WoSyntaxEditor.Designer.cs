using DevExpress.XtraEditors;

namespace WooW.SB.CodeEditor
{
    partial class WoSyntaxEditor
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
            sccSyntaxEditor = new DevExpress.XtraEditors.SplitContainerControl();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxEditor.Panel1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxEditor.Panel2).BeginInit();
            sccSyntaxEditor.SuspendLayout();
            SuspendLayout();
            // 
            // sccSyntaxEditor
            // 
            sccSyntaxEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            sccSyntaxEditor.Horizontal = false;
            sccSyntaxEditor.Location = new System.Drawing.Point(0, 0);
            sccSyntaxEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            sccSyntaxEditor.Name = "sccSyntaxEditor";
            sccSyntaxEditor.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            // 
            // sccSyntaxEditor.Panel1
            // 
            sccSyntaxEditor.Panel1.Text = "Panel1";
            // 
            // sccSyntaxEditor.Panel2
            // 
            sccSyntaxEditor.Panel2.Text = "Panel2";
            sccSyntaxEditor.Size = new System.Drawing.Size(1367, 820);
            sccSyntaxEditor.SplitterPosition = 202;
            sccSyntaxEditor.TabIndex = 0;
            // 
            // WoSyntaxEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(sccSyntaxEditor);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "WoSyntaxEditor";
            Size = new System.Drawing.Size(1367, 820);
            ((System.ComponentModel.ISupportInitialize)sccSyntaxEditor.Panel1).EndInit();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxEditor.Panel2).EndInit();
            ((System.ComponentModel.ISupportInitialize)sccSyntaxEditor).EndInit();
            sccSyntaxEditor.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl sccSyntaxEditor;
    }
}
