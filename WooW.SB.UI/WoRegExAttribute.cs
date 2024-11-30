using System;

namespace WooW.SB.UI
{

    public class WoRegExAttribute : Attribute
    {
        public WoRegExAttribute(string regEx)
        {
            RegEx = regEx;
        }

        public string RegEx { get; set; }
    }
}