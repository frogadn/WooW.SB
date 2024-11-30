namespace WooW.SB.Config.Class
{

    public class ModeloApp
    {
        private string appid;
        public string AppId { get => appid; set => appid = (value == null ? string.Empty : value.Trim()); }

        public ModeloApp()
        {
            AppId = string.Empty;
        }
    }
}

