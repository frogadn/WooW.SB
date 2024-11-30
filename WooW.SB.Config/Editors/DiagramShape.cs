using DevExpress.Utils.Serializing;
using DevExpress.XtraDiagram;
using System.ComponentModel;

namespace WooW.SB.Config
{
    public enum eShapeSelection
    {
        Transaccion,
        Estado,
        Transicion
    }

    public class DiagramShapeEstado : DiagramShape
    {
        #region Propiedades

        private int _Estado;

        [XtraSerializableProperty, Category("Estado"), Description(@"Estado")]
        public int Estado
        {
            get { return _Estado; }
            set { _Estado = value; }
        }

        private string _Descripcion;

        [XtraSerializableProperty, Category("Descripción"), Description(@"Descripción")]
        public string Descripcion
        {
            get { return _Descripcion; }
            set { _Descripcion = value; }
        }

        private string _ID;

        [XtraSerializableProperty, Category("ID"), Description(@"ID del objeto")]
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private eEstadoTipo _TipoEstado;

        [XtraSerializableProperty, Category("Tipo"), Description(@"Tipo de Estado")]
        public eEstadoTipo TipoEstado
        {
            get { return _TipoEstado; }
            set { _TipoEstado = value; }
        }

        #endregion Propiedades
    }

    public class DiagramShapeTransition : DiagramConnector
    {
        #region Propiedades

        private int _Estado;

        [XtraSerializableProperty, Category("Estado"), Description(@"Estado")]
        public int Estado
        {
            get { return _Estado; }
            set { _Estado = value; }
        }

        private string _Descripcion;

        [XtraSerializableProperty, Category("Descripción"), Description(@"Descripción")]
        public string Descripcion
        {
            get { return _Descripcion; }
            set { _Descripcion = value; }
        }

        private string _ID;

        [XtraSerializableProperty, Category("ID"), Description(@"ID del objeto")]
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private eTransicionTipo _TipoTransaccion;

        [XtraSerializableProperty, Category("Tipo"), Description(@"Tipo de Transacción")]
        public eTransicionTipo TipoTransaccion
        {
            get { return _TipoTransaccion; }
            set { _TipoTransaccion = value; }
        }

        private eTransicionNotificar _Notificacion_Tipo;

        [XtraSerializableProperty, Category("Tipo"), Description(@"Tipo de Notificación")]
        public eTransicionNotificar Notificacion_Tipo
        {
            get { return _Notificacion_Tipo; }
            set { _Notificacion_Tipo = value; }
        }

        #endregion Propiedades
    }
}