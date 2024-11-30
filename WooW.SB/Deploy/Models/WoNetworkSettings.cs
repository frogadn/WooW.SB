using System.ComponentModel;

namespace WooW.SB.Deploy.Models
{
    public class WoNetworkSettings
    {
        /// <summary>
        /// Identificador de la id
        /// </summary>
        [Browsable(false)]
        public string NetworkId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la red
        /// </summary>
        public string NetworkName { get; set; } = string.Empty;

        /// <summary>
        /// Numero de dockers desplegados en la red.
        /// </summary>
        [Browsable(false)]
        public int Dockers { get; set; } = 0;
    }
}
