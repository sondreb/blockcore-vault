using Blockcore.Vault.Storage;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.Vault.Tests.Storage
{
    public class InMemoryDatabaseFactory : IDatabaseFactory
    {
        private readonly string dbConnection;

        /// <summary>
        /// A connection used only when the data store is in memory only.
        /// SQLite in-memory mode will keep the db as long as there is one connection open.
        /// https://github.com/dotnet/docs/blob/master/samples/snippets/standard/data/sqlite/InMemorySample/Program.cs
        /// /// </summary>
        private readonly SqliteConnection inmemorySqliteConnection;

        public InMemoryDatabaseFactory()
        {
            var tmpconn = Guid.NewGuid().ToString();
            this.dbConnection = $"Data Source={tmpconn};Mode=Memory;Cache=Shared";
            this.inmemorySqliteConnection = new SqliteConnection(this.dbConnection);
            this.inmemorySqliteConnection.Open();
        }

        public SqliteConnection CreateConnection()
        {
            return this.inmemorySqliteConnection;
        }
    }
}
