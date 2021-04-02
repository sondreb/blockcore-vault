using Blockcore.Vault.Helpers;
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
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        private readonly IMoney money;

        private readonly IDatabaseConnectionFactory db;

        private readonly DatabaseRepository store;

        public DataController(IMoney money, IDatabaseConnectionFactory db, DatabaseRepository store)
        {
            this.money = money;
            this.db = db;
            this.store = store;
        }

        [HttpGet("list")]
        public ActionResult GetList()
        {
            // store.GetVaultServer();

            var conn = db.CreateConnection();

            conn.Connection.Open();
            conn.Connection.Close();

            return Ok(1);
        }

        //[HttpGet("items")]
        //public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter filter)
        //{
        //    var route = Request.Path.Value;
        //    var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

        //    var items = store.GetItemData((validFilter.PageNumber - 1) * validFilter.PageSize, validFilter.PageSize);

        //    //var pagedData = await context.Customers
        //    //   .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        //    //   .Take(validFilter.PageSize)
        //    //   .ToListAsync();

        //    var totalRecords = store.GetItemDataCount(); //await context.Customers.CountAsync();

        //    var pagedReponse = PaginationHelper.CreatePagedReponse<ItemData>(items, validFilter, totalRecords, uriService, route);

        //    return Ok(pagedReponse);

        //    //store.GetAll();

        //    //return Ok(1);
        //}

        [AllowAnonymous]
        [HttpGet("all")]
        public ActionResult GetAll()
        {
            return Ok(money.GetAll());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult Get(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest(ModelState);
            }

            return Ok(true);

            // return Ok(_employeeRepository.GetEmployeeByID(id));
        }
    }
}
