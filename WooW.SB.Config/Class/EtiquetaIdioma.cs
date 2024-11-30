namespace WooW.SB.Config
{
    public class EtiquetaIdioma
    {
        private string idiomaId;
        private string texto;

        public EtiquetaIdioma()
        {
        }

        public EtiquetaIdioma(string idiomaId, string texto)
        {
            IdiomaId = idiomaId;
            Texto = texto;
        }

        public string IdiomaId { get => idiomaId; set => idiomaId = (value == null ? string.Empty : value.Trim()); }
        public string Texto { get => texto; set => texto = (value == null ? string.Empty : value.Trim()); }

        public override string ToString()
        {
            return Texto;
        }
    }
}