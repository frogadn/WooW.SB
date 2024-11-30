using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs
{
    public partial class fmTestIntegralSelector : DevExpress.XtraEditors.XtraForm
    {
        private Proyecto proyecto;

        private string _PruebaIntegral;

        private bool FirstLoad = true;

        public string PruebaIntegral
        {
            get { return _PruebaIntegral; }
        }

        public fmTestIntegralSelector()
        {
            InitializeComponent();
            proyecto = Proyecto.getInstance();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (
                (treeObjetos.FocusedNode.IsNull())
                || (treeObjetos.FocusedNode.Tag.ToSafeString().IndexOf("|") == -1)
            )
                return;

            string iTem = treeObjetos.FocusedNode.Tag.ToString();

            _PruebaIntegral = iTem;

            this.DialogResult = DialogResult.OK;
        }

        private void fmTestIntegralSelector_Load(object sender, EventArgs e)
        {
            if (FirstLoad)
                CargarDirectorios();
            FirstLoad = false;
        }

        private bool CargarDirectorios()
        {
            List<string> duplicados = new List<string>();

            treeObjetos.BeginUpdate();
            treeObjetos.BeginUnboundLoad();
            treeObjetos.Nodes.Clear();

            // Este proyecto
            CreaNodoPaquete(proyecto.ArchivoDeProyecto, proyecto, duplicados);

            treeObjetos.EndUnboundLoad();
            treeObjetos.EndUpdate();

            return true;
        }

        // Crea Paquete
        private void CreaNodoPaquete(string DirBase, Proyecto locProyecto, List<string> duplicados)
        {
            if (duplicados.IndexOf(locProyecto.Nombre) == -1)
            {
                duplicados.Add(locProyecto.Nombre);

                TreeListNode newNode = treeObjetos.AppendNode(
                    new object[] { "Paquete: " + locProyecto.Nombre },
                    -1,
                    null
                );

                CreaArbolDirectorios(
                    DirBase,
                    Path.GetFileNameWithoutExtension(locProyecto.ArchivoDeProyecto),
                    newNode.Id,
                    locProyecto.DirProyectData_Test_IntegralTest,
                    locProyecto.DirProyectData_Test_IntegralTest
                );
            }

            foreach (Paquete paquete in locProyecto.Paquetes)
            {
                Proyecto proyecto = new Proyecto();

                string pathReal = Proyecto.ArchivoPaquete(DirBase, paquete.Archivo);

                if (!File.Exists(pathReal))
                {
                    XtraMessageBox.Show(
                        $"No se encontró el paquete {paquete.Archivo} en {pathReal}",
                        "Alerta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                proyecto.Load(pathReal);
                CreaNodoPaquete(proyecto.ArchivoDeProyecto, proyecto, duplicados);
            }
        }

        private void CreaArbolDirectorios(
            string DirBase,
            string ProyectName,
            int NodeId,
            string locDirectory,
            string DirIntegralTest
        )
        {
            foreach (var FullSubDir in Directory.GetDirectories(locDirectory))
            {
                var SubDir = Path.GetFileName(FullSubDir);

                TreeListNode newNode = treeObjetos.AppendNode(
                    new object[] { SubDir },
                    NodeId,
                    FullSubDir
                );

                newNode.ImageIndex = 0;
                newNode.SelectImageIndex = 1;
                newNode.StateImageIndex = 2;

                CreaArbolDirectorios(DirBase, ProyectName, newNode.Id, FullSubDir, DirIntegralTest);
            }

            foreach (var File in Directory.GetFiles(locDirectory, "*.json"))
            {
                string OnlyNameFile = Path.GetFileName(File);

                string Referencia =
                    ProyectName + " | " + File.Substring(DirIntegralTest.Length + 1);

                TreeListNode newNode = treeObjetos.AppendNode(
                    new object[] { OnlyNameFile },
                    NodeId,
                    Referencia
                );

                newNode.ImageIndex = 3;
                newNode.SelectImageIndex = 3;
                newNode.StateImageIndex = 3;
            }
        }
    }
}
