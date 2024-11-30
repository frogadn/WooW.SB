using DevExpress.XtraEditors.Repository;

namespace WooW.SB.UI
{
    public class FrogEditorItem
    {
        private RepositoryItem _oEditor;
        private string _sCategoria;
        private string _sPropiedad;

        public FrogEditorItem(RepositoryItem oParEditor, string sParCategoria, string sParPropiedad)
        {
            _oEditor = oParEditor;
            _sCategoria = sParCategoria;
            _sPropiedad = sParPropiedad;
        }

        public RepositoryItem Editor
        {
            get { return _oEditor; }
            set { _oEditor = value; }
        }

        public string Categoria
        {
            get { return _sCategoria; }
            set { _sCategoria = value; }
        }

        public string Propiedad
        {
            get { return _sPropiedad; }
            set { _sPropiedad = value; }
        }
    }
}