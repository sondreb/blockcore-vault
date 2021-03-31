using Blockcore.Vault.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Storage
{
    public class DatabaseRepository
    {
        private const int WalletVersion = 1;
        private string dbConnection;
        private string dbPath;
        private readonly IDatabaseConnectionFactory db;

        public VaultData VaultData { get; private set; }

        public DatabaseRepository(IDatabaseConnectionFactory db)
        {
            this.db = db;

            Init();
            InitData();
        }

        private void Init()
        {
            // TODO: Add reading of database connection to settings.
            var dataFolder = "data";
            dbPath = Path.Combine(dataFolder, $"vault.db");
            dbConnection = "Data Source=" + this.dbPath;

            db.SetConnection(dbConnection);

            if (db.Persistent)
            {
                if (!Directory.Exists(dataFolder))
                {
                    Directory.CreateDirectory(dataFolder);
                }

                if (!File.Exists(this.dbPath))
                {
                    this.CreateDatabase();
                }
                else
                {
                    // Attempt to access the user version, this will crash if the loaded database is V5 and we use V4 packages.
                    try
                    {
                        var walletVersion = -1;

                        using (var conn = db.CreateConnection())
                        {
                            walletVersion = conn.Connection.QueryFirst<int>("SELECT DatabaseVersion FROM VaultData");
                        }

                        if (walletVersion != WalletVersion)
                        {
                            this.UpgradeDatabase(walletVersion);
                        }
                    }
                    catch (Microsoft.Data.Sqlite.SqliteException sqex)
                    {
                        // Errror that indicates that the file being opened does not appear to be an SQLite database file.
                        if (sqex.SqliteErrorCode != 26)
                            throw;

                        // This will make a backup copy of the old litedbv5 (or v4) databases.
                        // The reason the code base moved to use sqlite instead of litedb is because litedbv5 is not
                        // properly maintained anymore and has a critical hard to reproduce errors that happen randomly.
                        var dbBackupPath = Path.Combine(dataFolder, $"vault.{DateTime.UnixEpoch}.error.db");

                        // Move the problematic database file, which might be a V5 database.
                        File.Move(this.dbPath, dbBackupPath);

                        this.CreateDatabase();
                    }
                }
            }
            else
            {
                this.CreateDatabase();
            }
        }

        private void UpgradeDatabase(int oldVersion)
        {
            // Here can come code to upgrade the db from old to current version.
        }

        private void CreateDatabase()
        {
            using (var conn = db.CreateConnection())
            {
                // Make sure we only open and close for persistent database, the in-memory database must not close.
                if (db.Persistent)
                {
                    conn.Connection.Open();
                }

                conn.Connection.Execute(
                   @$"CREATE TABLE VaultData(
               Id            VARCHAR(3) NOT NULL PRIMARY KEY,
               EncryptedSeed VARCHAR(500) NULL,
               WalletName    VARCHAR(100) NULL,
               WalletTip     VARCHAR(75) NULL,
               DatabaseVersion INTEGER NOT NULL,
               BlockLocator  TEXT NULL)");

                conn.Connection.Execute(
                    @$"CREATE TABLE TransactionData(
                OutPoint                                           VARCHAR(66) NOT NULL PRIMARY KEY,
                Address                                            VARCHAR(34) NOT NULL,
                Id                                                 VARCHAR(64) NOT NULL,
                Amount                                             INTEGER  NOT NULL,
                IndexInTransaction                                 INTEGER  NOT NULL,
                BlockHeight                                        INTEGER  NULL,
                BlockHash                                          VARCHAR(64) NULL,
                BlockIndex                                         INTEGER NULL,
                CreationTime                                       INTEGER  NOT NULL,
                ScriptPubKey                                       VARCHAR(100) NOT NULL,
                IsPropagated                                       INTEGER  NULL,
                IsCoinBase                                         INTEGER  NULL,
                IsCoinStake                                        INTEGER  NULL,
                IsColdCoinStake                                    INTEGER  NULL,
                AccountIndex                                       INTEGER  NOT NULL,
                MerkleProof                                        TEXT NULL,
                Hex                                                TEXT NULL,
                SpendingDetailsTransactionId                       VARCHAR(64) NULL,
                SpendingDetailsBlockHeight                         INTEGER  NULL,
                SpendingDetailsBlockIndex                          INTEGER  NULL,
                SpendingDetailsIsCoinStake                         INTEGER  NULL,
                SpendingDetailsCreationTime                        INTEGER  NULL,
                SpendingDetailsPayments                            TEXT NULL,
                SpendingDetailsHex                                 TEXT NULL)");

                conn.Connection.Execute("CREATE INDEX 'address_index' ON 'TransactionData' ('Address')");
                conn.Connection.Execute("CREATE INDEX 'blockheight_index' ON 'TransactionData' ('BlockHeight')");
                conn.Connection.Execute("CREATE UNIQUE INDEX 'outpoint_index' ON 'TransactionData' ('OutPoint')");
                conn.Connection.Execute("CREATE UNIQUE INDEX 'key_index' ON 'VaultData' ('Id')");

                if (db.Persistent)
                {
                    conn.Connection.Close();
                }
            }
        }

        private void InitData()
        {
            //SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
            //SqlMapper.AddTypeHandler(new HashHeightPairHandler());
            //SqlMapper.AddTypeHandler(new CollectionOfuint256Handler());
            //SqlMapper.AddTypeHandler(new Uint256Handler());
            //SqlMapper.AddTypeHandler(new MoneyHandler());
            //SqlMapper.AddTypeHandler(new OutPointHandler());
            //SqlMapper.AddTypeHandler(new ScriptHandler());
            //SqlMapper.AddTypeHandler(new CollectionOfPaymentDetailsHandler());
            //SqlMapper.AddTypeHandler(new PartialMerkleTreeHandler());

            this.VaultData = this.GetData();

            if (this.VaultData != null)
            {
                //if (this.VaultData.EncryptedSeed != VaultData.EncryptedSeed)
                //{
                //    throw new WalletException("Invalid Wallet seed");
                //}
            }
            else
            {
                this.SetData(new VaultData
                {
                    Id = 1,
                    //Key = "Key",
                    //EncryptedSeed = wallet.EncryptedSeed,
                    //WalletName = wallet.Name,
                    //WalletTip = new HashHeightPair(this.network.GenesisHash, 0),
                    DatabaseVersion = WalletVersion
                });
            }
        }

        public void SetData(VaultData data)
        {
            var sql = @$"INSERT INTO VaultData
                      (Id, DatabaseVersion)
                      VALUES (@Id, @DatabaseVersion)
                      ON CONFLICT(Id) DO UPDATE SET
                      DatabaseVersion = @DatabaseVersion;";

            using var conn = db.CreateConnection();
            conn.Connection.Execute(sql, data);

            this.VaultData = data;
        }

        public VaultData GetData()
        {
            if (this.VaultData == null)
            {
                using var conn = db.CreateConnection();
                this.VaultData = conn.Connection.QueryFirstOrDefault<VaultData>("SELECT * FROM VaultData");
            }

            return this.VaultData;
        }

        //public SqliteConnection CreateConnection()
        //{
        //    return new SqliteConnection(this.dbConnection);
        //}

        public IEnumerable<dynamic> GetForAddress(string address)
        {
            using var conn = db.CreateConnection();

            var trxs = conn.Connection.Query<dynamic>(
                "SELECT * FROM TransactionData " +
                "WHERE Address = @address",
                new { address });

            return trxs;
        }

        //public IEnumerable<TransactionOutputData> GetUnspentForAddress(string address)
        //{
        //    using var conn = this.GetDbConnection();
        //    var trxs = conn.Query<TransactionData>(
        //        "SELECT * FROM 'TransactionData' " +
        //        "WHERE Address = @address " +
        //        "AND SpendingDetailsTransactionId IS NULL",
        //        new { address });

        //    return trxs.Select(this.Convert);
        //}
    }
}
