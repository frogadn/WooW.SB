namespace WooW.SB.CodeEditor.CodeComponents
{
    partial class WoBaseEditor
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
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            syeCodigo = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            errorListView = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Horizontal = false;
            splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            splitContainerControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainerControl1.Name = "splitContainerControl1";
            splitContainerControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(syeCodigo);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(errorListView);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(1121, 698);
            splitContainerControl1.SplitterPosition = 520;
            splitContainerControl1.TabIndex = 0;
            // 
            // syeCodigo
            // 
            syeCodigo.AllowDrop = true;
            syeCodigo.CanSplitHorizontally = false;
            syeCodigo.Cursor = System.Windows.Forms.Cursors.IBeam;
            syeCodigo.Dock = System.Windows.Forms.DockStyle.Fill;
            syeCodigo.IsCurrentLineHighlightingEnabled = true;
            syeCodigo.IsLineNumberMarginVisible = true;
            syeCodigo.Location = new System.Drawing.Point(0, 0);
            syeCodigo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            syeCodigo.Name = "syeCodigo";
            syeCodigo.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            syeCodigo.Size = new System.Drawing.Size(1121, 520);
            syeCodigo.TabIndex = 2;
            syeCodigo.DocumentParseDataChanged += OnCodeEditorDocumentParseDataChanged;
            syeCodigo.UserInterfaceUpdate += OnCodeEditorUserInterfaceUpdate;
            // 
            // errorListView
            // 
            errorListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            errorListView.Dock = System.Windows.Forms.DockStyle.Fill;
            errorListView.Location = new System.Drawing.Point(0, 0);
            errorListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            errorListView.Name = "errorListView";
            errorListView.Size = new System.Drawing.Size(1121, 168);
            errorListView.TabIndex = 3;
            errorListView.UseCompatibleStateImageBehavior = false;
            errorListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Line";
            columnHeader1.Width = 40;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Char";
            columnHeader2.Width = 40;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Description";
            columnHeader3.Width = 450;
            // 
            // WoBaseEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainerControl1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "WoBaseEditor";
            Size = new System.Drawing.Size(1121, 698);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.ListView errorListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor syeCodigo;
    }
}
