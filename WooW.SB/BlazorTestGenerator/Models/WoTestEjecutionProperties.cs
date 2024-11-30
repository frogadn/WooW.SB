namespace WooW.SB.BlazorTestGenerator.Models
{
    public enum eEjecutionType
    {
        Server,
        Client
    }

    public class WoTestEjecutionProperties
    {
        /// <summary>
        /// Nombre de la prueba
        /// </summary>
        public string TestName { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la prueba
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Ruta de la prueba
        /// </summary>
        public string TestPath { get; set; } = string.Empty;

        /// <summary>
        /// Tipo en el que se basa el test
        /// </summary>
        public string TestType { get; set; } = string.Empty;

        /// <summary>
        /// Status actual de la prueba
        /// </summary>
        public bool Status { get; set; } = false;

        /// <summary>
        /// Tipo de ejecución
        /// </summary>
        public eEjecutionType EjecutionType { get; set; }
    }
}
