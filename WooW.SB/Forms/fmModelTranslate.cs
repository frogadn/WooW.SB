using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmModelTranslate : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }
        public Proyecto proyecto { get; set; }

        public fmModelTranslate()
        {
            InitializeComponent();

            repElementos.Items.AddRange(TranslateHelper.TraduccirAccion);
            txtElementos.EditValue = TranslateHelper.TraduccirAccion[0];

            txtSeleccion.EditValue = repSeleccion.Items[0];
        }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return RibbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return buAceptarCambios.Enabled; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Refrescar()
        {
            if (buEditar.Enabled)
                ReCargar();
        }

        public void Cargar()
        {
            repIdioma.Items.Clear();

            foreach (var Idioma in proyecto.Idiomas)
                if (Idioma.Id != proyecto.esMX)
                    repIdioma.Items.Add(Idioma.Id);

            grdEtiquetaView.OptionsBehavior.Editable = false;
            grpTranslate.Enabled = false;
        }

        public void ReCargar()
        {
            if (txtIdioma.EditValue == null)
                return;

            Traduccion trad = null;
            Dictionary<string, Tuple<string, string>> TradActual = null;

            if (txtSeleccion.EditValue != repSeleccion.Items[0])
            {
                trad = ResourcesMaker.GetModelAyudaTraduccion(
                    proyecto,
                    txtIdioma.EditValue.ToString()
                );
                TradActual = proyecto.GetDiccAyudaTraduccion(txtIdioma.EditValue.ToString());
            }
            else
            {
                trad = ResourcesMaker.GetModelTraduccion(proyecto, txtIdioma.EditValue.ToString());
                TradActual = proyecto.GetDiccTraduccion(txtIdioma.EditValue.ToString());
            }

            DataTable dt = new DataTable();

            dt.Columns.Add(@"Id", typeof(string));
            dt.Columns.Add($"Texto_{proyecto.esMX}", typeof(string));

            if (txtSeleccion.EditValue != repSeleccion.Items[0])
                dt.Columns.Add($"Ayuda_{proyecto.esMX}", typeof(string));

            dt.Columns.Add(@"Texto", typeof(string));

            if (txtSeleccion.EditValue != repSeleccion.Items[0])
                dt.Columns.Add($"Ayuda", typeof(string));

            foreach (var item in trad.Items)
            {
                DataRow drRow = dt.NewRow();
                drRow["Id"] = item.Etiqueta;
                drRow[$"Texto_{proyecto.esMX}"] = item.Valor;

                if (txtSeleccion.EditValue != repSeleccion.Items[0])
                    drRow[$"Ayuda_{proyecto.esMX}"] = item.ValorAyuda;

                Tuple<string, string> traditem = Tuple.Create(string.Empty, string.Empty);
                if (TradActual.ContainsKey(item.Etiqueta))
                {
                    TradActual.TryGetValue(item.Etiqueta, out traditem);

                    bool bFalta = false;

                    drRow["Texto"] = traditem.Item1;
                    if (traditem.Item1.IsNullOrStringEmpty())
                        bFalta = true;

                    if (txtSeleccion.EditValue != repSeleccion.Items[0])
                    {
                        drRow[$"Ayuda"] = traditem.Item2;
                        if (traditem.Item2.IsNullOrStringEmpty())
                            bFalta = true;
                    }

                    if (txtIdioma.EditValue.ToString() != proyecto.esMX)
                        if ((optSinTraducir.EditValue.ToBoolean()) && (!bFalta))
                            continue;
                }
                dt.Rows.Add(drRow);
            }

            grdEtiqueta.DataSource = dt;

            GridColumn col = grdEtiquetaView.Columns["Id"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;

            col = grdEtiquetaView.Columns[$"Texto_{proyecto.esMX}"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;

            if (txtSeleccion.EditValue != repSeleccion.Items[0])
            {
                col = grdEtiquetaView.Columns[$"Ayuda_{proyecto.esMX}"];
                col.OptionsColumn.ReadOnly = true;
                col.Width = 300;
                col.OptionsColumn.AllowFocus = false;
            }

            col = grdEtiquetaView.Columns["Texto"];
            col.Width = 300;
            col.Caption = txtIdioma.EditValue.ToString();

            if (txtSeleccion.EditValue != repSeleccion.Items[0])
            {
                col = grdEtiquetaView.Columns["Ayuda"];
                col.Width = 300;
                col.Caption = txtIdioma.EditValue.ToString();
            }

            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            buEditar.Enabled = true;
            txtIdioma.Enabled = true;
            optSinTraducir.Enabled = true;
            txtSeleccion.Enabled = true;
            grpTranslate.Enabled = false;
        }

        private void grdEtiquetaView_InitNewRow(
            object sender,
            DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e
        )
        {
            DataRow dr = (DataRow)grdEtiquetaView.GetDataRow(e.RowHandle);
            dr["Texto"] = string.Empty;
        }

        private void grdEtiquetaView_ValidateRow(
            object sender,
            DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e
        )
        {
            DataRow dr = (DataRow)grdEtiquetaView.GetDataRow(e.RowHandle);

            e.Valid = true;
        }

        private void buAceptarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            grdEtiquetaView.PostEditor();
            if (!grdEtiquetaView.UpdateCurrentRow())
                return;

            DataTable dt = grdEtiqueta.DataSource as DataTable;

            var t = new Traduccion();
            t.IdiomaId = txtIdioma.EditValue.ToString();
            t.Items = new List<TraduccionItem>();

            foreach (DataRow drRow in dt.Rows)
            {
                if (!drRow["Texto"].ToSafeString().IsNullOrStringEmpty())
                {
                    var item = new TraduccionItem()
                    {
                        Etiqueta = drRow["Id"].ToString(),
                        Traduccion = drRow["Texto"].ToString()
                    };

                    if (txtSeleccion.EditValue != repSeleccion.Items[0])
                        item.Ayuda = drRow["Ayuda"].ToString();

                    t.Items.Add(item);
                }
            }
            if (txtSeleccion.EditValue != repSeleccion.Items[0])
            {
                var locTrad = proyecto
                    .ModeloAyudaTraduccion.Where(f => f.IdiomaId == txtIdioma.EditValue.ToString())
                    .FirstOrDefault();

                if (locTrad != null)
                {
                    foreach (var item in locTrad.Items)
                    {
                        if (
                            t.Items.Where(f => f.Etiqueta == item.Etiqueta).FirstOrDefault() == null
                        )
                            t.Items.Add(item.Clone());
                    }
                }
            }
            else
            {
                var locTrad = proyecto
                    .ModeloTraduccion.Where(f => f.IdiomaId == txtIdioma.EditValue.ToString())
                    .FirstOrDefault();

                if (locTrad != null)
                {
                    foreach (var item in locTrad.Items)
                    {
                        if (
                            t.Items.Where(f => f.Etiqueta == item.Etiqueta).FirstOrDefault() == null
                        )
                            t.Items.Add(item.Clone());
                    }
                }
            }

            if (txtSeleccion.EditValue != repSeleccion.Items[0])
            {
                var old = proyecto
                    .ModeloAyudaTraduccion.Where(f => f.IdiomaId == txtIdioma.EditValue.ToString())
                    .FirstOrDefault();
                if (old != null)
                    proyecto.ModeloAyudaTraduccion.Remove(old);

                proyecto.ModeloAyudaTraduccion.Add(t);
            }
            else
            {
                var old = proyecto
                    .ModeloTraduccion.Where(f => f.IdiomaId == txtIdioma.EditValue.ToString())
                    .FirstOrDefault();
                if (old != null)
                    proyecto.ModeloTraduccion.Remove(old);

                proyecto.ModeloTraduccion.Add(t);
            }

            proyecto.SaveModeloTraduccion();

            grdEtiquetaView.OptionsBehavior.Editable = false;

            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            txtIdioma.Enabled = true;
            optSinTraducir.Enabled = true;
            txtSeleccion.Enabled = true;
            buEditar.Enabled = false;
            grpTranslate.Enabled = false;

            txtIdioma.EditValue = null;
            grdEtiqueta.DataSource = null;
            grdEtiquetaView.Columns.Clear();
        }

        private void buDescartarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if (
                XtraMessageBox.Show(
                    "Descartar los cambios?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) == DialogResult.Yes
            )
            {
                ReCargar();
            }
        }

        private void txtIdioma_EditValueChanged(object sender, EventArgs e)
        {
            if (txtIdioma.EditValue != null)
                ReCargar();
        }

        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtIdioma.EditValue == null)
                return;

            if (proyecto.EtiquetasCambiadas)
                Cargar();

            if ((grdEtiqueta.DataSource as DataTable).Rows.Count == 0)
            {
                XtraMessageBox.Show(
                    $"No hay nada que editar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            grpTranslate.Enabled = (txtIdioma.EditValue.ToString() != proyecto.esMX);

            grdEtiquetaView.OptionsBehavior.Editable = true;

            buAceptarCambios.Enabled = true;
            buDescartarCambios.Enabled = true;
            txtIdioma.Enabled = false;
            optSinTraducir.Enabled = false;
            txtSeleccion.Enabled = false;
            buEditar.Enabled = false;
        }

        private void buTraducir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<string> sl = new List<string> { @"Texto" };

            if (txtSeleccion.EditValue != repSeleccion.Items[0])
                sl.Add(@"Ayuda");

            if (txtIdioma.EditValue.ToString() == proyecto.esMX)
                return;

            int Renglones = TranslateHelper.Renglones(txtElementos.EditValue.ToString());

            try
            {
                DataRow drBak = null;
                for (int i = 0; i < Renglones; i++)
                {
                    DataRow dr = grdEtiquetaView.GetFocusedDataRow();
                    if (dr == null)
                    {
                        XtraMessageBox.Show(
                            "Seleccione un renglón",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }

                    if ((drBak != null) && (dr == drBak))
                        break;

                    foreach (string s in sl)
                    {
                        if (
                            (!optRenglonesBlanco.EditValue.ToBoolean())
                            || (dr[s].IsDBNullOrStringEmpty())
                        )
                            dr[s] = TranslateHelper.Do(
                                dr[$"{s}_{proyecto.esMX}"].ToString(),
                                proyecto.esMX.Substring(0, 2),
                                txtIdioma.EditValue.ToString().Substring(0, 2)
                            );
                    }

                    drBak = dr;
                    grdEtiquetaView.MoveBy(1);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
        }

        private void txtSeleccion_EditValueChanged(object sender, EventArgs e)
        {
            txtIdioma.EditValue = null;
            grdEtiqueta.DataSource = null;
            grdEtiquetaView.Columns.Clear();
        }

        private void optSinTraducir_EditValueChanged(object sender, EventArgs e)
        {
            if ((txtSeleccion.EditValue != null) && (txtIdioma.EditValue != null))
                ReCargar();
        }
    }
}
