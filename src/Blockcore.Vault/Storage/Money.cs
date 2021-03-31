using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Storage
{
    public class Money : IMoney
    {
        public int GetAll()
        {
            return int.MaxValue;
        }
    }

    public interface IMoney
    {
        int GetAll();
    }
}
