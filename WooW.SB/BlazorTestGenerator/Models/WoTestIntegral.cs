using System.Collections.Generic;

namespace WooW.SB.BlazorTestGenerator.Models
{
    public class WoTestIntegral
    {
        /// <summary>
        /// Nombre de la prueba integral
        /// </summary>
        public string IntegralName { get; set; } = string.Empty;

        /// <summary>
        /// Description de la prueba integral
        /// </summary>
        public string IntegralDescription { get; set; } = string.Empty;

        /// <summary>
        /// Grupo al que pertenecen las clases principales
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Status de ejecución de las pruebas
        /// </summary>
        public bool TestStatus { get; set; } = false;

        /// <summary>
        /// Colección de pruebas que conforman la prueba integral
        /// </summary>
        public List<WoTestEjecutionProperties> TestCol { get; set; } =
            new List<WoTestEjecutionProperties>();
    }
}
