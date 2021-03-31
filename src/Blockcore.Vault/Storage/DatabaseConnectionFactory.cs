using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Storage
{
    public interface IDatabaseConnection: IDisposable
    { 
        public SqliteConnection Connection { get; set; }
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        public SqliteConnection Connection { get; set; }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }

    public interface IDatabaseConnectionFactory
    {
        IDatabaseConnection CreateConnection();

        void SetConnection(string connection);

        bool Persistent { get; }
    }

    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private string dbConnection;

        public bool Persistent => true;

        public IDatabaseConnection CreateConnection()
        {
            return new DatabaseConnection { Connection = new SqliteConnection(this.dbConnection) };
        }

        public void SetConnection(string connection)
        {
            this.dbConnection = connection;
        }
    }
}
