namespace WooW.SB.Dialogs
{
    partial class fmColumnsSelector
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtModelo = new DevExpress.XtraEditors.ComboBoxEdit();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.txtColumnas = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.optCombinar = new DevExpress.XtraEditors.CheckEdit();
            this.buCancelar = new DevExpress.XtraEditors.SimpleButton();
            this.buCopiar = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtFiltro = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtModelo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtColumnas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optCombinar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFiltro.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.txtFiltro);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtModelo);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(420, 72);
            this.panelControl1.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(24, 39);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(34, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Modelo";
            // 
            // txtModelo
            // 
            this.txtModelo.Location = new System.Drawing.Point(64, 36);
            this.txtModelo.Name = "txtModelo";
            this.txtModelo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtModelo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txtModelo.Size = new System.Drawing.Size(219, 20);
            this.txtModelo.TabIndex = 0;
            this.txtModelo.EditValueChanged += new System.EventHandler(this.txtModelo_EditValueChanged);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.txtColumnas);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 72);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(10);
            this.panelControl2.Size = new System.Drawing.Size(420, 401);
            this.panelControl2.TabIndex = 1;
            // 
            // txtColumnas
            // 
            this.txtColumnas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtColumnas.Location = new System.Drawing.Point(12, 12);
            this.txtColumnas.Name = "txtColumnas";
            this.txtColumnas.Size = new System.Drawing.Size(396, 377);
            this.txtColumnas.TabIndex = 0;
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.optCombinar);
            this.panelControl3.Controls.Add(this.buCancelar);
            this.panelControl3.Controls.Add(this.buCopiar);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl3.Location = new System.Drawing.Point(0, 473);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(420, 70);
            this.panelControl3.TabIndex = 2;
            // 
            // optCombinar
            // 
            this.optCombinar.Location = new System.Drawing.Point(12, 9);
            this.optCombinar.Name = "optCombinar";
            this.optCombinar.Properties.Caption = "Combinar el Nombre del modelo con el de la columna al copiar";
            this.optCombinar.Size = new System.Drawing.Size(315, 20);
            this.optCombinar.TabIndex = 2;
            // 
            // buCancelar
            // 
            this.buCancelar.Location = new System.Drawing.Point(300, 35);
            this.buCancelar.Name = "buCancelar";
            this.buCancelar.Size = new System.Drawing.Size(107, 23);
            this.buCancelar.TabIndex = 1;
            this.buCancelar.Text = "Cancelar";
            this.buCancelar.Click += new System.EventHandler(this.buCancelar_Click);
            // 
            // buCopiar
            // 
            this.buCopiar.Location = new System.Drawing.Point(12, 35);
            this.buCopiar.Name = "buCopiar";
            this.buCopiar.Size = new System.Drawing.Size(107, 23);
            this.buCopiar.TabIndex = 0;
            this.buCopiar.Text = "Copiar Columnas";
            this.buCopiar.Click += new System.EventHandler(this.buCopiar_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(34, 15);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(24, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Filtro";
            // 
            // txtFiltro
            // 
            this.txtFiltro.Location = new System.Drawing.Point(64, 12);
            this.txtFiltro.Name = "txtFiltro";
            this.txtFiltro.Size = new System.Drawing.Size(219, 20);
            this.txtFiltro.TabIndex = 4;
            this.txtFiltro.EditValueChanged += new System.EventHandler(this.txtFiltro_EditValueChanged);
            // 
            // fmColumnsSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 543);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl3);
            this.Controls.Add(this.panelControl1);
            this.MinimizeBox = false;
            this.Name = "fmColumnsSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selector de Columnas";
            this.Load += new System.EventHandler(this.fmColumnsSelector_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtModelo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtColumnas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.optCombinar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFiltro.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buCopiar;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit txtModelo;
        private DevExpress.XtraEditors.CheckEdit optCombinar;
        private DevExpress.XtraEditors.CheckedListBoxControl txtColumnas;
        private DevExpress.XtraEditors.TextEdit txtFiltro;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}