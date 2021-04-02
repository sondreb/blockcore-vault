using System;
using System.Collections.Generic;

namespace Blockcore.Vault.Authentication
{
    public class ApiKey
    {
        public int Id { get; set; }

        public bool Enabled { get; set; }

        public string Key { get; set; }

        public IReadOnlyCollection<string> Paths { get; set; }
    }
}
