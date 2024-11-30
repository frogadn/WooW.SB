using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Newtonsoft.Json;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.MenuDesigner.MenuDesignerModels;

namespace WooW.SB.Dialogs
{
    public partial class fmNewMenu : DevExpress.XtraEditors.XtraForm
    {
        private Proyecto proyecto;

        public fmNewMenu()
        {
            InitializeComponent();
            txtMenuName.Focus();
            proyecto = Proyecto.getInstance();
            foreach (var rol in proyecto.Roles)
            {
                cmbRoles.Properties.Items.Add(rol.EtiquetaId);
            }

            cmbRoles.SelectedIndex = 0;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (txtMenuName.EditValue.ToSafeString().IsNullOrStringEmpty())
                return;

            string Archivo = $"{proyecto.DirMenus}\\{txtMenuName.EditValue.ToString()}.json";

            if (File.Exists(Archivo))
            {
                XtraMessageBox.Show(
                    "El nombre del menú ya existe",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }
            var wo = GenerateMenu();
            //string json = JsonConvert.SerializeObject(wo, Formatting.Indented);
            string json = JsonConvert.SerializeObject(wo, Formatting.Indented);
            this.DialogResult = DialogResult.OK;
            try
            {
                File.WriteAllText(
                    proyecto.DirMenus + "\\" + txtMenuName.EditValue.ToString() + ".json",
                    json
                );
                XtraMessageBox.Show(
                    "El menú se creo correctamente",
                    "Menús",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    "Error:" + ex,
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        public WoMenuContainer GenerateMenu()
        {
            List<Modelo> modelos = new List<Modelo>();
            List<string> listProcess = new List<string>();
            WoMenuContainer MenuBase = new WoMenuContainer();

            MenuBase.Id = txtMenuName.EditValue.ToString();
            MenuBase.Label = txtMenuName.EditValue.ToString();
            MenuBase.ContainersCol = null;
            MenuBase.TypeContainer = eTypeContainer.Menu;
            MenuBase.Rol = cmbRoles.EditValue.ToString();

            //Si esta marcada la casilla de todos los procesos carga todo sin filtrar
            List<WoMenuContainer> woList = new List<WoMenuContainer>();
            foreach (dynamic process in checkListProc.CheckedItems)
            {
                if (process.Value != "Todos")
                {
                    listProcess.Add(process.Value);
                }
            }
            foreach (dynamic item in checkListProc.CheckedItems)
            {
                if (item.Value != "Todos")
                {
                    WoMenuContainer Submenu = new WoMenuContainer()
                    {
                        Enable = false,
                        Id = item.Value,
                        TypeContainer = eTypeContainer.SubMenu,
                        ProcessList = listProcess,
                        Order = 1
                    };
                    var xx = proyecto.ModeloCol.Modelos.Where(x =>
                        (x.ProcesoId == item.Value)
                        && (
                            x.TipoModelo == WoTypeModel.Request
                        //|| x.TipoModelo == WoTypeModel.Configuration
                        )
                        && (
                            x.SubTipoModelo == WoSubTypeModel.Report
                            || x.SubTipoModelo == WoSubTypeModel.DataService
                        )
                    );

                    List<WoMenuItem> woListItem = new List<WoMenuItem>();

                    foreach (var z in xx)
                    {
                        woListItem.Add(
                            new WoMenuItem()
                            {
                                Id = z.Id,
                                TypeItem = eTypeItem.MenuItem,
                                Referencia = z.ProcesoId + "/" + z.TipoModelo + "/" + z.Id
                            }
                        );
                    }
                    MenuBase.ProcessList = listProcess;
                    Submenu.ItemsCol = woListItem;
                    woList.Add(Submenu);
                }
            }
            MenuBase.ContainersCol = woList;
            return MenuBase;
        }

        private void fmNewMenu_Activated(object sender, EventArgs e)
        {
            txtMenuName.Focus();
        }

        private void fmNewMenu_Load(object sender, EventArgs e)
        {
            checkListProc.Items.Add(@"Todos");

            foreach (var modelo in proyecto.ModeloCol.Modelos)
            {
                if (
                    modelo.TipoModelo == WoTypeModel.Request
                    || modelo.SubTipoModelo == WoSubTypeModel.Report
                    || modelo.TipoModelo == WoTypeModel.Configuration
                )
                {
                    if (!checkListProc.Items.Contains(modelo.ProcesoId))
                        checkListProc.Items.Add(modelo.ProcesoId);
                }
            }
        }

        private void checkListProc_ItemCheck_1(
            object sender,
            DevExpress.XtraEditors.Controls.ItemCheckEventArgs e
        )
        {
            CheckedListBoxControl checkedListBox = (CheckedListBoxControl)sender;
            CheckedListBoxItem item = (CheckedListBoxItem)checkedListBox.GetItem(e.Index);
            if (item != null && item.CheckState == CheckState.Checked)
            {
                if (item.Value == "Todos")
                {
                    checkListProc.CheckAll();
                }
            }
            else if (item.CheckState == CheckState.Unchecked)
            {
                if (item.Value == "Todos")
                {
                    checkListProc.UnCheckAll();
                }
            }
        }
    }
}
