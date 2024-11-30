using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.GeneratorComponentBase.GeneratorInterfaces.FormDesigner
{
    public interface IWoCaptionDesing
    {
        eTextColor CaptionColor { get; set; }
        eTextItalic CaptionItalic { get; set; }
        eTextWeight CaptionWide { get; set; }
        eTextDecoration CaptionDecoration { get; set; }
    }
}
