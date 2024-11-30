using System.Windows.Forms;

namespace WooW.SB.Config.ControlProperties.Class
{
    public interface ITypeControlEditor
    {
        string Properties { get; set; }

        DialogResult ShowEditor();
    }
}