using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WooW.Core;

namespace WooW.SB.Config.Helpers
{
    public static class ProyectoConPaquetes
    {
        private static string lastArchivoDeProyecto = string.Empty;
        private static Proyecto lastProyect = null;

        public static void Clear()
        {
            lastArchivoDeProyecto = string.Empty;
            lastProyect = null;
        }

        public static Proyecto Get(string ArchivoDeProyecto)
        {
            if ((lastArchivoDeProyecto == ArchivoDeProyecto) &&
               (lastProyect != null))
                return lastProyect;

            lastProyect = null;

            Proyecto principal = new Proyecto();
            principal.Load(ArchivoDeProyecto);

            principal.DirectorioDeLosPaquetes = new List<Tuple<string, string>>();
            principal.DirectorioDeLosPaquetes.Add(Tuple.Create(principal.GetNameExtension(), principal.DirProyectData));

            // Traspasa paquete
            foreach (var paquete in principal.Paquetes)
                TraspasaPaquete(principal, ArchivoDeProyecto, paquete.Archivo);

            lastProyect = principal;
            lastArchivoDeProyecto = ArchivoDeProyecto;

            return lastProyect;
        }

        public static void TraspasaPaquete(Proyecto principal, string DirBase, string Paquete)
        {
            string pathReal = Proyecto.ArchivoPaquete(DirBase, Paquete);

            Proyecto local = new Proyecto();
            local.Load(pathReal);

            var itemPaqueteDirectorio = Tuple.Create(local.GetNameExtension(), local.DirProyectData);
            if (principal.DirectorioDeLosPaquetes.IndexOf(itemPaqueteDirectorio) == -1)
                principal.DirectorioDeLosPaquetes.Add(itemPaqueteDirectorio);

            // Roles
            foreach (var rol in local.Roles)
                if (!principal.Roles.Any(x => x.Id == rol.Id))
                    principal.Roles.Add(rol.Clone());

            // Permisos
            foreach (var permiso in local.Permisos)
                if (!principal.Permisos.Any(x => x.Id == permiso.Id))
                    principal.Permisos.Add(permiso.Clone());

            // Permisos
            foreach (var permiso in local.Permisos)
                if (!principal.Permisos.Any(x => x.Id == permiso.Id))
                    principal.Permisos.Add(permiso.Clone());

            // Etiquetas
            foreach (var etiqueta in local.EtiquetaCol.Etiquetas)
                if (!principal.EtiquetaCol.Etiquetas.Any(x => x.Id == etiqueta.Id))
                    principal.EtiquetaCol.Etiquetas.Add(etiqueta.Clone());

            // Mensajes
            foreach (var mensaje in local.MensajeCol.Mensajes)
                if (!principal.MensajeCol.Mensajes.Any(x => x.Id == mensaje.Id))
                    principal.MensajeCol.Mensajes.Add(mensaje.Clone());

            // Modelos
            foreach (var modelo in local.ModeloCol.Modelos)
                if (!principal.ModeloCol.Modelos.Any(x => x.Id == modelo.Id))
                {
                    var cloneModelo = modelo.Clone(principal);
                    cloneModelo.EsPaqueteExterno = true;
                    cloneModelo.DirectorioPaquete = Path.GetDirectoryName(pathReal);
                    principal.ModeloCol.Modelos.Add(cloneModelo);
                }
                else // Adiciona los campos
                {
                    var locModelo = principal.ModeloCol.Modelos.FirstOrDefault(x => x.Id == modelo.Id);
                    if ((locModelo.SubTipoModelo != WoSubTypeModel.Extension) &&
                        (locModelo.SubTipoModelo != WoSubTypeModel.Override))
                    {
                        foreach (var columna in modelo.Columnas)
                            if (!locModelo.Columnas.Any(x => x.Id == columna.Id))
                                locModelo.Columnas.Add(columna.Clone(principal));

                        locModelo.Diagrama = DiagramaJoin(locModelo, modelo);
                    }
                    else
                    {
                        var cloneModelo = modelo.Clone(principal);
                        cloneModelo.EsPaqueteExterno = true;
                        cloneModelo.DirectorioPaquete = Path.GetDirectoryName(pathReal);

                        foreach (var columna in locModelo.Columnas)
                        {
                            if (cloneModelo.Columnas.Any(x => x.Id == columna.Id))
                                throw new Exception($"La columna {columna.Id} del modelo {modelo.Id} se repite en el paquete {Paquete}");
                            cloneModelo.Columnas.Add(columna.Clone(principal));
                        }
                        cloneModelo.Diagrama = DiagramaJoin(modelo, locModelo);
                        principal.ModeloCol.Modelos.Remove(locModelo);
                        principal.ModeloCol.Modelos.Add(cloneModelo);
                    }


                }

            // Traspasa paquetes
            foreach (var paquete in local.Paquetes)
                TraspasaPaquete(principal, pathReal, paquete.Archivo);
        }

        public static List<string> GetFiles(Proyecto principal, string Dir, string Pattern)
        {
            string PathProyecto = Path.GetDirectoryName(principal.ArchivoDeProyecto);
            string PathBusqueda = Dir.Substring(PathProyecto.Length + 1);

            List<string> files = new List<string>();

            foreach (string File in Directory.GetFiles(Dir, Pattern))
                files.Add(File);

            foreach (var paquete in principal.Paquetes)
                if (!paquete.Archivo.IsNullOrStringEmpty())
                    GetFilesPaquete(principal, files, PathBusqueda, Pattern, paquete.Archivo);

            return files;
        }

        public static void GetFilesPaquete(
            Proyecto principal,
            List<string> files,
            string DirBase,
            string Pattern,
            string Paquete
        )
        {
            string path = Path.GetDirectoryName(
                Proyecto.ArchivoPaquete(principal.ArchivoDeProyecto, Paquete)
            );
            string pathFinal = Path.Combine(path, DirBase);

            string pathReal = Proyecto.ArchivoPaquete(principal.ArchivoDeProyecto, Paquete);
            Proyecto local = new Proyecto();
            local.Load(pathReal);

            foreach (string File in Directory.GetFiles(pathFinal, Pattern))
                if (files.IndexOf(File) == -1)
                    files.Add(File);

            foreach (var paquete in local.Paquetes)
                GetFilesPaquete(local, files, DirBase, Pattern, paquete.Archivo);
        }

        public static Modelo GetModeloBase(Proyecto principal, string Id)
        {
            Modelo modelo = principal.ModeloCol.Modelos.FirstOrDefault(x => x.Id == Id);
            if (modelo == null)
                throw new Exception($"No se encuentra el modelo {Id}");

            if ((modelo.SubTipoModelo != WoSubTypeModel.Extension) &&
                    (modelo.SubTipoModelo != WoSubTypeModel.Override))
                throw new Exception($"El modelo {Id} debe ser de tipo extension");

            foreach (var paquete in principal.Paquetes)
            {
                var locModelo = GetModeloPaquete(principal, Id, paquete.Archivo);
                if (locModelo != null)
                    return locModelo;
            }

            return null;
        }


        private static Modelo GetModeloPaquete(Proyecto principal, string Id, string Paquete)
        {
            string pathReal = Proyecto.ArchivoPaquete(principal.ArchivoDeProyecto, Paquete);
            Proyecto local = new Proyecto();
            local.Load(pathReal);

            Modelo modelo = local.ModeloCol.Modelos.FirstOrDefault(x => x.Id == Id);
            if (modelo != null)
                if ((modelo.SubTipoModelo != WoSubTypeModel.Extension) &&
                    (modelo.SubTipoModelo != WoSubTypeModel.Override))
                    return modelo;

            foreach (var paquete in local.Paquetes)
            {
                var locModelo = GetModeloPaquete(local, Id, paquete.Archivo);
                if (locModelo != null)
                    return locModelo;
            }

            return null;
        }


        public static ModeloDiagrama DiagramaExtend(Modelo modeloBase, Modelo modelExtension)
        {
            var Diagrama = modeloBase.Diagrama.Clone();

            if (modelExtension == null)
                return Diagrama;

            Diagrama.Roles.Clear();
            Diagrama.Roles.AddRange(modelExtension.Diagrama.Roles);
            Diagrama.RolesLectura.Clear();
            Diagrama.RolesLectura.AddRange(modelExtension.Diagrama.RolesLectura);

            // Traspasa transiciones las mandatorias son las que se encuentran en el modelo base
            foreach (var transicion in modelExtension.Diagrama.Transiciones)
            {
                var locTransicion = Diagrama.Transiciones.FirstOrDefault(x => x.Id == transicion.Id);
                if (locTransicion == null)
                    continue; // La transicion no se encuentra en el modelo base

                locTransicion.Roles.Clear();
                locTransicion.Roles.AddRange(transicion.Roles);
                locTransicion.Permisos.Clear();
                locTransicion.Permisos.AddRange(transicion.Permisos);

                locTransicion.DTO.Columnas.Clear();
                locTransicion.DTO.Columnas.AddRange(transicion.DTO.Columnas);
                locTransicion.DTO.ColumnasNoEditar.Clear();
                locTransicion.DTO.ColumnasNoEditar.AddRange(transicion.DTO.ColumnasNoEditar);

                // Concatena los DTO Esclavas
                foreach (var esclava in transicion.DTO.Colleccion)
                {
                    var locEsclavaDTO = locTransicion.DTO.Colleccion.FirstOrDefault(x => x.ModeloId == esclava.ModeloId);

                    if (locEsclavaDTO == null)  // Crea una
                    {
                        locEsclavaDTO = new ModeloDiagramaTransicionDTOColeccion()
                        {
                            Insertar = esclava.Insertar,
                            Borrar = esclava.Borrar,
                            Actualizar = esclava.Actualizar,
                            ModeloId = esclava.ModeloId,
                            Columnas = new List<string>(),
                            ColumnasNoEditar = new List<string>(),
                        };
                        foreach (var col in esclava.Columnas)
                            locEsclavaDTO.Columnas.Add(col);
                        foreach (var col in esclava.ColumnasNoEditar)
                            locEsclavaDTO.ColumnasNoEditar.Add(col);
                        locTransicion.DTO.Colleccion.Add(locEsclavaDTO);
                    }
                    else
                    {
                        // Se quedan los de la maestra
                        //locEsclavaDTO.Insertar = esclava.Insertar;
                        //locEsclavaDTO.Borrar = esclava.Borrar;
                        //locEsclavaDTO.Actualizar = esclava.Actualizar;

                        // Traspasa
                        locEsclavaDTO.Columnas.Clear();
                        locEsclavaDTO.Columnas.AddRange(esclava.Columnas);
                        locEsclavaDTO.ColumnasNoEditar.Clear();
                        locEsclavaDTO.ColumnasNoEditar.AddRange(esclava.ColumnasNoEditar);
                    }
                }
            }

            return Diagrama;
        }

        public static ModeloDiagrama DiagramaJoin(Modelo modeloBase, Modelo modelExtension)
        {
            var Diagrama = modeloBase.Diagrama.Clone();

            if (modelExtension == null)
                return Diagrama;

            // Junta los roles
            foreach (var rol in modelExtension.Diagrama.Roles)
                if (!Diagrama.Roles.Any(x => x.RolId == rol.RolId))
                {
                    var locRol = new ModeloRol()
                    {
                        RolId = rol.RolId,
                    };
                    locRol.ProyectoSetter(modeloBase.Proyecto);
                    Diagrama.Roles.Add(locRol);
                }

            // Junta los roles lectura
            foreach (var rol in modelExtension.Diagrama.RolesLectura)
                if (!Diagrama.RolesLectura.Any(x => x.RolId == rol.RolId))
                {
                    var locRol = new ModeloRol()
                    {
                        RolId = rol.RolId,
                    };
                    locRol.ProyectoSetter(modeloBase.Proyecto);
                    Diagrama.RolesLectura.Add(locRol);
                }

            // Traspasa transiciones 
            foreach (var transicion in modelExtension.Diagrama.Transiciones)
            {
                var locTransicion = Diagrama.Transiciones.FirstOrDefault(x => x.Id == transicion.Id);
                if (locTransicion == null)
                    continue; // Omite la transicion

                // Junta roles de la transicion
                foreach (var rol in transicion.Roles)
                    if (!locTransicion.Roles.Any(x => x.RolId == rol.RolId))
                    {
                        var locRol = new ModeloRol()
                        {
                            RolId = rol.RolId,
                        };
                        locRol.ProyectoSetter(modeloBase.Proyecto);
                        locTransicion.Roles.Add(locRol);
                    }

                // Junta los permisos de la transicion
                foreach (var permiso in transicion.Permisos)
                    if (!locTransicion.Permisos.Any(x => x.PermisoId == permiso.PermisoId))
                    {
                        var locPermiso = new TransicionPermiso()
                        {
                            PermisoId = permiso.PermisoId,
                        };
                        locPermiso.ProyectoSetter(modeloBase.Proyecto);
                        locTransicion.Permisos.Add(locPermiso);
                    }


                // Junta las DTO columnas
                foreach (var col in transicion.DTO.Columnas)
                    if (!locTransicion.DTO.Columnas.Any(x => x == col))
                        locTransicion.DTO.Columnas.Add(col);

                // Junta los DTO columnas no editar
                foreach (var col in transicion.DTO.ColumnasNoEditar)
                    if (!locTransicion.DTO.ColumnasNoEditar.Any(x => x == col))
                        locTransicion.DTO.ColumnasNoEditar.Add(col);

                // Concatena los DTO Esclavas
                foreach (var esclava in transicion.DTO.Colleccion)
                {
                    var locEsclavaDTO = locTransicion.DTO.Colleccion.FirstOrDefault(x => x.ModeloId == esclava.ModeloId);

                    if (locEsclavaDTO == null)  // Crea una
                    {
                        locEsclavaDTO = new ModeloDiagramaTransicionDTOColeccion()
                        {
                            Insertar = esclava.Insertar,
                            Borrar = esclava.Borrar,
                            Actualizar = esclava.Actualizar,
                            ModeloId = esclava.ModeloId,
                            Columnas = new List<string>(),
                            ColumnasNoEditar = new List<string>(),
                        };
                        foreach (var col in esclava.Columnas)
                            locEsclavaDTO.Columnas.Add(col);
                        foreach (var col in esclava.ColumnasNoEditar)
                            locEsclavaDTO.ColumnasNoEditar.Add(col);
                        locTransicion.DTO.Colleccion.Add(locEsclavaDTO);
                    }
                    else
                    {
                        // Junta los DTO columnas
                        foreach (var col in esclava.Columnas)
                            if (!locEsclavaDTO.Columnas.Any(x => x == col))
                                locEsclavaDTO.Columnas.Add(col);

                        // Junta los DTO columnas no editar
                        foreach (var col in esclava.ColumnasNoEditar)
                            if (!locEsclavaDTO.ColumnasNoEditar.Any(x => x == col))
                                locEsclavaDTO.ColumnasNoEditar.Add(col);
                    }
                }
            }

            return Diagrama;
        }



    }
}
