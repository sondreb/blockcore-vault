using Newtonsoft.Json;
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
        [JsonProperty("@context")]
        public string Context { get; set; }

        [JsonPropertyName("linked_dids")]
        [JsonProperty("linked_dids")]
        public object[] LinkedIdentities { get; set; }
    }
}
