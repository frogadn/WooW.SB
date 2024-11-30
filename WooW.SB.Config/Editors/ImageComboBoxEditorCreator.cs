using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WooW.SB.Config.Editors
{

    public class ImageComboBoxEditorCreator : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));

                if (editorService != null)
                {
                    fmIconoSelector modalEditor = new fmIconoSelector();
                    //modalEditor.EstadoCampoCol = (EstadoCampoCollection)value;

                    if (editorService.ShowDialog(modalEditor) == DialogResult.OK)
                        return modalEditor.Id;
                }
            }
            return base.EditValue(context, provider, value);
        }
    }

}
