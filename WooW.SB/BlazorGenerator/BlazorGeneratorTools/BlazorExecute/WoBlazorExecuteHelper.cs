using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;

namespace WooW.SB.BlazorGenerator.BlazorGeneratorTools.BlazorExecute
{
    public static class WoBlazorExecuteHelper
    {
        /// <summary>
        /// Busca si hay algún proceso de blazor que no se aya detenido o
        /// que este en ejecución fuera del proceso de la aplicación y lo detiene
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public static void StopExtraBlazorExecute()
        {
            try
            {
                Process[] processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    string commandLine = string.Empty;

                    using (
                        var searcher = new ManagementObjectSearcher(
                            $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"
                        )
                    )
                    {
                        var objects = searcher.Get();
                        commandLine = objects
                            .Cast<ManagementBaseObject>()
                            .SingleOrDefault()
                            ?["CommandLine"]?.ToString();

                        if (commandLine != null && commandLine.Contains("woow"))
                        {
                            process.Kill(true);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Inicia el navegador con el url que se recibe por parámetro
        /// </summary>
        /// <param name="url"></param>
        [SupportedOSPlatform("windows")]
        public static void StartNavigator(string url)
        {
            try
            {
                Process navigatorProcess = Process.Start("cmd", $@"/c start {url}");
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al intentar inicializar el navegador. {ex.Message}");
            }
        }
    }
}
