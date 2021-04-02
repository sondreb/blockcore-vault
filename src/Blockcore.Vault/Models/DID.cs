using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class DID
    {
        [JsonPropertyName("@context")]
        public string[] Context { get; set; }

        public string Id { get; set; }
    }
}
