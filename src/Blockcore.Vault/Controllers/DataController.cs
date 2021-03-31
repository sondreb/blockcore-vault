using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        private readonly IMoney money;

        private readonly IDatabaseFactory db;

        public DataController(IMoney money, IDatabaseFactory db)
        {
            this.money = money;
            this.db = db;
        }

        [HttpGet("list")]
        public ActionResult GetList()
        {
            var conn = db.CreateConnection();

            conn.Open();
            conn.Close();

            return Ok(1);
        }

        [HttpGet("all")]
        public ActionResult GetAll()
        {
            return Ok(money.GetAll());
        }

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
