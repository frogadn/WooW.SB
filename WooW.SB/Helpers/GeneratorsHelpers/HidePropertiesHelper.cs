using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.XtraEditors;

namespace WooW.SB.Helpers.GeneratorsHelpers
{
    public class HidePropertiesHelper
    {
        /// <summary>
        /// Función que nos permite ocultar las propiedades de alguna clase para no se ser mostradas ante un propertygrid
        /// Solo funciona después que las propiedades ya fueron asignadas al property grid
        /// </summary>
        /// <param name="value">Objeto de la clase</param>
        /// <param name="properties">Lista de propiedades a ocultar</param>
        public static void ModifyBrowsableAttribute(
            object props,
            List<string> properties,
            bool value = false
        )
        {
            PropertyDescriptorCollection propCollection = TypeDescriptor.GetProperties(
                props.GetType()
            );
            foreach (string prop in properties)
            {
                PropertyDescriptor descriptor = propCollection[prop];
                if (descriptor != null)
                {
                    BrowsableAttribute attrib = (BrowsableAttribute)
                        descriptor.Attributes[typeof(BrowsableAttribute)];

                    FieldInfo? isBrow = attrib
                        .GetType()
                        .GetField(
                            "<Browsable>k__BackingField",
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
                        );
                    //FieldInfo? isBrow = attrib
                    //    .GetType()
                    //    .GetField(
                    //        "browsable",
                    //        BindingFlags.NonPublic | BindingFlags.Instance
                    //    );
                    //Condition to Show or Hide set here:
                    //"browsable"
                    if (isBrow != null)
                        isBrow.SetValue(attrib, value);
                    else
                    {
                        XtraMessageBox.Show("Alert");
                    }
                }
            }
        }
    }
}
