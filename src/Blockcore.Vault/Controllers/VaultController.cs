using Blockcore.Vault.Filters;
using Blockcore.Vault.Helpers;
using Blockcore.Vault.Models;
using Blockcore.Vault.Services;
using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Controllers
{
    [ApiController]
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

        [HttpPost]
        public ActionResult Post([FromBody] VaultServer item)
        {
            store.InsertItem(item);
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
