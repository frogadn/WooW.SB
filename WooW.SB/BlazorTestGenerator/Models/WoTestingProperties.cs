using System;

namespace WooW.SB.BlazorTestGenerator.Models
{
    public class WoTestingProperties
    {
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public string Udn { get; set; } = string.Empty;
        public int Year { get; set; } = DateTime.Now.Year;
        public string InstanceType { get; set; } = string.Empty;
        public string BasePath { get; set; } = string.Empty;
        public double SpeedTest { get; set; } = 0.5;
    }
}
