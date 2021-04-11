using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class DIDDocumentResolution
    {
        [JsonPropertyName("@context")]
        [JsonProperty("@context")]
        public string Context => "https://w3id.org/did-resolution/v1";

        [JsonPropertyName("didDocument")]
        [JsonProperty("didDocument")]
        public DIDDocument DIDDocument { get; set; }

        [JsonPropertyName("didDocumentMetadata")]
        [JsonProperty("didDocumentMetadata")]
        public DIDDocumentMetadata DIDDocumentMetadata { get; set; }
    }
}
