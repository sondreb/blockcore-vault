using Blockcore.Vault.Models;
using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Produces("application/json")]
    [Route(".well-known")]
    public class WellKnownController : ControllerBase
    {
        private readonly DatabaseRepository store;

        public WellKnownController(DatabaseRepository store)
        {
            this.store = store;
        }

        [AllowAnonymous]
        [Swashbuckle.AspNetCore.Annotations.SwaggerOperation()]
        [HttpGet("vault-configuration.json")]
        public async Task<IActionResult> GetConfiguration()
        {
            var item = store.GetItem<VaultServer>(store.VaultData.Identity);

            if (item == null)
            {
                throw new ArgumentNullException("You must complete the setup.");
            }

            return Ok(item.WellKnownConfiguration);
        }

        [HttpGet("did.json")]
        public async Task<IActionResult> GetIdentity()
        {
            var did = new DID();

            did.Context = new string[] { "https://www.w3.org/ns/did/v1" };
            did.Id = "did:web:dv1.blockcore.net";

            return Ok(did);
        }

        [HttpGet("did-configuration.json")]
        public async Task<IActionResult> GetIdentityConfiguration()
        {
            var did = new DIDConfiguration();

            did.Context = new string[] { "https://identity.foundation/.well-known/did-configuration/v1" };

            return Ok(did);
        }
    }
}
