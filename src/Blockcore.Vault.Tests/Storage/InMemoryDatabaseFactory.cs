using Blockcore.Vault.Storage;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.Vault.Tests.Storage
{
    public class InMemoryDatabaseFactory : IDatabaseConnectionFactory, IDisposable
    {
        private readonly string dbConnection;

        /// <summary>
        /// A connection used only when the data store is in memory only.
        /// SQLite in-memory mode will keep the db as long as there is one connection open.
        /// https://github.com/dotnet/docs/blob/master/samples/snippets/standard/data/sqlite/InMemorySample/Program.cs
        /// /// </summary>
        private readonly SqliteConnection inmemorySqliteConnection;

        private readonly InMemoryDatabaseConnection connection;

        public InMemoryDatabaseFactory()
        {
            var tmpconn = Guid.NewGuid().ToString();
            dbConnection = $"Data Source={tmpconn};Mode=Memory;Cache=Shared";

            connection = new InMemoryDatabaseConnection { Connection = new SqliteConnection(this.dbConnection) };

            // inmemorySqliteConnection = new SqliteConnection(this.dbConnection);
            connection.Connection.Open();
            // inmemorySqliteConnection.Open();
        }

        public bool Persistent => false;

        public IDatabaseConnection CreateConnection()
        {
            return connection;
        }

        public void Dispose()
        {
            this.inmemorySqliteConnection?.Dispose();
        }

        public void SetConnection(string connection)
        {
            
        }
    }
}
