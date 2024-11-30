namespace WooW.SB.Config
{
    public class MensajeIdioma
    {
        private string idiomaId;
        private string texto;
        private string solucion;

        public string IdiomaId { get => idiomaId; set => idiomaId = (value == null ? string.Empty : value.Trim()); }
        public string Texto { get => texto; set => texto = (value == null ? string.Empty : value.Trim()); }
        public string Solucion { get => solucion; set => solucion = (value == null ? string.Empty : value.Trim()); }

        public MensajeIdioma()
        {
            IdiomaId = string.Empty;
            Texto = string.Empty;
            Solucion = string.Empty;
        }

        public MensajeIdioma(string idiomaId, string texto, string solucion)
        {
            IdiomaId = idiomaId;
            Texto = texto;
            Solucion = solucion;
        }

        public override string ToString()
        {
            return $"{IdiomaId}-{Texto}";
        }
    }
}

