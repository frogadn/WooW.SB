namespace WooW.SB.Forms
{
    partial class fmCompareModel
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
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            objModelDestino = new Dialogs.CompareModel.CompareModelGrid();
            objModelFuente = new Dialogs.CompareModel.CompareModelGrid();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            buIgualarOrden = new DevExpress.XtraEditors.SimpleButton();
            buBorrarRenglon = new DevExpress.XtraEditors.SimpleButton();
            buCopiarPropiedades = new DevExpress.XtraEditors.SimpleButton();
            buIgualarRenglon = new DevExpress.XtraEditors.SimpleButton();
            buCopiarRenglon = new DevExpress.XtraEditors.SimpleButton();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Horizontal = false;
            splitContainerControl1.Location = new System.Drawing.Point(10, 10);
            splitContainerControl1.Margin = new System.Windows.Forms.Padding(4);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(objModelDestino);
            splitContainerControl1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(objModelFuente);
            splitContainerControl1.Panel2.Controls.Add(panelControl2);
            splitContainerControl1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitContainerControl1.Size = new System.Drawing.Size(1848, 960);
            splitContainerControl1.SplitterPosition = 877;
            splitContainerControl1.TabIndex = 1;
            // 
            // objModelDestino
            // 
            objModelDestino.Dock = System.Windows.Forms.DockStyle.Fill;
            objModelDestino.Location = new System.Drawing.Point(10, 10);
            objModelDestino.Margin = new System.Windows.Forms.Padding(2);
            objModelDestino.Name = "objModelDestino";
            objModelDestino.Size = new System.Drawing.Size(1828, 857);
            objModelDestino.TabIndex = 0;
            // 
            // objModelFuente
            // 
            objModelFuente.Dock = System.Windows.Forms.DockStyle.Fill;
            objModelFuente.Location = new System.Drawing.Point(10, 91);
            objModelFuente.Margin = new System.Windows.Forms.Padding(2);
            objModelFuente.Name = "objModelFuente";
            objModelFuente.Size = new System.Drawing.Size(1828, 0);
            objModelFuente.TabIndex = 0;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(buIgualarOrden);
            panelControl2.Controls.Add(buBorrarRenglon);
            panelControl2.Controls.Add(buCopiarPropiedades);
            panelControl2.Controls.Add(buIgualarRenglon);
            panelControl2.Controls.Add(buCopiarRenglon);
            panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl2.Location = new System.Drawing.Point(10, 10);
            panelControl2.Margin = new System.Windows.Forms.Padding(4);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(1828, 81);
            panelControl2.TabIndex = 1;
            // 
            // buIgualarOrden
            // 
            buIgualarOrden.Location = new System.Drawing.Point(1367, 12);
            buIgualarOrden.Margin = new System.Windows.Forms.Padding(4);
            buIgualarOrden.Name = "buIgualarOrden";
            buIgualarOrden.Size = new System.Drawing.Size(308, 52);
            buIgualarOrden.TabIndex = 7;
            buIgualarOrden.Text = "Igualar Orden";
            buIgualarOrden.Click += buIgualarOrden_Click;
            // 
            // buBorrarRenglon
            // 
            buBorrarRenglon.Location = new System.Drawing.Point(1035, 12);
            buBorrarRenglon.Margin = new System.Windows.Forms.Padding(4);
            buBorrarRenglon.Name = "buBorrarRenglon";
            buBorrarRenglon.Size = new System.Drawing.Size(308, 52);
            buBorrarRenglon.TabIndex = 6;
            buBorrarRenglon.Text = "Borrar Renglón";
            buBorrarRenglon.Click += buBorrarRenglon_Click;
            // 
            // buCopiarPropiedades
            // 
            buCopiarPropiedades.Location = new System.Drawing.Point(694, 12);
            buCopiarPropiedades.Margin = new System.Windows.Forms.Padding(4);
            buCopiarPropiedades.Name = "buCopiarPropiedades";
            buCopiarPropiedades.Size = new System.Drawing.Size(308, 52);
            buCopiarPropiedades.TabIndex = 5;
            buCopiarPropiedades.Text = "Copia Propiedades";
            buCopiarPropiedades.Click += buCopiarPropiedades_Click;
            // 
            // buIgualarRenglon
            // 
            buIgualarRenglon.Location = new System.Drawing.Point(358, 12);
            buIgualarRenglon.Margin = new System.Windows.Forms.Padding(4);
            buIgualarRenglon.Name = "buIgualarRenglon";
            buIgualarRenglon.Size = new System.Drawing.Size(308, 52);
            buIgualarRenglon.TabIndex = 3;
            buIgualarRenglon.Text = "Igualar Renglón";
            buIgualarRenglon.Click += buIgualarRenglon_Click;
            // 
            // buCopiarRenglon
            // 
            buCopiarRenglon.Location = new System.Drawing.Point(24, 12);
            buCopiarRenglon.Margin = new System.Windows.Forms.Padding(4);
            buCopiarRenglon.Name = "buCopiarRenglon";
            buCopiarRenglon.Size = new System.Drawing.Size(308, 52);
            buCopiarRenglon.TabIndex = 2;
            buCopiarRenglon.Text = "Copiar Renglón";
            buCopiarRenglon.Click += buCopiarRenglon_Click;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(buCancelar);
            panelControl1.Controls.Add(buAceptar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(10, 970);
            panelControl1.Margin = new System.Windows.Forms.Padding(4);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(1848, 87);
            panelControl1.TabIndex = 2;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(356, 17);
            buCancelar.Margin = new System.Windows.Forms.Padding(4);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(308, 52);
            buCancelar.TabIndex = 1;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Enabled = false;
            buAceptar.Location = new System.Drawing.Point(24, 17);
            buAceptar.Margin = new System.Windows.Forms.Padding(4);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(308, 52);
            buAceptar.TabIndex = 0;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // fmCompareModel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1868, 1067);
            ControlBox = false;
            Controls.Add(splitContainerControl1);
            Controls.Add(panelControl1);
            Margin = new System.Windows.Forms.Padding(4);
            MinimizeBox = false;
            Name = "fmCompareModel";
            Padding = new System.Windows.Forms.Padding(10);
            Text = "Compara Modelos";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += fmCompareModel_Load;
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private Dialogs.CompareModel.CompareModelGrid objModelDestino;
        private Dialogs.CompareModel.CompareModelGrid objModelFuente;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private DevExpress.XtraEditors.SimpleButton buIgualarRenglon;
        private DevExpress.XtraEditors.SimpleButton buCopiarRenglon;
        private DevExpress.XtraEditors.SimpleButton buCopiarPropiedades;
        private DevExpress.XtraEditors.SimpleButton buBorrarRenglon;
        private DevExpress.XtraEditors.SimpleButton buIgualarOrden;
    }
}