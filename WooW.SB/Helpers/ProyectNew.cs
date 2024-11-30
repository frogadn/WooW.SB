using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Helpers
{
    public static class ProyectNew
    {
        public static void Validate(
            string DirBase,
            string Name,
            string Version,
            string Release,
            string Fix,
            string Origen
        )
        {
            if ((DirBase.IsNullOrStringEmpty()) || !Path.Exists(DirBase.ToString()))
                throw new Exception("Debe seleccionar un directorio que exista");

            Regex regex = new Regex(@"[A-Z][a-zA-Z0-9]*");
            if ((Name.IsNullOrStringEmpty()) || regex.Match(Name).Value.ToString() != Name)
                throw new Exception(
                    "Nombre del proyecto debe comenzar con una letra mayúsculas y continuar con letras minúsculas, mayúsculas o números"
                );

            if (
                (Version.Length != 2)
                || !Version.All(char.IsDigit)
                || (Release.Length != 2)
                || !Release.All(char.IsDigit)
                || (Fix.Length != 2)
                || !Fix.All(char.IsDigit)
            )
                throw new Exception("La versión, revisión y el fix deben de dos digitos númericos");

            string Nombre = $"{Name}.V{Version}.{Release}.{Fix}";

            var dirProyect = Path.Combine(DirBase.ToString(), Nombre, Nombre);

            if (Path.Exists(dirProyect))
                throw new Exception($"El directorio del proyecto {dirProyect} ya existe");

            var final = Path.Combine(DirBase.ToString(), Nombre, Nombre + ".wwsb");

            if (File.Exists(final))
                throw new Exception($"El nombre del proyecto {dirProyect} ya existe");

            PackageHelper.ValidaNombreOrigenYSoloLectura(final, Origen);
        }

        public static string Do(
            string DirBase,
            string Name,
            string Version,
            string Release,
            string Fix,
            string Origen,
            ProyectType Type,
            bool Multitenancy = true,
            bool ProyectoERP = true,
            bool IdiomasDefault = true,
            bool ProcesosDefault = true,
            bool RolesDefault = true,
            bool PermisosDefault = true
        )
        {
            string OrigenWooW = "Wo";

            try
            {
                string NombreProyecto = $"{Name}.V{Version}.{Release}.{Fix}";
                var dirProyect = Path.Combine(DirBase, NombreProyecto);

                ProyectNew.CopiaTemplate(DirBase, Name, Version, Release, Fix);

                // Obtiene la version del woow.sb
                var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                // Obtiene el Path de ejecucion del woow.sb
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // Repositorio donde se encuntran los archivos de plantilla
                var repoDir = Path.Combine(
                    path,
                    $"Templates.{assemblyVersion.Major}.{assemblyVersion.Minor}"
                );

                if (!Path.Exists(repoDir))
                    throw new Exception($"No se encontró el repositorio de plantillas {repoDir}");

                // Crea archivo de proyecto
                var archivoProyect = Path.Combine(dirProyect, NombreProyecto + ".wwsb");

                var locProyecto = new Proyecto();

                locProyecto.ArchivoDeProyecto = archivoProyect;
                locProyecto.Nombre = Name;
                locProyecto.Version = Version;
                locProyecto.Release = Release;
                locProyecto.Fix = Fix;
                locProyecto.TipoProyecto = Type;
                locProyecto.Dir = dirProyect;

                locProyecto.ParConexion = new ParametrosConexion();

                locProyecto.ParConexion.Usuario = "admin@email.com";
                locProyecto.ParConexion.Password = "mypass";
                locProyecto.ParConexion.Udn = string.Empty;
                locProyecto.ParConexion.Instance = "IIA040805DZ4";
                locProyecto.ParConexion.InstanceType = tWoIntanciaType.DEV;
                locProyecto.ParConexion.Year = DateTime.Today.Year;

                if (IdiomasDefault)
                {
                    locProyecto.Idiomas = new List<Idioma>
                    {
                        new Idioma { Id = "es-MX", EtiquetaId = "EspañolMexicano" },
                        new Idioma { Id = "en-US", EtiquetaId = "InglesNorteamericano" }
                    };
                }

                if (ProcesosDefault)
                {
                    locProyecto.Procesos = new List<Proceso>
                    {
                        new Proceso { Id = OrigenWooW + "Con", EtiquetaId = "Contabilidad" },
                        new Proceso { Id = OrigenWooW + "Inv", EtiquetaId = "Inventarios" },
                        new Proceso { Id = OrigenWooW + "Sys", EtiquetaId = "Sistema" },
                        new Proceso { Id = OrigenWooW + "Sat", EtiquetaId = "Sat" },
                        new Proceso { Id = OrigenWooW + "Dat", EtiquetaId = "DatosMaestros" },
                        new Proceso { Id = OrigenWooW + "Cxc", EtiquetaId = "CuentasPorCobrar" },
                        new Proceso { Id = OrigenWooW + "Com", EtiquetaId = "Compras" },
                        new Proceso { Id = OrigenWooW + "Vta", EtiquetaId = "Ventas" },
                        new Proceso { Id = OrigenWooW + "Fac", EtiquetaId = "Facturacion" },
                        new Proceso { Id = OrigenWooW + "Tes", EtiquetaId = "Tesoreria" },
                        new Proceso { Id = OrigenWooW + "Cxp", EtiquetaId = "CuentasPorPagar" }
                    };
                }

                if (RolesDefault)
                {
                    locProyecto.Roles = new List<Rol>();
                    foreach (var proceso in locProyecto.Procesos)
                    {
                        locProyecto.Roles.Add(
                            new Rol()
                            {
                                Id = proceso.Id + "Sup",
                                EtiquetaId = "SupervisorDe" + proceso.EtiquetaId
                            }
                        );
                        locProyecto.Roles.Add(
                            new Rol()
                            {
                                Id = proceso.Id + "Aux",
                                EtiquetaId = "AuxiliarDe" + proceso.EtiquetaId
                            }
                        );
                    }
                }

                if (PermisosDefault)
                {
                    locProyecto.Permisos = new List<Permiso>
                    {
                        new Permiso { Id = OrigenWooW + "Lectura", EtiquetaId = "Lectura" },
                        // Catalogos

                        new Permiso
                        {
                            Id = OrigenWooW + "CatalogoNuevo",
                            EtiquetaId = "CatalogoNuevo"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "CatalogoModificar",
                            EtiquetaId = "CatalogoModificar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "CatalogoSuspender",
                            EtiquetaId = "CatalogoSuspender"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "CatalogoActivar",
                            EtiquetaId = "CatalogoActivar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "CatalogoDarDeBaja",
                            EtiquetaId = "CatalogoDarDeBaja"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "CatalogoBorrar",
                            EtiquetaId = "CatalogoBorrar"
                        },
                        // Transacciones

                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionRegistrar",
                            EtiquetaId = "TransaccionRegistrar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionRegistrarYAfectar",
                            EtiquetaId = "TransaccionRegistrarYAfectar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionModificar",
                            EtiquetaId = "TransaccionModificar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionAfectar",
                            EtiquetaId = "TransaccionAfectar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionDescartar",
                            EtiquetaId = "TransaccionDescartar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionAntesContabilizar",
                            EtiquetaId = "TransaccionAntesContabilizar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionContabilizar",
                            EtiquetaId = "TransaccionContabilizar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "TransaccionDescontabilizar",
                            EtiquetaId = "TransaccionDescontabilizar"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "GenerarReportes",
                            EtiquetaId = "GenerarReportes"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "ImportarDatos",
                            EtiquetaId = "ImportarDatos"
                        },
                        new Permiso
                        {
                            Id = OrigenWooW + "ModificarParametros",
                            EtiquetaId = "ModificarParametros"
                        }
                    };
                }

                locProyecto.Apps = new List<App>()
                {
                    new App { Id = "WebService", EtiquetaId = "ServicioWeb" },
                    new App { Id = "WebClient", EtiquetaId = "ClienteWeb" },
                };

                locProyecto.EtiquetaCol = new EtiquetaCol();

                foreach (var item in locProyecto.Idiomas)
                    locProyecto.EtiquetaCol.Etiquetas.Add(
                        new Etiqueta(item.EtiquetaId, "es-MX", SplitCamelCase(item.EtiquetaId))
                    );

                foreach (var item in locProyecto.Procesos)
                    locProyecto.EtiquetaCol.Etiquetas.Add(
                        new Etiqueta(item.EtiquetaId, "es-MX", SplitCamelCase(item.EtiquetaId))
                    );

                foreach (var item in locProyecto.Roles)
                    locProyecto.EtiquetaCol.Etiquetas.Add(
                        new Etiqueta(item.EtiquetaId, "es-MX", SplitCamelCase(item.EtiquetaId))
                    );

                foreach (var item in locProyecto.Permisos)
                    locProyecto.EtiquetaCol.Etiquetas.Add(
                        new Etiqueta(item.EtiquetaId, "es-MX", SplitCamelCase(item.EtiquetaId))
                    );

                foreach (var item in locProyecto.Apps)
                    locProyecto.EtiquetaCol.Etiquetas.Add(
                        new Etiqueta(item.EtiquetaId, "es-MX", SplitCamelCase(item.EtiquetaId))
                    );

                locProyecto.Save();
                return archivoProyect;
            }
            catch (Exception ex)
            {
                ProyectNew.Clean(DirBase, Name, Version, Release, Fix);
                throw ex;
            }
        }

        // Rutina que separa cadena camel case palabras espaciadas
        public static string SplitCamelCase(string input)
        {
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        public static void CopiaTemplate(
            string DirBase,
            string Name,
            string Version,
            string Release,
            string Fix
        )
        {
            string NombreProyecto = $"{Name}.V{Version}.{Release}.{Fix}";

            // Crea el directorio
            var dirProyect = Path.Combine(DirBase, NombreProyecto);

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            // Obtiene el Path de ejecucion del woow.sb
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Repositorio donde se encuntran los archivos de plantilla
            var repoDir = Path.Combine(
                path,
                $"Templates.{assemblyVersion.Major}.{assemblyVersion.Minor}"
            );

            if (!Path.Exists(repoDir))
                throw new Exception($"No se encontró el repositorio de plantillas {repoDir}");

            // Copia archivos y carpetas de la carpeta plantilla al carpeta del proyecto incluyendo subcarpetas
            ProyectNew.CopyPlantillas(repoDir, dirProyect);
        }

        public static void Clean(
            string DirBase,
            string Name,
            string Version,
            string Release,
            string Fix
        )
        {
            string NombreProyecto = $"{Name}.V{Version}.{Release}.{Fix}";

            var dirProyect = Path.Combine(DirBase, NombreProyecto);

            if (Path.Exists(dirProyect))
                Directory.Delete(dirProyect, true);

            // Borrra archivo del proyecto
            var archivoProyect = Path.Combine(DirBase, NombreProyecto + ".wwsb");
            if (File.Exists(archivoProyect))
                File.Delete(archivoProyect);
        }

        // Rutina que copia los archivos de plantilla al proyecto incluyendo subdirectorios
        public static void CopyPlantillas(
            string sourceDir,
            string targetDir,
            string searchPattern = "*.*"
        )
        {
            if (!Path.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            // Copia los archivos
            List<string> files = Directory
                .GetFiles(sourceDir, "*.*")
                .Where(file => !file.ToLower().EndsWith(".js"))
                .ToList();
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
            }

            // Copia los subdirectorios
            string[] dirs = Directory.GetDirectories(sourceDir);
            foreach (var dir in dirs)
            {
                var dirName = Path.GetFileName(dir);
                var destDir = Path.Combine(targetDir, dirName);
                ProyectNew.CopyPlantillas(dir, destDir, searchPattern);
            }
        }
    }
}
