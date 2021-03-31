using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Controllers
{

    [ApiController]
    [Route("/api/sync")]
    public class SyncController : ControllerBase
    {
        private readonly DataStore store;

        public SyncController(DataStore store)
        {
            this.store = store;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            store.GetAll();

            return Ok(1);
        }
    }
}
