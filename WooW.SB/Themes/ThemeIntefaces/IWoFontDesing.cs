using System.Drawing;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Themes.ThemeIntefaces
{
    public interface IWoFontDesing
    {
        eTamaño FontSize { get; set; }
        Color FontColor { get; set; }
        bool Italic { get; set; }
        bool Underline { get; set; }
        bool Strikeout { get; set; }
        bool Bold { get; set; }
    }
}
