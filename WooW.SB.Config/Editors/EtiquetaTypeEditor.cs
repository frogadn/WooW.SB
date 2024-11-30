using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WooW.SB.Config
{
    public class EtiquetaTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            //if (context is DevExpress.XtraVerticalGrid.Data.DescriptorContext)
            //{
            //    (context as DevExpress.XtraVerticalGrid.Data.DescriptorContext).PropertyDescriptor.IsReadOnly = true;
            //}

            if (context != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));

                if (editorService != null)
                {
                    fmLabelsSelector modalEditor = new fmLabelsSelector();
                    //modalEditor.EstadoCampoCol = (EstadoCampoCollection)value;

                    if (editorService.ShowDialog(modalEditor) == DialogResult.OK)
                        return modalEditor.Id;
                }
            }
            return base.EditValue(context, provider, value);
        }
    }
}