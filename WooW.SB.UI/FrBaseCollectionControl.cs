using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid.Rows;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace WooW.SB.UI
{
    public partial class FrBaseCollectionControl : XtraUserControl, IFR_CollectionEditor
    {

        public bool ReadOnly
        {
            get
            {
                return buAgregar.Enabled;
            }
            set
            {
                buAgregar.Enabled = !value;
                buEliminar.Enabled = !value;
                buCopiar.Enabled = !value;
                buGuardar.Enabled = !value;
                buCargar.Enabled = !value;
                buUp.Enabled = !value;
                buDown.Enabled = !value;
            }
        }



        #region " Constantes"

        private const string FILTER_NAME = "Paquete Serializado WooW (*.wwpk)|*.wwpk";
        private const string DEFAULT_FILE_EXT = "wwpk";

        #endregion " Constantes"

        #region " Campos"

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private IList _Collection;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private Type _oItemType;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private FrogEditorItemCollection editorCollection;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private DXPopupMenu menuActivators;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private List<string> hiddenProperties;

        #endregion " Campos"

        #region " Propiedades"

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IList Collection
        {
            get
            {
                if (pgItem.SelectedObject != null)
                    pgItem.PostEditor();

                return _Collection;
            }
            set
            {
                _Collection = value;
                if (_Collection == null)
                {
                    lstItems.DataSource = null;

                    pgItem.SelectedObject = null;
                    pgItem.RetrieveFields();

                    return;
                }

                PropertyInfo oInfo = _Collection.GetType().GetProperty("Item", new Type[] { typeof(int) });
                _oItemType = oInfo.PropertyType;

                this.CargaDatos();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public int iCurrentItemIndex
        {
            get { return lstItems.SelectedIndex; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Func<bool> ValidateDelete { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Action<object> OnCreateNewInstance { get; set; }

        #endregion " Propiedades"

        public FrBaseCollectionControl()
        {
            InitializeComponent();
            IniciaEditores();

            editorCollection = new FrogEditorItemCollection();
            hiddenProperties = new List<string>();
            menuActivators = null;
        }

        #region " Métodos"

        public void RetrieveFields()
        {
            pgItem.RetrieveFields();
        }

        public void AsignaEditor(string sParCategoria, string sParNombre, RepositoryItem txtEditor)
        {
            if ((pgItem.Rows[sParCategoria] != null) && (pgItem.Rows[sParCategoria].ChildRows[sParNombre] != null))
            {
                EditorRow irow = pgItem.Rows[sParCategoria].ChildRows[sParNombre] as EditorRow;
                if (irow != null)
                    irow.Properties.RowEdit = txtEditor;
            }
        }

        public void AgregaEditor(RepositoryItem oParEditor, string sParCategoria, string sParPropiedad)
        {
            editorCollection.Add(new FrogEditorItem(oParEditor, sParCategoria, sParPropiedad));
            ActualizaGrid();
        }

        public void AgregaActivator(string parCaption, Func<object> parCustomActivator)
        {
            if (menuActivators == null)
            {
                menuActivators = new DXPopupMenu();

                buAgregar.ActAsDropDown = true;
                buAgregar.DropDownArrowStyle = DropDownArrowStyle.Show;
                buAgregar.DropDownControl = menuActivators;
            }

            DXMenuItem itemActivator = new DXMenuItem(parCaption);
            itemActivator.Click += delegate (object sender, EventArgs e)
                                    {
                                        AgregaElemento(parCustomActivator());
                                    };

            menuActivators.Items.Add(itemActivator);
        }

        public void OcultaHijos()
        {
            pgItem.OptionsView.ShowButtons = false;
            foreach (BaseRow oCatRow in pgItem.Rows)
            {
                if (oCatRow.HasChildren)
                {
                    foreach (BaseRow oChildRow in oCatRow.ChildRows)
                        oChildRow.OptionsRow.DblClickExpanding = false;
                }
            }
        }

        public void OcultaPropiedad(string parPropertyName)
        {
            this.hiddenProperties.Add(parPropertyName);
        }

        private void HideProperty(string parPropertyName)
        {
            BaseRow itemRow = pgItem.GetRowByFieldName(parPropertyName);
            if (itemRow == null)
                return;

            itemRow.Visible = false;
        }

        private void CheckForHiddenProperties()
        {
            foreach (string propertyName in this.hiddenProperties)
                HideProperty(propertyName);
        }

        private void IniciaEditores()
        {
            RepositoryItemCheckEdit richkBoolean = new RepositoryItemCheckEdit();
            richkBoolean.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
            pgItem.DefaultEditors.Add(typeof(Boolean), richkBoolean);

            RepositoryItemSpinEdit ritxtInteger = new RepositoryItemSpinEdit();
            ritxtInteger.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            ritxtInteger.IsFloatValue = false;
            pgItem.DefaultEditors.Add(typeof(int), ritxtInteger);

            RepositoryItemSpinEdit ritxtSmallInt = new RepositoryItemSpinEdit();
            ritxtSmallInt.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            ritxtSmallInt.IsFloatValue = false;
            pgItem.DefaultEditors.Add(typeof(Int16), ritxtSmallInt);

            RepositoryItemSpinEdit ritxtDecimal = new RepositoryItemSpinEdit();
            ritxtDecimal.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            ritxtDecimal.IsFloatValue = true;
            pgItem.DefaultEditors.Add(typeof(decimal), ritxtDecimal);
        }

        private void CargaDatos()
        {
            buEliminar.Enabled = false;
            lstItems.DataSource = _Collection;

            if (_Collection.Count > 0)
            {
                lstItems.SelectedIndex = 0;
                buEliminar.Enabled = true;
            }
            else
                this.RefrescaBotones();
        }

        private void Swap(int iParOldIndex, int iParNewIndex)
        {
            object temp = _Collection[iParNewIndex];
            _Collection[iParNewIndex] = _Collection[iParOldIndex];
            _Collection[iParOldIndex] = temp;
        }

        private int MoveUp(int iParIndex)
        {
            int iNewIndex;
            if (iParIndex == 0)
                return -1;

            iNewIndex = iParIndex - 1;
            Swap(iParIndex, iNewIndex);

            return iNewIndex;
        }

        private int MoveDown(int iParIndex)
        {
            int iNewIndex;
            if (iParIndex == _Collection.Count - 1)
                return -1;

            iNewIndex = iParIndex + 1;
            Swap(iParIndex, iNewIndex);

            return iNewIndex;
        }

        private void AgregaElemento(object parInstance)
        {
            if (parInstance != null)
            {
                _Collection.Add(parInstance);
                lstItems.SelectedValue = parInstance;
                lstItems.Refresh();

                pgItem.SelectedObject = parInstance;
                ActualizaGrid();
                pgItem.Focus();

                if (!buEliminar.Enabled)
                    buEliminar.Enabled = true;

                if (!buCopiar.Enabled)
                    buCopiar.Enabled = true;

                if (!buGuardar.Enabled)
                    buGuardar.Enabled = true;
            }
        }

        private void ActualizaGrid()
        {
            pgItem.RetrieveFields();
            if (pgItem.SelectedObject != null)
            {
                if (editorCollection.Count > 0)
                {
                    CheckForHiddenProperties();
                    foreach (BaseRow oRow in pgItem.Rows)
                    {
                        if (editorCollection.IndexOf(oRow.Name) == -1)
                            continue;

                        if (oRow.HasChildren)
                        {
                            foreach (BaseRow oChildRow in oRow.ChildRows)
                            {
                                int iPos = editorCollection.IndexOf(oRow.Name, oChildRow.Name);
                                if (iPos == -1)
                                {
                                    continue;
                                }

                                EditorRow iRow = (oChildRow as EditorRow);
                                iRow.Properties.RowEdit = editorCollection[iPos].Editor;

                                foreach (BaseRow oLitleChildRow in iRow.ChildRows)
                                {
                                    int iPosLitle = editorCollection.IndexOf(iRow.Name, oLitleChildRow.Name);
                                    if (iPosLitle == -1)
                                        continue;

                                    EditorRow mRow = (oLitleChildRow as EditorRow);
                                    mRow.Properties.RowEdit = editorCollection[iPosLitle].Editor;
                                }
                            }
                        }
                    }
                }
            }

            PropiedadesSoloLectura();
        }

        private void RefrescaBotones()
        {
            if (_Collection.Count == 0)
            {
                pgItem.SelectedObject = null;
                pgItem.RetrieveFields();

                buGuardar.Enabled = false;
                buCopiar.Enabled = false;
                buEliminar.Enabled = false;
            }
        }

        #endregion " Métodos"

        #region " Eventos Botones"

        private void buAgregar_Click(object sender, EventArgs e)
        {
            if (menuActivators != null)
                return;

            object oInstance = null;
            try
            {
                oInstance = Activator.CreateInstance(_oItemType, true);
            }
            catch
            {
                XtraMessageBox.Show("No se pudo crear un nuevo elemento");
                return;
            }

            if (OnCreateNewInstance != null && oInstance != null)
                OnCreateNewInstance(oInstance);

            AgregaElemento(oInstance);
        }

        private void buEliminar_Click(object sender, EventArgs e)
        {
            if (_Collection.Count > 0)
            {
                if (lstItems.SelectedValue == null)
                {
                    XtraMessageBox.Show("No hay ningún miembro seleccionado", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    if (this.ValidateDelete != null)
                    {
                        if (!this.ValidateDelete())
                            return;
                    }

                    _Collection.Remove(lstItems.SelectedValue);
                    lstItems.Refresh();
                }
            }

            RefrescaBotones();
        }

        private void buCopiar_Click(object sender, EventArgs e)
        {
            if (_Collection.Count > 0)
            {
                if (lstItems.SelectedValue == null)
                {
                    XtraMessageBox.Show("No hay ningún miembro seleccionado", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    object oInstance = null;

                    try
                    {
                        oInstance = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstItems.SelectedValue)); ;
                    }
                    catch (Exception Ex)
                    {
                        XtraMessageBox.Show($"Existió un error al copiar el miembro seleccionado\r\n\r\n{Ex.Message}");
                        return;
                    }

                    AgregaElemento(oInstance);
                }
            }

            RefrescaBotones();
        }

        private void buGuardar_Click(object sender, EventArgs e)
        {
            if (_Collection.Count > 0)
            {
                if (lstItems.SelectedValue == null)
                {
                    XtraMessageBox.Show("No hay ningún miembro seleccionado", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    string sJson = String.Empty;
                    try
                    {
                        sJson = JsonConvert.SerializeObject(lstItems.SelectedValue);
                    }
                    catch (Exception Ex)
                    {
                        XtraMessageBox.Show($"El miembro seleccionado no se puede serializar\r\n\r\n{Ex.Message}");
                        return;
                    }

                    SaveFileDialog SaveFileExport = new SaveFileDialog();
                    string sFileName = string.Empty;

                    SaveFileExport.FileName = string.Format("{0}.{1}", _oItemType.Name, FrBaseCollectionControl.DEFAULT_FILE_EXT);
                    SaveFileExport.DefaultExt = FrBaseCollectionControl.DEFAULT_FILE_EXT;
                    SaveFileExport.AddExtension = true;

                    SaveFileExport.Filter = FrBaseCollectionControl.FILTER_NAME;
                    SaveFileExport.FilterIndex = 1;

                    SaveFileExport.Title = "Guardar Elemento Serializado en archivo";
                    SaveFileExport.RestoreDirectory = true;

                    if (SaveFileExport.ShowDialog() == DialogResult.OK)
                    {
                        if (string.IsNullOrEmpty(SaveFileExport.FileName))
                            return;

                        if (Path.GetExtension(SaveFileExport.FileName).ToLower() != string.Format(".{0}", FrBaseCollectionControl.DEFAULT_FILE_EXT))
                        {
                            XtraMessageBox.Show(string.Format("Seleccione un archivo con extensión .{0}", FrBaseCollectionControl.DEFAULT_FILE_EXT),
                                                this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            return;
                        }

                        if (File.Exists(SaveFileExport.FileName))
                        {
                            File.Delete(SaveFileExport.FileName);
                        }

                        try
                        {
                            File.WriteAllText(SaveFileExport.FileName, sJson);
                        }
                        catch (Exception Ex)
                        {
                            XtraMessageBox.Show($"Existió un error al intentar guardar el archivo\r\n\r\n{Ex.Message}");
                        }
                    }
                }
            }
        }

        private void buCargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFilePlugin = new OpenFileDialog();
            openFilePlugin.InitialDirectory = Application.ExecutablePath;
            openFilePlugin.Filter = FrBaseCollectionControl.FILTER_NAME;
            openFilePlugin.FilterIndex = 1;
            openFilePlugin.RestoreDirectory = true;

            openFilePlugin.DefaultExt = FrBaseCollectionControl.DEFAULT_FILE_EXT;
            openFilePlugin.AddExtension = true;

            openFilePlugin.Title = "Buscar Elemento Serializado";

            if (openFilePlugin.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(openFilePlugin.FileName))
                    return;

                if (Path.GetExtension(openFilePlugin.FileName).ToLower() != string.Format(".{0}", FrBaseCollectionControl.DEFAULT_FILE_EXT))
                {
                    XtraMessageBox.Show(string.Format("Seleccione un archivo con extensión .{0}", FrBaseCollectionControl.DEFAULT_FILE_EXT),
                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }

                if (!File.Exists(openFilePlugin.FileName))
                {
                    XtraMessageBox.Show("El archivo especificado no existe", this.Text,
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string sJson = string.Empty;
                object oInstance = null;

                try
                {
                    sJson = File.ReadAllText(openFilePlugin.FileName);
                }
                catch (Exception Ex)
                {
                    XtraMessageBox.Show($"Existió un error al intentar leer el archivo\r\n\r\n{Ex.Message}");
                    return;
                }

                try
                {
                    oInstance = JsonConvert.DeserializeObject(sJson, _oItemType);
                }
                catch (Exception Ex)
                {
                    XtraMessageBox.Show($"Existió un error al deserializar el contenido del archivo\r\n\r\n{Ex.Message}");
                    return;
                }

                AgregaElemento(oInstance);
            }
        }

        private void buUp_Click(object sender, EventArgs e)
        {
            if (_Collection.Count > 0)
            {
                if (lstItems.SelectedIndex != -1)
                {
                    int iNewPos = MoveUp(lstItems.SelectedIndex);
                    if (iNewPos != -1)
                        lstItems.SelectedIndex = iNewPos;

                    lstItems.Refresh();
                }
            }
        }

        private void buDown_Click(object sender, EventArgs e)
        {
            if (_Collection.Count > 0)
            {
                if (lstItems.SelectedIndex != -1)
                {
                    int iNewPos = MoveDown(lstItems.SelectedIndex);
                    if (iNewPos != -1)
                        lstItems.SelectedIndex = iNewPos;

                    lstItems.Refresh();
                }
            }
        }

        #endregion " Eventos Botones"

        #region " Eventos Lista"

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstItems.SelectedIndex != -1)
            {
                pgItem.SelectedObject = lstItems.SelectedValue;
                ActualizaGrid();

            }
        }

        internal string InternalGetDisplayText(object value)
        {
            return this.GetDisplayText(value);
        }

        protected virtual string GetDisplayText(object value)
        {
            string str;
            if (value == null)
            {
                return string.Empty;
            }
            PropertyDescriptor defaultProperty = TypeDescriptor.GetProperties(value)["Name"];
            if ((defaultProperty != null) && (defaultProperty.PropertyType == typeof(string)))
            {
                str = (string)defaultProperty.GetValue(value);
                if ((str != null) && (str.Length > 0))
                {
                    return str;
                }
            }
            defaultProperty = TypeDescriptor.GetDefaultProperty(this._oItemType);
            if ((defaultProperty != null) && (defaultProperty.PropertyType == typeof(string)))
            {
                str = (string)defaultProperty.GetValue(value);
                if ((str != null) && (str.Length > 0))
                {
                    return str;
                }
            }
            str = TypeDescriptor.GetConverter(value).ConvertToString(value);
            if ((str != null) && (str.Length != 0))
            {
                return str;
            }
            return value.GetType().Name;
        }

        private void lstItems_DrawItem(object sender, DevExpress.XtraEditors.ListBoxDrawItemEventArgs e)
        {
            string itemText = " " + this.GetDisplayText(e.Item);
            if ((e.State & DrawItemState.Selected) != 0)
            {
                e.Cache.FillRectangle(e.Appearance.GetBackBrush(e.Cache), e.Bounds);
                //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);

                e.Cache.DrawString(itemText,
                                    e.Appearance.Font,
                                    e.Appearance.GetForeBrush(e.Cache),
                                    e.Bounds,
                                    e.Appearance.GetStringFormat());
                e.Handled = true;
                return;
            }

            e.Cache.DrawString(itemText,
                                e.Appearance.Font,
                                e.Appearance.GetForeBrush(e.Cache),
                                e.Bounds,
                                e.Appearance.GetStringFormat());
            e.Handled = true;
        }


        public void PropiedadesSoloLectura()
        {
            pgItem.RetrieveFields();

            if (pgItem.SelectedObject != null)
            {
                var selectedObject = pgItem.SelectedObject;

                var Properties = selectedObject.GetType().GetProperties();

                foreach (BaseRow oRow in pgItem.Rows)
                {
                    if (oRow.HasChildren)
                    {
                        foreach (BaseRow oChildRow in oRow.ChildRows)
                        {
                            var prop = Properties.Where(p => p.Name == oChildRow.Properties.FieldName).FirstOrDefault();
                            var attr = prop.GetCustomAttribute(typeof(EditorAttribute));
                            if (attr != null)
                                oChildRow.Properties.ReadOnly = true;

                        }
                    }
                }
            }

        }
        #endregion


        private void pgItem_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !buAgregar.Enabled;
        }
    }
}