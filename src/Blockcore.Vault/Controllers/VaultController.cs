using Blockcore.Vault.Authorization;
using Blockcore.Vault.Filters;
using Blockcore.Vault.Helpers;
using Blockcore.Vault.Models;
using Blockcore.Vault.Services;
using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blockcore.Vault.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/vault")]
    public class VaultController : ControllerBase
    {
        private readonly IMoney money;
        private readonly IDatabaseConnectionFactory db;
        private readonly DatabaseRepository store;
        private readonly IUriService uriService;

        public VaultController(IMoney money, IDatabaseConnectionFactory db, DatabaseRepository store, IUriService uriService)
        {
            this.money = money;
            this.db = db;
            this.store = store;
            this.uriService = uriService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetVaultServersAsync([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            //var items = store.GetItems<VaultServer>("Name", (validFilter.PageNumber - 1) * validFilter.PageSize, validFilter.PageSize);
            var items = store.GetItems<VaultServer>("Name", validFilter.PageNumber, validFilter.PageSize);

            var totalRecords = store.GetCount<VaultServer>();
            var pagedReponse = PaginationHelper.CreatePagedReponse(items, validFilter, totalRecords, uriService, Request.Path.Value);

            return Ok(pagedReponse);
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var item = store.GetItem<VaultServer>(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        public DIDDocument ResolveDidDocument(string did)
        {
            var file = System.IO.File.ReadAllText(Path.Combine("Data", "did-document-resolution.json"));

            var didDocumentResolution = JsonConvert.DeserializeObject<DIDDocumentResolution>(file);

            if (didDocumentResolution.Context != "https://w3id.org/did-resolution/v1")
            {
                throw new VerifiableCredentialException($"The context of the resolved document resolution is not correct, should be https://w3id.org/did-resolution/v1 and was {didDocumentResolution.Context}.");
            }

            // TODO: Verify the signature of the DIDDocument itself using the metadata.
            // We want to perform a validation of the DID Document itself to ensure that the keys provided in the DID Document, is valid and signed.

            // The consumer of DID Documents only needs the validated DID Document.
            return didDocumentResolution.DIDDocument;
        }

        public DomainLinkageCredential ParseJwtVC<T>(string data)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            //Check if readable token (string is in a JWT format)
            var readableToken = handler.CanReadToken(data);

            if (!readableToken)
            {
                throw new VerifiableCredentialException("The token doesn't seem to be in a proper JWT format.");
            }

            var token = handler.ReadJwtToken(data);

            // Resolve DID Document based upon the DID.

            var issuerDid = token.Issuer;

            // https://identity.foundation/.well-known/resources/did-configuration/#did-configuration-resource-verification
            var didDocument = ResolveDidDocument(issuerDid);

            // We must loop all keys and verify, as the DID might contain multiple keys.
            // TODO: Add some limit to amount of keys to avoid processing problematic DID Documents that has spam-keys.

            // From W3C specification:
            // If an entry fails validation during an iteration of the entries by the processing entity, there is no normative implication about the validity of other entries,
            // and the choice to continue iteration and validation of further entries is at the election of the processing entity.

            TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    //ValidIssuer = Jwt.Issuer,
                    //ValidAudience = Jwt.Audience,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,

                    IssuerSigningKeys = didDocument.GetSecurityKeys(),
                    ValidateIssuerSigningKey = true,

                    //Needed to force disabling signature validation
                    //SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                    //{
                    //    var jwt = new JwtSecurityToken(token);
                    //    return jwt;
                    //},
                };
            //}

            //If we succcessfully loaded metadata we do signature validation as well
            //if (metadataAvailable)
            //{
            //    validationParameters =
            //        new TokenValidationParameters
            //        {
            //            ValidIssuer = openIdConfig.Issuer,
            //            ValidAudience = Jwt.Audience,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidateAudience = true,
            //            ValidateIssuer = true,
            //            IssuerSigningKeys = openIdConfig.SigningKeys
            //        };
            //}

            try
            {
                var identity = handler.ValidateToken(data, validationParameters, out SecurityToken validatedToken);

                return new DomainLinkageCredential();

                //if (metadataAvailable)
                //{
                //    output += "Token is valid according to metadata!";
                //}
                //else
                //{
                //    output += "Token is valid according to a self-evaluation!";
                //}

                // jwtSignature = token.RawSignature;

            }
            catch (Exception e)
            {
                //Due to a bug in the AAD IdentityModel extension we need custom handling to print out the attributes in the error message.
                var customMessage = string.Empty;

                if (e.Message.Contains("IDX10223"))
                {
                    customMessage = $"Time values not valid. Current time: {DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToLongTimeString()}. Token valid from: '{token.ValidFrom.ToShortDateString()} {token.ValidFrom.ToLongTimeString()}' Token valid to: '{token.ValidTo.ToShortDateString()} {token.ValidTo.ToLongTimeString()}'";
                }
                if (e.Message.Contains("IDX10214"))
                {
                    customMessage = $"Incorrect Audience. Accepted audience '{validationParameters.ValidateAudience}'. Audience in token '{token.Audiences.FirstOrDefault()}'.";
                }
                if (e.Message.Contains("IDX10205"))
                {
                    customMessage = $"Incorrect Issuer. Accepted issuer '{validationParameters.ValidateIssuer}'. Issuer in token '{token.Issuer}.'";
                }
                //Unable to obtain metadata
                if (e.Message.Contains("IDX20803"))
                {
                    customMessage = $"Unable to obtain configuration.";
                }
                //Signature fail
                if (e.Message.Contains("IDX10501"))
                {
                    customMessage = $"Signature validation failed. Unable to match key: {token.Header.Kid}";
                }
                //Signature fail
                if (e.Message.Contains("IDX10508"))
                {
                    customMessage = "Signature validation failed. Signature is improperly formatted.";
                }
                else if (customMessage == string.Empty)
                {
                    customMessage = e.Message;
                }

                // output += $"Token failed to validate. {Environment.NewLine}{customMessage}";
            }

            return null;
        }

        //public static string Decode(string token, string key, bool verify)
        //{
        //    var parts = token.Split('.');
        //    var header = parts[0];
        //    var payload = parts[1];
        //    // byte[] crypto = Base64UrlDecode(parts[2]);
        //    var jwtSignature = parts[2];

        //    var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
        //    var headerData = JObject.Parse(headerJson);
        //    var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
        //    var payloadData = JObject.Parse(payloadJson);

        //    if (verify)
        //    {
        //        var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
        //        var keyBytes = Encoding.UTF8.GetBytes(key);
        //        var algorithm = (string)headerData["alg"];
        //        var computedJwtSignature = Encoding.UTF8.GetString(HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign));
        //        // var decodedCrypto = Convert.ToBase64String(crypto);
        //        // var decodedSignature = Convert.ToBase64String(signature);

        //        if (jwtSignature != computedJwtSignature)
        //        {
        //            throw new ApplicationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
        //        }
        //    }

        //    return payloadData.ToString();
        //}


        [HttpPost]
        public ActionResult Post([FromBody] DIDConfiguration configuration)
        {
            string context = "https://identity.foundation/.well-known/did-configuration/v1";

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (configuration.Context != context)
            {
                return BadRequest($"Invalid context, must be: {context}");
            }

            foreach (JsonElement json in configuration.LinkedIdentities)
            {
                // Parse if the linked identity is JSON-VC or JWT-VC
                if (json.ValueKind == JsonValueKind.String)
                {
                    var vc = ParseJwtVC<DomainLinkageCredential>(json.GetString());
                }
                else
                {
                    // var vc = ParseJsonVC<DomainLinkageCredential>(data);
                }


                //if (string.IsNullOrWhiteSpace(identity))
                //{
                //    continue;
                //}

                //var data = identity.Trim();

                //// Parse if the linked identity is JSON-VC or JWT-VC
                //if (data.Substring(1) == "{")
                //{

                //}
                //else
                //{

                //}
            }

            store.InsertItem(configuration);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Put([FromBody] VaultServer item)
        {
            store.UpdateItem(item);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var item = store.GetItem<VaultServer>(id);

            if (item == null)
            {
                throw new ArgumentNullException($"Item not found with id: {id}.");
            }

            store.DeleteItem(item);

            return Ok(id);
        }
    }
}
