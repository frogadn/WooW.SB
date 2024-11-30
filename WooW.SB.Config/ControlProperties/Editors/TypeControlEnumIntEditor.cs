using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WooW.Core;
using WooW.SB.Config.ControlProperties.Class;

namespace WooW.SB.Config.ControlProperties.Editors
{
    public partial class TypeControlEnumIntEditor<T> : DevExpress.XtraEditors.XtraForm, ITypeControlEditor where T : TypeControlEnumInt
    {

        private Proyecto proyecto;
        private T model;

        public string Properties
        {
            get
            {
                return JsonConvert.SerializeObject(model);
            }
            set
            {
                if (value.IsNullOrStringEmpty())
                    model = (T)Activator.CreateInstance(typeof(T));
                else
                    model = JsonConvert.DeserializeObject<T>(value);
            }
        }

        public DialogResult ShowEditor()
        {
            return this.ShowDialog();
        }

        public TypeControlEnumIntEditor()
        {
            InitializeComponent();
            proyecto = Proyecto.getInstance();
        }

        private void buCancelar_Click(object sender, EventArgs e)
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
                this.DialogResult = DialogResult.Cancel;

            return;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            DataTable dtEnum = grdEnum.DataSource as DataTable;

            model.Items.Clear();

            List<int> DuplicadosInt = new List<int>();
            List<string> DuplicadosString = new List<string>();

            foreach (DataRow drRow in dtEnum.Select(string.Empty, @"Numero asc"))
            {
                if (drRow["Nombre"].IsNullOrStringTrimEmpty())
                {
                    XtraMessageBox.Show(
                        "Seleccione un nombre para la enumeración",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (drRow["EtiquetaId"].IsNullOrStringTrimEmpty())
                {
                    XtraMessageBox.Show(
                        "Seleccione un etiqueta para la enumeración",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (DuplicadosInt.IndexOf(drRow["Numero"].ToInt16()) != -1)
                {
                    XtraMessageBox.Show(
                        "No puede duplicarse el número de los enum",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                DuplicadosInt.Add(drRow["Numero"].ToInt16());

                if (DuplicadosString.IndexOf(drRow["Nombre"].ToSafeString()) != -1)
                {
                    XtraMessageBox.Show(
                        "No puede duplicarse el nombre los enum",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                DuplicadosString.Add(drRow["Nombre"].ToSafeString());

                TypeControlEnumIntItem item = new TypeControlEnumIntItem();
                item.Numero = drRow["Numero"].ToInt16();
                item.Nombre = drRow["Nombre"].ToSafeString().Trim();
                item.EtiquetaId = drRow["EtiquetaId"].ToSafeString();
                model.Items.Add(item);
            }

            this.DialogResult = DialogResult.OK;
        }

        private void TypeControlEnumEditor_Load(object sender, EventArgs e)
        {
            DataTable dtEnum = new DataTable();

            dtEnum.Columns.Add("Numero", typeof(System.Int16));
            dtEnum.Columns.Add("Nombre", typeof(System.String));
            dtEnum.Columns.Add("EtiquetaId", typeof(System.String));
            dtEnum.Columns.Add("Texto", typeof(System.String));

            foreach (var item in model.Items)
            {
                DataRow drRow = dtEnum.NewRow();
                drRow["Numero"] = item.Numero;
                drRow["Nombre"] = item.Nombre;
                drRow["EtiquetaId"] = item.EtiquetaId;

                var Etiqueta = proyecto.EtiquetaCol.Etiquetas.Where(j => j.Id == item.EtiquetaId).FirstOrDefault();

                if (Etiqueta != null)
                {
                    var EtiquetaIdioma = Etiqueta.Idiomas.Where(f => f.IdiomaId == proyecto.esMX).FirstOrDefault();
                    if (EtiquetaIdioma != null)
                        drRow["Texto"] = EtiquetaIdioma.Texto;
                }

                dtEnum.Rows.Add(drRow);
            }

            grdEnum.DataSource = dtEnum;

            GridColumn col = grdEnumView.Columns["Numero"];
            col.Width = 30;

            col = grdEnumView.Columns["Nombre"];
            col.Width = 150;
            RepositoryItemTextEdit txtColumna = new RepositoryItemTextEdit();
            txtColumna.MaskSettings.Set(
                "MaskManagerType",
                typeof(DevExpress.Data.Mask.RegularMaskManager)
            );
            txtColumna.MaskSettings.Set(@"mask", @"[A-Z][a-zA-Z0-9]*");
            col.ColumnEdit = txtColumna;

            col = grdEnumView.Columns["EtiquetaId"];
            col.Width = 150;

            RepositoryItemButtonEdit txtEtiquetaId = new RepositoryItemButtonEdit();
            txtEtiquetaId.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            txtEtiquetaId.ButtonClick += TxtEtiquetaId_ButtonClick;

            col.ColumnEdit = txtEtiquetaId;

            grdEnumView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        grdEnumView.Columns["Numero"],
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdEnumView.Columns["Texto"];
            col.Width = 150;
            col.OptionsColumn.AllowEdit = false;
        }

        private void TxtEtiquetaId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow drRow = grdEnumView.GetFocusedDataRow();

            if (drRow == null)
                return;

            fmLabelsSelector modalEditor = new fmLabelsSelector();
            //modalEditor.EstadoCampoCol = (EstadoCampoCollection)value;

            if (modalEditor.ShowDialog() == DialogResult.OK)
            {
                drRow["EtiquetaId"] = modalEditor.Id;
                grdEnumView.SetFocusedValue(modalEditor.Id);
            }
        }

        private void grdEnumView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow drRow = grdEnumView.GetDataRow(e.FocusedRowHandle);

            if (drRow == null)
                return;

            drRow["Texto"] = string.Empty;

            var EtiquetaId = drRow["EtiquetaId"].ToString();

            var Etiqueta = proyecto.EtiquetaCol.Etiquetas.Where(j => j.Id == EtiquetaId).FirstOrDefault();

            if (Etiqueta != null)
            {
                var EtiquetaIdioma = Etiqueta.Idiomas.Where(f => f.IdiomaId == proyecto.esMX).FirstOrDefault();
                if (EtiquetaIdioma != null)
                    drRow["Texto"] = EtiquetaIdioma.Texto;
            }
        }
    }
}