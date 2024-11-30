namespace WooW.SB.Config
{
    public class ModeloColumnaIdioma
    {
        private string idiomaId;
        private string descripcion;
        private string grid;
        private string formulario;

        public string IdiomaId { get => idiomaId; set => idiomaId = (value == null ? string.Empty : value.Trim()); }
        public string Descripcion { get => descripcion; set => descripcion = (value == null ? string.Empty : value.Trim()); }
        public string Grid { get => grid; set => grid = (value == null ? string.Empty : value.Trim()); }
        public string Formulario { get => formulario; set => formulario = (value == null ? string.Empty : value.Trim()); }

        public override string ToString()
        {
            return $"{IdiomaId}-{Descripcion}";
        }
    }
}