namespace WooW.SB.Dialogs.CompareDiagram
{
    partial class fmCompareDiagram
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
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            buCopiarDiagrama = new DevExpress.XtraEditors.SimpleButton();
            buCancelar = new DevExpress.XtraEditors.SimpleButton();
            buAceptar = new DevExpress.XtraEditors.SimpleButton();
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            grpDestino = new DevExpress.XtraEditors.GroupControl();
            woDiagram1 = new Config.woDiagram();
            grpFuente = new DevExpress.XtraEditors.GroupControl();
            woDiagram2 = new Config.woDiagram();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grpDestino).BeginInit();
            grpDestino.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grpFuente).BeginInit();
            grpFuente.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(buCopiarDiagrama);
            panelControl1.Controls.Add(buCancelar);
            panelControl1.Controls.Add(buAceptar);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelControl1.Location = new System.Drawing.Point(10, 1136);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(2125, 94);
            panelControl1.TabIndex = 0;
            // 
            // buCopiarDiagrama
            // 
            buCopiarDiagrama.Location = new System.Drawing.Point(769, 20);
            buCopiarDiagrama.Name = "buCopiarDiagrama";
            buCopiarDiagrama.Size = new System.Drawing.Size(307, 52);
            buCopiarDiagrama.TabIndex = 4;
            buCopiarDiagrama.Text = "Copiar Diagrama";
            buCopiarDiagrama.Click += buCopiarDiagrama_Click;
            // 
            // buCancelar
            // 
            buCancelar.Location = new System.Drawing.Point(354, 20);
            buCancelar.Name = "buCancelar";
            buCancelar.Size = new System.Drawing.Size(307, 52);
            buCancelar.TabIndex = 3;
            buCancelar.Text = "Cancelar";
            buCancelar.Click += buCancelar_Click;
            // 
            // buAceptar
            // 
            buAceptar.Enabled = false;
            buAceptar.Location = new System.Drawing.Point(21, 20);
            buAceptar.Name = "buAceptar";
            buAceptar.Size = new System.Drawing.Size(307, 52);
            buAceptar.TabIndex = 2;
            buAceptar.Text = "Aceptar";
            buAceptar.Click += buAceptar_Click;
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Location = new System.Drawing.Point(10, 10);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(grpDestino);
            splitContainerControl1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(grpFuente);
            splitContainerControl1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            splitContainerControl1.Size = new System.Drawing.Size(2125, 1126);
            splitContainerControl1.SplitterPosition = 1923;
            splitContainerControl1.TabIndex = 1;
            // 
            // grpDestino
            // 
            grpDestino.Controls.Add(woDiagram1);
            grpDestino.Dock = System.Windows.Forms.DockStyle.Fill;
            grpDestino.Location = new System.Drawing.Point(10, 10);
            grpDestino.Name = "grpDestino";
            grpDestino.Padding = new System.Windows.Forms.Padding(10);
            grpDestino.Size = new System.Drawing.Size(1903, 1106);
            grpDestino.TabIndex = 1;
            grpDestino.Text = "groupControl1";
            // 
            // woDiagram1
            // 
            woDiagram1.currDiagrama = null;
            woDiagram1.currModelo = null;
            woDiagram1.Dock = System.Windows.Forms.DockStyle.Fill;
            woDiagram1.Enabled = false;
            woDiagram1.Location = new System.Drawing.Point(13, 55);
            woDiagram1.Margin = new System.Windows.Forms.Padding(6);
            woDiagram1.MostrarPermisos = false;
            woDiagram1.Name = "woDiagram1";
            woDiagram1.Size = new System.Drawing.Size(1877, 1038);
            woDiagram1.TabIndex = 0;
            // 
            // grpFuente
            // 
            grpFuente.Controls.Add(woDiagram2);
            grpFuente.Dock = System.Windows.Forms.DockStyle.Fill;
            grpFuente.Location = new System.Drawing.Point(10, 10);
            grpFuente.Name = "grpFuente";
            grpFuente.Padding = new System.Windows.Forms.Padding(10);
            grpFuente.Size = new System.Drawing.Size(162, 1106);
            grpFuente.TabIndex = 1;
            grpFuente.Text = "groupControl1";
            // 
            // woDiagram2
            // 
            woDiagram2.currDiagrama = null;
            woDiagram2.currModelo = null;
            woDiagram2.Dock = System.Windows.Forms.DockStyle.Fill;
            woDiagram2.Enabled = false;
            woDiagram2.Location = new System.Drawing.Point(13, 55);
            woDiagram2.Margin = new System.Windows.Forms.Padding(6);
            woDiagram2.MostrarPermisos = false;
            woDiagram2.Name = "woDiagram2";
            woDiagram2.Size = new System.Drawing.Size(136, 1038);
            woDiagram2.TabIndex = 0;
            // 
            // fmCompareDiagram
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(2145, 1240);
            ControlBox = false;
            Controls.Add(splitContainerControl1);
            Controls.Add(panelControl1);
            Name = "fmCompareDiagram";
            Padding = new System.Windows.Forms.Padding(10);
            Text = "Compara Diagrama";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += fmCompareDiagram_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grpDestino).EndInit();
            grpDestino.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grpFuente).EndInit();
            grpFuente.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.SimpleButton buCopiarDiagrama;
        private DevExpress.XtraEditors.SimpleButton buCancelar;
        private DevExpress.XtraEditors.SimpleButton buAceptar;
        private Config.woDiagram woDiagram1;
        private Config.woDiagram woDiagram2;
        private DevExpress.XtraEditors.GroupControl grpDestino;
        private DevExpress.XtraEditors.GroupControl grpFuente;
    }
}