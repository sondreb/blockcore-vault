using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class DIDDocumentMetadata
    {
        [JsonPropertyName("header")]
        [JsonProperty("header")]
        public dynamic Header { get; set; }

        [JsonPropertyName("signature")]
        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("data")]
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonPropertyName("recovery")]
        [JsonProperty("recovery")]
        public dynamic Recovery { get; set; }

        [JsonPropertyName("update")]
        [JsonProperty("update")]
        public dynamic Update { get; set; }

        [JsonPropertyName("method")]
        [JsonProperty("method")]
        public dynamic Method { get; set; }

        [JsonPropertyName("equivalentId")]
        [JsonProperty("equivalentId")]
        public dynamic EquivalentId { get; set; }
    }
}
