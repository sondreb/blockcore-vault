using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Storage
{
    public interface IDatabaseFactory
    {
        SqliteConnection CreateConnection();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly string dbConnection;

        public DatabaseFactory()
        {

        }

        public SqliteConnection CreateConnection()
        {
            return new SqliteConnection(this.dbConnection);
        }
    }
}
