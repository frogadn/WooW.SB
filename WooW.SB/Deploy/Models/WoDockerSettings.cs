namespace WooW.SB.Deploy.Models
{
    public class WoDockerSettings
    {
        /// <summary>
        /// Nombre del docker (contenedor)
        /// </summary>
        public string DockerName { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la imagen desplegada en el docker
        /// </summary>
        public string ImageName { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la red en la que se encuentra el docker
        /// </summary>
        public string NetworkName { get; set; } = string.Empty;

        /// <summary>
        /// Espacio de disco asignado al docker
        /// </summary>
        public int Size { get; set; } = 0;

        /// <summary>
        /// Puerto en el que escucha el docker
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// Cantidad de CPU asignada al docker
        /// </summary>
        public int CPU { get; set; } = 0;

        /// <summary>
        /// Cantidad de RAM asignada al docker
        /// </summary>
        public int RAM { get; set; } = 0;

        /// <summary>
        /// Indicador si el docker se encuentra en linea
        /// </summary>
        public bool EnLinea { get; set; } = false;
    }
}
