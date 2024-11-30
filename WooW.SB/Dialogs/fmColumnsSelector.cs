using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs
{
    public partial class fmColumnsSelector : DevExpress.XtraEditors.XtraForm
    {
        private Proyecto proyecto;

        public string Modelo { get; set; }

        public bool CombinarModeloColumna { get; set; }
        public List<string> ColSelected { get; set; }

        public fmColumnsSelector()
        {
            InitializeComponent();
            ColSelected = new List<string>();

            proyecto = Proyecto.getInstance();
        }

        public void HabilitaCombinar()
        {
            optCombinar.Checked = true;
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buCopiar_Click(object sender, EventArgs e)
        {
            if (txtModelo.EditValue == null)
                return;

            Modelo = txtModelo.EditValue.ToString();
            CombinarModeloColumna = optCombinar.Checked;

            ColSelected.Clear();
            foreach (CheckedListBoxItem i in txtColumnas.Items)
                if (i.CheckState == CheckState.Checked)
                    ColSelected.Add(i.Value.ToString());

            if (ColSelected.Count == 0)
            {
                XtraMessageBox.Show(
                    "Seleccione una o mas columnas",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            this.DialogResult = DialogResult.OK;
        }

        private void fmColumnsSelector_Load(object sender, EventArgs e)
        {
            txtModelo.Properties.Items.Clear();

            foreach (var model in proyecto.ModeloCol.Modelos)
                txtModelo.Properties.Items.Add(model.Id);
        }

        private void txtModelo_EditValueChanged(object sender, EventArgs e)
        {
            var model = this
                .proyecto.ModeloCol.Modelos.Where(f => f.Id == txtModelo.EditValue.ToString())
                .FirstOrDefault();

            if (model == null)
                return;

            txtColumnas.Items.Clear();
            foreach (var col in model.Columnas.OrderBy(j => j.Orden))
                txtColumnas.Items.Add(col.Id);
        }

        private void txtFiltro_EditValueChanged(object sender, EventArgs e)
        {
            txtColumnas.Items.Clear();

            txtModelo.Properties.Items.Clear();

            if (txtFiltro.EditValue.IsNullOrStringEmpty())
            {
                foreach (var model in proyecto.ModeloCol.Modelos)
                    txtModelo.Properties.Items.Add(model.Id);
            }
            else
            {
                foreach (
                    var model in proyecto.ModeloCol.Modelos.Where(x =>
                        x.Id.ToUpper().StartsWith(txtFiltro.EditValue.ToString().ToUpper())
                    )
                )
                    txtModelo.Properties.Items.Add(model.Id);
            }
        }
    }
}
