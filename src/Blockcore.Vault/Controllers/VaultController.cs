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

            var items = store.GetItems<VaultServer>("Name", (validFilter.PageNumber - 1) * validFilter.PageSize, validFilter.PageSize);
            var totalRecords = store.GetCount<VaultServer>();
            var pagedReponse = PaginationHelper.CreatePagedReponse(items, validFilter, totalRecords, uriService, Request.Path.Value);

            return Ok(pagedReponse);
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            return Ok(store.GetItem<VaultServer>(id));
        }

        [HttpPut("{id}")]
        public ActionResult Put(string id)
        {
            return Ok(store.GetItem<VaultServer>(id));
        }
    }
}
