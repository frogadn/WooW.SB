using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.GeneratorComponentBase.GeneratorInterfaces.FormDesigner
{
    public interface IWoFontDesing
    {
        eTextColor ComponentFontColor { get; set; }
        eTextSize ComponentFontSize { get; set; }
        eTextItalic ComponentFontItalic { get; set; }
        eTextWeight ComponentFontWide { get; set; }
        eTextDecoration ComponentFontDecoration { get; set; }
    }
}
