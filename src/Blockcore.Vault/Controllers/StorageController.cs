using Blockcore.Indexer.Storage.Mongo;
using Blockcore.Vault.Helpers;
using Blockcore.Vault.Models;
using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Controllers
{
    public class DataRequest
    { 
        public string Operation { get; set; }

        public string Payload { get; set; }
    }

    [AllowAnonymous]
    [ApiController]
    [Produces("application/json")]
    [Route("api/storage")]
    [ApiExplorerSettings(GroupName = "Storage")]
    public class StorageController : ControllerBase
    {
        private readonly IMoney money;

        private readonly IDatabaseConnectionFactory db;

        private readonly DatabaseRepository store;

        private readonly MongoData data;

        public StorageController(IMoney money, IDatabaseConnectionFactory db, DatabaseRepository store, MongoData data)
        {
            this.money = money;
            this.db = db;
            this.store = store;
            this.data = data;
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

        [HttpPost]
        public ActionResult Post([FromBody] DataRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jwt = request.Payload.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (jwt.Length != 3)
            {
                throw new VerifiableCredentialException("The submitted VC is not in correct JWT format.");
            }

            if (request.Operation != "create")
            {
                throw new VerifiableCredentialException("The operation only supports the create operation.");
            }

            var vc = new VerifiableCredential();
            vc.Id = jwt[2]; // The signature is the identifier.
            vc.Proof = new JwtProof2020 { Jwt = request.Payload };

            // If the same JWT is supplied, we will crash and not update the existing one. It should not be possible to update existing JWTs.
            data.VerifiableCredential.InsertOne(vc);

            return Ok();
        }
    }
}
