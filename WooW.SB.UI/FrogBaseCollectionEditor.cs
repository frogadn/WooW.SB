using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WooW.SB.UI
{
    #region " Interface IFR_CollectionEditor"

    public interface IFR_CollectionEditor
    {
        IList Collection { get; set; }
        int iCurrentItemIndex { get; }
    }


    #endregion " Interface IFR_CollectionEditor"

    #region "Evento Validar"

    public delegate bool ValidateCollectionHandler(IFR_CollectionEditor parEditor, List<string> parErrors);

    #endregion "Evento Validar"

    #region " Clase FrogBaseCollectionEditor"

    public partial class FrogBaseCollectionEditor : FrogBasePropertiesEditor, IFR_CollectionEditor
    {
        public bool ReadOnly2
        {
            get
            {
                return collectionEditor.ReadOnly;
            }
            set
            {
                collectionEditor.ReadOnly = value;
            }
        }


        #region " Campos"

        public event ValidateCollectionHandler ValidateCollection;

        #endregion " Campos"

        #region " Propiedades"

        public IList Collection
        {
            get { return collectionEditor.Collection; }
            set { collectionEditor.Collection = value; }
        }

        public int iCurrentItemIndex
        {
            get { return collectionEditor.iCurrentItemIndex; }
        }

        public Action<object> OnCreateNewInstance
        {
            get { return collectionEditor.OnCreateNewInstance; }
            set { collectionEditor.OnCreateNewInstance = value; }
        }

        #endregion " Propiedades"

        #region " Constructor"

        public FrogBaseCollectionEditor()
        {
            InitializeComponent();
        }

        #endregion " Constructor"

        #region " Métodos"

        public void RetrieveFields()
        {
            this.collectionEditor.RetrieveFields();
        }

        public void AsignaEditor(string sParCategoria, string sParNombre, RepositoryItem txtEditor)
        {
            this.collectionEditor.AsignaEditor(sParCategoria, sParNombre, txtEditor);
        }

        public void AgregaEditor(RepositoryItem oParEditor, string sParCategoria, string sParPropiedad)
        {
            this.collectionEditor.AgregaEditor(oParEditor, sParCategoria, sParPropiedad);
        }

        public void OcultaHijos()
        {
            this.collectionEditor.OcultaHijos();
        }

        public void OcultaPropiedad(string parPropertyName)
        {
            this.collectionEditor.OcultaPropiedad(parPropertyName);
        }

        #endregion " Métodos"

        #region " Override Validación"

        protected override bool bValidar()
        {
            if (ValidateCollection != null)
            {
                List<string> slErrors = new List<string>();
                bool bResult = ValidateCollection(this, slErrors);

                if (slErrors.Count > 0 && !bResult)
                {
                    StringBuilder sbErrors = new StringBuilder();

                    foreach (string sItem in slErrors)
                        sbErrors.AppendLine(sItem);

                    XtraMessageBox.Show(sbErrors.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                return bResult;
            }

            return true;
        }

        #endregion " Override Validación"

        #region " Eventos Lista"

        protected virtual string GetDisplayText(object value)
        {
            return this.collectionEditor.InternalGetDisplayText(value);
        }

        #endregion " Eventos Lista"

        private void FrogBaseCollectionEditor_Activated(object sender, EventArgs e)
        {
            this.collectionEditor.PropiedadesSoloLectura();
        }
    }

    #endregion " Clase FrogBaseCollectionEditor"
}