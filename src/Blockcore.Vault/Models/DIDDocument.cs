using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class DIDService
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        [MaxLength(20)] // Same requirements as the Sidetree Protocol Specification: https://github.com/ownyourdata/did-ion
        public string Id { get; set; }

        [JsonPropertyName("type")]
        [JsonProperty("type")]
        [MaxLength(30)]
        public string Type { get; set; }

        [JsonPropertyName("serviceEndpoint")]
        [JsonProperty("serviceEndpoint")]
        public string ServiceEndpoint { get; set; }
    }

    public class DIDDocument
    {
        [JsonPropertyName("@context")]
        [JsonProperty("@context")]
        public string[] Context { get; set; }

        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonPropertyName("service")]
        [JsonProperty("service")]
        public List<DIDService> Services { get; set; }

        [JsonPropertyName("verificationMethod")]
        [JsonProperty("verificationMethod")]
        public List<DIDVerificationMethod> VerificationMethod { get; set; }

        [JsonPropertyName("assertionMethod")]
        [JsonProperty("assertionMethod")]
        public string[] AssertionMethod { get; set; }

        [JsonPropertyName("authentication")]
        [JsonProperty("authentication")]
        public string[] Authentication { get; set; }
    }
}
