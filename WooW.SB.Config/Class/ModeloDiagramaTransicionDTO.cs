using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class ModeloDiagramaTransicionDTO
    {
        public ModeloDiagramaTransicionDTO()
        {
            Columnas = new List<string>();
            ColumnasNoEditar = new List<string>();
            Colleccion = new List<ModeloDiagramaTransicionDTOColeccion>();
        }

        public List<string> Columnas { get; set; }
        public List<string> ColumnasNoEditar { get; set; }
        public List<ModeloDiagramaTransicionDTOColeccion> Colleccion = new List<ModeloDiagramaTransicionDTOColeccion>();

        //public override string ToString()
        //{
        //    return $"{ModeloId}";
        //}
    }
}