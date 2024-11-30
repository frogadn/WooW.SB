using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using DevExpress.XtraGrid.Columns;
using System.Data;
using System.Linq;
using WooW.Core;
using WooW.Core.Common;

namespace WooW.SB.Config.Editors
{
    public partial class woModelInspector : DevExpress.XtraEditors.XtraUserControl
    {
        public woModelInspector()
        {
            InitializeComponent();
            CreaModeloPropiedades();

            woDiagram1.Enabled = true;
            woDiagram1.SetReadOnly(true);

            var language = new CSharpSyntaxLanguage();

            txtScriptPre.Document.Language = language;
            var formatterPre = (CSharpTextFormatter)txtScriptPre.Document.Language.GetTextFormatter();
            formatterPre.IsOpeningBraceOnNewLine = true;

            txtScriptPost.Document.Language = language;
            var formatterPost = (CSharpTextFormatter)txtScriptPost.Document.Language.GetTextFormatter();
            formatterPost.IsOpeningBraceOnNewLine = true;


        }

        private void CreaModeloPropiedades()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Columna", typeof(string));

            dt.Columns.Add(@"Descripcion", typeof(string));

            dt.Columns.Add(@"TipoCol", typeof(string));
            dt.Columns.Add(@"Longitud", typeof(int));
            dt.Columns.Add(@"Precision", typeof(int));

            dt.Columns.Add(@"TipoDato", typeof(string));
            dt.Columns.Add(@"Modelo", typeof(string));


            dt.Columns.Add(@"AceptaNulos", typeof(bool));
            dt.Columns.Add(@"Primaria", typeof(bool));
            dt.Columns.Add(@"Default", typeof(string));
            dt.Columns.Add(@"Control", typeof(string));
            dt.Columns.Add(@"Legacy", typeof(string));

            grdColumna.DataSource = dt;

            GridColumn col = grdColumnaView.Columns["Columna"];
            col.Width = 100;
            col = grdColumnaView.Columns[@"Descripcion"];
            col.Width = 200;
            col = grdColumnaView.Columns["TipoCol"];
            col.Width = 80;
            col.Caption = "Tipo de Columna";
            col = grdColumnaView.Columns["Longitud"];
            col.Width = 50;
            col = grdColumnaView.Columns[@"Precision"];
            col.Width = 50;
            col = grdColumnaView.Columns["TipoDato"];
            col.Width = 100;
            col.Caption = "Tipo de Dato";
            col = grdColumnaView.Columns["Modelo"];
            col.Width = 100;
            col = grdColumnaView.Columns["AceptaNulos"];
            col.Width = 50;
            col.Caption = "Acepta Nulos?";
            col = grdColumnaView.Columns["Primaria"];
            col.Width = 50;
            col.Caption = "LLave Primaria?";
            col = grdColumnaView.Columns["Default"];
            col.Width = 100;
            col.Caption = "Valor Default";
            col = grdColumnaView.Columns["Control"];
            col.Width = 50;
            col = grdColumnaView.Columns["Legacy"];
            col.Width = 100;
        }

        public void ConstruyePropiedadesModelo(Modelo model)
        {
            string[] SystemStringCols = new string[] { WoConst.CREATEDBY, WoConst.MODIFIEDBY, WoConst.DELETEDBY };
            string[] SystemDateCols = new string[] { WoConst.CREATEDDATE, WoConst.MODIFIEDDATE, WoConst.DELETEDDATE };

            woDiagram1.proyecto = model.Proyecto;
            woDiagram1.AsignaDTO(model);

            DataTable dt = grdColumna.DataSource as DataTable;

            dt.Rows.Clear();

            foreach (var col in model.Columnas.OrderBy(e => e.Orden))
            {
                AddColumRow(
                    dt,
                    col.Id,
                    col.Descripcion,
                    col.TipoColumna,
                    col.Longitud,
                    col.Precision,
                    col.TipoDato,
                    col.ModeloId,
                    col.Nulo,
                    col.Primaria,
                    col.Default,
                    col.TipoControl,
                    col.Legacy
                );
            }

            grdColumnaView.BestFitColumns();


            woDiagram1.New();
            woDiagram1.currDiagrama = model.Diagrama;
            woDiagram1.CreaDiagrama();

            txtScriptPre.Text = model.GetPreCondiciones();
            txtScriptPost.Text = model.GetPostCondiciones();

        }

        private void AddColumRow(
            DataTable dt,
            string Columna,
            string Descripcion,
            WoTypeColumn tc,
            int Long,
            int Prec,
            WoTypeData td,
            string Modelo,
            bool AceptaNulos,
            bool Primaria,
            string Default,
            WoTypeControl tco,
            string legacy
        )
        {
            DataRow dr;

            dr = dt.NewRow();

            dr[@"Columna"] = Columna;
            dr[@"Descripcion"] = Descripcion;
            dr[@"TipoCol"] = tc.ToString();
            dr[@"Longitud"] = Long;
            dr[@"Precision"] = Prec;
            dr[@"TipoDato"] = td.ToString();
            dr[@"Modelo"] = Modelo;
            dr[@"AceptaNulos"] = AceptaNulos;
            dr[@"Primaria"] = Primaria;
            dr[@"Default"] = Default;
            dr[@"Control"] = tco.ToString();
            dr[@"Legacy"] = legacy;

            dt.Rows.Add(dr);
        }



    }
}
