namespace WooW.SB.GeneratorComponentBase.GeneratorInterfaces.FormDesigner
{
    public interface IWoItemType
    {
        string ClassModelType { get; set; }

        string BindingType { get; set; }

        bool Nullable { get; set; }

        string Control { get; set; }
    }
}
