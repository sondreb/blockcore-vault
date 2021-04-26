using Blockcore.Vault.Filters;
using Blockcore.Vault.Helpers;
using Blockcore.Vault.Models;
using Blockcore.Vault.Services;
using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("/api/sync")]
    [ApiExplorerSettings(GroupName = "Sync")]
    public class SyncController : ControllerBase
    {
        private readonly DatabaseRepository store;
        private readonly IUriService uriService;

        public SyncController(DatabaseRepository store, IUriService uriService)
        {
            this.store = store;
            this.uriService = uriService;
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var items = store.GetItemData((validFilter.PageNumber - 1) * validFilter.PageSize, validFilter.PageSize);

            //var pagedData = await context.Customers
            //   .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            //   .Take(validFilter.PageSize)
            //   .ToListAsync();

            var totalRecords = store.GetItemDataCount(); //await context.Customers.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ItemData>(items, validFilter, totalRecords, uriService, route);

            return Ok(pagedReponse);

            //store.GetAll();

            //return Ok(1);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = store.GetSingleItemData(id);
            // var customer = await context.Customers.Where(a => a.Id == id).FirstOrDefaultAsync();
            return Ok(new Response<ItemData>(item));
        }
    }
}
