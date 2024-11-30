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
    public partial class fmLabels : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        public Proyecto proyecto { get; set; }

        public fmLabels()
        {
            InitializeComponent();

            repElementos.Items.AddRange(TranslateHelper.TraduccirAccion);
            txtElementos.EditValue = repElementos.Items[0];
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
            if (buRefrescar.Enabled)
                buRefrescar.PerformClick();
        }

        public void Cargar()
        {
            repIdioma.Items.Clear();

            foreach (var Idioma in proyecto.Idiomas)
                repIdioma.Items.Add(Idioma.Id);

            txtIdioma.EditValue = proyecto.esMX;

            ReCargar();

            proyecto.EtiquetasEditandose = false;
            proyecto.EtiquetasCambiadas = false;
            grdEtiquetaView.OptionsBehavior.Editable = false;
            grpTranslate.Enabled = false;
        }

        public void ReCargar()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Nuevo", typeof(bool));
            dt.Columns.Add(@"Id", typeof(string));
            dt.Columns.Add($"Texto_{proyecto.esMX}", typeof(string));
            dt.Columns.Add(@"Texto", typeof(string));

            foreach (var Etiqueta in proyecto.EtiquetaCol.Etiquetas)
            {
                DataRow drRow = dt.NewRow();
                drRow["Nuevo"] = false;
                drRow["Id"] = Etiqueta.Id;

                if (txtIdioma.EditValue.ToString() != proyecto.esMX)
                {
                    var EtiquetaIdiomaSPMX = Etiqueta
                        .Idiomas.Where(e => e.IdiomaId == proyecto.esMX)
                        .FirstOrDefault();
                    if (EtiquetaIdiomaSPMX != null)
                        drRow[$"Texto_{proyecto.esMX}"] = EtiquetaIdiomaSPMX.Texto;
                }

                var EtiquetaIdioma = Etiqueta
                    .Idiomas.Where(e => e.IdiomaId == txtIdioma.EditValue.ToString())
                    .FirstOrDefault();

                if (EtiquetaIdioma != null)
                {
                    drRow["Texto"] = EtiquetaIdioma.Texto;
                    if (txtIdioma.EditValue.ToString() != proyecto.esMX)
                        if (optSinTraducir.EditValue.ToBoolean())
                            continue;
                }
                dt.Rows.Add(drRow);
            }

            grdEtiqueta.DataSource = dt;

            GridColumn col = grdEtiquetaView.Columns["Nuevo"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 50;
            col.OptionsColumn.AllowFocus = false;

            col = grdEtiquetaView.Columns["Id"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;

            col = grdEtiquetaView.Columns[$"Texto_{proyecto.esMX}"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;
            col.Visible = (txtIdioma.EditValue.ToString() != proyecto.esMX);

            col = grdEtiquetaView.Columns["Texto"];
            col.Width = 300;
            col.Caption = txtIdioma.EditValue.ToString();

            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            buEditar.Enabled = true;
            buRefrescar.Enabled = true;
            txtIdioma.Enabled = true;
            optSinTraducir.Enabled = true;
        }

        private void grdEtiquetaView_InitNewRow(
            object sender,
            DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e
        )
        {
            DataRow dr = (DataRow)grdEtiquetaView.GetDataRow(e.RowHandle);
            dr["Nuevo"] = true;
            dr["Texto"] = string.Empty;
        }

        private void grdEtiquetaView_ValidateRow(
            object sender,
            DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e
        )
        {
            DataRow dr = (DataRow)grdEtiquetaView.GetDataRow(e.RowHandle);

            if (dr["Texto"].ToSafeString().Trim().IsNullOrStringEmpty())
            {
                e.ErrorText = "Etiqueta no puede ser nula";
                e.Valid = false;
                return;
            }

            if (dr["Nuevo"].ToBoolean())
            {
                dr["Id"] = Etiqueta.ToId(dr["Texto"].ToString());
            }

            if (e.RowHandle < 0)
            {
                DataTable dt = grdEtiqueta.DataSource as DataTable;
                foreach (DataRow drLoc in dt.Rows)
                {
                    if (drLoc["Id"].ToString() == dr["Id"].ToString())
                    {
                        e.ErrorText = "Identificador se repite";
                        e.Valid = false;
                        return;
                    }
                }
            }

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

            var dicc = new Dictionary<string, Etiqueta>();

            DataTable dt = grdEtiqueta.DataSource as DataTable;

            foreach (var Etiqueta in proyecto.EtiquetaCol.Etiquetas)
            {
                if (dicc.ContainsKey((string)Etiqueta.Id))
                {
                    XtraMessageBox.Show(
                        $"La clave {Etiqueta.Id} se repite, va a eliminar",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    DataRow[] drLocRows = dt.Select($"Id = '{Etiqueta.Id}'");
                    for (int i = 1; i < drLocRows.Length; i++)
                        drLocRows[i].Delete();
                    continue;
                }

                //DataRow[] drRows = dt.Select($"Id = '{Etiqueta.Id}'");
                //if (drRows.Length > 0)
                dicc.Add((string)Etiqueta.Id, Etiqueta);
            }

            var Etiquetas = new EtiquetaCol();

            foreach (DataRow drRow in dt.Rows)
            {
                var Id = drRow["Id"].ToString();

                if (dicc.ContainsKey(Id))
                {
                    var Etiqueta = new Etiqueta();
                    dicc.TryGetValue(Id, out Etiqueta);
                    var EtiquetaIdioma = Etiqueta
                        .Idiomas.Where(f => f.IdiomaId == txtIdioma.EditValue.ToString())
                        .FirstOrDefault();
                    if (EtiquetaIdioma.IsNull())
                    {
                        EtiquetaIdioma = new EtiquetaIdioma(
                            txtIdioma.EditValue.ToString(),
                            drRow["Texto"].ToString()
                        );
                        Etiqueta.Idiomas.Add(EtiquetaIdioma);
                    }
                    else
                    {
                        EtiquetaIdioma.Texto = drRow["Texto"].ToString();
                    }
                    Etiquetas.Etiquetas.Add(Etiqueta);
                }
                else
                {
                    var Etiqueta = new Etiqueta();
                    Etiqueta.Id = drRow["Id"].ToString();
                    var EtiquetaIdioma = new EtiquetaIdioma(
                        txtIdioma.EditValue.ToString(),
                        drRow["Texto"].ToString()
                    );
                    Etiqueta.Idiomas.Add(EtiquetaIdioma);
                    Etiquetas.Etiquetas.Add(Etiqueta);
                }
            }

            foreach (var item in dicc)
            {
                if (Etiquetas.Etiquetas.Where(f => f.Id == item.Key).FirstOrDefault() == null)
                    Etiquetas.Etiquetas.Add(item.Value);
            }

            proyecto.EtiquetaCol = Etiquetas;

            proyecto.SaveLabels();

            grdEtiquetaView.OptionsBehavior.Editable = false;

            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            txtIdioma.Enabled = true;
            optSinTraducir.Enabled = true;
            buRefrescar.Enabled = true;
            buEditar.Enabled = true;
            grpTranslate.Enabled = false;
            proyecto.EtiquetasEditandose = false;
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
                Cargar();
            }
        }

        private void txtIdioma_EditValueChanged(object sender, EventArgs e)
        {
            ReCargar();
        }

        private void buRefrescar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Cargar();
        }

        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
            buRefrescar.Enabled = false;
            buEditar.Enabled = false;

            proyecto.EtiquetasEditandose = true;
        }

        private void buTraducir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<string> sl = new List<string> { @"Texto" };

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

        private void optSinTraducir_EditValueChanged(object sender, EventArgs e)
        {
            ReCargar();
        }
    }
}
