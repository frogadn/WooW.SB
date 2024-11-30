using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs
{
    public partial class fmNumberedExceptionsSelector : DevExpress.XtraEditors.XtraForm
    {
        public const int INVALID_ERROR = -1;
        private const string KEY_GROUP = "key";

        public Proyecto proyecto { get; set; }
        public string NumeredExcepcion { get; set; }

        private bool Servidor;

        public fmNumberedExceptionsSelector(bool servidor)
        {
            InitializeComponent();
            proyecto = Proyecto.getInstance();

            Servidor = servidor;
        }

        private void fmErrrorExceptionSelector_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buSeleccionar_Click(object sender, EventArgs e)
        {
            DataRow drRow = grdMensajeView.GetFocusedDataRow();
            if (drRow == null)
                return;

            string Proceso = drRow[@"Proceso"].ToString();
            int Numero = drRow[@"Numero"].ToInt32();

            if (Servidor)
            {
                string Id = WoNumberedExceptionsHelper.RegresaId(Proceso, Numero);

                var Mensaje = proyecto.MensajeCol.Mensajes.Where(m => m.Id == Id).FirstOrDefault();

                if (Mensaje == null)
                    return;

                var MensajeIdioma = Mensaje
                    .Idiomas.Where(m => m.IdiomaId == proyecto.esMX)
                    .FirstOrDefault();

                if (MensajeIdioma == null)
                    return;

                string Texto = MensajeIdioma.Texto;
                string Solucion = MensajeIdioma.Solucion;

                NumeredExcepcion = fmNumberedExceptionsSelector.GetSampleCode(
                    "throw new WoScriptingException",
                    Proceso,
                    Numero,
                    Texto,
                    Solucion
                );
            }
            else
            {
                NumeredExcepcion = string.Format(
                    @"			try
            {{

            }}
            catch(Exception ex)
			{{
				if(WoNumberedExceptionsHelper.ContieneId(""{0}"", {1}, ex.Message))
				{{

				}}
            }}",
                    Proceso,
                    Numero
                );
            }

            this.DialogResult = DialogResult.OK;
        }

        public void Cargar()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Proceso", typeof(string));
            dt.Columns.Add(@"Numero", typeof(int));
            dt.Columns.Add(@"Texto", typeof(string));
            dt.Columns.Add(@"Solucion", typeof(string));

            foreach (var Etiqueta in proyecto.MensajeCol.Mensajes)
            {
                DataRow drRow = dt.NewRow();

                string Proceso;
                int Numero;
                string Id = Etiqueta.Id;

                if (!WoNumberedExceptionsHelper.SeparaId(Id, out Proceso, out Numero))
                    continue;

                drRow[@"Proceso"] = Proceso;
                drRow[@"Numero"] = Numero;

                var EtiquetaIdioma = Etiqueta
                    .Idiomas.Where(e => e.IdiomaId == proyecto.esMX)
                    .FirstOrDefault();
                if (EtiquetaIdioma != null)
                {
                    drRow[@"Texto"] = EtiquetaIdioma.Texto;
                    drRow[@"Solucion"] = EtiquetaIdioma.Solucion;
                }
                dt.Rows.Add(drRow);
            }

            grdMensaje.DataSource = dt;

            GridColumn col = grdMensajeView.Columns[@"Proceso"];
            col.Width = 100;

            col = grdMensajeView.Columns[@"Numero"];
            col.Width = 100;

            col = grdMensajeView.Columns[@"Texto"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;

            col = grdMensajeView.Columns[@"Solucion"];
            col.OptionsColumn.ReadOnly = true;
            col.Width = 300;
            col.OptionsColumn.AllowFocus = false;
        }

        public static string GetSampleCode(
            string sParFuncion,
            string sParProceso,
            int iParNumeroError,
            string sParDescripcion,
            string sParSolucion
        )
        {
            StringBuilder sbCode = new StringBuilder();

            sbCode.AppendFormat("// Descripción: {0}", sParDescripcion).AppendLine();
            sbCode.AppendFormat("// Solución: {0}", sParSolucion).AppendLine();
            sbCode.AppendFormat("{0}(\"{1}\", {2}", sParFuncion, sParProceso, iParNumeroError);

            Regex keyRegex = new Regex(
                @"({(?<key>\d)})",
                RegexOptions.CultureInvariant | RegexOptions.Compiled
            );
            MatchCollection errorMatches = keyRegex.Matches(sParDescripcion);
            MatchCollection solucionMatches = keyRegex.Matches(sParSolucion);

            List<int> liParameters = new List<int>();
            foreach (Match currMatch in errorMatches)
            {
                int iKey = Convert.ToInt32(currMatch.Groups[KEY_GROUP].Value);

                if (liParameters.IndexOf(iKey) == -1)
                    liParameters.Add(iKey);
            }

            foreach (Match currMatch in solucionMatches)
            {
                int iKey = Convert.ToInt32(currMatch.Groups[KEY_GROUP].Value);

                if (liParameters.IndexOf(iKey) == -1)
                    liParameters.Add(iKey);
            }

            for (int i = 0; i < liParameters.Count; i++)
                sbCode.AppendFormat(", param{0}", i + 1);

            sbCode.Append(");").AppendLine();
            return sbCode.ToString();
        }
    }
}
