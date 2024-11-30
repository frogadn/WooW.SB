using System;
using System.Collections.Generic;

namespace WooW.SB.Config
{
    public class ModeloDiagramaTransicionDTOColeccion
    {
        private string modeloId;

        public ModeloDiagramaTransicionDTOColeccion()
        {
            ModeloId = String.Empty;
            Insertar = false;
            Actualizar = false;
            Borrar = false;
            Columnas = new List<string>();
            ColumnasNoEditar = new List<string>();
        }

        public string ModeloId { get => modeloId; set => modeloId = (value == null ? string.Empty : value.Trim()); }

        public bool Insertar { get; set; }
        public bool Actualizar { get; set; }
        public bool Borrar { get; set; }
        public List<string> Columnas { get; set; }
        public List<string> ColumnasNoEditar { get; set; }

        public override string ToString()
        {
            return $"{ModeloId}";
        }
    }
}