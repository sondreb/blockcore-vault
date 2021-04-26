using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class JwtProof2020
    {
        public JwtProof2020()
        {
            Type = "JwtProof2020";
        }

        public string Type { get; set; }

        public string Jwt { get; set; }
    }
}
