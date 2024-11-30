using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using WooW.Core;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.TestTools
{
    public class WoTestFormValidator
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Método principal

        /// <summary>
        /// Validamos que este instalados todos los componentes necesarios
        /// para la ejecución de las pruebas de los formularios
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void ValidateEjecution()
        {
            try
            {
                CheckNode();
                ValidateTypeScript();
                ValidateLibrary();
                ValidateWoTestCafe();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error en la validación de los componentes para la ejecución de las pruebas de formulario. {ex.Message}"
                );
            }
        }

        #endregion Método principal


        #region Validación del node

        /// <summary>
        /// Comprueba que se encuentre una version instalada de node que sea compatible con TestCafe
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CheckNode()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process nodeVersionProcess = new Process();

                // Settings del app que genera
                nodeVersionProcess.EnableRaisingEvents = false;
                nodeVersionProcess.StartInfo.FileName = "cmd";

                // Settings de la salida de la consola
                nodeVersionProcess.StartInfo.RedirectStandardOutput = true;
                nodeVersionProcess.StartInfo.RedirectStandardError = true;
                nodeVersionProcess.StartInfo.CreateNoWindow = true;
                nodeVersionProcess.StartInfo.UseShellExecute = false;

                // Parámetros con las configuraciones para iniciar el docker
                StringBuilder strDockerSettings = new StringBuilder();

                strDockerSettings.Append($@"/c ");
                strDockerSettings.Append($@"Node -v");

                // Argumentos para el generador
                nodeVersionProcess.StartInfo.Arguments = strDockerSettings.ToString();

                // Arrancamos el proceso
                nodeVersionProcess.Start();

                // Recuperamos la salida del generador
                string output = nodeVersionProcess.StandardOutput.ReadToEnd();

                // Recuperamos la salida de error
                string error = nodeVersionProcess.StandardError.ReadToEnd();

                // Esperamos que termine la salida del generador
                nodeVersionProcess.WaitForExit();

                // Validamos que venga el identificador del contenedor en la salida
                if (output.IsDBNullOrStringEmpty())
                {
                    throw new Exception(
                        $"Error verificar la version de node \nInstale una versión node 20 o superior"
                    );
                }
                else
                {
                    // Verificamos el resultado en función de la salida para comprobar que la versión de node sea superior
                    // o igual a la versión 20
                    string[] outCol = output.Split('.');
                    int principalVercion = int.Parse(outCol.First().Replace("v", ""));
                    if (principalVercion < 20)
                    {
                        throw new Exception(
                            "Instale al menos una versión de node 20 o superior para utilizar el modulo de pruebas"
                        );
                    }
                }

                // En caso de que el proceso arroje un error enviamos una exception con el error
                if (error != string.Empty)
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al ejecutar la verificación de docker \nInstale una versión node 20 o superior. {ex.Message}"
                );
            }
        }

        #endregion Validación del node

        #region Validación e instalación de typescript

        /// <summary>
        /// Orquesta la validación e instalación de typescript
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ValidateTypeScript()
        {
            try
            {
                if (!ExistTypeScript())
                {
                    InstallTypeScript();
                    if (!ExistTypeScript())
                    {
                        throw new Exception(
                            "Error en la instalación de TypeScript, Instale TypeScript 5.4.5"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al validar typescript: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida la version de typescript
        /// </summary>
        /// <exception cref="Exception"></exception>
        private bool ExistTypeScript()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process nodeVersionProcess = new Process();

                // Settings del process
                nodeVersionProcess.EnableRaisingEvents = false;
                nodeVersionProcess.StartInfo.FileName = "cmd";

                // Settings de la salida de la consola
                nodeVersionProcess.StartInfo.RedirectStandardOutput = true;
                nodeVersionProcess.StartInfo.RedirectStandardError = true;
                nodeVersionProcess.StartInfo.CreateNoWindow = true;
                nodeVersionProcess.StartInfo.UseShellExecute = false;

                // Parámetros con las configuraciones para iniciar el docker
                StringBuilder strDockerSettings = new StringBuilder();

                strDockerSettings.Append($@"/c ");
                strDockerSettings.Append($@"tsc --version");

                // Argumentos para el generador
                nodeVersionProcess.StartInfo.Arguments = strDockerSettings.ToString();

                // Arrancamos el proceso
                nodeVersionProcess.Start();

                // Recuperamos la salida del generador
                string output = nodeVersionProcess.StandardOutput.ReadToEnd();

                // Recuperamos la salida de error
                string error = nodeVersionProcess.StandardError.ReadToEnd();

                // Esperamos que termine la salida del generador
                nodeVersionProcess.WaitForExit();

                // En caso de que el proceso arroje un error enviamos una exception con el error
                if (error != string.Empty)
                {
                    return false;
                }

                // Validamos que venga el identificador del contenedor en la salida
                if (
                    output
                        .Replace("Version", "")
                        .Replace(" ", "")
                        .Replace("\r", "")
                        .Replace("\n", "") == "5.4.5"
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al validar la version de typescript. {ex.Message}");
            }
        }

        /// <summary>
        /// Instala la version 5.4.5 de typescript
        /// </summary>
        private void InstallTypeScript()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process nodeVersionProcess = new Process();

                // Settings del process
                nodeVersionProcess.EnableRaisingEvents = false;
                nodeVersionProcess.StartInfo.FileName = "cmd";

                // Settings de la salida de la consola
                nodeVersionProcess.StartInfo.RedirectStandardOutput = true;
                nodeVersionProcess.StartInfo.RedirectStandardError = true;
                nodeVersionProcess.StartInfo.CreateNoWindow = true;
                nodeVersionProcess.StartInfo.UseShellExecute = false;

                // Parámetros con las configuraciones para iniciar el docker
                StringBuilder strDockerSettings = new StringBuilder();

                strDockerSettings.Append($@"/c ");
                strDockerSettings.Append($@"npm install -g typescript@5.4.5");

                // Argumentos para el generador
                nodeVersionProcess.StartInfo.Arguments = strDockerSettings.ToString();

                // Arrancamos el proceso
                nodeVersionProcess.Start();

                // Recuperamos la salida del generador
                string output = nodeVersionProcess.StandardOutput.ReadToEnd();

                // Recuperamos la salida de error
                string error = nodeVersionProcess.StandardError.ReadToEnd();

                // Esperamos que termine la salida del generador
                nodeVersionProcess.WaitForExit();

                // En caso de que el proceso arroje un error enviamos una exception con el error
                if (error != string.Empty)
                {
                    throw new Exception(error);
                }

                // Validamos que venga el identificador del contenedor en la salida
                if (output.IsDBNullOrStringEmpty())
                {
                    throw new Exception($"Error en la salida de la verificación de la librería");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al instalar typescript. {ex.Message}");
            }
        }

        #endregion Validación e instalación de typescript

        #region Librería de TestCafe

        /// <summary>
        /// Valida que la librería de woow este instalada y actualizada
        /// e intenta actualizar o instalar en caso que no lo encuentre
        /// </summary>
        private void ValidateLibrary()
        {
            try
            {
                if (!ExistTestCafeLibrary())
                {
                    InstallTestCafe();

                    if (!ExistTestCafeLibrary())
                    {
                        throw new Exception(
                            "La librería no se pudo instalar o actualizar, instale una version de TestCafe 3 o superior."
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al validar la librería de woow js, {ex.Message}");
            }
        }

        /// <summary>
        /// Comprobamos desde la cmd si la librería esta instalada en node
        /// </summary>
        /// <exception cref="Exception"></exception>
        private bool ExistTestCafeLibrary()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process nodeVersionProcess = new Process();

                // Directorio donde se instalara la librería
                string workingDirectory = "C:\\Users\\Frog";

                // Settings del process
                nodeVersionProcess.EnableRaisingEvents = false;
                nodeVersionProcess.StartInfo.FileName = "cmd";
                nodeVersionProcess.StartInfo.WorkingDirectory = workingDirectory;

                // Settings de la salida de la consola
                nodeVersionProcess.StartInfo.RedirectStandardOutput = true;
                nodeVersionProcess.StartInfo.RedirectStandardError = true;
                nodeVersionProcess.StartInfo.CreateNoWindow = true;
                nodeVersionProcess.StartInfo.UseShellExecute = false;

                // Parámetros con las configuraciones para iniciar el docker
                StringBuilder strDockerSettings = new StringBuilder();

                strDockerSettings.Append($@"/c ");
                strDockerSettings.Append($@"npm list ");
                strDockerSettings.Append($@"--depth=0");

                // Argumentos para el generador
                nodeVersionProcess.StartInfo.Arguments = strDockerSettings.ToString();

                // Arrancamos el proceso
                nodeVersionProcess.Start();

                // Recuperamos la salida del generador
                string output = nodeVersionProcess.StandardOutput.ReadToEnd();

                // Recuperamos la salida de error
                string error = nodeVersionProcess.StandardError.ReadToEnd();

                // Esperamos que termine la salida del generador
                nodeVersionProcess.WaitForExit();

                // En caso de que el proceso arroje un error enviamos una exception con el error
                if (error != string.Empty)
                {
                    throw new Exception(error);
                }

                // Validamos que venga el identificador del contenedor en la salida
                if (output.IsDBNullOrStringEmpty())
                {
                    throw new Exception($"Error en la salida de la verificación de la librería");
                }
                else
                {
                    // Verificamos que la librería este instalada en la ultima version
                    if (output.ToLower().Contains("testcafe@"))
                    {
                        string[] outCol = output.Split("testcafe@");
                        string[] vercionCol = outCol[1].Split('.');
                        return int.Parse(vercionCol[0]) >= 3;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error comprobando la versión. {ex.Message}");
            }
        }

        /// <summary>
        /// Instala la ultima version de la librería, permite actualizar o instalar la ultima version de TestCafe.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void InstallTestCafe()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process nodeVersionProcess = new Process();

                // Directorio donde se instalara la librería
                string workingDirectory = "C:\\Users\\Frog";

                // Settings del process
                nodeVersionProcess.EnableRaisingEvents = false;
                nodeVersionProcess.StartInfo.FileName = "cmd";
                nodeVersionProcess.StartInfo.WorkingDirectory = workingDirectory;

                // Settings de la salida de la consola
                nodeVersionProcess.StartInfo.RedirectStandardOutput = true;
                nodeVersionProcess.StartInfo.RedirectStandardError = true;
                nodeVersionProcess.StartInfo.CreateNoWindow = true;
                nodeVersionProcess.StartInfo.UseShellExecute = false;

                // Parámetros con las configuraciones para iniciar el docker
                StringBuilder strDockerSettings = new StringBuilder();

                strDockerSettings.Append($@"/c ");
                strDockerSettings.Append($@"npm install testcafe@latest");

                // Argumentos para el generador
                nodeVersionProcess.StartInfo.Arguments = strDockerSettings.ToString();

                // Arrancamos el proceso
                nodeVersionProcess.Start();

                // Recuperamos la salida del generador
                string output = nodeVersionProcess.StandardOutput.ReadToEnd();

                // Recuperamos la salida de error
                string error = nodeVersionProcess.StandardError.ReadToEnd();

                // Esperamos que termine la salida del generador
                nodeVersionProcess.WaitForExit();

                // En caso de que el proceso arroje un error enviamos una exception con el error
                if (error != string.Empty)
                {
                    if (
                        !error.Contains("npm WARN deprecated")
                        || !File.Exists(
                            $@"{workingDirectory}\node_modules\testcafe\bin\testcafe.js"
                        )
                    )
                    {
                        throw new Exception(error);
                    }
                }

                // Validamos que venga el identificador del contenedor en la salida
                if (output.IsDBNullOrStringEmpty())
                {
                    throw new Exception($"Error en la salida de la verificación de la librería");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error comprobando la versión. {ex.Message}");
            }
        }

        #endregion Librería de TestCafe

        #region Librería de WoTestCafe

        /// <summary>
        /// Validamos que la librería de woow de js se encuentre instalada y
        /// en caso de que no se actualiza o instala
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ValidateWoTestCafe()
        {
            try
            {
                if (!ExistWoLibrary())
                {
                    InstallWoLibary();
                    MoveLibary();
                    if (!ExistWoLibrary())
                    {
                        throw new Exception("Error al instalar la libraría.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al validar la librería de woow para TestCafe. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Valida si existe la Liberia a través de uno de los ficheros clave
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool ExistWoLibrary()
        {
            try
            {
                string libaryPath =
                    $@"{_project.DirProyectData}\Test\TestCafe\woow.testcafe.typescript\WoComponents\WoTextEdit.ts";
                return File.Exists(libaryPath);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al validar la librería de TestCafe. {ex.Message}");
            }
        }

        /// <summary>
        /// Instala la Iberia o la actualiza
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void InstallWoLibary()
        {
            try
            {
                // Instancia del proceso para la compilación
                Process nodeVersionProcess = new Process();

                // Directorio donde se instalara la librería
                string workingDirectory = $@"{_project.DirProyectData}\Test\TestCafe";

                // Settings del process
                nodeVersionProcess.EnableRaisingEvents = false;
                nodeVersionProcess.StartInfo.FileName = "cmd";
                nodeVersionProcess.StartInfo.WorkingDirectory = workingDirectory;

                // Settings de la salida de la consola
                nodeVersionProcess.StartInfo.RedirectStandardOutput = true;
                nodeVersionProcess.StartInfo.RedirectStandardError = true;
                nodeVersionProcess.StartInfo.CreateNoWindow = true;
                nodeVersionProcess.StartInfo.UseShellExecute = false;

                // Parámetros con las configuraciones para iniciar el docker
                StringBuilder strDockerSettings = new StringBuilder();

                strDockerSettings.Append($@"/c ");
                strDockerSettings.Append($@"npm install woow.testcafe.typescript@latest");

                // Argumentos para el generador
                nodeVersionProcess.StartInfo.Arguments = strDockerSettings.ToString();

                // Arrancamos el proceso
                nodeVersionProcess.Start();

                // Recuperamos la salida del generador
                string output = nodeVersionProcess.StandardOutput.ReadToEnd();

                // Recuperamos la salida de error
                string error = nodeVersionProcess.StandardError.ReadToEnd();

                // Esperamos que termine la salida del generador
                nodeVersionProcess.WaitForExit();

                // Validamos que venga el identificador del contenedor en la salida
                if (output.IsDBNullOrStringEmpty())
                {
                    throw new Exception($"Error en la salida de la verificación de la librería");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error comprobando la versión. {ex.Message}");
            }
        }

        /// <summary>
        /// Movemos la librería a un path mas accesible, para usarla como clases de js
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void MoveLibary()
        {
            try
            {
                WoDirectory.DeleteFile(
                    $@"{_project.DirProyectData}\Test\TestCafe\node_modules\woow.testcafe.typescript\Index.ts"
                );
                WoDirectory.DeleteFile(
                    $@"{_project.DirProyectData}\Test\TestCafe\node_modules\woow.testcafe.typescript\.gitattributes"
                );
                WoDirectory.DeleteFile(
                    $@"{_project.DirProyectData}\Test\TestCafe\node_modules\woow.testcafe.typescript\package.json"
                );
                WoDirectory.DeleteFile(
                    $@"{_project.DirProyectData}\Test\TestCafe\node_modules\woow.testcafe.typescript\TestingProperties.json"
                );

                string oldPathLibari =
                    $@"{_project.DirProyectData}\Test\TestCafe\node_modules\woow.testcafe.typescript";
                string newPahtLibary =
                    $@"{_project.DirProyectData}\Test\TestCafe\woow.testcafe.typescript";
                WoDirectory.MoveDirectory(oldPathLibari, newPahtLibary);

                WoDirectory.DeleteDirectory(
                    $@"{_project.DirProyectData}\Test\TestCafe\node_modules"
                );

                WoDirectory.DeleteFile($@"{_project.DirProyectData}\Test\TestCafe\package.json");
                WoDirectory.DeleteFile(
                    $@"{_project.DirProyectData}\Test\TestCafe\package-lock.json"
                );
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al intentar mover la librería. {ex.Message}");
            }
        }

        #endregion Librería de WoTestCafe
    }
}
