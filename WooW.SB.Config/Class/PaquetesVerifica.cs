using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WooW.Core;

namespace WooW.SB.Config.Class
{
    public class PaquetesVerifica
    {
        public PaquetesVerifica(string modelo, string paquete, WoSubTypeModel subTipoModelo)
        {
            Modelo = modelo;
            Paquete = paquete;
            SubTipoModelo = subTipoModelo;
            Columnas = new Dictionary<string, string>();
        }

        public string Modelo { get; set; }
        public WoSubTypeModel SubTipoModelo { get; set; }
        public string Paquete { get; set; }
        public Dictionary<string, string> Columnas { get; set; }

        private static void AgregaModelo(Dictionary<string, PaquetesVerifica> modelos,
            StringBuilder sbModelos, string paquete, string modelo, WoSubTypeModel subTipoModelo, List<ModeloColumna> modeloColumnas)
        {
            if (modelos.ContainsKey(modelo.ToUpper()))
            {
                if (modelos[modelo.ToUpper()].Modelo != modelo)
                    sbModelos.AppendLine($"Modelo: {modelos[modelo.ToUpper()].Modelo} / {modelo}, en Paquete: {modelos[modelo.ToUpper()].Paquete} y {paquete}, difiere en las mayusculas y minusculas ");

                if ((modelos[modelo.ToUpper()].SubTipoModelo == WoSubTypeModel.Extension) ||
                    (modelos[modelo.ToUpper()].SubTipoModelo == WoSubTypeModel.Override))
                {
                    if ((subTipoModelo == WoSubTypeModel.Extension) ||
                        (subTipoModelo == WoSubTypeModel.Override))
                        return;

                    var paqueteVerifica = new PaquetesVerifica(modelo, paquete, subTipoModelo);

                    foreach (var columna in modeloColumnas)
                    {
                        if (modelos[modelo.ToUpper()].Columnas.ContainsKey(columna.Id.ToUpper()))
                        {
                            if (modelos[modelo.ToUpper()].Columnas[columna.Id.ToUpper()] != paquete)
                                sbModelos.AppendLine($"Modelo: {modelo} en Paquete: {modelos[modelo.ToUpper()].Columnas[columna.Id.ToUpper()]} y {paquete}, la columna {columna.Id} se repite");
                        }
                        else
                            paqueteVerifica.Columnas.Add(columna.Id.ToUpper(), paquete);
                    }

                    // Checar
                    modelos.Remove(modelo.ToUpper());
                    modelos.Add(modelo.ToUpper(), paqueteVerifica);
                    return;
                }

                if ((modelos[modelo.ToUpper()].Paquete != paquete) &&
                    ((subTipoModelo != WoSubTypeModel.Extension) && (subTipoModelo != WoSubTypeModel.Override)))
                {
                    sbModelos.AppendLine($"Modelo: {modelo} en Paquete: {modelos[modelo.ToUpper()].Paquete} y {paquete}");
                }
            }
            else
            {
                var paqueteVerifica = new PaquetesVerifica(modelo, paquete, subTipoModelo);
                foreach (var columna in modeloColumnas)
                    paqueteVerifica.Columnas.Add(columna.Id.ToUpper(), paquete);
                modelos.Add(modelo.ToUpper(), paqueteVerifica);
            }
        }


        public static string RecursivosYModelosRepetidos(string file)
        {
            // Pila
            Stack<string> pila = new Stack<string>();

            Dictionary<string, PaquetesVerifica> modelos = new Dictionary<string, PaquetesVerifica>();
            StringBuilder sbModelos = new StringBuilder();

            Proyecto tmp = new Proyecto();
            tmp.Load(file);

            pila.Push(Path.GetFileName(file.ToUpper()));

            foreach (var modelo in tmp.ModeloCol.Modelos)
                AgregaModelo(
                    modelos, sbModelos,
                    Path.GetFileNameWithoutExtension(tmp.ArchivoDeProyecto).ToUpper(),
                    modelo.Id,
                    modelo.SubTipoModelo,
                    modelo.Columnas
                );

            foreach (var item in tmp.Paquetes)
                VerificaPaquete(pila, modelos, sbModelos, file, item.Archivo);

            return sbModelos.ToString();

        }

        public static void VerificaPaquete(Stack<string> pila, Dictionary<string, PaquetesVerifica> modelos,
            StringBuilder sbModelos, string DirBase, string file)
        {
            if (file.IsNullOrStringEmpty())
                return;


            string locfile = Path.GetFileName(file.ToUpper());

            if (pila.Contains(locfile))
            {
                string sPila = string.Empty;
                for (int i = pila.Count - 1; i >= 0; i--)
                    sPila += pila.ElementAt(i) + " -> ";
                sPila += locfile;
                throw new Exception($"Ciclo en la referencia de paquetes {sPila}\r\n\r\nEdite: {DirBase} en el nodo Paquetes y elimine la referencia a {Path.GetFileName(file)}");
            }

            pila.Push(locfile);

            string path = Proyecto.GetAbsolutePath(
                Path.GetDirectoryName(DirBase),
                Path.GetDirectoryName(file)
            );

            string pathFinal = Path.Combine(path, Path.GetFileName(locfile));

            Proyecto tmp = new Proyecto();
            tmp.Load(pathFinal);

            foreach (var modelo in tmp.ModeloCol.Modelos)
                AgregaModelo(
                    modelos,
                    sbModelos,
                    Path.GetFileNameWithoutExtension(tmp.ArchivoDeProyecto).ToUpper(),
                    modelo.Id,
                    modelo.SubTipoModelo,
                    modelo.Columnas
                );

            foreach (var item in tmp.Paquetes)
                VerificaPaquete(pila, modelos, sbModelos, pathFinal, item.Archivo);

            pila.Pop();
        }



    }
}
