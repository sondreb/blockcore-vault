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
                @$"CREATE TABLE VaultServer(
                   Id                      TEXT NOT NULL PRIMARY KEY,
                   Enabled                 INTEGER NOT NULL,
                   Name                    TEXT NULL,
                   Description             TEXT NULL,
                   Url                     TEXT NULL,
                   Added                   INTEGER NOT NULL,
                   Modified                INTEGER NOT NULL,
                   LastSeen                INTEGER NOT NULL,
                   LastFullSync            INTEGER NOT NULL,
                   State                   INTEGER NOT NULL,
                   WellKnownConfiguration  TEXT NULL)");

                conn.Connection.Execute(
                   @$"CREATE TABLE VaultData(
               Id            TEXT NOT NULL PRIMARY KEY,
               EncryptedSeed TEXT NULL,
               WalletName    TEXT NULL,
               WalletTip     TEXT NULL,
               DatabaseVersion INTEGER NOT NULL,
               BlockLocator  TEXT NULL)");

                conn.Connection.Execute(
                    @$"CREATE TABLE ItemData(
               Id       VARCHAR(64) NOT NULL PRIMARY KEY,
               Data     VARCHAR(500) NULL,
               Owner    VARCHAR(100) NULL)");

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

        public int GetItemDataCount()
        {
            using var conn = db.CreateConnection();

            return conn.Connection.QueryFirst<int>("SELECT COUNT(*) FROM ItemData");
        }

        public ItemData GetSingleItemData(int id)
        {
            using var conn = db.CreateConnection();

            var query = conn.Connection.Query<ItemData>(
                "SELECT * FROM TransactionData " +
                "WHERE Id = @id",
                new { id });

            return query.SingleOrDefault();
        }

        public List<T> GetItems<T>(string orderBy, int pageNumber = 0, int pageSize = 100)
        {
            using var conn = db.CreateConnection();
            var items = conn.Connection.GetListPaged<T>(pageNumber, pageSize, string.Empty, orderBy).ToList();
            return items;
        }

        public T GetItem<T>(string id)
        {
            using var conn = db.CreateConnection();
            return conn.Connection.Get<T>(id);
        }

        public void InsertItem<T>(T item)
        {
            using var conn = db.CreateConnection();
            conn.Connection.Insert<string, T>(item);
        }

        public void UpdateItem<T>(T item)
        {
            using var conn = db.CreateConnection();
            conn.Connection.Update(item);
        }

        public void DeleteItem<T>(T item)
        {
            using var conn = db.CreateConnection();
            conn.Connection.Delete(item);
        }

        public int GetCount<T>()
        {
            using var conn = db.CreateConnection();
            return conn.Connection.RecordCount<T>();
        }

        public List<ItemData> GetItemData(int skip = 0, int take = 100)
        {
            using var conn = db.CreateConnection();
            conn.Connection.Open();

            var sql = @$"SELECT * FROM ItemData
                      ORDER BY Owner DESC
                      LIMIT @take OFFSET @skip";

            var historySpentResult = conn.Connection.Query<ItemData>(sql, new { skip, take }).ToList();

            return historySpentResult;

            // var historySpent = historySpentResult.Select(this.Convert);

            // return items.OrderByDescending(x => x.CreationTime).ThenBy(x => x.BlockIndex);

            //sql = @$"SELECT * FROM TransactionData
            //      WHERE AccountIndex == @accountIndex
            //      {(excludeColdStake ? "AND (IsColdCoinStake = false OR IsColdCoinStake is null) " : "")}
            //      ORDER BY CreationTime DESC
            //      LIMIT @take OFFSET @skip";

            //var historyUnspentResult = conn.Query<TransactionData>(sql, new { accountIndex, skip, take }).ToList();

            //conn.Close();

            //var historyUnspent = historyUnspentResult.Select(this.Convert);

            //var items = new List<WalletHistoryData>();

            //items.AddRange(historySpent
            //    .GroupBy(g => g.SpendingDetails.TransactionId)
            //           .Select(s =>
            //           {
            //               var x = s.First();

            //               return new WalletHistoryData
            //               {
            //                   IsSent = true,
            //                   SentTo = x.SpendingDetails.TransactionId,
            //                   IsCoinStake = x.SpendingDetails.IsCoinStake,
            //                   CreationTime = x.SpendingDetails.CreationTime,
            //                   BlockHeight = x.SpendingDetails.BlockHeight,
            //                   BlockIndex = x.SpendingDetails.BlockIndex,
            //                   SentPayments = x.SpendingDetails.Payments?.Select(p => new WalletHistoryPaymentData
            //                   {
            //                       Amount = p.Amount,
            //                       PayToSelf = p.PayToSelf,
            //                       DestinationAddress = p.DestinationAddress
            //                   }).ToList(),

            //                   // when spent the amount represents the
            //                   // input that was spent not the output
            //                   Amount = x.Amount
            //               };
            //           }));

            //items.AddRange(historyUnspent
            //    .GroupBy(g => g.Id)
            //    .Select(s =>
            //    {
            //        var x = s.First();

            //        var ret = new WalletHistoryData
            //        {
            //            IsSent = false,
            //            OutPoint = x.OutPoint,
            //            BlockHeight = x.BlockHeight,
            //            BlockIndex = x.BlockIndex,
            //            IsCoinStake = x.IsCoinStake,
            //            CreationTime = x.CreationTime,
            //            ScriptPubKey = x.ScriptPubKey,
            //            Address = x.Address,
            //            Amount = x.Amount,
            //            IsCoinBase = x.IsCoinBase,
            //            IsColdCoinStake = x.IsColdCoinStake,
            //        };

            //        if (s.Count() > 1)
            //        {
            //            ret.Amount = s.Sum(b => b.Amount);
            //            ret.ReceivedOutputs = s.Select(b => new WalletHistoryData
            //            {
            //                IsSent = false,
            //                OutPoint = b.OutPoint,
            //                BlockHeight = b.BlockHeight,
            //                BlockIndex = b.BlockIndex,
            //                IsCoinStake = b.IsCoinStake,
            //                CreationTime = b.CreationTime,
            //                ScriptPubKey = b.ScriptPubKey,
            //                Address = b.Address,
            //                Amount = b.Amount,
            //                IsCoinBase = b.IsCoinBase,
            //                IsColdCoinStake = b.IsColdCoinStake,
            //            }).ToList();
            //        }

            //        return ret;
            //    }));

            //return items.OrderByDescending(x => x.CreationTime).ThenBy(x => x.BlockIndex);
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
