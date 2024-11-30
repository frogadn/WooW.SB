using System.Drawing;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Themes.ThemeIntefaces
{
    public interface IWoComponentDesign
    {
        eAlineacion Align { get; set; }

        Color BackgorundColor { get; set; }

        eBorderType Border { get; set; }

        Color BorderColor { get; set; }
    }
}
