namespace WooW.SB.GeneratorComponentBase.GeneratorInterfaces.MenuDesigner
{
    public interface IWoMenuItem
    {
        string Referencia { get; set; }
        bool Expanded { get; set; }
        int Orden { get; set; }
        bool InNewTab { get; set; }
        bool IsExternalReference { get; set; }
    }
}
