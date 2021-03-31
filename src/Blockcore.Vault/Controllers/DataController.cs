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
        public DataController()
        {

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
