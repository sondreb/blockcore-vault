using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class VaultData
    {
        public int Id { get; set; }

        public int DatabaseVersion { get; set; }

        /// <summary>
        /// This is the identity of the current vault server instance and is used to retrieve the correct .well-known configuration from the VaultServer table.
        /// </summary>
        public string Identity { get; set; }
    }
}
