using Blockcore.Vault.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.Vault.Tests.Fakes
{
    public class TestMoney : IMoney
    {
        public int GetAll()
        {
            return 0;
        }
    }
}
