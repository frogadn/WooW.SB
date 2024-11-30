using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Helpers;

namespace WooW.SB
{
    public partial class fmProyectoNuevo : DevExpress.XtraEditors.XtraForm
    {
        public string Origen { get; set; }

        private string _archivoProyecto;
        public string ArchivoProyecto
        {
            get => _archivoProyecto;
        }

        public fmProyectoNuevo()
        {
            InitializeComponent();
        }

        private void buCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buAceptar_Click(object sender, EventArgs e)
        {
            if (cmbTipoProyecto.EditValue == null)
            {
                XtraMessageBox.Show(
                    "Seleccione un tipo de proyecto",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (txtNombre.EditValue.ToSafeString().Length < 3)
                throw new Exception(
                    $"El archivo {txtNombre.EditValue.ToSafeString()} debe tener mas de 3 caracteres"
                );

            string Nombre = txtNombre.EditValue.ToSafeString();
            if (!Nombre.StartsWith(Origen))
            {
                XtraMessageBox.Show(
                    $"El proyecto {Nombre} debe comenzar con el origen '{Origen}'",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (!char.IsUpper(Nombre[2]))
            {
                XtraMessageBox.Show(
                    $"El proyecto {Nombre} la tercera letra debe se máyuscula",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            try
            {
                ProyectNew.Validate(
                    txtDirectorio.EditValue.ToSafeString(),
                    txtNombre.EditValue.ToSafeString(),
                    txtVersion.EditValue.ToSafeString(),
                    txtRevision.EditValue.ToSafeString(),
                    txtFix.EditValue.ToSafeString(),
                    Origen
                );

                // obtiene el tipo de proyecto por su atributo descripcion
                var ProyectType = Enum.GetValues(typeof(ProyectType))
                    .Cast<ProyectType>()
                    .FirstOrDefault(x =>
                        x.GetDescription() == cmbTipoProyecto.EditValue.ToSafeString()
                    );

                _archivoProyecto = ProyectNew.Do(
                    txtDirectorio.EditValue.ToSafeString(),
                    txtNombre.EditValue.ToSafeString(),
                    txtVersion.EditValue.ToSafeString(),
                    txtRevision.EditValue.ToSafeString(),
                    txtFix.EditValue.ToSafeString(),
                    Origen,
                    ProyectType,
                    optMultiTenancy.Checked,
                    optERP.Checked,
                    optIdiomasDefault.Checked,
                    optProcesosDefault.Checked,
                    optRolesDefault.Checked,
                    optPermisosDefault.Checked
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

            this.DialogResult = DialogResult.OK;
        }

        private void txtDirectorio_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            using (FolderBrowserDialog dirDialog = new FolderBrowserDialog())
            {
                if (dirDialog.ShowDialog() != DialogResult.OK)
                    return;

                if (!Directory.Exists(dirDialog.SelectedPath))
                {
                    XtraMessageBox.Show(
                        "El directorio no existe",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                if (Directory.GetFiles(dirDialog.SelectedPath).Length > 0)
                {
                    XtraMessageBox.Show(
                        "El directorio seleccionado no está vacío",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                txtDirectorio.EditValue = dirDialog.SelectedPath;
            }
        }

        private void fmProyectoNuevo_Load(object sender, EventArgs e)
        {
            cmbTipoProyecto.Properties.Items.Clear();
            cmbTipoProyecto.Properties.Items.AddRange(
                Enum.GetValues(typeof(ProyectType))
                    .Cast<ProyectType>()
                    .Select(x => x.GetDescription())
                    .ToArray()
            );
        }
    }
}
