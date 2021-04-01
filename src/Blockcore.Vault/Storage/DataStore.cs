using Blockcore.Vault.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Storage
{
    // TODO: Revive this repository wrapper at a later time to add 
    // extra abstraction, if needed, to allow alternative database implementations.
    // Temporarily removed to reduce duplicate work during initial development.
    //public class DataStore
    //{
    //    private readonly DatabaseRepository db;

    //    public DataStore(DatabaseRepository db)
    //    {
    //        this.db = db;
    //    }

    //    public void GetAll()
    //    {
    //        db.GetForAddress("");
    //    }

    //    public List<VaultServer> GetVaultServer(int skip = 0, int take = 100)
    //    {
    //        return db.GetVaultServer(skip, take);
    //    }

    //    public List<ItemData> GetItemData(int skip = 0, int take = 100)
    //    {
    //        return db.GetItemData(skip, take);
    //    }

    //    public int GetItemDataCount()
    //    {
    //        return db.GetItemDataCount();
    //    }

    //    public ItemData GetSingleItemData(int id)
    //    {
    //        return db.GetSingleItemData(id);
    //    }
    //}
}
