using System.Collections.Generic;
using System.Linq;
using WooW.SB.Config;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoDesignerLabels
    {
        #region Instancias singleton

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Metodo principal

        /// <summary>
        /// Agrega la lista de etiquetas requeridas para el diseñador de formularios.
        /// </summary>
        public void AddLabels()
        {
            List<(string labelId, string labelText)> addLabels = new List<(string, string)>()
            {
                ("Formulario", "Formulario"),
                ("Datos", "Datos"),
                ("FormularioDeControl", "Formulario de control"),
                ("NuevaPagina", "Nueva Página"),
                ("NuevoSubmenu", "Nuevo Submenú"),
                ("Status", "Status"),
                //Blazor
                ("Refrescar", "Refrescar"),
                ("Seleccionar", "Seleccionar"),
                ("Explorar", "Explorar"),
                (
                    "EsteCampoEsRequeridoYNoPuedeQuedarVacio",
                    "Este campo es requerido y no puede quedar vacio"
                ),
                ("Limpiar", "Limpiar"),
                ("IrAFormulario", "Ir a formulario"),
                ("Tema", "Tema"),
                ("Rol", "Rol"),
                ("Menu", "Menú"),
                ("Idioma", "Idioma"),
                ("Compartir", "Compartir"),
                ("Guardar", "Guardar"),
                ("Cancelar", "Cancelar"),
                ("Editar", "Editar"),
                ("Entradas", "Entradas"),
                ("EliminarRegistro", "Eliminar registro"),
                ("SeguroQueDeseaEliminarElRegistro", "Seguro que desea eliminar el registro"),
                ("Si", "Si"),
                ("No", "No"),
                ("SeleccioneVista", "Seleccione vista"),
                ("Recalcular", "Recalcular"),
                ("Parametros", "Parámetros"),
                ("Calcular", "Calcular"),
                ("Primero", "Primero"),
                ("Anterior", "Anterior"),
                ("Siguiente", "Siguiente"),
                ("Ultimo", "Último"),
                ("Transicion", "Transición"),
                ("Enviar", "Enviar"),
                ("EnviarTransicion", "Enviar Transición"),
                ("Cancelar", "Cancelar"),
                ("CancelarTransicion", "Cancelar transición"),
                ("Eliminar", "Eliminar"),
                ("EliminarRegistro", "Eliminar registro"),
                ("Lista", "Lista"),
                ("Inicio", "Inicio"),
                ("CopiarAlPortapapeles", "Copiar al portapapeles"),
                ("Anio", "Año"),
            };

            foreach (var item in addLabels)
            {
                AddLabel(item.labelId, item.labelText);
            }

            _project.SaveLabels();
        }

        #endregion Metodo principal


        #region Add label

        public void AddLabel(string labelId, string labelText)
        {
            if (_project.EtiquetaCol.Etiquetas.Where(x => x.Id == labelId).FirstOrDefault() == null)
            {
                _project.EtiquetaCol.Etiquetas.Add(
                    new Etiqueta()
                    {
                        Id = labelId,
                        Idiomas = new List<EtiquetaIdioma>()
                        {
                            new EtiquetaIdioma() { IdiomaId = "es-MX", Texto = labelText }
                        }
                    }
                );
            }
        }

        #endregion Add label
    }
}
