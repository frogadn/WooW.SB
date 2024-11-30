using System.Collections.Generic;

namespace WooW.SB.GeneratorComponentBase.GeneratorInterfaces.MenuDesigner
{
    public interface IWoMenuContainer
    {
        List<string> ProcessList { get; set; }
        int Orden { get; set; }
        string Rol { get; set; }
    }
}
