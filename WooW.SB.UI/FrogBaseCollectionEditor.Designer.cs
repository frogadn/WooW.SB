namespace WooW.SB.UI
{
    partial class FrogBaseCollectionEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrogBaseCollectionEditor));
            this.imgButtons = new DevExpress.Utils.ImageCollection(this.components);
            this.collectionEditor = new WooW.SB.UI.FrBaseCollectionControl();
            ((System.ComponentModel.ISupportInitialize)(this.imgButtons)).BeginInit();
            this.SuspendLayout();
            // 
            // imgButtons
            // 
            this.imgButtons.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imgButtons.ImageStream")));
            // 
            // collectionEditor
            // 
            this.collectionEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.collectionEditor.Location = new System.Drawing.Point(0, 0);
            this.collectionEditor.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.collectionEditor.Name = "collectionEditor";
            this.collectionEditor.Size = new System.Drawing.Size(1323, 813);
            this.collectionEditor.TabIndex = 3;
            // 
            // FrogBaseCollectionEditor
            // 
            this.ClientSize = new System.Drawing.Size(1323, 850);
            this.ControlBox = true;
            this.Controls.Add(this.collectionEditor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.Name = "FrogBaseCollectionEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Activated += new System.EventHandler(this.FrogBaseCollectionEditor_Activated);
            this.Controls.SetChildIndex(this.collectionEditor, 0);
            ((System.ComponentModel.ISupportInitialize)(this.imgButtons)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Utils.ImageCollection imgButtons;
        private FrBaseCollectionControl collectionEditor;

    }
}
