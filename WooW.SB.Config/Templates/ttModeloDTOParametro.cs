﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace WooW.SB.Config.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using WooW.Core;
    using WooW.SB.Config;
    using WooW.SB.Config.Enum;
    using WooW.SB.Config.Helpers;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class ttModeloDTOParametro : ttModeloDTOParametroBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"// ------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta. WooW.SB ttModeloParametro
//     Versión del runtime: 1.0.0.0
//  
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated> 
// ------------------------------------------------------------------------------//
using WooW.Core;
using WooW.Model;
using ServiceStack;
using ServiceStack.DataAnnotations;
using System.Collections.Generic;
using WooW.Resources.Labels;
using WooW.Resources.ModelData;
using WooW.Resources.ModelLabel;
using WooW.Resources.ModelComment;
using Newtonsoft.Json;

namespace WooW.DTO
{

#if DEBUG
#if SERVER");
            
            #line 34 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.GetRolesList()));
            
            #line default
            #line hidden
            this.Write("\r\n#endif\r\n    [ValidateIsAuthenticated]\r\n    [Tag(\"");
            
            #line 37 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("s\")]\r\n    [Route(\"/api/");
            
            #line 38 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("AutoQuery\", \"GET\")]\r\n    [JsonObject(MemberSerialization.OptOut)]\r\n    public cla" +
                    "ss ");
            
            #line 40 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("AutoQuery : QueryDb<");
            
            #line 40 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write(">, IWoInstanciaUdn\r\n    {\r\n    }\r\n#endif\r\n\r\n\r\n    #if SERVER");
            
            #line 46 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.GetRolesList()));
            
            #line default
            #line hidden
            this.Write("\r\n    [AutoApply(Behavior.AuditQuery)]\r\n    #endif\r\n    [ValidateIsAuthenticated]" +
                    "\r\n    [Tag(\"");
            
            #line 50 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("s\")]\r\n    [Route(\"/api/");
            
            #line 51 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("Get\", \"GET\")]\r\n    [JsonObject(MemberSerialization.OptOut)]\r\n    public class ");
            
            #line 53 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("Get : IWoInstanciaUdn\r\n    {\r\n    }\r\n\r\n    #if SERVER");
            
            #line 57 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.GetRolesList()));
            
            #line default
            #line hidden
            this.Write("\r\n    [AutoApply(Behavior.AuditQuery)]\r\n    #endif\r\n    [ValidateIsAuthenticated]" +
                    "\r\n    [Tag(\"");
            
            #line 61 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("s\")]\r\n    [Route(\"/api/");
            
            #line 62 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("Single\", \"GET\")]\r\n    [JsonObject(MemberSerialization.OptOut)]\r\n    public class " +
                    "");
            
            #line 64 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("Single : IWoInstanciaUdn\r\n    {\r\n    }\r\n\r\n");
            
            #line 68 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
  foreach(Transicion tra in modelo.Diagrama.Transiciones.Where( e => e.EstadoInicial == 0 ) )
    {
            
            #line default
            #line hidden
            this.Write("  \r\n    #if SERVER\r\n    [AutoApply(Behavior.AuditCreate)]");
            
            #line 71 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.GetRolesTra(tra)));
            
            #line default
            #line hidden
            
            #line 71 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.GetPermisos()));
            
            #line default
            #line hidden
            this.Write("\r\n    #endif\r\n    [ValidateIsAuthenticated]\r\n    [Tag(\"");
            
            #line 74 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("s\")]\r\n    [Route(\"/api/");
            
            #line 75 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            
            #line 75 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.Id));
            
            #line default
            #line hidden
            this.Write("\", \"POST\")]\r\n    [AutoPopulate(nameof(");
            
            #line 76 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write(".WoState), Value = e");
            
            #line 76 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("_WoState.");
            
            #line 76 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Diagrama.EstadoByNum(tra.EstadoFinal)));
            
            #line default
            #line hidden
            this.Write(")]\r\n    [WoModelBase(typeof(");
            
            #line 77 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("))]\r\n    [JsonObject(MemberSerialization.OptOut)]\r\n    public class ");
            
            #line 79 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            
            #line 79 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.Id));
            
            #line default
            #line hidden
            this.Write(" : IWoInstanciaUdn, IReturn<");
            
            #line 79 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write(">\r\n    {\r\n");
            
            #line 81 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
      foreach(ModeloColumna col in modelo.Columnas.OrderBy( e => e.Orden ) ) {
            var found = tra.DTO.Columnas.Where( e => e == col.Id ).FirstOrDefault();
            if(found == null)
                continue;
        
            
            #line default
            #line hidden
            
            #line 86 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.ToAttributesDTO()));
            
            #line default
            #line hidden
            this.Write("\r\n        ");
            
            #line 87 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.ToPropertyDTO(modelo, false)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 88 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
      }
        foreach(var coll in tra.DTO.Colleccion )
        {
            if((!coll.Insertar) && (!coll.Actualizar) && (!coll.Borrar))
                continue;

            var modeloSlave = proyecto.ModeloCol.Modelos.Where( t => t.Id == coll.ModeloId ).FirstOrDefault();
            if(modeloSlave == null)
                continue;
    
            
            #line default
            #line hidden
            this.Write("  \r\n        [Reference]\r\n");
            
            #line 99 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
          if(coll.Insertar) {
            
            #line default
            #line hidden
            this.Write("        [WoSlaveAllowInsert]\r\n");
            
            #line 101 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
           }
            
            #line default
            #line hidden
            
            #line 102 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
          if(coll.Actualizar) {
            
            #line default
            #line hidden
            this.Write("        [WoSlaveAllowUpdate]\r\n");
            
            #line 104 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
           }
            
            #line default
            #line hidden
            
            #line 105 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
          if(coll.Borrar) {
            
            #line default
            #line hidden
            this.Write("        [WoSlaveAllowDelete]\r\n");
            
            #line 107 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
           }
            
            #line default
            #line hidden
            this.Write("        [WoModelBase(typeof(");
            
            #line 108 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("))]\r\n        public virtual IList<");
            
            #line 109 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modeloSlave.Id));
            
            #line default
            #line hidden
            
            #line 109 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.Id));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 109 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modeloSlave.Id));
            
            #line default
            #line hidden
            this.Write("Col { get; set; }\r\n");
            
            #line 110 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
      }

            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n");
            
            #line 114 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
  }
            
            #line default
            #line hidden
            this.Write("  \r\n\r\n");
            
            #line 116 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
  foreach(Transicion tra in modelo.Diagrama.Transiciones.Where( e => e.EstadoInicial != 0 ) )
    {
            
            #line default
            #line hidden
            this.Write("  \r\n    #if SERVER\r\n    [AutoApply(Behavior.AuditModify)]");
            
            #line 119 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.GetRolesTra(tra)));
            
            #line default
            #line hidden
            
            #line 119 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.GetPermisos()));
            
            #line default
            #line hidden
            this.Write("\r\n    #endif\r\n    [ValidateIsAuthenticated]\r\n    [Tag(\"");
            
            #line 122 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("s\")]\r\n    [Route(\"/api/");
            
            #line 123 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            
            #line 123 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.Id));
            
            #line default
            #line hidden
            this.Write("\", \"PUT PATCH\")]\r\n    [AutoPopulate(nameof(");
            
            #line 124 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write(".WoState), Value = e");
            
            #line 124 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("_WoState.");
            
            #line 124 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Diagrama.EstadoByNum(tra.EstadoFinal)));
            
            #line default
            #line hidden
            this.Write(")]\r\n    [WoModelBase(typeof(");
            
            #line 125 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("))]\r\n    [JsonObject(MemberSerialization.OptOut)]\r\n    public class ");
            
            #line 127 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            
            #line 127 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.Id));
            
            #line default
            #line hidden
            this.Write(" : IWoInstanciaUdn, IReturn<");
            
            #line 127 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write(">\r\n    {\r\n");
            
            #line 129 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
      foreach(ModeloColumna col in modelo.Columnas.OrderBy( e => e.Orden ) ) {
            var found = tra.DTO.Columnas.Where( e => e == col.Id ).FirstOrDefault();
            if(found == null)
                continue;

            
            #line default
            #line hidden
            
            #line 134 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.ToAttributesDTO()));
            
            #line default
            #line hidden
            this.Write("\r\n        ");
            
            #line 135 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.ToPropertyDTO(modelo, true)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 136 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
      }

        foreach(var coll in tra.DTO.Colleccion )
        {
            if((!coll.Insertar) && (!coll.Actualizar) && (!coll.Borrar))
                continue;

            var modeloSlave = proyecto.ModeloCol.Modelos.Where( t => t.Id == coll.ModeloId ).FirstOrDefault();
            if(modeloSlave == null)
                continue;
    
            
            #line default
            #line hidden
            this.Write("  \r\n        [Reference]\r\n        [WoReferenceBase(typeof(");
            
            #line 148 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modelo.Id));
            
            #line default
            #line hidden
            this.Write("))]\r\n");
            
            #line 149 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
          if(coll.Insertar) {
            
            #line default
            #line hidden
            this.Write("        [WoSlaveAllowInsert]\r\n");
            
            #line 151 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
           }
            
            #line default
            #line hidden
            
            #line 152 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
          if(coll.Actualizar) {
            
            #line default
            #line hidden
            this.Write("        [WoSlaveAllowUpdate]\r\n");
            
            #line 154 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
           }
            
            #line default
            #line hidden
            
            #line 155 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
          if(coll.Borrar) {
            
            #line default
            #line hidden
            this.Write("        [WoSlaveAllowDelete]\r\n");
            
            #line 157 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
           }
            
            #line default
            #line hidden
            this.Write("        public virtual IList<");
            
            #line 158 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modeloSlave.Id));
            
            #line default
            #line hidden
            
            #line 158 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.Id));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 158 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modeloSlave.Id));
            
            #line default
            #line hidden
            this.Write("Col { get; set; }\r\n");
            
            #line 159 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
      }

            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n\r\n");
            
            #line 164 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
  }
            
            #line default
            #line hidden
            this.Write("  \r\n\r\n");
            
            #line 166 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
  foreach(Transicion tra in modelo.Diagrama.Transiciones )
    {
        foreach(var coll in tra.DTO.Colleccion )
        {
            if((!coll.Insertar) && (!coll.Actualizar) && (!coll.Borrar))
                continue;

            var modeloSlave = proyecto.ModeloCol.Modelos.Where( t => t.Id == coll.ModeloId ).FirstOrDefault();
            if(modeloSlave == null)
                continue;
    
            
            #line default
            #line hidden
            this.Write("  \r\n    [WoModelBase(typeof(");
            
            #line 177 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modeloSlave.Id));
            
            #line default
            #line hidden
            this.Write("))]\r\n    [JsonObject(MemberSerialization.OptOut)]\r\n    public class ");
            
            #line 179 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modeloSlave.Id));
            
            #line default
            #line hidden
            
            #line 179 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tra.Id));
            
            #line default
            #line hidden
            this.Write(" : IWoTypeOfSlaveDTO\r\n    {\r\n");
            
            #line 181 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
          foreach(ModeloColumna col in modeloSlave.Columnas.OrderBy( e => e.Orden ) ) {
                var found = coll.Columnas.Where( e => e == col.Id ).FirstOrDefault();
                if(found == null)
                    continue;

            
            #line default
            #line hidden
            
            #line 186 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.ToAttributesDTO()));
            
            #line default
            #line hidden
            this.Write("\r\n        ");
            
            #line 187 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(col.ToPropertyDTO(modelo, true)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 188 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
      }

        // Todo Pendiente a revisar como genera el esquema de la db para ver si se agrega la referencia
        //
        // Busca una tabla maestra que ocupe la esclava
        // Modelo modeloMaster = null;
        // foreach (var m in proyecto.ModeloCol.Modelos)
        // {
        //     var modelocoleccion = m.Colecciones
        //         .Where(c => c.ModeloId == modelo.Id)
        //         .FirstOrDefault();
        //     if (modelocoleccion != null)
        //     {
        //         modeloMaster = proyecto.ModeloCol.Modelos
        //             .Where(mm => mm.Id == m.Id)
        //             .FirstOrDefault();
        //         break;
        //     }
        // }
        // if (modeloMaster == null)
        //     throw new Exception("esclava no se ocupa por una maestra");

            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n");
            
            #line 212 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"
  }
}
            
            #line default
            #line hidden
            this.Write("  \r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 215 "C:\Frog\Blazor\WooW.SB.Config\Templates\ttModeloDTOParametro.tt"

public Modelo modelo { get; set; }  
public Proyecto proyecto { get; set; }  

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class ttModeloDTOParametroBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        public System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}