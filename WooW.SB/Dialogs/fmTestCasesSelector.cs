using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Dialogs
{
    public partial class fmTestCasesSelector : DevExpress.XtraEditors.XtraForm
    {
        private Proyecto proyecto;

        private string _PruebaUnitaria;

        private bool FirstLoad = true;

        public string PruebaUnitaria
        {
            get { return _PruebaUnitaria; }
        }

        public fmTestCasesSelector()
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
            if (treeObjetos.FocusedNode.IsNull())
                return;

            string iTem = treeObjetos.FocusedNode.Tag.ToString();

            if (File.Exists(iTem))
            {
                _PruebaUnitaria = iTem.Substring(proyecto.DirProyectData_Test_UnitTest.Length);

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                XtraMessageBox.Show(
                    "Seleccione una prueba unitaria",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void fmCasosPruebaPruebaUnitaria_Load(object sender, EventArgs e)
        {
            if (FirstLoad)
                CargarDirectorios();
            FirstLoad = false;
        }

        private bool CargarDirectorios()
        {
            treeObjetos.BeginUpdate();
            treeObjetos.BeginUnboundLoad();
            treeObjetos.Nodes.Clear();

            CreaArbolDirectorios(-1, proyecto.DirProyectData_Test_UnitTest);

            treeObjetos.EndUnboundLoad();
            treeObjetos.EndUpdate();

            return true;
        }

        private void CreaArbolDirectorios(int NodeId, string locDirectory)
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

                CreaArbolDirectorios(newNode.Id, FullSubDir);
            }

            foreach (var File in Directory.GetFiles(locDirectory, "*.cs"))
            {
                string OnlyNameFile = Path.GetFileName(File);

                TreeListNode newNode = treeObjetos.AppendNode(
                    new object[] { OnlyNameFile },
                    NodeId,
                    File
                );

                newNode.ImageIndex = 3;
                newNode.SelectImageIndex = 3;
                newNode.StateImageIndex = 3;
            }
        }
    }
}
