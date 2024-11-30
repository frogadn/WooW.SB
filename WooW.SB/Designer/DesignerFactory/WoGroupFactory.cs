using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraLayout;

namespace WooW.SB.Designer.DesignerFactory
{
    public class WoGroupFactory : AWoDesignerFactoryBase
    {
        /// <summary>
        /// Rows o filas que contendrá el grupo internamente.
        /// </summary>
        public int InternalRows { get; set; } = 1;

        /// <summary>
        /// Instancia del grupo de controles que se retorna.
        /// </summary>
        public LayoutControlGroup LcgNew = null;

        /// <summary>
        /// Se ocupa de generar la instancia con las propiedades que se encuentren
        /// configuradas y de validar estas mismas.
        /// </summary>
        /// <param name="indexGroup"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public LayoutControlGroup GenerateGroupControl(
            int indexGroup,
            string text = "",
            string name = ""
        )
        {
            if (!ValidateProperties())
                return null;

            _text = (text == "") ? $"Grupo{indexGroup}" : text;
            _name = (name == "") ? $"Grupo{indexGroup}" : name;

            LcgNew = new LayoutControlGroup();

            DefineColumns();
            DefineRows();

            LcgNew.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;

            LcgNew.Text = _text;
            LcgNew.Name = _name;

            LcgNew.OptionsTableLayoutItem.ColumnSpan = ColumnSpan;
            LcgNew.OptionsTableLayoutItem.RowSpan = RowSpan;

            LcgNew.OptionsTableLayoutItem.ColumnIndex = ColumnIndex;
            LcgNew.OptionsTableLayoutItem.RowIndex = RowIndex;

            return LcgNew;
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
            if (InternalRows < 1)
            {
                Observer.SetLog(InvalidInternalRows);
                return false;
            }

            return result;
        }

        /// <summary>
        /// Asigna a los grupos 12 columnas base para que se respete la division de los
        /// contenedores web de librerías como bootstrap.
        /// </summary>
        /// <param name="lcg"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private void DefineColumns()
        {
            LcgNew.OptionsTableLayoutGroup.ColumnDefinitions.Clear();

            List<ColumnDefinition> columns = new List<ColumnDefinition>();

            for (int i = 0; i < 12; i++)
            {
                columns.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 8 });
            }

            LcgNew.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(columns.ToArray());
        }

        /// <summary>
        /// Agrega las filas en función de las que se definan por defecto.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void DefineRows()
        {
            LcgNew.OptionsTableLayoutGroup.RowDefinitions.Clear();

            List<RowDefinition> rows = new List<RowDefinition>();

            for (int i = 0; i < InternalRows; i++)
            {
                rows.Add(
                    new RowDefinition()
                    {
                        SizeType = SizeType.Percent,
                        Height = (100 / InternalRows)
                    }
                );
            }

            LcgNew.OptionsTableLayoutGroup.RowDefinitions.AddRange(rows.ToArray());
        }
    }
}
