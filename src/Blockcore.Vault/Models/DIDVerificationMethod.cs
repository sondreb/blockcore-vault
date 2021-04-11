using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class DIDVerificationMethod
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonPropertyName("controller")]
        [JsonProperty("controller")]
        public string Controller { get; set; }

        [JsonPropertyName("publicKeyBase58")]
        [JsonProperty("publicKeyBase58")]
        public string PublicKeyBase58 { get; set; }
    }
}
