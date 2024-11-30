using System;
using System.Collections;

namespace WooW.SB.UI
{
    public class FrogEditorItemCollection : CollectionBase
    {
        #region " Constructor"

        public FrogEditorItemCollection()
        {
        }

        #endregion " Constructor"

        #region " Propiedades"

        public FrogEditorItem this[int index]
        {
            get { return ((FrogEditorItem)List[index]); }
            set { List[index] = value; }
        }

        public FrogEditorItem this[string sParCategoria]
        {
            get
            {
                int iIndex = this.IndexOf(sParCategoria);
                if (iIndex == -1)
                    throw new Exception("La Propiedad no existe " + sParCategoria);

                return this[iIndex];
            }
            set
            {
                int iIndex = this.IndexOf(sParCategoria);
                if (iIndex == -1)
                    throw new Exception("La Propiedad no existe " + sParCategoria);

                this[iIndex] = value;
            }
        }

        #endregion " Propiedades"

        #region " Métodos"

        public int Add(FrogEditorItem value)
        {
            return (List.Add(value));
        }

        public int IndexOf(FrogEditorItem value)
        {
            return (List.IndexOf(value));
        }

        public int IndexOf(string sParCategoria)
        {
            for (int i = 0; i < List.Count; i++)
            {
                FrogEditorItem oItem = (FrogEditorItem)List[i];
                if (oItem.Categoria.ToUpper() == sParCategoria.ToUpper())
                    return i;
            }
            return -1;
        }

        public int IndexOf(string sParCategoria, string sParPropiedad)
        {
            for (int i = 0; i < List.Count; i++)
            {
                FrogEditorItem oItem = (FrogEditorItem)List[i];
                if (oItem.Categoria.ToUpper() == sParCategoria.ToUpper() &&
                    oItem.Propiedad.ToUpper() == sParPropiedad.ToUpper())
                    return i;
            }
            return -1;
        }

        public void Insert(int index, FrogEditorItem value)
        {
            List.Insert(index, value);
        }

        public void Remove(FrogEditorItem value)
        {
            List.Remove(value);
        }

        public bool Contains(FrogEditorItem value)
        {
            return (List.Contains(value));
        }

        #endregion " Métodos"
    }
}