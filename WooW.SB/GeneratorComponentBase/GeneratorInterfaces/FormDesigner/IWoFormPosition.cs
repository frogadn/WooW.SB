namespace WooW.SB.GeneratorComponentBase.GeneratorInterfaces.FormDesigner
{
    public interface IWoFormPosition
    {
        bool BeginRow { get; set; }

        int ColSpan { get; set; }

        int RowSpan { get; set; }

        int ColumnIndex { get; set; }

        int RowIndex { get; set; }
    }
}
