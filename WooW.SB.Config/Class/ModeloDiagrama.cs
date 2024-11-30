using DevExpress.Mvvm.Native;
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
    #region " Clase ModeloDiagrama"

    [Serializable]
    public class ModeloDiagrama
    {
        private Proyecto proyecto;

        #region " Constructores"

        // Requerido para que sea Serializable
        public ModeloDiagrama()
        {
            this.Estados = new List<Estado>();
            this.Transiciones = new List<Transicion>();
            this.Roles = new List<ModeloRol>();
            this.RolesLectura = new List<ModeloRol>();

            this.Ancho = 0;
            this.Alto = 0;
        }

        public void ProyectoSetter(Proyecto _proyecto)
        {
            proyecto = _proyecto;

            foreach (var transicion in this.Transiciones)
                transicion.ProyectoSetter(proyecto);

            foreach (var rol in this.Roles)
                rol.ProyectoSetter(proyecto);

            foreach (var rol in this.RolesLectura)
                rol.ProyectoSetter(proyecto);

        }



        #endregion " Constructores"

        #region " Propiedades"

        [Browsable(false)]
        public List<Estado> Estados { get; set; }

        //[Category("Flujo"), Description("Colección de transiciones entre los estados la transacción"),
        //DisplayNameAttribute("Transiciones")]
        [Browsable(false)]
        public List<Transicion> Transiciones { get; set; }

        [Category("Diseño"), Description("Especifica el ancho de la ventana de diseño"),
        DisplayNameAttribute("Ancho")]
        public int Ancho { get; set; }

        [Category("Diseño"), Description("Especifica el alto de la ventana de diseño"),
        DisplayNameAttribute("Alto")]
        public int Alto { get; set; }

        [Category("Datos"), Description("Roles del modelo"),
        DisplayNameAttribute("Roles")]
        [EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<ModeloRol> Roles { get; set; }

        [Category("Datos"), Description("Roles de lectura del modelo"),
        DisplayNameAttribute("Roles de Lectura")]
        [EditorAttribute(typeof(FrogBaseCollectionTypedEditor), typeof(UITypeEditor))]
        public List<ModeloRol> RolesLectura { get; set; }


        #endregion " Propiedades"

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ModeloDiagrama FromJson(string Json)
        {
            if (Json.IsNullOrStringEmpty())
                return new ModeloDiagrama();
            else
                return JsonConvert.DeserializeObject<ModeloDiagrama>(Json);
        }

        public ModeloDiagrama Clone()
        {
            ModeloDiagrama md = JsonConvert.DeserializeObject<ModeloDiagrama>(JsonConvert.SerializeObject(this));
            md.ProyectoSetter(proyecto);
            return md;
        }

        public string EstadoByNum(int iEstado)
        {
            var e = Estados.Where(s => s.NumId == iEstado).FirstOrDefault();

            if (e.IsNull())
                throw new Exception($"Estado {iEstado} no existe");

            return e.Id;
        }
    }

    #endregion " Clase ModeloDiagrama"
}