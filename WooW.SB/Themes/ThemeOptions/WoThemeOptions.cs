using System.Collections.Generic;
using WooW.SB.Designer.DesignerModels;

namespace WooW.SB.Themes.ThemeOptions
{
    public class WoThemeOptions
    {
        #region Creación del singleton

        private static readonly WoThemeOptions _themeOptions = new WoThemeOptions();

        private static readonly object lockObject = new object();

        public static WoThemeOptions GetInstance()
        {
            return _themeOptions;
        }

        #endregion Creación del singleton

        #region Colors

        public Dictionary<string, WoColor> ColorOptions = new Dictionary<string, WoColor>();

        #endregion Colors
    }
}
