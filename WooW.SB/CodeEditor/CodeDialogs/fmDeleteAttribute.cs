using System;
using System.Collections.Generic;

namespace WooW.SB.CodeEditor.CodeDialogs
{
    public partial class fmDeleteAttribute : DevExpress.XtraEditors.XtraForm
    {
        public event EventHandler<string> DeleteAttributeEvt;

        public fmDeleteAttribute(List<string> customAttributesCol)
        {
            InitializeComponent();
            cmbAttrbutes.Properties.Items.AddRange(customAttributesCol);
        }

        private void btnAcept_Click(object sender, EventArgs e)
        {
            if (
                cmbAttrbutes.SelectedItem == null
                || cmbAttrbutes.SelectedItem.ToString() == string.Empty
            )
                return;
            DeleteAttributeEvt.Invoke(this, cmbAttrbutes.SelectedItem.ToString());
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
