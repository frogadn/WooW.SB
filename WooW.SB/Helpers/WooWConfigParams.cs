using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using WooW.SB.Config.Helpers;

namespace WooW.SB.Helpers
{
    /// <summary>
    /// Clase que representa los parámetros de configuración para WooW.
    /// Contiene propiedades y métodos necesarios para gestionar la configuración
    /// específica de la aplicación WooW.
    /// </summary>
    public class WooWConfigParams
    {
        #region Propiedades
        /// <summary>
        /// Propiedad que representa el origen asignado por Frog.
        /// </summary>
        /// <value>
        /// Una cadena que contiene el origen.
        /// </value>
        [Description("Origen"), Category("Origen asigando por Frog")]
        public string Origen { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public bool OrigenDiferenteSoloLectura { get; set; }

        /// <summary>
        /// Propiedad que representa la URL utilizada en los parámetros de GPT.
        /// </summary>
        /// <value>
        /// Una cadena que contiene la URL.
        /// </value>
        [Description("Url"), Category("Parámetros GPT")]
        public string GptUrl { get; set; }

        /// <summary>
        /// Representa el modelo utilizado en los parámetros de GPT.
        /// </summary>
        /// <value>
        /// Una cadena que contiene el nombre del modelo.
        /// </value>
        [Description("Modelo"), Category("Parámetros GPT")]
        public string GptModel { get; set; }

        /// <summary>
        /// Representa el usuario para ingresar al GPT.
        /// </summary>
        /// <value>
        /// Una cadena que contiene el nombre del usuario.
        /// </value>
        [Description("Usuario"), Category("Parámetros GPT")]
        public string GptUser { get; set; }

        /// <summary>
        /// Propiedad que representa la contraseña para ingresar al GPT.
        /// </summary>
        /// <value>
        /// Una cadena que contiene la contraseña.
        /// </value>
        [Description("Password"), Category("Parámetros GPT")]
        public string GptPassword { get; set; }

        /// <summary>
        /// Representa la clave de API utilizada para autenticar solicitudes.
        /// </summary>
        /// <value>
        /// La clave de API para ingresar al GPT.
        /// </value>
        [Description("Api Key"), Category("Parámetros GPT")]
        public string GptApiKey { get; set; }

        /// <summary>
        /// Propiedad que almacena el prompt utilizado para completar el script en el editor.
        /// </summary>
        /// <value>
        /// Una cadena de texto que representa el prompt para el completado del script.
        /// </value>
        [Description("Prompt para completar"), Category("Prompt para  GPT")]
        [EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorCompletar { get; set; }

        /// <summary>
        /// Propiedad que representa el prompt utilizado para agregar un test en el editor.
        /// </summary>
        /// <value>
        /// Una cadena de texto que contiene el prompt para la funcionalidad de agregar test.
        /// </value>
        [Description("Prompt para agregar test"), Category("Prompt para  GPT")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorAgregarTest { get; set; }

        /// <summary>
        /// Propiedad que almacena un prompt utilizado para encontrar errores en el editor de scripts.
        /// </summary>
        /// <value>
        /// Una cadena de texto que representa el prompt para la detección de errores.
        /// </value>
        [Description("Prompt para encontrar errores"), Category("Prompt para  GPT")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorEncuentraErrores { get; set; }

        /// <summary>
        /// Propiedad que almacena un prompt para optimizar el script en el editor.
        /// </summary>
        /// <value>
        /// Una cadena que representa el prompt utilizado para la optimización.
        /// </value>
        [Description("Prompt para optimizar"), Category("Prompt para  GPT")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorOptimizar { get; set; }

        /// <summary>
        /// Propiedad que almacena el prompt utilizado para el editor de scripts de comentarios.
        /// </summary>
        /// <value>
        /// Una cadena que representa el texto del prompt para el editor de scripts.
        /// </value>
        [Description("Prompt para comentar"), Category("Prompt para  GPT")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorComentar { get; set; }

        /// <summary>
        /// Propiedad que almacena el prompt utilizado para crear un resumen en el editor de scripts.
        /// </summary>
        /// <value>
        /// Una cadena de texto que representa el prompt para la generación de resúmenes.
        /// </value>
        [Description("Prompt crear resumen"), Category("Prompt para  GPT")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorResumen { get; set; }

        /// <summary>
        /// Propiedad que almacena el prompt utilizado para crear un resumen de la clase.
        /// </summary>
        /// <value>
        /// Una cadena de texto que representa el prompt para el editor de scripts.
        /// </value>
        [Description("Prompt crear resumen de la clase"), Category("Prompt para  GPT")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorResumenClase { get; set; }

        /// <summary>
        /// Propiedad que representa el prompt utilizado para el contexto de conexión en el editor de scripts.
        /// </summary>
        /// <value>
        /// Una cadena de texto que contiene el prompt para el contexto de conexión.
        /// </value>
        [Description("Prompt para contexto de conexión"), Category("Prompt para  GPT")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PromptScripEditorContexto { get; set; }

        #endregion Propiedades

        private static volatile WooWConfigParams uniqueInstance = null;

        private static readonly object padlock = new object();

        /// <summary>
        /// Clase que configura los parámetros de contexto para el asistente de programación WooWAi.
        /// Incluye prompts para diversas tareas como completar código, encontrar errores,
        /// agregar pruebas unitarias, optimizar y comentar código, así como crear resúmenes.
        /// </summary>
        public WooWConfigParams()
        {
            Origen = "Us";
            PromptScripEditorContexto =
                @"Eres un asistente de programación llamado WooWAi y tu función es ayudar a
los desarrolladores y resolver problemas de los programación, deberas
entregar tus respuestas en español y si es codigo de programación en c#,
el código debe ser legible y bien estructurado, usar de patrones conocidos,
buenas practicas de programcion, seguridad, etc., 
Sea muy conciso y directo en su respuesta";

            PromptScripEditorCompletar = "Completa el siguiente codigo";
            PromptScripEditorEncuentraErrores = "Encuentra errores en el siguiente codigo";
            PromptScripEditorAgregarTest = "Crea la prueba unitaria en nunit del siguiente codigo";
            PromptScripEditorOptimizar = "Optimiza el siguiente codigo";
            PromptScripEditorComentar = "Comenta el siguiente codigo";
            PromptScripEditorResumen = "Crea un resumen en el siguiente codigo";
            PromptScripEditorResumenClase = "Crea un resume de la clase en el siguiente codigo";

            //Haga una vista de código de los siguientes cambios e indique si hay algo que pueda o deba modificarse teniendo en cuenta los siguientes puntos: rendimiento, código legible y bien estructurado, uso de patrones conocidos, seguridad, posibles errores que deben eliminarse, etc. Si no hay ninguna recomendación para un punto, ignórelo; de lo contrario, diga lo que está mal y proporcione sugerencias, incluidos ejemplos de código. Sea muy conciso y directo en su respuesta, proporcione la revisión del código de manera muy resumida.
            //Según los cambios, escriba un comentario conciso que pueda usar en el archivo Léame y/o en el comentario push de cambios de git. Escribe cada cambio en una nueva línea. Utilice el formato Markdown.
            //Aplicar el cambio solicitado por el usuario al código, pero reescribir el código original que no fue modificado
        }

        /// <summary>
        /// Método estático que proporciona una instancia única de la clase WooWConfigParams.
        /// Implementa el patrón Singleton para asegurar que solo haya una instancia de la clase.
        /// Si la instancia no existe, se crea de manera segura en un bloque de bloqueo.
        /// Además, se intenta leer los valores de configuración; si falla, se graban los valores por defecto.
        /// </summary>
        /// <returns>
        /// Retorna la instancia única de WooWConfigParams.
        /// </returns>
        public static WooWConfigParams getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (padlock)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new WooWConfigParams();
                        if (!uniqueInstance.LeeValores())
                            uniqueInstance.GrabaValores();
                    }
                }
            }
            return uniqueInstance;
        }

        /// <summary>
        /// Método estático que limpia el estado del objeto único.
        /// Utiliza un bloqueo para asegurar que solo un hilo pueda acceder a la sección crítica a la vez.
        /// Si la instancia única no es nula, se limpia el contenido de 'ProyectoConPaquetes'
        /// y se establece 'uniqueInstance' a null, permitiendo así la recolección de basura.
        /// </summary>
        /// <summary>
        /// Método estático que limpia la instancia única de la clase y borra los paquetes del proyecto.
        /// Utiliza un bloqueo para asegurar que la operación es segura en entornos multihilo.
        /// Si la instancia única no es nula, se procede a limpiar los        /// <summary>
        /// Lee los valores de un archivo de configuración en formato JSON.
        /// Verifica si el archivo existe, lo deserializa y copia sus valores a la instancia actual.
        /// </summary>
        /// <returns>
        /// Devuelve true si el archivo fue leído y los valores fueron copiados exitosamente; de lo contrario, devuelve false.
        /// </returns>
        /// datos y se establece la instancia a nula.
        /// </summary>
        public static void Clear()
        {
            lock (padlock)
            {
                if (uniqueInstance != null)
                {
                    ProyectoConPaquetes.Clear();
                    uniqueInstance = null;
                }
            }
        }

        /// <summary>
        /// Lee los valores de un archivo de configuración en formato JSON.
        /// Verifica si el archivo existe, lo deserializa y copia sus propiedades a la instancia actual.
        /// </summary>
        /// <returns>
        /// Devuelve true si el archivo fue leído y los valores fueron copiados exitosamente; de lo contrario, devuelve false.
        /// </returns>
        public bool LeeValores()
        {
            var fileConfig = GetFileConfig();
            if (File.Exists(fileConfig))
            {
                // Lee el archivo de configuracion
                var json = System.IO.File.ReadAllText(fileConfig);

                // Deserializa el archivo de configuracion
                var config = JsonConvert.DeserializeObject<WooWConfigParams>(json);

                // Copia los valores de la clase deserializada a la clase actual
                foreach (var prop in typeof(WooWConfigParams).GetProperties())
                {
                    prop.SetValue(this, prop.GetValue(config));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Método que valida la clase actual, serializa sus valores a formato JSON
        /// y guarda el resultado en un archivo de configuración.
        /// </summary>
        public void GrabaValores()
        {
            // Valida la clase
            Valida();

            // Serializa la clase a Json
            var json = JsonConvert.SerializeObject(this);

            // Obtiene el archivo de configuracion
            var configFile = GetFileConfig();

            // Guarda el archivo de configuracion
            System.IO.File.WriteAllText(configFile, json);
        }

        /// <summary>
        /// Obtiene la ruta del archivo de configuración de la aplicación.
        /// Este método construye la ruta del archivo de configuración en formato JSON,
        /// utilizando el directorio donde se está ejecutando la aplicación y el nombre
        /// del ejecutable sin su extensión.
        /// </summary>
        /// <returns>
        /// Retorna la ruta completa del archivo de configuración en formato JSON.
        /// </returns>
        private string GetFileConfig()
        {
            // Obtiene el path donde se esta executando la aplicacion
            var path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            // Obtiene el nombre del archivo de la aplicacion
            var fileName = System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath);

            // crea el archivo de configuracion agregando punto y config, usa extension json
            return System.IO.Path.Combine(path, fileName + ".config.json");
        }

        private bool Valida()
        {
            // Valida usando una expresion regular que sea de 2 caracteres, mayusculas o numeros
            if (!Regex.IsMatch(Origen, @"^[A-Z][a-z0-9]$"))
                throw new Exception(
                    $"Origen <{Origen}> debe de ser de 2 caracteres, comenzar con una letra mayúsculas y seguir con una letra minuscula o número"
                );

            // Implement your validation logic here
            return true; // or false based on validation
        }
    }
}
