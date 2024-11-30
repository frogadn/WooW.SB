namespace WooW.SB.Deploy.Models
{
    /// <summary>
    /// Tipo del proyecto que se va a generar
    /// </summary>
    public enum eProyectType
    {
        Server,
        Wasm
    }

    public class WoDeployGenerationSettings
    {
        /// <summary>
        /// Nombre de la meta data que se recibió desde el cliente
        /// </summary>
        public string MetadataName { get; set; } = "Proyecto";

        /// <summary>
        /// Nombre del fichero principal .wwsb
        /// </summary>
        public string MetadataFileName { get; set; } = "Prueba";

        /// <summary>
        /// Nombre del fichero con la data de la generación integral
        /// </summary>
        public string IntegralDataName { get; set; } = "Test";

        /// <summary>
        /// Tipo del proyecto del que se realizara la generación
        /// </summary>
        public eProyectType ProyectType { get; set; } = eProyectType.Server;

        /// <summary>
        /// Nombre del docker donde desplegar
        /// </summary>
        public string Docker { get; set; } = string.Empty;
    }
}
