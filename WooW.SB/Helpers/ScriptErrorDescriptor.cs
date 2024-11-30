namespace WooW.SB.Class
{
    public class ScriptErrorDescriptor
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public int Line { get; set; }
        public int Column { get; set; }

        public ScriptErrorDescriptor() { }
    }
}
