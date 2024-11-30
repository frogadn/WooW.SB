using DevExpress.XtraEditors.Repository;
using FastMember;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WooW.SB.UI
{
    public class FrogBaseCollectionTypedEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
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
                    FrogBaseCollectionEditor modalEditor = new FrogBaseCollectionEditor();
                    modalEditor.Collection = (IList)value;
                    modalEditor.RetrieveFields();

                    var Accessor = TypeAccessor.Create(value.GetType().GenericTypeArguments[0]);

                    foreach (var Member in Accessor.GetMembers())
                    {
                        var Attr = (WoRegExAttribute)Member.GetAttribute(typeof(WoRegExAttribute), true);
                        if (Attr != null)
                        {
                            RepositoryItemTextEdit txtMask = new RepositoryItemTextEdit();
                            txtMask.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                            txtMask.Mask.EditMask = Attr.RegEx;
                            txtMask.Mask.UseMaskAsDisplayFormat = true;

                            var Category = "categoryVarios";
                            var CatAttr = (CategoryAttribute)Member.GetAttribute(typeof(CategoryAttribute), true);
                            if (CatAttr != null)
                            {
                                Category = "category" + CatAttr.Category;
                            }

                            modalEditor.AgregaEditor(txtMask, Category, "row" + Member.Name);
                        }
                    }
                    modalEditor.RetrieveFields();

                    try
                    {
                        object oContext = context.GetType().GetProperty("Instance").GetValue(context, null);
                        if (oContext != null)
                        {
                            var oProperty = oContext.GetType().GetProperty("PropiedadesSoloLectura");

                            if (oProperty != null)
                            {
                                bool ReadOnly = (bool)oProperty.GetValue(oContext, null);
                                modalEditor.ReadOnly = ReadOnly;
                                modalEditor.ReadOnly2 = ReadOnly;
                            }
                        }
                    }
                    catch { }


                    if (editorService.ShowDialog(modalEditor) == DialogResult.OK)
                        return modalEditor.Collection;
                }
            }
            return base.EditValue(context, provider, value);
        }
    }
}