using System;
using System.Windows.Forms;
using WooW.SB.Dialogs;

namespace WooW.SB.Menus.MenusComponents
{
    partial class WoNewMenu
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelControl3 = new DevExpress.XtraEditors.PanelControl();
            panelControl4 = new DevExpress.XtraEditors.PanelControl();
            panelControl6 = new DevExpress.XtraEditors.PanelControl();
            grdRoles = new DevExpress.XtraGrid.GridControl();
            grdRolesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            label1 = new Label();
            txtMenuName = new DevExpress.XtraEditors.TextEdit();
            panelControl5 = new DevExpress.XtraEditors.PanelControl();
            btnAcept = new DevExpress.XtraEditors.SimpleButton();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            grdProcess = new DevExpress.XtraGrid.GridControl();
            grdProcessView = new DevExpress.XtraGrid.Views.Grid.GridView();
            panelControl2 = new DevExpress.XtraEditors.PanelControl();
            xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)panelControl3).BeginInit();
            panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl4).BeginInit();
            panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl6).BeginInit();
            panelControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdRoles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdRolesView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtMenuName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl5).BeginInit();
            panelControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdProcess).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdProcessView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).BeginInit();
            panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).BeginInit();
            xtraTabControl1.SuspendLayout();
            xtraTabPage1.SuspendLayout();
            xtraTabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl3
            // 
            panelControl3.Controls.Add(panelControl4);
            panelControl3.Controls.Add(panelControl1);
            panelControl3.Dock = DockStyle.Fill;
            panelControl3.Location = new System.Drawing.Point(0, 0);
            panelControl3.Name = "panelControl3";
            panelControl3.Size = new System.Drawing.Size(435, 563);
            panelControl3.TabIndex = 1;
            panelControl3.Paint += panelControl3_Paint;
            // 
            // panelControl4
            // 
            panelControl4.Controls.Add(panelControl6);
            panelControl4.Dock = DockStyle.Fill;
            panelControl4.Location = new System.Drawing.Point(2, 48);
            panelControl4.Name = "panelControl4";
            panelControl4.Size = new System.Drawing.Size(431, 513);
            panelControl4.TabIndex = 6;
            // 
            // panelControl6
            // 
            panelControl6.Controls.Add(grdRoles);
            panelControl6.Dock = DockStyle.Fill;
            panelControl6.Location = new System.Drawing.Point(2, 2);
            panelControl6.Name = "panelControl6";
            panelControl6.Size = new System.Drawing.Size(427, 509);
            panelControl6.TabIndex = 0;
            // 
            // grdRoles
            // 
            grdRoles.Dock = DockStyle.Fill;
            grdRoles.Location = new System.Drawing.Point(2, 2);
            grdRoles.MainView = grdRolesView;
            grdRoles.Name = "grdRoles";
            grdRoles.Size = new System.Drawing.Size(423, 505);
            grdRoles.TabIndex = 3;
            grdRoles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdRolesView });
            // 
            // grdRolesView
            // 
            grdRolesView.GridControl = grdRoles;
            grdRolesView.Name = "grdRolesView";
            grdRolesView.OptionsView.ShowColumnHeaders = false;
            grdRolesView.OptionsView.ShowGroupPanel = false;
            grdRolesView.OptionsView.ShowIndicator = false;
            grdRolesView.PopupMenuShowing += grdRolesView_PopupMenuShowing;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(label1);
            panelControl1.Controls.Add(txtMenuName);
            panelControl1.Dock = DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(2, 2);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(431, 46);
            panelControl1.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 2);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(90, 13);
            label1.TabIndex = 3;
            label1.Text = "Nombre del Menú";
            // 
            // txtMenuName
            // 
            txtMenuName.Dock = DockStyle.Bottom;
            txtMenuName.Location = new System.Drawing.Point(2, 24);
            txtMenuName.Name = "txtMenuName";
            txtMenuName.Size = new System.Drawing.Size(427, 20);
            txtMenuName.TabIndex = 2;
            // 
            // panelControl5
            // 
            panelControl5.Controls.Add(btnAcept);
            panelControl5.Controls.Add(btnCancel);
            panelControl5.Dock = DockStyle.Bottom;
            panelControl5.Location = new System.Drawing.Point(0, 556);
            panelControl5.Name = "panelControl5";
            panelControl5.Size = new System.Drawing.Size(437, 32);
            panelControl5.TabIndex = 7;
            // 
            // btnAcept
            // 
            btnAcept.Dock = DockStyle.Right;
            btnAcept.Location = new System.Drawing.Point(215, 2);
            btnAcept.Name = "btnAcept";
            btnAcept.Size = new System.Drawing.Size(105, 28);
            btnAcept.TabIndex = 0;
            btnAcept.Text = "Aceptar";
            btnAcept.Click += btnAcept_Click;
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Right;
            btnCancel.Location = new System.Drawing.Point(320, 2);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(115, 28);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancelar";
            btnCancel.Click += btnCancel_Click;
            // 
            // grdProcess
            // 
            grdProcess.Dock = DockStyle.Fill;
            grdProcess.Location = new System.Drawing.Point(2, 2);
            grdProcess.MainView = grdProcessView;
            grdProcess.Name = "grdProcess";
            grdProcess.Size = new System.Drawing.Size(431, 559);
            grdProcess.TabIndex = 2;
            grdProcess.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grdProcessView });
            // 
            // grdProcessView
            // 
            grdProcessView.GridControl = grdProcess;
            grdProcessView.Name = "grdProcessView";
            grdProcessView.OptionsView.ShowColumnHeaders = false;
            grdProcessView.OptionsView.ShowGroupPanel = false;
            grdProcessView.OptionsView.ShowIndicator = false;
            grdProcessView.PopupMenuShowing += grdProcessView_PopupMenuShowing;
            // 
            // panelControl2
            // 
            panelControl2.Controls.Add(grdProcess);
            panelControl2.Dock = DockStyle.Fill;
            panelControl2.Location = new System.Drawing.Point(0, 0);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new System.Drawing.Size(435, 563);
            panelControl2.TabIndex = 3;
            // 
            // xtraTabControl1
            // 
            xtraTabControl1.Dock = DockStyle.Fill;
            xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            xtraTabControl1.Name = "xtraTabControl1";
            xtraTabControl1.SelectedTabPage = xtraTabPage1;
            xtraTabControl1.Size = new System.Drawing.Size(437, 588);
            xtraTabControl1.TabIndex = 4;
            xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { xtraTabPage1, xtraTabPage2 });
            // 
            // xtraTabPage1
            // 
            xtraTabPage1.Controls.Add(panelControl3);
            xtraTabPage1.Name = "xtraTabPage1";
            xtraTabPage1.Size = new System.Drawing.Size(435, 563);
            xtraTabPage1.Text = "Roles";
            // 
            // xtraTabPage2
            // 
            xtraTabPage2.Controls.Add(panelControl2);
            xtraTabPage2.Name = "xtraTabPage2";
            xtraTabPage2.Size = new System.Drawing.Size(435, 563);
            xtraTabPage2.Text = "Procesos";
            // 
            // WoNewMenu
            // 
            ClientSize = new System.Drawing.Size(437, 588);
            Controls.Add(panelControl5);
            Controls.Add(xtraTabControl1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "WoNewMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Menús";
            FormClosing += OnFormClosing;
            ((System.ComponentModel.ISupportInitialize)panelControl3).EndInit();
            panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl4).EndInit();
            panelControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl6).EndInit();
            panelControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdRoles).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdRolesView).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtMenuName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl5).EndInit();
            panelControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdProcess).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdProcessView).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl2).EndInit();
            panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).EndInit();
            xtraTabControl1.ResumeLayout(false);
            xtraTabPage1.ResumeLayout(false);
            xtraTabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }


        #endregion
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnAcept;
        private DevExpress.XtraEditors.TextEdit txtMenuName;
        private DevExpress.XtraGrid.GridControl grdProcess;
        private DevExpress.XtraGrid.Views.Grid.GridView grdProcessView;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl5;
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private DevExpress.XtraEditors.PanelControl panelControl6;
        private DevExpress.XtraGrid.GridControl grdRoles;
        private DevExpress.XtraGrid.Views.Grid.GridView grdRolesView;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private Label label1;
    }
}