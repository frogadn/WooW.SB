using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using WooW.Core;

namespace WooW.SB.Config
{
    [Serializable]
    public class ModeloRol
    {
        private Proyecto proyecto;
        private string rolId;

        [Description("Rol"), DisplayName("Rol"),
            EditorAttribute(typeof(RolTypeEditor), typeof(UITypeEditor))]
        public string RolId { get => rolId; set => rolId = (value == null ? string.Empty : value.Trim()); }

        public ModeloRol()
        {
            RolId = string.Empty;
            // TODO: Esta tomando la instancia generica en lugar de tomar el parent
        }

        public void ProyectoSetter(Proyecto _proyecto)
        {
            proyecto = _proyecto;
        }

        public override string ToString()
        {
            if (RolId.IsNullOrStringEmpty())
                return string.Empty;

            Rol rol = null;
            if (proyecto != null)
                rol = proyecto.Roles.Where(e => e.Id == RolId).FirstOrDefault();

            if (rol != null)
                return $"{RolId}-{EtiquetaCol.Get(rol.EtiquetaId)}";
            else
                return $"{RolId} => (Sin Etiqueta)";
        }
    }
}