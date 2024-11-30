using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using WooW.Core;
using WooW.SB.UI;

namespace WooW.SB.Config
{
    public enum eTransicionTipo
    {
        Local,
        Interna,
        Publica
    }

    public enum eTransicionNotificar
    {
        NotificarAlRol,
        NotificarAlUsuario,
        NoNotificar
    }

    [Serializable]
    public class TransicionPoint
    {
        public TransicionPoint(float fParX, float fParY)
        {
            x = fParX;
            y = fParY;
        }

        public TransicionPoint()
            : this(0f, 0f)
        {
        }

        [BrowsableAttribute(false)]
        public float x { get; set; }

        [BrowsableAttribute(false)]
        public float y { get; set; }

        [BrowsableAttribute(false)]
        public bool IsEmpty
        {
            get
            {
                if (x.Equals(0f) && y.Equals(0f))
                    return true;

                return false;
            }
        }
    }

    //[TypeConverter(typeof(FrTransicionConverter)), Serializable]
    [Serializable]
    public class Transicion
    {
        private Proyecto proyecto;
        private string id;
        private string etiquetaId;
        #region " Constructor"

        // Requerido para que sea Serializable
        public Transicion()
        {
            Id = string.Empty;

            EtiquetaId = string.Empty;

            EstadoInicial = 0;
            EstadoFinal = 0;
            Tipo = eTransicionTipo.Local;

            Roles = new List<ModeloRol>();

            IdGrafico = string.Empty;
            Abscisa = 0;
            Ordenada = 0;

            FirstControlPoint = new TransicionPoint();
            SecondControlPoint = new TransicionPoint();
            ThirdControlPoint = new TransicionPoint();
            FourControlPoint = new TransicionPoint();

            DTO = new ModeloDiagramaTransicionDTO();

            Permisos = new List<TransicionPermiso>();
        }

        public void ProyectoSetter(Proyecto _proyecto)
        {
            proyecto = _proyecto;
            foreach (var permiso in this.Permisos)
                permiso.ProyectoSetter(proyecto);
        }

        public override string ToString()
        {
            return Id;
        }


        #endregion " Constructor"

        #region " Propiedades"

        [Category("Datos"), Description("Descripción de la Transición"),
        DisplayNameAttribute("Transición")]
        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        [Category("Datos"), Description("Descripción del estado"),
        DisplayNameAttribute("Etiqueta")]
        [EditorAttribute(typeof(EtiquetaTypeEditor), typeof(UITypeEditor))]
        public string EtiquetaId { get => etiquetaId; set => etiquetaId = (value == null ? string.Empty : value.Trim()); }

        [Category("Parámetros"), Description("Estado del que inicia la Transición"),
        DisplayNameAttribute("Estado Inicial")]
        public int EstadoInicial { get; set; }

        [Category("Parámetros"), Description("Estado al que llega la Transición"),
        DisplayNameAttribute("Estado Final")]
        public int EstadoFinal { get; set; }

        [Category("Parámetros"), Description("Transición Interna, Local o Publica?"),
        DisplayNameAttribute("Interna, Local o Publica")]
        public eTransicionTipo Tipo { get; set; }

        [Category("Datos"), Description("Roles de la transición"),
        DisplayNameAttribute("Roles")]
        [EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<ModeloRol> Roles { get; set; }


        [BrowsableAttribute(false)]
        public string IdGrafico { get; set; }

        [BrowsableAttribute(false)]
        public float Abscisa { get; set; }

        [BrowsableAttribute(false)]
        public float Ordenada { get; set; }

        [BrowsableAttribute(false)]
        public TransicionPoint FirstControlPoint { get; set; } = new TransicionPoint(0f, 0f);

        [BrowsableAttribute(false)]
        public TransicionPoint SecondControlPoint { get; set; } = new TransicionPoint(0f, 0f);

        [BrowsableAttribute(false)]
        public TransicionPoint ThirdControlPoint { get; set; } = new TransicionPoint(0f, 0f);

        [BrowsableAttribute(false)]
        public TransicionPoint FourControlPoint { get; set; } = new TransicionPoint(0f, 0f);

        [BrowsableAttribute(false)]
        public int BeginItemPointIndex { get; set; } = -1;

        [BrowsableAttribute(false)]
        public int EndItemPointIndex { get; set; } = -1;

        [BrowsableAttribute(false)]
        public ModeloDiagramaTransicionDTO DTO { get; set; }

        [Category("Parámetros"), Description("Permisos que requiere la Transición"),
        DisplayNameAttribute("Permisos")]
        [EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<TransicionPermiso> Permisos { get; set; }


        [JsonIgnore]
        [BrowsableAttribute(false)]
        public bool Selected { get; set; } = false;


        #endregion " Propiedades"

        public string GetPermisos()
        {

            if ((Permisos.Count == 0) || (Permisos.Where(e => e.PermisoId == Proyecto.NOVALIDAR).FirstOrDefault() != null))
                return string.Empty;

            string sPermisos = string.Empty;
            foreach (var permiso in Permisos)
            {
                if (!sPermisos.IsNullOrStringEmpty())
                    sPermisos += ", ";
                sPermisos += $"nameof(Permiso.{permiso.PermisoId})";
            }

            if (sPermisos.IsNullOrStringEmpty())
                throw new Exception($"Transicion: {Id} faltan indicar permisos");

            return $"\r\n    [RequiresAnyPermission({sPermisos})]";
        }
    }
}