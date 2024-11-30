using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using WooW.Core;

namespace WooW.SB.Config
{
    public class TransicionPermiso
    {
        private Proyecto proyecto;
        private string permisoId;

        [Description("Permiso"), DisplayName("Permiso"),
            EditorAttribute(typeof(PermisoTypeEditor), typeof(UITypeEditor))]
        public string PermisoId { get => permisoId; set => permisoId = (value == null ? string.Empty : value.Trim()); }

        public TransicionPermiso()
        {
            PermisoId = string.Empty;

            // TODO: Esta tomando la instancia generica en lugar de tomar el parent
        }

        public void ProyectoSetter(Proyecto _proyecto)
        {
            proyecto = _proyecto;
        }


        public override string ToString()
        {
            if (PermisoId.IsNullOrStringEmpty())
                return string.Empty;

            Permiso permiso = null;
            if (proyecto != null)
                permiso = proyecto.Permisos.Where(e => e.Id == PermisoId).FirstOrDefault();

            if (permiso != null)
                return $"{PermisoId}-{EtiquetaCol.Get(permiso.EtiquetaId)}";
            else
                return $"{PermisoId} => (Sin Etiqueta)";
        }
    }
}