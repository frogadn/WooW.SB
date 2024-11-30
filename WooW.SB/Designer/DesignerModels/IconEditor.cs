using System.Drawing.Design;

namespace WooW.SB.Designer.DesignerModels
{
    public class IconEditor : UITypeEditor
    {
        //public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        //{
        //    return UITypeEditorEditStyle.DropDown;
        //}

        //public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        //{
        //    IWindowsFormsEditorService service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
        //    if (service != null)
        //    {
        //        ImageComboBoxEdit editor = new ImageComboBoxEdit();

        //        foreach (var value in Enum.GetValues(typeof(eIc)))
        //        {

        //        }

        //            editor.Properties.Items.AddEnum(typeof(MyEnum));


        //        editor.SelectedItem = value;
        //        service.DropDownControl(editor);
        //        value = editor.SelectedItem;
        //    }
        //    return value;
        //}

        //private void FillComboBox(ImageComboBoxEdit ctrl, Type enumeration, object selectedValue)
        //{
        //    string description;
        //    string resx;

        //    object[] attributes = null;
        //    MemberInfo[] memInfo = null;
        //    ImageComboBoxItem item = null;

        //    ctrl.Properties.Items.Clear();
        //    foreach (var value in Enum.GetValues(enumeration))
        //    {
        //        memInfo = enumeration.GetMember(value.ToString());
        //        attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        //        description = ((DescriptionAttribute)attributes[0]).Description;
        //        attributes = memInfo[0].GetCustomAttributes(typeof(EnumItemImage), false);
        //        resx = (attributes.Length > 0 ? ((EnumItemImage)attributes[0]).Image : null);

        //        item = new ImageComboBoxItem();
        //        item.Value = value;
        //        item.Description = description;
        //        item.ImageIndex = this.GetItemImage(ctrl, resx);

        //        ctrl.Properties.Items.Add(item);

        //        if (value == selectedValue)
        //        {
        //            ctrl.SelectedItem = item;
        //        }
        //    }
        //}
    }
}
