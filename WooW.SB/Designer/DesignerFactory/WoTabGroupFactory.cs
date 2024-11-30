using System.Runtime.Versioning;
using DevExpress.XtraLayout;

namespace WooW.SB.Designer.DesignerFactory
{
    public class WoTabGroupFactory : AWoDesignerFactoryBase
    {
        /// <summary>
        /// Instancia del grupo de tabs que se devolverá una vez configurado.
        /// </summary>
        public TabbedControlGroup TcgNew = null;

        /// <summary>
        /// Retorna la instancia que genera con las propiedades que se definieron.
        /// </summary>
        /// <param name="indexGroup"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public TabbedControlGroup GenerateTabGroup(
            int indexGroup,
            string text = "",
            string name = ""
        )
        {
            if (!ValidateProperties())
                return null;

            _text = (text == "") ? $"Grupo{indexGroup}" : text;
            _name = (name == "") ? $"Grupo{indexGroup}" : name;

            TcgNew = new TabbedControlGroup();

            TcgNew.Text = _text;
            TcgNew.Name = _name;

            TcgNew.OptionsTableLayoutItem.ColumnSpan = ColumnSpan;
            TcgNew.OptionsTableLayoutItem.RowSpan = RowSpan;

            TcgNew.OptionsTableLayoutItem.ColumnIndex = ColumnIndex;
            TcgNew.OptionsTableLayoutItem.RowIndex = RowIndex;

            return TcgNew;
        }

        /// <summary>
        /// Valida que todas las propiedades que se pasaron son validas
        /// </summary>
        /// <returns></returns>
        private bool ValidateProperties()
        {
            bool result = true;

            if (ColumnSpan < 1)
            {
                Observer.SetLog(InvalidColumnSpan);
                return false;
            }
            if (RowSpan < 1)
            {
                Observer.SetLog(InvalidRowSpan);
                return false;
            }
            if (ColumnIndex < 0)
            {
                Observer.SetLog(InvalidColumnIndex);
                return false;
            }
            if (RowIndex < 0)
            {
                Observer.SetLog(InvalidRowIndex);
                return false;
            }

            return result;
        }
    }
}
