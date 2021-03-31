using Dapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Storage
{
    public class DataStore
    {
        private readonly DatabaseRepository db;

        public DataStore(DatabaseRepository db)
        {
            this.db = db;
        }
     
        public void GetAll()
        {
            db.GetForAddress("");
        }
    }
}
