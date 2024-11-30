using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using WooW.Core;

namespace WooW.SB.Config
{
    public class ParametrosConexion
    {
        [Browsable(false)]
        public string Usuario { get => usuario; set => usuario = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string Password { get => password; set => password = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string Instance { get => instance; set => instance = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public tWoIntanciaType InstanceType { get => instancetype; set => instancetype = value; }

        [Browsable(false)]
        public string InstanceDataBase { get => instanceDataBase; set => instanceDataBase = value; }

        [Browsable(false)]
        public int Year { get; set; }

        [Browsable(false)]
        public string Udn { get => udn; set => udn = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string DbApp { get => dbApp; set => dbApp = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string UnitTestApp { get => unitTestApp; set => unitTestApp = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string DBRestore { get => dbRestore; set => dbRestore = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string DBRestoreApp { get => dbRestoreApp; set => dbRestoreApp = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string DBAccion { get => dbAccion; set => dbAccion = (value == null ? string.Empty : value.Trim()); }

        [Browsable(false)]
        public string DBAccionApp { get => dbAccionApp; set => dbAccionApp = (value == null ? string.Empty : value.Trim()); }


        [Browsable(false)]
        [JsonIgnore]
        public Process proc = null;

        private Proyecto proyecto;
        private string usuario;
        private string password;
        private string instance;
        private string instanceDataBase;
        private tWoIntanciaType instancetype;
        private string udn;
        private string dbApp;
        private string unitTestApp;
        private string dbRestore;
        private string dbRestoreApp;
        private string dbAccion;
        private string dbAccionApp;

        public ParametrosConexion()
        {
            Usuario = string.Empty;
            Password = string.Empty;
            Instance = string.Empty;
            InstanceType = tWoIntanciaType.DEV;
            InstanceDataBase = "DEV Sqlite";
            Udn = string.Empty;
            dbApp = string.Empty;
        }

        public void ProyectoSetter(Proyecto _proyecto)
        {
            proyecto = _proyecto;
        }

        public void CierraServicio()
        {
            try
            {
                if (proc != null)
                    proc.CloseMainWindow();
            }
            catch { }
        }


        public void IniciaServicio()
        {
            CierraServicio();

            string sProceso =
                $"{proyecto.DirApplication_WebService_WooWServer}\\bin\\Debug\\net8.0\\WooW.Server.exe";

            ProcessStartInfo startInfo = new ProcessStartInfo(sProceso, "-d"); // string.Empty);
            startInfo.WindowStyle = ProcessWindowStyle.Minimized; //  e.Hidden; 
            startInfo.Arguments = "--urls \"https://localhost:5101;http://localhost:5100\"";
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = $"{proyecto.DirApplication_WebService_WooWServer}";

            proc = Process.Start(startInfo); //Ejecuta el proceso
        }

        public string DbName()
        {
            if (InstanceType == tWoIntanciaType.DEV)
                return $"DB_{Instance}_{InstanceType.ToString()}_{Year}.sqlite";
            else
                return $"DB_{Instance}_{InstanceType.ToString()}_{Year}";
        }


    }
}
