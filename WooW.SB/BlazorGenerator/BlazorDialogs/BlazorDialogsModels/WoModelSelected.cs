namespace WooW.SB.BlazorGenerator.BlazorDialogs.BlazorDialogsModels
{
    public enum eGenerationType
    {
        List,
        FormList,
        Report,
        OdataReport
    }

    public class WoModelSelected
    {
        public string ModelName { get; set; }
        public eGenerationType ModelType { get; set; }
    }
}
