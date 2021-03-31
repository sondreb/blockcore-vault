using Blockcore.Vault.Storage;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.Vault.Tests.Storage
{
    public class InMemoryDatabaseConnection : IDatabaseConnection
    {
        public SqliteConnection Connection { get; set; }

        public void Dispose()
        {
            
        }
    }
}
