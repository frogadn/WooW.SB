using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Mvvm.Native;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList.Nodes;
using ServiceStack;
using ServiceStack.OrmLite;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Config.Class;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmPackageManager : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        Proyecto proyectoActual = null;

        public fmPackageManager()
        {
            InitializeComponent();
            mstHtmlEditor1.LicenseKey = "1U53706W222X4R1236Y7";
            proyecto = Proyecto.getInstance();
        }

        public RibbonControl CurrentRibbon
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

        public Proyecto proyecto { get; set; }

        public void Cargar()
        {
            Refrescar();
        }

        public void Refrescar()
        {
            CreateArbolPackages();
        }

        private void buRefrescar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Refrescar();
        }

        private void CreateArbolPackages()
        {
            string Repetidos = string.Empty;
            try
            {
                Repetidos = PaquetesVerifica.RecursivosYModelosRepetidos(
                    proyecto.ArchivoDeProyecto
                );
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

            if (proyecto.Paquetes == null)
                proyecto.Paquetes = new List<Paquete>();

            buAgregar.Enabled = false;
            buQuitar.Enabled = false;

            treePackages.BeginUpdate();
            treePackages.BeginUnboundLoad();
            treePackages.Nodes.Clear();

            treePackages.ClearNodes();

            TreeListNode newNode = treePackages.AppendNode(
                new object[] { Path.GetFileNameWithoutExtension(proyecto.ArchivoDeProyecto) },
                0,
                null
            );

            newNode.Tag = proyecto;
            newNode.ImageIndex = 0;
            newNode.SelectImageIndex = 0;
            newNode.StateImageIndex = 0;

            AgregaPaquetes(newNode, proyecto.ArchivoDeProyecto, proyecto);

            treePackages.FocusedNode = null;
            treePackages.FocusedNode = newNode;

            treePackages.ExpandAll();

            treePackages.EndUnboundLoad();
            treePackages.EndUpdate();

            if (!Repetidos.IsNullOrStringEmpty())
            {
                this.Parent.Refresh();

                Application.DoEvents();

                XtraMessageBox.Show(
                    "Modelos duplicados en:\r\n\r\n" + Repetidos.ToString(),
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void AgregaPaquetes(TreeListNode node, string DirBase, Proyecto prj)
        {
            if (prj.Paquetes == null)
                return;

            foreach (Paquete paquete in prj.Paquetes)
            {
                Proyecto actual = new Proyecto();

                string pathReal = Proyecto.ArchivoPaquete(DirBase, paquete.Archivo);

                if (!File.Exists(pathReal))
                {
                    TreeListNode locNode = node.Nodes.Add(
                        new object[]
                        {
                            Path.GetFileNameWithoutExtension(paquete.Archivo) + " << No existe >>"
                        }
                    );

                    locNode.Tag = null;
                    locNode.ImageIndex = 2;
                    locNode.SelectImageIndex = 2;
                    locNode.StateImageIndex = 2;
                    return;
                }

                TreeListNode newNode = node.Nodes.Add(
                    new object[] { Path.GetFileNameWithoutExtension(paquete.Archivo) }
                );

                actual.Load(pathReal);

                newNode.Tag = actual;
                newNode.ImageIndex = 0;
                newNode.SelectImageIndex = 0;
                newNode.StateImageIndex = 0;

                AgregaPaquetes(newNode, pathReal, actual);
            }
        }

        private void fmPackageManager_Load(object sender, EventArgs e) { }

        private void buAgregar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                string keyName = @"HKEY_CURRENT_USER\SOFTWARE\FROGadn\WooW\";
                string valueName = @"LastPackageDir";
                object LastDir;
                if ((LastDir = Microsoft.Win32.Registry.GetValue(keyName, valueName, null)) != null)
                {
                    if (Directory.Exists(LastDir.ToSafeString()))
                        openDialog.InitialDirectory = LastDir.ToSafeString();
                }

                openDialog.Filter = "Proyecto WooW Service Builder (*.wwsb)|*.wwsb";
                openDialog.FileName = string.Empty;
                openDialog.RestoreDirectory = true;

                if (openDialog.ShowDialog() != DialogResult.OK)
                    return;

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

                if (openDialog.FileName == proyecto.ArchivoDeProyecto)
                {
                    XtraMessageBox.Show(
                        "No puede seleccionar el mismo proyecto",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (proyecto.Paquetes.Any(x => x.Archivo == openDialog.FileName))
                {
                    XtraMessageBox.Show(
                        "El nombre del proyecto ya existe",
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

                string relativePath = Proyecto.GetRelativePath(
                    Path.GetDirectoryName(proyecto.ArchivoDeProyecto),
                    Path.GetDirectoryName(openDialog.FileName)
                );

                string Archivo = Path.Combine(relativePath, Path.GetFileName(openDialog.FileName));

                if (proyecto.Paquetes.Any(e => e.Archivo == Archivo))
                {
                    XtraMessageBox.Show(
                        "El paquete ya existe en el proyecto",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                string pathReal = Proyecto.ArchivoPaquete(proyecto.ArchivoDeProyecto, Archivo);

                if (!File.Exists(pathReal))
                {
                    XtraMessageBox.Show(
                        "Error en el algoritmo de rutas relativas",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                Paquete paquete = new Paquete() { Archivo = Archivo };
                proyecto.Paquetes.Add(paquete);
                proyecto.Save();

                try
                {
                    string ModelosRepetidos = PaquetesVerifica.RecursivosYModelosRepetidos(
                        proyecto.ArchivoDeProyecto
                    );
                    if (!ModelosRepetidos.IsNullOrStringEmpty())
                    {
                        XtraMessageBox.Show(
                            "Modelos duplicados en:\r\n\r\n" + ModelosRepetidos.ToString(),
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
                catch (Exception ex)
                {
                    string[] mensaje = ex.Message.Split(new char[] { '\r', '\n' });

                    proyecto.Paquetes.Remove(paquete);
                    proyecto.Save();

                    XtraMessageBox.Show(
                        mensaje[0],
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                Refrescar();
            }
        }

        private void treePackages_FocusedNodeChanged(
            object sender,
            DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e
        )
        {
            buAgregar.Enabled = false;
            buQuitar.Enabled = false;
            buAbrir.Enabled = false;

            if (e.Node == null)
            {
                grdModelos.DataSource = null;
                grdModelosView.Columns.Clear();
                return;
            }

            if ((e.Node.Tag != null) && (e.Node.Tag != proyecto))
            {
                buAbrir.Enabled = true;
                if ((e.Node.ParentNode != null) && (e.Node.ParentNode.Tag == proyecto))
                    buQuitar.Enabled = (wooWConfigParams.OrigenDiferenteSoloLectura ? false : true);
            }

            if (e.Node.Tag == proyecto)
            {
                buAgregar.Enabled = (wooWConfigParams.OrigenDiferenteSoloLectura ? false : true);
                buQuitar.Enabled = false;
            }

            CargarModelo(e.Node.Tag as Proyecto);
        }

        private void buAbrir_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (treePackages.FocusedNode == null)
                return;

            if (treePackages.FocusedNode.Tag == null)
                return;

            Proyecto prj = (Proyecto)treePackages.FocusedNode.Tag;

            if (prj == null)
                return;

            string archivo = prj.ArchivoDeProyecto;

            string CambiosPendientes = string.Empty;
            if ((this.Parent as fmMain).CambiosPendientesLogicaOScript(out CambiosPendientes))
            {
                XtraMessageBox.Show(
                    $"No se puede abrir el paquete {Path.GetFileNameWithoutExtension(archivo)} si tiene cambios por aplicar en {CambiosPendientes}",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (
                XtraMessageBox.Show(
                    $"Abre el paquete {Path.GetFileNameWithoutExtension(archivo)} ?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            (this.Parent as fmMain).OpenProyect(prj.ArchivoDeProyecto);
        }

        private void buQuitar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (treePackages.FocusedNode == null)
                return;

            if (treePackages.FocusedNode.Tag == null)
                return;

            Proyecto prj = (Proyecto)treePackages.FocusedNode.Tag;

            if (prj == null)
                return;

            string archivo = prj.ArchivoDeProyecto;

            if (
                XtraMessageBox.Show(
                    $"Quitar el paquete {Path.GetFileNameWithoutExtension(archivo)} ?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                ) != DialogResult.Yes
            )
                return;

            proyecto.Paquetes.RemoveAll(x =>
                x.Archivo.ToUpper().EndsWith(Path.GetFileName(archivo.ToUpper()))
            );
            proyecto.Save();

            Refrescar();
        }

        public void CargarModelo(Proyecto locProyecto)
        {
            #region Modelos

            grdModelos.DataSource = null;
            grdModelosView.Columns.Clear();

            if (locProyecto == null)
                return;

            proyectoActual = locProyecto;

            DataTable dt = new DataTable();

            dt.Columns.Add(@"Proceso", typeof(string));
            dt.Columns.Add(@"Tipo", typeof(string));
            dt.Columns.Add(@"OrdenCreacion", typeof(int));
            dt.Columns.Add(@"Repositorio", typeof(string));
            dt.Columns.Add(@"Modelo", typeof(string));
            dt.Columns.Add(@"Fecha", typeof(DateTime));
            dt.Columns.Add(@"Json", typeof(string));

            foreach (var Modelo in locProyecto.ModeloCol.Modelos.OrderBy(e => e.Id).ToList())
            {
                bool bFound = false;

                DataRow drRow = dt.NewRow();
                drRow[@"Proceso"] = Modelo.ProcesoId;
                drRow[@"Tipo"] = Modelo.TipoModelo.ToString();
                drRow[@"OrdenCreacion"] = Modelo.OrdenDeCreacion.ToInt32();
                drRow[@"Repositorio"] = Modelo.Repositorio.ToString();
                drRow[@"Modelo"] = Modelo.Id;
                drRow[@"Fecha"] = Modelo.FechaActualizacion;

                drRow[@"Json"] = Modelo.ToJson();
                dt.Rows.Add(drRow);
            }

            grdModelos.DataSource = dt;

            GridColumn col = grdModelosView.Columns[@"Proceso"];
            col.Width = 100;

            col = grdModelosView.Columns[@"Tipo"];
            col.Width = 100;

            col = grdModelosView.Columns[@"Repositorio"];
            col.Width = 100;

            col = grdModelosView.Columns[@"Modelo"];
            col.Width = 300;

            grdModelosView.SortInfo.AddRange(
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo[]
                {
                    new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                        col,
                        DevExpress.Data.ColumnSortOrder.Ascending
                    )
                }
            );

            col = grdModelosView.Columns[@"Fecha"];
            col.Caption = "Actualización";
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            col.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";

            col = grdModelosView.Columns[@"Json"];
            col.Visible = false;

            #endregion Modelos
            // Sirver para que el property grid no se pueda editar
            locProyecto.PropiedadesSoloLectura = true;
            propertyGrid.SelectedObject = locProyecto;
            mstHtmlEditor1.BodyHTML = locProyecto.HistorialDeVersiones;
        }

        private void grdModelosView_FocusedRowChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e
        )
        {
            DataRow dr = grdModelosView.GetDataRow(e.FocusedRowHandle);

            if ((dr == null) || (proyectoActual == null))
                return;

            var Modelo = proyectoActual
                .ModeloCol.Modelos.Where(e => e.Id == dr[@"Modelo"].ToString())
                .FirstOrDefault();

            if (Modelo == null)
                return;

            woModelInspector1.ConstruyePropiedadesModelo(Modelo);
        }

        private void propertyGrid_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
