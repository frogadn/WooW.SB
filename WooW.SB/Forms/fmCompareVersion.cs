using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using ServiceStack;
using WooW.Core;
using WooW.SB.Class;
using WooW.SB.Config;
using WooW.SB.Config.Helpers;
using WooW.SB.Dialogs.CompareDiagram;
using WooW.SB.Dialogs.CompareIntegralTest;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmCompareVersion : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        private enum eFormularioOGrid
        {
            Formulario,
            Grid
        }

        public WooWConfigParams wooWConfigParams { get; set; }

        private string ArchivoProyecto;
        private bool ShowConfirm = true;

        private Proyecto proyectoFuente = null;

        public Proyecto proyecto { get; set; }

        private DataTable dtFuente = null;

        List<string> Acciones = new List<string>()
        {
            "Modelos",
            "Pre Condiciones",
            "Post Condiciones",
            "Creación de Modelos",
            "Pólizas",
            "Scripts",
            "Etiquetas",
            "Mensajes",
            "Pruebas Unitarias",
            "Pruebas Integrales",
            "Formularios",
            "Esclavas",
            "Listas",
            "Menús",
            "Reportes",
            "Reportes Plantillas"
        };

        string[] ArchivosFormulario = new string[]
        {
            "Controls",
            "ScriptsUser",
            "CustomButtons",
            "ScriptsUserPartial",
            "Validator"
        };

        public fmCompareVersion()
        {
            InitializeComponent();

            proyecto = Proyecto.getInstance();
        }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return false; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Refrescar() { }

        public void Cargar()
        {
            foreach (var item in Acciones)
                repElemento.Items.Add(item);
        }

        #region Control

        private void txtElemento_EditValueChanged(object sender, System.EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (txtElemento.EditValue.ToString() == Acciones[0])
                ComparaModelos();
            else if (txtElemento.EditValue.ToString() == Acciones[1])
                ComparaPreCondiciones();
            else if (txtElemento.EditValue.ToString() == Acciones[2])
                ComparaPostCondiciones();
            else if (txtElemento.EditValue.ToString() == Acciones[3])
                ComparaCreacionModelos();
            else if (txtElemento.EditValue.ToString() == Acciones[4])
                ComparaPolizas();
            else if (txtElemento.EditValue.ToString() == Acciones[5])
                ComparaScripts();
            else if (txtElemento.EditValue.ToString() == Acciones[6])
                ComparaEtiquetas();
            else if (txtElemento.EditValue.ToString() == Acciones[7])
                ComparaMensajes();
            else if (txtElemento.EditValue.ToString() == Acciones[8])
                ComparaPruebaUnitarias();
            else if (txtElemento.EditValue.ToString() == Acciones[9])
                ComparaPruebasIntegrales();
            else if (txtElemento.EditValue.ToString() == Acciones[10])
                ComparaFormulariosYGrids(eFormularioOGrid.Formulario);
            else if (txtElemento.EditValue.ToString() == Acciones[11])
                ComparaEsclavas();
            else if (txtElemento.EditValue.ToString() == Acciones[12])
                ComparaFormulariosYGrids(eFormularioOGrid.Grid);
            else if (txtElemento.EditValue.ToString() == Acciones[13])
                ComparaMenus();
            else if (txtElemento.EditValue.ToString() == Acciones[14])
                ComparaReportes();
            else if (txtElemento.EditValue.ToString() == Acciones[15])
                ComparaReportesPlantillas();

            grdComparacion.ForceInitialize();
            grdComparacionView.BestFitColumns();
        }

        private void buAbrir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Proyecto WooW Service Builder (*.wwsb)|*.wwsb";
                openDialog.FileName = string.Empty;

                string keyName = @"HKEY_CURRENT_USER\SOFTWARE\FROGadn\WooW\";
                string valueName = @"LastCompareDir";
                object LastDir;
                if ((LastDir = Microsoft.Win32.Registry.GetValue(keyName, valueName, null)) != null)
                {
                    if (Directory.Exists(LastDir.ToSafeString()))
                        openDialog.InitialDirectory = LastDir.ToSafeString();
                }

                openDialog.RestoreDirectory = true;

                if (openDialog.ShowDialog() != DialogResult.OK)
                    return;

                ArchivoProyecto = openDialog.FileName;

                if (ArchivoProyecto == proyecto.ArchivoDeProyecto)
                {
                    XtraMessageBox.Show(
                        "El proyecto seleccionado es el mismo que el actual",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                try
                {
                    PackageHelper.IsPackagesCorrecteName(openDialog.FileName);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        ex.Message,
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (Microsoft.Win32.Registry.GetValue(keyName, valueName, null) == null)
                {
                    var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                        @"SOFTWARE\FROGadn\WooW"
                    );
                    key.SetValue(valueName, Path.GetDirectoryName(openDialog.FileName));
                }
                else
                {
                    var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        @"SOFTWARE\FROGadn\WooW",
                        true
                    );
                    key.SetValue(valueName, Path.GetDirectoryName(openDialog.FileName));
                }

                lblFuente.Text = $"Proyecto Fuente: {ArchivoProyecto}";
                lblDestino.Text = $"Proyecto Destino: {proyecto.ArchivoDeProyecto}";
            }

            proyectoFuente = new Proyecto();
            proyectoFuente.Load(ArchivoProyecto);

            txtElemento.Enabled = true;
            optSinDiferencias.Enabled = true;
            buActualizar.Enabled = true;

            txtElemento.EditValue = Acciones[0];
        }

        private void grdComparacionView_CustomDrawCell(
            object sender,
            DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e
        )
        {
            DataRow dtRow = grdComparacionView.GetDataRow(e.RowHandle);

            if (dtRow == null)
                return;

            if (dtRow[@"Destino"].ToString().Contains("No Existe"))
            {
                e.Appearance.BackColor = System.Drawing.Color.LightPink;
                e.Appearance.BackColor2 = System.Drawing.Color.LightPink;
            }
            else if (dtRow[@"Fuente"].ToString().Contains("No Existe"))
            {
                e.Appearance.BackColor = System.Drawing.Color.LightGreen;
                e.Appearance.BackColor2 = System.Drawing.Color.LightGreen;
            }
            else if (dtRow[@"Diferencia"] != DBNull.Value)
            {
                e.Appearance.BackColor = System.Drawing.Color.LightBlue;
                e.Appearance.BackColor2 = System.Drawing.Color.LightBlue;
            }
        }

        private void buCopiar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dtRow = grdComparacionView.GetFocusedDataRow();

            int index = grdComparacionView.FocusedRowHandle;

            if (dtRow[@"Destino"].ToString().Contains("Si Existe"))
            {
                if (ShowConfirm)
                {
                    XtraMessageBoxArgs xma = new XtraMessageBoxArgs();
                    xma.Load += Xma_Load;
                    xma.Closed += Xma_Closed;

                    if (XtraMessageBox.Show(xma) != DialogResult.Yes)
                        //XtraMessageBox.Show(
                        //    "El objeto ya existe en el proyecto actual, sobrescribir?",
                        //    this.Text,
                        //    MessageBoxButtons.YesNo,
                        //    MessageBoxIcon.Question,
                        //    MessageBoxDefaultButton.Button1
                        //) != DialogResult.Yes
                        return;
                }
            }

            if (dtRow[@"Fuente"].ToString().Contains("No Existe"))
            {
                XtraMessageBox.Show(
                    "El objeto no existe en el proyecto comparar",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            string item = dtRow[@"Objeto"].ToString();

            if (txtElemento.EditValue.ToString() == Acciones[0])
            {
                CopiaModelo(item);
                CopiaPrecondiciones(item);
                CopiaPostcondiciones(item);
                CopiaModelCreation(item);
                CopiaModelPoliza(item);
                CopiaScript(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[1])
            {
                CopiaPrecondiciones(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[2])
            {
                CopiaPostcondiciones(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[3])
            {
                CopiaModelCreation(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[4])
            {
                CopiaModelPoliza(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[5])
            {
                CopiaScript(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[6])
            {
                CopiaEtiquetas(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[7])
            {
                CopiaMensajes(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[8])
            {
                CopiaPruebaUnitaria(
                    proyecto.DirProyectData_Test_UnitTest,
                    proyectoFuente.DirProyectData_Test_UnitTest,
                    item
                );
            }
            else if (txtElemento.EditValue.ToString() == Acciones[9])
            {
                CopiaPruebaUnitaria(
                    proyecto.DirProyectData_Test_IntegralTest,
                    proyectoFuente.DirProyectData_Test_IntegralTest,
                    item
                );
            }
            else if (txtElemento.EditValue.ToString() == Acciones[10])
            {
                if (!dtRow[@"ArchivoDiferente"].IsNullOrStringEmpty())
                    CopiaFormularioOGridCodigo(
                        item,
                        dtRow[@"ArchivoDiferente"].ToString(),
                        eFormularioOGrid.Formulario
                    );
                else
                {
                    bool bTodo = dtRow[@"Destino"].ToString().Contains("No Existe");
                    CopiaFormularioOGrid(item, bTodo, eFormularioOGrid.Formulario);
                }
            }
            else if (txtElemento.EditValue.ToString() == Acciones[11])
            {
                CopiaEsclavas(item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[12])
            {
                if (!dtRow[@"ArchivoDiferente"].IsNullOrStringEmpty())
                    CopiaFormularioOGridCodigo(
                        item,
                        dtRow[@"ArchivoDiferente"].ToString(),
                        eFormularioOGrid.Grid
                    );
                else
                {
                    bool bTodo = dtRow[@"Destino"].ToString().Contains("No Existe");
                    CopiaFormularioOGrid(item, bTodo, eFormularioOGrid.Grid);
                }
            }
            else if (txtElemento.EditValue.ToString() == Acciones[13])
            {
                CopiaMenus(proyecto.DirMenus, proyectoFuente.DirMenus, item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[14])
            {
                CopiaReportes(proyecto.DirReportesVistas, proyectoFuente.DirReportesVistas, item);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[15])
            {
                CopiaReportesPlantillas(
                    proyecto.DirReportesPlantillas,
                    proyectoFuente.DirReportesPlantillas,
                    item
                );
            }

            proyecto.Load(proyecto.ArchivoDeProyecto);
            //            proyecto = Proyecto.getInstance();

            LoadData();

            (this.Parent as fmMain).RefrescarLogicaOScript();

            if (index < grdComparacionView.RowCount)
                grdComparacionView.FocusedRowHandle = index;
            else if (grdComparacionView.RowCount > 0)
                grdComparacionView.FocusedRowHandle = grdComparacionView.RowCount - 1;
        }

        private void Xma_Closed(object sender, XtraMessageBoxClosedArgs e)
        {
            if (e.DialogResult == DialogResult.Yes)
            {
                if (!e.Visible)
                    ShowConfirm = false;
            }
            else if (e.DialogResult == DialogResult.Cancel) { }
        }

        private void Xma_Load(object sender, XtraMessageBoxLoadArgs e)
        {
            e.MessageBoxArgs.Caption = this.Text;
            e.MessageBoxArgs.Buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No };
            e.MessageBoxArgs.DoNotShowAgainCheckBoxText = "No volver a mostrar";
            e.MessageBoxArgs.DoNotShowAgainCheckBoxVisible = true;
            e.MessageBoxArgs.Icon = StockIconHelper.GetWindows8AssociatedIcon(SystemIcons.Question);
            e.MessageBoxArgs.Text = "El objeto ya existe en el proyecto actual, sobrescribir?";

            //XtraMessageBox.Show(
            //    "El objeto ya existe en el proyecto actual, sobrescribir?",
            //    this.Text,
            //    MessageBoxButtons.YesNo,
            //    MessageBoxIcon.Question,
            //    MessageBoxDefaultButton.Button1
            //) != DialogResult.Yes
        }

        private void CreaDataSet()
        {
            if (grdComparacion.DataSource != null)
            {
                grdComparacion.DataSource = null;
                grdComparacionView.Columns.Clear();
            }

            dtFuente = new DataTable();

            dtFuente.Columns.Add(@"Objeto", typeof(string));
            dtFuente.Columns.Add(@"Destino", typeof(string));
            dtFuente.Columns.Add(@"Fuente", typeof(string));
            dtFuente.Columns.Add(@"Diferencia", typeof(string));
            dtFuente.Columns.Add(@"ArchivoDiferente", typeof(string));

            grdComparacion.DataSource = dtFuente;

            grdComparacionView.Columns[@"ArchivoDiferente"].Visible = false;
        }

        private void grdComparacionView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            DataRow dtRow = grdComparacionView.GetDataRow(e.FocusedRowHandle);

            if (dtRow == null)
            {
                buCopiar.Enabled = false;
                buComparaCodigo.Enabled = false;
                buNormalizar.Enabled = false;
                return;
            }

            buCopiar.Enabled =
                (!dtRow[@"Destino"].ToString().Contains("Si Existe"))
                || (!dtRow[@"Diferencia"].IsNullOrStringEmpty());
            if (dtRow[@"Fuente"].ToString().Contains("No Existe"))
                buCopiar.Enabled = false;

            buNormalizar.Enabled = buComparaCodigo.Enabled = dtRow[@"Diferencia"]
                .ToString()
                .Contains("Código Diferente");

            //if (
            //    (!dtRow[@"ArchivoDiferente"].IsNullOrStringEmpty())
            //    || (
            //        (dtRow[@"Diferencia"].ToString() == @"Diferentes")
            //        && (
            //            (txtElemento.EditValue.ToString() == Acciones[10])
            //            || (txtElemento.EditValue.ToString() == Acciones[11])
            //            || (txtElemento.EditValue.ToString() == Acciones[12])
            //            || (txtElemento.EditValue.ToString() == Acciones[13])
            //        )
            //    )
            //)
            //    buCopiar.Enabled = true;

            buComparaModelo.Enabled =
                (
                    (txtElemento.EditValue.ToString() == Acciones[0])
                    || (txtElemento.EditValue.ToString() == Acciones[9])
                )
                && (dtRow[@"Destino"].ToString().Contains("Si Existe"))
                && (dtRow[@"Fuente"].ToString().Contains("Si Existe"));

            buComparaDiagrama.Enabled =
                (txtElemento.EditValue.ToString() == Acciones[0])
                && (dtRow[@"Destino"].ToString().Contains("Si Existe"))
                && (dtRow["Fuente"].ToString().Contains("Si Existe"));
        }

        #endregion Control

        #region ComparaScripts
        private void ComparaScripts()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var modelo in proyecto.ModeloCol.Modelos)
                if (modelo.bScriptExist)
                    Merge.Add(modelo.Id);
            foreach (var modelo in proyectoFuente.ModeloCol.Modelos)
                if (!Merge.Contains(modelo.Id))
                    if (modelo.bScriptExist)
                        Merge.Add(modelo.Id);

            Merge.Sort();
            foreach (var modelo in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = modelo;

                var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloDestino != null) && (modeloDestino.bScriptExist))
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloFuente != null) && (modeloFuente.bScriptExist))
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if (
                    (modeloDestino != null)
                    && (modeloFuente != null)
                    && (modeloDestino.bScriptExist)
                    && (modeloFuente.bScriptExist)
                )
                {
                    if (modeloDestino.GetScript() != modeloFuente.GetScript())
                        dr["Diferencia"] = "Código Diferente";
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion ComparaScripts

        #region ComparaCreacionModelos
        private void ComparaCreacionModelos()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var modelo in proyecto.ModeloCol.Modelos)
                if (modelo.bModelCreationExist)
                    Merge.Add(modelo.Id);
            foreach (var modelo in proyectoFuente.ModeloCol.Modelos)
                if (!Merge.Contains(modelo.Id))
                    if (modelo.bModelCreationExist)
                        Merge.Add(modelo.Id);

            Merge.Sort();
            foreach (var modelo in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = modelo;

                var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloDestino != null) && (modeloDestino.bModelCreationExist))
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloFuente != null) && (modeloFuente.bModelCreationExist))
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if (
                    (modeloDestino != null)
                    && (modeloFuente != null)
                    && (modeloDestino.bModelCreationExist)
                    && (modeloFuente.bModelCreationExist)
                )
                {
                    if (modeloDestino.GetModelCreation() != modeloFuente.GetModelCreation())
                        dr["Diferencia"] = "Código Diferente";
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion

        #region ComparaPostCondiciones
        private void ComparaPostCondiciones()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var modelo in proyecto.ModeloCol.Modelos)
                if (modelo.bPostCondicionesExist)
                    Merge.Add(modelo.Id);
            foreach (var modelo in proyectoFuente.ModeloCol.Modelos)
                if (!Merge.Contains(modelo.Id))
                    if (modelo.bPostCondicionesExist)
                        Merge.Add(modelo.Id);

            Merge.Sort();
            foreach (var modelo in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = modelo;

                var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloDestino != null) && (modeloDestino.bPostCondicionesExist))
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloFuente != null) && (modeloFuente.bPostCondicionesExist))
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if (
                    (modeloDestino != null)
                    && (modeloFuente != null)
                    && (modeloDestino.bPostCondicionesExist)
                    && (modeloFuente.bPostCondicionesExist)
                )
                {
                    if (modeloDestino.GetPostCondiciones() != modeloFuente.GetPostCondiciones())
                        dr["Diferencia"] = "Código Diferente";
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion ComparaPostCondiciones

        #region ComparaPreCondiciones
        private void ComparaPreCondiciones()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var modelo in proyecto.ModeloCol.Modelos)
                if (modelo.bPreCondicionesExist)
                    Merge.Add(modelo.Id);
            foreach (var modelo in proyectoFuente.ModeloCol.Modelos)
                if (!Merge.Contains(modelo.Id))
                    if (modelo.bPreCondicionesExist)
                        Merge.Add(modelo.Id);

            Merge.Sort();
            foreach (var modelo in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = modelo;

                var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloDestino != null) && (modeloDestino.bPreCondicionesExist))
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloFuente != null) && (modeloFuente.bPreCondicionesExist))
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if (
                    (modeloDestino != null)
                    && (modeloFuente != null)
                    && (modeloDestino.bPreCondicionesExist)
                    && (modeloFuente.bPreCondicionesExist)
                )
                {
                    if (modeloDestino.GetPreCondiciones() != modeloFuente.GetPreCondiciones())
                        dr["Diferencia"] = "Código Diferente";
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion ComparaPreCondiciones

        #region ComparaPolizas
        private void ComparaPolizas()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var modelo in proyecto.ModeloCol.Modelos)
                if (modelo.bModelPolizaExist)
                    Merge.Add(modelo.Id);
            foreach (var modelo in proyectoFuente.ModeloCol.Modelos)
                if (!Merge.Contains(modelo.Id))
                    if (modelo.bModelPolizaExist)
                        Merge.Add(modelo.Id);

            Merge.Sort();
            foreach (var modelo in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = modelo;

                var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloDestino != null) && (modeloDestino.bModelPolizaExist))
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if ((modeloFuente != null) && (modeloFuente.bModelPolizaExist))
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if (
                    (modeloDestino != null)
                    && (modeloFuente != null)
                    && (modeloDestino.bModelPolizaExist)
                    && (modeloFuente.bModelPolizaExist)
                )
                {
                    if (modeloDestino.GetModelPoliza() != modeloFuente.GetModelPoliza())
                        dr["Diferencia"] = "Código Diferente";
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion ComparaPolizas

        #region ComparaMensajes
        private void ComparaMensajes()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var mensaje in proyecto.MensajeCol.Mensajes)
                Merge.Add(mensaje.Id);
            foreach (var mensaje in proyectoFuente.MensajeCol.Mensajes)
                if (!Merge.Contains(mensaje.Id))
                    Merge.Add(mensaje.Id);

            Merge.Sort();
            foreach (var mensaje in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = mensaje;

                var mensajeDestino = proyecto.MensajeCol.Mensajes.Find(x => x.Id == mensaje);

                if (mensajeDestino != null)
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var mensajeFuente = proyectoFuente.MensajeCol.Mensajes.Find(x => x.Id == mensaje);

                if (mensajeFuente != null)
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if ((mensajeDestino != null) && (mensajeFuente != null))
                {
                    if (mensajeDestino.ToJson() != mensajeFuente.ToJson())
                        dr["Diferencia"] = "Diferente";
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion ComparaMensajes

        #region ComparaEtiquetas
        private void ComparaEtiquetas()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var etiqueta in proyecto.EtiquetaCol.Etiquetas)
                Merge.Add(etiqueta.Id);
            foreach (var etiqueta in proyectoFuente.EtiquetaCol.Etiquetas)
                if (!Merge.Contains(etiqueta.Id))
                    Merge.Add(etiqueta.Id);

            Merge.Sort();
            foreach (var etiqueta in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = etiqueta;

                var etiquetaDestino = proyecto.EtiquetaCol.Etiquetas.Find(x => x.Id == etiqueta);

                if (etiquetaDestino != null)
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var etiquetaFuente = proyectoFuente.EtiquetaCol.Etiquetas.Find(x =>
                    x.Id == etiqueta
                );

                if (etiquetaFuente != null)
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if ((etiquetaDestino != null) && (etiquetaFuente != null))
                {
                    if (etiquetaDestino.ToJson() != etiquetaFuente.ToJson())
                        dr["Diferencia"] = "Diferentes";
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion ComparaEtiquetas

        #region ComparaModelos
        private void ComparaModelos()
        {
            CreaDataSet();

            List<string> Merge = new List<string>();
            foreach (var modelo in proyecto.ModeloCol.Modelos)
                Merge.Add(modelo.Id);
            foreach (var modelo in proyectoFuente.ModeloCol.Modelos)
                if (!Merge.Contains(modelo.Id))
                    Merge.Add(modelo.Id);

            Merge.Sort();
            foreach (var modelo in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = modelo;

                var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if (modeloDestino != null)
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                if (modeloFuente != null)
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if ((modeloDestino != null) && (modeloFuente != null))
                {
                    if (modeloDestino.Columnas.Count != modeloFuente.Columnas.Count)
                        dr["Diferencia"] = "Número de columnas diferentes";
                    else
                    {
                        foreach (var columna in modeloDestino.Columnas)
                        {
                            var columnaComparar = modeloFuente.Columnas.Find(x =>
                                x.Id == columna.Id
                            );

                            if (columnaComparar == null)
                            {
                                dr["Diferencia"] = $"Columna {columna.Id} no existe en Comparar";
                                break;
                            }
                            else
                            {
                                if (columna.ToJson() != columnaComparar.ToJson())
                                    dr["Diferencia"] =
                                        $"Columna {columna.Id} tiene propiedades diferentes";
                                break;
                            }
                        }

                        if (dr["Diferencia"] == DBNull.Value)
                        {
                            foreach (var columna in modeloFuente.Columnas)
                            {
                                var columnaDestino = modeloDestino.Columnas.Find(x =>
                                    x.Id == columna.Id
                                );

                                if (columnaDestino == null)
                                {
                                    dr["Diferencia"] = $"Columna {columna.Id} no existe en Actual";
                                    break;
                                }
                            }
                        }

                        if (dr["Diferencia"] == DBNull.Value)
                        {
                            if (modeloDestino.Diagrama.ToJson() != modeloFuente.Diagrama.ToJson())
                                dr["Diferencia"] = "Diagramas diferentes";
                            else if (modeloDestino.ToJson() != modeloFuente.ToJson())
                                dr["Diferencia"] = "Propiedades diferentes";
                        }
                    }
                }

                if (optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }

        #region ComparaPruebaUnitarias
        private void ComparaPruebaUnitarias()
        {
            ComparaPrueba(
                proyecto.DirProyectData_Test_UnitTest,
                proyectoFuente.DirProyectData_Test_UnitTest,
                "*.cs"
            );
        }

        private void ComparaEsclavas()
        {
            ComparaPrueba(proyecto.DirGrids, proyectoFuente.DirGrids, "*.Json", false);
        }

        public string PrettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);

            return JsonSerializer.Serialize(jsonElement, options);
        }

        private void ComparaPrueba(
            string DirDestino,
            string DirFuente,
            string Mask,
            bool bTodos = false
        )
        {
            CreaDataSet();

            List<string> Merge = new List<string>();

            List<string> ProyectFilesDestino = Directory
                .GetFiles(DirDestino, Mask, SearchOption.AllDirectories)
                .ToList();
            List<string> ProyectFilesFuente = Directory
                .GetFiles(DirFuente, Mask, SearchOption.AllDirectories)
                .ToList();

            foreach (var file in ProyectFilesDestino)
            {
                var relative = file.Substring(DirDestino.Length + 1);
                Merge.Add(relative);
            }

            foreach (var file in ProyectFilesFuente)
            {
                var relative = file.Substring(DirFuente.Length + 1);
                if (!Merge.Contains(relative))
                    Merge.Add(relative);
            }

            Merge.Sort();
            foreach (var modelo in Merge)
            {
                DataRow dr = dtFuente.NewRow();
                dr["Objeto"] = modelo;

                var archivoDestino = Path.Combine(DirDestino, modelo);
                var archivoFuente = Path.Combine(DirFuente, modelo);

                var indexDestino = ProyectFilesDestino.IndexOf(archivoDestino);
                var indexFuente = ProyectFilesFuente.IndexOf(archivoFuente);

                if (indexDestino != -1)
                    dr["Destino"] = "Si Existe";
                else
                    dr["Destino"] = "No Existe";

                if (indexFuente != -1)
                    dr["Fuente"] = "Si Existe";
                else
                    dr["Fuente"] = "No Existe";

                if ((indexDestino != -1) && (indexFuente != -1))
                {
                    string ContenidoDestino = File.ReadAllText(archivoDestino);
                    string ContenidoFuente = File.ReadAllText(archivoFuente);

                    if (archivoDestino.ToLower().EndsWith("json"))
                    {
                        ContenidoDestino = PrettyJson(ContenidoDestino);
                        ContenidoFuente = PrettyJson(ContenidoFuente);
                    }

                    if (ContenidoDestino != ContenidoFuente)
                    {
                        if (Mask == "*.cs")
                            dr["Diferencia"] = "Código Diferente";
                        else
                            dr["Diferencia"] = "Diferentes";
                    }
                }

                if (!bTodos && optSinDiferencias.EditValue.ToBoolean())
                {
                    if (
                        (dr["Diferencia"] != DBNull.Value)
                        || (dr["Destino"] != "Si Existe")
                        || (dr["Fuente"] != "Si Existe")
                    )
                        dtFuente.Rows.Add(dr);
                }
                else
                {
                    dtFuente.Rows.Add(dr);
                }
            }
        }
        #endregion ComparaPruebaUnitarias

        #region ComparaPruebasIntegrales
        private void ComparaPruebasIntegrales()
        {
            ComparaPrueba(
                proyecto.DirProyectData_Test_IntegralTest,
                proyectoFuente.DirProyectData_Test_IntegralTest,
                "*.Json"
            );
        }
        #endregion ComparaPruebasIntegrales


        #region ComparaFormulariosYGrids
        private void ComparaFormulariosYGrids(eFormularioOGrid formularioOGrid)
        {
            if (formularioOGrid == eFormularioOGrid.Formulario)
                ComparaPrueba(proyecto.DirFormDesign, proyectoFuente.DirFormDesign, "*.Json", true);
            else
                ComparaPrueba(proyecto.DirListDesign, proyectoFuente.DirListDesign, "*.Json", true);

            DataTable dtFuente = (DataTable)grdComparacion.DataSource;

            foreach (DataRow dr in dtFuente.Rows)
                dr[@"ArchivoDiferente"] = string.Empty;

            // Checa el Codigo
            foreach (DataRow dr in dtFuente.Rows)
            {
                if (
                    (dr["Destino"].ToString().Contains("No Existe"))
                    || (dr["Fuente"].ToString().Contains("No Existe"))
                )
                    continue;

                string SubDir;

                if (formularioOGrid == eFormularioOGrid.Formulario)
                    SubDir = Path.GetFileNameWithoutExtension(dr["Objeto"].ToString()) + "_proj";
                else
                    SubDir =
                        Path.GetFileNameWithoutExtension(dr["Objeto"].ToString()) + "GridList_proj";

                string DirDestino = Path.Combine(proyecto.DirFormDesignUserCode, SubDir);
                string DirFuente = Path.Combine(proyectoFuente.DirFormDesignUserCode, SubDir);

                // Comparar
                // AuxiliarContableRequestControls
                // AuxiliarContableRequestScriptsUser
                // AuxiliarContableRequestScriptsUserPartial
                // AuxiliarContableRequestValidator

                if (dr["Diferencia"].IsNullOrStringEmpty())
                {
                    foreach (string Archivo in ArchivosFormulario)
                    {
                        string ArchivoFull =
                            Path.GetFileNameWithoutExtension(dr["Objeto"].ToString())
                            + (formularioOGrid == eFormularioOGrid.Formulario ? "" : "GridList")
                            + Archivo
                            + (Archivo == "CustomButtons" ? ".json" : ".cs");
                        string ArchivoFuente = Path.Combine(DirFuente, ArchivoFull);
                        string ArchivoDestino = Path.Combine(DirDestino, ArchivoFull);

                        if (File.Exists(ArchivoFuente))
                        {
                            if (File.Exists(ArchivoDestino))
                            {
                                string ContenidoFuente = File.ReadAllText(ArchivoFuente);
                                string ContenidoDestino = File.ReadAllText(ArchivoDestino);

                                if (ContenidoFuente != ContenidoDestino)
                                {
                                    dr["Diferencia"] =
                                        dr["Diferencia"].ToString()
                                        + " Código Diferente "
                                        + ArchivoFull;
                                    dr[@"ArchivoDiferente"] = ArchivoFull;
                                    break;
                                }
                            }
                            else
                            {
                                dr["Destino"] = "No Existe";
                                dr["Diferencia"] =
                                    dr["Diferencia"].ToString()
                                    + " No Existe Código Destino "
                                    + ArchivoFull;
                                break;
                            }
                        }
                        else
                        {
                            if (File.Exists(ArchivoDestino))
                            {
                                dr["Fuente"] = "No Existe";
                                dr["Diferencia"] =
                                    dr["Diferencia"].ToString()
                                    + " No Existe Código Fuente "
                                    + ArchivoFull;
                            }
                            break;
                        }
                    }
                }
            }

            if (optSinDiferencias.EditValue.ToBoolean())
            {
                List<DataRow> RenglonesABorrar = new List<DataRow>();

                // Checa el Codigo
                foreach (DataRow dr in dtFuente.Rows)
                {
                    if (
                        (dr["Diferencia"].IsNullOrStringEmpty())
                        && (!dr["Fuente"].ToString().Contains("No Existe"))
                        && (!dr["Destino"].ToString().Contains("No Existe"))
                    )
                        RenglonesABorrar.Add(dr);
                }

                // Borra los renglones
                foreach (DataRow dr in RenglonesABorrar)
                    dtFuente.Rows.Remove(dr);
            }
        }
        #endregion ComparaFormulariosYGrids

        #region ComparaMenus
        private void ComparaMenus()
        {
            ComparaPrueba(proyecto.DirMenus, proyectoFuente.DirMenus, "*.Json");
        }
        #endregion ComparaMenus


        #region ComparaReportes
        private void ComparaReportes()
        {
            ComparaPrueba(proyecto.DirReportesVistas, proyectoFuente.DirReportesVistas, "*.Json");
        }
        #endregion ComparaReportes

        #region ComparaReportesPlantillas
        private void ComparaReportesPlantillas()
        {
            ComparaPrueba(
                proyecto.DirReportesPlantillas,
                proyectoFuente.DirReportesPlantillas,
                "*.Json"
            );
        }
        #endregion ComparaReportesPlantillas


        #endregion ComparaModelos

        #region Copias
        private void CopiaModelo(string modelo)
        {
            // Copia Modelo
            if (File.Exists(Path.Combine(proyectoFuente.DirModel, modelo + ".Model.json")))
            {
                File.Copy(
                    Path.Combine(proyectoFuente.DirModel, modelo + ".Model.json"),
                    Path.Combine(proyecto.DirModel, modelo + ".Model.json"),
                    true
                );

                var locModelo = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);
                if (locModelo != null)
                {
                    var etiquetaComparar = proyectoFuente.EtiquetaCol.Etiquetas.Find(x =>
                        x.Id == locModelo.EtiquetaId
                    );

                    if (etiquetaComparar != null)
                    {
                        if (!proyecto.EtiquetaCol.Etiquetas.Any(e => e.Id == etiquetaComparar.Id))
                        {
                            proyecto.EtiquetaCol.Etiquetas.Add(etiquetaComparar.Clone());
                            proyecto.EtiquetasCambiadas = true;
                            proyecto.SaveLabels();
                        }
                    }
                }
            }
        }

        private void CopiaPrecondiciones(string modelo)
        {
            string Archivo = modelo + "PreConditions.cs";

            // Copia Precondiciones
            if (File.Exists(Path.Combine(proyectoFuente.DirProyectDataLogicPreConditions, Archivo)))
                File.Copy(
                    Path.Combine(proyectoFuente.DirProyectDataLogicPreConditions, Archivo),
                    Path.Combine(proyecto.DirProyectDataLogicPreConditions, Archivo),
                    true
                );
        }

        private void CopiaEsclavas(string modelo)
        {
            string Archivo = modelo;

            // Copia Grids
            if (File.Exists(Path.Combine(proyectoFuente.DirGrids, Archivo)))
                File.Copy(
                    Path.Combine(proyectoFuente.DirGrids, Archivo),
                    Path.Combine(proyecto.DirGrids, Archivo),
                    true
                );
        }

        private void CopiaPostcondiciones(string modelo)
        {
            string Archivo = modelo + "PostConditions.cs";

            // Copia Postcondiciones
            if (
                File.Exists(Path.Combine(proyectoFuente.DirProyectDataLogicPostConditions, Archivo))
            )
                File.Copy(
                    Path.Combine(proyectoFuente.DirProyectDataLogicPostConditions, Archivo),
                    Path.Combine(proyecto.DirProyectDataLogicPostConditions, Archivo),
                    true
                );
        }

        private void CopiaModelCreation(string modelo)
        {
            string Archivo = modelo + "ModelCreation.cs";

            // Copia Model Creation
            if (File.Exists(Path.Combine(proyectoFuente.DirProyectDataLogicModelCreation, Archivo)))
                File.Copy(
                    Path.Combine(proyectoFuente.DirProyectDataLogicModelCreation, Archivo),
                    Path.Combine(proyecto.DirProyectDataLogicModelCreation, Archivo),
                    true
                );
        }

        private void CopiaModelPoliza(string modelo)
        {
            string Archivo = modelo + "ModelPoliza.cs";

            // Copia Model Poliza
            if (File.Exists(Path.Combine(proyectoFuente.DirProyectDataLogicModelPoliza, Archivo)))
                File.Copy(
                    Path.Combine(proyectoFuente.DirProyectDataLogicModelPoliza, Archivo),
                    Path.Combine(proyecto.DirProyectDataLogicModelPoliza, Archivo),
                    true
                );
        }

        private void CopiaScript(string modelo)
        {
            string Archivo = modelo + "Script.cs";

            // Copia Model Poliza
            if (File.Exists(Path.Combine(proyectoFuente.DirProyectDataLogicScripts, Archivo)))
                File.Copy(
                    Path.Combine(proyectoFuente.DirProyectDataLogicScripts, Archivo),
                    Path.Combine(proyecto.DirProyectDataLogicScripts, Archivo),
                    true
                );
        }

        private void CopiaMensajes(string mensaje)
        {
            var mensajeComparar = proyectoFuente.MensajeCol.Mensajes.Find(x => x.Id == mensaje);

            proyecto.MensajeCol.Mensajes.Add(mensajeComparar.Clone());
            proyecto.EtiquetasCambiadas = true;
            proyecto.SaveMessages();

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow["Destino"] = "Si Existe";
        }

        private void CopiaEtiquetas(string etiqueta)
        {
            var etiquetaComparar = proyectoFuente.EtiquetaCol.Etiquetas.Find(x => x.Id == etiqueta);

            proyecto.EtiquetaCol.Etiquetas.Add(etiquetaComparar.Clone());
            proyecto.EtiquetasCambiadas = true;
            proyecto.SaveLabels();

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow["Destino"] = "Si Existe";
        }

        private void CopiaPruebaUnitaria(string DirDestino, string DirFuente, string prueba)
        {
            string archivoDestino = Path.Combine(DirDestino, prueba);
            string archivoFuente = Path.Combine(DirFuente, prueba);

            string path = Path.GetDirectoryName(archivoDestino);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(archivoFuente))
                File.Copy(archivoFuente, archivoDestino, true);

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow["Destino"] = "Si Existe";
        }

        private void CopiaFormularioOGrid(
            string formulario,
            bool bTodo,
            eFormularioOGrid formularioOGrid
        )
        {
            string archivoDestino;
            string archivoFuente;

            if (formularioOGrid == eFormularioOGrid.Formulario)
            {
                archivoDestino = Path.Combine(proyecto.DirFormDesign, formulario);
                archivoFuente = Path.Combine(proyectoFuente.DirFormDesign, formulario);
            }
            else
            {
                archivoDestino = Path.Combine(proyecto.DirListDesign, formulario);
                archivoFuente = Path.Combine(proyectoFuente.DirListDesign, formulario);
            }

            string path = Path.GetDirectoryName(archivoDestino);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(archivoFuente))
                File.Copy(archivoFuente, archivoDestino, true);

            string SubDir;

            if (formularioOGrid == eFormularioOGrid.Formulario)
                SubDir = Path.GetFileNameWithoutExtension(formulario) + "_proj";
            else
                SubDir = Path.GetFileNameWithoutExtension(formulario) + "GridList_proj";

            string DirDestino = Path.Combine(proyecto.DirFormDesignUserCode, SubDir);
            string DirFuente = Path.Combine(proyectoFuente.DirFormDesignUserCode, SubDir);

            if (!bTodo)
                return;

            // Comparar
            // AuxiliarContableRequestControls
            // AuxiliarContableRequestScriptsUser
            // AuxiliarContableRequestScriptsUserPartial
            // AuxiliarContableRequestValidator


            foreach (string Archivo in ArchivosFormulario)
            {
                string ArchivoFull =
                    Path.GetFileNameWithoutExtension(formulario)
                    + (formularioOGrid == eFormularioOGrid.Formulario ? "" : "GridList")
                    + Archivo
                    + (Archivo == "CustomButtons" ? ".json" : ".cs");
                archivoFuente = Path.Combine(DirFuente, ArchivoFull);
                archivoDestino = Path.Combine(DirDestino, ArchivoFull);

                path = Path.GetDirectoryName(archivoDestino);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (File.Exists(archivoFuente))
                    File.Copy(archivoFuente, archivoDestino, true);
            }

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow["Destino"] = "Si Existe";
            dtRow["Diferencia"] = "";
        }

        private void CopiaFormularioOGridCodigo(
            string modelo,
            string archivo,
            eFormularioOGrid formularioOGrid
        )
        {
            string SubDir;

            if (formularioOGrid == eFormularioOGrid.Formulario)
                SubDir = Path.GetFileNameWithoutExtension(modelo) + "_proj";
            else
                SubDir = Path.GetFileNameWithoutExtension(modelo) + "GridList_proj";

            string DirDestino = Path.Combine(proyecto.DirFormDesignUserCode, SubDir);
            string DirFuente = Path.Combine(proyectoFuente.DirFormDesignUserCode, SubDir);

            // Comparar
            // AuxiliarContableRequestControls
            // AuxiliarContableRequestScriptsUser
            // AuxiliarContableRequestScriptsUserPartial
            // AuxiliarContableRequestValidator

            string archivoFuente = Path.Combine(DirFuente, archivo);
            string archivoDestino = Path.Combine(DirDestino, archivo);

            string path = Path.GetDirectoryName(archivoDestino);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(archivoFuente))
                File.Copy(archivoFuente, archivoDestino, true);

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow[@"Destino"] = "Si Existe";
            dtRow[@"Diferencia"] = "";
            dtRow[@"ArchivoDiferente"] = "";
        }

        private void CopiaMenus(string DirDestino, string DirFuente, string menu)
        {
            string archivoDestino = Path.Combine(DirDestino, menu);
            string archivoFuente = Path.Combine(DirFuente, menu);

            string path = Path.GetDirectoryName(archivoDestino);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(archivoFuente))
                File.Copy(archivoFuente, archivoDestino, true);

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow["Destino"] = "Si Existe";
        }

        private void CopiaReportes(string DirDestino, string DirFuente, string reporte)
        {
            string archivoDestino = Path.Combine(DirDestino, reporte);
            string archivoFuente = Path.Combine(DirFuente, reporte);

            string path = Path.GetDirectoryName(archivoDestino);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(archivoFuente))
                File.Copy(archivoFuente, archivoDestino, true);

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow["Destino"] = "Si Existe";
        }

        private void CopiaReportesPlantillas(string DirDestino, string DirFuente, string reporte)
        {
            string archivoDestino = Path.Combine(DirDestino, reporte);
            string archivoFuente = Path.Combine(DirFuente, reporte);

            string path = Path.GetDirectoryName(archivoDestino);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(archivoFuente))
                File.Copy(archivoFuente, archivoDestino, true);

            DataRow dtRow = grdComparacionView.GetFocusedDataRow();
            dtRow["Destino"] = "Si Existe";
        }

        #endregion Copias

        private void buComparaCodigo_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            DataRow drRow = grdComparacionView.GetFocusedDataRow();

            if (drRow == null)
                return;

            if (
                (drRow["Destino"].ToString().Contains("No Existe"))
                || (drRow["Fuente"].ToString().Contains("No Existe"))
            )
                return;

            string modelo = drRow["Objeto"].ToString();

            var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);
            var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

            string commandLineArgs = "/diff ";

            if (txtElemento.EditValue.ToString() == Acciones[1])
                commandLineArgs +=
                    $"\"{modeloFuente.PreConditionsFile}\" \"{modeloDestino.PreConditionsFile}\"";
            else if (txtElemento.EditValue.ToString() == Acciones[2])
                commandLineArgs +=
                    $"\"{modeloFuente.PostConditionsFile}\" \"{modeloDestino.PostConditionsFile}\"";
            else if (txtElemento.EditValue.ToString() == Acciones[3])
                commandLineArgs +=
                    $"\"{modeloFuente.ModelCreationFile}\" \"{modeloDestino.ModelCreationFile}\"";
            else if (txtElemento.EditValue.ToString() == Acciones[4])
                commandLineArgs +=
                    $"\"{modeloFuente.ModelPolizaFile}\" \"{modeloDestino.ModelPolizaFile}\"";
            else if (txtElemento.EditValue.ToString() == Acciones[5])
                commandLineArgs += $"\"{modeloFuente.ScriptFile}\" \"{modeloDestino.ScriptFile}\"";
            else if (txtElemento.EditValue.ToString() == Acciones[8])
            {
                var archivoDestino = Path.Combine(proyecto.DirProyectData_Test_UnitTest, modelo);
                var archivoFuente = Path.Combine(
                    proyectoFuente.DirProyectData_Test_UnitTest,
                    modelo
                );
                commandLineArgs += $"\"{archivoFuente}\" \"{archivoDestino}\"";
            }
            else if (txtElemento.EditValue.ToString() == Acciones[9])
            {
                var archivoDestino = Path.Combine(
                    proyecto.DirProyectData_Test_IntegralTest,
                    modelo
                );
                var archivoFuente = Path.Combine(
                    proyectoFuente.DirProyectData_Test_IntegralTest,
                    modelo
                );
                commandLineArgs += $"\"{archivoFuente}\" \"{archivoDestino}\"";
            }
            else if (txtElemento.EditValue.ToString() == Acciones[10])
            {
                if (drRow[@"ArchivoDiferente"].IsNullOrStringEmpty())
                    return;

                var archivo =
                    Path.GetFileNameWithoutExtension(modelo)
                    + "_proj\\"
                    + drRow[@"ArchivoDiferente"].ToString();

                var archivoDestino = Path.Combine(proyecto.DirFormDesignUserCode, archivo);
                var archivoFuente = Path.Combine(proyectoFuente.DirFormDesignUserCode, archivo);
                commandLineArgs += $"\"{archivoFuente}\" \"{archivoDestino}\"";
            }
            else
                return;

            woVisualStudio.AbreVisualStudio(commandLineArgs, false);
        }

        private void buActualizar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void buComparaModelo_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            DataRow drRow = grdComparacionView.GetFocusedDataRow();

            if (drRow == null)
                return;

            if (
                (drRow["Destino"].ToString().Contains("No Existe"))
                || (drRow["Fuente"].ToString().Contains("No Existe"))
            )
                return;

            string modelo = drRow["Objeto"].ToString();

            if (txtElemento.EditValue.ToString() == Acciones[0])
            {
                var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);
                var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                var mDestino = modeloDestino.Clone(proyecto);
                var mFuente = modeloFuente.Clone(proyecto);

                mDestino.Diagrama = null;
                mFuente.Diagrama = null;

                if (mDestino.ToJson() == mFuente.ToJson())
                {
                    XtraMessageBox.Show(
                        "Las propiedades de los modelos son iguales",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                fmCompareModel fmCompare = new fmCompareModel();

                fmCompare.ModelSetter(proyecto, proyectoFuente, modeloDestino, modeloFuente);

                if (fmCompare.ShowDialog() == DialogResult.OK)
                {
                    modeloDestino.Columnas.Clear();

                    foreach (var columna in fmCompare.modeloDestinoCambiado.Columnas)
                        modeloDestino.Columnas.Add(columna.Clone(proyecto));

                    modeloDestino.TipoModelo = modeloFuente.TipoModelo;
                    modeloDestino.SubTipoModelo = modeloFuente.SubTipoModelo;
                    modeloDestino.ProcesoId = modeloFuente.ProcesoId;
                    modeloDestino.EtiquetaId = modeloFuente.EtiquetaId;
                    modeloDestino.OrdenDeCreacion = modeloFuente.OrdenDeCreacion;
                    modeloDestino.Id = modeloFuente.Id;
                    modeloDestino.Legacy = modeloFuente.Legacy;
                    modeloDestino.Interface1 = modeloFuente.Interface1;
                    modeloDestino.Interface2 = modeloFuente.Interface2;
                    modeloDestino.Interface3 = modeloFuente.Interface3;
                    modeloDestino.Apps = modeloFuente.Apps.ToList();
                    modeloDestino.ScriptVistaRoles = modeloFuente.ScriptVistaRoles.Clone();

                    proyecto.SaveModel(modeloDestino);

                    buActualizar.PerformClick();
                }
            }
            else if (txtElemento.EditValue.ToString() == Acciones[9])
            {
                var archivoDestino = Path.Combine(
                    proyecto.DirProyectData_Test_IntegralTest,
                    modelo
                );
                var archivoFuente = Path.Combine(
                    proyectoFuente.DirProyectData_Test_IntegralTest,
                    modelo
                );

                if (File.ReadAllText(archivoDestino) == File.ReadAllText(archivoFuente))
                {
                    XtraMessageBox.Show(
                        "Las propiedades de las prueba integrales son iguales",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                fmCompareIntegralTest fmCompare = new fmCompareIntegralTest(
                    archivoDestino,
                    archivoFuente
                );

                if (fmCompare.ShowDialog() == DialogResult.OK)
                {
                    fmCompare.PruebaDestino.Pruebas = fmCompare
                        .PruebaDestino.Pruebas.OrderBy(o => o.Orden)
                        .ToList();
                    File.WriteAllText(archivoDestino, fmCompare.PruebaDestino.ToJson());
                    LoadData();
                }
            }
        }

        private void buComparaDiagrama_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            if (txtElemento.EditValue.ToString() != Acciones[0])
                return;

            DataRow drRow = grdComparacionView.GetFocusedDataRow();

            if (drRow == null)
                return;

            if (
                (drRow["Destino"].ToString().Contains("No Existe"))
                || (drRow["Fuente"].ToString().Contains("No Existe"))
            )
                return;

            string modelo = drRow["Objeto"].ToString();

            var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);

            if (
                (modeloDestino.TipoModelo == WoTypeModel.Configuration)
                || (modeloDestino.TipoModelo == WoTypeModel.CatalogType)
                || (modeloDestino.TipoModelo == WoTypeModel.Catalog)
                || (modeloDestino.TipoModelo == WoTypeModel.TransactionContable)
                || (modeloDestino.TipoModelo == WoTypeModel.TransactionNoContable)
                || (modeloDestino.TipoModelo == WoTypeModel.Control)
                || (modeloDestino.TipoModelo == WoTypeModel.Kardex)
                || (modeloDestino.TipoModelo == WoTypeModel.DataMart)
                || (modeloDestino.TipoModelo == WoTypeModel.Parameter)
            )
            {
                var modeloComparar = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

                var mDestino = modeloDestino.Clone(proyecto);
                var mFuente = modeloComparar.Clone(proyecto);

                if (mDestino.Diagrama.ToJson() == mFuente.Diagrama.ToJson())
                {
                    XtraMessageBox.Show(
                        "Los diagramas de los modelos son iguales",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                fmCompareDiagram fmCompare = new fmCompareDiagram();

                fmCompare.ModelSetter(proyecto, proyectoFuente, modeloDestino, modeloComparar);

                if (fmCompare.ShowDialog() == DialogResult.OK)
                {
                    modeloDestino.Diagrama = fmCompare.modeloFuenteCambiado.Diagrama.Clone();
                    proyecto.SaveModel(modeloDestino);
                    buActualizar.PerformClick();
                }
            }
            else
            {
                XtraMessageBox.Show(
                    "El modelo no ocupa diagrama",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void buNormalizar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (
                (txtElemento.EditValue.ToString() != Acciones[1])
                && (txtElemento.EditValue.ToString() != Acciones[2])
                && (txtElemento.EditValue.ToString() != Acciones[3])
                && (txtElemento.EditValue.ToString() != Acciones[4])
                && (txtElemento.EditValue.ToString() != Acciones[5])
                && (txtElemento.EditValue.ToString() != Acciones[8])
                && (txtElemento.EditValue.ToString() != Acciones[9])
            )
                return;

            DataRow drRow = grdComparacionView.GetFocusedDataRow();

            if (drRow == null)
                return;

            if (
                (drRow["Destino"].ToString().Contains("No Existe"))
                || (drRow["Fuente"].ToString().Contains("No Existe"))
            )
            {
                XtraMessageBox.Show(
                    "Los modelos deben existir",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            string modelo = drRow["Objeto"].ToString();
            var modeloDestino = proyecto.ModeloCol.Modelos.Find(x => x.Id == modelo);
            var modeloFuente = proyectoFuente.ModeloCol.Modelos.Find(x => x.Id == modelo);

            if (txtElemento.EditValue.ToString() == Acciones[1])
            {
                Normaliza(modeloDestino.PreConditionsFile);
                Normaliza(modeloFuente.PreConditionsFile);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[2])
            {
                Normaliza(modeloDestino.PostConditionsFile);
                Normaliza(modeloFuente.PostConditionsFile);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[3])
            {
                Normaliza(modeloDestino.ModelCreationFile);
                Normaliza(modeloFuente.ModelCreationFile);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[4])
            {
                Normaliza(modeloDestino.ModelPolizaFile);
                Normaliza(modeloFuente.ModelPolizaFile);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[5])
            {
                Normaliza(modeloDestino.ScriptFile);
                Normaliza(modeloFuente.ScriptFile);
            }
            else if (txtElemento.EditValue.ToString() == Acciones[8])
            {
                var archivoDestino = Path.Combine(proyecto.DirProyectData_Test_UnitTest, modelo);
                var archivoFuente = Path.Combine(
                    proyectoFuente.DirProyectData_Test_UnitTest,
                    modelo
                );
                Normaliza(archivoDestino);
                Normaliza(archivoFuente);
            }
            else
                return;

            buActualizar.PerformClick();
        }

        private void Normaliza(string Archivo)
        {
            string Contenido = File.ReadAllText(Archivo);
            File.WriteAllText(Archivo, SyntaxEditorHelper.PrettyPrint(Contenido));
        }
    }
}
