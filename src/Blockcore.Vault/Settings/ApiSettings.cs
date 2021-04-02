using Blockcore.Vault.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Settings
{
    public class ApiSettings
    {
        public ApiKeys API { get; set; }
    }

    public class ApiKeys
    {
        public List<ApiKey> Keys { get; set; }
    }
}
