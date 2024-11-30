using System.Text;

namespace WooW.SB.Helpers.GeneratorsHelpers
{
    public class FormatClassHelper
    {
        /// <summary>
        /// Genera un string con las tabs en función del nivel de identación que recibe por parámetro.
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public static string Ident(int ident)
        {
            StringBuilder tabs = new StringBuilder();
            for (int i = 0; i < ident; i++)
            {
                tabs.Append($@"    ");
            }
            return tabs.ToString();
        }
    }
}
