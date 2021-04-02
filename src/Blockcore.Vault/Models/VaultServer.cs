using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public enum VaultServerState
    { 
        Offline = 0,
        Online = 1,
        Error = 2
    }
    
    public class VaultServer
    {
        /// <summary>
        /// The public key of a vault server.
        /// </summary>
        [Key]
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Indicates if the vault server should be synced with.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Name of the vault server. Set by the well-known configuration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// General description about the vault server. Set by the well-known configuration.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Url of the vault server.
        /// </summary>
        public string Url { get; set; }

        public long Created { get; set; }

        public long Modified { get; set; }

        public long LastSeen { get; set; }

        public long LastFullSync { get; set; }

        public string WellKnownConfiguration { get; set; }

        /// <summary>
        /// The last known state of the vault server.
        /// </summary>
        public VaultServerState State { get; set; }
    }
}
