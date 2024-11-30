using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WooW.Core;
using WooW.SB.Config;

namespace WooW.SB.Helpers
{
    public static class PackageHelper
    {
        public static void IsPackagesCorrecteName(string fileName)
        {
            string origen = Path.GetFileNameWithoutExtension(fileName).Substring(0, 2);

            ValidaNombreOrigenYSoloLectura(fileName, origen);
        }

        public static bool ValidaNombreOrigenYSoloLectura(string fileName, string origen)
        {
            bool OrigenDiferenteSoloLectura = false;

            if (!Regex.IsMatch(origen.ToSafeString(), @"^[A-Z][a-z0-9]"))
                throw new Exception(
                    $"Origen ({origen.ToSafeString()}) debe de ser de 2 caracteres, comenzar con una letra mayúsculas y seguir con una letra minuscula o número"
                //"Origen debe de ser de 2 caracteres, comenzar con una letra mayúsculas y seguir con una letra minuscula o número"
                );

            if (Path.GetFileNameWithoutExtension(fileName).Length < 3)
                throw new Exception($"El archivo {fileName} debe tener mas de 3 caracteres");

            string locOrigen = Path.GetFileNameWithoutExtension(fileName).Substring(0, 3);
            if (!Regex.IsMatch(locOrigen.ToSafeString(), @"^[A-Z][a-z0-9]"))
                throw new Exception(
                    $"El archivo {fileName} debe comenzar con el origen (como el actual '{origen}' o otro), despues tener una letra mayúsculas y seguir con una letra minuscula o número"
                );

            if (!char.IsUpper(locOrigen[2]))
                throw new Exception($"El archivo {fileName} la tercera letra debe se máyuscula");

            //vexMAX_ERP.V01.01.01.wwsb
            // Valida que el archivo termine con V mayuscula mas dos digitos un punto mas dos digitos mas un punto mas dos digitos
            if (Path.GetExtension(fileName).ToLower() != ".wwsb")
                throw new Exception("El archivo no tiene la extensión correcta wwsb");

            // Valida que el archivo comienze con el origen
            if (!Path.GetFileNameWithoutExtension(fileName).StartsWith(origen))
                OrigenDiferenteSoloLectura = true;

            var name = Path.GetFileNameWithoutExtension(fileName);

            if (name.Length < 11) // .V01.01.01
                throw new Exception("El archivo no tiene el nombre correcto");

            var subfijo = name.Substring(name.Length - 10).ToLower();

            try
            {
                // .V01.01.01
                // 0123456789

                if (!subfijo.StartsWith(".v"))
                    throw new Exception();

                if ((subfijo.Substring(4, 1) != ".") || (subfijo.Substring(7, 1) != "."))
                    throw new Exception();

                var version = subfijo.Substring(2, 2);
                var release = subfijo.Substring(5, 2);
                var fix = subfijo.Substring(8, 2);

                if (
                    !version.All(char.IsDigit)
                    || !release.All(char.IsDigit)
                    || !fix.All(char.IsDigit)
                )
                    throw new Exception();
            }
            catch
            {
                throw new Exception("El archivo debe tener el subfijo .v##.##.##.wwsb");
            }

            // Validar que el directorio donde se encuentre se llame igual que el archivo

            var dir = Path.GetDirectoryName(fileName);
            var dirName = Path.GetFileName(dir);

            if (dirName != name)
                throw new Exception(
                    $"El directorio no tiene el mismo nombre que el nombre del proyecto {dirName} Vs {name}"
                );

            return OrigenDiferenteSoloLectura;
        }

        public static List<Etiqueta> GetPackagesEtiquetas(Proyecto proyecto)
        {
            var Etiquetas = new List<Etiqueta>();
            AddEtiqueta(proyecto, Etiquetas);
            return Etiquetas;
        }

        private static void AddEtiqueta(Proyecto proyecto, List<Etiqueta> Etiquetas)
        {
            foreach (var etiqueta in proyecto.EtiquetaCol.Etiquetas)
            {
                if (!Etiquetas.Any(e => e.Id == etiqueta.Id))
                    Etiquetas.Add(etiqueta);
            }

            foreach (var package in proyecto.Paquetes)
            {
                string pathReal = Proyecto.ArchivoPaquete(
                    Path.GetFullPath(proyecto.ArchivoDeProyecto),
                    package.Archivo
                );
                Proyecto local = new Proyecto();
                local.Load(pathReal);
                AddEtiqueta(local, Etiquetas);
            }
        }

        public static List<Mensaje> GetPackagesMensaje(Proyecto proyecto)
        {
            var Mensajes = new List<Mensaje>();
            AddMensaje(proyecto, Mensajes);
            return Mensajes;
        }

        private static void AddMensaje(Proyecto proyecto, List<Mensaje> Mensajes)
        {
            foreach (var mensaje in proyecto.MensajeCol.Mensajes)
            {
                if (!Mensajes.Any(e => e.Id == mensaje.Id))
                    Mensajes.Add(mensaje);
            }

            foreach (var package in proyecto.Paquetes)
            {
                string pathReal = Proyecto.ArchivoPaquete(
                    Path.GetFullPath(proyecto.ArchivoDeProyecto),
                    package.Archivo
                );
                Proyecto local = new Proyecto();
                local.Load(pathReal);
                AddMensaje(local, Mensajes);
            }
        }
    }
}
