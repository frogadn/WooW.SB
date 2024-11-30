using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.GeneratorComponentBase.GeneratorInterfaces.GeneralComponent
{
    public interface IWoComponentBehaviour
    {
        eItemEnabled Enable { get; set; }
        string Parent { get; set; }
    }
}
