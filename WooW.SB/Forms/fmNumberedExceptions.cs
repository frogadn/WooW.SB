using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Dialogs;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmNumberedExceptions : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        public Proyecto proyecto { get; set; }
        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public fmNumberedExceptions()
        {
            InitializeComponent();

            repElementos.Items.AddRange(TranslateHelper.TraduccirAccion);
            txtElementos.EditValue = repElementos.Items[0];
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
                ReCargar();
        }

        public void Cargar()
        {
            repIdioma.Items.Clear();

            foreach (var Idioma in proyecto.Idiomas)
                repIdioma.Items.Add(Idioma.Id);

            txtIdioma.EditValue = proyecto.esMX;

            ReCargar();

            proyecto.MensajesCambiados = false;
            grdMensajeView.OptionsBehavior.Editable = false;
            grpTranslate.Enabled = false;
        }

        public void ReCargar()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Nuevo", typeof(bool));
            dt.Columns.Add(@"Proceso", typeof(string));
            dt.Columns.Add(@"Numero", typeof(int));
            dt.Columns.Add($"Texto_{proyecto.esMX}", typeof(string));
            dt.Columns.Add($"Solucion_{proyecto.esMX}", typeof(string));
            dt.Columns.Add(@"Texto", typeof(string));
            dt.Columns.Add(@"Solucion", typeof(string));

            foreach (var Etiqueta in proyecto.MensajeCol.Mensajes)
            {
                DataRow drRow = dt.NewRow();
                drRow["Nuevo"] = false;

                string Proceso;
                int Numero;
                string Id = Etiqueta.Id;

                if (!WoNumberedExceptionsHelper.SeparaId(Id, out Proceso, out Numero))
                    continue;

                drRow["Proceso"] = Proceso;
                drRow["Numero"] = Numero;

                if (txtIdioma.EditValue.ToString() != proyecto.esMX)
                {
                    var EtiquetaIdiomaSPMX = Etiqueta
                        .Idiomas.Where(e => e.IdiomaId == proyecto.esMX)
                        .FirstOrDefault();
                    if (EtiquetaIdiomaSPMX != null)
                    {
                        drRow[$"Texto_{proyecto.esMX}"] = EtiquetaIdiomaSPMX.Texto;
                        drRow[$"Solucion_{proyecto.esMX}"] = EtiquetaIdiomaSPMX.Solucion;
                    }
                }

                var EtiquetaIdioma = Etiqueta
                    .Idiomas.Where(e => e.IdiomaId == txtIdioma.EditValue.ToString())
                    .FirstOrDefault();

                if (EtiquetaIdioma != null)
                {
                    drRow["Texto"] = EtiquetaIdioma.Texto;
                    drRow[@"Solucion"] = EtiquetaIdioma.Solucion;

                    if (txtIdioma.EditValue.ToString() != proyecto.esMX)
                        if (optSinTraducir.EditValue.ToBoolean())
                            continue;
                }
                dt.Rows.Add(drRow);
            }

            grdMensaje.DataSource = dt;

            int index = 0;

            GridColumn col = grdMensajeView.Columns["Nuevo"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 50;
            col.OptionsColumn.AllowFocus = false;
            col.VisibleIndex = index++;

            col = grdMensajeView.Columns["Proceso"];
            col.Width = 100;
            col.VisibleIndex = index++;

            RepositoryItemComboBox txtProceso = new RepositoryItemComboBox();
            txtProceso.TextEditStyle = DevExpress
                .XtraEditors
                .Controls
                .TextEditStyles
                .DisableTextEditor;

            foreach (var Proceso in proyecto.Procesos)
                txtProceso.Items.Add(Proceso.Id);

            col.ColumnEdit = txtProceso;

            col = grdMensajeView.Columns["Numero"];
            col.Width = 100;
            col.VisibleIndex = index++;

            col = grdMensajeView.Columns[$"Texto_{proyecto.esMX}"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;
            if (txtIdioma.EditValue.ToString() != proyecto.esMX)
            {
                col.VisibleIndex = index++;
            }
            else
                col.Visible = false;

            col = grdMensajeView.Columns[$"Solucion_{proyecto.esMX}"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;
            if (txtIdioma.EditValue.ToString() != proyecto.esMX)
            {
                col.VisibleIndex = index++;
            }
            else
                col.Visible = false;

            col = grdMensajeView.Columns["Texto"];
            col.Width = 300;
            col.VisibleIndex = index++;

            col = grdMensajeView.Columns[@"Solucion"];
            col.Width = 300;
            col.VisibleIndex = index++;

            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            if (txtIdioma.EditValue.ToString() == proyecto.esMX)
                buCopiarCodigoMuestra.Enabled = true;
            else
                buCopiarCodigoMuestra.Enabled = false;
            buEditar.Enabled = true;
            txtIdioma.Enabled = true;
            optSinTraducir.Enabled = true;
            buRefrescar.Enabled = true;
        }

        private void grdMensajeView_InitNewRow(
            object sender,
            DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e
        )
        {
            DataRow dr = (DataRow)grdMensajeView.GetDataRow(e.RowHandle);
            dr["Nuevo"] = true;
            dr["Proceso"] = string.Empty;
            dr["Numero"] = 0;
            dr["Texto"] = string.Empty;
            dr[@"Solucion"] = string.Empty;
        }

        private void grdMensajeView_ValidateRow(
            object sender,
            DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e
        )
        {
            DataRow dr = (DataRow)grdMensajeView.GetDataRow(e.RowHandle);

            if (dr["Proceso"].IsNullOrStringTrimEmpty())
            {
                e.ErrorText = "Columna Proceso no puede ser nula";
                e.Valid = false;
                return;
            }

            if (dr["Numero"].ToInt32() == 0)
            {
                e.ErrorText = "Columna Número debe tener un valor";
                e.Valid = false;
                return;
            }

            if (dr["Texto"].IsNullOrStringTrimEmpty())
            {
                e.ErrorText = "Columna texto no puede ser nula";
                e.Valid = false;
                return;
            }

            if (e.RowHandle < 0)
            {
                DataTable dt = grdMensaje.DataSource as DataTable;
                foreach (DataRow drLoc in dt.Rows)
                {
                    if (
                        (drLoc["Proceso"].ToString() == dr["Proceso"].ToString())
                        && (drLoc["Numero"].ToString() == dr["Numero"].ToString())
                    )
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
            grdMensajeView.PostEditor();
            if (!grdMensajeView.UpdateCurrentRow())
                return;

            var dicc = new Dictionary<string, Mensaje>();

            DataTable dt = grdMensaje.DataSource as DataTable;

            foreach (var Mensaje in proyecto.MensajeCol.Mensajes)
            {
                string Proceso;
                int Numero;

                if (!WoNumberedExceptionsHelper.SeparaId(Mensaje.Id, out Proceso, out Numero))
                    continue;

                if (dicc.ContainsKey(Mensaje.Id))
                {
                    XtraMessageBox.Show(
                        $"La clave {Mensaje.Id} se repite, va a eliminar",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    DataRow[] drLocRows = dt.Select($"Proceso = '{Proceso}' and Numero = {Numero}");
                    for (int i = 1; i < drLocRows.Length; i++)
                        drLocRows[i].Delete();
                    continue;
                }

                //DataRow[] drRows = dt.Select($"Proceso = '{Proceso}' and Numero = {Numero}");
                //if (drRows.Length > 0)
                dicc.Add(Mensaje.Id, Mensaje);
            }

            var Mensajes = new MensajeCol();

            foreach (DataRow drRow in dt.Rows)
            {
                var Id = WoNumberedExceptionsHelper.RegresaId(
                    drRow["Proceso"].ToString(),
                    drRow["Numero"].ToInt32()
                );

                if (dicc.ContainsKey(Id))
                {
                    var Mensaje = new Mensaje();
                    dicc.TryGetValue(Id, out Mensaje);
                    var MensajeIdioma = Mensaje
                        .Idiomas.Where(f => f.IdiomaId == txtIdioma.EditValue.ToString())
                        .FirstOrDefault();
                    if (MensajeIdioma.IsNull())
                    {
                        MensajeIdioma = new MensajeIdioma();
                        MensajeIdioma.IdiomaId = txtIdioma.EditValue.ToString();
                        MensajeIdioma.Texto = drRow["Texto"].ToString();
                        MensajeIdioma.Solucion = drRow[@"Solucion"].ToString();
                        Mensaje.Idiomas.Add(MensajeIdioma);
                    }
                    else
                    {
                        MensajeIdioma.Texto = drRow["Texto"].ToString();
                        MensajeIdioma.Solucion = drRow[@"Solucion"].ToString();
                    }
                    Mensajes.Mensajes.Add(Mensaje);
                }
                else
                {
                    var Mensaje = new Mensaje();
                    Mensaje.Id = Id;
                    var MensajeIdioma = new MensajeIdioma();
                    MensajeIdioma.IdiomaId = txtIdioma.EditValue.ToString();
                    MensajeIdioma.Texto = drRow["Texto"].ToString();
                    MensajeIdioma.Solucion = drRow[@"Solucion"].ToString();
                    Mensaje.Idiomas.Add(MensajeIdioma);
                    Mensajes.Mensajes.Add(Mensaje);
                }
            }

            foreach (var item in dicc)
            {
                if (Mensajes.Mensajes.Where(f => f.Id == item.Key).FirstOrDefault() == null)
                    Mensajes.Mensajes.Add(item.Value);
            }

            proyecto.MensajeCol = Mensajes;

            proyecto.SaveMessages();

            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            buEditar.Enabled = true;
            txtIdioma.Enabled = true;
            optSinTraducir.Enabled = true;
            buRefrescar.Enabled = true;
            grpTranslate.Enabled = false;

            if (txtIdioma.EditValue.ToString() == proyecto.esMX)
                buCopiarCodigoMuestra.Enabled = true;
            else
                buCopiarCodigoMuestra.Enabled = false;
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
            if (proyecto.MensajesCambiados)
                Cargar();

            grpTranslate.Enabled = (txtIdioma.EditValue.ToString() != proyecto.esMX);

            grdMensajeView.OptionsBehavior.Editable = true;

            buAceptarCambios.Enabled = true;
            buDescartarCambios.Enabled = true;
            txtIdioma.Enabled = false;
            optSinTraducir.Enabled = false;
            buRefrescar.Enabled = false;
            buEditar.Enabled = false;
            buCopiarCodigoMuestra.Enabled = false;

            proyecto.MensajesEditandose = true;
        }

        private void buTraducir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<string> sl = new List<string> { @"Texto", @"Solucion" };

            if (txtIdioma.EditValue.ToString() == proyecto.esMX)
                return;

            int Renglones = TranslateHelper.Renglones(txtElementos.EditValue.ToString());

            try
            {
                DataRow drBak = null;
                for (int i = 0; i < Renglones; i++)
                {
                    DataRow dr = grdMensajeView.GetFocusedDataRow();
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
                    grdMensajeView.MoveBy(1);
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

        private void buCopiarCodigoMuestra_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            DataRow dr = grdMensajeView.GetFocusedDataRow();
            if (dr == null)
                return;

            string NumeredExcepcion = fmNumberedExceptionsSelector.GetSampleCode(
                "throw new WoScriptingException",
                dr["Proceso"].ToString(),
                dr["Numero"].ToInt16(),
                dr["Texto"].ToString(),
                dr["Solucion"].ToString()
            );

            try
            {
                Clipboard.SetDataObject(NumeredExcepcion, true);
            }
            catch { }
        }
    }
}
