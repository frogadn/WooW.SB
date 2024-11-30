using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid.Rows;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.Helpers;
using WooW.SB.Interfaces;

namespace WooW.SB.Forms
{
    public partial class fmProyectProperties : DevExpress.XtraEditors.XtraUserControl, IForm
    {
        public WooWConfigParams wooWConfigParams { get; set; }

        public Proyecto proyecto { get; set; }

        private Proyecto PropiedadesDelProyecto = null;

        public fmProyectProperties()
        {
            InitializeComponent();
            mstHtmlEditor1.LicenseKey = "1U53706W222X4R1236Y7";
        }

        public DevExpress.XtraBars.Ribbon.RibbonControl CurrentRibbon
        {
            get { return ribbonControl1; }
        }

        public bool CambiosPendientes
        {
            get { return buAceptarCambios.Enabled; }
        }

        public string Nombre
        {
            get { return ribbonPage1.Text; }
        }

        public void Refrescar() { }

        public void Cargar()
        {
            PropiedadesDelProyecto = proyecto.Clone();
            propertyGrid.SelectedObject = PropiedadesDelProyecto;

            mstHtmlEditor1.BodyHTML = PropiedadesDelProyecto.HistorialDeVersiones.ToSafeString();

            mstHtmlEditor1.ToolbarButtons.SaveAs.Visible = false;
            mstHtmlEditor1.ToolbarButtons.SaveFile.Visible = false;
            mstHtmlEditor1.ToolbarButtons.NewFile.Visible = false;
            mstHtmlEditor1.ToolbarButtons.OpenFile.Visible = false;
            mstHtmlEditor1.ToolbarButtons.SaveAsSeparator.Visible = false;

            mstHtmlEditor1.ToolbarButtons.Print.Visible = false;
            mstHtmlEditor1.ToolbarButtons.PrintPreview.Visible = false;
            mstHtmlEditor1.ToolbarButtons.PrintPreviewSeparator.Visible = false;

            LoadDefaultEditors();
        }

        private void buDesartarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            Cargar();
            buEditar.Enabled = true;
            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            propertyGrid.Enabled = false;
            mstHtmlEditor1.Enabled = false;
        }

        private void buAceptarCambios_ItemClick(
            object sender,
            DevExpress.XtraBars.ItemClickEventArgs e
        )
        {
            // Validar que los procesos sean de 3 letras mayusculas
            foreach (var Proceso in PropiedadesDelProyecto.Procesos)
            {
                if (!Proceso.Id.StartsWith(wooWConfigParams.Origen))
                {
                    XtraMessageBox.Show(
                        $"El Id para los procesos debe comenzar con el origen {wooWConfigParams.Origen}, Id={Proceso.Id}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (Proceso.Id.Length != 5)
                {
                    XtraMessageBox.Show(
                        $"El Id para los procesos debe ser de longitud 5, Id={Proceso.Id}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Validar que el Id comience con mayúscula y siga con minúsculas o números
                if (
                    !char.IsUpper(Proceso.Id[2])
                    || !Proceso.Id.Substring(3).All(c => char.IsLower(c) || char.IsDigit(c))
                )
                {
                    XtraMessageBox.Show(
                        $"El Id para los procesos debe comenzar Origen y seguir con una letra mayúscula y con letras minúsculas o números, Id={Proceso.Id}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            if (
                (
                    PropiedadesDelProyecto.Apps.Where(x => x.Id == "WebService").FirstOrDefault()
                    == null
                )
                || (
                    PropiedadesDelProyecto.Apps.Where(x => x.Id == "WebClient").FirstOrDefault()
                    == null
                )
            )
            {
                XtraMessageBox.Show(
                    "Las Apps WebService, WebClient son obligatorias",
                    "Verifique...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Para cada elemento en PropiedadesDelProyecto.Idiomas
            // validar le expresion regular para el Id que cumpla con la forma ISO para idiomas
            Regex regexIdioma = new Regex(@"[a-z]{2}-[A-Z]{2}");
            foreach (var Idioma in PropiedadesDelProyecto.Idiomas)
            {
                if (regexIdioma.Match(Idioma.Id).Value.ToString() != Idioma.Id)
                {
                    XtraMessageBox.Show(
                        $"Nombre del idioma {Idioma.Id} debe comenzar con dos letra minúsculas un guión y dos letras mayúsculas",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            Regex regex = new Regex(@"[A-Z][A-Z0-9][a-zA-Z0-9]*");

            foreach (var Permiso in PropiedadesDelProyecto.Permisos)
            {
                if (!Permiso.Id.StartsWith(wooWConfigParams.Origen))
                {
                    XtraMessageBox.Show(
                        $"El Id para los Permisos debe comenzar con el origen {wooWConfigParams.Origen}, Id={Permiso.Id}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Validar que el Id comience con mayúscula y siga con minúsculas o números
                if (!char.IsUpper(Permiso.Id[2]))
                {
                    XtraMessageBox.Show(
                        $"El Id para los Permisos debe comenzar con el origen, seguir con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números, Id={Permiso.Id}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            foreach (var Rol in PropiedadesDelProyecto.Roles)
            {
                if (!Rol.Id.StartsWith(wooWConfigParams.Origen))
                {
                    XtraMessageBox.Show(
                        $"El Id para los Roles debe comenzar con el origen {wooWConfigParams.Origen}, Id={Rol.Id}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Validar que el Id comience con mayúscula y siga con minúsculas o números
                if (!char.IsUpper(Rol.Id[2]))
                {
                    XtraMessageBox.Show(
                        $"El Id para los Roles debe comenzar con el origen, seguir con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números, Id={Rol.Id}",
                        "Verifique...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            foreach (var App in PropiedadesDelProyecto.Apps)
            {
                if ((App.Id != "WebService") && (App.Id != "WebClient"))
                {
                    if (!App.Id.StartsWith(wooWConfigParams.Origen))
                    {
                        XtraMessageBox.Show(
                            $"El Id para las Apps debe comenzar con el origen {wooWConfigParams.Origen}, Id={App.Id}",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }
                    if (!char.IsUpper(App.Id[3]))
                    {
                        XtraMessageBox.Show(
                            $"Nombre de la app {App.Id} debe comenzar con el origen, seguir con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números",
                            "Verifique...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }
                }
            }

            proyecto.HistorialDeVersiones = mstHtmlEditor1.BodyHTML;
            proyecto.Nombre = PropiedadesDelProyecto.Nombre;
            proyecto.Idiomas = PropiedadesDelProyecto.Idiomas;
            proyecto.Permisos = PropiedadesDelProyecto.Permisos;
            proyecto.Roles = PropiedadesDelProyecto.Roles;
            proyecto.Procesos = PropiedadesDelProyecto.Procesos;
            proyecto.Apps = PropiedadesDelProyecto.Apps;

            proyecto.SaveProyect();

            buEditar.Enabled = true;
            buAceptarCambios.Enabled = false;
            buDescartarCambios.Enabled = false;
            propertyGrid.Enabled = false;
            mstHtmlEditor1.Enabled = false;
        }

        private void LoadDefaultEditors()
        {
            EditorRow row = propertyGrid.Rows[0].ChildRows["rowDirectorio"] as EditorRow;
            if (row != null)
            {
                RepositoryItemButtonEdit txtDirectorio = new RepositoryItemButtonEdit();
                txtDirectorio.TextEditStyle = DevExpress
                    .XtraEditors
                    .Controls
                    .TextEditStyles
                    .DisableTextEditor;
                propertyGrid.RepositoryItems.Add(txtDirectorio);
                row.Properties.RowEdit = txtDirectorio;
                txtDirectorio.ButtonClick += txtDirectorio_ButtonClick;
            }

            row = propertyGrid.Rows[0].ChildRows["rowNombre"] as EditorRow;
            if (row != null)
            {
                RepositoryItemTextEdit txtNombre = new RepositoryItemTextEdit();
                txtNombre.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                txtNombre.Mask.EditMask = @"[A-Z][a-zA-Z0-9]*";
                txtNombre.Mask.UseMaskAsDisplayFormat = true;
                propertyGrid.RepositoryItems.Add(txtNombre);
                row.Properties.RowEdit = txtNombre;
            }
        }

        private void txtDirectorio_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e
        )
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                (sender as ButtonEdit).EditValue = folder.SelectedPath;
                //propertyGrid.Refresh();
            }
        }

        private void buEditar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            buEditar.Enabled = false;
            buAceptarCambios.Enabled = (wooWConfigParams.OrigenDiferenteSoloLectura ? false : true);
            buDescartarCambios.Enabled = true;
            propertyGrid.Enabled = true;
            mstHtmlEditor1.Enabled = true;
        }
    }
}
