using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class DIDConfiguration
    {
        [JsonPropertyName("@context")]
        public string[] Context { get; set; }

        [JsonPropertyName("linked_dids")]
        public Identity[] LinkedIdentities { get; set; }
    }
}
